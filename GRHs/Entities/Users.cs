using System.Collections.Generic;

namespace GRHs.Entities
{
    public class Users
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // Store hashed passwords for security
        public string Image { get; set; } // the image is stored as a path or base64 string
        public string Name { get; set; }
        public int Role { get; set; }

        // Navigation property
        public ICollection<UserSessions> UserSessions { get; set; }
    }
}
