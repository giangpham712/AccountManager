using System.Collections.Generic;
using AccountManager.Domain.Entities.Git;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountManager.Persistence.PostgreSql.Configurations
{
    public class RepoConfiguration : EntityTypeConfigurationBase<Repo>
    {
        public override void Configure(EntityTypeBuilder<Repo> builder)
        {
            builder
                .ToTable("repo", "git")
                .HasKey(e => e.Name);

            AutoMapProperties(builder);

            builder.HasMany(e => e.Commits)
                .WithOne()
                .IsRequired()
                .HasForeignKey(e => e.Repo);
        }
    }

    public class CommitConfiguration : EntityTypeConfigurationBase<Commit>
    {
        public override void Configure(EntityTypeBuilder<Commit> builder)
        {
            builder
                .ToTable("commit", "git")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);
        }

        protected override IDictionary<string, string> ColumnMappings { get; } = new Dictionary<string, string>
        {
            { "BranchId", "branch" },
            { "Tags", null }
        };
    }

    public class TagConfiguration : EntityTypeConfigurationBase<Tag>
    {
        public override void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder
                .ToTable("tag", "git")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);
        }
    }

    public class BranchConfiguration : EntityTypeConfigurationBase<Branch>
    {
        public override void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder
                .ToTable("branch", "git")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);

            builder.HasMany(e => e.Commits)
                .WithOne(e => e.Branch)
                .IsRequired()
                .HasForeignKey(e => e.BranchId);
        }

        protected override IDictionary<string, string> ColumnMappings { get; } = new Dictionary<string, string>
        {
            { "BranchId", "branch" }
        };
    }
}