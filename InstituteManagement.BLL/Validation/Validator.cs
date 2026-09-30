using FluentValidation;
using AcademicManagement.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicManagement.BLL.Validation
{
    public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentDto>
    {
        public CreateDepartmentValidator()
        {
            RuleFor(d => d.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(d => d.Code)
                .NotEmpty()
                .MaximumLength(20)
                .Matches("^[A-Za-z0-9-]+$")
                .WithMessage("Code can contain only letters, numbers, and hyphens.");
        }
    }
}
