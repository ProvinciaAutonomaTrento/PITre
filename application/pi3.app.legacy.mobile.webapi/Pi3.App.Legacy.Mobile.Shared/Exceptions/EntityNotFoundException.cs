// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System.Net;

namespace Pi3.App.Legacy.Mobile.Shared.Exceptions;
public class EntityNotFoundException(string entity, long id) 
    : Core.SeedWork.NotFoundPi3Exception(
        Resources.ErrorMessages.EntityNotFoundException,
        Resources.ErrorMessages.ResourceManager,
        entity, id )
{
    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
}
