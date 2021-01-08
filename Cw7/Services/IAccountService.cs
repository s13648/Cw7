using System.Threading.Tasks;
using Cw7.Dto;

namespace Cw7.Services
{
    public interface IAccountService
    {
        public Task<AccessToken> GenerateAccessToken(Student studentByIndex);
    }
}
