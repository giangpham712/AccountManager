using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Application.Notification.Commands.SendMessage;
using AccountManager.Application.Notification.Queries.ListPendingMessage;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebApi.Controllers.Api
{
    [Route(ApiVersion + "/notification")]
    public class NotificationApiController : AuthorizedApiController
    {
        private readonly IMapper _mapper;
        public NotificationApiController(IMediator mediator, IMapper mapper) : base(mediator)
        {
            _mapper = mapper;
        }

        [HttpGet]
        [Route("messages")]
        public async Task<IEnumerable<MessageDto>> ListPendingMessages()
        {
            var messages = await Mediator.Send(new ListPendingMessagesQuery());

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        [HttpPost]
        [Route("send")]
        public async Task<Unit> SendMessage([FromBody] SendMessageCommand command)
        {
            return await Mediator.Send(command);
        }
    }
}