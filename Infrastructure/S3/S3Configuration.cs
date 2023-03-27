namespace Infrastructure.S3
{
    public class S3Configuration
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string Region { get; set; }
        public string Bucket { get; set; }
    }
}