using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Concurrent;
using Data.UsersQueues;
using Data.Context;
using System.Data.Entity;

namespace BankSimulation.SignalR
{
    [HubName("notificationHub")]
    public class NotificationHub : Hub
    {
        public override System.Threading.Tasks.Task OnConnected()
        {
            Clients.All.refreshNotification(new BankSimulation.SignalR.NotificationRepository().GetUsersQueues());
            return base.OnConnected();
        }

        public List<UsersQueuesDb> GetUsersQueues(DateTime date)
        {
            BankSimulationContext db = new BankSimulationContext();
            var usersQueues = db.UsersQueues.Where(uq=>uq.Created >= date && !uq.HandlingEnded).ToList();
            foreach (var item in usersQueues)
            {
                item.User = db.Users.Where(u => u.UsersId == item.UserID).FirstOrDefault();
            }
            return usersQueues;
        }
    }


}