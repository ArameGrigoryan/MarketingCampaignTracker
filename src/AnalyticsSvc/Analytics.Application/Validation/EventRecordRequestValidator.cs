using FluentValidation;
using Shared.Contracts;

namespace Analytics.Application.Validation;

public class EventRecordRequestValidator : AbstractValidator<EventRecordRequest>
{
    public EventRecordRequestValidator()
    {
        RuleFor(x => x.CampaignId)
            .GreaterThan(0).WithMessage("CampaignId must be greater than 0.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Event type is required.")
            .Must(t => new[] { "view", "click", "conversion" }.Contains(t.ToLower()))
            .WithMessage("Type must be one of: view, click, conversion.");

        RuleFor(x => x.OccurredAt)
            .LessThanOrEqualTo(_ => DateTimeOffset.UtcNow)  // runtime now
            .When(x => x.OccurredAt.HasValue)
            .WithMessage("OccurredAt cannot be in the future.");
    }
}
