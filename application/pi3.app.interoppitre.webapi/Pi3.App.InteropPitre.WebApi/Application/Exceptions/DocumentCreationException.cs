// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.InteropPitre.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.InteropPitre.WebApi.Application.Exceptions
{
    public class DocumentCreationException : Pi3Exception
    {
        public DocumentCreationException(string message) : base(message, ErrorDescriptions.ResourceManager, new { })
        {
                
        }
    }
}
