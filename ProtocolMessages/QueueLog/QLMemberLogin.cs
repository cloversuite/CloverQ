using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages.QueueLog
{
    public class QLMemberLogin : QLMessage
    {
        public QLMemberLogin()
        {
            base.EventName = this.GetType().Name;
        }
        public string Name { get { return base.data1; } set { base.data1 = value; } }
        public string DeviceId { get { return base.data2; } set { base.data2 = value; } }

    }
}
