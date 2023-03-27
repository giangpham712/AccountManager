using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountManager.Application.Machines.Queries.GetAllMachineLookups;
using AccountManager.Application.Machines.Queries.GetAllMachines;
using AccountManager.Application.Models.Dto;
using AccountManager.WebApi.Controllers.SaasApi;
using MediatR;

namespace AccountManager.WebApi.Controllers.Api
{
    [Route(ApiVersion + "/lookup")]
    [ApiController]
    public class LookupApiController : AuthorizedApiController
    {
        public LookupApiController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [Route("machines")]
        public async Task<IEnumerable<MachineLookupDto>> GetMachines()
        {
            return await Mediator.Send(new GetAllMachineLookupsQuery());
        }



        [HttpGet]
        [Route("reporting-categories")]
        public async Task<IEnumerable<string>> GetReportingCategories()
        {
            return new List<string>()
            {
                "Equipment",
                "Equipment Type",
                "Cable",
                "Cable Type",
                "Software",
                "Software Type",
                "Circuit",
                "Circuit Type",
                "Pathway",
                "Pathway Type",
                "Location",
                "Space",
                "Maintenance Hole",
                "Area",
                "Contact",
                "User",
                "Vendor",
                "Contract",
                "Grouping",
                "Project",
                "MAC",
                "Ticket",
                "Schedule",
                "IRM Admin",
                "Task Request",
                "Layer",
            };
        }
    }
}
