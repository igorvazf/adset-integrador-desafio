using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdSetIntegrador.Data.Entities
{
    public class VehicleOptionalFeature
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [ForeignKey("VehicleId")]
        public Vehicle Vehicle { get; set; }

        [Required]
        public int OptionalFeatureId { get; set; }

        [ForeignKey("OptionalFeatureId")]
        public OptionalFeature OptionalFeature { get; set; }
    }
}
