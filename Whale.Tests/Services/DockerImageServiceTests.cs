using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Whale.Services;
using Whale.Services.Interfaces;
using Whale.Utils;

namespace Whale.Tests.Services
{
    [TestFixture]
    public class DockerImageServiceTests
    {
        private IShellCommandRunner shellCommandRunnerMock;

        [SetUp]
        public void SetUp()
        {
            shellCommandRunnerMock = Substitute.For<IShellCommandRunner>();
        }

        [Test]
        public async Task ShouldReturnListOfImagesWhenGetImageListAsyncIsCalled()
        {
            // Arrange
            var std =
                """
                {"Containers":"N/A","CreatedAt":"2023-05-04 19:37:03 +0200 CEST","CreatedSince":"2 months ago","Digest":"\u003cnone\u003e","ID":"9c7a54a9a43c","Repository":"hello-world","SharedSize":"N/A","Size":"13.3kB","Tag":"latest","UniqueSize":"N/A","VirtualSize":"13.26kB"}
                {"Containers":"N/A","CreatedAt":"2023-04-25 19:30:49 +0200 CEST","CreatedSince":"2 months ago","Digest":"\u003cnone\u003e","ID":"3b418d7b466a","Repository":"ubuntu","SharedSize":"N/A","Size":"77.8MB","Tag":"latest","UniqueSize":"N/A","VirtualSize":"77.81MB"}
                """;

            shellCommandRunnerMock.RunCommandAsync("docker image ls --all --format json", default)
                .Returns(Result.Ok((std, string.Empty)));

            // Act
            var service = CreateDockerService();
            var result = await service.GetImageListAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
            result.Value.Should().Contain(image => image.ID == "9c7a54a9a43c");
            result.Value.Should().Contain(image => image.ID == "3b418d7b466a");
        }

        [Test]
        public async Task ShouldReturnNoImagesFoundWhenGetImageListAsyncIsCalled()
        {
            // Arrange
            var std = "";

            shellCommandRunnerMock.RunCommandAsync("docker image ls --all --format json", default)
                .Returns(Result.Ok((std, string.Empty)));

            // Act
            var service = CreateDockerService();
            var result = await service.GetImageListAsync();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("No images found");
        }

        [Test]
        public async Task ShouldReturnListOfImageLayersWhenGetImageLayersAsyncIsCalled()
        {
            // Arrange
            var containerId = "9c7a54a9a43c";
            var std =
                """
                {"Containers":"N/A","CreatedAt":"2023-05-04 19:37:03 +0200 CEST","CreatedSince":"2 months ago","Digest":"\u003cnone\u003e","ID":"9c7a54a9a43c","Repository":"hello-world","SharedSize":"N/A","Size":"13.3kB","Tag":"latest","UniqueSize":"N/A","VirtualSize":"13.26kB"}
                {"Containers":"N/A","CreatedAt":"2023-04-25 19:30:49 +0200 CEST","CreatedSince":"2 months ago","Digest":"\u003cnone\u003e","ID":"3b418d7b466a","Repository":"ubuntu","SharedSize":"N/A","Size":"77.8MB","Tag":"latest","UniqueSize":"N/A","VirtualSize":"77.81MB"}
                """;

            shellCommandRunnerMock.RunCommandAsync($"docker image history {containerId} --format json", default)
                .Returns(Result.Ok((std, string.Empty)));

            // Act
            var service = CreateDockerService();
            var result = await service.GetImageLayersAsync(containerId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
        }

        [Test]
        public async Task ShouldReturnNoImageLayersFoundWhenGetImageLayersAsyncIsCalled()
        {
            // Arrange
            var containerId = "abc123";
            var std = "";

            shellCommandRunnerMock.RunCommandAsync($"docker image history {containerId} --format json", default)
                .Returns(Result.Ok((std, string.Empty)));

            // Act
            var service = CreateDockerService();
            var result = await service.GetImageLayersAsync(containerId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Image not found");
        }

        [Test]
        public async Task ShouldReturnCommandFailedWhenGetImageLayersAsyncIsCalled()
        {
            // Arrange
            var containerId = "abc123";
            var std = "";

            shellCommandRunnerMock.RunCommandAsync($"docker image history {containerId} --format json", default)
                .Returns(Result.Ok((std, string.Empty)));

            // Act
            var service = CreateDockerService();
            var result = await service.GetImageLayersAsync(containerId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Image not found");
        }

        [Test]
        public async Task ShouldDeleteImage()
        {
            // Arrange
            string volumeId = "abc123";

            shellCommandRunnerMock.RunCommandAsync($"docker image remove {volumeId}", default)
                .Returns(Result.Ok((volumeId, string.Empty)));

            // Act
            var service = CreateDockerService();
            var result = await service.DeleteImageAsync(volumeId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Error.Should().BeNullOrEmpty();
            result.Value.Should().Be(volumeId);
        }

        private DockerImageService CreateDockerService()
        {
            return new DockerImageService(shellCommandRunnerMock);
        }
    }
}
