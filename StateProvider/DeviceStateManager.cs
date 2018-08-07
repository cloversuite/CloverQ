using System;
using System.Threading;
using AsterNET.ARI;
using AkkaActorSystem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AsterNET.ARI.Models;
using System.Collections.Generic;
using ProtocolMessages;
using ConfigProvider;

namespace StateProvider
{
    //esta clase debería monitorear el estado de los devices, esta info la saco del registrat/outbound proxy
    //por simplicidad supongo que el registrar/outbound proxy es un asterisk, pero debería ser un opensips o kamailio
    //para mayor capacidad en la cantidad de usuarios simultaneos que manejan
    //Uso el asternet.ari, solo para monitoreo de eventos, en realidad debería usar el asternet pero no corre sobre mono o .net core
    public class DeviceStateManager
    {
        private readonly SystemConfiguration systemConfig;

        DeviceCache deviceCache = new DeviceCache();
        ActorStateProxy actorStateProxy = null;
        AriClient pbx;
        string appName = "myStateManager";

        //Esta clase escucha eventos mediante asternet.ari websocket y los pasa al actor system
        //Los mensajes de cambio de estado y de contacto deberían llegarle al callditributor para 
        //saber si el dispositivo de un miembro esta listo para recibir llamadas
        //El dispositivo y uri de contacto lo deberia recibir en el login service al loguearse un miembro
        public DeviceStateManager(ActorStateProxy actorStateProxy, SystemConfiguration systemConfig)
        {
            this.systemConfig = systemConfig;
            this.appName = systemConfig.StasisStateAppName;
            this.actorStateProxy = actorStateProxy;
        }


        public void Connect()
        {
            ConfHost stateManagerHost = systemConfig.GetStateProviderFirstHost();
            Connect(stateManagerHost.Ip, stateManagerHost.Port, stateManagerHost.User, stateManagerHost.Password);
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

            //SUBSCRIBO A EVENTOS ARI
            pbx.OnEndpointStateChangeEvent += Pbx_OnEndpointStateChangeEvent;
            pbx.OnDeviceStateChangedEvent += Pbx_OnDeviceStateChangedEvent;
            pbx.OnPeerStatusChangeEvent += Pbx_OnPeerStatusChangeEvent;

            //SUBCRIBO A EVENTOS ActorStateProxy
            actorStateProxy.AttachMember += ActorStateProxy_AttachMember;
            actorStateProxy.DetachMember += ActorStateProxy_DetachMember;

            //pbx.OnUnhandledEvent += Pbx_OnUnhandledEvent;


            //CONECTO EL CLIENTE, true para habilitar reconexion, e intento cada 5 seg
            try
            {
                Console.WriteLine("Conectando state provider en: " + server);
                pbx.EventDispatchingStrategy = EventDispatchingStrategy.DedicatedThread;
                pbx.Connect(true, 5);
                Thread.Sleep(1000);
                //obtengo los endpoints (devices)
                List<Endpoint> endpoints = pbx.Endpoints.ListByTech("SIP");
                foreach (Endpoint endpoint in endpoints)
                {

                    Device device = new Device();
                    device.Id = endpoint.Technology + "/" + endpoint.Resource;
                    device.EndpointState = endpoint.State;

                    deviceCache.AddDevice(device);
                }

                //obtengo el estado de los dispositivod?? no se bien que devuelve esto... por ahora lo comento
                //List<DeviceState> devStates = pbx.DeviceStates.List();
                //foreach (DeviceState state in devStates) {
                //    deviceCache.UpdateDeviceState(state.Name, state.State);
                //}


                pbx.Applications.Subscribe(appName, "endpoint:SIP");
                pbx.Applications.Subscribe(appName, "deviceState:");
                //una vez que inicializo pongo a recibir eventos al actorStateProxy, de aca me llegan los attach u detach
                actorStateProxy.Start();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al conectar con asterisk", ex);
            }

        }



        #region ARI event handler
        private void Pbx_OnPeerStatusChangeEvent(IAriClient sender, AsterNET.ARI.Models.PeerStatusChangeEvent e)
        {
            Device device =  deviceCache.GetDeviceById(e.Endpoint.Technology + "/" + e.Endpoint.Resource);
            string destination = "SIP/";
            if (device != null)
            {
                if (!String.IsNullOrEmpty(e.Peer.Address))
                {
                    destination += e.Peer.Address + "/" + e.Endpoint.Resource;
                    //Solo actualizo si cambió el contact
                    if (device.Contact != destination)
                    {
                        device.Contact = destination;
                        //solo envio mensaje al calldistributor si el device posee un agente
                        if (!String.IsNullOrEmpty(device.MemberId))
                        {
                            this.actorStateProxy.Send(new MessageDeviceStateChanged() { DeviceId = device.Id, MemberId = device.MemberId, IsInUse = device.IsInUse, IsOffline = device.IsOffline, Contact = device.Contact });
                        }
                    }

                }

                device.EndpointState = e.Peer.Peer_status;
                Console.WriteLine("ESTADO "+ device.Id+" PEER:" + device.DeviceState + " Endpoint: " + device.EndpointState);
            }
        }

        private void Pbx_OnUnhandledEvent(object sender, AsterNET.ARI.Models.Event eventMessage)
        {
            // Console.WriteLine("STP: No manejé: " + eventMessage.Type);
        }

        private void Pbx_OnDeviceStateChangedEvent(IAriClient sender, AsterNET.ARI.Models.DeviceStateChangedEvent e)
        {
            Console.WriteLine("ESTADO el device:" + e.Device_state.Name + " esta en:" + e.Device_state.State);
            Device device = deviceCache.UpdateDeviceState(e.Device_state.Name, e.Device_state.State);
            //solo envio mensaje al calldistributor si el device posee un agente
            if (device != null && !String.IsNullOrEmpty(device.MemberId))
            {
                this.actorStateProxy.Send(new MessageDeviceStateChanged() { DeviceId = device.Id, MemberId = device.MemberId, IsInUse = device.IsInUse, IsOffline = device.IsOffline, Contact = device.Contact });
            }
        }

        private void Pbx_OnEndpointStateChangeEvent(IAriClient sender, AsterNET.ARI.Models.EndpointStateChangeEvent e)
        {
            Console.WriteLine("ESTADO el endpoint: " + e.Endpoint.Technology + "/" + e.Endpoint.Resource + " - tiene el estado: " + e.Endpoint.State + " - canales: " + e.Endpoint.Channel_ids.Count.ToString());
            string endpointId = e.Endpoint.Technology + "/" + e.Endpoint.Resource;
            Device device = deviceCache.UpdateEndpointState(endpointId, e.Endpoint.State);
            //solo envio mensaje al calldistributor si el device posee un agente
            if (!String.IsNullOrEmpty(device.MemberId))
            {
                //TODO: verificar si enviar este mensaje es totalemente necesario
                this.actorStateProxy.Send(new MessageDeviceStateChanged() { DeviceId = device.Id, MemberId = device.MemberId, IsInUse = device.IsInUse, IsOffline = device.IsOffline, Contact = device.Contact });
            }
        }

        #endregion

        //TODO: Aca tengo un thread diferente al del cliente ari tocando el cache, cambiar en el cache por un dictionary concurrente.
        //Lo mismo pasa en los dempas proxy que reciben eventos, por ejemplo el que usa el callmanager
        #region ActorProxyState event handlers
        private void ActorStateProxy_DetachMember(object sender, ProtocolMessages.MessageDetachMemberFromDevice message)
        {
            deviceCache.DetachMemberFromDevice(message.DeviceId, message.MemberId);
        }

        private void ActorStateProxy_AttachMember(object sender, ProtocolMessages.MessageAttachMemberToDevice message)
        {
            deviceCache.AttachMemberToDevice(message.DeviceId, message.MemberId);
        }
        #endregion

        public void Disconnect()
        {
            actorStateProxy.Diconnect();
            pbx.Disconnect();
        }

    }
}
