using System;
using System.Data;
using System.Collections;
using System.Text.RegularExpressions;
using DocsPaVO.utente;
using System.Collections.Generic;
using DocsPaVO.ricerche;
using log4net;
using DocsPaVO.Grid;

namespace BusinessLogic.Documenti
{

    /// <summary>
    /// Summary description for InfoDocManager.
    /// </summary>
    public class InfoDocManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(InfoDocManager));

        public static ArrayList getQuery(string idGruppo, string idPeople, DocsPaVO.filtri.FiltroRicerca[][] objQueryList)
        {
            ArrayList listaInfoDoc = new ArrayList();
            //DocsPaWS.Utils.Database db = DocsPa_V15_Utils.dbControl.getDatabase();
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            try
            {
                if (cercaStampeRegistro(objQueryList))
                {
                    listaInfoDoc = doc.AppendListaStampeRegistro(idGruppo, idPeople, listaInfoDoc, objQueryList);
                }
                else
                {
                    listaInfoDoc = doc.AppendListaDocProtocollati(idGruppo, idPeople, listaInfoDoc, objQueryList);
                }
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();

                logger.Debug("Errore nella gestione dell'InfoDocManager (getQuery)", e);
                throw new Exception("F_System");
            }

            return listaInfoDoc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idGruppo"></param>
        /// <param name="idPeople"></param>
        /// <param name="folder"></param>
        /// <param name="filtriRicerca"></param>
        /// <param name="mittDestIndirizzo"></param>
        /// <param name="selectedDocumentsId">Id dei documenti da esportare</param>
        /// <returns></returns>
        public static ArrayList getQueryExportDocInFasc(string idGruppo, string idPeople, DocsPaVO.fascicolazione.Folder folder, DocsPaVO.filtri.FiltroRicerca[][] filtriRicerca, bool mittDestIndirizzo, String[] selectedDocumentsId)
        {
            ArrayList listaInfoDoc = new ArrayList();
            DocsPaDB.Query_DocsPAWS.Fascicoli fasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            try
            {
                listaInfoDoc = fasc.GetDocumentiExport(idGruppo, idPeople, folder, filtriRicerca, mittDestIndirizzo, selectedDocumentsId);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in getQueryExportDocInFasc: ", e);
                listaInfoDoc = null;
            }
            return listaInfoDoc;
        }

        public static ArrayList getQueryExportDocInCest(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.filtri.FiltroRicerca[][] filtriRicerca)
        {
            ArrayList listaInfoDoc = new ArrayList();
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            try
            {
                listaInfoDoc = doc.GetListaDocInCestino(infoUtente, filtriRicerca);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in getQueryExportDocInCest: ", e);
                listaInfoDoc = null;
            }
            return listaInfoDoc;
        }

        //		public static ArrayList getQueryExport(string idGruppo, string idPeople, DocsPaVO.filtri.FiltroRicerca[][] filtri, bool mittDest_indirizzo)
        public static ArrayList getQueryExport(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.filtri.FiltroRicerca[][] filtri, bool mittDest_indirizzo, String[] documentsSystemId)
        {
            ArrayList listaInfoDoc = new ArrayList();

            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            try
            {
                // if (cercaDocProtocollati(filtri) || cercaDocGrigi(filtri))
                //{
                //					listaInfoDoc = doc.AppendListaDocProtocollatiExport(idGruppo, idPeople, listaInfoDoc, filtri, mittDest_indirizzo);	
                listaInfoDoc = doc.AppendListaDocProtocollatiExport(infoUtente, listaInfoDoc, filtri, mittDest_indirizzo, documentsSystemId);
                //}						
            }
            catch (Exception e)
            {
                logger.Debug("Errore in getQueryExport: ", e);
                listaInfoDoc = null;
            }

            return listaInfoDoc;
        }

        #region paginazione getQuery
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idGruppo"></param>
        /// <param name="idPeople"></param>
        /// <param name="objQueryList"></param>
        /// <param name="grigi"></param>
        /// <param name="numPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="security"></param>
        /// <param name="numTotPage"></param>
        /// <param name="nRec"></param>
        /// <param name="getIdProfilesList"></param>
        /// <param name="idProfileList"></param>
        /// <param name="searchValueCustom"></param>
        /// <returns></returns>
        public static ArrayList getQueryPaging(string idGruppo, string idPeople,
                DocsPaVO.filtri.FiltroRicerca[][] objQueryList,
                bool grigi, int numPage, int pageSize, bool security,
            out int numTotPage, out int nRec, bool getIdProfilesList, out List<SearchResultInfo> idProfileList,
            bool searchValueCustom)
        {
            ArrayList listaInfoDoc = new ArrayList();
            nRec = 0;
            numTotPage = 0;
            //int tipoDoc = 0;
            //DocsPaWS.Utils.Database db = DocsPa_V15_Utils.dbControl.getDatabase();
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            try
            {
                //db.openConnection();				
                if (grigi)
                    listaInfoDoc = doc.AppendListaDocGrigiPaging(idGruppo, idPeople, listaInfoDoc, objQueryList, numPage, pageSize, security, out numTotPage, out nRec, getIdProfilesList, out idProfileList);
                else
                {
                    if (cercaStampeRegistro(objQueryList))
                        listaInfoDoc = doc.AppendListaStampeRegistroPaging(idGruppo, idPeople, listaInfoDoc, objQueryList, numPage, pageSize, security, out numTotPage, out nRec, getIdProfilesList, out idProfileList);
                    else
                        if (cercaStampeRegistro(objQueryList))
                            listaInfoDoc = doc.AppendListaStampeRegistroPaging(idGruppo, idPeople, listaInfoDoc, objQueryList, numPage, pageSize, security, out numTotPage, out nRec, getIdProfilesList, out idProfileList);
                        else
                            listaInfoDoc = doc.ListaDocumentiPaging(idGruppo, idPeople, listaInfoDoc, objQueryList, numPage, pageSize, security, out numTotPage, out nRec, getIdProfilesList, out idProfileList, searchValueCustom);
                }
                //db.closeConnection();
                //throw new Exception ("stop");
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();
                logger.Debug("Errore nella gestione dell'InfoDocManager (getQueryPaging)", e);
                throw new Exception("F_System");
            }

            return listaInfoDoc;
        }

        public static int getNumDocInRisposta(string idGruppo, string idPeople, DocsPaVO.filtri.FiltroRicerca[][] objQueryList, bool security)
        {
            int numDocInRisposta = 0;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            try
            {
                numDocInRisposta = doc.GetNumDocInRisposta(idGruppo, idPeople, objQueryList, security);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                logger.Debug("Errore nella gestione dell'InfoDocManager (getNumDocInRisposta)", e);
                throw new Exception("F_System");
            }
            return numDocInRisposta;
        }

        #endregion paginazione getQuery

        #region InfoDocumento
        #region Metodi Commentati
        /*internal static DocsPaVO.documento.InfoDocumento getInfoDocumento (string idProfile, DocsPaVO.utente.InfoUtente objSicurezza, bool corr) 
		{
			logger.Debug("getInfoDocumento");
			if (idProfile == null) 
			{
				return null;
			}
			if (idProfile.Equals("")) 
			{
					return null;
			}
			
			//DocsPaWS.Utils.Query query = getQueryDocumento(objSicurezza);
			//query.Where += " AND A.SYSTEM_ID=" + idProfil

			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			string query = doc.GetInfoDoc(objSicurezza, idProfile);

			ArrayList listaDoc = new ArrayList();
			listaDoc = appendiListaDocumenti(listaDoc, query, corr);
			DocsPa_V15_Utils.Logger.log("fine getInfoDocumento", logLevelTime);
			if (listaDoc.Count > 0)
			{
				return (DocsPaVO.documento.InfoDocumento)listaDoc[0];
			}
			else
			{
				return null;
			}
		}*/


        //add per ricerca Top N Documenti
        //private static ArrayList appendiListaDocumenti(ArrayList listaDoc, DocsPaWS.Utils.Database db, DocsPaWS.Utils.Query query, bool corr)
        /*private static ArrayList appendiListaDocumenti(ArrayList listaDoc, string query, bool corr)
        {
             return appendiListaDocumenti(listaDoc, query, corr, null);
        }
		

        //private static ArrayList appendiListaDocumenti(ArrayList listaDoc, DocsPaWS.Utils.Database db, DocsPaWS.Utils.Query query, bool corr, string numRighe) 
        private static ArrayList appendiListaDocumenti(ArrayList listaDoc, string query, bool corr, string numRighe) 
        {
            // TODO: gestire
            //modifica per ricerca Top N Documenti
            string queryString = System.String.Empty;
            if ((numRighe==null) || (numRighe.Equals("0")))
            {
                //queryString = DocsPa_V15_Utils.dbControl.selectTop(query.getQuery());
                queryString = DocsPaDbManagement.Functions.Functions.SelectTop(query);
            }
            else
            {
                //queryString = DocsPa_V15_Utils.dbControl.selectTop(query.getQuery(),numRighe);
                queryString = DocsPaDbManagement.Functions.Functions.SelectTop(query,numRighe);
            }
            //end modifica per ricerca Top N Documenti
			
            logger.Debug(queryString);
            //DataSet dataSet = new DataSet();
            DataSet dataSet;
			
            //db.fillTable(queryString, dataSet, "DOCUMENTI");			
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            doc.AppListaDocumenti(out dataSet, queryString);
			
            DocsPa_V15_Utils.Logger.log("Dopo query documenti", logLevelTime);

            setTableCorrispondenti(/*ref*/
        //dataSet);

        /*foreach(DataRow dataRow in dataSet.Tables["DOCUMENTI"].Rows) 
        {
            listaDoc.Add(getDatiDocumento(dataSet, dataRow, corr));
        }
        dataSet.Dispose();
        registri.Clear();
			
        return listaDoc;
    }*/
        #endregion

        internal static void getQueryDocumento(string idGruppo, string idPeople, ref string queryWhere, ref string queryFrom, ref string queryColumns)
        {
            #region Codice Commentato
            // TODO: il metodo si dovrebbe eliminare, ma è usato in trasmissioni.QueryTrasmManager.cs
            //DocsPa_V15_Utils.Query query = new DocsPa_V15_Utils.Query();
            //Il campo C.ACCESSRIGHTS è stato tolto dalla select per risolvere il problema dei distinct
            //data 02-40-2003
            /*query.Columns = 
                "SELECT DISTINCT A.SYSTEM_ID, A.DOCNUMBER, D.VAR_DESC_OGGETTO, " +
                "A.ID_REGISTRO, A.CHA_TIPO_PROTO, A.CHA_EVIDENZA, " +
                DocsPaDbManagement.Functions.Functions.ToChar("A.CREATION_DATE",false) + " AS CREATION_DATE, " +
                "A.NUM_PROTO, A.VAR_SEGNATURA," +
                DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_PROTO",false) + " AS DTA_PROTO ";
			
            if(objSicurezza != null) 
            {
                /*query.From =
                    "FROM SECURITY C, PROFILE A, DPA_OGGETTARIO D ";
	
                query.Where =
                    "WHERE " +
                    "A.SYSTEM_ID=C.THING AND D.SYSTEM_ID=A.ID_OGGETTO AND C.ACCESSRIGHTS > 0 " +
                    "AND (C.PERSONORGROUP=" + objSicurezza.idGruppo + " OR C.PERSONORGROUP=" + objSicurezza.idPeople+" )";
				
            } else 
            {
                /*query.From =
                    "FROM PROFILE A, DPA_OGGETTARIO D ";
				
                query.Where =
                    "WHERE D.SYSTEM_ID=A.ID_OGGETTO ";
            }
            //return query;*/
            #endregion

            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            doc.GetQueryDocumento(idGruppo, idPeople, ref queryWhere, ref queryFrom, ref queryColumns);
        }

        #region Metodi Commentati
        /*private static DocsPaVO.documento.InfoDocumento getDatiDocumento(DataSet dataSet, DataRow dataRow, bool corr) 
		{
			logger.Debug("getDatiDocumento");
			DocsPaVO.documento.InfoDocumento infoDoc = new DocsPaVO.documento.InfoDocumento();
			infoDoc.idProfile = dataRow["SYSTEM_ID"].ToString();
			infoDoc.docNumber = dataRow["DOCNUMBER"].ToString();
			infoDoc.tipoProto = dataRow["CHA_TIPO_PROTO"].ToString();
			logger.Debug("Aggiunto tipo proto "+dataRow["CHA_TIPO_PROTO"].ToString());
			if(dataRow["ID_REGISTRO"] != null) 
			{
				infoDoc.idRegistro = dataRow["ID_REGISTRO"].ToString();
				infoDoc.codRegistro = getCodiceRegistro(infoDoc.idRegistro);
			}
			infoDoc.dataApertura = dataRow["CREATION_DATE"].ToString();
			infoDoc.oggetto = dataRow["VAR_DESC_OGGETTO"].ToString();

			if(dataRow["CHA_EVIDENZA"] != null) 
				infoDoc.evidenza = dataRow["CHA_EVIDENZA"].ToString();
			if(infoDoc==null) 
			{
				logger.Debug("Infodoc null");
			}
			else
			{
				logger.Debug("Infodoc pieno");
			}
			
			infoDoc = getProtoData(dataSet, dataRow, infoDoc, corr);

			return infoDoc;
		}*/


        /*private static string getCodiceRegistro(string idRegistro) 
        {
            logger.Debug("getDescrizioneRegistro");
            string codice;
            if (!(idRegistro != null && !idRegistro.Equals("")))
                return null;
            if (registri.ContainsKey(idRegistro))
                codice = (string) registri[idRegistro];
            else 
            {
                /*string queryString = 
                    "SELECT VAR_CODICE FROM DPA_EL_REGISTRI WHERE SYSTEM_ID=" + idRegistro;
                logger.Debug(queryString);
                codice = db.executeScalar(queryString).ToString();*/
        /*DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        doc.GetCodiceRegistro(out codice, idRegistro);

        registri.Add(idRegistro, codice);
    }
    DocsPa_V15_Utils.Logger.log("fine getDescrizioneRegistro", logLevelTime);
    return codice;

}*/


        /*private static DocsPaVO.documento.InfoDocumento getProtoData(DataSet dataSet, DataRow dataRow, DocsPaVO.documento.InfoDocumento infoDoc, bool corr) 
        {
            logger.Debug("getProtoData");
            if (infoDoc == null || !(infoDoc.tipoProto.Equals("A") || infoDoc.tipoProto.Equals("P"))) 
            {
                return infoDoc;
            }
            if (dataRow["NUM_PROTO"] != null && !dataRow["NUM_PROTO"].ToString().Equals("")) 
            {
                infoDoc.numProt = dataRow["NUM_PROTO"].ToString();
                infoDoc.dataApertura = dataRow["DTA_PROTO"].ToString().Trim();
                infoDoc.segnatura = dataRow["VAR_SEGNATURA"].ToString();
            }
            if (corr)
            {
                infoDoc.mittDest = getCorrispondenti(dataRow["SYSTEM_ID"].ToString(), dataSet);
            }
            return infoDoc;
        }*/
        #endregion

        #endregion

        #region Lista documenti grigi
        #region Metodi Commentati
        /*private static ArrayList appendListaDocGrigi(ArrayList listaDoc, DocsPaVO.filtri.FiltroRicerca[][] objQueryList, DocsPaVO.utente.InfoUtente objSicurezza) 
		{
			logger.Debug("appendListaDocGrigi");
			
			/*DocsPaWS.Utils.Query query = getQueryDocumento(objSicurezza);
			query = getQueryDocGrigia(query);
			query = getQueryCondDocGrigia(query, objQueryList,db);			
			query.OrderBy = "ORDER BY A.SYSTEM_ID DESC";*/

        //aggiunto veronica
        /*string query;
        string queryFrom="";
        string queryWhere = " AND (A.CHA_TIPO_PROTO='G' OR A.CHA_TIPO_PROTO='R')";
        string queryOrd = "A.SYSTEM_ID DESC";
        queryWhere += getQueryCondDocGrigia(objQueryList, ref queryFrom);
        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        query = doc.GetQueryProtocollo(objQueryList, objSicurezza, queryWhere, queryFrom, queryOrd);

			
        listaDoc = appendiListaDocumenti(listaDoc, query, false);
			
        DocsPa_V15_Utils.Logger.log("fine appendListaDocGrigi", logLevelTime);

        return listaDoc;
    }*/

        /*private static DocsPaWS.Utils.Query getQueryDocGrigia(DocsPaWS.Utils.Query query) {
            query.Where += " AND (A.CHA_TIPO_PROTO='G' OR A.CHA_TIPO_PROTO='R')";
            return query;
        }*/

        //private static DocsPaWS.Utils.Query getQueryCondDocGrigia(DocsPaWS.Utils.Query query, DocsPaVO.filtri.FiltroRicerca[][] objQueryList, DocsPaWS.Utils.Database db) {

        /*private static string getQueryCondDocGrigia(DocsPaVO.filtri.FiltroRicerca[][] objQueryList, ref string queryFrom) 
        {
            string queryWhere="";
            string andStr;
            int numAndStr = 0;
            ArrayList listaOR = new ArrayList();
            for (int i = 0; i < objQueryList.Length; i++) {
                andStr = " (";
                numAndStr = 0;
                for (int j = 0; j < objQueryList[i].Length; j++) 
                {
                    DocsPaVO.filtri.FiltroRicerca f = objQueryList[i][j];
                    queryWhere += getQueryCondComuni(queryWhere, f, ref andStr, ref numAndStr, ref queryFrom);
                    //query = getQueryCondComuni(query, f, ref andStr, ref numAndStr);					
                }				
                andStr += ") ";
                logger.Debug("andStr = " + andStr);
                if (andStr.Length > 4) 
                {
                    listaOR.Add(andStr);
                    numAndStr = 0;
                }
            }
            if (listaOR.Count > 0) 
            {
                queryWhere += " AND (" + (string)listaOR[0];
                for (int i = 1; i < numAndStr; i++) 
                    queryWhere += " OR " + listaOR[i];
                queryWhere += ") ";

            }
            return queryWhere;
        }*/
        #endregion

        private static bool cercaDocGrigi(DocsPaVO.filtri.FiltroRicerca[][] objQueryList)
        {
            for (int i = 0; i < objQueryList.Length; i++)
            {
                for (int j = 0; j < objQueryList[i].Length; j++)
                {
                    DocsPaVO.filtri.FiltroRicerca f = objQueryList[i][j];
                    if (f.argomento.Equals("TIPO"))
                    {
                        string[] valor = f.valore.Split('^');
                        if (valor[3].Equals("1"))
                            return true;
                    }
                }
            }
            return false;
        }

        # endregion

        #region Lista documenti protocollati
        #region Metodi Commentati
        //ricerca del filtro VISUALIZZA_TOP_N_DOCUMENTI
        /*private static string cercaTopNDocumenti(DocsPaVO.filtri.FiltroRicerca[][] objQueryList) 
        {
            /* 
            * se viene trovato questo filtro 
            * Si esegue la Top per un numero 
            * di elementi pari al valore 
            * del filtro stesso 
            */

        /*for (int i = 0; i < objQueryList.Length; i++) 
        {
            for (int j = 0; j < objQueryList[i].Length; j++) 
            {
                DocsPaVO.filtri.FiltroRicerca f = objQueryList[i][j];
                if (f.argomento.Equals("VISUALIZZA_TOP_N_DOCUMENTI")) 
                    return f.valore;
            }
        }
        return null;
    }*/


        /*private static ArrayList appendListaDocProtocollati(ArrayList listaDoc, DocsPaVO.filtri.FiltroRicerca[][] objQueryList, DocsPaVO.utente.InfoUtente objSicurezza) 
        {
            logger.Debug("appendListaDocProtocollati");
			
            /*DocsPaWS.Utils.Query query = getQueryDocumento(objSicurezza);
            query = getQueryProtocollo(query, objSicurezza);
            query = getQueryCondProtocollo(query, objQueryList,db);			
            query.OrderBy = "ORDER BY A.ID_REGISTRO, A.NUM_PROTO DESC";*/

        //aggiunto veronica
        /*string query;
        string queryFrom="";
        string queryWhere = " AND A.CHA_TIPO_PROTO IN('A', 'P') ";
        string queryOrd = "A.ID_REGISTRO, A.NUM_PROTO DESC";
        queryWhere += getQueryCondProtocollo(objQueryList, ref queryFrom);
        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        query = doc.GetQueryProtocollo(objQueryList, objSicurezza, queryWhere, queryFrom, queryOrd);

        string numRighe = cercaTopNDocumenti(objQueryList);//per ricerca Top N Documenti
        listaDoc = appendiListaDocumenti(listaDoc, query, true, numRighe );
				
        DocsPa_V15_Utils.Logger.log("fine appendListaDocProtocollati", logLevelTime);

        return listaDoc;
    }*/


        /*private static DocsPaWS.Utils.Query getQueryProtocollo(DocsPaWS.Utils.Query query, DocsPaVO.utente.InfoUtente objSicurezza) {
            //query.addJoin("DPA_L_RUOLO_REG I", "I.ID_REGISTRO=B.SYSTEM_ID AND I.ID_RUOLO_IN_UO=" + objSicurezza.idCorrGlobali);
            //query.addJoin("DPA_EL_REGISTRI B", "A.ID_REGISTRO=B.SYSTEM_ID");
		
            query.Where += " AND A.CHA_TIPO_PROTO IN('A', 'P') ";
            return query;
        }*/

        //private static DocsPaWS.Utils.Query getQueryCondProtocollo(DocsPaWS.Utils.Query query, DocsPaVO.filtri.FiltroRicerca[][] objQueryList, DocsPaWS.Utils.Database db) {

        /*private static string getQueryCondProtocollo(DocsPaVO.filtri.FiltroRicerca[][] objQueryList, ref string queryFrom) 
        {
            string queryWhere="";
            string andStr;
            int numAndStr = 0;
            ArrayList listaOR = new ArrayList();
            for (int i = 0; i < objQueryList.Length; i++) 
            {
                andStr = " (";				
                numAndStr = 0;
                for (int j = 0; j < objQueryList[i].Length; j++) 
                {
                    DocsPaVO.filtri.FiltroRicerca f = objQueryList[i][j];
                    queryWhere += getQueryCondComuni(queryWhere, f, ref andStr, ref numAndStr, ref queryFrom);
                    if (f.valore != null && !f.valore.Equals("")) 
                    {
                        // i confronti vengono fatti sui valori presi da
                        // DocsPaVO.filtri.ricercaArgomenti inerenti documenti protocollati
                        switch(f.argomento) 
                        {
                            case "DATA_PROT_IL":
                                if (numAndStr > 0)  
                                    andStr += " AND ";	
                                numAndStr += 1;
                                andStr += "A.DTA_PROTO=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore,false);				
                                break;
                            case "DATA_PROT_PRECEDENTE_IL":
                                if (numAndStr > 0)  
                                    andStr += " AND ";	
                                numAndStr += 1;
                                andStr += "A.DTA_PROTO<=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore,false);				
                                break;
                            case "DATA_PROT_SUCCESSIVA_AL":
                                if (numAndStr > 0) 
                                    andStr += " AND ";	
                                numAndStr += 1;
                                andStr += "A.DTA_PROTO>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore,false);				
                                break;
                            case "ID_MITT_DEST":
                                //query. addJoin("DPA_DOC_ARRIVO_PAR F", "F.ID_PROFILE=A.SYSTEM_ID");								
                                queryWhere += "AND F.ID_PROFILE=A.SYSTEM_ID";
                                queryFrom += ", DPA_DOC_ARRIVO_PAR F";
                                if (numAndStr > 0)  
                                    andStr += " AND ";	
                                numAndStr += 1;
                                andStr += "F.ID_MITT_DEST" + getQueryCorrispondente(f.valore, objQueryList);				
                                break;
                            case "MITT_DEST":
                                /*query. addJoin("DPA_DOC_ARRIVO_PAR F", "F.ID_PROFILE=A.SYSTEM_ID");		
                                query. addJoin("DPA_CORR_GLOBALI G", "G.SYSTEM_ID=F.ID_MITT_DEST");		*/
        /*queryWhere += "AND F.ID_PROFILE=A.SYSTEM_ID AND G.SYSTEM_ID=F.ID_MITT_DEST";
        queryFrom += " ,DPA_DOC_ARRIVO_PAR F ,DPA_CORR_GLOBALI G ";
        if (numAndStr++ > 0) 
            andStr += " AND ";
        andStr += "G.VAR_DESC_CORR LIKE '%" + f.valore + "%'";				
        break;
    case "NUM_PROTOCOLLO":
        if (numAndStr++ > 0) 
            andStr += " AND ";
        andStr += "A.NUM_PROTO=" + f.valore;				
        break;
    case "NUM_PROTOCOLLO_AL":
        if (numAndStr++ > 0) 
            andStr += " AND ";
        andStr += "A.NUM_PROTO<=" + f.valore;				
        break;
    case "NUM_PROTOCOLLO_DAL":
        if (numAndStr > 0)  
            andStr += " AND ";	
        numAndStr += 1;
        andStr += "A.NUM_PROTO>=" + f.valore;				
        break;
    case "ID_MITTENTE_INTERMEDIO":
        //query. addJoin("DPA_DOC_ARRIVO_PAR F", "F.ID_PROFILE=A.SYSTEM_ID");		
        queryWhere += "AND F.ID_PROFILE=A.SYSTEM_ID";
        queryFrom += ", DPA_DOC_ARRIVO_PAR F";
        if (numAndStr > 0)  
            andStr += " AND ";	
        numAndStr += 1;
        andStr += "F.CHA_TIPO_MITT_DEST='I' AND F.ID_MITT_DEST" + getQueryCorrispondente(f.valore, objQueryList);				
        break;
    case "MITTENTE_INTERMEDIO":
        if (numAndStr > 0)  
            andStr += " AND ";	
        numAndStr += 1;
        /*query. addJoin("DPA_DOC_ARRIVO_PAR F", "F.ID_PROFILE=A.SYSTEM_ID");		
        query. addJoin("DPA_CORR_GLOBALI G", "G.SYSTEM_ID=F.ID_MITT_DEST");	*/
        /*queryWhere += ", DPA_DOC_ARRIVO_PAR F, DPA_CORR_GLOBALI G";
        queryFrom += " AND F.ID_PROFILE=A.SYSTEM_ID AND G.SYSTEM_ID=F.ID_MITT_DEST";
        andStr += "G.VAR_DESC_CORR LIKE '%" + f.valore + "%'";				
        break;							
    case "PROTOCOLLO_MITTENTE":
        if (numAndStr > 0)  
            andStr += " AND ";	
        numAndStr += 1;
        andStr += "A.VAR_PROTO_IN LIKE '%" + f.valore + "%'";				
        break;						
    case "DATA_PROT_MITTENTE_IL":
        if (numAndStr++ > 0) 
            andStr += " AND ";
        andStr += "A.DTA_PROTO_IN=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore,false);				
        break;
    case "DATA_PROT_MITTENTE_PRECEDENTE_IL":
        if (numAndStr > 0)  
            andStr += " AND ";	
        numAndStr += 1;
        andStr += "A.DTA_PROTO_IN<=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore,false);				
        break;
    case "DATA_PROT_MITTENTE_SUCCESSIVA_AL":
        if (numAndStr++ > 0) 
            andStr += " AND ";
        andStr += "A.DTA_PROTO_IN>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore,false);				
        break;
    case "DATA_ARRIVO_IL":
        if (numAndStr++ > 0) 
            andStr += " AND ";
        //query.addJoin("VERSIONS H", "H.DOCNUMBER=A.DOCNUMBER");
        queryWhere += " AND H.DOCNUMBER=A.DOCNUMBER";
        queryFrom += " , VERSIONS H";
        andStr += "H.DTA_ARRIVO=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore,false);				
        break;
    case "DATA_ARRIVO_PRECEDENTE_IL":
        if (numAndStr > 0)  
            andStr += " AND ";	
        numAndStr += 1;
        //query.addJoin("VERSIONS H", "H.DOCNUMBER=A.DOCNUMBER");
        queryWhere += " AND H.DOCNUMBER=A.DOCNUMBER";
        queryFrom += " , VERSIONS H";
        andStr += "H.DTA_ARRIVO<=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore,false);				
        break;
    case "DATA_ARRIVO_SUCCESSIVA_AL":
        if (numAndStr++ > 0) 
            andStr += " AND ";
        //query.addJoin("VERSIONS H", "H.DOCNUMBER=A.DOCNUMBER");
        queryWhere += " AND H.DOCNUMBER=A.DOCNUMBER";
        queryFrom += " , VERSIONS H";
        andStr += "H.DTA_ARRIVO>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore,false);				
        break;
    case "NUM_PROTO_EMERGENZA":
        if (numAndStr++ > 0) 
            andStr += " AND ";
        andStr += "A.VAR_PROTO_EME='" + f.valore + "'";				
        break;
    case "DATA_PROTO_EMERGENZA_IL":
        if (numAndStr++ > 0) 
            andStr += " AND ";
        andStr += "A.DTA_PROTO_EME=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore,false);								
        break;
    case "DA_PROTOCOLLARE":
        if (numAndStr > 0) 
            andStr += " AND ";	
        numAndStr += 1;
        andStr += "A.CHA_DA_PROTO='" + f.valore + "'";				
        break;
    case "ANNULLATO":
        if (numAndStr > 0)  
            andStr += " AND ";	
        numAndStr += 1;
        if (f.valore.Equals("0"))
            andStr += " A.DTA_ANNULLA IS NULL"; 
        else
            andStr += " A.DTA_ANNULLA IS NOT NULL";
        break;
}
}
}				
andStr += ") ";
if (andStr.Length > 4) 
{
listaOR.Add(andStr);
numAndStr = 0;
}
}
if (listaOR.Count > 0) 
{
queryWhere += " AND (" + (string)listaOR[0];
for (int i = 1; i < numAndStr; i++) 
queryWhere += " OR " + listaOR[i];
queryWhere += ") ";

}
return queryWhere;
}*/
        #endregion

        private static bool cercaDocProtocollati(DocsPaVO.filtri.FiltroRicerca[][] objQueryList)
        {
            for (int i = 0; i < objQueryList.Length; i++)
            {
                for (int j = 0; j < objQueryList[i].Length; j++)
                {
                    DocsPaVO.filtri.FiltroRicerca f = objQueryList[i][j];
                    if (f.argomento.Equals("TIPO"))
                    {
                        string[] valor = f.valore.Split('^');
                        if (valor[0].Equals("1") || valor[1].Equals("1") || valor[2].Equals("1"))
                            return true;
                    }
                    //switch (f.argomento) 
                    //{
                    //    case "DATA_PROT_IL": return true;
                    //    case "DATA_PROT_MITTENTE_IL": return true;
                    //    case "DATA_PROT_MITTENTE_PRECEDENTE_IL": return true;					
                    //    case "DATA_PROT_MITTENTE_SUCCESSIVA_AL": return true;						
                    //    case "DATA_PROT_PRECEDENTE_IL": return true;						
                    //    case "DATA_PROT_SUCCESSIVA_AL": return true;						
                    //    case "ID_MITT_DEST": return true;						
                    //    case "ID_MITTENTE_INTERMEDIO": return true;						
                    //    case "MITT_DEST": return true;						
                    //    case "NUM_PROTOCOLLO": return true;						
                    //    case "NUM_PROTOCOLLO_AL": return true;						
                    //    case "NUM_PROTOCOLLO_DAL": return true;						
                    //    case "PROTOCOLLO_MITTENTE": return true;
                    //    case "NUM_PROTO_EMERGENZA": return true;
                    //    case "DATA_PROTO_EMERGENZA_IL": return true;
                    //    case "SEGNATURA": return true;
                    //}
                }
            }
            return false;
        }


        #endregion

        #region Lista stampe registro
        #region Metodo Commentato
        /*private static ArrayList appendListaStampeRegistro(ArrayList listaDoc, DocsPaVO.filtri.FiltroRicerca[][] objQueryList, DocsPaVO.utente.InfoUtente objSicurezza) 
		{
			logger.Debug("appendListaStampeRegistro");
			//valgono le stesse regole dei documenti grigi
			/*DocsPaWS.Utils.Query query = getQueryDocumento(objSicurezza);
			query = getQueryDocGrigia(query);
			query = getQueryCondDocGrigia(query, objQueryList,db);			
			query.OrderBy = "ORDER BY A.SYSTEM_ID DESC";*/

        //aggiunto veronica
        /*string query;
        string queryFrom="";
        string queryWhere = " AND (A.CHA_TIPO_PROTO='G' OR A.CHA_TIPO_PROTO='R')";
        string queryOrd = "A.SYSTEM_ID DESC";
        queryWhere += getQueryCondDocGrigia(objQueryList, ref queryFrom);
        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        query = doc.GetQueryProtocollo(objQueryList, objSicurezza, queryWhere, queryFrom, queryOrd);
			
        listaDoc = appendiListaDocumenti(listaDoc, query, false);
			
        DocsPa_V15_Utils.Logger.log("fine appendListaStampeRegistro", logLevelTime);

        return listaDoc;
    }*/
        #endregion

        private static bool cercaStampeRegistro(DocsPaVO.filtri.FiltroRicerca[][] objQueryList)
        {
            for (int i = 0; i < objQueryList.Length; i++)
            {
                for (int j = 0; j < objQueryList[i].Length; j++)
                {
                    DocsPaVO.filtri.FiltroRicerca f = objQueryList[i][j];
                    if (f.argomento.Equals("TIPO") && (f.valore.Equals("R") || f.valore.Equals("C")))
                        return true;
                    if (f.argomento.Equals("STAMPA_REG") && f.valore.Equals("true"))
                        return true;
                }
            }
            return false;
        }
        #endregion

        #region Filtri comuni (Tutto Commentato)
        //private static DocsPaWS.Utils.Query getQueryCondComuni(DocsPaWS.Utils.Query query, DocsPaVO.filtri.FiltroRicerca f, ref string andStr, ref int numAndStr) {
        /*private static string getQueryCondComuni(string queryWhere, DocsPaVO.filtri.FiltroRicerca f, ref string andStr, ref int numAndStr, ref string queryFrom) 
        {
			
            if (f.valore != null && !f.valore.Equals("")) 
            {
                // i confronti vengono fatti sui valori presi da
                // DocsPaVO.filtri.ricercaArgomenti inerenti documenti protocollati
                switch(f.argomento) {
                    case "DATA_CREAZIONE_IL":
                        if (numAndStr > 0)  
                            andStr += " AND ";	
                        numAndStr += 1;
                        andStr += "A.CREATION_DATE=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore,false);				
                        break;	
                    case "DATA_CREAZIONE_SUCCESSIVA_AL":
                        if (numAndStr > 0)  
                            andStr += " AND ";	
                        numAndStr += 1;
                        andStr += "A.CREATION_DATE>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore,false);				
                        break;	
                    case "DATA_CREAZIONE_PRECEDENTE_IL":
                        if (numAndStr > 0)  
                            andStr += " AND ";	
                        numAndStr += 1;
                        andStr += "A.CREATION_DATE<=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore,false);				
                        break;	
                    case "DOCNUMBER":
                        if (numAndStr++ > 0) 
                            andStr += " AND ";
                        andStr += "A.DOCNUMBER=" + f.valore;				
                        break;
                    case "DOCNUMBER_DAL":
                        if (numAndStr++ > 0) 
                            andStr += " AND ";
                        andStr += "A.DOCNUMBER>=" + f.valore;				
                        break;
                    case "DOCNUMBER_AL":
                        if (numAndStr++ > 0) 
                            andStr += " AND ";
                        andStr += "A.DOCNUMBER<=" + f.valore;				
                        break;
                    case "ID_OGGETTO":
                        if (numAndStr > 0)  
                            andStr += " AND ";	
                        numAndStr += 1;
                        andStr += "D.SYSTEM_ID=" + f.valore;				
                        break;
                    case "OGGETTO":
                        if (numAndStr > 0)  
                            andStr += " AND ";	
                        numAndStr += 1;
                        // La stringa di testo contenuta nel campo oggetto è messa in
                        // AND utilizzando come separatore la stringa fissa '&&'
                        Regex regex = new Regex("&&");
                        string[] lista = regex.Split(f.valore);
                        andStr += "D.VAR_DESC_OGGETTO LIKE '%" + lista[0] + "%'";
                        for (int i=1; i < lista.Length; i++)
                            andStr += " AND D.VAR_DESC_OGGETTO LIKE '%" + lista[i] + "%'";				
                        break;
                    case "REGISTRO":
                        if (numAndStr > 0)  
                            andStr += " AND ";	
                        numAndStr += 1;
                        andStr += "A.ID_REGISTRO IN (" + f.valore + ")";				
                        break;
                    case "TIPO":
                        if (numAndStr++ > 0) 
                            andStr += " AND ";
                        if(f.valore.Equals("T"))
                            andStr += "A.CHA_TIPO_PROTO IN ('A', 'P')";
                        else
                            andStr += "A.CHA_TIPO_PROTO='" + f.valore + "'";				
                        break;
                    case "TIPO_DOCUMENTO":
                        //query. addJoin("DOCUMENTTYPES H", "A.DOCUMENTTYPE=H.SYSTEM_ID");								
                        queryWhere = " AND A.DOCUMENTTYPE=H.SYSTEM_ID";
                        queryFrom = " , DOCUMENTTYPES H";
                        if (numAndStr > 0)  
                            andStr += " AND ";	
                        numAndStr += 1;
                        andStr += "H.TYPE_ID='" + f.valore + "'";				
                        break;
                    case "PAROLE_CHIAVE":
                        if (numAndStr > 0)  
                            andStr += " AND ";	
                        numAndStr += 1;
                        andStr += "A.SYSTEM_ID IN (SELECT ID_PROFILE FROM DPA_PROF_PAROLE WHERE ID_PAROLA IN (" + f.valore + "))";
                        break;
                    case "TIPO_ATTO":
                        if (numAndStr > 0)  
                            andStr += " AND ";	
                        numAndStr += 1;
                        andStr += "A.ID_TIPO_ATTO='" + f.valore + "'";				
                        break;	
                    case "NOTE":
                        if (numAndStr > 0)  
                            andStr += " AND ";	
                        numAndStr += 1;
                        andStr += "UPPER(A.VAR_NOTE) LIKE '%" + f.valore.ToUpper().Replace("'","''") + "%'";				
                        break;
                    case "FIRMATARIO_NOME":
                        /*query. addJoin("VERSIONS L", "L.DOCNUMBER=A.DOCNUMBER");
                        query. addJoin("DPA_FIRMA_VERS M", "M.ID_VERSIONE=L.VERSION_ID");								
                        query. addJoin("DPA_FIRMATARI N", "M.ID_FIRMATARIO=N.SYSTEM_ID");*/
        /*queryWhere = " AND L.DOCNUMBER=A.DOCNUMBER AND M.ID_VERSIONE=L.VERSION_ID AND M.ID_FIRMATARIO=N.SYSTEM_ID";
        queryFrom = " , VERSIONS L , DPA_FIRMA_VERS M, DPA_FIRMATARI N";
        if (numAndStr > 0)  
            andStr += " AND ";	
        numAndStr += 1;
        andStr += "UPPER(N.VAR_NOME) LIKE '%" + f.valore.ToUpper() + "%'";				
        break;
    case "FIRMATARIO_COGNOME":
        /*query. addJoin("VERSIONS L", "L.DOCNUMBER=A.DOCNUMBER");
        query. addJoin("DPA_FIRMA_VERS M", "M.ID_VERSIONE=L.VERSION_ID");								
        query. addJoin("DPA_FIRMATARI N", "M.ID_FIRMATARIO=N.SYSTEM_ID");	*/
        /*queryWhere = " AND L.DOCNUMBER=A.DOCNUMBER AND M.ID_VERSIONE=L.VERSION_ID AND M.ID_FIRMATARIO=N.SYSTEM_ID";
        queryFrom = " , VERSIONS L, DPA_FIRMA_VERS M, DPA_FIRMATARI N";
        if (numAndStr > 0)  
            andStr += " AND ";	
        numAndStr += 1;
        andStr += "UPPER(N.VAR_COGNOME) LIKE '%" + f.valore.ToUpper() + "%'";				
        break;
    case "EVIDENZA":
        if (numAndStr > 0)  
            andStr += " AND ";	
        numAndStr += 1;
        andStr += "A.CHA_EVIDENZA='" + f.valore + "'";				
        break;	
					
}
}
switch(f.argomento) 
{
case "MANCANZA_IMMAGINE":
    if (numAndStr > 0)  
        andStr += " AND ";	
    numAndStr += 1;
    andStr += "A.CHA_IMG='0'";				
    break;
case "MANCANZA_ASSEGNAZIONE":
    if (numAndStr > 0)  
        andStr += " AND ";	
    numAndStr += 1;
    andStr += "A.CHA_ASSEGNATO='0'";				
    break;
case "MANCANZA_FASCICOLAZIONE":
    if (numAndStr > 0)  
        andStr += " AND ";	
    numAndStr += 1;
    andStr += "A.SYSTEM_ID NOT IN (SELECT LINK FROM PROJECT_COMPONENTS WHERE TYPE='D')";				
    break;
}
return queryWhere;
}*/
        #endregion

        #region Ricerca Full Text

        /// <summary>
        /// Funzionalità di ricerca FullText
        /// </summary>
        /// <param name="testo"></param>
        /// <param name="libreria"></param>
        /// <param name="dst"></param>
        /// <param name="idReg"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static ArrayList RicercaFullText(string testo,
                                         string idReg,
                                         InfoUtente infoUtente,
                                         DocsPaVO.filtri.FiltroRicerca[][] objQueryList,
                                         int numPage,
                                         out int numTotPage,
                                         out int nRec)
        {
            numTotPage = 0;
            nRec = 0;

            DocsPaDocumentale.Documentale.FullTextSearchManager fullTextSearchManager = new DocsPaDocumentale.Documentale.FullTextSearchManager(infoUtente);
            return fullTextSearchManager.FullTextSearch(testo, idReg, numPage, out numTotPage, out nRec);
        }

        /// <summary>
        /// Funzionalità di ricerca FullText
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ArrayList FullTextSearch(InfoUtente infoUtente, ref DocsPaVO.ricerche.FullTextSearchContext context)
        {
            DocsPaDocumentale.Documentale.FullTextSearchManager fullTextSearchManager = new DocsPaDocumentale.Documentale.FullTextSearchManager(infoUtente);
            return fullTextSearchManager.FullTextSearch(ref context);
        }

        /// <summary>
        /// Restituzione del numero massimo di record che è possibile far restituire dalla ricerca fulltext
        /// </summary>
        /// <returns></returns>
        public static int FullTextSearchMaxRows(InfoUtente infoUtente)
        {
            DocsPaDocumentale.Documentale.FullTextSearchManager fullTextSearchManager = new DocsPaDocumentale.Documentale.FullTextSearchManager(infoUtente);
            return fullTextSearchManager.GetMaxRowCount();
        }

        /// <summary>
        /// Funzionalità di ricerca FullText
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ArrayList simpleFullTextSearch(InfoUtente infoUtente, ref DocsPaVO.ricerche.FullTextSearchContext context)
        {
            DocsPaDocumentale.Documentale.FullTextSearchManager fullTextSearchManager = new DocsPaDocumentale.Documentale.FullTextSearchManager(infoUtente);
            return fullTextSearchManager.simpleFullTextSearch(ref context);
        }

        #endregion

        #region Salvataggio Ricerche
        public static DocsPaVO.ricerche.SearchItem SaveSearchItem(DocsPaVO.ricerche.SearchItem item, bool ADL, bool customGrid, string gridId, Grid tempGrid, DocsPaVO.utente.InfoUtente infoUser, Grid.GridTypeEnumeration gridType)
        {
            string tipoRic = string.Empty;
            switch (item.tipo)
            {
                case "D":
                    tipoRic = "documenti";
                    break;

                case "F":
                    tipoRic = "fascicoli";
                    break;

                case "T":
                    tipoRic = "trasmissioni";
                    break;
            }

            string log = "Salvataggio ricerca '" + item.descrizione + "' di tipo '" + tipoRic + "' sulla pagina '" + item.pagina + "' per ";
            log += (item.owner_idPeople != 0) ? "l'utente '" + item.owner_idPeople + "' " : " ";
            log += (item.owner_idGruppo != 0) ? "il ruolo '" + item.owner_idGruppo + "' " : " ";
            logger.Debug("SaveSearchItem - " + log);

            if (item.system_id != 0)
            {
                string msg = "I criteri di ricerca non possono essere salvati su una voce esistente.";
                logger.Debug(msg);
                throw new Exception(msg);
            }

            if (ExistsSearchItem(item))
            {
                string msg = "I criteri di ricerca sono gia\' stati utilizzati. Utilizzare un diverso nome.";
                logger.Debug(msg);
                throw new Exception(msg);
            }

            string queryName = "";
            string sql = "";
            string searchName = "";

            DocsPaDB.DBProvider provider = null;
            try
            {
                string id = null;
                try
                {
                    queryName = "I_DPA_SALVA_RICERCHE";
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery(queryName);
                    id = DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_SALVA_RICERCHE");
                    if (id != null && id != "")
                        q.setParam("param1", id);
                    searchName = item.descrizione;
                    q.setParam("param2", (item.descrizione != null) ? "'" + item.descrizione.Replace("'", "''") + "'" : "null");
                    q.setParam("param3", (item.owner_idPeople != 0) ? item.owner_idPeople.ToString() : "null");
                    q.setParam("param4", (item.owner_idGruppo != 0) ? item.owner_idGruppo.ToString() : "null");
                    q.setParam("param5", (item.pagina != null) ? "'" + item.pagina + "'" : "null");
                    //ADL
                    if (!ADL) q.setParam("param6", "'0'");
                    else q.setParam("param6", "'1'");
                    q.setParam("param7", item.tipo);

                    string idGriglia = "''";

                    if (customGrid)
                    {
                        if (!string.IsNullOrEmpty(gridId))
                        {
                            if (gridId.Equals("-2"))
                            {
                                //Crea la griglia per la ricerca
                                //idGriglia = newId appena creato
                                idGriglia = BusinessLogic.Grids.GridManager.SaveTempGridRapidSearch(tempGrid, infoUser, searchName, gridType);
                            }
                            else
                            {
                                idGriglia = gridId;
                            }
                        }
                        item.gridId = idGriglia;
                    }
                    //id della griglia se presente
                    q.setParam("param8", idGriglia);

                    sql = q.getSQL();
                    logger.Debug(queryName);
                    logger.Debug(sql);
                }
                catch (Exception)
                {
                    throw new Exception("Fallita la preparazione della query al database");
                }

                try
                {
                    provider = new DocsPaDB.DBProvider();
                    provider.BeginTransaction();
                    int r = 0;
                    provider.ExecuteNonQuery(sql, out r);
                    if (r != 0)
                    {
                        sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_SALVA_RICERCHE");
                        logger.Debug(sql);

                        using (System.Data.IDataReader reader = provider.ExecuteReader(sql))
                        {
                            while (reader.Read())
                            {
                                id = reader.GetValue(0).ToString();
                            }
                        }
                        if (id == null)
                            throw new Exception("Fallita identificazione della riga inserita");
                        item.system_id = Int32.Parse(id);

                        bool resUpdate = false;
                        if (item.filtri != null)
                        {
                            resUpdate = provider.SetLargeText("DPA_SALVA_RICERCHE", id, "VAR_FILTRI_RIC", item.filtri);
                        }
                        item.system_id = Int32.Parse(id);

                        if (resUpdate)
                        {
                            provider.CommitTransaction();
                            logger.Debug("SaveSearchItem - Effettuato salvataggio della ricerca " + item.ToString());
                            return item;
                        }
                        else
                        {
                            logger.Debug("SaveSearchItem - Salvataggio della ricerca fallito: Errore SetLargeText");
                            provider.RollbackTransaction();
                            return null;
                        }
                    }
                    else
                    {
                        provider.RollbackTransaction();
                        throw new Exception("Errore generico durante l'inserimento");
                    }
                }
                catch (Exception ex)
                {
                    provider.RollbackTransaction();
                    string msg = "Operazione sul database fallita. Errore: " + ex.Message;
                    logger.Debug(msg);
                    throw new Exception(msg);
                }
            }
            catch (Exception e)
            {
                logger.Debug("SaveSearchItem - Salvataggio della ricerca fallito: " + e.Message);
                provider.RollbackTransaction();
                throw e;
            }
            return null;
        }

        public static DocsPaVO.ricerche.SearchItem[] GetSearchList(int idPeople, int idGruppo, string pgName, bool inADL, string tipo)
        {
            try
            {
                logger.Debug("GetSearchList - Selezione delle ricerche per:\n" +
                    "idPeople = " + idPeople + "\n" +
                    "idGruppo = " + idGruppo + "\n" +
                    "Pagina = " + pgName + "\n" +
                    "Tipo = " + tipo);

                ArrayList list = new ArrayList();
                string queryName = string.Empty;

                queryName = (pgName != null && pgName != "") ? "S_DPA_SALVA_RICERCHE_LIST" : "S_DPA_SALVA_RICERCHE_LIST_NOPAGE";

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery(queryName);
                if (pgName != null && pgName != "")
                    q.setParam("param1", "'" + pgName + "'");
                q.setParam("param2", idPeople.ToString());
                q.setParam("param3", idGruppo.ToString());
                // nuova ADL
                if (inADL) q.setParam("param4", " and sr.CHA_IN_ADL='1'");
                else q.setParam("param4", " and sr.CHA_IN_ADL='0'");
                q.setParam("param5", " and sr.TIPO = '" + tipo + "'");

                string sql = q.getSQL();
                logger.Debug(queryName);
                logger.Debug(sql);

                DocsPaDB.DBProvider provider = new DocsPaDB.DBProvider();
                DataSet ds = new DataSet();
                provider.ExecuteQuery(out ds, sql);
                logger.Debug("GetSearchList - Trovati " + ds.Tables[0].Rows.Count + " risultati.");
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DocsPaVO.ricerche.SearchItem item = new DocsPaVO.ricerche.SearchItem();
                    item.system_id = Int32.Parse(dr["system_id"].ToString());
                    item.descrizione = (string)dr["var_descrizione"];
                    list.Add(item);
                }

                DocsPaVO.ricerche.SearchItem[] outcome = new DocsPaVO.ricerche.SearchItem[list.Count];
                list.CopyTo(outcome);
                return outcome;
            }
            catch (Exception e)
            {
                logger.Debug("Eccezione: " + e.Message);
                throw e;
            }
            return null;
        }

        public static bool DeleteSearchItem(int system_id)
        {
            try
            {
                logger.Debug("DeleteSearchItem - Cancellazione della ricerca: " + system_id);
                string queryName = "D_DPA_SALVA_RICERCHE_ID";
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery(queryName);
                q.setParam("param1", system_id.ToString());
                string sql = q.getSQL();
                logger.Debug(queryName);
                logger.Debug(sql);

                DocsPaDB.DBProvider provider = new DocsPaDB.DBProvider();
                int r = 0;
                provider.ExecuteNonQuery(sql, out r);
                if (r != 0)
                {
                    logger.Debug("DeleteSearchItem - Ricerca cancellata.");
                    return true;
                }
            }
            catch (Exception e)
            {

                throw e;
            }

            logger.Debug("DeleteSearchItem - Ricerca non cancellata.");
            return false;
        }

        public static DocsPaVO.ricerche.SearchItem GetSearchItem(int system_id)
        {
            try
            {
                logger.Debug("GetSearchItem - Recupero della ricerca: " + system_id);

                string queryName = "S_DPA_SALVA_RICERCHE_ID";
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery(queryName);
                q.setParam("param1", system_id.ToString());
                string sql = q.getSQL();
                logger.Debug(queryName);
                logger.Debug(sql);

                DocsPaDB.DBProvider provider = new DocsPaDB.DBProvider();
                DataSet ds = new DataSet();
                provider.ExecuteQuery(out ds, sql);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    DocsPaVO.ricerche.SearchItem item = new DocsPaVO.ricerche.SearchItem();
                    item.system_id = Int32.Parse(dr["system_id"].ToString());
                    item.descrizione = (string)dr["var_descrizione"];
                    string aux = dr["id_people"].ToString();
                    item.owner_idPeople = (aux != null && aux != "") ? Int32.Parse(aux) : 0;
                    aux = dr["id_gruppo"].ToString();
                    item.owner_idGruppo = (aux != null && aux != "") ? Int32.Parse(aux) : 0;
                    item.pagina = (string)dr["var_pagina_ric"];
                    item.filtri = provider.GetLargeText("DPA_SALVA_RICERCHE", dr["system_id"].ToString(), "VAR_FILTRI_RIC");
                    item.tipo = (string)dr["TIPO"];
                    if (dr["GRID_ID"] != null)
                    {
                        item.gridId = dr["GRID_ID"].ToString();
                    }
                    else
                    {
                        item.gridId = string.Empty;
                    }

                    logger.Debug("GetSearchItem - trovata ricerca: " + item.ToString());
                    return item;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            logger.Debug("GetSearchItem - Ricerca non trovata.");
            return null;
        }

        public static bool ExistsSearchName(string nome, DocsPaVO.utente.InfoUtente infoUtente, string pagina, string adl)
        {
            try
            {
                logger.Debug("ExistsSearchName - Ricerca da verificare: " + nome);
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_SALVA_RICERCHE_COUNT_NAME");
                // q.setParam("param1", "'" + nome.ToUpper().Replace("'", "''") + "'");
                q.setParam("param1", nome.ToUpper().Replace("'", "''"));
                q.setParam("idPeople", infoUtente.idPeople);
                q.setParam("idGruppo", infoUtente.idGruppo);
                q.setParam("pagina", pagina);
                q.setParam("adl", adl);
                string sql = q.getSQL();
                logger.Debug(sql);

                DocsPaDB.DBProvider provider = new DocsPaDB.DBProvider();
                string cnt = null;
                provider.ExecuteScalar(out cnt, sql);
                if (Int32.Parse(cnt) != 0)
                {
                    logger.Debug("ExistsSearchItem - Ricerca trovata.");
                    return true;
                }
            }
            catch (Exception e)
            {

            }
            logger.Debug("ExistsSearchName - Ricerca non trovata.");
            return false;
        }

        public static bool ExistsSearchItem(DocsPaVO.ricerche.SearchItem item)
        {
            if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "FE_VERIFICA_NOME")) && DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "FE_VERIFICA_NOME").Equals("1"))
            {
                return false;
            }
            else
            {
                try
                {
                    logger.Debug("ExistsSearchItem - Ricerca da verificare: " + item.ToString());

                    string queryName = "S_DPA_SALVA_RICERCHE_COUNT";
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery(queryName);
                    q.setParam("param1", "'" + item.descrizione.Replace("'", "''") + "'");
                    q.setParam("param2", item.owner_idPeople.ToString());
                    q.setParam("param3", item.owner_idGruppo.ToString());
                    q.setParam("param4", item.tipo);
                    string sql = q.getSQL();
                    logger.Debug(queryName);
                    logger.Debug(sql);

                    DocsPaDB.DBProvider provider = new DocsPaDB.DBProvider();
                    string cnt = null;
                    provider.ExecuteScalar(out cnt, sql);
                    if (Int32.Parse(cnt) != 0)
                    {
                        logger.Debug("ExistsSearchItem - Ricerca trovata.");
                        return true;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            logger.Debug("ExistsSearchItem - Ricerca non trovata.");
            return false;
        }

        #endregion Salvataggio Ricerche

        #region DatiProtocollo per FrameVisibilità
        public static int DO_getIdProfileByData(out string inArchivio, string numProto, string AnnoProto, string idRegistro, DocsPaVO.utente.InfoUtente infoUtente)
        {
            inArchivio = "-1";
            string result = String.Empty;
            string commandText = String.Empty;
            DocsPaUtils.Query queryMngArchivio;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();
            if ((AnnoProto == null) && (idRegistro == null))
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DO_GET_IDPROFILE_BY_DATA_DOC_GRIGIO");
                if (dbProvider.DBType.ToUpper() == "SQL")
                {
                    string userDB = DocsPaDbManagement.Functions.Functions.GetDbUserSession();
                    if (!string.IsNullOrEmpty(userDB))
                        queryMng.setParam("dbuser", userDB);
                }

                queryMng.setParam("ID_DOC", numProto);
                queryMng.setParam("idGruppo", infoUtente.idGruppo);
                queryMng.setParam("idPeople", infoUtente.idPeople);

                //if (!obj.isFiltroAooEnabled())
                //{
                //    queryMng.setParam("security", " and (@dbuser@.checkSecurityDocumento(P.SYSTEM_ID, @idPeople@, @idGruppo@,'D') > 0)");
                //}
                //else
                //    queryMng.setParam("security", "");

                commandText = queryMng.getSQL();
            }
            else
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DO_GET_IDPROFILE_BY_DATA");
                if (dbProvider.DBType.ToUpper() == "SQL")
                {
                    string userDB = DocsPaDbManagement.Functions.Functions.GetDbUserSession();
                    if (!string.IsNullOrEmpty(userDB))
                        queryMng.setParam("dbuser", userDB);
                }
                if (string.IsNullOrEmpty(numProto)) numProto = "0";
                queryMng.setParam("NUMPROTO", numProto);
                queryMng.setParam("ANNOPROTO", AnnoProto);
                queryMng.setParam("IDREGISTRO", idRegistro);
                queryMng.setParam("idGruppo", infoUtente.idGruppo);
                queryMng.setParam("idPeople", infoUtente.idPeople);
                //if (!obj.isFiltroAooEnabled())
                //{
                //    queryMng.setParam("security", " and (@dbuser@.checkSecurityDocumento(P.SYSTEM_ID, @idPeople@, @idGruppo@,'D') > 0)");
                //}
                //else
                //    queryMng.setParam("security", "");
                commandText = queryMng.getSQL();
            }

            logger.Debug("InfoDocManager - DO_GET_IDPROFILE_BY_DATA: " + commandText);
            dbProvider.ExecuteScalar(out result, commandText);


            queryMngArchivio = DocsPaUtils.InitQuery.getInstance().getQuery("S_DOC_INARCHIVIO");
            queryMngArchivio.setParam("ID_DOC", numProto);
            commandText = queryMngArchivio.getSQL();
            dbProvider.ExecuteScalar(out inArchivio, commandText);

            return Convert.ToInt32(result);
        }
        #endregion

        #region Trasferimento in deposito
        /// <summary>
        /// Restituisce la lista di una serie di documenti repertoriati
        /// </summary>
        /// <param name="template"></param>
        /// <param name="numPage"></param>
        /// <param name="numTotPage"></param>
        /// <param name="nRec"></param>
        /// <returns></returns>
        public static ArrayList getDocInSerie(string idGruppo, DocsPaVO.ProfilazioneDinamica.Templates template, int numPage, out int numTotPage, out int nRec, string oggetto, string anno, string rfAOO)
        //public static ArrayList getDocInSerie(DocsPaVO.ProfilazioneDinamica.Templates template, out int nRec)
        {
            numTotPage = 0;
            nRec = 0;
            ArrayList listaInfoDoc = new ArrayList();
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            try
            {
                listaInfoDoc = doc.ListaDocumentiInSeriePaging(idGruppo, listaInfoDoc, template, numPage, out numTotPage, out nRec, oggetto, anno, rfAOO);
                //listaInfoDoc = doc.ListaDocumentiInSerie(listaInfoDoc, template, out nRec);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                logger.Debug("Errore nella gestione dell'InfoDocManager (getDocInSerie)", e);
                throw new Exception("F_System");
            }
            return listaInfoDoc;
        }

        #endregion

        #region Date per le ricerche
        public static string getFirstDayOfWeek()
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.getFirstDayOfWeek();
        }

        public static string getFirstDayOfMonth()
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.getFirstDayOfMonth();
        }

        public static string getLastDayOfWeek()
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.getLastDayOfWeek();
        }

        public static string getLastDayOfMonth()
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.getLastDayOfMonth();
        }

        public static string toDay()
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.toDay();
        }

        public static string toFirstDayOfYear()
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.toFirstDayOfYear();
        }

        public static string GetYesterday()
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.GetYesterday();
        }

        public static string GetLastSevenDay()
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.GetLastSevenDay();
        }

        public static string GetLastThirtyOneDay()
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.GetLastThirtyOneDay();
        }
        #endregion

        public static DocsPaVO.utente.Canale associaCanalePrefDestinatario(string idProfile, string idDestinatario)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.associaCanalePrefDestinatario(idProfile, idDestinatario);
        }

        public static DocsPaVO.utente.Canale getCanaleBySystemId(string idDocumenttypes)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.getCanaleBySystemId(idDocumenttypes);
        }
        public static int getNumFolderDoc(string idProfile)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.getNumFolderDoc(idProfile);
        }
        public static string getIdMezzoSpedizioneByDesc(string desc)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.getIdMezzoSpedizioneByDesc(desc);
        }
        public static ArrayList getQueryExportAsRic(string idGruppo, string idPeople,
       DocsPaVO.filtri.FiltroRicerca[][] objQueryList, bool security, String[] documentsSystemId)
        {
            ArrayList listaInfoDoc = new ArrayList();
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            try
            {

                listaInfoDoc = doc.ListaDocumentiExportAsRic(idGruppo, idPeople, listaInfoDoc, objQueryList, security, documentsSystemId);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();
                logger.Debug("Errore nella gestione dell'InfoDocManager (getQueryPaging)", e);
                throw new Exception("F_System");
            }

            return listaInfoDoc;
        }

        public static bool ExistsSearchNameModify(string nome, DocsPaVO.utente.InfoUtente infoUtente, string pagina, string idRicerca)
        {
            try
            {
                logger.Debug("ExistsSearchName - Ricerca da verificare: " + nome);
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_SALVA_RICERCHE_COUNT_NAME_MODIFY");
                // q.setParam("param1", "'" + nome.ToUpper().Replace("'", "''") + "'");
                q.setParam("param1", nome.ToUpper().Replace("'", "''"));
                q.setParam("idPeople", infoUtente.idPeople);
                q.setParam("idGruppo", infoUtente.idGruppo);
                q.setParam("pagina", pagina);
                q.setParam("system_id", idRicerca);
                string sql = q.getSQL();
                logger.Debug(sql);

                DocsPaDB.DBProvider provider = new DocsPaDB.DBProvider();
                string cnt = null;
                provider.ExecuteScalar(out cnt, sql);
                if (Int32.Parse(cnt) != 0)
                {
                    logger.Debug("ExistsSearchItem - Ricerca trovata.");
                    return true;
                }
            }
            catch (Exception e)
            {

            }
            logger.Debug("ExistsSearchName - Ricerca non trovata.");
            return false;
        }

        public static DocsPaVO.ricerche.SearchItem ModifySearchItem(DocsPaVO.ricerche.SearchItem item, bool customGrid, string gridId, Grid tempGrid, DocsPaVO.utente.InfoUtente infoUser, Grid.GridTypeEnumeration gridType)
        {
            string queryName = "";
            string sql = "";
            string searchName = "";
            DocsPaDB.DBProvider provider = null;
            try
            {
                queryName = "U_DPA_MODIFICA_RICERCHE";
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_MODIFICA_RICERCHE");
                q.setParam("idRicerca", (item.system_id).ToString());
                q.setParam("descrizione", item.descrizione);
                searchName = item.descrizione;
                if (item.owner_idPeople != 0)
                {
                    q.setParam("idPeople", item.owner_idPeople.ToString());
                }
                else
                {
                    q.setParam("idPeople", "-1");
                }
                if (item.owner_idGruppo != 0)
                {
                    q.setParam("idGruppo", item.owner_idGruppo.ToString());
                }
                else
                {
                    q.setParam("idGruppo", "-1");
                }


                string idGriglia = "''";

                if (customGrid)
                {
                    if (!string.IsNullOrEmpty(gridId))
                    {
                        if (gridId.Equals("-2"))
                        {
                            //Crea la griglia per la ricerca
                            //idGriglia = newId appena creato
                            idGriglia = BusinessLogic.Grids.GridManager.SaveTempGridRapidSearch(tempGrid, infoUser, searchName, gridType);
                        }
                        else
                        {
                            idGriglia = gridId;
                        }
                    }
                    item.gridId = idGriglia;
                }
                //id della griglia se presente
                q.setParam("grid_id", idGriglia);

                sql = q.getSQL();
                logger.Debug(sql);

                provider = new DocsPaDB.DBProvider();
                provider.BeginTransaction();
                int r = 0;
                provider.ExecuteNonQuery(sql, out r);
                if (item.filtri != null)
                {
                    provider.SetLargeText("DPA_SALVA_RICERCHE", (item.system_id).ToString(), "VAR_FILTRI_RIC", item.filtri);
                }
                provider.CommitTransaction();
                logger.Debug("SaveSearchItem - Effettuato salvataggio della ricerca " + item.ToString());
                return item;
            }

            catch (Exception)
            {
                throw new Exception("Fallita la preparazione della query al database");
                return null;
            }

        }

        public static ArrayList getQueryPagingCustom(DocsPaVO.utente.InfoUtente infoUtente,
              DocsPaVO.filtri.FiltroRicerca[][] objQueryList,
              int numPage, int pageSize, bool security, bool export, bool gridPersonalization, Field[] visibleFieldsTemplate, String[] documentsSystemId, out int numTotPage, out int nRec, bool getIdProfilesList, out List<SearchResultInfo> idProfileList)
        {
            ArrayList listaInfoDoc = new ArrayList();
            nRec = 0;
            numTotPage = 0;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            try
            {
                //DA FARE
                bool stampaReg = false;
                if (cercaStampeRegistro(objQueryList))
                {
                    stampaReg = true;
                }

                //Ricerca fullText
                bool isFullText = false;
                string searchQuery = string.Empty;
                ArrayList fullTextResult = new ArrayList();
                if (cercaFullText(objQueryList, out searchQuery))
                {
                    isFullText = true;
                    DocsPaVO.ricerche.FullTextSearchContext fTContext = new FullTextSearchContext();
                    fTContext.TextToSearch = searchQuery;
                    string maxDocumentResult = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_FULLTEXT_MAXROWS");
                    fTContext.TotalRecordCount = string.IsNullOrEmpty(maxDocumentResult) ? 200 : int.Parse(maxDocumentResult);

                    fullTextResult = simpleFullTextSearch(infoUtente, ref fTContext);
                    logger.Debug("Risultati dalla ricerca FULLTEXT = " + fullTextResult.Count.ToString());
                }

                listaInfoDoc = doc.ListaDocumentiPagingCustom(infoUtente, listaInfoDoc, objQueryList, numPage, pageSize, security, out numTotPage, out nRec, getIdProfilesList, export, gridPersonalization, visibleFieldsTemplate, documentsSystemId, out idProfileList, stampaReg, isFullText, fullTextResult);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                logger.Debug("Errore nella gestione dell'InfoDocManager (getQueryPagingCustom)", e);
                throw new Exception("F_System");
            }

            return listaInfoDoc;
        }

        #region FullText
        // ricerca FullText

        private static bool cercaFullText(DocsPaVO.filtri.FiltroRicerca[][] objQueryList, out string searchQuery)
        {

            bool result = false;
            searchQuery = string.Empty;
            foreach (DocsPaVO.filtri.FiltroRicerca[] filtri in objQueryList)
            {
                foreach (DocsPaVO.filtri.FiltroRicerca filtro in filtri)
                {
                    switch (filtro.argomento)
                    {
                        case ("SEARCH_DOCUMENT_SIMPLE"):
                            searchQuery = filtro.valore;
                            break;

                        case ("RICERCA_FULL_TEXT"):
                            result = filtro.valore.ToLower().Equals("true") ? true : false;
                            break;
                    }
                }
            }

            return result;

        }


        #endregion

        public static string stopWord(string ricerca)
        {
            string retValue = string.Empty;
            try
            {
                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                retValue = doc.StopWord(ricerca);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                logger.Debug("Errore nella gestione dell'InfoDocManager (stopWord)", e);
                throw new Exception("F_System");
            }
            return retValue;
        }

        /// <summary>
        /// Metodo per il recupero del dettaglio di un mezzo di spedizione a partire dal suo codice
        /// </summary>
        /// <param name="codice">Codice del mezzo di spedizione</param>
        /// <returns>Mezzo di spedizione</returns>
        internal static DocsPaVO.amministrazione.MezzoSpedizione GetMezzoSpedizioneDaCodice(string codice)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.GetMezzoSpedizioneByCodice(codice);
        }

        public static string DayOnYearBeforeToday()
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.DayOnYearBeforeToday();
        }

        public static ArrayList GetModifiedDcoumentAdv(string startDate, string events)
        {
            ArrayList docs = new ArrayList();
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            bool searchAnnullamentoLiquidazione = false;
            
            try
            {
                if (events.ToUpper().Contains("ANNULLATA_LIQ"))
                {
                    searchAnnullamentoLiquidazione = true;
                    events = events.Replace("ANNULLATA_LIQ", "");
                }

                if (events.Contains(";;"))
                {
                    events = events.Replace(";;", ";");
                }

                if (events.EndsWith(";"))
                {
                    events = events.Remove(events.Length - 1);
                }


                string eventString = "(";
                string[] eventsSplitted = events.Trim().Split(';');
                
                for(int i = 0; i < eventsSplitted.Length - 1; i++)
                {
                    eventString += "'";
                    eventString += eventsSplitted[i].ToString();
                    eventString += "',";
                }

                eventString += "'";
                eventString += eventsSplitted[eventsSplitted.Length-1];
                eventString += "'";
                eventString += ")";

                docs = doc.GetModifiedDcoumentAdv(startDate, eventString, searchAnnullamentoLiquidazione);


            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                logger.Debug("Errore nella gestione dell'InfoDocManager (GetModifiedDcoumentAdv)", ex);

            }

            return docs;
        }
    }
}
