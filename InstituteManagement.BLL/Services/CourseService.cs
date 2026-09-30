using AcademicManagement.BLL.DTOs;
using AcademicManagement.DAL.Repository;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public CourseService(CourseRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesAsync()
        {
            var courses = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<CourseDto>>(courses);
        }
    }
}
