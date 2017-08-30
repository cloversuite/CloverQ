using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages.QueueLog
{
    public class QLMemberPause : QLMessage
    {
        public QLMemberPause()
        {
            base.EventName = this.GetType().Name;

        }
        public string Reason { get { return base.data1; } set { base.data1 = value; } }
    }
}
