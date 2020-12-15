using System;
using DocsPaDbManagement.Functions;
using System.Configuration;
using System.Threading;
using System.Data;
using System.Collections;
using log4net;
using DocsPaUtils.Data;
using DocsPaVO.amministrazione;
using System.Linq;
using DocsPaUtils;

namespace DocsPaDB.Query_DocsPAWS
{
	/// <summary>
	/// Classe per la gestione del documentale ETDOCS
	/// </summary>
	public class Documentale : DBProvider
	{
        private ILog logger = LogManager.GetLogger(typeof(Documentale));
		public long GetRND()
		{
			long retValue;

			System.Random random = new System.Random();
			retValue = 1000000000 + random.Next(10000000);

			return retValue;
		}
        
		/// <summary>
		/// Crea un documento
		/// </summary>
		/// <param name="schedaDocumento"></param>
		/// <returns></returns>
		public string CreateDocument(DocsPaVO.documento.SchedaDocumento schedaDocumento)
		{
			string result = null;

			try
			{			
				this.BeginTransaction();
				long randomized;
				
				lock(this)
				{					
					randomized = this.GetRND();
				
					logger.Debug("Generato codice random: " + randomized.ToString());

					//Leggi documentType 
					string docTypeResult = null;

					//string sql = "SELECT SYSTEM_ID FROM DocumentTypes WHERE TYPE_ID='" + schedaDocumento.typeId + "'";

					DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DocumentTypes4");	
					q.setParam("param1", "'" + schedaDocumento.typeId + "'");
					string sql = q.getSQL();
					logger.Debug(sql);				
					bool queryResult = this.ExecuteScalar(out docTypeResult, sql);

					if(!queryResult || docTypeResult == null) throw new Exception();

					//INSERT su Profile
					string profileResult = null;

					//sql = "INSERT INTO Profile (SYSTEM_ID, TYPIST, AUTHOR, DOCUMENTTYPE, CREATION_DATE, CREATION_TIME) VALUES (" + Functions.GetSystemIdNextVal(null) + schedaDocumento.idPeople + ", " + schedaDocumento.idPeople + ", " + docTypeResult + "," + Functions.GetDate() +  ", " + Functions.GetDate() + ")";
					q = DocsPaUtils.InitQuery.getInstance().getQuery("I_Profile");	
					q.setParam("param0", Functions.GetSystemIdColName());
					q.setParam("param1", Functions.GetSystemIdNextVal(null));
					q.setParam("param2", schedaDocumento.idPeople);
					q.setParam("param3", schedaDocumento.idPeople);
					q.setParam("param4", docTypeResult);
					q.setParam("param5", Functions.GetDate(false));
					q.setParam("param6", Functions.GetDate());
					q.setParam("param7", randomized.ToString());
					sql = q.getSQL();
					logger.Debug(sql);
					if(!this.ExecuteNonQuery(sql)) throw new Exception();
				

					//sql = "SELECT SYSTEM_ID FROM Profile WHERE DOCNUMBER=" + randomized + " ORDER BY SYSTEM_ID DESC";
					q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Profile0");	
					q.setParam("param1", "SYSTEM_ID");
					q.setParam("param2", "DOCNUMBER = " + randomized + " ORDER BY SYSTEM_ID DESC");				
					sql = q.getSQL();
					logger.Debug(sql);
					if(!this.ExecuteScalar(out profileResult, sql)) throw new Exception();

					// aggiunto da Maurizio per versione Filenet il 10/06/2005
					if (schedaDocumento.docNumber==null || schedaDocumento.docNumber.Length==0)
						schedaDocumento.docNumber=profileResult;

					//sql = "UPDATE Profile SET DOCNUMBER=SYSTEM_ID WHERE SYSTEM_ID=" + profileResult;
					q = DocsPaUtils.InitQuery.getInstance().getQuery("U_Profile");	
					q.setParam("param1", "DOCNUMBER=" + schedaDocumento.docNumber);
					q.setParam("param2", "SYSTEM_ID = " + profileResult);				
					sql = q.getSQL();
					logger.Debug(sql);
					if(!this.ExecuteNonQuery(sql)) throw new Exception();

					//INSERT su Versions
					string versionsResult = null;

					//				sql = "INSERT INTO Versions (DOCNUMBER, VERSION, SUBVERSION, VERSION_LABEL, AUTHOR, TYPIST, DTA_CREAZIONE) VALUES (" + profileResult +  ", 1, '!', '1'," + schedaDocumento.idPeople + ", " + schedaDocumento.idPeople + ", '" + Functions.GetDate(true) + "')";
					//				if(!this.ExecuteNonQuery(sql)) throw new Exception();
					if(!this.InsertIntoVersions("1", "!", "1", schedaDocumento.idPeople, schedaDocumento.idPeople, schedaDocumento.docNumber)) 
						throw new Exception();

					//sql = "SELECT VERSION_ID FROM Versions WHERE TYPIST=" + schedaDocumento.idPeople + " ORDER BY VERSION_ID DESC";
					q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Versions");
					q.setParam("param1", "MAX(VERSION_ID)");
					q.setParam("param2", "TYPIST = " + schedaDocumento.idPeople);				
					sql = q.getSQL();
					logger.Debug(sql);
					if(!this.ExecuteScalar(out versionsResult, sql)) throw new Exception();

					//INSERT su Components				   
					if(!InsertIntoComponent(versionsResult, schedaDocumento.docNumber)) throw new Exception();

					//INSERT su Security				
					//sql = "INSERT INTO Security (THING, PERSONORGROUP, ACCESSRIGHTS) VALUES (" + profileResult + ", " + schedaDocumento.idPeople + ", 0)";
					q = DocsPaUtils.InitQuery.getInstance().getQuery("I_Security");	
					q.setParam("param1", profileResult + ", " + schedaDocumento.idPeople + ", 0, NULL, NULL, NULL");				
					sql = q.getSQL();
					logger.Debug(sql);


					if(!this.ExecuteNonQuery(sql)) throw new Exception();

					string documentType=ConfigurationManager.AppSettings["documentale"];
					if (documentType.ToUpper()=="FILENET")
						result=schedaDocumento.docNumber;
					else
						result = profileResult;
				}
				this.CommitTransaction();
				this.CloseConnection();
			}
			catch(Exception exception)
			{
				logger.Debug("Errore nella creazione del documento.", exception);
				this.RollbackTransaction();
				this.CloseConnection();
				result = null;
			}

			return result;
		}

		/// <summary>
		/// Questo metodo effettua la creazione di un Nuovo Documento mediante una Store Procedure
		/// </summary>
		/// <param name="schedaDocumento">Scheda documento corrente</param>
		/// <returns></returns>
		public string CreateDocumentSP(DocsPaVO.documento.SchedaDocumento schedaDocumento)
		{
            logger.Info("BEGIN");

			//retProc = systemId del documento inserito
			string retProc=null;
			
			try
			{
				logger.Debug("INIZIO transazione in CreateDocumentSP");
				this.BeginTransaction();

				// Creazione parametri per la Store Procedure
				ArrayList parameters = new ArrayList();
				DocsPaUtils.Data.ParameterSP outParam;
				outParam = new DocsPaUtils.Data.ParameterSP("systemId",new Int32(),10, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
					
				if((schedaDocumento.idPeople!=null && schedaDocumento.idPeople!="")
					|| (schedaDocumento.typeId!=null && schedaDocumento.typeId!=""))
				{
					parameters.Add(this.CreateParameter("idpeople",schedaDocumento.idPeople));
					parameters.Add(this.CreateParameter("doctype",schedaDocumento.typeId));

                    string idPeopleDelegato = string.Empty;
                    if (schedaDocumento.creatoreDocumento != null && schedaDocumento.creatoreDocumento.idPeopleDelegato != null)
                        idPeopleDelegato = schedaDocumento.creatoreDocumento.idPeopleDelegato;
                    parameters.Add(this.CreateParameter("idPeopleDelegato", idPeopleDelegato));
                    if(schedaDocumento != null && schedaDocumento.documenti != null && schedaDocumento.documenti.Count >= 1)
                        parameters.Add(this.CreateParameter("isFirmato", ((DocsPaVO.documento.FileRequest) schedaDocumento.documenti[0]).firmato));
                    else
                        parameters.Add(this.CreateParameter("isFirmato", "0"));
                    parameters.Add(outParam);
                    

					//this.ExecuteStoreProcedure("createDocSP",parameters);
					this.ExecuteStoredProcedure("createDocSP",parameters,null);
				}
				if(outParam.Valore!=null && outParam.Valore.ToString()!="" && outParam.Valore.ToString()!="0")
				{
					retProc=outParam.Valore.ToString();
					this.CommitTransaction();
					logger.Debug("COMMIT transazione in CreateDocumentSP");			
				}
				else
				{
					this.RollbackTransaction();
					logger.Debug("ROLLBACK transazione in CreateDocumentSP");
				}	

			}
			catch(Exception e)
			{
				this.RollbackTransaction();
				logger.Debug("ROLLBACK transazione in CreateDocumentSP" + e.Message);
				
			}
			finally
			{
				this.CloseConnection();
				logger.Debug("FINE transazione in CreateDocumentSP");
                logger.Info("END");
			}
            
			return retProc;
			
		}

        public string CreazioneNuovaVersioneSP(string docNumber, string subVersion,  string idPeople, string descrizione, string cartaceo)
        {
            //retProc = systemId del documento inserito
            string retProc = null;

            try
            {
                logger.Debug("INIZIO transazione in CreazioneNuovaVersioneSP");

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    // Creazione parametri per la Store Procedure
                    ArrayList parameters = new ArrayList();
                    DocsPaUtils.Data.ParameterSP outParam;
                    outParam = new DocsPaUtils.Data.ParameterSP("versionID", new Int32(), 10, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);

                    parameters.Add(this.CreateParameter("idPeople", idPeople));
                    parameters.Add(this.CreateParameter("docNumber", docNumber));
                    parameters.Add(this.CreateParameter("subVersion", subVersion));
                    parameters.Add(this.CreateParameter("descrizione", descrizione));
                    parameters.Add(this.CreateParameter("cartaceo", cartaceo));

                    parameters.Add(outParam);

                    dbProvider.ExecuteStoredProcedure("addNuovaVersione", parameters, null);

                    if (outParam.Valore != null && outParam.Valore.ToString() != "" && outParam.Valore.ToString() != "0")
                    {
                        retProc = outParam.Valore.ToString();
                        logger.Debug("COMMIT transazione in CreazioneNuovaVersioneSP");
                    }
                    else
                    {
                        logger.Debug("ROLLBACK transazione in CreazioneNuovaVersioneSP");
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("ROLLBACK transazione in CreazioneNuovaVersioneSP" + e.Message);

            }
            finally
            {
                logger.Debug("FINE transazione in CreazioneNuovaVersioneSP");
            }

            return retProc;

        }

		private DocsPaUtils.Data.ParameterSP CreateParameter(string name, object value)
		{	
			return new DocsPaUtils.Data.ParameterSP(name,value);
		}

		/// <summary>
		/// Aggiunge una versione del documento.
		/// </summary>
		/// <param name="fileRequest"></param>
		/// <param name="idPeople"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public bool AddVersion(ref DocsPaVO.documento.FileRequest fileRequest, string idPeople, string userId)
		{
            logger.Info("BEGIN");
			bool result = true; // Presume successo

			try
			{
                if (!string.IsNullOrEmpty(fileRequest.versionId))
				{
					//UPDATE su Versions
					//string sql = "UPDATE Versions SET SUBVERSION='A', VERSION_LABEL='" + fileRequest.versionLabel + "A" + "' WHERE VERSION_ID=" + fileRequest.versionId;
					DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_Versions");

                    //string param1 = string.Empty;
                    //bool isAcquired = false;

                    //if (!string.IsNullOrEmpty(fileRequest.fileName))
                    //{
                    //    // Verifica se il file risulta acquisito
                    //    int fileSize;
                    //    Int32.TryParse(fileRequest.fileSize, out fileSize);
                    //    isAcquired = (fileSize > 0);
                    //}

                    //if (isAcquired)
                    //{
                    //    int cartaceo = 0;
                    //    if (fileRequest.cartaceo)
                    //        cartaceo = 1;
                    //    q.setParam("param1", "SUBVERSION = 'A', CARTACEO = " + cartaceo.ToString());				
                    //}
                    //else
                    //{
                    //    q.setParam("param1", "SUBVERSION = '!', CARTACEO = 0");
                    //}

                    int cartaceo = 0;
                    if (fileRequest.cartaceo)
                        cartaceo = 1;
                    
                    q.setParam("param1", "SUBVERSION = 'A', CARTACEO = " + cartaceo.ToString());				
					q.setParam("param2", "VERSION_ID = " + fileRequest.versionId);
					string sql = q.getSQL();
					logger.Debug(sql);
					if(!this.ExecuteNonQuery(sql)) throw new Exception();
				} 
				else
                {
                    #region old (ora è gestita con una store procedure) 
                    /*
                    string versionId = null;
                    //string sql = "SELECT VERSION FROM VERSIONS WHERE DOCNUMBER=" + fileRequest.docNumber + " ORDER BY VERSION DESC";
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Versions");	
                    q.setParam("param1", "VERSION");				
                    q.setParam("param2", "DOCNUMBER=" + fileRequest.docNumber + " ORDER BY VERSION DESC");
                    string sql = q.getSQL();
                    logger.Debug(sql);
                    if(!this.ExecuteScalar(out versionId, sql)) throw new Exception();

                    //INSERT su Versions
                    string versionsResult = null;				
                    int newVersion = (Int32.Parse(versionId) + 1);

                    
                    if(!this.InsertIntoVersions(newVersion.ToString(), "!", newVersion.ToString(), idPeople, idPeople, fileRequest.docNumber, fileRequest.descrizione, fileRequest.cartaceo)) 
                        throw new Exception();
					
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Versions");	
                    q.setParam("param1", "VERSION_ID");				
                    q.setParam("param2", "TYPIST = " + idPeople + " ORDER BY VERSION_ID DESC");
                    sql = q.getSQL();
                    logger.Debug(sql);
                    if(!this.ExecuteScalar(out versionsResult, sql)) throw new Exception();

                    //INSERT su Components				   
                    if(!this.InsertIntoComponent(versionsResult, fileRequest.docNumber)) throw new Exception();
                    
                    fileRequest.versionId = versionsResult;
                  */
                    #endregion

                    string cartaceo = "";
                    if (fileRequest.cartaceo)
                        cartaceo = "1";
                    else
                        cartaceo = "0";

                    fileRequest.versionId = CreazioneNuovaVersioneSP(fileRequest.docNumber, "!",
                        idPeople, fileRequest.descrizione, cartaceo);
                }

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'aggiunta di una versione.", exception);

				result = false;
			}

            logger.Info("END");
			return result;
		}

		/// <summary>
		/// Aggiunge un allegato.
		/// </summary>
		/// <param name="allegato"></param>
		/// <param name="idPeople"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public bool AddAttachment(ref DocsPaVO.documento.Allegato allegato, string idPeople, string userId)
		{
			bool result = true; // Presume successo

			try
			{
				//INSERT su Versions
				string versionsResult = null;				

				//					string sql = "INSERT INTO Versions (DOCNUMBER, VERSION, SUBVERSION, VERSION_LABEL, AUTHOR, TYPIST) VALUES (" + allegato.docNumber +  ", 0, '', '" + allegato.versionLabel + "', " + idPeople + ", " + idPeople + ")";
				//					if(!this.ExecuteNonQuery(sql)) throw new Exception();
				if(!this.InsertIntoVersions("0", "", allegato.versionLabel, idPeople, idPeople, allegato.docNumber)) throw new Exception();

				//string sql = "SELECT VERSION_ID FROM Versions WHERE TYPIST=" + idPeople + " ORDER BY VERSION_ID DESC";
				DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Versions");	
				q.setParam("param1", "VERSION_ID");				
				q.setParam("param2", "TYPIST = " + idPeople + " ORDER BY VERSION_ID DESC");
				string sql = q.getSQL();
				logger.Debug(sql);
				if(!this.ExecuteScalar(out versionsResult, sql)) throw new Exception();

				//INSERT su Components				   
				if(!InsertIntoComponent(versionsResult, allegato.docNumber)) throw new Exception();

				allegato.versionId = versionsResult;
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'aggiunta di un allegato.", exception);

				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// Rimuove una versione
		/// </summary>
		/// <param name="docNumber"></param>
		/// <param name="versionId"></param>
		/// <returns></returns>
		public bool RemoveVersion(string docNumber, string versionId)
		{
			bool result = true; // Presume successo

			try
			{
				//DELETE su Versions				   
				//				string sql = "DELETE FROM VERSIONS WHERE DOCNUMBER=" + docNumber + " AND VERSION_ID=" + versionId;
				//				if(!this.ExecuteNonQuery(sql)) throw new Exception();
				if(!this.DeleteTable("VERSIONS", versionId, docNumber)) throw new Exception();

				//DELETE su Components				   
				//				sql = "DELETE FROM COMPONENTS WHERE DOCNUMBER=" + docNumber + " AND VERSION_ID=" + versionId;
				//				if(!this.ExecuteNonQuery(sql)) throw new Exception();
				if(!this.DeleteTable("COMPONENTS", versionId, docNumber)) throw new Exception();

                //AGGIORNO L'ESTENZIONE NEL CAMPO EXT DI PROFILE
                UpdateExtensionIntoProfile(docNumber);

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione di una versione.", exception);

				result = false;
			}
		
			return result;
		}


        public bool UpdateExtensionIntoProfile(string docnumber)
        {
            bool result = true; // Presume successo

            try
            {
                //string sql = "UPDATE COMPONENTS SET PATH='" + filePath + "' WHERE VERSION_ID=" + versionId;
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_EXT_PROFILE");
                q.setParam("docnumber", docnumber);
                string sql = q.getSQL();
                logger.Debug(sql);
                if (!this.ExecuteNonQuery(sql)) throw new Exception();
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante l'aggiornamento del nome del file.", exception);

                result = false;
            }

            return result;
        }

		/// <summary>
		/// Aggiorna il nome del file sul database.
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="versionId"></param>
		/// <returns></returns>
		public bool UpdateFileName(string filePath, string versionId)
		{
			bool result = true; // Presume successo

			try
			{
				//string sql = "UPDATE COMPONENTS SET PATH='" + filePath + "' WHERE VERSION_ID=" + versionId;
				DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_Components");	
				q.setParam("param1", "PATH = '" + filePath + "'");				
				q.setParam("param2", "VERSION_ID = " + versionId);
				string sql = q.getSQL();
				logger.Debug(sql);
				if(!this.ExecuteNonQuery(sql)) throw new Exception();
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'aggiornamento del nome del file.", exception);

				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// Ritorna il nome del file sul database
		/// </summary>
		/// <param name="versionId"></param>
		/// <returns></returns>
		public string GetFileName(string versionId)
		{
            logger.Info("BEGIN");
			string result = null;

			try
			{
				//string sql = "SELECT PATH FROM COMPONENTS WHERE VERSION_ID=" + versionId;
				DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Components");	
				q.setParam("param1", "PATH");				
				q.setParam("param2", "VERSION_ID = " + versionId);
				string sql = q.getSQL();
				logger.Debug(sql);
				if(!this.ExecuteScalar(out result, sql)) throw new Exception();
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'acquisizione del nome del file.", exception);

				result = null;
			}

            logger.Info("END");
            return result;
		}

		/// <summary>
		/// Cancella un documento
		/// </summary>
		/// <param name="docNumber"></param>
		/// <returns></returns>
		public bool DeleteDocument(string docNumber)
		{
			bool result = true; // Presume successo

			try
			{
				//SELECT per estrarre SYSTEM_ID da PROFILE
				string systemId;
				//string sql = "SELECT SYSTEM_ID FROM Profile WHERE DOCNUMBER=" + docNumber;
				DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Profile0");	
				q.setParam("param1", "SYSTEM_ID");				
				q.setParam("param2", "DOCNUMBER = " + docNumber);
				string sql = q.getSQL();
				logger.Debug(sql);
				if(!this.ExecuteScalar(out systemId, sql)) throw new Exception();

				//DELETE su Profile
				//sql = "DELETE FROM Profile WHERE DOCNUMBER=" + docNumber;
				q = DocsPaUtils.InitQuery.getInstance().getQuery("D_Profile");							
				q.setParam("param1", "DOCNUMBER = " + docNumber);
				sql = q.getSQL();
				logger.Debug(sql);
				if(!this.ExecuteNonQuery(sql)) throw new Exception();

				//DELETE su Versions
				//sql = "DELETE FROM Versions WHERE DOCNUMBER=" + docNumber;
				q = DocsPaUtils.InitQuery.getInstance().getQuery("D_Versions");								
				q.setParam("param1", "DOCNUMBER = " + docNumber);
				sql = q.getSQL();
				logger.Debug(sql);
				if(!this.ExecuteNonQuery(sql)) throw new Exception();

				//DELETE su Components				   
				//sql = "DELETE FROM Components WHERE DOCNUMBER=" + docNumber;
				q = DocsPaUtils.InitQuery.getInstance().getQuery("D_Components");								
				q.setParam("param1", "DOCNUMBER = " + docNumber);
				sql = q.getSQL();
				logger.Debug(sql);
				if(!this.ExecuteNonQuery(sql)) throw new Exception();

				//DELETE su Security	
				//sql = "DELETE FROM Security WHERE THING=" + systemId;
				q = DocsPaUtils.InitQuery.getInstance().getQuery("D_SECURITY");								
				q.setParam("param1", "THING = " + systemId);
				sql = q.getSQL();
				logger.Debug(sql);
				if(!this.ExecuteNonQuery(sql)) throw new Exception();
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione di un documento.", exception);

				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// Estrazione dell'ID dell'ultima versione inserita
		/// </summary>
		/// <param name="docNumber"></param>
		/// <returns></returns>
		public string GetLatestVersionId(string docNumber)
		{
			string result = null;

			try
			{
				//string sql = "SELECT VERSION_ID FROM Versions WHERE DOCNUMBER=" + docNumber + " ORDER BY VERSION DESC";
				DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Versions");	
				q.setParam("param1", "VERSION_ID");				
				q.setParam("param2", "DOCNUMBER=" + docNumber + " ORDER BY VERSION DESC");
				string sql = q.getSQL();
				logger.Debug(sql);
				if(!this.ExecuteScalar(out result, sql)) throw new Exception();
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura dell'ultima versionId.", exception);

				result = null;
			}
		
			return result;
		}

        /// <summary>
        /// Prelievo della version a partire dalla versionId, serve per la carta di identita del documento.
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns></returns>
        public string GetVersionFromVersionId(string versionId)
        {
            string retVal = null;

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Versions");
                q.setParam("param1", "VERSION");
                q.setParam("param2", "VERSION_ID = " + versionId);
                string sql = q.getSQL();
                logger.Debug(sql);
                if (!this.ExecuteScalar(out retVal, sql)) throw new Exception();
            }
            catch (Exception ex)
            {
                logger.Debug("Errore nella lettura dell'ultima version a partire dal versionId");
            }

            return retVal;
        }

		/// <summary>
		/// Crea un nuovo progetto
		/// </summary>
		/// <param name="descrizione"></param>
		/// <returns></returns>
		public string CreateProject(string descrizione, string idPeople)
		{
			string result = null;
			//IDataReader dr=null;
			try
			{
				//INSERT su Project
				/* Utilizzo la generazione di un numero RANDOM per l'identificazione 
				 * univoca del record all'interno della tabella project.
				 */
				//				System.Random random = new System.Random();
				//				long randomized = 1000000000 + random.Next(10000000);

				//string sql = "INSERT INTO Project (SYSTEM_ID, DESCRIPTION, ICONIZED, ETDOC_RANDOM_ID) VALUES (" + Functions.GetSystemIdNextVal(null) + "'" + descrizione + "', 'Y', " + randomized + ")";
				DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_PROJECT_ET");					
				q.setParam("colID",DocsPaDbManagement.Functions.Functions.GetSystemIdColName() );
				//q.setParam("param1", " DESCRIPTION, ICONIZED, ETDOC_RANDOM_ID");
				q.setParam("param1", " DESCRIPTION, ICONIZED");
				//q.setParam("param2", "'" + DocsPaUtils.Functions.Functions.ReplaceApexes(descrizione) + "', 'Y', " + randomized);
				q.setParam("param2", "'" + DocsPaUtils.Functions.Functions.ReplaceApexes(descrizione) + "', 'Y'");
				q.setParam("id",DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("PROJECT"));
				string sql = q.getSQL();
				logger.Debug(sql);
				int rowAffected;
				this.BeginTransaction();
				if(this.ExecuteNonQuery(sql, out rowAffected))
				{
					if (rowAffected>0)
					{
						sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("PROJECT");
						this.ExecuteScalar(out result, sql);
					}
				}
				else
				{
					throw new Exception();
				}

				
				//				//sql = "SELECT SYSTEM_ID FROM Project WHERE ETDOC_RANDOM_ID=" + randomized;
				//				q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project0");	
				//				q.setParam("param1", "SYSTEM_ID");				
				//				q.setParam("param2", "WHERE ETDOC_RANDOM_ID = " + randomized);
				//				sql = q.getSQL();
				//				logger.Debug(sql);
				//				if(!this.ExecuteScalar(out result, sql)) throw new Exception();

				

				#region Codice Commentato
				//sql = "SELECT SYSTEM_ID FROM Project WHERE ETDOC_RANDOM_ID=" + randomized;
				//				q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project0");	
				//				q.setParam("param1", "SYSTEM_ID");				
				//				q.setParam("param2", "WHERE ETDOC_RANDOM_ID = " + randomized);
				//				sql = q.getSQL();
				//				logger.Debug(sql);
				//				if(!this.ExecuteScalar(out result, sql)) throw new Exception();
				#endregion

				//sql = "UPDATE Project SET ETDOC_RANDOM_ID=0 WHERE ETDOC_RANDOM_ID=" + randomized;
				//				q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROJECTRANDOM");	
				//				q.setParam("param1", randomized.ToString());				
				//				sql = q.getSQL();
				//				logger.Debug(sql);
				//				if(!this.ExecuteNonQuery(sql)) throw new Exception();

				//INSERT su Security				
				//sql = "INSERT INTO Security (THING, ACCESSRIGHTS) VALUES (" + result + ", 0)";
				q = DocsPaUtils.InitQuery.getInstance().getQuery("I_Security");	
				q.setParam("param1", result + ", " + idPeople + ", 0, NULL, NULL, NULL");								
				sql = q.getSQL();
				logger.Debug(sql);
				if(!this.ExecuteNonQuery(sql)) throw new Exception();

				this.CommitTransaction();
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante linserimento su tabella PROJECT.", exception);
				this.RollbackTransaction();
				result = null;
			}
			finally
			{
				
				this.CloseConnection();
			}
		
			return result;
		}

		/// <summary>
		/// Crea un nuovo progetto
		/// </summary>
		/// <param name="descrizione"></param>
		/// <returns></returns>
		public string CreateProject(string descrizione, string idPeople, DocsPaDB.DBProvider dbProvider)
		{
			string result = null;
	
			try
			{
				
				DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_PROJECT_ET");					
				q.setParam("colID",DocsPaDbManagement.Functions.Functions.GetSystemIdColName() );
				
				q.setParam("param1", " DESCRIPTION, ICONIZED");
				q.setParam("param2", "'" + DocsPaUtils.Functions.Functions.ReplaceApexes(descrizione) + "', 'Y'");
				q.setParam("id",DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("PROJECT"));
				string sql = q.getSQL();
				logger.Debug(sql);
				int rowAffected;
				//this.BeginTransaction();
				if(dbProvider.ExecuteNonQuery(sql, out rowAffected))
				{
					if (rowAffected>0)
					{
						sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("PROJECT");
						dbProvider.ExecuteScalar(out result, sql);
					}
				}
				else
				{
					throw new Exception();
				}
				
				
				q = DocsPaUtils.InitQuery.getInstance().getQuery("I_Security");	
				q.setParam("param1", result + ", " + idPeople + ", 0, NULL, NULL, NULL");								
				sql = q.getSQL();
				logger.Debug(sql);
				if(!dbProvider.ExecuteNonQuery(sql)) throw new Exception();

				//this.CommitTransaction();
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante linserimento su tabella PROJECT.", exception);
				//this.RollbackTransaction();
				result = null;
			}
			//			finally
			//			{
			//				
			//				this.CloseConnection();
			//			}
		
			return result;
		}

        public string CreateProjectSP(string descrizione, string idPeopleDelegato, string idPeople)
        {
            logger.Info("BEGIN");
            //retProc = systemId del project creato
            string retProc = null;

            try
            {
                logger.Debug("INIZIO CreateProjectSP");

                // Creazione parametri per la Store Procedure
                ArrayList parameters = new ArrayList();
                DocsPaUtils.Data.ParameterSP outParam;
                outParam = new DocsPaUtils.Data.ParameterSP("projectId", new Int32(), 10, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);

                if ((descrizione != null && descrizione != "")
                    || (idPeople != null && idPeople != ""))
                {
                    parameters.Add(this.CreateParameter("idpeople", idPeople));
                    parameters.Add(this.CreateParameter("description", descrizione));

                    parameters.Add(this.CreateParameter("idPeopleDelegato", idPeopleDelegato));

                    parameters.Add(outParam);

                    using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                        dbProvider.ExecuteStoredProcedure("createProjectSP", parameters, null);
                }
                if (outParam.Valore != null && outParam.Valore.ToString() != "" && outParam.Valore.ToString() != "0")
                {
                    retProc = outParam.Valore.ToString();
                   
                    logger.Debug("FINE CreateProjectSP");
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore durante il metodo CreateProjectSP" + e.Message);
                retProc = null;        
            }
            logger.Info("END");
            return retProc;

        }

        /// <summary>
		/// Cancella progetto
		/// </summary>
		/// <param name="idProject"></param>
		/// <returns></returns>
		public bool DeleteProject(string idProject)
		{
			bool result = true; // Presume successo

			try
			{
				//string sql = "DELETE FROM Project WHERE SYSTEM_ID=" + idProject;
				DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_PROJECT");	
				q.setParam("param1", "SYSTEM_ID = " + idProject);								
				string sql = q.getSQL();
				logger.Debug(sql);
				if(!this.ExecuteNonQuery(sql)) throw new Exception();

				//sql = "DELETE FROM Security WHERE THING=" + idProject;
				q = DocsPaUtils.InitQuery.getInstance().getQuery("D_SECURITY");	
				q.setParam("param1", "THING = " + idProject);								
				sql = q.getSQL();
				logger.Debug(sql);
				if(!this.ExecuteNonQuery(sql)) throw new Exception();
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione di un project.", exception);

				result = false;
			}
		
			return result;
		}

		#region Esecuzione Comandi SQL
		/// <summary>
		/// Esegue una insert sulla tabella COMPONENTS
		/// </summary>
		/// <param name="version"></param>
		/// <param name="docNumber"></param>
		/// <returns></returns>
		public bool InsertIntoComponent(string versionId, string docNumber)
		{			   
			//string sql = "INSERT INTO Components (VERSION_ID, DOCNUMBER, FILE_SIZE) VALUES (" + versionId + ", " + docNumber + ", 0)";
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_Components");	
			q.setParam("param1", "VERSION_ID, DOCNUMBER, FILE_SIZE");
			q.setParam("param2", versionId + ", " + docNumber + ", 0");					
			string sql = q.getSQL();
			logger.Debug(sql);
			return this.ExecuteNonQuery(sql);
		}

		/// <summary>
        /// Esegue una insert sulla tabella VERSIONS
		/// </summary>
		/// <param name="version"></param>
		/// <param name="docNumber"></param>
		/// <returns></returns>
		public bool InsertIntoVersions(string versionId, string subVersion, string versionLabel, string author, string typist, string docNumber)
		{
			//string sql = "INSERT INTO Versions (VERSION_ID, DOCNUMBER, VERSION, SUBVERSION, VERSION_LABEL, AUTHOR, TYPIST, DTA_CREAZIONE) VALUES (" + Functions.GetSystemIdNextVal(null) + docNumber +  ", " + versionId + ", '" + subVersion + "', '" + versionLabel + "'," + author + ", " + typist + ", " + Functions.GetDate() + ")";
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_VERSIONS");	
			q.setParam("param1", Functions.GetVersionIdColName() + " DOCNUMBER, VERSION, SUBVERSION, VERSION_LABEL, AUTHOR, TYPIST, DTA_CREAZIONE");
			q.setParam("param2", Functions.GetSystemIdNextVal(null) + docNumber +  ", " + versionId + ", '" + subVersion + "', '" + versionLabel + "'," + author + ", " + typist + ", " + Functions.GetDate());					
			string sql = q.getSQL();
			logger.Debug(sql);
			return this.ExecuteNonQuery(sql);
		}

        /// <summary>
        /// Esegue una insert nella tabella VERSIONS,
        /// con possibilità di inserire l'informazione se il doc è cartaceo o meno
        /// </summary>
        /// <param name="versionId"></param>
        /// <param name="subVersion"></param>
        /// <param name="versionLabel"></param>
        /// <param name="author"></param>
        /// <param name="typist"></param>
        /// <param name="docNumber"></param>
        /// <param name="descrizione"></param>
        /// <param name="cartaceo"></param>
        /// <returns></returns>
        public bool InsertIntoVersions(string versionId, string subVersion, string versionLabel, string author, string typist, string docNumber, string descrizione, bool cartaceo)
        {
            //string sql = "INSERT INTO Versions (VERSION_ID, DOCNUMBER, VERSION, SUBVERSION, VERSION_LABEL, AUTHOR, TYPIST, DTA_CREAZIONE) VALUES (" + Functions.GetSystemIdNextVal(null) + docNumber +  ", " + versionId + ", '" + subVersion + "', '" + versionLabel + "'," + author + ", " + typist + ", " + Functions.GetDate() + ")";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_VERSIONS");

            string param1 = Functions.GetVersionIdColName() + " DOCNUMBER, VERSION, SUBVERSION, VERSION_LABEL, AUTHOR, TYPIST, DTA_CREAZIONE, COMMENTS, CARTACEO";
            string param2 = Functions.GetSystemIdNextVal(null) + docNumber + ", " + versionId + ", '" + subVersion + "', '" + versionLabel + "'," + author + ", " + typist + ", " + Functions.GetDate() + ", '" + descrizione.Replace("'", "''") + "'";
            if (cartaceo)
                param2 += ", 1";
            else
                param2 += ", 0";

            q.setParam("param1", param1);
            q.setParam("param2", param2);

            string sql = q.getSQL();
            logger.Debug(sql);
            return this.ExecuteNonQuery(sql);
        }

		/// <summary>
		/// Esegue una insert nella tabella VERSIONS, 
        /// con possibilità di inserire la descrizione
		/// </summary>
		/// <param name="versionId"></param>
		/// <param name="subVersion"></param>
		/// <param name="versionLabel"></param>
		/// <param name="author"></param>
		/// <param name="typist"></param>
		/// <param name="docNumber"></param>
		/// <returns></returns>
		public bool InsertIntoVersions(string versionId, string subVersion, string versionLabel, string author, string typist, string docNumber, string descrizione)
		{
            return this.InsertIntoVersions(versionId, subVersion, versionLabel, author, typist, docNumber, descrizione, false);
		}

		/// <summary>
		/// Esegue una delete su una tabella
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="version"></param>
		/// <param name="docNumber"></param>
		/// <returns></returns>
		public bool DeleteTable(string tableName, string versionId, string docNumber)
		{			   
			//string sql = "DELETE FROM " + tableName + " WHERE DOCNUMBER=" + docNumber + " AND VERSION_ID=" + versionId;
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DEFAULT");	
			q.setParam("param1", tableName);
			q.setParam("param2", "WHERE DOCNUMBER = " + docNumber + " AND VERSION_ID = " + versionId);					
			string sql = q.getSQL();
			logger.Debug(sql);
			return this.ExecuteNonQuery(sql);
		}
		#endregion

		#region query per la gestione del path del documentale

		public bool DOC_GetCorrByIdPeople(string idPeople,out System.Data.DataSet corrispondente)
		{			   
			//string sql = "DELETE FROM " + tableName + " WHERE DOCNUMBER=" + docNumber + " AND VERSION_ID=" + versionId;
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("DOC_GET_CORR_BY_ID_PEOPLE");	
			q.setParam("param1", idPeople);
			string sql = q.getSQL();
			logger.Debug(sql);
			return this.ExecuteQuery(out corrispondente, sql);
		}



		public string DOC_GetUoById(string idUp)
		{			   
			string result=null;
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("DOC_GET_UO_BY_ID");	
			q.setParam("param1", idUp);
			string sql = q.getSQL();
			logger.Debug(sql);
			if(!this.ExecuteScalar(out result,sql))
			{
				result=null;
			}
			return result;
		}

		public string DOC_GetIdUoBySystemId(string idRuolo)
		{			   
			string result=null;
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("DOC_GET_ID_UO_BY_SYSTEM_ID");	
			q.setParam("param1", idRuolo);
			string sql = q.getSQL();
			logger.Debug(sql);
			if(!this.ExecuteScalar(out result,sql))
			{
				result=null;
			}
			return result;
		}

		public bool DOC_GetDocByDocNumber(string docNumber,out System.Data.DataSet dataSet)
		{			   
			bool result=true; //presume successo
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("DOC_GET_DOC_BY_DOCNUMBER");	
			q.setParam("param1", docNumber);
			string sql = q.getSQL();
			logger.Debug(sql);
			if(!this.ExecuteQuery(out dataSet,"DOCUMENTO",sql))
			{
				result=false;
			}
			return result;
		}

		public string DOC_GetRegistroById(string idRegistro)
		{			   
			string result=null; 
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("DOC_GET_REGISTRO_BY_ID");	
			q.setParam("param1", idRegistro);
			string sql = q.getSQL();
			logger.Debug(sql);
			if(!this.ExecuteScalar(out result,sql))
			{
				result=null;
			}
			return result;
		}




		#endregion

		public  string getDKPath(string docNumber)
		{
			string result = null;
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("DK_PATH");					
				q.setParam("param1", docNumber);
				string sql = q.getSQL();
				logger.Debug(sql);
				System.Data.DataSet ds ;
				this.ExecuteQuery(out ds,sql);
				result =  ds.Tables[0].Rows[0][0].ToString();
				logger.Debug(result);
				return result ;
			}
			catch(Exception exception)
			{
				logger.Debug("Errore getDkPath.", exception);
				return result;
			}
		}

		public string getIdDK(string collId)
		{
			string result = null;
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("DK_ID");					
				q.setParam("param1", collId);
				string sql = q.getSQL();
				logger.Debug(sql);
				System.Data.DataSet ds ;
				this.ExecuteQuery(out ds,sql);
				result = ds.Tables[0].Rows[0][0].ToString();
				logger.Debug(result.ToString());
				return result ;

			}
			catch(Exception exception)
			{
				logger.Debug("Errore getIdDk.", exception);
				return result;
			}
		
		}

		/// <summary>
		/// Verifica se un determinato utente ha i permessi
		/// per accedere ad un determinato documento
		/// </summary>
		/// <param name="docNumber"></param>
		/// <param name="infoUtente"></param>
		/// <returns></returns>
		public bool DocumentoCheckUserVisibility(string docNumber,string idPeople,string idGroup)
		{
			bool retValue=false;

			DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("S_DOCUMENTO_CHECK_USER_VISIBILITY");					
			queryDef.setParam("docNumber",docNumber);
			queryDef.setParam("idPeople",idPeople);
			queryDef.setParam("idGroup",idGroup);

			string commandText=queryDef.getSQL();

			logger.Debug(commandText);

			string outParam=string.Empty;
			this.ExecuteScalar(out outParam,commandText);

			try
			{
				retValue=(Convert.ToBoolean(Convert.ToInt32(outParam) > 0));
			}
			catch
			{
			}

			return retValue;
		}

		public bool DocumentoIsAcquisito(string docnumber)
		{
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("CHECK_DOCUMENTO_ACQUISITO");					
				q.setParam("param1", docnumber);
				string sql = q.getSQL();
				logger.Debug(sql);
				string var_impronta;
				this.ExecuteScalar(out var_impronta, sql);
				if (var_impronta != null && var_impronta.Length > 0)
					return true;
				else
					return false;
			}
			catch(Exception e)
			{
				logger.Debug("Errore documentoIsAcquisito.", e);
				throw e;
			}
		}

		public bool DocumentoIsAcquisito(string docnumber, out string segnatura)
		{
			segnatura="";
			try
			{
				bool result = DocumentoIsAcquisito(docnumber);
				if (! result) return false;
			}
			catch(Exception e)
			{
				logger.Debug("Errore documentoIsAcquisito.", e);
				throw e;
			}

			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("CHECK_PROTO_ACQUISITO");					
				q.setParam("param1", docnumber);
				string sql = q.getSQL();
				logger.Debug(sql);
				this.ExecuteScalar(out segnatura, sql);
//				if ( segnatura.Length > 0)
//					return true;
//				else
//					return false;

				return true;
			}
			catch(Exception e)
			{
				logger.Debug("Errore documentoIsAcquisito.", e);
				throw e;
			}
		}

        /// <summary>
        /// Esegue la stored procedure per il calcolo della atipicità di un documento / fascicolo
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idDocFasc"></param>
        /// <param name="DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico"></param>
        /// <returns></returns>
        public DocsPaVO.Security.InfoAtipicita CalcolaAtipicita(DocsPaVO.utente.InfoUtente infoUtente, string idDocFasc, DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico tipo)
        {
            DocsPaVO.Security.InfoAtipicita infoAtipicita = null;
            try
            {
                logger.Debug("INIZIO transazione in CalcolaAtipicita");

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {

                    string valoreChiaveAtipicita = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "ATIPICITA_DOC_FASC");
                    if(string.IsNullOrEmpty(valoreChiaveAtipicita))
                        valoreChiaveAtipicita = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "ATIPICITA_DOC_FASC");

                    if (!string.IsNullOrEmpty(valoreChiaveAtipicita) && valoreChiaveAtipicita.Equals("1"))
                    {
                        ArrayList parameters = new ArrayList();
                        DocsPaUtils.Data.ParameterSP outParam;
                                
                        switch (tipo)
                        {
                            case DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.DOCUMENTO:
                                // Creazione parametri per la Store Procedure
                                outParam = new DocsPaUtils.Data.ParameterSP("codice_atipicita", new Int32(), 255, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.String);
                                parameters.Add(this.CreateParameter("id_amm", infoUtente.idAmministrazione));
                                parameters.Add(this.CreateParameter("doc_number", idDocFasc));
                                parameters.Add(outParam);

                                logger.Debug("INIZIO ESECUZIONE STORED - VIS_DOC_ANOMALA_DOC_NUMBER");
                                dbProvider.ExecuteStoredProcedure("VIS_DOC_ANOMALA_DOC_NUMBER", parameters, null);
                                logger.Debug("FINE ESECUZIONE STORED - VIS_DOC_ANOMALA_DOC_NUMBER");

                                if (!string.IsNullOrEmpty(outParam.Valore.ToString()))
                                {
                                    infoAtipicita = new DocsPaVO.Security.InfoAtipicita();
                                    infoAtipicita.TipoOggetto = DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.DOCUMENTO;
                                    infoAtipicita.IdDocFasc = idDocFasc;
                                    infoAtipicita.CodiceAtipicita = outParam.Valore.ToString();
                                }
                                break;

                            case DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.FASCICOLO:
                                // Creazione parametri per la Store Procedure
                                outParam = new DocsPaUtils.Data.ParameterSP("codice_atipicita", new Int32(), 255, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.String);
                                parameters.Add(this.CreateParameter("id_amm", infoUtente.idAmministrazione));
                                parameters.Add(this.CreateParameter("id_project", idDocFasc));
                                parameters.Add(outParam);

                                logger.Debug("INIZIO ESECUZIONE STORED - VIS_FASC_ANOMALA_ID_PROJECT");
                                dbProvider.ExecuteStoredProcedure("VIS_FASC_ANOMALA_ID_PROJECT", parameters, null);
                                logger.Debug("FINE ESECUZIONE STORED - VIS_FASC_ANOMALA_ID_PROJECT");

                                if (!string.IsNullOrEmpty(outParam.Valore.ToString()))
                                {
                                    infoAtipicita = new DocsPaVO.Security.InfoAtipicita();
                                    infoAtipicita.TipoOggetto = DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.FASCICOLO;
                                    infoAtipicita.IdDocFasc = idDocFasc;
                                    infoAtipicita.CodiceAtipicita = outParam.Valore.ToString();
                                }      
                                break;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                logger.Debug("ROLLBACK transazione in CalcolaAtipicita" + e.Message);
            }
            finally
            {
                logger.Debug("FINE transazione in CalcolaAtipicita");
            }

            return infoAtipicita;
        }

        /// <summary>
        /// Esegue la stored procedure per il calcolo della atipicità di un documento / fascicolo
        /// </summary>
        /// <param name="role"></param>
        public void CalcolaAtipicitaRuoliSottoposti(OrgRuolo role)
        {
            using (DBProvider dbProvider = new DBProvider())
            {
                // Esecuzione della store procedure per il calcolo delle atipicità nei documenti
                // e fascicoli dei sottoposti del ruolo
                ArrayList arguments = new ArrayList();
                arguments.Add(new ParameterSP("IdAmm", role.IDAmministrazione, DirectionParameter.ParamInput));
                arguments.Add(new ParameterSP("idUo", role.IDUo, DirectionParameter.ParamInput));

                dbProvider.ExecuteStoreProcedure("COMPUTEATIPICITAINSROLE", arguments);
            }
                
        }

        public static DocsPaVO.Security.InfoAtipicita GetInfoAtipicita(DataRow dataRow, DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico tipo)
        {
            DocsPaVO.Security.InfoAtipicita infoAtipicita = new DocsPaVO.Security.InfoAtipicita();

            switch (tipo)
            {
                case DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.DOCUMENTO :
                    if (dataRow.Table.Columns.Contains("CHA_COD_T_A") && dataRow.Table.Columns.Contains("DOCNUMBER"))
                    {
                        infoAtipicita.CodiceAtipicita = dataRow["CHA_COD_T_A"].ToString();
                        infoAtipicita.IdDocFasc = dataRow["DOCNUMBER"].ToString();
                        infoAtipicita.TipoOggetto = DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.DOCUMENTO;
                    }
                    break;

                case DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.FASCICOLO:
                    if (dataRow.Table.Columns.Contains("CHA_COD_T_A") && dataRow.Table.Columns.Contains("SYSTEM_ID"))
                    {
                        infoAtipicita.CodiceAtipicita = dataRow["CHA_COD_T_A"].ToString();
                        infoAtipicita.IdDocFasc = dataRow["SYSTEM_ID"].ToString();
                        infoAtipicita.TipoOggetto = DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.FASCICOLO;
                    }
                    break;
            }           

            return infoAtipicita;
        }

        public DocsPaVO.Security.InfoAtipicita GetInfoAtipicita(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico tipoOggetto, string idDocOrFasc)
        {
            DocsPaVO.Security.InfoAtipicita infoAtipicita = new DocsPaVO.Security.InfoAtipicita();
            DocsPaUtils.Query queryDef;
            string commandText = string.Empty;
            DataSet ds = null;

            try
            {
                if (infoUtente != null && !string.IsNullOrEmpty(idDocOrFasc))
                {
                    switch(tipoOggetto)
                    {
                        case DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.DOCUMENTO: 
                            queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("S_PROFILE0");					
			                queryDef.setParam("param1"," CHA_COD_T_A ") ;
                            queryDef.setParam("param2", " DOCNUMBER = " + idDocOrFasc);
			                commandText=queryDef.getSQL();
                            logger.Debug(commandText);
                            ds = new DataSet();
                            this.ExecuteQuery(ds, commandText);
                            if (ds.Tables[0].Rows.Count == 1)
                            {
                                infoAtipicita.CodiceAtipicita = ds.Tables[0].Rows[0]["CHA_COD_T_A"].ToString();
                                infoAtipicita.IdDocFasc = idDocOrFasc;
                                infoAtipicita.TipoOggetto = tipoOggetto;
                            }
                            break;
                        case DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.FASCICOLO:
                            queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("S_PROJECT0");					
			                queryDef.setParam("param1"," CHA_COD_T_A ") ;
                            queryDef.setParam("param2", " WHERE SYSTEM_ID = " + idDocOrFasc);
			                commandText=queryDef.getSQL();
                            logger.Debug(commandText);
                            ds = new DataSet();
                            this.ExecuteQuery(ds, commandText);
                            if (ds.Tables[0].Rows.Count == 1)
                            {
                                infoAtipicita.CodiceAtipicita = ds.Tables[0].Rows[0]["CHA_COD_T_A"].ToString();
                                infoAtipicita.IdDocFasc = idDocOrFasc;
                                infoAtipicita.TipoOggetto = tipoOggetto;
                            }
                            break;
                    }                    
			    }
            }
            catch (Exception e)
            {
                logger.Debug("Errore : GetInfoAtipicita " + e.Message);
            }

            return infoAtipicita;
        }

        public DocsPaVO.amministrazione.EsitoOperazione CopyVisibility(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Security.CopyVisibility copyVisibility)
        {
            DocsPaVO.amministrazione.EsitoOperazione esitoOperazione = new DocsPaVO.amministrazione.EsitoOperazione();

            string logBePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "BE_LOG_PATH");
            if(String.IsNullOrEmpty(logBePath))
                logBePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_LOG_PATH");
            logBePath = logBePath.Replace("%DATA", "");
            if (!System.IO.Directory.Exists(logBePath))
            {
                System.IO.Directory.CreateDirectory(logBePath);
            }
            DocsPaDB.Utils.SimpleLog sl = null;
            if (!String.IsNullOrEmpty(logBePath))
                sl = new DocsPaDB.Utils.SimpleLog(logBePath + "\\CopiaVisibilita_" + DateTime.Now.ToString("yyyyMMdd") + "_ROrigine_" + copyVisibility.codRuoloOrigine + "_RDestinazione_" + copyVisibility.codRuoloDestinazione);

            try
            {
                sl.Log(System.DateTime.Now + " - Inizio Copia Visibilità");
                int rows=0;
                string reportSuperiori = string.Empty;

                string deleteFromLog = " delete from dpa_copy_log ";
                this.ExecuteNonQuery(deleteFromLog, out rows);

                if(rows==0)
                    logger.Debug("Attenzione, non è stato possibile cancellare i log della DPA_COPY_LOG");

                //Documenti
                logger.Debug("Inizio copia visibilità documenti");
                sl.Log(System.DateTime.Now + " - Inizio copia visibilità documenti");
                DocsPaUtils.Query queryDocumenti = this.getQueryCopiaVis(copyVisibility, objTypeCopyVisiblity.DOCUMENTI, queryNameCopyVisibility.COPY_VISIBILITY, false);
                if (queryDocumenti != null && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloDestinazione) && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloOrigine))
                {
                    queryDocumenti.setParam("idGruppoRuoloDestinatario", copyVisibility.idGruppoRuoloDestinazione);
                    queryDocumenti.setParam("idGruppoRuoloOrigine", copyVisibility.idGruppoRuoloOrigine);
                    int rowsAffected = 0;
                    logger.Debug("query copia vis doc: " + queryDocumenti.getSQL());
                    bool rtn=this.ExecuteNonQuery(queryDocumenti.getSQL(), out rowsAffected);
                    esitoOperazione.numDocCopiati = rowsAffected / 2;
                }
                logger.Debug("Fine copia visibilità documenti");
                sl.Log(System.DateTime.Now + " - Fine copia visibilità documenti");

                //Fascicoli
                logger.Debug("Inizio copia visibilità fascicoli");
                sl.Log(System.DateTime.Now + " - Inizio copia visibilità fascicoli");
                DocsPaUtils.Query queryFascicoli = this.getQueryCopiaVisFasc(copyVisibility, objTypeCopyVisiblity.FASCICOLI, queryNameCopyVisibility.COPY_VISIBILITY_FASC, false);
                if (queryFascicoli != null && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloDestinazione) && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloOrigine))
                {
                    queryFascicoli.setParam("idGruppoRuoloDestinatario", copyVisibility.idGruppoRuoloDestinazione);
                    queryFascicoli.setParam("idGruppoRuoloOrigine", copyVisibility.idGruppoRuoloOrigine);
                    queryFascicoli.setParam("condsup", "");
                    int rowsAffected = 0;
                    logger.Debug("query copia vis fasc: "+ queryFascicoli.getSQL());
                    this.ExecuteNonQuery(queryFascicoli.getSQL(), out rowsAffected);
                    esitoOperazione.numeroFascCopiati = rowsAffected / 2;
                }
                logger.Debug("Fine copia visibilità fascicoli");

                logger.Debug("Inizio copia visibilità documenti in fascicolo");
                sl.Log(System.DateTime.Now + " - Inizio copia visibilità documenti in fascicolo");
                DocsPaUtils.Query queryDocInFasc = this.getQueryCopiaVisFasc(copyVisibility, objTypeCopyVisiblity.FASCICOLI, queryNameCopyVisibility.COPIA_DOC_IN_FASC_COPIA_VISIB, false);
                if (queryDocInFasc != null && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloDestinazione) && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloOrigine))
                {
                    queryDocInFasc.setParam("idGruppoRuoloDestinatario", copyVisibility.idGruppoRuoloDestinazione);
                    queryDocInFasc.setParam("idGruppoRuoloOrigine", copyVisibility.idGruppoRuoloOrigine);
                    queryDocInFasc.setParam("condsup", "");
                    int rowsAffected = 0;
                    logger.Debug("query copia vis fasc: " + queryDocInFasc.getSQL());
                    this.ExecuteNonQuery(queryDocInFasc.getSQL(), out rowsAffected);
                    esitoOperazione.numeroDocinFascCopiati = rowsAffected / 2;
                }
                logger.Debug("Fine copia visibilità documenti in fascicolo");

                sl.Log(System.DateTime.Now + " - Fine copia visibilità fascicoli");

                //Estendi visibilità ai ruoli superiori dei documenti / fascicoli
                if (copyVisibility.estendiVisibilita.ToUpper().Equals("SI") || copyVisibility.estendiVisibilita.ToUpper().Equals("NO_ATIPICI"))
                {
                    DataSet dsIdGruppoRuoliSuperiori = getIdGruppoRuoliSuperiori(copyVisibility);

                    //Documenti
                    logger.Debug("Inizio estensione visibilità documenti per copia visibilità");
                    sl.Log(System.DateTime.Now + " - Inizio estensione visibilità documenti per copia visibilità");
                    for (int i = 0; i < dsIdGruppoRuoliSuperiori.Tables[0].Rows.Count; i++)
                    {
                        string idGruppoSup = dsIdGruppoRuoliSuperiori.Tables[0].Rows[i]["ID_GRUPPO"].ToString();

                        queryDocumenti = this.getQueryCopiaVis(copyVisibility, objTypeCopyVisiblity.DOCUMENTI, queryNameCopyVisibility.COPY_VISIBILITY, true);
                        int rowsAffected = 0;
                        if (queryDocumenti != null && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloDestinazione) && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloOrigine))
                        {
                            queryDocumenti.setParam("idGruppoRuoloDestinatario", idGruppoSup);
                            queryDocumenti.setParam("idGruppoRuoloOrigine", copyVisibility.idGruppoRuoloOrigine);
                            logger.Debug("query copia vis doc al sup: "+idGruppoSup + " " + queryDocumenti.getSQL());
                            this.ExecuteNonQuery(queryDocumenti.getSQL(), out rowsAffected);
                        }
                        reportSuperiori += dsIdGruppoRuoliSuperiori.Tables[0].Rows[i]["VAR_CODICE"].ToString() + ":" + rowsAffected + "^";
                    }
                    logger.Debug("Fine estensione visibilità documenti per copia visibilità");
                    sl.Log(System.DateTime.Now + " - Fine estensione visibilità documenti per copia visibilità");
                    reportSuperiori += "/";

                    //Fascicoli
                    logger.Debug("Inizio estensione visiblità fascicoli per copia visibilità");
                    sl.Log(System.DateTime.Now + " - Inizio estensione visiblità fascicoli per copia visibilità");
                    for (int i = 0; i < dsIdGruppoRuoliSuperiori.Tables[0].Rows.Count; i++)
                    {
                        string idGruppoSup = dsIdGruppoRuoliSuperiori.Tables[0].Rows[i]["ID_GRUPPO"].ToString();
                        queryFascicoli = this.getQueryCopiaVisFasc(copyVisibility, objTypeCopyVisiblity.FASCICOLI, queryNameCopyVisibility.COPY_VISIBILITY_FASC, true);
                        int rowsAffected = 0;
                        if (queryFascicoli != null && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloDestinazione) && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloOrigine))
                        {
                            //devo inserire anche un controllo sul l'id gruppo destinatario, perchè potrebbe capitare che il gruppo destinatario non vede il titolario
                            //e quindi non acquisisce in copia la visibilità dei fascicoli, di conseguenza neppure i suoi superiori devono acquisire in copia gli stessi fasccioli
                            queryFascicoli.setParam("condsup", " AND  EXISTS (SELECT /*+index (s) */ 'x' FROM security where security.THING= P.SYSTEM_ID AND security.PERSONORGROUP =" + copyVisibility.idGruppoRuoloDestinazione + " ) ");
                            queryFascicoli.setParam("idGruppoRuoloDestinatario", idGruppoSup);
                            queryFascicoli.setParam("idGruppoRuoloOrigine", copyVisibility.idGruppoRuoloOrigine);
                            logger.Debug("query copia vis fasc al sup: " + idGruppoSup + " " + queryFascicoli.getSQL());
                            this.ExecuteNonQuery(queryFascicoli.getSQL(), out rowsAffected);
                        }
                        reportSuperiori += dsIdGruppoRuoliSuperiori.Tables[0].Rows[i]["VAR_CODICE"].ToString() + ":" + rowsAffected + "^";
                    }
                    logger.Debug("Fine estensione visibilità fascicoli per copia visibilità");
                    sl.Log(System.DateTime.Now + " - Fine estensione visibilità fascicoli per copia visiblità");
                    reportSuperiori += "/";

                    logger.Debug("Inizio copia visibilità documenti in fascicolo");
                    sl.Log(System.DateTime.Now + " - Inizio copia visibilità documenti in fascicolo");
                    for (int i = 0; i < dsIdGruppoRuoliSuperiori.Tables[0].Rows.Count; i++)
                    {
                        string idGruppoSup = dsIdGruppoRuoliSuperiori.Tables[0].Rows[i]["ID_GRUPPO"].ToString();
                        queryDocInFasc = this.getQueryCopiaVisFasc(copyVisibility, objTypeCopyVisiblity.FASCICOLI, queryNameCopyVisibility.COPIA_DOC_IN_FASC_COPIA_VISIB, true);
                        int rowsAffected = 0;
                        if (queryDocInFasc != null && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloDestinazione) && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloOrigine))
                        {
                            queryDocInFasc.setParam("condsup", " AND  EXISTS (SELECT /*+index (s) */ 'x' FROM security where security.THING = PG.LINK AND security.PERSONORGROUP =" + copyVisibility.idGruppoRuoloDestinazione + " ) ");
                            queryDocInFasc.setParam("idGruppoRuoloDestinatario", idGruppoSup);
                            queryDocInFasc.setParam("idGruppoRuoloOrigine", copyVisibility.idGruppoRuoloOrigine);
                            logger.Debug("query copia vis doc in  fasc al sup : " + idGruppoSup + " " + queryDocInFasc.getSQL());
                            this.ExecuteNonQuery(queryDocInFasc.getSQL(), out rowsAffected);
                        }
                        reportSuperiori += dsIdGruppoRuoliSuperiori.Tables[0].Rows[i]["VAR_CODICE"].ToString() + ":" + rowsAffected + "^";
                    }
                    logger.Debug("Fine estensione visiblità doc in fasc per copia visibilità");
                    sl.Log(System.DateTime.Now + " - Fine estensione visiblità doc in fasc per copia visiblità");
                }

                //Esecuzione Stored per atipicita, viene eseguita solo se non si è richiesto l'estensione della visibilità
                string valoreChiaveAtipicita = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "ATIPICITA_DOC_FASC");
                if (string.IsNullOrEmpty(valoreChiaveAtipicita))
                    valoreChiaveAtipicita = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "ATIPICITA_DOC_FASC");

                if (!string.IsNullOrEmpty(valoreChiaveAtipicita) && valoreChiaveAtipicita.Equals("1")/* && copyVisibility.estendiVisibilita.ToUpper().Equals("NO")*/)
                {
                    //Documenti
                    logger.Debug("Inizio calcolo atipicità documenti per copia visiblità");
                    sl.Log(System.DateTime.Now + " - Inizio calcolo atipicità documenti per copia visiblità");

                    DocsPaUtils.Query qDocumenti = this.getQueryCopiaVis(copyVisibility, DocsPaDB.Query_DocsPAWS.Documentale.objTypeCopyVisiblity.DOCUMENTI, DocsPaDB.Query_DocsPAWS.Documentale.queryNameCopyVisibility.GET_ID_DOC_OR_FASC_COPY_VIS, false);
                    if (qDocumenti != null && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloDestinazione) && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloOrigine))
                    {
                        qDocumenti.setParam("idGruppoRuoloOrigine", copyVisibility.idGruppoRuoloOrigine);
                        qDocumenti.setParam("idGruppoRuoloDestinatario", copyVisibility.idGruppoRuoloDestinazione);

                        ArrayList parameters = new ArrayList();
                        parameters.Add(this.CreateParameter("id_amm", infoUtente.idAmministrazione));
                        parameters.Add(this.CreateParameter("queryDoc", qDocumenti.getSQL()));
                        
                        this.ExecuteStoredProcedure("VIS_DOC_ANOMALA_CUSTOM", parameters, null);                        
                    }

                    logger.Debug("Fine calcolo atipicità documenti per copia visiblità");
                    sl.Log(System.DateTime.Now + " - Fine calcolo atipicità documenti per copia visiblità");

                    //Fascicoli
                    logger.Debug("Inizio calcolo atipicità fascicoli per copia visiblità");
                    sl.Log(System.DateTime.Now + " - Inizio calcolo atipicità fascicoli per copia visiblità");

                    DocsPaUtils.Query qFascicoli = this.getQueryCopiaVisFasc(copyVisibility, DocsPaDB.Query_DocsPAWS.Documentale.objTypeCopyVisiblity.FASCICOLI, DocsPaDB.Query_DocsPAWS.Documentale.queryNameCopyVisibility.CALCOLA_ATIP_COPIA, false);
                    if (qFascicoli != null && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloDestinazione) && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloOrigine))
                    {
                        qFascicoli.setParam("idGruppoRuoloOrigine", copyVisibility.idGruppoRuoloOrigine);
                        qFascicoli.setParam("idGruppoRuoloDestinatario", copyVisibility.idGruppoRuoloDestinazione);

                        ArrayList parameters = new ArrayList();
                        parameters.Add(this.CreateParameter("id_amm", infoUtente.idAmministrazione));
                        parameters.Add(this.CreateParameter("queryFasc", qFascicoli.getSQL()));

                        this.ExecuteStoredProcedure("VIS_FASC_ANOMALA_CUSTOM", parameters, null);
                    }

                    logger.Debug("Fine calcolo atipicità fascicoli per copia visiblità");
                    sl.Log(System.DateTime.Now + " - Fine calcolo atipicità fascicoli per copia visiblità");

                    // Documenti nel fascicolo
                    logger.Debug("Inizio calcolo atipicità documenti nel fascicolo per copia visiblità");
                    sl.Log(System.DateTime.Now + " - Inizio calcolo atipicità documenti nel fascicolo per copia visiblità");

                    DocsPaUtils.Query qDocInFasc = this.getQueryCopiaVisFasc(copyVisibility, objTypeCopyVisiblity.FASCICOLI, queryNameCopyVisibility.CALCOLA_ATIP_COPIA_DOC_IN_FASC, false);
                    if (qDocInFasc != null && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloDestinazione) && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloOrigine))
                    {
                        qDocInFasc.setParam("idGruppoRuoloOrigine", copyVisibility.idGruppoRuoloOrigine);
                        qDocInFasc.setParam("idGruppoRuoloDestinatario", copyVisibility.idGruppoRuoloDestinazione);

                        ArrayList parameters = new ArrayList();
                        parameters.Add(this.CreateParameter("id_amm", infoUtente.idAmministrazione));
                        parameters.Add(this.CreateParameter("queryFasc", qDocInFasc.getSQL()));

                        this.ExecuteStoredProcedure("VIS_FASC_ANOMALA_CUSTOM", parameters, null);
                    }

                    logger.Debug("Fine calcolo atipicità documenti nel fascicolo per copia visiblità");
                    sl.Log(System.DateTime.Now + " - Fine calcolo atipicità documenti nel fascicolo per copia visiblità");
                }

                sl.Log(System.DateTime.Now + " - Fine Copia Visibilità");
                sl.Log(System.DateTime.Now + " - Operazione avvenuta con successo");

                esitoOperazione.Codice = 0;
                esitoOperazione.Descrizione = "Operazione avvenuta con successo.";
                if (copyVisibility.estendiVisibilita.ToUpper().Equals("SI") || copyVisibility.estendiVisibilita.ToUpper().Equals("NO_ATIPICI"))
                    esitoOperazione.Descrizione += "*" + reportSuperiori;

                return esitoOperazione;
            }
            catch (Exception e)
            {
                logger.Debug("Errore : CopyVisibility " + e.Message);
                sl.Log(System.DateTime.Now + " - Errore : CopyVisibility " + e.Message);

                esitoOperazione.Codice = -1;
                esitoOperazione.Descrizione = "Problemi durante la copia della visibilità. Cosultare il file di log.";
                return esitoOperazione;
            }
        }

        public DocsPaUtils.Query getQueryCopiaVis(DocsPaVO.Security.CopyVisibility copyVisibility, objTypeCopyVisiblity type, queryNameCopyVisibility queryName, bool extendVisibility)
        {
            try
            {
                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery(queryName.ToString());
                bool insCondComuni = false;
                query.setParam("optmizerMode", " /*+ cost */ ");

                //Condizioni Documenti
                if (type == objTypeCopyVisiblity.DOCUMENTI && (copyVisibility.docNonProtocollati || copyVisibility.docProtocollati))
                {
                    query.setParam("objType", "profile");

                    //Documenti protocollati e non
                    if (copyVisibility.docNonProtocollati && copyVisibility.docProtocollati)
                        query.setParam("condizioneDocFasc", " AND profile.cha_tipo_proto in ('A','P','I','G') ");

                    //Documenti non protocollati
                    if (copyVisibility.docNonProtocollati && !copyVisibility.docProtocollati)
                        query.setParam("condizioneDocFasc", " AND profile.cha_tipo_proto in ('G') ");

                    //Documenti protocollati
                    if (!copyVisibility.docNonProtocollati && copyVisibility.docProtocollati)
                        query.setParam("condizioneDocFasc", " AND profile.cha_tipo_proto in ('A','P','I') ");

                    query.setParam("projectSelect", String.Empty);

                    //Condizione registri
                    query.setParam("condizioneIdRegistro", getCondizioneRegistri(copyVisibility, objTypeCopyVisiblity.DOCUMENTI));

                    //Condizione atipicita (presa in considerazione solo se si sta effettuando estensione della visibilità)
                    if (copyVisibility.estendiVisibilita.ToUpper().Equals("NO_ATIPICI") && extendVisibility)
                        query.setParam("condizioneAtipicita", " AND (profile.cha_cod_t_a is null or profile.cha_cod_t_a = 'T') ");
                    else
                        query.setParam("condizioneAtipicita", "");

                    insCondComuni = true;
                }

                //Condizioni Fascicoli
                if (type == objTypeCopyVisiblity.FASCICOLI && copyVisibility.fascicoliProcedimentali)
                {
                    query.setParam("objType", "project");
                    
                    //Fascicoli procedimentali

                    // Se si sta eseguendo il calcolo di atipicità bisogna considerare la condizione preesistente altrimenti no
                    if(queryName == queryNameCopyVisibility.GET_ID_DOC_OR_FASC_COPY_VIS)
                        query.setParam("condizioneDocFasc", " AND project.cha_tipo_proj = 'F' AND project.cha_tipo_fascicolo = 'P' ");
                    query.setParam("condizioneDocFasc", String.Empty);

                    // Condizione per selezione fascicoli e root folder dei fascicoli
                    query.setParam("projectSelect",  " And  " +
                        " exists SELECT 'x'  FROM project p  WHERE project.system_id=p.system_id and (p.cha_tipo_proj = 'F' OR cha_tipo_proj = 'C') " +
                        " AND NVL (p.cha_tipo_fascicolo, 'P') = 'P' @regCond@) ");

                    // condizione VISIBILITA' NODI TITOLARIO CONTENTENTI I FASCICOLI DEL RUOLO DESTINAZIONE.
                    query.setParam("codizioneNodiTit", "  and exists ( select 'x' from project T where t.system_id =project.id_parent and cha_tipo_proj ='T' and exists (select 'x' from security s where s.PERSONORGROUP in ( " + copyVisibility.idGruppoRuoloDestinazione + " ) AND accessrights > 20 and s.thing = T.SYSTEM_ID )) @regCond@ Union ");
                    //AND EXISTS ( SELECT 'x' FROM project T WHERE t.system_id =project.id_parent AND cha_tipo_proj ='T' AND EXISTS (SELECT 'x' FROM security s WHERE s.PERSONORGROUP IN ( 6649364                                          ) AND accessrights > 20 AND s.thing = T.SYSTEM_ID )) and (project.ID_REGISTRO in (86107) or project.ID_REGISTRO   is null ) UNION

                    //Condizione registri
                    query.setParam("regCond", getCondizioneRegistri(copyVisibility, objTypeCopyVisiblity.FASCICOLI));
                    query.setParam("condizioneIdRegistro", String.Empty);

                    //Condizione atipicita ( presa in considerazione solo se si sta facendo estensione visibilità)
                    if (copyVisibility.estendiVisibilita.ToUpper().Equals("NO_ATIPICI") && extendVisibility)
                        query.setParam("condizioneAtipicita", " AND (project.cha_cod_t_a is null or project.cha_cod_t_a = 'T') ");
                    else
                        query.setParam("condizioneAtipicita", "");

                    insCondComuni = true;
                }

                //Condizioni comuni
                //Verifichiamo se la variabile insCondComuni è a true, altrimenti è inutile processare le condizioni comuni,
                //in quanto non abbiamo impostato nessuna condizione nè per i documenti nè per i fascicoli 
                //ed in questo caso ritorniamo la query = null
                if (insCondComuni)
                {
                    //Condizione visibilità attiva e precedente copia di visibilità
                    if (copyVisibility.visibilitaAttiva && copyVisibility.precedenteCopiaVisibilita)
                    {
                        string cond = " AND " +
                                        "( " +
                                        "security.cha_tipo_diritto = 'P' " +
                                        "or " +
                                        "security.cha_tipo_diritto = 'T' ";

                        // Se si sta facendo copia visibilità dei fascicoli bisogna selezionare anche i diritti di tipo F
                        // ottenuti quando si copia la visibilità di un fascicolo ricevuto per trasmissione
                        if (type == objTypeCopyVisiblity.FASCICOLI && copyVisibility.fascicoliProcedimentali && queryName == queryNameCopyVisibility.COPY_VISIBILITY)
                            cond += " OR security.cha_tipo_diritto = 'F' ";

                        cond += "or " +
                                "(security.cha_tipo_diritto = 'A' and security.var_note_sec = 'ACQUISITO PER COPIA VISIBILITA') " +
                                ") ";
                        
                        query.setParam("codizioneVisibilita", cond);
                    }

                    //Condizione visibilità attiva senza precedente copia di visiblità
                    if (copyVisibility.visibilitaAttiva && !copyVisibility.precedenteCopiaVisibilita)
                    {
                        string cond = " AND " +
                                        "( " +
                                        "security.cha_tipo_diritto = 'P' " +
                                        "or " +
                                        "security.cha_tipo_diritto = 'T' ";

                        // Se si sta facendo copia visibilità dei fascicoli bisogna selezionare anche i diritti di tipo F
                        // ottenuti quando si copia la visibilità di un fascicolo ricevuto per trasmissione
                        if (type == objTypeCopyVisiblity.FASCICOLI && copyVisibility.fascicoliProcedimentali && queryName == queryNameCopyVisibility.COPY_VISIBILITY)
                            cond += " OR security.cha_tipo_diritto = 'F' ";

                        cond += ") ";

                        query.setParam("codizioneVisibilita", cond);
                    }

                    //Condizione visibilità non attiva e precedente copia di visiblità
                    if (!copyVisibility.visibilitaAttiva && copyVisibility.precedenteCopiaVisibilita)
                    {
                        string cond = " AND security.cha_tipo_diritto in ('P','T','A','F') ";
                        query.setParam("codizioneVisibilita", cond);
                    }

                    //Condizione visibilità non attiva senza precedente copia di visiblità
                    if (!copyVisibility.visibilitaAttiva && !copyVisibility.precedenteCopiaVisibilita)
                    {
                        string cond = String.Format(" AND " +
                                        "( " +
                                        "security.cha_tipo_diritto in('P','T','F') " +
                                        "or " +
                                        "(security.cha_tipo_diritto = 'A' and {0} <> 'ACQUISITO PER COPIA VISIBILITA') " +
                                        ") ", this.dbType == "ORACLE" ? "nvl(security.var_note_sec, 'NON ACQ')" : "ISNULL(security.var_note_sec, 'NON ACQ')");
                        query.setParam("codizioneVisibilita", cond);
                    }
                }
                else
                {
                    query = null;
                }

                return query;
            }
            catch (Exception ex)
            {
                logger.Debug("Errore : getQueryCopiaVis " + ex.Message);
                return null;
            }
        }

        public DataSet getIdGruppoRuoliSuperiori(DocsPaVO.Security.CopyVisibility copyVisibility)
        {
            DataSet dsIdGruppoRuoliSuperiori = new DataSet();

            try
            {
                DocsPaUtils.Query queryRuoliSuperiori = DocsPaUtils.InitQuery.getInstance().getQuery("GET_ID_GRUPPO_RUOLI_SUP");
                queryRuoliSuperiori.setParam("idAmm", copyVisibility.idAmm);
                queryRuoliSuperiori.setParam("idGruppoRuoloDestinatario", copyVisibility.idGruppoRuoloDestinazione);
                this.ExecuteQuery(dsIdGruppoRuoliSuperiori, queryRuoliSuperiori.getSQL());

                return dsIdGruppoRuoliSuperiori;
            }
            catch (Exception ex)
            {
                logger.Debug("Errore : getIdGruppoRuoliSuperiori " + ex.Message);
                return dsIdGruppoRuoliSuperiori;
            }
        }

        public DataSet getIdDocOrFascCopyVis(DocsPaVO.Security.CopyVisibility copyVisibility, objTypeCopyVisiblity type)
        {
            DataSet dsIdDocOrFasc = new DataSet();

            try
            {
                //Documenti
                if (type == objTypeCopyVisiblity.DOCUMENTI)
                {
                    DocsPaUtils.Query queryDocumenti = this.getQueryCopiaVis(copyVisibility, DocsPaDB.Query_DocsPAWS.Documentale.objTypeCopyVisiblity.DOCUMENTI, DocsPaDB.Query_DocsPAWS.Documentale.queryNameCopyVisibility.GET_ID_DOC_OR_FASC_COPY_VIS, false);
                    if (queryDocumenti != null && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloDestinazione) && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloOrigine))
                    {
                        queryDocumenti.setParam("idGruppoRuoloOrigine", copyVisibility.idGruppoRuoloOrigine);
                        queryDocumenti.setParam("idGruppoRuoloDestinatario", copyVisibility.idGruppoRuoloDestinazione);
                        DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                        dbProvider.ExecuteQuery(dsIdDocOrFasc, queryDocumenti.getSQL());
                    }
                }

                //Fascicoli
                if (type == objTypeCopyVisiblity.FASCICOLI)
                {
                    DocsPaUtils.Query queryFascicoli = this.getQueryCopiaVis(copyVisibility, DocsPaDB.Query_DocsPAWS.Documentale.objTypeCopyVisiblity.FASCICOLI, DocsPaDB.Query_DocsPAWS.Documentale.queryNameCopyVisibility.GET_ID_DOC_OR_FASC_COPY_VIS, false);
                    if (queryFascicoli != null && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloDestinazione) && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloOrigine))
                    {
                        queryFascicoli.setParam("idGruppoRuoloOrigine", copyVisibility.idGruppoRuoloOrigine);
                        queryFascicoli.setParam("idGruppoRuoloDestinatario", copyVisibility.idGruppoRuoloDestinazione);
                        DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                        dbProvider.ExecuteQuery(dsIdDocOrFasc, queryFascicoli.getSQL());
                    }
                }

                return dsIdDocOrFasc;
            }
            catch (Exception ex)
            {
                logger.Debug("Errore : getIdDocOrFascCopyVis " + ex.Message);
                return dsIdDocOrFasc;
            }
        }

        private string getCondizioneRegistri(DocsPaVO.Security.CopyVisibility copyVisibility, objTypeCopyVisiblity type)
        {
            string result = string.Empty;

            try
            {
                if (copyVisibility != null && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloOrigine) && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloDestinazione))
                {
                    Utenti ut = new Utenti();
                    DocsPaVO.utente.Ruolo ruoloOrigine = ut.GetRuoloByIdGruppo(copyVisibility.idGruppoRuoloOrigine);
                    DocsPaVO.utente.Ruolo ruoloDestinazione = ut.GetRuoloByIdGruppo(copyVisibility.idGruppoRuoloDestinazione);

                    ArrayList registriDaConsiderare = new ArrayList();

                    //Ricerco quali registri hanno in comune il ruolo destinazione e quello di origine, in modo da impostare la condizione solo su di essi
                    if (ruoloOrigine != null && ruoloDestinazione != null && ruoloOrigine.registri != null && ruoloDestinazione.registri != null)
                    {
                        foreach (DocsPaVO.utente.Registro registroOrigine in ruoloOrigine.registri)
                        {
                            foreach (DocsPaVO.utente.Registro registroDestinazione in ruoloDestinazione.registri)
                            {
                                if (registroDestinazione.codRegistro.Equals(registroOrigine.codRegistro))
                                    registriDaConsiderare.Add(registroDestinazione);
                            }
                        }
                    }

                    //Compongo la string per la condizione sugli id registro
                    string idRegistri = string.Empty;
                    foreach (DocsPaVO.utente.Registro registro in registriDaConsiderare)
                    {
                        idRegistri += registro.systemId + ",";
                    }

                    if (idRegistri.EndsWith(","))
                        idRegistri = idRegistri.Substring(0, idRegistri.Length - 1);

                    //Condizione su id registro e id registro null
                    if (!string.IsNullOrEmpty(idRegistri))
                    {
                        string condizioneRegistri = string.Empty;
                        if (type == objTypeCopyVisiblity.DOCUMENTI)
                            condizioneRegistri = " AND (profile.id_registro in(@idRegistri@) or profile.id_registro is null) ";
                        if (type == objTypeCopyVisiblity.FASCICOLI)
                            condizioneRegistri = " AND (project.id_registro in(@idRegistri@) or project.id_registro is null) ";

                        condizioneRegistri = condizioneRegistri.Replace("@idRegistri@", idRegistri);

                        result = condizioneRegistri;
                    }
                    //Condizione su id registro null
                    else
                    {
                        string condizioneRegistri = string.Empty;
                        if (type == objTypeCopyVisiblity.DOCUMENTI)
                            condizioneRegistri = " AND profile.id_registro is null ";
                        if (type == objTypeCopyVisiblity.FASCICOLI)
                            condizioneRegistri = " AND project.id_registro is null ";

                        result = condizioneRegistri;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.Debug("Errore : getCondizioneRegistri " + ex.Message);
                return null;
            }
        }

        public void insertInObjectsSyncPending(DocsPaVO.Security.CopyVisibility copyVisibility, objTypeCopyVisiblity type)
        {
            try
            {
                 //Documenti
                if (type == objTypeCopyVisiblity.DOCUMENTI)
                {
                    DocsPaUtils.Query queryDocumenti = this.getQueryCopiaVis(copyVisibility, DocsPaDB.Query_DocsPAWS.Documentale.objTypeCopyVisiblity.DOCUMENTI, DocsPaDB.Query_DocsPAWS.Documentale.queryNameCopyVisibility.I_DPA_OBJECTS_SYNC_PENDING, false);
                    if (queryDocumenti != null && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloDestinazione) && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloOrigine))
                    {
                        queryDocumenti.setParam("idGruppoRuoloDestinatario", copyVisibility.idGruppoRuoloDestinazione);
                        queryDocumenti.setParam("idGruppoRuoloOrigine", copyVisibility.idGruppoRuoloOrigine);
                        queryDocumenti.setParam("typeDocOrFasc", "D");
                        int rowsAffected = 0;
                        this.ExecuteNonQuery(queryDocumenti.getSQL(), out rowsAffected);
                    }
                }
                //Fascicoli
                if (type == objTypeCopyVisiblity.FASCICOLI)
                {
                    DocsPaUtils.Query queryFascicoli = this.getQueryCopiaVis(copyVisibility, DocsPaDB.Query_DocsPAWS.Documentale.objTypeCopyVisiblity.FASCICOLI, DocsPaDB.Query_DocsPAWS.Documentale.queryNameCopyVisibility.I_DPA_OBJECTS_SYNC_PENDING, false);
                    if (queryFascicoli != null && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloDestinazione) && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloOrigine))
                    {
                        queryFascicoli.setParam("idGruppoRuoloDestinatario", copyVisibility.idGruppoRuoloDestinazione);
                        queryFascicoli.setParam("idGruppoRuoloOrigine", copyVisibility.idGruppoRuoloOrigine);
                        queryFascicoli.setParam("typeDocOrFasc", "F");
                        int rowsAffected = 0;
                        this.ExecuteNonQuery(queryFascicoli.getSQL(), out rowsAffected);
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Debug("Errore : insertInObjectsSyncPending " + ex.Message);                
            }
        }

        public void removeObjSycPending(string idDocOrFasc)
        {
            try
            {
                string commandText = string.Format("DELETE FROM DPA_OBJECTS_SYNC_PENDING WHERE ID_DOC_OR_FASC = {0}", idDocOrFasc);
                this.ExecuteNonQuery(commandText);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore : removeObjSycPending " + ex.Message);
            }
        }

        public enum objTypeCopyVisiblity
        {
            DOCUMENTI,
            FASCICOLI
        }

        public enum queryNameCopyVisibility
        {
            COPY_VISIBILITY,
            GET_ID_FOR_COPY_VISIBLITY,
            GET_ID_DOC_OR_FASC_COPY_VIS,
            I_DPA_OBJECTS_SYNC_PENDING,
            COPY_VISIBILITY_FASC,
            CALCOLA_ATIP_COPIA,
            COPIA_DOC_IN_FASC_COPIA_VISIB,
            CALCOLA_ATIP_COPIA_DOC_IN_FASC
        }

        /// <summary>
        /// Metodo per l'estensione di visibilità ai ruoli superiori di un ruolo
        /// </summary>
        /// <param name="idAmm">Id dell'amministrazione</param>
        /// <param name="idGroup">Id del gruppo di cui estendere la visibilità</param>
        /// <param name="extendScope">Scope di estensione</param>
        /// <param name="copyIdToTempTable">True se bisogna copiare gli id id dei documenti e fascicoli in una tabella tamporanea per l'allineamento asincrono della visibilità</param>
        /// <returns>True se l'operazione è andata a buon fine</returns>
        public bool ExtendVisibilityToHigherRoles(
            String idAmm,
            String idGroup,
            DocsPaVO.amministrazione.SaveChangesToRoleRequest.ExtendVisibilityOption extendScope,
            bool copyIdToTempTable)
        {
            using (DBProvider dbProvider = new DBProvider())
            {
                ArrayList args = new ArrayList();
                args.Add(new ParameterSP("idAmm", idAmm, DirectionParameter.ParamInput));
                args.Add(new ParameterSP("idGroup", idGroup, DirectionParameter.ParamInput));
                args.Add(new ParameterSP("extendScope", extendScope.ToString(), DirectionParameter.ParamInput));
                args.Add(new ParameterSP("copyIdToTempTable", copyIdToTempTable ? "1" : "0", DirectionParameter.ParamInput));

                return dbProvider.ExecuteStoreProcedure("ExtendVisibilityToHigherRoles", args) == 0;

            }
        }

        public DocsPaUtils.Query getQueryCopiaVisFasc(DocsPaVO.Security.CopyVisibility copyVisibility, objTypeCopyVisiblity type, queryNameCopyVisibility queryName, bool extendVisibility)
        {
            try
            {
                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery(queryName.ToString());
                bool insCondComuni = false;
                query.setParam("optmizerMode", " /*+ cost */ ");

                //Condizioni Fascicoli
                if (type == objTypeCopyVisiblity.FASCICOLI && copyVisibility.fascicoliProcedimentali)
                {
                    //Fascicoli procedimentali
                    query.setParam("regCond", getCondizioneRegistriFasc(copyVisibility, objTypeCopyVisiblity.FASCICOLI));
                    
                    //Condizione atipicita ( presa in considerazione solo se si sta facendo estensione visibilità)
                    if (copyVisibility.estendiVisibilita.ToUpper().Equals("NO_ATIPICI") && extendVisibility)
                        // Se si stanno copiando i documenti nel fascicolo, la condizione è diversa
                        if(queryName == queryNameCopyVisibility.COPIA_DOC_IN_FASC_COPIA_VISIB)
                            query.setParam("condizioneAtipicita", " and ((Select nvl(profile.cha_cod_t_a, 'T') from profile where system_id = link) = 'T')");
                        else
                            query.setParam("condizioneAtipicita", " AND (p.cha_cod_t_a is null or p.cha_cod_t_a = 'T') ");
                    else
                        query.setParam("condizioneAtipicita", "");

                    insCondComuni = true;
                }

                //Condizioni comuni
                //Verifichiamo se la variabile insCondComuni è a true, altrimenti è inutile processare le condizioni comuni,
                //in quanto non abbiamo impostato nessuna condizione nè per i documenti nè per i fascicoli 
                //ed in questo caso ritorniamo la query = null
                if (insCondComuni)
                {
                    //Condizione visibilità attiva e precedente copia di visibilità
                    if (copyVisibility.visibilitaAttiva && copyVisibility.precedenteCopiaVisibilita)
                    {

                        string cond = " AND " +
                                        "( " +
                                        "s.cha_tipo_diritto = 'P' " +
                                        "or " +
                                        "s.cha_tipo_diritto = 'T' ";

                        // Se si sta facendo copia visibilità dei fascicoli bisogna selezionare anche i diritti di tipo F
                        // ottenuti quando si copia la visibilità di un fascicolo ricevuto per trasmissione
                        if (type == objTypeCopyVisiblity.FASCICOLI && copyVisibility.fascicoliProcedimentali && queryName == queryNameCopyVisibility.COPY_VISIBILITY_FASC)
                            cond += " OR s.cha_tipo_diritto = 'F' ";

                        cond += "or " +
                                "(s.cha_tipo_diritto = 'A' and s.var_note_sec = 'ACQUISITO PER COPIA VISIBILITA') " +
                                ") ";

                        query.setParam("codizioneVisibilita", cond);
                    }

                    //Condizione visibilità attiva senza precedente copia di visiblità
                    if (copyVisibility.visibilitaAttiva && !copyVisibility.precedenteCopiaVisibilita)
                    {
                        string cond = " AND " +
                                        "( " +
                                        "s.cha_tipo_diritto = 'P' " +
                                        "or " +
                                        "s.cha_tipo_diritto = 'T' ";

                        // Se si sta facendo copia visibilità dei fascicoli bisogna selezionare anche i diritti di tipo F
                        // ottenuti quando si copia la visibilità di un fascicolo ricevuto per trasmissione
                        if (type == objTypeCopyVisiblity.FASCICOLI && copyVisibility.fascicoliProcedimentali && queryName == queryNameCopyVisibility.COPY_VISIBILITY_FASC)
                            cond += " OR s.cha_tipo_diritto = 'F' ";

                        cond += ") ";

                        query.setParam("codizioneVisibilita", cond);
                    }

                    //Condizione visibilità non attiva e precedente copia di visiblità
                    if (!copyVisibility.visibilitaAttiva && copyVisibility.precedenteCopiaVisibilita)
                    {
                        string cond = " AND s.cha_tipo_diritto in ('P','T','A','F') ";
                        query.setParam("codizioneVisibilita", cond);
                    }

                    //Condizione visibilità non attiva senza precedente copia di visiblità
                    if (!copyVisibility.visibilitaAttiva && !copyVisibility.precedenteCopiaVisibilita)
                    {
                        string cond = String.Format(" AND " +
                                        "( " +
                                        "s.cha_tipo_diritto in('P','T','F') " +
                                        "or " +
                                        "(s.cha_tipo_diritto = 'A' and {0} <> 'ACQUISITO PER COPIA VISIBILITA') " +
                                        ") ", this.dbType == "ORACLE" ? "nvl(s.var_note_sec, 'NON ACQ')" : "ISNULL(s.var_note_sec, 'NON ACQ')");
                        query.setParam("codizioneVisibilita", cond);
                    }
                }
                else
                {
                    query = null;
                }

                return query;
            }
            catch (Exception ex)
            {
                logger.Debug("Errore : getQueryCopiaVisFasc " + ex.Message);
                return null;
            }
        }

        private string getCondizioneRegistriFasc(DocsPaVO.Security.CopyVisibility copyVisibility, objTypeCopyVisiblity type)
        {
            string result = string.Empty;

            try
            {
                if (copyVisibility != null && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloOrigine) && !string.IsNullOrEmpty(copyVisibility.idGruppoRuoloDestinazione))
                {
                    Utenti ut = new Utenti();
                    DocsPaVO.utente.Ruolo ruoloOrigine = ut.GetRuoloByIdGruppo(copyVisibility.idGruppoRuoloOrigine);
                    DocsPaVO.utente.Ruolo ruoloDestinazione = ut.GetRuoloByIdGruppo(copyVisibility.idGruppoRuoloDestinazione);

                    ArrayList registriDaConsiderare = new ArrayList();

                    //Ricerco quali registri hanno in comune il ruolo destinazione e quello di origine, in modo da impostare la condizione solo su di essi
                    if (ruoloOrigine != null && ruoloDestinazione != null && ruoloOrigine.registri != null && ruoloDestinazione.registri != null)
                    {
                        foreach (DocsPaVO.utente.Registro registroOrigine in ruoloOrigine.registri)
                        {
                            foreach (DocsPaVO.utente.Registro registroDestinazione in ruoloDestinazione.registri)
                            {
                                if (registroDestinazione.codRegistro.Equals(registroOrigine.codRegistro))
                                    registriDaConsiderare.Add(registroDestinazione);
                            }
                        }
                    }

                    //Compongo la string per la condizione sugli id registro
                    string idRegistri = string.Empty;
                    foreach (DocsPaVO.utente.Registro registro in registriDaConsiderare)
                    {
                        idRegistri += registro.systemId + ",";
                    }

                    if (idRegistri.EndsWith(","))
                        idRegistri = idRegistri.Substring(0, idRegistri.Length - 1);

                    //Condizione su id registro e id registro null
                    if (!string.IsNullOrEmpty(idRegistri))
                    {
                        string condizioneRegistri = string.Empty;
                        if (type == objTypeCopyVisiblity.FASCICOLI)
                            condizioneRegistri = " AND (p.id_registro in(@idRegistri@) or p.id_registro is null) ";

                        condizioneRegistri = condizioneRegistri.Replace("@idRegistri@", idRegistri);

                        result = condizioneRegistri;
                    }
                    //Condizione su id registro null
                    else
                    {
                        string condizioneRegistri = string.Empty;
                        if (type == objTypeCopyVisiblity.FASCICOLI)
                            condizioneRegistri = " AND p.id_registro is null ";

                        result = condizioneRegistri;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.Debug("Errore : getCondizioneRegistri " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Metodo per l'estensione di visibilità di un documento ai ruoli superiori per
        /// un dato ruolo
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="roleId"></param>
        /// <param name="docId"></param>
        internal void ExtendVisibilityByQuery(string idAmm, string roleId, string docId)
        {
            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("I_EXTEND_VISIBILITY");
                query.setParam("documentId", docId);
                query.setParam("idAmministrazione", idAmm);
                query.setParam("roleId", roleId);
                string querystring = query.getSQL();
                dbProvider.ExecuteNonQuery(querystring);
            }
        }
    }
}
