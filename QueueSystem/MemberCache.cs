using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueSystem
{
    /// <summary>
    /// Esta clase contiene todos los miembros del sistema
    /// </summary>
    class MemberCache
    {
        //Diccionario para acceder a un member por su id
        Dictionary<string, Member> memberID_idx = new Dictionary<string, Member>();

        //Diccionario para acceder a una lista de agentes por su número de cola, cada lista es una vista de la lista de miembros
        Dictionary<string, List<Member>> queueMembers = new Dictionary<string, List<Member>>();


        public MemberCache() { }

        #region Métodos

        public void MemberLogin(Member m) { }
        public void MemberLogoff(Member m) { }
        //public void MemberPause(Member m) { }
        //public void MemberUnpause(Member m) { }
        //public void MemberAdd(Member m, Queue q) { }
        //public void MemberRemove(Member m, Queue q) { }
        //public void MemberGetFree(Queue q) { }

        #endregion

    }
}
