using System.Collections.Generic;
using AccountManager.Domain.Entities.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountManager.Persistence.PostgreSql.Configurations
{
    public class FileConfiguration : EntityTypeConfigurationBase<File>
    {
        public override void Configure(EntityTypeBuilder<File> builder)
        {
            builder
                .ToTable("file", "library")
                .HasKey(e => e.Id);

            builder.HasMany(e => e.Packages)
                .WithOne(e => e.File)
                .HasForeignKey(e => e.FileId);

            AutoMapProperties(builder);
        }

        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>
        {
            { "ReleaseStage", null },
            { "BaseVersion", null },
            { "Used", null }
        };
    }

    public class PackageConfiguration : EntityTypeConfigurationBase<Package>
    {
        public override void Configure(EntityTypeBuilder<Package> builder)
        {
            builder
                .ToTable("package", "library")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);
        }

        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>
        {
            { "FileId", "file" }
        };
    }
}