using AcademicManagement.BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly CourseService _courseService;

        public CourseController(CourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            return Ok(await _courseService.GetCoursesAsync());
        }
    }
}
