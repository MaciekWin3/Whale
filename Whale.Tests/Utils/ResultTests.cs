using FluentAssertions;
using NUnit.Framework;
using Whale.Utils;

namespace Whale.Tests.Utils
{
    [TestFixture]
    public class ResultTests
    {
        private object testObject;
        private const string errorMessageExample = "Example error message";

        [SetUp]
        public void Setup()
        {
            testObject = new object();
        }

        [Test]
        public void ShouldReturnFailResult()
        {
            var result = Result.Fail(errorMessageExample);

            result.Error.Should().Be(errorMessageExample);
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
        }

        [Test]
        public void ShouldReturnOkResult()
        {
            var result = Result.Ok();

            result.Error.Should().Be(string.Empty);
            result.IsSuccess.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
        }

        [Test]
        public void ShouldReturnOkResultForGeneric()
        {
            var result = Result.Ok(testObject);
            result.Error.Should().Be(string.Empty);
            result.Value.Should().BeEquivalentTo(testObject);
            result.IsSuccess.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
        }
    }
}
