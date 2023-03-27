using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountManager.Application.Services;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace Infrastructure.S3
{
    public class S3BackupFileService : S3FileServiceBase, IBackupFileService
    {
        private readonly IAmazonS3 _client;

        public S3BackupFileService(S3Configuration configuration) : base(configuration)
        {
            _client = new AmazonS3Client(new BasicAWSCredentials(configuration.AccessKey, configuration.SecretKey),
                RegionEndpoint.GetBySystemName(configuration.Region));
        }

        public async Task<IEnumerable<BackupFile>> ListBackupFiles(string accountUrlName, string software = null)
        {
            var bucketName = Configuration.Bucket;

            var request = new ListObjectsRequest
            {
                BucketName = bucketName,
                Prefix = $"backups/{accountUrlName}/{software}"
            };

            var response = await _client.ListObjectsAsync(request);
            return response.S3Objects.Select(x =>
            {
                var parts = x.Key.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                return new BackupFile
                {
                    Name = parts.Last(),
                    SoftwareName = parts[parts.Length - 2],
                    CreatedAt = x.LastModified,
                    UpdatedAt = x.LastModified
                };
            });
        }
    }
}