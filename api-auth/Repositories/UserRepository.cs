using System;
using api_auth.Models;
using Microsoft.EntityFrameworkCore;

namespace api_auth.Repositories
{
    public interface IUserRepository
    {
        Task<User> AddAsync(User user);
        Task<User> GetByEmailAsync(string email);
        Task<PasswordResetToken> CreatePasswordResetTokenAsync(int userId);
        Task<User> GetUserByPasswordResetTokenAsync(string token);
        Task UpdateUserPasswordAsync(User user);
        // Add other necessary methods
    }

    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Mail == email);
        }

        public async Task<PasswordResetToken> CreatePasswordResetTokenAsync(int userId)
        {
            var token = new PasswordResetToken
            {
                UserId = userId,
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddHours(2) // The token is valid for 2 hours
            };

            await _context.PasswordResetTokens.AddAsync(token);
            await _context.SaveChangesAsync();

            return token;
        }

        public async Task<User> GetUserByPasswordResetTokenAsync(string token)
        {
            var resetToken = await _context.PasswordResetTokens.FirstOrDefaultAsync(t => t.Token == token && t.Expires > DateTime.UtcNow);
            if (resetToken == null)
                return null;

            return await _context.Users.FindAsync(resetToken.UserId);
        }

        public async Task UpdateUserPasswordAsync(User user)
        {
            var dbUser = await _context.Users.FindAsync(user.Id);
            if (dbUser != null)
            {
                dbUser.PasswordHash = user.PasswordHash;
                dbUser.PasswordSalt = user.PasswordSalt;

                // Delete any existing password reset tokens for this user
                var tokens = _context.PasswordResetTokens.Where(t => t.UserId == user.Id);
                _context.PasswordResetTokens.RemoveRange(tokens);

                await _context.SaveChangesAsync();
            }
        }
    }
}

