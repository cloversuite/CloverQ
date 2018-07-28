using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages.Commands
{
    public class RESCall
    {
        public DateTime StartTime { get; set; }
        public string CallHandlerId { get; set; }
        public string ChannelId { get; set; }
        public bool IsDispatching { get; set; } //  esto indica que ya mandé al callhandler un memeber para que contacte, y aún no concretó
        public bool Connected { get; set; }
        public long SenderUid { get; set; }
        public string QueueMemberId { get; set; }
        public string QueueMemberName { get; set; }
        public string QueueMemberChannelId { get; set; }

    }
}
