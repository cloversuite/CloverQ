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
        private const string appName = "bridge_test";

        public Router() { }

        public void Start() {
            try
            {
                //creo el cliente para conecctarme al asterisk
                actionClient = new AriClient(new StasisEndpoint("develop01.cloudapp.net", 8088, "asterisk", "pelo2dos"), appName);
                //subscribo a los eventos necesarios
                actionClient.OnChannelCreatedEvent += ActionClient_OnChannelCreatedEvent;
                actionClient.OnStasisStartEvent += ActionClient_OnStasisStartEvent;
                actionClient.OnStasisEndEvent += ActionClient_OnStasisEndEvent;
                actionClient.OnUnhandledEvent += ActionClient_OnUnhandledEvent;
                //Conecto el cliente al asterisk
                actionClient.Connect(true,5);
                //creo un bridge, aca el nombre del bridge habria que ver si no va con el nomnre del agente o el id del que llama
                routerBridge = actionClient.Bridges.Create("mixing", Guid.NewGuid().ToString(), appName);
                //me subscribo a los eventos del bridge
                actionClient.Applications.Subscribe(appName, "bridge:" + routerBridge.Id);
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
            actionClient.Channels.Originate("PJSIP/1000",null,null,null,null, appName, "", "1111", 20, null, null, null, null);
        }

        private void ActionClient_OnStasisStartEvent(IAriClient sender, StasisStartEvent e)
        {
            //atiendo el canal
            actionClient.Channels.Answer(e.Channel.Id);
            //inicio musica en espera
            //actionClient.Bridges.StartMoh(routerBridge.Id, "default");
            //agrego el canal al bridge
            actionClient.Bridges.AddChannel(routerBridge.Id, e.Channel.Id, null);

        }

        private void ActionClient_OnStasisEndEvent(IAriClient sender, StasisEndEvent e)
        {
            // remuevo el canal del bridge
            actionClient.Bridges.RemoveChannel(routerBridge.Id, e.Channel.Id);

            // cuelgo la llamada entrante
            actionClient.Channels.Hangup(e.Channel.Id, "normal");
        }
    }
}
