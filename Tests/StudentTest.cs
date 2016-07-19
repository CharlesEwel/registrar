using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;
using Registrar.Objects;

namespace Registrar.Tests
{
  public class StudentTest : IDisposable
  {
    private DateTime? enrollmentDate = new DateTime(2016, 7, 12);
    public StudentTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=registrar_tutorial_test;Integrated Security=SSPI;";
    }

    public void Dispose()
    {
      Student.DeleteAll();
    }

    [Fact]
    public void Test_DatabaseEmptyAtFirst()
    {
      //Arrange, Act
      int result = Student.GetAll().Count;
      //Assert
      Assert.Equal(0,result);
    }

    [Fact]
    public void Test_Equal_ReturnsTrueIfNamesAndEnrollmentDatesAreTheSame()
    {
      //Arrange, Act
      Student firstStudent = new Student("Chad", enrollmentDate);
      Student secondStudent = new Student("Chad", enrollmentDate);

      //Assert
      Assert.Equal(firstStudent, secondStudent);
    }

    [Fact]
    public void Test_Save_SavesToDatabase()
    {
      //Arrange
      DateTime? taskDate = new DateTime(2016, 7, 12);
      Student testStudent = new Student("Chad", taskDate);

      //Act
      testStudent.Save();
      List<Student> result = Student.GetAll();
      List<Student> testList = new List<Student>{testStudent};

      //Assert
      Assert.Equal(testList, result);
    }
    
    // [Fact]
    // public void Test_Save_AssignsIdToObject()
    // {
    //   //Arrange
    //   DateTime? taskDate = new DateTime(2016, 7, 12);
    //   Student testStudent = new Student("Chad", taskDate);
    //
    //   //Act
    //   testStudent.Save();
    //   Student savedStudent = Student.GetAll()[0];
    //
    //   int result = savedStudent.GetId();
    //   int testId = testStudent.GetId();
    //
    //   //Assert
    //   Assert.Equal(testId, result);
    // }
  }
}
