using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tutorial6.Models;

namespace tutorial6.Services
{
    public interface IStudentsDbService
    {
        Student EnrollStudent(Student student);
        Enrollment PromoteStudent(Enrollment enrollment);

        Student GetStudentbyIndex(string index);

        void SaveLogData(string data);
    }
}
