using AdsManagement.App.DTOs.Advertisement;
using FluentValidation;
using System.Text.RegularExpressions;

namespace AdsManagement.App.Validators.Advertisement
{
    public class UpdateAdvertisementDtoValidator : AbstractValidator<UpdateAdvertisementDto>
    {
        public UpdateAdvertisementDtoValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty().WithMessage("The ID cannot be empty");

            RuleFor(c => c.Title)
                .MaximumLength(120).WithMessage("Invalid title length.")
                .Must(title => !Regex.IsMatch(title, "<script", RegexOptions.IgnoreCase))
                .WithMessage("Prohibited content");

            RuleFor(c => c.Text)
                .MaximumLength(3000).WithMessage("Invalid text length.")
                .Must(text => !Regex.IsMatch(text, "<script", RegexOptions.IgnoreCase))
                .WithMessage("Prohibited content");
        }
    }
}
