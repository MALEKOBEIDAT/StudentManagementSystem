using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    public class StudentsController : Controller
    {
        private StudentManagementDBEntities db = new StudentManagementDBEntities();

        // GET: Students
        public async Task<ActionResult> Index()
        {
            if (Session["Username"] == null) // Check if user is authenticated
            {
                return RedirectToAction("Login", "Auth"); // Redirect to Login page
            }

            return View(await db.Students.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = await db.Students.FindAsync(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "StudentID,StudentNumber,NameArabic,DateOfBirth,NameEnglish,TawjehiAverage,SchoolName,NationalNumber,PhysicsGrade,MathGrade")] Student student, HttpPostedFileBase PictureFile)
        {
            if (ModelState.IsValid)
            {
                if (PictureFile != null && PictureFile.ContentLength > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(PictureFile.FileName);
                    string serverPath = Server.MapPath("~/Uploads/Images/");

                    if (!System.IO.Directory.Exists(serverPath))
                    {
                        System.IO.Directory.CreateDirectory(serverPath);
                    }

                    string filePath = System.IO.Path.Combine(serverPath, fileName);
                    PictureFile.SaveAs(filePath);
                    student.PicturePath = "/Uploads/Images/" + fileName;
                }

                db.Students.Add(student);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }

            // Extract validation errors from ModelState
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                           .Select(e => e.ErrorMessage)
                                           .ToList();

            return Json(new { success = false, message = "Validation failed.", errors = errors });
        }


        // GET: Students/Edit/5
        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var student = await db.Students.FindAsync(id);
            if (student == null)
            {
                return HttpNotFound();
            }

            return View(student);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Student student)
        {
            if (ModelState.IsValid)
            {
                // Check if StudentNumber is already in use by another student
                var existingStudentNumber = await db.Students
                    .Where(s => s.StudentNumber == student.StudentNumber && s.StudentID != student.StudentID)
                    .FirstOrDefaultAsync();

                if (existingStudentNumber != null)
                {
                    ModelState.AddModelError("StudentNumber", "This Student Number is already in use by another student.");
                }

                // Check if NationalNumber is already in use by another student
                var existingNationalNumber = await db.Students
                    .Where(s => s.NationalNumber == student.NationalNumber && s.StudentID != student.StudentID)
                    .FirstOrDefaultAsync();

                if (existingNationalNumber != null)
                {
                    ModelState.AddModelError("NationalNumber", "This National Number is already in use by another student.");
                }

                if (!ModelState.IsValid)
                {
                    return View(student); // Return the view with errors
                }

                db.Entry(student).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = await db.Students.FindAsync(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Student student = await db.Students.FindAsync(id);
            db.Students.Remove(student);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
