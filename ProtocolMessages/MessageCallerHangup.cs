using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages
{
    //el call manager indica al actor que el CALLER terminò la llamada
    class MessageCallerHangup
    {
        public string HangUpCode { get; set; }
        //ej: normal clearing.
        public string HangUpReason { get; set; }
    }
}
