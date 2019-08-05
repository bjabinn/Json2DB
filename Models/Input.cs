using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Json2DB
{
    public partial class Input
    {
        [JsonProperty("Recurringid")]
        public Guid Recurringid { get; set; }

        [JsonProperty("NonSesitivePanToken")]
        public string NonSesitivePanToken { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("CustomerUuid")]        
        public long CustomerUuid { get; set; }
    }
}
