using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebAPI2.Data;
using WebAPI2.Validators;

namespace WebAPI2.Models;

public class StudentDTO
{
    public int Id
    {
        get;
        set;
    }

    [Required(ErrorMessage = "student name is required"), StringLength(40, MinimumLength = 3, ErrorMessage = "'Name' must be between 2 and 40 characters")]
    public string Name { get; set; }
    [EmailAddress(ErrorMessage = "please enter a valid email address"), Required(ErrorMessage = "email address is required")]
    public string Email { get; set; }
    [Required(ErrorMessage = "address is required")]
    public string Address { get; set; }
    [DateCheck]
    public DateTime DOB { get; set; }
    
}