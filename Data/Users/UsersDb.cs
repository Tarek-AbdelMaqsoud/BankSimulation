using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Context;
using Data.Roles;

namespace Data.Users
{
    [Table("Users")]
    public class UsersDb : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UsersId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsVIP { get; set; }
        public int RoleID { get; set; }


        [ForeignKey("RoleID")]
        public RoleDb Role { get; set; }
    }
}
