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
        /// <summary>
        /// Indica que el dispositivo esta siendo utilizado con una llamada o mas
        /// </summary>
        public bool InUse { get; set; }
        /// <summary>
        /// Ubicación del dispositivo Ej: sip:1000@293.268.56.123:5060
        /// </summary>
        public string Contact { get; set; } //tal vez debería tener un mensaje para si mimsmo, algo como MessageRegistrationChanged?
    }
}
