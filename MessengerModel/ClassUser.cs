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
        public string IPadress {  get; set; }      
        public byte[]? Avatar { get; set; }
        public User(string nick, string password, string ipadress, byte[] avatar)
        {
            Nick = nick;
            Password = password;
            IPadress = ipadress;
            Avatar = avatar;
        }
        public virtual ICollection<Message> Messages { get; set; }
        
        public override string ToString()
        {
            return Nick;
        }
    }
}//public string Mail {  get; set; }// обсудить возможность подтверждения аккаунта по почте
