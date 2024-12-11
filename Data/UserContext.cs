using System;
using Microsoft.EntityFrameworkCore;
using userproj.Models;

namespace userproj.Data;

public class UserContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public UserContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=users.db");
    }
}
