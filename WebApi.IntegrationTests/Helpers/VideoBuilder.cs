using System;
using WebApi.Models;

namespace WebApi.IntegrationTests.Helpers
{
    public class VideoBuilder
    {
        private readonly Video _video;

        private VideoBuilder(string videoName) =>
            _video = new Video
            {
                VideoId = Guid.NewGuid(),
                PartnerId = Guid.NewGuid(),
                VideoName = videoName,
                Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ"
            };

        public static VideoBuilder CreateVideo(string videoName) => new VideoBuilder(videoName);

        public VideoBuilder WithId(Guid id)
        {
            _video.VideoId = id;
            return this;
        }

        public Video Build() => _video;
    }
}
