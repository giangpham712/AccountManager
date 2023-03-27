using System;
using AccountManager.Domain;
using AutoMapper;

namespace AccountManager.Application.Mappings
{
    public class StringVersionInfoConverter : ITypeConverter<string, VersionInfo>
    {
        private readonly IServiceProvider _serviceProvider;

        public StringVersionInfoConverter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public VersionInfo Convert(string source, VersionInfo destination, ResolutionContext context)
        {
            return new VersionInfo(source);
        }
    }
}