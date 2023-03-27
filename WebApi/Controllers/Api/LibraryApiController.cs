using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountManager.Application.Library.Commands.BatchDeleteLibraryFiles;
using AccountManager.Application.Library.Commands.DeleteLibraryFile;
using AccountManager.Application.Library.Queries.GetAllLibraryFiles;
using AccountManager.Application.Library.Queries.GetAllLibraryPackages;
using AccountManager.Application.Library.Queries.GetAllSampleDataFiles;
using AccountManager.Application.Models.Dto;
using AccountManager.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebApi.Controllers.Api
{
    [Route(ApiVersion + "/library")]
    public class LibraryApiController : AuthorizedApiController
    {
        public LibraryApiController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [Route("files")]
        public async Task<IEnumerable<FileDto>> GetAllLibraryFiles()
        {
            return await Mediator.Send(new GetAllLibraryFilesQuery());
        }

        [HttpGet]
        [Route("packages")]
        public async Task<IEnumerable<PackageDto>> GetAllPackages()
        {
            return await Mediator.Send(new GetAllLibraryPackagesQuery());
        }

        [HttpGet]
        [Route("sample-data-files")]
        public async Task<IEnumerable<SampleDataFile>> GetAllSampleDataFiles()
        {
            return await Mediator.Send(new GetAllSampleDataFilesQuery());
        }

        #region Commands

        [HttpDelete]
        [Route("files/{id}")]
        public async Task<Unit> DeleteFile([FromRoute] long id)
        {
            return await Mediator.Send(new DeleteLibraryFileCommand { Id = id });
        }

        [HttpDelete]
        [Route("files/batch")]
        public async Task<Unit> BatchDeleteFiles([FromQuery] string ids)
        {
            var fileIds = ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();

            return await Mediator.Send(new BatchDeleteLibraryFilesCommand { Ids = fileIds });
        }

        #endregion
    }
}