using AdSetIntegrador.Data.Entities;
using AdSetIntegrador.Web.Models;
using AdSetIntegrador.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
