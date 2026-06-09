using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using ZooTech.Application.Common.Behaviors;
using ValidationException = ZooTech.Application.Common.Exceptions.ValidationException;

namespace ZooTech.Application.UnitTests.Common.Behaviors;

public class ValidationBehaviorTests
{
    public record TestRequest : IRequest<string>;

    [Fact]
    public async Task Should_Continue_When_No_Validators_Are_Registered()
    {
        // Arrange
        var validators = Array.Empty<IValidator<TestRequest>>();
        var behavior = new ValidationBehavior<TestRequest, string>(validators);

        // Act
        var result = await behavior.Handle(new TestRequest(), _ => Task.FromResult("ok"), CancellationToken.None);

        // Assert
        result.Should().Be("ok");
    }

    [Fact]
    public async Task Should_Continue_When_Validation_Passes()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(v => v.Validate(It.IsAny<ValidationContext<TestRequest>>()))
            .Returns(new ValidationResult());

        var behavior = new ValidationBehavior<TestRequest, string>(new[] { validatorMock.Object });

        // Act
        var result = await behavior.Handle(new TestRequest(), ct => Task.FromResult("ok"), CancellationToken.None);

        // Assert
        result.Should().Be("ok");
    }

    [Fact]
    public async Task Should_Throw_ValidationException_When_Validation_Fails()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new("Property", "Error message")
        };

        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(v => v.Validate(It.IsAny<ValidationContext<TestRequest>>()))
            .Returns(new ValidationResult(failures));

        var behavior = new ValidationBehavior<TestRequest, string>(new[] { validatorMock.Object });

        // Act
        var act = async () => await behavior.Handle(new TestRequest(), ct => Task.FromResult("ok"), CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<ValidationException>();
        ex.Which.Errors.Should().ContainSingle()
           .Which.PropertyName.Should().Be("Property");
    }
}
