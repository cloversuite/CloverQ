using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages.QueueLog
{
    public class QLMessage
    {
        public QLMessage()
        {
            EventDate = DateTime.Now;
        }
        public DateTime EventDate { get; protected set; }
        public string EventName { get; protected set; }
        public string Channel { get; set; }
        public string QueueId { get; set; }
        public string MemberId { get; set; }
        public string data1 { get; protected set; }
        public string data2 { get; protected set; }
        public string data3 { get; protected set; }
        public string data4 { get; protected set; }
        public override string ToString()
        {
            return $"{EventDate.ToString("s")}|{Channel}|{QueueId}|{EventName}|{data1}|{data2}|{data3}|{data4}";
        }
    }
}
