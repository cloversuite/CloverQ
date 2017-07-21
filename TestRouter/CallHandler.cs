using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsterNET.ARI.Models;
using AsterNET.ARI;
using ProtocolMessages;

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

        public Channel CallTo(string dst)
        {

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

        public void CallToSuccess(string channelId)
        {
            if (this.agent.Id != channelId)
            {
                throw new Exception("Callhandler: CallToSucces: " + channelId + " no es un canal de agente: ");
            }

            try
            {
                pbx.Bridges.StopMoh(this.Bridge.Id);
                //agrego el canal al bridge, controlar que pasa si falla el originate
                pbx.Bridges.AddChannel(this.Bridge.Id, channelId, null);
            }
            catch (Exception ex)
            {
                throw new Exception("Callhandler: Error al agregar el agent: " + channelId + " al bridge: " + bridge.Id, ex);
            }

        }

        public void AnswerCaller(string mediaType, string media)
        {
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
                throw new Exception("Callhandler: Error al agregar el caller: " + caller.Id + " al bridge: " + bridge.Id, ex);
            }
        }

        public ProtocolMessages.Message ChannelStateChangedEvent(string channelId, string newState)
        {
            ProtocolMessages.Message msg = null;

            if (channelId == caller.Id)
            {
                caller.State = newState;
            }
            else if (channelId == agent.Id)
            {
                agent.State = newState;
                if (newState == "Up") //Esto indica que el canal es de un agente y atendió la llamada
                {
                    msg = new MessageCallToSuccess() { CallHandlerId = this.id };
                }
            }
            else
            {
                Console.WriteLine("Callhandler: El canal " + caller.Id + " no está en la llamada: " + this.id);
            }

            return msg;
        }

        //por lo que pude relevar el evento channelDestry solo llega si el canal no fué atendido
        //hay que ver bien cause que valores toma en los casos de una llamada terminada por timeout, o por falla
        //basado en lo que dije en las lineas anteriores, no debería llegar aca por un canal de caller, ya que al entrar en stasis
        //debería haber sido atendido. Si pasa con las llamadas que inicio hacia los agentes
        public ProtocolMessages.Message ChannelDestroyEvent(string channelId, int cause, string causeText)
        {
            ProtocolMessages.Message msg = null;
            //Hay que pulir la lógica del hangup, también hay que tener en cuenta los transfer
            if (channelId == caller.Id)
            {
                msg = new MessageCallerHangup() { CallHandlerId = this.id, HangUpCode = cause.ToString(), HangUpReason = causeText };
            }
            else if (channelId == agent.Id)
            {
                msg = new MessageCallToFailed() { CallHandlerId = this.id, Code = cause, Reason = causeText };
            }
            else
                Console.WriteLine("Callhandler: El canal " + caller.Id + " no está en la llamada: " + this.id);

            return msg;
        }

        //por lo que pude relevar el evento channelHangup solo llega si el canal no atendido
        //hay que ver bien cause que valores toma en los casos de una llamada terminada por falla
        //basado en lo que dije en las lineas anteriores, aca debería llegar por un canal de caller que corta, ya que al entrar en stasis
        //debería haber sido atendido. o bien por un agente que corta
        public ProtocolMessages.Message ChannelHangupEvent(string channelId, int cause, string causeText)
        {
            ProtocolMessages.Message msg = null;
            //Hay que pulir la lógica del hangup, también hay que tener en cuenta los transfer
            if (channelId == caller.Id)
            {
                msg = new MessageCallerHangup() { CallHandlerId = this.id, HangUpCode = cause.ToString(), HangUpReason = causeText };
                TerminateAgent();
            }
            else if (channelId == agent.Id)
            {
                msg = new MessageAgentHangup() { CallHandlerId = this.id, HangUpCode = cause.ToString(), HangUpReason = causeText };
                TerminateCaller();
            }
            else
                Console.WriteLine("Callhandler: El canal " + caller.Id + " no está en la llamada: " + this.id);

            return msg;
        }

        private void TerminateCaller() {
            TerminateLeg(this.caller.Id);
        }
        private void TerminateAgent() {
            TerminateLeg(this.agent.Id);
        }

        private void TerminateLeg(string channelId) {
            try
            {
                pbx.Channels.Hangup(channelId);
            }
            catch (Exception ex) {
                Console.WriteLine("CallHandler: " + this.id + " request hangup failed on channel: " + channelId + "Error: " + ex.Message );
            }

        }

    }
}
