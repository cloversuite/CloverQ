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
            //Comienzo a recibir mensajitos
            Receiver();
        }
        public void Diconnect()
        {
            if (threadReceiver != null)
            {
                try
                {
                    threadReceiver.Interrupt();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al detener el thread receiver del ActorPbxProxy: " + ex.Message);
                }
                
            }
        }

        private void Receiver()
        {
            threadReceiver = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        object msg = inbox.Receive(TimeSpan.FromDays(1));

                        //todos los mensajes

                        if (AnswerCall != null && msg is MessageAnswerCall)
                        {
                            Console.WriteLine("El ActorPbx recibió AnswerCall");
                            this.AnswerCall(this, (MessageAnswerCall)msg);
                        }
                        if (AnswerCall != null && msg is MessageCallTo)
                        {
                            Console.WriteLine("El ActorPbx recibió un CallTo");
                            this.CallTo(this, (MessageCallTo)msg);
                        }
                        //All Messages
                        if (Receive != null && msg is Message)
                        {
                            Console.WriteLine("El ActorPbx recibió un mensaje");
                            this.Receive(this, (Message)msg);
                        }
                        Thread.Sleep(100);
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine("El thread receiver del ActorPbxProxy se detuvo: " + ex.Message);
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
            Console.WriteLine("El ActorPbx envió un mensaje al ActorMsgRouter");
        }
    }
}
