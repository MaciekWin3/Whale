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
            var command = "dotnet --info";


            // Act
            var service = CreateShellCommandRunner();
            var result = await service.RunCommandAsync(command);

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
            var command = "echo hello";

            // Act
            var service = CreateShellCommandRunner();
            var result = await service.RunCommandAsync(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.std.ToString()
                .Trim()
                .Should()
                .Be("hello");
        }

        private static ShellCommandRunner CreateShellCommandRunner()
        {
            return new ShellCommandRunner();
        }
    }
}
