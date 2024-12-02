using System.Data;

namespace crud_system.Models
{
    public class Course
    {
        public Guid CourseID { get; set; }

        public string CourseName { get; set; } // Unique constraint will be applied via EF Fluent API

        // Navigation property for related chapters
        public ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
    }

}
