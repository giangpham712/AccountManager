using System;
using System.Collections.Generic;
using System.Text;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Public.Queries.GetComponentConfig
{
    public class GetComponentConfigQuery : IRequest<ComponentConfigDto>
    {
        public long Id { get; set; }
    }
}
