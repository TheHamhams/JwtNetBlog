namespace JwtNetBlog
{
    public class UserRegistrationDto
    {
        public string Usersame { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public string Password { get; }
        public string PasswordConfirm { get; }
    }
}
