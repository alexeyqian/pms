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
using System.Globalization;
using System.Collections.Immutable;

namespace PMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly List<Bug> _bugs;
        Func<DateTime, int> weekProjector =
                d => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                    d, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

            // get data
            string fileName = "data.json";
            string jsonString = System.IO.File.ReadAllText(fileName);
            _bugs = JsonConvert.DeserializeObject<List<Bug>>(jsonString);
        }

        public IActionResult Index()
        {
            SetCategoryChart(_bugs);
            SetProductivityChart(_bugs);
            return View();
        }

        public IActionResult ByDeveloper()
        {
            SetByDeveloperChart(_bugs);
            return View();
        }

        private void SetCategoryChart(List<Bug> bugs)
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

            ViewData["TotalBugs"] = bugs.Count();
            ViewData["CategoryLabels"] = "\"" + String.Join("\",\"", labelList.ToArray()) + "\"";
            ViewData["CategoryValues"] = String.Join(",", valueList.ToArray());
        }

        private void SetProductivityChart(List<Bug> bugs)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();

            // TODO: replate hard code with complex algorithm
            // cal for 2020
            var groups2020 = from b in bugs
                             where b.FixedDate.HasValue && b.FixedDate.Value.Year == 2020
                             group b by weekProjector(b.FixedDate.Value);

            foreach (var grp in groups2020)
            {   
                dict.Add("2020W" + grp.Key.ToString("00"), grp.Count());                
            }

            // calc for 2021
            var groups2021 = from b in bugs
                             where b.FixedDate.HasValue && b.FixedDate.Value.Year == 2021
                             group b by weekProjector(b.FixedDate.Value);

            foreach (var grp in groups2021)
            {
                dict.Add("2021W" + grp.Key.ToString("00"), grp.Count());
            }

            var sortedDict = new Dictionary<string, int>();
            foreach (var item in dict.OrderBy(i => i.Key))
            {
                sortedDict.Add(item.Key, item.Value);
            }
            
            ViewData["ProductivityLabels"] = "\"" + String.Join("\",\"", sortedDict.Keys.ToArray()) + "\"";
            ViewData["ProductivityValues"] = String.Join(",", sortedDict.Values.ToArray());
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
