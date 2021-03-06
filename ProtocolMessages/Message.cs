﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages
{
    public class Message
    {
        public Message()
        {
            EventTime = DateTime.Now;
        }

        public DateTime EventTime { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string CallHandlerId { get; set; }
        public string QueueId { get; set; }
    }
}
