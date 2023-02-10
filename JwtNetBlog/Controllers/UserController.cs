using JwtNetBlog.Data;
using Microsoft.AspNetCore.Identity;
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

        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        // GET Requests
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAll()
        {
            return Ok(await _context.Users.ToListAsync());
        }

        // POST Requests
        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser(UserRegistrationDto request)
        {
            // Check if submitted passwords match
            if (request.Password != request.PasswordConfirm)
            {
                return BadRequest("Passwords do not match");
            }

            // Verify that username and email are not already registered
            var verifyUsername = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            var verifyEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

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
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt

            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            
            if (user == null) 
            {
                return BadRequest("Username not found");
            }

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt)) 
            { 
                return BadRequest("User and Password do not match");
            }

            return Ok("Logged In");
        }

        // PUT requests
        [HttpPut]
        public async Task<ActionResult<User>> PutUser(UserPutDto request)
        {
            // Check for user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
            {
                return BadRequest("Username not found");
            }
            
            // Check that passwords match
            if (request.Password != null && request.PasswordConfirm != null)
            {
                if (request.Password != request.PasswordConfirm)
                {
                    return BadRequest("Passwords do not match");
                }

                CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            // Change FirstName if included in request
            if (request.FirstName!= null)
            {
                user.FirstName = request.FirstName;
            }

            // Change LastName if included in request
            if (request.LastName!= null)
            {
                user.LastName = request.LastName;
            }

            // Change Email if included in request
            if (request.Email!= null)
            {
                var verifyEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

                if (verifyEmail != null)
                {
                    return BadRequest("Email are already in use");
                }
                user.Email = request.Email;
            }

            await _context.SaveChangesAsync();

            return (user);
        }

        // DELETE requests
        [HttpDelete("{username}")]
        public async Task<ActionResult<string>> DeleteUser(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return BadRequest("Username not found");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok($"{username} deleted");
        }

    }
}
