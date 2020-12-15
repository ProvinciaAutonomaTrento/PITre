using System;
using System.Data;
using System.Configuration;
using log4net;

namespace BusinessLogic.Interoperabilità.Fax
{
	/// <summary>
	/// </summary>
	public class FaxInvio
	{
        private static ILog logger = LogManager.GetLogger(typeof(FaxInvio));
		/// <summary>
		/// </summary>
		/// <param name="infoUtente"></param>
		/// <param name="reg"></param>
		/// <param name="schedaDoc"></param>
		/// <param name="debug"></param>
		public static void FaxInvioMethod(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Registro reg,DocsPaVO.documento.SchedaDocumento schedaDoc)
		{
			DataSet dataSet;
			
			try
			{
				#region Codice Commentato
				//db.openConnection();
				//si estraggono i campi dalla tabella dei registri
				/*
				string queryString="SELECT A.VAR_CODICE, B.VAR_CODICE_AMM FROM DPA_EL_REGISTRI A, DPA_AMMINISTRA B WHERE A.SYSTEM_ID="+reg.systemId+" AND B.SYSTEM_ID=A.ID_AMM";
				db.fillTable(queryString,dataSet,"REGISTRO");
				*/
				#endregion

				DocsPaDB.Query_DocsPAWS.Fax obj = new DocsPaDB.Query_DocsPAWS.Fax();
				obj.getDataReg(out dataSet,reg);

				if(dataSet.Tables["REGISTRO"].Rows.Count==0)
				{
					return;
				}

				DataRow regRow=dataSet.Tables["REGISTRO"].Rows[0];
				
				//si completa l'oggetto registro
				reg.codAmministrazione=regRow["VAR_CODICE_AMM"].ToString();

				string faxUserLogin = obj.getFaxUsrLogin(infoUtente.idCorrGlobali);

				if(faxUserLogin==null || faxUserLogin.Equals(""))
				{
//				    logger.addMessage("Utenza faxserver non trovata");
					logger.Debug("Utenza faxserver non trovata");

					return;
				} 

				ZfLib.ZfAPI api=new ZfLib.ZfAPI();
//				logger.addMessage("Tentativo di login con "+faxUserLogin);
				logger.Debug("Tentativo di login con "+faxUserLogin);

				ZfLib.UserSession session=api.Logon(faxUserLogin,false);

				// inserimento dei file in una cartella temporanei
				//string basePathFiles=ConfigurationManager.AppSettings["InteropFilePath"]+"\\Invio_files";
				//string pathFiles=basePathFiles+"\\"+regRow["VAR_CODICE"].ToString();
				string pathFiles=ConfigurationManager.AppSettings["faxTexp"];
				
				DocsPaUtils.Functions.Functions.CheckEsistenzaDirectory(pathFiles);

//				logger.addMessage("Estrazione dei file da inviare");
				logger.Debug("Estrazione dei file da inviare");

				estrazioneFiles(infoUtente,schedaDoc,pathFiles);
				
//				logger.addMessage("Estrazione dei destinatari");
				logger.Debug("Estrazione dei destinatari");

				//si uniscono i destinatari e i destinatari per conoscenza
				System.Collections.ArrayList dest=new System.Collections.ArrayList();
				
				for(int j=0;j<((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari.Count;j++)
				{   
					dest.Add(((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari[j]);
//					logger.addMessage("Aggiunto destinatario");
					logger.Debug("Aggiunto destinatario");
				}
				
				for(int j=0;j<((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Count;j++)
				{
					dest.Add(((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza[j]);
//					logger.addMessage("Aggiunto destinatario per conoscenza");
					logger.Debug("Aggiunto destinatario per conoscenza");
				}
				
//				logger.addMessage("Divisione dei destinatari");
				logger.Debug("Divisione dei destinatari");

				System.Collections.Hashtable destDivisi=dividiDestinatari(dest);
				System.Collections.IEnumerator keys=destDivisi.Keys.GetEnumerator();
				
				while(keys.MoveNext())
				{			
					//creazione ed invio mail
//					logger.addMessage("Creazione ed invio del fax al numero "+(string) keys.Current);
					logger.Debug("Creazione ed invio del fax al numero "+(string) keys.Current);

					creaFax(schedaDoc,session,(string)keys.Current,(System.Collections.ArrayList) destDivisi[keys.Current],pathFiles);
				}
				//cancellazione della directory
				DocsPaUtils.Functions.Functions.CancellaDirectory(pathFiles);
				//InteroperabilitaUtils.cancellaDirectory(basePathFiles);
			}
			catch(Exception e)
			{
				logger.Debug("Errore nella gestione dei fax (FaxInvioMethod)",e);
				throw e;				
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="infoUtente"></param>
		/// <param name="schedaDoc"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		private static bool estrazioneFiles(DocsPaVO.utente.InfoUtente infoUtente,DocsPaVO.documento.SchedaDocumento schedaDoc,string path)
		{
			System.IO.FileStream fs=null;
			System.IO.FileStream fsAll=null;
			
			try
			{
				//estrazione documento principale
				DocsPaVO.documento.Documento doc=getDocumentoPrincipale(schedaDoc);
				string docPrincipaleName="Documento_principale."+doc.fileName.Substring(doc.fileName.IndexOf(".")+1);
				fs=new System.IO.FileStream(path+"\\"+docPrincipaleName,System.IO.FileMode.Create);
				byte[] content=getDocument(infoUtente,doc.docNumber,doc.version,doc.versionId,doc.versionLabel);
				fs.Write(content,0,content.Length);
				fs.Close();
				
				//estrazione degli allegati
				for(int i=0;i<schedaDoc.allegati.Count;i++)
				{
					DocsPaVO.documento.Allegato all=(DocsPaVO.documento.Allegato) schedaDoc.allegati[i];
					string allegatoName="Allegato_"+i;
					fsAll=new System.IO.FileStream(path+"\\"+allegatoName+"."+getEstensione(all.fileName),System.IO.FileMode.Create);
					byte[] all_content=getDocument(infoUtente,all.docNumber,all.version,all.versionId,all.versionLabel);
					fsAll.Write(all_content,0,all_content.Length);
					fsAll.Close();
				}
				
				return true;
			} 
			catch(Exception e)
			{
//				logger.addMessage("Estrazione del file non eseguita.Eccezione: "+e.ToString());
				logger.Debug("Estrazione del file non eseguita.Eccezione: "+e.ToString());

				if(fs!=null) fs.Close();
				if(fsAll!=null) fsAll.Close();
				
				return false;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="infoUtente"></param>
		/// <param name="docNumber"></param>
		/// <param name="version"></param>
		/// <param name="versionId"></param>
		/// <param name="versionLabel"></param>
		/// <param name="logger"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		public static byte[] getDocument(DocsPaVO.utente.InfoUtente infoUtente, string docNumber, string version, string versionId, string versionLabel)
		{
			DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
			byte[] result = documentManager.GetFile(docNumber, version, versionId, versionLabel);

			if(result == null)
			{
				//TODO: gestire la throw
				throw new Exception();
			}

			return result;
		}

		/// <summary>
		/// </summary>
		/// <param name="schedaDoc"></param>
		/// <param name="session"></param>
		/// <param name="numDest"></param>
		/// <param name="destinatari"></param>
		/// <param name="pathFiles"></param>
		/// <param name="logger"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		private static bool creaFax(DocsPaVO.documento.SchedaDocumento schedaDoc, ZfLib.UserSession session, string numDest, System.Collections.ArrayList destinatari, string pathFiles)
		{
			try
			{
				ZfLib.NewMessage mess=session.CreateNewMsg();
//				logger.addMessage("Creazione del messaggio al numero "+numDest);
				logger.Debug("Creazione del messaggio al numero "+numDest);

				string bodyFax="Si trasmette come file allegato il documento e gli eventuali allegati.";
				bodyFax=bodyFax+"Registro: " + schedaDoc.registro.codRegistro+" ";
				bodyFax=bodyFax+"Numero di protocollo: " + schedaDoc.protocollo.numero+" ";
				bodyFax=bodyFax+"Data protocollazione: " + schedaDoc.protocollo.dataProtocollazione+" ";
				bodyFax=bodyFax+"Segnatura: " + schedaDoc.protocollo.segnatura;
				mess.Text=bodyFax;
				string[] files=System.IO.Directory.GetFiles(pathFiles);
				
				for(int i=0;i<files.Length;i++)
				{
					if(isValidFile(files[i]))
					{
						mess.Attachments.Add(files[i]);
//						logger.addMessage("Attachment "+files[i]+" inserito");
						logger.Debug("Attachment "+files[i]+" inserito");
					}
					else
					{
//					    logger.addMessage("Attachment "+files[i]+" non valido");
						logger.Debug("Attachment "+files[i]+" non valido");
					}
				};
				
//				logger.addMessage("Creazione stringa destinatari");
				logger.Debug("Creazione stringa destinatari");

				string dest="";
				
				for(int j=0;j<destinatari.Count;j++)
				{
				   dest=dest+((DocsPaVO.utente.Corrispondente)destinatari[j]).descrizione+" ";
				}

				mess.Recipients.AddFaxRecipient(dest,"",numDest);
//				logger.addMessage("Messaggio creato");
				logger.Debug("Messaggio creato");

				mess.Send();
//				logger.addMessage("Messaggio inviato");
				logger.Debug("Messaggio inviato");
				
				return true;
			}
			catch(Exception e)
			{
//				logger.addMessage("Creazione ed invio fax non eseguito. Eccezione: "+e.ToString());
				logger.Debug("Creazione ed invio fax non eseguito. Eccezione: "+e.ToString());
				
				return false;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="dest"></param>
		/// <param name="db"></param>
		/// <param name="debug"></param>
		/// <param name="logger"></param>
		/// <returns></returns>
		private static System.Collections.Hashtable dividiDestinatari(System.Collections.ArrayList dest)
		{
			System.Collections.Hashtable result=new System.Collections.Hashtable();
			
			for(int i=0;i<dest.Count;i++) 
			{
				try 
				{
					if (dest[i].GetType()!=typeof(DocsPaVO.utente.Corrispondente)) 
					{
						bool isFaxPrefCorr;
						string numCorr=null;

						isFaxPrefCorr=isFaxPreferred((DocsPaVO.utente.Corrispondente)dest[i]);
						
						if(isFaxPrefCorr) 
						{
							numCorr=getNumFaxCorr(((DocsPaVO.utente.Corrispondente)dest[i]).systemId);
//							logger.addMessage("Fax destinatario "+i+": "+numCorr);
							logger.Debug("Fax destinatario "+i+": "+numCorr);
						}
						else 
						{
//							logger.addMessage("Il fax non e' canale preferenziale del destinatario. Destinatario "+i+" scartato");
							logger.Debug("Il fax non e' canale preferenziale del destinatario. Destinatario "+i+" scartato");

							numCorr=null;
						}

						if(numCorr!=null && !numCorr.Equals("")) 
						{
							if(result.ContainsKey(numCorr)) 
							{
								((System.Collections.ArrayList)result[numCorr]).Add(dest[i]); 
							}
							else 
							{
								System.Collections.ArrayList al=new System.Collections.ArrayList();
								al.Add(dest[i]);
								result.Add(numCorr,al);
							}
						}
					}
				}
				catch (Exception e) 
				{
					logger.Debug(e.Message);
				}
			}

			return result;
		}

		/// <summary>
		/// </summary>
		/// <param name="corr"></param>
		/// <param name="db"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		private static bool isFaxPreferred(DocsPaVO.utente.Corrispondente corr)
		{
			//System.Data.DataSet ds=new System.Data.DataSet();
			System.Data.DataSet ds;

			try
			{
				DocsPaDB.Query_DocsPAWS.Fax obj = new DocsPaDB.Query_DocsPAWS.Fax();
				obj.getFax(out ds,corr);

				if(ds.Tables["CANALE"].Rows.Count==0)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			catch(Exception) 
			{
				return false;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="systemId"></param>
		/// <param name="db"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		private static string getNumFaxCorr(string systemId)
		{
			string res=null;
			
			try
			{
				/*
				string queryString="SELECT VAR_FAX FROM DPA_DETT_GLOBALI WHERE ID_CORR_GLOBALI="+systemId;
                res=(string) db.executeScalar(queryString).ToString();
				*/
				DocsPaDB.Query_DocsPAWS.Fax obj = new DocsPaDB.Query_DocsPAWS.Fax();
				res = obj.getNumFax(systemId/*,db*/);
			}
			catch(Exception e) 
			{
				logger.Debug(e.Message);
			}
			
			return res;
		
		}

		/// <summary>
		/// </summary>
		/// <param name="schedaDoc"></param>
		/// <returns></returns>
		private static DocsPaVO.documento.Documento getDocumentoPrincipale(DocsPaVO.documento.SchedaDocumento schedaDoc)
		{
			return (DocsPaVO.documento.Documento)schedaDoc.documenti[0];
		}

		/// <summary>
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		private static string getEstensione(string fileName)
		{
			char[] dot={'.'};
			string[] parts=fileName.Split(dot);
			string suffix=parts[parts.Length-1];
			
			if(suffix.ToUpper().Equals("P7M"))
			{
				suffix=fileName.Substring(fileName.IndexOf(".")+1);
			}
			
			return suffix;
		}

		/// <summary>
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		private static bool isValidFile(string fileName)
		{
			char[] sep={';'};
            string est=fileName.Substring(fileName.LastIndexOf(".")+1);
			string[] estensioni=ConfigurationManager.AppSettings["FaxEstensioni"].Split(sep);
			
			for(int i=0;i<estensioni.Length;i++)
			{
				if(est.Equals(estensioni[i].Trim())) 
				{
					return true;
				}
			}

			return false;
		}
	}
}
