using System;
using System.Collections.Generic;
using System.Linq;
using AccountManager.Application.Identity;
using AccountManager.Application.Models.Dto;
using AccountManager.Application.Models.Dto.Saas;
using AccountManager.Application.Tasks;
using AccountManager.Application.Utils;
using AccountManager.Common.Extensions;
using AccountManager.Domain;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Audit;
using AccountManager.Domain.Entities.Git;
using AccountManager.Domain.Entities.Library;
using AccountManager.Domain.Entities.Machine;
using AccountManager.Domain.Entities.Public;
using AutoMapper;
using Newtonsoft.Json;

namespace AccountManager.Application.Mappings
{
    public class DtoMappingProfile : ProfileBase
    {
        public DtoMappingProfile()
        {
            CreateMap<Account, AccountListingDto>()
                .ForMember(d => d.Class, o => o.MapFrom(src => src.Class != null ? src.Class.Name : string.Empty))
                .ForMember(d => d.ExpirationTime,
                    o => o.MapFrom(src =>
                        src.LicenseConfig == null ? default(DateTimeOffset) : src.LicenseConfig.ExpirationTime))
                .ForMember(d => d.CloudCredits,
                    o => o.MapFrom(src => src.LicenseConfig == null ? 0 : src.LicenseConfig.CloudCredits))
                .ForMember(d => d.BillingPeriod,
                    o => o.MapFrom(src => src.Billing == null ? string.Empty : src.Billing.Period.ToString()))
                .ForMember(d => d.InstancePolicy,
                    o => o.MapFrom(src =>
                        src.LicenseConfig == null
                            ? string.Empty
                            : src.LicenseConfig.InstancePolicy.ToString().ToSentenceCase()));

            CreateMap<Account, AccountDto>()
                .ForMember(d => d.BackupConfig, o => o.Condition(src => src.BackupConfig != null));

            CreateMap<Account, DraftAccountDto>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.BackupConfig, o => o.Condition(src => src.BackupConfig != null));

            CreateMap<Customer, CustomerDto>();

            CreateMap<Class, ClassDto>();

            CreateMap<Contact, ContactDto>();
            CreateMap<Billing, BillingDto>()
                .ForMember(d => d.Period, o => o.MapFrom(s => s.Period.ToString()));

            CreateMap<string, VersionInfo>().ConvertUsing<StringVersionInfoConverter>();

            CreateMap<MachineConfig, MachineConfigDto>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name ?? (s.Account == null ? null : s.Account.Name)))
                .ForMember(d => d.LauncherVersionMode,
                    opts => opts.MapFrom(s => MapVersionMode(s.LauncherVersionMode, s.LauncherVersion, s.IsTemplate)))
                .ForMember(d => d.ReportingVersionMode,
                    opts => opts.MapFrom(s => MapVersionMode(s.ReportingVersionMode, s.ReportingVersion, s.IsTemplate)))
                .ForMember(d => d.PdfExportVersionMode,
                    opts => opts.MapFrom(s => MapVersionMode(s.PdfExportVersionMode, s.PdfExportVersion, s.IsTemplate)))
                .ForMember(d => d.SiteMasterVersionMode,
                    opts => opts.MapFrom(
                        s => MapVersionMode(s.SiteMasterVersionMode, s.SiteMasterVersion, s.IsTemplate)))
                .ForMember(d => d.ClientVersionMode,
                    opts => opts.MapFrom(s => MapVersionMode(s.ClientVersionMode, s.ClientVersion, s.IsTemplate)))
                .ForMember(d => d.SqlExportVersionMode,
                    opts => opts.MapFrom(s => MapVersionMode(s.SqlExportVersionMode, s.SqlExportVersion, s.IsTemplate)))
                .ForMember(d => d.DeployerVersionMode,
                    opts => opts.MapFrom(s => MapVersionMode(s.DeployerVersionMode, s.DeployerVersion, s.IsTemplate)))
                .ForMember(d => d.PopulateVersionMode,
                    opts => opts.MapFrom(s => MapVersionMode(s.PopulateVersionMode, s.PopulateVersion, s.IsTemplate)))
                .ForMember(d => d.LinkwareVersionMode,
                    opts => opts.MapFrom(s => MapVersionMode(s.LinkwareVersionMode, s.LinkwareVersion, s.IsTemplate)))
                .ForMember(d => d.SmchkVersionMode,
                    opts => opts.MapFrom(s => MapVersionMode(s.SmchkVersionMode, s.SmchkVersion, s.IsTemplate)))
                .ForMember(d => d.DiscoveryVersionMode,
                    opts => opts.MapFrom(s => MapVersionMode(s.DiscoveryVersionMode, s.DiscoveryVersion, s.IsTemplate)))
                .ForMember(d => d.FiberSenSysVersionMode,
                    opts => opts.MapFrom(s =>
                        MapVersionMode(s.FiberSenSysVersionMode, s.FiberSenSysVersion, s.IsTemplate)))
                .ForMember(d => d.FiberMountainVersionMode,
                    opts => opts.MapFrom(s =>
                        MapVersionMode(s.FiberMountainVersionMode, s.FiberMountainVersion, s.IsTemplate)))
                .ForMember(d => d.ServiceNowVersionMode,
                    opts => opts.MapFrom(s =>
                        MapVersionMode(s.ServiceNowVersionMode, s.ServiceNowVersion, s.IsTemplate)))
                .ForMember(d => d.CommScopeVersionMode,
                    opts => opts.MapFrom(s =>
                        MapVersionMode(s.CommScopeVersionMode, s.CommScopeVersion, s.IsTemplate)))
                .ForMember(d => d.MainLibraryMode,
                    opts => opts.MapFrom(s =>
                        s.IsTemplate ? s.MainLibraryMode :
                        s.MainLibraryFiles != null && s.MainLibraryFiles.Any() ? "Select" : "None"))
                .ForMember(d => d.MainLibraryFileId, opts => opts.MapFrom(s => s.MainLibraryFile))
                .ForMember(d => d.AccountLibraryMode,
                    opts => opts.MapFrom(s =>
                        s.IsTemplate ? s.AccountLibraryMode : s.AccountLibraryFile.HasValue ? "Select" : "None"))
                .ForMember(d => d.AccountLibraryFileId, opts => opts.MapFrom(s => s.AccountLibraryFile))
                .ForMember(d => d.MainLibraryFileIds,
                    opts => opts.MapFrom(s => s.MainLibraryFiles))
                .ForMember(d => d.MainLibraryFile, opts => opts.Ignore())
                .ForMember(d => d.AccountLibraryFile, opts => opts.Ignore())
                .ForMember(d => d.MainLibraryFiles, opts => opts.Ignore());


            CreateMap<LicenseConfig, LicenseConfigDto>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name ?? (s.Account == null ? null : s.Account.Name)))
                .ForMember(d => d.InstancePolicy, o => o.MapFrom(s => s.InstancePolicy.ToString()))
                
                .BeforeMap((src, dest) => src.Features = src.Features.Select(x => x == "Remedy" ? "BMC" : x).ToArray());

            CreateMap<BackupConfig, BackupConfigDto>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name ?? (s.Account == null ? null : s.Account.Name)))
                .ForMember(d => d.Times, o => o.MapFrom(s => s.Times));

            CreateMap<Site, SiteDto>();

            CreateMap<CloudRegion, RegionDto>();
            CreateMap<CloudInstance, CloudInstanceDto>()
                .ForMember(dest => dest.MachineName, opts => opts.MapFrom(src => src.Machine.Name))
                .ForMember(dest => dest.MachineClassName, opts => opts.MapFrom(src => src.Machine.Class.Name));
            CreateMap<CloudInstanceType, CloudInstanceTypeDto>();
            CreateMap<RedisCloudInstanceType, RedisCloudInstanceTypeDto>();
            CreateMap<MmaInstance, MmaInstanceDto>();

            CreateMap<CloudBaseImage, CloudBaseImageDto>();

            CreateMap<Machine, MachineDto>()
                .ForMember(dest => dest.CloudInstanceTypeId, opts => opts.MapFrom(src => src.CloudInstanceTypeId));

            CreateMap<Config, ConfigDto>()
                .ForMember(d => d.ComponentConfig, opts => opts.MapFrom((src, dest) => Deserialize<Dictionary<string, object>>(src.ComponentConfigJson)))
                .ForMember(d => d.DeployerConfig, opts => opts.MapFrom((src, dest) => Deserialize<Dictionary<string, object>>(src.DeployerConfigJson)));

            CreateMap<ComponentConfig, ComponentConfigDto>()
                .ForMember(d => d.SaasDefaultValue, opts => opts.MapFrom((src, dest) => ConfigHelper.GetDataTypeValue(src.SaasDefaultValue, src.DataType)))
                .ForMember(d => d.OnPremDefaultValue, opts => opts.MapFrom((src, dest) => ConfigHelper.GetDataTypeValue(src.OnPremDefaultValue, src.DataType)))
                .ReverseMap();

            CreateMap<DeployerConfig, DeployerConfigDto>()
                .ForMember(d => d.DefaultValue, opts => opts.MapFrom((src, dest) => ConfigHelper.GetDataTypeValue(src.DefaultValue, src.DataType)))
                .ReverseMap();

            CreateMap<Message, MessageDto>();
            CreateMap<State, StateDto>()
                .ForMember(d => d.MachineName, opts => opts.MapFrom(s => s.Machine.Name))
                .ForMember(d => d.MachineClassName, opts => opts.MapFrom(s => s.Machine.Class.Name))
                .ForMember(d => d.LibraryFileIds, opts => opts.MapFrom(s => s.LibraryFiles))
                .ForMember(d => d.LibraryFileId, opts => opts.MapFrom(s => s.LibraryFile))
                .ForMember(d => d.AccountLibraryFileId, opts => opts.MapFrom(s => s.AccountLibraryFile))
                .ForMember(d => d.AccountLibraryFile, opts => opts.Ignore())
                .ForMember(d => d.LibraryFile, opts => opts.Ignore())
                .ForMember(d => d.LibraryFiles, opts => opts.Ignore());

            CreateMap<HistoricalDesiredState, StateDto>()
                .ForMember(d => d.LibraryFileIds, opts => opts.MapFrom(s => s.LibraryFiles))
                .ForMember(d => d.LibraryFileId, opts => opts.MapFrom(s => s.LibraryFile))
                .ForMember(d => d.AccountLibraryFileId, opts => opts.MapFrom(s => s.AccountLibraryFile))
                .ForMember(d => d.AccountLibraryFile, opts => opts.Ignore())
                .ForMember(d => d.LibraryFile, opts => opts.Ignore())
                .ForMember(d => d.LibraryFiles, opts => opts.Ignore());

            CreateMap<RequestStatistics, RequestStatisticsDto>()
                .ForMember(d => d.MachineName, opts => opts.MapFrom(s => s.Machine.Name));

            CreateMap<Commit, CommitDto>();
            CreateMap<Repo, RepoDto>();
            CreateMap<Branch, BranchDto>();

            CreateMap<File, FileDto>();
            CreateMap<Package, PackageDto>();

            CreateMap<IdleSchedule, IdleScheduleDto>();

            CreateMap<Operation, OperationDto>()
                .ForMember(d => d.MachineName, opts => opts.MapFrom(src => src.Machine.Name))
                .ForMember(d => d.MachineClassName, opts => opts.MapFrom(src => src.Machine.Class.Name))
                .ForMember(d => d.TypeName, opts => opts.MapFrom(src => src.TypeName));

            CreateMap<UserOperation, OperationDto>()
                .ForMember(d => d.FinishTime, opts => opts.MapFrom(src => src.Timestamp))
                .ForMember(d => d.TypeName, opts => opts.MapFrom(src => src.TypeName))
                .ForMember(d => d.OperationTypeName, opts => opts.MapFrom(src => src.Type.Name))
                .ForMember(d => d.OperationTypeDescription, opts => opts.MapFrom(src => src.Type.Description));

            CreateMap<OperationType, OperationTypeDto>();
            CreateMap<UserOperationType, OperationTypeDto>();

            CreateMap<Account, AccountRef>();
            CreateMap<Machine, MachineRef>();

            CreateMap<Account, SaasAccountDto>();

            CreateMap<ITask, AMTaskDto>()
                .ForMember(dest => dest.Type, opts => opts.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Status, opts => opts.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Description, opts => opts.MapFrom<AMTaskDescriptionResolver>());

            CreateMap<LdapUser, UserDto>();

            CreateMap<LauncherUser, LauncherLoginDto>()
                .ForMember(dest => dest.UserName, opts => opts.MapFrom(src => src.Un))
                .ForMember(dest => dest.FirstName, opts => opts.MapFrom(src => src.Fn))
                .ForMember(dest => dest.LastName, opts => opts.MapFrom(src => src.Ln))
                .ForMember(dest => dest.Password, opts => opts.MapFrom(src => src.Pw));
        }

        private string MapVersionMode(string versionMode, string hash, bool isTemplate)
        {
            return isTemplate ? versionMode : new VersionInfo(hash).IsNone() ? "None" : "Select";
        }
    }

    public class AMTaskDescriptionResolver : IValueResolver<ITask, AMTaskDto, string>
    {
        public string Resolve(ITask source, AMTaskDto destination, string destMember, ResolutionContext context)
        {
            switch (source.Type)
            {
                case TaskType.StartMachine:
                    return "Starting";
                case TaskType.StopMachine:
                    return "Stopping";
                case TaskType.TerminateMachine:
                    return "Terminating";
                case TaskType.BackupMachine:
                    return "Backing up";
                case TaskType.RestoreMachineBackup:
                    return "Restoring backup";
                case TaskType.ChangeMachineInstanceType:
                    return "Updating instance type";
                default:
                    return string.Empty;
            }
        }
    }
}