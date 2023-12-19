using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MessengerModel
{
    class User
    {
        public int Id { get; set; }
        public string Nick {  get; set; }
        public string Password { get; set; }
        public string Phone {  get; set; }      
        public byte[]? Avatar { get; set; }
        public User(string nick, string password, string phone, byte[] avatar)
        {
            Nick = nick;
            Password = password;
            Phone = phone;
            Avatar = avatar;
        }
        public virtual ICollection<Message> Messages { get; set; }
        //public string Mail {  get; set; }// обсудить возможность подтверждения аккаунта по почте
    }
}
