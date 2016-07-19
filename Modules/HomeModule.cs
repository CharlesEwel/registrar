using Nancy;
using System.Collections.Generic;
using System;
using Registrar.Objects;

namespace Registrar
{
  public class HomeModule : NancyModule
  {
    public HomeModule()
    {
      Get["/"]=_=>View["index.cshtml"];

      Get["/courses"]=_=>
      {
        List<Course> allCourses = Course.GetAll();
        return View["courses.cshtml", allCourses];
      };

      Get["/courses/new"]=_=>View["course_new.cshtml"];

      Post["/courses/new"]=_=>
      {
        Course newCourse = new Course(Request.Form["course-name"], Request.Form["department"]);
        newCourse.Save();
        List<Course> allCourses = Course.GetAll();
        return View["courses.cshtml", allCourses];
      };

      Get["/students"]=_=>
      {
        List<Student> allStudents = Student.GetAll();
        return View["students.cshtml", allStudents];
      };

      Get["/students/new"]=_=>View["student_new.cshtml"];

      Post["/students/new"]=_=>
      {
        Student newStudent = new Student(Request.Form["student-name"], Request.Form["enrollment-date"]);
        newStudent.Save();
        List<Student> allStudents = Student.GetAll();
        return View["students.cshtml", allStudents];
      };
    }
  }
}
