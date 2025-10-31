using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TMS_DAL.Common;
using TMS_DAL.Entities;
using TMS_DAL.Interfaces.Auth;
using TMS_DAL.Models.Auth;
using TMS_DAL.Models.Common;
using TMS_DAL.Models.Task;

namespace TMS_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILoginRepository _loginRepository;

        public AuthController(ILoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = new ExecutionResponseModel();

            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                result = new ExecutionResponseModel()
                {
                    ExecutionResultId = (int)ExecutionResultEnum.ValidationError,
                    ResponseText = "Please provide the all the required fields."
                };
            }
            else
            {
                var loginData = await _loginRepository.GetLoginData(model.Username);

                if(loginData == null)
                {
                    result = new ExecutionResponseModel()
                    {
                        ExecutionResultId = (int)ExecutionResultEnum.InvalidCredentials,
                        ResponseText = "Username or password is invalid."
                    };
                }
                else
                {

                    if(Functions.VerifyPassword(model.Password, loginData.PasswordHash))
                    {
                        var tokenHandler = new JwtSecurityTokenHandler();

                        var key = Encoding.ASCII.GetBytes("8a0e8c52-821e-4c77-90ff-59b1b3fcd71a");

                        var expires = model.RememberMe == true ? DateTime.UtcNow.AddDays(7)
                            : DateTime.UtcNow.AddHours(2);

                        var claims = new[]
                        {
                             new Claim("UserId", loginData.UserId),
                        };

                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Expires = expires,
                            SigningCredentials = new SigningCredentials(
                                new SymmetricSecurityKey(key),
                                SecurityAlgorithms.HmacSha256Signature),
                            Subject = new ClaimsIdentity(claims),
                        };

                        var token = tokenHandler.CreateToken(tokenDescriptor);
                        var jwtToken = tokenHandler.WriteToken(token);

                        result = new ExecutionResponseModel()
                        {
                            ExecutionResultId = (int)ExecutionResultEnum.Success,
                            Data = new
                            {
                                Token = jwtToken,
                                FirstName = loginData.FirstName,
                                LastName = loginData.LastName
                            }
                        };
                    }
                    else
                    {
                        result = new ExecutionResponseModel()
                        {
                            ExecutionResultId = (int)ExecutionResultEnum.InvalidCredentials,
                            ResponseText = "Username or password is invalid."
                        };
                    }
                }
            }

            return Ok(result);
        }
    }
}
