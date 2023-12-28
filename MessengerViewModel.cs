using CommandDLL;
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
using System.Runtime.Intrinsics.X86;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace MessengerPigeon
{
    [DataContract]
    class MessengerViewModel : INotifyPropertyChanged
    {
        public TcpClient tcpClient;
        public NetworkStream netstream;
        public MessengerViewModel()
        {
            UserReg = new User();
        }
        private User User;
        private User UserReg;
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
        // Свойства для привязки обьекта User к блокам регистрации и авторизации .(NickReg, PasswordReg, PasswordTwo)
        public string NickReg
        {
            get { return UserReg.Nick; }
            set
            {
                UserReg.Nick = value;
                OnPropertyChanged(nameof(NickReg));
            }
        }
        public string PasswordReg
        {
            get { return UserReg.Password; }
            set
            {
                UserReg.Password = value;
                OnPropertyChanged(nameof(PasswordReg));
            }
        }
        public string PasswordTwo
        {
            get { return UserReg.Password; }
            set
            {
                if (Password == value)
                {
                    UserReg.Password = value;
                    OnPropertyChanged(nameof(PasswordTwo));
                }
                else
                {
                    MessageBox.Show("Пароли не совпадают");
                    UserReg.Password = "";
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
                try
                {
                    string IP = "26.27.154.150";
                    tcpClient = new TcpClient(IP, 49152);
                    netstream = tcpClient.GetStream();
                    MemoryStream stream = new MemoryStream();
                    Wrapper wrapper = new Wrapper();
                    wrapper.commands = 0;
                    wrapper.user = UserReg;
                    var jsonFormatter = new DataContractJsonSerializer(typeof(Wrapper));
                    jsonFormatter.WriteObject(stream, wrapper);
                    byte[] msg = stream.ToArray();
                    await netstream.WriteAsync(msg, 0, msg.Length); // записываем данные в NetworkStream.
                    Receive(tcpClient);
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
        private async void Receive(TcpClient tcpClient)
        {
            await Task.Run(async () =>
            {
                try
                {
                    // Получим объект NetworkStream, используемый для приема и передачи данных.
                    netstream = tcpClient.GetStream();
                    byte[] arr = new byte[tcpClient.ReceiveBufferSize];
                    while (true)
                    {
                        int len = await netstream.ReadAsync(arr, 0, tcpClient.ReceiveBufferSize);
                        if (len == 0)
                        {
                            netstream.Close();
                            tcpClient.Close(); // закрываем TCP-подключение и освобождаем все ресурсы, связанные с объектом TcpClient.
                            return;
                        }
                        // Создадим поток, резервным хранилищем которого является память.
                        byte[] copy = new byte[len];
                        Array.Copy(arr, 0, copy, 0, len);
                        MemoryStream stream = new MemoryStream(copy);
                        var jsonFormatter = new DataContractJsonSerializer(typeof(List<User>));
                        var list = (List<User>)jsonFormatter.ReadObject(stream);
                        if (list.Count != 0)
                        {
                            Users = new ObservableCollection<User>(list);
                        }
                        else
                        {
                            MessageBox.Show("Такой пользователь уже зарегистрирован!!");
                            break;
                        }
                        
                        stream.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент: " + ex.Message);
                    netstream?.Close();
                    tcpClient?.Close(); // закрываем TCP-подключение и освобождаем все ресурсы, связанные с объектом TcpClient.
                }
                finally
                {
                    netstream?.Close();
                    tcpClient?.Close();
                    User = new User();
                    User = UserReg;
                    UserReg.Nick = "";
                    UserReg.Password = "";
                }
            });
        }
    }
}
