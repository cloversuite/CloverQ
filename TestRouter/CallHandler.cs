using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public string Id
        {
            get
            {
                return id;
            }
        }
        public Bridge Bridge
        {
            get
            {
                return bridge;
            }

            set
            {
                bridge = value;
            }
        }
        public Channel Caller
        {
            get
            {
                return caller;
            }

            set
            {
                caller = value;
            }
        }
        public Channel Agent
        {
            get
            {
                return agent;
            }

            set
            {
                agent = value;
            }
        }

        

        //Constructor
        public CallHandler(string appName, AriClient pbx, Bridge bridge, Channel caller) 
        {
            this.id = Guid.NewGuid().ToString();
            this.appName = appName;
            this.pbx = pbx;
            this.bridge = bridge;
            bridge.Channels.Add(caller.Id);
            this.caller = caller;
            this.agent = null;

        }

        public Channel CallTo(string dst) {

            try
            {
                agent = pbx.Channels.Originate(dst, null, null, null, null, appName, "", "1111", 20, null, null, null, null);
                bridge.Channels.Add(agent.Id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error llamando a:  " + dst, ex);
            }

            return agent;
        }

        public void AnswerCaller(string mediaType, string media) {
            try
            {
                //atiendo el caller
                pbx.Channels.Answer(caller.Id);
                //agrego el canal al bridge
                pbx.Bridges.AddChannel(bridge.Id, caller.Id, null);
                //inicio musica en espera si playMOH es true
                if (!String.IsNullOrEmpty(mediaType)) pbx.Bridges.StartMoh(bridge.Id, media);

            }
            catch (Exception ex)
            {
                throw new Exception("Error al agregar el caller: " + caller.Id + " al bridge: " + bridge.Id, ex);
            }
        }

        public void ChannelHangup(string channelId) {
            bridge.Channels.Remove(channelId);
            if (channelId == caller.Id)
                Console.WriteLine("El llamante colgó - canal: " + caller.Id + "CallHandler: " + this.id);
            else if (channelId == agent.Id)
                Console.WriteLine("El agent colgó - canal: " + caller.Id + "CallHandler: " + this.id);
            else
                Console.WriteLine("El canal " + caller.Id + " no está en la llamada: " + this.id);
        }

    }
}
