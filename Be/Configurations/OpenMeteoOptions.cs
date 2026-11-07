using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Be.Configurations
{
    public class OpenMeteoOptions
    {
        public const string SectionName = "OpenMeteo";
        public string BaseUrl { get; set; } = string.Empty;     
    }
}