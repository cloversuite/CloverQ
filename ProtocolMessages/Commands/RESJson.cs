using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages.Commands
{
    /// <summary>
    /// Esta clase solo lleva un string con el json de una respuesta, esto lo hago porque estoy teniendo
    /// inconvenientes con la serializacion de akka.net, y no logro serializar objetos complejos (no POCO)
    /// TODO: hacer funcionar la serializacion de akka para remoting y reemplazar esta clase con la clase que corresponde
    /// dentro de ActorRestApiGW y dejar de usar JSON.NET de forma directa
    /// </summary>
    public class RESJson
    {
        public string JsonString { get; set; }
    }
}
