using System;
using System.Linq;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace LocalstackInitialiser
{
    public class S3Initialiser : IDisposable
    {
        private readonly string _bucketName;
        private readonly AmazonS3Client _amazonS3Client;

        public S3Initialiser(string bucketName)
        {
            _bucketName = bucketName;

            var basicAwsCredentials = new BasicAWSCredentials("localstack", "localstack");
            _amazonS3Client = new AmazonS3Client(basicAwsCredentials, new AmazonS3Config
            {
                ServiceURL = "http://localhost:4572",
                AuthenticationRegion = "eu-west-1",
                ForcePathStyle = true
            });
        }

        public void Init()
        {
            CreateS3Bucket();
        }

        private async void CreateS3Bucket()
        {
            //var listBucketsResponse = await _amazonS3Client.ListBucketsAsync();

            //if (listBucketsResponse.Buckets.Any(x => x.BucketName == _bucketName))
            //    return;

            await _amazonS3Client.PutBucketAsync(new PutBucketRequest
            {
                BucketName = _bucketName,
                BucketRegion = S3Region.EUW1
            });
        }

        public void Dispose()
        {
            DestroyBucket();
            _amazonS3Client.Dispose();
        }

        private async void DestroyBucket()
        {
            var listObjectsResponse =
                await _amazonS3Client.ListObjectsV2Async(new ListObjectsV2Request { BucketName = _bucketName });

            await _amazonS3Client.DeleteObjectsAsync(new DeleteObjectsRequest
            {
                BucketName = _bucketName,
                Objects = listObjectsResponse.S3Objects.Select(x => new KeyVersion { Key = x.Key }).ToList()
            });

            await _amazonS3Client.DeleteBucketAsync(_bucketName);
        }
    }
}
