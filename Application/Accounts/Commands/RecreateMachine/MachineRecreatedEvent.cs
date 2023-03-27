using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Machine;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.RecreateMachine
{
    public class MachineRecreatedEvent : INotification
    {
        public string User { get; set; }
        public Machine Machine { get; set; }
        public dynamic Command { get; set; }
    }
}