using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicManagement.DAL.Entities
{
    public class Instructor
    {
        public int InstructorId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!; // null! silences the nullable warning, i.e it default avoids null warnings for required text
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
