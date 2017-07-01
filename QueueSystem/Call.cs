using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueSystem
{
    class Call
    {
        public string CallHandlerId { get; set; }
        public bool IsDispatching { get; set; } // será que esto indica que la mandé conectar???
    }
}
