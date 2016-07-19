using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Registrar.Objects
{
  public class Department
  {
    private int _id;
    private string _name;

    public Department(string name, int Id = 0)
    {
      _id = Id;
      _name = name;
    }

    public override bool Equals(System.Object otherDepartment)
    {
      if (!(otherDepartment is Department))
      {
        return false;
      }
      else
      {
        Department newDepartment = (Department) otherDepartment;
        bool idEquality = (this.GetId() == newDepartment.GetId());
        bool nameEquality = (this.GetName() == newDepartment.GetName());
        return (idEquality && nameEquality);
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

    public static List<Department> GetAll()
    {
      List<Department> allDepartments = new List<Department> {};
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM departments;", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        string courseName = rdr.GetString(0);
        int courseId = rdr.GetInt32(1);
        Department newDepartment = new Department(courseName, courseId);
        allDepartments.Add(newDepartment);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allDepartments;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO departments (name) OUTPUT INSERTED.id VALUES (@DepartmentName);", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@DepartmentName";
      nameParameter.Value = this.GetName();

      cmd.Parameters.Add(nameParameter);

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

    public static Department Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM departments WHERE id = @DepartmentId;", conn);
      SqlParameter courseIdParameter = new SqlParameter();
      courseIdParameter.ParameterName = "@DepartmentId";
      courseIdParameter.Value = id.ToString();
      cmd.Parameters.Add(courseIdParameter);
      rdr = cmd.ExecuteReader();


      int foundDepartmentId = 0;
      string foundDepartmentName = null;

      while(rdr.Read())
      {
        foundDepartmentName = rdr.GetString(0);
        foundDepartmentId = rdr.GetInt32(1);
      }
      Department foundDepartment = new Department(foundDepartmentName, foundDepartmentId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundDepartment;
    }

    public static void DeleteAll()
     {
       SqlConnection conn = DB.Connection();
       conn.Open();
       SqlCommand cmd = new SqlCommand("DELETE FROM departments; DELETE FROM majors; DELETE FROM courses; DELETE FROM class_enrollment;", conn);
       cmd.ExecuteNonQuery();
     }

  }
}
