using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages.QueueLog
{
    public class QLRingNoAnswer : QLMessage
    {
        public QLRingNoAnswer()
        {
            base.EventName = this.GetType().Name;

        }
        public int Code { get { return int.Parse(base.data1); } set { base.data1 = value.ToString(); } }
        public string Reason { get { return base.data2; } set { base.data2 = value; } }
    }
}
