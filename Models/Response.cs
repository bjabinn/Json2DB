using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json2DB.Models
{
    public class ResponseModel
    {
        public int NumRowsUpdated { get; set; }
        public ResponseStatus Status { get; set; }
        public string TraceError { get; set; }
    }
}
