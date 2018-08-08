using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsterNET.ARI.Models;

namespace PbxCallManager
{
    //decorator para usar en la lista cacheBridge
    public class BridgeItem
    {
        Bridge bridge;
      
        public BridgeItem(Bridge bridge)
        {
            this.bridge = bridge;
            //si el bridge que le paso posee canales, seteo Free=false
            this.Free = bridge.Channels.Count == 0 ? true : false;
        }


        /// <summary>
        /// Get de underlying bridge
        /// </summary>
        public Bridge Bridge
        {
            get
            {
                return bridge;
            }
        }

        public string Id { get { return bridge.Id; } set { bridge.Id = value; } }
        public bool Free { get; set; }

    }
}
