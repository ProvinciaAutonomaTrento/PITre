// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Runtime.Serialization;

namespace DocsPaVO.amministrazione
{
	/// <summary>
	/// Summary description for OrgRisultatoRicerca.
	/// </summary>
	[DataContract]
    public class OrgRisultatoRicerca
	{
		[DataMember]
        public string Tipo { get; set; } = string.Empty;
        // valori: 'U', 'R', 'P'

        [DataMember]
        public string IDCorrGlob { get; set; } = string.Empty;
        // system_id della dpa_corr_globali

        [DataMember]
        public string Codice { get; set; } = string.Empty;
        // var_cod_rubrica della dpa_corr_globali

        [DataMember]
        public string Descrizione { get; set; } = string.Empty;
        // var_desc_corr della dpa_corr_globali

        [DataMember]
        public string IDParent { get; set; } = string.Empty;
        // ID del PARENT:
        // per la UO => system_id della UO superiore o 0 (id_parent)
        // per il ruolo => system_id della UO di appertenenza (id_uo)
        // per l'utente => system_id del ruolo di appartenenza (GROUPS.system_id)

        [DataMember]
        public string DescParent { get; set; } = string.Empty;
        // descrizione del PARENT:
        // per la UO => UO superiore
        // per il ruolo => UO di appertenenza
        // per l'utente => il ruolo di appartenenza

        [DataMember]
        public string IDGruppo { get; set; } = string.Empty;
        // per il ruolo => system_id della groups

        [DataMember]
        public string IDPeople { get; set; } = string.Empty;
        // per l'utente => system_id della people

        /// <summary>
        /// Matricola dell'utente 
        /// </summary>
        [DataMember]
        public string Matricola { get; set; } = string.Empty;
	}
}
