﻿//using System;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Formatting;
//using System.Net.Http.Headers;
//using System.Net.Mime;
//using System.Threading.Tasks;
//using Amazon.S3.Model;
//using FluentAssertions;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Http.Internal;
//using NSubstitute;
//using Xunit;

//namespace WebApi.IntegrationTests.Controllers.ImagesController.Post
//{
//    [Collection(nameof(TestCollection))]
//    public class GivenAPostRequest : IClassFixture<GivenAPostRequest.PostRequest>
//    {
//        public class PostRequest : IAsyncLifetime
//        {
//            private readonly ApiWebApplicationFactory _factory;
//            private ListObjectsV2Response _listObjectsResponse;
//            public HttpResponseMessage Response { get; private set; }
//            public PostRequest(ApiWebApplicationFactory factory) => _factory = factory;

//            public async Task InitializeAsync()
//            {
//                var fileName = Guid.NewGuid().ToString();
//                var fileMock = Substitute.For<IFormFile>();
//                fileMock.ContentType.Returns("image/png");
//                fileMock.FileName.Returns(fileName);
//                fileMock.Length.Returns(1000);
//                fileMock.OpenReadStream().Returns(new MemoryStream());
//                fileMock.ContentType.Returns($"inline; filename={fileName}");

//                var content = new MultipartFormDataContent
//                {
//                    new ByteArrayContent(new byte[] {0x21, 0x22, 0x23, 0x24})
//                };

//                byte[] fileBytes;
//                using (var stream = File.OpenRead("../../../Controllers/ImagesController/Images/1x1.gif"))
//                {
//                    var binaryReader = new BinaryReader(stream);
//                    fileBytes = binaryReader.ReadBytes((int)stream.Length);
//                }
//                var imageBinaryContent = new ByteArrayContent(fileBytes);

//                var multipartContent = new MultipartFormDataContent
//                {
//                    { imageBinaryContent, "file" }
//                };

//                FormFile file;
//                using (var stream = File.OpenRead("../../../Controllers/ImagesController/Images/1x1.gif"))
//                {
//                    file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
//                    {
//                        Headers = new HeaderDictionary(),
//                        ContentType = "image/jpeg"
//                    };
//                }

//                MultipartFormDataContent content;
//                using (content = new MultipartFormDataContent())
//                {
//                    content.Add(new StreamContent(file.OpenReadStream())
//                    {
//                        Headers =
//                        {
//                            ContentLength = file.Length,
//                            ContentType = new MediaTypeHeaderValue(file.ContentType)
//                        }
//                    }, file.Name, file.FileName);
//                }

//                Response = await _factory.Client.PostAsync("/api/images", content);
//            }

//            public async Task<long> ImageCount()
//            {
//                _listObjectsResponse = await _factory.AmazonS3Client.ListObjectsV2Async(new ListObjectsV2Request { BucketName = _factory.ImageBucketName });

//                return _listObjectsResponse.KeyCount;
//            }

//            public async Task DisposeAsync()
//            {
//                await _factory.AmazonS3Client.DeleteObjectsAsync(new DeleteObjectsRequest
//                {
//                    BucketName = _factory.ImageBucketName,
//                    Objects = _listObjectsResponse.S3Objects.Select(x => new KeyVersion { Key = x.Key }).ToList()
//                });
//            }
//        }

//        private readonly PostRequest _fixture;

//        public GivenAPostRequest(PostRequest fixture) => _fixture = fixture;

//        [Fact]
//        public void ThenTheExpectedResponseStatusCodeWasReceived() =>
//            _fixture.Response.StatusCode.Should().Be(HttpStatusCode.Created);

//        [Fact]
//        public void ThenTheExpectedResponseContentTypeWasReceived() =>
//            _fixture.Response.Content.Headers.ContentType.Should()
//                    .BeEquivalentTo(new MediaTypeHeaderValue("application/json") { CharSet = "utf-8" });

//        [Fact]
//        public async Task ThenTheEventShouldBeStored() =>
//            (await _fixture.ImageCount()).Should().Be(1);

//    }
//}