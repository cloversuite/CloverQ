using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages
{
    public class MessageMemberRemove : Message
    {
        //Member ID
        public string MemberId { get; set; }
        //colas de donde lo tengo que remover, si null lo saco de todas
        public List<string> QueuesId { get; set; }
    }
}
