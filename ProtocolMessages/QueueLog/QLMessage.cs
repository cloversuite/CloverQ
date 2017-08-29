using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMessages.QueueLog
{
    public class QLMessage
    {
        public DateTime EventDate { get; set; }
        public string EventName { get; set; }
        public string Channel { get; set; }
        public string Queue { get; set; }
        public string data1 { get; set; }
        public string data2 { get; set; }
        public string data3 { get; set; }
        public override string ToString()
        {
            return $"{EventDate.ToString("s")}|{Channel}|{Queue}|{EventName}|{data1}|{data2}|{data3}";
        }
    }
}
