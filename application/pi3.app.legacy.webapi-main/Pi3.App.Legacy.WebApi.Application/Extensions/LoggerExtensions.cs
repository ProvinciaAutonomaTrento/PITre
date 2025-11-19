// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Extensions
{
    public static class LoggerExtensions
    {
        public static void LogWebMethodError(this ILogger logger, Exception ex, [CallerMemberName] string? callerWebMethodName = null)
        {
            logger.LogError(ex, Descriptions.LogWebMethodError, callerWebMethodName);
        }
    }
}
