// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Crea;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.App.DocumentoAmministrativo.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Profili;

[ResourceSwaggerSchema(nameof(Documentation.StoreProfiloCommandResponse))]
public class StoreProfiloCommandResponse : ValueObject {
    [ResourceSwaggerSchema(nameof(Documentation.IdDocumentoRiferimento))]
    public string Id { get; set; }

    [ResourceSwaggerSchema(nameof(Documentation.listaServiziApi))]
    public IEnumerable<Link> Links { get; set; }
}

//[ResourceSwaggerSchema(nameof(Documentation.CampoProfilo))]
//public class Campo
//{
//    [ResourceSwaggerSchema(nameof(Documentation.NomeCampoProfilo))]
//    public string Nome { get; set; }
//    [ResourceSwaggerSchema(nameof(Documentation.ValoreCampoProfilo))]
//    public string? Valore { get; set; }
//    [ResourceSwaggerSchema(nameof(Documentation.ValoriCampoProfilo))]
//    public IReadOnlyList<string>? Valori { get; set; }
//    [ResourceSwaggerSchema(nameof(Documentation.TipoCampoProfilo))]
//    public TipoCampoEnum Tipo { get; set; }
//}

public class StoreProfiloCommand: IRequest<StoreProfiloCommandResponse>
{
    public string Id { get; set; }
    public Profilo Profilo { get; set; }
}
