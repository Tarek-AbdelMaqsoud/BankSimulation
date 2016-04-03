using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Context;
using Data.Users;

namespace Data.Queues
{
    [Table("Queues")]
    public class QueueDb : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QueueId { get; set; }
        public string QueueName { get; set; }
    }
}
