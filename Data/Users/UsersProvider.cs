using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Context;

namespace Data.Users
{
    public class UsersProvider
    {
        public bool AddTeller(string FirstName, string LastName)
        {
            return AddUser(FirstName, LastName, true, false);
        }
        public bool AddUser(string FirstName, string LastName)
        {
            return AddUser(FirstName, LastName, false, false);
        }
        public bool AddVIPUser(string FirstName, string LastName)
        {
            return AddUser(FirstName, LastName, false, true);
        }
        public static UsersDb GetUser(long id)
        {
            var dc = new BankSimulationContext();
            var user = dc.Users.Where(x => x.UsersId == id).FirstOrDefault();
            return user;
        }
        public static List<UsersDb> GetAllUsers()
        {
            var dc = new BankSimulationContext();
            var users = dc.Users.ToList();
            users = Fill(users);
            

            return users;
        }
        public static List<UsersDb> GetUsersByRole(int roleId)
        {
            var dc = new BankSimulationContext();
            var users = dc.Users.Where(u => u.RoleID == roleId).ToList();
            return users;
        }
        private bool AddUser(string FirstName, string LastName, bool isTeller, bool isVIP)
        {

            var dc = new BankSimulationContext();
            UsersDb j = new UsersDb();
            j.FirstName = FirstName;
            j.LastName = LastName;
            j.IsVIP = isVIP;

            if (isTeller)
            {
                var role = dc.Roles.Where(r => r.RoleName.ToLower() == "teller").FirstOrDefault();
                role = (role == null) ? dc.Roles.FirstOrDefault() : role;
                j.RoleID = role.RoleId;
            }
            else
            {
                var role = dc.Roles.Where(r => r.RoleName.ToLower() == "normal").FirstOrDefault();
                role = (role == null) ? dc.Roles.FirstOrDefault() : role;
                j.RoleID = role.RoleId;
            }
            j.IsVIP = isVIP;

            return dc.SaveChanges() > 0;
        }
        private static List<UsersDb> Fill(List<UsersDb> users)
        {
            foreach (var user in users)
            {
                Fill(user);
            }
            return users;
        }


        private static UsersDb Fill(UsersDb user)
        {
            var dc = new BankSimulationContext();
            user.Role = dc.Roles.Where(r => r.RoleId == user.RoleID).FirstOrDefault();
            return user;
        }
    }
}
