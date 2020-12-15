using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;
using System.Data;

namespace NttDataWA.UIManager
{
    public class DiagrammiManager
    {       
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        public static bool isUltimaDaAccettare(string idTrasmissione, Page page)
        {
            try
            {
                return docsPaWS.isUltimaDaAccettare(idTrasmissione);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static Stato getStatoSuccessivoAutomatico(string docNumber)
        {
            try
            {
                return docsPaWS.getStatoSuccessivoAutomatico(docNumber);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Stato getStatoSuccessivoAutomaticoFasc(string idProject)
        {
            try
            {
                return docsPaWS.getStatoSuccessivoAutomaticoFasc(idProject);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
        /// <summary>
        /// Restituisce lo stato corrente del documento nel caso di documento tipizzato con diagramma di stato associato
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static DocsPaWR.Stato GetStateDocument(string idDoc = "")
        {
            try
            {
                if (string.IsNullOrEmpty(idDoc))
                {
                    if (DocumentManager.getSelectedRecord() == null)
                        return null;
                    else
                    {
                        return docsPaWS.getStatoDoc(DocumentManager.getSelectedRecord().docNumber);
                    }
                }
                return docsPaWS.getStatoDoc(idDoc);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Restituisce un booleano che indica per i documenti  tipizzati con diagramma di stato associato, se lo stato è finale o meno
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static bool IsDocumentInFinalState(string idDoc="")
        {
            try
            {
                DocsPaWR.Stato state = null;
                if (string.IsNullOrEmpty(idDoc))
                {
                    if (DocumentManager.getSelectedRecord() == null)
                        return false;
                    else
                    {
                        state = docsPaWS.getStatoDoc(DocumentManager.getSelectedRecord().docNumber);
                        if (state != null && state.STATO_FINALE)
                            return true;
                        else return false;
                    }
                }
                state = docsPaWS.getStatoDoc(idDoc);
                if (state != null && state.STATO_FINALE)
                    return true;
                else return false;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }   

        public static DiagrammaStato getDiagrammaById(string idDiagramma)
        {
            try
            {
                return docsPaWS.getDiagrammaById(idDiagramma);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void salvaModificaStato(string docNumber, string idStato, DiagrammaStato diagramma, string idUtente, InfoUtente user, string dataScadenza, Page page)
        {
            try
            {
                docsPaWS.salvaModificaStato(docNumber, idStato, diagramma, idUtente, user, dataScadenza);

                SchedaDocumento docSel = DocumentManager.getSelectedRecord();

                //Controllo che lo stato sia uno stato di conversione pdf lato server
                //In caso affermativo faccio partire la conversione
                if (utils.isEnableConversionePdfLatoServer() == "true" &&
                    docSel != null && docSel.documenti != null && !String.IsNullOrEmpty(docSel.documenti[0].fileName))
                {
                    DocsPaWR.Stato statoAttuale = DiagrammiManager.GetStateDocument(docNumber);
                    if (statoAttuale != null && statoAttuale.CONVERSIONE_PDF)
                    {
                        FileManager fileManager = new FileManager();
                        DocsPaWR.FileDocumento fileDocumento = fileManager.getFile(page);
                        if (fileDocumento != null && fileDocumento.content != null && fileDocumento.name != null && fileDocumento.name != "")
                        {
                            FileManager.EnqueueServerPdfConversion(UserManager.GetInfoUser(), fileDocumento.content, fileDocumento.name, DocumentManager.getSelectedRecord());
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static DocumentConsolidationStateInfo GetDocumentConsolidationState(string idDocument)
        {
            DocumentConsolidationStateInfo info = null;
            try
            {
                info = docsPaWS.GetDocumentConsolidationState(UserManager.GetInfoUser(), idDocument);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return info;
        }

        public static void deleteStoricoTrasmDiagrammi(string docNumber, string idStato)
        {
            try
            {
                docsPaWS.deleteStoricoTrasmDiagrammi(docNumber, idStato);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static ArrayList isStatoTrasmAuto(string idAmm, string idStato, string idTemplate)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.isStatoTrasmAuto(idAmm, idStato, idTemplate));
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void salvaStoricoTrasmDiagrammiFasc(string idTrasm, string idProject, string idStato)
        {
            try
            {
                docsPaWS.salvaStoricoTrasmDiagrammiFasc(idTrasm, idProject, idStato);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static DiagrammaStato getDgByIdTipoDoc(string systemIdTipoDoc, string idAmm)
        {
            try
            {
                DiagrammaStato dg = docsPaWS.getDgByIdTipoDoc(systemIdTipoDoc, idAmm);
                //verifico che il ruolo abbia visibilità sul diagramma
                if (dg != null && (!string.IsNullOrEmpty(dg.SYSTEM_ID.ToString())) &&
                    (!DiagrammiManager.IsAssociatoRuoloDiagramma(dg.SYSTEM_ID.ToString(),RoleManager.GetRoleInSession().idGruppo)))
                    dg = null;
                return dg;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DiagrammaStato getDgByIdTipoFasc(string systemIdTipoFasc, string idAmm)
        {
            try
            {
                return docsPaWS.getDgByIdTipoFasc(systemIdTipoFasc, idAmm);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool IsAssociatoRuoloDiagramma(string idDiagramma, string idRuolo)
        {
            try
            {
                return docsPaWS.IsAssociatoRuoloDiagramma(idDiagramma, idRuolo);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static bool IsRuoloAssociatoStatoDia(string idDiagramma, string idRuolo, string idStato)
        {
            try
            {
                return docsPaWS.IsRuoloAssociatoStatoDia(idDiagramma, idRuolo, idStato);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string getStatoDocStorico(string docNumber)
        {
            try
            {
                return docsPaWS.getStatoDocStorico(docNumber);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool isStatoAuto(string idStato, string idDiagramma)
        {
            try
            {
                return docsPaWS.isStatoAuto(idStato, idDiagramma);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static void salvaDataScadenzaDoc(string docNumber, string dataScadenza, string idTipoAtto)
        {
            try
            {
                docsPaWS.salvaDataScadenzaDoc(docNumber, dataScadenza, idTipoAtto);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void salvaStoricoTrasmDiagrammi(string idTrasm, string docNumber, string idStato)
        {
            try
            {
                docsPaWS.salvaStoricoTrasmDiagrammi(idTrasm, docNumber, idStato);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static DataSet getDiagrammaStoricoDoc(string docNumber)
        {
            try
            {
                return docsPaWS.getDiagrammaStoricoDoc(docNumber);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DataSet getDiagrammaStoricoFasc(string idProject)
        {
            try
            {
                return docsPaWS.getDiagrammaStoricoFasc(idProject);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }


        public static Stato getStatoFasc(string idProject)
        {
            try
            {
                return docsPaWS.getStatoFasc(idProject);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }


        public static int getDiagrammaAssociato(string idTipoDoc)
        {
            try
            {
                return docsPaWS.getDiagrammaAssociato(idTipoDoc);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return 0;
            }
        }

        //Laura 9 Aprile
        public static int getDiagrammaAssociatoFasc(string idTipoDoc)
        {
            try
            {
                return docsPaWS.getDiagrammaAssociatoFasc(idTipoDoc);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return 0;
            }
        }

        public static Stato[] getStatiPerRicerca(string idDiagramma, string docOrFasc)
        {
            try
            {
                return docsPaWS.getStatiPerRicerca(idDiagramma, docOrFasc);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void salvaModificaStatoFasc(string idProject, string idStato, DiagrammaStato diagramma, string idUtente, InfoUtente user, string dataScadenza)
        {
            try
            {
                docsPaWS.salvaModificaStatoFasc(idProject, idStato, diagramma, idUtente, user, dataScadenza);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void salvaDataScadenzaFasc(string idProject, string dataScadenza, string idTipoFasc)
        {
            try
            {
                docsPaWS.salvaDataScadenzaFasc(idProject, dataScadenza, idTipoFasc);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static ArrayList isStatoTrasmAutoFasc(string idAmm, string idStato, string idTipoFasc)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.isStatoTrasmAutoFasc(idAmm, idStato, idTipoFasc));
                return result;
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static List<AssPhaseStatoDiagramma> GetFaseDiagrammaByIdFase(string idDiagramma, string idFase)
        {
            try
            {
                return docsPaWS.GetFaseDiagrammaByIdFase(idDiagramma, idFase, UIManager.UserManager.GetInfoUser()).ToList();
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static List<AssPhaseStatoDiagramma> GetFasiStatiDiagramma(string idDiagramma)
        {
            try
            {
                return docsPaWS.GetFasiStatiDiagramma(idDiagramma, UIManager.UserManager.GetInfoUser()).ToList();
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Stato GetStatoById(string idStato)
        {
            try
            {
                return docsPaWS.GetStatoById(idStato, UIManager.UserManager.GetInfoUser());
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ArrayList ChangeStateGetMissingRoles(string idProject, string idStato)
        {
            try
            {
                return new ArrayList(docsPaWS.ChangeStateGetMissingRoles(idProject, idStato, UserManager.GetInfoUser(), ProjectManager.getInfoFascicoloDaFascicolo(UIManager.ProjectManager.getProjectInSession())));   
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void CreateTransmissionsMissingRoles(string[] roleReasonsList, string idStato)
        {
            try
            {
                docsPaWS.ChangeStateTransmissionsMissingRoles(roleReasonsList, UserManager.GetInfoUser(), ProjectManager.getInfoFascicoloDaFascicolo(UIManager.ProjectManager.getProjectInSession()), idStato);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void ChangeStateSendTransmissions(string idStato)
        {
            try
            {
                docsPaWS.ChangeStateSendTransmissions(idStato, ProjectManager.getInfoFascicoloDaFascicolo(UIManager.ProjectManager.getProjectInSession()), UserManager.GetInfoUser());
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static string[] Albo_GetFileDaPubblicare(string idDocPrincipale, string option)
        {
            try
            {
                return docsPaWS.Albo_GetFileDaPubblicare(idDocPrincipale, option);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool Albo_InsFileDaPubblicare(string idDocPrincipale, string  docNumber, string daPubb)
        {
            try
            {
                return docsPaWS.Albo_InsFileDaPubblicare(idDocPrincipale, docNumber, daPubb);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static string[] Albo_GetStatiSelezioneFiles(string idDiagramma)
        {
            try
            {
                return docsPaWS.Albo_GetStatiSelezioneFiles(idDiagramma);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

    }
}