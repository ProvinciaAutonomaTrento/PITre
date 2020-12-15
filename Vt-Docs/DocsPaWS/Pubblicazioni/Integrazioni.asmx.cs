using DocsPaVO.utente;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml.Serialization;
using log4net;

namespace DocsPaWS.Pubblicazioni
{
    /// <summary>
    /// Summary description for Integrazioni
    /// </summary>
    [WebService(Namespace = "http://localhost")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    //[WebService(Namespace = "http://tempuri.org/")]
    //[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    //[System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Integrazioni : System.Web.Services.WebService
    {
        private ILog logger = LogManager.GetLogger(typeof(Integrazioni));

        #region Bacheca
        [WebMethod]
        [XmlInclude(typeof(DocsPaVO.ExternalServices.MIBACT_Bacheca_info))]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.ExternalServices.MIBACT_Bacheca_info))]
        public virtual ArrayList BACHECA_getDocsDaNotificare(string statoInvia, string statoAggiorna, string campoNCirc)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.MIBACT_BACHECA_getDocsDaNotificare(statoInvia, statoAggiorna, campoNCirc);
        }

        [WebMethod]
        public virtual bool BACHECA_cambiaStatoDoc(string idDoc, string statoDest, string idAmm, string idPeople, string idGruppo)
        {
            bool retVal = false;
            try
            {
                Utente utente = BusinessLogic.Utenti.UserManager.getUtenteById(idPeople);
                Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(idGruppo);
                InfoUtente infoUtente = new InfoUtente(utente, ruolo);

                DocsPaVO.documento.SchedaDocumento documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, idDoc, idDoc);
                if (documento != null)
                {
                    if (documento.template != null)
                    {
                        int idDiagramma = 0;
                        DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;
                        DocsPaVO.DiagrammaStato.Stato statoAttuale = null;
                        idDiagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaAssociato(documento.template.SYSTEM_ID.ToString());
                        if (idDiagramma != 0)
                        {
                            diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(idDiagramma.ToString());
                            if (diagramma != null)
                            {
                                if (diagramma.STATI != null && diagramma.STATI.Count > 0)
                                {
                                    statoAttuale = BusinessLogic.DiagrammiStato.DiagrammiStato.getStatoDoc(documento.docNumber);

                                    if (statoAttuale == null)
                                    {
                                        foreach (DocsPaVO.DiagrammaStato.Stato stato in diagramma.STATI)
                                        {
                                            if (stato.DESCRIZIONE.ToUpper().Equals(statoDest.ToUpper()))
                                            {
                                                BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStato(documento.docNumber, stato.SYSTEM_ID.ToString(), diagramma, infoUtente.idPeople, infoUtente, string.Empty);
                                                retVal = true;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 0; i < diagramma.PASSI.Count; i++)
                                        {
                                            DocsPaVO.DiagrammaStato.Passo step = (DocsPaVO.DiagrammaStato.Passo)diagramma.PASSI[i];
                                            if (step.STATO_PADRE.SYSTEM_ID == statoAttuale.SYSTEM_ID)
                                            {
                                                for (int j = 0; j < step.SUCCESSIVI.Count; j++)
                                                {
                                                    if (((DocsPaVO.DiagrammaStato.Stato)step.SUCCESSIVI[j]).DESCRIZIONE.ToUpper().Equals(statoDest.ToUpper()))
                                                    {
                                                        BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStato(documento.docNumber, ((DocsPaVO.DiagrammaStato.Stato)step.SUCCESSIVI[j]).SYSTEM_ID.ToString(), diagramma, infoUtente.idPeople, infoUtente, string.Empty);
                                                        retVal = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (!retVal)
                                    {
                                        throw new Exception("Stato del diagramma non trovato");
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Diagramma di stato non trovato");
                        }
                    }
                    else
                    {
                        throw new Exception("Tipologia non trovata");
                    }
                }
                else
                {
                    throw new Exception("Documento non trovato");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return retVal;

        }

        [WebMethod]
        [XmlInclude(typeof(DocsPaVO.ExternalServices.MIGR_File_Info))]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.ExternalServices.MIGR_File_Info))]
        public virtual ArrayList BACHECA_GetFileInfoDoc(string idDoc)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.MIBACT_BACHECA_GetFileInfoDoc(idDoc);
        }

        [WebMethod]
        public virtual byte[] BACHECA_GetFile(string docnumber, string versionId, string version, string versionLabel, string password, string idUtente, string idRuolo)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.MIGR_FS_GetFile(docnumber, versionId, version, versionLabel, password, idUtente, idRuolo);
        }
   

        #endregion

        #region BigFiles FTP
        [WebMethod]
        [XmlInclude(typeof(DocsPaVO.ExternalServices.FileFtpUpInfo))]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.ExternalServices.FileFtpUpInfo))]
        public virtual ArrayList BigFilesFTP_getFiles()
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.BigFilesFTP_GetFilesToTransfer();
        }

        [WebMethod]
        public virtual bool BigFilesFtp_updateTable(DocsPaVO.ExternalServices.FileFtpUpInfo infoFile)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.BigFilesFTP_updateTable(infoFile);
        }
        #endregion

        #region Fattura milano
        [WebMethod]
        public string GetIdAutFatturaPassiva(string numAutorizzazione, string idCampo, string idTemplate)
        {
            string retval = null;
            ArrayList items = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getIdDocByAssTemplates(idCampo, numAutorizzazione, "DESC", idTemplate);
            if (items != null && items.Count > 0)
            {
                retval = items[0].ToString();
            }
            return retval;
        }

        [WebMethod]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(string))]
        public ArrayList GetIdDocsByDiagramStatus(string descStato, string descDiagramma, string codAmm)
        {
            return BusinessLogic.DiagrammiStato.DiagrammiStato.GetIdDocsByDiagramStatus(descStato, descDiagramma, codAmm);
        }
        #endregion
    }
}
