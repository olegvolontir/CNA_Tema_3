using System;
using Grpc.Net.Client;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ChatService.Protos;
using Google.Protobuf.WellKnownTypes;
using System.Threading;

namespace ChatClientConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Chat.ChatClient(channel);

            var rand = new Random();

            User user = new User() { Name = "User" + rand.Next(1, 10), ID = Guid.NewGuid().ToString() };

            //var response = await client.SendMessageAsync(new SendMessageRequest()
            //{
            //    ChatMessage = new ChatMessage()
            //    {
            //        Sender = user,
            //        Content = "Content1",
            //        DateTimeStamp = DateTime.UtcNow.ToTimestamp()
            //    }
            //});

            var loginResponse = await client.LogInAsync(user);


            using (var chat = client.SendMessage())
            {
                await chat.RequestStream.WriteAsync(new ChatMessage() 
                { 
                    Sender = user, 
                    Content = "", 
                    DateTimeStamp = DateTime.UtcNow.ToTimestamp() 
                });

                var inputStream = Task.Run(async () =>
                {
                    while (await chat.ResponseStream.MoveNext(cancellationToken: CancellationToken.None))
                    {
                        Console.WriteLine(chat.ResponseStream.Current.Sender.Name + ": " + chat.ResponseStream.Current.Content);
                    }
                });

                string line;
                while ((line = Console.ReadLine()) != null)
                {
                    if (line.ToLower() == "bye")
                    {
                        break;
                    }

                    await chat.RequestStream.WriteAsync(new ChatMessage()
                    {
                        Sender = user,
                        Content = line,
                        DateTimeStamp = DateTime.UtcNow.ToTimestamp()
                    });

                }

                await chat.RequestStream.CompleteAsync();
            }

            await Task.Delay(2000);

            var logoutResponse = await client.LogOutAsync(user);

            ////var response2 = client.SendMessage(new SendMessageRequest()
            ////{
            ////    ChatMessage = new ChatMessage()
            ////    {
            ////        Sender = user,
            ////        Content = "Content2",
            ////        DateTimeStamp = DateTime.UtcNow.ToTimestamp()
            ////    }
            ////});

            //await Task.Delay(3000);

            //var messages = client.GetAllMessages(new Empty());

            //Console.WriteLine("Done");

            //var logout = await client.LogOutAsync(user);
        }
    }
}
