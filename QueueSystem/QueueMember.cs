using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueSystem
{
    /// <summary>
    /// Decorardor para un Member, esta clase contiene al 
    /// Member y agrega atributos propios de una cola, por ejemplo priority 
    /// que es una propidad del member especifica de cada cola.
    /// En resumen, tengo una instancia de Member unica para cada agente, y para agregar el agente a la cola creo un new QueueMember(Member)
    /// QueueMember deberá contener las propiedades de ese Member dentro de esa cola
    /// </summary>
    public class QueueMember
    {
        private Member member;

        public QueueMember(Member member)
        {
            this.member = member;
            Priority = 0;
        }

        public string Id
        {
            get { return this.member.Id; }
        }
        public Member Member { get { return member; } }
        public DateTime LastCall { get; set; }
        public int Priority { get; set; }
        public bool IsPaused { get; set; }

        public void MarkLastCallTime()
        {
            LastCall = DateTime.Now;
        }

    }
}
