using System.Collections.Generic;

namespace WebApplication2.Models
{
    public class AdminUserViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Roller
        public bool IsAdmin { get; set; }
        public bool IsRegisterforer { get; set; }
        public bool IsPilot { get; set; }

        public IList<string> Roles { get; set; } = new List<string>();
    }
}
