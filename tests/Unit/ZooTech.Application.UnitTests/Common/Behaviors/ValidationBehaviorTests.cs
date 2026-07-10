using FluentAssertions;
using Moq;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Validator;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.UnitTests.Common.Behaviors;

public class ValidationBehaviorTests
{
    public record TestRequest;

    private class TestValidator : ICommandValidator<TestRequest>
    {
        private readonly List<string> _errors;
        public ModuleName ModuleName => ModuleName.Tenancing;

        public TestValidator(List<string>? errors = null)
        {
            _errors = errors ?? new List<string>();
        }

        public List<string> Validate(TestRequest request) => _errors;
    }

    [Fact]
    public async Task Should_Continue_When_Validation_Passes()
    {
        // Arrange
        var validator = new TestValidator();
        var behavior = new ValidationBehavior<TestRequest, string>(validator);

        // Act
        var result = await behavior.Handle(new TestRequest(), () => Task.FromResult("ok"));

        // Assert
        result.Should().Be("ok");
    }

    [Fact]
    public async Task Should_Throw_ValidationException_When_Validation_Fails()
    {
        // Arrange
        var errors = new List<string> { "TENANCING_CREATE-TENANT-CODE-NULL" };
        var validator = new TestValidator(errors);
        var behavior = new ValidationBehavior<TestRequest, string>(validator);

        // Act
        var act = async () => await behavior.Handle(new TestRequest(), () => Task.FromResult("ok"));

        // Assert
        var ex = await act.Should().ThrowAsync<ValidationException>();
        ex.Which.Details.Should().ContainSingle()
           .Which.Should().Be("TENANCING_CREATE-TENANT-CODE-NULL");
    }
}
