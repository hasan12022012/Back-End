using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        [StringLength(255)]
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        [StringLength(255)]
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        [StringLength(255)]
        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; } = DateTime.UtcNow;
    }
}
