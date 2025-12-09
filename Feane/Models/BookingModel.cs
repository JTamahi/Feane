using System;
using System.ComponentModel.DataAnnotations;

namespace Feane.Models
{
    public class BookingModel
    {
        [Required(ErrorMessage = "Введите имя")]
        [StringLength(100, ErrorMessage = "Имя не должно превышать 100 символов")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите телефон")]
        [Phone(ErrorMessage = "Неверный формат телефона")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите количество персон")]
        [Range(1, 20, ErrorMessage = "Количество персон от 1 до 20")]
        public int? Persons { get; set; }

        [Required(ErrorMessage = "Укажите дату")]
        [DataType(DataType.Date)]
        [FutureDate(ErrorMessage = "Дата должна быть в будущем")]
        public DateTime? Date { get; set; }
    }
}