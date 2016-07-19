using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Registrar.Objects
{
  public class Student
  {
    private int _id;
    private string _name;
    private DateTime? _enrollmentDate;

    public Student(string name, DateTime? date, int Id = 0)
    {
      _id = Id;
      _name = name;
      _enrollmentDate = date;
    }

    public override bool Equals(System.Object otherStudent)
    {
      if (!(otherStudent is Student))
      {
        return false;
      }
      else
      {
        Student newStudent = (Student) otherStudent;
        bool idEquality = (this.GetId() == newStudent.GetId());
        bool nameEquality = (this.GetName() == newStudent.GetName());
        bool enrollmentDateEquality = (this.GetEnrollmentDate() == newStudent.GetEnrollmentDate());
        return (idEquality && nameEquality && enrollmentDateEquality);
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

    public DateTime? GetEnrollmentDate()
    {
      return _enrollmentDate;
    }

    public static List<Student> GetAll()
    {
      List<Student> allStudents = new List<Student>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM students ORDER BY enrollment_date;", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        string studentDescription = rdr.GetString(0);
        DateTime? studentDate = rdr.GetDateTime(1);
        int studentId = rdr.GetInt32(2);
        Student newStudent = new Student(studentDescription, studentDate, studentId);
        allStudents.Add(newStudent);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allStudents;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO students (name, enrollment_date) OUTPUT INSERTED.id VALUES (@StudentName, @StudentEnrollmentDate);", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@StudentName";
      nameParameter.Value = this.GetName();


      SqlParameter enrollmentDateParameter = new SqlParameter();
      enrollmentDateParameter.ParameterName = "@StudentEnrollmentDate";
      enrollmentDateParameter.Value = this.GetEnrollmentDate();

      cmd.Parameters.Add(nameParameter);
      cmd.Parameters.Add(enrollmentDateParameter);

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

    public static Student Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM students WHERE id = @StudentId;", conn);
      SqlParameter studentIdParameter = new SqlParameter();
      studentIdParameter.ParameterName = "@StudentId";
      studentIdParameter.Value = id.ToString();
      cmd.Parameters.Add(studentIdParameter);
      rdr = cmd.ExecuteReader();


      int foundStudentId = 0;
      string foundStudentName = null;
      DateTime? foundStudentEnrollmentDate = null;

      while(rdr.Read())
      {
        foundStudentName = rdr.GetString(0);
        foundStudentEnrollmentDate = rdr.GetDateTime(1);
        foundStudentId = rdr.GetInt32(2);
      }
      Student foundStudent = new Student(foundStudentName, foundStudentEnrollmentDate, foundStudentId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundStudent;
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM students; DELETE FROM majors; DELETE FROM class_enrollment", conn);
      cmd.ExecuteNonQuery();
    }

    public void AddCourse(int courseID)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO class_enrollment (student_id, course_id, isComplete) VALUES (@StudentID, @CourseID, 0);", conn);

      SqlParameter idParameter = new SqlParameter();
      idParameter.ParameterName = "@StudentID";
      idParameter.Value = this.GetId();

      SqlParameter courseIDParameter = new SqlParameter();
      courseIDParameter.ParameterName = "@CourseID";
      courseIDParameter.Value = courseID;

      cmd.Parameters.Add(idParameter);
      cmd.Parameters.Add(courseIDParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public void DropCourse(int courseID)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM class_enrollment WHERE class_enrollment.student_id = @StudentId AND class_enrollment.course_id = @CourseId;", conn);

      SqlParameter idParameter = new SqlParameter();
      idParameter.ParameterName = "@StudentId";
      idParameter.Value = this.GetId();

      SqlParameter courseIDParameter = new SqlParameter();
      courseIDParameter.ParameterName = "@CourseId";
      courseIDParameter.Value = courseID;

      cmd.Parameters.Add(idParameter);
      cmd.Parameters.Add(courseIDParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public List<Course> GetCourses()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT courses.* FROM students JOIN class_enrollment ON (students.id = class_enrollment.student_id)	JOIN courses ON (class_enrollment.course_id = courses.id)	WHERE students.id = @StudentId;", conn);

      SqlParameter idParameter = new SqlParameter();
      idParameter.ParameterName = "@StudentId";
      idParameter.Value = _id;


      cmd.Parameters.Add(idParameter);
      rdr = cmd.ExecuteReader();

      List<Course> foundCourses = new List<Course>{};

      string foundCourseName = null;
      int foundCourseDeparmentId = 0;
      int foundCourseId = 0;

      while(rdr.Read())
      {
        foundCourseName = rdr.GetString(0);
        foundCourseDeparmentId = rdr.GetInt32(1);
        foundCourseId = rdr.GetInt32(2);
        Course foundCourse = new Course(foundCourseName, foundCourseDeparmentId, foundCourseId);
        foundCourses.Add(foundCourse);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundCourses;
    }

    public void AddMajor(int departmentID)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO majors (student_id, department_id) VALUES (@StudentID, @DepartmentID);", conn);

      SqlParameter idParameter = new SqlParameter();
      idParameter.ParameterName = "@StudentID";
      idParameter.Value = this.GetId();

      SqlParameter departmentIDParameter = new SqlParameter();
      departmentIDParameter.ParameterName = "@DepartmentID";
      departmentIDParameter.Value = departmentID;

      cmd.Parameters.Add(idParameter);
      cmd.Parameters.Add(departmentIDParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public List<Department> GetMajors()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT departments.* FROM students JOIN majors ON (students.id = majors.student_id)	JOIN departments ON (majors.department_id = departments.id)	WHERE students.id = @StudentId;", conn);

      SqlParameter idParameter = new SqlParameter();
      idParameter.ParameterName = "@StudentId";
      idParameter.Value = _id;


      cmd.Parameters.Add(idParameter);
      rdr = cmd.ExecuteReader();

      List<Department> foundMajors = new List<Department>{};

      string foundMajorName = null;
      int foundDepartmentId = 0;

      while(rdr.Read())
      {
        foundMajorName = rdr.GetString(0);
        foundDepartmentId = rdr.GetInt32(1);
        Department foundMajor = new Department(foundMajorName, foundDepartmentId);
        foundMajors.Add(foundMajor);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundMajors;
    }

    public List<Course> GetAllCourses()
    {
      return Course.GetAll();
    }
    public List<Department> GetAllDepartments()
    {
      return Department.GetAll();
    }

    public void DropMajor(int departmentID)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM majors WHERE majors.student_id = @StudentId AND majors.department_id = @DepartmentId;", conn);

      SqlParameter idParameter = new SqlParameter();
      idParameter.ParameterName = "@StudentId";
      idParameter.Value = this.GetId();

      SqlParameter departmentIDParameter = new SqlParameter();
      departmentIDParameter.ParameterName = "@DepartmentId";
      departmentIDParameter.Value = departmentID;

      cmd.Parameters.Add(idParameter);
      cmd.Parameters.Add(departmentIDParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

  }
}
