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

        public QueueMember GetNext() {
            QueueMember next = null;

            if (members != null && members.Count > 0)
            {
                do
                {
                    //Implementación muy simple de round robin con memoria
                    next = members[0];
                    members.RemoveAt(0);
                    members.Add(next);
                } while (!next.IsPaused);
            }
            //Si next es nulo no hay agentes disponibles
            return next;
        }

        public QueueMember GetNext(int wrapupTime)
        {
            QueueMember next = null;
            QueueMember last = null;

            if (members != null && members.Count > 0)
            {
                last = members[0];
                bool allBusy = false;
                bool hasOne = false;
                do {
                    //Implementación muy simple de round robin con memoria
                    next = members[0];
                    members.RemoveAt(0);
                    members.Add(next);

                    //TODO: poner esta logica dentro del queuemember cosa de solo consultar una propiedad 
                    if ((next.Member.IsAvailable && !next.IsPaused && next.LastCall.AddSeconds((double)wrapupTime) < DateTime.Now)) //!next.Member.IsAvailable ||
                    {
                        hasOne = true;
                        next.Member.IsAvailable = false;
                    }

                    if (next == last) //ya recorri todos y etan ocupados
                    {
                        allBusy = true;
                    }
                } while ( !allBusy && !hasOne );

                if (!hasOne) next = null;
            }
            //Si next es nulo no hay agentes disponibles
            return next;
        }
    }
}
