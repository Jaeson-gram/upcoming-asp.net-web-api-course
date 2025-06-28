using Microsoft.EntityFrameworkCore;
using AutoMapper;
using WebAPI2.Models;

namespace WebAPI2.Data.Repository;

public class StudentRepository : IStudentRepository
{
    private readonly SchoolDbContext _dbContext;
    private readonly IMapper _mapper;

    public StudentRepository(SchoolDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    
    public async Task<List<Student?>> GetAllAsync()
    {
        return await _dbContext.Students.ToListAsync();
    }

    public async Task<Student?> GetByIdAsync(int id, bool noTracking = false)
    {
        if (noTracking)
        {
            return await _dbContext.Students.AsNoTracking().Where(s => s.Id == id).FirstOrDefaultAsync();
        }
        // return await _dbContext.Students.FindAsync(id);
        
        return await _dbContext.Students.Where(s => s.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Student> GetByNameAsync(string name)
    {
        return await _dbContext.Students.Where(stu => stu.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync();
    }

    public async Task<Student> CreateAsync(Student student)
    {
        _dbContext.Students.Add(student);
        await _dbContext.SaveChangesAsync();
        return student;
    }

    public async Task<Student> Update(Student student)
    {
        //using the same query code but FirstOrDefault() and not FirstOrDefaultAsync() is not awaitable, in which case .Where() is used first, then FirstOrDefault()

        // await _dbContext.Students.ExecuteUpdateAsync(studentToUpdate);
        // await _dbContext.SaveChangesAsync();
        
        // it needs to return only after it's saved. so i need it synchronous so if anything goes wrong it will be visible before returning student
        _dbContext.Students.Update(student);
        _dbContext.SaveChanges();

        return student;
    }

    public bool Delete(int id)
    {
        var student = _dbContext.Students.FirstOrDefault(s => s.Id == id);

        if (student is not null)
        {
            _dbContext.Students.Remove(student);
            _dbContext.SaveChanges();
            return true;
        }

        return false;

    }
}