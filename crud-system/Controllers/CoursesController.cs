using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using crud_system.Models;
using crud_system.Data;

public class CoursesController : Controller
{
    private readonly CourseManagementContext _context;

    public CoursesController(CourseManagementContext context)
    {
        _context = context;
    }

    // GET: Courses
    public async Task<IActionResult> Index()
    {
        return View(await _context.Courses.ToListAsync());
    }

    // GET: Courses/Create
    public IActionResult Create()
    {
        return View();
    }
    // POST: Courses/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("CourseID,CourseName,CourseChapter")] Course course)
    {
        // Check if the course name already exists in the database
        if (_context.Courses.Any(c => c.CourseName == course.CourseName))
        {
            ModelState.AddModelError("CourseName", "A course with this name already exists.");
            return View(course); // Return the view with the error message
        }

        if (ModelState.IsValid)
        {
            course.CourseID = Guid.NewGuid(); // Set the CourseID
            _context.Add(course);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Course Added successfully!";
            return RedirectToAction(nameof(Index));
        }
        return View(course);
    }


    // GET: Courses/Edit/5
    public async Task<IActionResult> Edit(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Invalid course ID.");
        }

        var course = await _context.Courses.FindAsync(id);
        if (course == null)
        {
            return NotFound("Course not found.");
        }

        return View(course);
    }

    // POST: Courses/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Course course)
    {
        if (id != course.CourseID)
        {
            return BadRequest("Course ID mismatch.");
        }

        // Check if the course name already exists in the database (excluding the current course)
        if (_context.Courses.Any(c => c.CourseName == course.CourseName && c.CourseID != id))
        {
            ModelState.AddModelError("CourseName", "A course with this name already exists.");
            return View(course); // Return the view with the error message
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Entry(course).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CourseExists(course.CourseID))
                {
                    return NotFound("Course no longer exists.");
                }
                throw;
            }

            TempData["SuccessMessage"] = "Course updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        return View(course);
    }



    // Helper method to check course existence
    private async Task<bool> CourseExists(Guid id)
    {
        return await _context.Courses.AnyAsync(e => e.CourseID == id);
    }

    // GET: Courses/Delete/5
    public async Task<IActionResult> Delete(Guid id)
    {
        if (id == Guid.Empty)
        {
            return NotFound();
        }

        var course = await _context.Courses
            .FirstOrDefaultAsync(m => m.CourseID == id);
        if (course == null)
        {
            return NotFound();
        }

        return View(course);
    }

    // POST: Courses/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course != null)
        {
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }
        TempData["SuccessMessage"] = "Course Deleted successfully!";
        return RedirectToAction(nameof(Index));
    }

    // GET: Courses/Chapters/5
    public async Task<IActionResult> Chapters(Guid id)
    {
        // Check if the course exists
        var course = await _context.Courses
            .Include(c => c.Chapters) // Include chapters in the course query
            .FirstOrDefaultAsync(c => c.CourseID == id);

        if (course == null)
        {
            return NotFound("Course not found.");
        }

        // Return a view with the list of chapters for this course
        return View(course.Chapters);
    }

    // GET: Courses/AddChapter/{courseId}
    public IActionResult AddChapter(Guid courseId)
    {
        // Fetch the course to make sure it exists
        var course = _context.Courses.FirstOrDefault(c => c.CourseID == courseId);
        if (course == null)
        {
            return NotFound("Course not found.");
        }

        // Create a new Chapter model and pass the course ID to it
        var model = new Chapter { CourseName = course.CourseName };
        return View(model);
    }
    // POST: Courses/AddChapter/{courseId}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddChapter(Guid courseId, Chapter chapter, IFormFile chapterVideo, IFormFile chapterPDF)
    {
        if (chapterVideo != null && !chapterVideo.ContentType.StartsWith("video/") && !chapterVideo.ContentType.Equals("image/gif"))
        {
            ModelState.AddModelError("ChapterVideo", "Only video files or GIFs are allowed.");
        }

        if (chapterPDF != null && !chapterPDF.ContentType.Equals("application/pdf"))
        {
            ModelState.AddModelError("ChapterPDF", "Only PDF files are allowed.");
        }

        // If there are errors in the model state, return the view with errors
        if (!ModelState.IsValid)
        {
            return View(chapter);
        }

        // Handle file uploads
        string videoFilePath = null;
        string pdfFilePath = null;

        if (chapterVideo != null)
        {
            // Generate file name
            videoFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "videos", chapterVideo.FileName);
            using (var fileStream = new FileStream(videoFilePath, FileMode.Create))
            {
                await chapterVideo.CopyToAsync(fileStream);
            }
        }

        if (chapterPDF != null)
        {
            // Generate file name
            pdfFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "pdfs", chapterPDF.FileName);
            using (var fileStream = new FileStream(pdfFilePath, FileMode.Create))
            {
                await chapterPDF.CopyToAsync(fileStream);
            }
        }

        // Set the file paths in the chapter
        chapter.ChapterVideo = videoFilePath;
        chapter.ChapterPDF = pdfFilePath;

        // Save chapter to database
        _context.Chapters.Add(chapter);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Chapter added successfully!";
        return RedirectToAction(nameof(Chapters), new { id = courseId });
    }

}
