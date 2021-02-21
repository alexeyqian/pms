using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PMS.Models
{
    public class Bug
    {
        public int Id { get; set; }
        public int NO{get;set;}
        public string Status{get;set;}
        public string StatusInVS{get;set;}
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FixedDate{get;set;}
        public DateTime? StartedDate{get;set;}
        public DateTime? ApprovedDate{get;set;}        
        public DateTime? ResovedDate{get;set;}
        [DefaultValue(0)]
        public decimal EstimatedHours{get;set;}
        [DefaultValue(0)]
        public decimal ActualHours{get;set;}
        [DefaultValue(0)]
        public int RejectedTimes{get;set;}
        public string Developer{get;set;}
        public string Team{get;set;}
        public string Note{get;set;}        
    }
}
