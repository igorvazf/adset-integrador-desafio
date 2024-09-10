using AdSetIntegrador.Data.Entities;
using AdSetIntegrador.Web.Models;
using AdSetIntegrador.Web.Services;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Diagnostics;

namespace AdSetIntegrador.Web.Controllers
{
    public class VehicleController : Controller
    {
        private readonly VehicleService _vehicleService;
        
        public VehicleController(VehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        public IActionResult Index()
        {
            var model = _vehicleService.GetVehicles();
            ViewBag.OptionalFeatures = _vehicleService.GetOptionalFeatures();
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Action = "Create";
            ViewBag.OptionalFeatures = _vehicleService.GetOptionalFeatures();
            return View("Edit", new Vehicle());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Vehicle vehicle, int[] selectedOptionalFeatures, ICollection<IFormFile> images)
        {
            if (ModelState.IsValid)
            {
                _vehicleService.AddVehicle(vehicle, selectedOptionalFeatures, images);
                return RedirectToAction("Index");
            }
            ViewBag.Action = "Create";
            ViewBag.OptionalFeatures = _vehicleService.GetOptionalFeatures();
            return View("Edit", vehicle);
        }

        public IActionResult Edit(int id)
        {
            var vehicle = _vehicleService.GetVehicleById(id);
            if (vehicle == null)
                return NotFound();

            ViewBag.Action = "Edit";
            ViewBag.OptionalFeatures = _vehicleService.GetOptionalFeatures();
            return View("Edit", vehicle);
        }

        [HttpPost]
        public IActionResult Edit(Vehicle vehicle, int[] selectedOptionalFeatures, ICollection<IFormFile> images)
        {
            if (ModelState.IsValid)
            {
                _vehicleService.EditVehicle(vehicle, selectedOptionalFeatures, images);
                return RedirectToAction("Index");
            }
            ViewBag.Action = "Edit";
            ViewBag.OptionalFeatures = _vehicleService.GetOptionalFeatures();
            return View("Edit", vehicle);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _vehicleService.DeleteVehicle(id);
            ViewBag.OptionalFeatures = _vehicleService.GetOptionalFeatures();
            return RedirectToAction("Index");
        }

        public IActionResult ExportToExcel()
        {
            var vehicles = _vehicleService.GetVehicles();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Veiculos");
                worksheet.Cells[1, 1].Value = "Marca";
                worksheet.Cells[1, 2].Value = "Modelo";
                worksheet.Cells[1, 3].Value = "Ano";
                worksheet.Cells[1, 4].Value = "Placa";
                worksheet.Cells[1, 5].Value = "Km";
                worksheet.Cells[1, 6].Value = "Cor";
                worksheet.Cells[1, 7].Value = "Preço";
                worksheet.Cells[1, 8].Value = "Imagens";
                worksheet.Cells[1, 9].Value = "Opcionais";

                int row = 2;
                foreach (var vehicle in vehicles)
                {
                    worksheet.Cells[row, 1].Value = vehicle.Brand;
                    worksheet.Cells[row, 2].Value = vehicle.Model;
                    worksheet.Cells[row, 3].Value = vehicle.Year;
                    worksheet.Cells[row, 4].Value = vehicle.Plate;
                    worksheet.Cells[row, 5].Value = vehicle.Mileage;
                    worksheet.Cells[row, 6].Value = vehicle.Color;
                    worksheet.Cells[row, 7].Value = vehicle.Price;
                    worksheet.Cells[row, 8].Value = vehicle.Images.Count();
                    worksheet.Cells[row, 9].Value = vehicle.VehicleOptionalFeatures.Count();
                    row++;
                }

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Veiculos.xlsx");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
