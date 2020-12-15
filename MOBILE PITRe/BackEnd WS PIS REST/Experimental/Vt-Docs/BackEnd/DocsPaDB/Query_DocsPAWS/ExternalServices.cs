using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using log4net;
using DocsPaVO.ExternalServices;
using DocsPaUtils;

namespace DocsPaDB.Query_DocsPAWS
{

    public class ExternalServices
    {
        private ILog logger = LogManager.GetLogger(typeof(DocsPaDB.Query_DocsPAWS.Fatturazione));

        public List<Servizio> getServizi()
        {
            List<Servizio> retList = new List<Servizio>();

            try {
                 Query query = InitQuery.getInstance().getQuery("S_SERVIZI_ESTERNI");
                 string commandText = query.getSQL();
                 logger.Debug(commandText);

                 using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                 {
                     using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                     {
                         while (reader.Read())
                         {
                             Servizio tempServizio = new Servizio();
                             tempServizio.setId(reader.GetValue(reader.GetOrdinal("SYSTEM_ID")).ToString());
                             tempServizio.setDescrizione(reader.GetValue(reader.GetOrdinal("DESCRIZIONE")).ToString());

                             retList.Add(tempServizio);
                         }
                     }
                 }

                 return retList;

            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                throw new Exception(ex.Message);
            }
        }

        public List<Servizio> getServizi(string docNumber)
        {
            List<Servizio> retList = new List<Servizio>();

            try
            {
                Query query = InitQuery.getInstance().getQuery("S_SERVIZI_ESTERNI_STATO");
                query.setParam("docNumber", docNumber);
                string commandText = query.getSQL();
                

                logger.Debug(commandText);

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            Servizio tempServizio = new Servizio();
                            tempServizio.setId(reader.GetValue(reader.GetOrdinal("SYSTEM_ID")).ToString());
                            tempServizio.setDescrizione(reader.GetValue(reader.GetOrdinal("DESCRIZIONE")).ToString());
                            tempServizio.setCodiceEsecutore(reader.GetValue(reader.GetOrdinal("CODICE_ESECUTORE")).ToString());
                            caricaParametri(tempServizio);

                            retList.Add(tempServizio);
                        }
                    }
                }

                return retList;

            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                throw new Exception(ex.Message);
            }
        }

        public void caricaParametri(Servizio servizio)
        {
            List<Servizio> retList = new List<Servizio>();

            try
            {
                Query query = InitQuery.getInstance().getQuery("S_PARAMETRI_SERVIZIO_ESTERNO");
                query.setParam("idServizio", servizio.getId());

                string commandText = query.getSQL();
                logger.Debug(commandText);

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            string descrizioneParametro = reader.GetValue(reader.GetOrdinal("DESCRIZIONE")).ToString();
                            string tipoParametro = reader.GetValue(reader.GetOrdinal("TIPO_VALORE")).ToString();
                            
                            servizio.addParametro(descrizioneParametro, tipoParametro);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                throw new Exception(ex.Message);
            }
        }
    }
}
