using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities.Machine;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Mma.Queries.GetAllClasses
{
    public class GetAllClassesQueryHandler : IRequestHandler<GetAllClassesQuery, IEnumerable<ClassDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;

        public GetAllClassesQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClassDto>> Handle(GetAllClassesQuery request, CancellationToken cancellationToken)
        {
            var classes = await _context.Set<Class>().Include(x => x.MmaInstances).ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<ClassDto>>(classes);
        }
    }
}