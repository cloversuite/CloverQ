using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Akka.Actor;
using ProtocolMessages;


namespace AkkaActorSystem
{
    public class ActorCallDistributor : ReceiveActor
    {
        /// <summary>
        /// Esta clase va a manejar la relación colas <-> miembros
        /// Mantiene una lista de todos los miembros logeados (y los que no) y el estado de su dispositivo
        /// Interactua con el ActorQueueRouter
        /// </summary>
        public ActorCallDistributor()
        {
            //Receive<String>(s => s.Equals("Start"), (s) => { proxyClient.Connect(); }); //ejemplito
            //Es estado del del sipositivo de
            Receive<MessageStateChanged>(cf =>
                {
                //    Sender.Tell(new MessageAnswerCall() { CallHandlerId = cf.CallHandlerId });
                  //  Sender.Tell(new MessageCallTo { CallHandlerId = cf.CallHandlerId, destination = "SIP/192.168.56.1:6060/2000" });
                });
            

        }
        protected override void Unhandled(object message)
        {
            base.Unhandled(message);
            Console.WriteLine("MemberManager mensaje no manejado");
        }
    }
}
