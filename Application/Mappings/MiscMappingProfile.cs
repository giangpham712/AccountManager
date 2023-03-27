using System;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Machine;
using AccountManager.Domain.Security.Licensing;
using Newtonsoft.Json;

namespace AccountManager.Application.Mappings
{
    public class MiscMappingProfile : ProfileBase
    {
        public MiscMappingProfile()
        {
            CreateMap<State, HistoricalDesiredState>();

            CreateMap<LicenseConfig, IrmLicense>()
                .ForMember(dest => dest.Id, opts => opts.Ignore())
                .ForMember(dest => dest.MaxAreaCount, opts => opts.MapFrom(src => src.MaxAreas))
                .ForMember(dest => dest.MaxEquipCount, opts => opts.MapFrom(src => src.MaxEquips))
                .ForMember(dest => dest.MaxCircuitCount, opts => opts.MapFrom(src => src.MaxCircuits))
                .ForMember(dest => dest.MaxSiteCount, opts => opts.MapFrom(src => src.MaxSites))
                .ForMember(dest => dest.MaxSoftwareCount, opts => opts.MapFrom(src => src.MaxSoftwares))
                .ForMember(dest => dest.MaxMainHoleCount, opts => opts.MapFrom(src => src.MaxMainholes))
                .ForMember(dest => dest.MaxUserCount, opts => opts.MapFrom(src => src.MaxUsers))
                .ForMember(dest => dest.MaxPathwayCount, opts => opts.MapFrom(src => src.MaxPathways))
                .ForMember(dest => dest.MaxFaceplateCount, opts => opts.MapFrom(src => src.MaxFaceplates))
                .ForMember(dest => dest.MaxRackCount, opts => opts.MapFrom(src => src.MaxRacks))
                .ForMember(dest => dest.MaxCloudInstanceCredits, opts => opts.MapFrom(src => src.CloudCredits))
                .ForMember(dest => dest.Expiration, opts => opts.MapFrom(src => LicenseExpiration(src)))
                .ForMember(dest => dest.ReportCategories, opts => opts.MapFrom(src => src.ReportingCategories));
        }

        private DateTime LicenseExpiration(LicenseConfig licenseConfig)
        {
            return licenseConfig.ExpirationTime?.LocalDateTime ?? DateTime.MaxValue;
        }
    }
}