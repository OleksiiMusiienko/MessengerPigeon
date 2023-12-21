using MessengerModel;
using MessengerPigeon.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MessengerPigeon
{
    class MessengerViewModel : INotifyPropertyChanged
    {
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
                User.Password  = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        
        public byte[]? Avatar
        {
            get { return User.Avatar; }
            set
            {
                User.Avatar   = value;
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
                Message.Mes  = value;
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
                return CommandSend;
            }
        }
        private void Reg(object o)
        {
            //логика регистрации(сделать метод асинхронным) или вызов отдельного async метода
        }
        private bool CanReg(object o)
        {
            //логика проверки доступности регистрации пользователя (или доступна всегда)
            return true;
        }
        //реализация команды регистрации конец
    }
}
