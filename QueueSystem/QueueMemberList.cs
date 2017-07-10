using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueSystem
{
    /// <summary>
    /// Representa la lista de miembros de una cola, posee una estrategia para seleccionar el próximo libre
    /// Es una lista de QueueMember no de Member
    /// </summary>
    class QueueMemberList
    {
        /// <summary>
        /// Estrategia que contiene el algoritmo para selecciónar el próximo miembro disponible
        /// </summary>
        IMemberStrategy strategy = null;

        /// <summary>
        /// Mantiene una lista de los miembros que pertenecen a un CallDispatcher
        /// </summary>
        List<QueueMember> members = new List<QueueMember>();

        /// <summary>
        /// Cosntructor por defecto que crea una lista vacia y una estrategia rr memory por defecto
        /// </summary>
        public QueueMemberList() {
            members = new List<QueueMember>();
            strategy = new MemberStrategyRRM(members);
        }

        /// <summary>
        /// Cosntructor que recibe una lista de miembros
        /// </summary>
        /// <param name="members"></param>
        public QueueMemberList(List<QueueMember> members, IMemberStrategy strategy) {
            this.members = members;
            this.strategy = strategy;
            this.strategy.Members = this.members;

        }

        /// <summary>
        /// Método que me permite cambiar la estrategia de seleccion de miembro
        /// </summary>
        /// <param name="strategy"></param>
        public void SetMemberSrtategy(IMemberStrategy strategy) {
            this.strategy = strategy;
        }

        /// <summary>
        /// Propiedad que permite asignar u obtener la lista subyacente de miembros
        /// </summary>
        List<QueueMember> Members { get { return members; } set { members = value; } }

        /// <summary>
        /// Meétodo para actualizar el estado del device de un member
        /// </summary>
        /// <param name="member">member con su state actualizado</param>
        public void UpdateState(QueueMember member) {
            // TODO: analizar si conviene recibir un member o el ID y su nuevo estado 
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Agrega un miembro a la lista
        /// </summary>
        /// <param name="member"></param>
        public void AddMember(QueueMember member) { throw new NotImplementedException(); }
        
        /// <summary>
        /// Remueve un miembro de la lista
        /// </summary>
        /// <param name="member"></param>
        public void RemoveMember(QueueMember member) { throw new NotImplementedException(); }
        
        /// <summary>
        /// Pone un miembro en "No disponible" o pausado
        /// </summary>
        /// <param name="member">Miembro de la lista a pausar</param>
        /// /// <param name="member">Motivo por el que se pausó el miembro de la lista</param>
        public void PauseMember(QueueMember member, string reazon) { throw new NotImplementedException(); }
        
        /// <summary>
        /// Pone un miembro de la lista como disponible
        /// </summary>
        /// <param name="member"></param>
        public void UnpauseMember(QueueMember member) { throw new NotImplementedException(); }

        /// <summary>
        /// Devuelve el próximo miembro dsiponible según la estrategia indicada
        /// </summary>
        /// <returns></returns>
        public QueueMember NextAvailable() {
            return strategy.GetNext();
        }
    }
}
