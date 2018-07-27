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

        public int WatingTime { get { return int.Parse(base.data3); } set { base.data3 = value.ToString(); } }
        public int HoldingTime { get { return int.Parse(base.data4); } set { base.data4 = value.ToString(); } }
        public int TalkingTime { get { return int.Parse(base.data5); } set { base.data5 = value.ToString(); } }
    }
}
