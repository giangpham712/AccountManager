using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AccountManager.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountManager.Persistence.PostgreSql.Configurations
{
    public abstract class EntityTypeConfigurationBase<T> : IEntityTypeConfiguration<T> where T : class
    {
        private static readonly HashSet<Type> MappableTypes = new HashSet<Type>
        {
            typeof(string), typeof(string[]), 
            typeof(int[]), typeof(long[]), 
            typeof(bool), 
            typeof(DateTime), typeof(DateTimeOffset), typeof(DateTimeOffset[]), 
            typeof(byte[])
        };

        protected virtual IDictionary<string, string> ColumnMappings => new Dictionary<string, string>();

        protected void AutoMapProperties(EntityTypeBuilder<T> builder)
        {
            foreach (var property in typeof(T).GetProperties().Where(p => ShouldMap(p.PropertyType)))
            {
                if (!ColumnMappings.TryGetValue(property.Name, out var columnName))
                {
                    columnName = property.Name.ToUnderscoreCase();
                }

                if (columnName == null)
                {
                    builder.Ignore(property.Name);
                }
                else
                {
                    builder.Property(property.Name).HasColumnName(columnName);
                }
            }
        }

        private bool ShouldMap(Type type)
        {
            var typeToCheck = Nullable.GetUnderlyingType(type) ?? type;
            return typeToCheck.IsPrimitive ||
                   typeToCheck.IsEnum ||
                   MappableTypes.Contains(typeToCheck);
        }

        public abstract void Configure(EntityTypeBuilder<T> builder);
    }
}