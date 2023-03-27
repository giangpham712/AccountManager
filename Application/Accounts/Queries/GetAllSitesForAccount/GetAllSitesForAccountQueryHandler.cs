using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Queries.GetAllSitesForAccount
{
    public class GetAllSitesForAccountQueryHandler : IRequestHandler<GetAllSitesForAccountQuery, IEnumerable<SiteDto>>
    {
        private readonly ICloudStateDbContext _context;
        private readonly IMapper _mapper;

        public GetAllSitesForAccountQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SiteDto>> Handle(GetAllSitesForAccountQuery request,
            CancellationToken cancellationToken)
        {
            var sites = await _context.Set<Site>()
                .Where(x => x.AccountId == request.Id)
                .ToListAsync(cancellationToken);

            return await Task.FromResult(_mapper.Map<IEnumerable<SiteDto>>(sites));
        }
    }
}