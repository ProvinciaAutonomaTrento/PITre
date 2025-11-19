// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Serilog;

namespace BusinessLogic.Fascicoli
{
    public class TitolarioManager
    {
        private static ILogger logger = Log.ForContext(typeof(TitolarioManager));


        /// <summary>
        /// </summary>
        /// <param name="idClassificazione"></param>
        /// <returns></returns>
        public static DocsPaVO.fascicolazione.Classifica[] getGerarchia(string idClassificazione, string idAmm)
        {
            logger.Debug("getGerarchia");
            if (idClassificazione != null && !idClassificazione.Trim().Equals(""))
                return getGerarchia(idClassificazione, null, idAmm);
            else
                return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="idClassificazione"></param>
        /// <param name="codiceClassificazione"></param>
        /// <returns></returns>
        public static DocsPaVO.fascicolazione.Classifica[] getGerarchia(string idClassificazione, string codiceClassificazione, string idAmm)
        {
            return getGerarchia(idClassificazione, codiceClassificazione, null, idAmm);
        }

        //aggiunta per gestione filtro registro
        /// <summary>
        /// </summary>
        /// <param name="idClassificazione"></param>
        /// <param name="codiceClassificazione"></param>
        /// <param name="registro"></param>
        /// <returns></returns>
        private static DocsPaVO.fascicolazione.Classifica[] getGerarchia(string idClassificazione, string codiceClassificazione, DocsPaVO.utente.Registro registro, string idAmm)
        {
            DocsPaVO.fascicolazione.Classifica[] lista = null;

            #region Codice Commentato
            /*DocsPa_V15_Utils.Database db = DocsPa_V15_Utils.dbControl.getDatabase();
			try {
				db.openConnection();
				int numLivello = 0;
				string idParent = "0";
				string queryString =
					"SELECT A.VAR_COD_LIV1, A.VAR_COD_LIV2, A.VAR_COD_LIV3, A.VAR_COD_LIV4, " +
					"A.VAR_COD_LIV5, A.VAR_COD_LIV6, A.VAR_COD_LIV7, A.VAR_COD_LIV8, " +
					"A.DESCRIPTION, A.ID_PARENT, A.NUM_LIVELLO, A.VAR_CODICE, A.SYSTEM_ID "+
					"FROM PROJECT A WHERE A.CHA_TIPO_PROJ='T' AND ";

				//add per filtro su registro
				if (registro != null)
				{
					string condRegistro = "";
					condRegistro = " (A.ID_REGISTRO IS NULL OR A.ID_REGISTRO='" + registro.systemId + "') ";
					queryString += condRegistro; 
					queryString += " AND ";
				}
				//end add 

				if(idClassificazione != null)
					queryString += "A.SYSTEM_ID=" + idClassificazione;
				else
					queryString += "A.VAR_CODICE='" + codiceClassificazione + "'";

				logger.Debug(queryString);
				IDataReader dr = db.executeReader(queryString);
				if (dr.Read()) {
					numLivello = Int32.Parse(dr.GetValue(10).ToString());
					lista = new DocsPaVO.fascicolazione.Classifica[numLivello];
					for (int i=0; i<numLivello; i++) {
						lista[i] = new DocsPaVO.fascicolazione.Classifica();
						//lista[i].codice = dr.GetValue(i).ToString();
					}
					numLivello -= 1;
					lista[numLivello].systemId = dr.GetValue(12).ToString();;
					lista[numLivello].descrizione = dr.GetValue(8).ToString();					
					lista[numLivello].codice = dr.GetValue(11).ToString();
					idParent = dr.GetValue(9).ToString();
				} 
				dr.Close();

				while (!idParent.Equals("0") && numLivello > 0) {
					numLivello -= 1;
					lista[numLivello].systemId = idParent;
					queryString = 
						"SELECT DESCRIPTION, ID_PARENT, NUM_LIVELLO, VAR_CODICE " + 
						"FROM PROJECT WHERE SYSTEM_ID=" + idParent;
					logger.Debug(queryString);
					dr = db.executeReader(queryString);
					if (dr.Read()) {
						lista[numLivello].descrizione = dr.GetValue(0).ToString();					
						lista[numLivello].codice = dr.GetValue(3).ToString();
					}
					idParent = dr.GetValue(1).ToString();
					dr.Close();
				}

				db.closeConnection();
			} catch (Exception e) {
				logger.Debug (e.Message);				
				db.closeConnection();
				throw new Exception("F_System");
			}*/
            #endregion

            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            lista = fascicoli.GetGerarchia(idClassificazione, codiceClassificazione, registro, idAmm);

            /*
			if(lista == null)
			{
				logger.Debug("Errore nella gestione dei fascicoli. (newGerarchia)");
				throw new Exception("F_System");								
			}
            */

            return lista;
        }

        public static string GetIdTitolarioAttivo(string idAmministrazione)
        {
            string retValue = string.Empty;

            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            retValue = fascicoli.GetIdTitolarioAttivo(idAmministrazione);

            return retValue;
        }
    }
}
