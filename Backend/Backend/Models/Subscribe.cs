using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Subscribe : BaseEntity
    {
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
    }
}
