using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRouter
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
            bridges.Add(callHandler.Bridge.Id, callHandler);
            calls.Add(callHandler.Id, callHandler);
            if (callHandler.Agent != null)
                channels.Add(callHandler.Agent.Id, callHandler);

        }
        public void RemoveCallHandler(string id) {
            CallHandler c = this.GetByCallHandlerlId(id);
            channels.Remove(c.Caller.Id);
            bridges.Remove(c.Bridge.Id);
            calls.Remove(c.Id);
            if (c.Agent != null)
                channels.Remove(c.Agent.Id);
        }
        public CallHandler GetByChannelId(string id) { return channels[id]; }
        public CallHandler GetByBridgeId(string id) { return bridges[id]; }
        public CallHandler GetByCallHandlerlId(string id) { return calls[id]; }

        public void AddChannelToCallHandler(string callHandlerId, string channelId) {
            CallHandler c = this.GetByCallHandlerlId(callHandlerId);
            channels.Add(channelId, c);
        }

        public void RemoveChannel(string channelId) {
            channels.Remove(channelId);
        }
    }
}
