// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later

using Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.Domain;

namespace Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.ElaborateNewInteroperabilityMessage
{
    public class ElaborateNewInteroperabilityMessageResponse
    {
        public ElaborateInteroperabilityMessageResult Result { get; set; }
    }
}
