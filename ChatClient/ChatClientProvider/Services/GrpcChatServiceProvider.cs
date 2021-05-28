using ChatService.Protos;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatClientProvider.Services
{
    class GrpcChatServiceProvider
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
