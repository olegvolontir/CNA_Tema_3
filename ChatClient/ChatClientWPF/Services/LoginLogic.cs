using ChatClientProvider.Protos;
using ChatClientProvider.Services;
using ChatClientWPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ChatClientWPF.Services
{
    class LoginLogic
    {
        private Chat.ChatClient _client = new GrpcChatServiceProvider().GetChatClient();

        public async Task UserLogIn(object obj)
        {
            var _params = (object[])obj;

            if (string.IsNullOrEmpty(_params[0] as string)) return;

            Helper.user = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Name = _params[0] as string
            };

            await _client.LogInAsync(Helper.user);


            ChatWindow window = new ChatWindow();
            window.Show();
            (_params[1] as Window).Close();
        }
    }
}
