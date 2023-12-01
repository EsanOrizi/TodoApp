using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public record AuthenticationData(string? UserName, string? Password);
        public record UserData(int Id, string FirstName, string LastName, string UserName);

        [HttpPost("token")]
        [AllowAnonymous]
        public ActionResult<string> Authenticate([FromBody] AuthenticationData data)
        {
            var user = ValidateCredentials(data);

            if (user == null)
            {
                return Unauthorized();
            }

            string token = GenerateToken(user);
            return Ok(token);
        }

        private string GenerateToken(UserData user)
        {
            var secretKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(configuration.GetValue<string>("Authentication:SecretKey")));

            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new();
            claims.Add(new(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
            claims.Add(new(JwtRegisteredClaimNames.UniqueName, user.UserName));
            claims.Add(new(JwtRegisteredClaimNames.GivenName, user.FirstName));
            claims.Add(new(JwtRegisteredClaimNames.FamilyName, user.LastName));

            var token = new JwtSecurityToken(
                configuration.GetValue<string>("Authentication:Issuer"),
                configuration.GetValue<string>("Authentication:Audience"),
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(1),
                signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);




        }

        private UserData? ValidateCredentials(AuthenticationData data)
        {
            // This is not production code, replace this with a call to your auth system
            if(CompareValues(data.UserName, "esan") &&
               CompareValues(data.Password, "Test123"))
            {
                return new UserData(1, "Esan", "Orizi", data.UserName!);
            }
            if (CompareValues(data.UserName, "massa") &&
               CompareValues(data.Password, "Test123"))
            {
                return new UserData(2, "Massa", "Orizi", data.UserName!);
            }

            return null;
        }

        private bool CompareValues(string? actual, string expected)
        {
            if (actual is not null)
            {
                if (actual.Equals(expected))
                {
                    return true;
                }
            }
            return false;
        }

    }

}
