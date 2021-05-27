using System;
using Grpc.Net.Client;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ChatService.Protos;
using Google.Protobuf.WellKnownTypes;

namespace ChatClientConsole
{
    class Program
    {

        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Chat.ChatClient(channel);
            User user = new User() { Name = "Oleg" };

            var response = await client.SendMessageAsync(new SendMessageRequest()
            {
                ChatMessage = new ChatMessage()
                {
                    Sender = user,
                    Content = "Content1",
                    DateTimeStamp = DateTime.UtcNow.ToTimestamp()
                }
            });

            //var response2 = client.SendMessage(new SendMessageRequest()
            //{
            //    ChatMessage = new ChatMessage()
            //    {
            //        Sender = user,
            //        Content = "Content2",
            //        DateTimeStamp = DateTime.UtcNow.ToTimestamp()
            //    }
            //});

            await Task.Delay(3000);

            var messages = client.GetAllMessages(new Empty());

            Console.WriteLine("Done");

            var logout = await client.LogOutAsync(user);
        }
    }
}
