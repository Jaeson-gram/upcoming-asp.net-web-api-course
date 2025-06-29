namespace WebAPI2.Data.Repository;

// inherits from the ICommonRepo so we can add just table (student) specific functionality to this
public interface IStudentRepository : ISchoolRepository<Student>
{
    // student specific functionality
    
    // Task<List<Student>> GetByGradeLevelAsync(char gradeLevel);
}

// general functionality will still be done in the common repo, and specific will be done here