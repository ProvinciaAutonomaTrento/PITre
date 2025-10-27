// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocumentFormat.OpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.DocumentFormat.OpenXml.Services
{
    internal static class OpenXmlElementExtensions
    {
        // Extension method creato per disambiguare le invocazioni ai metodi append
        internal static OpenXmlElement[] AsArray(this OpenXmlElement element)
        {
            return new OpenXmlElement[1] { element };
        }
    }
}
