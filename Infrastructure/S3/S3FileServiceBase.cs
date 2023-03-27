namespace Infrastructure.S3
{
    public abstract class S3FileServiceBase
    {
        protected readonly S3Configuration Configuration;

        protected S3FileServiceBase(S3Configuration configuration)
        {
            Configuration = configuration;
        }

        protected string GetFileUrl(string key)
        {
            return
                $"https://{Configuration.Bucket}.s3.{Configuration.Region}.amazonaws.com/{key}";
        }
    }
}