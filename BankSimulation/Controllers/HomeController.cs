using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BankSimulation.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var users = Data.Users.UsersProvider.GetAllUsers();
            var roles = Data.Roles.RoleProvider.GetAllRoles();
            var customerRole = roles.Where(r => r.RoleName.ToLower() == "customer").FirstOrDefault();
            var tellerRole = roles.Where(r => r.RoleName.ToLower() == "teller").FirstOrDefault();

            var usersQueues = Data.UsersQueues.UsersQueuesProvider.GetAll().Where(u => !u.IsRemoved).ToList();
            var queuedUsers = new List<Data.Users.UsersDb>();
            foreach (var userQueue in usersQueues)
            {
                queuedUsers.Add(Data.Users.UsersProvider.GetUser(userQueue.UserID));
            }
            ViewBag.Customers = Data.Users.UsersProvider.GetUsersByRole((customerRole == null)?0:customerRole.RoleId);
            ViewBag.Tellers = Data.Users.UsersProvider.GetUsersByRole((tellerRole == null) ? 0 : tellerRole.RoleId);
            ViewBag.QueuedUsers = queuedUsers;

            return View(users);
        }
    }
}