using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using log4net;

namespace VerifyAndSetTypeDocOrFasc
{
    class DBManagerCDC
    {
        private static ILog logger = LogManager.GetLogger(typeof(DBManagerCDC));

        public static DateTime getMaxDataRegistrazione(string idTemplate, string idOggetto, string idAooRf)
        {
            try
            {
                string maxDataRegistrazione = string.Empty;

                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("GET_MAX_DTA_REGISTRAZIONE");
                queryMng.setParam("idTemplate", idTemplate);
                queryMng.setParam("idOggetto", idOggetto);
                queryMng.setParam("idAooRf", idAooRf);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getMaxDataRegistrazione - VerifyAndSetCDC DBManagerCDC.cs - QUERY : " + commandText);
                logger.Debug("SQL - getMaxDataRegistrazione - VerifyAndSetCDC DBManagerCDC.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count != 0)
                {
                    maxDataRegistrazione = ds.Tables[0].Rows[0]["DTA_REGISTRAZIONE"].ToString();
                }

                if (!string.IsNullOrEmpty(maxDataRegistrazione))
                    return Convert.ToDateTime(maxDataRegistrazione);
                else
                    return DateTime.MinValue;
            }
            catch (Exception ex)
            {
                return DateTime.MinValue;
            }
        }

        public static string getValoreCampoDB(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom, string docNumber)
        {
            string result = string.Empty;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            string commandText = "select valore_oggetto_db " +
                                    "from dpa_associazione_templates " +
                                    "where " +
                                    "id_oggetto = " + oggettoCustom.SYSTEM_ID.ToString() +
                                    " and doc_number = " + docNumber;

            System.Diagnostics.Debug.WriteLine("SQL - getDataRegistrazioneDB - VerifyAndSetCDC DBManagerCDC.cs - QUERY : " + commandText);
            logger.Debug("SQL - getDataRegistrazioneDB - VerifyAndSetCDC DBManagerCDC.cs - QUERY : " + commandText);

            DataSet ds = new DataSet();
            dbProvider.ExecuteQuery(ds, commandText);

            if (ds.Tables[0].Rows.Count != 0)
            {
                result = ds.Tables[0].Rows[0]["VALORE_OGGETTO_DB"].ToString();
            }

            return result;
        }

        public static DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli getDirittiCampo(string idRuolo, string idTemplate, string idOggetto)
        {
            DocsPaDB.Query_DocsPAWS.Model model = new DocsPaDB.Query_DocsPAWS.Model();
            return model.getDirittiCampoTipologiaDoc(idRuolo, idTemplate, idOggetto);
        }

        public static void setVisiblitaDocumento(string docNumber)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            ArrayList parameters = new ArrayList();
            parameters.Add(new DocsPaUtils.Data.ParameterSP("docNumber", docNumber));
            dbProvider.ExecuteStoredProcedure("SP_CDC_ADD_VISIBILITA", parameters, null);
        }
    }
}
