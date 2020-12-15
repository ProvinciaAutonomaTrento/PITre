using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using log4net;
namespace DocsPaDB.Query_DocsPAWS.Formazione
{
    public class UnitaOrganizzativa : DBProvider
    {
        private static ILog logger = LogManager.GetLogger(typeof(Amministrazione));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idUo"></param>
        /// <returns></returns>
        public bool PulisciUnitaOrganizzativa(string idUo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            int retValue = 0;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    ArrayList sp_params = new ArrayList();
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("idUo", Int32.Parse(idUo)));
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("resultValue", new Int32(), 10, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32));

                    retValue = dbProvider.ExecuteStoredProcedure("SP_PULISCI_UO", sp_params, null);
                    if(retValue != 1)
                    {
                        result = false;
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Error("Errore in PulisciUnitaOrganizzativa: " + e.Message);
            }

            return result;
        }

    }
}
