using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NttDataWA.Utils
{
    public class Error
    {
        public string fileName { get; set; }
        public string methodName { get; set; }
        public int line { get; set; }
        public int col { get; set; }
    }
}