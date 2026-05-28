using Microsoft.AspNetCore.Identity;

namespace JMDB.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
