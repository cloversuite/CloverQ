using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages
{
    public class MessageCallToSuccess : Message
    {
        /// <summary>
        /// Time elapsed between call enqueued and agent answer
        /// </summary>
        public int HoldTime { get; set; }
    }
}
