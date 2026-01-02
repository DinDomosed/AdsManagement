using AdsManagement.App.DTOs.Role;
using FluentValidation;
using System.Text.RegularExpressions;

namespace AdsManagement.App.Validators.Role
{
    public class CreateRoleDtoValidator : AbstractValidator<CreateRoleDto>
    {
        public CreateRoleDtoValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("The name cannot be empty")
                .MaximumLength(100).WithMessage("Invalid name length")
                .Must(name => !Regex.IsMatch(name, "<script", RegexOptions.IgnoreCase))
                .WithMessage("Prohibited content");
        }
    }
}
