using System.Diagnostics;
using ClinicAppointment.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointment.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var viewModel = new HomeViewModel
        {
            CurrentWaitTime = 25, // This would typically come from a service
            AvailableAppointmentTypes = new List<AppointmentType>
            {
                new AppointmentType 
                { 
                    Type = "general-checkup",
                    Name = "General Checkup",
                    Description = "Regular health checkup and consultation"
                },
                new AppointmentType 
                { 
                    Type = "vaccination",
                    Name = "Vaccinations",
                    Description = "Various vaccination services"
                }
            }
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
