using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPAWA.DocsPaWR;
using DocsPAWA.AdminTool.Manager;
using System.Web.UI;

namespace DocsPAWA.utils
{
    /// <summary>
    /// Questa classe fornisce funzionalità per la gestione dell'IS
    /// </summary>
    public class InteroperabilitaSemplificataManager
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
                InfoUtente infoUtente;

                try
                {
                    infoUtente = UserManager.getInfoUtente();
                }
                catch (Exception e)
                {
                    SessionManager sm = new SessionManager();
                    infoUtente = sm.getUserAmmSession();
                    string[] amministrazione = ((string)HttpContext.Current.Session["AMMDATASET"]).Split('@');
                    string codiceAmministrazione = amministrazione[0];
                    infoUtente.idAmministrazione = new DocsPaWebService().getIdAmmByCod(codiceAmministrazione);

                }

                String valoreChiaveInteropUrl = DocsPAWA.utils.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "INTEROP_SERVICE_URL");
                if (string.IsNullOrEmpty(valoreChiaveInteropUrl))
                    valoreChiaveInteropUrl = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "INTEROP_SERVICE_URL");

                return valoreChiaveInteropUrl;
            }

        }

        /// <summary>
        /// Booleano utilizzato per indicare se è attiva l'interoperabilità semplificata
        /// </summary>
        public static bool IsEnabledSimpInterop
        {
            get
            {
                InfoUtente infoUtente;

                try
                {
                    infoUtente = UserManager.getInfoUtente();
                }
                catch (Exception e)
                {
                    SessionManager sm = new SessionManager();
                    infoUtente = sm.getUserAmmSession();
                    string[] amministrazione = ((string)HttpContext.Current.Session["AMMDATASET"]).Split('@');
                    string codiceAmministrazione = amministrazione[0];
                    infoUtente.idAmministrazione = new DocsPaWebService().getIdAmmByCod(codiceAmministrazione);

                }

                String valoreChiaveInterop = DocsPAWA.utils.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "INTEROP_SERVICE_ACTIVE");
                if (string.IsNullOrEmpty(valoreChiaveInterop))
                    valoreChiaveInterop = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "INTEROP_SERVICE_ACTIVE");

                return valoreChiaveInterop == "1";
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
            catch (Exception e)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(e);

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
            catch (Exception e)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(e);

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
            bool isInteroperable = IsEnabledSimpInterop && _ws.IsElementInteroperableWithSimplifiedInteroperability(id, tipo == global::RubricaComune.Proxy.Elementi.Tipo.RF);

            if (isInteroperable)
                return new global::RubricaComune.Proxy.Elementi.UrlInfo[] { new global::RubricaComune.Proxy.Elementi.UrlInfo { Url = InteroperabilitaSemplificataManager.InteroperabilityServiceUrl } };
            return null;
                
        }

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
            InteroperabilitySettings settings = LoadSimplifiedInteroperabilitySettings(registryId);
            return proto is ProtocolloEntrata &&
                !String.IsNullOrEmpty(typeId) && typeId == SimplifiedInteroperabilityId &&
                settings.ManagementMode == ManagementType.M && settings.KeepPrivate;

        }

        /// <summary>
        /// Questo metodo verifica se un documento ricevuto per interoperabilità semplificata
        /// è stato ricevuto marcato come privato
        /// </summary>
        /// <param name="documentId">Id del documento da controllare</param>
        /// <returns>Flag indicante se il documento è stato ricevuto marcato come privato</returns>
        public static bool IsDocumentReceivedPrivate(String documentId)
        {
            return _ws.IsDocumentReceivedFromSimplifiedInteroperabilityMarkedAdPrivate(documentId);
        }

        /// <summary>
        /// Metodo utilizzato per verificare se un ruolo è abilitato alla rimozione del predisposto (se è
        /// abilitato compare, nel dettaglio della trasmissione, un pulsante "Non di competenza dell'amministrazione"
        /// se il documento è un documento ricevuto per IS
        /// </summary>
        /// <returns></returns>
        public static bool IsRoleEnabledToRemoveDocument(Page page)
        {
            return UserManager.ruoloIsAutorized(page, "DELPREDIS");
        }

        /// <summary>
        /// Descrizione del canale interoperabilità semplificata
        /// </summary>
        public static String ChannelDescription
        {
            get
            {
                return "Interoperabilità PITRE";
            }
        }

        /// <summary>
        /// Etichetta per il filtro sulle ricevute per la maschera degli allegati
        /// </summary>
        public static String SearchItemDescriprion
        {
            get
            {
                return "PITRE";
            }
        }

        public static bool IsDocumentReceivedWithIS(String idProfile)
        {
            return _ws.IsDocumentReceivedWithIS(idProfile);
        }


    }
}