using AdsManagement.App.DTOs.Comment;
using FluentValidation;
using System.Text.RegularExpressions;

namespace AdsManagement.App.Validators.Comment
{
    public class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
    {
        public CreateCommentDtoValidator()
        {
            RuleFor(c => c.UserId)
                .NotEmpty().WithMessage("The user's ID cannot be empty");

            RuleFor(c => c.AdvertisementId)
                .NotEmpty().WithMessage("The advertisement's ID cannot be empty");

            RuleFor(c => c.Text)
                .NotEmpty().WithMessage("The text of the comment cannot be empty")
                .MaximumLength(500).WithMessage("The comment exceeds the allowed length")
                .Must(text => !Regex.IsMatch(text, "<script", RegexOptions.IgnoreCase))
                .WithMessage("Prohibited content");

            RuleFor(c => c.Estimation)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
        }
    }
}
