using System;
using System.Xml;
using log4net;

namespace BusinessLogic.AmministrazioneXml
{
	/// <summary>
	/// Classe per la gestione delle tipologie di funzione e relative funzioni di DocsPA tramite XML
	/// </summary>
	public class FunzioniXml
	{
        private ILog logger = LogManager.GetLogger(typeof(FunzioniXml));
		private ErrorCode errorCode;
		private XmlDocument parser;

		
		/// <summary>
		/// Acquisisce uno stream XML
		/// </summary>
		/// <param name="xmlSource"></param>
		public FunzioniXml(string xmlSource)
		{
			errorCode = ErrorCode.NoError;

			try
			{
				// Validazione file XML
				parser = new XmlDocument();
				parser.LoadXml(xmlSource);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la validazione dell'XML", exception);
				errorCode = ErrorCode.BadXmlFile;
			}
		}

		/// <summary>
		/// </summary>
		public FunzioniXml()
		{
		}

		/// <summary>Ritorna l'ultimo codice di errore</summary>
		/// <returns></returns>
		public ErrorCode GetErrorCode()
		{
			return errorCode;
		}

		/// <summary>
		/// Cancella la struttura dei documenti esistenti
		/// </summary>
		/// <returns></returns>
		public bool DropAll()
		{
			bool result = true;
			
			try
			{
				if(errorCode == ErrorCode.NoError)
				{
					DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
					
					if(!amministrazioneXml.ClearFunzioni())
					{
						throw new Exception();
					}
					
					amministrazioneXml.Dispose();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione del titolario", exception);
				errorCode = ErrorCode.GenericError;
			}

			if(errorCode != ErrorCode.NoError)
			{
				result = false;	
			}

			return result;
		}

		
		
		/// <summary>
		/// Ritorna il testo contenuto in un dato tag XML
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		private string GetXmlField(string fieldName, XmlNode node, bool nulled)
		{
			string result = null;

			if(!nulled)
			{
				result = "";
			}

			XmlNode child = node.SelectSingleNode(fieldName);

			if(child != null)
			{
				if(child.InnerText != "")
				{
					result = child.InnerText;
				}
			}

			return result;
		}


		/// <summary>
		/// Crea la struttura documentale a partire dall'XML passato al costruttore
		/// </summary>
		/// <returns></returns>
		public bool CreateStructure(XmlNode node, string idAmm, bool dropAll)
		{
			bool result = true;
			errorCode = ErrorCode.NoError;
			
			try
			{
				if(errorCode == ErrorCode.NoError)
				{
					XmlAttribute attribute = node.Attributes["dropAll"];

					if(attribute != null)
					{
						dropAll |= Boolean.Parse(attribute.InnerText);
					}

					if(dropAll)
					{
						this.DropAll();
					}
						
					NavigateXml(node, idAmm);
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la creazione dell'XML", exception);
				errorCode = ErrorCode.GenericError;
			}

			if(errorCode != ErrorCode.NoError)
			{
				result = false;	
			}

			return result;
		}

		public bool UpdateStructure(XmlNode node)
		{
			bool result = true;
			errorCode = ErrorCode.NoError;
			
			try
			{
				if(errorCode == ErrorCode.NoError)
				{
					NavigateXmlForUpdate(node);
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura dell'XML", exception);
				errorCode = ErrorCode.GenericError;
			}

			if(errorCode != ErrorCode.NoError)
			{
				result = false;	
			}

			return result;
		}

		/// <summary>
		/// Procedura per la lettura dell'XML
		/// </summary>
		/// <param name="rootNode"></param>
		private void NavigateXml(XmlNode rootNode, string idAmm)
		{
			XmlNodeList nodiFunzione = rootNode.SelectNodes("TIPO");

			// Estrazione dati e nodi sottostanti
			foreach(XmlNode node in nodiFunzione)
			{
				// Leggi dati
				XmlNode dati = node.SelectSingleNode("DATI");
				if(dati == null)
				{
					logger.Debug("Errore corpo DATI tipo funzione");
					throw new Exception();
				}

				string codice	   = this.GetXmlField("CODICE", dati, false);
				string descrizione = this.GetXmlField("DESCRIZIONE", dati, false);

				codice=codice.ToUpper();

				DocsPaUtils.LogsManagement.ImportExportLogger.getInstance("import", 0).SetMessage(6, 10, "Import tipo funzione: " + codice);

				// Inserisci il tipo funzione
				DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();							
				string idTipoFunzione = amministrazioneXml.NewTipoFunzione(codice, descrizione);
				
				if(idTipoFunzione == null)
				{
					logger.Debug("Errore creazione tipo funzione");
					throw new Exception();
				}

				// Leggi funzioni
				XmlNode bloccoFunzioni = node.SelectSingleNode("FUNZIONI");
				if(bloccoFunzioni == null)
				{
					logger.Debug("Errore tag FUNZIONI tipo funzione");
					throw new Exception();
				}

				XmlNodeList funzioni = bloccoFunzioni.SelectNodes("FUNZIONE");

				foreach(XmlNode functionNode in funzioni)
				{
					descrizione = this.GetXmlField("DESCRIZIONE", functionNode, false);
					codice	   = this.GetXmlField("CODICE", functionNode, false);
					string accesso     = this.GetXmlField("ACCESSO", functionNode, true);

					codice=codice.ToUpper();

					DocsPaUtils.LogsManagement.ImportExportLogger.getInstance("import", 0).SetMessage(6, 10, "Import funzione: " + codice);

					if(accesso == null)
					{
						accesso = "Full";
					}

					// Inserisci la funzione
					string idFunzione = amministrazioneXml.NewFunzione(codice, descrizione, accesso, idTipoFunzione);
					if(idFunzione == null)
					{
						logger.Debug("Errore creazione funzione");
						throw new Exception();
					}
				}
			}
		}

		private void NavigateXmlForUpdate(XmlNode rootNode)
		{
			XmlNodeList nodiFunzione = rootNode.SelectNodes("TIPO");

			// Estrazione dati e nodi sottostanti
			foreach(XmlNode node in nodiFunzione)
			{
				string mode="";
				XmlAttribute attribute = node.Attributes["MODE"];
				if(attribute != null)
				{
					mode=attribute.InnerText.ToUpper() ;
				}

				// Leggi dati
				XmlNode dati = node.SelectSingleNode("DATI");
				if(dati == null)
				{
					logger.Debug("Errore corpo DATI tipo funzione");
					//logger.DebugAdm(true,"Errore corpo DATI tipo funzione",null);
					throw new Exception();
				}

				string codice	   = this.GetXmlField("CODICE", dati, false);
				string descrizione = this.GetXmlField("DESCRIZIONE", dati, false);
				descrizione=DocsPaUtils.Functions.Functions.ReplaceApexes(descrizione);
				codice=codice.ToUpper();

				DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();							

				if(mode=="MODIFIED" || mode=="CREATED")
				{
					string idTipoFunzione=null;

					if(mode=="MODIFIED")
					{
						// Modifica il tipo funzione
						idTipoFunzione=amministrazioneXml.UpdateTipoFunzione(codice, descrizione);
						if(idTipoFunzione==null)
						{
							logger.Debug("Errore aggiornamento tipo funzione");
							throw new Exception();
						}

						//cancellazione delle funzioni associate al tipo
						if(!amministrazioneXml.DeleteFunzioni(idTipoFunzione))
						{
							logger.Debug("Errore cancellazioni funzioni associate al tipo funzione");
							throw new Exception();
						}
					}
					if(mode=="CREATED")
					{
						// Inserisce il tipo funzione
						idTipoFunzione=amministrazioneXml.NewTipoFunzione(codice, descrizione);
						if(idTipoFunzione==null)
						{
							logger.Debug("Errore creazione tipo funzione");
							throw new Exception();
						}
					}

					// ricarica le funzioni
					XmlNode bloccoFunzioni = node.SelectSingleNode("FUNZIONI");
				
					if(bloccoFunzioni == null)
					{
						logger.Debug("Errore tag FUNZIONI tipo funzione");
						//logger.DebugAdm(true,"Errore tag FUNZIONI tipo funzione",null);
						throw new Exception();
					}

					XmlNodeList funzioni = bloccoFunzioni.SelectNodes("FUNZIONE");

					foreach(XmlNode functionNode in funzioni)
					{
						descrizione = this.GetXmlField("DESCRIZIONE", functionNode, false);
						codice	   = this.GetXmlField("CODICE", functionNode, false);
						string accesso     = this.GetXmlField("ACCESSO", functionNode, true);

						codice=codice.ToUpper();

						if(accesso == null)
						{
							accesso = "Full";
						}

						// Inserisci la funzione
						string idFunzione = amministrazioneXml.NewFunzione(codice, codice, accesso, idTipoFunzione);
						if(idFunzione == null)
						{
							logger.Debug("Errore creazione funzione");
							throw new Exception();
						}
					}
				}

				if(mode=="DELETED")
				{
					//recupera l'id del tipo funzione
					string idTipoFunzione=amministrazioneXml.GetTipoFunzione(codice);
					if(idTipoFunzione==null)
					{
						logger.Debug("Codice tipo funzione sconosciuto: " + codice);
						//logger.DebugAdm(true,"Codice tipo funzione sconosciuto: " + codice,null);
						throw new Exception();
					}
					//cancella il tipo funzione
					if(!amministrazioneXml.DeleteTipoFunzione(codice))
					{
						logger.Debug("Errore cancellazione tipo funzione");
						throw new Exception();
					}
					//cancellazione delle funzioni associate al tipo funzione
					if(!amministrazioneXml.DeleteFunzioni(idTipoFunzione))
					{
						logger.Debug("Errore cancellazioni funzioni");
						throw new Exception();
					}
				}
			}
		}

		public bool ExportStructure(XmlDocument doc,XmlNode root)
		{
			bool result=true; //presume successo
			try
			{
				DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();							
				System.Data.DataSet dataSetTipi;
				//amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();							
				result=amministrazioneXml.Exp_GetTipiFunzione(out dataSetTipi);
				if(!result)
				{
					logger.Debug("Errore lettura tipi funzioni");
					throw new Exception();
				}

				if(dataSetTipi != null)
				{
					XmlNode tipiFunzione=root.AppendChild (doc.CreateElement("TIPIFUNZIONE"));

					foreach( System.Data.DataRow rowTipo in dataSetTipi.Tables["TIPIFUNZIONE"].Rows)
					{

						string idTipoFunzione=rowTipo["SYSTEM_ID"].ToString ();

						XmlNode tipo=tipiFunzione.AppendChild (doc.CreateElement("TIPO"));
						XmlNode datiTipo=tipo.AppendChild (doc.CreateElement("DATI"));
						datiTipo.AppendChild(doc.CreateElement("CODICE")).InnerText = rowTipo["VAR_COD_TIPO"].ToString ().ToUpper();
						datiTipo.AppendChild(doc.CreateElement("DESCRIZIONE")).InnerText = rowTipo["VAR_DESC_TIPO_FUN"].ToString ();

						//esportazione funzioni
						System.Data.DataSet dataSetFunzioni;
						result=amministrazioneXml.Exp_GetFunzioni(out dataSetFunzioni,idTipoFunzione);
						if(!result)
						{
							logger.Debug("Errore lettura funzioni");
							throw new Exception();
						}

						if(dataSetFunzioni != null)
						{
							XmlNode funzioni=tipo.AppendChild (doc.CreateElement("FUNZIONI"));
							foreach( System.Data.DataRow rowFunzione in dataSetFunzioni.Tables["FUNZIONI"].Rows)
							{
								XmlNode funzione=funzioni.AppendChild (doc.CreateElement("FUNZIONE"));
								funzione.AppendChild(doc.CreateElement("DESCRIZIONE")).InnerText = rowFunzione["VAR_DESC_FUNZIONE"].ToString ();
								funzione.AppendChild(doc.CreateElement("CODICE")).InnerText = rowFunzione["COD_FUNZIONE"].ToString ().ToUpper();
								string tipoFunz=rowFunzione["ID_TIPO_FUNZIONE"].ToString ();
								string tipoFunzFull="Full";
								if(tipoFunz=="R")
								{
									tipoFunzFull="Read";
								}
								if(tipoFunz=="W")
								{
									tipoFunzFull="Write";
								}
								funzione.AppendChild(doc.CreateElement("ACCESSO")).InnerText = tipoFunzFull;
							}
						}

					}
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'esportazione delle funzioni", exception);
				result=false;
			}
			return result;
		}


		public bool ExportAnagraficaFunzioni(XmlDocument doc,XmlNode root)
		{
			bool result=true; //presume successo
			try
			{
				System.Data.DataSet dataSetFunzioniElementari;
				DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();							
				result=amministrazioneXml.Exp_GetFunzioniElementari(out dataSetFunzioniElementari);
				if(!result)
				{
					logger.Debug("Errore lettura anagrafica funzioni elementari");
					throw new Exception();
				}

				if(dataSetFunzioniElementari != null)
				{
					XmlNode funzioniElementari=root.AppendChild (doc.CreateElement("FUNZIONIELEMENTARI"));
					foreach( System.Data.DataRow rowFunzioni in dataSetFunzioniElementari.Tables["FUNZIONIELEMENTARI"].Rows)
					{
						XmlNode funzioneElementare=funzioniElementari.AppendChild (doc.CreateElement("FUNZIONEELEMENTARE"));
						funzioneElementare.AppendChild(doc.CreateElement("DESCRIZIONE")).InnerText = rowFunzioni["VAR_DESC_FUNZIONE"].ToString ();
						funzioneElementare.AppendChild(doc.CreateElement("CODICE")).InnerText = rowFunzioni["COD_FUNZIONE"].ToString ().ToUpper();
						string tipoFunzFull="Full";
						string tipoFunz=rowFunzioni["CHA_TIPO_FUNZ"].ToString ();
						if(tipoFunz=="R")
						{
							tipoFunzFull="Read";
						}
						if(tipoFunz=="W")
						{
							tipoFunzFull="Write";
						}
						funzioneElementare.AppendChild(doc.CreateElement("ACCESSO")).InnerText = tipoFunzFull;
					}
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'esportazione della anagrafica funzioni elementari", exception);
				result=false;
			}
			return result;
		}
	}
}
