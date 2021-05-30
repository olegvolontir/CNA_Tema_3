using ChatClientProvider.Protos;
using ChatClientProvider.Services;
using ChatClientWPF.ViewModels;
using ChatClientWPF.Views;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatClientWPF.Services
{
    class ChatLogic
    {
        private static Chat.ChatClient _client;
        public static User CurrentUser { get; set; }
        private bool _newMessage = false;
        private ChatVM _chatVM;

        public ChatLogic(ChatVM chatVM)
        {
            _chatVM = chatVM;
            _client = new GrpcChatServiceProvider().GetChatClient();
        }

        public async Task UserLogIn(object obj)
        {
            var _params = (object[])obj;

            if (string.IsNullOrEmpty(_params[0] as string)) return;

            CurrentUser = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Name = _params[0] as string
            };

            await _client.LogInAsync(CurrentUser);

            (_params[1] as Window).Hide();
            var chatWindow = new ChatWindow();
            chatWindow.ShowDialog();
        }

        public async Task UserLogOut(object obj)
        {
            await _client.LogOutAsync(CurrentUser);
            (obj as Window).Close();
        }

        public void Send(object obj)
        {
            _chatVM.CurrentMessage = obj as string;
            _newMessage = true;
        }

        public static void SendMessage(object obj)
        {
            string content = obj as string;

            ChatMessage message = new ChatMessage
            {
                Sender = CurrentUser,
                Content = content,
                DateTimeStamp = DateTime.UtcNow.ToTimestamp()
            };

            var task = Task.Run(async () => 
            {
                await _client.SendMessage().RequestStream.WriteAsync(message); 
            });
        }
    }
}
