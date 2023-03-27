using System.Collections.Generic;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Machine;
using AccountManager.Domain.Entities.Public;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountManager.Persistence.PostgreSql.Configurations
{
    public class MachineConfiguration : EntityTypeConfigurationBase<Machine>
    {
        public override void Configure(EntityTypeBuilder<Machine> builder)
        {
            builder
                .ToTable("machine", "machine")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);

            builder.HasOne(e => e.Account)
                .WithMany(e => e.Machines)
                .HasForeignKey(e => e.AccountId);

            builder.HasOne(e => e.Class)
                .WithMany()
                .HasForeignKey(e => e.ClassId);

            builder.HasOne(e => e.CloudInstanceType)
                .WithMany()
                .HasForeignKey(e => e.CloudInstanceTypeId)
                .IsRequired();

            builder.HasMany(e => e.CloudInstances)
                .WithOne(e => e.Machine)
                .IsRequired()
                .HasForeignKey(e => e.MachineId);

            builder.HasMany(e => e.Operations)
                .WithOne(e => e.Machine)
                .IsRequired()
                .HasForeignKey(e => e.MachineId);

            builder.HasMany(e => e.Messages)
                .WithOne(e => e.Machine)
                .IsRequired()
                .HasForeignKey(e => e.MachineId);
        }

        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>
        {
            { "AccountId", "account" },
            { "ClassId", "class" },
            { "CloudInstanceTypeId", "cloud_instance_type" },
            { "IsSiteMaster", "is_sitemaster" },
            { "SiteMasterUrl", "sitemaster_url" },
            { "CloudBackupsSiteMaster", "cloud_backups_sitemaster" },
            { "CloudBackupsLauncher", "cloud_backups_launcher" },
        };
    }

    public class MessageConfiguration : EntityTypeConfigurationBase<Message>
    {
        public override void Configure(EntityTypeBuilder<Message> builder)
        {
            builder
                .ToTable("message", "machine")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);
        }

        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>
        {
            { "MachineId", "machine" }
        };
    }

    public class ClassConfiguration : EntityTypeConfigurationBase<Class>
    {
        public override void Configure(EntityTypeBuilder<Class> builder)
        {
            builder
                .ToTable("class", "machine")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);
        }

        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>
        {
            { "IsProduction", null }
        };
    }

    public class StateConfiguration : EntityTypeConfigurationBase<State>
    {
        public override void Configure(EntityTypeBuilder<State> builder)
        {
            builder
                .ToTable("state", "machine")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);

            builder.HasOne(e => e.Machine)
                .WithMany(e => e.States)
                .HasForeignKey(e => e.MachineId);
        }

        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>
        {
            { "SiteMaster", "sitemaster" },
            { "PdfExport", "pdfexport" },
            { "RelExport", "relexport" },
            { "SqlExport", "sqlexport" },
            { "FiberSenSys", "fibersensys" },
            { "FiberMountain", "fibermountain" },
            { "ServiceNow", "servicenow" },
            { "CommScope", "commscope" },
            { "MachineId", "machine" },
            { "DesiredState", null },
            { "SiteMasterBackup", "sitemaster_backup" }
        };
    }

    public class HistoricalDesiredStateConfiguration : EntityTypeConfigurationBase<HistoricalDesiredState>
    {
        public override void Configure(EntityTypeBuilder<HistoricalDesiredState> builder)
        {
            builder
                .ToTable("historical_desired_state", "machine")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);

            builder.HasOne(e => e.Machine)
                .WithMany()
                .HasForeignKey(e => e.MachineId);
        }

        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>
        {
            { "SiteMaster", "sitemaster" },
            { "PdfExport", "pdfexport" },
            { "RelExport", "relexport" },
            { "MachineId", "machine" },
            { "SiteMasterBackup", "sitemaster_backup" }
        };
    }

    public class CloudInstanceConfiguration : EntityTypeConfigurationBase<CloudInstance>
    {
        public override void Configure(EntityTypeBuilder<CloudInstance> builder)
        {
            builder
                .ToTable("cloud_instance", "machine")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);
        }

        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>
        {
            { "HostName", "hostname" },
            { "MachineId", "machine" },
            { "SiteMasterPopulated", "sitemaster_populated" }
        };
    }

    public class OperationConfiguration : EntityTypeConfigurationBase<Operation>
    {
        public override void Configure(EntityTypeBuilder<Operation> builder)
        {
            builder
                .ToTable("operation", "machine")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);

            builder.HasOne(x => x.Type)
                .WithMany()
                .HasForeignKey(x => x.TypeName);
        }

        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>
        {
            { "MachineId", "machine" },
            { "TypeName", "type" }
        };
    }

    public class UserOperationConfiguration : EntityTypeConfigurationBase<UserOperation>
    {
        public override void Configure(EntityTypeBuilder<UserOperation> builder)
        {
            builder
                .ToTable("user_operation", "machine")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);

            builder.HasOne(x => x.Type)
                .WithMany()
                .HasForeignKey(x => x.TypeName)
                .IsRequired();
        }

        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>
        {
            { "MachineId", "machine" },
            { "TypeName", "type" }
        };
    }

    public class OperationTypeConfiguration : EntityTypeConfigurationBase<OperationType>
    {
        public override void Configure(EntityTypeBuilder<OperationType> builder)
        {
            builder
                .ToTable("operation_type", "machine")
                .HasKey(e => e.Name);

            AutoMapProperties(builder);
        }
    }

    public class UserOperationTypeConfiguration : EntityTypeConfigurationBase<UserOperationType>
    {
        public override void Configure(EntityTypeBuilder<UserOperationType> builder)
        {
            builder
                .ToTable("user_operation_type", "machine")
                .HasKey(e => e.Name);

            AutoMapProperties(builder);
        }
    }

    public class BackupProfileConfiguration :  EntityTypeConfigurationBase<BackupProfile>
    {
        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>
        {
            { "CloudInstanceId", "cloud_instance" }
        };

        public override void Configure(EntityTypeBuilder<BackupProfile> builder)
        {
            builder
                .ToTable("backup_profile", "machine")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);
        }
    }

    public class RequestStatisticsConfiguration : EntityTypeConfigurationBase<RequestStatistics>
    {
        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>
        {
            { "MachineId", "machine" }
        };

        public override void Configure(EntityTypeBuilder<RequestStatistics> builder)
        {
            builder
                .ToTable("request_statistic", "machine")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);
        }
    }

    public class ConfigConfiguration : EntityTypeConfigurationBase<Config>
    {
        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>
        {
            { "MachineId", "machine" },
            { "Id", null },
        };

        public override void Configure(EntityTypeBuilder<Config> builder)
        {
            builder
                .ToTable("config", "machine")
                .HasKey(e => e.MachineId);

            builder
                .HasOne(x => x.Machine)
                .WithOne(x => x.Config)
                .HasForeignKey<Config>(x => x.MachineId)
                .IsRequired();

            AutoMapProperties(builder);
        }
    }
}