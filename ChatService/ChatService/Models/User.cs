using ChatService.Protos;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatService.Models
{
    public class User
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public IServerStreamWriter<ChatMessage> ResponseStream { get; set; }
    }
}
