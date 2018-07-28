using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages.Commands
{
    public class CMDMessage
    {
        public CMDMessage()
        {
            CmdDate = DateTime.Now;
        }
        public DateTime CmdDate { get; protected set; }
        public string CmdName { get; protected set; }
    }
}
