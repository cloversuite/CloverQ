using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProtocolMessages
{
    //Este mensaje lo usa el queuesystem para cargar los miembros
    public class DAGetMemberById
    {
        public string MemberId { get; set; }
    }
}