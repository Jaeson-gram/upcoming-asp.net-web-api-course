using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebAPI2.Data.Config;

public class DepartmentConfig : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");
        builder.HasKey(k => k.Id);

        builder.Property(p => p.Id).UseIdentityColumn();
        builder.Property(p => p.DepartmentName).IsRequired();
        builder.Property(p => p.FacultyName).IsRequired();
        builder.Property(p => p.Description).IsRequired(false);


        //pass default data
        builder.HasData(new List<Department>()
        {
            new Department
            {
                Id = 1,
                DepartmentName = "Systems Engineering, Design and Analysis",
                Description = "systems from the ground up",
                FacultyName = "Computing"
            },
            new Department
            {
                Id = 2,
                DepartmentName = "Artificial Intelligence",
                Description = "making computers intelligent",
                FacultyName = "Computing",
                
            },
            new Department
            {
                Id = 3,
                DepartmentName = "Cloud Computing",
                Description = "computing on the cloud",
                FacultyName = "Computing"
            },
            new Department
            {
                Id = 4,
                DepartmentName = "Quantum Physics",
                Description = "the physics of small things",
                FacultyName = "Physical Sciences",
            },
            new Department()
            {
                Id = 5,
                DepartmentName = "Theoretical Physics",
                Description = "when physics is simply observation and poetry",
                FacultyName = "Physical Sciences",
            },
            new Department()
            {
                Id = 6,
                DepartmentName = "Abstract Mathematics",
                Description = "the maths of the unseen",
                FacultyName = "Mathematical Sciences"
            },
            new Department()
            {
                Id = 7,
                DepartmentName = "Applied Mathematics",
                Description = "the maths of everyday things",
                FacultyName = "Mathematical Sciences"
            },
        });

    }
}