// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.Legacy.Mobile.Models.ServiceDtos;

public class GetRuoliUtenteResult
{
    public long Id { get; set; }
    public string? Descrizione { get; set; }
    public long? IdGruppo { get; set; }
    public string? Codice { get; set; }
    public string? Livello { get; set; }
    public long? IdUO { get; set; }
    public IEnumerable<long>? Registri { get; set; }
}
