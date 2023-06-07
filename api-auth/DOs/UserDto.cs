using System;
namespace api_auth.DOs
{
	public class UserDto
	{
        public string? Mail { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
    }
}

