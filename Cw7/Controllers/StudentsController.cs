using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Cw7.Dto;
using Cw7.Helper;
using Cw7.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Cw7.Controllers
{
    [Authorize(Roles = "employee")]
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentDbService studentDbService;
        private readonly IAccountService accountService;

        public StudentsController(
            IStudentDbService studentDbService, 
            IAccountService accountService)
        {
            this.studentDbService = studentDbService;
            this.accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> GeStudents()
        {
            return Ok(await studentDbService.GetStudents());
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            return Ok(student);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var studentByIndex = await studentDbService.GetByIndex(request.Login);
            if (studentByIndex == null)
                return StatusCode(StatusCodes.Status403Forbidden);

            if (!PasswordHashHelper.Validate(studentByIndex.Password,studentByIndex.Salt,request.Password))
                return StatusCode(StatusCodes.Status403Forbidden);

            var token = await accountService.GenerateAccessToken(studentByIndex);
            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("refreshToken/{refreshToken}")]
        public async Task<IActionResult> RefreshToken(Guid refreshToken)
        {
            var studentByIndex = await studentDbService.GetByRefreshToken(refreshToken);
            if (studentByIndex == null)
                return StatusCode(StatusCodes.Status403Forbidden);

            
            var token = await accountService.GenerateAccessToken(studentByIndex);
            return Ok(token);
        }

        [HttpPut("{id}")]
        public IActionResult PutStudent(int id)
        {
            return Ok("Ąktualizacja dokończona");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            return Ok("Usuwanie ukończone");
        }
    }
}
