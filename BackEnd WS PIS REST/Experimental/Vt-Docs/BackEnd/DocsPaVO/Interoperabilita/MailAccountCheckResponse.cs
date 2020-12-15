using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.Interoperabilita
{
	/// <summary>
	/// Esito del controllo dell'account di posta elettronica certificata
	/// nell'ambito della verifica della casella istituzionale
	/// </summary>
	public class MailAccountCheckResponse
	{
		public string MailUserID=string.Empty;
		public string MailServer=string.Empty;
		public string MailAddress=string.Empty;
		public string Registro=string.Empty;

		/// <summary>
		/// Messaggio in caso di di errore nel controllo della casella di posta
		/// </summary>
		public string ErrorMessage=string.Empty;

		/// <summary>
		/// Numero di mail processate non valide
		/// </summary>
		public int InvalidMailProcessedCount=0;

        /// <summary>
        /// Data dello scarico
        /// </summary>
        public DateTime DtaConcluded;
		
		/// <summary>
		/// Lista singole mail lette
		/// </summary>
		[XmlArray()]
		[XmlArrayItem(typeof(MailProcessed))]
		public ArrayList MailProcessedList=new ArrayList();

		/// <summary>
		/// 
		/// </summary>
		public MailAccountCheckResponse()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mailUserID"></param>
		/// <param name="mailServer"></param>
		/// <param name="mailAddress"></param>
		/// <param name="registro"></param>
		public MailAccountCheckResponse(string mailUserID,
										string mailServer,
										string mailAddress,
										string registro)
		{
			this.MailUserID=mailUserID;
			this.MailServer=mailServer;
			this.MailAddress=mailAddress;
			this.Registro=registro;
		}

		/// <summary>
		/// Rappresentazione della singola mail verificata
		/// </summary>
		public class MailProcessed
		{
			public string MailID=string.Empty;

			public DateTime Date=DateTime.Now;

			public string From=string.Empty;

			[XmlArray()]
			[XmlArrayItem(typeof(string))]
			public ArrayList Recipients=new ArrayList();

			public string Subject=string.Empty;

			public int CountAttatchments=0;

			public string ErrorMessage=string.Empty;
			
			public MailProcessed.MailProcessedType ProcessedType= MailProcessed.MailProcessedType.NonPEC;
            public MailProcessed.MailPecXRicevuta PecXRicevuta = MailPecXRicevuta.unknown;

			public MailProcessed()
			{
			}

			public enum MailProcessedType
			{
				Signature,
				ConfirmReception,
				Pec,
				NotifyCancellation,
                DatiCert,
                Eccezione,
                NonPEC
			}

            public enum MailPecXRicevuta
            {
                unknown,
                Delivery_Status_Notification,
                PEC_Delivered_Notify,
                PEC_Delivered_Notify_Short,
                PEC_Delivered,
                From_Non_PEC,
                PEC_Accept_Notify,
                PEC_Alert_Virus,
                PEC_Contain_Virus,
                PEC_Error,
                PEC_Error_Delivered_Notify_By_Virus,
                PEC_Error_Preavviso_Delivered_Notify,
                PEC_Non_Accept_Notify,
                PEC_Presa_In_Carico,
                PEC_NO_XRicevuta,
                PEC_Mancata_Consegna
            }

			/// <summary>
			/// Classe per il confronto delle date delle mail
			/// </summary>
			public class MailProcessedComparer : IComparer
			{
				#region IComparer Members

				public int Compare(object x, object y)
				{
					MailProcessed mailX=x as MailProcessed;
					MailProcessed mailY=y as MailProcessed;
					
					return mailY.Date.CompareTo(mailX.Date);
				}

				#endregion
			}
		}
	}
}
