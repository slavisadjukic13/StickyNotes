using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Try environment variable
        var connStr = Environment.GetEnvironmentVariable("DefaultConnection");

        if (string.IsNullOrEmpty(connStr))
        {
            // Fallback if .env not loaded during design time
            connStr = "Server=DESKTOP-MGO5819\\SQLEXPRESS;Database=StickyNotesDb;Trusted_Connection=True;MultipleActiveResultSets=true";
        }

        optionsBuilder.UseSqlServer(connStr);

        return new AppDbContext(optionsBuilder.Options);
    }
}