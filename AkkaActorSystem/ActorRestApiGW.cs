using Akka.Actor;
using Akka.Event;
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

        public ActorRestApiGW()
        {
            logger = Context.GetLogger();
            Receive<string>(msg =>
            {
                logger.Info("FROM Nancy: " + msg);
            });
        }
    }
}
