﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JimCarrey.FB
{
    public class Edge<T> : Response
    {
        public T[] data { get; set; }
    }
}
