using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Heysundue.Models;
public class ArticleContext : DbContext
{
    public ArticleContext(DbContextOptions<ArticleContext> options)
        : base(options)
    {
    }

    public DbSet<Article> Article { get; set; } = default!;
    public DbSet<Person> Persons { get; set; } = default!;
    public DbSet<Member> Members { get; set; } = default!;
    public DbSet<Registration> Registrations { get; set; } = default!;
    public DbSet<Login> Login { get; set; } = default!;
    public DbSet<Joinlist> Joinlists { get; set; } = default!;
    public DbSet<Sessionuser> Sessionusers { get; set; } = default!;
    public DbSet<Doorsystem> Doorsystems { get; set; } = default!;
    public DbSet<Meeting> Meetings { get; set; }
    public DbSet<Accessdoor> Accessdoors { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Login as a keyless entity
        modelBuilder.Entity<Login>().HasNoKey();

        // Configure the relationship between Sessionuser and Meeting with cascade delete
        modelBuilder.Entity<Sessionuser>()
            .HasOne<Meeting>()
            .WithMany(m => m.Sessionusers)
            .HasForeignKey(s => s.MeetingID)
            .OnDelete(DeleteBehavior.Cascade);

        // Add other entity configurations here if needed

        base.OnModelCreating(modelBuilder);
    }
    
        private readonly string _connectionString;
}

