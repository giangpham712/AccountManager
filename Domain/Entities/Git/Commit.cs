using System;
using System.Collections.Generic;

namespace AccountManager.Domain.Entities.Git
{
    public class Commit : IEntity
    {
        private IEnumerable<Tag> _tags;
        public DateTimeOffset Timestamp { get; set; }
        public string Author { get; set; }
        public string Message { get; set; }

        public string FullHash { get; set; }
        public string ShortHash { get; set; }

        public string Repo { get; set; }
        public long BranchId { get; set; }
        public Branch Branch { get; set; }

        public bool? IsBreaking { get; set; }
        public string BreakingMsg { get; set; }

        public long Id { get; set; }
    }

    public class Repo
    {
        private ICollection<Commit> _commits;

        public string Name { get; set; }
        public string Url { get; set; }

        public ICollection<Commit> Commits
        {
            get => _commits ?? (_commits = new List<Commit>());
            set => _commits = value;
        }
    }

    public class Tag : IEntity
    {
        public string Name { get; set; }
        public string TargetCommitHash { get; set; }
        public string Repo { get; set; }
        public long Id { get; set; }
    }

    public class Branch : IEntity
    {
        private ICollection<Commit> _commits;
        public string Name { get; set; }
        public string Repo { get; set; }
        public bool Stable { get; set; }

        public ICollection<Commit> Commits
        {
            get => _commits ?? (_commits = new List<Commit>());
            set => _commits = value;
        }

        public long Id { get; set; }
    }
}