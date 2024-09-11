namespace AdSetIntegrador.Web.Models.Filters
{
    public class VehicleFilter
    {
        public string? Plate { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public int? YearMin { get; set; }
        public int? YearMax { get; set; }
        public int? PriceRange { get; set; }
        public int? Photos { get; set; }
        public int? Optional { get; set; }
        public string? Color { get; set; }
    }
}
