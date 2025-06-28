using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace WebAPI2.Data.Repository;

public class SchoolRepository<T> : ISchoolRepository<T> where T : class
{
    private readonly SchoolDbContext _dbContext;
    private DbSet<T> _dbSet;

    public SchoolRepository(SchoolDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }
    
    public async Task<List<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }


     public async Task<T?> GetByIdAsync(Expression<Func<T?, bool>> filter, bool noTracking = false)
     {
            if (noTracking)
            {
                return await _dbSet.AsNoTracking().Where(filter).FirstOrDefaultAsync();
            }
            // return await _dbContext.Students.FindAsync(id);
            
            return await _dbSet.Where(filter).FirstOrDefaultAsync();
     }

     public async Task<T?> GetByNameAsync(Expression<Func<T, bool>> filter)
     {
         return await _dbSet.Where(filter).FirstOrDefaultAsync();
     }

    public async Task<T> CreateAsync(T dbRecord)
    {
        //i need this first so not awaiting...
        _dbSet.Add(dbRecord); 
        
        await _dbContext.SaveChangesAsync();
        return dbRecord;
    }

    public async Task<T> Update(T dbRecord)
    {
        _dbSet.Update(dbRecord);
        _dbContext.SaveChanges();

        return dbRecord;
    }

    public bool Delete(T dbRecord)
    {
        _dbSet.Remove(dbRecord); 
        _dbContext.SaveChanges();
        return true;    
    }
}