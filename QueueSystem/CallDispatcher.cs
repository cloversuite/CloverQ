using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueSystem
{
    //Por lo que parece esta clase debería ir en la cola y encargarse de enrutar???
    class CallDispatcher
    {
        CallList calls = new CallList();
        MemberList members = new MemberList();
        
        public CallDispatcher() {
            
        }

        public void DispatchNext() {
            Call c = calls.NextToDispatch();
            Member m = members.NextAvailable();
            //Aca mandar mensaje akka al callhandler con el contacto del miembro para que intente conectarlo
            //debería recibir un mensaje trying y luego failed o success para poder saber en que anda la llamada 
            //que mandamos a hacer desde acá
        }

    }
}
