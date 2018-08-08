using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Akka.Actor;
using Akka.Configuration;
using ProtocolMessages;
using Serilog;

namespace AkkaActorSystem
{
    public class ActorStateProxy
    {
        #region Eventos
        public delegate void DelegateMessage(object sender, Message message);
        public delegate void DelelegateAttachMember(object sender, MessageAttachMemberToDevice message);
        public delegate void DelelegateDetachMember(object sender, MessageDetachMemberFromDevice message);
        public event DelegateMessage Receive;
        public event DelelegateAttachMember AttachMember;
        public event DelelegateDetachMember DetachMember;
        #endregion

        #region Atributos
        Thread threadReceiver = null;
        IActorRef actorCallDitributor;
        Inbox inbox;
        #endregion

        public ActorStateProxy(Inbox inbox, IActorRef actorCallDitributor)
        {
            this.actorCallDitributor = actorCallDitributor;
            this.inbox = inbox;
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
                    Log.Logger.Debug("Error al detener el thread receiver del ActorStateProxy: " + ex.Message);
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

                        if (AttachMember != null && msg is MessageAttachMemberToDevice)
                        {
                            Log.Logger.Debug("El ActorStateProxy recibió AttachMember");
                            this.AttachMember(this, (MessageAttachMemberToDevice)msg);
                        }
                        if (DetachMember != null && msg is MessageDetachMemberFromDevice)
                        {
                            Log.Logger.Debug("El ActorStateProxy recibió un DetachMember");
                            this.DetachMember(this, (MessageDetachMemberFromDevice)msg);
                        }
                        //All Messages
                        if (Receive != null && msg is Message)
                        {
                            Log.Logger.Debug("El ActorStateProxy recibió un mensaje");
                            this.Receive(this, (Message)msg);
                        }
                        Thread.Sleep(100);
                    }
                }
                catch (Exception ex)
                {
                    Log.Logger.Debug("El thread receiver del ActorStateProxy se detuvo: " + ex.Message);
                }
            });
            threadReceiver.Start();
        }

        public void Start()
        {
            //Esto es solo si el proxy va a poder mandar mensajes 
            //Comienzo a recibir mensajitos
            Receiver(); 
        }

        public void Send(Message message)
        {
            inbox.Send(actorCallDitributor, message);
            Log.Logger.Debug("El ActorStateProxy envió un mensaje al callDitributor");
        }

    }
}
