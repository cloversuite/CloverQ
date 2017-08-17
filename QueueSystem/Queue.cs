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
    public class Queue
    {
        QueueMemberList queueMeberList;
        CallList callList;

        public Queue()
        {
            CreateLists("", "");
        }

        public Queue(string memberStrategy, string callOrderStrategy)
        {
            CreateLists(memberStrategy, callOrderStrategy);
        }

        private void CreateLists(string memberStrategy, string callOrderStrategy)
        {

            queueMeberList = new QueueMemberList();
            callList = new CallList();

            if (!String.IsNullOrEmpty(memberStrategy))
            {
                queueMeberList.SetMemberSrtategy(memberStrategy);
            }

            if (!String.IsNullOrEmpty(callOrderStrategy))
            {
                callList.SetCallOrderSrtategy(callOrderStrategy);
            }
        }

        public QueueMemberList members { get { return this.queueMeberList; } set { this.queueMeberList = value; } }
        public CallList calls { get { return this.callList; } set { this.callList = value; } }
        public string Id { get; set; }
        public string MoH { get; set; }
        public int Weight { get; set; }
        public int WrapupTime { get; set; }

        #region Métodos publicos
        public Call CheckPendingCalls() {
            Call call = calls.NextToDispatch();
            if (call != null) {
                QueueMember queueMember;
                queueMember = members.NextAvailable(WrapupTime);
                if (queueMember != null) {
                    call.QueueMember = queueMember;
                }else
                {

                    call.IsDispatching = false; //la devuelvo a la cola en espera porque no tengo member para asignarle
                }
            }
            return call;
        }

        //TODO: excepción si trato de hacer add de un id que ya existe
        public QueueMember AddCall(Call call)
        {
            this.callList.AddCall(call);
            return members.NextAvailable(WrapupTime); //nulo si no hay agente diponible
        }
        //Cuando un agente se libera, llega un mensaje al QueueCache que posee las colas ordenadas por su peso, entonces el QueueCache
        // se fija a que colas pertenece el Member consultando al  QueueMemberRel y 
        //toma las colas de a una segun su peso, y llama a este método, si el método devuelve una llamada entonces encontró una llamada
        //para el agente
        public Call HasCallForMember(Member member)
        {
            //TODO: Buscar en la lista de llamadas si existe una llamada que esté lista para ser atendida y le paso al callhandler el contacto
            // Digo una llamada que esté lista porque puede estar en la lista de llamadas pero en proceso de contacto, por lo que esa llamada
            // no la tengo en cuenta en este caso.
            // si no encuentra una llamada 
            Call call = calls.NextToDispatch();
            QueueMember queueMember;
            if (call != null)
            {
                queueMember = members.NextAvailable(WrapupTime);
                if (queueMember == null)
                {
                    call = null;
                }
                else
                {
                    call.QueueMember = queueMember;
                }
            }
            return call;
        }

        //Este método agrega un QueueMember a la cola, el queue memeber ya me tiene que llegar con los dato de prioridad y demas cargados
        //La idea es que una cola solo tenga lo miembros que va a utlizar, esto es para no tener una lista de 1000 miembros para recorrer 
        //y que solo 10 se encuentren logueados en la cola 
        public void AddQueueMember(QueueMember queueMember)
        {
            queueMeberList.AddMember(queueMember);
        }
        #endregion

    }
}
