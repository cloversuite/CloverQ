using System;
using System.Threading;
using AsterNET.ARI;
using AkkaActorSystem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StateProvider
{
    //esta clase debería monitorear el estado de los devices, esta info la saco del registrat/outbound proxy
    //por simplicidad supongo que el registrar/outbound proxy es un asterisk, pero debería ser un opensips o kamailio
    //para mayor capacidad en la cantidad de usuarios simultaneos que manejan
    //Uso el asternet.ari, solo para monitoreo de eventos, en realidad debería usar el asternet pero no corre sobre mono o .net core
    public class DeviceStateManager
    {
        ActorStateProxy actorStateProxy = null;
        AriClient pbx;
        string appName = "myStateManager";

        //Esta clase escucha eventos mediante asternet.ari websocket y los pasa al actor system
        //Los mensajes de cambio de estado y de contacto deberían llegarle al callditributor para 
        //saber si el dispositivo de un miembro esta listo para recibir llamadas
        //El dispositivo y uri de contacto lo deberia recibir en el login service al loguearse un miembro
        public DeviceStateManager(ActorStateProxy actorStateProxy)
        {
            this.actorStateProxy = actorStateProxy;
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

            //SUBSCRIBO A EVENTOS
            pbx.OnEndpointStateChangeEvent += Pbx_OnEndpointStateChangeEvent;
            pbx.OnDeviceStateChangedEvent += Pbx_OnDeviceStateChangedEvent;
            pbx.OnUnhandledEvent += Pbx_OnUnhandledEvent;

            //CONECTO EL CLIENTE, true para habilitar reconexion, e intento cada 5 seg
            try
            {
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


        private void Pbx_OnUnhandledEvent(object sender, AsterNET.ARI.Models.Event eventMessage)
        {
            Console.WriteLine("STP: No manejé: " + eventMessage.Type);
        }

        private void Pbx_OnDeviceStateChangedEvent(IAriClient sender, AsterNET.ARI.Models.DeviceStateChangedEvent e)
        {
            Console.WriteLine("ESTADO el device:" + e.Device_state.Name + " esta en:" + e.Device_state.State);
        }

        private void Pbx_OnEndpointStateChangeEvent(IAriClient sender, AsterNET.ARI.Models.EndpointStateChangeEvent e)
        {
            Console.WriteLine("ESTADO el endpoint: " + e.Endpoint.Technology +"/"+ e.Endpoint.Resource + " - tiene el estado: " + e.Endpoint.State + " - canales: " + e.Endpoint.Channel_ids.Count.ToString());
        }

        public void Disconnect()
        {
            pbx.Disconnect();
        }

    }
}
