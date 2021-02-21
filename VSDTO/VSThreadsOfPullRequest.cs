using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.VSDTO
{
    public class VSThreadsOfPullRequest
    {
        public VSTValue[] value { get; set; }
        public int count { get; set; }
    }

    public class VSTValue
    {
        public object pullRequestThreadContext { get; set; }
        public int id { get; set; }
        public DateTime publishedDate { get; set; }
        public DateTime lastUpdatedDate { get; set; }
        public Comment[] comments { get; set; }
        public object threadContext { get; set; }
        public Properties properties { get; set; }
        public Identities identities { get; set; }
        public bool isDeleted { get; set; }      
        public string status { get; set; }
    }

    public class Properties
    {
        public Codereviewthreadtype CodeReviewThreadType { get; set; }
        public Codereviewpolicytype CodeReviewPolicyType { get; set; }
        public Codereviewrequiredreviewerexamplepaththattriggered CodeReviewRequiredReviewerExamplePathThatTriggered { get; set; }
        public Codereviewrequiredreviewerisrequired CodeReviewRequiredReviewerIsRequired { get; set; }
        public Codereviewrequiredreviewernumfilesthattriggered CodeReviewRequiredReviewerNumFilesThatTriggered { get; set; }
        public Codereviewrequiredreviewernumreviewers CodeReviewRequiredReviewerNumReviewers { get; set; }
        public Codereviewrequiredreviewerexamplerevieweridentities CodeReviewRequiredReviewerExampleReviewerIdentities { get; set; }
    }

    public class Codereviewthreadtype
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Codereviewpolicytype
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Codereviewrequiredreviewerexamplepaththattriggered
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Codereviewrequiredreviewerisrequired
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Codereviewrequiredreviewernumfilesthattriggered
    {
        public string type { get; set; }
        public int value { get; set; }
    }

    public class Codereviewrequiredreviewernumreviewers
    {
        public string type { get; set; }
        public int value { get; set; }
    }

    public class Codereviewrequiredreviewerexamplerevieweridentities
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Identities
    {
        public _1 _1 { get; set; }
    }

    public class _1
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public bool isContainer { get; set; }
        public string descriptor { get; set; }
    }

    public class Comment
    {
        public int id { get; set; }
        public int parentCommentId { get; set; }
        public AuthorVST author { get; set; }
        public string content { get; set; }
        public DateTime publishedDate { get; set; }
        public DateTime lastUpdatedDate { get; set; }
        public DateTime lastContentUpdatedDate { get; set; }
        public string commentType { get; set; }
        public object[] usersLiked { get; set; }      
    }

    public class AuthorVST
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }
   
}
