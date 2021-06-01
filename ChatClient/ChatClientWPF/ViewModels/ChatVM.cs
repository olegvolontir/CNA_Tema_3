using ChatClientProvider.Protos;
using ChatClientWPF.Commands;
using ChatClientWPF.Models;
using ChatClientWPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ChatClientWPF.ViewModels
{
    class ChatVM : BasePropertyChanged
    {
        private ChatLogic _chatLogic;
        public object _chatMessagesLock = new();
        public object _userListLock = new();

        public ChatVM()
        {
            currentMessage = "";
            ChatMessages = new();
            Users = new();
            BindingOperations.EnableCollectionSynchronization(ChatMessages, _chatMessagesLock);
            BindingOperations.EnableCollectionSynchronization(Users, _userListLock);
            _chatLogic = new(this);
        }


        private ObservableCollection<User> users;
        public ObservableCollection<User> Users
        {
            get
            {
                return users;
            }
            set
            {
                users = value;
                NotifyPropertyChanged(nameof(Users));
            }
        }

        private ObservableCollection<DisplayedMessage> chatMessages;
        public ObservableCollection<DisplayedMessage> ChatMessages
        {
            get
            {
                return chatMessages;
            }
            set
            {
                chatMessages = value;
                NotifyPropertyChanged(nameof(ChatMessages));
            }
        }

        private string currentMessage;
        public string CurrentMessage {
            get
            {
                return currentMessage;
            }
            set
            {
                currentMessage = value;
                _chatLogic._newMessage = true;
            } 
        }

        private ICommand sendMessageCommand;
        public ICommand SendMessageCommand
        {
            get
            {
                if (sendMessageCommand == null)
                {
                    sendMessageCommand = new RelayCommand(_chatLogic.SendMessage);
                }
                return sendMessageCommand;
            }
        }

        private ICommand logOutCommand;
        public ICommand LogOutCommand
        {
            get
            {
                if (logOutCommand == null)
                {
                    //logOutCommand = new RelayCommandGeneric<string>(async param => await chatLogic.UserLogIn(param));
                    logOutCommand = new RelayCommand(async param => await _chatLogic.UserLogOut(param));
                }
                return logOutCommand;
            }
        }
    }
}