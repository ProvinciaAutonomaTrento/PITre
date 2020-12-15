using System;

namespace DocsPaVO.amministrazione
{
	/// <summary>
	/// Summary description for OrgRisultatoRicerca.
	/// </summary>
	public class OrgRisultatoRicerca
	{
		public string Tipo = string.Empty;	
		// valori: 'U', 'R', 'P'
 
		public string IDCorrGlob = string.Empty;
		// system_id della dpa_corr_globali

		public string Codice = string.Empty;
		// var_cod_rubrica della dpa_corr_globali

		public string Descrizione = string.Empty;
		// var_desc_corr della dpa_corr_globali

		public string IDParent = string.Empty;
		// ID del PARENT:
		// per la UO => system_id della UO superiore o 0 (id_parent)
		// per il ruolo => system_id della UO di appertenenza (id_uo)
		// per l'utente => system_id del ruolo di appartenenza (GROUPS.system_id)

		public string DescParent = string.Empty;		
		// descrizione del PARENT:
		// per la UO => UO superiore
		// per il ruolo => UO di appertenenza
		// per l'utente => il ruolo di appartenenza

		public string IDGruppo = string.Empty;
		// per il ruolo => system_id della groups

		public string IDPeople = string.Empty;
		// per l'utente => system_id della people

        /// <summary>
        /// Matricola dell'utente 
        /// </summary>
        public string Matricola = string.Empty;
	}
}
