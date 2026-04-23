using System;
using System.Collections.Generic;
using System.Linq;
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
                PasswordHash = HashPassword("Admin123!"),
                Role = "Admin",
                IsActive = true
            };

            _staffRepository.Add(admin);
            _staffRepository.SaveChanges();
        }

        public IEnumerable<Staff> GetAllStaffUsers()
        {
            return _staffRepository.GetAll();
        }

        public (bool Success, string? ErrorMessage) CreateStaffUser(
            string username,
            string password,
            string confirmPassword,
            string role,
            bool isActive)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return (false, "Username is required.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return (false, "Password is required.");
            }

            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                return (false, "Confirm password is required.");
            }

            if (string.IsNullOrWhiteSpace(role))
            {
                return (false, "Role is required.");
            }

            username = username.Trim();
            role = role.Trim();

            if (username.Length < 3 || username.Length > 50)
            {
                return (false, "Username must be between 3 and 50 characters.");
            }

            if (_staffRepository.GetByUsername(username) != null)
            {
                return (false, "Username already exists.");
            }

            if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                return (false, "Password and confirm password do not match.");
            }

            if (!IsValidPassword(password))
            {
                return (false, "Password must be at least 8 characters and include uppercase, lowercase, number, and special character.");
            }

            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(role, "Staff", StringComparison.OrdinalIgnoreCase))
            {
                return (false, "Role must be either 'Admin' or 'Staff'.");
            }

            var staff = new Staff
            {
                Username = username,
                PasswordHash = HashPassword(password),
                Role = NormalizeRole(role),
                IsActive = isActive
            };

            _staffRepository.Add(staff);
            _staffRepository.SaveChanges();

            return (true, null);
        }

        public (bool Success, string? ErrorMessage) SetStaffActiveStatus(int targetStaffId, bool isActive, int currentUserId)
        {
            var staff = _staffRepository.GetById(targetStaffId);
            if (staff == null)
            {
                return (false, "User not found.");
            }

            if (staff.Id == currentUserId && !isActive)
            {
                return (false, "You cannot deactivate your own account.");
            }

            if (staff.Role == "Admin" && !isActive && staff.IsActive)
            {
                var activeAdminCount = _staffRepository.CountActiveAdmins();
                if (activeAdminCount <= 1)
                {
                    return (false, "You cannot deactivate the last active admin.");
                }
            }

            staff.IsActive = isActive;
            _staffRepository.Update(staff);
            _staffRepository.SaveChanges();

            return (true, null);
        }

        public (bool Success, string? ErrorMessage) DeleteStaffUser(int targetStaffId, int currentUserId)
        {
            var staff = _staffRepository.GetById(targetStaffId);
            if (staff == null)
            {
                return (false, "User not found.");
            }

            if (staff.Id == currentUserId)
            {
                return (false, "You cannot delete your own account.");
            }

            if (staff.Role == "Admin" && staff.IsActive)
            {
                var activeAdminCount = _staffRepository.CountActiveAdmins();
                if (activeAdminCount <= 1)
                {
                    return (false, "You cannot delete the last active admin.");
                }
            }

            _staffRepository.Delete(staff);
            _staffRepository.SaveChanges();

            return (true, null);
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool IsValidPassword(string password)
        {
            return password.Length >= 8
                && password.Any(char.IsUpper)
                && password.Any(char.IsLower)
                && password.Any(char.IsDigit)
                && password.Any(ch => !char.IsLetterOrDigit(ch));
        }

        private string NormalizeRole(string role)
        {
            return string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase)
                ? "Admin"
                : "Staff";
        }
    }
}