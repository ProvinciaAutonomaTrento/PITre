using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using DocsPAWA.DocsPaWR;
using log4net;
using DocsPAWA.AdminTool.Manager;

namespace DocsPAWA
{
    public class InteroperabilitaManager : Page
    {
        private static ILog logger = LogManager.GetLogger(typeof(InteroperabilitaManager));
        protected static DocsPaWebService ws = ProxyManager.getWS();

        public static Notifica[] getNotifica(string docnumber)
        {
            Notifica[] notifica = null;

            if (string.IsNullOrEmpty(docnumber))
                return notifica;

            try
            {
                notifica = ws.ricercaNotifica(docnumber);
            }
            catch(Exception e)
            {
                logger.Debug("errore in InetroperabilitaManager - getNotifica: "+e.Message);
            }

            return notifica;
        }

        public static Notifica[] getNotificaFiltra(FiltroRicerca[] filtri)
        {
            Notifica[] notifica = null;

            if (string.IsNullOrEmpty(filtri[0].valore)) // se idDoc null return
                return notifica;

            try
            {
                notifica = ws.ricercaNotificaFiltra(filtri);
            }
            catch (Exception e)
            {
                logger.Debug("errore in InetroperabilitaManager - getFiltraNotifica: " + e.Message);
            }

            return notifica;
        }
        public static TipoNotifica getTipoNotifica(string systemIdTipoNotifica)
        {
            TipoNotifica tiponotifica = null;

            if (string.IsNullOrEmpty(systemIdTipoNotifica))
                return tiponotifica;

            try
            {
                tiponotifica = ws.getTipoNotifica(systemIdTipoNotifica);
            }
            catch(Exception e)
            {
                logger.Debug("errore in InetroperabilitaManager - getNotifica: "+e.Message);
            }

            return tiponotifica;
        }

    }
}
