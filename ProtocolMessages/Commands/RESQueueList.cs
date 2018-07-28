using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages.Commands
{
    public class RESQueueList
    {
        private List<RESQueue> queues = new List<RESQueue>();

        public string Nombre { get; set; }

        public List<RESQueue> Queues
        {
            get
            {
                return this.queues;
            }
        }

    }
}
