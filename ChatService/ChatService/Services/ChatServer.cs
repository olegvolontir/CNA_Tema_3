using System;
using System.Collections.Generic;
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
        public ChatServer(ILogger<ChatServer> logger)
        {
            _logger = logger;
        }

        public override Task GetAllMessages(Empty request, IServerStreamWriter<GetAllMessagesResponse> responseStream, ServerCallContext context)
        {
            return base.GetAllMessages(request, responseStream, context);
        }
        public override Task<Empty> SendMessage(SendMessageRequest request, ServerCallContext context)
        {
            return base.SendMessage(request, context);
        }

        public override Task<LogResponse> LogIn(User request, ServerCallContext context)
        {
            _logger.Log(LogLevel.Information, request.Name + " has connected.");
            Console.WriteLine(request.Name + " has connected.");
            return Task.FromResult(new LogResponse() { Status = LogResponse.Types.Status.Connected });
        }

        public override Task<LogResponse> LogOut(User request, ServerCallContext context)
        {
            _logger.Log(LogLevel.Information, request.Name + " has disconnected.");
            Console.WriteLine(request.Name + " has disconnected.");
            return Task.FromResult(new LogResponse() { Status = LogResponse.Types.Status.Disconnected });
        }

    }
}
