namespace WebAPI2.Data.Repository;

public interface IStudentRepository
{
    Task<List<Student?>> GetAllAsync();
    Task<Student?> GetByIdAsync(int id, bool noTracking);
    Task<Student> GetByNameAsync(string name);
    Task<Student> CreateAsync(Student student);
    Task<Student> Update(Student student);
    bool Delete(int id);

}