using FluentValidation;
using TagTheSpot.Services.Moderation.Application.DTO.UseCases;

namespace TagTheSpot.Services.Moderation.Application.Validators
{
    public sealed class RejectSubmissionRequestValidator
        : AbstractValidator<RejectSubmissionRequest>
    {
        public RejectSubmissionRequestValidator()
        {
            RuleFor(x => x.SubmissionId)
                .NotEmpty();

            RuleFor(x => x.RejectionReason)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MinimumLength(5)
                .WithMessage("Rejection reason must be at least 5 characters long.")
                .MaximumLength(500)
                .WithMessage("Rejection reason must not exceed 500 characters.");
        }
    }
}
