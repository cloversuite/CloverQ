using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages.Commands
{
    public class RESQueue
    {
        private List<RESCall> calls = new List<RESCall>();
        private List<RESMember> members = new List<RESMember>();

        public RESQueue()
        {

        }

        public string Id { get; set; }
        public string Media { get; set; }
        public string MediaType { get; set; }
        public int Weight { get; set; }
        public int WrapupTime { get; set; }

        public List<RESCall> Calls {
            get { return this.calls; }
        }

        public List<RESMember> Members
        {
            get { return this.members; }
        }
    }
}
