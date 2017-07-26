using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages
{
    /// <summary>
    /// Este mensaje lo envía el state provider para indicar que un dispositivo cambio su estado
    /// </summary>
    public class MessageStateChanged : Message
    {
        public string MemberId { get; set; }
        public string DeviceId { get; set; }
        public bool InUse { get; set; }
        public string Contact { get; set; } //tal vez debería tener un mensaje para si mimsmo, algo como MessageRegistrationChanged?

    }
}
