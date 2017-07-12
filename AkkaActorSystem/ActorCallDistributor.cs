using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Akka.Actor;
using ProtocolMessages;
using QueueSystem;



namespace AkkaActorSystem
{
    public class ActorCallDistributor : ReceiveActor
    {
        QueueSystemManager queueSystem;

        /// <summary>
        /// Esta clase va a manejar la relación colas <-> miembros
        /// Mantiene una lista de todos los miembros logeados (y los que no) y el estado de su dispositivo
        /// Interactua con el ActorQueueRouter
        /// </summary>
        public ActorCallDistributor()
        {

            queueSystem = new QueueSystemManager(); 

            // Ejemplo de filtro de mensaje: Receive<String>(s => s.Equals("Start"), (s) => { proxyClient.Connect(); }); //ejemplito
            //Es estado del del sipositivo de
            Receive<MessageStateChanged>(cf =>
                {
                //    Sender.Tell(new MessageAnswerCall() { CallHandlerId = cf.CallHandlerId });
                  //  Sender.Tell(new MessageCallTo { CallHandlerId = cf.CallHandlerId, destination = "SIP/192.168.56.1:6060/2000" });
                });
            Receive<MessageNewCall>(nc => {
                Queue queue = queueSystem.QueueCache.GetQueue(nc.QueueId);
                Call call = new Call() { CallHandlerId = nc.CallHandlerId };
                QueueMember queueMember = queue.AddCall(call); //agrega la llamada y si hay un qm para atenderla lo devuelve
                Sender.Tell(new MessageAnswerCall() { CallHandlerId = nc.CallHandlerId, MediaType = "MoH", Media = "Default" });
                if (queueMember != null)
                {
                    Sender.Tell(new MessageCallQueued() { CallHandlerId = nc.CallHandlerId });
                }
                else {
                    call.IsDispatching = true;
                    Sender.Tell(new MessageCallTo() { CallHandlerId = nc.CallHandlerId, Destination = queueMember.Member.Contact });
                }
                
            });

        }
        protected override void Unhandled(object message)
        {
            base.Unhandled(message);
            Console.WriteLine("MemberManager mensaje no manejado");
        }
    }
}
