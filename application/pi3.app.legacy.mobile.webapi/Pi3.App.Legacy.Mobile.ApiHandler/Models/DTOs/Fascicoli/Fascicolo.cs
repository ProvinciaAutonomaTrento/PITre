// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.Legacy.Mobile.ApiHandler.Models.DTOs.Fascicoli;
public class Fascicolo
{
    public string? IdFasc { get; set; }
    public string? Codice { get; set; }
    public string? Descrizione { get; set; }
    public string? Note { get; set; }
    public DateTime? DataApertura { get; set; }
    public DateTime? DataChiusura { get; set; }
    public bool CanTransmit { get; set; }
    public string? AccessRights { get; set; }
}
