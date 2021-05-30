using ChatClientProvider.Protos;
using ChatClientWPF.Commands;
using ChatClientWPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatClientWPF.ViewModels
{
    class ChatVM : BasePropertyChanged
    {
        private ChatLogic chatLogic;

        public ChatVM()
        {
            chatLogic = new(this);
            ChatMessages = new();
        }

        private ObservableCollection<ChatMessage> chatMessages;
        public ObservableCollection<ChatMessage> ChatMessages
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

        public string CurrentMessage { get; set; }

        private ICommand sendMessageCommand;
        public ICommand SendMessageCommand
        {
            get
            {
                if (sendMessageCommand == null)
                {
                    sendMessageCommand = new RelayCommand(chatLogic.Send);
                }
                return sendMessageCommand;
            }
        }

        private ICommand logInCommand;
        public ICommand LogInCommand
        {
            get
            {
                if(logInCommand == null)
                {
                    //logInCommand = new RelayCommandGeneric<string>(async param => await chatLogic.UserLogIn(param));
                    logInCommand = new RelayCommand(async param => await chatLogic.UserLogIn(param));
                }
                return logInCommand;
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
                    logOutCommand = new RelayCommand(async param => await chatLogic.UserLogOut(param));
                }
                return logOutCommand;
            }
        }
    }
}