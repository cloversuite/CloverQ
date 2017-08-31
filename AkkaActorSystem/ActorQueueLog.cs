using Akka.Actor;
using Akka.Event;
using ProtocolMessages.QueueLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaActorSystem
{
    public class ActorQueueLog : ReceiveActor, ILogReceive
    {
        private readonly ILoggingAdapter logger;
        public ActorQueueLog()
        {
            //TODO: crear aca un logger especifico para este actor 
            //logger = Context.GetLogger();
            logger = Context.GetLogger();
            Receive<QLMessage>(qlmsg =>
               {
                   logger.Info(qlmsg.ToString());
               });
        }


    }
}
