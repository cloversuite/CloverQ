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
    public class QueueCache
    {
        //Este array es para mantener las colas (listas) segun su peso, el indice debe coincidir con el peso de la cola
        //de esta manera puedo recorrela de menor a mayor peso y priorizar llamadas segun peso de la cola
        List<Queue> queuesWeight = new List<Queue>();
        //Comparador para el sort, compara por Weight
        QueueWeightComparer queueWeightComparer = new QueueWeightComparer();

        public QueueCache()
        {

        }

        public void AddQueue(Queue queue)
        {
            queuesWeight.Add(queue);
            queuesWeight.Sort(queueWeightComparer);
        }

        public List<Queue> QueueList
        {
            get
            {
                return this.queuesWeight;
            }
        }

    }


    //TODO: sacar esto afuera, tal vez sea necesario tener diferentes estrategias de ordenamiento
    class QueueWeightComparer : IComparer<Queue>
    {
        public int Compare(Queue x, Queue y)
        {
            //TODO:Controlar que ninguno sea nulo
            if (x.Weight == y.Weight)
            {
                return 0;
            }
            else if (x.Weight > y.Weight)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
}
