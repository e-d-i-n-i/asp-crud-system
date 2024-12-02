namespace crud_system.Models
{
    public class Chapter
    {
        public Guid ChapterID { get; set; }

        public Guid CourseID { get; set; } // Foreign key to Course

        public string CourseName { get; set; }

        public int ChapterNumber { get; set; }

        public string ChapterVideo { get; set; }


        public string ChapterPDF { get; set; }

        // Navigation property for the related course
        public Course Course { get; set; }
    }

}
