using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Ldap
{
    public class LdapUserStore : IUserStore<LdapUser>
    {
        private readonly LdapConfiguration _configuration;

        private static readonly IDictionary<string, string[]> EmployeeTypePermissions = new Dictionary<string, string[]>
        {
            { "readonly", new[] { "read" } }
        };

        public LdapUserStore(LdapConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Dispose()
        {
        }

        public Task<string> GetUserIdAsync(LdapUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserNameAsync(LdapUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(LdapUser user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(LdapUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(LdapUser user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> CreateAsync(LdapUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(LdapUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(LdapUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<LdapUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<LdapUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var connection = GetLdapConnection();

            var distinguishedName = $"uid={normalizedUserName},{_configuration.BaseDn}";
            var searchRequest = new SearchRequest
            {
                DistinguishedName = distinguishedName
            };

            var response = connection.SendRequest(searchRequest) as SearchResponse;

            if (response == null || response.Entries.Count == 0) return null;

            var entry = response.Entries[0];

            var employeeTypes = GetEntryAttributeValue(entry, "employeeType")
                ?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var user = new LdapUser()
            {
                Name = GetEntryAttributeValue(entry, "cn"),
                Permissions = GetPermissions(employeeTypes),
                ProviderUserKey = GetEntryAttributeValue(entry, "uidnumber"),
                Email = GetEntryAttributeValue(entry, "mail"),
                UserName = normalizedUserName
            };

            return await Task.FromResult(user);
        }

        private LdapConnection GetLdapConnection()
        {
            var ldapDirectoryIdentifier = new LdapDirectoryIdentifier(_configuration.ServerUrl, _configuration.Port);

            var connection = new LdapConnection(ldapDirectoryIdentifier);

            connection.SessionOptions.SecureSocketLayer = _configuration.EnableSsl;
            connection.SessionOptions.ProtocolVersion = 3;
            connection.AuthType = AuthType.Basic;
            connection.Credential = new NetworkCredential(_configuration.Admin, _configuration.AdminPassword);

            return connection;
        }

        private static string GetEntryAttributeValue(SearchResultEntry entry, string attributeName, string separator = null)
        {
            if (entry.Attributes[attributeName] == null)
                return null;

            var values = new List<string>();
            for (var index = 0; index < entry.Attributes[attributeName].Count; index++)
                values.Add(entry.Attributes[attributeName][index].ToString());

            return string.Join(separator ?? ",", values);
        }

        private static string[] GetPermissions(string[] employeeTypes)
        {
            if (employeeTypes == null || !employeeTypes.Any()) return new[] { "write" };

            var allPermissions = new List<string>();
            foreach (var employeeType in employeeTypes)
            {
                if (!EmployeeTypePermissions.TryGetValue(employeeType.Trim().ToLower(), out var permissions)) continue;
                allPermissions.AddRange(permissions);
            }

            return allPermissions.Distinct().ToArray();
        }
    }
}