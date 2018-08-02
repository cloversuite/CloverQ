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
        CallTimeoutHandler callTimeOutHandler = new CallTimeoutHandler();
        private const string appName = "bridge_test";

        public CallManager(ActorPbxProxy actorPbxProxy)
        {
            //Creo el sistema de actores y el actor proxy para la pbx
            this.actorPbxProxy = actorPbxProxy;
            actorPbxProxy.Receive += ActorPbxProxy_Receive;
            actorPbxProxy.AnswerCall += ActorPbxProxy_AnswerCall;
            actorPbxProxy.CallTo += ActorPbxProxy_CallTo;
            //subcribo al evento del call timeout handler
            callTimeOutHandler.CallTimeOutEvent += CallTimeOutHandler_CallTimeOutEvent;

            //comienzo a monitorear los calltimeout
            callTimeOutHandler.Start();
            //Comienzo a recibir eventos
            actorPbxProxy.Start();
        }

        #region Handle Call Timeout Events
        /// <summary>
        /// This event fires every time that a call has timeout waiting on the queue
        /// </summary>
        /// <param name="callTimeOut">Holds the call id and the timeout</param>
        private void CallTimeOutHandler_CallTimeOutEvent(CallTimeOut callTimeOut)
        {
            //TODO: ver como hago que la llamada continue y enviar el msg adecuado al callditributor
            //Esto deberia generar un mensaje EXIT WITH TIMEOUT
            Console.WriteLine("La LLamada: " + callTimeOut.CallHandlerId + " Expiro!");
        }
        #endregion

        #region Handle Actor Sistem Events
        private void ActorPbxProxy_AnswerCall(object sender, MessageAnswerCall message)
        {
            CallHandler callHandler = callHandlerCache.GetByCallHandlerlId(message.CallHandlerId);
            try
            {
                if (message.MediaType == "MoH")
                {
                    callHandler.AnswerCaller(message.MediaType, message.Media);
                    //Si el calldistributor me envia timeout = 0 entonces no hay timeout
                    //Si el call handler posee timeout > 0 tiene precedencia sobre el message.timeout
                    int callTimeOut = 0;
                    if (callHandler.TimeOut > 0)
                        callTimeOut = callHandler.TimeOut;
                    else if (message.TimeOut > 0)
                        callTimeOut = message.TimeOut;

                    //si calltimeout = 0 significa quen no hay timeout
                    if (callTimeOut > 0)
                    {
                        callTimeOutHandler.AddCallTimeOut(
                            new CallTimeOut()
                            {
                                CallHandlerId = message.CallHandlerId,
                                TimeOut = callTimeOut
                            });
                    }
                    Console.WriteLine("El canal: " + callHandler.Caller.Id + " fué atendido correctamente ");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("No se pudo atender el canal: " + callHandler.Caller.Id + " Error: " + ex.Message);
            }

        }
        private void ActorPbxProxy_Receive(object sender, ProtocolMessages.Message message)
        {
            //Aca entran todos los eventos del sistema de actores
        }
        private void ActorPbxProxy_CallTo(object sender, MessageCallTo message)
        {
            CallHandler callHandler = callHandlerCache.GetByCallHandlerlId(message.CallHandlerId);
            Channel ch = null;
            try
            {
                //No deberia usar esto en vez de hacer el originate aca directamente?
                Console.WriteLine("CALL TO: " + message.Destination);
                ch = callHandler.CallTo(message.Destination);

                //Origino la llamada al agente. Seguramente hay que hacerlo async
                //ch = pbx.Channels.Originate(message.Destination, null, null, null, null, appName, "", "1111", 20, null, null, null, null);

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
            Console.WriteLine("La llamada " + ch.Id + " al agente: " + message.Destination + " se inició correctamente");

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
            pbx.EventDispatchingStrategy = EventDispatchingStrategy.DedicatedThread;
            //SUBSCRIBO A EVENTOS
            pbx.OnStasisStartEvent += Pbx_OnStasisStartEvent; //Se dispara cuando un canal ejecuta la app stasis en el dialplan. el canal queda ahi a la espera de ser manejado
            pbx.OnStasisEndEvent += Pbx_OnStasisEndEvent; //el canal abandonó la app stasis (no quiere decir que cortó)
            pbx.OnChannelHangupRequestEvent += Pbx_OnChannelHangupRequestEvent; //Se solicito terminar el canal (posee la causa ej: normal clearing)
            pbx.OnChannelStateChangeEvent += Pbx_OnChannelStateChangeEvent; //cambió el estado del canal ej: down->up->ringing. No se si lo voy a usar
            pbx.OnChannelDestroyedEvent += Pbx_OnChannelDestroyedEvent; //el canal fué terminado, sehizo efectivo el hangup
            pbx.OnChannelHoldEvent += Pbx_OnChannelHoldEvent; //el canal se puso onhold
            pbx.OnChannelUnholdEvent += Pbx_OnChannelUnholdEvent;
            pbx.OnChannelLeftBridgeEvent += Pbx_OnChannelLeftBridgeEvent;
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
            callTimeOutHandler.Stop();
            actorPbxProxy.Stop();
            pbx.Disconnect();
        }

        #region Handle ARI Events
        private void Pbx_OnBridgeBlindTransferEvent(IAriClient sender, BridgeBlindTransferEvent e)
        {
            ProtocolMessages.Message msg = null;
            CallHandler callHandler = callHandlerCache.GetByChannelId(e.Transferee.Id);
            if (callHandler == null)
            {
                callHandler = callHandlerCache.GetByChannelId(e.Replace_channel.Id);
            }
            if (callHandler != null)
            {
                msg = callHandler.AttendedTransferEvent(e.Transferee, e.Replace_channel);
            }
            //Mando el mensaje
            if (msg != null)
            {
                actorPbxProxy.Send(msg);
            }
            else
            {
                Console.WriteLine("UnAttTransfer devolvió msg = null");
            }
        }

        private void Pbx_OnBridgeAttendedTransferEvent(IAriClient sender, BridgeAttendedTransferEvent e)
        {
            ProtocolMessages.Message msg = null;
            //Este evento trae muchisima info, requiere de mayor estudio/prueba
            //ver como queda el canal del caller, seguro hay un rename por ahi
            CallHandler callHandler = callHandlerCache.GetByChannelId(e.Transferee.Id);
            if (callHandler == null)
            {
                callHandler = callHandlerCache.GetByChannelId(e.Transfer_target.Id);
            }
            if (callHandler != null)
            {
                msg = callHandler.AttendedTransferEvent(e.Transferee, e.Transfer_target);
            }
            //Mando el mensaje
            if (msg != null)
            {
                actorPbxProxy.Send(msg);
            }
            else
            {
                Console.WriteLine("AttTransfer devolvió msg = null");
            }

        }

        private void Pbx_OnChannelHoldEvent(IAriClient sender, ChannelHoldEvent e)
        {
            ProtocolMessages.Message msg = null;
            try
            {
                msg = callHandlerCache.GetByChannelId(e.Channel.Id).ChannelHoldEvent(e.Channel.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Channel Hold: ERROR " + ex.Message + "\n" + ex.StackTrace);
            }

            if (msg != null)
            {
                actorPbxProxy.Send(msg);
            }
            else
            {
                Console.WriteLine("Channel Hold: " + e.Channel.Id + " el callhandler devolvió msg = null");
            }
            Console.WriteLine("Channel Hold: " + e.Channel.Id);
        }

        private void Pbx_OnChannelUnholdEvent(IAriClient sender, ChannelUnholdEvent e)
        {
            ProtocolMessages.Message msg = null;
            try
            {
                msg = callHandlerCache.GetByChannelId(e.Channel.Id).ChannelUnHoldEvent(e.Channel.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Channel UnHold: ERROR " + ex.Message + "\n" + ex.StackTrace);
            }

            if (msg != null)
            {
                actorPbxProxy.Send(msg);
            }
            else
            {
                Console.WriteLine("Channel UnHold: " + e.Channel.Id + " el callhandler devolvió msg = null");
            }
            Console.WriteLine("Channel UnHold: " + e.Channel.Id);
        }

        private void Pbx_OnChannelHangupRequestEvent(IAriClient sender, ChannelHangupRequestEvent e)
        {
            ProtocolMessages.Message msg = null;
            try
            {
                msg = callHandlerCache.GetByChannelId(e.Channel.Id).ChannelHangupEvent(e.Channel.Id, e.Cause, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Channel HangUpReques: ERROR " + ex.Message + "\n" + ex.StackTrace);
            }

            if (msg != null)
            {
                actorPbxProxy.Send(msg);
            }
            else
            {
                Console.WriteLine("Channel HangUpReques: " + e.Channel.Id + " el callhandler devolvió msg = null");
            }
            //si la llamada finalizó remuevo todo
            CallHandler callHandler = callHandlerCache.GetByChannelId(e.Channel.Id);
            if (callHandler != null && callHandler.IsCallTerminated())
            {
                Console.WriteLine("Channel HangUpRequest: " + e.Channel.Id + ", call TERMINATED remuevo todo el callhandler: " + callHandler);
                callHandlerCache.RemoveCallHandler(callHandler.Id);
                
                Console.WriteLine("Channel HangUpRequest: el bridge: " + callHandler.Bridge.Id + " lo marco como free");
                bridgesList.SetFreeBridge(callHandler.Bridge.Id);
            }
            else // hago lo mismo que el channel destroy
            {
                callHandlerCache.RemoveChannel(e.Channel.Id);
                Console.WriteLine("Channel HangUpRequest: " + e.Channel.Id + " remuevo channel del callhandler");
            }
        }

        private void Pbx_OnChannelDestroyedEvent(IAriClient sender, ChannelDestroyedEvent e)
        {
            ProtocolMessages.Message msg = null;
            try
            {
                //si aun existe el callhandler manejo el evento
                if (callHandlerCache.GetByChannelId(e.Channel.Id) != null)
                    msg = callHandlerCache.GetByChannelId(e.Channel.Id).ChannelDestroyEvent(e.Channel.Id, e.Cause, e.Cause_txt);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en Pbx_OnChannelDestroyedEvent - " + ex.Message);
            }

            if (msg != null)
            {
                actorPbxProxy.Send(msg);
            }
            else
            {
                Console.WriteLine("Channel Destroy: " + e.Channel.Id + " el callhandler devolvió msg = null");
            }
            callHandlerCache.RemoveChannel(e.Channel.Id);
            Console.WriteLine("Channel Destroy: " + e.Channel.Id + " remuevo channel del callhandler");

        }

        private void Pbx_OnChannelLeftBridgeEvent(IAriClient sender, ChannelLeftBridgeEvent e)
        {
            //Aca no puedo marcar un bridge como free, el primer channel entra y sale varias veces por la negociacion o algo asi y en 
            //consecuencia cuando el primer channerl entra y sale y en ese momento tengo 0 channels en el bridge y lo marco como free, lo que es incorrecto
            //Esto produce que varias llamadas entrantes queden conectadas al mismo bridge, muuuuy malo!

            //string bridgeId = e.Bridge.Id;
            //int bridgeChannelsCount = e.Bridge.Channels.Count;
            //Console.WriteLine("El canal: " + e.Channel.Id+ ", dejo el bridge: " + bridgeId + " posee: " + bridgeChannelsCount);
            //if (bridgeChannelsCount == 0) {
            //    //esto si lo hago aca, ya que si el bridge no posee mas canales lo marco como libre para otra llamada
            //    Console.WriteLine("El bridge: " + bridgeId + " lo marco como free");
            //    bridgesList.SetFreeBridge(bridgeId);
            //}

        }

        private void Pbx_OnChannelStateChangeEvent(IAriClient sender, ChannelStateChangeEvent e)
        {
            ProtocolMessages.Message msg = null;
            //track channel state changes
            try
            {
                msg = callHandlerCache.GetByChannelId(e.Channel.Id).ChannelStateChangedEvent(e.Channel.Id, e.Channel.State);
                if (msg != null)
                {
                    actorPbxProxy.Send(msg);
                }
                else
                {
                    Console.WriteLine("Channel State Change: " + e.Channel.Id + " el callhandler devolvió msg = null");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR!!: Pbx_OnChannelStateChangeEvent chan:" + e.Channel.Id + ", error: " + ex.Message);
            }

            //log to console
            Console.WriteLine("El canal: " + e.Channel.Id + " cambio su estado a: " + e.Channel.State.ToString());
        }

        private void Pbx_OnStasisEndEvent(IAriClient sender, StasisEndEvent e)
        {
            Console.WriteLine("El canal: " + e.Channel.Id + " salió de la app: " + e.Application);
            //uno de los dos cortó o por algun motivo se fue de stasis, transfer?? la cosa es que no estan mas en la app asi que los remuevo
            //aca debería ver el abandono, si sale de la app sin que lo atiendan abandonó?
            CallHandler callHandler = callHandlerCache.GetByChannelId(e.Channel.Id);
            if (callHandler != null) //esto es en caso de que existan llamadas en stasis antes de arrancar la app, debería cargar la info de lo preexistente en la pbx
            {
                callHandlerCache.RemoveCallHandler(callHandler.Id);
                Console.WriteLine("El canal: " + e.Channel.Id + ", remuevo el callhandler: " + callHandler.Id);
            }
        }

        private void Pbx_OnStasisStartEvent(IAriClient sender, StasisStartEvent e)
        {
            if (e.Replace_channel != null) //esto me indica si no es null que hubo un rename, por ejemplo por un transfer
            {
                CallHandler callHandler = callHandlerCache.GetByChannelId(e.Replace_channel.Id);
                if (callHandler != null)
                {
                    //Lo comento porque si remplazo el canal, el nuevo canal no esta en stasis y nunca detecto el hangup
                    //para poder hacer esto debería recibir los eventos de todos los canales osea pbx.Applications.Subscribe(appName, "channel:"); 
                    callHandler.ChannelReplace(e.Replace_channel, e.Channel);
                }

            }
            else
            {

                //TODO: si e.Replace_channel != null (rename) es un nuevo canal que reemplaza a otro, hasta ahora solo me pasa con las transferencias atendidas, debo buscar el callhandler que tiene el e.Replace_channel y reemplazarlo por el nuevo channel
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
                        //Seteo la variabl callhandlerid del canal para identificarlo, esto solo para el caller
                        //ver que pasa cuando se hace un transfer a una cola, deberia cambiar el callhandlerid?
                        //En el hangup pregunto por esta variable y si la encuentro, libero la llamada y marco el bridge como libre
                        pbx.Channels.SetChannelVar(e.Channel.Id, "cq_callhandlerid",callHandler.Id);
                        //agrego el canal al bridge, controlar que pasa si falla el originate
                        pbx.Bridges.AddChannel(callHandler.Bridge.Id, e.Channel.Id, null);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("No se pudo agregar el canal: " + e.Channel.Id + " al bridge: " + callHandler.Bridge.Id + " Error: " + ex.Message);
                    }

                    //supongo que aca debo avisar a akka que cree el manejador para esta llamada y me mande el mesajito para que atienda
                    //TODO: si este parametro no existe no entrar al evento stasisstart
                    var queueId = e.Args[0];
                    if (e.Args.Count >= 1)
                        callHandler.SetTimeOut(e.Args[1]);
                    ProtocolMessages.Message msg = null;
                    msg = callHandler.SetCurrentQueue(queueId);
                    if (msg != null)
                        actorPbxProxy.Send(msg);
                }
                else //si no es null entonces el canal lo agregé yo cuando hice el CallTo
                {
                    CallHandler callHandler = callHandlerCache.GetByChannelId(e.Channel.Id);
                    try
                    {
                        //lo conecte a un member, asi que temuevo el timeout
                        callTimeOutHandler.CancelCallTimOut(callHandler.Id);
                        //Le digo al callhandler que el canal generado en el callto ya está en el dial plan, cuando el estado pasa a Up es que contestó el agente
                        callHandler.CallToSuccess(e.Channel.Id); 
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("No se pudo agregar el canal: " + e.Channel.Id + " al bridge: " + callHandler.Bridge.Id + " Error: " + ex.Message);
                    }
                }
            }
        }

        #endregion
    }
}
