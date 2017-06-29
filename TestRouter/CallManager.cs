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
        QActorSystem qActorSystem = new QActorSystem();
        ActorPbxProxy actorPbxProxy = null;
        BridgesList bridgesList = new BridgesList();
        CallHandlerCache callHandlerCache = new CallHandlerCache();
        private const string appName = "bridge_test";

        public CallManager()
        {
            //Creo el sistema de actores y el actor proxy para la pbx
            actorPbxProxy = qActorSystem.GetActorPbxProxy();
            actorPbxProxy.Receive += ActorPbxProxy_Receive;
            actorPbxProxy.AnswerCall += ActorPbxProxy_AnswerCall;
        }

        #region Handle Actor Sistem Events
        private void ActorPbxProxy_AnswerCall(object sender, MessageAnswerCall message)
        {
            pbx.Channels.Answer(callHandlerCache.GetByCallHandlerlId(message.CallHandlerId).Caller.Id);
        }
        private void ActorPbxProxy_Receive(object sender, ProtocolMessages.Message message)
        {
            //Aca entran todos los eventos del sistema de actores
        }

        #endregion

        /// <summary>
        /// Connecto to Asterisk ARI and WebSocket events
        /// </summary>
        /// <param name="server">IP or host name of asterisk</param>
        /// <param name="port">Port ej: 8088</param>
        /// <param name="usu">ARI user</param>
        /// <param name="pass">ARI password</param>
        public void Connect(string server, int port, string usu, string pass) {
            //CREO EL CLIENTE
            pbx = new AriClient(new StasisEndpoint(server, port, usu, pass), appName);
            //SUBSCRIBO A EVENTOS
            pbx.OnStasisStartEvent += Pbx_OnStasisStartEvent; //Se dispara cuando un canal ejecuta la app stasis en el dialplan. el canal queda ahi a la espera de ser manejado
            pbx.OnStasisEndEvent += Pbx_OnStasisEndEvent; //el canal abandonó la app stasis (no quiere decir que cortó)
            // pbx.OnChannelHangupRequestEvent //Se solicito terminar el canal (posee la causa ej: normal clearing)
            pbx.OnChannelStateChangeEvent += Pbx_OnChannelStateChangeEvent; //cambió el estado del canal ej: down->up->ringing. No se si lo voy a usar
            pbx.OnChannelDestroyedEvent += Pbx_OnChannelDestroyedEvent; //el canal fué terminado, sehizo efectivo el hangup
            pbx.OnChannelHoldEvent += Pbx_OnChannelHoldEvent; //el canal se puso onhold
            pbx.OnBridgeAttendedTransferEvent += Pbx_OnBridgeAttendedTransferEvent;
            pbx.OnBridgeBlindTransferEvent += Pbx_OnBridgeBlindTransferEvent;

            //CONECTO EL CLIENTE, true para habilitar reconexion, e intento cada 5 seg
            try
            {
                pbx.Connect(true, 5);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al conectar con asterisk", ex);
            }
            
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

        private void Pbx_OnChannelDestroyedEvent(IAriClient sender, ChannelDestroyedEvent e)
        {
            callHandlerCache.RemoveChannel(e.Channel.Id);
            Console.WriteLine("Channel Destroy: " + e.Channel.Id + " remuevo channel del callhandler" );
        }

        private void Pbx_OnChannelStateChangeEvent(IAriClient sender, ChannelStateChangeEvent e)
        {
            //por ahora no hago nada, solo logueo
            Console.WriteLine("El canal: " + e.Channel.Id + " cambio su estado a: " + e.Channel.State.ToString());
        }

        private void Pbx_OnStasisEndEvent(IAriClient sender, StasisEndEvent e)
        {
            Console.WriteLine("El canal: " + e.Channel.Id + " salió de la app: " + e.Application);
        }

        private void Pbx_OnStasisStartEvent(IAriClient sender, StasisStartEvent e)
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

            //supongo que aca debo avisar a akka que cree el manejador para esta llamada y me mande el mesajito para que atienda
            actorPbxProxy.Send(new MessageNewCall() { CallHandlerId = callHandler.Id });

            
        }

        #endregion
    }
}
