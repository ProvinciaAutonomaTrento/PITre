// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Pi3.App.Legacy.Mobile.Models.SelectTemplates;
public class TrasmissioneJoinSingolaRagioneUtente
{
    public long Id { get; set; }
    public DateTime? DataInvio { get; set; }
    public string? NoteGenerali { get; set; }
    public string? NoteSingole { get; set; }
    public string? Ragione { get; set; }
    public long IdTrasmissioneUtente { get; set; }
    public DateTime? DataAccettata { get; set; }
    public DateTime? DataRifiutata { get; set; }
    public string? TipoRagione { get; set; }
    public long? IdPeopleDelegato { get; set; }
    public long? IdPeople { get; set; }
}
