using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections;
using Debugger = ConservazioneWA.Utils.Debugger;
using System.IO;
using ConservazioneWA.DocsPaWR;
using System.Xml;
using System.Collections.Generic;



namespace ConservazioneWA.Utils
{
    public class ConservazioneManager
    {
        //protected static WSConservazioneLocale.DocsPaConservazioneWS wss = new ProxyManager().getProxy();
        //protected static WSConservazioneLocale.DocsPaConservazioneWSwse wse = new ProxyManager().getProxyWse();

        protected static WSConservazioneLocale.DocsPaConservazioneWS wss = new ProxyManager().getProxy();
        protected static DocsPaWR.DocsPaWebService ws = new ProxyManager().getProxyDocsPa();

        static bool? suppRimovibiliVerificabili = null;
        static string httpStorageRemoteUrlAddressUrl = null;
        //aggiungere a tutte le chiamate ((WSConservazioneLocale.InfoUtente)Session["infoutCons"])

        public static WSConservazioneLocale.InfoConservazione[] getAreaConservazione(WSConservazioneLocale.InfoUtente infoUt)
        {
            WSConservazioneLocale.InfoConservazione[] infoCons = null;
            try
            {
                infoCons = wss.getAllInfoConservazione(infoUt);
            }
            catch (Exception e)
            {
                infoCons = null;
                Debugger.Write("Errore nel get delle istanze di conservazione: " + e.Message);
            }
            return infoCons;
        }

        public static WSConservazioneLocale.AreaConservazioneValidationResult validateIstanzaConservazione(string idIstanza, WSConservazioneLocale.InfoUtente infoUt)
        {
            WSConservazioneLocale.AreaConservazioneValidationResult retValue = null;
            try
            {
                retValue = wss.validateIstanzaConservazione(idIstanza, infoUt);
            }
            catch (Exception ex)
            {
                retValue = null;
                Debugger.Write("Errore nella validazione dell'istanza di conservazione: " + ex.Message);
            }
            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <param name="infoUt"></param>
        /// <returns></returns>
        public static WSConservazioneLocale.AreaConservazioneValidationResult ValidaFileFirmati(string idIstanza, WSConservazioneLocale.InfoUtente infoUt, out int totale, out int valid, out int invalid)
        {
            WSConservazioneLocale.AreaConservazioneValidationResult retValue = null;

            totale = 0;
            valid = 0;
            invalid = 0;

            try
            {
                retValue = wss.ValidaFileFirmati(idIstanza, infoUt, out totale, out valid, out invalid);
            }
            catch (Exception ex)
            {
                retValue = null;
                Debugger.Write("Errore nella validazione dell'istanza di conservazione: " + ex.Message);
            }

            return retValue;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <param name="infoUt"></param>
        /// <returns></returns>
        public static WSConservazioneLocale.AreaConservazioneValidationResult ValidaFormatoFile(string idIstanza, WSConservazioneLocale.InfoUtente infoUt)
        {
            WSConservazioneLocale.AreaConservazioneValidationResult retValue = null;

            try
            {
                retValue = wss.ValidaFormatoFile(idIstanza, infoUt);
            }
            catch (Exception ex)
            {
                retValue = null;
                Debugger.Write("Errore nella validazione dell'istanza di conservazione: " + ex.Message);
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static WSConservazioneLocale.ItemsConservazione[] getItemsConservazioneWithContentValidation(string idIstanza, WSConservazioneLocale.InfoUtente infoUtente)
        {
            return wss.getItemsConservazioneWithContentValidation(idIstanza, infoUtente);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <param name="infoUt"></param>
        /// <returns></returns>
        public static WSConservazioneLocale.AreaConservazioneValidationResult ValidaFileMarcati(string idIstanza, WSConservazioneLocale.InfoUtente infoUt, out int totale, out int valid, out int invalid)
        {
            WSConservazioneLocale.AreaConservazioneValidationResult retValue = null;

            totale = 0;
            valid = 0;
            invalid = 0;

            try
            {
                retValue = wss.ValidaFileMarcati(idIstanza, infoUt, out totale, out valid, out invalid);
            }
            catch (Exception ex)
            {
                retValue = null;
                Debugger.Write("Errore nella validazione dell'istanza di conservazione: " + ex.Message);
            }

            return retValue;
        }


        public static WSConservazioneLocale.ItemsConservazione[] getItemsConservazione(string idIstanza, WSConservazioneLocale.InfoUtente infoUt)
        {
            WSConservazioneLocale.ItemsConservazione[] itemsCons = null;
            try
            {
                itemsCons = wss.getItemsConservazione(idIstanza, infoUt);
            }
            catch (Exception ex)
            {
                itemsCons = null;
                Debugger.Write("Errore nel get degli items in conservazione: " + ex.Message);
            }
            return itemsCons;
        }

        /// <summary>
        /// Metodo invocato per effettuare la ricerca degli item conservazione presenti nella security
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <param name="infoUt"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public static WSConservazioneLocale.ItemsConservazione[] getItemsConservazioneWithSecurity(string idIstanza, WSConservazioneLocale.InfoUtente infoUt, string idGruppo)
        {
            WSConservazioneLocale.ItemsConservazione[] itemsCons = null;
            try
            {
                itemsCons = wss.getItemsConservazioneWithSecurity(idIstanza, infoUt, idGruppo);
            }
            catch (Exception ex)
            {
                itemsCons = null;
                Debugger.Write("Errore nel get degli items in conservazione con security: " + ex.Message);
            }
            return itemsCons;
        }

        public static WSConservazioneLocale.ItemsConservazione[] getItemsConservazioneLite(string idIstanza, WSConservazioneLocale.InfoUtente infoUt)
        {
            WSConservazioneLocale.ItemsConservazione[] itemsCons = null;
            try
            {
                itemsCons = wss.getItemsConservazioneLite(idIstanza, infoUt);
            }
            catch (Exception ex)
            {
                itemsCons = null;
                Debugger.Write("Errore nel get degli items in conservazione: " + ex.Message);
            }
            return itemsCons;
        }

        public static WSConservazioneLocale.InfoConservazione[] getAreaConservazioneFiltro(string filtro, WSConservazioneLocale.InfoUtente infoUt)
        {
            WSConservazioneLocale.InfoConservazione[] infoCons = null;
            try
            {
                infoCons = wss.RicercaInfoConservazione(filtro, infoUt);
            }
            catch (Exception e)
            {
                infoCons = null;
                Debugger.Write("Errore nella ricerca delle istanze di conservazione: " + e.Message);
            }
            return infoCons;
        }

        public static WSConservazioneLocale.InfoSupporto[] getInfoSupporto(string filtro, WSConservazioneLocale.InfoUtente infoUtente)
        {
            WSConservazioneLocale.InfoSupporto[] infoSupp = null;
            try
            {
                infoSupp = wss.RicercaInfoSupporto(filtro, infoUtente);
            }
            catch (Exception e)
            {
                infoSupp = null;
                Debugger.Write("Errore nella ricerca dei supporti: " + e.Message);
            }
            return infoSupp;
        }

        public static bool insertSupporto(string filtro, WSConservazioneLocale.InfoUtente infoUt)
        {
            bool result = true;
            try
            {
                result = wss.InsertInfoSupporto(filtro, infoUt);
            }
            catch (Exception e)
            {
                result = false;
                Debugger.Write("Errore nell'inserimento dei supporti: " + e.Message);
            }
            return result;
        }

        public static bool updateInfoConservazione(string filtro, WSConservazioneLocale.InfoUtente infoUt)
        {
            bool result = true;
            try
            {
                result = wss.UpdateInfoConservazione(filtro, infoUt);
            }
            catch (Exception e)
            {
                result = false;
                Debugger.Write("Errore nell'aggiornamento delle istanze di conservazione: " + e.Message);
            }
            return result;
        }

        public static WSConservazioneLocale.esitoCons generaFolder(string sysId, WSConservazioneLocale.InfoUtente infoUt)
        {
            //nuova gestione dei messaggi di errore **********************************
            WSConservazioneLocale.esitoCons esito = new ConservazioneWA.WSConservazioneLocale.esitoCons();
            bool result = true;
            esito.esito = result;

            try
            {
                esito = wss.avviaConservazione(sysId, infoUt);
            }
            catch (Exception e)
            {
                result = false;
                //Debugger.Write("Errore nella generazione della cartella: " + e.Message);
                Debugger.Write("Errore nella comunicazione con il servizio: " + e.Message);

                //nuova gestione dei messaggi di errore **********************************
                esito.esito = result;
                esito.messaggio = "Errore nella comunicazione con il servizio riprovare in seguito";
            }

            return esito;
        }

        public static bool UpdateSupporto(string filtro, WSConservazioneLocale.InfoUtente infoUt)
        {
            bool result = true;
            try
            {
                result = wss.UpdateInfoSupporto(filtro, infoUt);
            }
            catch (Exception e)
            {
                result = false;
                Debugger.Write("Errore nell'aggiornamento dei supporti: " + e.Message);
            }
            return result;
        }

        /// <summary>
        /// Reperimento dei tipi supporto
        /// </summary>
        /// <returns></returns>
        public static WSConservazioneLocale.TipoSupporto[] GetTipiSupporto()
        {
            return wss.GetTipiSupporto();
        }

        public static int setSupporto(string copia, string collFisica, string dataUltimaVer, string dataEliminazione,
                                        string esitoUltimaVer, string numVer, string dataProxVer, string dataAppoMarca, string dataScadMarca, string marca, string idCons,
                                        string tipoSupp, string stato, string note, string query, string idSupp, string percVerifica, WSConservazioneLocale.InfoUtente infoUt, string progressivoMarca,
                                        out int newId)
        {
            newId = 0;
            int result = 0;
            try
            {
                result = wss.SetDpaSupporto(copia, collFisica, dataUltimaVer, dataEliminazione, esitoUltimaVer, numVer, dataProxVer, dataAppoMarca, dataScadMarca, marca, idCons, tipoSupp, stato, note, query, idSupp, percVerifica, infoUt, progressivoMarca, out newId);
            }
            catch (Exception e)
            {
                result = -1;
                Debugger.Write("Errore nel setSupporto: " + e.Message);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public static void createZipFile(string idIstanza, WSConservazioneLocale.InfoUtente infoUt)
        {
            try
            {
                wss.createZipFile(idIstanza, infoUt);
            }
            catch (Exception e)
            {
                Debugger.Write("Errore di comunicazione con il servizio per la creazione del file zip: " + e.Message);
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public static bool SubmitToRemoteFolder(string idIstanza, WSConservazioneLocale.InfoUtente infoUt)
        {
            try
            {
                return wss.SubmitToRemoteFolder(idIstanza, infoUt);
            }
            catch (Exception e)
            {
                Debugger.Write("Errore di comunicazione con il servizio per la l'invio dell'istanza sullo store remoto " + e.Message);
                return false;
            }
        }

        public static bool trasmettiNotifica(string idIstanzaCons, WSConservazioneLocale.InfoUtente infoUtente)
        {
            bool result = true;
            try
            {
                result = wss.trasmettiNotifica(infoUtente, idIstanzaCons, infoUtente.idPeople);
            }
            catch (Exception e)
            {
                result = false;
                Debugger.Write("Errore nella trasmissione della notifica: " + e.Message);
            }
            return result;
        }

        public static bool apponiMarca(string idIstanzaCons, WSConservazioneLocale.InfoUtente infoUtente, string progressivoMarca, string idProfileTrasm)
        {
            bool result = true;
            try
            {
                result = wss.getTSR(idIstanzaCons, infoUtente, progressivoMarca, idProfileTrasm);
            }
            catch (Exception e)
            {
                result = false;
                Debugger.Write("Errore nella rigenerazione della marca temporale: " + e.Message);
            }
            return result;
        }
        public static bool trasmettiNotificaRifiuto(string noteRifiuto, string idIstanzaCons, WSConservazioneLocale.InfoUtente infoUtente)
        {
            bool result = true;
            try
            {
                result = wss.trasmettiNotificaRifiuto(infoUtente, idIstanzaCons, noteRifiuto);


            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }
        public static string getDbType()
        {
            string dbType = string.Empty;
            try
            {
                dbType = wss.getDbType();
            }
            catch
            {
                Debugger.Write("Errore di comunicazione con il Server (metodo: getDbType)");
            }
            return dbType;
        }

        public static string getXmlPrefixName()
        {
            string xmlPrefixName = string.Empty;
            try
            {
                xmlPrefixName = wss.consXmlPrefixName();
            }
            catch
            {
                Debugger.Write("Errore di comunicazione con il server (metodo: getXmlPrefixName)");
            }
            return xmlPrefixName;
        }

        public static string getImpronta(WSConservazioneLocale.InfoUtente utente, string pathFile)
        {
            string impronta = string.Empty;
            try
            {
                FileStream fs = new FileStream(pathFile, FileMode.Open, FileAccess.Read);
                DocsPaWR.FileDocumento fileDoc = new ConservazioneWA.DocsPaWR.FileDocumento();
                fileDoc.content = new byte[fs.Length];
                fs.Read(fileDoc.content, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                return wss.calcolaImprontaFile(utente, fileDoc.content, true);
            }
            catch
            {
                Debugger.Write("Errore di comunicazione con il server (WSConservazioneLocale.InfoUtente utentemetodo: getImpronta)");
                return impronta;
            }
        }


        public static int verificaFileFirmato(string path, WSConservazioneLocale.InfoUtente infoUtente, string IDCons)
        {
            int result = 0;
            try
            {
                result = wss.verificaFirma(path, infoUtente, IDCons);
            }
            catch
            {
                Debugger.Write("Errore di comunicazione con il server (metodo: verificaFileFirmato)");
            }
            return result;
        }
        public static string verificaMarca(string path, WSConservazioneLocale.InfoUtente infoUtente, string IDCons)
        {
            string result = "";
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                DocsPaWR.FileDocumento fileDoc = new ConservazioneWA.DocsPaWR.FileDocumento();
                fileDoc.content = new byte[fs.Length];
                fs.Read(fileDoc.content, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                result = wss.verifyMarca(fileDoc.content, infoUtente, IDCons);
            }
            catch
            {
                Debugger.Write("Errore di comunicazione con il server (metodo: verificaMarca)");
                result = "Errore di comunicazione con il server, riprovare in seguito";
            }
            return result;
        }

        public static bool insertRisultatoVerifica(WSConservazioneLocale.InfoUtente infoUtente, string idSupporto, string idIstanza, string note, string percentuale, string num_ver, string esito, string tipoVerifica)
        {
            bool result = false;
            try
            {
                result = wss.insertConsVerifica(infoUtente, idSupporto, idIstanza, note, percentuale, num_ver, esito, tipoVerifica);
            }
            catch
            {
                Debugger.Write("Errore di comunicazione con il server (metodo: insertRisultatoVerifica)");
            }
            return result;
        }
        public static WSConservazioneLocale.InfoSupporto[] getReportVerificheSupporto(string idConservazione, string idSupporto, WSConservazioneLocale.InfoUtente infoUtente)
        {
            WSConservazioneLocale.InfoSupporto[] infoSupp = null;
            try
            {
                infoSupp = wss.conservazioneGetReportVerifiche(idConservazione, idSupporto, infoUtente);
            }
            catch
            {
                Debugger.Write("Errore di comunicazione con il server (metodo: getReportVerificheSupporto)");
                infoSupp = null;
            }
            return infoSupp;
        }
        public static void deleteIstanza(string idIstanza, WSConservazioneLocale.InfoUtente utente)
        {
            try
            {
                wss.deleteDirectoryIstanzaCons(idIstanza, utente);
            }
            catch
            {
                Debugger.Write("Errore di comunicazione con il server (metodo: deleteIstanza)");
            }
        }

        public static string getHttpFullPath()
        {
            string httpRootPath = GetHttpRootPath();

            httpRootPath += HttpContext.Current.Request.ApplicationPath;

            return httpRootPath;
        }

        /// <summary>
        /// Reperimento url root dell'applicazione
        /// </summary>
        /// <returns></returns>
        private static string GetHttpRootPath()
        {
            string httpRootPath = string.Empty;

            if (HttpContext.Current.Session["useStaticRootPath"] != null)
            {
                // Reperimento root path statico impostato nella configurazione del file web.config
                bool useStaticRoot;

                if (bool.TryParse(HttpContext.Current.Session["useStaticRootPath"].ToString(), out useStaticRoot))
                {
                    if (useStaticRoot)
                    {
                        httpRootPath = System.Configuration.ConfigurationManager.AppSettings["STATIC_ROOT_PATH"];
                    }
                }
            }

            if (string.IsNullOrEmpty(httpRootPath))
            {
                // Se non è stata impostata alcuna configurazione da web.config relativamente al path statico
                // o se l'applicazione è stata avviata senza l'utilizzo di quest'ultimo (ossia in modalità standard),
                // viene effettuato il reperimento del path in maniera dinamica
                HttpRequest request = HttpContext.Current.Request;

                httpRootPath = request.Url.Scheme + "://" + request.Url.Host;

                if (!request.Url.Port.Equals(80))
                    httpRootPath += ":" + request.Url.Port;
            }

            return httpRootPath;
        }


        public static WSConservazioneLocale.Policy[] GetListaPolicy(int idAmm, string tipo)
        {
            WSConservazioneLocale.Policy[] result = null;
            try
            {
                result = wss.GetListaPolicy(idAmm, tipo);
            }
            catch (Exception ex)
            {
                result = null;
                Debugger.Write("Errore nella validazione dell'istanza di conservazione: " + ex.Message);
            }
            return result;
        }

        public static bool ValidateIstanzaConservazioneConPolicy(string idPolicy, string idIstanza, WSConservazioneLocale.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                result = wss.ValidateIstanzaConservazioneConPolicy(idPolicy, idIstanza, infoUtente);
            }
            catch (Exception ex)
            {
                result = false;
                Debugger.Write("Errore nella validazione dell'istanza di conservazione con la policy: " + ex.Message);
            }
            return result;
        }

        public static bool DeleteValidateIstanzaConservazioneConPolicy(string idPolicy, string idIstanza, WSConservazioneLocale.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                result = wss.DeleteValidateIstanzaConservazioneConPolicy(idPolicy, idIstanza, infoUtente);
            }
            catch (Exception ex)
            {
                result = false;
                Debugger.Write("Errore nella validazione dell'istanza di conservazione con la policy: " + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Registrazione di un supporto rimovibile
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <param name="idSupporto"></param>
        /// <param name="collocazione"></param>
        /// <param name="note"></param>
        public static void RegistraSupportoRimovibile(WSConservazioneLocale.InfoUtente infoUtente, string idIstanza, string idSupporto, string collocazione, string note)
        {
            wss.RegistraSupportoRimovibile(infoUtente, idIstanza, idSupporto, collocazione, note);
        }

        /// <summary>
        /// Registrazione dell'esito della verifica di integrità di un supporto rimovibile registrato
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <param name="idSupporto"></param>
        /// <param name="esitoVerifica"></param>
        /// <param name="percentualeVerifica"></param>
        /// <param name="dataProssimaVerifica"></param>
        /// <param name="noteDiVerifica"></param>
        public static void RegistraEsitoVerificaSupportoRegistrato(
                                            WSConservazioneLocale.InfoUtente infoUtente, 
                                            string idIstanza, 
                                            string idSupporto, 
                                            bool esitoVerifica, 
                                            string percentualeVerifica,
                                            string dataProssimaVerifica,
                                            string noteDiVerifica,
                                            string tipoVerifica)
        {
            wss.RegistraEsitoVerificaSupportoRegistrato(
                                                    infoUtente,
                                                    idIstanza, 
                                                    idSupporto, 
                                                    esitoVerifica, 
                                                    percentualeVerifica, 
                                                    dataProssimaVerifica,
                                                    noteDiVerifica,
                                                    tipoVerifica);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static WSConservazioneLocale.TipoIstanzaConservazione[] GetTipiIstanza()
        {
            return wss.GetTipiIstanza();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static WSConservazioneLocale.StatoIstanza[] GetStatiIstanza()
        {
            return wss.GetStatiIstanza();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static WSConservazioneLocale.StatoSupporto[] GetStatiSupporto()
        {
            return wss.GetStatiSupporto();
        }

        public static WSConservazioneLocale.Contatori GetContatori(WSConservazioneLocale.InfoUtente infoUtente)
        {
            return wss.GetContatori(infoUtente);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public static WSConservazioneLocale.Policy GetPolicyIstanza(WSConservazioneLocale.InfoUtente infoUtente, string idIstanza)
        {
            return wss.GetPolicyIstanza(infoUtente, idIstanza);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idConservazione"></param>
        public static void IniziaVerificaSupportoRemoto(WSConservazioneLocale.InfoUtente infoUtente, string idConservazione)
        {
            wss.IniziaVerificaSupportoRemoto(infoUtente, idConservazione);
        }

        public static WSConservazioneLocale.Registro[] GetRfByIdAmm(int idAmm, string tipo)
        {
            WSConservazioneLocale.Registro[] result = null;
            try
            {
                result = wss.GetRfByIdAmm(idAmm, tipo);
            }
            catch (Exception ex)
            {
                result = null;
                Debugger.Write("Errore nel reperimento dei registri dell'amministrazione: " + ex.Message);
            }
            return result;
        }

        public static WSConservazioneLocale.Fascicolo[] GetFascicoloDaCodiceNoSecurity(string codiceFasc, string idAmm, bool isRicFasc)
        {
            try
            {
                WSConservazioneLocale.Fascicolo[] result = wss.GetFascicoloDaCodiceNoSecurity(codiceFasc, idAmm, string.Empty,false, isRicFasc);

                return result;
            }
            catch (Exception es)
            {
                return null;
            }
        }

        public static WSConservazioneLocale.Fascicolo GetFascicoloByID(string idFascicolo)
        {
            try
            {
                WSConservazioneLocale.Fascicolo result = wss.GetFascicoloById(idFascicolo);

                return result;
            }
            catch (Exception es)
            {
                return null;
            }
        }

        public static WSConservazioneLocale.Folder FascicolazioneGetFolder(WSConservazioneLocale.InfoUtente infout, WSConservazioneLocale.Fascicolo fascicolo) 
        {
            WSConservazioneLocale.Folder result;

            try
            {
                result = wss.FascicolazioneGetFolder(infout.idPeople, infout.idGruppo, fascicolo);
            }
            catch (Exception exc) 
            {
                result = null;
            }
            return result;
        }

        public static WSConservazioneLocale.TemplateLite[] GetTypeDocumentsWithDiagramByIdAmm(int idAmministrazione, string type)
        {
            WSConservazioneLocale.TemplateLite[] result = null;

            try
            {
                result = wss.GetTypeDocumentsWithDiagramByIdAmm(idAmministrazione, type); ;

            }
            catch (Exception e)
            {
                return null;
                throw e;
            }
            return result;
        }

        public static ArrayList getExtFileAcquisiti(DocsPaWR.InfoUtente infoUtente)
        {
            ArrayList result = null;
            try
            {
                result = new ArrayList(wss.getListaExtFileAcquisiti(infoUtente.idAmministrazione));
            }
            catch (Exception e)
            {
                result = null;
            }
            return result;
        }

        public static WSConservazioneLocale.SearchObject[] getQueryInfoDocumentoPagingCustom(WSConservazioneLocale.InfoUtente infoUtente, WSConservazioneLocale.FiltroRicerca[][] query, int numPage, out int numTotPage, out int nRec, bool security, bool getIdProfilesList, bool gridPersonalization, int pageSize, bool export, WSConservazioneLocale.Field[] visibleFieldsTemplate, String[] documentsSystemId, out WSConservazioneLocale.SearchResultInfo[] idProfilesList)
        {
            // La lista dei system id dei documenti restituiti dalla ricerca
            WSConservazioneLocale.SearchResultInfo[] idProfiles = null;

            nRec = 0;
            numTotPage = 0;
            try
            {
                WSConservazioneLocale.SearchObject[] DocS = null;

                DocS = wss.DocumentoGetQueryDocumentoPagingCustom(infoUtente, query, numPage, security, pageSize, getIdProfilesList, gridPersonalization, export, visibleFieldsTemplate, documentsSystemId, out numTotPage, out nRec, out idProfiles);
               

                if (DocS == null)
                {
                    throw new Exception();
                }

                // Impostazione della lista dei sisyem id dei documento
                idProfilesList = idProfiles;

                return DocS;
            }
            catch (Exception es)
            {
                Debugger.Write("Errore nel getQueryInfoDocumentoPagingCustom dell'amministrazione: " + es.Message);
            }

            idProfilesList = idProfiles;
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idConservazione"></param>
        /// <param name="noteIstanza"></param>
        /// <param name="copieSupportiRimovibili"></param>
        public static void MettiInLavorazioneAsync(WSConservazioneLocale.InfoUtente infoUtente, string idConservazione, string noteIstanza, int copieSupportiRimovibili)
        {
            wss.MettiInLavorazioneAsync(infoUtente, idConservazione, noteIstanza, copieSupportiRimovibili);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <returns></returns>
        public static bool IsInPreparazioneAsync(string idConservazione)
        {
            return wss.IsInPreparazioneAsync(idConservazione);
        }

        public static ArrayList getTitolariUtilizzabili(string idAmministrazione)
        {
            ArrayList titolari = new ArrayList();
            try
            {
                titolari = new ArrayList(wss.getTitolariUtilizzabili(idAmministrazione));
                return titolari;
            }
            catch (Exception e)
            {
                Debugger.Write("Errore in ConservazioneManager - metodo: getTitolariUtilizzabili", e);
                return titolari;
            }
        }

        public static WSConservazioneLocale.SearchObject[] GetListaFascicoliPagingCustom(WSConservazioneLocale.FascicolazioneClassificazione classificazione, WSConservazioneLocale.Registro registro, WSConservazioneLocale.FiltroRicerca[] filtriRicerca, bool childs, int requestedPage, out int numTotPage, out int nRec, int pageSize, bool getSystemIdList, out  WSConservazioneLocale.SearchResultInfo[] idProjectList, byte[] datiExcel, bool showGridPersonalization, bool export, WSConservazioneLocale.Field[] visibleFieldsTemplate, String[] documentsSystemId, WSConservazioneLocale.InfoUtente infoUtente, bool security)
        {
            nRec = 0;
            numTotPage = 0;

            // Lista dei system id dei fascicoli
            WSConservazioneLocale.SearchResultInfo[] idProjects = null;

            try
            {
                WSConservazioneLocale.SearchObject[] result = wss.FascicolazioneGetListaFascicoliPagingCustom(infoUtente, classificazione, registro, filtriRicerca, false, false, childs, requestedPage, pageSize, getSystemIdList, datiExcel, showGridPersonalization, export, visibleFieldsTemplate, documentsSystemId, security,out numTotPage, out nRec, out idProjects);

                if (result == null)
                {
                    throw new Exception();
                }

                // Salvataggio della lista dei system id dei fascicoli
                idProjectList = idProjects;

                return result;
            }
            catch (Exception es)
            {
                Debugger.Write("Errore in ConservazioneManager - metodo: getListaFascicoliPagingCustom", es);
            }

            idProjectList = null;

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public static bool IsIstanzaConservazioneInterna(WSConservazioneLocale.InfoUtente infoUtente, string idIstanza)
        {
            return wss.IsIstanzaConservazioneInterna(infoUtente, idIstanza);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static WSConservazioneLocale.InfoNotifica[] GetNotifiche(string idAmm)
        {
            return wss.GetNotifiche(idAmm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <param name="dataIniziale"></param>
        /// <param name="dataFinale"></param>
        /// <param name="utente"></param>
        /// <returns></returns>
        public static WSConservazioneLocale.LogConservazione[] GetLogs(WSConservazioneLocale.InfoUtente infoUtente,
                                              string idIstanza,
                                              string dataIniziale,
                                              string dataFinale,
                                              string utente,
                                              string azione,
                                              string esito)
        {
            return wss.GetLogs(infoUtente, idIstanza, dataIniziale, dataFinale, utente, azione, esito);
        }

        public static WSConservazioneLocale.LogConservazione[] GetListAzioniLog()
        {
            return wss.GetListAzioniLog();

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <param name="dataIniziale"></param>
        /// <param name="dataFinale"></param>
        /// <param name="utente"></param>
        /// <returns></returns>
        public static WSConservazioneLocale.InfoAmministrazione GetInfoAmmCorrente(string idAmm)
        {
            return wss.AmmGetInfoAmmCorrente(idAmm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idDocumento"></param>
        /// <param name="indiceAllegato"></param>
        /// <returns></returns>
        public static WSConservazioneLocale.FileDocumento GetFileDocumentoNotifica(
                                WSConservazioneLocale.InfoUtente infoUtente,
                                string idDocumento,
                                int indiceAllegato)
        {
            return wss.GetFileDocumentoNotifica(infoUtente, idDocumento, indiceAllegato);
        }

        public static WSConservazioneLocale.FileDocumento GetFileDocumentoFirmato(
            WSConservazioneLocale.InfoUtente infoUtente, string idDocumento, int indiceAllegato)
        {
            return wss.GetFileDocumentoFirmato(infoUtente, idDocumento, indiceAllegato);
        }

        public static WSConservazioneLocale.Templates getTemplateFascById(string idTemplate, Page page, WSConservazioneLocale.InfoUtente infoUtente)
        {
            try
            {
                WSConservazioneLocale.Templates template = wss.getTemplateFascById(idTemplate);

                //Se la tipologia è di campi comuni (Iperfascicolo) richiamo il metodo che mi restituisce il temmplate
                //affinchè vengano visualizzati solo i campi comuni sui quali si ha visibilità rispetto alle tipologie
                //di fascicolo associate al ruolo
                if (template != null && template.IPER_FASC_DOC == "1" && page != null)
                {
                    try
                    {
                        template = wss.getTemplateFascCampiComuniById(infoUtente, idTemplate);
                    }
                    catch (Exception e)
                    {
                        //In questo caso vuol dire che provengo da amministrazione e l'infoUtente non esiste
                        //quindi il template non va filtrato per visibilità
                    }
                }

                if (template != null)
                    return template;
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static WSConservazioneLocale.Templates getTemplateById(string idTemplate, Page page, WSConservazioneLocale.InfoUtente infoUtente)
        {
            try
            {
                WSConservazioneLocale.Templates template = wss.getTemplateById(idTemplate);

                //Se la tipologia è di campi comuni (Iperdocumento) richiamo il metodo che mi restituisce il temmplate
                //affinchè vengano visualizzati solo i campi comuni sui quali si ha visibilità rispetto alle tipologie
                //di documento associate al ruolo
                if (template != null && template.IPER_FASC_DOC == "1" && page != null)
                {
                    try
                    {
                        template = wss.getTemplateCampiComuniById(infoUtente, idTemplate);
                    }
                    catch (Exception e)
                    {
                        //In questo caso vuol dire che provengo da amministrazione e l'infoUtente non esiste
                        //quindi il template non va filtrato per visibilità
                    }
                }

                if (template != null)
                    return template;
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static ConservazioneWA.UserControl.Calendar.VisibleTimeModeEnum getVisibleTimeMode(WSConservazioneLocale.OggettoCustom oggettoCustom)
        {
            switch (oggettoCustom.FORMATO_ORA.ToUpper())
            {
                case "HH":
                    return ConservazioneWA.UserControl.Calendar.VisibleTimeModeEnum.Hours;

                case "HH:MM":
                    return ConservazioneWA.UserControl.Calendar.VisibleTimeModeEnum.Minutes;

                case "HH:MM:SS":
                    return ConservazioneWA.UserControl.Calendar.VisibleTimeModeEnum.Seconds;
                default:
                    return ConservazioneWA.UserControl.Calendar.VisibleTimeModeEnum.Nothing;
            }
        }

        public static WSConservazioneLocale.Corrispondente getCorrispondenteBySystemID(Page page, string systemID)
        {
            try
            {
                return wss.AddressbookGetCorrispondenteBySystemId(systemID);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static WSConservazioneLocale.Corrispondente getCorrispondenteByCodRubrica(Page page, string codice, WSConservazioneLocale.Registro[] regAll, WSConservazioneLocale.InfoUtente infoUtente)
        {
            try
            {
                string condRegistri = string.Empty;
                if (regAll != null && regAll.Length > 0)
                {
                    condRegistri = " and (id_registro in (";
                    foreach (WSConservazioneLocale.Registro reg in regAll)
                        condRegistri += reg.systemId + ",";
                    condRegistri = condRegistri.Substring(0, condRegistri.Length - 1);
                    condRegistri += ") OR id_registro is null)";
                }

                return wss.AddressbookGetCorrispondenteByCodRubrica(codice, infoUtente, condRegistri);
            }
            catch (Exception e)
            {
                return null;
            }
            
        }

        public static WSConservazioneLocale.ElementoRubrica getElementoRubrica(Page page, string cod, string condregistri, WSConservazioneLocale.InfoUtente infoUtente)
        {
            try
            {
                return wss.rubricaGetElementoRubrica(cod, infoUtente, null, condregistri);
            }
            catch (Exception e)
            {
                return null;
            }
            
        }

        public static WSConservazioneLocale.ElementoRubrica getElementoRubricaSimpleBySystemId(Page page, string systemId, WSConservazioneLocale.InfoUtente infoUtente)
        {
            try
            {
                return wss.rubricaGetElementoRubricaSimpleBySystemId(systemId, infoUtente);
            }
             catch (Exception e)
            {
                
            }
            return null;
        }

        /// <summary>
        /// Verifica se un'istanza è abilitata alla lavorazione
        /// </summary>
        /// <param name="idConservazione">id dell'istanza di Conservazione</param>
        /// <returns></returns>
        public static bool abilitaLavorazione(string idConservazione)
        {
            try
            {
                return wss.abilitaLavorazione(idConservazione);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica se un'istanza è abilitata alla lavorazione
        /// </summary>
        /// <param name="idConservazione">id dell'istanza di Conservazione</param>
        /// <returns></returns>
        public static int getValidationMask(string idConservazione)
        {
            try
            {
                return wss.getValidationMask(idConservazione);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        /// <summary>
        /// Verifica se un'istanza ha la policy validata
        /// </summary>
        /// <param name="idConservazione">id dell'istanza della conservazione</param>
        /// <returns></returns>
        public static bool policyVerificata(string idConservazione)
        {
            try
            {
                return wss.policyVerificata(idConservazione);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static void inserimentoInRegistroControlli(string idConservazione, string docnumber, WSConservazioneLocale.InfoUtente infoUtente, string tipoOperazione, bool esito, int verificati, int validi, int invalidi)
        {
            wss.inserimentoInRegistroControlli (idConservazione, docnumber, infoUtente, tipoOperazione, esito, verificati, validi, invalidi);
        }

        public static void inserimentoInRegistroCons(WSConservazioneLocale.RegistroCons regCons, WSConservazioneLocale.InfoUtente infoUtente)
        {
            wss.inserimentoInRegistroCons(regCons, infoUtente);
        }

        public static void inserimentoInDpaLog(WSConservazioneLocale.InfoUtente infoUtente, string WebMethodName, string ID_Oggetto, string Var_desc_Oggetto, WSConservazioneLocale.Esito Cha_Esito)
        {
            wss.inserimentoInDpaLog(infoUtente, WebMethodName, ID_Oggetto, Var_desc_Oggetto, Cha_Esito);
        }


        /// <summary>
        /// Ricava l'hash di un file dallo storage remoto, specificando l'istanza
        /// </summary>
        /// <param name="infoutente">utente loggato</param>
        /// <param name="idConservazione">id conservazione</param>
        /// <param name="path"> path relativo del file in conservazione</param>
        /// <returns>bytearray del file</returns>
        public static string getFileHashFromStore(ConservazioneWA.WSConservazioneLocale.InfoUtente infoutente, string idConservazione, string path,bool localStore)
        {
            return wss.getFileHashFromStore(infoutente, idConservazione, path,localStore);
        }

        /// <summary>
        /// Ricava un file dallo storage remoto, specificando l'istanza
        /// </summary>
        /// <param name="infoutente">utente loggato</param>
        /// <param name="idConservazione">id conservazione</param>
        /// <param name="path"> path relativo del file in conservazione</param>
        /// <returns>bytearray del file</returns>
        public static byte[] getFileFromStore(ConservazioneWA.WSConservazioneLocale.InfoUtente infoutente, string idConservazione, string path, bool localStore)
        {
            return wss.getFileFromStore(infoutente, idConservazione, path,localStore);
        }

        /// <summary>
        /// Metodo per l'analisi del file chiusura.xml, nel quale sono registrati gli id dei file, il loro content type, il path e l'hash.
        /// </summary>
        /// <param name="infoutente">l'utente loggato</param>
        /// <param name="idConservazione">id dell'istanza sulla quale si sta effettuando il test di leggibilità</param>
        /// <returns>oggetto Dictionary con i dati dei file, utilizzando come chiave l'id del documento</returns>
        public static Dictionary <String,String>  getFilesFromUniSincro(ConservazioneWA.WSConservazioneLocale.InfoUtente infoutente,string idConservazione,bool localStore)
        {
            Stream fileChiusura;
            XmlDocument xmlFile = new XmlDocument();
            fileChiusura = new MemoryStream(wss.getFileFromStore(infoutente, idConservazione, "\\Chiusura\\file_chiusura.xml", localStore));
            fileChiusura.Seek(0, SeekOrigin.Begin);
            Dictionary<String, String> retval = new Dictionary<string, string>();
            xmlFile.Load(fileChiusura);
            
            XmlNodeList nodes = xmlFile.GetElementsByTagName("sincro:File");
            foreach (XmlNode n in nodes)
            {
                string formato = n.Attributes[0].InnerText;
                string idDocumento = ((XmlNode)n.ChildNodes[0]).InnerText;
                string pathFile = ((XmlNode)n.ChildNodes[1]).InnerText;
                string hashSupporto = ((XmlNode)n.ChildNodes[2]).InnerText;
                try
                {
                    retval.Add(idDocumento, string.Format("{0}§{1}§{2}§{3}", formato, idDocumento, pathFile, hashSupporto));
                }
                catch (Exception ex)
                {
                    // probabile id ripetuta. non fare nulla. 
                    // Il documento è presente in più fascicoli inseriti in conservazione. L'hash è lo stesso.
                }
            }
            return retval;
        }


        /// <summary>
        /// Metodo per modificare la validation mask in seguito al controllo della leggibilità dei file.
        /// </summary>
        /// <param name="idConservazione">id dell'istanza di conservazione</param>
        /// <param name="passed">vero se i file sono leggibili</param>
        /// <returns></returns>
        public static bool esitoLeggibilita(ConservazioneWA.WSConservazioneLocale.InfoUtente infoutente,string idConservazione, bool passed)
        {
            return wss.esitoLeggibilita(infoutente, idConservazione, passed);
        }

        /// <summary>
        /// Metodo per modificare la validation mask in seguito al controllo della leggibilità dei file.
        /// </summary>
        /// <param name="idConservazione">id dell'istanza di conservazione</param>
        /// <param name="passed">vero se i file sono leggibili</param>
        /// <returns></returns>
        public static WSConservazioneLocale.FileDocumento sbustaFileFirmato(string idConservazione, string pathFile,bool localStore)
        {
            return wss.sbustaFileFirmato(idConservazione, pathFile,localStore);
        }

        /// <summary>
        /// ritorna se i supporti esterni sono rimovibili o meno
        /// </summary>
        /// <returns></returns>
        public static bool supportiRimovibiliVerificabili()
        {

            if (suppRimovibiliVerificabili == null)
            {
                suppRimovibiliVerificabili = wss.supportiRimovibiliVerificabili();
            }
            return suppRimovibiliVerificabili.Value;
        }

        /// <summary>
        /// ritorna l'url del servizio di download dello zip
        /// </summary>
        /// <returns></returns>
        public static string httpStorageRemoteUrlAddress()
        {


            if (httpStorageRemoteUrlAddressUrl == null)
            {
                httpStorageRemoteUrlAddressUrl = wss.httpStorageRemoteUrlAddress();
            }
            return httpStorageRemoteUrlAddressUrl;
        }


        
        /// <summary>
        /// Controlla se la dimensione tramite il metodo ctrlDimensioniIstanza nel Backend. Setta la validationMask.
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <returns></returns>
        public static bool dimensioneValida(string idConservazione, WSConservazioneLocale.InfoUtente infoUtente)
        {
            //bool retval = false;
            //int vm = getValidationMask(idConservazione);
            //if ((vm & 0x80) != 0x80)
            //{
            //    if ((vm & 0x10) != 0x10)
            //        retval = wss.ctrlDimensioniIstanza(idConservazione);
            //    else
            //        retval = true; 
            //}
            //else
            //{
            //    if ((vm & 0x10) != 0x10)
            //        retval = false;
            //    else
            //        retval = true;
            //}

            return wss.ctrlDimensioniIstanzaUt(idConservazione, infoUtente);
        }

        /// <summary>
        /// Controlla se la dimensione è valida tramite la validation mask.
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <returns></returns>
        public static bool dimensioneValidaVM(string idConservazione)
        {
            bool retval = false;
            int vm = getValidationMask(idConservazione);
            if ((vm & 0x10) != 0x10)
                retval = false;
            else
                retval = true;
            return retval;
        }

        /// <summary>
        /// Quando la dimensione eccede il massimo. Visualizza il messaggio di errore con le dimensioni massime consentite.
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <returns></returns>
        public static string getErrorMaxDimensioneIstanza(string idConservazione)
        {
            string maxDims = wss.getMaxDimensioniIstanza(idConservazione);
            string[] mdvals = maxDims.Split('§');
            string retval = string.Format(" - Dimensione istanza superiore a quella massima. Numero di documenti consentiti: {0}. Massima dimensione: {1} MB.", mdvals[0], mdvals[1]);
            return retval;
        }

        /// <summary>
        /// Nel caso un istanza non sia ancora stata verificata automaticamente, e non abbia una policy assegnata, questo metodo
        /// modifica la validation mask in maniera che non risulti l'errore di policy non valida (essendo assente, il comportamento 
        /// sarebbe errato).
        /// </summary>
        /// <param name="idConservazione"></param>
        public static void setPolicyVerificataLite(string idConservazione)
        {
            wss.setPolicyVerificataLite(idConservazione);
        }

        public static List<WSConservazioneLocale.StampaConservazione> GetListStampaConservazione(List<WSConservazioneLocale.FiltroRicerca> filters, WSConservazioneLocale.InfoUtente infoUtente)
        {
            
            WSConservazioneLocale.FiltroRicerca[] filtri = filters.ToArray();
            WSConservazioneLocale.StampaConservazione[] outStampa = wss.RicercaStampaConservazione(filtri, infoUtente);
            return outStampa.ToList();
            //return new List<WSConservazioneLocale.StampaConservazione>();
        }



        /// <summary>
       
        /// file generato per il report
        /// </summary>
        /// <param name="idConservazione"></param>
        public static WSConservazioneLocale.FileDocumento getFileReport(WSConservazioneLocale.FiltroRicerca[] qV, string tipoReport, string tipoFile,string titolo,string rKey,string cName, WSConservazioneLocale.InfoUtente infoUt)
        {
            return wss.createReportConservazione(qV,tipoReport, tipoFile,titolo,rKey,cName, infoUt);
            
        }


        /// <summary>
        /// Nel caso un istanza non sia ancora stata verificata automaticamente, e non abbia una policy assegnata, questo metodo
        /// modifica la validation mask in maniera che non risulti l'errore di policy non valida (essendo assente, il comportamento 
        /// sarebbe errato).
        /// </summary>
        /// <param name="idConservazione"></param>
        public static string getSegnatura_ID_Doc(string idProfile)
        {
            return wss.getSegnatura_ID_Doc(idProfile);
        }
        /// <summary>
        /// Restituisce l'elenco delle verifiche alle policy fallite.
        /// Utilizzato nel tooltip del dettaglio istanza
        /// </summary>
        /// <param name="maskPolicy"></param>
        /// <returns></returns>
        public static string GetListNonConfPolicy(string maskPolicy)
        {
            return wss.GetListNonConfPolicy(maskPolicy);
        }


        #region MEV CONS 1.4 - ESIBIZIONE

        public static WSConservazioneLocale.InfoEsibizione[] GetInfoEsibizione(WSConservazioneLocale.InfoUtente infoUtente, List<WSConservazioneLocale.FiltroRicerca> filters)
        {
            WSConservazioneLocale.InfoEsibizione[] infoEs = null;
            WSConservazioneLocale.FiltroRicerca[] searchFilters = filters.ToArray();
            infoEs = wss.GetInfoEsibizione(infoUtente, searchFilters);
            

            return infoEs;
        }

        public static WSConservazioneLocale.ItemsConservazione[] GetItemsEsibizione(WSConservazioneLocale.InfoUtente infoUtente, string idEsibizione)
        {

            WSConservazioneLocale.ItemsConservazione[] itemsEs = null;
            itemsEs = wss.GetItemsEsibizione(infoUtente, idEsibizione);

            return itemsEs;

        }

        public static bool RemoveItemsEsibizione(WSConservazioneLocale.InfoUtente infoUtente, string idDocumento)
        {
            return wss.RemoveItemsEsibizione(infoUtente, idDocumento);
        }

        public static bool RemoveIstanzaEsibizione(WSConservazioneLocale.InfoUtente infoUtente, string idEsibizione)
        {
            return wss.RemoveIstanzaEsibizione(infoUtente, idEsibizione);
        }

        public static bool SaveEsibizioneFields(WSConservazioneLocale.InfoUtente infoUtente, string idEsibizione, string descrizione, string note)
        {
            return wss.SaveFieldsEsibizione(infoUtente, idEsibizione, descrizione, note);
        }

        public static bool RichiediCertificazioneIstanzaEsibizione(WSConservazioneLocale.InfoUtente infoUtente, string idEsibizione, string descrizione, string note)
        {
            return wss.RichiediCertificazioneIstanzaEsibizione(infoUtente, idEsibizione, descrizione, note);
        }

        public static bool UpdateCertificazioneIstanzaEsibizione(WSConservazioneLocale.InfoUtente infoUtente, string idEsibizione)
        {
            return wss.UpdateCertificazioneIstanzaEsibizione(infoUtente, idEsibizione);
        }

        public static bool RifiutaCertificazioneIstanzaEsibizione(WSConservazioneLocale.InfoUtente infoUtente, string idEsibizione, string note)
        {
            return wss.RifiutaCertificazioneIstanzaEsibizione(infoUtente, idEsibizione, note);
        }

        public static string RiabilitaIstanzaEsibizione(WSConservazioneLocale.InfoUtente infoUtente, string idEsibizione)
        {
            return wss.RiabilitaIstanzaEsibizione(infoUtente, idEsibizione);
        }

        public static void ChiudiIstanzaEsibizione(WSConservazioneLocale.InfoUtente infoUtente, string idEsibizione, string descrizione, string note)
        {
            wss.ChiudiIstanzaEsibizioneAsync(infoUtente, idEsibizione, descrizione, note);


        }


        public static bool MarcaCertificazioneIstanzaEsibizione(string idEsibizione, WSConservazioneLocale.InfoUtente infoUtente, byte[] content)
        {
            return wss.MarcaCertificazioneIstanzaEsibizione(idEsibizione, infoUtente, content);
        }


        public static string GetEsibizioneDownloadUrl(WSConservazioneLocale.InfoUtente infoUtente, string idEsibizione)
        {
            return wss.GetEsibizioneDownloadUrl(infoUtente, idEsibizione);
        }

        #endregion


        /// <summary>
        /// Restituisce la stringa del profilo utente che ha effettuato accesso al Centro Servizi (CONSERVAZIONE / ESIBIZIONE).
        /// </summary>
        /// <param name="maskPolicy"></param>
        /// <returns></returns>
        public static string CalcolaProfiloUtente(string idPeople, string idAmm)
        {
            // Default l'utente è conservazione
            string ProfiloUtente = "CONSERVAZIONE";

            try
            {
                bool AbilitatoCentroServizi = false;
                bool AbilitatoEsibizione = false;

                // Chiamata al Backend che calcola il profilo Utente a partire dall'infoutente che si è loggato
                // passando i seguenti parametri: this.infoUtente.idAmministrazione, this.infoUtente.idPeople
                Dictionary<string, string> dAbilitatoCS = new Dictionary<string, string>();
                Dictionary<string, string> dAbilitatoE = new Dictionary<string, string>();

                // chiave del dictionary: idPeople_idAmm
                string key = idPeople + "_" + idAmm;

                string valueCS = string.Empty;
                string valueE = string.Empty;

                if (!dAbilitatoCS.ContainsKey(key))
                {
                    // Valore del campo abilitato CS proveniente dalla chiamata al BE
                    valueCS = wss.GetAbilitatoCentroServizi(idPeople, idAmm);

                    if (string.IsNullOrEmpty(valueCS))
                        valueCS = "0";

                    dAbilitatoCS.Add(key, valueCS);
                }
                else
                {
                    dAbilitatoCS.TryGetValue(key, out valueCS);
                }

                if (!dAbilitatoE.ContainsKey(key))
                {
                    // Valore del campo abilitato Esibizione proveniente dalla chiamata al BE
                    valueE = wss.GetAbilitatoEsibizione(idPeople, idAmm);

                    if (string.IsNullOrEmpty(valueE))
                        valueE = "0";
                    
                    dAbilitatoE.Add(key, valueE);
                }
                else
                {
                    dAbilitatoE.TryGetValue(key, out valueE);
                }

                // Valorizzo la variabile AbilitatoCentroServizi
                AbilitatoCentroServizi = valueCS.ToUpper().Equals("1") ? true : false;
                AbilitatoEsibizione = valueE.ToUpper().Equals("1") ? true : false;

                if (AbilitatoCentroServizi)
                {
                    ProfiloUtente = "CONSERVAZIONE";
                }

                if (AbilitatoEsibizione)
                {
                    ProfiloUtente = "ESIBIZIONE";
                }

                if (AbilitatoEsibizione && AbilitatoCentroServizi)
                {
                    ProfiloUtente = "CONSERVAZIONE_ESIBIZIONE";
                    //ProfiloUtente = "ESIBIZIONE";
                }
            }
            catch (Exception e) 
            {
                // Resta da decidere in caso di problemi su quale profilo andare:
                // Profilo Conservazione
                // Profilo Esibizione
                ProfiloUtente = "CONSERVAZIONE";
            }

            return ProfiloUtente;
        }

        /// <summary>
        /// Calcola i contatori della HomePage di esibizione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public static WSConservazioneLocale.ContatoriEsibizione GetContatoriEsibizione(WSConservazioneLocale.InfoUtente infoUtente, string idGruppo)
        {
            return wss.GetContatoriEsibizione(infoUtente, idGruppo);
        }

        /// <summary>
        /// Calcola i contatori di esibizione della HomePage di conservazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static WSConservazioneLocale.ContatoriEsibizione GetContatoriEsibizioneConservazione(WSConservazioneLocale.InfoUtente infoUtente)
        {
            return wss.GetContatoriEsibizioneConservazione(infoUtente);
        }

        /// <summary>
        /// Metodo per l'inserimento di documenti / fascicoli in area Esibizione
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="idProject"></param>
        /// <param name="docNumber"></param>
        /// <param name="infoUtente"></param>
        /// <param name="tipoOggetto"></param>
        /// <returns></returns>
        public static string CreateAndAddDocInAreaEsibizione(string idProfile, string idProject, string docNumber, WSConservazioneLocale.InfoUtente infoUtente, string tipoOggetto, string idConservazione, out WSConservazioneLocale.SchedaDocumento sd) 
        {
            // -1 : Errore
            string result = string.Empty;

            //
            // Invoco il metodo dell'asmx per la creazione dell'istanza di esibizione in DPA_AREA_ESIBIZIONE
            // e l'inserimento dell'item in DPA_ITEMS_ESIBIZIONE
            result = wss.CreateAndAddDocInAreaEsibizione(idProfile, idProject, docNumber, infoUtente, tipoOggetto, idConservazione, out sd);

            return result;
        }

        /// <summary>
        /// Serializza la scheda doc per calcolare la dimensione dell'item esibizione
        /// </summary>
        /// <param name="page"></param>
        /// <param name="schedaDoc"></param>
        /// <param name="idItem"></param>
        /// <returns></returns>
        public static int getItemSizeEsib(Page page, WSConservazioneLocale.SchedaDocumento schedaDoc, string idItem)
        {
            int size = 0;
            try
            {
                size = wss.SerializeSchedaEsib(schedaDoc, idItem);
                if (size == -1)
                    throw new Exception();
            }
            catch (Exception e)
            {
            }
            return size;
        }

        /// <summary>
        /// Aggiorna la dimensione dell'item esibizione
        /// </summary>
        /// <param name="page"></param>
        /// <param name="sysId"></param>
        /// <param name="size"></param>
        public static void insertSizeInItemEsib(Page page, string sysId, int size)
        {
            try
            {
                bool result = wss.UpdateSizeItemEsib(sysId, size);
                if (!result)
                    throw new Exception();
            }
            catch (Exception es)
            {   
            }
        }

        /// <summary>
        /// Aggiorna le informazioni dell'item esibizione
        /// </summary>
        /// <param name="page"></param>
        /// <param name="tipoFile"></param>
        /// <param name="numAllegati"></param>
        /// <param name="sysId"></param>
        public static void updateItemsEsibizione(Page page, string tipoFile, string numAllegati, string sysId)
        {
            try
            {
                bool result = wss.updateItemsEsib(tipoFile, numAllegati, sysId);
                if (!result)
                    throw new Exception();
            }
            catch (Exception es)
            {
            }
        }

        /// <summary>
        /// Metodo per verificare se il documento è presente in esibizione
        /// </summary>
        /// <param name="id_profile"></param>
        /// <param name="id_project"></param>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanzaConservazione"></param>
        /// <returns></returns>
        public static bool checkItemEsibizionePresenteInIstanzaEsibizione(string id_profile, string id_project, string type, WSConservazioneLocale.InfoUtente infoUtente, string idIstanzaConservazione)
        {
            bool presente = false;
            
            try
            {
                presente = wss.checkItemEsibizionePresenteInIstanzaEsibizione(id_profile, id_project, type, infoUtente, idIstanzaConservazione);
            }
            catch(Exception ex)
            {
            }

            return presente;
        }

        /// <summary>
        /// Metodo per recuperare le informazioni dell'istanza di conservazione a partire dal systemId
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static WSConservazioneLocale.InfoConservazione getInfoConservazione(string systemID, WSConservazioneLocale.InfoUtente infoUtente)
        {
            WSConservazioneLocale.InfoConservazione infoCons = null;

            try
            {
                string filtro = "WHERE SYSTEM_ID = " + systemID;
                infoCons = wss.RicercaInfoConservazione(filtro, infoUtente)[0];
            }
            catch (Exception ex)
            {
            }

            return infoCons;
        }

        /// <summary>
        /// Verifica se lo storage delle istanze è remoto o no
        /// </summary>
        /// <returns></returns>
        public static bool isLocalStore()
        {
            bool result = true;

            try
            {
                
                result = wss.isLocalStore();
            }
            catch (Exception ex)
            {
                result = true;
            }

            return result;
        }

        public static string GetIdCorrGlobaliEsibizione(string idGruppo)
        {

            return wss.GetIdCorrGlobaliEsibizione(idGruppo);

        }

        public static string rigeneraIstanza(string idConservazione, string idSupporto, WSConservazioneLocale.InfoUtente infoUtente)
        {
            string message = "";
            try
            {
                bool esito;
                esito = wss.rigeneraIstanza(idConservazione, idSupporto, infoUtente, out message);
            }
            catch (Exception ex)
            {
                message = "Si è verificato un errore nella rigenerazione dell'istanza";
            }

            return  message;        
        }

        public static bool isIstanzaRigenerata(string idIstanzaVecchia, WSConservazioneLocale.InfoUtente infoUtente)
        {
            try
            {
                return wss.isIstanzaRigenerata(idIstanzaVecchia, infoUtente);
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        //MEV CS 1.5 - Alert Conservazione

        /// <summary>
        /// Recupera la chiave di configurazione richiesta da DB
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetChiaveConfigurazione(string idAmm, string key)
        {
            return wss.GetChiaveConfigurazione(idAmm, key);
        }

        /// <summary>
        /// Determina se un dato alert è attivo o meno per l'amministrazione corrente.
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="codice"></param>
        /// <returns></returns>
        public static bool IsAlertConservazioneAttivo(string idAmm, string codice)
        {
            return wss.IsAlertConservazioneAttivo(idAmm, codice);
        }

        /// <summary>
        /// Recupera i parametri di configurazione impostati per un dato alert
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="codice"></param>
        /// <returns></returns>
        public static string GetParametriAlertConservazione(string idAmm, string codice)
        {
            return wss.GetParametriAlertConservazione(idAmm, codice);
        }

        /// <summary>
        /// Invia la mail corrispondente all'alert selezionato.
        /// Per gli alert con contatore, esegue una verifica preliminare sui raggiungimento
        /// delle condizioni necessarie per l'invio
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="codice"></param>
        /// <param name="idIstanza">id istanza di conservazione - utilizzato per gli alert sulle verifiche di leggibilità</param>
        /// <param name="idSupporto">id del supporto - utilizzato per gli alert sulle verifiche di leggibilità</param>
        public static void InvioAlertAsync(WSConservazioneLocale.InfoUtente infoUtente, string codice, string idIstanza, string idSupporto)
        {
            wss.InvioAlertAsync(infoUtente, codice, idIstanza, idSupporto);
        }

        /// <summary>
        /// Registrazione dell'esito della verifica di leggibilità di un supporto rimovibile registrato
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <param name="idSupporto"></param>
        /// <param name="esitoVerifica"></param>
        /// <param name="percentualeVerifica"></param>
        /// <param name="dataProssimaVerifica"></param>
        /// <param name="noteDiVerifica"></param>
        public static void RegistraEsitoVerificaLeggibilitaSupportoRegistrato(
                                            WSConservazioneLocale.InfoUtente infoUtente,
                                            string idIstanza,
                                            string idSupporto,
                                            bool esitoVerifica,
                                            string percentualeVerifica,
                                            string dataProssimaVerifica,
                                            string noteDiVerifica,
                                            string tipoVerifica)
        {
            wss.RegistraEsitoVerificaLeggibilitaSupportoRegistrato(
                                                    infoUtente,
                                                    idIstanza,
                                                    idSupporto,
                                                    esitoVerifica,
                                                    percentualeVerifica,
                                                    dataProssimaVerifica,
                                                    noteDiVerifica,
                                                    tipoVerifica);
        }

        /// <summary>
        /// Determina se una verifica di leggibilità viene eseguita prima dei termini previsti
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="idSupporto"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public static bool IsVerificaLeggibilitaAnticipata(string idConservazione, string idSupporto, string idAmm)
        {
            return wss.IsVerificaLeggibilitaAnticipata(idConservazione, idSupporto, idAmm);
        }

        //end MEV CS 1.5 - Alert Conservazione

        public static bool isRoleAuthorized(string idCorrGlobali, string codFunzione)
        {
            bool result = false;

            DocsPaWR.Ruolo role = ws.GetRuolo(idCorrGlobali);
            if (role != null && role.funzioni != null)
            {
                foreach (Funzione f in role.funzioni)
                {
                    if (f.codice.ToUpper() == codFunzione.ToUpper())
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }
    }
}
 