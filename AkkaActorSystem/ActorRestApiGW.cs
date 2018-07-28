using Akka.Actor;
using Akka.Event;
using Newtonsoft.Json;
using ProtocolMessages.Commands;
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
            IActorRef originalSender;

            logger = Context.GetLogger();

            Receive<CMDListQueues>(msg =>
            {
                originalSender = Sender;
                RESQueueList rql =  actorCallDistributor.Ask<RESQueueList>(msg).Result;
                rql.Nombre = "Ahi lo mando ";
                RESJson rqljson = new RESJson()
                {
                    JsonString = JsonConvert.SerializeObject(rql)
                };
                
                originalSender.Tell(rqljson, Self );

                logger.Info("=> ResJson FROM Nancy: " + rqljson.JsonString);
            });

        }
    }
}
