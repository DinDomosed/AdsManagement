using AdsManagement.App.DTOs.Advertisement;
using FluentValidation;
using System.Text.RegularExpressions;

namespace AdsManagement.App.Validators.Advertisement
{
    public class AdFilterDtoValidator : AbstractValidator<AdFilterDto>
    {
        public AdFilterDtoValidator()
        {
            RuleFor(c => c.Title)
                .MaximumLength(120)
                .WithMessage("Invalid title length.")
                .Must(title => !Regex.IsMatch(title, "<script", RegexOptions.IgnoreCase))
                .WithMessage("Prohibited content")
                .When(c => c.Title != null);

            RuleFor(c => c.Text)
                .MaximumLength(200).WithMessage("Invalid text length")
                .Must(text => !Regex.IsMatch(text, "<script", RegexOptions.IgnoreCase))
                .WithMessage("Prohibited content")
                .When(c => c.Text != null);

            RuleFor(c => c.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5")
                .When(c => c.Rating.HasValue);

            RuleFor(c => c.Number)
                .GreaterThan(0).WithMessage("Ad number must be greater than 0.")
                .When(c => c.Number.HasValue);

            RuleFor(c => c.CreatedDateFrom)
                .LessThanOrEqualTo(c => c.CreatedDateTo).WithMessage("The start date cannot be later than end date.")
                .When(c => c.CreatedDateTo.HasValue && c.CreatedDateFrom.HasValue);

            RuleFor(c => c.CreatedDateTo)
                .GreaterThanOrEqualTo(c => c.CreatedDateFrom).WithMessage("The end date cannot be erlier than start date.")
                .When(c => c.CreatedDateFrom.HasValue && c.CreatedDateTo.HasValue);

            RuleFor(c => c.ExpiresDateFrom)
                .LessThanOrEqualTo(c => c.ExpiresDateTo).WithMessage("The start date cannot be later than end date.")
                .When(c => c.ExpiresDateFrom.HasValue && c.ExpiresDateTo.HasValue);

            RuleFor(c => c.ExpiresDateTo)
                .GreaterThanOrEqualTo(c => c.ExpiresDateFrom).WithMessage("The end date cannot be erlier than start date.")
                .When(c => c.ExpiresDateFrom.HasValue && c.ExpiresDateTo.HasValue);

            RuleFor(c => c.SortBy)
             .MaximumLength(20).WithMessage("Incorrect length.")
             .When(c => c.SortBy != null);

            RuleFor(c => c.Page)
              .GreaterThanOrEqualTo(1).WithMessage("The page number cannot be less than 1.");

            RuleFor(c => c.PageSize)
                .GreaterThanOrEqualTo(5).WithMessage("Page size cannot be less than 5")
                .LessThanOrEqualTo(30).WithMessage("Page size cannot be greater than 30");

        }
    }
}
