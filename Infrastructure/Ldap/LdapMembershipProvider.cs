using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using AccountManager.Domain;

namespace Infrastructure.Ldap
{
    public class CustomLdapMembershipProvider : MembershipProvider
    {
        private const int LdapErrorInvalidCredentials = 0x31;

        private static readonly IDictionary<string, string[]> employeeTypePermissions = new Dictionary<string, string[]>
        {
            { "readonly", new[] { "read" } }
        };

        private bool _enableSsl;
        private string _ldapAdmin;
        private string _ldapAdminPassword;
        private string _ldapBaseDn;
        private int _ldapPort;

        private string _ldapServerUrl;

        public override bool EnablePasswordRetrieval { get; }
        public override bool EnablePasswordReset { get; }
        public override bool RequiresQuestionAndAnswer { get; }
        public override string ApplicationName { get; set; }
        public override int MaxInvalidPasswordAttempts { get; }
        public override int PasswordAttemptWindow { get; }
        public override bool RequiresUniqueEmail { get; }
        public override MembershipPasswordFormat PasswordFormat { get; }
        public override int MinRequiredPasswordLength { get; }
        public override int MinRequiredNonAlphanumericCharacters { get; }
        public override string PasswordStrengthRegularExpression { get; }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            _ldapServerUrl = GetConfigValue(config["ldapServerUrl"], null);
            _ldapBaseDn = GetConfigValue(config["ldapBaseDn"], null);
            _ldapPort = Convert.ToInt32(GetConfigValue(config["ldapPort"], "389"));
            _enableSsl = Convert.ToBoolean(GetConfigValue(config["ldapUseSsl"], "false"));

            _ldapAdmin = GetConfigValue(config["ldapAdmin"], null);
            _ldapAdminPassword = GetConfigValue(config["ldapAdminPassword"], null);
        }

        public override MembershipUser CreateUser(string username, string password, string email,
            string passwordQuestion, string passwordAnswer,
            bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password,
            string newPasswordQuestion,
            string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            try
            {
                var ldapDirectoryIdentifier = new LdapDirectoryIdentifier(_ldapServerUrl, _ldapPort);
                var credentials = new NetworkCredential(string.Format("uid={0},{1}", username, _ldapBaseDn), password);
                using (var ldapConnection = new LdapConnection(ldapDirectoryIdentifier, credentials, AuthType.Basic))
                {
                    ldapConnection.SessionOptions.SecureSocketLayer = false;
                    ldapConnection.SessionOptions.ProtocolVersion = 3;
                    ldapConnection.Bind();
                }

                return true;
            }
            catch (LdapException ldapException)
            {
                if (ldapException.ErrorCode.Equals(LdapErrorInvalidCredentials)) return false;
                throw;
            }
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            var connection = GetLdapConnection();

            var distinguishedName = $"uid={username},{_ldapBaseDn}";
            var searchRequest = new SearchRequest
            {
                DistinguishedName = distinguishedName
            };

            var response = connection.SendRequest(searchRequest) as SearchResponse;

            if (response == null || response.Entries.Count == 0) return null;

            var entry = response.Entries[0];

            var employeeTypes = GetEntryAttributeValue(entry, "employeeType")
                ?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var user = new LdapMembershipUser(
                Name,
                username,
                GetEntryAttributeValue(entry, "uidnumber"),
                GetEntryAttributeValue(entry, "mail"),
                null,
                true,
                false,
                DateTime.MinValue,
                DateTime.Now,
                DateTime.Now,
                DateTime.MinValue,
                DateTime.MinValue)
            {
                Name = GetEntryAttributeValue(entry, "cn"),
                Permissions = GetPermissions(employeeTypes)
            };

            return user;
        }

        private string[] GetPermissions(string[] employeeTypes)
        {
            if (employeeTypes == null || !employeeTypes.Any()) return new[] { "write" };

            var allPermissions = new List<string>();
            foreach (var employeeType in employeeTypes)
            {
                string[] permissions;
                if (!employeeTypePermissions.TryGetValue(employeeType.Trim().ToLower(), out permissions)) continue;
                allPermissions.AddRange(permissions);
            }

            return allPermissions.Distinct().ToArray();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize,
            out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize,
            out int totalRecords)
        {
            throw new NotImplementedException();
        }

        private LdapConnection GetLdapConnection()
        {
            var ldapDirectoryIdentifier = new LdapDirectoryIdentifier(_ldapServerUrl, _ldapPort);

            var connection = new LdapConnection(ldapDirectoryIdentifier);

            connection.SessionOptions.SecureSocketLayer = _enableSsl;
            connection.SessionOptions.ProtocolVersion = 3;
            connection.AuthType = AuthType.Basic;
            connection.Credential = new NetworkCredential(_ldapAdmin, _ldapAdminPassword);

            return connection;
        }

        private string GetEntryAttributeValue(SearchResultEntry entry, string attributeName, string separator = null)
        {
            if (entry.Attributes[attributeName] == null)
                return null;

            var values = new List<string>();
            for (var index = 0; index < entry.Attributes[attributeName].Count; index++)
                values.Add(entry.Attributes[attributeName][index].ToString());

            return string.Join(separator ?? ",", values);
        }

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (string.IsNullOrEmpty(configValue)) return defaultValue;

            return configValue;
        }
    }

    public class LdapMembershipUser : MembershipUser, IAccountManagerMembershipUser
    {
        public LdapMembershipUser(
            string providerName,
            string userName,
            string providerUserKey,
            string email,
            string comment,
            bool isApproved,
            bool isLockedOut,
            DateTime creationDate,
            DateTime lastLoginDate,
            DateTime lastActivityDate,
            DateTime lastPasswordChangedDate,
            DateTime lastLockoutDate) :
            base(
                providerName,
                userName,
                providerUserKey,
                email,
                null,
                comment,
                isApproved,
                isLockedOut,
                creationDate,
                lastLoginDate,
                lastActivityDate,
                lastPasswordChangedDate,
                lastLockoutDate)
        {
        }

        public string Name { get; set; }
        public string[] Permissions { get; set; }
    }
}