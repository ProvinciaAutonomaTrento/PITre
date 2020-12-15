using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;
using DocsPaVO.utente;
using DocsPaVO.documento;
using System.ComponentModel;

namespace DocsPaVO.Interoperabilita
{
	/// <summary>
	/// Informazioni di ritorno dell'operazione di spedizione del protocollo ai destinatari
	/// </summary>
	public class SendDocumentResponse
	{
		/// <summary>
		/// Esito della spedizione del documento.
		/// False se tutti o almeno una spedizione ad un destinatario
		/// non è andata a buon fine.
		/// </summary>
		/// 
		public bool SendSucceded
		{
			get
			{
				bool retValue=true;

				foreach (SendDocumentMailResponse mailResponse in this.SendDocumentMailResponseList)
				{
					if (!mailResponse.SendSucceded)
					{
						retValue=false;
						break;
					}
				}

				return retValue;
			}
			set
			{
			}
		}

		/// <summary>
		/// Data e ora della spedizione
		/// </summary>
		public DateTime SendDateTime=DateTime.Now;

		/// <summary>
		/// Documento inviato
		/// </summary>
		public SchedaDocumento SchedaDocumento=null;

		/// <summary>
		/// Reperimento esito della spedizione di un documento a partire dall'indirizzo mail
		/// </summary>
		/// <param name="mailAddress"></param>
		/// <returns></returns>
		public SendDocumentResponse.SendDocumentMailResponse 
            GetDocumentResponseByMail(string mailAddress)
		{
			SendDocumentResponse.SendDocumentMailResponse retValue=null;
			
			foreach (SendDocumentResponse.SendDocumentMailResponse response in this.SendDocumentMailResponseList)
			{
				if (response.MailAddress.Equals(mailAddress))
				{
					retValue=response;
					break;
				}
			}

			return retValue;
		}

        /// <summary>
        /// Reperimento dell'esito della spedizione di un documento ad un destinatario
        /// </summary>
        /// <param name="destinatario"></param>
        /// <returns></returns>
        public SendDocumentResponse.SendDocumentMailResponse FindDocumentResponse(DocsPaVO.utente.Corrispondente destinatario)
        {
            foreach (SendDocumentResponse.SendDocumentMailResponse response in this.SendDocumentMailResponseList)
            {
                DocsPaVO.utente.Corrispondente[] destinatari = (DocsPaVO.utente.Corrispondente[]) response.Destinatari.ToArray(typeof(DocsPaVO.utente.Corrispondente));

                // Ricerca del destinatario tra i corrispondenti cui è stato inviato il documento
                //if (destinatari.Where(e => e.systemId == destinatario.systemId) != null)
                //    return response;
                foreach (DocsPaVO.utente.Corrispondente destX in destinatari)
                {
                    if (destX.systemId == destinatario.systemId) return response;
                }
            }

            return null;
        }

		[XmlArray()]
		[XmlArrayItem(typeof(SendDocumentMailResponse))]
		public ArrayList SendDocumentMailResponseList=new ArrayList();

		public SendDocumentResponse()
		{
		}

		public SendDocumentResponse(bool sendSucceded,SchedaDocumento schedaDocumento) : this(sendSucceded,DateTime.Now,schedaDocumento)
		{
		}

		public SendDocumentResponse(bool sendSucceded,DateTime sendDateTime,SchedaDocumento schedaDocumento)
		{
			this.SendSucceded=sendSucceded;
			this.SendDateTime=sendDateTime;
			this.SchedaDocumento=schedaDocumento;
		}

		/// <summary>
		/// Informazioni di ritorno della spedizione di un protocollo ai destinatari
		/// </summary>
		public class SendDocumentMailResponse
		{
			/// <summary>
			/// Indirizzo mail del destinatario
			/// </summary>
			public string MailAddress=string.Empty;

			/// <summary>
			/// Esito della spedizione del documento
			/// </summary>
			public bool SendSucceded=false;

			/// <summary>
			/// Messaggio di errore in caso di mancata spedizione del documento
			/// </summary>
			public string SendErrorMessage=string.Empty;

			/// <summary>
			/// Flag che definisce se l'indirizzo mail è interoperante o meno
			/// </summary>
			public bool MailNonInteroperante=false;

            /// <summary>
            /// Dati relativi alla protocollazione su registro automatico
            /// Popolato solo per i registri automatici e se INTEROP_INT_NO_MAIL="1"
            /// </summary>
            public DocsPaVO.Interoperabilita.DatiInteropAutomatica datiInteropAutomatica;


           /// <summary>
			/// Destinatari del documento
			/// </summary>
			[XmlArray()]
			[XmlArrayItem(typeof(Corrispondente))]
			public ArrayList Destinatari=new ArrayList();

			public SendDocumentMailResponse()
			{
			}

			public SendDocumentMailResponse(string mailAddress)
			{
				this.MailAddress=mailAddress;
			}

			public SendDocumentMailResponse(string mailAddress,bool sendSucceded,string sendErrorMessage) : this(mailAddress)
			{
				this.SendSucceded=sendSucceded;
				this.SendErrorMessage=sendErrorMessage;
			}

		}

	}
}
