using FluentAssertions;
using Moq;
using NUnit.Framework;
using Whale.Services;
using Whale.Utils;

namespace Whale.Tests.Services
{
    [TestFixture]
    public class DockerServiceTests
    {
        private Mock<IShellCommandRunner> shellCommandRunnerMock;

        [SetUp]
        public void SetUp()
        {
            shellCommandRunnerMock = new Mock<IShellCommandRunner>();
        }

        [Test]
        public async Task GetContainerListAsync_WhenCalled_ReturnsListOfContainers()
        {
            // Arrange
            var std =
                """
                CONTAINER ID   IMAGE         COMMAND    CREATED       STATUS                     PORTS     NAMES
                addd94f3ac8e   ubuntu        "bash"     11 days ago   Exited (255) 5 days ago              goofy_shockley
                547a330db120   hello-world   "/hello"   11 days ago   Exited (0) 11 days ago               nifty_banach
                """;



            shellCommandRunnerMock.Setup(x => x.RunCommandAsync("docker", "ps", "-a"))
                .ReturnsAsync(Result.Ok((std, string.Empty)));

            // Act
            var service = CreateDockerService();
            var result = await service.GetContainerListAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        private DockerService CreateDockerService()
        {
            return new DockerService(shellCommandRunnerMock.Object);
        }
    }
}
