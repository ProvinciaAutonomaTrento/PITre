using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;

namespace NttDataWA.UIManager
{
    public class GridItemAddDocInProject
    {
        public string IdDocumento { get; set; }
        public string Data { get; set; }
        public string Registro { get; set; }
        public string TipoDocumento { get; set; }
        public string Oggetto { get; set; }
        public bool Fascicola { get; set; }
        public string idRegistro { get; set; }
        public string idProfile { get; set; }
        public string Personale { get; set; }
        public string Privato { get; set; }
    }

    public class AddDocInProjectManager
    {
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        private static FullTextSearchContext GetFullTextSearchContextinSession
        {
            get
            {
                return HttpContext.Current.Session["FULL_TEXT_CONTEXT"] as FullTextSearchContext;
            }

            set
            {
                HttpContext.Current.Session["FULL_TEXT_CONTEXT"] = value;
            }
        }

        private static void RemoveFullTextSearchContext()
        {
            try
            {
                HttpContext.Current.Session.Remove("FULL_TEXT_CONTEXT");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static DocsPaWR.FiltroRicerca[][] RicercaDocDaFascicolare(string tipoSelezione, string idRegistro,
            string numeroDa, string numeroA, string dataDa, string dataA, string anno, string oggetto)
        {
            DocsPaWR.FiltroRicerca[][] qV = new DocsPaWR.FiltroRicerca[1][];
            qV[0] = new DocsPaWR.FiltroRicerca[1];
            DocsPaWR.FiltroRicerca[] fVList = new DocsPaWR.FiltroRicerca[0];
            try
            {
                //array contenitore degli array filtro di ricerca

                switch (tipoSelezione)
                {
                    case "P":
                        {
                            #region FILTRI PER RICERCA DOCUMENTI PROTOCOLLATI
                            if (!string.IsNullOrEmpty(idRegistro) && !isFiltroAooEnabled())
                            {
                                fVList = AddFiltroRegistro(idRegistro, fVList);
                            }
                            fVList = AddFiltroNumeroProtocollo(numeroDa, numeroA, fVList);

                            fVList = AddFiltroAnno(anno, fVList);

                            fVList = AddFiltroDataProtocollo(dataDa, dataA, fVList);

                            fVList = AddFiltroDaProtocollare(fVList);

                            fVList = AddFiltroDaProtocolloInArrivo(fVList);

                            fVList = AddFiltroDaProtocolloInPartenza(fVList);

                            fVList = AddFiltroProtocolloInterno(fVList);

                            #endregion

                            break;
                        }
                    case "G":
                        {
                            #region FILTRI PER RICERCA DOCUMENTI GRIGI

                            fVList = AddFiltroDataCreazioneDocumento(dataDa, dataA, fVList);

                            fVList = AddFiltroNumeroDocumento(numeroDa, numeroA, fVList);

                            fVList = AddFiltroTipo(fVList);

                            fVList = AddFiltroGrigio(fVList);

                            fVList = AddFiltroAnno(anno, fVList);

                            #endregion
                            break;
                        }

                    case "PRED":
                        {
                            #region FILTRI PER RICERCA PREDISPOSTI
                            if (!string.IsNullOrEmpty(idRegistro) && !isFiltroAooEnabled())
                            {
                                fVList = AddFiltroRegistro(idRegistro, fVList);
                            }
                            fVList = AddFiltroDataCreazioneDocumento(dataDa, dataA, fVList);

                            fVList = AddFiltroNumeroDocumento(numeroDa, numeroA, fVList);

                            fVList = AddFiltroTipo(fVList);

                            fVList = AddFiltroPredisposto(fVList);

                            fVList = AddFiltroAnno(anno, fVList);
                            #endregion

                            break;
                        }
                    case "ADL":
                        {
                            #region FILTRI PER RICERCA ADL

                            fVList = AddFiltroDocumentiInADL(fVList, UserManager.GetUserInSession(), UIManager.RoleManager.GetRoleInSession());

                            fVList = AddFiltroTipo(fVList);

                            fVList = AddFiltroDaProtocolloInArrivo(fVList);

                            fVList = AddFiltroDaProtocolloInPartenza(fVList);

                            fVList = AddFiltroProtocolloInterno(fVList);

                            fVList = AddFiltroGrigio(fVList);

                            fVList = AddFiltroAllegati(fVList);

                            fVList = AddFiltroPredisposto(fVList);


                            if (!isFiltroAooEnabled())
                            {
                                fVList = AddFiltroRegistro(idRegistro, fVList);
                            }

                            fVList = AddFiltroDaProtocollare(fVList);

                            #endregion
                            break;
                        }
                    case "ADL_ROLE":
                        #region FILTRI PER RICERCA ADL
                        fVList = AddFiltroDocumentiInADLRole(fVList, UserManager.GetUserInSession(), UIManager.RoleManager.GetRoleInSession());

                        fVList = AddFiltroTipo(fVList);

                        fVList = AddFiltroDaProtocolloInArrivo(fVList);

                        fVList = AddFiltroDaProtocolloInPartenza(fVList);

                        fVList = AddFiltroProtocolloInterno(fVList);

                        fVList = AddFiltroGrigio(fVList);

                        fVList = AddFiltroAllegati(fVList);

                        fVList = AddFiltroPredisposto(fVList);


                        if (!isFiltroAooEnabled())
                        {
                            fVList = AddFiltroRegistro(idRegistro, fVList);
                        }

                        fVList = AddFiltroDaProtocollare(fVList);
                        #endregion
                        break;
                    case "STAMPE":
                        #region Filtri per Ricerca Stampe Registro
                        if (!string.IsNullOrEmpty(idRegistro) && !isFiltroAooEnabled())
                            {
                                fVList = AddFiltroRegistro(idRegistro, fVList);
                            }
                            //fVList = AddFiltroDataCreazioneDocumento(dataDa, dataA, fVList);

                            fVList = AddFiltroNumeroDocumento(numeroDa, numeroA, fVList);

                            fVList = AddFiltroTipo(fVList);

                            fVList = AddFiltroStampeRegistro(dataDa, dataA, anno, fVList);

                            //fVList = AddFiltroAnno(anno, fVList);
                        #endregion
                        break;
                }


                fVList = AddFiltroNoAnnullato(fVList);

                if (!string.IsNullOrEmpty(oggetto))
                {
                    fVList = AddFiltroOggetto(oggetto, fVList);
                }

                qV[0] = fVList;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return qV;
        }

        /// <summary>
        /// aggiunge ai filtri i documenti in ADL
        /// </summary>
        /// <param name="fVList"></param>
        /// <param name="utente"></param>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        private static FiltroRicerca[] AddFiltroDocumentiInADLRole(FiltroRicerca[] fVList, Utente utente, Ruolo ruolo)
        {
            try
            {
                DocsPaWR.FiltroRicerca fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DOC_IN_ADL.ToString();
                fV1.valore = "0@" + RoleManager.GetRoleInSession().systemId.ToString();
                fVList = addToArrayFiltroRicerca(fVList, fV1);
                fV1 = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fVList;
        }

        /// <summary>
        /// verifica se il filtro Aoo è attivo
        /// </summary>
        /// <returns></returns>
        public static bool isFiltroAooEnabled()
        {
            bool result = false;
            try
            {
                result = docsPaWS.isFiltroAooEnabled();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return result;

        }

        /// <summary>
        /// aggiunge un filtro ricerca 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="nuovoElemento"></param>
        /// <returns></returns>
        public static FiltroRicerca[] addToArrayFiltroRicerca(FiltroRicerca[] array, FiltroRicerca nuovoElemento)
        {
            try
            {
                FiltroRicerca[] nuovaLista;
                if (array != null)
                {
                    int len = array.Length;
                    nuovaLista = new FiltroRicerca[len + 1];
                    array.CopyTo(nuovaLista, 0);
                    nuovaLista[len] = nuovoElemento;
                }
                else
                {
                    nuovaLista = new FiltroRicerca[1];
                    nuovaLista[0] = nuovoElemento;
                }
                return nuovaLista;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// aggiunge il filtro relativo alla data
        /// </summary>
        /// <param name="dataDa"></param>
        /// <param name="dataA"></param>
        /// <param name="fVList"></param>
        /// <returns></returns>
        private static FiltroRicerca[] AddFiltroDataProtocollo(string dataDa, string dataA, FiltroRicerca[] fVList)
        {
            try
            {
                DocsPaWR.FiltroRicerca fV1 = new DocsPaWR.FiltroRicerca();
                if (!string.IsNullOrEmpty(dataDa) && string.IsNullOrEmpty(dataA))
                {
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString();
                    fV1.valore = dataDa;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                else
                {//valore singolo carico DATA_PROTOCOLLO_DAL - DATA_PROTOCOLLO_AL
                    if (!string.IsNullOrEmpty(dataDa) && !string.IsNullOrEmpty(dataA))
                    {
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString();
                        fV1.valore = dataDa;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);

                        fV1 = new FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString();
                        fV1.valore = dataA;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fVList;
        }

        /// <summary>
        /// aggiunge il filtro relativo alla data
        /// </summary>
        /// <param name="numeroDa"></param>
        /// <param name="numeroA"></param>
        /// <param name="fVList"></param>
        /// <returns></returns>
        private static FiltroRicerca[] AddFiltroNumeroProtocollo(string numeroDa, string numeroA, FiltroRicerca[] fVList)
        {
            try
            {
                DocsPaWR.FiltroRicerca fV1 = new DocsPaWR.FiltroRicerca();
                DocsPaWR.FiltroRicerca fV2 = new DocsPaWR.FiltroRicerca();

                if (!string.IsNullOrEmpty(numeroDa) && string.IsNullOrEmpty(numeroA))
                {
                    fV1 = new FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString();
                    fV1.valore = numeroDa;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                else
                {//valore singolo carico NUM_PROTOCOLLO_DAL - NUM_PROTOCOLLO_AL
                    if (!string.IsNullOrEmpty(numeroDa) && !string.IsNullOrEmpty(numeroA))
                    {
                        fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString();
                        fV1.valore = numeroDa;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);

                        fV2.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString();
                        fV2.valore = numeroA;
                        fVList = addToArrayFiltroRicerca(fVList, fV2);
                    }

                }
                fV1 = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fVList;
        }

        /// <summary>
        /// Aggiunge il numero realtivo al filtro del numero di protocollo
        /// </summary>
        /// <param name="numeroDa"></param>
        /// <param name="numeroA"></param>
        /// <param name="fVList"></param>
        /// <returns></returns>
        private static FiltroRicerca[] AddFiltroNumeroDocumento(string numeroDa, string numeroA, FiltroRicerca[] fVList)
        {
            try
            {
                DocsPaWR.FiltroRicerca fV1 = new DocsPaWR.FiltroRicerca();

                if (!string.IsNullOrEmpty(numeroDa) && string.IsNullOrEmpty(numeroA))
                {

                    fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER.ToString();
                    fV1.valore = numeroDa;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
                else
                {
                    if (!string.IsNullOrEmpty(numeroDa) && string.IsNullOrEmpty(numeroA))
                    {

                        fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString();
                        fV1.valore = numeroDa;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);

                        fV1 = new FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString();
                        fV1.valore = numeroA;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
            }

            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fVList;
        }

        /// <summary>
        /// aggiunge l'anno al filtro della ricerca
        /// </summary>
        /// <param name="anno"></param>
        /// <param name="fVList"></param>
        /// <returns></returns>
        private static FiltroRicerca[] AddFiltroAnno(string anno, FiltroRicerca[] fVList)
        {
            try
            {
                DocsPaWR.FiltroRicerca fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.ANNO_PROTOCOLLO.ToString();
                fV1.valore = anno;
                fVList = addToArrayFiltroRicerca(fVList, fV1);
                fV1 = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fVList;
        }

        /// <summary>
        /// aggiunge il filtro Tipologia
        /// </summary>
        /// <param name="anno"></param>
        /// <param name="fVList"></param>
        /// <returns></returns>
        private static FiltroRicerca[] AddFiltroTipo(FiltroRicerca[] fVList)
        {
            try
            {
                DocsPaWR.FiltroRicerca fV1 = new DocsPaWR.FiltroRicerca();

                fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                fV1.valore = "TIPO";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
                fV1 = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fVList;
        }

        /// <summary>
        /// aggiunge il filtro relativo al registro
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <param name="fVList"></param>
        /// <returns></returns>
        private static FiltroRicerca[] AddFiltroRegistro(string idRegistro, FiltroRicerca[] fVList)
        {
            try
            {
                DocsPaWR.FiltroRicerca fV1 = new DocsPaWR.FiltroRicerca();

                // se è "" allora il registro associato nodo di Titolario nel quale si 
                //classifica è NULL, cioè visibile a tutti i registri

                fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRO.ToString();
                fV1.valore = idRegistro;
                fVList = addToArrayFiltroRicerca(fVList, fV1);
                fV1 = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fVList;
        }

        /// <summary>
        /// aggiunger il filtro da protocollare
        /// </summary>
        /// <param name="fVList"></param>
        /// <returns></returns>
        private static FiltroRicerca[] AddFiltroDaProtocollare(FiltroRicerca[] fVList)
        {
            try
            {
                DocsPaWR.FiltroRicerca fV1 = new DocsPaWR.FiltroRicerca();

                fV1.argomento = DocsPaWR.FiltriDocumento.DA_PROTOCOLLARE.ToString();
                fV1.valore = "0";  //corrisponde a 'false'
                fVList = addToArrayFiltroRicerca(fVList, fV1);
                fVList = AddFiltroTipo(fVList);
                fV1 = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fVList;
        }

        /// <summary>
        /// Aggiunge il filtro ai protocolli in Arrivo
        /// </summary>
        /// <param name="fVList"></param>
        /// <returns></returns>
        private static FiltroRicerca[] AddFiltroDaProtocolloInArrivo(FiltroRicerca[] fVList)
        {
            try
            {
                DocsPaWR.FiltroRicerca fV1 = new DocsPaWR.FiltroRicerca();

                fV1.argomento = DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString();
                fV1.valore = "true";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
                fV1 = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fVList;
        }

        /// <summary>
        /// aggiunge il filtro ai protocolli in Partenza
        /// </summary>
        /// <param name="fVList"></param>
        /// <returns></returns>
        private static FiltroRicerca[] AddFiltroDaProtocolloInPartenza(FiltroRicerca[] fVList)
        {
            try
            {
                DocsPaWR.FiltroRicerca fV1 = new DocsPaWR.FiltroRicerca();

                fV1.argomento = DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString();
                fV1.valore = "true";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
                fV1 = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fVList;
        }

        /// <summary>
        /// aggiunge il filtro ai protocolli interni
        /// </summary>
        /// <param name="fVList"></param>
        /// <returns></returns>
        private static FiltroRicerca[] AddFiltroProtocolloInterno(FiltroRicerca[] fVList)
        {
            try
            {
                DocsPaWR.FiltroRicerca fV1 = new DocsPaWR.FiltroRicerca();

                fV1.argomento = DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString();
                fV1.valore = "true";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
                fV1 = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fVList;
        }

        /// <summary>
        /// Aggiunge il filtro realtivo ai documenti grigi
        /// </summary>
        /// <param name="fVList"></param>
        /// <returns></returns>
        private static FiltroRicerca[] AddFiltroGrigio(FiltroRicerca[] fVList)
        {
            try
            {
                DocsPaWR.FiltroRicerca fV1 = new DocsPaWR.FiltroRicerca();

                fV1.argomento = DocsPaWR.FiltriDocumento.GRIGIO.ToString();
                fV1.valore = "true";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
                fV1 = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fVList;
        }

        /// <summary>
        /// aggiunge ai filtri la data di creazione del documento
        /// </summary>
        /// <param name="dataDa"></param>
        /// <param name="dataA"></param>
        /// <param name="fVList"></param>
        /// <returns></returns>
        private static FiltroRicerca[] AddFiltroDataCreazioneDocumento(string dataDa, string dataA, FiltroRicerca[] fVList)
        {
            try
            {
                DocsPaWR.FiltroRicerca fV1 = new DocsPaWR.FiltroRicerca();
                if (!string.IsNullOrEmpty(dataDa) && string.IsNullOrEmpty(dataA))
                {
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString();
                    fV1.valore = dataDa;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                    fV1 = null;
                }
                else
                {
                    if (!string.IsNullOrEmpty(dataDa) && !string.IsNullOrEmpty(dataA))
                    {
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                        fV1.valore = dataDa;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);

                        fV1 = new FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                        fV1.valore = dataA;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fVList;
        }

        /// <summary>
        /// aggiunge ai filtri i documenti predisposti
        /// </summary>
        /// <param name="fVList"></param>
        /// <returns></returns>
        private static FiltroRicerca[] AddFiltroPredisposto(FiltroRicerca[] fVList)
        {
            try
            {
                DocsPaWR.FiltroRicerca fV1 = new DocsPaWR.FiltroRicerca();

                fV1.argomento = DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString();
                fV1.valore = "true";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
                fV1 = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fVList;
        }

        private static FiltroRicerca[] AddFiltroStampeRegistro(string anno, string dataDa, string dataA, FiltroRicerca[] fVList)
        {
            try
            {
                DocsPaWR.FiltroRicerca fV1 = new DocsPaWR.FiltroRicerca();

                fV1.argomento = DocsPaWR.FiltriDocumento.STAMPA_REG.ToString();
                fV1.valore = "true";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
                fV1 = null;

                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriStampaRegistro.ANNO_PROTOCOLLO_STAMPA.ToString();
                fV1.valore = anno;
                fVList = addToArrayFiltroRicerca(fVList, fV1);
                fV1 = null;

                fV1 = new DocsPaWR.FiltroRicerca();
                if (!string.IsNullOrEmpty(dataDa) && string.IsNullOrEmpty(dataA))
                {
                    fV1.argomento = DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO.ToString();
                    fV1.valore = dataDa;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                else
                {//valore singolo carico DATA_PROTOCOLLO_DAL - DATA_PROTOCOLLO_AL
                    if (!string.IsNullOrEmpty(dataDa) && !string.IsNullOrEmpty(dataA))
                    {
                        fV1.argomento = DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO_DAL.ToString();
                        fV1.valore = dataDa;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);

                        fV1 = new FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO_AL.ToString();
                        fV1.valore = dataA;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);
                    }
                }

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fVList;
        }

        /// <summary>
        /// Aggiunge ai filtri gli allegati
        /// </summary>
        /// <param name="fVList"></param>
        /// <returns></returns>
        private static FiltroRicerca[] AddFiltroAllegati(FiltroRicerca[] fVList)
        {
            try
            {
                DocsPaWR.FiltroRicerca fV1 = new DocsPaWR.FiltroRicerca();

                fV1.argomento = DocsPaWR.FiltriDocumento.ALLEGATO.ToString();
                fV1.valore = "false";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
                fV1 = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fVList;
        }

        /// <summary>
        /// aggiunge ai filtri i documenti in ADL
        /// </summary>
        /// <param name="fVList"></param>
        /// <param name="utente"></param>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        private static FiltroRicerca[] AddFiltroDocumentiInADL(FiltroRicerca[] fVList, Utente utente, Ruolo ruolo)
        {
            try
            {
                DocsPaWR.FiltroRicerca fV1 = new DocsPaWR.FiltroRicerca();

                fV1.argomento = DocsPaWR.FiltriDocumento.DOC_IN_ADL.ToString();
                fV1.valore = utente.idPeople.ToString() + "@" + ruolo.systemId.ToString();
                fVList = addToArrayFiltroRicerca(fVList, fV1);
                fV1 = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fVList;
        }

        /// <summary>
        /// aggiunge ai filtri i documenti non annullati
        /// </summary>
        /// <param name="fVList"></param>
        /// <returns></returns>
        private static FiltroRicerca[] AddFiltroNoAnnullato(FiltroRicerca[] fVList)
        {
            try
            {
                DocsPaWR.FiltroRicerca fV1 = new DocsPaWR.FiltroRicerca();

                fV1.argomento = DocsPaWR.FiltriDocumento.ANNULLATO.ToString();
                fV1.valore = "0";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fVList;
        }

        /// <summary>
        /// Aggiunge ai filtri l'oggetto
        /// </summary>
        /// <param name="oggetto"></param>
        /// <param name="fVList"></param>
        /// <returns></returns>
        private static FiltroRicerca[] AddFiltroOggetto(string oggetto, FiltroRicerca[] fVList)
        {
            try
            {
                DocsPaWR.FiltroRicerca fV1 = new DocsPaWR.FiltroRicerca();

                fV1.argomento = DocsPaWR.FiltriDocumento.OGGETTO.ToString();
                fV1.valore = DO_AdattaString(oggetto);
                fVList = addToArrayFiltroRicerca(fVList, fV1);
                fV1 = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fVList;
        }

        /// <summary>
        /// rimuove i newline e eventuali formattazioni speciali dal testo
        /// </summary>
        /// <param name="valore"></param>
        /// <returns></returns>
        private static string DO_AdattaString(string valore)
        {
            try
            {
                valore = valore.Trim();
                valore = valore.Replace("\r", "");
                valore = valore.Replace("\n", "");
                return valore;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// restituisce i risultati della ricerca
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="query"></param>
        /// <param name="numPage"></param>
        /// <param name="numTotPage"></param>
        /// <param name="nRec"></param>
        /// <param name="security"></param>
        /// <param name="getIdProfilesList"></param>
        /// <param name="gridPersonalization"></param>
        /// <param name="pageSize"></param>
        /// <param name="export"></param>
        /// <param name="visibleFieldsTemplate"></param>
        /// <param name="documentsSystemId"></param>
        /// <param name="idProfilesList"></param>
        /// <returns></returns>
        public static SearchObject[] getQueryInfoDocumentoPagingCustom(InfoUtente infoUtente, DocsPaWR.FiltroRicerca[][] query, int numPage, out int numTotPage, out int nRec, bool security, bool getIdProfilesList, bool gridPersonalization, int pageSize, bool export, Field[] visibleFieldsTemplate, String[] documentsSystemId, out SearchResultInfo[] idProfilesList)
        {
            // La lista dei system id dei documenti restituiti dalla ricerca
            SearchResultInfo[] idProfiles = null;
            SearchObject[] DocS = null;
            nRec = 0;
            numTotPage = 0;
            try
            {
                string textToSearch = string.Empty;

                if (!IsRicercaFullText(query, out textToSearch))
                {
                    DocS = docsPaWS.DocumentoGetQueryDocumentoPagingCustom(infoUtente, query, numPage, security, pageSize, getIdProfilesList, gridPersonalization, export, visibleFieldsTemplate, documentsSystemId, out numTotPage, out nRec, out idProfiles);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            idProfilesList = idProfiles;
            return DocS;
        }

        /// <summary>
        /// verirfica se si deve eseguire una ricerca fulltext
        /// </summary>
        /// <param name="objQueryList"></param>
        /// <param name="textToSearch"></param>
        /// <returns></returns>
        private static bool IsRicercaFullText(FiltroRicerca[][] objQueryList, out string textToSearch)
        {
            string oggetto = string.Empty;
            string testoContenuto = string.Empty;
            textToSearch = string.Empty;
            bool ricercaFullText = false;
            try
            {
                for (int i = 0; i < objQueryList.Length; i++)
                {
                    for (int j = 0; j < objQueryList[i].Length; j++)
                    {
                        FiltroRicerca f = objQueryList[i][j];

                        switch (f.argomento)
                        {
                            case "RICERCA_FULL_TEXT":
                                if (f.valore != "0")
                                    ricercaFullText = true;
                                break;

                            case "TESTO_RICERCA_FULL_TEXT":
                                testoContenuto = f.valore;
                                break;

                            case "OGGETTO":
                                oggetto = f.valore;
                                break;

                            default:
                                break;
                        }
                    }
                }
                textToSearch = (ricercaFullText ? testoContenuto : oggetto);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return (ricercaFullText && textToSearch != string.Empty ? true : false);
        }

        public static InfoDocumento[] FullTextSearch(InfoUtente infoUtente, ref FullTextSearchContext context)
        {
            InfoDocumento[] retValue = null;

            try
            {
                retValue = docsPaWS.FullTextSearch(infoUtente, ref context);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return retValue;
        }

        public static InfoDocumento[] getQueryInfoDocumentoPaging(InfoUtente infoutente, DocsPaWR.FiltroRicerca[][] query, int numPage, out int numTotPage, out int nRec, bool comingPopUp, bool grigi, bool security, bool getIdProfilesList, out SearchResultInfo[] idProfilesList)
        {
            // La lista dei system id dei documenti restituiti dalla ricerca
            SearchResultInfo[] idProfiles = null;
            InfoDocumento[] DocS = null;
            nRec = 0;
            numTotPage = 0;
            try
            {

                string textToSearch = string.Empty;

                if (!IsRicercaFullText(query, out textToSearch))
                {
                    DocS = docsPaWS.DocumentoGetQueryDocumentoPaging(infoutente.idGruppo, infoutente.idPeople, query, comingPopUp, grigi, numPage, security, getIdProfilesList, out numTotPage, out nRec, out idProfiles);
                }
                else
                {
                    // reperimento oggetto infoutente

                    // Reperimento dalla sessione del contesto di ricerca fulltext
                    FullTextSearchContext context = GetFullTextSearchContextinSession;

                    if (context == null)
                        // Prima ricerca fulltext
                        context = new FullTextSearchContext();
                    else if (!textToSearch.Equals(context.TextToSearch))
                        // Se il testo inserito per la ricerca è differente
                        // da quello presente in sessione viene creato 
                        // un nuovo oggetto di contesto per la ricerca
                        context = new FullTextSearchContext();

                    // Impostazione indice pagina richiesta
                    context.RequestedPageNumber = numPage;
                    // Impostazione testo da ricercare
                    context.TextToSearch = textToSearch;

                    // Ricerca fulltext
                    DocS = FullTextSearch(infoutente, ref context);

                    // Reperimento numero pagine e record totali
                    numTotPage = context.TotalPageNumber;
                    nRec = context.TotalRecordCount;

                    // Impostazione in sessione del contesto di ricerca fulltext
                    GetFullTextSearchContextinSession = context;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

            // Impostazione della lista dei sisyem id dei documento
            idProfilesList = idProfiles;

            return DocS;
        }

        /// <summary>
        /// inserisce un documento in un fascicolo
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="idFolder"></param>
        /// <param name="infoutente"></param>
        /// <returns></returns>
        public static bool addDocumentoInFolder(string idProfile, string idFolder, InfoUtente infoutente)
        {
            bool result = false;
            try
            {
                //True: se il documento è già classificato nella folder indicata, false altrimenti
                bool isInFolder = docsPaWS.IsDocumentoClassificatoInFolder(idProfile, idFolder);

                if (!isInFolder)
                {
                    Folder fol = UIManager.ProjectManager.getFolder(idFolder, infoutente);
                    //se il doc non è già classificato nella folder indicata allora lo inserisco
                    Fascicolo fasc = UIManager.ProjectManager.getFascicoloById(fol.idFascicolo);
                    string msg = string.Empty;
                    result = docsPaWS.FascicolazioneAddDocFolder(infoutente, idProfile, fol, fasc.descrizione, out msg);
                    fol = null;
                    fasc = null;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return result;
        }

        /// <summary>
        /// verifica se un docuemnto risulta già inserito nel fascicolo
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="idFolder"></param>
        /// <returns></returns>
        public static bool isDocumentiInFolder(string idProfile, string idFolder)
        {
            bool result = false;
            try
            {
                return result = docsPaWS.IsDocumentoClassificatoInFolder(idProfile, idFolder);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return result;
        }
    }
}
