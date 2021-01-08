using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Cw7.Dto;

namespace Cw7.Services
{
    public class StudentDbService : IStudentDbService
    {
        private const string GetStudentsSql = @"SELECT 
	                                        S.FirstName,
	                                        S.LastName,
	                                        S.BirthDate,
	                                        ST.Name AS StudyName,
	                                        E.Semester
                                        FROM 
	                                        [Student] AS S JOIN 
	                                        [Enrollment] AS E ON S.IdEnrollment = E.IdEnrollment JOIN
	                                        [Studies] AS ST ON E.IdStudy = ST.IdStudy
                                        ";

        private const string ExistsQuery = @"SELECT TOP 1 1
                                                FROM 
	                                                [Student] AS ST 
                                                WHERE
	                                                ST.[IndexNumber] = @IndexNumber";


        private const string GetByIndexQuery = @"SELECT 
	                                        S.FirstName,
	                                        S.LastName,
	                                        S.BirthDate,
                                            S.Password,
                                            S.IndexNumber,
	                                        ST.Name AS StudyName,
	                                        E.Semester
                                        FROM 
	                                        [Student] AS S JOIN 
	                                        [Enrollment] AS E ON S.IdEnrollment = E.IdEnrollment JOIN
	                                        [Studies] AS ST ON E.IdStudy = ST.IdStudy
                                        WHERE
                                            S.[IndexNumber] = @IndexNumber";

        private const string GetByRefreshTokenQuery = @"SELECT 
	                                        S.FirstName,
	                                        S.LastName,
	                                        S.BirthDate,
                                            S.Password,
                                            S.IndexNumber,
	                                        ST.Name AS StudyName,
	                                        E.Semester
                                        FROM 
	                                        [Student] AS S JOIN 
	                                        [Enrollment] AS E ON S.IdEnrollment = E.IdEnrollment JOIN
	                                        [Studies] AS ST ON E.IdStudy = ST.IdStudy
                                        WHERE
                                            S.[RefreshToken] = @RefreshToken";

        private const string InsertStudentQuery = @"INSERT INTO [dbo].[Student]
                           ([IndexNumber]
                           ,[FirstName]
                           ,[LastName]
                           ,[Password]
                           ,[BirthDate]
                           ,[IdEnrollment])
                     VALUES
                           (@IndexNumber
                           ,@FirstName
                           ,@LastName
                           ,@Password
                           ,@BirthDate
                           ,@IdEnrollment)";

        private const string SetRefreshTokenQuery = @"UPDATE Student
                                                    SET RefreshToken = @RefreshToken
                                                    WHERE IndexNumber = @IndexNumber";

        private readonly IConfig config;

        public StudentDbService(IConfig config)
        {
            this.config = config;
        }

        public async Task<IEnumerable<Student>> GetStudents()
        {
            await using var sqlConnection = new SqlConnection(config.ConnectionString);
            await using var command = new SqlCommand(GetStudentsSql,sqlConnection) {CommandType = CommandType.Text};
            await sqlConnection.OpenAsync();

            await using var sqlDataReader = await command.ExecuteReaderAsync();
            var students = new List<Student>();
            while (await sqlDataReader.ReadAsync())
            {
                var student = new Student
                {
                    BirthDate = DateTime.Parse(sqlDataReader[nameof(Student.BirthDate)]?.ToString()),
                    FirstName = sqlDataReader[nameof(Student.FirstName)].ToString(),
                    LastName = sqlDataReader[nameof(Student.LastName)].ToString(),
                    Semester = int.Parse(sqlDataReader[nameof(Student.Semester)].ToString()),
                    StudyName = sqlDataReader[nameof(Student.StudyName)].ToString()
                };
                students.Add(student);
            }

            return students;
        }

        public async Task<bool> Exists(string indexNumber)
        {
            await using var sqlConnection = new SqlConnection(config.ConnectionString);

            await using var command = new SqlCommand(ExistsQuery, sqlConnection) { CommandType = CommandType.Text };
            command.Parameters.AddWithValue("IndexNumber", indexNumber);

            await sqlConnection.OpenAsync();

            await using var sqlDataReader = await command.ExecuteReaderAsync();
            return await sqlDataReader.ReadAsync();
        }

        public async Task Create(EnrollStudent model, SqlTransaction sqlTransaction, int idEnrollment)
        {
            await using var command = new SqlCommand(InsertStudentQuery, sqlTransaction.Connection)
            {
                CommandType = CommandType.Text,
                Transaction = sqlTransaction
            };

            command.Parameters.AddWithValue("@IndexNumber", model.IndexNumber);
            command.Parameters.AddWithValue("@FirstName", model.FirstName);
            command.Parameters.AddWithValue("@Password", model.Password);
            command.Parameters.AddWithValue("@LastName", model.LastName);
            command.Parameters.AddWithValue("@BirthDate", model.BirthDate);
            command.Parameters.AddWithValue("@IdEnrollment", idEnrollment);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<Student> GetByIndex(string index)
        {
            await using var sqlConnection = new SqlConnection(config.ConnectionString);
            await using var command = new SqlCommand(GetByIndexQuery, sqlConnection) { CommandType = CommandType.Text };
            command.Parameters.AddWithValue("IndexNumber", index);

            await sqlConnection.OpenAsync();

            await using var sqlDataReader = await command.ExecuteReaderAsync();
            while (await sqlDataReader.ReadAsync())
            {
                return new Student
                {
                    BirthDate = DateTime.Parse(sqlDataReader[nameof(Student.BirthDate)]?.ToString()),
                    FirstName = sqlDataReader[nameof(Student.FirstName)].ToString(),
                    LastName = sqlDataReader[nameof(Student.LastName)].ToString(),
                    IndexNumber = sqlDataReader[nameof(Student.IndexNumber)].ToString(),
                    Semester = int.Parse(sqlDataReader[nameof(Student.Semester)].ToString()),
                    Password = sqlDataReader[nameof(Student.Password)].ToString(),
                    StudyName = sqlDataReader[nameof(Student.StudyName)].ToString()
                };
            }

            return null;
        }

        public async Task SetRefreshToken(string studentIndex, Guid refreshToken)
        {
            await using var sqlConnection = new SqlConnection(config.ConnectionString);
            await using var command = new SqlCommand(SetRefreshTokenQuery)
            {
                Connection = sqlConnection,
                CommandType = CommandType.Text,
            };

            command.Parameters.AddWithValue("@IndexNumber", studentIndex);
            command.Parameters.AddWithValue("@RefreshToken", refreshToken);
            
            await sqlConnection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<Student> GetByRefreshToken(Guid refreshToken)
        {
            await using var sqlConnection = new SqlConnection(config.ConnectionString);
            await using var command = new SqlCommand(GetByRefreshTokenQuery, sqlConnection) { CommandType = CommandType.Text };
            command.Parameters.AddWithValue("RefreshToken", refreshToken);

            await sqlConnection.OpenAsync();

            await using var sqlDataReader = await command.ExecuteReaderAsync();
            while (await sqlDataReader.ReadAsync())
            {
                return new Student
                {
                    BirthDate = DateTime.Parse(sqlDataReader[nameof(Student.BirthDate)]?.ToString()),
                    FirstName = sqlDataReader[nameof(Student.FirstName)].ToString(),
                    LastName = sqlDataReader[nameof(Student.LastName)].ToString(),
                    IndexNumber = sqlDataReader[nameof(Student.IndexNumber)].ToString(),
                    Semester = int.Parse(sqlDataReader[nameof(Student.Semester)].ToString()),
                    Password = sqlDataReader[nameof(Student.Password)].ToString(),
                    StudyName = sqlDataReader[nameof(Student.StudyName)].ToString()
                };
            }

            return null;
        }
    }
}
