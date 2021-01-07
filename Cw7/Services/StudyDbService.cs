using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Cw7.Dto;

namespace Cw7.Services
{
    public class StudyDbService : IStudyDbService
    {
        private readonly IConfig config;

        private const string GetByNameQuery = @"SELECT 
	                        S.IdStudy,
	                        S.[Name]
                        FROM [Studies] AS S
                        WHERE S.[Name] = @Name
                        ";
        public StudyDbService(IConfig config)
        {
            this.config = config;
        }

        public async Task<Study> GetByName(string modelStudies)
        {
            await using var sqlConnection = new SqlConnection(config.ConnectionString);
            await using var command = new SqlCommand(GetByNameQuery, sqlConnection) { CommandType = CommandType.Text };
            command.Parameters.AddWithValue("Name", modelStudies);
            await sqlConnection.OpenAsync();

            await using var sqlDataReader = await command.ExecuteReaderAsync();
            while (await sqlDataReader.ReadAsync())
            {
                return new Study
                {
                    IdStudy = int.Parse(sqlDataReader[nameof(Study.IdStudy)]?.ToString()),
                    Name = sqlDataReader[nameof(Study.Name)].ToString()
                };
            }

            return null;
        }
    }
}
