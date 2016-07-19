using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Registrar.Objects
{
  public class Course
  {
    private int _id;
    private string _name;
    private int _departmentId;

    public Course(string name, int departmentId, int Id = 0)
    {
      _id = Id;
      _name = name;
      _departmentId = departmentId;
    }

    public override bool Equals(System.Object otherCourse)
    {
      if (!(otherCourse is Course))
      {
        return false;
      }
      else
      {
        Course newCourse = (Course) otherCourse;
        bool idEquality = (this.GetId() == newCourse.GetId());
        bool nameEquality = (this.GetName() == newCourse.GetName());
        bool departmentEquality = (this.GetDepartmentID() == newCourse.GetDepartmentID());
        return (idEquality && nameEquality && departmentEquality);
      }
    }

    public int GetId()
    {
      return _id;
    }

    public string GetName()
    {
      return _name;
    }

    public int GetDepartmentID()
    {
      return _departmentId;
    }

    public static List<Course> GetAll()
    {

      List<Course> allCourses = new List<Course> {};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM courses;", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        string courseName = rdr.GetString(0);
        int courseDate = rdr.GetInt32(1);
        int courseId = rdr.GetInt32(2);
        Course newCourse = new Course(courseName, courseDate, courseId);
        allCourses.Add(newCourse);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allCourses;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO courses (name, department_id) OUTPUT INSERTED.id VALUES (@CourseName, @CourseDepartmentID);", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@CourseName";
      nameParameter.Value = this.GetName();


      SqlParameter departmentIdParameter = new SqlParameter();
      departmentIdParameter.ParameterName = "@CourseDepartmentID";
      departmentIdParameter.Value = this.GetDepartmentID();

      cmd.Parameters.Add(nameParameter);
      cmd.Parameters.Add(departmentIdParameter);

      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

    }

    public static Course Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM courses WHERE id = @CourseId;", conn);
      SqlParameter courseIdParameter = new SqlParameter();
      courseIdParameter.ParameterName = "@CourseId";
      courseIdParameter.Value = id.ToString();
      cmd.Parameters.Add(courseIdParameter);
      rdr = cmd.ExecuteReader();


      int foundCourseId = 0;
      string foundCourseName = null;
      int foundCourseDepartmentID = 0;

      while(rdr.Read())
      {
        foundCourseName = rdr.GetString(0);
        foundCourseDepartmentID = rdr.GetInt32(1);
        foundCourseId = rdr.GetInt32(2);
      }
      Course foundCourse = new Course(foundCourseName, foundCourseDepartmentID, foundCourseId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundCourse;
    }

    public List<Department> GetAllDepartments()
    {
      return Department.GetAll();
    }

    public static void DeleteAll()
     {
       SqlConnection conn = DB.Connection();
       conn.Open();
       SqlCommand cmd = new SqlCommand("DELETE FROM courses; DELETE FROM class_enrollment", conn);
       cmd.ExecuteNonQuery();
     }

  }
}
