using System.Security.Claims;

namespace JwtNetBlog.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly HttpContextAccessor _httpContextAccessor;

        public UserService()
        {
            _httpContextAccessor = new HttpContextAccessor();
        }

        public string GetMyName()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            }

            return result;
        }
    }
}
