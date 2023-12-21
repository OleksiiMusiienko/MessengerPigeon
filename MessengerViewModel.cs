using MessengerModel;
using MessengerPigeon.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;

namespace MessengerPigeon
{
    [DataContract]
    class MessengerViewModel : INotifyPropertyChanged
    {
        public MessengerViewModel()
        {
            User = new User();
        }
        private User User;
        private Message Message;

        public string Nick
        {
            get { return User.Nick; }
            set
            {
                User.Nick = value;
                OnPropertyChanged(nameof(Nick));
            }
        }

        public string Password
        {
            get { return User.Password; }
            set
            {
                User.Password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public string PasswordTwo
        {
            get { return User.Password; }
            set
            {
                if (Password == value)
                {
                    User.Password = value;
                    OnPropertyChanged(nameof(PasswordTwo));
                }
                else
                {
                    MessageBox.Show("Пароли не совпадают");
                    User.Password = "";
                    Password = "";
                }
            }
        }

        public byte[]? Avatar
        {
            get { return User.Avatar; }
            set
            {
                User.Avatar = value;
                OnPropertyChanged(nameof(Avatar));
            }
        }

        public DateTime Date_Time
        {
            get { return Message.Date_Time; }
            set
            {
                Message.Date_Time = value;
                OnPropertyChanged(nameof(Date_Time));
            }
        }
        public string Mes
        {
            get { return Message.Mes; }
            set
            {
                Message.Mes = value;
                OnPropertyChanged(nameof(Mes));
            }
        }

        private ObservableCollection<User> _users = new ObservableCollection<User>();
        public ObservableCollection<User> Users
        {
            get
            {
                return _users;
            }
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        private ObservableCollection<Message> _messages = new ObservableCollection<Message>();
        public ObservableCollection<Message> Messages
        {
            get
            {
                return _messages;
            }
            set
            {
                _messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //21.12.23 реализация команды отправки сообщения начало
        private CommandSendMessage CommandSend;
        public ICommand ButtonSend
        {
            get
            {
                if (CommandSend == null)
                {
                    CommandSend = new CommandSendMessage(Send, CanSend);
                }
                return CommandSend;
            }
        }
        private void Send(object o)
        {
            //логика отправки сообщения(сделать метод асинхронным) или вызов отдельного async метода отправки сообщения
        }
        private bool CanSend(object o)
        {
            if (Mes == "")
                return false;
            return true;
        }
        //реализация команды отправки конец

        //21.12.23 реализация команды регистрации пользователя начало


        private CommandRegistration CommandReg;
        public ICommand ButtonReg
        {
            get
            {
                if (CommandReg == null)
                {
                    CommandReg = new CommandRegistration(Reg, CanReg);
                }
                return CommandReg;
            }
        }
        private async void Reg(object o)
        {
            await Task.Run(async () =>
            {
                // соединяемся с удаленным устройством
                try
                {
                    string IP = "26.238.242.38";
                    // Конструктор TcpClient инициализирует новый экземпляр класса и подключает его к указанному порту заданного узла.
                    TcpClient tcpClient = new TcpClient(IP /* IP-адрес хоста */, 49152 /* порт */);
                    // Получим объект NetworkStream, используемый для приема и передачи данных.
                    NetworkStream netstream = tcpClient.GetStream();
                    MemoryStream stream = new MemoryStream();
                    var jsonFormatter = new DataContractJsonSerializer(typeof(User));
                    jsonFormatter.WriteObject(stream, User);
                    byte[] msg = stream.ToArray();
                    await netstream.WriteAsync(msg, 0, msg.Length); // записываем данные в NetworkStream.
                    MessageBox.Show("Клиент " + Nick + " успешно зарегистрирован !!!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент: " + ex.Message);
                }
            });
        }
        private bool CanReg(object o)
        {
            if (User.Nick == null && User.Password != PasswordTwo)
                return false;
            return true;
        }
        //реализация команды регистрации конец
        
        
    }
}
