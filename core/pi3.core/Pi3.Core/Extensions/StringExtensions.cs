// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Extensions
{
    public static class StringExtensions
    {
        public static long AsLong(this string longAsString)
        {
            if (long.TryParse(longAsString, out long result))
                return result;
            else
                throw new InvalidCastException(longAsString ?? "null");
        }

        public static DateTime AsDateTime(this string dateAsString)
        {
            DateTime result;
            if (DateTime.TryParseExact(dateAsString,
                new List<string>()
                {
                        "yyyy-MM-ddTHH:mm:ss.000+0000",
                        "dd/MM/yyyy HH:mm:ss",
                        "dd/MM/yyyy",
                        "yyyy-MM-dd",
                        "yyyyMMddHHmmss",
                        "yyyy-MM-dd'T'HH:mm:ss'Z'",
                        "yyyy-MM-ddTHH:mm:ssZ"
                }.ToArray(),
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out result))
            {
                return result;
            }
            else
                throw new InvalidCastException();
        }

        public static string PathAsUnixPath(this string pathAsString)
        {
            var separator = Path.AltDirectorySeparatorChar.ToString();

            return pathAsString
                .Replace(@"\", separator)
                .Replace(separator + separator, separator);
        }
    }
}
