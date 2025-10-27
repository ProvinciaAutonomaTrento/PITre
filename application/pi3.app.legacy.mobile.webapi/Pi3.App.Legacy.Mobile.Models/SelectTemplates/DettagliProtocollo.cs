// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Legacy.Mobile.Models.SelectTemplates;
public class DettagliProtocollo
{
    public string? TipoUrp { get; set; }
    public string? Descrizione { get; set; }
    public string? Cognome { get; set; }
    public string? Nome { get; set; }
    public string? Tipo { get; set; } // D = Destinatario M = Mittente
}
