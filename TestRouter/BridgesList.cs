using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsterNET.ARI.Models;

namespace TestRouter
{
    public class BridgesList
    {
        List<BridgeItem> bridgeCache = new List<BridgeItem>();

        public BridgesList()
        {

        }
        public Bridge GetFreeBridge()
        {
            Bridge b = null;
            foreach (BridgeItem bi in bridgeCache)
            {
                if (bi.Free)
                    b = bi.Bridge;
            }
            return b;
        }

        public BridgeItem AddNewBridge(Bridge bridge)
        {
            BridgeItem bi = new BridgeItem(bridge);
            bi.Free = true;
            bridgeCache.Add(bi);
            return bi;
        }

        public List<Bridge> Bridges
        {
            get
            {
                List<Bridge> bridges = new List<Bridge>();
                foreach (BridgeItem b in this.bridgeCache) {
                    bridges.Add(b.Bridge);
                }
                return bridges;
            }
        }

    }
}
