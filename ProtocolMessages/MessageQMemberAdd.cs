using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages
{
    public class MessageQMemberAdd
    {
        //Member ID
        public string MemberId { get; set; }

        //Lista de IDs de las colas a la que quiere loguearse
        public List<string> QueuesId { get; set; }
        public string RequestId { get; set; }
    }
}
