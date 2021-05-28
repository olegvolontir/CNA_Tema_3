using ChatService.Models;
using ChatService.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatService.Services
{
    public class ChatRoomService
    {
        public List<Models.User> Users { get; set; } = new();

        public async Task SendMessageToUsers(ChatMessage chatMessage)
        {
            foreach(var user in Users)
            {
                if (user.ID != chatMessage.Sender.ID)
                {
                    await user.ResponseStream.WriteAsync(chatMessage);
                }
            }
        }
    }
}
