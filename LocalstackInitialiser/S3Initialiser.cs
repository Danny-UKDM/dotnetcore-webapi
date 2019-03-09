using System.Linq;
using Amazon.S3;
using Amazon.S3.Model;

namespace LocalstackInitialiser
{
    public class S3Initialiser
    {
        private readonly string _bucketName;
        private readonly AmazonS3Config _amazonS3Config;

        public S3Initialiser(string bucketName)
        {
            _bucketName = bucketName;

            _amazonS3Config = new AmazonS3Config
            {
                ServiceURL = "http://localhost:4572",
                AuthenticationRegion = "eu-west-1",
                ForcePathStyle = true
            };
        }

        public void Init()
        {
            CreateS3Bucket();
        }

        private async void CreateS3Bucket()
        {
            using (var amazonS3Client = new AmazonS3Client(_amazonS3Config))
            {
                var listBucketsResponse = await amazonS3Client.ListBucketsAsync();

                if (listBucketsResponse.Buckets.Any(x => x.BucketName == _bucketName))
                    return;

                await amazonS3Client.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = _bucketName,
                    BucketRegion = S3Region.EUW1
                });
            }
        }

        public void Dispose()
        {
            DestroyBucket();
        }

        private async void DestroyBucket()
        {
            using (var amazonS3Client = new AmazonS3Client(_amazonS3Config))
            {
                var listObjectsResponse =
                    await amazonS3Client.ListObjectsV2Async(new ListObjectsV2Request { BucketName = _bucketName });

                if (listObjectsResponse.S3Objects.Any())
                {
                    await amazonS3Client.DeleteObjectsAsync(new DeleteObjectsRequest
                    {
                        BucketName = _bucketName,
                        Objects = listObjectsResponse.S3Objects.Select(x => new KeyVersion { Key = x.Key }).ToList()
                    });
                }

                await amazonS3Client.DeleteBucketAsync(_bucketName);
            }
        }
    }
}
