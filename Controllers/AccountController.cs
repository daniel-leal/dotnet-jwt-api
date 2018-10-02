using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebApiJwt.Controllers
{
    public class Validate
    {
        public int Key { get; set; }

        public List<string> Message { get; set; }
    }

    public class Auth
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }


    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly SignInManager<WebAppUser> _signInManager;
        private readonly UserManager<WebAppUser> _userManager;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<WebAppUser> userManager,
            SignInManager<WebAppUser> signInManager,
            IConfiguration configuration
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<object> Login([FromBody] LoginDto model)
        {
            var v = new Validate() { Key = 0, Message = new List<string>() };

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            if (result.Succeeded)
            {
                var appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
                var token = GenerateJwtToken(model.Email, appUser);

                Auth a = new Auth()
                {
                    UserId = appUser.Id,
                    Token = token.ToString(),
                    Email = appUser.Email
                };

                return a;
            }
            else
            {
                v.Key = 0;
                v.Message.Add("Invalid Email or Password");
                return Json(v);
            }
        }

        [HttpPost]
        public async Task<object> Register([FromBody] RegisterDto model)
        {
            var v = new Validate() { Key = 0, Message = new List<string>() };

            if (ModelState.IsValid)
            {
                var user = new WebAppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    AvatarUrl = model.AvatarUrl,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return GenerateJwtToken(model.Email, user);
                }
                else
                {
                    v.Key = 999;
                    v.Message = result.Errors.Select(x => x.Description).ToList();
                    return Json(v);
                }
            }
            else
            {
                foreach (var x in ModelState)
                {
                    foreach (var erro in x.Value.Errors)
                    {
                        v.Key = 999;
                        v.Message.Add(erro.ErrorMessage);
                    }
                }
            }


            return Json(v);
        }

        private object GenerateJwtToken(string email, WebAppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public class LoginDto
        {
            [Required]
            public string Email { get; set; }

            [Required]
            public string Password { get; set; }

        }

        public class RegisterDto
        {
            [Required]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [StringLength(100, ErrorMessage = "PASSWORD_MIN_LENGTH", MinimumLength = 6)]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password")]
            public string PasswordConfirmation { get; set; }

            // WebApp Properties
            [Required]
            public string Name { get; set; }

            public string AvatarUrl { get; set; }
        }
    }
}