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
                //IActorRef a = callHandlers[cf.Id];
                //Context.Stop(a);
                //callHandlers.Remove(cf.Id);

            });
            Receive<MessageAnswerCall>((ci) =>
            {
                //IActorRef callHandler = Context.ActorOf(Props.Create(() => new CallHandler()), "Handler:" + ci.Id);
                //callHandler.Tell(new CallInfo(ci.Id));
                //callHandlers.Add(ci.Id, callHandler);
            });
        }
    }
}
