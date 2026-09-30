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
    public class CreateStudentValidator : AbstractValidator<CreateStudentDto>
    {
        public CreateStudentValidator()
        {
            RuleFor(s => s.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(s => s.LastName).NotEmpty().MaximumLength(50);
            RuleFor(s => s.Email).NotEmpty().EmailAddress().MaximumLength(100);
            RuleFor(s => s.DepartmentId).GreaterThan(0);
        }
    }
    public class UpdateStudentValidator : AbstractValidator<UpdateStudentDto>
    {
        public UpdateStudentValidator()
        {
            RuleFor(s => s.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(s => s.LastName).NotEmpty().MaximumLength(50);
            RuleFor(s => s.Email).NotEmpty().EmailAddress().MaximumLength(100);
            RuleFor(s => s.DepartmentId).GreaterThan(0);
        }
    }
    public class PatchStudentValidator : AbstractValidator<PatchStudentDto>
    {
        public PatchStudentValidator()
        {
            RuleFor(s => s.FirstName)
                .MaximumLength(50)
                .When(s => s.FirstName != null);

            RuleFor(s => s.LastName)
                .MaximumLength(50)
                .When(s => s.LastName != null);

            RuleFor(s => s.Email)
                .EmailAddress()
                .MaximumLength(100)
                .When(s => s.Email != null);

            RuleFor(s => s.DepartmentId)
                .GreaterThan(0)
                .When(s => s.DepartmentId.HasValue);

            RuleFor(s => s)
                .Must(s => s.FirstName != null ||
                           s.LastName != null ||
                           s.Email != null ||
                           s.DepartmentId.HasValue)
                .WithMessage("At least one student field must be supplied.");
        }
    }
    public class CreateEnrollmentValidator : AbstractValidator<CreateEnrollmentDto>
    {
        public CreateEnrollmentValidator()
        {
            RuleFor(e => e.CourseId).GreaterThan(0);
        }
    }
}
