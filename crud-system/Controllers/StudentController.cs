﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using crud_system.Models;
using crud_system.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

public class StudentsController : Controller
{
    private readonly CourseManagementContext _context;

    public StudentsController(CourseManagementContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        // Get the total count of students for pagination
        var totalStudents = await _context.Students.CountAsync();

        // Fetch students for the current page with course data loaded
        var students = await _context.Students
                                      .Include(s => s.Course)  // Ensure Course data is loaded
                                      .Skip((pageNumber - 1) * pageSize)
                                      .Take(pageSize)
                                      .ToListAsync();

        // Prepare pagination information
        var model = new StudentIndexViewModel
        {
            Students = students,
            TotalStudents = totalStudents,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalStudents / pageSize)
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        // Populate any needed dropdowns or data for the view here
        // For example, passing a list of courses to the view (if needed)
        ViewBag.Courses = _context.Courses.ToList(); // If you need to select a Course

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateStudentViewModel model)
    {
        if (model == null)
        {
            // Return an error or handle the case where the model object is null
            ViewBag.ErrorMessage = "Student object cannot be null.";
            return View();
        }

        // Ensure that you are assigning a valid Course to the student.
        var course = await _context.Courses
                                   .FirstOrDefaultAsync(c => c.CourseName == model.CourseName);

        if (course == null)
        {
            // If no matching course is found, return an error message
            ModelState.AddModelError("CourseName", "The selected course does not exist.");
        }

        // If model is valid and course is found, proceed with creating the Student and StudentAccess
        if (ModelState.IsValid)
        {
            try
            {
                // Create StudentAccess record first
                var studentAccess = new StudentAccess
                {
                    AccessNumber = Guid.NewGuid(),
                    Username = $"{model.FirstName.ToLower()}.{model.LastName.ToLower()}",  // Simple Username logic
                    Password = "TempPassword123",  // Temporary or generated password (you should hash it before storing in production)
                    CourseName = model.CourseName,
                    CourseType = model.ClassType,  // Assuming ClassType correlates to CourseType
                };

                // Create the full Student object
                var newStudent = new Student
                {
                    StudentID = Guid.NewGuid(),  // Generate a unique StudentID
                    FirstName = model.FirstName,
                    MiddleName = model.MiddleName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    ClassType = model.ClassType,
                    CourseName = model.CourseName,
                    Course = course,  // Assign the course object
                    StudentAccess = studentAccess  // Assign the related StudentAccess object
                };

                // Add the new student and student access to the database
                await _context.Students.AddAsync(newStudent);
                await _context.StudentAccesses.AddAsync(studentAccess);  // Add the StudentAccess record
                await _context.SaveChangesAsync();

                // Redirect to the Index page after successful creation
                TempData["SuccessMessage"] = "Student created successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Handle errors
                ViewBag.ErrorMessage = "An error occurred while creating the student. Please try again.";
                ViewBag.Courses = _context.Courses.ToList();  // Pass courses if needed
                return View(model);
            }
        }

        // If ModelState is invalid, return the same view with validation errors
        ViewBag.Courses = _context.Courses.ToList();  // Pass available courses to the view
        return View(model);
    }



}
