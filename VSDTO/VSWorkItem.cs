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
        public int SystemId { get; set; }
        public int SystemAreaId { get; set; }
        public string SystemAreaPath { get; set; }
        public string SystemTeamProject { get; set; }
        public string SystemNodeName { get; set; }
        public string SystemAreaLevel1 { get; set; }
        public string SystemAreaLevel2 { get; set; }
        public int SystemRev { get; set; }
        public DateTime SystemAuthorizedDate { get; set; }
        public DateTime SystemRevisedDate { get; set; }
        public int SystemIterationId { get; set; }
        public string SystemIterationPath { get; set; }
        public string SystemIterationLevel1 { get; set; }
        public string SystemWorkItemType { get; set; }
        public string SystemState { get; set; }
        public string SystemReason { get; set; }
        public SystemAssignedto SystemAssignedTo { get; set; }
        public DateTime SystemCreatedDate { get; set; }
        public SystemCreatedby SystemCreatedBy { get; set; }
        public DateTime SystemChangedDate { get; set; }
        public SystemChangedby SystemChangedBy { get; set; }
        public SystemAuthorizedas SystemAuthorizedAs { get; set; }
        public int SystemPersonId { get; set; }
        public int SystemWatermark { get; set; }
        public int SystemCommentCount { get; set; }
        public string SystemTitle { get; set; }
        public DateTime MicrosoftVSTSCommonStateChangeDate { get; set; }
        public DateTime MicrosoftVSTSCommonActivatedDate { get; set; }
        public MicrosoftVSTSCommonActivatedby MicrosoftVSTSCommonActivatedBy { get; set; }
        public int MicrosoftVSTSCommonPriority { get; set; }
        public string MicrosoftVSTSCommonSeverity { get; set; }
        public string MicrosoftVSTSCommonValueArea { get; set; }
        public string OfficeProductStudioPSDatabase { get; set; }
        public string MicrosoftVSTSCommonTriage { get; set; }
        public bool O365SecurityImpact { get; set; }
        public bool O365IsException { get; set; }
        public string SystemDescription { get; set; }
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