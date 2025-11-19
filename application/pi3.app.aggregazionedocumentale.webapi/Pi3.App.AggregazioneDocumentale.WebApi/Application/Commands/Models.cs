// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands
{
    public class IdDoc : ValueObject
    {
        public string Identiticativo { get; set; }
    }

    public class FolderHierarcy : ValueObject
    {
        public string Nome { get; set; }
        public IEnumerable<IdDoc>? Documenti { get; set; }
        public IEnumerable<FolderHierarcy>? Sottofascicoli { get; set; }
    }

    public class UtenteDestinatario
    {
        public string UserId { get; set; }
        public string RagioneTrasmissione { get; set; }
        public string? NoteTrasmissione { get; set; }
        public int? GiorniScadenza { get; set; }
    }


    public class GruppoDestinatario
    {
        public string CodiceGruppo { get; set; }
        public string RagioneTrasmissione { get; set; }
        public string? Tipo { get; set; }
        public string? NoteTrasmissione { get; set; }
        public int? GiorniScadenza { get; set; }
        public IEnumerable<string>? UtentiNotificati { get; set; }
    }


}
