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
    public class MemberCache
    {
        //Diccionario para acceder a un member por su id
        Dictionary<string, Member> members = new Dictionary<string, Member>();

        //Diccionario para acceder a una lista de agentes por su número de cola, cada lista es una vista de la lista de miembros
        Dictionary<string, List<Member>> queueMembers = new Dictionary<string, List<Member>>();


        public MemberCache() { }

        #region Métodos

        public void Add(Member member)
        {
            if (!members.ContainsKey(member.Id))
            {
                members.Add(member.Id, member);
            }
        }

        public Member Remove(string memberId)
        {
            Member member = null;
            if (members.ContainsKey(memberId))
            {
                member = members[memberId];
                members.Remove(memberId);
            }
            return member;
        }

        public Member GetMemberById(string memberId)
        {
            Member member = null;
            if (members.ContainsKey(memberId))
            {
                member = members[memberId];
            }
            return member;
        }

        //La info de contacto la puedo obtener si se logea con una llamada desde el tel
        //Habria que ver para poder hacer login desde una aplicacion, como le paso el tel que le pertences?
        public void MemberLogin(Member member)
        {
            //TODO: Validar credenciales, debería actualizar la info de contacto aca??
            if (members.ContainsKey(member.Id))
            {
                Member m = members[member.Id];
                //verifico si se relogueo desde otro endpoint, si es asi actualizo el contacto
                if (members[member.Id].DeviceId != member.DeviceId)
                {
                    members[member.Id].DeviceId = member.DeviceId;
                    members[member.Id].Contact = member.Contact;
                }
                member.IsLogedIn = true;
                members[member.Id].IsAvailable = true;
                m.SetLoginTime();
            }
        }
        public void MemberLogoff(string memberId)
        {
            if (members.ContainsKey(memberId))
            {
                Member m = members[memberId];
                m.SetLogoffTime();
                m.PauseCode = "";
                m.PauseReason = "";
                m.IsPaused = false;
                m.IsLogedIn = false;
                

            }
        }
        //public void Pause(Member m) { }
        //public void Unpause(Member m) { }
        //public void Add(Member m, Queue q) { }
        //public void Remove(Member m, Queue q) { }
        //public void GetFree(Queue q) { }
        public Member MemberUnPause(string memberId)
        {
            Member m = null;
            if (members.ContainsKey(memberId))
            {
                m = members[memberId];
                m.PauseCode = "";
                m.PauseReason = "";
                m.IsPaused = false;
                m.SetUnpauseTime();
            }
            return m;
        }
        public Member MemberPause(string memberId, string pauseCode, string pauseReaon)
        {
            Member m = null;
            if (members.ContainsKey(memberId))
            {
                m = members[memberId];
                m.PauseCode = pauseCode;
                m.PauseReason = pauseReaon;
                m.IsPaused = true;
                m.SetPauseTime();
            }
            return m;
        }
        #endregion

    }
}
