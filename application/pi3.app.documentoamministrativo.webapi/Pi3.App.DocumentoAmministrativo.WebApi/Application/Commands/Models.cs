// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.App.DocumentoAmministrativo.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands;

[ResourceSwaggerSchema(nameof(Documentation.UtenteDestinatario))]
public class UtenteDestinatario
{
    [ResourceSwaggerSchema(nameof(Documentation.UserIdDestinatario))]
    public string UserId { get; set; }

    [ResourceSwaggerSchema(nameof(Documentation.RagioneTrasmissione))]
    public string RagioneTrasmissione { get; set; }

    [ResourceSwaggerSchema(nameof(Documentation.NoteTrasmissione))]
    public string? NoteTrasmissione { get; set; }

    [ResourceSwaggerSchema(nameof(Documentation.GiorniScadenza))]
    public int? GiorniScadenza { get; set; }
}

[ResourceSwaggerSchema(nameof(Documentation.GruppoDestinatario))]
public class GruppoDestinatario
{
    [ResourceSwaggerSchema(nameof(Documentation.CodiceGruppoDestinatario))]
    public string CodiceGruppo { get; set; }

    [ResourceSwaggerSchema(nameof(Documentation.RagioneTrasmissione))]
    public string RagioneTrasmissione { get; set; }

    [ResourceSwaggerSchema(nameof(Documentation.TipoTrasmissione))]
    public string? Tipo { get; set; }

    [ResourceSwaggerSchema(nameof(Documentation.NoteTrasmissione))]
    public string? NoteTrasmissione { get; set; }

    [ResourceSwaggerSchema(nameof(Documentation.GiorniScadenza))]
    public int? GiorniScadenza { get; set; }

    [ResourceSwaggerSchema(nameof(Documentation.UtentiNotificati))]
    public IEnumerable<string>? UtentiNotificati { get; set; }
}

[ResourceSwaggerSchema(nameof(Documentation.Campo_Profilo))]
public class Campo
{
    [ResourceSwaggerSchema(nameof(Documentation.NomeCampoProfilo))]
    public string Nome { get; set; }
    [ResourceSwaggerSchema(nameof(Documentation.ValoreCampoProfilo))]
    public string? Valore { get; set; }
    [ResourceSwaggerSchema(nameof(Documentation.ValoriCampoProfilo))]
    public IReadOnlyList<string>? Valori { get; set; }
    [ResourceSwaggerSchema(nameof(Documentation.TipoCampoProfilo))]
    public TipoCampoEnum Tipo { get; set; }
}

[ResourceSwaggerSchema(nameof(Documentation.Profilo))]
public class Profilo : ValueObject
{
    [ResourceSwaggerSchema(nameof(Documentation.Profilo_Nome))]
    public string Nome { get; set; }
    [ResourceSwaggerSchema(nameof(Documentation.Profilo_Campi))]
    public IReadOnlyList<Campo> Campi { get; set; }
}

[ResourceSwaggerSchema(nameof(Documentation.TipoCampoEnum))]
public enum TipoCampoEnum
{
    CampoDiTesto,
    CasellaSelezione,
    CasellaSelezioneEsclusiva,
    MenuTendina,
    Corrispondente,
    Contatore,
    Data,
    Orario,
    OrarioSecondi
}
