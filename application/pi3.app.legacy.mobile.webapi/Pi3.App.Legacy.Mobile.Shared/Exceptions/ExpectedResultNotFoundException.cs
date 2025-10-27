// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System.Net;

namespace Pi3.App.Legacy.Mobile.Shared.Exceptions;
public class ExpectedResultNotFoundException( string operationDetails ) : 
    NotFoundPi3Exception(
        Resources.ErrorMessages.ExpectedResultNotFoundException, 
        Resources.ErrorMessages.ResourceManager,
        operationDetails) 
{
    public override HttpStatusCode StatusCode => HttpStatusCode.InternalServerError;
}
