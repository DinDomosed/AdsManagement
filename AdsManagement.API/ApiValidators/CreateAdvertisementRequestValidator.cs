using AdsManagement.API.Requests;
using FluentValidation;
using System.Text.RegularExpressions;

namespace AdsManagement.API.ApiValidators
{
    public class CreateAdvertisementRequestValidator : AbstractValidator<CreateAdvertisementRequest>
    {
        public CreateAdvertisementRequestValidator()
        {
            RuleFor(c => c.UserId)
               .NotEmpty().WithMessage("The user ID cannot be empty");

            RuleFor(c => c.Title)
                .MaximumLength(120).WithMessage("Invalid title length")
                .Must(title => !Regex.IsMatch(title, "<script", RegexOptions.IgnoreCase))
                .WithMessage("Prohibited content");

            RuleFor(c => c.Text)
                .MaximumLength(3000)
                .WithMessage("Invalid text length")
                .Must(text => !Regex.IsMatch(text, "<script", RegexOptions.IgnoreCase))
                .WithMessage("Prohibited content");

            RuleFor(c => c.FormFile)
                .Must(c => c.ContentType.StartsWith("image/"))
                .WithMessage("The file must be an image.")
                .When(c => c.FormFile != null); 
        }
    }
}
