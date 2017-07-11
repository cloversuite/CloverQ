using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    //representa las propiedades de la cola, MoH, Wrapup time, etc..
    public class DTOQueue
    {
        public string Id { get; set; }
        public string MoH { get; set; }
        public int Weight { get; set; }
        public TimeSpan WrapupTime { get; set; }

        public List<DTOQueueMember> QueueMembers { get; set; }

    }
}
