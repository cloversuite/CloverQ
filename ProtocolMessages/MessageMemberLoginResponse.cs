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
        public string MemberId { get; set; }
        public string DeviceId { get; set; }
        public string Contact { get; set; }
        public string ResquestId { get; set; }
    }
}
