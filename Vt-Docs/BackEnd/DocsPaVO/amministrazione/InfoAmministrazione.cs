using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

namespace DocsPaVO.amministrazione
{
	/// <summary>
	/// Definizione oggetto Amministrazione 
	/// relativo alla Informazioni delle Amministrazioni nel tool di Amm.ne DocsPA
	/// </summary>
    [XmlInclude(typeof(InfoTimbro))]
	public class InfoAmministrazione
	{				
		public string IDAmm=string.Empty;
		public string Codice=string.Empty;
		public string Descrizione=string.Empty;
		public string LibreriaDB=string.Empty;
		public string Segnatura=string.Empty;
        public string Timbro_pdf=string.Empty;
		public string Fascicolatura=string.Empty;
		public string Dominio=string.Empty;
		public string ServerSMTP=string.Empty;
		public string PortaSMTP=string.Empty;
		public string UserSMTP=string.Empty;
		public string PasswordSMTP=string.Empty;
		public string IDRagioneTO=string.Empty;
		public string IDRagioneCC=string.Empty;
        public string IDRagioneCompetenza = string.Empty;
        public string IDRagioneConoscenza = string.Empty;	
		public string AttivaGGPermanenzaTDL=string.Empty;
		public string GGPermanenzaTDL=string.Empty;
        public string SslSMTP = string.Empty;
        public string StaSMTP = string.Empty;
        public string FromEmail = string.Empty;
        public string Timbro_carattere = string.Empty;
        //public string Timbro_dimensione = string.Empty;
        public string Timbro_colore = string.Empty;
        public string Timbro_rotazione = string.Empty;
        public string Timbro_orientamento = string.Empty;
        public string Timbro_posizione = string.Empty;
        //public string Timbro_coordinate = string.Empty;
        public InfoTimbro Timbro;
        public string formatoDominio = string.Empty;
        public string formatoProtTitolario = string.Empty;
        public string TipologiaDocumentoObbligatoria = string.Empty;
        public string Banner = string.Empty;

        /// <summary>
        /// Identificativo del motore di elaborazione client side per la generazione dei modelli 
        /// </summary>
        public int IdClientSideModelProcessor = 0;

        /// <summary>
        /// Configurazioni 
        /// </summary>
        public DocsPaVO.Spedizione.ConfigSpedizioneDocumento SpedizioneDocumenti;

        /// <summary>
        /// Informazioni di profilo amministrazione sull'utilizzo dei componenti SmartClient 
        /// </summary>
        public DocsPaVO.SmartClient.SmartClientConfigurations SmartClientConfigurations = new SmartClient.SmartClientConfigurations();
        //public DocsPaVO.amministrazione.DispositivoStampaEtichetta DispositivoStampa = new DispositivoStampaEtichetta();
        public int? DispositivoStampa = null;

        //MEV-Firma 1 < - Aggiunto campo DETTAGLIO_FIRMA
            public string DettaglioFirma = string.Empty;
        //> 

    }
}
