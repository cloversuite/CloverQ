using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsterNET.ARI.Models;
using AsterNET.ARI;
using AkkaActorSystem;
using ProtocolMessages;

namespace TestRouter
{
    public class CallManager
    {
        AriClient pbx;

        ActorPbxProxy actorPbxProxy = null;
        BridgesList bridgesList = new BridgesList();
        CallHandlerCache callHandlerCache = new CallHandlerCache();
        private const string appName = "bridge_test";

        public CallManager(ActorPbxProxy actorPbxProxy)
        {
            //Creo el sistema de actores y el actor proxy para la pbx
            this.actorPbxProxy = actorPbxProxy;
            actorPbxProxy.Receive += ActorPbxProxy_Receive;
            actorPbxProxy.AnswerCall += ActorPbxProxy_AnswerCall;
            actorPbxProxy.CallTo += ActorPbxProxy_CallTo;
            //Comienzo a recibir eventos
            actorPbxProxy.Start();
        }



        #region Handle Actor Sistem Events
        private void ActorPbxProxy_AnswerCall(object sender, MessageAnswerCall message)
        {
            string channelId = callHandlerCache.GetByCallHandlerlId(message.CallHandlerId).Caller.Id;
            try
            {
                pbx.Channels.Answer(channelId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("No se pudo atender el canal: " + channelId + " Error: " + ex.Message);
            }
            Console.WriteLine("El canal: " + channelId + " fué atendido correctamente ");
        }
        private void ActorPbxProxy_Receive(object sender, ProtocolMessages.Message message)
        {
            //Aca entran todos los eventos del sistema de actores
        }
        private void ActorPbxProxy_CallTo(object sender, MessageCallTo message)
        {
            CallHandler callHandler = callHandlerCache.GetByCallHandlerlId(message.CallHandlerId);
            try
            {
                //Origino la llamada al agente. Seguramente hay que hacerlo async
                Channel ch = pbx.Channels.Originate(message.Destination, null, null, null, null, appName, "", "1111", 20, null, null, null, null);
                //guardo el canal en el callhandler
                callHandlerCache.AddChannelToCallHandler(message.CallHandlerId, ch.Id);
                //Aca no puedo agregarlo al bridge porque aun no entra en el stasis, lo agrego en el stasisStart en el else
                //actualizo el canal del agente
                callHandler.Agent = ch;

            }
            catch (Exception ex)
            {
                Console.WriteLine("No se pudo conectar con el agente: " + message.Destination + " Error: " + ex.Message);
            }
            Console.WriteLine("La llamada al agente: " + message.Destination + " se concretó correctamente");

        }

        #endregion

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
            pbx.OnStasisStartEvent += Pbx_OnStasisStartEvent; //Se dispara cuando un canal ejecuta la app stasis en el dialplan. el canal queda ahi a la espera de ser manejado
            pbx.OnStasisEndEvent += Pbx_OnStasisEndEvent; //el canal abandonó la app stasis (no quiere decir que cortó)
            pbx.OnChannelHangupRequestEvent += Pbx_OnChannelHangupRequestEvent; //Se solicito terminar el canal (posee la causa ej: normal clearing)
            pbx.OnChannelStateChangeEvent += Pbx_OnChannelStateChangeEvent; //cambió el estado del canal ej: down->up->ringing. No se si lo voy a usar
            pbx.OnChannelDestroyedEvent += Pbx_OnChannelDestroyedEvent; //el canal fué terminado, sehizo efectivo el hangup
            pbx.OnChannelHoldEvent += Pbx_OnChannelHoldEvent; //el canal se puso onhold
            pbx.OnBridgeAttendedTransferEvent += Pbx_OnBridgeAttendedTransferEvent;
            pbx.OnBridgeBlindTransferEvent += Pbx_OnBridgeBlindTransferEvent;

            //CONECTO EL CLIENTE, true para habilitar reconexion, e intento cada 5 seg
            try
            {
                pbx.Connect(true, 5);
                if (pbx.Connected)
                {
                    List<Bridge> brs = pbx.Bridges.List();
                    foreach (Bridge b in brs)
                    {
                        bridgesList.AddNewBridge(b);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al conectar con asterisk", ex);
            }

        }

        public void Disconnect()
        {

            foreach (Bridge b in bridgesList.Bridges)
            {
                try
                {
                    pbx.Bridges.Destroy(b.Id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al remover un bridge: " + ex.Message);
                }

            }
            actorPbxProxy.Stop();

            pbx.Disconnect();
        }

        #region Handle ARI Events
        private void Pbx_OnBridgeBlindTransferEvent(IAriClient sender, BridgeBlindTransferEvent e)
        {
            Console.WriteLine("Blind Transfer");
            Console.WriteLine(e);
        }

        private void Pbx_OnBridgeAttendedTransferEvent(IAriClient sender, BridgeAttendedTransferEvent e)
        {
            Console.WriteLine("Blind Transfer");
            Console.WriteLine(e);
        }

        private void Pbx_OnChannelHoldEvent(IAriClient sender, ChannelHoldEvent e)
        {
            Console.WriteLine("Channel OnHold: " + e.Channel.Id);
        }

        private void Pbx_OnChannelHangupRequestEvent(IAriClient sender, ChannelHangupRequestEvent e)
        {
            Console.WriteLine("Channel hangup request para el canal: " + e.Channel.Id);
        }

        private void Pbx_OnChannelDestroyedEvent(IAriClient sender, ChannelDestroyedEvent e)
        {
            CallHandler callHandler = callHandlerCache.GetByChannelId(e.Channel.Id);
            if (callHandler.Caller.Id == e.Channel.Id)
            {
                actorPbxProxy.Send(new MessageCallerHangup() { CallHandlerId = callHandler.Id, HangUpCode = e.Cause.ToString(), HangUpReason = e.Cause_txt });
            }
            else if (callHandler.Caller.Id == e.Channel.Id)
            {
                actorPbxProxy.Send(new MessageHangUpAgent { CallHandlerId = callHandler.Id, HangUpCode = e.Cause.ToString(), HangUpReason = e.Cause_txt });
            }
            else {
                //algo salió mal, si estoy acá es porque el cache tiene un callhandler asociado al canal que cortó, pero el callhandler internamente no lo tienen ni como caller ni como agent a ese canal.
                Console.WriteLine("Pbx_OnChannelDestroyedEvent: no se pudo identificar mediante el id de canal si colgó el agente o el caller");
            }
            callHandlerCache.RemoveChannel(e.Channel.Id);
            Console.WriteLine("Channel Destroy: " + e.Channel.Id + " remuevo channel del callhandler");
        }

        private void Pbx_OnChannelStateChangeEvent(IAriClient sender, ChannelStateChangeEvent e)
        {
            //por ahora no hago nada, solo logueo.
            Console.WriteLine("El canal: " + e.Channel.Id + " cambio su estado a: " + e.Channel.State.ToString());
        }

        private void Pbx_OnStasisEndEvent(IAriClient sender, StasisEndEvent e)
        {
            Console.WriteLine("El canal: " + e.Channel.Id + " salió de la app: " + e.Application);
            callHandlerCache.GetByChannelId(e.Channel.Id);
            //uno de los dos cortó o por algun motivo se fue de stasis, transfer?? la cosa es que no estan mas en la app asi que los remuevo
            //aca debería ver el abandono, si sale de la app sin que lo atiendan abandonó?
            CallHandler callHandler = callHandlerCache.GetByChannelId(e.Channel.Id);
            if (callHandler != null) //esto es en caso de que existan llamadas en stasis antes de arrancar la app, debería cargar la info de lo preexistente en la pbx
                callHandlerCache.RemoveCallHandler(callHandler.Id);

        }

        private void Pbx_OnStasisStartEvent(IAriClient sender, StasisStartEvent e)
        {
            //Verifico: si el canal es de una llamada que ya existe no creo nada. Esto es para el caso en que hago un originate al agente, ya tengo un callhandler creado por el caller que llamó inicialmente
            if (callHandlerCache.GetByChannelId(e.Channel.Id) == null)
            {
                Console.WriteLine("El canal: " + e.Channel.Id + " entró a la app: " + e.Application);
                Bridge bridge = bridgesList.GetFreeBridge();
                if (bridge == null) //si no hay un bridge libre creo uno y lo agrego a la lista
                {
                    bridge = pbx.Bridges.Create("mixing", Guid.NewGuid().ToString());
                    bridgesList.AddNewBridge(bridge);
                    Console.WriteLine("Se crea un Bridge: " + bridge.Id);
                }
                else
                {
                    Console.WriteLine("Se usa un Bridge existente: " + bridge.Id);
                }


                CallHandler callHandler = new CallHandler(appName, pbx, bridge, e.Channel);
                callHandlerCache.AddCallHandler(callHandler);
                Console.WriteLine("Se crea un callhandler: " + callHandler.Id + " para el canal: " + e.Channel.Id);

                //Agrego el canal al bridge
                try
                {
                    //agrego el canal al bridge, controlar que pasa si falla el originate
                    pbx.Bridges.AddChannel(callHandler.Bridge.Id, e.Channel.Id, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("No se pudo agregar el canal: " + e.Channel.Id + " al bridge: " + callHandler.Bridge.Id + " Error: " + ex.Message);
                }

                //supongo que aca debo avisar a akka que cree el manejador para esta llamada y me mande el mesajito para que atienda
                actorPbxProxy.Send(new MessageNewCall() { CallHandlerId = callHandler.Id });
            }
            else
            {
                CallHandler callHandler = callHandlerCache.GetByChannelId(e.Channel.Id);
                try
                {
                    //agrego el canal al bridge, controlar que pasa si falla el originate
                    pbx.Bridges.AddChannel(callHandler.Bridge.Id, e.Channel.Id, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("No se pudo agregar el canal: " + e.Channel.Id + " al bridge: " + callHandler.Bridge.Id + " Error: " + ex.Message);
                }
            }

        }

        #endregion
    }
}
