using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BankSimulation.Controllers
{
    public class UsersController : Controller
    {
        //
        // GET: /Users/
        [HttpPost]
        public ActionResult AddCustomerToTheQueue(FormCollection form)
        {
            var selectedCustomers = form["customers"];
            if (selectedCustomers != null)
            {
                var customersIDs = selectedCustomers.Split(',').ToList();
                var usersQueues = Data.UsersQueues.UsersQueuesProvider.GetAll();
                foreach (var customerID in customersIDs)
                {
                    var userQueue = usersQueues.Where(u => u.UserID == Convert.ToInt32(customerID) && !u.IsRemoved).FirstOrDefault();
                    if (userQueue != null)
                    {
                        continue;
                    }
                    else
                    {
                        Data.UsersQueues.UsersQueuesProvider.InsertUserQueue(Convert.ToInt32(customerID));
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Tellers()
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
            ViewBag.Customers = Data.Users.UsersProvider.GetUsersByRole((customerRole == null) ? 0 : customerRole.RoleId);
            ViewBag.Tellers = Data.Users.UsersProvider.GetUsersByRole((tellerRole == null) ? 0 : tellerRole.RoleId);
            ViewBag.QueuedUsers = queuedUsers;

            return View(users);


        }
    }
}