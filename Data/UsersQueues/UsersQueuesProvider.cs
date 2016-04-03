using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Context;

namespace Data.UsersQueues
{
    public class UsersQueuesProvider
    {
        public static List<UsersQueuesDb> GetAll()
        {
            var dc = new BankSimulationContext();
            var usersQueues = dc.UsersQueues.ToList();

            return usersQueues;
        }
        public static int Update(UsersQueuesDb userQueue)
        {
            var dc = new BankSimulationContext();
            dc.UsersQueues.Attach(userQueue);
            var entry = dc.Entry(userQueue);
            entry.State = System.Data.Entity.EntityState.Modified;
            // other changed properties
            return dc.SaveChanges();
        }
        public static int InsertUserQueue(int userID)
        {
            var dc = new BankSimulationContext();
            UsersQueuesDb userQueue = new UsersQueuesDb();
            userQueue.UserID = userID;
            var user = Users.UsersProvider.GetUser(userID);
            var queues = Queues.QueueProvider.GetAll();
            if (user.IsVIP)
            {
                var queue = queues.Where(q => q.QueueName.ToLower() == "vip").FirstOrDefault();
                userQueue.QueueID = queue.QueueId;
            }
            else
            {
                var queue = queues.Where(q => q.QueueName.ToLower() == "normal").FirstOrDefault();
                userQueue.QueueID = queue.QueueId;
            }
            dc.UsersQueues.Add(userQueue);
            return dc.SaveChanges();
        }
    }
}
