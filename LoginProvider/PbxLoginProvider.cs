using System;
using System.Threading;
using AsterNET.ARI;
using AkkaActorSystem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProtocolMessages;
using System.Text.RegularExpressions;
using ConfigProvider;

namespace LoginProvider
{
    //esta clase recibe los login /logoff desde un asterisk y realiza esas funciones contra el actorMemberLoginService
    public class PbxLoginProvider
    {
        private readonly SystemConfiguration systemConfig;

        DeviceMemberMap deviceMemberMap = new DeviceMemberMap();
        ActorLoginProxy actorLoginProxy = null;
        AriClient pbx;
        string appName = "myPbxLoginProvider";

        public PbxLoginProvider(ActorLoginProxy actorLoginProxy, SystemConfiguration systemConfig)
        {
            this.systemConfig = systemConfig;
            this.appName = systemConfig.StasisLoginAppName;

            this.actorLoginProxy = actorLoginProxy;
            actorLoginProxy.LoginResponse += ActorLoginProxy_LoginResponse;
            //Comienzo a recibir eventos
            actorLoginProxy.Start();
        }

        public void Connect()
        {
            ConfHost loginProviderHost = systemConfig.GetLoginProvidersFirstHost();
            Connect(loginProviderHost.Ip, loginProviderHost.Port, loginProviderHost.User, loginProviderHost.Password);
        }

        /// <summary>
        /// Connecto to Asterisk ARI and WebSocket events
        /// </summary>
        /// <param name="server">IP or host name of asterisk</param>
        /// <param name="port">Port ej: 8088</param>
        /// <param name="usu">ARI user</param>
        /// <param name="pass">ARI password</param>
        public void Connect(string server, int port, string usu, string pass)
        {
            //CREO EL CLIENTE
            pbx = new AriClient(new StasisEndpoint(server, port, usu, pass), appName);
            //USEREVENT para el login -> en desuso
            //pbx.OnChannelUsereventEvent += Pbx_OnChannelUsereventEvent;

            //SUBSCRIBO A EVENTOS
            pbx.OnUnhandledEvent += Pbx_OnUnhandledEvent;
            pbx.OnStasisStartEvent += Pbx_OnStasisStartEvent;
            pbx.OnStasisEndEvent += Pbx_OnStasisEndEvent;


            //CONECTO EL CLIENTE, true para habilitar reconexion, e intento cada 5 seg
            try
            {
                Console.WriteLine("Conectando login provider en: " + server);
                pbx.EventDispatchingStrategy = EventDispatchingStrategy.DedicatedThread;
                pbx.Connect(true, 5);
                //Thread.Sleep(1000);
                //pbx.Applications.Subscribe(appName, "endpoint:SIP");
                //pbx.Applications.Subscribe(appName, "deviceState:");

            }
            catch (Exception ex)
            {
                throw new Exception("Error al conectar con asterisk", ex);
            }

        }

        private void Pbx_OnStasisEndEvent(IAriClient sender, AsterNET.ARI.Models.StasisEndEvent e)
        {
            Console.WriteLine("EL canal: " + e.Channel.Id + "abandonó la app: " + appName);
        }

        /// <summary>
        /// This stasis app receives in e.Args[0] the action like "login" and then send de corresponding msg to the login proxy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pbx_OnStasisStartEvent(IAriClient sender, AsterNET.ARI.Models.StasisStartEvent e)
        {
            //string eventname = ((JObject)e.Userevent).SelectToken("eventname").Value<string>();
            string eventname = e.Args[0]; //["eventname"];
            string memberId = e.Args[1]; //["agent"];
            string password = e.Args[2]; //["password"];
            string contact = e.Args[3]; //["contact"];
            string pauseReason = "";

            if (e.Args.Count >= 5) //verifico que tenga 4 o mas argumentos para tratar de recuperar el pause reason
                pauseReason = e.Args[4];

            string channelId = e.Channel.Id; //Envio el channel ID para trackear la respuesta
            string deviceId = "";
            try
            {
                //obtenemos el deviceId del string de contacto
                deviceId = Regex.Match(contact, @"\<(.+?)\@").Groups[1].Value.Replace(":", "/").ToUpper();
            }
            catch (Exception ex)
            {
                Console.WriteLine("PbxLoginProvider: Error al obtener deviceId del contacto. " + ex.Message);
            }


            //Si no me pasan el memberId, trato de recuperarlo del deviceMemberMap
            if (String.IsNullOrEmpty(memberId))
            {
                memberId = deviceMemberMap.GetMemberIdFromDeviceId(deviceId);
                //Si tampoco lo encuentro en el deviceMemberMap
                if (String.IsNullOrEmpty(memberId))
                {
                    Console.WriteLine("PbxLoginProvider: no se pudo determinar el memeberId para el device: " + deviceId);
                }
            }

            if (eventname == "login")
            {
                //meter todo este parse en un metodo estático tal vez una clase contact Contact.Parse?
                contact = contact.Replace(";", ">");
                deviceId = Regex.Match(contact, @"\<(.+?)\@").Groups[1].Value.Replace(":", "/").ToUpper();
                string number = Regex.Match(contact, @"\:(.+?)\@").Groups[1].Value;
                string address = Regex.Match(contact, @"\@(.+?)\>").Groups[1].Value;
                string uri = Regex.Match(contact, @"\<(.+?)\>").Groups[1].Value.Replace("<", "").Replace(">", "");
                string destination = "SIP/" + address + "/" + number;

                //esta llamada la tengo que pasar de ask a tell para poder hacer todo async
                //MessageMemberLoginResponse mlr = await 
                Console.WriteLine("Login para: " + memberId);
                actorLoginProxy.Send(new MessageMemberLogin() { MemberId = memberId, Password = password, Contact = destination, DeviceId = deviceId, RequestId = channelId });
            }
            else if (eventname == "logoff")
            {
                if (!String.IsNullOrEmpty(memberId))
                {
                    deviceMemberMap.UnTrackMemberDeviceId(deviceId);
                    actorLoginProxy.Send(new MessageMemberLogoff() { MemberId = memberId, Password = password, RequestId = e.Channel.Id });
                }
                else
                    Console.WriteLine("LogOff: Error MembderId es nulo o vacio");

                //como no manejo nada mas sigo adelante en el dialplan
                pbx.Channels.ContinueInDialplan(e.Channel.Id);
            }
            else if (eventname == "pause")
            {
                if (!String.IsNullOrEmpty(memberId))
                    actorLoginProxy.Send(new MessageMemberPause() { MemberId = memberId, Password = password, PauseReason = pauseReason, RequestId = e.Channel.Id });
                else
                    Console.WriteLine("Pause: Error MembderId es nulo o vacio");

                //como no manejo nada mas sigo adelante en el dialplan
                pbx.Channels.ContinueInDialplan(e.Channel.Id);
            }
            else if (eventname == "unpause")
            {
                if (!String.IsNullOrEmpty(memberId))
                    actorLoginProxy.Send(new MessageMemberUnPause() { MemberId = memberId, Password = password, RequestId = e.Channel.Id });
                else
                    Console.WriteLine("UnPause: Error MembderId es nulo o vacio");

                //como no manejo nada mas sigo adelante en el dialplan
                pbx.Channels.ContinueInDialplan(e.Channel.Id);
            }

        }

        private void ActorLoginProxy_LoginResponse(object sender, MessageMemberLoginResponse message)
        {
            //Manejo la respuesta del login 
            Console.WriteLine("Member " + message.MemberId + "login from:" + message.Contact + " response, " + message.Reason);

            //si el login es satisfactorio lo asicio al device
            if (message.LoguedIn)
            {
                deviceMemberMap.TrackMemberDeviceId(message.DeviceId, message.MemberId);
            }

            //En el RequestId me viene el canal que originó el mesaje de login
            pbx.Channels.SetChannelVar(message.ResquestId, "logedin", message.LoguedIn.ToString());
            pbx.Channels.ContinueInDialplan(message.ResquestId);
        }


        //private async void Pbx_OnChannelUsereventEvent(IAriClient sender, AsterNET.ARI.Models.ChannelUsereventEvent e)
        //{
        //    Console.WriteLine("User event from: " + e.Channel.Name);

        //    //string eventname = ((JObject)e.Userevent).SelectToken("eventname").Value<string>();
        //    string eventname = (string)((JObject)e.Userevent)["eventname"];
        //    string memberId = (string)((JObject)e.Userevent)["agent"];
        //    string password = (string)((JObject)e.Userevent)["password"];
        //    string contact = (string)((JObject)e.Userevent)["contact"];

        //    if (eventname == "login")
        //    {
        //        //meter todo este parse en un metodo estático tal vez una clase contact Contact.Parse?
        //        contact = contact.Replace(";", ">");
        //        string deviceId = Regex.Match(contact, @"\<(.+?)\@").Groups[1].Value.Replace(":", "/").ToUpper();
        //        string number = Regex.Match(contact, @"\:(.+?)\@").Groups[1].Value;
        //        string address = Regex.Match(contact, @"\@(.+?)\>").Groups[1].Value;
        //        string uri = Regex.Match(contact, @"\<(.+?)\>").Groups[1].Value.Replace("<", "").Replace(">", "");
        //        string destination = "SIP/" + address + "/" + number;

        //        MessageMemberLoginResponse mlr = await actorLoginProxy.LogIn(new MessageMemberLogin() { MemberId = memberId, Password = password, Contact = destination, DeviceId = deviceId });

        //        Console.WriteLine("Member " + memberId + "login from:" + contact + " response, " + mlr.Reason);

        //        //En el dialplan espero un segundo para dar tienpo al setvar, esto es para prueba, en prod el login services es un IVR hecho con ari, agi o async agi
        //        sender.Channels.SetChannelVar(e.Channel.Id, "logedin", mlr.LoguedIn.ToString());
        //        sender.Channels.ContinueInDialplan(e.Channel.Id);
        //    }
        //    else if (eventname == "logoff")
        //    {
        //        actorLoginProxy.Send(new MessageMemberLogoff() { MemberId = memberId, Password = password });
        //    }
        //    else if (eventname == "pause")
        //    {
        //        actorLoginProxy.Send(new MessageMemberPause() { MemberId = memberId, Password = password });
        //    }
        //    else if (eventname == "unpause")
        //    {
        //        actorLoginProxy.Send(new MessageMemberUnPause() { MemberId = memberId, Password = password });
        //    }

        //}

        private void Pbx_OnUnhandledEvent(object sender, AsterNET.ARI.Models.Event eventMessage)
        {
            // Console.WriteLine("PLP: No manejé: " + eventMessage.Type);
        }

        public void Disconnect()
        {
            actorLoginProxy.Stop();
            pbx.Disconnect();
        }


    }
}
