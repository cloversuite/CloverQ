using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueSystem
{
    /// <summary>
    /// Representa una cloa, posee una lista de QueueMember (QueueMemberList) y una lista de Call (CallList)
    /// </summary>
    class Queue
    {
        QueueMemberList queueMeberList;
        CallList callList;

        public Queue(){
        }

        public QueueMemberList Members { get { return this.queueMeberList; }  set { this.queueMeberList = value; } }
        public CallList Calls { get { return this.callList; } set {this.callList = value; } }
        public string Id { get; set; }
        public string MoH { get; set; }

        #region Métodos publicos
        public void AddCall(Call call) {
            this.callList.AddCall(call);
            //TODO: una vez que agrego la llamada a la lista debo ver si tengo un QueueMember de la lista que pueda atenderla
            // si tengo uno libre envío un mensaje al callhandler con el contacto para que lo llame
            // de lo contrario envío un mensaje al callhandler que le avisa que esta en cola y que espere. 
            //En este caso el callhandler de pasar MoH o anuncio o lo que fuese
        }
        //Cuando un agente se libera, llega un mensaje al QueueCache que posee las colas ordenadas por su peso, entonces el QueueCache
        // se fija a que colas pertenece el Member consultando al  QueueMemberRel y 
        //toma las colas de a una segun su peso, y llama a este método, si el método devuelve una llamada entonces encontró una llamada
        //para el agente
        public Call HasCallForMember(Member member) {
            //TODO: Buscar en la lista de llamadas si existe una llamada que esté lista para ser atendida y le paso al callhandler el contacto
            // Digo una llamada que esté lista porque puede estar en la lista de llamadas pero en proceso de contacto, por lo que esa llamada
            // no la tengo en cuenta en este caso.
            // si no encuentra una llamada 
            return null;
        }

        //Este método agrega un QueueMember a la cola, el queue memeber ya me tiene que llegar con los dato de prioridad y demas cargados
        //La idea es que una cola solo tenga lo miembros que va a utlizar, esto es para no tener una lista de 1000 miembros para recorrer 
        //y que solo 10 se encuentren logueados en la cola 
        public void AddQueueMember(QueueMember queueMember) {
            queueMeberList.AddMember(queueMember);
        }
        #endregion

    }
}
