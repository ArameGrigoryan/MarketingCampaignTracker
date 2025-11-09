using FluentValidation;
using Shared.Contracts;

namespace Campaign.Application.Validation;

public class CreateCampaignRequestValidator : AbstractValidator<CreateCampaignRequest>
{
    public CreateCampaignRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Campaign name is required.")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters.");

        RuleFor(x => x.TargetAudience)
            .NotEmpty().WithMessage("Target audience is required.")
            .MinimumLength(3).WithMessage("Target audience must be at least 3 characters.");
    }
}