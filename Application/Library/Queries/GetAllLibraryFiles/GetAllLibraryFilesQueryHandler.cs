using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Key;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities.Library;
using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AccountManager.Application.Library.Queries.GetAllLibraryFiles
{
    public class GetAllLibraryFilesQueryHandler : IRequestHandler<GetAllLibraryFilesQuery, List<FileDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;
        private readonly IKeysManager _keyManager;

        public GetAllLibraryFilesQueryHandler(ICloudStateDbContext context, IKeysManager keyManager, IMapper mapper)
        {
            _context = context;
            _keyManager = keyManager;
            _mapper = mapper;
        }

        public async Task<List<FileDto>> Handle(GetAllLibraryFilesQuery request, CancellationToken cancellationToken)
        {
            var files = await _context.Set<File>()
                .Include(x => x.Packages)
                .Where(x => x.Id > 1)
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync(cancellationToken);

            var dtos = new List<FileDto>();

            foreach (var file in files)
            {
                var dto = _mapper.Map<FileDto>(file);
                dtos.Add(dto);
            }

            return await Task.FromResult(dtos);
        }

        private bool VerifyLibraryFileSignature(File libraryFile, string lctInterServerPublicKey)
        {
            // Create LctSignatureBlock object
            var signatureBlock = new
            {
                libraryFile.Type,
                libraryFile.Url,
                AccountName = libraryFile.AddedBy,
                Timestamp = libraryFile.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fffK", CultureInfo.InvariantCulture)
            };

            var json = JsonConvert.SerializeObject(signatureBlock, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                TypeNameHandling = TypeNameHandling.None
            });
            var bytes = Encoding.UTF8.GetBytes(json);
            var signature = Convert.FromBase64String(libraryFile.Signature);

            using (var dsa = DSA.Create())
            {
                dsa.FromXmlString(lctInterServerPublicKey);

                return VerifySignature(dsa, bytes, signature, SHA512.Create()) ||
                       VerifySignature(dsa, bytes, signature, SHA1.Create()); // backward compatible
            }
        }

        private bool VerifySignature(DSA dsa, byte[] bytes, byte[] signature, HashAlgorithm algorithm)
        {
            try
            {
                var rgbHash = algorithm.ComputeHash(bytes);
                return dsa.VerifySignature(rgbHash, signature);
            }
            catch
            {
                return false;
            }
        }
    }
}