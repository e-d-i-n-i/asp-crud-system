using crud_system.Models;
using Microsoft.EntityFrameworkCore;

namespace crud_system.Data
{
    public class CourseManagementContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentAccess> StudentAccesses { get; set; }

        public CourseManagementContext(DbContextOptions<CourseManagementContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define relationships

            // Course -> Chapter: One-to-Many
            modelBuilder.Entity<Chapter>()
                .HasOne(c => c.Course)
                .WithMany(c => c.Chapters)
                .HasForeignKey(c => c.CourseID);

            // Ensure CourseName is a unique alternate key
            modelBuilder.Entity<Course>()
                .HasAlternateKey(c => c.CourseName);

            // Course -> Student: One-to-Many
            modelBuilder.Entity<Student>()
                        .HasOne(s => s.Course)
                        .WithMany(c => c.Students)
                        .HasForeignKey(s => s.CourseName)
                        .HasPrincipalKey(c => c.CourseName); // Matchs foreign key to the alternate key


            // Student -> StudentAccess: One-to-One
            modelBuilder.Entity<StudentAccess>()
                .HasOne(sa => sa.Student)
                .WithOne(s => s.StudentAccess)
                .HasForeignKey<StudentAccess>(sa => sa.StudentID);

            // Unique constraint for CourseName
            modelBuilder.Entity<Course>()
                .HasIndex(c => c.CourseName)
                .IsUnique();
                       

        }
    }
}
