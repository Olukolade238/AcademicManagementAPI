using AcademicManagement.BLL.DTOs;
using AcademicManagement.BLL.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly DepartmentService _departmentService;
        private readonly IValidator<CreateDepartmentDto> _validator;
        public DepartmentController(
            DepartmentService departmentService,
            IValidator<CreateDepartmentDto> validator)
        {
            _departmentService = departmentService;
            _validator = validator;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDepartments()
        {
            return Ok(await _departmentService.GetDepartmentsAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDepartment(int id)
        {
            var department = await _departmentService.GetDepartmentAsync(id);

            if (department == null)
                return NotFound();

            return Ok(department);
        }

        // This creates a new department.
        [HttpPost]
        [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateDepartment(CreateDepartmentDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());

                return ValidationProblem(
                    new ValidationProblemDetails(errors)
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Validation failed"
                    });
            }
            var department = await _departmentService.CreateDepartmentAsync(dto);
            return CreatedAtAction(
                nameof(GetDepartment),
                new { id = department.DepartmentId },
                department);
        }
    }
}
