// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Legacy.Mobile.ApiHandler.Models.DTOs.Deleghe;
public class ModelloDelega
{
    public string? Id { get; set; }

    public string? Nome { get; set; }


    public string? DescrUtenteDelegato { get; set; }

    public string? DescrRuoloDelegante { get; set; }

    public DateTime? DataInizioDelega { get; set; }

    public DateTime? DataFineDelega { get; set; }
}
