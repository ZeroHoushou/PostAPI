using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SocialMedia.Core.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia.Api.Controllers
{
    [Route ("api/[Controller]")]
    [ApiController]
    public class TokenController:ControllerBase
    {
        private readonly IConfiguration _configuration;
        public TokenController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost]
        public IActionResult Authentication( UserLogin login ) 
        { 
            if(IsValidUser())
            {
                var token = GenerarToken();
                return Ok(new { token  });
            }

            return NotFound();
        }

        private bool IsValidUser()
        {
            return true;
        }

        private string GenerarToken()
        {
            var _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]));
            var signinCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(signinCredentials);

            var claims = new[]
            {
                 new Claim(ClaimTypes.Name,"Zero Houshou"),
                 new Claim (ClaimTypes.Email,"zero@gmail.com"),
                 new Claim (ClaimTypes.Role, "Administrator")
            };

            //Payload
            var payload = new JwtPayload
            (
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claims,
                DateTime.Now,
                DateTime.UtcNow.AddMinutes(2)
            );

            var token = new JwtSecurityToken(header,payload);
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
