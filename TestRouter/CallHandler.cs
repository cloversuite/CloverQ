using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRouter.Proxy;
using AsterNET.ARI.Models;
using AsterNET.ARI;

namespace TestRouter
{
    public class CallHandler
    {
        string id;
        string appName;
        AriClient pbx;
        Bridge bridge;
        Channel caller;
        Channel agent;

        //Constructor
        public CallHandler(string appName, AriClient pbx, Bridge bridge, Channel caller) 
        {
            this.id = Guid.NewGuid().ToString();
            this.appName = appName;
            this.pbx = pbx;
            this.bridge = bridge;
            this.caller = caller;
            this.agent = null;

        }

        public Channel CallTo(string dst) {

            try
            {
                agent = pbx.Channels.Originate(dst, null, null, null, null, appName, "", "1111", 20, null, null, null, null);
            }
            catch (Exception ex)
            {
                throw new Exception("Error llamando a:  " + dst, ex);
            }

            return agent;
        }

        public void AnswerCaller(bool playMOH) {
            try
            {
                //atiendo el caller
                pbx.Channels.Answer(caller.Id);
                //inicio musica en espera si playMOH es true
                if (playMOH) pbx.Bridges.StartMoh(bridge.Id, "default");
                //agrego el canal al bridge
                pbx.Bridges.AddChannel(bridge.Id, caller.Id, null);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al agregar el caller: " + caller.Id + " al bridge: " + bridge.Id, ex);
            }
        }

    }
}
