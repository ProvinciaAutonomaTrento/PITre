// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.Legacy.Mobile.Models.ServiceDtos;
public class Trasmissione
{
    public long? Id { get; internal set; }
    public string? NoteGenerali { get; set; }
    public string? Mittente { get; set; }
    public DateTime? Data { get; set; }


    public long? IdTrasmUtente { get; internal set; }
    public string? Ragione { get; set; }
    public string? NoteIndividuali { get; set; }
    public bool HasWorkflow { get; set; }
    public bool Accettata { get; set; }
    public bool Rifiutata { get; set; }
}
