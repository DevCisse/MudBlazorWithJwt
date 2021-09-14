using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MudBlazorWithJwt.Server.Models;
using MudBlazorWithJwt.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazorWithJwt.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    //[ApiConventionType(typeof(DefaultApiConventions))]
    [Produces(MediaTypeNames.Application.Json)]
    //[Consumes(MediaTypeNames.)]
    public class AccountController : ControllerBase
    {
        private static UserModel LoggedOutUser = new UserModel { IsAuthenticated = false };

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, IConfiguration configuration,
                           SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
        }

        [HttpPost("Register")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(typeof(RegisterResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RegisterResult), StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<RegisterResult>> Post([FromBody] RegisterModel model)
        {
            var newUser = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, JobTitle = model.JobTitle };

            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description);

                return BadRequest(new RegisterResult { Successful = false, Errors = errors });

            }

            return Ok(new RegisterResult { Successful = true });
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<LoginResult>> Login([FromBody] LoginModel login)
        {
            var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, false, false);

            if (!result.Succeeded) return BadRequest(new LoginResult { Successful = false, Error = "Username and password are invalid." });

            var user = await _userManager.FindByEmailAsync(login.Email);

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, login.Email),
            new Claim("FirstName",user.FirstName?? ""),
            new Claim("LastName",user.LastName?? ""),
            new Claim("JobTitle",user.JobTitle ?? "")
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecurityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddDays(Convert.ToInt32(_configuration["JwtExpiryInDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtAudience"],
                claims,
                expires: expiry,
                signingCredentials: creds
            );

            return Ok(new LoginResult { Successful = true, Token = new JwtSecurityTokenHandler().WriteToken(token) });
        }



        [HttpPatch("patch")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> Patch([FromBody] UpdateModel update)
        {
            var user = await _userManager.FindByEmailAsync(update.Email);
            if (user is null)
            {
                return BadRequest("error");
            }

            user.JobTitle = update.JobTitle;
            user.FirstName = update.FirstName;
            user.LastName = update.LastName;


            await _userManager.UpdateAsync(user);
            return Ok();

        }



        [Authorize]
        [HttpGet("userdetails/{email}")]
        [ProducesResponseType(typeof(UserDetails),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDetails>> UserDetails(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return NotFound("user not found");
            }

            UserDetails details = new UserDetails
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
                JobTitle = user.JobTitle
            };

            return Ok(details);
        }


        //[HttpPost("Test")]
        //public IActionResult Test([FromHeader(Name = "Authorization")][Required] string requiredHeader)
        //{
        //    return Ok();
        //}
    }
}
