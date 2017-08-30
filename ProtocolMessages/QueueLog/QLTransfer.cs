using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages.QueueLog
{
    public class QLTransfer : QLMessage
    {
        public QLTransfer()
        {
            base.EventName = this.GetType().Name;

        }
        public string TargetChannelId { get { return base.data1; } set { base.data1 = value; } }
        public string TargetChannelName { get { return base.data2; } set { base.data2 = value; } }
    }
}
