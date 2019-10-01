using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaDB
{
    static class Utils
    {

        public static int ConvertField(string field)
        {
            int val = 0;
            if (!string.IsNullOrEmpty(field))
            {
                try
                {
                    val = Convert.ToInt32(field);
                }
                catch (Exception exc)
                {
                }
            }
            return val;
        }
    }
}
