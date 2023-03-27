using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities.Public;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Mma.Queries.GetMmaStats
{
    public class GetMmaStatsQuery : IRequest<MmaStatsDto>
    {
    }

    public class GetMmaStatsQueryHandler : IRequestHandler<GetMmaStatsQuery, MmaStatsDto>
    {
        private readonly ICloudStateDbContext _context;

        public GetMmaStatsQueryHandler(ICloudStateDbContext context)
        {
            _context = context;
        }

        public async Task<MmaStatsDto> Handle(GetMmaStatsQuery request, CancellationToken cancellationToken)
        {
            var standbyClasses = await _context.Set<MmaInstance>()
                .Where(x => x.Standby)
                .ToListAsync(cancellationToken);

            return new MmaStatsDto
            {
                Standby = standbyClasses.Count
            };
        }
    }
}