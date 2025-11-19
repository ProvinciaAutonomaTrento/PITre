// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.Legacy.Mobile.ApiHandler.Models.DTOs.Documents;

public class GetDocumentInfoDto
{
    public Documento? DocInfo { get; set; }
    public IEnumerable<Documento>? Allegati { get; set; }
    public Trasmissione? TrasmInfo { get; set; }
}
