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
      Course.DeleteAll();
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
      Student testStudent = new Student("Chad", enrollmentDate);

      //Act
      testStudent.Save();
      List<Student> result = Student.GetAll();
      List<Student> testList = new List<Student>{testStudent};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_Save_AssignsIdToObject()
    {
      //Arrange
      Student testStudent = new Student("Chad", enrollmentDate);

      //Act
      testStudent.Save();
      Student savedStudent = Student.GetAll()[0];

      int result = savedStudent.GetId();
      int testId = testStudent.GetId();

      //Assert
      Assert.Equal(testId, result);
    }

    [Fact]
    public void Test_Find_FindsStudentInDatabase()
    {
      //Arrange
      Student testStudent = new Student("Chad", enrollmentDate);
      testStudent.Save();

      //Act
      Student foundStudent = Student.Find(testStudent.GetId());

      //Assert
      Assert.Equal(testStudent, foundStudent);
    }

    [Fact]
    public void Test_AddCourse_DisplaysAddedCourses()
    {
      Student testStudent2 = new Student("Bob", enrollmentDate);
      testStudent2.Save();
      Course testCourse = new Course("CS101", 1);
      testCourse.Save();

      testStudent2.AddCourse(testCourse.GetId());
      List<Course> resultList = testStudent2.GetCourses();
      List<Course> expectedList= new List<Course>{testCourse};

      Assert.Equal(expectedList, resultList);
    }

    [Fact]
    public void Test_DropCourse_DropsSelectedCourse()
    {
      Student testStudent = new Student("Bob", enrollmentDate);
      testStudent.Save();
      Course testCourse1 = new Course("CS101", 1);
      testCourse1.Save();
      testStudent.AddCourse(testCourse1.GetId());
      Course testCourse2 = new Course("PHIL101", 2);
      testCourse2.Save();
      testStudent.AddCourse(testCourse2.GetId());

      testStudent.DropCourse(testCourse1.GetId());

      List<Course> resultList = testStudent.GetCourses();
      List<Course> expectedList= new List<Course>{testCourse2};

      Assert.Equal(expectedList, resultList);
    }
    [Fact]
    public void Test_AddMajor_DisplaysAddedMajors()
    {
      Student testStudent = new Student("Bob", enrollmentDate);
      testStudent.Save();
      Department testDepartment = new Department("History");
      testDepartment.Save();

      testStudent.AddMajor(testDepartment.GetId());
      List<Department> resultList = testStudent.GetMajors();
      List<Department> expectedList= new List<Department>{testDepartment};

      Assert.Equal(expectedList, resultList);
    }
    [Fact]
    public void Test_DropMajor_DropsSelectedMajor()
    {
      Student testStudent = new Student("Bob", enrollmentDate);
      testStudent.Save();
      Department testDepartment1 = new Department("Chinese");
      testDepartment1.Save();
      testStudent.AddMajor(testDepartment1.GetId());
      Department testDepartment2 = new Department("Spanish");
      testDepartment2.Save();
      testStudent.AddMajor(testDepartment2.GetId());

      testStudent.DropMajor(testDepartment1.GetId());

      List<Department> resultList = testStudent.GetMajors();
      List<Department> expectedList= new List<Department>{testDepartment2};

      Assert.Equal(expectedList, resultList);
    }

  }
}
