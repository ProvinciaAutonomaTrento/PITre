// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.amministrazione
{
	/// <summary>
	/// Rappresentazione di un nodo di titolario
	/// </summary>
    [DataContract]
	public class OrgNodoTitolario
	{
        [DataMember]
        public string ID { get; set; } = string.Empty;

        [DataMember]
        public string Codice { get; set; } = string.Empty;

        [DataMember]
        public string Descrizione { get; set; } = string.Empty;

        /// <summary>
        /// Se true, nel nodo di titolario possono essere creati fascicoli
        /// </summary>
        [DataMember]
        public bool CreazioneFascicoliAbilitata { get; set; } = false;

        /// <summary>
        /// Mesi di conservazione di un nodo di titolario
        /// </summary>
        [DataMember]
        public int NumeroMesiConservazione { get; set; } = 0;

        /// <summary>
        /// Codice dell'amministrazione cui il nodo di titolario fa parte
        /// </summary>
        [DataMember]
        public string CodiceAmministrazione { get; set; } = string.Empty;

        /// <summary>
        /// ID del registro associato al nodo di titolario.
        /// Se "Empty", il nodo � associato a tutti i registri dell'amministrazione corrente.
        /// </summary>
        [DataMember]
        public string IDRegistroAssociato { get; set; } = string.Empty;

        [DataMember]
        public string CodiceLivello { get; set; } = string.Empty;

        [DataMember]
        public string Livello { get; set; } = string.Empty; // (NUM_LIVELLO)

        /// <summary>
        /// ID del nodo titolario parent.
        /// </summary>
        [DataMember]
        public string IDParentNodoTitolario { get; set; } = string.Empty;

        /// <summary>
        /// Numero nodi titolario figli
        /// </summary>
        [DataMember]
        public int CountChildNodiTitolario { get; set; } = 0;

        /// <summary>
        /// System Id tipo fascicolo
        /// </summary>
        [DataMember]
        public string ID_TipoFascicolo { get; set; } = string.Empty;
        /// <summary>
        /// Blocca tipo fascicolo
        /// </summary>
        [DataMember]
        public string bloccaTipoFascicolo { get; set; } = string.Empty;
        /// <summary>
        /// Blocca tipo fascicolo
        /// </summary>
        [DataMember]
        public string ID_Titolario { get; set; } = string.Empty;
        /// <summary>
        /// Data Attivazione
        /// </summary>
        [DataMember]
        public string dataAttivazione { get; set; } = string.Empty;
        /// <summary>
        /// Data Cessazione
        /// </summary>
        [DataMember]
        public string dataCessazione { get; set; } = string.Empty;

        /// <summary>
        /// Stato
        /// </summary>
        [DataMember]
        public string stato { get; set; } = string.Empty;
        /// <summary>
        /// Note
        /// </summary>
        [DataMember]
        public string note { get; set; } = string.Empty;

        /// <summary>
        /// Parole chiavi 
        /// </summary>
        [DataMember]
        public string[] ParoleChiavi { get; set; } = null;

        /// <summary>
        /// Rappresentazione stringa del nodo di titolario
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} - {1}", this.Codice, this.Descrizione);
        }

        [DataMember]
        public string bloccaNodiFigli { get; set; } = string.Empty;

        [DataMember]
        public string contatoreAttivo { get; set; } = string.Empty;

        [DataMember]
        public string numProtoTit { get; set; } = string.Empty;

        [DataMember]
        public string dataCreazione { get; set; } = string.Empty;

        [DataMember]
        public string consentiClassificazione { get; set; } = string.Empty;

        [DataMember]
        public string consentiFascicolazione { get; set; } = string.Empty;
        [DataMember]
        public string isImport { get; set; } = string.Empty;

        [DataMember]
        public string IDTemplateStrutturaSottofascicoli { get; set; } = string.Empty;
    }
}
