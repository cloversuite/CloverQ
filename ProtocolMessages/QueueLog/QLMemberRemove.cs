using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages.QueueLog
{
    public class QLMemberRemove : QLMessage
    {
        public QLMemberRemove()
        {
            base.EventName = this.GetType().Name;

        }
    }
}
