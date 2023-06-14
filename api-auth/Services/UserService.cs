using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using api_auth.DOs;
using api_auth.Models;
using api_auth.Repositories;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace api_auth.Services
{
    public interface IUserService
    {
        Task<UserResponseDto> RegisterAsync(UserDto userDto);
        Task<UserResponseDto> LoginAsync(UserDto userDto);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(int userId, string token, string newPassword);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IInstanceRepository _instanceRepository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IInstanceRepository instanceRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _instanceRepository = instanceRepository;
            _configuration = configuration;
        }

        public async Task<UserResponseDto> RegisterAsync(UserDto userDto)
        {
            // Hash and salt password
            var salt = GenerateSalt();
            var hashedPassword = HashPassword(userDto.Password, salt);

            var user = new User
            {
                Mail = userDto.Mail,
                PasswordHash = hashedPassword,
                PasswordSalt = salt,
                Name = userDto.Name,
                LastName = userDto.LastName
            };

            user = await _userRepository.AddAsync(user);

            var token = GenerateToken(user);

            // Here you will implement your logic to assign an API instance to the user.
            // For simplicity, we are just getting the first instance.
            var instance = await _instanceRepository.GetFirstInstanceAsync();

            return new UserResponseDto
            {
                Mail = user.Mail,
                Name = user.Name,
                LastName = user.LastName,
                Token = token,
                InstanceUrl = instance?.UrlEndpoint
            };
        }

        public async Task<UserResponseDto> LoginAsync(UserDto userDto)
        {
            var user = await _userRepository.GetByEmailAsync(userDto.Mail);
            if (user == null)
            {
                return null;
            }

            var hashedPassword = HashPassword(userDto.Password, user.PasswordSalt);
            if (user.PasswordHash != hashedPassword)
            {
                return null;
            }

            var token = GenerateToken(user);

            // Again, just getting the first instance for simplicity.
            var instance = await _instanceRepository.GetFirstInstanceAsync();

            return new UserResponseDto
            {
                Mail = user.Mail,
                Name = user.Name,
                LastName = user.LastName,
                Token = token,
                InstanceUrl = instance?.UrlEndpoint
            };
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return false;

            var token = await _userRepository.CreatePasswordResetTokenAsync(user.Id);

            // Send an email to the user with the reset token
            // This is a placeholder function. Replace it with your actual email sending code.
            //SendResetPasswordEmail(user.Email, token.Token);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(int userId, string token, string newPassword)
        {
            var user = await _userRepository.GetUserByPasswordResetTokenAsync(token);
            if (user == null || user.Id != userId)
                return false;

            user.PasswordSalt = GenerateSalt();
            user.PasswordHash = HashPassword(newPassword, user.PasswordSalt);
            await _userRepository.UpdateUserPasswordAsync(user);

            return true;
        }

        private string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Mail)
            }),
                Expires = user.IsAppUser! ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddYears(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private string GenerateSalt()
        {
            byte[] bytes = new byte[128 / 8];
            using var keyGenerator = RandomNumberGenerator.Create();
            keyGenerator.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private string HashPassword(string password, string salt)
        {
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return hashed;
        }
    }
}

