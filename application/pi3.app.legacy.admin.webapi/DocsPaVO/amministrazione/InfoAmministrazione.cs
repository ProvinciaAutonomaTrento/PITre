// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace DocsPaVO.amministrazione
{
	/// <summary>
	/// Definizione oggetto Amministrazione 
	/// relativo alla Informazioni delle Amministrazioni nel tool di Amm.ne DocsPA
	/// </summary>
    [XmlInclude(typeof(InfoTimbro))]
    [DataContract]
	public class InfoAmministrazione
    {				
        [DataMember]
		public string IDAmm=string.Empty;
        [DataMember]
        public string Codice=string.Empty;
        [DataMember]
        public string Descrizione=string.Empty;
        [DataMember]
        public string LibreriaDB=string.Empty;
        [DataMember]
        public string Segnatura=string.Empty;
        [DataMember]
        public string Timbro_pdf=string.Empty;
        [DataMember]
        public string Fascicolatura=string.Empty;
        [DataMember]
        public string Dominio=string.Empty;
        [DataMember]
        public string ServerSMTP=string.Empty;
        [DataMember]
        public string PortaSMTP=string.Empty;
        [DataMember]
        public string UserSMTP=string.Empty;
        [DataMember]
        public string PasswordSMTP=string.Empty;
        [DataMember]
        public string IDRagioneTO=string.Empty;
        [DataMember]
        public string IDRagioneCC=string.Empty;
        [DataMember]
        public string IDRagioneCompetenza = string.Empty;
        [DataMember]
        public string IDRagioneConoscenza = string.Empty;
        [DataMember]
        public string AttivaGGPermanenzaTDL=string.Empty;
        [DataMember]
        public string GGPermanenzaTDL=string.Empty;
        [DataMember]
        public string SslSMTP = string.Empty;
        [DataMember]
        public string StaSMTP = string.Empty;
        [DataMember]
        public string FromEmail = string.Empty;
        [DataMember]
        public string Timbro_carattere = string.Empty;
        [DataMember]
        //public string Timbro_dimensione = string.Empty;
        public string Timbro_colore = string.Empty;
        [DataMember]
        public string Timbro_rotazione = string.Empty;
        [DataMember]
        public string Timbro_orientamento = string.Empty;
        [DataMember]
        public string Timbro_posizione = string.Empty;
        //public string Timbro_coordinate = string.Empty;
        [DataMember]
        public InfoTimbro Timbro;
        [DataMember]
        public string formatoDominio = string.Empty;
        [DataMember]
        public string formatoProtTitolario = string.Empty;
        [DataMember]
        public string TipologiaDocumentoObbligatoria = string.Empty;
        [DataMember]
        public string Banner = string.Empty;

        /// <summary>
        /// Identificativo del motore di elaborazione client side per la generazione dei modelli 
        /// </summary>
        [DataMember]
        public int IdClientSideModelProcessor = 0;

        /// <summary>
        /// Configurazioni 
        /// </summary>
        [DataMember]
        public DocsPaVO.Spedizione.ConfigSpedizioneDocumento SpedizioneDocumenti;

        /// <summary>
        /// Informazioni di profilo amministrazione sull'utilizzo dei componenti SmartClient 
        /// </summary>
        [DataMember]
        public DocsPaVO.SmartClient.SmartClientConfigurations SmartClientConfigurations = new SmartClient.SmartClientConfigurations();
        //public DocsPaVO.amministrazione.DispositivoStampaEtichetta DispositivoStampa = new DispositivoStampaEtichetta();
        [DataMember]
        public int? DispositivoStampa = null;

        //MEV-Firma 1 < - Aggiunto campo DETTAGLIO_FIRMA
        [DataMember]
        public string DettaglioFirma = string.Empty;
        //> 

        [DataMember]
        public string codiceIpa = string.Empty;
        [DataMember]
        public string indirizzoDigitaleRiferimento = string.Empty;


        //Nuova gestione per casella di posta MS Exchange
        [DataMember]
        public string Provider = string.Empty;
        [DataMember]
        public string TenantId = string.Empty;
        [DataMember]
        public string ClientId = string.Empty;
        [DataMember]
        public string ClientSecret = string.Empty;
    }
}
