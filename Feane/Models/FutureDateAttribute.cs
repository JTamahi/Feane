using System;
using System.ComponentModel.DataAnnotations;

namespace Feane.Models
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Если значение пустое — пусть [Required] обрабатывает отсутствие даты
            if (value is null)
                return ValidationResult.Success;

            if (value is DateTime date)
            {
                if (date.Date <= DateTime.Today)
                    return new ValidationResult(ErrorMessage ?? "Дата должна быть в будущем");

                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage ?? "Неверный формат даты");
        }
    }
}