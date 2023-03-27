using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities.Machine;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Mma.Commands.UpdateMmaClass
{
    public class UpdateMmaClassCommandHandler : CommandHandlerBase<UpdateMmaClassCommand, ClassDto>
    {
        private readonly IMapper _mapper;
        public UpdateMmaClassCommandHandler(IMediator mediator, ICloudStateDbContext context, IMapper mapper) : base(mediator,
            context)
        {
            _mapper = mapper;
        }

        public override async Task<ClassDto> Handle(UpdateMmaClassCommand command, CancellationToken cancellationToken)
        {
            var mmaClass = await Context.Set<Class>().Include(x => x.MmaInstances)
                .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

            if (mmaClass == null) throw new EntityNotFoundException(nameof(Class), command.Id);

            var mmaInstance = mmaClass.MmaInstances.FirstOrDefault();

            if (mmaInstance == null) throw new CommandException();

            if (mmaInstance.DeployerVer != command.DeployerVersion)
                mmaInstance.DeployerVerUpdatedAt = DateTimeOffset.Now;

            if (mmaInstance.Standby != command.Standby)
            {
                var machinesWithClass = await Context.Set<Machine>().Where(x => x.ClassId == command.Id).ToListAsync(cancellationToken);
                foreach (var machine in machinesWithClass)
                {
                    machine.SetOperationModeToNormal();
                }
            }

            if (command.Name != null)
            {
                mmaClass.Name = command.Name;
            }

            mmaInstance.DeployerBranch = command.DeployerBranch;
            mmaInstance.DeployerVer = command.DeployerVersion;
            mmaInstance.Standby = command.Standby;
            mmaInstance.PriorityGroup = command.PriorityGroup;

            Context.SetCommandTimeout(TimeSpan.FromSeconds(120));
            await Context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ClassDto>(mmaClass);
        }
    }
}