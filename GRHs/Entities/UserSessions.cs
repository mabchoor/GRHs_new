using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRHs.Entities
{
    public class UserSessions
    {
        public int Id { get; set; }
        public int UserId { get; set; }  // Foreign key property
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public DateTime ExpirationTime { get; set; }

        // Navigation property
        public Users User { get; set; }
    }
}