﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JimCarrey.FB
{
    public class TokenExtended : Response
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }

        public DateTime GetExpireTime()
        {
            return DateTime.UtcNow.AddSeconds(double.Parse(expires_in));
        }
    }
}