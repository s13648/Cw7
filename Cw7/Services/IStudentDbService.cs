using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Cw7.Dto;

namespace Cw7.Services
{
    public interface IStudentDbService
    {
        public Task<IEnumerable<Student>> GetStudents();

        public Task<bool> Exists(string indexNumber);

        public Task Create(EnrollStudent model, SqlTransaction sqlTransaction, int idEnrollment);

        public Task<Student> GetByIndex(string index);
        
        public Task SetRefreshToken(string studentIndex, Guid refreshToken);
        
        Task<Student> GetByRefreshToken(Guid refreshToken);
    }
}
