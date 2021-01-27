using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tutorial7.Models;

namespace tutorial7.Services
{
    public interface IStudentsDbService
    {
        Student EnrollStudent(Student student);
        Enrollment PromoteStudent(Enrollment enrollment);
        bool validationCredential(string login, string password);

        void assignRefreshToken(string login , Guid rtoken);

        bool checkrefreshToken(string token);

        void updateRefreshToken(string oldtoken,Guid newtoken);

      

    }
}
