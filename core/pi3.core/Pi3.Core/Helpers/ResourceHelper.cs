// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Helpers
{
    public static class ResourceHelpers
    {
        public static string GetResourceLookup(Type resourceType, string resourceKey)
        {
            if ((resourceType != null) && (resourceKey != null))
            {
                PropertyInfo property = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);
                if (property == null)
                {
                    return null;
                }
                if (property.PropertyType != typeof(string))
                {
                    return null;
                }
                return (string)property.GetValue(null, null);
            }
            return null;
        }
    }
}
