using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PMS.Models;

using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace PMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly List<Bug> _bugs;
                
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

            string fileName = "data.json";
            string jsonString = System.IO.File.ReadAllText(fileName);
            //var weatherForecast = JsonSerializer.Deserialize<Bug>(jsonString);
            _bugs = JsonConvert.DeserializeObject<List<Bug>>(jsonString);
        }

        public IActionResult Index()
        {
            SetOverviewChart(_bugs);      
            return View();
        }

        public IActionResult ByDeveloper()
        {
            SetByDeveloperChart(_bugs);
            return View();
        }

        private void SetOverviewChart(List<Bug> bugs)
        {
            var labelList = new List<string>();
            var valueList = new List<int>();

            var groups = bugs.GroupBy(b => b.Status);

            foreach (var grp in groups)
            {
                if (String.IsNullOrEmpty(grp.Key)) continue;

                labelList.Add(grp.Key);
                valueList.Add(grp.Count());
            }

            ViewData["OverViewTotal"] = bugs.Count();
            ViewData["OverViewLabels"] = "\"" + String.Join("\",\"", labelList.ToArray()) + "\"";
            ViewData["OverViewValues"] = String.Join(",", valueList.ToArray());
        }

        private void SetByDeveloperChart(List<Bug> bugs)
        {
            var labelList = new List<string>();
            var valueList = new List<int>();

            var groups = bugs.GroupBy(b => b.Developer);

            foreach (var grp in groups)
            {
                if (String.IsNullOrEmpty(grp.Key)) continue;

                labelList.Add(grp.Key);
                valueList.Add(grp.Count());
            }

            ViewData["ByDeveloperTotal"] = bugs.Count();
            ViewData["ByDeveloperLabels"] = "\"" + String.Join("\",\"", labelList.ToArray()) + "\"";
            ViewData["ByDeveloperValues"] = String.Join(",", valueList.ToArray());
        }

        public IActionResult Detail()
        {
            return View(_bugs);
        }

        public IActionResult Wiki()
        {
            return View();
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
}
