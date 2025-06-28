using Microsoft.EntityFrameworkCore;
using WebAPI2.Data.Config;

namespace WebAPI2.Data;

public class SchoolDbContext : DbContext
{
    public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Student?> Students { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfiguration(new StudentConfig()); 
        //we no longer have to go through the steps below anymore since we can have several tables
        //and not want to have verbose in our dbcontext class.. so each table has its own config and is called here

        
        
        // base.OnModelCreating(modelBuilder);
        //add default data
        // modelBuilder.Entity<Student>().HasData(new List<Student>()
        // {
        //     new Student
        //     {
        //         Id = 1,
        //         Name = "yej",
        //         Email = "yej@jey.com",
        //         Address = "port 11, esville, ah, riv, ng, af",
        //         DateOfBirth = new DateOnly(2000,04,13)
        //     },
        //     new Student
        //     {
        //         Id = 2,
        //         Name = "uk",
        //         Email = "uk@okey.com",
        //         Address = "port 11, esville, ah, riv, ng, af",
        //         DateOfBirth = new DateOnly(2006,04,07)
        //     },
        //     new Student
        //     {
        //         Id = 3,
        //         Name = "josh",
        //         Email = "josh@yeshua.com",
        //         Address = "port 13, esville, ah, riv, ng, af",
        //         DateOfBirth = new DateOnly(2001,03, 28)
        //     },
        // });
        
        //this is set in our studentconfig file nowso..

        
        //set schema rules (usually just to save db space because other rules can be set elsewhere
        // modelBuilder.Entity<Student>(entity =>
        // {
        //     entity.Property(p => p.Name).IsRequired().HasMaxLength(40);
        //     entity.Property(p => p.Email).IsRequired().HasMaxLength(40);
        //     entity.Property(p => p.Address).IsRequired(false).HasMaxLength(300);
        //     entity.Property(p => p.DateOfBirth).IsRequired();
        // });

        //setting this in studentconfig file now..
    }
}