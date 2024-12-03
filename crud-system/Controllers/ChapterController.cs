using crud_system.Data;
using crud_system.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace crud_system.Controllers
{
    [Authorize]
    public class ChapterController : Controller
    {
        private readonly CourseManagementContext _context;

        public ChapterController(CourseManagementContext context)
        {
            _context = context;
        }

        // GET: Chapters
        public async Task<IActionResult> Index()
        {
            return View(await _context.Chapters.ToListAsync());
        }

        // GET: Chapters/Create
        public IActionResult Create()
        {
            ViewBag.Courses = _context.Courses.ToList();  // Get courses to display in the dropdown
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Chapter chapter, IFormFile chapterVideo, IFormFile chapterPDF)
        {
            // Directory paths for video and PDF files
            string videoDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "videos");
            string pdfDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "pdfs");

            // Check if the directories exist, create them if they don't
            if (!Directory.Exists(videoDirectory))
            {
                Directory.CreateDirectory(videoDirectory);
                Console.WriteLine("Created video directory: " + videoDirectory);
            }

            if (!Directory.Exists(pdfDirectory))
            {
                Directory.CreateDirectory(pdfDirectory);
                Console.WriteLine("Created pdf directory: " + pdfDirectory);
            }

            // Validate Course
            if (!_context.Courses.Any(c => c.CourseName == chapter.CourseName))
            {
                ModelState.AddModelError("CourseName", "Selected course does not exist.");
                ViewBag.Courses = _context.Courses.ToList();
                return View(chapter);
            }

            // Validate Files
            if (chapterVideo != null)
            {
                if (!chapterVideo.ContentType.StartsWith("video/") && !chapterVideo.ContentType.Equals("image/gif"))
                {
                    ModelState.AddModelError("ChapterVideo", "Only video files or GIFs are allowed.");
                }
                else
                {
                    string videoFilePath = Path.Combine(videoDirectory, chapterVideo.FileName);
                    Console.WriteLine("Saving video file to: " + videoFilePath);
                    using (var fileStream = new FileStream(videoFilePath, FileMode.Create))
                    {
                        await chapterVideo.CopyToAsync(fileStream);
                    }
                    chapter.ChapterVideo = "/uploads/videos/" + chapterVideo.FileName;
                }
            }

            if (chapterPDF != null)
            {
                if (!chapterPDF.ContentType.Equals("application/pdf"))
                {
                    ModelState.AddModelError("ChapterPDF", "Only PDF files are allowed.");
                }
                else
                {
                    string pdfFilePath = Path.Combine(pdfDirectory, chapterPDF.FileName);
                    Console.WriteLine("Saving PDF file to: " + pdfFilePath);
                    using (var fileStream = new FileStream(pdfFilePath, FileMode.Create))
                    {
                        await chapterPDF.CopyToAsync(fileStream);
                    }
                    chapter.ChapterPDF = "/uploads/pdfs/" + chapterPDF.FileName;
                }
            }

            // Logging the values before saving
            Console.WriteLine("Chapter Video Path: " + chapter.ChapterVideo);
            Console.WriteLine("Chapter PDF Path: " + chapter.ChapterPDF);

            // If validation is successful, proceed to save the chapter
            
                chapter.ChapterID = Guid.NewGuid(); // Assign a new ChapterID
                _context.Add(chapter);

                // Increment chapter count for the associated course
                var course = _context.Courses.FirstOrDefault(c => c.CourseName == chapter.CourseName);
                if (course != null)
                {
                    course.CourseChapter++;
                    _context.Update(course);
                }

                // Save changes to the database
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Chapter added successfully!";
                    return RedirectToAction(nameof(Index));  // Redirect to the list of chapters
                }
                catch (Exception ex)
                {
                    // Log the error (could be logging framework or Console)
                    Console.WriteLine("Error saving chapter: " + ex.Message);
                    TempData["ErrorMessage"] = "Error occurred while saving the chapter.";
                    return View(chapter);
                }
            

        }

        // GET: Chapters/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid Chapter ID.");
            }

            var chapter = await _context.Chapters.FindAsync(id);
            if (chapter == null)
            {
                return NotFound("Chapter not found.");
            }

            ViewBag.Courses = _context.Courses.Select(c => new { c.CourseName }).ToList();
            return View(chapter);
        }

        // POST: Chapters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Chapter chapter, IFormFile chapterVideo, IFormFile chapterPDF)
        {
            if (id != chapter.ChapterID)
            {
                return BadRequest("Chapter ID mismatch.");
            }

            if (!_context.Courses.Any(c => c.CourseName == chapter.CourseName))
            {
                ModelState.AddModelError("CourseName", "Selected course does not exist.");
            }

            // Directory paths for video and PDF files
            string videoDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "videos");
            string pdfDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "pdfs");

            // Check if the directories exist, create them if they don't
            if (!Directory.Exists(videoDirectory))
            {
                Directory.CreateDirectory(videoDirectory);
            }

            if (!Directory.Exists(pdfDirectory))
            {
                Directory.CreateDirectory(pdfDirectory);
            }

            // Handle file uploads if new files are provided
            if (chapterVideo != null)
            {
                if (!chapterVideo.ContentType.StartsWith("video/") && !chapterVideo.ContentType.Equals("image/gif"))
                {
                    ModelState.AddModelError("ChapterVideo", "Only video files or GIFs are allowed.");
                }
                else
                {
                    string videoFilePath = Path.Combine(videoDirectory, chapterVideo.FileName);
                    using (var fileStream = new FileStream(videoFilePath, FileMode.Create))
                    {
                        await chapterVideo.CopyToAsync(fileStream);
                    }
                    chapter.ChapterVideo = "/uploads/videos/" + chapterVideo.FileName;
                }
            }

            if (chapterPDF != null)
            {
                if (!chapterPDF.ContentType.Equals("application/pdf"))
                {
                    ModelState.AddModelError("ChapterPDF", "Only PDF files are allowed.");
                }
                else
                {
                    string pdfFilePath = Path.Combine(pdfDirectory, chapterPDF.FileName);
                    using (var fileStream = new FileStream(pdfFilePath, FileMode.Create))
                    {
                        await chapterPDF.CopyToAsync(fileStream);
                    }
                    chapter.ChapterPDF = "/uploads/pdfs/" + chapterPDF.FileName;
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chapter);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Chapter updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ChapterExists(chapter.ChapterID))
                    {
                        return NotFound("Chapter no longer exists.");
                    }
                    throw;
                }
            }

            ViewBag.Courses = _context.Courses.Select(c => new { c.CourseName }).ToList();
            return View(chapter);
        }

        // POST: Chapters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var chapter = await _context.Chapters.FindAsync(id);
            if (chapter != null)
            {
                var course = _context.Courses.FirstOrDefault(c => c.CourseName == chapter.CourseName);
                if (course != null)
                {
                    course.CourseChapter--;
                }

                _context.Chapters.Remove(chapter);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Chapter deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ChapterExists(Guid id)
        {
            return await _context.Chapters.AnyAsync(e => e.ChapterID == id);
        }
    }
}
