using AdsManagement.App.DTOs.Advertisement;
using FluentValidation;

namespace AdsManagement.App.Validators.Advertisement
{
    public class UserAdvertisementFilterDtoValidator : AbstractValidator<UserAdvertisementFilterDto>
    {
        public UserAdvertisementFilterDtoValidator()
        {
            RuleFor(c => c.Page)
                .GreaterThanOrEqualTo(1).WithMessage("The page number cannot be less than 1.");

            RuleFor(c => c.PageSize)
                .InclusiveBetween(5, 30).WithMessage("Page size must be between 5 and 30.");

        }
    }
}
