using System;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using FluentAssertions;
using Xunit;

namespace LocalstackInitialiser.Tests
{
    public class GivenAS3Initialiser : IDisposable
    {
        private readonly string _bucketName;
        private readonly S3Initialiser _initialiser;
        private readonly AmazonS3Config _amazonS3Config;

        public GivenAS3Initialiser()
        {
            _bucketName = "testbucket";
            _initialiser = new S3Initialiser(_bucketName);
            _initialiser.Init();

            _amazonS3Config = new AmazonS3Config
            {
                ServiceURL = "http://localhost:4572",
                AuthenticationRegion = "eu-west-1",
                ForcePathStyle = true
            };
            ;
        }

        [Fact]
        public async Task ThenTheBucketIsCreated()
        {
            ListBucketsResponse listBucketsResponse;
            using (var amazonS3Client = new AmazonS3Client(_amazonS3Config))
            {
                listBucketsResponse = await amazonS3Client.ListBucketsAsync();
            }

            listBucketsResponse.Buckets.Should().Contain(x => x.BucketName == _bucketName);
        }

        public void Dispose()
        {
            _initialiser.Dispose();
        }
    }
}
