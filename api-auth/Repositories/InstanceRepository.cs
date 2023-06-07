using System;
using api_auth.Models;
using Microsoft.EntityFrameworkCore;

namespace api_auth.Repositories
{
    public interface IInstanceRepository
    {
        Task<Instance> GetFirstInstanceAsync();
    }

    public class InstanceRepository : IInstanceRepository
    {
        private readonly AppDbContext _context;

        public InstanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Instance> GetFirstInstanceAsync()
        {
            return await _context.Instances.FirstOrDefaultAsync();
        }
    }

}

