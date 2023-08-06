using System.ComponentModel.DataAnnotations;

namespace Backend.ViewModels
{
    public class ChangePasswordVM
    {
        [Required, DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required,
            DataType(DataType.Password), Compare(nameof(NewPassword))]
        public string NewConfirmPassword { get; set; }
    }
}
