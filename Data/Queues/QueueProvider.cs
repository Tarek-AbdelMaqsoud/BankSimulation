using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Context;

namespace Data.Queues
{
    public static class QueueProvider
    {
        public static List<QueueDb> GetAll()
        {
            var dc = new BankSimulationContext();
            return dc.Queues.ToList();
        }
    }
}
