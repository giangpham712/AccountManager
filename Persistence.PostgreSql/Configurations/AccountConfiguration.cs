using System.Collections.Generic;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountManager.Persistence.PostgreSql.Configurations
{
    public class AccountConfiguration : EntityTypeConfigurationBase<Account>
    {
        public override void Configure(EntityTypeBuilder<Account> builder)
        {
            builder
                .ToTable("account", "account")
                .HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("id");

            builder.Property(e => e.Name).HasColumnName("name");
            builder.Property(e => e.Description).HasColumnName("description");
            builder.Property(e => e.UrlFriendlyName).HasColumnName("url_friendly_name");
            builder.Property(e => e.LauncherUrl).HasColumnName("launcher_url");
            builder.Property(e => e.License).HasColumnName("license");

            builder.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            builder.Property(e => e.IsActive).HasColumnName("is_active");
            builder.Property(e => e.Managed).HasColumnName("managed");
            builder.Property(e => e.AutoTest).HasColumnName("auto_test");
            builder.Property(e => e.AutoTestBranch).HasColumnName("auto_test_branch");
            builder.Property(e => e.WhiteGlove).HasColumnName("white_glove");

            builder.Property(e => e.IsTemplate).HasColumnName("is_template");
            builder.Property(e => e.IsPublic).HasColumnName("is_public");
            builder.Property(e => e.Customer).HasColumnName("customer");

            builder.Property(e => e.ClassId).HasColumnName("class");
            builder.Property(e => e.ParentId).HasColumnName("parent");

            builder.Property(e => e.Creator).HasColumnName("creator");
            builder.Property(e => e.CreationTime).HasColumnName("creation_time");

            builder.Property(e => e.LastUserCycle).HasColumnName("last_user_cycle");

            builder.HasOne(e => e.Class)
                .WithMany()
                .HasForeignKey(e => e.ClassId);

            builder.HasOne(e => e.Parent)
                .WithMany()
                .HasForeignKey(e => e.ParentId);
        }
    }

    public class ContactConfiguration : EntityTypeConfigurationBase<Contact>
    {
        protected override IDictionary<string, string> ColumnMappings { get; } = new Dictionary<string, string>
        {
        };

        public override void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder
                .ToTable("contact", "account")
                .HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.AccountId).HasColumnName("account");
            builder.Property(e => e.Name).HasColumnName("name");
            builder.Property(e => e.Phone1).HasColumnName("phone_1");
            builder.Property(e => e.Phone2).HasColumnName("phone_2");
            builder.Property(e => e.Email).HasColumnName("email");


            builder.HasOne(e => e.Account)
                .WithOne(e => e.Contact)
                .HasForeignKey<Contact>(e => e.AccountId)
                .IsRequired();
        }
    }

    public class LicenseConfigConfiguration : EntityTypeConfigurationBase<LicenseConfig>
    {
        protected override IDictionary<string, string> ColumnMappings { get; } = new Dictionary<string, string>
        {
            { nameof(LicenseConfig.AccountId), "account" },
        };

        public override void Configure(EntityTypeBuilder<LicenseConfig> builder)
        {
            builder
                .ToTable("license_config", "account")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);

            builder.Property(x => x.Features).HasColumnType("jsonb");
            builder.Property(x => x.ReportingCategories).HasColumnType("jsonb").UsePropertyAccessMode(PropertyAccessMode.Property);

            builder.HasOne(e => e.Account)
                .WithOne(e => e.LicenseConfig)
                .HasForeignKey<LicenseConfig>(e => e.AccountId);
        }
    }

    public class MachineConfigConfiguration : EntityTypeConfigurationBase<MachineConfig>
    {
        public override void Configure(EntityTypeBuilder<MachineConfig> builder)
        {
            builder
                .ToTable("machine_config", "account")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);

            builder.HasOne(e => e.Account)
                .WithOne(e => e.MachineConfig)
                .HasForeignKey<MachineConfig>(e => e.AccountId);
        }

        protected override IDictionary<string, string> ColumnMappings { get; } = new Dictionary<string, string>
        {
            { nameof(MachineConfig.AccountId), "account" },
            { nameof(MachineConfig.SiteMasterVersionMode), "sitemaster_version_mode" },
            { nameof(MachineConfig.SiteMasterVersion), "sitemaster_version" },
            { nameof(MachineConfig.MainLibraryFile), "main_library_file" },
            { nameof(MachineConfig.MainLibraryFiles), "main_library_files" },
            { nameof(MachineConfig.AccountLibraryFile), "account_library_file" },
            { nameof(MachineConfig.SqlExportVersionMode), "sqlexport_version_mode" },
            { nameof(MachineConfig.SqlExportVersion), "sqlexport_version" },
            { nameof(MachineConfig.PdfExportVersion), "pdfexport_version" },
            { nameof(MachineConfig.FiberSenSysVersion), "fibersensys_version" },
            { nameof(MachineConfig.FiberSenSysVersionMode), "fibersensys_version_mode" },
            { nameof(MachineConfig.FiberMountainVersion), "fibermountain_version" },
            { nameof(MachineConfig.FiberMountainVersionMode), "fibermountain_version_mode" },
            { nameof(MachineConfig.ServiceNowVersion), "servicenow_version" },
            { nameof(MachineConfig.ServiceNowVersionMode), "servicenow_version_mode" },
            { nameof(MachineConfig.CommScopeVersion), "commscope_version" },
            { nameof(MachineConfig.CommScopeVersionMode), "commscope_version_mode" }
        };
    }

    public class BillingConfiguration : EntityTypeConfigurationBase<Billing>
    {
        protected override IDictionary<string, string> ColumnMappings { get; } = new Dictionary<string, string>
        {
            { nameof(Billing.AccountId), "account" },
        };

        public override void Configure(EntityTypeBuilder<Billing> builder)
        {
            builder
                .ToTable("billing", "account")
                .HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.AccountId).HasColumnName("account");
            builder.Property(e => e.Amount).HasColumnName("amount");
            builder.Property(e => e.Period).HasColumnName("period");

            builder.HasOne(e => e.Account)
                .WithOne(e => e.Billing)
                .HasForeignKey<Billing>(e => e.AccountId);
        }
    }

    public class KeysConfiguration : EntityTypeConfigurationBase<Keys>
    {
        protected override IDictionary<string, string> ColumnMappings { get; } = new Dictionary<string, string>
        {
            { nameof(Keys.AccountId), "account" },
            { nameof(Keys.SqlExportPass), "sqlexport_pass" }
        };

        public override void Configure(EntityTypeBuilder<Keys> builder)
        {
            builder
                .ToTable("keys", "account")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);

            builder.HasOne(e => e.Account)
                .WithOne(e => e.Keys)
                .HasForeignKey<Keys>(e => e.AccountId);
        }
    }

    public class SiteConfiguration : EntityTypeConfigurationBase<Site>
    {
        protected override IDictionary<string, string> ColumnMappings { get; } = new Dictionary<string, string>
        {
            { nameof(Site.AccountId), "account" },
            { nameof(Site.MachineId), "machine" },
        };

        public override void Configure(EntityTypeBuilder<Site> builder)
        {
            builder
                .ToTable("site", "account")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);

            builder.HasOne(e => e.Account)
                .WithMany(e => e.Sites)
                .HasForeignKey(x => x.AccountId)
                .IsRequired();

            builder.HasOne(e => e.Machine)
                .WithMany()
                .HasForeignKey(x => x.MachineId);
        }
    }

    public class BackupConfigConfiguration : EntityTypeConfigurationBase<BackupConfig>
    {
        protected override IDictionary<string, string> ColumnMappings { get; } = new Dictionary<string, string>
        {
            { nameof(BackupConfig.AccountId), "account" }
        };

        public override void Configure(EntityTypeBuilder<BackupConfig> builder)
        {
            builder
                .ToTable("backup_config", "account")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);

            builder.HasOne(e => e.Account)
                .WithOne(e => e.BackupConfig)
                .HasForeignKey<BackupConfig>(e => e.AccountId);
        }
    }

    public class IdleScheduleConfiguration : EntityTypeConfigurationBase<IdleSchedule>
    {
        protected override IDictionary<string, string> ColumnMappings { get; } = new Dictionary<string, string>
        {
            { nameof(IdleSchedule.AccountId), "account" }
        };

        public override void Configure(EntityTypeBuilder<IdleSchedule> builder)
        {
            builder
                .ToTable("idle_schedule", "account")
                .HasKey(e => e.Id);

            AutoMapProperties(builder);

            builder.HasOne(e => e.Account)
                .WithMany(e => e.IdleSchedules)
                .HasForeignKey(a => a.AccountId)
                .IsRequired();
        }
    }
}