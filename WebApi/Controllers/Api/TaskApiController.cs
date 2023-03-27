using System.Threading.Tasks;
using AccountManager.Application.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebApi.Controllers.Api
{
    [Route(ApiVersion + "/tasks")]
    public class TaskApiController : AuthorizedApiController
    {
        private readonly ITaskManager _taskManager;

        public TaskApiController(IMediator mediator, ITaskManager taskManager) : base(mediator)
        {
            _taskManager = taskManager;
        }

        #region Commands

        [HttpDelete]
        [Route("{id}")]
        public async Task<Unit> Delete([FromRoute] string id)
        {
            return Unit.Value;
        }

        #endregion

        #region Queries

        #endregion
    }
}