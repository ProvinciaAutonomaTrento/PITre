// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions
{
    public class LookupNotFoundP3Exception : Pi3Exception
    {
        public LookupNotFoundP3Exception(string message, System.Resources.ResourceManager resourceManager, string idAggregazioneDocumentale)
            : base(message, null, resourceManager) { 
        }
    }
}
