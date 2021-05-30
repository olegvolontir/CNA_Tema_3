using ChatClientProvider.Protos;
using ChatClientProvider.Services;
using ChatClientWPF.ViewModels;
using ChatClientWPF.Views;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ChatClientWPF.Services
{
    class ChatLogic
    {
        private static Chat.ChatClient _client;
        public static User CurrentUser { get; set; }
        public bool _newMessage = false;
        public bool _isRunning = true;
        private ChatVM _chatVM;

        public ChatLogic(ChatVM chatVM)
        {
            _client = new GrpcChatServiceProvider().GetChatClient();
            _chatVM = chatVM;
            CurrentUser = Helper.user;
            StartChatting();
        }

        //public async Task UserLogIn(object obj)
        //{
        //    var _params = (object[])obj;

        //    if (string.IsNullOrEmpty(_params[0] as string)) return;


        //    CurrentUser = new User()
        //    {
        //        ID = Guid.NewGuid().ToString(),
        //        Name = _params[0] as string
        //    };

        //    await _client.LogInAsync(CurrentUser);

        //    await Chatting();
        //}

        public async Task UserLogOut(object obj)
        {
            await _client.LogOutAsync(CurrentUser);
            (obj as Window).Close();
        }

        public void SendMessage(object obj)
        {
            _chatVM.CurrentMessage = obj as string;
            _newMessage = true;
        }

        public async void StartChatting()
        {
            await Chatting();
        }

        public async Task Chatting()
        {
            using (var chat = _client.SendMessage())
            {
                await chat.RequestStream.WriteAsync(new ChatMessage()
                {
                    Sender = CurrentUser,
                    Content = "",
                    DateTimeStamp = DateTime.UtcNow.ToTimestamp()
                });
                var task = Task.Run(async () =>
                {
                    while (await chat.ResponseStream.MoveNext(cancellationToken: CancellationToken.None))
                    {
                        _chatVM.ChatMessages.Add(chat.ResponseStream.Current);
                    }
                });
                while (_isRunning)
                {
                    await Task.Delay(500);
                    if (_newMessage)
                    {
                        await chat.RequestStream.WriteAsync(new ChatMessage()
                        {
                            Sender = CurrentUser,
                            Content = _chatVM.CurrentMessage,
                            DateTimeStamp = DateTime.UtcNow.ToTimestamp()
                        });
                        _newMessage = false;
                    }
                }
            }
            await _client.LogOutAsync(CurrentUser);
        }
    }
}
