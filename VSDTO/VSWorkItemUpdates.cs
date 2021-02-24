using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.VSDTO
{  
    public class VSWorkItemUpdates
    {
        public int count { get; set; }
        public UpdateLog[] value { get; set; }
    }

    public class UpdateLog
    {
        public int id { get; set; }
        public int workItemId { get; set; }
        public int rev { get; set; }
        public DateTime revisedDate { get; set; }
        public UpdateFields fields { get; set; }

        public class UpdateFields
        {
            // focus on newVaue="Resolved"
            [JsonProperty(PropertyName = "System.State")]            
            public OldValueNewValue SystemState { get; set; }
           
            [JsonProperty(PropertyName = "System.Reason")]
            public OldValueNewValue SystemReason { get; set; }
            [JsonProperty(PropertyName = "Microsoft.VSTS.Common.ResolvedDate")]
            public NewValue MicrosoftVSTSCommonResolvedDate { get; set; }
            [JsonProperty(PropertyName = "Microsoft.VSTS.Common.ResolvedReason")]
            public NewValue MicrosoftVSTSCommonResolvedReason { get; set; }
        }

        public class OldValueNewValue
        {
            public string newValue { get; set; }
            public string oldValue { get; set; }
        }

        public class NewValue
        {
            public string newValue { get; set; }
        }
    }
}
