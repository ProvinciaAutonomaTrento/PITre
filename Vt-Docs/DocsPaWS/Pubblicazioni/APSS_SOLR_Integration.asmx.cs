using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using log4net;
using BusinessLogic.Amministrazione;
using System.Xml.Serialization;
using System.Collections;
using VtDocsWS.Domain;
using VtDocsWS.WebServices;

namespace DocsPaWS.Pubblicazioni
{
    /// <summary>
    /// Summary description for APSS_SOLR_Integration
    /// </summary>
    [WebService(Namespace = "http://localhost")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    //[System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class APSS_SOLR_Integration : System.Web.Services.WebService
    {
        private ILog logger = LogManager.GetLogger(typeof(APSS_SOLR_Integration));


        #region APSS - Delibere e determine - SOLR
        [WebMethod]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.ExternalServices.PubblicazioneAPSS))]
        public virtual ArrayList APSSgetDelDetDaPubbl(string ogg_custom, string statiDiagramma, string templates, string dataUltimaEsecuzione)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.APSSgetDelDetDaPubbl(ogg_custom, statiDiagramma, templates, dataUltimaEsecuzione);
        }

        [WebMethod]
        public virtual string APSSgetLastExecutionDate(string tipologie)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.APSSgetLastExecutionDate(tipologie);
        }

        [WebMethod]
        public virtual bool APSSInsertInPubTable(DocsPaVO.ExternalServices.PubblicazioneAPSS pubb)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.APSSInsertInPubTable(pubb);
        }

        [WebMethod]
        public virtual bool APSSUpdateResultPubbInTable(DocsPaVO.ExternalServices.PubblicazioneAPSS pubb)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.APSSUpdateResultPubbInTable(pubb);
        }


        [WebMethod]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.ExternalServices.PubblicazioneAPSS))]
        public virtual ArrayList APSSgetDelDetDaRiPubbl(string ogg_custom, string statiDiagramma, string templates, string dataUltimaEsecuzione, string tipoevento)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.APSSgetDelDetDaRiPubbl(ogg_custom, statiDiagramma, templates, dataUltimaEsecuzione, tipoevento);
        }

        [WebMethod]
        public virtual bool APSSCtrlAttachExt(string iddoc)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.APSSCtrlAttachExt(iddoc);
        }
        #endregion

        #region Integrazione CDS

        [WebMethod]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.ExternalServices.EventoCDS))]
        public virtual ArrayList CDS_getLogEvents(string lastLog, string idTipoCDS, string idOggAppliant, string idOggLocat)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.CDS_getLogEvents(lastLog, idTipoCDS, idOggAppliant, idOggLocat);
        }

        [WebMethod]
        public virtual string CDS_getLastLogId()
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.CDS_getLastLogID();
        }

        [WebMethod]
        public virtual bool CDS_InsertEventInTable(DocsPaVO.ExternalServices.EventoCDS evento)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.CDS_InsertEventInTable(evento);
        }
        #endregion

        #region StampaRepertorio manuale
        [WebMethod]
        public virtual void StampaRep_GeneratePrintByYear(string counterId, string year)
        {
            BusinessLogic.utenti.RegistriRepertorioPrintManager.GeneratePrintByYear(counterId, year);
        }

        [WebMethod]
        public virtual string StampaRep_GenPrintByDocumentId(string idDoc)
        {
            return BusinessLogic.utenti.RegistriRepertorioPrintManager.GeneratePrintByDocumentId(idDoc);
        }

        [WebMethod]
        public virtual string StampaRep_GenPrintByRanges(string anno, string counterId, string numRepStart, string numRepEnd, string idRegistro, string dataStampa, bool ultimastampa)
        {
            return BusinessLogic.utenti.RegistriRepertorioPrintManager.GeneratePrintByRanges(anno, counterId, numRepStart, numRepEnd, idRegistro, dataStampa, ultimastampa);
        }

        #endregion

        #region StampaRegistri recupero
        [WebMethod]
        public virtual string Recupero_SReg(string username, string codeReg, string protoDa, string protoA, string anno, string dataStampa, string idOldDoc, string idAmm)
        {
            return BusinessLogic.Report.RegistriStampa.StampaRegExperimental(username, codeReg, protoDa, protoA, anno, dataStampa, idOldDoc, idAmm);
        }

        [WebMethod]
        public virtual string Recupero_SReg_Lite(string username, string idDoc)
        {
            return BusinessLogic.Report.RegistriStampa.StampaRegExp2(idDoc, username);
        }


        #endregion

        #region Albo Telematico
        [WebMethod]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.ExternalServices.PubblicazioneAlbo))]

        public virtual ArrayList Albo_getDocsDaNotificare(string idDocMinimo)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.Albo_getDocsDaNotificare(idDocMinimo);
        }

        [WebMethod]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.ExternalServices.PubblicazioneAlbo))]

        public virtual ArrayList Albo_UNITN_getDocsDaNotificare(string idDocMinimo)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.Albo_UNITN_getDocsDaNotificare(idDocMinimo);
        }

        #endregion

        #region Controllo Stampe repertorio
        [WebMethod]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.ExternalServices.Ctrl_Stmp_Rep))]
        public virtual ArrayList ControlloStampeRepertorio_Errori()
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.Ctrl_stmp_rep_errori();
        }

        [WebMethod]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.ExternalServices.Ctrl_Stmp_Rep))]
        public virtual ArrayList ControlloStampeRepertorio_File()
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.Ctrl_stmp_rep_file();
        }

        #endregion

        #region MIGR_FS Migrazione File System
        [WebMethod]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.ExternalServices.MIGR_File_Info))]
        public virtual ArrayList MIGR_FS_GetListMIGRFileInfo(string minVersionId, string maxVersionId)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.MIGR_FS_GetListMIGRFileInfo(minVersionId, maxVersionId);
        }

        [WebMethod]
        public virtual bool MIGR_FS_insertDataInLogTable(DocsPaVO.ExternalServices.MIGR_File_Info migr_file_info)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.MIGR_FS_insertDataInLogTable(migr_file_info);
        }

        [WebMethod]
        public virtual byte[] MIGR_FS_GetFile(string docnumber, string versionId, string version, string versionLabel, string password, string idUtente, string idRuolo)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.MIGR_FS_GetFile(docnumber, versionId, version, versionLabel, password, idUtente, idRuolo);
        }


        #endregion

        #region Cerca.Tre
        [WebMethod]
        [XmlInclude(typeof(C3Document))]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(C3Document))]
        public virtual ArrayList C3GetDocs(string data, string mod)
        {
            int numAllegati = 0;
            string fromTime = "", toTime = "", optionTime = "";
            bool modificati = false;
            //if (!string.IsNullOrEmpty(request.DateLimitsOptions))
            //    optionTime = request.DateLimitsOptions;
            //else
            //{
            //    if (!string.IsNullOrEmpty(request.FromDateTime))
            //    {
            //        fromTime = request.FromDateTime;
            //        if (!string.IsNullOrEmpty(request.ToDateTime))
            //            toTime = request.ToDateTime;
            //    }
            //    else
            //    {
            //        optionTime = "6";
            //    }
            //}

            DateTime dataX1 = DateTime.ParseExact(data, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            fromTime = dataX1.AddDays(-1).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 00:00:00";
            toTime = dataX1.AddDays(-1).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 23:59:59";


            if (!string.IsNullOrEmpty(mod) && mod.ToLower() == "true")
                modificati = true;
            System.Data.DataTable fromDB = null;
            if (!modificati)
                fromDB = BusinessLogic.Amministrazione.SistemiEsterni.C3GetDocs(fromTime, toTime, optionTime);
            else
                fromDB = BusinessLogic.Amministrazione.SistemiEsterni.C3GetDocsMod(fromTime, toTime, optionTime);
            C3Document docX = null;
            ArrayList listaDocs = new ArrayList();
            Correspondent corrTemp = null, corrMitt = null;
            DocsPaVO.ProfilazioneDinamica.Templates templateDocX;
            ArrayList mitts = new ArrayList(), dest = new ArrayList(), destCC = new ArrayList();
            if (fromDB != null && fromDB.Rows != null && fromDB.Rows.Count > 0)
            {
                foreach (System.Data.DataRow r in fromDB.Rows)
                {
                    templateDocX = null;
                    docX = new C3Document();
                    docX.Id = r["SYSTEM_ID"].ToString();
                    docX.DocumentType = r["CHA_TIPO_PROTO"].ToString();
                    docX.Object = r["OGGETTO"].ToString();
                    docX.CreationDate = r["DATACREAZIONE"].ToString();
                    if (docX.DocumentType != "G")
                    {
                        docX.Signature = r["DOCNAME"].ToString();
                        docX.ProtocolNumber = r["NUM_PROTO"].ToString();
                        docX.ProtocolYear = r["NUM_ANNO_PROTO"].ToString();
                        docX.ProtocolDate = r["DATAPROTO"].ToString();
                        if (r["REGISTRO"] != null && !string.IsNullOrEmpty(r["REGISTRO"].ToString()))
                        {
                            docX.Register = new Register();
                            docX.Register.Id = r["REGISTRO"].ToString().Split('§')[0];
                            docX.Register.Code = r["REGISTRO"].ToString().Split('§')[1];
                            docX.Register.Description = r["REGISTRO"].ToString().Split('§')[2];
                            docX.Register.IsRF = r["REGISTRO"].ToString().Split('§')[3] == "1" ? true : false;
                        }
                        if (r["MITT_DEST"] != null && !string.IsNullOrEmpty(r["MITT_DEST"].ToString()))
                        {
                            string[] arrCorr = r["MITT_DEST"].ToString().Split(';');
                            foreach (string sX in arrCorr)
                            {
                                corrTemp = new Correspondent();
                                if (sX.Contains("(D)"))
                                {
                                    corrTemp.Description = sX.Replace("(D)", "");
                                    dest.Add(corrTemp);
                                }
                                else if (sX.Contains("(CC)"))
                                {
                                    corrTemp.Description = sX.Replace("(CC)", "");
                                    destCC.Add(corrTemp);
                                }
                                else
                                {
                                    corrTemp.Description = sX;
                                    mitts.Add(corrTemp);
                                }
                            }

                            if (mitts.Count < 2 && mitts.Count > 0) corrMitt = (Correspondent)mitts[0];

                            if (corrMitt != null) docX.Sender = corrMitt;
                            if (corrMitt == null && mitts.Count > 0) docX.MultipleSenders = (Correspondent[])mitts.ToArray(typeof(Correspondent));
                            if (dest.Count > 0) docX.Recipients = (Correspondent[])dest.ToArray(typeof(Correspondent));
                            if (destCC.Count > 0) docX.RecipientsCC = (Correspondent[])destCC.ToArray(typeof(Correspondent));
                        }
                    }
                    docX.Author = r["AUTORE"].ToString();
                    docX.AuthorId = r["IDAUTORE"].ToString();
                    docX.AuthorRole = r["RUOLOCREATORE"].ToString();
                    docX.AuthorRoleId = r["IDRUOLOAUTORE"].ToString();
                    docX.AuthorUO = r["UOCREATRICE"].ToString();
                    if (r["TIPOLOGIA"] != null && !string.IsNullOrEmpty(r["TIPOLOGIA"].ToString()))
                    {
                        docX.Template = new Template();
                        docX.Template.Id = r["TIPOLOGIA"].ToString().Split('§')[0];
                        docX.Template.Name = r["TIPOLOGIA"].ToString().Split('§')[1];
                        templateDocX = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateDettagli(docX.Id);
                        docX.Template = Utils.GetDetailsTemplateDoc(templateDocX, docX.Id);
                    }

                    if (r["DOC_PRINCIPALE"] != null && !string.IsNullOrEmpty(r["DOC_PRINCIPALE"].ToString()))
                    {
                        docX.MainDocument = new C3File();
                        docX.MainDocument.Name = r["DOC_PRINCIPALE"].ToString().Split('§')[0];
                        docX.MainDocument.VersionId = r["DOC_PRINCIPALE"].ToString().Split('§')[1];
                        docX.MainDocument.MimeType = r["DOC_PRINCIPALE"].ToString().Split('§')[2];
                        docX.MainDocument.VersionIdinDB = r["DOC_PRINCIPALE"].ToString().Split('§')[3];
                        docX.MainDocument.PathName = r["DOC_PRINCIPALE"].ToString().Split('§')[4];
                        docX.MainDocument.FileSize = r["DOC_PRINCIPALE"].ToString().Split('§')[5];
                    }

                    if (r["NUM_ALLEGATI"] != null && !string.IsNullOrEmpty(r["NUM_ALLEGATI"].ToString()) && Int32.Parse(r["NUM_ALLEGATI"].ToString()) > 0)
                    {
                        C3File allX = null;
                        ArrayList allXs = new ArrayList();
                        System.Data.DataTable fromDBAll = BusinessLogic.Amministrazione.SistemiEsterni.C3GetAllByIdDoc(docX.Id);
                        foreach (System.Data.DataRow rAll in fromDBAll.Rows)
                        {
                            allX = new C3File();
                            allX.Description = rAll["OGGETTO"].ToString();
                            allX.Id = rAll["SYSTEM_ID"].ToString();
                            if (rAll["FILE_ALLEGATO"] != null && !string.IsNullOrEmpty(rAll["FILE_ALLEGATO"].ToString()))
                            {
                                allX.Name = rAll["FILE_ALLEGATO"].ToString().Split('§')[0];
                                allX.VersionId = rAll["FILE_ALLEGATO"].ToString().Split('§')[1];
                                allX.MimeType = rAll["FILE_ALLEGATO"].ToString().Split('§')[2];
                                allX.VersionIdinDB = rAll["FILE_ALLEGATO"].ToString().Split('§')[3];
                                allX.PathName = rAll["FILE_ALLEGATO"].ToString().Split('§')[4];
                                allX.FileSize = rAll["FILE_ALLEGATO"].ToString().Split('§')[5];
                            }

                            allXs.Add(allX);
                        }
                        numAllegati += fromDBAll.Rows.Count;
                        if (allXs != null && allXs.Count > 0)
                            docX.Attachments = (C3File[])allXs.ToArray(typeof(C3File));
                    }

                    listaDocs.Add(docX);
                }
            }

            return listaDocs;
        }
        #endregion
        // #region metodi temporanei
        // [WebMethod]
        // public virtual P3SBCLib.Contracts.ElencoPratiche P3SBCByPass(string anno)
        // {
        //     //string retval = "";
        //     P3SBCLib.P3SBCServices sbcsvc = new P3SBCLib.P3SBCServices();
        //     P3SBCLib.Contracts.FiltriRicercaPratiche filtriPra = new P3SBCLib.Contracts.FiltriRicercaPratiche();
        //     filtriPra.CercaInClassificazioniFiglie = true;
        //     filtriPra.ClassificazionePratica = "25";
        //     filtriPra.SoloPraticheNonAssegnate = false;
        //     if (!string.IsNullOrEmpty(anno))
        //     {
        //         filtriPra.DataAperturaPratica = DateTime.ParseExact("01/01/" + anno, "d", System.Globalization.CultureInfo.InvariantCulture);
        //         filtriPra.DataAperturaPraticaFinale = DateTime.ParseExact("12/31/" + anno, "d", System.Globalization.CultureInfo.InvariantCulture);
        //     }

        //     P3SBCLib.Contracts.CriteriPaginazione pagPra = new P3SBCLib.Contracts.CriteriPaginazione();
        //     pagPra.OggettiPerPagina = 20;
        //     pagPra.Pagina = 1;

        //     return sbcsvc.GetPratiche("IMPORT_ELENCO_BENI", filtriPra, pagPra);

        //     //return retval;
        // }

        //[WebMethod]
        // public string controlloMarche(string idAmm, string userId, string idruolo, string iddoc, string version)
        // {
        //     string retval = "";
        //     try
        //     {
        //         retval += string.Format("IdAmm: {0} - user: {1} - idDoc {2} - vers {3} - idruolo {4} - ", idAmm, userId, iddoc, version, idruolo);
        //         DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(userId, idAmm);
        //         //DocsPaVO.utente.Ruolo ruolo = ((DocsPaVO.utente.Ruolo[])BusinessLogic.Utenti.UserManager.getRuoliUtente(utente.idPeople).ToArray(typeof(DocsPaVO.utente.Ruolo)))[0];
        //         DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(idruolo);
        //         DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

        //         //DocsPaVO.documento.SchedaDocumento documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, iddoc, iddoc);
        //         DocsPaVO.documento.SchedaDocumento documento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, iddoc);
        //         int numVersione = 0;
        //         bool result = Int32.TryParse(version, out numVersione);
        //         if (result)
        //         {
        //             if (documento.documenti.Count < numVersione || numVersione <= 0)
        //             {
        //                 throw new Exception("FILE_VERSION_NOT_FOUND");
        //             }
        //             else
        //             {
        //                 numVersione = documento.documenti.Count - numVersione;
        //             }
        //         }
        //         DocsPaVO.documento.FileDocumento fileDocumento = BusinessLogic.Documenti.FileManager.getFileFirmato((DocsPaVO.documento.FileRequest)documento.documenti[numVersione], infoUtente, false);

        //         ArrayList timestamps = BusinessLogic.Documenti.TimestampManager.getTimestampsDoc(infoUtente, (DocsPaVO.documento.FileRequest)documento.documenti[numVersione]);
        //         DocsPaVO.areaConservazione.OutputResponseMarca outmarcax = null;
        //         foreach (DocsPaVO.documento.TimestampDoc tsx in timestamps)
        //         {
        //             outmarcax = BusinessLogic.Documenti.TimestampManager.VerificaMarca(fileDocumento.content, Convert.FromBase64String(tsx.TSR_FILE));
        //             retval += string.Format("TSDoc: {0} - Esito: {1} - Desc_errore: {2}",outmarcax.timestampedDoc, outmarcax.esito, outmarcax.descrizioneErrore);
        //         }

        //     }
        //     catch (Exception ex2)
        //     {
        //         logger.Error(ex2);
        //         retval+= "Eccezione: "+ex2;
        //     }
        //     return retval;
        // }
        // #endregion
    }
}
