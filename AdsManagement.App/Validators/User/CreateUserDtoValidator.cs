using AdsManagement.App.DTOs.User;
using FluentValidation;
using System.Text.RegularExpressions;

namespace AdsManagement.App.Validators.User
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Name cannot be empty")
                .NotNull().WithMessage("Name cannot be null")
                .MaximumLength(100).WithMessage("Invalid name length")
                .Must(name => !Regex.IsMatch(name, "<script", RegexOptions.IgnoreCase))
                .WithMessage("Prohibited content");
        }
    }
}
