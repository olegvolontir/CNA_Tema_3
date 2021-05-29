using ChatClientProvider.Protos;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClientProvider.Services
{
    public class GrpcChatServiceProvider
    {
        private string Url { get; set; }
        private Lazy<GrpcChannel> RpcChannel { get; set; }
        private Chat.ChatClient ChatClient { get; set; }

        public GrpcChatServiceProvider()
        {
            this.Url = "https://localhost:5001";
            this.RpcChannel = new Lazy<GrpcChannel>(GrpcChannel.ForAddress(this.Url));
        }

        public Chat.ChatClient GetChatClient() => this.ChatClient ??= new Chat.ChatClient(this.RpcChannel.Value);

    }
}
