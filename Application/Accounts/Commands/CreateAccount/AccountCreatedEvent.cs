using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.CreateAccount
{
    public class AccountCreatedEvent : INotification
    {
        public string User { get; internal set; }
        public Account Account { get; internal set; }
        public dynamic Command { get; set; }
    }
}