using System;
using System.Collections;
using System.Data;
using log4net;

namespace BusinessLogic.Rubrica
{
    public class RF
	{
        private static ILog logger = LogManager.GetLogger(typeof(RF));

		public RF(){}

        public static ArrayList getCorrispondentiByCodRF(string codiceRF) 
		{
			ArrayList corr = new ArrayList();
			try 
			{
				DocsPaDB.Query_DocsPAWS.RF rf = new DocsPaDB.Query_DocsPAWS.RF();
				corr = rf.getCorrispondentiByCodRF(codiceRF);
			}
			catch(Exception e) 
			{
				logger.Debug("Errore in Rubrica-RF  - metodo: getCorrispondentiByCodRF",e);
				return null;
			}
			return corr;			
		}

        public static ArrayList getCorrispondentiByDescRF(string descRF)
        {
            ArrayList corr = new ArrayList();
            try
            {
                DocsPaDB.Query_DocsPAWS.RF rf = new DocsPaDB.Query_DocsPAWS.RF();
                corr = rf.getCorrispondentiByDescRF(descRF);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in Rubrica-RF  - metodo: getCorrispondentiByDescRF", e);
                return null;
            }
            return corr;
        }

        public static string getNomeRF(string codiceRF)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.RF rf = new DocsPaDB.Query_DocsPAWS.RF();
                return rf.getNomeRF(codiceRF);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in Rubrica-RF  - metodo: getNomeRF", e);
                return null;
            }
        }

        public static string getSystemIdRF(string codiceRF)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.RF rf = new DocsPaDB.Query_DocsPAWS.RF();
                return rf.getSystemIdRF(codiceRF);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in Rubrica-RF  - metodo: getSystemIdRF", e);
                return null;
            }
        }
	}
}
