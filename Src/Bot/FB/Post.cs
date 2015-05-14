using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JimCarrey.FB
{
    public class Post : Response
    {
        public string id { get; set; }
        public string type { get; set; }
        public string story { get; set; }
        public Edge<User> likes { get; set; }
    }
}
