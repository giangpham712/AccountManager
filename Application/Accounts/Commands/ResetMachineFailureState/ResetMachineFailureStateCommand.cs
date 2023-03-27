using MediatR;

namespace AccountManager.Application.Accounts.Commands.ResetMachineFailureState
{
    public class ResetMachineFailureStateCommand : IRequest
    {
        public long Id { get; set; }
    }
}