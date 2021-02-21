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
}
