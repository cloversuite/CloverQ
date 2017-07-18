using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages
{
    public class MessageMemberLoginResponse : Message
    {
        public bool LoguedIn { get; set; }
        public string Reason { get; set; }
    }
}
