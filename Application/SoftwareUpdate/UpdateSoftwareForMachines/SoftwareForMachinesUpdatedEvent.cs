using System.Collections.Generic;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Machine;
using MediatR;

namespace AccountManager.Application.SoftwareUpdate.UpdateSoftwareForMachines
{
    public class SoftwareUpdatedEvent : INotification
    {
        private IEnumerable<Account> _accounts;
        private IEnumerable<Machine> _machines;
        public string Actor { get; internal set; }

        public IEnumerable<Machine> Machines
        {
            get => _machines ?? (_machines = new List<Machine>());
            internal set => _machines = value;
        }

        public IEnumerable<Account> Accounts
        {
            get => _accounts ?? (_accounts = new List<Account>());
            internal set => _accounts = value;
        }

        public dynamic Command { get; internal set; }
    }
}