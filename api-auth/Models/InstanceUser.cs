using System;
namespace api_auth.Models
{
    public class InstanceUser
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int InstanceId { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public Instance? Instance { get; set; }
    }

}

