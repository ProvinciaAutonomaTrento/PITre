using System;
using ProtocollazioneIngresso.Log;
using DocsPAWA;

namespace ProtocollazioneIngresso.Protocollo
{
	/// <summary>
	/// Summary description for ProtocolloMng.
	/// </summary>
	public class ProtocolloMng
	{
		private System.Web.UI.Page _page=null;


		public ProtocolloMng(System.Web.UI.Page page)
		{
			this._page=page;
		}

		/// <summary>
		/// Creazione di un nuovo oggetto SchedaDocumento
		/// </summary>
		/// <returns></returns>
		public void CreaNuovoDocumento(string tipoProto) 
		{	
			DocsPAWA.DocsPaWR.SchedaDocumento retValue=new DocsPAWA.DocsPaWR.SchedaDocumento();

			retValue.predisponiProtocollazione=true;

			// Tipologia protocollo in "Arrivo"
            //retValue.tipoProto="A"; // OLD

			ProtocollazioneIngresso.Login.LoginMng loginMng=new ProtocollazioneIngresso.Login.LoginMng(this._page);
			
			DocsPAWA.DocsPaWR.Utente utente=loginMng.GetUtente();
			DocsPAWA.DocsPaWR.Ruolo ruolo=loginMng.GetRuolo();

			retValue.systemId=null;
			retValue.oggetto=new DocsPAWA.DocsPaWR.Oggetto();

			// campi obbligatori per DocsFusion
			retValue.idPeople = utente.idPeople;
			retValue.userId = utente.userId;
			

			retValue.typeId = DocsPAWA.DocumentManager.getTypeId();
			retValue.appId = "ACROBAT";
			retValue.privato = "0";  // doc non privato

			//this.SetDocumentoCorrente(retValue); // OLD
            if (tipoProto.Equals("A"))
            {
                setDatiProtocolloIngresso(retValue);
            }
            else
            {
                if (tipoProto.Equals("P"))
                {
                    setDatiProtocolloUscita(retValue);
                }
            }

		}

        /// <summary>
        /// Setta le proprietà del protocollo in uscita e mette la SchedaDocumento in sessione
        /// </summary>
        /// <param name="schedaDoc"></param>
        public void setDatiProtocolloUscita(DocsPAWA.DocsPaWR.SchedaDocumento schedaDoc)
        {
            
            // Tipologia protocollo in "Uscita"
            schedaDoc.tipoProto = "P";
            this.SetDocumentoCorrente(schedaDoc);
        }

        /// <summary>
        /// Setta le proprietà del protocollo in ingresso e mette la SchedaDocumento in sessione
        /// </summary>
        /// <param name="schedaDoc"></param>
        public void setDatiProtocolloIngresso(DocsPAWA.DocsPaWR.SchedaDocumento schedaDoc)
        {
            // Tipologia protocollo in "Arrivo"
            schedaDoc.tipoProto = "A";
            this.SetDocumentoCorrente(schedaDoc);
        }
		/// <summary>
		/// Gestione riproposizione documento corrente
		/// </summary>
		public void RiproponiDocumento(string tipoProto)
		{
            this.RiproponiDocumento(false, tipoProto);
		}

        

		/// <summary>
		/// Gestione riproposizione documento corrente
		/// </summary>
		/// <param name="enableUfficioReferente"></param>
        public void RiproponiDocumento(bool enableUfficioReferente, string tipoProto)
		{
			DocsPAWA.DocsPaWR.SchedaDocumento schedaDoc=this.GetDocumentoCorrente();
			
			if (schedaDoc==null)
			{
                this.CreaNuovoDocumento(tipoProto);
				
			}
			else
			{
				schedaDoc=DocsPAWA.DocumentManager.riproponiDati(this._page,schedaDoc,enableUfficioReferente);
				schedaDoc.predisponiProtocollazione=true;
				this.SetDocumentoCorrente(schedaDoc);
			}
		}

		/// <summary>
		/// Protocollazione del documento corrente
		/// </summary>
		/// <returns></returns>
		public bool ProtocollaDocumentoCorrente(out string errorMessage)
		{
			bool retValue=false;
			errorMessage=string.Empty;

			DocsPAWA.DocsPaWR.SchedaDocumento documentoCorrente=GetDocumentoCorrente();

			DocsPAWA.DocsPaWR.ResultProtocollazione esitoProtocollazione;
			documentoCorrente=DocsPAWA.DocumentManager.protocolla(this._page,documentoCorrente,out esitoProtocollazione);
			
			switch (esitoProtocollazione)
			{	
				case DocsPAWA.DocsPaWR.ResultProtocollazione.OK:
					// Impostazione in sessione del nuovo documento
					this.SetDocumentoCorrente(documentoCorrente);
					retValue=true;
					break;
				case DocsPAWA.DocsPaWR.ResultProtocollazione.AMMINISTRAZIONE_MANCANTE: 
					errorMessage="Identificativo dell'amministrazione non trovata.";
					break;	
				case DocsPAWA.DocsPaWR.ResultProtocollazione.DESTINATARIO_MANCANTE: 
					errorMessage="Il destinatario è obbligatorio.";
					break;	
				case DocsPAWA.DocsPaWR.ResultProtocollazione.MITTENTE_MANCANTE: 
					errorMessage="Il mittente è obbligatorio.";
					break;	
				case DocsPAWA.DocsPaWR.ResultProtocollazione.OGGETTO_MANCANTE: 
					errorMessage="L'oggetto è obbligatorio.";
					break;	
				case DocsPAWA.DocsPaWR.ResultProtocollazione.REGISTRO_MANCANTE: 
					errorMessage="Il registro è obbligatorio.";
					break;	
				case DocsPAWA.DocsPaWR.ResultProtocollazione.REGISTRO_CHIUSO: 
					errorMessage="Il registro non è aperto.";
					break;	
				case DocsPAWA.DocsPaWR.ResultProtocollazione.STATO_REGISTRO_ERRATO: 
					errorMessage="Lo stato del registro non è corretto.";
					break;	
				case DocsPAWA.DocsPaWR.ResultProtocollazione.DATA_SUCCESSIVA_ATTUALE: 
					errorMessage="La data di protocollazione è successiva a quella attuale.";
					break;	
				case DocsPAWA.DocsPaWR.ResultProtocollazione.DATA_ERRATA: 
					errorMessage="La data di protocollazione non è valida.";
					break;
                case DocsPAWA.DocsPaWR.ResultProtocollazione.FORMATO_SEGNATURA_MANCANTE:
                    errorMessage="Formato della segnatura non impostato. Contattare l\\'Amministratore";
                    
                    break;
				case DocsPAWA.DocsPaWR.ResultProtocollazione.APPLICATION_ERROR: 
					errorMessage="In questo momento non è stato possibile protocollare il documento. Si prega di ripetere  l'operazione.";
					break;	
			}

			try
			{
				// Scrittura log
				if (retValue)
				{
					string numProtocollo=documentoCorrente.protocollo.numero.ToString();

					ProtocollazioneIngressoLog.WriteLogEntry(
						string.Format("Protocollazione (Esito: {0} - NumeroProtocollo: {1})",
						retValue.ToString(),
						numProtocollo));
				}
				else
				{
					ProtocollazioneIngressoLog.WriteLogEntry(
						string.Format("Protocollazione (Esito: {0} )",errorMessage));
				}
			}
			catch
			{

			}

			return retValue;
		}

		/// <summary>
		/// Reperimento del protocollo corrente
		/// </summary>
		/// <returns></returns>
		public DocsPAWA.DocsPaWR.SchedaDocumento GetDocumentoCorrente()
		{
			return (DocsPAWA.DocsPaWR.SchedaDocumento)
				this._page.Session["ProtocollazioneIngresso.ProtocolloCorrente"];
		}

		/// <summary>
		/// Rimozione del documento corrente dalla sessione
		/// </summary>
		public void ReleaseDocumentoCorrente()
		{
			this._page.Session.Remove("ProtocollazioneIngresso.ProtocolloCorrente");
		}

		/// <summary>
		/// Verifica della presenza del documento corrente
		/// </summary>
		/// <returns></returns>
		public bool ContainsDocumento()
		{
			return DocsPAWA.DocumentManager.cercaDuplicati(this._page,this.GetDocumentoCorrente());
		}

		/// <summary>
		/// Verifica della presenza del documento corrente
		/// riportando inoltre i dettagli dei dati duplicati
		/// </summary>
		/// <param name="infoProtocolloDuplicato"></param>
		/// <returns></returns>
        public DocsPAWA.DocsPaWR.EsitoRicercaDuplicatiEnum ContainsDocumento(out DocsPAWA.DocsPaWR.InfoProtocolloDuplicato[] infoProtocolloDuplicato)
		{
            // DA SOSTITUIRE PER 2.17 al posoto della riga successiva
            //string cercaDuplicati2 = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "CERCA_DUPLICATI_PROTOCOLLO_2");
            string cercaDuplicati2 = ConfigSettings.getKey(ConfigSettings.KeysENUM.CERCA_DUPLICATI_PROTOCOLLO_2);
            if (cercaDuplicati2 == null || !cercaDuplicati2.Equals("1"))
                cercaDuplicati2 = "0";
            infoProtocolloDuplicato = new DocsPAWA.DocsPaWR.InfoProtocolloDuplicato[0];
			return DocsPAWA.DocumentManager.cercaDuplicati(this._page,this.GetDocumentoCorrente(), cercaDuplicati2, out infoProtocolloDuplicato);
		}

		/// <summary>
		/// Impostazione del protocollo corrente
		/// </summary>
		/// <param name="schedaDocumento"></param>
		private void SetDocumentoCorrente(DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento)
		{
			this._page.Session["ProtocollazioneIngresso.ProtocolloCorrente"]=schedaDocumento;
		}
	}
}