using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using SAAdminTool.DocsPaWR;
using System.Data;
using System.Collections.Generic;

namespace SAAdminTool
{
    public class DiagrammiManager
    {
        private static DocsPaWebService docsPaWS = ProxyManager.getWS();

        public static void salvaDiagramma(DiagrammaStato dg, string idAmm, Page page)
        {
            try
            {
                docsPaWS.salvaDiagramma(dg, idAmm);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static DiagrammaStato getDiagrammaById(string idDiagramma, Page page)
        {
            try
            {
                return docsPaWS.getDiagrammaById(idDiagramma);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }        
        }

        public static ArrayList getDiagrammi(string idAmm, Page page)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getDiagrammi(idAmm));
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }        
        }

        public static bool isUniqueNameDiagramma(string nomeDiagramma, Page page)
        {
            try
            {
                return docsPaWS.isUniqueNameDiagramma(nomeDiagramma);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return false;
            }
        }

        public static void delDiagramma(DiagrammaStato dg, Page page)
        {
            try
            {
                docsPaWS.delDiagramma(dg);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static void updateDiagramma(DiagrammaStato dg, Page page)
        {
            try
            {
                docsPaWS.updateDiagramma(dg);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static bool associaTipoDocDiagramma(string idTipoDoc, string idDiagramma, Page page)
        {
            try
            {
                return docsPaWS.associaTipoDocDiagramma(idTipoDoc, idDiagramma);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return false;
            }
        }

        public static bool getDocOrFascInStato(string idStato, Page page)
        {
            try
            {
                return docsPaWS.getDocOrFascInStato(idStato);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return false;
            }
        }

        public static void disassociaTipoDocDiagramma(string idTipoDoc, Page page)
        {
            try
            {
                docsPaWS.disassociaTipoDocDiagramma(idTipoDoc);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static int getDiagrammaAssociato(string idTipoDoc, Page page)
        {
            try
            {
                return docsPaWS.getDiagrammaAssociato(idTipoDoc);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return 0;
            }
        }

        public static bool isModificabile(int systemIdDiagramma, Page page)
        {
            try
            {
                return docsPaWS.isModificabile(systemIdDiagramma);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return false;
            }
        }

        public static DiagrammaStato getDgByIdTipoDoc(string systemIdTipoDoc, string idAmm, Page page)
        {
            try
            {
                DiagrammaStato dg = docsPaWS.getDgByIdTipoDoc(systemIdTipoDoc, idAmm);
                //verifico che il ruolo abbia visibilità sul diagramma
                if (dg != null && (!string.IsNullOrEmpty(dg.SYSTEM_ID.ToString())) &&
                    (!DiagrammiManager.IsAssociatoRuoloDiagramma(dg.SYSTEM_ID.ToString(), UserManager.getRuolo().idGruppo)))
                    dg = null;
                return dg;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static void salvaModificaStato(string docNumber, string idStato, DiagrammaStato diagramma, string idUtente, InfoUtente user, string dataScadenza, Page page)
        {
            try
            {
                docsPaWS.salvaModificaStato(docNumber, idStato, diagramma, idUtente, user, dataScadenza);

                SchedaDocumento docSel = DocumentManager.getDocumentoSelezionato();
                if (docSel == null)
                    docSel = DocumentManager.getDocumentoInLavorazione();

                //Controllo che lo stato sia uno stato di conversione pdf lato server
                //In caso affermativo faccio partire la conversione
                if (SAAdminTool.Utils.isEnableConversionePdfLatoServer() == "true" &&
                    docSel != null &&  docSel.documenti != null && !String.IsNullOrEmpty(docSel.documenti[0].fileName))
                {
                    SAAdminTool.DocsPaWR.Stato statoAttuale = DiagrammiManager.getStatoDoc(docNumber, page);
                    if (statoAttuale.CONVERSIONE_PDF)
                    {
                        //if (schedaDocumento != null)
                        //{
                        FileManager fileManager = new FileManager();
                        DocsPaWR.FileDocumento fileDocumento = fileManager.getFile(page);
                        if (fileDocumento != null && fileDocumento.content != null && fileDocumento.name != null && fileDocumento.name != "")
                        {
                            FileManager.EnqueueServerPdfConversion(page, UserManager.getInfoUtente(page), fileDocumento.content, fileDocumento.name, DocumentManager.getDocumentoInLavorazione(page));
                        }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static ArrayList isStatoTrasmAuto(string idAmm, string idStato, string idTemplate, Page page)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.isStatoTrasmAuto(idAmm, idStato, idTemplate));
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static bool isStatoAuto(string idStato, string idDiagramma, Page page)
        {
            try
            {
                return docsPaWS.isStatoAuto(idStato, idDiagramma);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return false;
            }
        }

        public static Stato getStatoDoc(string docNumber, Page page)
        {
            try
            {
                return docsPaWS.getStatoDoc(docNumber);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static string getStatoDocStorico(string docNumber, Page page)
        {
            try
            {
                return docsPaWS.getStatoDocStorico(docNumber);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static DataSet getDiagrammaStoricoDoc(string docNumber, Page page)
        {
            try
            {
                return docsPaWS.getDiagrammaStoricoDoc(docNumber);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static void salvaStoricoTrasmDiagrammi(string idTrasm, string docNumber, string idStato, Page page)
        {
            try
            {
                docsPaWS.salvaStoricoTrasmDiagrammi(idTrasm, docNumber, idStato);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static void deleteStoricoTrasmDiagrammi(string docNumber, string idStato, Page page)
        {
            try
            {
                docsPaWS.deleteStoricoTrasmDiagrammi(docNumber, idStato);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static Stato getStatoSuccessivoAutomatico(string docNumber, Page page)
        {
            try
            {
                return docsPaWS.getStatoSuccessivoAutomatico(docNumber);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static bool isUltimaDaAccettare(string idTrasmissione, Page page)
        {
            try
            {
                return docsPaWS.isUltimaDaAccettare(idTrasmissione);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return false;
            }
        }

        public static bool isDocumentiInStatoFinale(string idDiagramma, string idTemplate, Page page)
        {
            try
            {
                return docsPaWS.isDocumentiInStatoFinale(idDiagramma, idTemplate);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return false;
            }
        }

        public static int getDiagrammaAssociatoFasc(string idTipoFasc, Page page)
        {
            try
            {
                return docsPaWS.getDiagrammaAssociatoFasc(idTipoFasc);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return 0;
            }
        }

        public static bool associaTipoFascDiagramma(string idTipoFasc, string idDiagramma, Page page)
        {
            try
            {
                return docsPaWS.associaTipoFascDiagramma(idTipoFasc, idDiagramma);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return false;
            }
        }

        public static bool isFascicoliInStatoFinale(string idDiagramma, string idTemplate, Page page)
        {
            try
            {
                return docsPaWS.isFascicoliInStatoFinale(idDiagramma, idTemplate);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return false;
            }
        }

        public static void disassociaTipoFascDiagramma(string idTipoFasc, Page page)
        {
            try
            {
                docsPaWS.disassociaTipoFascDiagramma(idTipoFasc);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);                
            }
        }

        public static DiagrammaStato getDgByIdTipoFasc(string systemIdTipoFasc, string idAmm, Page page)
        {
            try
            {
                return docsPaWS.getDgByIdTipoFasc(systemIdTipoFasc, idAmm);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static void salvaModificaStatoFasc(string idProject, string idStato, DiagrammaStato diagramma, string idUtente, InfoUtente user, string dataScadenza, Page page)
        {
            try
            {
                docsPaWS.salvaModificaStatoFasc(idProject, idStato, diagramma, idUtente, user, dataScadenza);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static ArrayList isStatoTrasmAutoFasc(string idAmm, string idStato, string idTipoFasc, Page page)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.isStatoTrasmAutoFasc(idAmm, idStato, idTipoFasc));
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static void salvaStoricoTrasmDiagrammiFasc(string idTrasm, string idProject, string idStato, Page page)
        {
            try
            {
                docsPaWS.salvaStoricoTrasmDiagrammiFasc(idTrasm, idProject, idStato);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static Stato getStatoFasc(string idProject, Page page)
        {
            try
            {
                return docsPaWS.getStatoFasc(idProject);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static void salvaDataScadenzaFasc(string idProject, string dataScadenza, string idTipoFasc, Page page)
        {
            try
            {
                docsPaWS.salvaDataScadenzaFasc(idProject, dataScadenza, idTipoFasc);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static void salvaDataScadenzaDoc(string docNumber, string dataScadenza, string idTipoAtto, Page page)
        {
            try
            {
                docsPaWS.salvaDataScadenzaDoc(docNumber, dataScadenza, idTipoAtto);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static DataSet getDiagrammaStoricoFasc(string idProject, Page page)
        {
            try
            {
                return docsPaWS.getDiagrammaStoricoFasc(idProject);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static void deleteStoricoTrasmDiagrammiFasc(string idProject, string idStato, Page page)
        {
            try
            {
                docsPaWS.deleteStoricoTrasmDiagrammiFasc(idProject, idStato);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static Stato getStatoSuccessivoAutomaticoFasc(string idProject, Page page)
        {
            try
            {
                return docsPaWS.getStatoSuccessivoAutomaticoFasc(idProject);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static Stato[] getStatiPerRicerca(string idDiagramma, string docOrFasc, Page page)
        {
            try
            {
                return docsPaWS.getStatiPerRicerca(idDiagramma, docOrFasc);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }
                            
        #region Visibilità ruolo /stati diagramma

        public static List<DocsPaWR.AssRuoloStatiDiagramma> GetRuoliStatiDiagramma(int idDiagramma)
        {
            try
            {
                return new List<AssRuoloStatiDiagramma>(docsPaWS.getRuoliStatiDiagramma(idDiagramma));
            }
            catch (Exception ex)
            {
                return new List<AssRuoloStatiDiagramma>();
            }
        }

        public static bool ModifyRuoloStatiDiagramma(List<DocsPaWR.AssRuoloStatiDiagramma> listAssRuoloStatiDia)
        {
            try
            {
                return docsPaWS.ModifyRuoloStatiDiagramma(listAssRuoloStatiDia.ToArray());
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool IsAssociatoRuoloDiagramma(string idDiagramma, string idRuolo)
        {
            try
            {
                return docsPaWS.IsAssociatoRuoloDiagramma(idDiagramma, idRuolo);
            }
            catch (Exception ex)
            {
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
        #endregion
    }
}