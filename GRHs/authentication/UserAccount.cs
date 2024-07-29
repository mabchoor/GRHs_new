using GRHs.Data;
using GRHs.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace GRHs.authentication
{
    public class UserAccount
    {
        private readonly EmployeeManagementDbContext _dbContext;

        public UserAccount()
        {
            _dbContext = DbContextProvider.Instance;
        }

        public EmployeeManagementDbContext DbContext => _dbContext;


        // Register a new user with additional details
        public async Task<(string Message, bool Status)> RegisterAsync(string username, string password, string email, string name, string imagePath = null, int role = 0)
        {
            // Validate input
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
            {
                return ("All fields are required.", false);
            }

            // Check if the username or email already exists
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == username || u.Email == email);

            if (existingUser != null)
            {
                return ("Username or email already exists.", false);
            }

            // Hash the password
            var hashedPassword = HashPassword(password);

            // Create a new user with default role
            var user = new Entities.Users
            {
                Username = username,
                Password = hashedPassword,
                Email = email,
                Name = name,
                Image = imagePath, // Allow for optional image
                Role = role // Set default role here
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return ("User registered successfully.", true);
        }



        public async Task<(Entities.Users User, UserSessions UserSession, string Message)> LoginAsync(string pseudo, string password)
        {
            // Find the user with the given username or email
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == pseudo || u.Email == pseudo);

            if (user == null || !VerifyPassword(password, user.Password))
            {
                return (null, null, "Invalid username or email or password.");
            }

            // Create a new session
            UserSessions userSession = CreateUserSession(user.UserID);

            return (user, userSession, "Login successful.");
        }



        // Helper method to hash a password
        private string HashPassword(string password)
        {
            // Generate a salt
            byte[] salt = new byte[16];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Hash the password
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32));

            // Combine salt and hashed password
            return Convert.ToBase64String(salt) + ":" + hashed;
        }

        // Helper method to verify a password
        private bool VerifyPassword(string password, string storedPassword)
        {
            // Extract the salt and hash from the stored password
            var parts = storedPassword.Split(':');
            var salt = Convert.FromBase64String(parts[0]);
            var storedHash = parts[1];

            // Hash the provided password
            var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32));

            // Compare hashes
            return hash == storedHash;
        }

        // Helper method to create a user session (optional)
        private UserSessions CreateUserSession(int userId)
        {
            UserSessions userSession = new UserSessions
            {
                UserId = userId,
                LoginTime = DateTime.Now,
                ExpirationTime = DateTime.Now.AddMinutes(60)  // Set expiration time (e.g., 30 minutes)
            };

            _dbContext.UserSessions.Add(userSession);
            _dbContext.SaveChanges();
            return userSession;

        }
        
    }
}