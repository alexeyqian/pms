using Newtonsoft.Json;
using System;

// Auto generated from json file.
namespace PMS.VSDTO
{
    public class VSWorkItem
    {
        public int id { get; set; }
        public int rev { get; set; }
        public Fields fields { get; set; }
        public Relation[] relations { get; set; }
        public _LinksSelf _links { get; set; }
        public string url { get; set; }
    }

    public class Fields
    {               
        [JsonProperty(PropertyName = "System.WorkItemType")]
        public string SystemWorkItemType { get; set; }
        [JsonProperty(PropertyName = "System.State")]
        public string SystemState { get; set; }
        [JsonProperty(PropertyName = "System.Reason")]
        public string SystemReason { get; set; }
        [JsonProperty(PropertyName = "System.AssignedTo")]
        public SystemAssignedto SystemAssignedTo { get; set; }
        [JsonProperty(PropertyName = "System.CreatedDate")]
        public DateTime SystemCreatedDate { get; set; }
        [JsonProperty(PropertyName = "System.CommentCount")]
        public int SystemCommentCount { get; set; }
        [JsonProperty(PropertyName = "System.Title")]
        public string SystemTitle { get; set; }
        [JsonProperty(PropertyName = "System.Description")]
        public string SystemDescription { get; set; }
        [JsonProperty(PropertyName = "System.Tags")]
        public string SystemTags { get; set; }
    }

    public class SystemAssignedto
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class SystemCreatedby
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class SystemChangedby
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }


    public class SystemAuthorizedas
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

       public class MicrosoftVSTSCommonActivatedby
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class _LinksSelf
    {
        public Href self { get; set; }
        public Workitemupdates workItemUpdates { get; set; }
        public Workitemrevisions workItemRevisions { get; set; }
        public Workitemcomments workItemComments { get; set; }
        public Html html { get; set; }
        public Workitemtype workItemType { get; set; }
        public Fields1 fields { get; set; }
    }


    public class Workitemupdates
    {
        public string href { get; set; }
    }

    public class Workitemrevisions
    {
        public string href { get; set; }
    }

    public class Workitemcomments
    {
        public string href { get; set; }
    }

    public class Html
    {
        public string href { get; set; }
    }

    public class Workitemtype
    {
        public string href { get; set; }
    }

    public class Fields1
    {
        public string href { get; set; }
    }

    public class Relation
    {
        public string rel { get; set; }
        public string url { get; set; }
        public Attributes attributes { get; set; }
    }

    public class Attributes
    {
        public bool isLocked { get; set; }
        public string name { get; set; }
        public DateTime authorizedDate { get; set; }
        public int id { get; set; }
        public DateTime resourceCreatedDate { get; set; }
        public DateTime resourceModifiedDate { get; set; }
        public DateTime revisedDate { get; set; }
        public int resourceSize { get; set; }
    }
}