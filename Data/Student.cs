using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI2.Models;

namespace WebAPI2.Data;

public class Student
{
    // [Key] set as key in student config file
    // [DatabaseGenerated(DatabaseGeneratedOption.Identity)] set to autogenere in student config file
    public int Id
    {
        get;
        set;
    }

    public string Name { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public DateTime DateOfBirth { get; set; }
    // -> this id of the dept the student belongs to and the 'Id' in the Department class will be created as a foreign key to shwo the relationship
    public int? DeptId { get; set; }
    public virtual Department? Dept { get; set; }
}