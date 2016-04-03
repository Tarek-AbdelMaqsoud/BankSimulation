using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Data.UsersQueues;
using System.Timers;

namespace BankSimulation.SignalR
{
    public class NotificationRepository
    {
        string connString = ConfigurationManager.ConnectionStrings["BankSimulationContextDatabaseConnectionName"].ConnectionString;
        int userID = 0;
        DateTime startingDate = DateTime.MinValue;
        List<long> shoulNotBeHandeledAgain = new List<long>();
        public List<UsersQueuesDb> GetUsersQueues(int ID = 0)
        {

            if (startingDate == DateTime.MinValue)
            {
                startingDate = DateTime.Now;
            }

            List<UsersQueuesDb> notifications = new List<UsersQueuesDb>();
            string commandText = string.Empty;
            commandText = @"
                                    SELECT [UsersQueueID]
                                          ,[UserID]
                                          ,[QueueID]
                                          ,[TellerID]
                                          ,[HandlingStarted]
                                          ,[HandlingEnded]
                                          ,[Created]
                                          ,[LastModified]
                                          ,[IsRemoved]
                                      FROM [dbo].[UsersQueues]                                 
                                    ";
            using (SqlConnection connection = new SqlConnection(connString))
            {

                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    connection.Open();
                    var sqlDependency = new SqlDependency(command);
                    sqlDependency.OnChange += new OnChangeEventHandler(sqlDependency_OnChange);

                    // NOTE: You have to execute the command, or the notification will never fire.
                    List<object> allNotifications = new List<object>();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (int.Parse(reader["UserID"].ToString()) == userID && reader["Read"].ToString() == "0")
                            {
                                allNotifications.Add(reader[0]);
                            }
                        }
                    }
                    notifications = UsersQueuesProvider.GetAll();
                    notifications = notifications.Where(n => n.IsRemoved == false).OrderBy(n => n.Created).ToList();
                    if (notifications.Count != 0)
                    {
                        //Get customers not assigned to tellers
                        var queuedCustomers = notifications.Where(n => n.TellerID == null).ToList();
                        if (queuedCustomers.Count > 0)
                        {
                            //Pop from the queue and assign to a teller

                            //Step 1: Find a free teller
                            var role = Data.Roles.RoleProvider.GetAllRoles().Where(r => r.RoleName.ToLower() == "teller").First();
                            var tellers = Data.Users.UsersProvider.GetUsersByRole(role.RoleId);
                            var freeTellers = new List<Data.Users.UsersDb>();
                            var freeTeller = Data.UsersQueues.UsersQueuesProvider.GetAll();
                            var busyTellers = notifications.GroupBy(t => t.TellerID)
                                                           .Select(grp => grp.First()).ToList()
                                                           .Where(x => x.TellerID != null)
                                                           .Select(x => x.TellerID).ToList();

                            //freeTeller = freeTeller.Where(t => t.TellerID != teller.UsersId && t.IsRemoved == false).ToList();

                            freeTellers = (from t in tellers
                                           where !(from ft in busyTellers
                                                   select ft).Contains(t.UsersId)
                                           select t).ToList();
                            if (freeTellers.Count == 0)
                            {
                                //No Free Tellers Available to handle the queued customers
                            }
                            else
                            {
                                var userQueue = queuedCustomers[0];
                                userQueue.TellerID = freeTellers[0].UsersId;
                                int time = new Random().Next(1, 30);
                                userQueue.TimeInSeconds = time;

                                StateObjClass StateObj = new StateObjClass();
                                StateObj.TimerCanceled = true;
                                StateObj.UserQueueID = userQueue.UsersQueueID;
                                System.Threading.TimerCallback TimerDelegate = new System.Threading.TimerCallback(timer_Elapsed);

                                System.Threading.Timer TimerItem = new System.Threading.Timer(TimerDelegate, StateObj, time * 1000, time * 1000);

                                // Save a reference for Dispose.
                                StateObj.TimerReference = TimerItem;
                                Data.UsersQueues.UsersQueuesProvider.Update(userQueue);

                            }
                        }
                        //TimerExample.Start();
                    }
                    //return allNotifications.Count;
                }
            }
            return notifications;
        }

        private void sqlDependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();

            //Get user's latest notifications
            List<UsersQueuesDb> objList = new NotificationHub().GetUsersQueues(startingDate);
            objList = (from y in objList
                       where !(
                                   from x in shoulNotBeHandeledAgain
                                   select x
                               ).Contains(y.UsersQueueID)
                       select y).ToList();
            foreach (var item in objList)
            {
                //broadcast the notification for all users
                context.Clients.All.addLatestNotification(item);
            }

            var shoulNotBeHandeledAgainUsersQueues = objList.Where(u => !u.HandlingEnded && u.IsRemoved).ToList();
            var shoulNotBeHandeledAgainIDs = (from u in shoulNotBeHandeledAgainUsersQueues
                                              select u.UsersQueueID).ToList();
            shoulNotBeHandeledAgain.AddRange(shoulNotBeHandeledAgainIDs);

            GetUsersQueues();
        }
        public void timer_Elapsed(object StateObj)
        {
            //_l.Add(DateTime.Now); // Add date on each timer event
            StateObjClass State = (StateObjClass)StateObj;
            var notifications = UsersQueuesProvider.GetAll();
            var notification = notifications.Where(n => n.UsersQueueID == State.UserQueueID).FirstOrDefault();
            if (notifications.Count != 0)
            {
                var userQueue = notification;
                userQueue.IsRemoved = true;
                Data.UsersQueues.UsersQueuesProvider.Update(userQueue);
            }

            if (State.TimerCanceled)
            // Dispose Requested.
            {
                State.TimerReference.Dispose();
                System.Diagnostics.Debug.WriteLine("Done  " + DateTime.Now.ToString());
            }
        }
    }

    public static class TimerExample // In App_Code folder
    {
        static Timer _timer; // From System.Timers
        public static void Start()
        {
            _timer = new Timer(30000); // Set up the timer for 3 seconds
            //
            // Type "_timer.Elapsed += " and press tab twice.
            //
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _timer.Enabled = true; // Enable it
        }
        public static void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //_l.Add(DateTime.Now); // Add date on each timer event
            var notifications = UsersQueuesProvider.GetAll();
            notifications = notifications.Where(n => n.IsRemoved == false).OrderBy(n => n.Created).ToList();
            if (notifications.Count != 0)
            {
                var userQueue = notifications[0];
                userQueue.IsRemoved = true;
                Data.UsersQueues.UsersQueuesProvider.Update(userQueue);
            }
        }
    }
    public class StateObjClass
    {
        // Used to hold parameters for calls to TimerTask.
        public long UserQueueID;
        public System.Threading.Timer TimerReference;
        public bool TimerCanceled;
    }
}