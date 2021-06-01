using ChatClientProvider.Protos;
using ChatClientProvider.Services;
using ChatClientWPF.Models;
using ChatClientWPF.ViewModels;
using ChatClientWPF.Views;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ChatClientWPF.Services
{
    class ChatLogic
    {
        private static Chat.ChatClient _client;
        public static User CurrentUser { get; set; }
        public bool _newMessage = false;
        private ChatVM _chatVM;
        private Grpc.Core.AsyncDuplexStreamingCall<ChatMessage, ChatMessage> chat;

        public ChatLogic(ChatVM chatVM)
        {
            _client = new GrpcChatServiceProvider().GetChatClient();
            chat = _client.SendMessage();
            _chatVM = chatVM;
            CurrentUser = Helper.user;
            StartChatting();
        }

        public async Task UserLogOut(object obj)
        {
            await _client.LogOutAsync(CurrentUser);
            await chat.RequestStream.CompleteAsync();
            (obj as Window).Close();
        }

        public void SendMessage(object obj)
        {
            _chatVM.CurrentMessage = obj as string;
            _newMessage = true;
        }

        public async void StartChatting()
        {
            // await UpdateUserList();
            await Chatting();
        }

        public void UpdateUserList(ChatMessage message)
        {
            Regex pattern = new Regex(@"\w+\shas\sconnected");
            Regex pattern2 = new Regex(@"\w+\shas\sdisconnected");
            if (pattern.IsMatch(message.Content))
            {
                lock (_chatVM._userListLock)
                {
                    _chatVM.Users.Add(message.Sender);
                }
            }
            else if (pattern2.IsMatch(message.Content))
            {

                lock (_chatVM._userListLock)
                {
                    _chatVM.Users.Remove(_chatVM.Users.Where(u => u.ID == message.Sender.ID).FirstOrDefault());
                }
            }
        }

        public TextBlock FormatMessage(string message)
        {
            Regex regex = new Regex(@"(?<=\s\*)([^\*]*)(?=\*\s)");
            var matches = regex.Matches(message);
            TextBlock formattedMessage=new ();
            int start = 0;
            foreach(Match match in matches)
            {
                formattedMessage.Inlines.Add(message.Substring(start, match.Index-1));
                formattedMessage.Inlines.Add(new Run(message.Substring(match.Index, match.Index+match.Value.Length)) { FontWeight = FontWeights.Bold });
                start = match.Index + match.Length+1;
            }
            formattedMessage.Inlines.Add(message.Substring(start, message.Length-1));

            return formattedMessage;
        }
        public async Task Chatting()
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
                    chat.ResponseStream.Current.DateTimeStamp = DateTime.UtcNow.ToTimestamp();
                    _chatVM.ChatMessages.Add(new DisplayedMessage()
                    { 
                        Sender = chat.ResponseStream.Current.Sender,
                        Content = FormatMessage(chat.ResponseStream.Current.Content),
                        SentTime = chat.ResponseStream.Current.DateTimeStamp.ToDateTime()
                    }) ;
                    UpdateUserList(chat.ResponseStream.Current);
                }
            });
            while (true)
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
            await chat.RequestStream.CompleteAsync();


        }
    }
}
