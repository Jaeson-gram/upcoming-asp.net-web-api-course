using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebAPI2.Data.Config;

public class StudentConfig : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");
        builder.HasKey(k => k.Id);

        builder.Property(b => b.Id).UseIdentityColumn();
        builder.Property(s => s.Name).IsRequired().HasMaxLength(40);
        builder.Property(s => s.Email).IsRequired().HasMaxLength(60);
        builder.Property(s => s.Address).IsRequired(false).HasMaxLength(300);
        builder.Property(s => s.DateOfBirth).IsRequired();

        //pass default data
        builder.HasData(new List<Student>()
        {
            new Student
            {
                Id = 1,
                Name = "yej",
                Email = "yej@jey.com",
                Address = "port 11, esville, ah, riv, ng, af",
                DateOfBirth = new DateTime(2000, 04, 13)
            },
            new Student
            {
                Id = 2,
                Name = "uk",
                Email = "uk@okey.com",
                Address = "port 11, esville, ah, riv, ng, af",
                DateOfBirth = new DateTime(2006, 04, 07)
            },
            new Student
            {
                Id = 3,
                Name = "josh",
                Email = "josh@yeshua.com",
                Address = "port 13, esville, ah, riv, ng, af",
                DateOfBirth = new DateTime(2001, 03, 28)
            },
        });

    }
}