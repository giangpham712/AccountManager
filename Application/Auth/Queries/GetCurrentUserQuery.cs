using System.Collections.Generic;
using System.Text;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Auth.Queries
{
    public class GetCurrentUserQuery : IRequest<UserDto>
    {
    }
}
