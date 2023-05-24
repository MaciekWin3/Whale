using FluentAssertions;
using NUnit.Framework;

namespace Whale.Tests
{
    [TestFixture]
    public class ShellCommandRunnderTests
    {
        [Test]
        public async Task RunCommandShouldReturnHello()
        {
            // Arrange
            var command = "echo";
            var expectedOutput = "hello";
            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            string output = await ShellCommandRunner.RunCommandAsync(command.);

            // Assert
            output.TrimEnd().Should().Be(expectedOutput);
        }

        [Test]
        public void RunCommandShouldReturnError()
        {
            // Arrange
            var command = "invalid-command";
            var expectedOutput = "Error!";
            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            var output = ShellCommandRunner.RunCommand(command);

            // Assert
            writer.ToString().TrimEnd().Should().BeEmpty();
            output.Should().Be(expectedOutput);
        }
    }
}
