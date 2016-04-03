using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Context;
using Data.Users;
using Data.Queues;

namespace Data.UsersQueues
{
    [Table("UsersQueues")]
    public class UsersQueuesDb : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UsersQueueID { get; set; }
        public long UserID { get; set; }
        public int QueueID { get; set; }
        public Nullable<long> TellerID { get; set; }
        public bool HandlingStarted{ get; set; }
        public bool HandlingEnded { get; set; }
        public Nullable<int> TimeInSeconds { get; set; }

        [ForeignKey("UserID")]
        public UsersDb User { get; set; }


        [ForeignKey("QueueID")]
        public QueueDb Queue { get; set; }
        
    }
}
