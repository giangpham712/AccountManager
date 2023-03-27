using System.Collections.Generic;

namespace AccountManager.Application.Notification.Commands.SendMessage
{
    public class SendMessageCommand : CommandBase
    {
        public IEnumerable<long> Accounts { get; set; }
        public IEnumerable<string> AccountUrlNames { get; set; }

        public IEnumerable<long> Machines { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }
    }
}