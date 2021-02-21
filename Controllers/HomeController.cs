﻿using System;
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

        private int _beginYear = 2020;
        private int _endYear = 2021;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

            // get data
            string fileName = "data.json";
            string jsonString = System.IO.File.ReadAllText(fileName);
            _bugs = JsonConvert.DeserializeObject<List<Bug>>(jsonString, 
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore})
                .Where(b => b.Status != "WorkedAndContinue").ToList();
            foreach (var b in _bugs){
                if (b.Status.StartsWith("Fixed"))
                    b.Status = "Fixed";
            }
        }

        public IActionResult Index(DateTime? startdate, DateTime? enddate)
        {
            if (!startdate.HasValue) startdate = new DateTime(2020,1,1);
            if (!enddate.HasValue) enddate = new DateTime(2030, 1, 1);

            var bugs = _bugs.Where(b => b.FixedDate >= startdate && b.FixedDate <= enddate.Value.AddDays(1)).ToList();

            SetCategoryChart(bugs);
            SetProductivityChart(bugs);
            SetQualityChart(bugs);
            SetEfficiencyChart(bugs);
            return View();
        }

        public IActionResult ByDeveloper()
        {
            SetByDeveloperChart(_bugs);
            return View();
        }

        public IActionResult WeeklyReport(DateTime? startdate, DateTime? enddate)
        {
            if (!startdate.HasValue) startdate = new DateTime(2020, 1, 1);
            if (!enddate.HasValue) enddate = new DateTime(2030, 1, 1);

            var bugs = _bugs.Where(b => b.FixedDate >= startdate && b.FixedDate <= enddate.Value.AddDays(1)).ToList();
            SetCategoryChart(bugs);
            return View(bugs);
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

            for(int i = _beginYear; i <= _endYear; i++)
            {
                var groups = from b in bugs
                                 where b.FixedDate.HasValue && b.FixedDate.Value.Year == i
                                 group b by weekProjector(b.FixedDate.Value);

                foreach (var grp in groups)
                    dict.Add(i.ToString() +  "W" + grp.Key.ToString("00"), grp.Count());
            }
           
            var sortedDict = new Dictionary<string, int>();
            foreach (var item in dict.OrderBy(i => i.Key))
            {
                sortedDict.Add(item.Key, item.Value);
            }
            
            ViewData["ProductivityLabels"] = "\"" + String.Join("\",\"", sortedDict.Keys.ToArray()) + "\"";
            ViewData["ProductivityValues"] = String.Join(",", sortedDict.Values.ToArray());
        }

        private void SetQualityChart(List<Bug> bugs)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            for(int i = _beginYear; i <= _endYear; i++)
            {                
                var groups = from b in bugs
                                 where b.FixedDate.HasValue && b.FixedDate.Value.Year == i && b.RejectedTimes > 0
                                 group b by weekProjector(b.FixedDate.Value);

                foreach (var grp in groups)
                    dict.Add(i.ToString() + "W" + grp.Key.ToString("00"), grp.Count());
            }

            var sortedDict = new Dictionary<string, int>();
            foreach (var item in dict.OrderBy(i => i.Key))
            {
                sortedDict.Add(item.Key, item.Value);
            }

            ViewData["QualityLabels"] = "\"" + String.Join("\",\"", sortedDict.Keys.ToArray()) + "\"";
            ViewData["QualityValues"] = String.Join(",", sortedDict.Values.ToArray());
        }

        private void SetEfficiencyChart(List<Bug> bugs)
        {
            Dictionary<string, decimal> dict = new Dictionary<string, decimal>();
            for (int i = _beginYear; i <= _endYear; i++)
            {
                var groups = from b in bugs
                             where b.FixedDate.HasValue && b.FixedDate.Value.Year == i && b.ActualHours > 0
                             group b by weekProjector(b.FixedDate.Value);

                foreach (var grp in groups)
                    dict.Add(i.ToString() + "W" + grp.Key.ToString("00"), grp.Sum(s => s.ActualHours) / grp.Count());
            }

            var sortedDict = new Dictionary<string, decimal>();
            foreach (var item in dict.OrderBy(i => i.Key))
            {
                sortedDict.Add(item.Key, item.Value);
            }

            ViewData["EfficiencyLabels"] = "\"" + String.Join("\",\"", sortedDict.Keys.ToArray()) + "\"";
            ViewData["EfficiencyValues"] = String.Join(",", sortedDict.Values.ToArray());
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
        public IActionResult Data()
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
