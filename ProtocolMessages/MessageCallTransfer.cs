using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages
{
    public class MessageCallTransfer : Message
    {
        public string TargetId { get; set; }
        public string TargetName { get; set; }
        public int WatingTime { get; set; }
        public int HoldingTime { get; set; }
        public int TalkingTime { get; set; }
    }
}
