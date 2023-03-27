using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities.Git;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Git.Commands.SetCommitBreaking
{
    public class SetCommitBreakingCommandHandler : CommandHandlerBase<SetCommitBreakingCommand, Unit>
    {
        private readonly ISoftwareVersionResolver _softwareVersionResolver;

        public SetCommitBreakingCommandHandler(
            IMediator mediator,
            ICloudStateDbContext context,
            ISoftwareVersionResolver softwareVersionResolver) : base(mediator, context)
        {
            _softwareVersionResolver = softwareVersionResolver;
        }

        public override async Task<Unit> Handle(SetCommitBreakingCommand command, CancellationToken cancellationToken)
        {
            var repo = _softwareVersionResolver.GetRepoForSoftware(command.Software);
            var commit = await Context.Set<Commit>()
                .FirstOrDefaultAsync(c => c.ShortHash == command.Hash && c.Repo == repo, cancellationToken);

            if (commit == null)
                throw new EntityNotFoundException(nameof(Commit), command.Hash);

            commit.IsBreaking = command.IsBreaking;

            await Context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}