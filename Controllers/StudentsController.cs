using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MvcDatabase.Models;

namespace MvcDatabase.Controllers
{
    public class StudentsController : Controller
    {
        private SchoolContext db = new SchoolContext();

        // GET: Students
        public ActionResult Index()
        {
            return View(db.Students.ToList());
        }

        // GET: Students/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
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

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(student);
        }

        public ActionResult AddCoursesView(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            List<Course> courses = db.Courses.Where(course => course.Students.FirstOrDefault(s => s.Id == id) == null).ToList();
            ViewData["isStudent"] = true;

            return PartialView("_AddCoursesToPerson", courses);
        }

        [HttpPost]
        public ActionResult AddCourses(int? id, int[] courseIds)
        {
            if (id == null || courseIds == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            
            IQueryable<Course> courses = db.Courses.Where(course => courseIds.Contains(course.Id));
            foreach (Course course in courses)
            {
                student.Assignments = student.Assignments.Concat(course.Assignments).ToList();
            }
            student.Courses.AddRange(courses);
            db.SaveChanges();
            return RedirectToAction("Edit", new { id, list = "Courses" });
        }
        
        public ActionResult Edit(int? id, string list = "")
        {
            string view = "Edit" + list;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            ViewEngineResult result = ViewEngines.Engines.FindView(ControllerContext, view, null);
            if (student == null || result.View == null)
            {
                return HttpNotFound();
            }
            return View(view, student);
        }
        
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult RemoveAssignment(int? id, int requestId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(requestId);
            if (student == null)
            {
                return HttpNotFound();
            }
            student.Assignments.RemoveAt(student.Assignments.FindIndex(assignment => assignment.Id == id));
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = requestId, list = "Assignments" });
        }

        [HttpPost]
        public ActionResult RemoveCourse(int? id, int requestId, string redirect)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(requestId);
            if (student == null)
            {
                return HttpNotFound();
            }
            Course course = student.Courses.FirstOrDefault(c => c.Id == id);
            if (course == null)
            {
                return HttpNotFound();
            }
            student.Assignments.RemoveAll(assignment => course.Assignments.Contains(assignment));

            student.Courses.Remove(course);
            db.SaveChanges();
            return Redirect(redirect);
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
