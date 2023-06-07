using api_auth.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Instance> Instances { get; set; }
    public DbSet<InstanceUser> InstanceUsers { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

}
