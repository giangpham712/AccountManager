using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.Application;
using AccountManager.Application.Accounts.Commands.BatchUpdateMachines;
using AccountManager.Application.Accounts.Commands.RecreateMachine;
using AccountManager.Application.Accounts.Commands.ResetLastOperation;
using AccountManager.Application.Accounts.Commands.ResetMachineFailureState;
using AccountManager.Application.Accounts.Commands.StartMachine;
using AccountManager.Application.Accounts.Commands.StopMachine;
using AccountManager.Application.Accounts.Commands.TerminateMachine;
using AccountManager.Application.Accounts.Commands.UpdateMachine;
using AccountManager.Application.Machines.Commands.ChangeInstanceTypeForMachine;
using AccountManager.Application.Machines.Commands.ForceBackupForMachine;
using AccountManager.Application.Machines.Commands.ForcePopulateForMachine;
using AccountManager.Application.Machines.Commands.ForceSmokeTestForMachine;
using AccountManager.Application.Machines.Commands.PublishAsSampleData;
using AccountManager.Application.Machines.Commands.QueueOperations;
using AccountManager.Application.Machines.Commands.RestoreBackupForMachine;
using AccountManager.Application.Machines.Commands.UpdateComponentConfigForMachine;
using AccountManager.Application.Machines.Commands.UpdateDeployerConfigForMachine;
using AccountManager.Application.Machines.Commands.UpdateOperation;
using AccountManager.Application.Machines.Queries.GetAllCloudInstancesForMachine;
using AccountManager.Application.Machines.Queries.GetAllMachines;
using AccountManager.Application.Machines.Queries.GetAllOperationsForMachine;
using AccountManager.Application.Machines.Queries.GetAllTasksForMachine;
using AccountManager.Application.Machines.Queries.GetDefaultComponentConfigForMachine;
using AccountManager.Application.Machines.Queries.GetDefaultDeployerConfigForMachine;
using AccountManager.Application.Machines.Queries.GetDesiredStateForMachine;
using AccountManager.Application.Machines.Queries.GetMachineStats;
using AccountManager.Application.Machines.Queries.GetRequestStatisticsForMachine;
using AccountManager.Application.Machines.Queries.GetSiteServerStatusForMachine;
using AccountManager.Application.Machines.Queries.GetSoftwareInfoForMachine;
using AccountManager.Application.Machines.Queries.GetStatesForMachine;
using AccountManager.Application.Models.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebApi.Controllers.Api
{
    [Route(ApiVersion + "/machines")]
    public class MachineApiController : AuthorizedApiController
    {
        public MachineApiController(IMediator mediator) : base(mediator)
        {
        }

        #region Queries

        [HttpGet]
        [Route("")]
        public async Task<IEnumerable<MachineDto>> GetAll()
        {
            return await Mediator.Send(new GetAllMachinesQuery());
        }

        [HttpGet]
        [Route("stats")]
        public async Task<MachineStatsDto> GetStats()
        {
            return await Mediator.Send(new GetMachineStatsQuery());
        }

        [HttpGet]
        [Route("{id}/state-info")]
        public async Task<MachineStateInfoDto> GetSoftwareInfoForMachine([FromRoute] long id)
        {
            var query = new GetSoftwareInfoForMachineQuery { Id = id };
            return await Mediator.Send(query);
        }

        [HttpGet]
        [Route("{id}/site-server-status")]
        public async Task<SiteServerStatusDto> GetSiteServerStatusForMachine(long id)
        {
            return await Mediator.Send(new GetSiteServerStatusForMachineQuery { Id = id });
        }

        [HttpGet]
        [Route("{id}/operations")]
        public async Task<PagedResult<OperationDto>> GetOperations(
            [FromRoute] long id, 
            [FromQuery] int startIndex = 0,
            [FromQuery] int limit = 20)
        {
            var query = new GetAllOperationsForMachineQuery { Id = id, StartIndex = startIndex, Limit = limit };
            return await Mediator.Send(query);
        }

        [HttpGet]
        [Route("{id}/cloud-instances")]
        public async Task<PagedResult<CloudInstanceDto>> GetCloudInstances(
            [FromRoute] long id,
            [FromQuery] int startIndex = 0,
            [FromQuery] int limit = 20)
        {
            return await Mediator.Send(new GetAllCloudInstancesForMachineQuery() { Id = id, StartIndex = startIndex, Limit = limit });
        }

        [HttpGet]
        [Route("{id}/tasks")]
        public async Task<IEnumerable<AMTaskDto>> GetTasks([FromRoute] long id)
        {
            var query = new GetAllTasksForMachineQuery { Id = id };
            return await Mediator.Send(query);
        }

        [HttpGet]
        [Route("{id}/states")]
        public async Task<PagedResult<StateDto>> GetStates(
            [FromRoute] long id,
            [FromQuery] bool desired = false,
            [FromQuery] bool current = false,
            [FromQuery] int startIndex = 0,
            [FromQuery] int limit = 20)
        {
            var query = new GetStatesForMachineQuery() { Id = id, Desired = desired, Current = current, StartIndex = startIndex, Limit = limit};
            return await Mediator.Send(query);
        }

        [HttpGet]
        [Route("{id}/states/desired")]
        public async Task<StateDto> GetDesiredState([FromRoute] long id)
        {
            var query = new GetDesiredStateForMachineQuery { Id = id };
            return await Mediator.Send(query);
        }

        [HttpGet]
        [Route("{id}/request-statistics")]
        public async Task<PagedResult<RequestStatisticsDto>> GetRequestStatistics(
            [FromRoute] long id,
            [FromQuery] int startIndex = 0,
            [FromQuery] int limit = 20)
        {
            var query = new GetRequestStatisticsForMachineQuery() { Id = id, StartIndex = startIndex, Limit = limit };
            return await Mediator.Send(query);
        }

        [HttpGet]
        [Route("{id}/default-component-config")]
        public async Task<ConfigDto> GetDefaultComponentConfig([FromRoute] long id)
        {
            return await Mediator.Send(new GetDefaultComponentConfigForMachineQuery() { Id = id });
        }

        [HttpGet]
        [Route("{id}/default-deployer-config")]
        public async Task<ConfigDto> GetDefaultDeployerConfig([FromRoute] long id)
        {
            return await Mediator.Send(new GetDefaultDeployerConfigForMachineQuery() { Id = id });
        }

        #endregion

        #region Commands

        [HttpPatch]
        [Route("{id}")]
        public async Task<Unit> Update([FromRoute] long id, [FromBody] UpdateMachineCommand command)
        {
            command.MachineId = id;

            return await Mediator.Send(command);
        }

        [HttpPost]
        [Route("{id}/start")]
        public async Task<Unit> Start([FromRoute] long id)
        {
            return await Mediator.Send(new StartMachineCommand { Id = id });
        }

        [HttpPost]
        [Route("{id}/stop")]
        public async Task<Unit> Stop([FromRoute] long id)
        {
            return await Mediator.Send(new StopMachineCommand { Id = id });
        }

        [HttpPost]
        [Route("{id}/terminate")]
        public async Task<Unit> Terminate([FromRoute] long id)
        {
            return await Mediator.Send(new TerminateMachineCommand { Id = id });
        }

        [HttpPost]
        [Route("{id}/reset-failure-state")]
        public async Task<Unit> ResetFailureState([FromRoute] long id)
        {
            return await Mediator.Send(new ResetMachineFailureStateCommand { Id = id });
        }

        [HttpPost]
        [Route("{id}/reset-last-operation")]
        public async Task<Unit> ResetLastOperation([FromRoute] long id)
        {
            return await Mediator.Send(new ResetLastOperationCommand { Id = id });
        }

        [HttpPost]
        [Route("{id:long}/recreate")]
        public async Task<Unit> Recreate([FromRoute] long id)
        {
            return await Mediator.Send(new RecreateMachineCommand { MachineId = id });
        }

        [HttpPost]
        [Route("{id:long}/force-backup")]
        public async Task<Unit> ForceBackup([FromRoute] long id, [FromBody] ForceBackupForMachineCommand command)
        {
            command.Id = id;
            return await Mediator.Send(command);
        }

        [HttpPost]
        [Route("{id:long}/force-populate")]
        public async Task<Unit> ForcePopulate([FromRoute] long id,
            [FromBody] ForcePopulateForMachineCommand command)
        {
            command.Id = id;
            return await Mediator.Send(command);
        }

        [HttpPost]
        [Route("{id}/restore-backup")]
        public async Task<Unit> RestoreBackup([FromRoute] long id,
            [FromBody] RestoreBackupForMachineCommand command)
        {
            command.Id = id;
            return await Mediator.Send(command);
        }

        [HttpPost]
        [Route("{id}/queue-operations")]
        public async Task<Unit> QueueOperations([FromRoute] long id,
            [FromBody] QueueOperationsCommand command)
        {
            command.Id = id;
            return await Mediator.Send(command);
        }

        [HttpPatch]
        [Route("{id}/operations/{operationId}")]
        public async Task<Unit> QueueOperations([FromRoute] long id, [FromRoute] long operationId,
            [FromBody] UpdateOperationCommand command)
        {
            command.MachineId = id;
            command.OperationId = operationId;
            return await Mediator.Send(command);
        }

        [HttpPost]
        [Route("{id}/run-smoke-test")]
        public async Task<Unit> ForceSmokeTest([FromRoute] long id,
            [FromBody] ForceSmokeTestForMachineCommand command)
        {
            command.Id = id;
            return await Mediator.Send(command);
        }

        [HttpPost]
        [Route("{id}/publish-sample-data")]
        public async Task<Unit> PublishAsSampleData([FromRoute] long id,
            [FromBody] PublishAsSampleDataCommand command)
        {
            command.Id = id;
            return await Mediator.Send(command);
        }

        [HttpPost]
        [Route("{id}/change-instance-type")]
        public async Task<Unit> ChangeInstanceType([FromRoute] long id,
            [FromBody] ChangeInstanceTypeForMachineCommand command)
        {
            command.Id = id;
            return await Mediator.Send(command);
        }

        [HttpPut]
        [Route("batch")]
        public async Task<Unit> BatchUpdate([FromBody] BatchUpdateMachinesCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPut]
        [Route("{id}/component-config")]
        public async Task<Unit> UpdateComponentConfig([FromRoute] long id, [FromBody] UpdateComponentConfigForMachineCommand command)
        {
            command.Id = id;
            return await Mediator.Send(command);
        }

        [HttpPut]
        [Route("{id}/deployer-config")]
        public async Task<Unit> UpdateDeployerConfig([FromRoute] long id, [FromBody] UpdateDeployerConfigForMachineCommand command)
        {
            command.Id = id;
            return await Mediator.Send(command);
        }

        #endregion
    }
}