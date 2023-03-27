using System.Collections.Generic;
using AccountManager.Domain;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.UpdateAccount
{
    public class AccountPropertiesPushedEvent : INotification
    {
        public string User { get; internal set; }
        public Account Account { get; internal set; }
        public dynamic Command { get; internal set; }
        public IEnumerable<EntityChangeLog> Changes { get; internal set; }
    }
}