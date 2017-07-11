using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    //Representa un miembro o agente, tiene propiedades relacionadas a la persona, agentid, pass, etc...
    public class DTOMember
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Contact { get; set; } //sip uri?, puede ser por si lo leo de la BD de un opensips/kamailio? 
        public bool IsLogedIn { get; set; } //no se si va aca, puede ser por si lo leo de la BD de un opensips/kamailio?
        public bool IsAvailable { get; set; } //por si inicialmente lo leo de un prescense server?
        public int DeviceState { get; set; } //no se si es int, es por si lo leo de un prescense server / registrar?
    }
}
