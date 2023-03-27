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
    public class S3SampleDataFileService : S3FileServiceBase, ISampleDataFileService
    {
        private readonly IAmazonS3 _client;

        public S3SampleDataFileService(S3Configuration configuration) : base(configuration)
        {
            _client = new AmazonS3Client(new BasicAWSCredentials(configuration.AccessKey, configuration.SecretKey),
                RegionEndpoint.GetBySystemName(configuration.Region));
        }

        public async Task<IEnumerable<SampleDataFile>> ListFiles()
        {
            var bucketName = Configuration.Bucket;

            var request = new ListObjectsRequest
            {
                BucketName = bucketName,
                Prefix = "SampleTestData/"
            };

            var response = await _client.ListObjectsAsync(request);
            return response.S3Objects.Where(x => x.Key != "SampleTestData/").Select(x => new SampleDataFile
                { Name = x.Key.Replace("SampleTestData/", ""), Size = x.Size, LastModified = x.LastModified });
        }
    }
}