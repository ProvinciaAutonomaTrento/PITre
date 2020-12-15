using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.amministrazione
{
	/// <summary>
	/// Rappresentazione di un nodo di titolario
	/// </summary>
	public class OrgNodoTitolario
	{
		public string ID=string.Empty;

		public string Codice=string.Empty;

		public string Descrizione=string.Empty;

		/// <summary>
		/// Se true, nel nodo di titolario possono essere creati fascicoli
		/// </summary>
		public bool CreazioneFascicoliAbilitata=false;

		/// <summary>
		/// Mesi di conservazione di un nodo di titolario
		/// </summary>
		public int NumeroMesiConservazione=0;

		/// <summary>
		/// Codice dell'amministrazione cui il nodo di titolario fa parte
		/// </summary>
		public string CodiceAmministrazione=string.Empty;

		/// <summary>
		/// ID del registro associato al nodo di titolario.
		/// Se "Empty", il nodo è associato a tutti i registri dell'amministrazione corrente.
		/// </summary>
		public string IDRegistroAssociato=string.Empty;

		public string CodiceLivello=string.Empty;

		public string Livello=string.Empty; // (NUM_LIVELLO)

		/// <summary>
		/// ID del nodo titolario parent.
		/// </summary>
		public string IDParentNodoTitolario=string.Empty;

		/// <summary>
		/// Numero nodi titolario figli
		/// </summary>
		public int CountChildNodiTitolario=0;

        /// <summary>
        /// System Id tipo fascicolo
        /// </summary>
        public string ID_TipoFascicolo = string.Empty;
        /// <summary>
        /// Blocca tipo fascicolo
        /// </summary>
        public string bloccaTipoFascicolo = string.Empty;
        /// <summary>
        /// Blocca tipo fascicolo
        /// </summary>
        public string ID_Titolario = string.Empty;
        /// <summary>
        /// Data Attivazione
        /// </summary>
        public string dataAttivazione = string.Empty;
        /// <summary>
        /// Data Cessazione
        /// </summary>
        public string dataCessazione = string.Empty;

        /// <summary>
        /// Stato
        /// </summary>
        public string stato = string.Empty;
        /// <summary>
        /// Note
        /// </summary>
        public string note = string.Empty;

        /// <summary>
        /// Parole chiavi 
        /// </summary>
        public string[] ParoleChiavi = null;

        /// <summary>
        /// Rappresentazione stringa del nodo di titolario
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} - {1}", this.Codice, this.Descrizione);
        }

        public string bloccaNodiFigli = string.Empty;

        public string contatoreAttivo = string.Empty;

        public string numProtoTit = string.Empty;

        public string dataCreazione = string.Empty;

        public string consentiClassificazione = string.Empty;

        public string consentiFascicolazione = string.Empty;
        public string isImport = string.Empty;

        public string IDTemplateStrutturaSottofascicoli = string.Empty;
    }
}
