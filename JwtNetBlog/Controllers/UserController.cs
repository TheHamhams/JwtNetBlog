using Microsoft.AspNetCore.Mvc;


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
    }
}
