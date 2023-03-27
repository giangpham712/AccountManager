using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities.Public;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Public.Queries.GetDeployerConfig
{
    public class GetDeployerConfigQueryHandler : IRequestHandler<GetDeployerConfigQuery, DeployerConfigDto>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;

        public GetDeployerConfigQueryHandler(IMapper mapper, ICloudStateDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<DeployerConfigDto> Handle(GetDeployerConfigQuery request, CancellationToken cancellationToken)
        {
            var componentConfig = await _context.Set<DeployerConfig>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (componentConfig == null)
                throw new EntityNotFoundException(nameof(DeployerConfig), request.Id);

            return _mapper.Map<DeployerConfigDto>(componentConfig);
        }
    }
}