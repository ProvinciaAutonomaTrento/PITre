using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using SAAdminTool.DocsPaWR;

namespace SAAdminTool.RubricaComune
{
    /// <summary>
    /// Classe per il reperimento delle configurazioni DocsPa
    /// per l'utilizzo del sistema Rubrica Comune 
    /// </summary>
    public sealed class Configurazioni
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static ConfigurazioniRubricaComune GetConfigurazioni(InfoUtente infoUtente)
        {
            return new DocsPaWebService().GetConfigurazioniRubricaComune(infoUtente);
        }
    }
}