using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Public.Queries.GetComponentConfigByKey
{
    public class GetComponentConfigByKeyQuery : IRequest<ComponentConfigDto>
    {
        public string Key { get; set; }
    }
}
