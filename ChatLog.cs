using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demo2
{
    public class ChatLog
    {
        public string Sender { get; set; } // "User" or "Bot"
        public string Message { get; set; }
        public string Timestamp => DateTime.Now.ToString("HH:mm:ss");

        public override string ToString()
        {
            return $"[{Timestamp}] {Sender}: {Message}";
        }
    }
}