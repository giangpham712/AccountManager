using System;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Identity;
using AccountManager.Application.Models.Dto;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace AccountManager.Application.Auth.Queries
{
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto>
    {
        private readonly IMapper _mapper;
        private readonly UserManager<LdapUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public GetCurrentUserQueryHandler(IHttpContextAccessor contextAccessor, UserManager<LdapUser> userManager, IMapper mapper)
        {
            _contextAccessor = contextAccessor;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var currentUserName = _contextAccessor.HttpContext.User?.Identity.Name;
            if (currentUserName == null)
            {
                return null;
            }

            var currentUser = await _userManager.FindByNameAsync(currentUserName);
            return _mapper.Map<UserDto>(currentUser);
        }
    }
}