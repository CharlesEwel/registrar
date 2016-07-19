using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;
using Registrar.Objects;

namespace Registrar.Tests
{
  public class DepartmentTest : IDisposable
  {
    public DepartmentTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=registrar_tutorial_test;Integrated Security=SSPI;";
    }

    public void Dispose()
    {
      Department.DeleteAll();
      Course.DeleteAll();
      Student.DeleteAll();
    }

    [Fact]
    public void Test_DatabaseEmptyAtFirst()
    {
      //Arrange, Act
      int result = Department.GetAll().Count;
      //Assert
      Assert.Equal(0,result);
    }

    [Fact]
    public void Test_Equal_ReturnsTrueIfNamesAndEnrollmentDatesAreTheSame()
    {
      //Arrange, Act
      Department firstDepartment = new Department("History");
      Department secondDepartment = new Department("History");

      //Assert
      Assert.Equal(firstDepartment, secondDepartment);
    }

    [Fact]
    public void Test_Save_SavesToDatabase()
    {
      //Arrange
      Department testDepartment = new Department("History");

      //Act
      testDepartment.Save();
      List<Department> result = Department.GetAll();
      List<Department> testList = new List<Department>{testDepartment};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_Save_AssignsIdToObject()
    {
      //Arrange
      Department testDepartment = new Department("History");

      //Act
      testDepartment.Save();
      Department savedDepartment = Department.GetAll()[0];

      int result = savedDepartment.GetId();
      int testId = testDepartment.GetId();

      //Assert
      Assert.Equal(testId, result);
    }

    [Fact]
    public void Test_Find_FindsDepartmentInDatabase()
    {
      //Arrange
      Department testDepartment = new Department("History");
      testDepartment.Save();

      //Act
      Department foundDepartment = Department.Find(testDepartment.GetId());

      //Assert
      Assert.Equal(testDepartment, foundDepartment);
    }
  }
}
