using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueSystem
{
    /// <summary>
    /// Esta clase contiene todos las colas del sistema
    /// </summary>
    class QueueCache
    {
        //Este array es para mantener las colas (listas) segun su peso, el indice debe coincidir con el peso de la cola
        //de esta manera puedo recorrela de menor a mayor peso y priorizar llamadas segun peso de la cola
        List<Queue> queuesWeight = new List<Queue>();
        
        public QueueCache() {
            
        }
    }
}
