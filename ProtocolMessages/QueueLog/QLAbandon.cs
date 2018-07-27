using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages.QueueLog
{
    public class QLAbandon : QLMessage
    {
        public QLAbandon()
        {
            base.EventName = this.GetType().Name;
        }

        public int WaitingTime { get { return int.Parse(base.data1); } set { base.data1 = value.ToString(); } }
    }
}
