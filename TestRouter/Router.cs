using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsterNET.ARI.Models;
using AsterNET.ARI;

namespace TestRouter
{
    class Router
    {
        AriClient actionClient;
        Bridge routerBridge;
        Dictionary<string, Channel> chs = new Dictionary<string, Channel>();
        private const string appName = "bridge_test";

        string asterisk = "develop01.cloudapp.net";
        

        public Router() { }

        public void Start() {
            try
            {
                //creo el cliente para conecctarme al asterisk 
                asterisk = "192.168.56.101";
                actionClient = new AriClient(new StasisEndpoint(asterisk, 8088, "asterisk", "pelo2dos"), appName);
                //subscribo a los eventos necesarios
              //  actionClient.OnChannelCreatedEvent += ActionClient_OnChannelCreatedEvent;
                actionClient.OnStasisStartEvent += ActionClient_OnStasisStartEvent;
                actionClient.OnStasisEndEvent += ActionClient_OnStasisEndEvent;
                actionClient.OnDialEvent += ActionClient_OnDialEvent;
                actionClient.OnBridgeCreatedEvent += ActionClient_OnBridgeCreatedEvent;
                actionClient.OnChannelCreatedEvent += ActionClient_OnChannelCreatedEvent1;
                actionClient.OnUnhandledEvent += ActionClient_OnUnhandledEvent;
                //Conecto el cliente al asterisk
                actionClient.Connect(true,5);
                //creo un bridge, aca el nombre del bridge habria que ver si no va con el nomnre del agente o el id del que llama //Guid.NewGuid().ToString()
                routerBridge = actionClient.Bridges.Create("mixing", "beaf1045-4680-40ed-9fbe-b2312baa0824" , appName);
                //me subscribo a los eventos del bridge
                actionClient.Applications.Subscribe(appName, "bridge:" + routerBridge.Id);
                actionClient.Applications.Subscribe(appName, "endpoint:PJSIP/1000" );
                var done = false;
                while (!done)
                {
                    var lastKey = Console.ReadKey();
                    switch (lastKey.KeyChar.ToString())
                    {
                        case "*":
                            done = true;
                            break;
                        case "2":
                            actionClient.Bridges.StartMoh(routerBridge.Id, "default");
                            break;
                        default:
                            ConnectTo("");
                            break;
                    }
                }

                actionClient.Bridges.Destroy(routerBridge.Id);
                actionClient.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }

        }

        private void ActionClient_OnDialEvent(IAriClient sender, DialEvent e)
        {
            Console.WriteLine("Dial event: " + e.Peer.Id + " - estado " + e.Dialstatus);
        }

        private void ActionClient_OnChannelCreatedEvent1(IAriClient sender, ChannelCreatedEvent e)
        {
            Console.WriteLine("Se creó el canal: " + e.Channel.Id + " - Estado: " + e.Channel.State.ToString());
        }

        private void ActionClient_OnBridgeCreatedEvent(IAriClient sender, BridgeCreatedEvent e)
        {
            Console.WriteLine("Se creó bridge: "+e.Bridge.Id);
        }

        private void ActionClient_OnChannelCreatedEvent(IAriClient sender, ChannelCreatedEvent e)
        {
            Console.WriteLine("Nuevo canal: " + e.Channel.Id + " : " + e.Channel.State);
        }

        private void ActionClient_OnUnhandledEvent(object sender, Event eventMessage)
        {
            Console.WriteLine(eventMessage.ToString());
        }

        public void ConnectTo(String dst)
        {

            /* Ejemplo:
            * Channel ch = ActionClient.Channels.Originate("SIP/101", "102", "from-internal-custom", 1,
            * "test_label", "hello-world", "originated", "test_caller_id", 60, null, e.Channel.Id, "", "");
            */
            Channel ch = actionClient.Channels.Originate("SIP/192.168.56.111:4040/1000",null,null,null,null, appName, "", "1111", 20, null, null, null, null);
            chs.Add(ch.Id,ch);
            Console.WriteLine("Llamando al canal: " + ch.Id);
        }

        private void ActionClient_OnStasisStartEvent(IAriClient sender, StasisStartEvent e)
        {
            Console.WriteLine("El canal: " + e.Channel.Id + " entro en stasis app: " + e.Application.ToString());
            //atiendo el canal
            actionClient.Channels.Answer(e.Channel.Id);
            //inicio musica en espera
            //actionClient.Bridges.StartMoh(routerBridge.Id, "default");
            //agrego el canal al bridge
            actionClient.Bridges.AddChannel(routerBridge.Id, e.Channel.Id, null);

        }

        private void ActionClient_OnStasisEndEvent(IAriClient sender, StasisEndEvent e)
        {


            // cuelgo la llamada entrante
            if (chs.Keys.Contains(e.Channel.Id))
            {
                chs.Remove(e.Channel.Id);
                try
                {
                    actionClient.Channels.Hangup(e.Channel.Id, "normal");
                    // remuevo el canal del bridge
                    actionClient.Bridges.RemoveChannel(routerBridge.Id, e.Channel.Id);
                    Console.WriteLine("Se removió el channel: " + e.Channel.Id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al remover channel: " + e.Channel.Id + "\n" + ex.Message);
                }
                
            }
        }
    }
}
