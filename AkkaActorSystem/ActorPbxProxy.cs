using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Akka.Actor;
using Akka.Configuration;
using ProtocolMessages;

namespace AkkaActorSystem
{
    /// <summary>
    /// Esta clase se encarga de comunicar al que la usa con el sistema de actores mediante un Inbox del sistema de acoteres
    /// </summary>
    public class ActorPbxProxy
    {
        public delegate void DelegateMessage(object sender, Message message);
        public delegate void DelelegateMessageAnswerCall(object sender, MessageAnswerCall message);
        public delegate void DelelegateMessageCallTo(object sender, MessageCallTo message);
        public event DelegateMessage Receive;
        public event DelelegateMessageAnswerCall AnswerCall;
        public event DelelegateMessageCallTo CallTo;

        Thread threadReceiver = null;
        IActorRef actorMessageRouter;
        Inbox inbox;

        public ActorPbxProxy(Inbox inbox, IActorRef actorMessageRouter)
        {
            this.actorMessageRouter = actorMessageRouter;
            this.inbox = inbox;
        }
        public void Connect()
        {
            threadReceiver.Start();
        }
        public void Diconnect()
        {
            threadReceiver.Interrupt();
        }

        private void Receiver()
        {
            threadReceiver = new Thread(() =>
            {
                while (true)
                {
                    object msg = inbox.Receive(Timeout.InfiniteTimeSpan);

                    //todos los mensajes
                    if (Receive != null && msg is Message)
                    {
                        this.Receive(this, (Message)msg);
                    }
                    if (AnswerCall!= null && msg is MessageAnswerCall)
                    {
                        this.AnswerCall(this, (MessageAnswerCall)msg);
                    }
                    if (AnswerCall != null && msg is MessageCallTo)
                    {
                        this.CallTo(this, (MessageCallTo)msg);
                    }
                }
            });
            threadReceiver.Start();
        }

        private void Start()
        {
            Receiver();
            //Crear algo que monitoree esto?
        }

        public void Send(Message message)
        {
            inbox.Send(actorMessageRouter, message);
        }
    }
}
