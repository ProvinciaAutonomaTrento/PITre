// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
	public class RicercaDuplicati : FileRequest 
	{
        public enum EsitoRicercaDuplicatiEnum
        {
            // Ecco i valori possibili di ritorno
            NessunDuplicato,
            ProtocolloNullo,
            NoProtocolloIngresso,
            NoMittente,
            DuplicatiMittenteProtocollo,
            DuplicatiMittenteOggetto,
            DuplicatiMittenteData,
            ErroreGenerico
        }
    }
}