using System.Collections.Generic;
using MediatR;

namespace AccountManager.Application.Machines.Commands
{
    public class MachineUpdatedEvent : INotification
    {
        public string User { get; set; }
        public Domain.Entities.Machine.Machine Machine { get; set; }
        public Dictionary<string, object> Updates { get; set; }
    }
}