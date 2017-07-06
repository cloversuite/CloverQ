using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages
{
    public class MessageRouteCall : Message
    {
        //Id de la llamada a enrutar
        public string CallHandlerId { get; set; }
    }
}
