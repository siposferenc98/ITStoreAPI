﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InfoBoltAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly infoboltContext _context;
        public UsersController(infoboltContext infoboltContext)
        {
            _context = infoboltContext;
        }

        /// <summary>
        /// Finds the user in the DB, creates claims and adds an AuthCookie to the response.
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<ActionResult<User?>> Login(User u)
        {
            User? user = _context.Users.FirstOrDefault(user => user.Email == u.Email && user.Pw == md5(u.Pw));
            if (user is not null)
            {
                var claimId = new Claim(ClaimTypes.NameIdentifier, user.Id.ToString());
                var claimEmail = new Claim(ClaimTypes.Name, user.Email);
                var claimsIdentity = new ClaimsIdentity(new[] { claimId, claimEmail }, "serverAuth");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);
            }
            return await Task.FromResult(user);
        }
        
        [HttpPost("ChangePassword/{email}")]
        public async Task<bool> ChangePassword(string email, [FromBody] List<string> passwords)
        {
            if (User.Identity.IsAuthenticated) //Gets the ClaimsPrincipal from the sent auth cookie.
            {
                var claimEmail = User.FindFirstValue(ClaimTypes.Name);
                if(claimEmail == email)
                {
                    User? user = await CheckPassword(email, passwords[0]);
                    if (user is null) //old password doesnt match
                        return false;
                    user.Pw = md5(passwords[1]);
                    if(await _context.SaveChangesAsync() > 0)
                    {
                        return true;
                    }
                    return false;
                }

            }
            return false;
        }
        private async Task<User?> CheckPassword(string email, string oldPw)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(user => user.Email == email && user.Pw == md5(oldPw));
            return user;
        }
        /// <summary>
        /// Used for checking if the user is authenticated or not.
        /// </summary>
        /// <returns>If the user is authenticated, we return a User with its email + id, if not we return an empty User.</returns>
        [HttpGet("GetUser")]
        public async Task<ActionResult<User>> GetUser()
        {
            User currentUser = new(); //We will return this user.

            if(User.Identity.IsAuthenticated) //Gets the ClaimsPrincipal from the sent auth cookie.
            {
                var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                User? dbUser = await _context.Users.FirstOrDefaultAsync(user => user.Id == int.Parse(id));
                if (dbUser is not null)
                {
                    currentUser.Email = dbUser.Email;
                    currentUser.Id = dbUser.Id;
                }

            }
            return await Task.FromResult(currentUser);
        }

        /// <summary>
        /// Deletes the client side Auth Cookie.
        /// </summary>
        /// <returns></returns>
        [HttpGet("Logout")]
        public async Task<ActionResult<string>> LogoutUser()
        {
            await HttpContext.SignOutAsync();
            return "Success";
        }

        /// <summary>
        /// Gets the user by ID.
        /// </summary>
        /// <param name="id">Id of user.</param>
        /// <returns>User or null</returns>
        [HttpGet("Profile/{id}")]
        public async Task<User?> GetProfile(int id)
        {
            if (User.Identity.IsAuthenticated) //Gets the ClaimsPrincipal from the sent auth cookie.
            {
                var loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if(int.Parse(loggedUserId) == id)
                    return await _context.Users.FirstOrDefaultAsync(user => id == user.Id);
            }
            return null;
        }

        /// <summary>
        /// Updates a user's profile by ID.
        /// </summary>
        /// <param name="id">User's ID.</param>
        /// <param name="u">Updated user object.</param>
        /// <returns>The updated user.</returns>
        [HttpPut("Profile/{id}")]
        public async Task<User?> UpdateProfile(int id, User u)
        {
            if (User.Identity.IsAuthenticated) //Gets the ClaimsPrincipal from the sent auth cookie.
            {
                var loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.Parse(loggedUserId) == id)
                {
                    User? updateThisUser = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);
                    if (updateThisUser is not null)
                    {

                        updateThisUser.Email = u.Email;

                        updateThisUser.Address = u.Address;
                        updateThisUser.Phone = u.Phone;
                        updateThisUser.City = u.City;

                        if (await _context.SaveChangesAsync() > 0)
                        {
                            return await Task.FromResult(u);
                        }
                    }
                }
            }

            return null;
        }

        [HttpPut]
        public async Task<User?> Register(User u)
        {
            if (_context.Users.Any(x => x.Email == u.Email))
                return null;

            u.Role = 1;
            u.Pw = md5(u.Pw);
            _context.Users.Add(u);

            if(await _context.SaveChangesAsync() > 0)
                return await Task.FromResult(u);

            return null;
        }

    }
}
