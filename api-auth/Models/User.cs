using System;
namespace api_auth.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Mail { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? PasswordHash { get; set; }
        public string? PasswordSalt { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsAppUser { get; set; }
    }

}

