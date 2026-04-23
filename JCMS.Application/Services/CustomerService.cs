using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JCMS.Infrastructure.Entities;
using JCMS.Infrastructure.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace JCMS.Application.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public IEnumerable<Customer> Search(string? term)
        {
            return _customerRepository.Search(term);
        }

        public Customer? GetById(int id)
        {
            return _customerRepository.GetById(id);
        }

        public (bool Success, string? ErrorMessage) CreateCustomer(string firstName, string lastName, string email, string phone)
        {
            firstName = firstName.Trim();
            lastName = lastName.Trim();
            email = email.Trim();
            phone = phone.Trim();

            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone))
            {
                return (false, "All fields are required");
            }

            if (!IsValidEmail(email))
            {
                return (false, "Email format is invalid");
            }

            if (!IsValidPhone(phone))
            {
                return (false, "Phone number format must be (XXX) XXX-XXXX");
            }

            var existingCustomer = _customerRepository.GetByEmail(email);
            if (existingCustomer != null)
            {
                return (false, "Another customer already uses this email");
            }

            var customer = new Customer
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone
            };

            try
            {
                _customerRepository.Add(customer);
                _customerRepository.SaveChanges();
            }
            catch (DbUpdateException ex) when (IsDuplicateEmailException(ex))
            {
                return (false, "Another customer already uses this email");
            }

            return (true, null);
        }

        public (bool Success, string? ErrorMessage) UpdateCustomer(int id, string email, string phone)
        {
            email = email.Trim();
            phone = phone.Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone))
            {
                return (false, "Email and phone are required");
            }

            if (!IsValidEmail(email))
            {
                return (false, "Email format is invalid");
            }

            if (!IsValidPhone(phone))
            {
                return (false, "Phone number format must be (XXX) XXX-XXXX");
            }

            var customer = _customerRepository.GetById(id);
            if (customer == null)
            {
                return (false, "Customer not found");
            }

            var existingWithEmail = _customerRepository.GetByEmail(email);
            if (existingWithEmail != null && existingWithEmail.Id != id)
            {
                return (false, "Another customer already uses this email");
            }

            customer.Email = email;
            customer.Phone = phone;

            try
            {
                _customerRepository.Update(customer);
                _customerRepository.SaveChanges();
            }
            catch (DbUpdateException ex) when (IsDuplicateEmailException(ex))
            {
                return (false, "Another customer already uses this email");
            }

            return (true, null);
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsValidPhone(string phone)
        {
            var pattern = @"^\(\d{3}\) \d{3}-\d{4}$";
            return Regex.IsMatch(phone, pattern);
        }

        private static bool IsDuplicateEmailException(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                return (sqlEx.Number == 2601 || sqlEx.Number == 2627) &&
                       sqlEx.Message.Contains("IX_Customers_Email");
            }

            if (ex.InnerException?.InnerException is SqlException innerSqlEx)
            {
                return (innerSqlEx.Number == 2601 || innerSqlEx.Number == 2627) &&
                       innerSqlEx.Message.Contains("IX_Customers_Email");
            }

            return false;
        }
    }
}