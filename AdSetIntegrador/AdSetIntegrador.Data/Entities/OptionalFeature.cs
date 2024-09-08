using System.ComponentModel.DataAnnotations;

namespace AdSetIntegrador.Data.Entities
{
    public class OptionalFeature
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public ICollection<VehicleOptionalFeature> VehicleOptionalFeatures { get; set; } = new List<VehicleOptionalFeature>();
    }
}
