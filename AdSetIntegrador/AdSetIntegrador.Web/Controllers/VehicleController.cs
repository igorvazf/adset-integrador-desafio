using AdSetIntegrador.Data.Entities;
using AdSetIntegrador.Web.Models;
using AdSetIntegrador.Web.Services;
using Microsoft.AspNetCore.Mvc;
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
            var vehicle = new Vehicle();
            if (vehicle == null)
            {
                return NotFound();
            }
            ViewBag.Action = "Edit";
            return View("Edit", vehicle);
        }

        [HttpPost]
        public IActionResult Edit(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Action = "Edit";
            return View("Edit", vehicle);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
