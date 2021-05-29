using ChatClientProvider.Protos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClientWPF.ViewModels
{
    class ChatVM : BasePropertyChanged
    {
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
    }
}
