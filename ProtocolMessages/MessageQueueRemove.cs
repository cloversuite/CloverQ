using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages
{
    public class MessageQueueRemove : Message
    {
        public string QueueId { get; set; }
    }
}
