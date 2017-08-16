using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueSystem
{
    public class Call
    {
        public string CallHandlerId { get; set; }
        public bool IsDispatching { get; set; } //  esto indica que ya mandé al callhandler un memeber para que contacte, y aún no concretó
        public bool Connected { get; set; }
        public long SenderUid { get; set; }
        public QueueMember QueueMember { get; set; } //contiene el memeber que mandé a conectar
    }
}
