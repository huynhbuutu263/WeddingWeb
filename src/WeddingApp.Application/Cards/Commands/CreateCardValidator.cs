using FluentValidation;

namespace WeddingApp.Application.Cards.Commands;

public class CreateCardValidator : AbstractValidator<CreateCardCommand>
{
    public CreateCardValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.");

        RuleFor(x => x.SlugUrl)
            .NotEmpty().WithMessage("SlugUrl is required.")
            .Matches("^[a-z0-9-]+$").WithMessage("SlugUrl must contain only lowercase letters, digits, and hyphens.");

        RuleFor(x => x.EventDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("EventDate must be in the future.");

        RuleFor(x => x.TemplateId)
            .NotEmpty().WithMessage("TemplateId is required.");
    }
}
