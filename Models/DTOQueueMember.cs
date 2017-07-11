using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    //representa un miembro de una cola, posee propiedades como priority que son específicas de la persona dentro de la cola
    public class DTOQueueMember
    {
        public string MemberId { get; set; }
        public int Priority { get; set; }

    }
}
