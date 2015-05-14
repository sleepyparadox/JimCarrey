using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JimCarrey.FB
{
    public class Token
    {
        public string app_id { get; set; }
        public string application { get; set; }
        public string expires_at { get; set; }
        public bool is_valid { get; set; }
        public string issued_at { get; set; }
        public string[] scopes { get; set; }
        public string user_id { get; set; }

        public DateTime GetExpireTime()
        {
            return DateTime.UtcNow.AddSeconds(double.Parse(expires_at));  
        }
    }
}
