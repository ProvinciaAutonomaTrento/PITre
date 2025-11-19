// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System.Net;

namespace Pi3.App.Legacy.Mobile.Shared.Exceptions;
public class ConnectionStringNotFoundPi3Exception() 
    : NotFoundPi3Exception(
        Resources.ErrorMessages.ConnectionStringNotFound, 
        Resources.ErrorMessages.ResourceManager)
{
    public override HttpStatusCode StatusCode => HttpStatusCode.InternalServerError;
}

