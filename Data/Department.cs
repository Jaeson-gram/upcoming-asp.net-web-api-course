namespace WebAPI2.Data;

public class Department
{
    public int Id { get; set; }
    public string DepartmentName { get; set; }
    public string FacultyName { get; set; }
    public string? Description {get; set;}
    public virtual ICollection<Student> Students { get; set; }
}