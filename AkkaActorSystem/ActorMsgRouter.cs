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
    /// Este actor debería crear, si aún no lo creó, un actor que maneje cada cola basado en el queueid del MessageNewCall? (es una idea)
    /// </summary>
    public class ActorMsgRouter : ReceiveActor
    {
        //este diccionario debería llamarse queueHandlers y tener un actor por cola?...
        //Dictionary<string, IActorRef> callHandlers = new Dictionary<string, IActorRef>();

        IActorRef callDistributor;

        public ActorMsgRouter(IActorRef callDistributor)
        {
            this.callDistributor = callDistributor;

            Receive<MessageNewCall>(nc =>
            {
                callDistributor.Tell(nc, Sender); //reenvio el mensaje al call distributor, y pongo como sender al sender original, entonces cuando el calldistributor responda al sender le reponde directo al pbxrpoxy
            });

            Receive<MessageMemberLogin>(mlin =>
            {
                callDistributor.Tell(mlin, Sender);
            });
            Receive<MessageDeviceStateChanged>(cf =>
            {
                callDistributor.Tell(cf, Sender);
            });
            Receive<MessageCallToFailed>(ctf =>
            {
                callDistributor.Tell(ctf, Sender);
            });
            Receive<MessageCallToSuccess>(cts =>
            {
                callDistributor.Tell(cts, Sender);
            });
            Receive<MessageCallerHangup>(chup =>
            {
                callDistributor.Tell(chup, Sender);
            });
            Receive<MessageAgentHangup>(ahup =>
            {
                callDistributor.Tell(ahup, Sender);
            });
            Receive<MessageCallTransfer>(ahup =>
            {
                callDistributor.Tell(ahup, Sender);
            });

            

        }
        protected override void Unhandled(object message)
        {
            base.Unhandled(message);
            Console.WriteLine("Router mensaje no manejado: " + message.ToString());
        }
    }
}
