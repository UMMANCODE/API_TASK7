using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TASK3_Business.Dtos.AuthDtos;

namespace TASK3_Business.Validators.AuthValidators {
  public class LoginDtoValidator : AbstractValidator<LoginDto> {
    public LoginDtoValidator() {
      RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required");
      RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required")
        .MinimumLength(8).MaximumLength(20).WithMessage("Password must be between 8 and 20 characters");
    }
  }
}
