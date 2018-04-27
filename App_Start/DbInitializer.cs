using MvcDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcDatabase
{
    public class DbInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<SchoolContext>
    {
        protected override void Seed(SchoolContext context)
        {
            var students = new List<Student>
            {
            new Student{Id=1, Name="Carl", Courses=new List<Course>()},
            new Student{Id=2, Name="Lukas", Courses=new List<Course>()},
            new Student{Id=3, Name="Calvin", Courses=new List<Course>()},
            new Student{Id=4, Name="Fredrik", Courses=new List<Course>()},
            new Student{Id=5, Name="Kent", Courses=new List<Course>()},
            new Student{Id=6, Name="Edvin", Courses=new List<Course>()},
            new Student{Id=7, Name="Simon", Courses=new List<Course>()},
            new Student{Id=8, Name="Henrik", Courses=new List<Course>()}
            };

            var teachers = new List<Teacher>
            {
            new Teacher{Id=9, Name="Carl", Courses=new List<Course>()},
            new Teacher{Id=10, Name="Lukas", Courses=new List<Course>()},
            new Teacher{Id=11, Name="Calvin", Courses=new List<Course>()},
            new Teacher{Id=12, Name="Fredrik", Courses=new List<Course>()},
            new Teacher{Id=13, Name="Kent", Courses=new List<Course>()},
            new Teacher{Id=14, Name="Edvin", Courses=new List<Course>()}
            };

            var courses = new List<Course>
            {
            new Course{Id=1,Name="Chemistry",Assignments=new List<Assignment>{
                new Assignment{ Name="C1", Description="", CourseId=1},
                new Assignment{ Name="C2", Description="Description", CourseId=1},
                new Assignment{ Name="C3", Description="", CourseId=1},
                new Assignment{ Name="C4", Description="", CourseId=1},
                new Assignment{ Name="C5", Description="Description as well", CourseId=1},
                new Assignment{ Name="C6", Description="", CourseId=1}
            }, Students=new List<Student>(), Teachers=new List<Teacher>() },
            new Course{Id=2,Name="Programming",Assignments=new List<Assignment>{
                new Assignment{ Name="P1", Description="", CourseId=2},
                new Assignment{ Name="P2", Description="Description", CourseId=2},
                new Assignment{ Name="P3", Description="", CourseId=2},
                new Assignment{ Name="P4", Description="", CourseId=2},
                new Assignment{ Name="P5", Description="Description as well", CourseId=2},
                new Assignment{ Name="P6", Description="", CourseId=2}
            }, Students=new List<Student>(), Teachers=new List<Teacher>() },
            new Course{Id=3,Name="History",Assignments=new List<Assignment>{
                new Assignment{ Name="H1", Description="", CourseId=3},
                new Assignment{ Name="H2", Description="Description", CourseId=3},
                new Assignment{ Name="H3", Description="", CourseId=3},
                new Assignment{ Name="H4", Description="", CourseId=3},
                new Assignment{ Name="H5", Description="Description as well", CourseId=3},
                new Assignment{ Name="H6", Description="", CourseId=3}
            }, Students=new List<Student>(), Teachers=new List<Teacher>() },

            };

            int[][] relations = new int[][] {
                new int[]{1,1}, new int[]{1,2}, new int[]{1,3},
                new int[]{2,2},
                new int[]{3,1}, new int[]{3,2},
                new int[]{4,3},
                new int[]{5,2}, new int[]{5,3},
                new int[]{6,2}, new int[]{6,3},
                new int[]{7,1}, new int[]{7,2}, new int[]{7,3},

                new int[]{9,1},
                new int[]{10,2},
                new int[]{11,3},
                new int[]{12,1},
                new int[]{13,2},
                new int[]{14,3}
            };
            
            foreach(int[] relation in relations)
            {
                students.ForEach(student => {
                    if (student.Id == relation[0])
                    {
                        Course course = courses.Find(c => c.Id == relation[1]);
                        course.Students.Add(student);
                        student.Courses.Add(course);
                        student.Assignments = course.Assignments;
                    }
                });
                teachers.ForEach(teacher => {
                    if (teacher.Id == relation[0])
                    {
                        Course course = courses.Find(c => c.Id == relation[1]);
                        course.Teachers.Add(teacher);
                        teacher.Courses.Add(course);
                    }
                });
            }

            var assignments = new List<Assignment>();
            foreach(Course course in courses)
            {
                assignments = assignments.Concat(course.Assignments).ToList();
            }

            courses.ForEach(s => context.Courses.Add(s));
            assignments.ForEach(s => context.Assignments.Add(s));
            teachers.ForEach(s => context.Teachers.Add(s));
            students.ForEach(s => context.Students.Add(s));
            context.SaveChanges();
        }
    }
}