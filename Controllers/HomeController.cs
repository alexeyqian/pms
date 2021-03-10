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
using Microsoft.Extensions.Configuration;
using PMS.Data;

namespace PMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly PMSDBContext _context;

        private readonly List<Bug> _bugs;
        Func<DateTime, int> weekProjector =
                d => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                    d, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);

        private int _beginYear = 2020;
        private int _endYear = 2021;

        public HomeController(ILogger<HomeController> logger, IConfiguration configration, PMSDBContext context)
        {            
            _logger = logger;
            _configuration = configration;
            _context = context;
                    
            _bugs = _context.Bug.ToList();

            // clean fixed types
            
            string status = "Active";
            foreach (var b in _bugs){
                if (!string.IsNullOrEmpty(b.Status))
                {
                    string s = b.Status.ToLower();
                    if (s == "fixedwithoutcodechange")
                        status = "FixedWithoutCodeChange";
                    else if (s == "fixedandapproved" || s.Contains("waitingforapproval"))
                        status = "FixedWithCodeChange";
                    else if (b.Status.ToLower().Contains("continue"))
                        status = "Active";
                    else
                        status = b.Status;
                }

                b.Status = status;
            }
        }

        public IActionResult Index(DateTime? startdate, DateTime? enddate)
        {   
            if (!startdate.HasValue) startdate = new DateTime(2020, 1, 1);
            if (!enddate.HasValue) enddate = DateTime.Now.AddDays(1);

            var bugs = _bugs.Where(b => !b.FixedDate.HasValue 
                || ( b.FixedDate >= startdate && b.FixedDate <= enddate.Value.AddDays(1))).ToList();

            SetCategoryChart(bugs);
            SetFixedTypeChart(bugs);
            SetProductivityChart(bugs);
            SetQualityChart(bugs);
            SetEfficiencyChart(bugs);
            SetBugCommentsChart(bugs);
            SetPRCommentsChart(bugs);
            SetPREngagementChart(bugs);
            SetTagChart(bugs);
            SetSeverityChart(bugs);
            SetPriorityChart(bugs);

            ViewBag.StartDate = startdate.HasValue ? startdate.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.EndDate = enddate.HasValue ? enddate.Value.ToString("yyyy-MM-dd") : "";

            return View();
        }

        public IActionResult ByDeveloper()
        {
            SetByDeveloperChart(_bugs);
            return View();
        }

        public IActionResult WeeklyReport(DateTime? startdate, DateTime? enddate)
        {
            if (!startdate.HasValue) startdate = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            if (!enddate.HasValue) enddate = DateTime.Now.AddDays(1);

            var bugs = _bugs.Where(b => !string.IsNullOrEmpty(b.StatusInVS) 
                && b.FixedDate >= startdate && b.FixedDate <= enddate.Value.AddDays(1))
                .ToList();

            SetStatusChart(bugs);
            SetPriorityChart(bugs);

            ViewBag.StartDate = startdate.HasValue ? startdate.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.EndDate = enddate.HasValue? enddate.Value.ToString("yyyy-MM-dd") : "";

            return View(bugs);
        }

        private void SetStatusChart(List<Bug> bugs)
        {
            var analyzer = new DataAnalyzer();
            var chartData = analyzer.GetStatusInVSData(bugs);

            ViewData["StatusTotal"] = chartData.Total;
            ViewData["StatusLabels"] = chartData.CategoryLabels;
            ViewData["StatusValues"] = chartData.CategoryValues;
        }


        private void SetCategoryChart(List<Bug> bugs)
        {
            var labelList = new List<string>();
            var valueList = new List<int>();

            var groups = bugs.GroupBy(b => b.StatusInVS);

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

        private void SetFixedTypeChart(List<Bug> bugs)
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
            ViewData["FixedTypeLabels"] = "\"" + String.Join("\",\"", labelList.ToArray()) + "\"";
            ViewData["FixedTypeValues"] = String.Join(",", valueList.ToArray());
        }

        private void SetTagChart(List<Bug> bugs)
        {
            var analyzer = new DataAnalyzer();
            var chartData = analyzer.GetTagData(bugs);

            ViewData["TotalTags"] = chartData.Total;
            ViewData["TagLabels"] = chartData.CategoryLabels;
            ViewData["TagValues"] = chartData.CategoryValues;
        }

        private void SetSeverityChart(List<Bug> bugs)
        {
            var analyzer = new DataAnalyzer();
            var chartData = analyzer.GetSeverityData(bugs);

            ViewData["TotalSeverity"] = chartData.Total;
            ViewData["SeverityLabels"] = chartData.CategoryLabels;
            ViewData["SeverityValues"] = chartData.CategoryValues;
        }

        private void SetPriorityChart(List<Bug> bugs)
        {
            var analyzer = new DataAnalyzer();
            var chartData = analyzer.GetPriorityData(bugs);

            ViewData["TotalPriority"] = chartData.Total;
            ViewData["PriorityLabels"] = chartData.CategoryLabels;
            ViewData["PriorityValues"] = chartData.CategoryValues;
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

        private void SetQualityChart_NOTUSED(List<Bug> bugs)
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

        private void SetQualityChart(List<Bug> bugs)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            for (int i = _beginYear; i <= _endYear; i++)
            {
                var groups = from b in bugs
                             where b.FirstPullRequestDate.HasValue && b.FirstPullRequestDate.Value.Year == i && b.PullRequestCount > 0
                             group b by weekProjector(b.FirstPullRequestDate.Value);

                foreach (var grp in groups)
                    dict.Add(i.ToString() + "W" + grp.Key.ToString("00"), grp.Sum(c => c.FirstPullRequestIterationCount) / grp.Sum(s => s.PullRequestCount));
            }

            var sortedDict = new Dictionary<string, int>();
            foreach (var item in dict.OrderBy(i => i.Key))
            {
                sortedDict.Add(item.Key, item.Value);
            }

            ViewData["QualityLabels"] = "\"" + String.Join("\",\"", sortedDict.Keys.ToArray()) + "\"";
            ViewData["QualityValues"] = String.Join(",", sortedDict.Values.ToArray());
        }

        private void SetBugCommentsChart(List<Bug> bugs)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            for (int i = _beginYear; i <= _endYear; i++)
            {
                var groups = from b in bugs
                             where b.FixedDate.HasValue && b.FixedDate.Value.Year == i
                             group b by weekProjector(b.FixedDate.Value);

                foreach (var grp in groups)
                    dict.Add(i.ToString() + "W" + grp.Key.ToString("00"), grp.Sum(b => b.CommentCount) / grp.Count());
            }

            var sortedDict = new Dictionary<string, int>();
            foreach (var item in dict.OrderBy(i => i.Key))
            {
                sortedDict.Add(item.Key, item.Value);
            }

            ViewData["BugCommentsLabels"] = "\"" + String.Join("\",\"", sortedDict.Keys.ToArray()) + "\"";
            ViewData["BugCommentsValues"] = String.Join(",", sortedDict.Values.ToArray());
        }

        private void SetPRCommentsChart(List<Bug> bugs)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            for (int i = _beginYear; i <= _endYear; i++)
            {
                var groups = from b in bugs
                             where b.FirstPullRequestDate.HasValue && b.FirstPullRequestDate.Value.Year == i && b.PullRequestCount > 0
                             group b by weekProjector(b.FirstPullRequestDate.Value);

                foreach (var grp in groups)
                    dict.Add(i.ToString() + "W" + grp.Key.ToString("00"), grp.Sum(c => c.FirstPullRequestCommentCount) / grp.Sum(s => s.PullRequestCount));
            }

            var sortedDict = new Dictionary<string, int>();
            foreach (var item in dict.OrderBy(i => i.Key))
            {
                sortedDict.Add(item.Key, item.Value);
            }

            ViewData["PRCommentsLabels"] = "\"" + String.Join("\",\"", sortedDict.Keys.ToArray()) + "\"";
            ViewData["PRCommentsValues"] = String.Join(",", sortedDict.Values.ToArray());
        }

        private void SetPREngagementChart(List<Bug> bugs)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            for (int i = _beginYear; i <= _endYear; i++)
            {
                var groups = from b in bugs
                             where b.PullRequestCount > 0 
                                   && b.FirstPullRequestDate.HasValue && b.FirstPullRequestDate.Value.Year == i
                                   && b.FirstPullRequestCommentDate.HasValue && b.FirstPullRequestCommentDate >= b.FirstPullRequestDate
                             group b by weekProjector(b.FirstPullRequestDate.Value);

                foreach (var grp in groups)
                    dict.Add(i.ToString() + "W" + grp.Key.ToString("00"), grp.Sum(c => c.FirstPullRequestCommentDate.Value.Subtract(c.FirstPullRequestDate.Value).Hours) / grp.Count());
            }

            var sortedDict = new Dictionary<string, int>();
            foreach (var item in dict.OrderBy(i => i.Key))
            {
                sortedDict.Add(item.Key, item.Value);
            }

            ViewData["PREngagementLabels"] = "\"" + String.Join("\",\"", sortedDict.Keys.ToArray()) + "\"";
            ViewData["PREngagementValues"] = String.Join(",", sortedDict.Values.ToArray());
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

    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}
