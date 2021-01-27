using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using tutorial4.Models;

namespace tutorial4.Controllers
{

    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {

        private string ConnString = "Data Source=db-mssql;Initial Catalog=s19687;Integrated Security=True";

        [HttpGet]
        public IActionResult GetStudents()
        {


            var listStudents = new List<Student>();
            using (SqlConnection con = new SqlConnection(ConnString))  // connect to the SQL Server database (opening,closing,managing the database)
            using (SqlCommand com = new SqlCommand())  //represents sqlquery or other command to send the databse 
            {                                              //using block otomatik olarak disposesu calistiriyor
                com.Connection = con;
                com.CommandText = "select FirstName, LastName, BirthDate, s.Name 'studies',e.Semester from Student stu " +
                    "Join Enrollment e on stu.idEnrollment = e.idEnrollment " +
                    "Join Studies s on s.idStudy = e.idStudy; ";

                con.Open();  //opens connection to database it should open before sending command
                SqlDataReader dr = com.ExecuteReader();  //executing command 
                //sqldatareader -> fetch stream of data/rows
                while (dr.Read())  //fetch next record
                {
                    var student = new Student();

                    student.FirstName = dr["FirstName"].ToString();  //name of the column
                    student.LastName = dr["LastName"].ToString();
                    student.BirthDate = dr["BirthDate"].ToString();
                    student.studies = dr["studies"].ToString();
                    student.semester = dr.GetInt32(4);
                    //index of the column but it could be a problem because order of columns can change

                    listStudents.Add(student);
                }
                //con.Dispose();

                return Ok(listStudents);
            }



        }

        [HttpGet("{id}")]
        public IActionResult GetSemesterEntry(string id)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                // com.CommandText = "select e.StartDate from Student s Join Enrollment e on s.idEnrollment = e.idEnrollment where s.IndexNumber='"+id+"';" ;
                //this is unsafe we should protect the databases from sqlinjection attacks so
                com.CommandText = "select e.StartDate from Student s " +
                    "Join Enrollment e on s.idEnrollment = e.idEnrollment where s.IndexNumber=@index";

                com.Parameters.AddWithValue("index", id);
                string date;
                con.Open();
                var dr = com.ExecuteReader();
                if (dr.Read()) //if it exists
                {
                    date = dr["StartDate"].ToString();
                    return Ok(date);
                }
                return Ok();


            }
        }

        //adding new data to the database
        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {

                var index = $"s{new Random().Next(1, 20000)}";
                com.Connection = con;
                com.CommandText = "insert into Student values('" + index + "',@par1,@par2,@par3,2);";
                com.Parameters.AddWithValue("par1", student.FirstName);
                com.Parameters.AddWithValue("par2", student.LastName);
                com.Parameters.AddWithValue("par3", student.BirthDate);
                con.Open();
                int affected = com.ExecuteNonQuery();
                return Ok(affected);
            }
            return Ok();


        }


        [HttpGet("procedure")]
        public IActionResult getStudentwithProc()
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "TestProc";
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.Parameters.AddWithValue("FirstName", "Berkay");

                con.Open();
                var dr = com.ExecuteReader();
                if (dr.Read())
                {
                    var student = new Student();

                    student.FirstName = dr["FirstName"].ToString();
                    student.LastName = dr["LastName"].ToString();
                    student.BirthDate = dr["BirthDate"].ToString();
                    return Ok(student);
                }

            }
            return Ok();
        }
    }
}