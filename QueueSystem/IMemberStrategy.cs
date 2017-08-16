using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueSystem
{
    public interface IMemberStrategy
    {
        /// <summary>
        /// Estrategia para determinar el pròximo miembro diponible 
        /// </summary>
        /// <returns></returns>
        QueueMember GetNext();
        QueueMember GetNext(int wrapupTime);
        List<QueueMember> Members { get; set; }
    }
}
