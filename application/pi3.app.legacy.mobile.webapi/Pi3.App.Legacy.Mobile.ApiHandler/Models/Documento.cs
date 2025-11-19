// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.Legacy.Mobile.ApiHandler.Models;

public class Documento
{
    public string? IdDoc { get; internal set; }
    public string? OriginalFileName { get; internal set; }
    public string? Oggetto { get; internal set; }
    public DateTime? DataDoc { get; internal set; }
    public bool HasPreview { get; internal set; }
    public bool IsAcquisito { get; internal set; }
    public string? TipoProto { get; internal set; }
    public bool CanTransmit { get; internal set; }
    public string? IdDocPrincipale { get; internal set; }
    public string? AccessRights { get; internal set; }


    public string? Note { get; internal set; }
    public List<List<string>>? Fascicoli { get; internal set; }

    public bool IsProtocollato { get; internal set; }
    public string? Segnatura { get; internal set; }
    public DateTime? DataProto { get; internal set; }

    public string? Mittente { get; internal set; }
    public List<string>? Destinatari { get; internal set; }



    public List<string>? DescrFasc { get; set; } // mai valorizzata sul vecchio
    public string? OggettoDocPrincipale { get; set; } // utilizzati solo in librofirma
    public string? Extension { get; set; } // utilizzati solo in librofirma
}
