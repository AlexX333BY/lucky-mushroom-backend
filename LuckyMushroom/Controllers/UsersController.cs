using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuckyMushroom.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace LuckyMushroom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly LuckyMushroomContext _context;

        public UsersController(LuckyMushroomContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] string email, [FromBody] string sha512PasswordHash)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            const byte hashLength = 128;
            string trimmedEmail = email.Trim().ToLower(), trimmedPasswordHash = sha512PasswordHash.Trim().ToUpper();

            if ((trimmedEmail.Length == 0) || (trimmedPasswordHash.Length != hashLength))
            {
                return BadRequest("Illegal register data");
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                if (await _context.UserCredentials.AnyAsync((creds) => creds.UserMail == trimmedEmail))
                {
                    return BadRequest("Email already exists");
                }

                Role newUserRole = await _context.Roles.Where((role) => role.RoleAlias == "user").FirstAsync();

                User newUser = (await _context.Users.AddAsync(new User() { RoleId = newUserRole.RoleId })).Entity;

                await _context.UserCredentials.AddAsync(new UserCredentials() { UserId = newUser.UserId, UserMail = trimmedEmail, UserPasswordHash = trimmedPasswordHash });
                await _context.SaveChangesAsync();

                transaction.Commit();
                await Authenticate(newUser);

                return Created("signup", (newUser.UserId, newUser.UserCredentials.UserMail));
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromBody] string email, [FromBody] string sha512PasswordHash)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            const byte hashLength = 128;
            string trimmedEmail = email.Trim().ToLower(), trimmedPasswordHash = sha512PasswordHash.Trim().ToUpper();

            if ((trimmedEmail.Length == 0) || (trimmedPasswordHash.Length != hashLength))
            {
                return BadRequest("Illegal login data");
            }

            UserCredentials authorizedCreds = await _context.UserCredentials
                .Where((creds) => creds.UserMail == trimmedEmail && creds.UserPasswordHash == trimmedPasswordHash).SingleOrDefaultAsync();
            if (authorizedCreds != null)
            {
                await Authenticate(authorizedCreds.User);
                return Ok((authorizedCreds.UserId, authorizedCreds.UserMail));
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var deletedUser = await _context.Users.Where((user) => user.UserCredentials.UserMail == User.Identity.Name).SingleOrDefaultAsync();
            if (deletedUser == null)
            {
                return NotFound();
            }

            _context.Users.Remove(deletedUser);
            await _context.SaveChangesAsync();

            return await LogOut();
        }

        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserCredentials.UserMail),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.RoleAlias)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
