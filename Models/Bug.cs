using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PMS.Models
{
    public class Bug
    {
        public int Id { get; set; }
        public int NO{get;set;}       
        public string StatusInVS{get;set;}
        public string Title { get; set; }
        public string Tags { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; internal set; }
        [DataType(DataType.Date)]
        public DateTime? ResovedDate{get;set;} 
        public string ResolvedReason { get; set; }
        [DefaultValue(0)]
        public decimal EstimatedHours{get;set;}
        [DefaultValue(0)]
        public decimal ActualHours{get;set;}        
        public int CommentCount { get; internal set; }
        public int PullRequestCount { get; internal set; }
        [DataType(DataType.Date)]
        public DateTime? FirstPullRequestDate { get; internal set; } // Our Resolved Date
        public string FirstPullRequestStatus { get; internal set; }
        public DateTime? FirstPullRequestCommentDate { get; set; }
        public int FirstPullRequestIterationCount { get; set; }
        public int FirstPullRequestCommentCount { get; internal set; }
        public int FirstPullRequestCommitCount { get; internal set; }

       
        // manual maintained fields
        public string Status { get; set; } // CSI Status
        public string Developer { get; set; }
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? StartedDate { get; set; }             
        public string Note { get; set; }

        // deprecated fields
        public string Team { get; set; }
        [DefaultValue(0)]
        public int RejectedTimes { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ApprovedDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? FixedDate { get; set; }
    }
}
