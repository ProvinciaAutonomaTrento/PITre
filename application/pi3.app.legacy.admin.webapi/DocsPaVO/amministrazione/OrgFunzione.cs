// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Runtime.Serialization;

namespace DocsPaVO.amministrazione
{
    /// <summary>
    /// Rappresentazione di una funzione elementare in anagrafica amministrazione
    /// </summary>
    [DataContract]
    public class OrgFunzioneAnagrafica
	{
        [DataMember]
        public string Codice { get; set; } = string.Empty;

        [DataMember]
        public string Descrizione { get; set; } = string.Empty;

        [DataMember]
        public string TipoFunzione { get; set; } = string.Empty;
	}

	/// <summary>
	/// Rappresentazione del legame tra una funzione elementare e un tipo funzione
	/// </summary>
	[DataContract]
	public class OrgFunzione
	{
		/// <summary>
		/// ID della funzione
		/// </summary>
		[DataMember]
		public string ID { get; set; } = string.Empty;

        /// <summary>
        /// ID del tipo funzione
        /// </summary>
        [DataMember]
        public string IDTipoFunzione { get; set; } = string.Empty;

        /// <summary>
        /// Associazione con la corrispondente funzione in anagrafica
        /// </summary>
        [DataMember]
        public OrgFunzioneAnagrafica FunzioneAnagrafica { get; set; } = null;

        /// <summary>
        /// Flag, true se la funzione elementare � associata al tipo funzione
        /// </summary>
        [DataMember]
        public bool Associato { get; set; } = false;

        /// <summary>
        /// Stato della funzione, default: non modificato
        /// </summary>
        [DataMember]
        public StatoOrgFunzioneEnum StatoFunzione { get; set; } = StatoOrgFunzioneEnum.Unchanged;

		/// <summary>
		/// Definizione degli stati della funzione
		/// </summary>
		public enum StatoOrgFunzioneEnum
		{
			Unchanged,	// non modificato
			Inserted,	// inserito il legame
			Deleted		// cancellato il legame
		}

        /// <summary>
        /// idAmministrazione di riferimento
        /// </summary>
        [DataMember]
        public string IDAmministrazione { get; set; } = string.Empty;
	}
}