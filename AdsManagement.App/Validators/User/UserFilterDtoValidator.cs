using AdsManagement.App.DTOs.User;
using FluentValidation;
using System.Text.RegularExpressions;

namespace AdsManagement.App.Validators.User
{
    public class UserFilterDtoValidator : AbstractValidator<UserFilterDto>
    {
        public UserFilterDtoValidator()
        {
            RuleFor(c => c.Name)
                .MaximumLength(100).WithMessage("Invalid name length.")
                .Must(name => !Regex.IsMatch(name, "<script", RegexOptions.IgnoreCase))
                .WithMessage("Prohibited content")
                .When(c => c.Name != null);

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
