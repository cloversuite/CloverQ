using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages
{
    /// <summary>
    /// Este mensaje lo envía el state provider e indica que un dispositivo cambió su información de contacto (ubicación)
    /// </summary>
    class MessageContactChanged
    {
        /// <summary>
        /// Nueva ubicacion del dispositivo Ej: sip:1000@192.168.56.123:5060
        /// </summary>
        public string Contact { get; set; }
    }
}
