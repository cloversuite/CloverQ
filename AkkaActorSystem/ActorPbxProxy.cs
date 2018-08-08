using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Akka.Actor;
using Akka.Configuration;
using ProtocolMessages;
using Serilog;

namespace AkkaActorSystem
{
    /// <summary>
    /// Esta clase se encarga de comunicar al que la usa con el sistema de actores mediante un Inbox del sistema de acoteres
    /// </summary>
    public class ActorPbxProxy
    {
        #region Eventos
        public delegate void DelegateMessage(object sender, Message message);
        public delegate void DelelegateMessageAnswerCall(object sender, MessageAnswerCall message);
        public delegate void DelelegateMessageCallTo(object sender, MessageCallTo message);
        public event DelegateMessage Receive;
        public event DelelegateMessageAnswerCall AnswerCall;
        public event DelelegateMessageCallTo CallTo;
        #endregion

        #region Atributos
        Thread threadReceiver = null;
        IActorRef actorMessageRouter;
        Inbox inbox;
        #endregion

        public ActorPbxProxy(Inbox inbox, IActorRef actorMessageRouter)
        {
            this.actorMessageRouter = actorMessageRouter;
            this.inbox = inbox;
        }

        public void Stop()
        {
            if (threadReceiver != null)
            {
                try
                {
                    threadReceiver.Interrupt();
                }
                catch (Exception ex)
                {
                    Log.Logger.Debug("Error al detener el thread receiver del ActorPbxProxy: " + ex.Message);
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
                            Log.Logger.Debug("El ActorPbx recibió AnswerCall");
                            this.AnswerCall(this, (MessageAnswerCall)msg);
                        }
                        if (CallTo != null && msg is MessageCallTo)
                        {
                            Log.Logger.Debug("El ActorPbx recibió un CallTo");
                            this.CallTo(this, (MessageCallTo)msg);
                        }
                        //All Messages
                        if (Receive != null && msg is Message)
                        {
                            //Log.Logger.Debug("El ActorPbx recibió un mensaje");
                            this.Receive(this, (Message)msg);
                        }
                        Thread.Sleep(100);
                    }
                }
                catch (Exception ex) {
                    Log.Logger.Debug("El thread receiver del ActorPbxProxy se detuvo: " + ex.Message);
                }
            });
            threadReceiver.Start();
        }

        public void Start()
        {
            //Comienzo a recibir mensajitos
            Receiver(); 
        }

        public void Send(Message message)
        {
            inbox.Send(actorMessageRouter, message);
            //Log.Logger.Debug("El ActorPbx envió un mensaje al ActorMsgRouter");
        }
    }
}
