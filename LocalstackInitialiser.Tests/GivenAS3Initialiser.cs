using System;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Xunit;

namespace LocalstackInitialiser.Tests
{
    public class GivenAS3Initialiser : IDisposable
    {
        private string _bucketName;
        private readonly S3Initialiser _initialiser;
        private AmazonS3Client _amazonS3Client;

        public GivenAS3Initialiser()
        {
            _bucketName = "testbucket";
            _initialiser = new S3Initialiser(_bucketName);
            _initialiser.Init();

            var basicAwsCredentials = new BasicAWSCredentials("localstack", "localstack");
            _amazonS3Client = new AmazonS3Client(basicAwsCredentials, new AmazonS3Config
            {
                ServiceURL = "http://localhost:4572",
                AuthenticationRegion = "eu-west-1",
                ForcePathStyle = true
            });
        }

        [Fact]
        public async Task TheTheBucketIsCreated()
        {
            var getBucketLocationResponse = await _amazonS3Client.GetBucketLocationAsync(_bucketName);
        }

        public void Dispose()
        {
            _initialiser.Dispose();
        }
    }
}
