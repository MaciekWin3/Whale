using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Whale.Services;
using Whale.Services.Interfaces;
using Whale.Utils;

namespace Whale.Tests.Services
{
    [TestFixture]
    public class DockerContainerServiceTests
    {
        private IShellCommandRunner shellCommandRunnerMock;

        [SetUp]
        public void SetUp()
        {
            shellCommandRunnerMock = Substitute.For<IShellCommandRunner>();
        }

        [Test]
        public async Task ShouldReturnListOfContainersWhenGetContainerListAsyncIsCalled()
        {
            // Arrange
            var std =
                """
                {"Command":"\"/bin/sh\"","CreatedAt":"2023-06-26 22:44:32 +0200 CEST","ID":"d9f5c4922862","Image":"alpine:latest","Labels":"","LocalVolumes":"0","Mounts":"","Names":"busy_poitras","Networks":"bridge","Ports":"","RunningFor":"43 hours ago","Size":"0B","State":"exited","Status":"Exited (0) 26 hours ago"}
                {"Command":"\"/hello\"","CreatedAt":"2023-06-19 19:25:10 +0200 CEST","ID":"7241c57ca1c6","Image":"hello-world:latest","Labels":"","LocalVolumes":"0","Mounts":"","Names":"angry_cohen","Networks":"bridge","Ports":"","RunningFor":"8 days ago","Size":"0B","State":"exited","Status":"Exited (0) 8 days ago"}
                {"Command":"\"bash\"","CreatedAt":"2023-05-28 19:33:19 +0200 CEST","ID":"addd94f3ac8e","Image":"ubuntu","Labels":"org.opencontainers.image.ref.name=ubuntu,org.opencontainers.image.version=22.04","LocalVolumes":"0","Mounts":"","Names":"goofy_shockley","Networks":"bridge","Ports":"","RunningFor":"4 weeks ago","Size":"0B","State":"running","Status":"Up 7 hours"}
                """;

            shellCommandRunnerMock.RunCommandAsync("docker container ls --all --format json", default)
                .Returns(Result.Ok((std, string.Empty)));

            // Act
            var service = CreateDockerService();
            var result = await service.GetContainerListAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Test]
        public async Task ShouldReturnErrorWhenGetContainerListAsyncIsCalled()
        {
            // Arrange
            var std = "Command failed";

            shellCommandRunnerMock.RunCommandAsync("docker container ls --all --format json", default)
                .Returns(Result.Fail<(string std, string err)>(std));

            // Act
            var service = CreateDockerService();
            var result = await service.GetContainerListAsync();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(std);
        }

        [Test]
        public async Task ShouldDeleteContainer()
        {
            // Arrange
            string containerId = "containerId";

            shellCommandRunnerMock.RunCommandAsync($"docker container remove {containerId}", default)
                .Returns(Result.Ok((containerId, string.Empty)));

            // Act
            var service = CreateDockerService();
            var result = await service.DeleteContainerAsync(containerId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Error.Should().BeNullOrEmpty();
            result.Value.Should().Be(containerId);
        }

        [Test]
        public async Task ShouldFailWhenDeletingContainerIfContainerDoesNotExists()
        {
            // Arrange
            string containerId = "notExistingContainerId";
            var errorMessage =
                """
                Error response from daemon: No such container: sdafasdjfasdf
                NativeCommandExitException: Program "docker.exe" ended with non-zero exit code: 1.
                """;

            shellCommandRunnerMock.RunCommandAsync($"docker container remove {containerId}", default)
                .Returns(Result.Fail<(string std, string err)>(errorMessage));

            // Act
            var service = CreateDockerService();
            var result = await service.DeleteContainerAsync(containerId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(errorMessage);
        }

        private DockerContainerService CreateDockerService()
        {
            return new DockerContainerService(shellCommandRunnerMock);
        }
    }
}
