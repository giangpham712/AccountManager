using System.Threading.Tasks;
using AccountManager.Application.Services;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace Infrastructure.S3
{
    public class S3LibraryFileService : S3FileServiceBase, ILibraryFileService
    {
        private readonly IAmazonS3 _client;

        public S3LibraryFileService(S3Configuration configuration) : base(configuration)
        {
            _client = new AmazonS3Client(new BasicAWSCredentials(configuration.AccessKey, configuration.SecretKey),
                RegionEndpoint.GetBySystemName(configuration.Region));
        }

        public async Task<bool> FileExists(string fileUrl)
        {
            var bucketName = Configuration.Bucket;
            var key = fileUrl.Substring($"https://s3.amazonaws.com/{bucketName}/".Length);

            var request = new GetObjectMetadataRequest
            {
                BucketName = bucketName,
                Key = key
            };

            try
            {
                var response = await _client.GetObjectMetadataAsync(request);
                return true;
            }
            catch (AmazonS3Exception e)
            {
                var errorCode = e.ErrorCode;
                if (errorCode == "NotFound")
                    return false;
                throw;
            }
        }
    }
}