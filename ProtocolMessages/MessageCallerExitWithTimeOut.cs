using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages
{
    public class MessageCallerExitWithTimeOut : Message
    {
        public int TimeOut { get; set; }
    }
}
