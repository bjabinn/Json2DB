using Newtonsoft.Json;
using System;

namespace Json2DB
{
    public class DbModel
    {
        [JsonProperty(PropertyName = "Recurringid")]
        public string RecurringPaymentId;
        [JsonProperty(PropertyName = "CustomerUuid")]
        public long CustomerId;
        public string Status;
        [JsonProperty(PropertyName = "NonSesitivePanToken")]
        public string NonSensitivePanToken;
        public DateTime InsertedDate;
        public string FileName;
        public DateTime LastUpdateDate;
    }
}
