using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages
{
    //el call manager indica al actor que el CALLER terminò la llamada
    public class MessageCallerHangup : Message
    {
        public string HangUpCode { get; set; }
        //ej: normal clearing.
        public string HangUpReason { get; set; }
        public int WatingTime { get; set; }
        public int ConnectedTime { get; set; }
        public int HoldingTime { get; set; }
        public int TalkingTime { get; set; }
    }
}
