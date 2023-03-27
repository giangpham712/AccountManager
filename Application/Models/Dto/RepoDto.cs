using System;

namespace AccountManager.Application.Models.Dto
{
    public class RepoDto
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class CommitDto
    {
        public long Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Author { get; set; }
        public string Message { get; set; }

        public string FullHash { get; set; }
        public string ShortHash { get; set; }

        public string Repo { get; set; }
        public long BranchId { get; set; }
        public BranchDto Branch { get; set; }

        public bool? IsBreaking { get; set; }
        public string BreakingMsg { get; set; }

        public TagDto[] Tags { get; set; }
    }

    public class TagDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string TargetCommitHash { get; set; }
        public string Repo { get; set; }
    }

    public class BranchDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Repo { get; set; }
        public string Software { get; set; }
    }
}