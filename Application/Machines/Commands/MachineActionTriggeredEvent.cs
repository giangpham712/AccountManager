using MediatR;

namespace AccountManager.Application.Machines.Commands
{
    public class MachineActionTriggeredEvent : INotification
    {
        public string User { get; set; }
        public Domain.Entities.Machine.Machine Machine { get; set; }
        public string Action { get; set; }
        public string Params { get; set; }
    }
}