using Microsoft.AspNetCore.Identity;

namespace CineScope.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
