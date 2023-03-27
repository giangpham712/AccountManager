using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities.Machine;
using AccountManager.Domain.Entities.Public;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AccountManager.Application.Machines.Commands.UpdateComponentConfigForMachine
{
    public class UpdateComponentConfigForMachineCommandHandler : CommandHandlerBase<UpdateComponentConfigForMachineCommand, Unit>
    {
        public UpdateComponentConfigForMachineCommandHandler(IMediator mediator, ICloudStateDbContext context) : base(mediator, context)
        {
        }

        public override async Task<Unit> Handle(UpdateComponentConfigForMachineCommand command, CancellationToken cancellationToken)
        {
            var machine = await Context.Set<Domain.Entities.Machine.Machine>()
                .Include(x => x.Config)
                .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

            if (machine == null)
                throw new EntityNotFoundException(nameof(Domain.Entities.Machine.Machine), command.Id);

            var componentConfigs = await Context.Set<ComponentConfig>().ToListAsync(cancellationToken);
            var componentConfigMapByKey = componentConfigs.ToDictionary(x => $"{x.RootKey}.{x.SubKey}", x => x);

            if (machine.Config == null)
            {
                machine.Config = new Config();
            }

            var machineComponentConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(machine.Config.ComponentConfigJson ?? "{}");

            foreach (var entry in command.ComponentConfig)
            {
                if (!componentConfigMapByKey.TryGetValue(entry.Key, out var componentConfig))
                {
                    continue;
                }

                if (!machineComponentConfig.ContainsKey(entry.Key))
                {
                    machineComponentConfig[entry.Key] = entry.Value;
                }
                
                if (!componentConfig.Protected && !Equals(machineComponentConfig[entry.Key], entry.Value))
                {
                    machineComponentConfig[entry.Key] = entry.Value;
                }
            }

            machine.Config.ComponentConfigJson = JsonConvert.SerializeObject(machineComponentConfig);

            await Context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}