using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities.Machine;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Mma.Queries.GetAllOperationTypes
{
    public class
        GetAllOperationTypesQueryHandler : IRequestHandler<GetAllOperationTypesQuery, IEnumerable<OperationTypeDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;

        public GetAllOperationTypesQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OperationTypeDto>> Handle(GetAllOperationTypesQuery request,
            CancellationToken cancellationToken)
        {
            var classes = await _context.Set<OperationType>().ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<OperationTypeDto>>(classes);
        }
    }
}