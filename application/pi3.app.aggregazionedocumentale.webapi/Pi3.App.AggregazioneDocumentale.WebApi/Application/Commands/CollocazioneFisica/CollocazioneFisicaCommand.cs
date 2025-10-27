// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Binders;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.CollocazioneFisica
{
    [ResourceSwaggerSchema(nameof(Documentation.CollocazioneFisica))]
    public class CollocazioneFisica : ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.CollocazioneFisica_Codice))]
        public string Codice { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.CollocazioneFisica_Cartaceo))]
        public bool Cartaceo { get; set; }
    }

    [ResourceSwaggerSchema(nameof(Documentation.CollocazioneFisicaCommandResponse))]
    public class CollocazioneFisicaCommandResponse: ValueObject 
    {
        [ResourceSwaggerSchema(nameof(Documentation.listaServiziApi))]
        public IEnumerable<Link> Links { get; set; }
    }

    [ResourceSwaggerSchema(nameof(Documentation.CollocazioneFisicaCommand_Head))]
    public class CollocazioneFisicaCommand: IRequest<CollocazioneFisicaCommandResponse>
    {
        public string? Id { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.CollocazioneFisica))]
        public CollocazioneFisica CollocazioneFisica { get; set; }
    }
}
