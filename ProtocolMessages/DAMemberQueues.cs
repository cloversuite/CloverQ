using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages
{
    public class DAMemberQueues
    {
        public string MemberId { get; set; }
        public List<string> MemberQueues { get; set; }
        public string RequestId { get; set; }
        public string From { get; set; }
    }
}
