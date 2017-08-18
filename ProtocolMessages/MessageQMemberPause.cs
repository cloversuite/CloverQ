using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages
{
    public class MessageQMemberPause
    {
        public string MemberId { get; set; }
        public string PauseReason { get; set; }
    }
}
