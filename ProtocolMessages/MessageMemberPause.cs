using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages
{
    public class MessageMemberPause : Message
    {
        public string MemberId { get; set; }
        public string Password { get; set; }
        public string PauseReason { get; set; }
        public string RequestId { get; set; }
    }
}
