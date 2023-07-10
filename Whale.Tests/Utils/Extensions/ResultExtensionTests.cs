using FluentAssertions;
using NUnit.Framework;
using Whale.Utils;
using Whale.Utils.Extensions;

namespace Whale.Tests.Utils.Extensions
{
    [TestFixture]
    public class ResultExtensionTests
    {
        [Test]
        public void ShouldThrowInvalidOperationExceptionForMapFailIfResultIsNull()
        {
            // Arrange
            Result result = null!;

            // Act
            Action toOkResult = () => result.MapFail<string>();

            // Assert
            toOkResult.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void ShouldReturnFailResultForMapFailIfResultIsNotSuccessful()
        {
            // Arrange
            var result = Result.Fail<string>("Error message example");
            var expectedFailResult = Result.Fail<string>(result.Error!);

            // Act
            var failResult = result.MapFail<string>();

            // Assert
            failResult.Should().BeEquivalentTo(expectedFailResult);
        }
    }
}
