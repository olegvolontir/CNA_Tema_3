using ChatService.Models;
using ChatService.Protos;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatService.Services
{
    public class ChatRoomService
    {
        public List<Models.User> Users { get; set; } = new();

        public List<ChatMessage> ChatHistory { get; set; } = new();

        public async Task SendMessageToUsers(ChatMessage chatMessage)
        {
            ChatHistory.Add(chatMessage);
            foreach (var user in Users)
            {
                    await user.ResponseStream.WriteAsync(chatMessage);
            }
        }

        public async Task GetAllMessages(IServerStreamWriter<ChatMessage> responseStream)
        {
            foreach (var message in ChatHistory)
            {
                await responseStream.WriteAsync(message);
            }
        }

       
    }
}
