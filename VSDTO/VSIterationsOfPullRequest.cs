using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.VSDTO
{
    public class VSIterationsOfPullRequest
    {
        public VSPRIterationValue[] value { get; set; }
        public int count { get; set; }
    }

    public class VSPRIterationValue
    {
        public int id { get; set; }
        public string description { get; set; }
        public Author author { get; set; }
        public DateTime createdDate { get; set; }  
    }

    public class Author
    {
        public string id { get; set; }
        public string uniqueName { get; set; }
    }
}
