using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AdSetIntegrador.Data.Entities
{
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [DisplayName("Marca")]
        public string Brand { get; set; }

        [Required]
        [MaxLength(100)]
        [DisplayName("Modelo")]
        public string Model { get; set; }

        [Range(2000, 2024)]
        [DisplayName("Ano")]
        public int Year { get; set; }

        [Required]
        [MaxLength(20)]
        [DisplayName("Placa")]
        public string Plate { get; set; }

        [Range(0, int.MaxValue)]
        [DisplayName("Km")]
        public int? Mileage { get; set; }

        [Required]
        [MaxLength(50)]
        [DisplayName("Cor")]
        public string Color { get; set; }

        [Required]
        [Range(10000, double.MaxValue)]
        [DisplayName("Preço")]
        public decimal Price { get; set; }

        public ICollection<VehicleOptionalFeature> VehicleOptionalFeatures { get; set; } = new List<VehicleOptionalFeature>();
        public ICollection<VehicleImage> Images { get; set; } = new List<VehicleImage>();
    }
}
