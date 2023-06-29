using System;
using api_auth.DOs;
using api_auth.Services;
using Microsoft.AspNetCore.Mvc;

namespace api_auth.Controllers
{
    [Route("auth/api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // POST api/user/register
        [HttpPost("register")]
        public async Task<ActionResult> Register(UserDto userDto)
        {
            var user = await _userService.RegisterAsync(userDto);
            if (user == null) return BadRequest("Registration failed");
            return Ok(user);
        }

        // POST api/user/login
        [HttpPost("login")]
        public async Task<ActionResult> Login(UserDto userDto)
        {
            var user = await _userService.LoginAsync(userDto);
            if (user == null) return Unauthorized();
            return Ok(user);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var result = await _userService.ForgotPasswordAsync(email);

            if (!result)
                return BadRequest(new { message = "There was a problem processing your request. Please try again later." });

            return Ok();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var result = await _userService.ResetPasswordAsync(resetPasswordDto.UserId, resetPasswordDto.Token, resetPasswordDto.NewPassword);

            if (!result)
                return BadRequest(new { message = "Invalid or expired reset token." });

            return Ok();
        }
    }

}

