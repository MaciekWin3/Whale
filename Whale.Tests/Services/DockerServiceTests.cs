using FluentAssertions;
using NUnit.Framework;
using Whale.Services;

namespace Whale.Tests.Services
{
    [TestFixture]
    public class DockerServiceTests
    {
        [Test]
        public async Task GetContainerListAsync_WhenCalled_ReturnsListOfContainers()
        {
            // Arrange
            var dockerService = new DockerService();

            // Act
            var result = await dockerService.GetContainerListAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
        }
    }
}
