using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using ProtocolMessages;
using System.Threading;

namespace AkkaActorSystem
{
    public class ActorLoginProxy
    {
        #region Eventos
        public delegate void DelegateMessage(object sender, Message message);
        public delegate void DelegateLoginResponse(object sender, MessageMemberLoginResponse message);

        public event DelegateMessage Receive;
        public event DelegateLoginResponse LoginResponse;
        #endregion

        #region Atributos
        Thread threadReceiver = null;
        IActorRef actorMemberLoginService;
        Inbox inbox;
        #endregion

        public ActorLoginProxy(Inbox inbox, IActorRef actorMemberLoginService)
        {
            this.actorMemberLoginService = actorMemberLoginService;
            this.inbox = inbox;
        }

        public void Stop()
        {
            //Esto es solo si el proxy va a poder mandar mensajes a la pbx
            if (threadReceiver != null)
            {
                try
                {
                    threadReceiver.Interrupt();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al detener el thread receiver del ActorLoginProxy: " + ex.Message);
                }
            }
        }

        private void Receiver()
        {
            //Esto es solo si el proxy va a poder mandar mensajes
            threadReceiver = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        object msg = inbox.Receive(TimeSpan.FromDays(1));

                        //todos los mensajes

                        if (LoginResponse != null && msg is MessageMemberLoginResponse)
                        {
                            Console.WriteLine("El ActorPbx recibió MessageMemberLoginResponse");
                            this.LoginResponse(this, (MessageMemberLoginResponse)msg);
                        }

                        //All Messages
                        if (Receive != null && msg is Message)
                        {
                            Console.WriteLine("El ActorLoginProxy recibió un mensaje");
                            this.Receive(this, (Message)msg);
                        }
                        Thread.Sleep(100);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("El thread receiver del ActorLoginProxy se detuvo: " + ex.Message);
                }
            });
            threadReceiver.Start();
        }

        public void Start()
        {
            //Esto es solo si el proxy va a poder mandar mensajes a la pbx
            //Comienzo a recibir mensajitos
            Receiver(); //Comentado ya que el state provaider por el momento no recibe nada
        }

        public void Send(Message message)
        {
            inbox.Send(actorMemberLoginService, message);
            Console.WriteLine("El ActorPbx envió un mensaje al ActorMsgRouter");
        }
        public async Task<MessageMemberLoginResponse> LogIn(MessageMemberLogin message)
        {
            return await actorMemberLoginService.Ask<MessageMemberLoginResponse>(message);
        }

    }
}

