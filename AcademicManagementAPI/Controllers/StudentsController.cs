using AcademicManagement.BLL.DTOs;
using AcademicManagement.BLL.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly StudentService _studentService;
        private readonly IValidator<CreateStudentDto> _createValidator;
        private readonly IValidator<UpdateStudentDto> _updateValidator;
        private readonly IValidator<PatchStudentDto> _patchValidator;
        private readonly IValidator<CreateEnrollmentDto> _enrollmentValidator;
        public StudentsController(
            StudentService studentService,
            IValidator<CreateStudentDto> createValidator,
            IValidator<UpdateStudentDto> updateValidator,
            IValidator<PatchStudentDto> patchValidator,
            IValidator<CreateEnrollmentDto> enrollmentValidator)
        {
            _studentService = studentService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _patchValidator = patchValidator;
            _enrollmentValidator = enrollmentValidator;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudents(
            string? search,
            int? departmentId,
            int pageNumber = 1,
            int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Invalid pagination",
                    Detail = "pageNumber must be at least 1 and pageSize must be between 1 and 100."
                });
            }
            var result = await _studentService.GetStudentsAsync(
                search, departmentId, pageNumber, pageSize);
            Response.Headers.Append("X-Total-Count", result.TotalCount.ToString());
            return Ok(result.Items);
        }

        /// Gets a student by ID.
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudent(int id)
        {
            var student = await _studentService.GetStudentAsync(id);
            if (student == null)
                return NotFound();
            return Ok(student);
        }

        // Creates a new student.
        [HttpPost]
        [ProducesResponseType(typeof(StudentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateStudent(CreateStudentDto dto)
        {
            var validation = await _createValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ValidationFailed(validation);
            var student = await _studentService.CreateStudentAsync(dto);
            return CreatedAtAction(
                nameof(GetStudent),
                new { id = student.StudentId },
                student);
        }

        // Replaces all editable fields of an existing student.
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateStudent(int id, UpdateStudentDto dto)
        {
            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ValidationFailed(validation);
            var updated = await _studentService.UpdateStudentAsync(id, dto);
            if (!updated)
                return NotFound();
            return NoContent();
        }

        // Partially updates one or more student fields.
        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> PatchStudent(int id, PatchStudentDto dto)
        {
            var validation = await _patchValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ValidationFailed(validation);
            var updated = await _studentService.PatchStudentAsync(id, dto);
            if (!updated)
                return NotFound();
            return NoContent();
        }

        // Deletes a student.
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var deleted = await _studentService.DeleteStudentAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }

        // Enrolls an existing student in an existing course.
        [HttpPost("{studentId:int}/enrollments")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> EnrollStudent(int studentId, CreateEnrollmentDto dto)
        {
            var validation = await _enrollmentValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ValidationFailed(validation);
            await _studentService.EnrollStudentAsync(studentId, dto);
            return StatusCode(StatusCodes.Status201Created);
        }

        // Turns FluentValidation errors into a 400 ValidationProblemDetails response
        private ActionResult ValidationFailed(ValidationResult validation)
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
    }
}
