using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages
{
    //el call manager envía este mensaje al actor para indicar que el CALLER cambio el estado de hold
    //true el caller hold, false el caller unhold
    class MessaageCallerOnHold
    {
        public bool OnHold { get; set; }
    }
}
