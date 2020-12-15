using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using DocsPaVO.DesktopApps;

namespace BusinessLogic.DesktopApps
{
    public class DesktopAppsManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(DesktopAppsManager));

        /// <summary>
        /// Metodo che restituisce i dati relativi ad una applicazione desktop
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static DesktopApp GetDesktopApp(string nomeApp, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DesktopApp deskApp = null;
            List<DesktopApp> listDesktopApp = new List<DesktopApp>();
            try
            {
                DocsPaDB.Query_DocsPAWS.DesktopApps desktopApp = new DocsPaDB.Query_DocsPAWS.DesktopApps();
                listDesktopApp = desktopApp.getDesktopApps();

                deskApp = listDesktopApp.FirstOrDefault(o => o.Nome == nomeApp);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetEventNotification ", e);
            }
            return deskApp;
        }
    }
}
