using Microsoft.EntityFrameworkCore;
using Supervaga.Models;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }

    public DbSet<Ad> Ads { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<Requirement> Requirements { get; set; }
    public DbSet<Advantage> Advantages { get; set; }
}