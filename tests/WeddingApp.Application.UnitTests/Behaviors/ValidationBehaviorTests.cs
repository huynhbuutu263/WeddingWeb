using FluentValidation;
using FluentValidation.Results;
using MediatR;
using WeddingApp.Application.Common.Behaviors;

namespace WeddingApp.Application.UnitTests.Behaviors;

public class ValidationBehaviorTests
{
    private sealed record TestRequest(string Value) : IRequest<string>;

    private sealed class TestValidator : AbstractValidator<TestRequest>
    {
        public TestValidator()
        {
            RuleFor(x => x.Value).NotEmpty().WithMessage("Value is required.");
        }
    }

    [Fact]
    public async Task Handle_NoValidators_CallsNext()
    {
        // Arrange
        var validators = Enumerable.Empty<IValidator<TestRequest>>();
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var request = new TestRequest("valid");
        var nextCalled = false;

        Task<string> Next(CancellationToken ct)
        {
            nextCalled = true;
            return Task.FromResult("ok");
        }

        // Act
        var result = await behavior.Handle(request, Next, CancellationToken.None);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal("ok", result);
    }

    [Fact]
    public async Task Handle_ValidRequest_CallsNext()
    {
        // Arrange
        var validators = new List<IValidator<TestRequest>> { new TestValidator() };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var request = new TestRequest("valid");
        var nextCalled = false;

        Task<string> Next(CancellationToken ct)
        {
            nextCalled = true;
            return Task.FromResult("ok");
        }

        // Act
        var result = await behavior.Handle(request, Next, CancellationToken.None);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal("ok", result);
    }

    [Fact]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var validators = new List<IValidator<TestRequest>> { new TestValidator() };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var request = new TestRequest(string.Empty);

        Task<string> Next(CancellationToken ct) => Task.FromResult("should not reach");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<WeddingApp.Application.Common.Exceptions.ValidationException>(
            () => behavior.Handle(request, Next, CancellationToken.None));

        Assert.True(exception.Errors.ContainsKey("Value"));
    }
}
