using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Machine;
using MediatR;

namespace AccountManager.Application.Audit.Events
{
    public class UserActionNotification : INotification
    {
        public string Action { get; set; }
        public string User { get; set; }
        public Account[] Accounts { get; set; }
        public Machine[] Machines { get; set; }
        public dynamic Data { get; set; }
    }
}