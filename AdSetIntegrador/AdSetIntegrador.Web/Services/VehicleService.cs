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

        public Vehicle? GetVehicleById(int id)
        {
            return _context.Vehicles.Include(v => v.Images).Include(v => v.VehicleOptionalFeatures).FirstOrDefault(v => v.Id == id);
        }

        public void AddVehicle(Vehicle vehicle, int[] selectedOptionalFeatures, ICollection<IFormFile> images)
        {
            _context.Vehicles.Add(vehicle);
            _context.SaveChanges();

            UpdateVehicleOptionalFeatures(vehicle, selectedOptionalFeatures);
            SaveVehicleImages(vehicle.Id, images);

            _context.SaveChanges();
        }

        public void EditVehicle(Vehicle vehicle, int[] selectedOptionalFeatures, ICollection<IFormFile> images)
        {
            var vehicleDb = GetVehicleById(vehicle.Id);

            if (vehicleDb == null)
                throw new Exception("Vehicle not found.");

            UpdateVehicleInfo(vehicleDb, vehicle);
            UpdateVehicleOptionalFeatures(vehicleDb, selectedOptionalFeatures);
            SaveVehicleImages(vehicleDb.Id, images);

            _context.SaveChanges();
        }

        public void DeleteVehicle(int id)
        {
            var vehicle = GetVehicleById(id);

            if (vehicle == null)
                return;

            DeleteVehicleImages(vehicle);
            _context.Vehicles.Remove(vehicle);
            _context.SaveChanges();
        }

        private void UpdateVehicleInfo(Vehicle vehicleDb, Vehicle updatedVehicle)
        {
            vehicleDb.Brand = updatedVehicle.Brand;
            vehicleDb.Model = updatedVehicle.Model;
            vehicleDb.Year = updatedVehicle.Year;
            vehicleDb.Plate = updatedVehicle.Plate;
            vehicleDb.Mileage = updatedVehicle.Mileage;
            vehicleDb.Color = updatedVehicle.Color;
            vehicleDb.Price = updatedVehicle.Price;
        }

        private void UpdateVehicleOptionalFeatures(Vehicle vehicleDb, int[] selectedOptionalFeatures)
        {
            var currentFeatures = vehicleDb.VehicleOptionalFeatures.Select(vf => vf.OptionalFeatureId).ToList();
            var featuresToRemove = currentFeatures.Except(selectedOptionalFeatures).ToList();
            var featuresToAdd = selectedOptionalFeatures.Except(currentFeatures).ToList();

            foreach (var featureId in featuresToRemove)
            {
                var featureToRemove = vehicleDb.VehicleOptionalFeatures.FirstOrDefault(vf => vf.OptionalFeatureId == featureId);
                if (featureToRemove != null)
                {
                    _context.VehicleOptionalFeatures.Remove(featureToRemove);
                }
            }

            foreach (var featureId in featuresToAdd)
            {
                vehicleDb.VehicleOptionalFeatures.Add(new VehicleOptionalFeature
                {
                    VehicleId = vehicleDb.Id,
                    OptionalFeatureId = featureId
                });
            }
        }

        private void SaveVehicleImages(int vehicleId, ICollection<IFormFile> images)
        {
            if (images == null || !images.Any()) return;

            var existingImagesCount = _context.VehicleImages.Count(vi => vi.VehicleId == vehicleId);

            foreach (var image in images)
            {
                if (image.Length > 0)
                {
                    if (existingImagesCount >= 15)
                        break;

                    var fileName = $"{vehicleId}_{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                    var filePath = Path.Combine(_photosPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        image.CopyTo(stream);
                    }

                    _context.VehicleImages.Add(new VehicleImage
                    {
                        VehicleId = vehicleId,
                        ImageName = fileName
                    });

                    existingImagesCount++;
                }
            }
        }

        private void DeleteVehicleImages(Vehicle vehicle)
        {
            var vehicleImages = vehicle.Images.ToList();

            foreach (var image in vehicleImages)
            {
                var filePath = Path.Combine(_photosPath, image.ImageName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            _context.VehicleImages.RemoveRange(vehicleImages);
        }
    }
}
