using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages.QueueLog
{
    public class QLMemberAdd : QLMessage
    {
        public QLMemberAdd()
        {
            base.EventName = this.GetType().Name;
        }
    }
}
