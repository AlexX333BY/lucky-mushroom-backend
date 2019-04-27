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
using LuckyMushroom.DataTransferObjects;

namespace LuckyMushroom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        private readonly LuckyMushroomContext _context;

        public UsersController(LuckyMushroomContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] UserCredentialsDto userCredentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            const byte hashLength = 128;
            string trimmedEmail = userCredentials.UserMail?.Trim().ToLower(), trimmedPasswordHash = userCredentials.UserPasswordHash?.Trim().ToUpper();

            if ((trimmedEmail == null) || (trimmedPasswordHash == null) || (trimmedEmail.Length == 0) || (trimmedPasswordHash.Length != hashLength))
            {
                return BadRequest("Illegal register data");
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                /*  
                    Exception is throwed if using just Any(Async):
                        System.InvalidOperationException: "No coercion operator is defined between types 'System.Int16' and 'System.Boolean'."
                    Seems to be Oracle connector problem
                */
                if (_context.UserCredentials.ToArray().Any((creds) => creds.UserMail == trimmedEmail))
                {
                    return BadRequest("Email already exists");
                }

                Role newUserRole = _context.Roles.Where((role) => role.RoleAlias == "user").FirstOrDefault();
                if (newUserRole == null)
                {
                    throw new Exception("Error creating user: no needed role");
                }

                User newUser = _context.Users.Add(new User() { RoleId = newUserRole.RoleId }).Entity;
                newUser.Role = newUserRole;
                newUser.UserCredentials = _context.UserCredentials.Add(new UserCredentials() { UserId = newUser.UserId, UserMail = trimmedEmail, UserPasswordHash = trimmedPasswordHash }).Entity;

                await _context.SaveChangesAsync();
                transaction.Commit();

                await Authenticate(newUser);

                return Created(nameof(SignUp), new UserDto(newUser, false));
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromBody] UserCredentialsDto userCredentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            const byte hashLength = 128;
            string trimmedEmail = userCredentials.UserMail?.Trim().ToLower(), trimmedPasswordHash = userCredentials.UserPasswordHash?.Trim().ToUpper();

            if ((trimmedEmail == null) || (trimmedPasswordHash == null) || (trimmedEmail.Length == 0) || (trimmedPasswordHash.Length != hashLength))
            {
                return BadRequest("Illegal login data");
            }

            UserCredentials authorizedCreds = _context.UserCredentials
                .Where((creds) => creds.UserMail == trimmedEmail && creds.UserPasswordHash == trimmedPasswordHash).SingleOrDefault();
            if (authorizedCreds != null)
            {
                User loginUser = _context.Users.Where((user) => user.UserId == authorizedCreds.UserId).Single();
                loginUser.Role = _context.Roles.Where((role) => role.RoleId == loginUser.RoleId).Single();
                loginUser.UserCredentials = authorizedCreds;
                await Authenticate(loginUser);
                return Ok(new UserDto(loginUser, false));
            }
            else
            {
                return BadRequest("Illegal login data");
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

            var deletedUser = _context.Users.Where((user) => user.UserCredentials.UserMail == User.Identity.Name).SingleOrDefault();
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
