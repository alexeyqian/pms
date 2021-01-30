using System;

namespace PMS.Models
{
    public class Bug
    {
        public int NO{get;set;}
        public string Status{get;set;}
        public string StatusInVS{get;set;}
        public DateTime? FixedDate{get;set;}
        public DateTime? StartedDate{get;set;}
        public DateTime? ApprovedDate{get;set;}
        public DateTime? ResovedDate{get;set;}
        public decimal EstimatedHours{get;set;}
        public decimal ActualHours{get;set;}
        public int RejectedTimes{get;set;}
        public string Developer{get;set;}
        public string Team{get;set;}
        public string Note{get;set;}
    }
}
