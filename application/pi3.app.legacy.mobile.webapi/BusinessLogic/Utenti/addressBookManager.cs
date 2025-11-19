// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Serilog;
using System.Collections;
using System.Data;

namespace BusinessLogic.Utenti;

public class addressBookManager
{
    private static ILogger logger = Serilog.Log.ForContext(typeof(addressBookManager));

    public static ArrayList listaCorrispondentiIntMethod(DocsPaVO.addressbook.QueryCorrispondente qco)
    {
        ArrayList list = new ArrayList();
        DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
        list = utenti.ListaCorrispondentiInt(qco);
        if (list == null)
        {
            logger.Debug("Errore nella gestione degli utenti (listaCorrispondentiIntMethod)");
            throw new Exception();
        }
        return list;

    }

    public static System.Collections.Generic.List<DocsPaVO.utente.MailCorrispondente> GetMailCorrispondente(string idCorrispondente)
    {
        DocsPaDB.Query_DocsPAWS.Utenti ut = new DocsPaDB.Query_DocsPAWS.Utenti();
        DataSet ds = new System.Data.DataSet();
        System.Collections.Generic.List<DocsPaVO.utente.MailCorrispondente> listMailCorr = new System.Collections.Generic.List<DocsPaVO.utente.MailCorrispondente>();
        try
        {
            ds = ut.GetMailCorr(idCorrispondente);
            if (ds != null && ds.Tables["CASELLE_CORRISPONDENTE"].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables["CASELLE_CORRISPONDENTE"].Rows)
                {
                    DocsPaVO.utente.MailCorrispondente mailCorr = new DocsPaVO.utente.MailCorrispondente();
                    mailCorr.systemId = row["SystemId"].ToString();
                    mailCorr.Email = row["Email"].ToString();
                    mailCorr.Principale = row["Principale"].ToString();
                    mailCorr.Note = row["Note"].ToString();
                    listMailCorr.Add(mailCorr);
                }
            }
            return listMailCorr;
        }
        catch (Exception e)
        {
            return new System.Collections.Generic.List<DocsPaVO.utente.MailCorrispondente>();
        }
    }

    /// <summary></summary>
    /// <param name="uo"></param>
    /// <returns>ruoli appartenenti alla UO - TODO: ruoli autorizzati </returns>
    public static ArrayList getRuoliRiferimentoAutorizzati(DocsPaVO.addressbook.QueryCorrispondenteAutorizzato queryCorr, DocsPaVO.utente.UnitaOrganizzativa uo)
    {
        logger.Debug("getRuoliRiferimentoAutorizzati");
        System.Collections.ArrayList ruoli = new System.Collections.ArrayList();
        DataSet dataSet = new DataSet();
        bool openDb = false;
        try
        {
            // TODO: Utilizzare il progetto DocsPaDbManagement
            //database.openConnection();
            openDb = true;

            DocsPaDB.Query_Utils.Utils utils = new DocsPaDB.Query_Utils.Utils();
            utils.GetRuoliRiferimentoAutorizzati(out dataSet, queryCorr, uo);

            if (dataSet.Tables["RUOLI_RIF"] != null)
            {
                for (int i = 0; i < dataSet.Tables["RUOLI_RIF"].Rows.Count; i++)
                {
                    DocsPaVO.utente.Ruolo ruoloRif = new DocsPaVO.utente.Ruolo();
                    System.Data.DataRow ruoloRow = dataSet.Tables["RUOLI_RIF"].Rows[i];
                    ruoloRif.codiceRubrica = ruoloRow["VAR_COD_RUBRICA"].ToString();
                    ruoloRif.descrizione = ruoloRow["VAR_DESC_RUOLO"].ToString();
                    ruoloRif.systemId = ruoloRow["SYSTEM_ID"].ToString();
                    ruoloRif.livello = ruoloRow["NUM_LIVELLO"].ToString();
                    ruoloRif.idGruppo = ruoloRow["ID_GRUPPO"].ToString();
                    ruoloRif.codiceCorrispondente = ruoloRow["VAR_CODICE"].ToString();
                    ruoloRif.idAmministrazione = uo.idAmministrazione;
                    ruoloRif.uo = uo;

                    ruoli.Add(ruoloRif);
                }
            }
            // TODO: Utilizzare il progetto DocsPaDbManagement
            //database.closeConnection();
            openDb = false;
            return ruoli;
        }
        catch (Exception e)
        {
            if (openDb)
            {
                // TODO: Utilizzare il progetto DocsPaDbManagement
                //database.closeConnection();
            }
            logger.Debug(e.Message);
            logger.Debug("Errore nella gestione degli utenti (getRuoliSuperioriInUO)", e);
            throw e;
        }

    }

    public static ArrayList listaCorrispondentiOccMethod(DocsPaVO.addressbook.QueryCorrispondente qco)
    {
        DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
        return utenti.ListaCorrispondentiOcc(qco);

    }

    public static bool VerificaAutorizzazioneRuolo(string idRuolo, string idregistro)
    {
        bool result = false;
        try
        {
            DocsPaDB.Query_DocsPAWS.Utenti ut = new DocsPaDB.Query_DocsPAWS.Utenti();
            result = ut.VerificaRuoloAut(idRuolo, idregistro);
        }
        catch
        {
            logger.Debug("Errore nella verifica del ruolo autorizzato (VerificaAutorizzazioneRuolo)");
            throw new Exception();
        }
        return result;
    }

    public static ArrayList getListaCorrispondenti(DocsPaVO.addressbook.QueryCorrispondente queryCorrispondente)
    {
        ArrayList objListaCorrispondenti = null;
        //metodo per ottenere la lista
        if (queryCorrispondente.codiceGruppo != null || queryCorrispondente.descrizioneGruppo != null)
        {
            //ricerca per campi riguardanti il gruppo
            //commentato perchè non esiste la gestione dei gruppi 07/03/2005
            //objListaCorrispondenti= addressBookManager.getCorrGruppiMethod(queryCorrispondente);
        }
        else
        {
            //ricerca per campi riguardanti UO, ruolo o utente
            if (queryCorrispondente.tipoUtente == DocsPaVO.addressbook.TipoUtente.ESTERNO)
            {
                objListaCorrispondenti = addressBookManager.listaCorrEstSciolti(queryCorrispondente);
            }
            if (queryCorrispondente.tipoUtente == DocsPaVO.addressbook.TipoUtente.INTERNO)
            {
                objListaCorrispondenti = addressBookManager.listaCorrispondentiIntMethod(queryCorrispondente);
            }
            if (queryCorrispondente.tipoUtente == DocsPaVO.addressbook.TipoUtente.GLOBALE)
            {
                ArrayList objListaCorrispondentiInt = addressBookManager.listaCorrispondentiIntMethod(queryCorrispondente);
                logger.Debug("Corr int:" + objListaCorrispondentiInt.Count);
                if ((queryCorrispondente.codiceRubrica != null && queryCorrispondente.codiceRubrica != "") && objListaCorrispondentiInt.Count > 0)
                {
                    objListaCorrispondenti = objListaCorrispondentiInt;
                }
                else
                {
                    objListaCorrispondenti = addressBookManager.listaCorrEstSciolti(queryCorrispondente);
                    logger.Debug("Corr est:" + objListaCorrispondenti.Count);
                    for (int i = 0; i < objListaCorrispondentiInt.Count; i++)
                    {
                        objListaCorrispondenti.Add(objListaCorrispondentiInt[i]);
                    }
                }
            }
            //se il codice rubrica non è nullo, ricerca anche dei gruppi
            //commentata perchè non esiste la gestione dei gruppi 07/03/2005
            /*if(queryCorrispondente.codiceRubrica!=null) 
            {
                logger.Debug("Ricerca nei gruppi");
                ArrayList objListaCorrGruppo=addressBookManager.getCorrGruppiMethod(queryCorrispondente);
                for(int i=0;i<objListaCorrGruppo.Count;i++) 
                {
                    objListaCorrispondenti.Add(objListaCorrGruppo[i]);
                }
            }*/
        }
        return objListaCorrispondenti;
    }

    public static ArrayList listaCorrEstSciolti(DocsPaVO.addressbook.QueryCorrispondente qco)
    {
        DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
        ArrayList lista = utenti.ListaCorrEstSciolti(qco);
        if (lista == null)
        {
            logger.Debug("Errore nella gestione degli utenti (listaCorrEstSciolti)");
            throw new Exception();
        }
        return lista;

        #region Codice Commentato
        /*logger.Debug("listaCorrEstSciolti");
			ArrayList listaCorr = listaCorrispondentiEstMethod(qco);
			ArrayList listaSciolti = listaUtentiScioltiMethod(qco);
			ArrayList temp = new ArrayList();
			for(int i=0;i<listaSciolti.Count;i++)
			{
				DocsPaVO.utente.Utente ut=(DocsPaVO.utente.Utente) listaSciolti[i];
				logger.Debug("Utente sciolto: "+ut.systemId);
				bool isInEst=false;
				for(int k=0;k<listaCorr.Count;k++)
				{
					if(ut.systemId.Equals(((DocsPaVO.utente.Corrispondente)listaCorr[k]).systemId)) isInEst=true;
				}
				if(!isInEst) temp.Add(ut);
			}
			for(int j=0;j<temp.Count;j++)
			{
				logger.Debug("Aggiunto utente sciolto");
				listaCorr.Add(temp[j]);
			}
			return listaCorr;*/
        #endregion
    }

    public static ArrayList GetParentUo(string idUo, string livelloUO)
    {
        try
        {
            DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
            return gerarchia.getChildrenUO(idUo, livelloUO);
        }
        catch
        {
            logger.Debug("Errore nel reperimento dei figli di una Uo: metodo getChildrenUO");
            throw new Exception();
        }
    }

    public static bool isCorrispondenteValido(string idCorrispondente)
    {
        DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
        return utenti.isCorrispondenteValido(idCorrispondente);
    }

    public static ArrayList getListaCorrispondentiAutProtInt(DocsPaVO.addressbook.QueryCorrispondente queryCorrispondente)
    {
        ArrayList objListaCorrispondenti = null;

        if (queryCorrispondente.tipoUtente == DocsPaVO.addressbook.TipoUtente.INTERNO)
        {
            objListaCorrispondenti = addressBookManager.listaCorrispondentiInt_Aut_Method(queryCorrispondente);
        }

        return objListaCorrispondenti;
    }

    public static ArrayList listaCorrispondentiInt_Aut_Method(DocsPaVO.addressbook.QueryCorrispondente qco)
    {
        ArrayList list = new ArrayList();
        DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
        DocsPaVO.trasmissione.RagioneTrasmissione ragTrasm = null;
        ragTrasm = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("TO", qco.idAmministrazione);
        list = utenti.ListaCorrispondentiInt_Aut(qco, ragTrasm);
        if (list == null)
        {
            logger.Debug("Errore nella gestione degli utenti (listaCorrispondentiInt_Aut_Method)");
            throw new Exception();
        }
        return list;

    }
}
