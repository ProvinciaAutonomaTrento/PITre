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
    public static class DateTimeExtensions
    {
        public static string AsHoursMinutesSecondsFormat(this DateTime? date)
        {
            if (date.HasValue)
                return date.Value.AsHoursMinutesSecondsFormat();
            else
                return null;
        }

        public static string AsHoursMinutesSecondsFormat(this DateTime date)
        {
            return date.ToString(DateTimeExtensionsResources.HoursMinutesSecondsFormat);
        }

        public static string? AsDateFormat(this DateTime? date)
        {
            if (date.HasValue)
                return date.Value.AsDateFormat();
            else
                return null;
        }

        public static string AsDateFormat(this DateTime date)
        {
            return date.ToString(DateTimeExtensionsResources.DateFormat);
        }

        public static string? AsDateTimeFormat(this DateTime? date)
        {
            if (date.HasValue)
                return date.Value.AsDateTimeFormat();
            else
                return null;
        }

        public static string AsDateTimeFormat(this DateTime date)
        {
            return date.ToString(DateTimeExtensionsResources.DateTimeFormat);
        }
    }
}
