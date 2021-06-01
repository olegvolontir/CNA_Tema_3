using ChatClientProvider.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ChatClientWPF.Models
{
    class DisplayedMessage
    {
        public User Sender { get; set; }
        public TextBlock Content { get; set; }
        public DateTime SentTime { get; set; }
    }
}
