using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Cw7.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Cw7.Services
{
    public class AccountService : IAccountService
    {
        private readonly IConfiguration configuration;
        private readonly IStudentDbService studentDbService;

        public AccountService(IConfiguration configuration, IStudentDbService studentDbService)
        {
            this.configuration = configuration;
            this.studentDbService = studentDbService;
        }

        public async Task<AccessToken> GenerateAccessToken(Student studentByIndex)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, studentByIndex.IndexNumber),
                new Claim(ClaimTypes.Name, $"{studentByIndex.FirstName} {studentByIndex.LastName}"),
                new Claim(ClaimTypes.Role, "student"),
                new Claim(ClaimTypes.Role, "employee")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );


            var accessToken = new AccessToken
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = Guid.NewGuid()
            };

            await studentDbService.SetRefreshToken(studentByIndex.IndexNumber, accessToken.RefreshToken);

            return accessToken;
        }
    }
}
