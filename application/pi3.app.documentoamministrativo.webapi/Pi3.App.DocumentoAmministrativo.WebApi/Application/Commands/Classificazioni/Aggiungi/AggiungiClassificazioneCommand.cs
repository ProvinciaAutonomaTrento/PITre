// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Classificazioni.Aggiungi;

[ResourceSwaggerSchema("AggiungiClassificazioneCommandResponse")]
public class AggiungiClassificazioneCommandResponse : ValueObject
{
    [ResourceSwaggerSchema("DocumentoAmministrativo")]
    public Documento DocumentoAmministrativo { get; init; }
    [ResourceSwaggerSchema("listaServiziApi")]
    public IEnumerable<Link> Links { get; set; }

}

/// <summary>
/// dati per la aggiunta di una classificazione di un documento
/// </summary>
//public class Classification : ValueObject
//{
//    /// <summary>
//    /// codice della classificazione
//    /// </summary>
//    [Required(AllowEmptyStrings = false)]
//    public string CodiceClassificazione { get; set; }
//    /// <summary>
//    ///  codice della tipologia di fascicolo
//    /// </summary>
//    public string TipologiaFascicolo { get; set; }
//}



public class AggiungiClassificazioneCommand : IRequest<AggiungiClassificazioneCommandResponse>
{
    public string Id { get; init; }
    public string Classificazione { get; init; }
}
