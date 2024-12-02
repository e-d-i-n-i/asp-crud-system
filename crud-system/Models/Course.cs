using System.Data;

namespace crud_system.Models
{
    public class Course
    {
        public Guid CourseID { get; set; }

        public string CourseName { get; set; } // Unique constraint will be applied via EF Fluent API

        public int CourseChapter { get; set; } // Holds the number of chapters in the course

        // Navigation property for related chapters
        public ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();

        // Navigation property for related students
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }


}
