using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AccountManager.Application.Services;
using AccountManager.Domain;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace Infrastructure.S3
{
    public class S3BuildFileService : S3FileServiceBase, IBuildFileService
    {
        private static readonly IDictionary<Software, string> BuildFolderBySoftware;
        private readonly IAmazonS3 _client;

        static S3BuildFileService()
        {
            var enumType = typeof(Software);
            BuildFolderBySoftware = Enum.GetValues(typeof(Software)).Cast<Software>().ToDictionary(s => s, s =>
            {
                var softwareComponentAttribute = enumType.GetMember(s.ToString()).First()
                    .GetCustomAttribute<SoftwareComponentAttribute>();

                return softwareComponentAttribute.BuildFolderName;
            });
        }

        public S3BuildFileService(S3Configuration configuration) : base(configuration)
        {
            _client = new AmazonS3Client(new BasicAWSCredentials(configuration.AccessKey, configuration.SecretKey),
                RegionEndpoint.GetBySystemName(configuration.Region));
        }

        public async Task<BuildFile> GetBuildFile(string key)
        {
            var bucketName = Configuration.Bucket;

            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            try
            {
                var response = await _client.GetObjectAsync(request);
                var url = GetFileUrl(response.Key);
                ;
                return null;
            }
            catch (AmazonS3Exception e)
            {
                return null;
            }
        }

        public async Task<IEnumerable<BuildFile>> ListBuildFiles(Software software, BuildFileType? type = null)
        {
            if (BuildFolderBySoftware.TryGetValue(software, out var softwareFolder))
            {
                var tasks = new List<Task<IEnumerable<BuildFile>>>();
                if (type == BuildFileType.Daily)
                {
                    tasks.Add(ListDailyBuildFiles(softwareFolder));
                }
                else if (type == BuildFileType.Release)
                {
                    tasks.Add(ListReleaseBuildFiles(softwareFolder));
                }
                else
                {
                    tasks.Add(ListDailyBuildFiles(softwareFolder));
                    tasks.Add(ListReleaseBuildFiles(softwareFolder));
                }

                var results = await Task.WhenAll(tasks);
                return results.SelectMany(x => x);
            }

            return new List<BuildFile>();
        }

        private async Task<IEnumerable<BuildFile>> ListDailyBuildFiles(string software)
        {
            var bucketName = Configuration.Bucket;

            var prefix = $"daily/{software}/";
            var request = new ListObjectsRequest
            {
                BucketName = bucketName,
                Prefix = prefix
            };

            try
            {
                var response = await _client.ListObjectsAsync(request);
                return response.S3Objects.Where(x => x.Key != prefix).Select(x =>
                {
                    try
                    {
                        var url = GetFileUrl(x.Key);
                        ;
                        return new BuildFile(x.Key.Replace(prefix, ""), url, x.Size, x.LastModified,
                            BuildFileType.Daily);
                    }
                    catch
                    {
                        return null;
                    }
                }).Where(x => x != null);
            }
            catch (AmazonS3Exception e)
            {
                return new List<BuildFile>();
            }
        }

        private async Task<IEnumerable<BuildFile>> ListReleaseBuildFiles(string software)
        {
            var bucketName = Configuration.Bucket;

            var prefix = $"releases/{software}/";
            var request = new ListObjectsRequest
            {
                BucketName = bucketName,
                Prefix = prefix
            };

            try
            {
                var response = await _client.ListObjectsAsync(request);
                return response.S3Objects.Where(x => x.Key != prefix).Select(x =>
                {
                    try
                    {
                        var url = GetFileUrl(x.Key);
                        return new BuildFile(x.Key.Replace(prefix, ""), url, x.Size, x.LastModified,
                            BuildFileType.Release);
                    }
                    catch
                    {
                        return null;
                    }
                }).Where(x => x != null);
            }
            catch (AmazonS3Exception e)
            {
                return new List<BuildFile>();
            }
        }
    }
}