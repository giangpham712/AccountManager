using AccountManager.Domain;
using MediatR;

namespace AccountManager.Application.Git.Commands.SetCommitBreaking
{
    public class SetCommitBreakingCommand : CommandBase<Unit>
    {
        public Software Software { get; set; }
        public string Hash { get; set; }
        public bool IsBreaking { get; set; }
    }
}