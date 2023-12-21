using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace MessengerModel
{

    [DataContract]
    class User
    {
        public int Id { get; set; }
        [DataMember]
        public string Nick {  get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string IPadress {  get; set; }
        [DataMember]
        public byte[]? Avatar { get; set; }
        public User() { }
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
