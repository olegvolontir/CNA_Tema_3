﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ChatService.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace ChatService.Services
{
    public class ChatServer : Chat.ChatBase
    {
        private readonly ILogger<ChatServer> _logger;
        private ChatRoomService _chatRoomService;

        public ChatServer(ILogger<ChatServer> logger, ChatRoomService chatRoomService)
        {
            _logger = logger;
            _chatRoomService = chatRoomService;
        }

        public override async Task SendMessage(IAsyncStreamReader<ChatMessage> requestStream, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
        {
            if (!await requestStream.MoveNext()) return;

            _chatRoomService
                .Users
                .Where(u => u.ID == requestStream.Current.Sender.ID)
                .FirstOrDefault().ResponseStream = responseStream;

            try
            {
                while (await requestStream.MoveNext())
                {
                    var message = requestStream.Current;
                    await _chatRoomService.SendMessageToUsers(message);
                    _logger.Log(LogLevel.Information, "Message sent: " + message.Sender.Name 
                        + ": " + message.Content + " at " + message.DateTimeStamp.ToDateTime().Hour
                        + ":" + message.DateTimeStamp.ToDateTime().Minute);
                }
            }
            catch(IOException)
            {
                await LogOut(requestStream.Current.Sender, context);
            }
        }

        public override Task<LogResponse> LogIn(User request, ServerCallContext context)
        {
            _logger.Log(LogLevel.Information, request.Name + " has connected.");

            _chatRoomService.Users.Add(new Models.User() { ID = request.ID, Name = request.Name });

            return Task.FromResult(new LogResponse() { Status = LogResponse.Types.Status.Connected });
        }

        public override Task<LogResponse> LogOut(User request, ServerCallContext context)
        {
            _logger.Log(LogLevel.Information, request.Name + " has disconnected.");

            _chatRoomService.Users.Remove(_chatRoomService.Users.Single(u => u.ID == request.ID));

            return Task.FromResult(new LogResponse() { Status = LogResponse.Types.Status.Disconnected });
        }

    }
}
