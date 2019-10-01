using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace printPen
{
    class Utils
    {
        public static string FormatJs(string text)
        {
            try
            {
                string result = "";
                if (!string.IsNullOrEmpty(text)) result = text.Replace("\"", "\\\"").Replace("'", "\\'").Replace("\r\n", "\\n").Replace("\n", "\\n");
                return result;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }
    }
}
