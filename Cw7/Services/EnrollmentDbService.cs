using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Cw7.Dto;

namespace Cw7.Services
{
    public class EnrollmentDbService : IEnrollmentDbService
    {
        private readonly IConfig config;
        private readonly IStudentDbService studentDbService;

        private const string ExistsByStudyAndSemesterQuery = @"SELECT TOP 1 1
                                                        FROM 
	                                                        [Studies] AS S JOIN
	                                                        [Enrollment] AS E ON E.IdStudy = S.IdStudy
                                                        WHERE
	                                                        S.[Name] = @Studies AND E.Semester = @Semester";

        private const string GetByStudyAndSemesterQuery = @"SELECT *
                                                        FROM 
	                                                        [Studies] AS S JOIN
	                                                        [Enrollment] AS E ON E.IdStudy = S.IdStudy
                                                        WHERE
	                                                        S.[Name] = @Studies AND E.Semester = @Semester";

        private const string GetActualEnrollment = @"SELECT TOP 1 
	                                    E.IdEnrollment
                                    FROM
	                                    [Studies] AS S JOIN
	                                    [Enrollment] AS E ON E.IdStudy = S.IdStudy
                                    WHERE E.Semester = 1 AND S.[Name] = @StudyName
                                    ORDER BY E.StartDate DESC";

        private const string InsertEnrollment = @"
                                    INSERT INTO [Enrollment]
                                               (
                                               [IdEnrollment]
                                               ,[Semester]
                                               ,[IdStudy]
                                               ,[StartDate])
                                         VALUES
                                               (
                                                @IdEnrollment
                                                ,1
                                               ,@IdStudy
                                               ,GETDATE());";

        private const string GetEnrollmentLastId = @"SELECT TOP 1 E.IdEnrollment
                            FROM [Enrollment] AS E
                            ORDER BY E.IdEnrollment DESC";

        private const string GetByIdQuery = @"SELECT E.*
                                    FROM [Enrollment] AS E
                                    WHERE E.IdEnrollment = @IdEnrollment";

        public EnrollmentDbService(IConfig config, IStudentDbService studentDbService)
        {
            this.config = config;
            this.studentDbService = studentDbService;
        }

        public async Task<Enrollment> EnrollStudent(EnrollStudent model, Study study)
        {
            await using var sqlConnection = new SqlConnection(config.ConnectionString);
            await sqlConnection.OpenAsync();
            var transaction = (SqlTransaction) await sqlConnection.BeginTransactionAsync();
            
            try
            {
                var actualEnrollmentId = await ActualEnrollmentGetByName(transaction, model.Studies) 
                                         ?? await ActualEnrollmentCreate(transaction,study);

                await studentDbService.Create(model, transaction, actualEnrollmentId);;

                await transaction.CommitAsync();
                return await GetById(actualEnrollmentId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task Promotions(Promotions promotions)
        {
            await using var sqlConnection = new SqlConnection(config.ConnectionString);

            await using var command = new SqlCommand("Enrollment_Promotions", sqlConnection) { CommandType = CommandType.StoredProcedure };
            command.Parameters.AddWithValue("Studies", promotions.Studies);
            command.Parameters.AddWithValue("Semester", promotions.Semester);

            await sqlConnection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }

        public async Task<Enrollment> GetBy(string studies,int semester)
        {
            await using var sqlConnection = new SqlConnection(config.ConnectionString);

            await using var command = new SqlCommand(GetByStudyAndSemesterQuery, sqlConnection) { CommandType = CommandType.Text };
            command.Parameters.AddWithValue("Studies", studies);
            command.Parameters.AddWithValue("Semester", semester);

            await sqlConnection.OpenAsync();

            await using var sqlDataReader = await command.ExecuteReaderAsync();
            while (await sqlDataReader.ReadAsync())
            {
                var enrollment = new Enrollment
                {
                    StartDate = DateTime.Parse(sqlDataReader[nameof(Enrollment.StartDate)]?.ToString()),
                    Semester = sqlDataReader[nameof(Enrollment.Semester)].ToString(),
                    IdStudy = int.Parse(sqlDataReader[nameof(Enrollment.IdStudy)].ToString()),
                    IdEnrollment = int.Parse(sqlDataReader[nameof(Enrollment.IdEnrollment)].ToString())
                };
                return enrollment;
            }

            return null;
        }

        private async Task<Enrollment> GetById(int id)
        {
            await using var sqlConnection = new SqlConnection(config.ConnectionString);
            await using var command = new SqlCommand(GetByIdQuery, sqlConnection) { CommandType = CommandType.Text };
            command.Parameters.AddWithValue("@IdEnrollment", id);
            await sqlConnection.OpenAsync();

            await using var sqlDataReader = await command.ExecuteReaderAsync();
            while (await sqlDataReader.ReadAsync())
            {
                var enrollment = new Enrollment
                {
                    StartDate = DateTime.Parse(sqlDataReader[nameof(Enrollment.StartDate)]?.ToString()),
                    Semester = sqlDataReader[nameof(Enrollment.Semester)].ToString(),
                    IdStudy = int.Parse(sqlDataReader[nameof(Enrollment.IdStudy)].ToString()),
                    IdEnrollment = int.Parse(sqlDataReader[nameof(Enrollment.IdEnrollment)].ToString())
                };
                return enrollment;
            }

            return null;
        }

        private async Task<int> ActualEnrollmentCreate(SqlTransaction sqlTransaction, Study study)
        {
            var lastId = await GetLastId(sqlTransaction);
            await using var command = new SqlCommand(InsertEnrollment, sqlTransaction.Connection)
            {
                CommandType = CommandType.Text,
                Transaction = sqlTransaction
            };

            command.Parameters.AddWithValue("@IdEnrollment", lastId + 1);
            command.Parameters.AddWithValue("@IdStudy", study.IdStudy);
            await command.ExecuteNonQueryAsync();
            return await GetLastId(sqlTransaction);
        }

        private async Task<int?> ActualEnrollmentGetByName(SqlTransaction sqlTransaction, string studyName)
        {
            await using var command = new SqlCommand(GetActualEnrollment, sqlTransaction.Connection)
            {
                CommandType = CommandType.Text,
                Transaction = sqlTransaction
            };
            command.Parameters.AddWithValue("StudyName", studyName);

            await using var sqlDataReader = await command.ExecuteReaderAsync();
            while (await sqlDataReader.ReadAsync())
            {
                return int.Parse(sqlDataReader["IdEnrollment"].ToString());
            }

            return null;
        }

        private async Task<int> GetLastId(SqlTransaction sqlTransaction)
        {
            await using var command = new SqlCommand(GetEnrollmentLastId, sqlTransaction.Connection)
            {
                CommandType = CommandType.Text,
                Transaction = sqlTransaction
            };

            await using var sqlDataReader = await command.ExecuteReaderAsync();
            while (await sqlDataReader.ReadAsync())
            {
                return int.Parse(sqlDataReader["IdEnrollment"].ToString());
            }

            return 0;
        }

        public async Task<bool> Exists(string studies,int semester)
        {
            await using var sqlConnection = new SqlConnection(config.ConnectionString);

            await using var command = new SqlCommand(ExistsByStudyAndSemesterQuery, sqlConnection) { CommandType = CommandType.Text };
            command.Parameters.AddWithValue("Studies", studies);
            command.Parameters.AddWithValue("Semester", semester);

            await sqlConnection.OpenAsync();

            await using var sqlDataReader = await command.ExecuteReaderAsync();
            return await sqlDataReader.ReadAsync();
        }
    }
}
