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
    public class CoursesController : Controller
    {
        private SchoolContext db = new SchoolContext();

        // GET: Courses
        public ActionResult Index()
        {
            return View(db.Courses.ToList());
        }

        // GET: Courses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        public ActionResult AddPeopleView(int? id, bool isStudent)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            List<Person> people = isStudent ? db.Students.ToList<Person>() : db.Teachers.ToList<Person>();
            people = people.Where(person => !person.Courses.Contains(course)).ToList();
            ViewData["isStudent"] = isStudent;

            return PartialView("_AddPeopleToCourse", people);
        }
        
        [HttpPost]
        public ActionResult AddPeople(int? id, int[] peopleIds, bool isStudent)
        {
            if (id == null || peopleIds == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }

            if (isStudent)
            {
                IQueryable<Student> students = db.Students.Where(student => peopleIds.Contains(student.Id));
                foreach(Student student in students)
                {
                    student.Assignments = student.Assignments.Concat(course.Assignments).ToList();
                }
                course.Students.AddRange(students);
            }
            else
                course.Teachers.AddRange(db.Teachers.Where(teacher => peopleIds.Contains(teacher.Id)));
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = course.Id, list = isStudent ? "Students" : "Teachers" });
        }

        // GET: Courses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(course);
        }

        // GET: Courses/Edit/5
        public ActionResult Edit(int? id, string list = "")
        {
            string view = "Edit" + list;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            ViewEngineResult result = ViewEngines.Engines.FindView(ControllerContext, view, null);
            if (course == null || result.View == null)
            {
                return HttpNotFound();
            }
            return View(view, course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAssignment([Bind(Include = "Id,Name,CourseId,Description")] Assignment assignment, string redirect)
        {
            if (ModelState.IsValid)
            {
                Course course = db.Courses.Find(assignment.CourseId);
                if(course != null)
                {
                    assignment.Course = course;
                    db.Assignments.Add(assignment);
                    foreach(Student student in course.Students)
                    {
                        student.Assignments.Add(assignment);
                    }
                    db.SaveChanges();
                    return Redirect(redirect);
                }
            }
            return RedirectToAction("Index");
        }

        // GET: Courses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult RemoveAssignment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            db.Assignments.Remove(assignment);
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = assignment.CourseId, list = "Assignments" });
        }

        [HttpPost]
        public ActionResult RemovePerson(int? id, int requestId, bool isStudent, string redirect)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = isStudent ? (Person)db.Students.Find(id) : db.Teachers.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            Course course = person.Courses.FirstOrDefault(c => c.Id == requestId);
            if (course == null)
            {
                return HttpNotFound();
            }
            if(isStudent)
            {
                (person as Student).Assignments.RemoveAll(assignment => course.Assignments.Contains(assignment));
            }
            person.Courses.Remove(course);
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
