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
        public string Title { get; set; }
        public string Tags { get; set; }
        public string Developer { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]        
        public DateTime CreatedDate { get; internal set; }
        public DateTime? StartedDate{get;set;}
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FixedDate { get; set; }
        public DateTime? ApprovedDate{get;set;}
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ResovedDate{get;set;}
        [DefaultValue(0)]
        public decimal EstimatedHours{get;set;}
        [DefaultValue(0)]
        public decimal ActualHours{get;set;}
        [DefaultValue(0)]
        public int RejectedTimes{get;set;}    
        public int CommentCount { get; internal set; }

        public int PullRequestCount { get; internal set; }
        public DateTime? FirstPullRequestDate { get; internal set; }
        public string FirstPullRequestStatus { get; internal set; }
        public int FirstPullRequestCommentCount { get; internal set; }
        public int FirstPullRequestCommitCount { get; internal set; }

        public string Team { get; set; }
        public string Note { get; set; }
    }
}
