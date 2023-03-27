using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities.Public;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Public.Queries.GetComponentConfig
{
    public class GetComponentConfigQueryHandler : IRequestHandler<GetComponentConfigQuery, ComponentConfigDto>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;

        public GetComponentConfigQueryHandler(IMapper mapper, ICloudStateDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ComponentConfigDto> Handle(GetComponentConfigQuery request, CancellationToken cancellationToken)
        {
            var componentConfig = await _context.Set<ComponentConfig>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (componentConfig == null)
                throw new EntityNotFoundException(nameof(ComponentConfig), request.Id);

            return _mapper.Map<ComponentConfigDto>(componentConfig);
        }
    }
}