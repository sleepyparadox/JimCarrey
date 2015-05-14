using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JimCarrey.FB
{
    public class Error
    {
        public string message { get; set; }
        public string type { get; set; }
        public string code { get; set; }
        public WebExceptionStatus WebExceptionStatus { get; set; }
        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
