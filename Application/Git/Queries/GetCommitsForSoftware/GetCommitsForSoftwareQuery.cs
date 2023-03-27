using AccountManager.Application.Models.Dto;
using AccountManager.Domain;
using MediatR;

namespace AccountManager.Application.Git.Queries.GetCommitsForSoftware
{
    public class GetCommitsForSoftwareQuery : IRequest<PagedResult<CommitDto>>
    {
        public GetCommitsForSoftwareQuery()
        {
            StartIndex = 0;
            Limit = 20;
        }

        public Software Software { get; set; }
        public int? BranchId { get; set; }
        public bool TagsOnly { get; set; }
        public bool BuildsOnly { get; set; }
        public string Search { get; set; }
        public int StartIndex { get; set; }
        public int Limit { get; set; }
    }
}