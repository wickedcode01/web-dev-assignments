using System.Diagnostics;
using ClinicAppointment.Models;
using Microsoft.AspNetCore.Mvc;
using ClinicAppointment.Services;

namespace ClinicAppointment.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IWaitTimeService _waitTimeService;

    public HomeController(ILogger<HomeController> logger, IWaitTimeService waitTimeService)
    {
        _logger = logger;
        _waitTimeService = waitTimeService;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new HomeViewModel
        {
            CurrentWaitTime = await _waitTimeService.GetCurrentWaitTimeAsync(),
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
