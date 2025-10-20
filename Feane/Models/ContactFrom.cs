//namespace Feane.Models
//{
//    public class ContactFrom
//    {
//    }
//}
using System.ComponentModel.DataAnnotations;

namespace Feane.Models
{
    public class ContactFrom
    {
        [Required]
        public string name { get; set; }
        [EmailAddress]
        public string email { get; set; }
        [StringLength(1000, ErrorMessage = "Текст сообщения слишком большой")]
        public string message { get; set; }
    }
}