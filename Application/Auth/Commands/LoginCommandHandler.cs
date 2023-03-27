using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Identity;
using AccountManager.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AccountManager.Application.Auth.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginCommandResult>
    {
        private readonly UserManager<LdapUser> _userManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly ITokenFactory _tokenFactory;

        public LoginCommandHandler(
            IJwtFactory jwtFactory, 
            ITokenFactory tokenFactory, 
            UserManager<LdapUser> userManager)
        {
            _jwtFactory = jwtFactory;
            _tokenFactory = tokenFactory;
            _userManager = userManager;
        }

        public async Task<LoginCommandResult> Handle(LoginCommand command, CancellationToken cancellationToken)
        {

            var user = await _userManager.FindByNameAsync(command.Username);
            if (await _userManager.CheckPasswordAsync(user, command.Password))
            {
                var refreshToken = _tokenFactory.GenerateToken();
                return new LoginCommandResult(
                    await _jwtFactory.GenerateEncodedToken(user.ProviderUserKey as string, user.UserName,
                        user.Permissions),
                    refreshToken, true);
            }

            return new LoginCommandResult(new[] { new Error("login_failure", "Invalid username or password.") });
        }
    }
}