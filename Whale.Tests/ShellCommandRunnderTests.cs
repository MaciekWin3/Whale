using FluentAssertions;
using NUnit.Framework;

namespace Whale.Tests
{
    [TestFixture]
    public class ShellCommandRunnderTests
    {
        [Test]
        public void RunCommandShouldReturnHelloOnWindows()
        {
            // Arrange
            var command = "echo hello";
            var expectedOutput = "hello";
            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            ShellCommandRunner.RunCommand(command);

            // Assert
            writer.ToString().TrimEnd().Should().Be(expectedOutput);
        }

        [Test]
        public void RunCommandShouldReturnHelloOnMacOS()
        {
            // Arrange
            var command = "echo hello";
            var expectedOutput = "hello";
            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            ShellCommandRunner.RunCommand(command);

            // Assert
            writer.ToString().TrimEnd().Should().Be(expectedOutput);
        }

        [Test]
        public void RunCommandShouldReturnHelloOnLinux()
        {
            // Arrange
            var command = "echo hello";
            var expectedOutput = "hello";
            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            ShellCommandRunner.RunCommand(command);

            // Assert
            writer.ToString().TrimEnd().Should().Be(expectedOutput);
        }
    }
}
