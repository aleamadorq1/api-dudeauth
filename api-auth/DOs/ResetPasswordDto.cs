using System;
namespace api_auth.DOs
{
    public class ResetPasswordDto
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}

