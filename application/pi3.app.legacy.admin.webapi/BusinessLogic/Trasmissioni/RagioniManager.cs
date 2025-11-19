// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Serilog;

namespace BusinessLogic.Trasmissioni
{
    public class RagioniManager
	{
        private static ILogger logger = Log.ForContext(typeof(RagioniManager));

        public static DocsPaVO.trasmissione.RagioneTrasmissione getRagione(string idRagione)
		{
			logger.Debug("getRagione");
			DocsPaVO.trasmissione.RagioneTrasmissione objRagione = new DocsPaVO.trasmissione.RagioneTrasmissione();
			string queryString = getQueryRagione(idRagione);
			logger.Debug(queryString);

			DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
			strDB.GetRag(queryString, ref objRagione);

			return objRagione;
		}

		private static string getQueryRagione(string idRagione)
		{
			#region Codice Commentato
			/*string queryString =
				"SELECT SYSTEM_ID, VAR_DESC_RAGIONE, CHA_TIPO_RAGIONE, " +
				"CHA_TIPO_DIRITTI, CHA_RISPOSTA, CHA_TIPO_DEST, VAR_NOTE, CHA_EREDITA, CHA_TIPO_RISPOSTA FROM DPA_RAGIONE_TRASM WHERE ";
			if (idRagione != null)
				queryString += " SYSTEM_ID=" + idRagione;
			else
				queryString += " DPA_RAGIONE_TRASM.CHA_VIS = '1'";
			return queryString;*/
			#endregion

			DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
			return trasmissione.GetQueryRagione(idRagione);

		}

        /// <summary>
        /// Estrae la ragione di trasmissione in base al tipo di operazione
        /// </summary>
        /// <param name="tipoOperazione"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public static DocsPaVO.trasmissione.RagioneTrasmissione GetRagioneByTipoOperazione(string tipoOperazione, string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            return trasmissione.GetRagioneByTipoOperazione(tipoOperazione, idAmm);
        }

        /// <summary>
        /// Prende la ragione di trasmissione per il protocollo interno
        /// tramite la system_id dell'amministrazione ed il tipo destinatario
        /// </summary>
        /// <param name="tipoDest">tipo del destinatario (TO o CC)</param>
        /// <param name="idAmm">system_id dell'amministrazione</param>
        /// <returns>Object Ragione</returns>
        public static DocsPaVO.trasmissione.RagioneTrasmissione GetRagione(string tipoDest, string idAmm)
        {
            logger.Debug("GetRagione");
            DocsPaVO.trasmissione.RagioneTrasmissione objRagione = new DocsPaVO.trasmissione.RagioneTrasmissione();
            string queryString = getQueryRagione(tipoDest, idAmm);
            logger.Debug(queryString);

            DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            strDB.GetRag(queryString, ref objRagione);

            return objRagione;
        }

        private static string getQueryRagione(string tipoDest, string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            return trasmissione.GetQueryRagione(tipoDest, idAmm);
        }

        public static System.Collections.ArrayList getListaRagioni(DocsPaVO.trasmissione.Diritti objDiritti, bool flgDaRicercaTrasm, bool sysExt = false)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            return strDB.GetListRag(objDiritti, flgDaRicercaTrasm, sysExt);
        }


        public static System.Collections.ArrayList getListaRagioniATutti(DocsPaVO.trasmissione.Diritti objDiritti)
        {

            DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            return strDB.GetListRagATutti(objDiritti);

        }
    }
}
