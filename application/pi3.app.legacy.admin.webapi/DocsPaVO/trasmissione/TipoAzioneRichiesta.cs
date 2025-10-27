// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.trasmissione
{
    /// <summary>
    /// </summary>
    [XmlType("TipoAzioneRichiesta")]
    public enum TipoAzioneRichiesta
    {
        DIGITALE_CADES, //FIRMA CADES
        DIGITALE_PADES, //FIRMA PADES
        ELETTRONICA_CON_SOTTOSCRIZIONE, // FIRMA ELETTRONICA
        ELETTRONICA_AVANZAMENTO_ITER  //AVANZAMENTO DEL PROCESSO DI FIRMA
    }
}
