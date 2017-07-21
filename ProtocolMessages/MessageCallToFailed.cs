using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages
{
    public class MessageCallToFailed : Message
    {
        public int Code { get; set; }
        public string Reason { get; set; }
    }
}
