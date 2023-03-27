using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities.Public;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Public.Queries.GetComponentConfigByKey
{
    public class GetComponentConfigByKeyQueryHandler : IRequestHandler<GetComponentConfigByKeyQuery, ComponentConfigDto>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;

        public GetComponentConfigByKeyQueryHandler(IMapper mapper, ICloudStateDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ComponentConfigDto> Handle(GetComponentConfigByKeyQuery request, CancellationToken cancellationToken)
        {
            var componentConfig = await _context.Set<ComponentConfig>().FirstOrDefaultAsync(x => x.RootKey + "." + x.SubKey == request.Key, cancellationToken);
            if (componentConfig == null)
                throw new EntityNotFoundException(nameof(ComponentConfig), request.Key);

            return _mapper.Map<ComponentConfigDto>(componentConfig);
        }
    }
}