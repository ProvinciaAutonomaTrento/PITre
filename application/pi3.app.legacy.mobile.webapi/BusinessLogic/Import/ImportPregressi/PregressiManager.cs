// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;

namespace BusinessLogic.Import.ImportPregressi
{
    public class PregressiManager
    {
        public static List<string> GetExistingNotProtocol(string whereClausole)
        {
            List<string> result = null;

            DocsPaDB.Query_DocsPAWS.ImportPregressi importPregressi = new DocsPaDB.Query_DocsPAWS.ImportPregressi();
            result = importPregressi.GetExistingProtocolNumber(whereClausole);
            return result;
        }

        public static List<string> GetExistingProtocolNumber(string idRegistro, string whereClausole, string regYear)
        {
            List<string> result = null;

            DocsPaDB.Query_DocsPAWS.ImportPregressi importPregressi = new DocsPaDB.Query_DocsPAWS.ImportPregressi();
            result = importPregressi.GetExistingProtocolNumber(idRegistro, whereClausole, regYear);
            return result;
        }

        public static List<string> GetIdPeopleIdRuoloFromCodici(string cod_utente, string cod_ruolo, string id_amm)
        {
            List<string> result = null;

            DocsPaDB.Query_DocsPAWS.ImportPregressi importPregressi = new DocsPaDB.Query_DocsPAWS.ImportPregressi();
            result = importPregressi.GetIdPeopleIdRuoloFromCodici(cod_utente, cod_ruolo, id_amm);
            return result;
        }

        public static List<string> GetInvalidOldNumber(List<string> functionParameter)
        {
            List<string> result = null;

            DocsPaDB.Query_DocsPAWS.ImportPregressi importPregressi = new DocsPaDB.Query_DocsPAWS.ImportPregressi();
            result = importPregressi.GetInvalidOldNumber(functionParameter);
            return result;
        }

        public static List<string> GetInvalidProtocolNumber(List<string> functionParameter)
        {
            List<string> result = null;

            DocsPaDB.Query_DocsPAWS.ImportPregressi importPregressi = new DocsPaDB.Query_DocsPAWS.ImportPregressi();
            result = importPregressi.GetInvalidProtocolNumber(functionParameter);
            return result;
        }

        public static DocsPaVO.Import.Pregressi.ReportPregressi[] GetReports(bool getItems, InfoUtente infoUtente, bool daAmm)
        {
            DocsPaVO.Import.Pregressi.ReportPregressi[] result = null;

            DocsPaDB.Query_DocsPAWS.ImportPregressi allImportPreg = new DocsPaDB.Query_DocsPAWS.ImportPregressi();
            result = allImportPreg.GetReports(getItems, infoUtente, daAmm);

            return result;
        }

        public static bool DeleteReport(string idReport)
        {
            bool result = false;

            DocsPaDB.Query_DocsPAWS.ImportPregressi importPregressi = new DocsPaDB.Query_DocsPAWS.ImportPregressi();
            result = importPregressi.DeleteReport(idReport);
            return result;
        }

    }
}
