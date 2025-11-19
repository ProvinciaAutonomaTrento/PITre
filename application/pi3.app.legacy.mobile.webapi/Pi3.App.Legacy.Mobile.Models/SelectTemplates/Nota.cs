// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Pi3.App.Legacy.Mobile.Models.SelectTemplates;
public class Nota
{
    public long SystemId { get; set; }
    public string? Testo { get; set; }
    public DateTime DataCreazione { get; set; }
    public long? IdFascicoloAssociato { get; set; }
    public long IdOggettoAssociato { get; set; }
    public long IdUtenteCreatore { get; set; }
    public long IdRuoloCreatore { get; set; }
    public string? TipoVisibilita { get; set; }
    public string? TipoOggettoAssociato { get; set; }
}
