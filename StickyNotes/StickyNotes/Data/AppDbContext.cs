using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StickyNotes.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<StickyNote> StickyNotes { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<UserRole> UserRoles { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<User>()
            .HasMany(u => u.StickyNotes)
            .WithOne(sn => sn.User)
            .HasForeignKey(sn => sn.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .Property(u => u.Username)
            .IsRequired();

        modelBuilder.Entity<StickyNote>()
            .Property(sn => sn.Content)
            .IsRequired();

        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Manager" },
            new Role { Id = 3, Name = "User" });

        // seed admin
        var hasher = new PasswordHasher<User>();
        var adminUser = new User
        {
            Id = 1,
            Username = "admin",
            IsDisabled = false
        };
        adminUser.PasswordHash = hasher.HashPassword(adminUser, "admin123");

        modelBuilder.Entity<User>().HasData(adminUser);

        modelBuilder.Entity<UserRole>().HasData(
            new UserRole
            {
                UserId = 1,
                RoleId = 1
            }
        );



    }
}