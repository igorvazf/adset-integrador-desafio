using AdSetIntegrador.Data.Context;
using AdSetIntegrador.Data.Entities;
using AdSetIntegrador.Web.Models.Enums;
using AdSetIntegrador.Web.Models.Filters;
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

        public List<OptionalFeature> GetOptionalFeatures() =>
            _context.OptionalFeatures.ToList();

        public List<Vehicle> GetVehiclesList() =>
            GetVehicles().ToList();

        public Vehicle? GetVehicleById(int id) =>
            GetVehicles().FirstOrDefault(v => v.Id == id);

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

        public IEnumerable<Vehicle> GetFilteredVehicles(VehicleFilter filter)
        {
            var query = GetVehicles();

            if (!string.IsNullOrEmpty(filter.Plate))
                query = query.Where(q => q.Plate.Contains(filter.Plate));

            if (!string.IsNullOrEmpty(filter.Brand))
                query = query.Where(q => q.Brand.Contains(filter.Brand));

            if (!string.IsNullOrEmpty(filter.Model))
                query = query.Where(q => q.Model.Contains(filter.Model));

            if (filter.YearMin.HasValue)
                query = query.Where(q => q.Year >= filter.YearMax.Value);

            if (filter.YearMax.HasValue)
                query = query.Where(q => q.Year <= filter.YearMax.Value);

            if (filter.PriceRange.HasValue)
            {
                query = query.Where(q =>
                            filter.PriceRange == (int)PriceRange.From10To50K ? q.Price >= 10000 && q.Price <= 50000 :
                            filter.PriceRange == (int)PriceRange.From50To90K ? q.Price >= 50000 && q.Price <= 90000 :
                            q.Price >= 90000); //PriceRange.Above90K
            }

            if (filter.Photos.HasValue)
            {
                query = query.Where(q =>
                            filter.Photos == (int)PhotoFilter.WithPhotos ? q.Images.Count() > 0 :
                            q.Images.Count() == 0); //PhotoFilter.WithoutPhotos
            }

            if (filter.Optional.HasValue)
                query = query.Where(q => q.VehicleOptionalFeatures.Select(of => of.Id).Contains(filter.Optional.Value));

            if (!string.IsNullOrEmpty(filter.Color))
                query = query.Where(q => q.Color == filter.Color);

            return query.ToList();
        }

        #region Private Methods
        private IQueryable<Vehicle> GetVehicles()
        {
            return _context.Vehicles.Include(v => v.Images).Include(v => v.VehicleOptionalFeatures);
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
        #endregion
    }
}
