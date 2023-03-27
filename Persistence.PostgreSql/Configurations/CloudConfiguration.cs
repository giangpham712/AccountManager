using System.Collections.Generic;
using AccountManager.Domain.Entities.Public;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountManager.Persistence.PostgreSql.Configurations
{
    public class CloudInstanceTypeConfiguration : EntityTypeConfigurationBase<CloudInstanceType>
    {
        public override void Configure(EntityTypeBuilder<CloudInstanceType> builder)
        {
            builder
                .ToTable("cloud_instance_type", "public")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);
        }
    }

    public class RedisCloudInstanceTypeConfiguration : EntityTypeConfigurationBase<RedisCloudInstanceType>
    {
        public override void Configure(EntityTypeBuilder<RedisCloudInstanceType> builder)
        {
            builder
                .ToTable("redis_cloud_instance_type", "public")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);
        }
    }

    public class CloudRegionConfiguration : EntityTypeConfigurationBase<CloudRegion>
    {
        public override void Configure(EntityTypeBuilder<CloudRegion> builder)
        {
            builder
                .ToTable("cloud_region", "public")
                .HasKey(e => e.CloudCode);

            AutoMapProperties(builder);
        }
    }

    public class CloudBaseImageConfiguration : EntityTypeConfigurationBase<CloudBaseImage>
    {
        public override void Configure(EntityTypeBuilder<CloudBaseImage> builder)
        {
            builder
                .ToTable("cloud_base_image", "public")
                .HasKey(e => e.Name);

            AutoMapProperties(builder);
        }
    }

    public class MmaInstanceConfiguration : EntityTypeConfigurationBase<MmaInstance>
    {
        public override void Configure(EntityTypeBuilder<MmaInstance> builder)
        {
            builder
                .ToTable("mma_instance", "public")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);

            builder.HasOne(e => e.MachineClass)
                .WithMany(e => e.MmaInstances)
                .HasForeignKey(e => e.MachineClassId)
                .IsRequired();
        }

        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>
        {
            { "MachineClassId", "machine_class" }
        };
    }
}