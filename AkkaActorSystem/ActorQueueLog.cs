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
    public class ActorQueueLog : ReceiveActor
    {
        private readonly ILoggingAdapter logger;
        public ActorQueueLog()
        {
            logger= Context.GetLogger();
            
            Receive<QLMessage>( qlmsg =>
                {
                    //logger.Info();
                });
        }


    }
}
