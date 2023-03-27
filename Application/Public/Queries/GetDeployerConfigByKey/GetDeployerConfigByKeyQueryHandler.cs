using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities.Public;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Public.Queries.GetDeployerConfigByKey
{
    public class GetDeployerConfigByKeyQueryHandler : IRequestHandler<GetDeployerConfigByKeyQuery, DeployerConfigDto>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;

        public GetDeployerConfigByKeyQueryHandler(IMapper mapper, ICloudStateDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<DeployerConfigDto> Handle(GetDeployerConfigByKeyQuery request, CancellationToken cancellationToken)
        {
            var componentConfig = await _context.Set<DeployerConfig>().FirstOrDefaultAsync(x => x.RootKey + "." + x.SubKey == request.Key, cancellationToken);
            if (componentConfig == null)
                throw new EntityNotFoundException(nameof(DeployerConfig), request.Key);

            return _mapper.Map<DeployerConfigDto>(componentConfig);
        }
    }
}