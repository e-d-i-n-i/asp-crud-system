﻿using System.ComponentModel.DataAnnotations;

namespace crud_system.Models
{
    public class Chapter
    {
        [Key]
        public Guid ChapterID { get; set; }

        public string CourseName { get; set; }

        public int ChapterNumber { get; set; }

        public string ChapterVideo { get; set; }


        public string ChapterPDF { get; set; }

        // Navigation property for the related course
        public Course Course { get; set; }
    }

}
