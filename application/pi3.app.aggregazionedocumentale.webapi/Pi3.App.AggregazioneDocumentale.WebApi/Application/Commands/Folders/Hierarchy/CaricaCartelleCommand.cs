// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Binders;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Folders.Hierarchy
{

    [ResourceSwaggerSchema(nameof(Documentation.CaricaCartelleCommandResponse))]
    public class CaricaCartelleCommandResponse : ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.listaServiziApi))]
        public IEnumerable<Link> Links { get; set; }
    }

    [ResourceSwaggerSchema(nameof(Documentation.CaricaCartelleCommand))]
    public class CaricaCartelleCommand : IRequest<CaricaCartelleCommandResponse>
    {
        public string? Id { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.Sottofascicoli))]
        public FolderHierarcy Sottofascicoli { get; set; }
    }
}
