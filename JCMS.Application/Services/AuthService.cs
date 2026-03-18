using System;
using System.Security.Cryptography;
using System.Text;
using JCMS.Infrastructure.Entities;
using JCMS.Infrastructure.Repositories;

namespace JCMS.Application.Services
{
    public class AuthService
    {
        private readonly IStaffRepository _staffRepository;

        public AuthService(IStaffRepository staffRepository)
        {
            _staffRepository = staffRepository;
        }

        public Staff? Authenticate(string username, string password, out string? errorMessage)
        {
            errorMessage = null;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Username and password are required.";
                return null;
            }

            var staff = _staffRepository.GetByUsername(username);
            if (staff == null || !staff.IsActive)
            {
                errorMessage = "Invalid username or password.";
                return null;
            }

            var hashedInput = HashPassword(password);
            if (!string.Equals(hashedInput, staff.PasswordHash, StringComparison.Ordinal))
            {
                errorMessage = "Invalid username or password.";
                return null;
            }

            return staff;
        }

        public void SeedAdminIfNeeded()
        {
            var existing = _staffRepository.GetByUsername("admin");
            if (existing != null)
            {
                return;
            }

            var admin = new Staff
            {
                Username = "admin",
                PasswordHash = HashPassword("Admin123!"), // default dev password
                Role = "Admin",
                IsActive = true
            };

            _staffRepository.Add(admin);
            _staffRepository.SaveChanges();
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
