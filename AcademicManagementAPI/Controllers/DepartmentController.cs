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
    }
}
