using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using AccountManager.Domain.Entities.Public;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountManager.Persistence.PostgreSql.Configurations
{
    public class ComponentConfigConfiguration : EntityTypeConfigurationBase<ComponentConfig>
    {
        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>
        {
            { "OnPremDefaultValue", "onprem_default_value" },
        };

        public override void Configure(EntityTypeBuilder<ComponentConfig> builder)
        {
            builder
                .ToTable("component_configs", "public")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);
        }
    }
}
