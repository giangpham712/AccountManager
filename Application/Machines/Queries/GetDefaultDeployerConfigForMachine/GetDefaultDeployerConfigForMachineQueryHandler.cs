using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Configs;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Machine;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AccountManager.Application.Machines.Queries.GetDefaultDeployerConfigForMachine
{
    public class GetDefaultDeployerConfigForMachineQueryHandler : CommandHandlerBase<GetDefaultDeployerConfigForMachineQuery, ConfigDto>
    {
        private readonly IMapper _mapper;
        private readonly IConfigGenerator _configGenerator;

        public GetDefaultDeployerConfigForMachineQueryHandler(
            IMediator mediator, 
            ICloudStateDbContext context, 
            IConfigGenerator configGenerator, 
            IMapper mapper) : base(mediator, context)
        {
            _configGenerator = configGenerator;
            _mapper = mapper;
        }

        public override async Task<ConfigDto> Handle(GetDefaultDeployerConfigForMachineQuery query, CancellationToken cancellationToken)
        {
            var machine = await Context.Set<Domain.Entities.Machine.Machine>()
                .Include(x => x.Config)
                .FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken);

            if (machine == null)
                throw new EntityNotFoundException(nameof(Machine), query.Id);

            var account = await Context.Set<Account>()
                .Include(x => x.MachineConfig)
                .Include(x => x.Keys)
                .FirstOrDefaultAsync(x => x.Id == machine.AccountId, cancellationToken);

            var site = await Context.Set<Site>().FirstOrDefaultAsync(x => x.MachineId == machine.Id, cancellationToken);

            var deployerConfig = await _configGenerator.GenerateDeployerConfig(machine, site);

            return _mapper.Map<ConfigDto>(new Config()
            {
                MachineId = machine.Id,
                DeployerConfigJson = JsonConvert.SerializeObject(deployerConfig)
            });
        }
    }
}