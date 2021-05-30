using ChatClientWPF.Commands;
using ChatClientWPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatClientWPF.ViewModels
{
    class LoginVM
    {
        private LoginLogic _loginLogic;

        public LoginVM()
        {
            _loginLogic = new LoginLogic();
        }

        private ICommand logInCommand;
        public ICommand LogInCommand
        {
            get
            {
                if (logInCommand == null)
                {
                    //logInCommand = new RelayCommandGeneric<string>(async param => await chatLogic.UserLogIn(param));
                    logInCommand = new RelayCommand(async param => await _loginLogic.UserLogIn(param));
                }
                return logInCommand;
            }
        }
    }
    
}
