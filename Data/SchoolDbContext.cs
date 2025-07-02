using Microsoft.EntityFrameworkCore;
using WebAPI2.Data.Config;

namespace WebAPI2.Data;

public class SchoolDbContext : DbContext
{
    public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Student> Students { get; set; }
    public DbSet<Department> Departments {get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // add configurations from config files...
        modelBuilder.ApplyConfiguration(new StudentConfig()); 
        modelBuilder.ApplyConfiguration(new DepartmentConfig());

    }
}