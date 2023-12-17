
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerModel
{
    class Message
    {
        public string Mes {  get; set; }
        public DateTime Date_Time {  get; set; } 
        public Message(string mes, DateTime date_Time)
        {
            Mes = mes;
            Date_Time = DateTime.Now;
        }
    }
}
