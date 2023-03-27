using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities.Git;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Git.Queries.GetAllRepos
{
    public class GetAllReposQueryHandler : IRequestHandler<GetAllReposQuery, List<RepoDto>>
    {
        private readonly ICloudStateDbContext _context;
        private readonly IMapper _mapper;

        public GetAllReposQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<RepoDto>> Handle(GetAllReposQuery request, CancellationToken cancellationToken)
        {
            var repos = await _context.Set<Repo>().ToListAsync(cancellationToken);
            return await Task.FromResult(_mapper.Map<List<RepoDto>>(repos));
        }
    }
}