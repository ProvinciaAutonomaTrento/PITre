using System;
using System.Web;
using System.Linq;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;
using System.Collections.Generic;

namespace NttDataWA.UIManager
{
    public class SimplifiedInteroperabilityManager
    {
        private static DocsPaWebService _ws = new DocsPaWebService();
        /// <summary>
        /// Stringa identificativa del canale di comunicazione per l'interoperabilità semplificata
        /// </summary>
        public const String SimplifiedInteroperabilityId = "SIMPLIFIEDINTEROPERABILITY";

        /// <summary>
        /// Url del servizio di interoperabilità impostato per l'istanza corrente.
        /// </summary>
        public static String InteroperabilityServiceUrl
        {
            get
            {
                try
                {
                    InfoUtente infoUtente;

                    try
                    {
                        infoUtente = UserManager.GetInfoUser();
                    }
                    catch (Exception e)
                    {
                        //SessionManager sm = new SessionManager();
                        infoUtente = UserManager.GetInfoUser();
                        string[] amministrazione = ((string)HttpContext.Current.Session["AMMDATASET"]).Split('@');
                        string codiceAmministrazione = amministrazione[0];
                        infoUtente.idAmministrazione = new DocsPaWebService().getIdAmmByCod(codiceAmministrazione);
                    }
                    String valoreChiaveInteropUrl = InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "INTEROP_SERVICE_URL");
                    if (string.IsNullOrEmpty(valoreChiaveInteropUrl))
                        valoreChiaveInteropUrl = InitConfigurationKeys.GetValue("0", "INTEROP_SERVICE_URL");

                    return valoreChiaveInteropUrl;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return null;
                }
            }
        }

        /// <summary>
        /// Booleano utilizzato per indicare se è attiva l'interoperabilità semplificata
        /// </summary>
        public static bool IsEnabledSimpInterop
        {
            get
            {
                try
                {
                    InfoUtente infoUtente;

                    try
                    {
                        infoUtente = UserManager.GetInfoUser();
                    }
                    catch (Exception e)
                    {
                        //SessionManager sm = new SessionManager();
                        infoUtente = UserManager.GetInfoUser();
                        string[] amministrazione = ((string)HttpContext.Current.Session["AMMDATASET"]).Split('@');
                        string codiceAmministrazione = amministrazione[0];
                        infoUtente.idAmministrazione = new DocsPaWebService().getIdAmmByCod(codiceAmministrazione);
                    }

                    String valoreChiaveInterop = InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "INTEROP_SERVICE_ACTIVE");
                    if (string.IsNullOrEmpty(valoreChiaveInterop))
                        valoreChiaveInterop = InitConfigurationKeys.GetValue("0", "INTEROP_SERVICE_ACTIVE");

                    return valoreChiaveInterop == "1";
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// Metodo per il salvataggio delle impostazioni di interoperabilità semplificata
        /// </summary>
        /// <param name="interoperabilitySettings">Impostazioni da salvare</param>
        /// <returns>Esito del salvataggio</returns>
        public static bool SaveSimplifiedInteroperabilitySettings(InteroperabilitySettings interoperabilitySettings)
        {
            try
            {
                return _ws.SaveSimplifiedInteroperabilitySettings(interoperabilitySettings);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Metodo per il caricamento delle impostazioni di interoperabilità semplificata
        /// </summary>
        /// <param name="registryId">Id del registro per cui recuperare le impostazioni</param>
        /// <returns>Esito del salvataggio</returns>
        public static InteroperabilitySettings LoadSimplifiedInteroperabilitySettings(String registryId)
        {
            try
            {
                return _ws.LoadSimplifiedInteroperabilitySettings(registryId);

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Metodo per la creazione dell'array con gli URL per l'interoperabilità semplificata. Questo metodo restituisce
        /// URL solo se l'AOO associata alla UO o all'RF è abilitata all'Interoperabilità semplificata
        /// </summary>
        /// <param name="tipo">Tipo di corrispondente</param>
        /// <param name="id">Id dell'elemento per cui bisogna restituire gli URL</param>
        /// <returns>URL dei servizi di interoperabilità semplificata</returns>
        internal static global::RubricaComune.Proxy.Elementi.UrlInfo[] GetUrls(global::RubricaComune.Proxy.Elementi.Tipo tipo, String id)
        {
            try
            {
                bool isInteroperable = IsEnabledSimpInterop && _ws.IsElementInteroperableWithSimplifiedInteroperability(id, tipo == global::RubricaComune.Proxy.Elementi.Tipo.RF);

                if (isInteroperable)
                    return new global::RubricaComune.Proxy.Elementi.UrlInfo[] { new global::RubricaComune.Proxy.Elementi.UrlInfo { Url = SimplifiedInteroperabilityManager.InteroperabilityServiceUrl } };
                return null;

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Metodo per la creazione dell'array con gli URL per l'interoperabilità semplificata. Questo metodo restituisce
        /// URL solo se l'AOO associata alla UO o all'RF è abilitata all'Interoperabilità semplificata
        /// </summary>
        /// <param name="tipo">Tipo di corrispondente</param>
        /// <param name="id">Id dell'elemento per cui bisogna restituire gli URL</param>
        /// <returns>URL dei servizi di interoperabilità semplificata</returns>
        /*
                internal static global::RubricaComune.Proxy.Elementi.UrlInfo[] GetUrls(global::RubricaComune.Proxy.Elementi.Tipo tipo, String id)
                {
                    bool isInteroperable = IsEnabledSimpInterop && _ws.IsElementInteroperableWithSimplifiedInteroperability(id, tipo == global::RubricaComune.Proxy.Elementi.Tipo.RF);

                    if (isInteroperable)
                        return new global::RubricaComune.Proxy.Elementi.UrlInfo[] { new global::RubricaComune.Proxy.Elementi.UrlInfo { Url = SimplifiedInteroperabilityManager.InteroperabilityServiceUrl } };
                    return null;

                }
         */

        /// <summary>
        /// Questo metodo verifica se è possibile agire sul flag "Privato" in un predisposto.
        /// L'azione è possibile solo se il documento è un predisposto in ingresso, è stato ricevuto 
        /// per interoperabilità semplificata e per il registro in cui il documento è stato creato, 
        /// è attiva la gestione manuale ed il mantenimento del documento come pendente
        /// </summary>
        /// <param name="typeId">Identificativo della tipologia di documento</param>
        /// <param name="registryId">Id del del registro su cui è stato creato il predisposto</param>
        /// <returns>Esito della verifica</returns>
        public static bool EnablePrivateCheck(Protocollo proto, String typeId, String registryId)
        {
            try
            {
                InteroperabilitySettings settings = LoadSimplifiedInteroperabilitySettings(registryId);
                return proto is ProtocolloEntrata &&
                    !String.IsNullOrEmpty(typeId) && typeId == SimplifiedInteroperabilityId &&
                    settings.ManagementMode == ManagementType.M && settings.KeepPrivate;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Questo metodo verifica se un documento ricevuto per interoperabilità semplificata
        /// è stato ricevuto marcato come privato
        /// </summary>
        /// <param name="documentId">Id del documento da controllare</param>
        /// <returns>Flag indicante se il documento è stato ricevuto marcato come privato</returns>
        public static bool IsDocumentReceivedPrivate(String documentId)
        {
            try
            {
                return _ws.IsDocumentReceivedFromSimplifiedInteroperabilityMarkedAdPrivate(documentId);

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Metodo utilizzato per verificare se un ruolo è abilitato alla rimozione del predisposto (se è
        /// abilitato compare, nel dettaglio della trasmissione, un pulsante "Non di competenza dell'amministrazione"
        /// se il documento è un documento ricevuto per IS
        /// </summary>
        /// <returns></returns>
        public static bool IsRoleEnabledToRemoveDocument()
        {
            try
            {
                return UserManager.IsAuthorizedFunctions("DELPREDIS");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Descrizione del canale interoperabilità semplificata
        /// </summary>
        public static String ChannelDescription
        {
            get
            {
                try
                {
                    if ((HttpContext.Current.Session["SimplifiedInteroperability.ChannelDescription"]) == null)
                    {
                        string description = _ws.GetDescrizioneTipoDocumento(SimplifiedInteroperabilityId);
                        if (string.IsNullOrEmpty(description))
                            return "Interoperabilità PITRE";
                        HttpContext.Current.Session["SimplifiedInteroperability.ChannelDescription"] = description;
                    }

                    return HttpContext.Current.Session["SimplifiedInteroperability.ChannelDescription"].ToString();
                    //return "Interoperabilità PITRE";
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return null;
                }
            }
        }

        /// <summary>
        /// Etichetta per il filtro sulle ricevute per la maschera degli allegati
        /// </summary>
        public static String SearchItemDescriprion
        {
            get
            {
                try
                {
                    if ((HttpContext.Current.Session["SimplifiedInteroperability.SearchItemDescriprion"]) == null)
                    {
                            string label = _ws.GetLabelTipoDocumento(SimplifiedInteroperabilityId);
                            if (string.IsNullOrEmpty(label))
                                return "PITRE";
                            HttpContext.Current.Session["SimplifiedInteroperability.SearchItemDescriprion"] =label;
                    }

                    return HttpContext.Current.Session["SimplifiedInteroperability.SearchItemDescriprion"].ToString();
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return null;
                }
            }
        }

        public static bool IsDocumentReceivedWithIS(String idProfile)
        {
            try
            {
                return _ws.IsDocumentReceivedWithIS(idProfile);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Metodo per la ricerca delle notifiche di spedizione relative al documento specificato
        /// </summary>
        public static Notifica[] getNotifica(string docnumber)
        {
            try
            {
                Notifica[] notifica = null;

                if (string.IsNullOrEmpty(docnumber))
                    return notifica;

                try
                {
                    notifica = _ws.ricercaNotifica(docnumber);
                }
                catch (Exception e)
                {
                    throw SoapExceptionParser.GetOriginalException(e);
                }

                return notifica;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static TipoNotifica getTipoNotifica(string systemIdTipoNotifica)
        {
            try
            {
                TipoNotifica tiponotifica = null;

                if (string.IsNullOrEmpty(systemIdTipoNotifica))
                    return tiponotifica;

                try
                {
                    tiponotifica = _ws.getTipoNotifica(systemIdTipoNotifica);
                }
                catch (Exception e)
                {
                    throw SoapExceptionParser.GetOriginalException(e);
                }

                return tiponotifica;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
    }
}