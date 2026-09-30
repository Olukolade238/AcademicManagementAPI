using AcademicManagement.BLL.DTOs;
using AcademicManagement.DAL.Repository;
using AcademicManagement.BLL.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstituteManagement.BLL.Services
{
    public class CourseService
    {
        private readonly CourseRepo _repository;

        public CourseService(CourseRepo repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesAsync()
        {
            var courses = await _repository.GetAllAsync();
            return courses.ToDtoList();
        }
    }
}
