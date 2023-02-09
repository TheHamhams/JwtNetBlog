namespace JwtNetBlog
{
    public interface UserRegistrationDto
    {
        string Usersame { get; }
        string FirstName { get; }
        string LastName { get; }
        string Email { get; }
        string Password { get; }
        string PasswordConfirm { get; }
    }
}
