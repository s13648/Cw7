using System.Threading.Tasks;
using Cw7.Dto;

namespace Cw7.Services
{
    public interface IStudyDbService
    {
        Task<Study> GetByName(string modelStudies);
    }
}
