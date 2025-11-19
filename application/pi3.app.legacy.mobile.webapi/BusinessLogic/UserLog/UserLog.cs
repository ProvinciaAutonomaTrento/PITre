// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Serilog;
using System.Data;
using System.Xml;

namespace BusinessLogic.UserLog;

public class UserLog
{
    private static ILogger logger = Serilog.Log.ForContext(typeof(UserLog));

    public static bool WriteLog(DocsPaVO.utente.InfoUtente infoUtente, string WebMethodName, string ID_Oggetto, string Var_desc_Oggetto, DocsPaVO.Logger.CodAzione.Esito Cha_Esito, string idAmm)
    {
        return WriteUserLOG(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, idAmm, WebMethodName, ID_Oggetto, Var_desc_Oggetto, Cha_Esito, infoUtente.delegato, string.Empty);
    }

    private static bool WriteUserLOG(string UserID_Operatore, string ID_People_Operatore, string ID_Gruppo_Operatore, string ID_Amministrazione,
        string WebMethodName, string ID_Oggetto, string Var_desc_Oggetto, DocsPaVO.Logger.CodAzione.Esito Cha_Esito,
        DocsPaVO.utente.InfoUtente delegato, string codWorkingApplication, string checkNotify = "", string id_trasm = "", string dataAzione = "")
    {

        bool result = true;
        DocsPaVO.Logger.CodAzione.infoOggetto InfoOggetto;

        try
        {
            logger.Debug("Evento: " + WebMethodName + "\nIdObject: " + ID_Oggetto);

            //logger.Debug("WriteLog -UserLog-");
            //Reperisco le informazioni relative al WebMethod.

            InfoOggetto = DocsPaDB.Query_DocsPAWS.Log.getInfoOggetto(WebMethodName, ID_Amministrazione);

            // Intercetto eventi di firma per passaggi di stato
            if (WebMethodName.ToUpper() == "DOC_SIGNATURE" || WebMethodName.ToUpper() == "DOC_SIGNATURE_P")
            {
                Procedimenti.ProcedimentiManager.CheckEventiFirma(ID_Oggetto, ID_People_Operatore, ID_Gruppo_Operatore, ID_Amministrazione);
            }

            //Insert sulla DPA_LOG.
            if (InfoOggetto.Attivo == 1)
            {
                //se il tipo di evento non è notificabile allora imposto cheNotify = string.empy
                //il servizio windows per la generazione degli eventi non deve processare questo evento
                if (string.IsNullOrEmpty(InfoOggetto.Notify) || InfoOggetto.Notify.Equals("NN"))
                    checkNotify = string.Empty;

                result = DocsPaDB.Query_DocsPAWS.Log.InsertLog(UserID_Operatore, ID_People_Operatore, ID_Gruppo_Operatore, ID_Amministrazione,
                        InfoOggetto.Oggetto.Trim(), ID_Oggetto, Var_desc_Oggetto,
                        InfoOggetto.Codice.Trim(), InfoOggetto.Descrizione.Trim(), Cha_Esito, delegato, codWorkingApplication, checkNotify, id_trasm, dataAzione);

                if (!result)
                    logger.Error(String.Format("L'inserimento del log ha dato esito negativo: {0} {1} {2}",
                                                    InfoOggetto.Codice, InfoOggetto.Descrizione, Var_desc_Oggetto));

            }
            else
            {
                string codice = string.Empty;
                string desc = string.Empty;
                if (InfoOggetto.Codice == null)
                    codice = "Var_codice_log null";
                else codice = InfoOggetto.Codice;

                // logger.Error(String.Format("Il log non risulta attivo: {0}", codice.Trim()));

            }

            //Inserisco nella tabella eventi per la creazione/modifica metadati xml (normativa AGID 2021)
            if (!string.IsNullOrEmpty(ID_Oggetto) && Convert.ToInt32(ID_Oggetto) != 0 && !string.IsNullOrEmpty(InfoOggetto.Codice))
            {
                logger.Debug("INIZIO InsertEventDocument");
                string idDelegato = (delegato != null && !string.IsNullOrEmpty(delegato.idPeople)) ? delegato.idPeople : string.Empty;
                DocsPaDB.Query_DocsPAWS.Log db = new DocsPaDB.Query_DocsPAWS.Log();
                db.InsertEventDocument(ID_People_Operatore, ID_Gruppo_Operatore, ID_Amministrazione, InfoOggetto.Oggetto.Trim(), ID_Oggetto,
                    Var_desc_Oggetto, InfoOggetto.Codice.Trim(), InfoOggetto.Descrizione.Trim(), id_trasm, idDelegato);
                logger.Debug("FINE InsertEventDocument");
            }

        }
        catch (Exception ex)
        {
            result = false;
            logger.Error(ex.Message.ToString() + " " + ex.StackTrace.ToString(), ex);

        }

        return result;
    }

    public static bool WriteLog(DocsPaVO.utente.InfoUtente infoUtente, string WebMethodName, string ID_Oggetto, string Var_desc_Oggetto, DocsPaVO.Logger.CodAzione.Esito Cha_Esito)
    {
        if (WebMethodName == "EDITINGACL" || WebMethodName == "EDITINGFASCACL")
            return WriteUserLOG_ACL(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, WebMethodName, ID_Oggetto, Var_desc_Oggetto, Cha_Esito, infoUtente.delegato);
        else
            return WriteUserLOG(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, WebMethodName, ID_Oggetto, Var_desc_Oggetto, Cha_Esito, infoUtente.delegato, infoUtente.codWorkingApplication);
    }

    private static bool WriteUserLOG_ACL(string UserID_Operatore, string ID_People_Operatore, string ID_Gruppo_Operatore, string ID_Amministrazione, string WebMethodName, string ID_Oggetto, string Var_desc_Oggetto, DocsPaVO.Logger.CodAzione.Esito Cha_Esito, DocsPaVO.utente.InfoUtente delegato)
    {

        bool result = true;
        DocsPaVO.Logger.CodAzione.infoOggetto InfoOggetto;

        try
        {

            //logger.Debug("WriteLog -UserLog-");
            //Reperisco le informazioni relative al WebMethod.
            InfoOggetto = DocsPaDB.Query_DocsPAWS.Log.getInfoOggetto(WebMethodName, ID_Amministrazione);
            //Insert sulla DPA_LOG.

            result = DocsPaDB.Query_DocsPAWS.Log.InsertLog(UserID_Operatore, ID_People_Operatore, ID_Gruppo_Operatore, ID_Amministrazione,
                  InfoOggetto.Oggetto.Trim(), ID_Oggetto, Var_desc_Oggetto,
                  InfoOggetto.Codice.Trim(), InfoOggetto.Descrizione.Trim(), Cha_Esito, delegato, string.Empty);

        }
        catch (Exception ex)
        {
            result = false;
            logger.Error(ex.Message.ToString(), ex);
        }

        return result;
    }

    public static bool VerificaLogAggiungiDocumento(string intervallo)
    {
        bool result = false;
        try
        {
            DocsPaDB.Query_DocsPAWS.Log log = new DocsPaDB.Query_DocsPAWS.Log();
            result = log.VerficiaTemporaleLogAggiungiDocumento(intervallo);
        }
        catch (Exception ex)
        {
            logger.Debug(ex.Message.ToString(), ex);
            return false;
        }

        return result;
    }

    public string ReadWSLog(string codAmm)
    {
        string result = null;

        try
        {
            // prende la system_id dell'amm.ne
            DocsPaDB.Query_DocsPAWS.AmministrazioneXml amm = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
            string idAmm = amm.GetAdminByName(codAmm);

            #region gestione con file xml (DISATTIVATO)
            //				XmlDocument xmlDoc = new XmlDocument();
            //				string xmldocpath = AppDomain.CurrentDomain.BaseDirectory + @"xml\WSLog.xml";
            //				xmlDoc.Load(xmldocpath);
            //
            //				string path = ".//AMMINISTRAZIONE[@ID='" + idAmm + "']";
            //				XmlNode nodoAmm  = xmlDoc.SelectSingleNode(path);
            //
            //				result = nodoAmm.OuterXml;
            #endregion

            #region gestione con lettura sul db

            System.Data.DataSet ds;
            System.Data.DataSet ds2;
            DataRow row;

            DocsPaDB.Query_DocsPAWS.Log log = new DocsPaDB.Query_DocsPAWS.Log();
            ds = log.GetLogAttivi(idAmm);

            if (ds != null && ds.Tables["AZIONI"].Rows.Count > 0)
            {
                // ha trovato record di log attivi
                ds2 = log.GetLogDisattivi(idAmm); // prende quelli disattivi
                if (ds2 != null && ds2.Tables["AZIONI_DISATT"].Rows.Count > 0)
                {
                    // unisce i due dataset
                    foreach (DataRow rowDis in ds2.Tables["AZIONI_DISATT"].Rows)
                    {
                        row = ds.Tables["AZIONI"].NewRow();
                        row["codice"] = rowDis["codice"];
                        row["descrizione"] = rowDis["descrizione"];
                        row["oggetto"] = rowDis["oggetto"];
                        row["metodo"] = rowDis["metodo"];
                        row["attivo"] = "0";
                        row["notify"] = "NN";
                        row["notification"] = rowDis["notification"];
                        row["configurable"] = rowDis["configurable"];

                        ds.Tables["AZIONI"].Rows.Add(row);
                    }
                }
                result = ds.GetXml();
            }
            else
            {
                // non ha trovato record di log attivi, prende quindi tutti quelli dell'anagrafica
                ds = log.LogIsNotActive(idAmm);
                if (ds != null && ds.Tables["AZIONI"].Rows.Count > 0)
                {
                    result = ds.GetXml();
                }
            }

            #endregion
        }
        catch
        {
            result = null;
        }
        return result;
    }

    public string GetAdminByName(string codAmm)
    {
        try
        {
            // prende la system_id dell'amm.ne
            DocsPaDB.Query_DocsPAWS.AmministrazioneXml amm = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
            return amm.GetAdminByName(codAmm);
        }
        catch (Exception ex)
        {
            logger.Debug(ex.Message.ToString(), ex);
            return string.Empty;
        }
    }

    public bool WriteWSLog(string streamXml, string codAmm)
    {
        bool result = true;

        try
        {
            // prende la system_id dell'amm.ne
            DocsPaDB.Query_DocsPAWS.AmministrazioneXml amm = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
            string idAmm = amm.GetAdminByName(codAmm);

            #region gestione con file xml (DISATTIVATO)

            //				string codice;
            //				string attivo;
            //				bool passato = false;

            //				// xml già presente sul WS
            //				XmlDocument xmlDoc = new XmlDocument();
            //				string xmldocpath = AppDomain.CurrentDomain.BaseDirectory + @"xml\WSLog.xml";
            //				xmlDoc.Load(xmldocpath);
            //
            //				string path = ".//AMMINISTRAZIONE[@ID='" + idAmm + "']";
            //				XmlNode nodoAmm  = xmlDoc.SelectSingleNode(path);
            //
            //				// xml passato in stream
            //				XmlDocument xmlDocStream = new XmlDocument();
            //				xmlDocStream.LoadXml(streamXml);
            //
            //				if(nodoAmm != null && nodoAmm.ChildNodes.Count > 0)
            //				{
            //					// ChildNodes dell'xml presente sul WS
            //					foreach (XmlNode azione in nodoAmm.ChildNodes)
            //					{
            //						codice = azione.ChildNodes[0].InnerText;
            //						attivo = azione.ChildNodes[4].InnerText;
            //
            //						// prende il nodo ATTIVO dello stream passato
            //						XmlNode azioneStream = xmlDocStream.SelectSingleNode(".//AZIONI/CODICE[text()='"+ codice +"']");
            //						if(azioneStream != null)
            //						{
            //							XmlNode NodeAttivoStream = azioneStream.ParentNode.SelectSingleNode("ATTIVO");
            //
            //							// modifica il nodo ATTIVO dell'xml presente sul WS con quello passato in stream
            //							if(attivo != NodeAttivoStream.InnerText)
            //							{
            //								azione.ChildNodes[4].InnerText = NodeAttivoStream.InnerText;
            //								passato = true;
            //							}
            //						}
            //						else
            //						{
            //							return false;
            //						}
            //					}
            //
            //					if(passato)
            //					{
            //						xmlDoc.Save(xmldocpath);
            //					}
            //
            //				}
            //				else
            //				{
            //					return false;
            //				}
            #endregion

            #region gestione con lettura sul db

            string codice;
            string notify;
            // Contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                // elimina tutti i record di questa amm.ne
                DocsPaDB.Query_DocsPAWS.Log log = new DocsPaDB.Query_DocsPAWS.Log();
                if (log.DelLogAttivi(idAmm))
                {
                    // xml passato in stream
                    XmlDocument xmlDocStream = new XmlDocument();
                    xmlDocStream.LoadXml(streamXml);

                    XmlNode listaAzioni = xmlDocStream.SelectSingleNode("NEWDATASET");
                    if (listaAzioni.ChildNodes.Count > 0)
                    {
                        XmlNodeList listaAttivi = listaAzioni.SelectNodes(".//ATTIVO[text()='1']");
                        if (listaAttivi.Count > 0)
                        {
                            foreach (XmlNode nodoAttivo in listaAttivi)
                            {
                                // prende il codice
                                codice = nodoAttivo.ParentNode.SelectSingleNode("CODICE").InnerText;

                                //prende il tipo notifica
                                notify = nodoAttivo.ParentNode.SelectSingleNode("NOTIFY").InnerText;

                                // esegue la insert su db
                                log.AddLogAttivi(codice, idAmm, notify);
                            }
                        }
                    }
                }
                else
                {
                    return false;
                }

                transactionContext.Complete();
            }

            #endregion
        }
        catch (Exception ex)
        {
            logger.Debug(ex.Message.ToString(), ex);
            return false;
        }

        return result;
    }

    public string VerificaArchivioLogPath()
    {
        string result = DocsPaVO.Settings.AppSettings.Instance.ARCHIVIO_LOG_PATH;

        return result;
    }

    public void ContaArchivioLog(out string result, string codAmm, string type)
    {
        result = "";

        try
        {
            // prende la system_id dell'amm.ne
            DocsPaDB.Query_DocsPAWS.AmministrazioneXml amm = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
            string idAmm = amm.GetAdminByName(codAmm);

            DocsPaDB.Query_DocsPAWS.AmministrazioneXml log = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
            result = log.ContaArchivioLog(idAmm, type);
        }
        catch (Exception exception)
        {
            logger.Debug("Errore nel metodo ContaArchivioLog: ", exception);
        }
    }

    public string[] ListaFilesLog(string codAmm)
    {
        string[] files = null;

        try
        {
            string pathArchivio = DocsPaUtils.Functions.Functions.GetArchivioLogPath();
            string searchPattern = codAmm + "*.pdf";

            files = Directory.GetFiles(pathArchivio, searchPattern);
        }
        catch (Exception exception)
        {
            logger.Debug("Errore nel metodo ListaFilesLog: ", exception);
            files = null;
        }
        return files;
    }

    public string GetXmlLogFiltrato(string dataDa, string dataA, string user, string oggetto, string azione, string codAmm, string esito, string type, int table)
    {
        return this.GetXmlLogFiltrato(dataDa, dataA, user, string.Empty, oggetto, azione, codAmm, esito, type, table);
    }

    public string GetXmlLogFiltrato(string dataDa, string dataA, string user, string idOggetto, string oggetto, string azione, string codAmm, string esito, string type, int table)
    {
        System.Data.DataSet ds;

        string result;

        try
        {
            DocsPaDB.Query_DocsPAWS.Log log = new DocsPaDB.Query_DocsPAWS.Log();
            ds = log.GetXmlLogFiltrato(dataDa, dataA, user, idOggetto, oggetto, azione, codAmm, esito, type, table);
            if (ds != null)
            {
                result = ds.GetXml();
            }
            else
            {
                result = null;
            }
        }
        catch
        {
            result = null;
        }
        return result;
    }

    public string ReadWSLogAmm(string codAmm)
    {
        string result = null;

        try
        {
            // prende la system_id dell'amm.ne
            DocsPaDB.Query_DocsPAWS.AmministrazioneXml amm = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
            string idAmm = amm.GetAdminByName(codAmm);

            #region gestione con file xml (DISATTIVATO)
            //				XmlDocument xmlDoc = new XmlDocument();
            //				string xmldocpath = AppDomain.CurrentDomain.BaseDirectory + @"xml\WSLog.xml";
            //				xmlDoc.Load(xmldocpath);
            //
            //				string path = ".//AMMINISTRAZIONE[@ID='" + idAmm + "']";
            //				XmlNode nodoAmm  = xmlDoc.SelectSingleNode(path);
            //
            //				result = nodoAmm.OuterXml;
            #endregion

            #region gestione con lettura sul db

            System.Data.DataSet ds;
            System.Data.DataSet ds2;
            DataRow row;

            DocsPaDB.Query_DocsPAWS.Log log = new DocsPaDB.Query_DocsPAWS.Log();
            ds = log.GetLogAttiviAmm(idAmm);

            if (ds != null && ds.Tables["AZIONIAMM"].Rows.Count > 0)
            {
                // ha trovato record di log attivi
                ds2 = log.GetLogDisattiviAmm(idAmm); // prende quelli disattivi
                if (ds2 != null && ds2.Tables["AZIONI_DISATTIVE"].Rows.Count > 0)
                {
                    // unisce i due dataset
                    foreach (DataRow rowDis in ds2.Tables["AZIONI_DISATTIVE"].Rows)
                    {
                        row = ds.Tables["AZIONIAMM"].NewRow();
                        row["codice"] = rowDis["codice"];
                        row["descrizione"] = rowDis["descrizione"];
                        row["oggetto"] = rowDis["oggetto"];
                        row["metodo"] = rowDis["metodo"];
                        row["attivo"] = "0";
                        ds.Tables["AZIONIAMM"].Rows.Add(row);
                    }
                }
                result = ds.GetXml();
            }
            else
            {
                // non ha trovato record di log attivi, prende quindi tutti quelli dell'anagrafica
                ds = log.GetLogDisattiviAmm();
                if (ds != null && ds.Tables["AZIONIAMM"].Rows.Count > 0)
                {
                    result = ds.GetXml();
                }
            }

            #endregion
        }
        catch
        {
            result = null;
        }
        return result;
    }

    public static bool WriteLog(string UserID, string ID_People, string ID_Gruppo, string ID_Amministrazione, string WebMethodName, string ID_Oggetto, string Var_desc_Oggetto, DocsPaVO.Logger.CodAzione.Esito Cha_Esito, DocsPaVO.utente.InfoUtente delegato, string checkNotify = "", string id_trasm = "", string dataAzione = "")
    {
        return WriteUserLOG(UserID, ID_People, ID_Gruppo, ID_Amministrazione, WebMethodName, ID_Oggetto, Var_desc_Oggetto, Cha_Esito, delegato, string.Empty, checkNotify, id_trasm, dataAzione);
    }


}
