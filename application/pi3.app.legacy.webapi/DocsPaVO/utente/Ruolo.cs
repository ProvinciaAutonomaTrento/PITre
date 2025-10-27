// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.utente 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	public class Ruolo: Corrispondente 
	{
        /// <summary>
        /// livello del tipo Ruolo del Ruolo
        /// </summary>
		public string livello;
        /// <summary>
        /// codice rubrica
        /// </summary>
		public string codice;
        /// <summary>
        /// id univoco del Ruolo
        /// </summary>
        /// <remarks>GROUPS.SYSTEM_ID</remarks>
		public string idGruppo;
		public string codiceIstat;
        /// <summary>
        /// deprecato
        /// </summary>
		public bool selezionato = false;
        /// <summary>
        /// tipo Ruolo del ruolo
        /// </summary>
		public TipoRuolo tipoRuolo;
        /// <summary>
        /// uo Padre del ruolo
        /// </summary>
		public UnitaOrganizzativa uo;

        /// <summary>
        /// Booleano che indica se bisogna visualizzare il pulsante della storia del ruolo nella
        /// maschera di visibilit� del documento.
        /// Il tipo String � stato utilizzato al posto di boolean per mantenere la compatibilit� con
        /// chi utilizza gli smart services in quanto il tipo boolean produce un wsdl con minOccur 1
        /// </summary>
        public String ShowHistory { get; set; }

        /// <summary>
        /// arraylist delle funzioni abilitate
        /// </summary>
		public DocsPaVO.utente.Funzione[] funzioni;

        /// <summary>
        /// arraylist dei registri associati al Ruolo
        /// </summary>
		public DocsPaVO.utente.Registro[] registri;

        /// <summary>
        /// 
        /// </summary>
        public bool Responsabile = false;

        /// <summary>
        /// 
        /// </summary>
        public bool Segretario = false;

        /// <summary>
        /// Autenticazione Sistemi Esterni
        /// Se � un sistema esterno � 1
        /// </summary>
        public string RuoloDiSistema;

		/// <summary>
		/// </summary>
		public Ruolo()
		{
		}

        public Ruolo(long? idGruppo)
        {
            if (idGruppo.HasValue)
                this.idGruppo = idGruppo.ToString();
        }

        /// <summary>
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="descrizione"></param>
        /// <param name="codice"></param>
        /// <param name="livello"></param>
        /// <param name="idGruppo"></param>
        /// <param name="tipoRuolo"></param>
        /// <param name="funzioni"></param>
        public Ruolo(string systemId,
					 string descrizione,
					 string codice,
					 string livello,
					 string idGruppo,
					 TipoRuolo tipoRuolo,
                     DocsPaVO.utente.Funzione[] funzioni)
		{
			this.systemId=systemId;
			this.descrizione=descrizione;
			this.codice=codice;
			this.livello=livello;
			this.idGruppo=idGruppo;
			this.funzioni=funzioni;
			this.tipoRuolo=tipoRuolo;
			
		}
	}
}