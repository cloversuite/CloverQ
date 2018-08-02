using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using ProtocolMessages;
using ProtocolMessages.QueueLog;
using QueueSystem;
using Models;
using ProtocolMessages.Commands;

namespace AkkaActorSystem
{
    public class ActorCallDistributor : ReceiveActor
    {
        Dictionary<long, IActorRef> callSenders; //almacena los senders, si la llamada quedó enconlada tengo qeu saber donde mandar el callto
        QueueSystemManager queueSystem;
        IActorRef actorDataAccess;
        IActorRef actorQueueLog;

        /// <summary>
        /// Esta clase va a manejar la relación colas <-> miembros
        /// Mantiene una lista de todos los miembros logeados (y los que no) y el estado de su dispositivo
        /// Interactua con el ActorQueueRouter
        /// </summary>
        public ActorCallDistributor(IActorRef actorDataAccess, IActorRef actorQueueLog)
        {
            callSenders = new Dictionary<long, IActorRef>();
            this.actorDataAccess = actorDataAccess;
            //solicita todas las colas que estan persistidas (estaticas) tal vez desde el mysql? como hago para manejar la actualización de valores?
            this.actorDataAccess.Tell(new DAGetQueues());

            queueSystem = new QueueSystemManager();

            //Creo las colas que tengo persistidas
            Receive<DAQueues>(daq =>
            {
                if (daq.Queues != null)
                {
                    foreach (DTOQueue dtoq in daq.Queues)
                    {
                        Queue q = new Queue(dtoq.MemberStrategy, dtoq.CallOrderStrategy) { Id = dtoq.Id, Media = dtoq.Media, MediaType = dtoq.MediaType, Weight = dtoq.Weight, WrapupTime = dtoq.WrapupTime };

                        if (dtoq.QueueMembers != null)
                        {
                            //foreach(DTOQueueMember qm in dtoq.QueueMembers)
                            //{
                            //TODO: revisar esto, me parece que esta demás, de la freepbx por ejemplo no puedo obtebner esta info
                            //}
                        }

                        queueSystem.QueueCache.AddQueue(q); //si ya exite la cola, hago update?
                    }
                }
            });

            Receive<MessageMemberLogin>(mlin =>
            {
                //Mensaje que proviene del ActorMemberLoginService, aca creo un nuevo member, cuando me llegan los QMemberAdd creo los
                //QueueMember en base a este objeto. El member quue creo aca tambien recibe mensajes del stateprovider
                queueSystem.MemberCache.Add(new Member() { Id = mlin.MemberId, Name = mlin.Name, Contact = mlin.Contact, Password = mlin.Password, DeviceId = mlin.DeviceId });
                actorQueueLog.Tell(new QLMemberLogin() { MemberId = mlin.MemberId, Name = mlin.Name, DeviceId = mlin.DeviceId });
            });

            Receive<MessageMemberLogoff>(mlof =>
            {
                //Mensaje que proviene del ActorMemberLoginService 
                queueSystem.MemberCache.Remove(mlof.MemberId);
                actorQueueLog.Tell(new QLMemberLogoff() { MemberId = mlof.MemberId });
            });

            Receive<MessageQMemberPause>(mpau =>
            {
                queueSystem.MemberCache.MemberPause(mpau.MemberId, mpau.PauseReason, mpau.PauseReason);
                actorQueueLog.Tell(new QLMemberPause() { MemberId = mpau.MemberId, Reason = mpau.PauseReason });

            });

            Receive<MessageQMemberUnpause>(munpau =>
            {
                queueSystem.MemberCache.MemberUnPause(munpau.MemberId);
                actorQueueLog.Tell(new QLMemberUnpause() { MemberId = munpau.MemberId });
            });

            Receive<MessageQMemberAdd>(memberQueues =>
            {
                //Mensaje que proviene del ActorMemberLoginService, posee una lista de los id de las colas de un miembro
                //me parece que esta logica deberia estar dentro del queue sistem, aca no.
                foreach (string queueId in memberQueues.QueuesId)
                {
                    // mmmm... es correcto aca recuperar el member,crear un queue member y recien ahi agregarlo?
                    Member member = queueSystem.MemberCache.GetMemberById(memberQueues.MemberId);
                    if (member != null)
                    {
                        QueueMember qm = new QueueMember(member);
                        queueSystem.QueueCache.GetQueue(queueId).AddQueueMember(qm);
                        actorQueueLog.Tell(new QLMemberAdd() { QueueId = queueId, MemberId = member.Id });
                    }
                }
            });

            Receive<MessageQMemberRemove>(memberQueues =>
            {
                //Mensaje que proviene del ActorMemberLoginService, posee una lista de los id de las colas de un miembro
                //me parece que esta logica deberia estar dentro del queue sistem, aca no.
                foreach (string queueId in memberQueues.QueuesId)
                {
                    // mmmm... es correcto aca recuperar el member,crear un queue member y recien ahi agregarlo?
                    Member member = queueSystem.MemberCache.GetMemberById(memberQueues.MemberId);
                    if (member != null)
                    {
                        QueueMember qm = new QueueMember(member);
                        queueSystem.QueueCache.GetQueue(queueId).RemoveQueueMember(qm);
                        actorQueueLog.Tell(new QLMemberRemove() { QueueId = queueId, MemberId = member.Id });
                    }
                }
            });
            // Ejemplo de filtro de mensaje: Receive<String>(s => s.Equals("Start"), (s) => { proxyClient.Connect(); }); //ejemplito
            //Es estado del del sipositivo de
            Receive<MessageDeviceStateChanged>(dsc =>
            {
                Member member = queueSystem.MemberCache.GetMemberById(dsc.MemberId);
                //verifico que sea del mismo dispositivo
                if (dsc.DeviceId == member.DeviceId)
                {
                    if(!String.IsNullOrEmpty(dsc.Contact))
                        member.Contact = dsc.Contact;

                    member.DeviceIsInUse = dsc.IsInUse;
                    member.EndpointIsOfline = dsc.IsOffline;
                    Console.WriteLine("CALL DIST: member STATE changed, Contact: " + member.Contact + ", IsInUse: " + member.DeviceIsInUse);
                }
            });
            Receive<MessageNewCall>(nc =>
            {
                Queue queue = queueSystem.QueueCache.GetQueue(nc.QueueId);
                Call call = new Call() { CallHandlerId = nc.CallHandlerId, ChannelId = nc.ChannelId };
                QueueMember queueMember = null;
                if (queue != null)
                {
                    queueMember = queue.AddCall(call); //agrega la llamada y si hay un qm para atenderla lo devuelve
                    actorQueueLog.Tell(new QLEnterQueue() { QueueId = queue.Id, Channel = call.ChannelId });
                }
                Sender.Tell(new MessageAnswerCall() { CallHandlerId = nc.CallHandlerId, MediaType = queue.MediaType, Media = queue.Media });
                if (queueMember == null)
                {
                    //guardo en el call el uid del sender
                    call.SenderUid = Sender.Path.Uid;
                    //guardo una referencia al sender si no la tengo
                    if (!callSenders.ContainsKey(Sender.Path.Uid))
                        callSenders.Add(Sender.Path.Uid, Sender);
                    //le aviso al sender que su llamada quedó encolada hasta que se libere un member
                    Sender.Tell(new MessageCallQueued() { CallHandlerId = nc.CallHandlerId });

                }
                else
                {
                    call.IsDispatching = true;
                    call.QueueMember = queueMember;
                    Sender.Tell(new MessageCallTo() { CallHandlerId = nc.CallHandlerId, Destination = queueMember.Member.Contact });
                }
            });
            Receive<MessageCallToFailed>(ctf =>
            {
                //Busco otro member
                Console.WriteLine("CALL DIST: callto failed with code: " + ctf.Code.ToString() + " Reason: " + ctf.Reason);
                Queue queue = queueSystem.QueueCache.GetQueue(ctf.QueueId);
                if (queue != null)
                {
                    Call call = queue.calls.GetCallById(ctf.CallHandlerId);
                    if (call != null)
                    {
                        call.QueueMember.Member.IsAvailable = true;
                        QueueMember queueMember = queue.members.NextAvailable(queue.WrapupTime);
                        if (queueMember != null)
                        {
                            call.QueueMember = queueMember;
                            Sender.Tell(new MessageCallTo() { CallHandlerId = ctf.CallHandlerId, Destination = queueMember.Member.Contact });
                            actorQueueLog.Tell(new QLRingNoAnswer() { QueueId = queue.Id
                                , Channel = call.ChannelId
                                , MemberId = call.QueueMember.Id
                                , Code = ctf.Code, Reason = ctf.Reason
                                , RingingTime = ctf.RingingTime
                            });
                        }
                    }
                }
            });
            Receive<MessageCallToSuccess>(cts =>
            {
                Console.WriteLine("CALL DIST: callto success");
                Queue queue = queueSystem.QueueCache.GetQueue(cts.QueueId);
                Call call;
                if (queue != null)
                {
                    call = queue.calls.GetCallById(cts.CallHandlerId);
                    if (call != null)
                    {
                        call.IsDispatching = false;
                        call.Connected = true;
                        actorQueueLog.Tell(new QLConnect() { QueueId = queue.Id
                            , Channel = call.ChannelId
                            , MemberId = call.QueueMember.Id
                            , HoldTime = cts.HoldTime
                        });
                    }
                }

            });
            Receive<MessageCallerHangup>(chup =>
            {
                //si caller hangup termino toda la llamada?, tal vez comportamiento configurable?
                Console.WriteLine("CALL DIST: Caller Hangup");
                Queue queue = queueSystem.QueueCache.GetQueue(chup.QueueId);
                if (queue != null)
                {
                    Call call = queue.calls.RemoveCall(chup.CallHandlerId);
                    QueueMember queueMember = call.QueueMember;
                    if (queueMember != null)
                    {
                        queueMember.MarkLastCallTime();
                        queueMember.Member.IsAvailable = true;
                    }
                    if (call.Connected == false)
                    {
                        actorQueueLog.Tell(new QLAbandon() { QueueId = queue.Id,
                            Channel = call.ChannelId,
                            WaitingTime = chup.WatingTime 
                        });
                    }
                    else
                    {
                        actorQueueLog.Tell(new QLCallerHangUp() { QueueId = queue.Id,
                            Channel = call.ChannelId,
                            MemberId = call.QueueMember.Id,
                            WatingTime = chup.WatingTime,
                            HoldingTime = chup.HoldingTime,
                            TalkingTime = chup.TalkingTime
                        });
                    }
                }
            });

            Receive<MessageAgentHangup>(ahup =>
            {
                //Si agent hangup hago que la llamada del caller siga en el dialplan?
                Console.WriteLine("CALL DIST: Agent Hangup");
                Queue queue = queueSystem.QueueCache.GetQueue(ahup.QueueId);
                if (queue != null)
                {
                    Call call = queue.calls.RemoveCall(ahup.CallHandlerId);
                    QueueMember queueMember;
                    if (call != null)
                    {
                        queueMember = call.QueueMember;

                        if (queueMember != null)
                        {
                            queueMember.MarkLastCallTime();
                            queueMember.Member.IsAvailable = true;
                            actorQueueLog.Tell(new QLAgentHangUp() { QueueId = queue.Id,
                                    Channel = call.ChannelId,
                                    MemberId = call.QueueMember.Id,
                                    WatingTime = ahup.WatingTime,
                                    HoldingTime = ahup.HoldingTime,
                                    TalkingTime = ahup.TalkingTime
                               });
                        }
                    }
                }
            });

            Receive<MessageCallTransfer>(ctrans =>
            {
                Console.WriteLine("CALL DIST: Call Trasnfer: dst: " + ctrans.TargetName);
                Queue queue = queueSystem.QueueCache.GetQueue(ctrans.QueueId);
                if (queue != null)
                {
                    Call call = queue.calls.RemoveCall(ctrans.CallHandlerId);
                    QueueMember queueMember = call.QueueMember;
                    if (queueMember != null)
                    {
                        queueMember.MarkLastCallTime();
                        queueMember.Member.IsAvailable = true;
                        actorQueueLog.Tell(new QLTransfer() { QueueId = queue.Id,
                            Channel = call.ChannelId,
                            MemberId = call.QueueMember.Id,
                            TargetChannelId = ctrans.TargetId,
                            TargetChannelName = ctrans.TargetName,
                            WatingTime = ctrans.WatingTime,
                            HoldingTime = ctrans.HoldingTime,
                            TalkingTime = ctrans.TalkingTime
                        });
                    }
                }
            });

            Receive<MessageCallHold>(cho =>
            {
                Queue queue = queueSystem.QueueCache.GetQueue(cho.QueueId);
                if (queue != null)
                {
                    Call call = queue.calls.GetCallById(cho.CallHandlerId);
                    QueueMember queueMember = call.QueueMember;
                    if (queueMember != null)
                    {
                        actorQueueLog.Tell(new QLCallHoldStart() { QueueId = queue.Id, Channel = call.ChannelId, MemberId = call.QueueMember.Id});
                    }
                }
            });

            Receive<MessageCallUnHold>(cuho =>
            {
                Queue queue = queueSystem.QueueCache.GetQueue(cuho.QueueId);
                if (queue != null)
                {
                    Call call = queue.calls.GetCallById(cuho.CallHandlerId);
                    QueueMember queueMember = call.QueueMember;
                    if (queueMember != null)
                    {
                        actorQueueLog.Tell(new QLCallHoldStop() { QueueId = queue.Id, Channel = call.ChannelId, MemberId = call.QueueMember.Id, HoldTime = cuho.HoldTime });
                    }
                }
            });

            Receive<MessageCheckReadyMember>(rdymem =>
            {
                //Console.WriteLine("CALL DIST: Check Ready Member: ");
                foreach (Queue queue in queueSystem.QueueCache.QueueList)
                    if (queue != null)
                    {
                        Call call = queue.CheckPendingCalls();
                        QueueMember queueMember = null;

                        if (call != null)
                            queueMember = call.QueueMember;

                        if (queueMember != null)
                        {
                            //guardo una referencia al sender si no la tengo
                            if (callSenders.ContainsKey(call.SenderUid))
                            {
                                call.IsDispatching = true;
                                callSenders[call.SenderUid].Tell(new MessageCallTo() { CallHandlerId = call.CallHandlerId, Destination = queueMember.Member.Contact });
                            }
                        }
                    }
            });

            #region "MANEJO DE CONSULTAS"

            /**************** MANEJO DE CONSULTAS *******************/

            Receive<CMDListQueues>(cmdlq => {
                RESQueueList rql = new RESQueueList();

                foreach (Queue queue in queueSystem.QueueCache.QueueList) {
                    RESQueue rq = new RESQueue() {
                        Id = queue.Id,
                        MediaType = queue.MediaType
                    };

                    foreach (Call call in queue.calls.Calls) {
                        RESCall rc = new RESCall()
                        {
                            CallHandlerId = call.CallHandlerId,
                            ChannelId = call.ChannelId,
                            QueueMemberId = call.QueueMember!=null? call.QueueMember.Member.Id:null,
                            QueueMemberName = call.QueueMember != null ? call.QueueMember.Member.Name : null,
                            StartTime = call.StartTime,
                            IsDispatching = call.IsDispatching,
                            Connected = call.Connected
                        };
                        rq.Calls.Add(rc);
                    }

                    foreach (QueueMember qMember in queue.members.Members)
                    {
                        RESMember rm = new RESMember()
                        {
                            Id = qMember.Member.Id,
                            Contact = qMember.Member.Contact,
                            DeviceId = qMember.Member.DeviceId,
                            Name = qMember.Member.Name,
                            IsPaused = qMember.Member.IsPaused,
                            IsLogedIn = qMember.Member.IsLogedIn,
                            PauseCode = qMember.Member.PauseCode,
                            PauseReason = qMember.Member.PauseReason
                        };
                        rq.Members.Add(rm);
                    }

                    rql.Queues.Add(rq);
                }
                Sender.Tell(rql, Self);
            });

            #endregion

        }
        private void ScheduleMessageToCallDist()
        {
            //create a new instance of the performance counter
            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1),
                Self,
                new MessageCheckReadyMember(),
                Self);
        }

        protected override void PreStart()
        {
            base.PreStart();
            ScheduleMessageToCallDist();
        }

        protected override void Unhandled(object message)
        {
            base.Unhandled(message);
            Console.WriteLine("CallDistributor mensaje no manejado", message.ToString());
        }
    }
}
