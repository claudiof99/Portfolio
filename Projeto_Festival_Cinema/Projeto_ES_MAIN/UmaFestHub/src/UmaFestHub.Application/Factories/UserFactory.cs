using System;
using System.Collections.Generic;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Enums;

namespace UmaFestHub.Application.Factories
{
    public static class UserFactory
    {
        public static User CreateCustomer(string name, string email, string passwordHash, string passwordSalt)
            => new User { Name = name, Email = email, PasswordHash = passwordHash, PasswordSalt = passwordSalt, Roles = new List<UserRole> { UserRole.Customer } };

        public static User CreateOrganizer(string name, string email, string passwordHash, string passwordSalt)
            => new User { Name = name, Email = email, PasswordHash = passwordHash, PasswordSalt = passwordSalt, Roles = new List<UserRole> { UserRole.Organizer } };

        public static User CreateAdmin(string name, string email, string passwordHash, string passwordSalt)
            => new User { Name = name, Email = email, PasswordHash = passwordHash, PasswordSalt = passwordSalt, Roles = new List<UserRole> { UserRole.Admin } };

        public static User CreateWithRole(string name, string email, string passwordHash, string passwordSalt, UserRole role)
            => new User { Name = name, Email = email, PasswordHash = passwordHash, PasswordSalt = passwordSalt, Roles = new List<UserRole> { role } };
    }
}
