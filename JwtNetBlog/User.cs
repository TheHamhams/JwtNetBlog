using System.ComponentModel.DataAnnotations;

namespace JwtNetBlog
{
    public class User
    {
        public int Id { get; set; }
        [MaxLength(20)]
        public string Username { get; set; }
        [MaxLength(20)]
        public string FirstName { get; set; } = string.Empty;
        [MaxLength(20)]
        public string LastName { get; set; } = string.Empty;
        [MaxLength]
        public string Email { get; set; } 
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
