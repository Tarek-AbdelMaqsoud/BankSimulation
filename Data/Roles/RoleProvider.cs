using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Context;

namespace Data.Roles
{
    public class RoleProvider
    {
        public bool AddRole()
        {
            var dc = new BankSimulationContext();
            RoleDb r = new RoleDb();
            dc.Roles.Add(r);
            return dc.SaveChanges() > 0;
        }
        public static RoleDb GetRole(long id)
        {
            var dc = new BankSimulationContext();
            var role = dc.Roles.Where(x => x.RoleId == id).FirstOrDefault();
            return role;
        }

        public static List<RoleDb> GetAllRoles()
        {
            var dc = new BankSimulationContext();
            var roles = dc.Roles.ToList();
            return roles;
        }
    }
}
