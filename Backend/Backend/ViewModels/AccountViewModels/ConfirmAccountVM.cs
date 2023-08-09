using System.ComponentModel.DataAnnotations;

namespace Backend.ViewModels.AccountViewModels
{
    public class ConfirmAccountVM
    {
        public string Email { get; set; }
        [Required]
        public string OTP { get; set; }
    }
}
