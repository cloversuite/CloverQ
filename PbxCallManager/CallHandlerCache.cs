using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallManager
{
    public class CallHandlerCache
    {
        Dictionary<string, CallHandler> bridges = new Dictionary<string, CallHandler>();
        Dictionary<string, CallHandler> channels = new Dictionary<string, CallHandler>();
        Dictionary<string, CallHandler> calls = new Dictionary<string, CallHandler>();
        public CallHandlerCache()
        {

        }

        public void AddCallHandler(CallHandler callHandler) {
            channels.Add(callHandler.Caller.Id, callHandler);
            try
            {
                bridges.Add(callHandler.Bridge.Id, callHandler);
            }
            catch(Exception ex) {
                Console.WriteLine("No se pudo agregar al cache el bridge: " + callHandler.Bridge.Id + "Error: " + ex.Message);
            }
            calls.Add(callHandler.Id, callHandler);
            if (callHandler.Agent != null)
                channels.Add(callHandler.Agent.Id, callHandler);

        }
        public void RemoveCallHandler(string id) {
            CallHandler c = this.GetByCallHandlerlId(id);
            channels.Remove(c.Caller.Id);
            //bridges.Remove(c.Bridge.Id); //no lo debo remover, debo marcarlo como libre para otra llamada
            calls.Remove(c.Id);
            if (c.Agent != null)
                channels.Remove(c.Agent.Id);
        }
        public CallHandler GetByChannelId(string id) {
            if (channels.ContainsKey(id))
                return channels[id];
            else
                return null;
        }
        public CallHandler GetByBridgeId(string id)
        {
            if (bridges.ContainsKey(id))
                return bridges[id];
            else
                return null;
        }
   
        public CallHandler GetByCallHandlerlId(string id)
        {
            if (calls.ContainsKey(id))
                return calls[id];
            else
                return null;
        }
   

        public void AddChannelToCallHandler(string callHandlerId, string channelId) {
            CallHandler c = this.GetByCallHandlerlId(callHandlerId);
            channels.Add(channelId, c);
        }

        public void RemoveChannel(string channelId) {
            channels.Remove(channelId);
        }
    }
}
