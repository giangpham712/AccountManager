using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Application.Services;
using AccountManager.Domain.Entities.Git;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Git.Queries.GetCommitsForSoftware
{
    public class GetCommitsForSoftwareQueryHandler : IRequestHandler<GetCommitsForSoftwareQuery, PagedResult<CommitDto>>
    {
        private readonly IMapper _mapper;
        private readonly IBuildFileService _buildFileService;
        private readonly ICloudStateDbContext _context;
        private readonly ISoftwareVersionResolver _softwareVersionResolver;

        public GetCommitsForSoftwareQueryHandler(
            ICloudStateDbContext context,
            IBuildFileService buildFileService,
            ISoftwareVersionResolver softwareVersionResolver, IMapper mapper)
        {
            _context = context;
            _buildFileService = buildFileService;
            _softwareVersionResolver = softwareVersionResolver;
            _mapper = mapper;
        }

        public async Task<PagedResult<CommitDto>> Handle(GetCommitsForSoftwareQuery query,
            CancellationToken cancellationToken)
        {
            var repo = _softwareVersionResolver.GetRepoForSoftware(query.Software);
            var queryable = _context.Set<Commit>()
                .Where(x => x.Repo == repo)
                .Select(x => new
            {
                Commit = x,
                Tags = _context.Set<Tag>().Where(y => y.TargetCommitHash == x.FullHash).ToList()
            });

            if (query.BranchId.HasValue)
                queryable = queryable.Where(x => x.Commit.Branch != null && x.Commit.BranchId == query.BranchId);

            if (query.TagsOnly) queryable = queryable.Where(x => x.Tags.Any());

            if (query.BuildsOnly)
            {
                var buildFiles = await _buildFileService.ListBuildFiles(query.Software);
                var hashes = buildFiles.Select(x => x.Hash);

                queryable = queryable.Where(x => hashes.Contains(x.Commit.ShortHash));
            }

            if (!string.IsNullOrWhiteSpace(query.Search))
                queryable = queryable.Where(x => x.Commit.FullHash.Contains(query.Search));

            var total = await queryable.CountAsync(cancellationToken);

            var commits = (await queryable
                .OrderByDescending(x => x.Commit.Timestamp)
                .Skip(query.StartIndex)
                .Take(query.Limit)
                .ToListAsync(cancellationToken)).Select(x =>
                {
                    var commitDto = _mapper.Map<CommitDto>(x.Commit);
                    commitDto.Tags = _mapper.Map<TagDto[]>(x.Tags);
                    return commitDto;
                })
                .ToList();

            return new PagedResult<CommitDto>
            {
                Items = commits,
                TotalItems = total,
                StartIndex = query.StartIndex,
                Limit = query.Limit,
                HasMore = commits.Any()
            };
        }

        public async Task<PagedResult<CommitDto>> Handle2(GetCommitsForSoftwareQuery query,
            CancellationToken cancellationToken)
        {
            var repo = _softwareVersionResolver.GetRepoForSoftware(query.Software);
            var queryable = from commit in _context.Set<Commit>()
                join tag in _context.Set<Tag>() on commit.FullHash equals tag.TargetCommitHash into tags
                where commit.Repo == repo
                select new
                {
                    Commit = commit,
                    Tags = tags
                };

            if (query.BranchId.HasValue)
                queryable = queryable.Where(x => x.Commit.Branch != null && x.Commit.BranchId == query.BranchId);

            if (query.TagsOnly) queryable = queryable.Where(x => x.Tags.Any());

            if (query.BuildsOnly)
            {
                var buildFiles = await _buildFileService.ListBuildFiles(query.Software);
                var hashes = buildFiles.Select(x => x.Hash);

                queryable = queryable.Where(x => hashes.Contains(x.Commit.ShortHash));
            }

            if (!string.IsNullOrWhiteSpace(query.Search))
                queryable = queryable.Where(x => x.Commit.FullHash.Contains(query.Search));

            var total = await queryable.SumAsync(x => 1, cancellationToken);

            var commits = (await queryable
                .ToListAsync(cancellationToken)).Select(x =>
            {
                var commit = x.Commit;
                return commit;
            });

            return new PagedResult<CommitDto>
            {
                Items = _mapper.Map<IEnumerable<CommitDto>>(commits),
                TotalItems = total,
                StartIndex = query.StartIndex,
                Limit = query.Limit,
                HasMore = commits.Any()
            };
        }
    }
}