using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicManagement.DAL.Entities
{
    public class Instructor
    {
        public int StudentId { get; set; }
        public Student Student { get; set; } = null;
        public int CourseId { get; set; }
        public Course Course { get; set; } = null;
        public DateTime EnrolledOn { get; set; }
    }
}
