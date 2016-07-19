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

      Get["/courses/new"]=_=>{
        List<Department> departments = Department.GetAll();
        return View["course_new.cshtml" , departments];
    };

      Post["/courses/new"]=_=>
      {
        Course newCourse = new Course(Request.Form["course-name"], Request.Form["department"]);
        newCourse.Save();
        List<Course> allCourses = Course.GetAll();
        return View["courses.cshtml", allCourses];
      };

      // Get["courses/{id}"] = parameters =>
      // {
      //   Course course = Course.Find(parameters.id);
      //   return View["course.cshtml", course];
      // };

      Get["students/{id}"] = parameters =>
      {
        Student student = Student.Find(parameters.id);
        return View["student.cshtml", student];
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
      Post["/students/{id}/addCourse"] = parameters =>
      {
        Student student = Student.Find(parameters.id);
        student.AddCourse(Request.Form["course"]);
        return View["student.cshtml", student];
      };
      Post["/students/{id}/deleteCourse"] = parameters =>
      {
        Student student = Student.Find(parameters.id);
        student.DropCourse(Request.Form["course"]);
        return View["student.cshtml", student];
      };

      Post["/students/{id}/updatemajors"]=parameters=>
      {
        Student student = Student.Find(parameters.id);
        int majorCount = student.GetMajors().Count;
        if(majorCount == 0 && Request.Form["new-major"] !=0)
        {
          student.AddMajor(Request.Form["new-major"]);
        }
        else if(majorCount == 1)
        {
          if(Request.Form["current-major"]==0)
          {
            student.DropMajor(Request.Form["current-major"]);
          }
          else if(Request.Form["current-major"]!=student.GetMajors()[0].GetId())
          {
            student.DropMajor(student.GetMajors()[0].GetId());
            student.AddMajor(Request.Form["current-major"]);
          }
          if(Request.Form["additional-major"] !=0)
          {
            student.AddMajor(Request.Form["additonal-major"]);
          }
        }
        else
        {
          if(Request.Form["first-major"]==0)
          {
            student.DropMajor(Request.Form["first-major"]);
          }
          else if(Request.Form["first-major"]!=student.GetMajors()[0].GetId())
          {
            student.DropMajor(student.GetMajors()[0].GetId());
            student.AddMajor(Request.Form["first-major"]);
          }
          if(Request.Form["second-major"]==0)
          {
            student.DropMajor(Request.Form["second-major"]);
          }
          else if(Request.Form["second-major"]!=student.GetMajors()[1].GetId())
          {
            student.DropMajor(student.GetMajors()[1].GetId());
            student.AddMajor(Request.Form["second-major"]);
          }
        }
        return View["student.cshtml", student];
      };

      Get["/departments"]=_=>
      {
        List<Department> departments = Department.GetAll();
        return View["departments.cshtml", departments];
      };

      Get["/departments/new"]=_=>View["department_new.cshtml"];

      Post["/departments/new"]=_=>
      {
        Department department = new Department(Request.Form["department-name"]);
        department.Save();
        List<Department> departments = Department.GetAll();
        return View["departments.cshtml", departments];
      };

      Get["/departments/{id}"]=parameters=>
      {
        Department department = Department.Find(parameters.id);
        return View["department.cshtml", department];
      };

    }
  }
}
