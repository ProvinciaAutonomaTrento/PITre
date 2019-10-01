using System;
using System.Data;
using log4net;

namespace BusinessLogic.Interoperabilità.Fax
{
	/// <summary>
	/// </summary>
	public class FaxManager
	{
        private static ILog logger = LogManager.GetLogger(typeof(FaxManager));
		/// <summary>
		/// 
		/// </summary>
		/// <param name="serverName"></param>
		/// <param name="infoUtente"></param>
		/// <param name="ruolo"></param>
		/// <param name="registro"></param>
		/// <returns></returns>
		public static int processaCaselleFaxUO(string serverName,DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.Registro registro)
		{
		   logger.Debug("processaCaselleFaxUO");
		   System.Data.DataSet dataSet;

		   bool dbOpen=false;
			int rtnNumMess=0;
			try
			{
				// TODO: Utilizzare il progetto DocsPaDbManagement
				//database.openConnection();
				dbOpen=true;

				#region Codice Commentato
				/*
				string queryString="SELECT VAR_FAX_USER_LOGIN, VAR_COD_RUBRICA FROM DPA_CORR_GLOBALI WHERE ";
				queryString=queryString+" ID_PEOPLE IN (SELECT PEOPLE_SYSTEM_ID FROM PEOPLEGROUPS B, DPA_CORR_GLOBALI A WHERE A.ID_GRUPPO=B.GROUPS_SYSTEM_ID AND A.ID_UO="+ruolo.uo.systemId+")";	
				logger.Debug(queryString);
		        
				// TODO: Utilizzare il progetto DocsPaDbManagement
				database.fillTable(queryString,dataSet,"UTENTI_FAX");
				*/
				#endregion

				DocsPaDB.Query_DocsPAWS.Fax obj = new DocsPaDB.Query_DocsPAWS.Fax();
				obj.processFaxUO(out dataSet,ruolo);

				for(int i=0;i<dataSet.Tables["UTENTI_FAX"].Rows.Count;i++){
				    System.Data.DataRow utenteRow=dataSet.Tables["UTENTI_FAX"].Rows[i];
					if(utenteRow["VAR_FAX_USER_LOGIN"].ToString()!=null && !utenteRow["VAR_FAX_USER_LOGIN"].ToString().Equals(""))
					{
						logger.Debug("Processa casella fax dell'utente "+utenteRow["VAR_FAX_USER_LOGIN"].ToString());
						rtnNumMess+=processaCasellaFax(serverName,infoUtente,ruolo,registro,utenteRow["VAR_FAX_USER_LOGIN"].ToString(),utenteRow["VAR_COD_RUBRICA"].ToString());
					}
				}
				// TODO: Utilizzare il progetto DocsPaDbManagement
				//database.closeConnection();
				dbOpen=false;
		        return rtnNumMess;
			}
			catch(Exception e){
				logger.Debug(e.Message);
				if(dbOpen){
					// TODO: Utilizzare il progetto DocsPaDbManagement
					//database.closeConnection();
				}
				logger.Debug("Errore nella gestione dei fax (processaCasellaFaxUO)",e);
				throw new Exception("F_System");				
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serverName"></param>
		/// <param name="infoUtente"></param>
		/// <param name="ruolo"></param>
		/// <param name="registro"></param>
		/// <param name="faxUserLogin"></param>
		/// <param name="faxUserCodRubrica"></param>
		/// <returns></returns>
		public static int processaCasellaFax(string serverName,DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.Registro registro, string faxUserLogin, string faxUserCodRubrica)
		{
			logger.Debug("processaCasellaFax");
			int rtn=0;
			try
			{
				//si ricava il destinatario della trasmissione
                DocsPaVO.addressbook.QueryCorrispondente qco=new DocsPaVO.addressbook.QueryCorrispondente();
                System.Collections.ArrayList reg=new System.Collections.ArrayList();
				reg.Add(registro.systemId);
				qco.codiceRubrica=faxUserCodRubrica;
				qco.idAmministrazione=registro.idAmministrazione;
				qco.idRegistri=reg;
				qco.getChildren=false;
				System.Collections.ArrayList corr=BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qco);
				DocsPaVO.utente.Corrispondente destinatario=(DocsPaVO.utente.Corrispondente) corr[0];


				//ACCESSO AL FAX
				ZfLib.ZfAPI api=new ZfLib.ZfAPI();			
				logger.Debug("Tentativo di login con "+faxUserLogin);
				ZfLib.UserSession session=api.Logon(faxUserLogin,false);
				logger.Debug("Login effettuata al server ZFAX con utenza "+faxUserLogin);
				ZfLib.ServerInfo sinfo=session.Server.GetServerInfo();
				bool msgS=session.Inbox.CheckNewMsgStatus();
											  
				logger.Debug("Messaggi trovati bool: "+msgS);
				logger.Debug("Messaggi trovati: "+session.Inbox.GetMsgList().Count);
				string path=session.UserInDir;
				rtn=session.Inbox.GetMsgList().Count;
				for(int i=0;i<session.Inbox.GetMsgList().Count;i++)
				{
					logger.Debug("Esame messaggio "+i);
					logger.Debug("Esame messaggio "+path);
					logger.Debug("Esame messaggio "+destinatario.descrizione);
					ZfLib.Message mess=session.Inbox.GetMsgList()[i];
                    processaMessaggio(serverName,path,mess,infoUtente,ruolo,registro,destinatario);
					//cancellazione messaggio
					logger.Debug("Esame messaggio inizia cancellazione "+i);
					mess.DeleteMsg(false);
					logger.Debug("Esame messaggio finita cancellazione "+i);
				}             
			return rtn;
			}
			catch(Exception e)
			{
			   logger.Debug("Errore nella gestione dei fax (processaCasellaFax)",e);
               throw e;			   
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serverName"></param>
		/// <param name="path"></param>
		/// <param name="messaggio"></param>
		/// <param name="infoUtente"></param>
		/// <param name="ruolo"></param>
		/// <param name="registro"></param>
		/// <param name="destinatario"></param>
		private static void processaMessaggio(string serverName,string path,ZfLib.Message messaggio,DocsPaVO.utente.InfoUtente infoUtente,DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.Registro registro, DocsPaVO.utente.Corrispondente destinatario)
		{
			logger.Debug("processaMessaggio");
			System.IO.FileStream fs=null;
			bool daAggiornareUffRef = false;
			bool dbOpen=false;
			bool fsOpen=false;
			
			try
			{
                //informazioni sul fax
				string dataRic=messaggio.GetMsgHistories()[0].Date.ToString();
				logger.Debug(" @ dataric = "+dataRic);

			    //accesso al file del fax
				fs = getFileStream(path+messaggio.GetMsgInfo().Body);//=new System.IO.FileStream(path+messaggio.GetMsgInfo().Body+".G3N",System.IO.FileMode.Open,System.IO.FileAccess.ReadWrite,System.IO.FileShare.ReadWrite);
				
				logger.Debug(fs.CanRead.ToString()+"@ OK @");
				fsOpen=true;

				//creazione scheda documento
				DocsPaVO.documento.SchedaDocumento sd=new DocsPaVO.documento.SchedaDocumento();
				sd.idPeople=infoUtente.idPeople;
				
				logger.Debug("sd.idPeople = "+sd.idPeople.ToString());

				sd.userId = infoUtente.userId;
				logger.Debug("sd.userId = "+sd.userId.ToString());
				DocsPaVO.documento.Oggetto ogg=new DocsPaVO.documento.Oggetto();
				ogg.descrizione="Fax ricevuto in data "+dataRic;  //DA COMPLETARE
				sd.oggetto=ogg; 
				sd.predisponiProtocollazione=true;
				sd.registro=registro;
				sd.tipoProto="A";
				sd.typeId="MAIL";

				//aggiunta protocollo entrata
				DocsPaVO.documento.ProtocolloEntrata protEntr=new DocsPaVO.documento.ProtocolloEntrata();
				DocsPaVO.utente.Corrispondente mittente=new DocsPaVO.utente.Corrispondente();
				mittente.descrizione=messaggio.GetMsgInfo().Comment;
				logger.Debug( " @ mittente.descrizione = "+mittente.descrizione);//"QUI SI METTONO INFORMAZIONI";
				protEntr.mittente=mittente;
				
				sd.protocollo=protEntr;
				protEntr.dataProtocollazione=System.DateTime.Today.ToString();
				sd.appId="ACROBAT";
				sd=BusinessLogic.Documenti.DocSave.addDocGrigia(sd,infoUtente,ruolo);
				logger.Debug("Salvataggio doc...");
				sd=BusinessLogic.Documenti.DocSave.save(infoUtente, sd,false,out daAggiornareUffRef, ruolo);
				logger.Debug("Salvataggio eseguito");
				DocsPaVO.documento.FileDocumento fd=new DocsPaVO.documento.FileDocumento();
				byte[] buffer=new byte[fs.Length];
				fs.Read(buffer,0,(int)fs.Length);
				fd.content=buffer;
				fd.length=buffer.Length;
				fd.name="fax.tif";

				BusinessLogic.Documenti.FileManager.putFile((DocsPaVO.documento.FileRequest)sd.documenti[0],fd,infoUtente);
				logger.Debug("Documento inserito");
				fs.Close();
				fsOpen=false;
                
                //TRASMISSIONE
				DocsPaVO.trasmissione.Trasmissione trasm=new DocsPaVO.trasmissione.Trasmissione();
				trasm.ruolo=ruolo;
				//db.openConnection();
				dbOpen=true;
				trasm.utente = BusinessLogic.Utenti.UserManager.getUtente(infoUtente.idPeople);
				DocsPaVO.documento.InfoDocumento infoDoc=new DocsPaVO.documento.InfoDocumento();
				infoDoc.idProfile=sd.systemId;
				infoDoc.docNumber=sd.docNumber;
				infoDoc.oggetto=sd.oggetto.descrizione;
				infoDoc.tipoProto="A";
				trasm.infoDocumento=infoDoc;
				//costruzione singole trasmissioni
				DocsPaVO.trasmissione.RagioneTrasmissione ragione=getRagioneTrasm();
				System.Collections.ArrayList dest=new System.Collections.ArrayList();
				dest.Add(destinatario);
				System.Collections.ArrayList trasmissioniSing=new System.Collections.ArrayList();

				for(int i=0;i<dest.Count;i++)
				{
					DocsPaVO.trasmissione.TrasmissioneSingola trSing=new DocsPaVO.trasmissione.TrasmissioneSingola();
					trSing.ragione=ragione;
					logger.Debug(dest[i].GetType().ToString());
					if(dest[i].GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
					{
						logger.Debug("ruolo");
						trSing.corrispondenteInterno=(DocsPaVO.utente.Ruolo) dest[i];
					}
					else 
					{
						logger.Debug("utente");
						trSing.corrispondenteInterno=(DocsPaVO.utente.Utente) dest[i];
					}
					logger.Debug("ok");
					trSing.tipoTrasm="S";
					if(dest[i].GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
					{
						logger.Debug("caso ruolo");
						trSing.tipoDest=DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
						//ricerca degli utenti del ruolo
						System.Collections.ArrayList utenti=new System.Collections.ArrayList();
						DocsPaVO.addressbook.QueryCorrispondente qc=new DocsPaVO.addressbook.QueryCorrispondente();
						qc.codiceRubrica=((DocsPaVO.utente.Ruolo) dest[i]).codiceRubrica;
						System.Collections.ArrayList registri=new System.Collections.ArrayList();
						registri.Add(registro.systemId);
						qc.idRegistri=registri;
						qc.idAmministrazione=registro.idAmministrazione;
						qc.getChildren=true;
						utenti=BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qc);
						System.Collections.ArrayList trasmissioniUt=new System.Collections.ArrayList();
						for(int k=0;k<utenti.Count;k++)
						{
							DocsPaVO.trasmissione.TrasmissioneUtente trUt=new DocsPaVO.trasmissione.TrasmissioneUtente();
							trUt.utente=(DocsPaVO.utente.Utente) utenti[k];
							trasmissioniUt.Add(trUt);
						}
						trSing.trasmissioneUtente=trasmissioniUt;
					}
					else
					{
						logger.Debug("Caso utente");
						trSing.tipoDest=DocsPaVO.trasmissione.TipoDestinatario.UTENTE;
						System.Collections.ArrayList trasmissioniUt=new System.Collections.ArrayList();
							DocsPaVO.trasmissione.TrasmissioneUtente trUt=new DocsPaVO.trasmissione.TrasmissioneUtente();
							trUt.utente=(DocsPaVO.utente.Utente) dest[i];
							trasmissioniUt.Add(trUt);
						trSing.trasmissioneUtente=trasmissioniUt;
					}
					trasmissioniSing.Add(trSing);
				}

				trasm.trasmissioniSingole=trasmissioniSing;
                if (infoUtente.delegato != null)
                    trasm.delegato = ((DocsPaVO.utente.InfoUtente)(infoUtente.delegato)).idPeople;
				//BusinessLogic.Trasmissioni.TrasmManager.saveTrasmMethod(trasm);
				//BusinessLogic.Trasmissioni.ExecTrasmManager.executeTrasmMethod(serverName,trasm);
                BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(serverName, trasm);
			}
			catch(Exception e)
			{
				logger.Debug(e.Message);
				if(fsOpen){
				  fs.Close();
				}
				if(dbOpen)
				{
					//db.closeConnection();
				}
				
				logger.Debug("Errore nella gestione dei fax (processaMessaggio)",e);
				throw e;				
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pathFile"></param>
		/// <returns></returns>
		private static System.IO.FileStream getFileStream(string pathFile)
		{
			logger.Debug("getFileStream");
			System.IO.FileStream fs=null;
			string[] ExtensionFaxFiles =  {".G3N",".G3S",".G3F",".G3M"};
			try{ 
				for(int i=0;i<ExtensionFaxFiles.Length;++i)
				{
					logger.Debug("getFileStream ciclo for "+i.ToString());
					logger.Debug("getFileStream ciclo for path "+pathFile+ExtensionFaxFiles[i].ToString());
					System.IO.FileInfo fi=new System.IO.FileInfo(pathFile+ExtensionFaxFiles[i]);
					logger.Debug("getFileStream fi "+fi.Extension);
					if(fi.Exists)
					{
						logger.Debug("getFileStream Exist OK");
						fs=new System.IO.FileStream(pathFile+ExtensionFaxFiles[i],System.IO.FileMode.Open,System.IO.FileAccess.ReadWrite,System.IO.FileShare.ReadWrite);
						logger.Debug("getFileStream Stream ok tipo: "+ExtensionFaxFiles[i]);  
					}
				}
				return fs;
				}
			catch(Exception ex)
			{ 
				logger.Debug(ex.Message);
				logger.Debug("Errore nella gestione dei fax (getFileStream)",ex);
				throw ex;				
			}
		}

		#region Metodo Commentato
//		/// <summary>
//		/// </summary>
//		/// <param name="infoUtente"></param>
//		/// <returns></returns>
//		private static string getUserLogin(string idPeople)
//		{
//			logger.Debug("getUserLogin");
//			//DocsPa_V15_Utils.Database db=DocsPa_V15_Utils.dbControl.getDatabase();
//			bool dbOpen=false;
//			System.Data.DataSet ds=new System.Data.DataSet();
//			string userLogin=null;
//			try
//			{
//			   //db.openConnection();
//               dbOpen=true;
//
//				#region Codice Commentato
//				/*
//               string queryString="SELECT VAR_FAX_USER_LOGIN FROM DPA_CORR_GLOBALI WHERE ID_PEOPLE="+infoUtente.idPeople;
//               logger.Debug(queryString);
//			   userLogin=(string) db.executeScalar(queryString).ToString();
//			   */
//				#endregion
//
//				DocsPaDB.Query_DocsPAWS.Fax obj = new DocsPaDB.Query_DocsPAWS.Fax();
//				userLogin = obj.getFaxUserLogin(idPeople);
//
//			   //db.closeConnection();
//			   dbOpen=false;
//			   return userLogin;
//			}
//			catch(Exception e){
//			   logger.Debug(e.Message);
//				if(dbOpen){
//				  //db.closeConnection();
//				}
//
//				logger.Debug("Errore nella gestione dei fax (getUserLogin)",e);
//				throw e;				
//			}
//		}
		#endregion

		/// <summary>
		/// </summary>
		/// <returns></returns>
		private static DocsPaVO.trasmissione.RagioneTrasmissione getRagioneTrasm()
		{
			logger.Debug("getRagioneTrasm");
			//DocsPa_V15_Utils.Database db=DocsPa_V15_Utils.dbControl.getDatabase();
			DataSet ds=new DataSet();
			DocsPaVO.trasmissione.RagioneTrasmissione rt=new DocsPaVO.trasmissione.RagioneTrasmissione();
			try
			{  
				/*
				string queryString="SELECT * FROM DPA_RAGIONE_TRASM WHERE CHA_TIPO_RAGIONE='N' AND CHA_TIPO_DIRITTI='W'";
				db.fillTable(queryString,ds,"RAGIONE");
				*/
				DocsPaDB.Query_DocsPAWS.Fax obj = new DocsPaDB.Query_DocsPAWS.Fax();
				obj.getRagTrasm(out ds);

				DataRow ragione=ds.Tables["RAGIONE"].Rows[0];
				rt.descrizione=ragione["VAR_DESC_RAGIONE"].ToString();
				rt.risposta=ragione["CHA_RISPOSTA"].ToString();
				rt.systemId=ragione["SYSTEM_ID"].ToString();
				rt.tipo="N";
				logger.Debug("prima di tipo dest"+ragione["CHA_TIPO_DEST"].ToString()+" "+ DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.RagioneTrasmissione.tipoGerarchiaStringa,ragione["CHA_TIPO_DEST"].ToString()).GetType());
				rt.tipoDestinatario=(DocsPaVO.trasmissione.TipoGerarchia) DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.RagioneTrasmissione.tipoGerarchiaStringa,ragione["CHA_TIPO_DEST"].ToString());
				
				rt.tipoDiritti=DocsPaVO.trasmissione.TipoDiritto.WRITE;
			
				rt.eredita=ragione["CHA_EREDITA"].ToString();
			}
			catch(Exception e)
			{
				logger.Debug(e.ToString());
				//db.closeConnection();
				
				logger.Debug("Errore nella gestione dei fax (getRagioneTrasm)",e);
				throw e;				
			}

			return rt;
		}
	}
}
