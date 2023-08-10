using Microsoft.AspNetCore.Identity;

namespace Backend.Models
{
    public class AppUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? OTP { get; set; }
        public Basket? Basket { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<BlogComment>? BlogComments { get; set; }
    }
}
