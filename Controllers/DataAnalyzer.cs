using Microsoft.AspNetCore.Mvc.Rendering;
using PMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.Controllers
{
    public class ChartData
    {
        public int Total { get; set; }
        public string CategoryLabels { get; set; }
        public string CategoryValues { get; set; }
    }
    public class DataAnalyzer
    {
        public ChartData GetTagData(List<Bug> bugs)
        {
            var tagDict = new Dictionary<string, int>();

            foreach (var bug in bugs)
            {
                if (string.IsNullOrEmpty(bug.Tags)) continue;
                var tagList = bug.Tags.Split(";").ToList();
                foreach (var tag in tagList)
                {
                    if (tagDict.Keys.Contains(tag))
                        tagDict[tag]++;
                    else
                    {
                        tagDict.Add(tag, 1);
                    }
                }
            }

            var sortedDict = tagDict.Where(kv => kv.Value >= 20).OrderByDescending(kv => kv.Value);
            return getChartDataFromDict(sortedDict);
        }

        public ChartData GetSeverityData(List<Bug> bugs)
        {            
            var dict = new Dictionary<string, int>();

            foreach (var bug in bugs)
            {
                if (string.IsNullOrEmpty(bug.Severity)) continue;

                var key = bug.Severity;
                if (dict.Keys.Contains(key))
                    dict[key]++;
                else                
                    dict.Add(key, 1);
            }

            var sortedDict = dict.OrderByDescending(kv => kv.Value);
            return getChartDataFromDict(sortedDict);
        }

        public ChartData GetPriorityData(List<Bug> bugs)
        {
            var dict = new Dictionary<string, int>();

            foreach (var bug in bugs)
            {
                if (string.IsNullOrEmpty(bug.Priority)) continue;

                var key = bug.Priority;
                if (dict.Keys.Contains(key))
                    dict[key]++;
                else
                    dict.Add(key, 1);
            }

            var sortedDict = dict.OrderByDescending(kv => kv.Value);
            return getChartDataFromDict(sortedDict);
        }

        private ChartData getChartDataFromDict(IOrderedEnumerable<KeyValuePair<string, int>> sortedDict)
        {
            if (sortedDict.Count() <= 0) throw new Exception("Dictionary cannot be empty.");

            var labelList = new List<string>();
            var valueList = new List<int>();
            var total = 0;
            
            foreach (var kv in sortedDict)
            {
                labelList.Add(kv.Key);
                valueList.Add(kv.Value);
                total += kv.Value;
            }

            var chartData = new ChartData();
            chartData.Total = total;
            chartData.CategoryLabels = "\"" + String.Join("\",\"", labelList.ToArray()) + "\"";
            chartData.CategoryValues = String.Join(",", valueList.ToArray()); ;

            return chartData;
        }
       
    }
}
