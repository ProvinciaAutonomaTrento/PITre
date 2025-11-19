// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System.Net;

namespace Pi3.App.Legacy.Mobile.Shared.Exceptions;

public class RequestParamNotFoundException( string param ) 
    : Pi3.Core.SeedWork.Pi3Exception(
        Resources.ErrorMessages.RequestParamNotFoundException,
        Resources.ErrorMessages.ResourceManager,
        null,
        param)
{
    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}
