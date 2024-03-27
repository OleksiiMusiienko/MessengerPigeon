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
using Microsoft.Toolkit.Uwp.Notifications;
using NAudio;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using NAudio.FileFormats;
using System.Text.RegularExpressions;
using NAudio.Utils;

namespace MessengerPigeon
{
    [DataContract]
    class MessengerViewModel : INotifyPropertyChanged
    {
        public TcpClient tcpClient;
        public NetworkStream netstream;
        public TcpClient tcpClientMessage;
        public NetworkStream netstreamMessage;
        public bool isEditMessage;
        public CaesarCipher cipher;
        public MessengerViewModel()
        {
            User = new User();
            Message = new Message();
            cipher = new CaesarCipher();
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
                OnPropertyChanged(nameof(MyUser));
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
        public string Phone
        {
            get { return User.Phone; }
            set
            {

                User.Phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }
        public bool Online
        {
            get { return User.Online; }
            set
            {
                User.Online = value;
                OnPropertyChanged(nameof(Online));
            }
        }
        // Свойства для привязки обьекта User к блокам регистрации и авторизации .(NickReg, PasswordReg, PasswordTwo)
        private string _nickReg = string.Empty;
        public string NickReg
        {
            get { return _nickReg; }
            set
            {
                if(value == "" && !IsEnableOnline)
                    MessageBox.Show("You didn't enter your login!");
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
                string PASSWORD_PATTERN = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[^a-zA-Z0-9])\S{6,16}$";
                if (!Regex.IsMatch(value, PASSWORD_PATTERN, RegexOptions.IgnoreCase) && !IsEnableOnline)
                {
                    MessageBox.Show("Password doesn't meet requirements!");
                    value = "";
                }
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
                if(value != _passwordReg && !IsEnableOnline)
                {
                    MessageBox.Show("Password and Confirm password do not match!");
                    value = "";
                }
                if (_nickReg == "" && !IsEnableOnline)
                    MessageBox.Show("Enter your login to sign in!");
                if (_phoneReg == "" && !IsEnableOnline)
                    MessageBox.Show("Enter your phone number to sign in!");
                _passwordTwo = value;
                OnPropertyChanged(nameof(PasswordTwo));
            }
        }
        private string _phoneReg = string.Empty;
        public string PhoneReg
        {
            get { return _phoneReg; }
            set
            {
                string pattern = @"^((\+)?\b(8|38)?(0[\d]{2}))([\d-]{5,8})([\d]{2})";

                if (!Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase) && !IsEnableOnline)
                {
                    MessageBox.Show("Incorrect phone number! \n Phone number format: 0123456789");
                    value = "";
                }
                _phoneReg = value;
                OnPropertyChanged(nameof(PhoneReg));
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
        public byte[]? MesAudio
        {
            get { return Message.MesAudio; }
            set
            {
                Message.MesAudio = value;
                OnPropertyChanged(nameof(MesAudio));
            }
        }
        public string? MesAudioUri
        {
            get { return Message.MesAudioUri; }
            set
            {
                Message.MesAudioUri = value;
                OnPropertyChanged(nameof(MesAudioUri));
            }
        }
        private User _userRecepient;
        public User UserRecepient
        {
            get { return _userRecepient; }
            set
            {
                if (value != null)
                    HistoryMessages();
                _userRecepient = value;
                OnPropertyChanged(nameof(UserRecepient));
            }
        }
        private Message _selectedMessage;
        public Message SelectedMessage
        {
            get { return _selectedMessage; }
            set
            {
                _selectedMessage= value;
                OnPropertyChanged(nameof(SelectedMessage));
            }
        }
        
        private bool _isButtonEnable = true;
        private bool _isButtonEnableOnline;
        private bool _isButtonAuthorization = true;
        private bool _isButtonRegistration;
        public bool IsEnable
        {
            get
            {
                return _isButtonEnable;
            }
            set
            {
                _isButtonEnable = value;
                OnPropertyChanged(nameof(IsEnable));
            }
        }
        public bool IsEnableOnline
        {
            get
            {
                return _isButtonEnableOnline;
            }
            set
            {
                if(value == true)
                {
                    ConnectionForMessage();
                }
                _isButtonEnableOnline = value;
                OnPropertyChanged(nameof(IsEnableOnline));
            }
        }
        
        public bool IsEnabledAuthorization
        {
            get
            {
                return _isButtonAuthorization;
            }
            set
            {
                if (MyUser == null && value == true)
                {
                    value = true;
                    if (_isButtonRegistration)
                        IsEnabledRegistration = false;
                }
                else
                    value = false;
                _isButtonAuthorization = value;
                OnPropertyChanged(nameof(IsEnabledAuthorization));
            }
        }
        public bool IsEnabledRegistration
        {
            get
            {
                return _isButtonRegistration;
            }
            set
            {
                if (MyUser == null && value == true)
                {
                    value = true;
                    if (_isButtonAuthorization)
                        IsEnabledAuthorization = false;
                }
                else
                    value = false;
                _isButtonRegistration = value;
                OnPropertyChanged(nameof(IsEnabledRegistration));
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
                _indexUsers = Users.IndexOf(UserRecepient);
                if(value != null)
                {
                    for (int i=0; i< value.Count; i++)
                    {
                        if(i == IndexUsers)
                            UserRecepient = value[i];
                    }
                }
                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }
        private int _indexUsers;
        public int IndexUsers
        {
            get { return _indexUsers; }
            set
            {
                _indexUsers = value;
                OnPropertyChanged(nameof(IndexUsers));
            }
        }
        //------------
        private List<User> tempUsers = new List<User>();
        private string search = string.Empty;
        private bool searchFlag;
        //--------------
        public string _search
        {
            get { return search; }
            set
            {
                search = value;
                OnPropertyChanged(nameof(_search));
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
                if (value != null)
                {
                    IndexMessage = value.Count - 1;
                }
                _messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }
        private int _indexMessage;
        public int IndexMessage
        {
            get { return _indexMessage; }
            set
            {
                _indexMessage = value;
                OnPropertyChanged(nameof(IndexMessage));
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
                 ReceiveMessage(tcpClientMessage);
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
                    Message mes = new Message();

                    if (isEditMessage == true)
                    { 
                        mes.Id = SelectedMessage.Id;
                        mes.UserSenderId = SelectedMessage.UserSenderId;
                        mes.UserRecepientId = SelectedMessage.UserRecepientId;
                        mes.Date_Time = SelectedMessage.Date_Time;
                        mes.Mes = cipher.Encrypt(MyUser.Nick + ":" + "   " + Mes + " (edited message)"); // шифровка сообщения
                        isEditMessage = false;
                    }
                    else
                    {
                        if (MesAudio != null)
                        {
                            mes.Mes = cipher.Encrypt(MyUser.Nick + ":" + "   " + Mes);// шифровка сообщения
                            mes.MesAudio = MesAudio;
                            mes.MesAudioUri = MesAudioUri;
                        }
                        else
                        {
                            mes.Mes = cipher.Encrypt(MyUser.Nick + ":" + "   " + Mes); // шифровка сообщения
                        }                                              
                        mes.Date_Time = DateTime.Now;
                        mes.UserSenderId = myUser.Id;
                        mes.UserRecepientId = UserRecepient.Id;
                    }
                    MemoryStream stream = new MemoryStream();                        
                    var jsonFormatter = new DataContractJsonSerializer(typeof(Message));
                    jsonFormatter.WriteObject(stream, mes);
                    byte[] msg = stream.ToArray();
                    await netstreamMessage.WriteAsync(msg, 0, msg.Length); // записываем данные в NetworkStream.
                    Mes = "";
                    MesAudio = null;
                    MesAudioUri = null;
                    //HistoryMessages();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент: " + ex.Message);
                }
            });
        }
        private bool CanSend(object o)
        {
            //if (Mes == "")
            //    return false;
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
                    User us = new User(NickReg, PasswordReg,null,null, PhoneReg);
                    us.Online = true;
                    wrapper.user = us;
                    var jsonFormatter = new DataContractJsonSerializer(typeof(Wrapper));
                    jsonFormatter.WriteObject(stream, wrapper);
                    byte[] msg = stream.ToArray();
                    await netstream.WriteAsync(msg, 0, msg.Length); // записываем данные в NetworkStream.
                    myUser = new User();
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
            if (NickReg == null)
            {
                MessageBox.Show("Вы не ввели имя для регистрации! ");
                return false;
            }
            else if (PasswordReg != PasswordTwo)
            {
                MessageBox.Show("Пароли не совпадают! ");
                return false;
            }
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
                    User us = new User(NickReg, PasswordReg, null, null, PhoneReg);
                    us.Online = true;
                    wrapper.user = us;
                    var jsonFormatter = new DataContractJsonSerializer(typeof(Wrapper));
                    jsonFormatter.WriteObject(stream, wrapper);
                    byte[] msg = stream.ToArray();
                    await netstream.WriteAsync(msg, 0, msg.Length); // записываем данные в NetworkStream.
                    myUser = new User();
                    Receive(tcpClient);
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
            if (PasswordReg != MyUser.Password )
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
                    if(Nick == "")
                        Nick = MyUser.Nick;
                    if(PasswordTwo == "")
                        PasswordTwo = MyUser.Password;
                    User us = new User(Nick, PasswordReg, null, Avatar, MyUser.Phone);
                    us.Online = true;
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
                    wrapper.user = MyUser;
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
            return true;
        }
        //реализация команды удаления конец

        //24.01.2024 реализация команды выхода пользователя начало
        private CommandExit CommandEx;
        public ICommand ButtonEx
        {
            get
            {
                if (CommandEx == null)
                {
                    CommandEx = new CommandExit(Exit, CanExit);
                }
                return CommandEx;
            }
        }
        private async void Exit(object o)
        {
            await Task.Run(async () =>
            {
                try
                {
                    MemoryStream stream = new MemoryStream();
                    Wrapper wrapper = new Wrapper();
                    wrapper.commands = Wrapper.Commands.ExitOnline;
                    MyUser.Online = false;
                    wrapper.user = MyUser;
                    var jsonFormatter = new DataContractJsonSerializer(typeof(Wrapper));
                    jsonFormatter.WriteObject(stream, wrapper);
                    byte[] msg = stream.ToArray();
                    await netstream.WriteAsync(msg, 0, msg.Length); // записываем данные в NetworkStream. MemoryStream stream1 = new MemoryStream();

                    MemoryStream stream1 = new MemoryStream();
                    Message mes1 = new Message();
                    mes1.Mes = "ExitOnline";
                    mes1.Date_Time = DateTime.Now;
                    var jsonFormatter1 = new DataContractJsonSerializer(typeof(Message));
                    jsonFormatter1.WriteObject(stream1, mes1);
                    byte[] msg1 = stream1.ToArray();
                    await netstreamMessage.WriteAsync(msg1, 0, msg1.Length); // записываем данные в NetworkStream.         
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент: " + ex.Message);
                }
            });
        }
        private bool CanExit(object o)
        {
            return true;
        }
        //реализация команды выхода пользователя конец
        //-------01.02.2024 реализация команды поиска пользователя
        private CommandSearch CommandSearch;
        public ICommand ButtonSearch
        {
            get
            {
                if (CommandSearch == null)
                {
                    CommandSearch = new CommandSearch(Search, CanSearch);
                }
                return CommandSearch;
            }
        }
        private void Search(object o)
        {
            if(_search !="" && !searchFlag)
            {
                foreach (User user in _users)
                {
                    if (user.Nick.Contains(_search))
                    {
                        SearchUser();
                        return;
                    }                    
                }
                MessageBox.Show("Пользователь не найден!", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                _search = "";
            }
            else if (searchFlag)
            {
                if(_search!="")
                {
                    _search = "";
                }
                _users.Clear();
                Users = new ObservableCollection<User>(tempUsers);
                tempUsers.Clear();
                searchFlag = false;
            }
        }
        private void SearchUser()
        {
            searchFlag = true;
            tempUsers = _users.ToList();
            _users = new ObservableCollection<User>();
                     
            foreach (User user in tempUsers)
            {
                if (user.Nick.Contains(_search))
                    _users.Add(user);
            }
            Users = _users;
            _search = "";
        }
        private bool CanSearch(object o)
        {
            return true;
        }
        //---------реализация команды поиска пользователя конец
        // метод прослушки ответов запросов на регистрацию от сервера 
        private async void Receive(TcpClient tcpClient)
        {
            await Task.Run(async () =>
            {
                try
                {
                    // Получим объект NetworkStream, используемый для приема и передачи данных.
                    netstream = tcpClient.GetStream();
                    byte[] arr = new byte[100000000];
                    while (true)
                    {
                        int len = await netstream.ReadAsync(arr, 0, arr.Length);
                        if (len == 0)
                        {
                            MyUser.Avatar = null;
                            MyUser = null;
                            Users = null;
                            Messages = null;
                            IsEnableOnline = false;
                            IsEnable = true;
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
                            foreach (var user in list)
                            {
                                if(user.Nick == NickReg && user.Password == PasswordReg ||
                                user.Nick == MyUser.Nick && user.Password == MyUser.Password)
                                {
                                    MyUser = user;
                                    IsEnableOnline = true;
                                    IsEnable = false;
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
                            PhoneReg = "";
                        }
                        else if (res.command == "Пользователь успешно удален!")
                        {
                            MessageBox.Show(res.command);
                            MyUser.Avatar = null;
                            MyUser = null;
                            Users = null;
                            UserRecepient = null;
                            IsEnableOnline = false;
                            IsEnable = true;
                            return;
                        }
                        else
                        {
                            MessageBox.Show(res.command);
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
                //finally
                //{
                //    netstream?.Close();
                //    tcpClient?.Close();
                //}
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
                    byte[] arr = new byte[100000000];
                    byte[] arrbuf = null;
                    int length = 0;
                    int buf_len = 0;
                    List<Message> res = null;
                    while (true)
                    {
                        int len = await netstreamMessage.ReadAsync(arr, 0, arr.Length);
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
                        if (len < 3)
                        {
                            Messages = null;
                        }
                        else if (copy.Length > 2 && copy.Length < 100)
                        {
                            var jsonFormatter1 = new DataContractJsonSerializer(typeof(object));
                            var obj = jsonFormatter1.ReadObject(stream) as Object;
                            if (obj == null)
                            {
                                netstreamMessage?.Close();
                                return;
                            }
                            else
                            {
                                length = (int)obj;
                                RepeatHistoryMessages();
                                arrbuf = new byte[length];
                                buf_len = 0;
                            }
                            
                        }
                        else if (len <= length)
                        {
                            Array.Copy(copy, 0, arrbuf, buf_len, copy.Length);
                            length -= len;
                            buf_len += len;
                            if (length == 0)
                            {
                                MemoryStream streambuf = new MemoryStream(arrbuf);
                                var jsonFormatter = new DataContractJsonSerializer(typeof(List<Message>));
                                res = jsonFormatter.ReadObject(streambuf) as List<Message>;
                                if (res.Count != 0)
                                {
                                    if (res[res.Count - 1].UserSenderId != myUser.Id)
                                    {
                                        new ToastContentBuilder().AddText(res[res.Count - 1].Mes)
                                        .AddText(res[res.Count - 1].Date_Time.ToString())
                                        .SetToastDuration(ToastDuration.Short)
                                        .SetToastScenario(ToastScenario.Default)
                                        .Show();
                                    }
                                    foreach (Message mes in res)
                                    {
                                        if (mes.MesAudio != null && mes.MesAudioUri != null)
                                        {
                                            await File.WriteAllBytesAsync(mes.MesAudioUri, mes.MesAudio);
                                        }
                                        mes.Mes = cipher.Decrypt(mes.Mes);
                                    }

                                    Messages = new ObservableCollection<Message>(res);
                                }
                                else
                                {
                                    Messages = null;
                                }
                                streambuf.Close();
                            }
                            stream.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент: " + ex.Message);
                    netstreamMessage?.Close();
                    tcpClientMessage?.Close(); // закрываем TCP-подключение и освобождаем все ресурсы, связанные с объектом TcpClient.
                }                
            });
        }
        private async void RepeatHistoryMessages()
        {
            await Task.Run(async () =>
            {
                try
                {
                    MemoryStream stream = new MemoryStream();
                    Date_Time = DateTime.Now;
                    Message mes = new Message("Repeat", Date_Time);
                    mes.UserSenderId = myUser.Id;
                    mes.UserRecepientId = UserRecepient.Id;
                    var jsonFormatter = new DataContractJsonSerializer(typeof(Message));
                    jsonFormatter.WriteObject(stream, mes);
                    byte[] msg = stream.ToArray();
                    await netstreamMessage.WriteAsync(msg, 0, msg.Length); // записываем данные в NetworkStream.
                    stream.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент: " + ex.Message);
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
                    stream.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент: " + ex.Message);
                }
            });
        }
        //реализация команды удаление сообщения начало
        private CommandRemoveMessage CommandRemoveMessage;
        public ICommand ButtonRemoveMessage
        {
            get
            {
                if (CommandRemoveMessage == null)
                {
                    CommandRemoveMessage = new CommandRemoveMessage(RemoveMessage, CanRemoveMessage);
                }
                return CommandRemoveMessage;
            }
        }
        private async void RemoveMessage(object o)
        {
            await Task.Run(async () =>
            {
                try
                {
                    if(MessageBox.Show("Are you sure you want to delete this message?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        MemoryStream stream = new MemoryStream();
                        Message mes = SelectedMessage;
                        mes.Mes = "CommandRemoveMessage";
                        var jsonFormatter = new DataContractJsonSerializer(typeof(Message));
                        jsonFormatter.WriteObject(stream, mes);
                        byte[] msg = stream.ToArray();
                        await netstreamMessage.WriteAsync(msg, 0, msg.Length);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент: " + ex.Message);
                }
            });
        }
        private bool CanRemoveMessage(object o)
        {
            return true;
        }
        //реализация команды удаления сообщения конец

        //реализация команды удаление истории сообщений начало
        private CommandRemoveAllMessages CommandRemoveAllMessages;
        public ICommand ButtonRemoveAllMessages
        {
            get
            {
                if (CommandRemoveAllMessages == null)
                {
                    CommandRemoveAllMessages = new CommandRemoveAllMessages(RemoveAllMessages, CanRemoveAllMessages);
                }
                return CommandRemoveAllMessages;
            }
        }
        private async void RemoveAllMessages(object o)
        {
            await Task.Run(async () =>
            {
                try
                {
                    if (MessageBox.Show("Are you sure you want to delete message history?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        MemoryStream stream = new MemoryStream();
                        Message mes = SelectedMessage;
                        mes.Mes = "CommandRemoveAllMessages";
                        var jsonFormatter = new DataContractJsonSerializer(typeof(Message));
                        jsonFormatter.WriteObject(stream, mes);
                        byte[] msg = stream.ToArray();
                        await netstreamMessage.WriteAsync(msg, 0, msg.Length);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент: " + ex.Message);
                }
            });
        }
        private bool CanRemoveAllMessages(object o)
        {
            return true;
        }
        //реализация команды удаления истории сообщений конец

        //реализация команды редактирования сообщения начало
        private CommandEditMessage CommandEditMessage;
        public ICommand ButtonEditMessage
        {
            get
            {
                if (CommandEditMessage == null)
                {
                    CommandEditMessage = new CommandEditMessage(EditMessage, CanEditMessage);
                }
                return CommandEditMessage;
            }
        }
        private async void EditMessage(object o)
        {
            await Task.Run(async () =>
            {
                try
                {
                    Message mes = SelectedMessage;
                    Mes = mes.Mes.Substring(mes.Mes.IndexOf(" ")).Trim();
                    isEditMessage = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент: " + ex.Message);
                }
            });
        }
        private bool CanEditMessage(object o)
        {
            return true;
        }
        //реализация команды редактирования сообщения конец

              
        }
    }
