using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages
{
    public class MessageMemberUnpause : Message
    {
        public string MemberId { get; set; }
    }
}
