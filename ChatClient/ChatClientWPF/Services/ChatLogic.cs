using ChatClientProvider.Protos;
using ChatClientProvider.Services;
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

        public ChatLogic()
        {
            _client = new GrpcChatServiceProvider().GetChatClient();
            Random r = new();
            CurrentUser = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Name = "User" + r.Next(1, 100)
            };
            var task = Task.Run(async () =>
            {
                await _client.LogInAsync(CurrentUser);
                await _client.SendMessage().RequestStream.WriteAsync(new ChatMessage() { Sender = CurrentUser, Content = "", DateTimeStamp = DateTime.UtcNow.ToTimestamp() });
            });
        }

        public void UserLogIn(object obj)
        {

        }

        public void Send(object obj)
        {
            SendMessage(obj);
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
