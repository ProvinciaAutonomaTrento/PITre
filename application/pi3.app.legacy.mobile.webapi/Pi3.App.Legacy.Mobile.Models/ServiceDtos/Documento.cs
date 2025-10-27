// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Legacy.Mobile.Models.ServiceDtos;
public class Documento
{
    public long IdDocumento { get; set; }
    public string? NomeFileOriginale { get; set; }
    public string? Oggetto { get; set; }
    public DateTime? DataCreazione { get; set; }
    public bool HasAnteprima{ get; set; }
    public bool IsAcquisito { get; set; }
    public long? DimensioneFile { get; set; }
    public string? FilePath { get; set; }
    public string? TipoProtocollo { get; set; }
    public bool TrasmissioneAbilitata { get; set; }
    public long? IdDocPrincipale { get; set; }

    public long? DirittiDiAccesso { get; set; }


    public string? Note { get; set; }
    public List<List<string>>? Fascicoli { get; set; }

    public bool IsProtocollato { get; set; }
    public string? Segnatura { get; set; }
    public DateTime? DataProtocollazione { get; set; }

    public string? Mittente { get; set; }
    public List<string>? Destinatari { get; set; }
}
