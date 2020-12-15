using System;
using System.IO;
using System.Linq;
using System.Data.OleDb;
using System.Collections.Generic;
using log4net;
using System.Collections;
using DocsPaVO.utente;
using DocsPaVO.Import.Pregressi;


namespace BusinessLogic.Import.ImportPregressi
{
    public class PregressiManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(PregressiManager));
        private static string separatoreMessaggi = "|";
        private static Dictionary<string, List<string>> listaCodiciUtenteVerificati = new Dictionary<string, List<string>>();

        //Andrea De Marco - Get Report Import Pregressi
        public static DocsPaVO.Import.Pregressi.ReportPregressi GetReportPregressi(string sysId, bool getItems)
        {
            DocsPaVO.Import.Pregressi.ReportPregressi result = null;

            DocsPaDB.Query_DocsPAWS.ImportPregressi importPreg = new DocsPaDB.Query_DocsPAWS.ImportPregressi();
            result = importPreg.GetReportPregressi(sysId, getItems);

            return result;
        }

        //Andrea De Marco - Get Reports di tutti i report
        public static DocsPaVO.Import.Pregressi.ReportPregressi[] GetReports(bool getItems, InfoUtente infoUtente, bool daAmm)
        {
            DocsPaVO.Import.Pregressi.ReportPregressi[] result = null;

            DocsPaDB.Query_DocsPAWS.ImportPregressi allImportPreg = new DocsPaDB.Query_DocsPAWS.ImportPregressi();
            result = allImportPreg.GetReports(getItems, infoUtente, daAmm);

            return result;
        }

        //Andrea De Marco - Insert del Report Import Pregressi
        public static string InsertReportPregressi(DocsPaVO.Import.Pregressi.ReportPregressi report)
        {
            string system_id= string.Empty;

            DocsPaDB.Query_DocsPAWS.ImportPregressi importPregressi = new DocsPaDB.Query_DocsPAWS.ImportPregressi();
            system_id = importPregressi.InsertReportPregressi(report);
            return system_id;
        }

        //Andrea De Marco - Insert del Item Report
        public static string InsertItemReportPregressi(DocsPaVO.Import.Pregressi.ItemReportPregressi item, string system_id)
        {
            //ID item
            string idItem = string.Empty;

            DocsPaDB.Query_DocsPAWS.ImportPregressi importPregressi = new DocsPaDB.Query_DocsPAWS.ImportPregressi();
            idItem = importPregressi.InsertItemReportPregressi(item, system_id);
            return idItem;
        }

        //Andrea De Marco - Delete Report
        public static bool DeleteReport(string idReport)
        {
            bool result = false;

            DocsPaDB.Query_DocsPAWS.ImportPregressi importPregressi = new DocsPaDB.Query_DocsPAWS.ImportPregressi();
            result = importPregressi.DeleteReport(idReport);
            return result;
        }

        //Andrea De Marco - Update Item Report
        public static bool UpdateItemReportPregressi(DocsPaVO.Import.Pregressi.ItemReportPregressi item, string sysId)
        {
            bool result = false;

            DocsPaDB.Query_DocsPAWS.ImportPregressi importPregressi = new DocsPaDB.Query_DocsPAWS.ImportPregressi();
            result = importPregressi.UpdateItemReportPregressi(item, sysId);
            return result;
        }

        //Andrea De Marco - Update Report
        public static bool UpdateReportPregressi(string sysId)
        {
            bool result = false;

            DocsPaDB.Query_DocsPAWS.ImportPregressi importPregressi = new DocsPaDB.Query_DocsPAWS.ImportPregressi();
            result = importPregressi.UpdateReportPregressi(sysId);
            return result;
        }

        private static bool numProtoUnivocoInFile(ItemReportPregressi protocollo, List<ItemReportPregressi> listaProtocolli)
        {
            bool esito = false;

            int numprotofound = (from ItemReportPregressi tempProto in listaProtocolli where tempProto.idNumProtocolloExcel == protocollo.idNumProtocolloExcel && tempProto.idRegistro == protocollo.idRegistro select protocollo.idNumProtocolloExcel).Count();
            if (numprotofound < 2)
            {
                esito = true;
            }

            return esito;
        }

        private static bool tipoProtoCorretto(ItemReportPregressi protocollo)
        {
            return (protocollo.tipoProtocollo.ToUpper().Equals("P") || protocollo.tipoProtocollo.ToUpper().Equals("A"));
        }

        private static bool tipoOperazioneCorretta(ItemReportPregressi protocollo)
        {
            return (protocollo.tipoOperazione.ToUpper().Equals("M") || protocollo.tipoOperazione.Equals("I") || protocollo.tipoOperazione.Equals("C"));
        }

        private static bool annoProtocolloValido(ItemReportPregressi protocollo, Registro registro)
        {
            DateTime dataProtocollo = DateTime.ParseExact(protocollo.data, "dd/mm/yyyy", null);

            return (dataProtocollo.Year.ToString().Equals(registro.anno_pregresso.ToString()));
        }

        private static bool registroValido(Registro registro)
        {
            return (registro != null ? registro.flag_pregresso : false);
        }

        public static List<string> GetExistingProtocolNumber(string idRegistro, string whereClausole, string regYear)
        {
            List<string> result = null;

            DocsPaDB.Query_DocsPAWS.ImportPregressi importPregressi = new DocsPaDB.Query_DocsPAWS.ImportPregressi();
            result = importPregressi.GetExistingProtocolNumber(idRegistro, whereClausole, regYear);
            return result;
        }

        public static List<string> GetInvalidProtocolNumber(List<string> functionParameter)
        {
            List<string> result = null;

            DocsPaDB.Query_DocsPAWS.ImportPregressi importPregressi = new DocsPaDB.Query_DocsPAWS.ImportPregressi();
            result = importPregressi.GetInvalidProtocolNumber(functionParameter);
            return result;
        }

        public static List<string> GetIdPeopleIdRuoloFromCodici(string cod_utente, string cod_ruolo, string id_amm)
        {
            List<string> result = null;

            DocsPaDB.Query_DocsPAWS.ImportPregressi importPregressi = new DocsPaDB.Query_DocsPAWS.ImportPregressi();
            result = importPregressi.GetIdPeopleIdRuoloFromCodici(cod_utente, cod_ruolo, id_amm);
            return result;
        }

        public static List<string> GetExistingNotProtocol(string whereClausole)
        {
            List<string> result = null;

            DocsPaDB.Query_DocsPAWS.ImportPregressi importPregressi = new DocsPaDB.Query_DocsPAWS.ImportPregressi();
            result = importPregressi.GetExistingProtocolNumber(whereClausole);
            return result;
        }


        public static List<string> GetInvalidOldNumber(List<string> functionParameter)
        {
            List<string> result = null;

            DocsPaDB.Query_DocsPAWS.ImportPregressi importPregressi = new DocsPaDB.Query_DocsPAWS.ImportPregressi();
            result = importPregressi.GetInvalidOldNumber(functionParameter);
            return result;
        }

      
    }
}
