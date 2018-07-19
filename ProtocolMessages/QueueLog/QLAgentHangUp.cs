using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages.QueueLog
{
    public class QLAgentHangUp: QLMessage
    {
        public QLAgentHangUp()
        {
            base.EventName = this.GetType().Name;
        }

        public int WatingTime { get { return int.Parse(base.data1); } set { base.data1 = value.ToString(); } }
        public int HoldingTime { get { return int.Parse(base.data3); } set { base.data2 = value.ToString(); } }
        public int TalkingTime { get { return int.Parse(base.data4); } set { base.data3 = value.ToString(); } }
    }
}
