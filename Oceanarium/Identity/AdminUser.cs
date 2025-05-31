using Microsoft.AspNetCore.Identity;

namespace Oceanarium.Identity
{
    public class AdminUser : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsTemporary { get; set; } = true;
    }
}
