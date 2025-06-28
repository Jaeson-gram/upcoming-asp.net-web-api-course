using System.ComponentModel.DataAnnotations;

namespace WebAPI2.Validators;

public class DateCheckAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        // var date = (DateTime) value;
        // if ((DateTime) value < DateTime.Today)
        // {
        //     return new ValidationResult("We only accept students admitted after yesterday");
        // }
        //
        // return ValidationResult.Success;
        
        return (DateTime) value >= DateTime.Today ? new ValidationResult("We only accept students born before today") : ValidationResult.Success;
    }
}