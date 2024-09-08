using AdSetIntegrador.Data.Context;
using AdSetIntegrador.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdSetIntegrador.Web.Services
{
    public class VehicleService
    {
        private AppDbContext _context;
        private readonly string _photosPath;

        public VehicleService(AppDbContext context)
        {
            _context = context;
            _photosPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "photos");
        }

        public List<OptionalFeature> GetOptionalFeatures()
        {
            return _context.OptionalFeatures.ToList();
        }

        public List<Vehicle> GetVehicles()
        {
            return _context.Vehicles.Include(v => v.Images).Include(v => v.VehicleOptionalFeatures).ToList();
        }

        public void AddVehicle(Vehicle vehicle, int[] selectedOptionalFeatures, ICollection<IFormFile> images)
        {
            _context.Vehicles.Add(vehicle);
            _context.SaveChanges();

            if (selectedOptionalFeatures != null)
            {
                foreach (var optionalFeatureId in selectedOptionalFeatures)
                {
                    _context.VehicleOptionalFeatures.Add(new VehicleOptionalFeature
                    {
                        VehicleId = vehicle.Id,
                        OptionalFeatureId = optionalFeatureId
                    });
                }
            }

            if (images != null && images.Any())
            {
                foreach (var image in images)
                {
                    if (image.Length > 0)
                    {
                        var fileName = $"{vehicle.Id}_{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                        var filePath = Path.Combine(_photosPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            image.CopyTo(stream);
                        }

                        _context.VehicleImages.Add(new VehicleImage
                        {
                            VehicleId = vehicle.Id,
                            ImageName = fileName
                        });
                    }
                }
            }

            _context.SaveChanges();
        }
    }
}
