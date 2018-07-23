using Akka.Actor;
using Akka.Event;
using ProtocolMessages.RestApiGW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaActorSystem
{
    public class ActorRestApiGW : ReceiveActor, ILogReceive
    {
        private readonly ILoggingAdapter logger;

        public ActorRestApiGW(IActorRef actorCallDistributor)
        {
            logger = Context.GetLogger();
            Receive<string>(msg =>
            {
                if (msg == "status") {
                    logger.Info("FROM Nancy: " + msg + " required");
                }
                else
                {
                    logger.Error("FROM Nancy: " + msg + ", no manejado");
                }

                logger.Info("FROM Nancy: " + msg);
            });

        }
    }
}
