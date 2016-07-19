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
        this._id = rdr.GetInt32(2);
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

    public static void DeleteAll()
     {
       SqlConnection conn = DB.Connection();
       conn.Open();
       SqlCommand cmd = new SqlCommand("DELETE FROM students; DELETE FROM majors; DELETE FROM class_enrollment", conn);
       cmd.ExecuteNonQuery();
     }

  }
}
