using Newtonsoft.Json;
using System;

namespace Json2DB
{
    public class DbModel
    {
        public long CustomerPaymentId;
        [JsonProperty(PropertyName = "CustomerUuid")]
        public long CustomerId;
        public string Status;        
        public DateTime InsertedDate;
        public string FileName;
        public DateTime LastUpdateDate;
    }
}
