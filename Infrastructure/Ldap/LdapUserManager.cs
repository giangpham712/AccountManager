using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Application.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Ldap
{
    public class LdapUserManager : UserManager<LdapUser>
    {
        private const int LdapErrorInvalidCredentials = 0x31;

        private readonly LdapConfiguration _configuration;


        public LdapUserManager(
            IUserStore<LdapUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<LdapUser> passwordHasher,
            IEnumerable<IUserValidator<LdapUser>> userValidators,
            IEnumerable<IPasswordValidator<LdapUser>> passwordValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
            IServiceProvider services, ILogger<UserManager<LdapUser>> logger,
            LdapConfiguration configuration) :
            base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _configuration = configuration;
        }

        public override async Task<bool> CheckPasswordAsync(LdapUser user, string password)
        {
            try
            {
                var ldapDirectoryIdentifier = new LdapDirectoryIdentifier(_configuration.ServerUrl, _configuration.Port);
                var credentials = new NetworkCredential($"uid={user.UserName},{_configuration.BaseDn}", password);
                using (var ldapConnection = new LdapConnection(ldapDirectoryIdentifier, credentials, AuthType.Basic))
                {
                    ldapConnection.SessionOptions.SecureSocketLayer = false;
                    ldapConnection.SessionOptions.ProtocolVersion = 3;
                    ldapConnection.Bind();
                }

                return await Task.FromResult(true);
            }
            catch (LdapException ldapException)
            {
                if (ldapException.ErrorCode.Equals(LdapErrorInvalidCredentials)) return false;
                throw;
            }
        }
    }
}
