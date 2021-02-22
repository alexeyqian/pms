using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.VSDTO
{
    public class Avatar
    {
        public string href { get; set; }
    }

    public class _Links
    {
        public Avatar avatar { get; set; }
    }

    public class Href
    {
        public string href { get; set; }
    }
    public class Createdby
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
