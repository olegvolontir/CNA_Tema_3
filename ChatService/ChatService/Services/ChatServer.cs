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

        private static List<ChatMessage> messages = new();

        public ChatServer(ILogger<ChatServer> logger)
        {
            _logger = logger;
        }        

        public override async Task GetAllMessages(Empty request, IServerStreamWriter<GetAllMessagesResponse> responseStream, ServerCallContext context)
        {
            var response = new GetAllMessagesResponse();
            response.Messages.Add(messages[0]);

            await responseStream.WriteAsync(response);
        }

        public override Task<Empty> SendMessage(SendMessageRequest request, ServerCallContext context)
        {
            messages.Add(request.ChatMessage);
            return Task.FromResult(new Empty());
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
