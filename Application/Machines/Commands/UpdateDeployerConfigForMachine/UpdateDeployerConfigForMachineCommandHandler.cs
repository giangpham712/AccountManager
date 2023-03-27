using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities.Machine;
using AccountManager.Domain.Entities.Public;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AccountManager.Application.Machines.Commands.UpdateDeployerConfigForMachine
{
    public class UpdateDeployerConfigForMachineCommandHandler : CommandHandlerBase<UpdateDeployerConfigForMachineCommand, Unit>
    {
        public UpdateDeployerConfigForMachineCommandHandler(IMediator mediator, ICloudStateDbContext context) : base(mediator, context)
        {
        }

        public override async Task<Unit> Handle(UpdateDeployerConfigForMachineCommand command, CancellationToken cancellationToken)
        {
            var machine = await Context.Set<Domain.Entities.Machine.Machine>()
                .Include(x => x.Config)
                .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

            if (machine == null)
                throw new EntityNotFoundException(nameof(Domain.Entities.Machine.Machine), command.Id);

            var deployerConfigs = await Context.Set<DeployerConfig>().ToListAsync(cancellationToken);
            var deployerConfigMapByKey = deployerConfigs.ToDictionary(x => $"{x.RootKey}.{x.SubKey}", x => x);

            if (machine.Config == null)
            {
                machine.Config = new Config();
            }

            var machineDeployerConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(machine.Config.DeployerConfigJson ?? "{}");

            foreach (var entry in command.DeployerConfig)
            {
                if (!deployerConfigMapByKey.TryGetValue(entry.Key, out var componentConfig))
                {
                    continue;
                }

                if (!machineDeployerConfig.ContainsKey(entry.Key))
                {
                    machineDeployerConfig[entry.Key] = entry.Value;
                }

                if (!componentConfig.Protected && !Equals(machineDeployerConfig[entry.Key], entry.Value))
                {
                    machineDeployerConfig[entry.Key] = entry.Value;
                }
            }

            machine.Config.DeployerConfigJson = JsonConvert.SerializeObject(machineDeployerConfig);

            await Context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}