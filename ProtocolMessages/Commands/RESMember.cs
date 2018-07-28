using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages.Commands
{
    public class RESMember
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Contact { get; set; }
        public bool IsLogedIn { get; set; }
        public bool IsPaused { get; set; }
        public string PauseCode { get; set; }
        public string PauseReason { get; set; }
        public bool IsAvailable { get; set; }
        public string DeviceId { get; set; }
        public bool DeviceIsInUse { get; set; }
        public bool EndpointIsOfline { get; set; }
    }
}
