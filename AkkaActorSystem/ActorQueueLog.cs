using Akka.Actor;
using Akka.Dispatch.SysMsg;
using Akka.Event;
using ProtocolMessages.QueueLog;
using Serilog;
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
            //Voy a utilizar una instancia serilog aparte del log.logger, 
            Serilog.Core.Logger queueLog = new LoggerConfiguration()
                .WriteTo.RollingFile("queuelog-{date}.txt")
                .MinimumLevel.Information()
                .CreateLogger();

            //Este logger es el del sistema
            logger = Context.GetLogger();
            Receive<QLMessage>(qlmsg =>
            {
                queueLog.Information(qlmsg.ToString());
            });
            Receive<Terminated>(t =>
            {
                queueLog.Dispose();
            });
        }
    }
}
