using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsterNET.ARI.Models;
using AsterNET.ARI;

namespace TestRouter
{
    public class CallManager
    {
        AriClient pbx;
        List<BridgeItem> bridgeCache = new List<BridgeItem>();
        CallHandlerCache callHandlerCache = new CallHandlerCache();
        private const string appName = "bridge_test";

        public CallManager()
        {
          
        }
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

        private void Pbx_OnChannelHoldEvent(IAriClient sender, ChannelHoldEvent e)
        {
            throw new NotImplementedException();
        }

        private void Pbx_OnChannelDestroyedEvent(IAriClient sender, ChannelDestroyedEvent e)
        {
            callHandlerCache.RemoveChannel(e.Channel.Id);
        }

        private void Pbx_OnChannelStateChangeEvent(IAriClient sender, ChannelStateChangeEvent e)
        {
            //por ahora no hago nada, solo logueo
            Console.WriteLine("El canal: " + e.Channel.Id + " cambio su estado a: " + e.Channel.State.ToString());
        }

        private void Pbx_OnStasisEndEvent(IAriClient sender, StasisEndEvent e)
        {
            throw new NotImplementedException();
        }

        private void Pbx_OnStasisStartEvent(IAriClient sender, StasisStartEvent e)
        {
            Bridge bridge = GetFreeBridge();
            CallHandler callHandler = new CallHandler(appName, pbx, bridge, e.Channel);
            callHandlerCache.AddCallHandler(callHandler);
            //va aca??
            callHandler.AnswerCaller(true);
            //supongo que aca debo avisar a akka que cree el manejador para esta llamada
        }

        private Bridge GetFreeBridge() {
            Bridge b = null;
            foreach (BridgeItem bi in bridgeCache) {
                if (bi.Free)
                    b = bi.Bridge;
            }
            if (b == null)
                b = NewBridgeItem().Bridge;

            return b;
        }

        private BridgeItem NewBridgeItem() {
            Bridge b = pbx.Bridges.Create("mixing", Guid.NewGuid().ToString());
            BridgeItem bi = new BridgeItem(b);
            bi.Free = true;
            return bi;
        }
    }
}
