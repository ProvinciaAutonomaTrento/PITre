// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Legacy.Mobile.Models.SelectTemplates;
public class DettagliDocumento
{
    public long SystemId { get; set; }
    public string? NomeOriginale { get; set; }
    public string? Oggetto { get; set; }
    public DateTime? CreationDate { get; set; }
    public long? FileSize { get; set; }
    public string? Path { get; set; }
    public string? TipoProto { get; set; }
    public long? IdDocumentoPrincipale { get; set; }
    public long? AccessRight { get; set; }

    public string? Segnatura { get; set; }
    public string? DaProtocollare { get; set; }
    public DateTime? DataProtocollazione { get; set; }

    public long? VersionId { get; set; }
}
