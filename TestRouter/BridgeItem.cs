using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsterNET.ARI.Models;

namespace TestRouter
{
    //decorator para usar en la lista cacheBridge
    public class BridgeItem
    {
        Bridge bridge;
      
        public BridgeItem(Bridge bridge)
        {
            this.bridge = bridge;
        }


        /// <summary>
        /// Get de underlaying bridge
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
