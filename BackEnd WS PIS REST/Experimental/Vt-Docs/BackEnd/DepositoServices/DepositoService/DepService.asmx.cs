using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using log4net;
using DocsPaVO.utente;
using DocsPaVO.amministrazione;

namespace DepositoService
{
    /// <summary>
    /// Summary description for DepService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class DepService : System.Web.Services.WebService
    {
        private static ILog logger = LogManager.GetLogger(typeof(DepositoService.DepService));

        private DocsPaUtils.Data.ParameterSP CreateParameter(string name, object value)
        {
            return new DocsPaUtils.Data.ParameterSP(name, value);
        }

        [WebMethod]
        public int DO_getIdProfileByData(string numProto, string AnnoProto, string idRegistro, string idGruppo, string idPeople)
        {
            try
            {
                return DO_getIdProfileByDataPvt(numProto, AnnoProto, idRegistro, idGruppo, idPeople);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: DO_getIdProfileByData", ex);
                return -1;
            }

        }

        private int DO_getIdProfileByDataPvt(string numProto, string AnnoProto, string idRegistro, string idGruppo, string idPeople)
        {
            string inArchivio = string.Empty;
            string result = String.Empty;
            string commandText = String.Empty;
            DocsPaUtils.Query queryMngArchivio;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            if ((AnnoProto == null) && (idRegistro == null))
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DO_GET_IDPROFILE_BY_DATA_DOC_GRIGIO");
                queryMng.setParam("ID_DOC", numProto);
                queryMng.setParam("idGruppo", idGruppo);
                queryMng.setParam("idPeople", idPeople);
                commandText = queryMng.getSQL();
            }
            else
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DO_GET_IDPROFILE_BY_DATA");
                if (string.IsNullOrEmpty(numProto)) numProto = "0";
                queryMng.setParam("NUMPROTO", numProto);
                queryMng.setParam("ANNOPROTO", AnnoProto);
                queryMng.setParam("IDREGISTRO", idRegistro);
                queryMng.setParam("idGruppo", idGruppo);
                queryMng.setParam("idPeople", idPeople);
                commandText = queryMng.getSQL();
            }

            logger.Debug("InfoDocManager - DO_GET_IDPROFILE_BY_DATA: " + commandText);
            dbProvider.ExecuteScalar(out result, commandText);

            return Convert.ToInt32(result);
        }

    }
}
