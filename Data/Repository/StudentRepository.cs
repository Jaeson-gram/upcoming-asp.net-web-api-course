using Microsoft.EntityFrameworkCore;
using AutoMapper;
using WebAPI2.Models;

namespace WebAPI2.Data.Repository;

// inherits all the implementations from the commonRepo so we can add just table (student) specific functionality to this
// general functionality will still be done in the common repo, and specific will be done here
public class StudentRepository : SchoolRepository<Student>, IStudentRepository
{
    private readonly SchoolDbContext _dbContext;
    private readonly IMapper _mapper;

    public StudentRepository(SchoolDbContext dbContext, IMapper mapper) : base(dbContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    // public Task<List<Student>> GetByGradeLevelAsync(char gradeLevel)
    // {
    //     //return bla bla bla
    // }
}