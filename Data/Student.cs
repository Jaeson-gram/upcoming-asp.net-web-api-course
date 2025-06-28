using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
}