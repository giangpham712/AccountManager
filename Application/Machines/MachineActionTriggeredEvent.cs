using MediatR;

namespace AccountManager.Application.Machines
{
    public class MachineUserActionEvent : INotification
    {
        public Domain.Entities.Machine.Machine Machine { get; set; }
        public string Action { get; set; }
        public string User { get; set; }
    }
}