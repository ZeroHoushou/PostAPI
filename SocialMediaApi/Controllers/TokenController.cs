﻿using Microsoft.AspNetCore.Mvc;
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
using SocialMedia.Core.Interfaces;
using SocialMedia.Infrastructure.Services;
using SocialMedia.Infrastructure.Interfaces;

namespace SocialMedia.Api.Controllers
{
    [Route ("api/[Controller]")]
    [ApiController]
    public class TokenController:ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ISecurityService _securityService;
        private readonly IPasswordService _passwordService;
        public TokenController(IConfiguration configuration, ISecurityService securityService,IPasswordService passwordService)
        {
            _configuration = configuration;
            _securityService = securityService;
            _passwordService = passwordService;
        }
        [HttpPost]
        public async Task<IActionResult> Authentication( UserLogin login ) 
        {
            var validation = await IsValidUser(login);
            if(validation.Item1)
            {
                var token = GenerarToken(validation.Item2);
                return Ok(new { token  });
            }

            return NotFound();
        }

        private async Task<(bool,Security)> IsValidUser(UserLogin login)
        {
            var user = await _securityService.GetLoginByCredentials(login);
            var isValid = _passwordService.Check(user.Password, login.Password);
            return (isValid,user);
        }

        private string GenerarToken(Security security)
        {
            //Header
            var _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]));
            var signinCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(signinCredentials);

            var claims = new[]
            {
                 new Claim(ClaimTypes.Name,security.UserName),
                 new Claim("User",security.User),
                 new Claim (ClaimTypes.Role, security.Role.ToString())
            };

            //Payload
            var payload = new JwtPayload
            (
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claims,
                DateTime.Now,
                DateTime.UtcNow.AddMinutes(10)
            );

            var token = new JwtSecurityToken(header,payload);
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
