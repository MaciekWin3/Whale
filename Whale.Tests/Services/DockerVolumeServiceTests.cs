﻿using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Whale.Services;
using Whale.Services.Interfaces;
using Whale.Utils;

namespace Whale.Tests.Services
{
    [TestFixture]
    public class DockerVolumeServiceTests
    {
        private IShellCommandRunner shellCommandRunnerMock;

        [SetUp]
        public void SetUp()
        {
            shellCommandRunnerMock = Substitute.For<IShellCommandRunner>();
        }

        [Test]
        public async Task ShouldReturnListOfVolumesWhenGetVolumeListAsyncIsCalled()
        {
            // Arrange
            var std =
                """
                {"Availability":"N/A","Driver":"local","Group":"N/A","Labels":"com.docker.volume.anonymous=","Links":"N/A","Mountpoint":"/var/lib/docker/volumes/volume1/_data","Name":"volume1","Scope":"local","Size":"N/A","Status":"N/A"}
                {"Availability":"N/A","Driver":"local","Group":"N/A","Labels":"com.docker.volume.anonymous=","Links":"N/A","Mountpoint":"/var/lib/docker/volumes/volume2/_data","Name":"volume2","Scope":"local","Size":"N/A","Status":"N/A"}
                """;

            shellCommandRunnerMock.RunCommandAsync("docker volume ls --format json", default)
                .Returns(Result.Ok((std, string.Empty)));

            // Act
            var service = CreateDockerVolumeService();
            var result = await service.GetVolumeListAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().HaveCount(2);
        }

        [Test]
        public async Task ShouldReturnNoVolumesFoundWhenGetVolumeListAsyncIsCalled()
        {
            // Arrange
            var std = "";

            shellCommandRunnerMock.RunCommandAsync("docker volume ls --format json", default)
                .Returns(Result.Ok((std, string.Empty)));

            // Act
            var service = CreateDockerVolumeService();
            var result = await service.GetVolumeListAsync();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Container not found");
        }

        private DockerVolumeService CreateDockerVolumeService()
        {
            return new DockerVolumeService(shellCommandRunnerMock);
        }
    }
}
