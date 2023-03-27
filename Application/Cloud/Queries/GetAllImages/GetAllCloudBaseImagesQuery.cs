using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Cloud.Queries.GetAllImages
{
    public class GetAllImagesQuery : IRequest<List<CloudBaseImageDto>>
    {
    }
}