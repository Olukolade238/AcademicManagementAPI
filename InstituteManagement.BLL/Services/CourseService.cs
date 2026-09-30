using AcademicManagement.BLL.DTOs;
using AcademicManagement.DAL.Repository;
using AcademicManagement.BLL.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicManagement.BLL.Services
{
    public class CourseService
    {
        private readonly CourseRepo _repo;
        public CourseService(CourseRepo repo)
        {
            _repo = repo;
        }
        public async Task<IEnumerable<CourseDto>> GetCoursesAsync()
        {
            var courses = await _repo.GetAllAsync();
            return courses.ToDtoList();
        }
    }
}
