using System;
using System.Collections;
using System.Data;
using log4net;

namespace BusinessLogic.Fascicoli
{
	/// <summary>
	/// </summary>
	internal class ProjectsManager 
	{
        private static ILog logger = LogManager.GetLogger(typeof(ProjectsManager));
		/// <summary>
		/// </summary>
		/// <param name="db"></param>
		/// <param name="idProject"></param>
		/// <param name="idParent"></param>
		/// <param name="debug"></param>
		internal static void setVisibilita(/*DocsPaWS.Utils.Database db,*/ string idProject, string idParent) 
		{
			#region Codice Commentato
			/*string insertStr = 
				"INSERT INTO SECURITY (THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO) " +
				"(SELECT " + idProject + ", PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO FROM SECURITY WHERE THING=" + idParent + ")";
			logger.Debug(insertStr);
			db.executeNonQuery(insertStr);*/
			#endregion

			DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
			fascicoli.SetVisibilita(idProject,idParent);
			fascicoli.Dispose();
		}
		
		/// <summary>
		/// </summary>
		/// <param name="db"></param>
		/// <param name="tipoProj"></param>
		/// <param name="listaID"></param>
		/// <param name="corrAdd"></param>
		/// <param name="corrRemove"></param>
		/// <param name="debug"></param>
		internal static void setVisibilita(/*DocsPaWS.Utils.Database db,*/ string tipoProj, ArrayList listaID, DocsPaVO.utente.Corrispondente[] corrAdd, DocsPaVO.utente.Corrispondente[] corrRemove) 
		{
			//string insertStr;
			//string deleteStr;
			string personOrGroup = "";	
			
			if(listaID == null)
			{
				return;
			}

			if(listaID.Count == 0)
			{
				return;
			}

			listaID = getChildren(/*db,*/ tipoProj, listaID);
			
			string idProject;

			if(corrAdd != null && corrAdd.Length > 0 && listaID.Count > 0) 
			{
				for (int j=0; j < listaID.Count; j++) 
				{
					idProject = (string)listaID[j];

					for (int i=0; i < corrAdd.Length; i++) 
					{
						if(corrAdd[i].GetType().Equals(typeof(DocsPaVO.utente.Utente)))
						{
							personOrGroup = ((DocsPaVO.utente.Utente)corrAdd[i]).idPeople;
						}
						else if(corrAdd[i].GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
						{
							personOrGroup = ((DocsPaVO.utente.Ruolo)corrAdd[i]).idGruppo;
						}
						else 
						{
							logger.Debug("Errore nella gestione dei fascicoli. (setVisibilita)");
							throw new Exception("Tipo non supportato");							
						}

						#region Codice Commentato
						/*insertStr = 
							"INSERT INTO SECURITY (THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO) " +
							"VALUES (" + idProject + ", " + personOrGroup + ", 255, " + personOrGroup + ", 'P')";
						logger.Debug(insertStr);
						db.executeLocked(insertStr);*/
						#endregion

						DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
						fascicoli.SetVisibilitaInsert(idProject,personOrGroup);
						fascicoli.Dispose();
					}
				}
			}

			if(corrRemove != null && corrRemove.Length > 0 && listaID.Count > 0) 
			{
				idProject = (string)listaID[0];
				
				for (int i=1; i < listaID.Count; i++)
				{
					idProject += "," + (string)listaID[i];
				}

				personOrGroup = "(" + getIdUtenteRuolo(corrRemove[0]);
				
				for (int i=1; i < corrRemove.Length; i++) 
				{
					personOrGroup += "," + getIdUtenteRuolo(corrRemove[i]);
				}

				personOrGroup += ")";

				#region Codice Commentato
				/*deleteStr = 
					"DELETE FROM SECURITY WHERE THING IN (" + idProject + ") AND PERSONORGROUP IN " + personOrGroup;
				logger.Debug(deleteStr);
				db.executeLocked(deleteStr);*/
				#endregion

				DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
				fascicoli.SetVisibilitaDelete(idProject,personOrGroup);
				fascicoli.Dispose();
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="db"></param>
		/// <param name="tipoProj"></param>
		/// <param name="listaID"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		internal static ArrayList getChildren(/*DocsPaWS.Utils.Database db,*/ string tipoProj, ArrayList listaID) 
		{
			ArrayList listaIdFascicoli = new ArrayList();

			#region Codice Commentato
		/*	string queryStr;		
			IDataReader dr;
			string idProject = (string)listaID[0];
			for (int i=1; i < listaID.Count; i++)
				idProject += "," + (string)listaID[i];
			if(tipoProj.Equals("T")) {	
				queryStr = 
					"SELECT SYSTEM_ID FROM PROJECT WHERE CHA_TIPO_PROJ='F' AND ID_PARENT IN (" + idProject + ")";
				logger.Debug(queryStr);
				dr = db.executeReader(queryStr);
				while(dr.Read()) {
					listaID.Add(dr.GetValue(0).ToString());
					listaIdFascicoli.Add(dr.GetValue(0).ToString());
				}
				dr.Close();
			} else if(tipoProj.Equals("F"))
				listaIdFascicoli = listaID;
			string idFascicoli = (string)listaIdFascicoli[0];
			for (int i=1; i < listaIdFascicoli.Count; i++)
				idFascicoli += "," + (string)listaIdFascicoli[i];
			queryStr = 
				"SELECT SYSTEM_ID FROM PROJECT WHERE CHA_TIPO_PROJ='C' AND ID_FASCICOLO IN (" + idFascicoli + ")";
			logger.Debug(queryStr);
			dr = db.executeReader(queryStr);
			while(dr.Read())
				listaID.Add(dr.GetValue(0).ToString());
			dr.Close();*/
			#endregion

			DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            listaID = fascicoli.GetChildren(/*db,*/tipoProj,listaID);

			return listaID;
		}

		/// <summary>
		/// </summary>
		/// <param name="corrispondente"></param>
		/// <returns></returns>
		internal static string getIdUtenteRuolo(DocsPaVO.utente.Corrispondente corrispondente) 
		{
			if(corrispondente.GetType().Equals(typeof(DocsPaVO.utente.Utente)))
			{
				return ((DocsPaVO.utente.Utente)corrispondente).idPeople;
			}
			else if(corrispondente.GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
			{
				return ((DocsPaVO.utente.Ruolo)corrispondente).idGruppo;
			}
			else 
			{
				logger.Debug("Errore nella gestione dei fascicoli. (getIdUtenteRuolo)");
				//TODO: gestire la throw
				throw new Exception("Tipo non supportato");				
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="db"></param>
		/// <param name="idProject"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		internal static ArrayList getVisibilita(/*DocsPaWS.Utils.Database db,*/ string idProject, bool cercaRimossi,string rootFolder) 
		{
			
			System.Collections.ArrayList listaDiritti=new System.Collections.ArrayList();
			
			//query distinte per i ruoli e per gli utenti
            //1-->inserimento ruoli
            DataSet dataSet;//= new  DataSet();
			DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
			fascicoli.GetVisibilita(out dataSet,idProject);
			fascicoli.Dispose();
			foreach(DataRow ruoloRow in dataSet.Tables["DIRITTI_RUOLI"].Rows) 
			{
				DocsPaVO.fascicolazione.DirittoOggetto dirittoOggetto=new DocsPaVO.fascicolazione.DirittoOggetto();
				DocsPaVO.utente.Ruolo ruolo=new DocsPaVO.utente.Ruolo();
				DocsPaVO.addressbook.QueryCorrispondente qco=new DocsPaVO.addressbook.QueryCorrispondente();
				qco.codiceRubrica=ruoloRow["VAR_COD_RUBRICA"].ToString();
				qco.getChildren=false;
				
				if(ruoloRow["ID_REGISTRO"]!=null&&!ruoloRow["ID_REGISTRO"].ToString().Equals("")) 
				{
					System.Collections.ArrayList reg=new System.Collections.ArrayList();
					reg.Add(ruoloRow["ID_REGISTRO"].ToString());
					qco.idRegistri=reg;
				}

				if(ruoloRow["ID_AMM"]!=null&&!ruoloRow["ID_AMM"].ToString().Equals("")) {
					qco.idAmministrazione=ruoloRow["ID_AMM"].ToString();
				}

				qco.tipoUtente=DocsPaVO.addressbook.TipoUtente.INTERNO;
				ruolo=(DocsPaVO.utente.Ruolo) BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qco)[0];
				dirittoOggetto.idObj=idProject;
				dirittoOggetto.soggetto=ruolo;
				dirittoOggetto.tipoDiritto=getDirittoFasc(ruoloRow["CHA_TIPO_DIRITTO"].ToString());
                dirittoOggetto.accessRights = Convert.ToInt32(ruoloRow["ACCESSRIGHTS"]);
                dirittoOggetto.deleted = false;
                dirittoOggetto.personorgroup = ruoloRow["PERSONORGROUP"].ToString();
                dirittoOggetto.rootFolder = rootFolder;
                    
                listaDiritti.Add(dirittoOggetto);
			}

            //2-->Inserimento utenti
			DocsPaDB.Query_DocsPAWS.Fascicoli fascicoliUtenti = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            DataSet dataSet1 ;
			fascicoliUtenti.GetVisibilita1(out dataSet1 ,idProject);
			fascicoliUtenti.Dispose();
            DocsPaVO.addressbook.QueryCorrispondente qco1 = new DocsPaVO.addressbook.QueryCorrispondente();
            System.Collections.ArrayList utentiInt = new ArrayList();
            DocsPaVO.utente.Corrispondente soggettoPropr = null;
            string delegante = "";

            if (dataSet1.Tables["DIRITTI_UTENTI"].Rows.Count > 1)
            {
                foreach (DataRow utenteRow in dataSet1.Tables["DIRITTI_UTENTI"].Rows)
                {
                    DocsPaVO.fascicolazione.TipoDiritto tipoDiritto = getDirittoFasc(utenteRow["CHA_TIPO_DIRITTO"].ToString());
                    if (tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_PROPRIETARIO))
                    {
                        qco1.codiceRubrica = utenteRow["VAR_COD_RUBRICA"].ToString();
                        qco1.getChildren = false;
                        if (utenteRow["ID_REGISTRO"] != null && !utenteRow["ID_REGISTRO"].ToString().Equals(""))
                        {
                            System.Collections.ArrayList reg = new System.Collections.ArrayList();
                            reg.Add(utenteRow["ID_REGISTRO"].ToString());
                            qco1.idRegistri = reg;
                        }
                        if (utenteRow["ID_AMM"] != null && !utenteRow["ID_AMM"].ToString().Equals(""))
                        {
                            qco1.idAmministrazione = utenteRow["ID_AMM"].ToString();
                        }
                        qco1.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;

                        //gadamo 16.12.2008
                        qco1.fineValidita = false; // voglio recuperare anche i disabilitati ( non mette "DTA_FINE IS NULL" nella query nel metodo Utenti.ListaCorrispondentiInt(qco) )


                        utentiInt = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qco1);
                        //DocsPaVO.utente.Utente utProprietario = new DocsPaVO.utente.Utente();
                        //utentiInt = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qco1);
                        if (utentiInt != null && utentiInt.Count > 0)
                        {
                            soggettoPropr = (DocsPaVO.utente.Utente)utentiInt[0];
                            //soggettoPropr = utProprietario;
                        }
                    }
                }
            }
            else
            {
                DataRow row = (DataRow)(dataSet1.Tables["DIRITTI_UTENTI"].Rows[0]);
                delegante = row["VAR_COGNOME"].ToString() + " " + row["VAR_NOME"].ToString();
            }
                

			foreach(DataRow utenteRow in dataSet1.Tables["DIRITTI_UTENTI"].Rows) 
			{
				DocsPaVO.fascicolazione.DirittoOggetto dirittoOggetto=new DocsPaVO.fascicolazione.DirittoOggetto();
				DocsPaVO.utente.Utente utente=new DocsPaVO.utente.Utente();
				DocsPaVO.addressbook.QueryCorrispondente qco=new DocsPaVO.addressbook.QueryCorrispondente();
				qco.codiceRubrica=utenteRow["VAR_COD_RUBRICA"].ToString();
				qco.getChildren=false;
				
				if(utenteRow["ID_REGISTRO"]!=null&&!utenteRow["ID_REGISTRO"].ToString().Equals("")) 
				{
					System.Collections.ArrayList reg=new System.Collections.ArrayList();
					reg.Add(utenteRow["ID_REGISTRO"].ToString());
					qco.idRegistri=reg;
				}
				
				if(utenteRow["ID_AMM"]!=null&&!utenteRow["ID_AMM"].ToString().Equals("")) {
					qco.idAmministrazione=utenteRow["ID_AMM"].ToString();
				}

				qco.tipoUtente=DocsPaVO.addressbook.TipoUtente.INTERNO;
				ArrayList utenti = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qco);
				utente=(DocsPaVO.utente.Utente) BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qco)[0];
				dirittoOggetto.idObj=idProject;
                dirittoOggetto.tipoDiritto = getDirittoFasc(utenteRow["CHA_TIPO_DIRITTO"].ToString());
                //dirittoOggetto.soggetto=utente;
                if (dirittoOggetto.tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_DELEGATO))
                {
                    dirittoOggetto.soggetto = utente;
                    if (soggettoPropr!=null)
                        dirittoOggetto.soggetto.descrizione = utente.descrizione + " delegato da " + soggettoPropr.descrizione;
                    else
                        dirittoOggetto.soggetto.descrizione = utente.descrizione + " delegato da " + delegante;
                }
                else
                    dirittoOggetto.soggetto = utente;
				dirittoOggetto.accessRights = Convert.ToInt32(utenteRow["ACCESSRIGHTS"]);
                dirittoOggetto.deleted = false;
                dirittoOggetto.personorgroup = utenteRow["PERSONORGROUP"].ToString();
                dirittoOggetto.rootFolder = rootFolder;
                listaDiritti.Add(dirittoOggetto);
				logger.Debug("Utente inserito");
			};

            //3-4 --> inserimento ruoli e utenti rimossi
            if (cercaRimossi)
            {
                //3-->gestione ruoli diritti rimossi
                DataSet dsRuoliRimossi;//= new  DataSet();
                DocsPaDB.Query_DocsPAWS.Fascicoli fascRuoliRimossi = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                fascRuoliRimossi.GetVisibilita_rimossi(out dsRuoliRimossi, idProject);
                fascRuoliRimossi.Dispose();
                foreach (DataRow ruoloRow in dsRuoliRimossi.Tables["DIRITTI_RUOLI_RIMOSSI"].Rows)
                {
                    DocsPaVO.fascicolazione.DirittoOggetto dirittoOggetto = new DocsPaVO.fascicolazione.DirittoOggetto();
                    DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                    DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                    qco.codiceRubrica = ruoloRow["VAR_COD_RUBRICA"].ToString();
                    qco.getChildren = false;
                    if (ruoloRow["ID_REGISTRO"] != null && !ruoloRow["ID_REGISTRO"].ToString().Equals(""))
                    {
                        System.Collections.ArrayList reg = new System.Collections.ArrayList();
                        reg.Add(ruoloRow["ID_REGISTRO"].ToString());
                        qco.idRegistri = reg;
                    }
                    if (ruoloRow["ID_AMM"] != null && !ruoloRow["ID_AMM"].ToString().Equals(""))
                    {
                        qco.idAmministrazione = ruoloRow["ID_AMM"].ToString();
                    }
                    qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                    ruolo = (DocsPaVO.utente.Ruolo)BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qco)[0];
                    dirittoOggetto.idObj = idProject;
                    dirittoOggetto.soggetto = ruolo;
                    dirittoOggetto.tipoDiritto = getDirittoFasc(ruoloRow["CHA_TIPO_DIRITTO"].ToString());
                    dirittoOggetto.accessRights = Convert.ToInt32(ruoloRow["ACCESSRIGHTS"]);
                    dirittoOggetto.deleted = true;
                    dirittoOggetto.note = ruoloRow["NOTE"].ToString();
                    dirittoOggetto.personorgroup = ruoloRow["PERSONORGROUP"].ToString();
                    dirittoOggetto.rootFolder = rootFolder;
                    listaDiritti.Add(dirittoOggetto);
                }

                //4-->gestione utenti diritti rimossi
                DocsPaDB.Query_DocsPAWS.Fascicoli fascUtentiRimossi = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                DataSet dsUtentiRimossi;
                fascUtentiRimossi.GetVisibilita_UtentiRimossi(out dsUtentiRimossi, idProject);
                fascUtentiRimossi.Dispose();
                foreach (DataRow utenteRow in dsUtentiRimossi.Tables["DIRITTI_UTENTI_RIMOSSI"].Rows)
                {
                    DocsPaVO.fascicolazione.DirittoOggetto dirittoOggetto = new DocsPaVO.fascicolazione.DirittoOggetto();
                    DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();
                    DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                    qco.codiceRubrica = utenteRow["VAR_COD_RUBRICA"].ToString();
                    qco.getChildren = false;
                    if (utenteRow["ID_REGISTRO"] != null && !utenteRow["ID_REGISTRO"].ToString().Equals(""))
                    {
                        System.Collections.ArrayList reg = new System.Collections.ArrayList();
                        reg.Add(utenteRow["ID_REGISTRO"].ToString());
                        qco.idRegistri = reg;
                    }

                    if (utenteRow["ID_AMM"] != null && !utenteRow["ID_AMM"].ToString().Equals(""))
                    {
                        qco.idAmministrazione = utenteRow["ID_AMM"].ToString();
                    }

                    qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                    ArrayList utenti = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qco);
                    utente = (DocsPaVO.utente.Utente)BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qco)[0];
                    dirittoOggetto.idObj = idProject;
                    dirittoOggetto.soggetto = utente;
                    dirittoOggetto.tipoDiritto = getDirittoFasc(utenteRow["CHA_TIPO_DIRITTO"].ToString());
                    dirittoOggetto.accessRights = Convert.ToInt32(utenteRow["ACCESSRIGHTS"]);
                    dirittoOggetto.deleted = true;
                    dirittoOggetto.note = utenteRow["NOTE"].ToString();
                    dirittoOggetto.personorgroup = utenteRow["PERSONORGROUP"].ToString();
                    dirittoOggetto.rootFolder = rootFolder;
                    listaDiritti.Add(dirittoOggetto);
                    logger.Debug("Utente inserito");
                }
            }

			//ritorna la lista dei diritti
			return listaDiritti;
		}

		/// <summary>
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		private static DocsPaVO.fascicolazione.TipoDiritto getDirittoFasc(string str) 
		{
			if(str.Equals("P")) return DocsPaVO.fascicolazione.TipoDiritto.TIPO_PROPRIETARIO;
			if(str.Equals("T")) return DocsPaVO.fascicolazione.TipoDiritto.TIPO_TRASMISSIONE;
			if(str.Equals("S")) return DocsPaVO.fascicolazione.TipoDiritto.TIPO_SOSPESO;
            if(str.Equals("D")) return DocsPaVO.fascicolazione.TipoDiritto.TIPO_DELEGATO;
			
			return DocsPaVO.fascicolazione.TipoDiritto.TIPO_ACQUISITO;
		}

		#region Metodo Commentato
//		/// <summary>
//		/// </summary>
//		/// <param name="documento"></param>
//		/// <param name="descrizione"></param>
//		/// <param name="infoUtente"></param>
//		/// <param name="debug"></param>
//		/// <returns></returns>
//		public static string createPCDProject(out DocsPaDocManagement.Documentale.Documento documento, string descrizione, DocsPaVO.utente.InfoUtente infoUtente) 
//		{
//			string idAmministrazione = infoUtente.idAmministrazione;
//			string library = DocsPaDB.Utils.Personalization.getInstance(infoUtente.idAmministrazione).getLibrary();
//		
//			//PCDCLIENTLib.PCDDocObject docObj = new PCDCLIENTLib.PCDDocObject();
//
//			documento = new DocsPaDocManagement.Documentale.Documento(infoUtente.dst, library, DocsPaDocManagement.Documentale.Tipi.ObjectType.Cyd_CmnProjects);
//			
//			/*
//			logger.Debug("library="+library);
//			docObj.SetDST(infoUtente.dst);
//			docObj.SetObjectType("CYD_CMNPROJECTS");
//			docObj.SetProperty("%TARGET_LIBRARY", library);
//			*/
//
//			/*
//			docObj.SetProperty("DESCRIPTION", descrizione);	
//			*/
//			documento.Description = descrizione;
//			
//			// è necessario inserire anche i permessi per l'utente TYPIST_ID
//			documento.SetTrustee(infoUtente.userId,2,0);
//			documento.Create();		
//
//			/*
//			DocsPaWS.Utils.ErrorHandler.checkPCDOperation(docObj,"Errore nella creazione del project");
//			*/
//			if(documento.GetErrorCode() != 0)
//			{
//				throw new Exception("Errore nella creazione del project");
//			}
//
//			logger.Debug("project creato");
//
//			/*
//			string idProject = docObj.GetReturnProperty("%OBJECT_IDENTIFIER").ToString();
//			*/
//			string idProject = documento.ObjectIdentifier;
//
//			documento = new DocsPaDocManagement.Documentale.Documento(infoUtente.dst, library, DocsPaDocManagement.Documentale.Tipi.ObjectType.Cyd_CmnProjects);
//			
//			/*
//			docObj.SetDST(infoUtente.dst);
//			docObj.SetObjectType("CYD_CMNPROJECTS");
//			docObj.SetProperty("%TARGET_LIBRARY", library);
//			*/
//
//			/*
//			docObj.SetProperty("%OBJECT_IDENTIFIER", idProject);
//			*/
//			documento.ObjectIdentifier = idProject;
//
//			documento.Update();		
//			
//			return idProject;
//		}
		#endregion

		#region Metodo Commentato
//		/// <summary>
//		/// </summary>
//		/// <param name="idProject"></param>
//		/// <param name="infoUtente"></param>
//		/// <param name="debug"></param>
//		public static void deletePCDProject(string idProject, DocsPaVO.utente.InfoUtente infoUtente) 
//		{	
//			string idAmministrazione = infoUtente.idAmministrazione;
//			string library = DocsPaDB.Utils.Personalization.getInstance(infoUtente.idAmministrazione).getLibrary();
//			
//			/*
//			PCDCLIENTLib.PCDDocObject docObj = new PCDCLIENTLib.PCDDocObject();
//			*/
//			DocsPaDocManagement.Documentale.Documento documento = new DocsPaDocManagement.Documentale.Documento(infoUtente.dst, library, DocsPaDocManagement.Documentale.Tipi.ObjectType.Cyd_CmnProjects);
//
//			/*
//			logger.Debug("Cancellazione del project ID: " + idProject + " - Utente: " + infoUtente.userId);
//			docObj.SetDST(infoUtente.dst);
//			docObj.SetObjectType("CYD_CMNPROJECTS");
//			docObj.SetProperty("%TARGET_LIBRARY", library);
//			*/
//
//			/*
//			docObj.SetProperty("%OBJECT_IDENTIFIER", idProject);
//			*/
//			documento.ObjectIdentifier = idProject;
//
//			documento.Delete();
//
//			/*
//			DocsPaWS.Utils.ErrorHandler.checkPCDOperation(docObj,"Errore nella cancellazione del project");
//			*/
//			if(documento.GetErrorCode() != 0)
//			{
//				throw new Exception("Errore nella cancellazione del project");
//			}
//
//			logger.Debug("project "+ idProject + " cancellato");					
//		}
		#endregion

        /// <summary>
        /// </summary>
        /// <param name="db"></param>
        /// <param name="thing"></param>
        /// <param name="infoUtente"></param>
        /// <param name="objRuolo"></param>
        /// <param name="debug"></param>
        public static void setProjectTrustees(string idPeople, string thing, DocsPaVO.utente.Ruolo objRuolo, string idClassificazione)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            System.Collections.ArrayList ruoliSuperiori;
            fascicoli.SetProjectTrustees(idPeople, thing, objRuolo, idClassificazione, out ruoliSuperiori);
            fascicoli.Dispose();
        }

        /// <summary>
        /// </summary>
        /// <param name="db"></param>
        /// <param name="idProject"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        internal static ArrayList getVisibilitaSemplificata(/*DocsPaWS.Utils.Database db,*/ string idProject, bool cercaRimossi, string rootFolder)
        {

            System.Collections.ArrayList listaDiritti = new System.Collections.ArrayList();

            //query distinte per i ruoli e per gli utenti
            //1-->inserimento ruoli
            DataSet dataSet;//= new  DataSet();
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            fascicoli.GetVisibilitaSemplificata(out dataSet, idProject);
            fascicoli.Dispose();
            string IDAMM = string.Empty;
            foreach (DataRow ruoloRow in dataSet.Tables["DIRITTI_RUOLI"].Rows)
            {
                DocsPaVO.fascicolazione.DirittoOggetto dirittoOggetto = new DocsPaVO.fascicolazione.DirittoOggetto();
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                // Visualizzazione del pulsante della storia del ruolo se il ruolo ha una storia
                ruolo.ShowHistory = ruoloRow["ShowHistory"] != DBNull.Value ? ruoloRow["ShowHistory"].ToString() : String.Empty;

                // Impostazione dell'id del ruolo
                ruolo.systemId = ruoloRow["system_id"].ToString();

                ruolo.codiceRubrica = ruoloRow["VAR_COD_RUBRICA"].ToString();

                ruolo.dta_fine = ruoloRow["dta_fine"].ToString();

                if (ruoloRow["ID_REGISTRO"] != null && !ruoloRow["ID_REGISTRO"].ToString().Equals(""))
                {
                    System.Collections.ArrayList reg = new System.Collections.ArrayList();
                    reg.Add(ruoloRow["ID_REGISTRO"].ToString());
                    ruolo.registri = reg;
                }
                if (ruoloRow["ID_AMM"] != null && !ruoloRow["ID_AMM"].ToString().Equals(""))
                {
                    ruolo.idAmministrazione = ruoloRow["ID_AMM"].ToString();
                    IDAMM = ruolo.idAmministrazione;
                }

                if (ruoloRow["VAR_DESC_CORR"] != null && !ruoloRow["VAR_DESC_CORR"].ToString().Equals(""))
                {
                    ruolo.descrizione = ruoloRow["VAR_DESC_CORR"].ToString();
                }

                if (ruoloRow["ID_UO"] != null && !ruoloRow["ID_UO"].ToString().Equals(""))
                {
                    ruolo.uo = new DocsPaVO.utente.UnitaOrganizzativa();
                    ruolo.uo.systemId = ruoloRow["ID_UO"].ToString();
                    ruolo.uo.idAmministrazione = IDAMM;
                    if (ruoloRow["DESC_UO"] != null && !ruoloRow["DESC_UO"].ToString().Equals(""))
                    {
                        ruolo.uo.descrizione = ruoloRow["DESC_UO"].ToString();
                    }
                    if (ruoloRow["CHA_TIPO_UO"] != null && !ruoloRow["CHA_TIPO_UO"].ToString().Equals(""))
                    {
                        ruolo.uo.tipoIE = ruoloRow["CHA_TIPO_UO"].ToString();
                    }
                    if (ruoloRow["VAR_COD_UO"] != null && !ruoloRow["VAR_COD_UO"].ToString().Equals(""))
                    {
                        ruolo.uo.codiceRubrica = ruoloRow["VAR_COD_UO"].ToString();
                    }
                }

                if (ruoloRow["CHA_TIPO_URP"] != null && !ruoloRow["CHA_TIPO_URP"].ToString().Equals(""))
                {
                    ruolo.tipoCorrispondente = ruoloRow["CHA_TIPO_URP"].ToString();
                }

                if (ruoloRow["CHA_TIPO_IE"] != null && !ruoloRow["CHA_TIPO_IE"].ToString().Equals(""))
                {
                    ruolo.tipoIE = ruoloRow["CHA_TIPO_IE"].ToString();
                }
                ruolo.idGruppo = ruoloRow["PERSONORGROUP"].ToString();
                dirittoOggetto.idObj = idProject;
                dirittoOggetto.soggetto = ruolo;
                dirittoOggetto.tipoDiritto = getDirittoFasc(ruoloRow["CHA_TIPO_DIRITTO"].ToString());
                dirittoOggetto.accessRights = Convert.ToInt32(ruoloRow["ACCESSRIGHTS"]);
                dirittoOggetto.deleted = false;
                dirittoOggetto.personorgroup = ruoloRow["PERSONORGROUP"].ToString();
                dirittoOggetto.rootFolder = rootFolder;

                if (ruoloRow["TS_INSERIMENTO"] != DBNull.Value)
                    dirittoOggetto.dtaInsSecurity = ruoloRow["TS_INSERIMENTO"].ToString();
                if (ruoloRow["VAR_NOTE_SEC"] != DBNull.Value)
                    dirittoOggetto.noteSecurity = ruoloRow["VAR_NOTE_SEC"].ToString();

                listaDiritti.Add(dirittoOggetto);
                logger.Debug("Ruolo inserito");
            }

            //2-->Inserimento utenti
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoliUtenti = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            DataSet dataSet1;
            fascicoliUtenti.GetVisibilita1Semplificata(out dataSet1, idProject);
            fascicoliUtenti.Dispose();

            foreach (DataRow utenteRow in dataSet1.Tables["DIRITTI_UTENTI"].Rows)
            {
                DocsPaVO.utente.Utente utenteTemp = new DocsPaVO.utente.Utente();
                DocsPaVO.fascicolazione.DirittoOggetto dirittoOggetto = new DocsPaVO.fascicolazione.DirittoOggetto();
                DocsPaVO.fascicolazione.TipoDiritto tipoDiritto = getDirittoFasc(utenteRow["CHA_TIPO_DIRITTO"].ToString());
                utenteTemp.codiceRubrica = utenteRow["VAR_COD_RUBRICA"].ToString();
                if (utenteRow["ID_REGISTRO"] != null && !utenteRow["ID_REGISTRO"].ToString().Equals(""))
                {
                    string reg = utenteRow["ID_REGISTRO"].ToString();
                    utenteTemp.idRegistro = reg;
                }
                if (utenteRow["ID_AMM"] != null && !utenteRow["ID_AMM"].ToString().Equals(""))
                {
                    utenteTemp.idAmministrazione = utenteRow["ID_AMM"].ToString();
                }

                if (utenteRow["VAR_DESC_CORR"] != null && !utenteRow["VAR_DESC_CORR"].ToString().Equals(""))
                {
                    utenteTemp.descrizione = utenteRow["VAR_DESC_CORR"].ToString();
                }

                if (utenteRow["CHA_TIPO_URP"] != null && !utenteRow["CHA_TIPO_URP"].ToString().Equals(""))
                {
                    utenteTemp.tipoCorrispondente = utenteRow["CHA_TIPO_URP"].ToString();
                }

                if (utenteRow["CHA_TIPO_IE"] != null && !utenteRow["CHA_TIPO_IE"].ToString().Equals(""))
                {
                    utenteTemp.tipoIE = utenteRow["CHA_TIPO_IE"].ToString();
                }

                utenteTemp.idPeople = utenteRow["PERSONORGROUP"].ToString();

                dirittoOggetto.idObj = idProject;
                dirittoOggetto.soggetto = utenteTemp;
                dirittoOggetto.tipoDiritto = getDirittoFasc(utenteRow["CHA_TIPO_DIRITTO"].ToString());
                dirittoOggetto.accessRights = Convert.ToInt32(utenteRow["ACCESSRIGHTS"]);
                dirittoOggetto.deleted = false;
                dirittoOggetto.personorgroup = utenteRow["PERSONORGROUP"].ToString();
                dirittoOggetto.rootFolder = rootFolder;

                if (dirittoOggetto.tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_DELEGATO))
                {
                    dirittoOggetto.soggetto = utenteTemp;
                    dirittoOggetto.soggetto.descrizione = utenteTemp.descrizione + " delegato da " + utenteTemp.descrizione;
                }
                else
                    dirittoOggetto.soggetto = utenteTemp;

                listaDiritti.Add(dirittoOggetto);
                logger.Debug("Utente inserito");
            }

            //3-4 --> inserimento ruoli e utenti rimossi
            if (cercaRimossi)
            {
                //3-->gestione ruoli diritti rimossi
                DataSet dsRuoliRimossi;//= new  DataSet();
                DocsPaDB.Query_DocsPAWS.Fascicoli fascRuoliRimossi = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                fascRuoliRimossi.GetVisibilita_rimossiSemplificata(out dsRuoliRimossi, idProject);
                fascRuoliRimossi.Dispose();
                foreach (DataRow ruoloRow in dsRuoliRimossi.Tables["DIRITTI_RUOLI_DELETED"].Rows)
                {
                    DocsPaVO.fascicolazione.DirittoOggetto dirittoOggetto = new DocsPaVO.fascicolazione.DirittoOggetto();
                    DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                    ruolo.codiceRubrica = ruoloRow["VAR_COD_RUBRICA"].ToString();

                    if (ruoloRow["ID_REGISTRO"] != null && !ruoloRow["ID_REGISTRO"].ToString().Equals(""))
                    {
                        System.Collections.ArrayList reg = new System.Collections.ArrayList();
                        reg.Add(ruoloRow["ID_REGISTRO"].ToString());
                        ruolo.registri = reg;
                    }
                    if (ruoloRow["ID_AMM"] != null && !ruoloRow["ID_AMM"].ToString().Equals(""))
                    {
                        ruolo.idAmministrazione = ruoloRow["ID_AMM"].ToString();
                        IDAMM = ruolo.idAmministrazione;
                    }

                    if (ruoloRow["VAR_DESC_CORR"] != null && !ruoloRow["VAR_DESC_CORR"].ToString().Equals(""))
                    {
                        ruolo.descrizione = ruoloRow["VAR_DESC_CORR"].ToString();
                    }

                    if (ruoloRow["ID_UO"] != null && !ruoloRow["ID_UO"].ToString().Equals(""))
                    {
                        ruolo.uo = new DocsPaVO.utente.UnitaOrganizzativa();
                        ruolo.uo.systemId = ruoloRow["ID_UO"].ToString();
                        ruolo.uo.idAmministrazione = IDAMM;
                        if (ruoloRow["DESC_UO"] != null && !ruoloRow["DESC_UO"].ToString().Equals(""))
                        {
                            ruolo.uo.descrizione = ruoloRow["DESC_UO"].ToString();
                        }
                        if (ruoloRow["CHA_TIPO_UO"] != null && !ruoloRow["CHA_TIPO_UO"].ToString().Equals(""))
                        {
                            ruolo.uo.tipoIE = ruoloRow["CHA_TIPO_UO"].ToString();
                        }
                        if (ruoloRow["VAR_COD_UO"] != null && !ruoloRow["VAR_COD_UO"].ToString().Equals(""))
                        {
                            ruolo.uo.codiceRubrica = ruoloRow["VAR_COD_UO"].ToString();
                        }
                    }

                    if (ruoloRow["CHA_TIPO_URP"] != null && !ruoloRow["CHA_TIPO_URP"].ToString().Equals(""))
                    {
                        ruolo.tipoCorrispondente = ruoloRow["CHA_TIPO_URP"].ToString();
                    }

                    if (ruoloRow["CHA_TIPO_IE"] != null && !ruoloRow["CHA_TIPO_IE"].ToString().Equals(""))
                    {
                        ruolo.tipoIE = ruoloRow["CHA_TIPO_IE"].ToString();
                    }

                    ruolo.idGruppo = ruoloRow["PERSONORGROUP"].ToString();
                    dirittoOggetto.idObj = idProject;
                    dirittoOggetto.soggetto = ruolo;
                    dirittoOggetto.tipoDiritto = getDirittoFasc(ruoloRow["CHA_TIPO_DIRITTO"].ToString());
                    dirittoOggetto.accessRights = Convert.ToInt32(ruoloRow["ACCESSRIGHTS"]);
                    dirittoOggetto.deleted = true;
                    dirittoOggetto.personorgroup = ruoloRow["PERSONORGROUP"].ToString();
                    dirittoOggetto.rootFolder = rootFolder;

                    listaDiritti.Add(dirittoOggetto);
                    logger.Debug("Ruolo rimosso inserito");
                }

                //4-->gestione utenti diritti rimossi
                DocsPaDB.Query_DocsPAWS.Fascicoli fascUtentiRimossi = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                DataSet dsUtentiRimossi;
                fascUtentiRimossi.GetVisibilita_UtentiRimossiSemplificata(out dsUtentiRimossi, idProject);
                fascUtentiRimossi.Dispose();
                foreach (DataRow utenteRow in dsUtentiRimossi.Tables["DIRITTI_UTENTI_RIMOSSI"].Rows)
                {
                    DocsPaVO.utente.Utente utenteTemp = new DocsPaVO.utente.Utente();
                    DocsPaVO.fascicolazione.DirittoOggetto dirittoOggetto = new DocsPaVO.fascicolazione.DirittoOggetto();
                    DocsPaVO.fascicolazione.TipoDiritto tipoDiritto = getDirittoFasc(utenteRow["CHA_TIPO_DIRITTO"].ToString());
                    utenteTemp.codiceRubrica = utenteRow["VAR_COD_RUBRICA"].ToString();
                    if (utenteRow["ID_REGISTRO"] != null && !utenteRow["ID_REGISTRO"].ToString().Equals(""))
                    {
                        string reg = utenteRow["ID_REGISTRO"].ToString();
                        utenteTemp.idRegistro = reg;
                    }
                    if (utenteRow["ID_AMM"] != null && !utenteRow["ID_AMM"].ToString().Equals(""))
                    {
                        utenteTemp.idAmministrazione = utenteRow["ID_AMM"].ToString();
                    }

                    if (utenteRow["VAR_DESC_CORR"] != null && !utenteRow["VAR_DESC_CORR"].ToString().Equals(""))
                    {
                        utenteTemp.descrizione = utenteRow["VAR_DESC_CORR"].ToString();
                    }

                    if (utenteRow["CHA_TIPO_URP"] != null && !utenteRow["CHA_TIPO_URP"].ToString().Equals(""))
                    {
                        utenteTemp.tipoCorrispondente = utenteRow["CHA_TIPO_URP"].ToString();
                    }

                    if (utenteRow["CHA_TIPO_IE"] != null && !utenteRow["CHA_TIPO_IE"].ToString().Equals(""))
                    {
                        utenteTemp.tipoIE = utenteRow["CHA_TIPO_IE"].ToString();
                    }

                    utenteTemp.idPeople = utenteRow["PERSONORGROUP"].ToString();

                    dirittoOggetto.idObj = idProject;
                    dirittoOggetto.soggetto = utenteTemp;
                    dirittoOggetto.tipoDiritto = getDirittoFasc(utenteRow["CHA_TIPO_DIRITTO"].ToString());
                    dirittoOggetto.accessRights = Convert.ToInt32(utenteRow["ACCESSRIGHTS"]);
                    dirittoOggetto.deleted = true;
                    dirittoOggetto.personorgroup = utenteRow["PERSONORGROUP"].ToString();
                    dirittoOggetto.rootFolder = rootFolder;

                    if (dirittoOggetto.tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_DELEGATO))
                    {
                        dirittoOggetto.soggetto = utenteTemp;
                        dirittoOggetto.soggetto.descrizione = utenteTemp.descrizione + " delegato da " + utenteTemp.descrizione;
                    }
                    else
                        dirittoOggetto.soggetto = utenteTemp;

                    listaDiritti.Add(dirittoOggetto);
                    logger.Debug("Utente rimosso inserito");
                }
            }

            //ritorna la lista dei diritti
            return listaDiritti;
        }
	}
}
