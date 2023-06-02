using FluentAssertions;
using NUnit.Framework;

namespace Whale.Tests
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
            var result = await ShellCommandRunner.RunCommandAsync(command, arguments);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.std.ToString()
                .Substring(0, 4)
                .Should()
                .Be(".NET");
        }

        [Test]
        public async Task ShouldReturnCmdSuccess()
        {
            // Arrange
            var command = "cmd";
            var arguments = new[] { "/C", "echo", "hello" };

            // Act
            var result = await ShellCommandRunner.RunCommandAsync(command, arguments);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.std.ToString()
                .Trim()
                .Should()
                .Be("hello");
        }
    }
}
