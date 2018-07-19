using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages
{
    public class MessageCallUnHold : Message
    {
        /// <summary>
        /// Time elapsed between hold and unhold
        /// </summary>
        public int HoldTime { get; set; }
    }
}
