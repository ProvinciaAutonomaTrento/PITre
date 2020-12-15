using System;

namespace InformaticaTrentinaPCL.Network
{
    public static class NetworkConstants
    {
        
        static string BASE_URL_SVIL_SERVICE =  "https://mobile.pitre.tn.it/pat";
        static string BASE_URL_COLL_SERVICE = "https://mobile.pitre.tn.it/pat";
        static string BASE_URL_PROD_SERVICE = "https://mobile.pitre.tn.it/pat";
        static string BASE_URL_PREFERED = "https://mobile.pitre.tn.it/pat";
        public const string APP_PROTOCOL = "p3it";
        public const string APP_BASE_URL = "informaticatrentina.pi3.it";
        public const string APP_URL_ACTION = "share";

        public static string APP_BASE_SHARE_URL = APP_PROTOCOL + "://" + APP_BASE_URL + "/" + APP_URL_ACTION + "/";

        // Usato per switchare tra il server di SVILUPPO e quello di COLLAUDO 
        // quando si usano le direttive #DEBUG o #SVIL
        public static bool isCollaudo = true;

        //Max month ws limit for search documents
        public static int MAX_SEARCH_MONTH_LIMIT = 6;

        public static string GetBaseUrl()
        {
#if DEBUG || SVIL
            return isCollaudo ? BASE_URL_COLL_SERVICE : BASE_URL_SVIL_SERVICE;
#elif COLL
                    return BASE_URL_COLL_SERVICE;
#else
                    return BASE_URL_PROD_SERVICE;
#endif
        }

        /// <summary>
        /// Gets the URL prefered.
        /// </summary>
        /// <returns>The URL prefered.</returns>
        public static string GetUrlPrefered()
        {
            return BASE_URL_PREFERED;
        }

        /// <summary>
        /// Sets the URL prefered.
        /// </summary>
        /// <param name="url">URL.</param>
        public static void SetUrlPrefered(string url)
        {
            BASE_URL_PREFERED = url;
        }

        public static string GetDownloadUrl()
        {
            return GetBaseUrl() + "/downloadApp.html";
        }

        public static string GetShareLink(string key)
        {
            return GetBaseUrl() + "/share.aspx?key=" + Uri.EscapeDataString(key);
        }

        private static UsersListForTestHelper userListForTestHelper
          = new UsersListForTestHelper(3);
        public static string DEFAULT_USERNAME = userListForTestHelper.GetUsername();
        public static string DEFAULT_PASSWORD = userListForTestHelper.GetPassword();

        public static string RESPONSE_ERROR = "ERROR";
        public static string NETWORK_TOKEN_EMPTY = "SSO=";
        public static int DEFAULT_LIST_PAGE_SIZE = 10;
        public static string DEFAULT_STATO_DELEGA = //TODO for release set default to "A" 
#if RELEASE
            "N";
#else
          //TODO UPDATE THIS VALUE
          "N";
#endif
        public static string CONSTANT_ACCETTA_TRANSMISSION = "ACCETTA";
        public static string CONSTANT_RIFIUTA_TRANSMISSION = "RIFIUTA";
        public static string CONSTANT_ADD_ADL_ACTION = "ADD";
        public static string CONSTANT_REMOVE_ADL_ACTION = "REMOVE";

        public static string TIPO_RICERCA_DOC = "DOC"; // Per cercare tra i documenti
        public static string TIPO_RICERCA_FASC = "FASC"; // per cercare tra i fascicoli
        public static string TIPO_RICERCA_ALL = "ALL"; // per cercare sia i documenti che i fascicoli
        public static string TIPO_ADL_DOC = "ADL_DOC"; //per cercare tra i documenti dell'area di lavoro
        public static string TIPO_ADL_FASC = "ADL_FASC"; // per cercare tra i fascicoli dell'area di lavoro
        public static string TIPO_ADL_ALL = "ADL_ALL"; // per cercare sia documenti che fascicoli in area di lavoro

        public static int NUM_MAX_RESULT = 50;

        public static string TYPE_FAVORITE_DELEGA = "D";
        public static string TYPE_FAVORITE_TRASMISSIONE = "A";

        public static string TYPE_FAVORITE_URP_U = "U";
        public static string TYPE_FAVORITE_URP_R = "R";
        public static string TYPE_FAVORITE_URP_P = "P";

        public static string TYPE_DOCUMENT_F = "F";
        public static string TYPE_DOCUMENT_D = "D";

        public static string TIPO_TRASMISSIONE_S = "S";
        public static string TIPO_TRASMISSIONE_T = "T";
        public static string RAGIONE_TRASMISSIONE_COMPETENZA = "COMPETENZA";

        public static string TIPO_FIRMA_CADES = "CADES";
        public static string TIPO_FIRMA_PADES = "PADES";

        public static int CONSTANT_SEARCH_CHARS = 3;

        public static string KEY_USER_IMAGE = "userImage";

        public static String CONSTANT_DA_FIRMARE = "da_firmare";
        public static String CONSTANT_DA_RESPINGERE = "da_respingere";
        public static String CONSTANT_PROPOSTO = "proposto";

        public static String CONSTANT_TIPO_FIRMA_DOC_VERIFIED = "doc_verified";
        public static String CONSTANT_TIPO_FIRMA_DOC_STEP_OVER = "doc_step_over";
        public static String CONSTANT_TIPO_FIRMA_DOC_SIGNATURE_cades = "doc_signature";
        public static String CONSTANT_TIPO_FIRMA_DOC_SIGNATURE_P = "doc_signature_p";

        #region PINNING SSL

        public static readonly string[] SupportedPublicKeyForPinning =
        {
      "3082010A0282010100B630BF83EE1FB5C6D46AA3D4C3C15DB1C3E86518BEBC29BA12" +
      "080C9DBE2A5AB57D21E5C674C31FA9E279B07EB62D2EBDB843DC58867E5ADCFAF82E" +
      "88D279FF605686D7FC562888BF980A475124272D50E72F547719D89826DB50F2AA99" +
      "419B89BAC6144EAC9B8D47EF076EB41A5D739E3E6E5254C1D2CE8A2A7DA2248B6146" +
      "B7FEA51CCB8A640DD1347DFE4E2C4D855A87C9D7D2C8058E770E079D15C55673B25E" +
      "65D49160E08516FDCE77D83A1C5A895D3FBCAFCADCC9B3D7094DC964E80407E6A094" +
      "978E0268A609499F2813E95EFCF0E1C34E4636F2EEEAF2917EF1D9A8A0085CC5601D" +
      "0CA5A7F89DEE9046DADC2AC8AFB49C8F8C3F7E4062C3267105CE490203010001"
    };

        public static bool IsPinningSSLEnabled()
        {
#if RELEASE
            return  true;
#else
            return  false;
#endif
        }

        #endregion
    }

    public static class Constants
    {
        public static String DESCRIPTION_THANK_YOU_PAGE = "DESCRIPTION_THANK_YOU_PAGE";
        public static String TAG_NEW_LINE = "#newline#";

        public static string ACTION_SIGN = "ACTION_SIGN";
        public static string ACTION_REJECT = "ACTION_REJECT";
        public const string ACTION_SIGN_CADES = "ACTION_SIGN_CADES";
        public const string ACTION_SIGN_CADES_NOT_SIGNED = "ACTION_SIGN_CADES_NOT_SIGNED";
        public const string ACTION_SIGN_PADES = "ACTION_SIGN_PADES";

        public static string DOCUMENT_TYPE_DECRIPTION_KEY = "document_type_description";
    }
}