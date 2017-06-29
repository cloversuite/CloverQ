using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using ProtocolMessages;


namespace AkkaActorSystem
{
    /// <summary>
    /// Esta clase debe enrutar los mensajes provenientes de la pbx hacia los actores que los manejan;
    /// en caso de no existir un actor que lo maneje, crea uno si es pertinente.
    /// </summary>
    public class ActorMsgRouter : ReceiveActor
    {
        Dictionary<string, IActorRef> callHandlers = new Dictionary<string, IActorRef>();

        public ActorMsgRouter()
        {
            //Receive<String>(s => s.Equals("Start"), (s) => { proxyClient.Connect(); }); //ejemplito
            Receive<MessageNewCall>(cf =>
            {
                Sender.Tell(new MessageAnswerCall(){ CallHandlerId = cf.CallHandlerId });
                Sender.Tell(new MessageCallTo { CallHandlerId = cf.CallHandlerId, destination = "SIP/192.168.56.1:6060/2000" });
            });
            //Receive<MessageHangUpAgent>((ci) =>
            //{
            //    //Sender.Tell( nada);
            //});
        }
        protected override void Unhandled(object message)
        {
            base.Unhandled(message);
            Console.WriteLine("Router mensaje no manejado");
        }
    }
}
