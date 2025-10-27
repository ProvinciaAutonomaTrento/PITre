// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System.Net;

namespace Pi3.App.Legacy.Mobile.Shared.Exceptions;

public class UnexpectedException(string argument)
    : Pi3.Core.SeedWork.Pi3Exception(
        Resources.ErrorMessages.UnexpectedException,
        Resources.ErrorMessages.ResourceManager,
        null,
        argument)
{
    public override HttpStatusCode StatusCode => HttpStatusCode.InternalServerError;
}
