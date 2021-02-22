using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.VSDTO
{
    public class VSPullRequest
    {
        public Repository repository { get; set; }
        public int pullRequestId { get; set; }       
        public string status { get; set; }
        public Createdby createdBy { get; set; }
        public DateTime creationDate { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string mergeStatus { get; set; }
        public bool isDraft { get; set; }
        public string url { get; set; }
        public bool supportsIterations { get; set; }
        public string artifactId { get; set; }
    }

    public class Repository
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }       
    }

}
