using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueSystem
{
    /// <summary>
    /// Esta clase es utilizada por la clase MemberList, implementa un rrmemory 
    /// para la lista de miembros (MemberList) a la que pertenece
    /// </summary>
    class MemberStrategyRRM : IMemberStrategy
    {
        List<QueueMember> members;

        public MemberStrategyRRM(List<QueueMember> members) {
            this.members = members;
        }

        public MemberStrategyRRM() {
        }

        public List<QueueMember> Members {
            set { members = value; }
            get { return members; }
        }

        public QueueMember GetNext()
        {
            QueueMember next = null;

            if (members != null && members.Count > 0)
            {
                //Implementación muy simple de round robin con memoria
                next = members[0];
                members.RemoveAt(0);
                members.Add(next);
            }
            //Si next es nulo no hay agentes disponibles
            return next;
        }
    }
}
