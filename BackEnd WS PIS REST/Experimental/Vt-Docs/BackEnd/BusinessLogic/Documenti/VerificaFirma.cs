//using System;
//using Debugger = DocsPaUtils.LogsManagement.Debugger;
//
//namespace BusinessLogic.Documenti
//{
//	/// <summary>
//	/// </summary>
//	public class VerificaFirma
//	{
//		/// <summary>
//		/// </summary>
//		/// <param name="base64content"></param>
//		/// <param name="fileReq"></param>
//		/// <param name="infoUtente"></param>
//		/// <param name="cofirma"></param>
//		/// <param name="addVersion"></param>
//		/// <returns></returns>
//		public static DocsPaVO.documento.FirmaResult verificaFirmaMethod(string base64content, DocsPaVO.documento.FileRequest fileReq, DocsPaVO.utente.InfoUtente infoUtente, bool cofirma, bool addVersion)
//		{
//			//DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
//			DocsPaVO.documento.FirmaResult firmaRes=new DocsPaVO.documento.FirmaResult();
////			try 
////			{
////				checkP7M(base64content, ref fileReq, cofirma);
////			} 
////			catch(Exception e) 
////			{
////				firmaRes.errore=e.Message;
////				return firmaRes;
////			}
//			try	
//			{
//				//si modifica il fileRequest
//				if(addVersion) 
//				{
//					DocsPaVO.documento.Applicazione app=new DocsPaVO.documento.Applicazione();
//					if(cofirma) 
//					{
//						app.estensione=getAppSuffix(fileReq.fileName);
//					}
//					else 
//					{
//						app.estensione=getAppSuffix(fileReq.fileName + ".P7M");
//					}
//				
//					logger.Debug("Estensione="+app.estensione);
//					fileReq.applicazione=app;
//					//logger.Debug("Applicazione non passata");
//					DocsPaVO.documento.FileDocumento fileDoc=new DocsPaVO.documento.FileDocumento();
//					fileDoc.content=Convert.FromBase64String(base64content);
//					fileDoc.length=fileDoc.content.Length;
//					if(cofirma) 
//					{
//						fileDoc.name=fileReq.fileName;
//					}
//					else 
//					{
//						fileDoc.name=fileReq.fileName+".P7M";
//					}
//					if(cofirma) 
//					{
//						fileDoc.estensioneFile=getAppSuffix(fileReq.fileName);
//					}
//					else 
//					{
//						fileDoc.estensioneFile=getAppSuffix(fileReq.fileName+".p7m");
//					}
//					logger.Debug("Prima:"+fileReq.dataInserimento);
//					//AGGIUNTO DA SAB il 28/02/2005
//					fileReq.versionId="";
//					BusinessLogic.Documenti.VersioniManager.addVersion(fileReq,infoUtente,true);
//					logger.Debug("Dopo:"+fileReq.dataInserimento);
//					BusinessLogic.Documenti.FileManager.putFile(fileReq,fileDoc,infoUtente);
//				}	
//				
//				//si inseriscono i dati nel DB
//				//updateFirmatari(fileReq);
//
//				firmaRes.fileRequest=fileReq;
//			} 
//			catch(Exception e)
//			{
//				logger.Debug(e.ToString());
//				logger.Debug("Errore nella gestione della verifica firma (verificaFirmaMethod)",e);
//				throw e;		
//			}
//
//			return firmaRes;
//		}
//
////		/// <summary>
////		/// </summary>
////		/// <param name="base64content"></param>
////		/// <param name="fileReq"></param>
////		/// <param name="cofirma"></param>
////		public static void checkP7M(string base64content, ref DocsPaVO.documento.FileRequest fileReq, bool cofirma) 
////		{
////			logger.Debug("checkP7M");
////			//si trova il numero di buste p7m concatenate
////			logger.Debug("FILENAME:"+fileReq.fileName);
////			int numP7M=getNumP7M(fileReq.fileName);
////			//nel caso di cofirma l'indice va abbassato
////			if(cofirma)
////			{
////				numP7M=numP7M-1;
////			}
////            logger.Debug("numero livelli: "+(numP7M+1));
////			
////			//ESAME DELLE BUSTE 
////			string contentInt=base64content;
////			System.Collections.ArrayList firmatariInt=new System.Collections.ArrayList();
////			
////			for(int j=0;j<numP7M+1;j++)
////			{
////				CAPICOM.SignedDataClass signedDataInt=new CAPICOM.SignedDataClass();
////				//si verifica la validità della firma
////				signedDataInt.Verify(contentInt,false,CAPICOM.CAPICOM_SIGNED_DATA_VERIFY_FLAG.CAPICOM_VERIFY_SIGNATURE_ONLY);
////				CAPICOM.ISigners signersInt=signedDataInt.Signers;
////				logger.Debug("livello "+(numP7M-j+1)+":"+signersInt.Count);
////				
////				for(int i=1;i<=signersInt.Count;i++) 
////				{
////					logger.Debug("qui "+i);
////					DocsPaVO.documento.Firmatario firmatarioInt=new DocsPaVO.documento.Firmatario();
////					firmatarioInt=getFirmatario(((CAPICOM.Signer) signersInt[i]).Certificate.SubjectName);
////					firmatarioInt.livello=numP7M-j+1;
////					logger.Debug("firm dati:"+firmatarioInt.codiceFiscale);
////					firmatariInt.Add(firmatarioInt);
////				}
////
////				//si passa alla busta interna
////				contentInt=signedDataInt.Content;
////			}
////
////			fileReq.firmatari=firmatariInt;
////		}
//
////		/// <summary>
////		/// </summary>
////		/// <param name="fileReq"></param>
////		public static void updateFirmatari(DocsPaVO.documento.FileRequest fileReq) 
////		{
////			#region Codice Commentato
////			/*	
////			DataSet ds=new DataSet();
////			bool openTrans=false;
////			try
////			{
////				string queryString="SELECT SYSTEM_ID, VAR_COD_FISCALE FROM DPA_FIRMATARI WHERE VAR_COD_FISCALE IN (";
////				for(int i=0;i<fileReq.firmatari.Count;i++)
////				{
////					queryString=queryString+"'"+((DocsPaVO.documento.Firmatario) fileReq.firmatari[i]).codiceFiscale+"'";
////					if(i<fileReq.firmatari.Count-1)
////					{
////						queryString=queryString+",";
////					}
////				}
////				queryString=queryString+")";
////				logger.Debug(queryString);
////				db.fillTable(queryString,ds,"FIRMATARIO");
////				db.beginTransaction();
////				openTrans=true;
////				for(int j=0;j<fileReq.firmatari.Count;j++) 
////				{
////					DocsPaVO.documento.Firmatario objFirm=(DocsPaVO.documento.Firmatario) fileReq.firmatari[j];
////					string firmId;
////					DataRow[] dr=ds.Tables["FIRMATARIO"].Select("VAR_COD_FISCALE='"+objFirm.codiceFiscale+"'");
////					if(dr.Length>0) 
////					{
////						firmId=dr[0]["SYSTEM_ID"].ToString();
////					}
////					else 
////					{
////						string insertFirmString="INSERT INTO DPA_FIRMATARI ("+DocsPaWS.Utils.dbControl.getSystemIdColName();
////						insertFirmString=insertFirmString+"VAR_NOME,VAR_COGNOME,VAR_COD_FISCALE,VAR_TITOLARE) VALUES (";
////						insertFirmString=insertFirmString+DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_FIRMATARI");
////						insertFirmString=insertFirmString+"'"+objFirm.nome+"',";
////						insertFirmString=insertFirmString+"'"+objFirm.cognome+"',";
////						insertFirmString=insertFirmString+"'"+objFirm.codiceFiscale+"',";
////						insertFirmString=insertFirmString+"'"+objFirm.identificativoCA+"'";
////						insertFirmString=insertFirmString+")";
////						logger.Debug(insertFirmString);
////						firmId=db.insertLocked(insertFirmString,"DPA_FIRMATARI");
////					}
////					
////					//si inserisce il firmatario nella tabella VERSIONI
////					string insertVersionString="INSERT INTO DPA_FIRMA_VERS ("+DocsPaWS.Utils.dbControl.getSystemIdColName();
////					insertVersionString=insertVersionString+"ID_FIRMATARIO,ID_VERSIONE,NUM_LIVELLO) VALUES (";
////					insertVersionString=insertVersionString+firmId+","+fileReq.versionId+","+objFirm.livello+")";
////					logger.Debug(insertVersionString);
////					db.insertLocked(insertVersionString,"DPA_FIRMA_VERS");
////				}
////				db.commitTransaction();
////			}
////			catch(Exception e)
////			{
////				if(openTrans)
////				{
////					db.rollbackTransaction();
////				}
////			  throw e;
////			}
////			*/
////			#endregion
////
////			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
////			if (!doc.UpdateFirmatari(fileReq))
////			{
////				throw new Exception();
////			}
////		}
//
////		/// <summary>
////		/// </summary>
////		/// <param name="subjectName"></param>
////		/// <returns></returns>
////		private static DocsPaVO.documento.Firmatario getFirmatario(string subjectName)
////		{
////		   DocsPaVO.documento.Firmatario firmatario=new DocsPaVO.documento.Firmatario();
////		   char[] sep={','};
////		   char[] sep2={'/'};
////		   char[] space={' '};
////		   char[] apici={'"'};
////		   string[] firmatarioSplit=subjectName.Split(sep);
////		   string descrizione=null;
////		   string cn=null;
////		   for(int i=0;i<firmatarioSplit.Length;i++)
////		   {
////				if(firmatarioSplit[i].TrimStart(space).ToUpper().StartsWith("DESCRIPTION"))
////				{
////                    descrizione=firmatarioSplit[i].TrimStart(space).Substring(12).Trim(apici);
////				}
////			}
////			for(int i=0;i<firmatarioSplit.Length;i++)
////			{
////				if(firmatarioSplit[i].TrimStart(space).ToUpper().StartsWith("CN"))
////				{
////					cn=firmatarioSplit[i].TrimStart(space).Substring(3);
////				}
////			}
////			if(descrizione==null || cn==null) 
////			{
////				logger.Debug("Il campo subjectName del certificato non è a norma di legge. SubjectName = '"+subjectName+"'. (getFirmatario)");
////				throw new Exception("Il campo subjectName del certificato non è a norma di legge. SubjectName="+subjectName);				
////			}
////			string[] cnSplit=cn.Split(sep2);
////			string[] descrizioneSplit=descrizione.Split(sep2);
////			if(cnSplit.Length<3) 
////			{
////				logger.Debug("Il campo cn non è a norma di legge. cn = "+cn+"'. (getFirmatario)");
////				throw new Exception("Il campo cn non è a norma di legge. cn="+cn);				
////			}
////			//campi tratti dal cn
////			firmatario.codiceFiscale=cnSplit[2];
////			firmatario.identificativoCA=cnSplit[3];
////            //campi tratti dalla description
////            firmatario.nome=getProperty(descrizioneSplit,"N");
////			firmatario.cognome=getProperty(descrizioneSplit,"C");
////			firmatario.dataNascita=getProperty(descrizioneSplit,"D");
////			if(firmatario.nome==null||firmatario.cognome==null||firmatario.dataNascita==null) 
////			{
////				logger.Debug("Il campo descrizione non contiene i campi obbligatori. Descrizione: "+descrizione+"'. (getFirmatario)");
////				throw new Exception("Il campo descrizione non contiene i campi obbligatori. Descrizione: "+descrizione);				
////			}
////			return firmatario;
////		}
////
////		/// <summary>
////		/// </summary>
////		/// <param name="description"></param>
////		/// <param name="nomeProp"></param>
////		/// <returns></returns>
////		private static string getProperty(string[] description,string nomeProp)
////		{
////			char[] space={' '};
////			for(int i=0;i<description.Length;i++)
////			{
////				if(description[i].Trim(space).StartsWith(nomeProp+"="))
////				{
////					return description[i].Trim(space).Substring(nomeProp.Length+1);
////				}
////			}
////		    return null;
////		}
//
////		/// <summary>
////		/// </summary>
////		/// <param name="filename"></param>
////		/// <returns></returns>
////		private static int getNumP7M(string filename)
////		{
////		   logger.Debug("getNumP7M");
////			char[] dot={'.'};
////            string[] parts=filename.Split(dot);
////			int res=0;
////            string suffix=parts[parts.Length-1];
////            while(suffix.ToUpper().Equals("P7M"))
////			{
////				res=res+1;
////			    suffix=parts[parts.Length-res-1];
////                logger.Debug(""+res);
////			}
////			return res;
////		}
//		 
//		/// <summary>
//		/// </summary>
//		/// <param name="filename"></param>
//		/// <returns></returns>
//		private static string getAppSuffix(string filename)
//		{
//			logger.Debug("getApp");
//			char[] dot={'.'};
//			string[] parts=filename.Split(dot);
//			string suffix=parts[parts.Length-1];
//			if(suffix.ToUpper().Equals("P7M"))
//			{
//				string res="";
//				int index=1;
//				while(suffix.ToUpper().Equals("P7M"))
//				{
//					index=index+1;
//					res=".P7M"+res;
//					suffix=parts[parts.Length-index];
//				}
//				res=suffix+res;
//				return res;
//			}
//			else
//			{
//				return suffix;
//			}
//		}
//	}
//}
