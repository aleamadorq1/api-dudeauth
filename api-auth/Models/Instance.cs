using System;
namespace api_auth.Models
{
    public class Instance
    {
        public int Id { get; set; }
        public string? UrlEndpoint { get; set; }
        public string? Name { get; set; }
        public int OwnerUserId { get; set; }
        public bool IsActive { get; set; }

        // Navigation property
        public User? OwnerUser { get; set; }
    }

}

