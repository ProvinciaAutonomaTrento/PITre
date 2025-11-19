// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System.Net;

namespace Pi3.App.Legacy.Mobile.Shared.Exceptions;

public class UnauthorizedException() 
    : Pi3.Core.SeedWork.Pi3Exception(
        Resources.ErrorMessages.UnauthorizedException,
        Resources.ErrorMessages.ResourceManager)
{
    public override HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;
}
