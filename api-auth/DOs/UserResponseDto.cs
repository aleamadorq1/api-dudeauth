using System;
namespace api_auth.DOs
{
    public class UserResponseDto
    {
        public string Mail { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Token { get; set; }
        public string InstanceUrl { get; set; }
    }

}

