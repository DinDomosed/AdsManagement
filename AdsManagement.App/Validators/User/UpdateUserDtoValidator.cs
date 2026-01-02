using AdsManagement.App.DTOs.User;
using FluentValidation;
using System.Text.RegularExpressions;

namespace AdsManagement.App.Validators.User
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty().WithMessage("The ID cannot be empty");

            RuleFor(c => c.Name)
               .NotEmpty().WithMessage("Name cannot be empty")
               .NotNull().WithMessage("Name cannot be null")
               .MaximumLength(100).WithMessage("Invalid name length")
               .Must(name => !Regex.IsMatch(name, "<script", RegexOptions.IgnoreCase))
               .WithMessage("Prohibited content");

        }
    }
}
