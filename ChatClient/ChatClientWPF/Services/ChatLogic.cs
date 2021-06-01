﻿using ChatClientProvider.Protos;
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
using System.Windows.Threading;

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

        public TextBlock FormatMessage(string input)
        {
            var textBlock = new TextBlock();

            Regex regex = new Regex(@"(?<=\s\*)(.*?)(?=\*\s)");

            var matches = regex.Matches(input);
            string[] split = Regex.Split(input, @"\s\*([^\*]*)\*\s");

            int j = 0;
            for (int i = 0; i < split.Length; ++i)
            {
                if (j < matches.Count && split[i].Equals(matches[j].Value))
                {
                    textBlock.Inlines.Add(new Run(split[i] + ' ') { FontWeight = FontWeights.Bold });
                    ++j;
                }
                else
                {
                    textBlock.Inlines.Add(split[i] + ' ');
                }
            }

            return textBlock;
        }

        public async Task Chatting()
        {
            await chat.RequestStream.WriteAsync(new ChatMessage()
            {
                Sender = CurrentUser,
                Content = "",
                DateTimeStamp = DateTime.UtcNow.ToTimestamp()
            });

            string text = "";

            var dispatcher = Application.Current.Dispatcher;

            Thread t1 = new Thread(async () =>
            {
                while (await chat.ResponseStream.MoveNext(cancellationToken: CancellationToken.None))
                {
                    chat.ResponseStream.Current.DateTimeStamp = DateTime.UtcNow.ToTimestamp();

                    text = chat.ResponseStream.Current.Content;

                    dispatcher.Invoke((Action)(() =>
                    {
                        _chatVM.ChatMessages.Add(new DisplayedMessage()
                        {
                            Sender = chat.ResponseStream.Current.Sender,
                            Content = FormatMessage(chat.ResponseStream.Current.Content),
                            SentTime = chat.ResponseStream.Current.DateTimeStamp.ToDateTime()
                        });
                    }));

                    UpdateUserList(chat.ResponseStream.Current);
                }
            });
            t1.SetApartmentState(ApartmentState.STA);
            t1.Start();

            while (true)
            {
                await Task.Delay(200);
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
    }
}
