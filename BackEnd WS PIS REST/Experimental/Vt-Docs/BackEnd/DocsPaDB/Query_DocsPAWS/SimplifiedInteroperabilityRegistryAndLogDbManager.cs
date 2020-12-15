using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Logger;
using System.Collections;
using DocsPaUtils.Data;
using DocsPaUtils;

namespace DocsPaDB.Query_DocsPAWS
{

    public class SimplifiedInteroperabilityRegistryAndLogDbManager : DBProvider
    {
        /// <summary>
        /// Metodo per l'inserimento di informazioni nel registro dei messaggi ricevuti per interoperabilità semplificata
        /// </summary>
        /// <param name="interoperabilityLogItem">Item con le informazioni da inserire</param>
        /// <returns>Esito dell'operazione di inserimento</returns>
        public bool InsertItemInRegistry(InteroperabilityLogItem interoperabilityLogItem)
        {
            using (DBProvider dbProvider = new DBProvider())
            {
                ArrayList parameters = new ArrayList();
                //parameters.Add(new ParameterSP("p_ProfileId", interoperabilityLogItem.ProfileId, 100, DirectionParameter.ParamInput, System.Data.DbType.Int32));
                parameters.Add(new ParameterSP("p_MessageId", interoperabilityLogItem.MessageIdentifier, 1000, DirectionParameter.ParamInput, System.Data.DbType.AnsiString));
                parameters.Add(new ParameterSP("p_ReceivedPrivate", interoperabilityLogItem.ReceivedPrivate ? 1 : 0, 100, DirectionParameter.ParamInput, System.Data.DbType.Int32));
                parameters.Add(new ParameterSP("p_Subject", interoperabilityLogItem.Subject, 4000, DirectionParameter.ParamInput, System.Data.DbType.AnsiString));
                parameters.Add(new ParameterSP("p_SenderDescription", interoperabilityLogItem.SenderDescription, 4000, DirectionParameter.ParamInput, System.Data.DbType.AnsiString));
                
                parameters.Add(new ParameterSP("p_SenderUrl", interoperabilityLogItem.SenderUrl, 2000, DirectionParameter.ParamInput, System.Data.DbType.AnsiString));
                parameters.Add(new ParameterSP("p_SenderAdministrationCode", interoperabilityLogItem.SenderRecordInfo.AdministrationCode, 2000, DirectionParameter.ParamInput, System.Data.DbType.AnsiString));
                parameters.Add(new ParameterSP("p_AOOCode", interoperabilityLogItem.SenderRecordInfo.AOOCode, 2000, DirectionParameter.ParamInput, System.Data.DbType.AnsiString));
                parameters.Add(new ParameterSP("p_RecordNumber", interoperabilityLogItem.SenderRecordInfo.RecordNumber, 4000, DirectionParameter.ParamInput, System.Data.DbType.Int32));
                parameters.Add(new ParameterSP("p_RecordDate", interoperabilityLogItem.SenderRecordInfo.RecordDate, 4000, DirectionParameter.ParamInput, System.Data.DbType.Date));
                parameters.Add(new ParameterSP("p_ReceiverCode", interoperabilityLogItem.ReceiverCode, 2000, DirectionParameter.ParamInput, System.Data.DbType.AnsiString));

                int result = dbProvider.ExecuteStoredProcedure("IS_InsertDataInReceivedMsg", parameters, null);

                return result == 1;
            }

        }

        /// <summary>
        /// Metodo per l'aggiornamento dell'id profile relativo ad un messaggio ricevuto per interoperabilità semplificata
        /// </summary>
        /// <param name="messageId">Identificativo (GUID) del messaggio da aggiornare</param>
        /// <param name="idProfile">Id del documento generato</param>
        /// <returns>Esito dell'operazione di aggiornamento</returns>
        public bool SetIdProfileForMessage(String messageId, String idProfile)
        {
            using (DBProvider dbProvider = new DBProvider())
            {
                ArrayList parameters = new ArrayList();
                parameters.Add(new ParameterSP("p_ProfileId", idProfile, 1000, DirectionParameter.ParamInput, System.Data.DbType.Int32));
                parameters.Add(new ParameterSP("p_MessageId", messageId, 1000, DirectionParameter.ParamInput, System.Data.DbType.AnsiString));

                int result = dbProvider.ExecuteStoredProcedure("IS_SetIdProfForSimpInteropMsg", parameters, null);

                return result == 1;
                
            }
        }

        /// <summary>
        /// Metodo per l'inserimento di informazioni nel log delle operazioni
        /// </summary>
        /// <param name="interoperabilityLogItem">Item con le informazioni da inserire</param>
        /// <returns>Esito dell'operazione di inserimento</returns>
        public bool InsertItemInLog(InteroperabilityLogItem interoperabilityLogItem)
        {
            using (DBProvider dbProvider = new DBProvider())
            {
                ArrayList parameters = new ArrayList();
                parameters.Add(new ParameterSP("p_ProfileId", interoperabilityLogItem.ProfileId, 100, DirectionParameter.ParamInput, System.Data.DbType.Int32));
                parameters.Add(new ParameterSP("p_ErrorMessage", interoperabilityLogItem.IsErrorMessage ? 1 : 0, 1, DirectionParameter.ParamInput, System.Data.DbType.Int32));
                parameters.Add(new ParameterSP("p_Text", interoperabilityLogItem.LogMessage, 4000, DirectionParameter.ParamInput, System.Data.DbType.AnsiString));

                int result = dbProvider.ExecuteStoredProcedure("IS_InsertDataInSimpInteropLog", parameters, null);

                return result == 1;
            }

        }
    }
}
