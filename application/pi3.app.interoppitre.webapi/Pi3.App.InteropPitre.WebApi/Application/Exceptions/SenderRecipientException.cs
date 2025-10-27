// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.InteropPitre.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.InteropPitre.WebApi.Application.Exceptions
{
    public class SenderRecipientException : Pi3Exception
    {
        public SenderRecipientException(string message) : base(message, ErrorDescriptions.ResourceManager, null, new { }) 
        {
                
        }


    }
}
