﻿using JwtNetBlog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace JwtNetBlog.Controllers
{

    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public static List<User> _users = new List<User>
        {
            new User
            {
                Id = 0,
                Username = "Username",
                FirstName = "First",
                LastName = "Last",
                Email = "Email",
                PasswordHash = Array.Empty<byte>(),
                PasswordSalt = Array.Empty<byte>()
            }
        };
        
        private readonly DataContext _context;

        public UserController(DataContext context) 
        {
            _context = context;
        }

        // Password Hashing Functions
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        // GET Requests
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAll()
        {
            return Ok(await _context.Users.ToListAsync());
        }

        // POST Requests
        [HttpPost]
        public async Task<ActionResult<User>> RegisterUser(UserRegistrationDto request)
        {
            // Check if submitted passwords match
            if (request.Password != request.PasswordConfirm)
            {
                return BadRequest("Passwords do not match");
            }

            // Verify that username and email are not already registered
            var verifyUsername = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            var verifyEmail = await _context.Users.FirstOrDefaultAsync(_u => _u.Email == request.Email);

            if (verifyUsername != null || verifyEmail != null) 
            {
                return BadRequest("Username or Email are already in use");
            }

            // Create user
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            User user = new User
            {
                Username = request.Username,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash= passwordHash,
                PasswordSalt= passwordSalt

            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            return Ok(user);
        }
    }
}
