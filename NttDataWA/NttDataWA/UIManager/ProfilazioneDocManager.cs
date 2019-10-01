using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;

namespace NttDataWA.UIManager
{
    public class ProfilazioneDocManager
    {

        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();


        public static Templates getTemplateById(string idTemplate, Page page)
        {
            try
            {
                Templates template = docsPaWS.getTemplateById(idTemplate);

                //Se la tipologia è di campi comuni (Iperdocumento) richiamo il metodo che mi restituisce il temmplate
                //affinchè vengano visualizzati solo i campi comuni sui quali si ha visibilità rispetto alle tipologie
                //di documento associate al ruolo
                if (template != null && template.IPER_FASC_DOC == "1" && page != null)
                {
                    try
                    {
                        DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
                        template = docsPaWS.getTemplateCampiComuniById(UserManager.GetInfoUser(), idTemplate);
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
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

    }
}