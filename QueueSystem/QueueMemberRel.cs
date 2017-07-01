using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueSystem
{
    /// <summary>
    /// Sirve para mantener la relacion entre los id de colas y miembros.
    /// Utiliza strings para los id de ambos.
    /// </summary>
    public class QueueMemberRel
    {
        /// <summary>
        /// Diccionario Id_Member -> Queues
        /// </summary>
        Dictionary<string, List<string>> memberQueues = new Dictionary<string, List<string>>();

        /// <summary>
        /// Diccionario Id_Queue -> Members 
        /// </summary>
        Dictionary<string, List<string>> queueMembers = new Dictionary<string, List<string>>();

        #region Métodos
        public void AddQueue(string queueId)
        {
            if (!queueMembers.ContainsKey(queueId))
                queueMembers.Add(queueId, null);
        }
        public void AddQueuesRange(List<string> queueIds)
        {
            foreach (string qid in queueIds)
                AddQueue(qid);
        }
        public void AddMember(string memberId)
        {
            if (!memberQueues.ContainsKey(memberId))
                memberQueues.Add(memberId, null);
        }
        public void AddMembersRange(List<string> memberIds)
        {
            foreach (string mid in memberIds)
                AddMember(mid);
        }
        public void AddMemberToQueue(string memberId, string queueId)
        {
            if (memberQueues.ContainsKey(memberId) && queueMembers.ContainsKey(queueId))
            {
                //agrego la cola al member
                if (memberQueues[memberId] == null)
                {
                    memberQueues[memberId] = new List<string>() { queueId };
                }
                else
                {
                    memberQueues[memberId].Add(queueId);
                }

                //agrego el member a la cola
                if (queueMembers[queueId] == null)
                {
                    queueMembers[queueId] = new List<string> { memberId };
                }
                else
                {
                    queueMembers[queueId].Add(memberId);
                }

            }
        }
        public void AddMembersToQueue(List<string> memberIds, string queueId)
        {
            foreach (string mid in memberIds) { AddMemberToQueue(mid, queueId); }
        }

        public void RemoveQueue(string queueId)
        {
            if (queueMembers[queueId] != null)
            {
                foreach (string mid in queueMembers[queueId])
                {
                    memberQueues[mid].Remove(queueId);
                }
                queueMembers.Remove(queueId);

            }

        }
        public void RemoveQueuesRange(List<string> queueIds)
        {
            foreach (string qid in queueIds)
            {
                RemoveQueue(qid);
            }
        }
        public void RemoveMember(string memberId)
        {
            if (memberQueues[memberId] != null)
            {
                foreach (string qid in memberQueues[memberId])
                {
                    queueMembers[qid].Remove(memberId);
                }
                memberQueues.Remove(memberId);
            }
        }
        public void RemoveMembersRange(List<string> memberIds)
        {
            foreach (string mid in memberIds)
            {
                RemoveMember(mid);
            }
        }
        public void RemoveMemberFromQueue(string memberId, string queueId) {
            if (queueMembers.ContainsKey(queueId)) {
                if(queueMembers[queueId] != null)
                    queueMembers[queueId].Remove(memberId);
            }
            if (memberQueues.ContainsKey(memberId)) {
                if (memberQueues[memberId] != null) {
                    memberQueues[memberId].Remove(queueId);
                }
            }
        }
        public void RemoveMembersFromQueue(List<string> memberIds, string queueId) {
            foreach (string mid in memberIds)
                RemoveMemberFromQueue(mid, queueId);
        }

        public int GetMembersCount() {
            return memberQueues.Count;
        }

        public int GetQueuesCount()
        {
            return queueMembers.Count;
        }
        public List<string> GetQueueMembers(string queueId) {
            return queueMembers[queueId];
        }
        public List<string> GetMemberQueues(string memberId) {
            return memberQueues[memberId];
        }

        #endregion

    }
}
