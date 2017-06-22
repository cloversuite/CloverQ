using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Akka.Actor;
using Akka.Configuration;

namespace AkkaActorSystem
{
    public class QActorSystem
    {
        private ActorPbxProxy actorPbxProxy;
        private IActorRef actorMsgRouter;

        /// <summary>
        /// Esta clase inicia el sistema de actores, crea un router de mensajes y una instancia del proxy para la pbx
        /// </summary>
        public QActorSystem()
        {
            //conf sanples
            //https://github.com/akkadotnet/akka.net/blob/v1.3/src/core/Akka/Configuration/Pigeon.conf
            //Conf Dispatcher
            //https://blog.knoldus.com/2016/01/15/sample-akka-dispatcher-configuration/
            var config = ConfigurationFactory.ParseString(@"
                akka.actor.my-pinned-dispatcher {
                    type = ""PinnedDispatcher""
                    executor = ""fork-join-executor""
                }
                akka.actor.inbox {
                    inbox-size=100000
              }");

            ActorSystem systemq = ActorSystem.Create("clover-q", config);
            Inbox inbox = Inbox.Create(systemq);
            actorMsgRouter = systemq.ActorOf(Props.Create(() => new ActorMsgRouter()).WithDispatcher("akka.actor.my-pinned-dispatcher"), "Proxy");

            actorPbxProxy = new ActorPbxProxy(inbox, actorMsgRouter);

        }

        public ActorPbxProxy GetActorPbxProxy()
        {
            return this.actorPbxProxy;
        }
    }
}
