using MessengerModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
