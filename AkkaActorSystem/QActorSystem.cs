using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Akka.Actor;
using Akka.Configuration;
using Serilog;
using Akka.Logger.Serilog;
using Akka.Remote;

namespace AkkaActorSystem
{
    public class QActorSystem
    {
        private ActorPbxProxy actorPbxProxy;
        private ActorStateProxy actorStateProxy;
        private ActorLoginProxy actorLoginProxy;

        ActorSystem systemq;
        private IActorRef actorMsgRouter;
        private IActorRef actorDataAccess;
        private IActorRef actorQueueLog;
        private IActorRef actorCallDistributor;
        private IActorRef actorMemberLoginService;
        private IActorRef actorRestApiGW;

        /// <summary>
        /// Esta clase inicia el sistema de actores, crea un router de mensajes y una instancia del proxy para la pbx
        /// </summary>
        public QActorSystem()
        {
            //conf sanples
            //https://github.com/akkadotnet/akka.net/blob/v1.3/src/core/Akka/Configuration/Pigeon.conf
            //Conf Dispatcher
            //https://blog.knoldus.com/2016/01/15/sample-akka-dispatcher-configuration/
            //loggers = [""Akka.Logger.Serilog.SerilogLogger, Akka.Logger.Serilog""]
            var config = ConfigurationFactory.ParseString(@"
                akka {
                        log-config-on-start = on
                        stdout-loglevel = DEBUG
                        loglevel = DEBUG
                        loggers = [""Akka.Logger.Serilog.SerilogLogger, Akka.Logger.Serilog""]
                
                    actor {
                            provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                            debug {
                                    receive = on
                                    autoreceive = on
                                    lifecycle = on
                                    event-stream = on
                                    unhandled = on
                                    }
                            my-pinned-dispatcher {
                                type = ""PinnedDispatcher""
                                executor = ""fork-join-executor""
                            }
                            inbox {
                                inbox-size=100000
                            }

                        }
                        remote {
                            dot-netty.tcp {
                                port = 8081
                                hostname = 0.0.0.0
                                public-hostname = localhost
                            }
                        }
                            
                    }");

            //Creo logger para actores, life cycle, y demas del sistema de actores.
            Serilog.Core.Logger logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .MinimumLevel.Debug()
                .CreateLogger();
            Serilog.Log.Logger = logger; //Esto es necesario para que akka utilice el serilog
            SerilogLogger serifake;
            Akka.Remote.RemoteActorRef fackeRef;
            systemq = ActorSystem.Create("clover-q", config);
            //systemq.Log.Info("Sistema de acotores iniciado.");

            Inbox inboxPbxProxy = Inbox.Create(systemq);
            Inbox inboxStateProxy = Inbox.Create(systemq);
            Inbox inboxLoginProxy = Inbox.Create(systemq);



            //Este actor se encarga de acceder al sistema de persistencia (DB)
            actorDataAccess = systemq.ActorOf(Props.Create(() => new ActorDataAccess()).WithDispatcher("akka.actor.my-pinned-dispatcher"), "ActorDataAccess");

            //Este actor se encarga de generar el queuelog
            actorQueueLog = systemq.ActorOf(Props.Create(() => new ActorQueueLog()).WithDispatcher("akka.actor.my-pinned-dispatcher"), "ActorQueueLog");

            //Creo el calldistributor, este actor es el que al recibir una llamada nueva intenta rutearla a un agente libre, 
            //tambien recibe mensajes del actorStateProxy para mantener el estado de los dispositivos de los agentes
            actorCallDistributor = systemq.ActorOf(Props.Create(() => new ActorCallDistributor(actorDataAccess, actorQueueLog)).WithDispatcher("akka.actor.my-pinned-dispatcher"), "CallDistributor");

            //Este actor se encarga de recibir mensajes desde la API REST con Nancy
            actorRestApiGW = systemq.ActorOf(Props.Create(() => new ActorRestApiGW(actorCallDistributor)).WithDispatcher("akka.actor.my-pinned-dispatcher"), "ActorRestApiGW");


            actorMsgRouter = systemq.ActorOf(Props.Create(() => new ActorMsgRouter(actorCallDistributor)).WithDispatcher("akka.actor.my-pinned-dispatcher"), "MsgRouter");
            actorMemberLoginService = systemq.ActorOf(Props.Create(() => new ActorMemberLoginService(actorCallDistributor, actorDataAccess, inboxStateProxy.Receiver)).WithDispatcher("akka.actor.my-pinned-dispatcher"), "MemberLoginService");

            actorPbxProxy = new ActorPbxProxy(inboxPbxProxy, actorMsgRouter);
            actorStateProxy = new ActorStateProxy(inboxStateProxy, actorCallDistributor);
            actorLoginProxy = new ActorLoginProxy(inboxLoginProxy, actorMemberLoginService);

        }

        public void Stop()
        {
            if (systemq != null)
            {
                try
                {
                    systemq.Terminate();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al detener el sistema de actores: " + ex.Message);
                }
                Console.WriteLine("El sistema de actores se detuvo.");
            }

        }

        public ActorPbxProxy GetActorPbxProxy()
        {
            return this.actorPbxProxy;
        }

        public ActorStateProxy GetActorStateProxy()
        {
            return this.actorStateProxy;
        }

        public ActorLoginProxy GetActorLoginProxy()
        {
            return this.actorLoginProxy;
        }
    }
}
