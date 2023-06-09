using FluentAssertions;
using NUnit.Framework;
using Whale.Services;

namespace Whale.Tests.Services
{
    [TestFixture]
    public class ShellCommandRunnderTests
    {
        [Test]
        public async Task ShouldReturnDotnetInfoOutput()
        {
            // Arrange
            var command = "dotnet";
            var arguments = new[] { "--info" };

            // Act
            var service = CreateShellCommandRunner();
            var result = await service.RunCommandAsync(command, arguments);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.std.ToString()
                .Substring(0, 4)
                .Should()
                .Be(".NET");
        }

        [Test]
        [Platform("Win")]
        public async Task ShouldReturnCmdSuccess()
        {
            // Arrange
            var command = "cmd";
            var arguments = new[] { "/C", "echo", "hello" };

            // Act
            var service = CreateShellCommandRunner();
            var result = await service.RunCommandAsync(command, arguments);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.std.ToString()
                .Trim()
                .Should()
                .Be("hello");
        }

        private ShellCommandRunner CreateShellCommandRunner()
        {
            return new ShellCommandRunner();
        }
    }
}
