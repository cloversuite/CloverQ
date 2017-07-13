using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages
{
    public class MessageMemberLogin : Message
    {
        public string MemberId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Contact { get; set; }
    }
}
