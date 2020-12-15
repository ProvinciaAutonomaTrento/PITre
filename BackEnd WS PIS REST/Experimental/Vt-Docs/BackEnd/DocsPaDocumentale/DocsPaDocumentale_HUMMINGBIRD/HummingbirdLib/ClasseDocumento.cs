using System;
using PCDCLIENTLib;
using log4net;

namespace DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib
{
	/// <summary>
	/// Questa classe gestisce tutti gli accessi a basso livello 
	/// all'oggetto PCDDocObjectClass della libreria Hummingbird
	/// </summary>
	public class ClasseDocumento : DocsPaUtils.Interfaces.FileManagement.IGestioneErrori
	{
        private ILog logger = LogManager.GetLogger(typeof(ClasseDocumento));
		// Istanza del documento
		protected PCDDocObjectClass docObject;

		#region Costruttori
		/// <summary>
		/// Questo costruttore instanzia un documento/progetto sul documentale
		/// </summary>
		/// <param name="dst"></param>
		/// <param name="library"></param>
		/// <param name="objectType"></param>
		public ClasseDocumento(string dst, string library, string objectType) 
		{
			docObject = new PCDDocObjectClass();
			docObject.SetDST(dst);
			docObject.SetProperty("%TARGET_LIBRARY", library);
			docObject.SetObjectType(objectType);
		}	
		#endregion

		#region Gestione errori
		/// <summary>
		/// Ritorna l'ultimo codice di errore.
		/// </summary>
		/// <returns>
		/// Codice errore dell'ultima operazione (0 = nessun errore)
		/// </returns>
		public int GetErrorCode()
		{
			int result = GetErrorCode(null);

			return result;
		}

		/// <summary>
		/// Ritorna l'ultimo codice di errore.
		/// </summary>
		/// <param name="customDescription">Eventuale descrizione custom.</param>
		/// <returns>
		/// Codice errore dell'ultima operazione (0 = nessun errore)
		/// </returns>
		public int GetErrorCode(string customDescription)
		{
			int result = 0; // Presume successo

			result = docObject.ErrNumber; 

			if(result != 0)
			{
				if(customDescription != null)
				{
					logger.Debug(customDescription);
				}

				logger.Debug("PCD Error Number: " + docObject.ErrNumber);
				logger.Debug("PCD Error Description: " + docObject.ErrDescription);
			}

			return result;
		}
		#endregion

		#region Proprietà
		/// <summary>
		/// Restituisce l'istanza del documento
		/// </summary>
		protected PCDDocObjectClass CurrentInstance
		{
			get
			{
				return this.docObject;
			}
		}

		/// <summary>
		/// </summary>
		public string AppId
		{
			set
			{
				docObject.SetProperty("APP_ID", value);
			}
		}

		/// <summary>
		/// Restituisce l'ID della versione del documento
		/// </summary>
		public string VersionId
		{
			get
			{
				return docObject.GetReturnProperty("%VERSION_ID").ToString();
			}
			set
			{
				docObject.SetProperty("%VERSION_ID", value);
			}
		}
		/// <summary>
		/// </summary>
		/// <param name="documentNumber"></param>
		/// <returns></returns>
		public string ObjectIdentifier
		{
			get
			{
				return docObject.GetReturnProperty("%OBJECT_IDENTIFIER").ToString();
			}
			set
			{
				docObject.SetProperty("%OBJECT_IDENTIFIER", value);
			}		
		}

		/// <summary>
		/// </summary>
		public string AttachmentId
		{
			set
			{
				docObject.SetProperty("%ATTACHMENT_ID", value);
			}
		}

		/// <summary>
		/// </summary>
		public string VersionComment
		{
			set
			{
				docObject.SetProperty("%VERSION_COMMENT", value);
			}
		}

		/// <summary>
		/// </summary>
		public string DocumentName
		{
			set
			{
				docObject.SetProperty("DOCNAME", value);	
			}
		}

		/// <summary>
		/// </summary>
		public string Author
		{
			set
			{
				docObject.SetProperty("AUTHOR", value);
			}
		}

		/// <summary>
		/// </summary>
		public string AuthorId
		{
			set
			{
				docObject.SetProperty("AUTHOR_ID", value);
			}
		}

		/// <summary>
		/// </summary>
		public string TypistId
		{
			set
			{
				docObject.SetProperty( "TYPIST_ID", value);	
			}
		}

		/// <summary>
		/// </summary>
		public string TypeId
		{
			set
			{
				docObject.SetProperty("TYPE_ID", value);
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="verifyOnly"></param>
		public string VerifyOnly
		{
			set
			{
				docObject.SetProperty("%VERIFY_ONLY", value);
			}
		}

		/// <summary>
		/// </summary>
		public string Status
		{
			set
			{
				docObject.SetProperty("%STATUS", value);				
			}
		}
		#endregion

		#region Metodi
		/// <summary>
		/// </summary>
		public void SetVersionDirective(string directive)
		{
			docObject.SetProperty("%VERSION_DIRECTIVE", directive);
		}

		/// <summary>
		/// </summary>
		public int Update()
		{
			int rtn;
			rtn= docObject.Update();
			
			if(docObject.ErrNumber != 0)
			{
				logger.Debug("ritento update docObject");
				System.Threading.Thread.Sleep(400);
				rtn= docObject.Update();
			}
			
			return rtn;
			// OLD:
			//return docObject.Update();
		}
		#endregion
	}
}
