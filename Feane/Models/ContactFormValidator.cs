//namespace Feane.Models
//{
//    public class ContactFormValidator
//    {
//    }
//}
using Feane.Models;
using FluentValidation;

namespace Feane.Models
{
    public class ContactFormValidator : AbstractValidator<ContactFrom>
    {
        public ContactFormValidator()
        {
            RuleFor(x => x.name)
                .NotEmpty()
                .WithMessage("Поле не должно быть пустым");


        }
    }
}

