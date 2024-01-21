using Azure.Core;
using CommandDLL;
using MessengerModel;
using MessengerPigeon.Command;
using Microsoft.VisualBasic;
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
using System.Windows.Documents;
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
        public TcpClient tcpClientMessage;
        public NetworkStream netstreamMessage;
        public MessengerViewModel()
        {
            User = new User();
            Message = new Message();
        }
        private User User;
        private User myUser;
        private Message Message;

        public User MyUser
        {
            get { return myUser; }
            set
            {
                myUser = value;
                OnPropertyChanged(nameof(myUser));
            }
        }
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
        private string _nickReg = string.Empty;
        public string NickReg
        {
            get { return _nickReg; }
            set
            {
                _nickReg = value;
                OnPropertyChanged(nameof(NickReg));
            }
        }
        private string _passwordReg = string.Empty;
        public string PasswordReg
        {
            get { return _passwordReg; }
            set
            {
                _passwordReg = value;
                OnPropertyChanged(nameof(PasswordReg));
            }
        }
        private string _passwordTwo = string.Empty;
        public string PasswordTwo
        {
            get { return _passwordTwo; }
            set
            {
                _passwordTwo = value;
                OnPropertyChanged(nameof(PasswordTwo));
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
        private User _userRecepient;
        public User UserRecepient
        {
            get { return _userRecepient; }
            set
            {
                _userRecepient= value;
                OnPropertyChanged(nameof(UserRecepient));
                HistoryMessages();
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

        private async void ConnectionForMessage()
        {
            await Task.Run(async () =>
            {
            try
            {
                 string IP = "26.27.154.150";
                 tcpClientMessage = new TcpClient(IP, 49153);
                 netstreamMessage = tcpClientMessage.GetStream();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Клиент: " + ex.Message);
            }
            });
        }
    private async void Send(object o)
        {
            await Task.Run(async () =>
            {
                try
                {
                    MemoryStream stream = new MemoryStream();
                   
                    Date_Time = DateTime.Now;
                    Message mes = new Message(Mes, Date_Time);
                    mes.UserSenderId = myUser.Id;
                    mes.UserRecepientId = UserRecepient.Id;
                    var jsonFormatter = new DataContractJsonSerializer(typeof(Message));
                    jsonFormatter.WriteObject(stream, mes);
                    byte[] msg = stream.ToArray();
                    await netstreamMessage.WriteAsync(msg, 0, msg.Length); // записываем данные в NetworkStream.
                    Mes = "";
                    ReceiveMessage(tcpClientMessage);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент: " + ex.Message);
                }
            });
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
                    wrapper.commands = Wrapper.Commands.Registratioin;
                    User us = new User(NickReg, PasswordReg,null,null);
                    wrapper.user = us;
                    var jsonFormatter = new DataContractJsonSerializer(typeof(Wrapper));
                    jsonFormatter.WriteObject(stream, wrapper);
                    byte[] msg = stream.ToArray();
                    await netstream.WriteAsync(msg, 0, msg.Length); // записываем данные в NetworkStream.
                    myUser = new User();
                    Receive(tcpClient);
                    ConnectionForMessage();// отдельное подключение для сообщений
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент: " + ex.Message);
                }
            });
        }

        private bool CanReg(object o)
        {
            //string PASSWORD_PATTERN ="((?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%]).{6,20})";
            //if (NickReg == null)
            //{
            //    MessageBox.Show("Вы не ввели имя для регистрации! ");
            //    return false;
            //}
            //else if (PasswordReg!=PASSWORD_PATTERN && PasswordReg == null)
            //{
            //    MessageBox.Show("Пароль не соответствует требованиям! ");
            //    return false;
            //}
            //else if(PasswordReg != PasswordTwo)
            //{
            //    MessageBox.Show("Пароли не совпадают! ");
            //    return false;
            //}
            return true;
        }
        //реализация команды регистрации конец

        //28.12.24 реализация команды авторизации пользователя начало
        private CommandAuthorization CommandAut;
        public ICommand ButtonAut
        {
            get
            {
                if (CommandAut == null)
                {
                    CommandAut = new CommandAuthorization(Aut, CanAut);
                }
                return CommandAut;
            }
        }
        private async void Aut(object o)
        {
            await Task.Run(async () =>
            {
                try
                {
                    User = new User();
                    string IP = "26.27.154.150";
                    tcpClient = new TcpClient(IP, 49152);
                    netstream = tcpClient.GetStream();
                    MemoryStream stream = new MemoryStream();
                    Wrapper wrapper = new Wrapper();
                    wrapper.commands = Wrapper.Commands.Authorization;
                    User us = new User(NickReg, PasswordReg, null, null);
                    wrapper.user = us;
                    var jsonFormatter = new DataContractJsonSerializer(typeof(Wrapper));
                    jsonFormatter.WriteObject(stream, wrapper);
                    byte[] msg = stream.ToArray();
                    await netstream.WriteAsync(msg, 0, msg.Length); // записываем данные в NetworkStream.
                    myUser = new User();

                    Receive(tcpClient);
                    ConnectionForMessage();// отдельное подключение для сообщений
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент: " + ex.Message);
                }
            });
        }

        private bool CanAut(object o)
        {
            if (NickReg == null && PasswordReg != PasswordTwo)
                return false;
            return true;
        }
        //реализация команды регистрации конец
        //31.12.23 реализация команды редактирования пользователя начало


        private CommandRegistration CommandRedact;
        public ICommand ButtonRedact
        {
            get
            {
                if (CommandRedact == null)
                {
                    CommandRedact = new CommandRegistration(Redact, CanRedact);
                }
                return CommandRedact;
            }
        }
        private async void Redact(object o)
        {
          if(PasswordReg!=User.Password || PasswordTwo=="")
            {
                MessageBox.Show("Не верный пароль пользователя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            await Task.Run(async () =>
            {
                try
                {
                    MemoryStream stream = new MemoryStream();
                    Wrapper wrapper = new Wrapper();
                    wrapper.commands = Wrapper.Commands.Redact;
                    User us = new User(Nick, Password,null, Avatar);
                    wrapper.NewPassword = PasswordTwo; 
                    wrapper.user = us;
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

        private bool CanRedact(object o)
        {
            if (User.IPadress == "")
                return false;
            return true;
        }
        //реализация команды регистрации конец

        //04.01.2024 реализация команды удаление пользователя начало
        private CommandRemove CommandRemov;
        public ICommand ButtonRemove
        {
            get
            {
                if (CommandRemov == null)
                {
                    CommandRemov = new CommandRemove(Remove, CanRemove);
                }
                return CommandRemov;
            }
        }
        private async void Remove(object o)
        {
            await Task.Run(async () =>
            {
                try
                {
                    MemoryStream stream = new MemoryStream();
                    Wrapper wrapper = new Wrapper();
                    wrapper.commands = Wrapper.Commands.Remove;
                    wrapper.user = User;
                    var jsonFormatter = new DataContractJsonSerializer(typeof(Wrapper));
                    jsonFormatter.WriteObject(stream, wrapper);
                    byte[] msg = stream.ToArray();
                    await netstream.WriteAsync(msg, 0, msg.Length); // записываем данные в NetworkStream.

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент: " + ex.Message);
                }
            });
        }
        private bool CanRemove(object o)
        {
            if (User.Nick == "")
                return false;
            return true;
        }
        //реализация команды отправки конец

        // метод прослушки ответов запросов на регистрацию от сервера 
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
                        var jsonFormatter = new DataContractJsonSerializer(typeof(ServerResponse));
                        ServerResponse res = jsonFormatter.ReadObject(stream) as ServerResponse;
                        if (res.list != null)
                        {
                            List<User> list = new List<User>();
                            list = res.list;
                            IPAddress address = Dns.GetHostAddresses(Dns.GetHostName()).First<IPAddress>(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                            foreach (var user in list)
                            {
                                if (user.IPadress == address.ToString())
                                {
                                    MyUser = user;
                                    list.Remove(user);
                                    break;
                                }
                            }
                            Users = new ObservableCollection<User>(list);                            
                            Nick = NickReg;
                            Password = PasswordReg;
                            Avatar = myUser.Avatar;
                            NickReg = "";
                            PasswordReg = "";
                            PasswordTwo = "";
                        }
                        else if (res.command == "Пользователь успешно удален!")
                        {
                            MessageBox.Show(res.command);
                            Users = null;
                            Nick = "";
                            return;
                        }
                        else
                        {
                            MessageBox.Show(res.command);
                            NickReg = "";
                            PasswordReg = "";
                            PasswordTwo = "";
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
                }
            });
        }
        private async void ReceiveMessage(TcpClient tcpClientMessage)
        {
            await Task.Run(async () =>
            {
                try
                {
                    // Получим объект NetworkStream, используемый для приема и передачи данных.
                    netstreamMessage = tcpClientMessage.GetStream();
                    byte[] arr = new byte[tcpClientMessage.ReceiveBufferSize];
                    while (true)
                    {
                        int len = await netstreamMessage.ReadAsync(arr, 0, tcpClient.ReceiveBufferSize);
                        if (len == 0)
                        {
                            netstreamMessage.Close();
                            tcpClientMessage.Close(); // закрываем TCP-подключение и освобождаем все ресурсы, связанные с объектом TcpClient.
                            return;
                        }
                        // Создадим поток, резервным хранилищем которого является память.
                        byte[] copy = new byte[len];
                        Array.Copy(arr, 0, copy, 0, len);
                        MemoryStream stream = new MemoryStream(copy);
                        var jsonFormatter = new DataContractJsonSerializer(typeof(List<Message>));
                        List<Message> res = jsonFormatter.ReadObject(stream) as List<Message>;

                        Messages = new ObservableCollection<Message>(res);

                        stream.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент: " + ex.Message);
                    netstreamMessage?.Close();
                    tcpClientMessage?.Close(); // закрываем TCP-подключение и освобождаем все ресурсы, связанные с объектом TcpClient.
                }
                finally
                {
                    netstreamMessage?.Close();
                    tcpClientMessage?.Close();
                }
            });
        }

        // реализация команды запроса истории сообщений 
        private async void HistoryMessages()
        {
            await Task.Run(async () =>
            {
                try
                {
                    MemoryStream stream = new MemoryStream();
                    Date_Time = DateTime.Now;
                    Message mes = new Message("", Date_Time);
                    mes.UserSenderId = myUser.Id;
                    mes.UserRecepientId = UserRecepient.Id;
                    var jsonFormatter = new DataContractJsonSerializer(typeof(Message));
                    jsonFormatter.WriteObject(stream, mes);
                    byte[] msg = stream.ToArray();
                    await netstreamMessage.WriteAsync(msg, 0, msg.Length); // записываем данные в NetworkStream.
           
                    ReceiveMessage(tcpClientMessage);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент: " + ex.Message);
                }
            });
        }

    }
}
