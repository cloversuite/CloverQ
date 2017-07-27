using System;
using System.Threading;
using AsterNET.ARI;
using AkkaActorSystem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProtocolMessages;
using System.Text.RegularExpressions;

namespace LoginProvider
{
    //esta clase recibe los login /logoff desde un asterisk y realiza esas funciones contra el actorMemberLoginService
    public class PbxLoginProvider
    {
        ActorLoginProxy actorLoginProxy = null;
        AriClient pbx;
        string appName = "myPbxLoginProvider";

        public PbxLoginProvider(ActorLoginProxy actorLoginProxy)
        {
            this.actorLoginProxy = actorLoginProxy;
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
            //USEREVENT para el login
            pbx.OnChannelUsereventEvent += Pbx_OnChannelUsereventEvent;

            //SUBSCRIBO A EVENTOS
            pbx.OnUnhandledEvent += Pbx_OnUnhandledEvent;


            //CONECTO EL CLIENTE, true para habilitar reconexion, e intento cada 5 seg
            try
            {
                pbx.EventDispatchingStrategy = EventDispatchingStrategy.DedicatedThread;
                pbx.Connect(true, 5);
                Thread.Sleep(1000);
                pbx.Applications.Subscribe(appName, "endpoint:SIP");
                pbx.Applications.Subscribe(appName, "deviceState:");

            }
            catch (Exception ex)
            {
                throw new Exception("Error al conectar con asterisk", ex);
            }

        }

        private async void  Pbx_OnChannelUsereventEvent(IAriClient sender, AsterNET.ARI.Models.ChannelUsereventEvent e)
        {
            Console.WriteLine("User event from: " + e.Channel.Name);
            //string eventname = ((JObject)e.Userevent).SelectToken("eventname").Value<string>();
            string eventname = (string)((JObject)e.Userevent)["eventname"];
            if (eventname == "login")
            {
                string memberId = (string)((JObject)e.Userevent)["agent"];
                string password = (string)((JObject)e.Userevent)["password"];
                string contact = (string)((JObject)e.Userevent)["contact"];
                //meter todo este parse en un metodo estático tal vez una clase contact Contact.Parse?
                contact = contact.Replace(";", ">");
                string deviceId = Regex.Match(contact, @"\<(.+?)\@").Groups[1].Value.Replace(":","/").ToUpper();
                string number = Regex.Match(contact, @"\:(.+?)\@").Groups[1].Value;
                string address = Regex.Match(contact, @"\@(.+?)\>").Groups[1].Value;
                string uri = Regex.Match(contact, @"\<(.+?)\>").Groups[1].Value.Replace("<","").Replace(">", "");
                string destination = "SIP/" + address + "/" + number;

                MessageMemberLoginResponse mlr = await actorLoginProxy.LogIn(new MessageMemberLogin() { MemberId = memberId, Password = password, Contact = destination, DeviceId = deviceId });

                Console.WriteLine("Member " + memberId + "login from:"+ contact +" response, " + mlr.Reason);
                
                //En el dialplan espero un segundo para dar tienpo al setvar, esto es para prueba, en prod el login services es un IVR hecho con ari, agi o async agi
                sender.Channels.SetChannelVar(e.Channel.Id, "logedin", mlr.LoguedIn.ToString());
                sender.Channels.ContinueInDialplan(e.Channel.Id);
            }

        }

        private void Pbx_OnUnhandledEvent(object sender, AsterNET.ARI.Models.Event eventMessage)
        {
           // Console.WriteLine("PLP: No manejé: " + eventMessage.Type);
        }

        public void Disconnect()
        {
            pbx.Disconnect();
        }


    }
}
