
using System.ComponentModel.DataAnnotations;

namespace SmartGreenhouse.Domain.Entities
{
    public class DeviceStateSnapshot
    {
        public int Id { get; set; }

        [Required]
        public int DeviceId { get; set; }

        [Required]
        [MaxLength(100)]
        public string StateName { get; set; }

        public DateTime EnteredAt { get; set; }

        public string? Notes { get; set; }
    }
}
