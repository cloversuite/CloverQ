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
        public string Media { get; set; }
        public string MediaType { get; set; }
        public int Weight { get; set; }
        public int WrapupTime { get; set; }
        public string MemberStrategy { get; set; }
        public string CallOrderStrategy { get; set; }

        public List<DTOQueueMember> QueueMembers { get; set; }

    }
}
