using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace SAAdminTool.FascicolazioneCartacea
{
    /// <summary>
    /// Gestione dei dati in sessione per la fascicolazione cartacea
    /// </summary>
    public sealed class SessionManager
    {
        /// <summary>
        /// 
        /// </summary>
        private const string DOCUMENTI_FASCICOLAZIONE_SESSION_KEY = "FascicolazioneCartacea.DocumentiFascicolazione";

        /// <summary>
        /// 
        /// </summary>
        private const string FILTRI_FASCICOLAZIONE_SESSION_KEY = "FascicolazioneCartacea.FiltriFascicolazione";

        /// <summary>
        /// 
        /// </summary>
        private SessionManager()
        {
        }

        /// <summary>
        /// Documenti correntemente visualizzati
        /// </summary>
        /// <returns></returns>
        public static List<DocsPaWR.DocumentoFascicolazione> Documenti
        {
            get
            {
                if (HttpContext.Current.Session[DOCUMENTI_FASCICOLAZIONE_SESSION_KEY] != null)
                    return HttpContext.Current.Session[DOCUMENTI_FASCICOLAZIONE_SESSION_KEY] as List<DocsPaWR.DocumentoFascicolazione>;
                else
                    return null;
            }
            set
            {
                if (HttpContext.Current.Session[DOCUMENTI_FASCICOLAZIONE_SESSION_KEY] == null)
                    HttpContext.Current.Session.Add(DOCUMENTI_FASCICOLAZIONE_SESSION_KEY, value);
                else
                    HttpContext.Current.Session[DOCUMENTI_FASCICOLAZIONE_SESSION_KEY] = value;
            }
        }

        
        /// <summary>
        /// Reperimento filtri correntemente impostati
        /// </summary>
        /// <returns></returns>
        public static DocsPaWR.FiltroRicerca[] Filtri
        {
            get
            {
                if (HttpContext.Current.Session[FILTRI_FASCICOLAZIONE_SESSION_KEY] != null)
                    return HttpContext.Current.Session[FILTRI_FASCICOLAZIONE_SESSION_KEY] as DocsPaWR.FiltroRicerca[];
                else
                    return null;
            }
            set
            {
                if (HttpContext.Current.Session[FILTRI_FASCICOLAZIONE_SESSION_KEY] == null)
                    HttpContext.Current.Session.Add(FILTRI_FASCICOLAZIONE_SESSION_KEY, value);
                else
                    HttpContext.Current.Session[FILTRI_FASCICOLAZIONE_SESSION_KEY] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Clear()
        {
            HttpContext.Current.Session.Remove(DOCUMENTI_FASCICOLAZIONE_SESSION_KEY);
            HttpContext.Current.Session.Remove(FILTRI_FASCICOLAZIONE_SESSION_KEY);
        }
    }
}
