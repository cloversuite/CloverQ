﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages
{
    public class MessageQMemberUnpause: Message
    {
        public string MemberId { get; set; }
        public string RequestId { get; set; }
    }
}
