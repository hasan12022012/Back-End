using System.ComponentModel.DataAnnotations;

namespace Backend.ViewModels.AccountViewModels
{
    public class ForgetPasswordVM
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
