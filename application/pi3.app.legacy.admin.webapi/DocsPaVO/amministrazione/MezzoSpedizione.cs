// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Runtime.Serialization;

namespace DocsPaVO.amministrazione
{
	/// <summary>
	/// Definizione oggetto Mezzo Spedizione 
	/// </summary>
	[DataContract]
	public class MezzoSpedizione
	{
		[DataMember]
        public string Descrizione = string.Empty;

        [DataMember]
        public string IDAmministrazione = string.Empty;

        [DataMember]
        public string IDSystem = string.Empty;

        [DataMember]
        public string chaTipoCanale = string.Empty;

        [DataMember]
        public string Disabled = string.Empty;
    }
}
