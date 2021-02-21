using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.VSDTO
{
    public class VSCommitsOfPullRequest
    {
        public int count { get; set; }
        public Value[] value { get; set; }
    }

    public class Value
    {
        public string commitId { get; set; }
        public Author author { get; set; }
        public Committer committer { get; set; }
        public string comment { get; set; }
        public string url { get; set; }
    }
}
