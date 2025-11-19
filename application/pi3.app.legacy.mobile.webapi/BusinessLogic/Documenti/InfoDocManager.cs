// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using BusinessLogic.Fascicoli;
using DocsPaVO.ricerche;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Documenti;

public class InfoDocManager
{
    private static ILogger logger = Log.ForContext(typeof(InfoDocManager));

    public static string getIdMezzoSpedizioneByDesc(string desc)
    {
        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        return doc.getIdMezzoSpedizioneByDesc(desc);
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

}
