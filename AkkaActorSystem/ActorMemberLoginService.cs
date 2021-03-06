﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using ProtocolMessages;
using Serilog;

namespace AkkaActorSystem
{
    //Este actor debe manejar los logins, le llega un login, lo valida, y si es correcto consulta las colas del miembro y 
    //le envía al call distributor los MessageQMemberAdd necesarios.
    //IDEA: deberia poder crear actores hijos del tipo proxy que implementen un ivr de login basado en FastAGI ó AsyncAGI, 
    //tambien podria crear algun tipo de actor que ofrezca un endponint para login via rest, exixte buena integracions para usar 
    //asp.net con akka por lo que podría probarse con .net core web api 
    class ActorMemberLoginService : ReceiveActor
    {
        IActorRef callDistributor;
        IActorRef actorStateProxy;

        public ActorMemberLoginService(IActorRef callDistributor, IActorRef actorDataAccess, IActorRef actorStateProxy)
        {
            this.callDistributor = callDistributor;
            this.actorStateProxy = actorStateProxy; //mediante este actor mando mensajes al DeviceStateManager

            Receive<MessageMemberLogin>(mlin =>
            {
                //Utilizo ask porque tengo al miembro en una llamada y necesita respuesta
                //En caso de ser un login mediante cti podría hacerlo asíncrono
                var t = Task.Run(async () =>
                {
                   DAMember m = await actorDataAccess.Ask<DAMember>(new DAGetMemberById() { MemberId = mlin.MemberId });
                    return m;
                });

                DAMember member = t.Result;

                if (member.Member != null && member.Member.Id == mlin.MemberId && member.Member.Password == mlin.Password)
                {
                    Sender.Tell(new MessageMemberLoginResponse() { LoguedIn = true, Reason = "Member authenticated and logedin.", MemberId = mlin.MemberId, ResquestId = mlin.RequestId, DeviceId = mlin.DeviceId });

                    //le agrego al mlin el nombre del member
                    mlin.Name = member.Member.Name;
                    callDistributor.Tell(mlin);

                    //Solicito de manera asincrónica las colas del miembro
                    actorDataAccess.Tell(new DAGetMemberQueues() { From = mlin.From, MemberId = mlin.MemberId, RequestId = mlin.RequestId }, Self);

                    actorStateProxy.Tell(new MessageAttachMemberToDevice() { DeviceId = mlin.DeviceId, MemberId = mlin.MemberId });
                }
                else
                {
                    Sender.Tell(new MessageMemberLoginResponse() { LoguedIn = false, Reason = "MemberId or Pass wrong.", MemberId = mlin.MemberId, ResquestId = mlin.RequestId, DeviceId = mlin.DeviceId });
                }

            });

            Receive<MessageMemberLogoff>(mlof =>
            {
                var t = Task.Run(async () =>
                {
                    DAMemberQueues q = await actorDataAccess.Ask<DAMemberQueues>(new DAGetMemberQueues() { MemberId = mlof.MemberId, RequestId = mlof.RequestId });
                    return q;
                });

                DAMemberQueues memberQueues = t.Result;

                
                actorStateProxy.Tell(new MessageDetachMemberFromDevice() { MemberId = mlof.MemberId });
                //le paso el queuesid list en null para desloguearlo de todas las colas
                callDistributor.Tell(new MessageQMemberRemove() { From = mlof.From, MemberId = mlof.MemberId, QueuesId = memberQueues.MemberQueues, RequestId = mlof.RequestId });

                callDistributor.Tell(mlof);
                
            });

            Receive<MessageMemberPause>(mpau =>
            {
                //le paso el queuesid list en null para desloguearlo de todas las colas
                callDistributor.Tell(new MessageQMemberPause() { From = mpau.From, MemberId = mpau.MemberId, PauseReason = mpau.PauseReason, RequestId = mpau.RequestId });
            });
            Receive<MessageMemberUnPause>(mupau =>
            {
                //le paso el queuesid list en null para desloguearlo de todas las colas
                callDistributor.Tell(new MessageQMemberUnpause() { From = mupau.From, MemberId = mupau.MemberId, RequestId = mupau.RequestId });
            });


            Receive<DAMemberQueues>(mqs =>
            {
                //Cuando el actor access service me responde le indico al call distrbutor que agregue el miembro a las colas
                callDistributor.Tell(new MessageQMemberAdd() { From = mqs.From, MemberId = mqs.MemberId, QueuesId = mqs.MemberQueues, RequestId = mqs.RequestId });
            });

        }

        protected override void Unhandled(object message)
        {
            base.Unhandled(message);
            Log.Logger.Debug("MemberLoginService mensaje no manejado");
        }
    }
}
