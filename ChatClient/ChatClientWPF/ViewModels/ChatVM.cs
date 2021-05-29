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
            chatLogic = new();
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
    }
}