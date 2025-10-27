// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Mobile.Shared.Extensions;
public static class StringExtensions
{
    public static string FirstCharToUpper( this string input )
    {
        if ( string.IsNullOrEmpty(input) )
        {
            return input;
        }

        return string.Concat(input[0].ToString().ToUpper(CultureInfo.CurrentCulture), input.AsSpan(1));
    }
}
