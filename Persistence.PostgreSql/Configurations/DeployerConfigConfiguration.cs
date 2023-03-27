using System.Collections.Generic;
using AccountManager.Domain.Entities.Public;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountManager.Persistence.PostgreSql.Configurations
{
    public class DeployerConfigConfiguration : EntityTypeConfigurationBase<DeployerConfig>
    {
        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>
        {
            { "DefaultValue", "default_value" },
        };

        public override void Configure(EntityTypeBuilder<DeployerConfig> builder)
        {
            builder
                .ToTable("deployer_configs", "public")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);
        }
    }
}