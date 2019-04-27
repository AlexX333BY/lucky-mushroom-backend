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
        public async Task<IActionResult> SignUp([FromBody] UserCredentials userCredentials)
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

                await _context.UserCredentials.AddAsync(new UserCredentials() { UserId = newUser.UserId, UserMail = trimmedEmail, UserPasswordHash = trimmedPasswordHash });
                await _context.SaveChangesAsync();

                transaction.Commit();
                await Authenticate(_context.UserCredentials.Where((creds) => creds.UserId == newUser.UserId).Single(), newUserRole);

                return Created(nameof(SignUp), new User { UserId = newUser.UserId, RoleId = newUser.UserId, 
                    Role = new Role { RoleAlias = newUser.Role.RoleAlias, RoleName = newUser.Role.RoleName }, 
                    UserCredentials = new UserCredentials { UserMail = newUser.UserCredentials.UserMail } });
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromBody] UserCredentials userCredentials)
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
                await Authenticate(
                    authorizedCreds, 
                    _context.Roles.Where((role) => role.RoleId == _context.Users.Where((user) => user.UserId == authorizedCreds.UserId).Single().RoleId).Single()
                );
                return Ok(authorizedCreds.UserMail);
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

            var deletedUser = await _context.Users.Where((user) => user.UserCredentials.UserMail == User.Identity.Name).SingleOrDefaultAsync();
            if (deletedUser == null)
            {
                return NotFound();
            }

            _context.Users.Remove(deletedUser);
            await _context.SaveChangesAsync();

            return await LogOut();
        }

        private async Task Authenticate(UserCredentials creds, Role role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, creds.UserMail),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role.RoleAlias)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
