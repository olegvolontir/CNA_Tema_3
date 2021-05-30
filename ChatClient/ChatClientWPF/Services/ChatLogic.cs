using ChatClientProvider.Protos;
using ChatClientProvider.Services;
using ChatClientWPF.ViewModels;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
           
            //var task = Task.Run(async () =>
            //{
            //    await _client.LogInAsync(CurrentUser);
            //    await _client.SendMessage().RequestStream.WriteAsync(new ChatMessage() { Sender = CurrentUser, Content = "", DateTimeStamp = DateTime.UtcNow.ToTimestamp() });
            //});
        }

        public async Task UserLogIn(object name)
        {
            User user = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Name = name as string
            };

            await _client.LogInAsync(user);
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

            //await _client.SendMessage().RequestStream.WriteAsync(message);

        }
    }
}
