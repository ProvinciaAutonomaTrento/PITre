// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Extensions
{
    public static class ErrorExtensions
    {
        public static string GetErrorCode(this string error, System.Resources.ResourceManager resourceManager = null)
        {
            if (resourceManager == null)
                return null;

            var entry =
                resourceManager.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true)
                  .OfType<DictionaryEntry>()
                  .FirstOrDefault(e => e.Value.ToString() == error);

            if (entry.Key == null)
                return null;
            else
                return entry.Key.ToString();
        }
    }
}
