using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Interoperabilita.Semplificata;
using System.Collections;
using DocsPaUtils.Data;
using DocsPaVO.utente;
using DocsPaUtils;
using System.Data;
using DocsPaVO.Logger;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{
    /// <summary>
    /// Classe per la gestione dell'interazione con il database relativa all'interoperabilità
    /// semplificata
    /// </summary>
    public class InteroperabilitaSemplificata : DBProvider
    {
        private static ILog logger = LogManager.GetLogger(typeof(InteroperabilitaSemplificata));

        /// <summary>
        /// Stringa identificativa del canale di comunicazione per l'interoperabilità semplificata
        /// </summary>
        public const String SimplifiedInteroperabilityId = "SIMPLIFIEDINTEROPERABILITY";

        /// <summary>
        /// Metodo per il salvataggio delle impostazioni sull'interoperabilità
        /// semplificata, relativamente ad uno specifico registro
        /// </summary>
        /// <param name="interoperabilitySettings">Impostazioni da salvare</param>
        /// <returns>Esito dell'operazione di salvataggio</returns>
        public bool SaveSettings(InteroperabilitySettings interoperabilitySettings)
        {
            using (DBProvider dbProvider = new DBProvider())
            {
                ArrayList parameters = new ArrayList();
                parameters.Add(new ParameterSP("p_RegistryId", interoperabilitySettings.RegistryId, 100, DirectionParameter.ParamInput, System.Data.DbType.Int32));
                parameters.Add(new ParameterSP("p_RoleId", interoperabilitySettings.RoleId, 100, DirectionParameter.ParamInput, System.Data.DbType.Int32));
                parameters.Add(new ParameterSP("p_UserId", interoperabilitySettings.UserId, 100, DirectionParameter.ParamInput, System.Data.DbType.Int32));
                parameters.Add(new ParameterSP("p_IsEnabledInteroperability", interoperabilitySettings.IsEnabledInteroperability ? 1 : 0, 100, DirectionParameter.ParamInput, System.Data.DbType.Int32));
                parameters.Add(new ParameterSP("p_ManagementMode", interoperabilitySettings.ManagementMode.ToString(), 100, DirectionParameter.ParamInput, System.Data.DbType.AnsiString));
                parameters.Add(new ParameterSP("p_KeepPrivate", interoperabilitySettings.KeepPrivate ? 1 : 0, 100, DirectionParameter.ParamInput, System.Data.DbType.Int32));

                int result = dbProvider.ExecuteStoredProcedure("IS_SaveSettings", parameters, null);

                return result == 1;
            }

        }

        /// <summary>
        /// Metodo per il caricamento delle impostazioni relative ad registro
        /// </summary>
        /// <param name="registryId">Id del registro</param>
        /// <returns>Impostazioni</returns>
        public InteroperabilitySettings LoadSettings(String registryId)
        {
            InteroperabilitySettings retVal = new InteroperabilitySettings();

            int roleId = 0, userId = 0, isEnabledInteroperability = 0, keepPrivate = 0;
            String managementMode = String.Empty;

            using (DBProvider dbProvider = new DBProvider())
            {
                ArrayList parameters = new ArrayList();
                parameters.Add(new ParameterSP("RegistryId", registryId, 100, DirectionParameter.ParamInput, System.Data.DbType.Int32));
                parameters.Add(new ParameterSP("RoleId", roleId, 100, DirectionParameter.ParamOutput, System.Data.DbType.Int32));
                parameters.Add(new ParameterSP("UserId", userId, 100, DirectionParameter.ParamOutput, System.Data.DbType.Int32));
                parameters.Add(new ParameterSP("IsEnabledInteroperability", isEnabledInteroperability, 100, DirectionParameter.ParamOutput, System.Data.DbType.Int32));
                parameters.Add(new ParameterSP("KeepPrivate", keepPrivate, 100, DirectionParameter.ParamOutput, System.Data.DbType.Int32));
                parameters.Add(new ParameterSP("ManagementMode", managementMode, 100, DirectionParameter.ParamOutput, System.Data.DbType.AnsiString));

                if (dbProvider.ExecuteStoredProcedure("IS_LoadSettings", parameters, null) == 1)
                {
                    // Lettura dei valori aggiornati
                    ParameterSP[] convertedParams = (ParameterSP[])parameters.ToArray(typeof(ParameterSP));
                    roleId = Int32.Parse(convertedParams.Where(p => p.Nome == "RoleId").First().Valore.ToString());
                    userId = Int32.Parse(convertedParams.Where(p => p.Nome == "UserId").First().Valore.ToString());
                    isEnabledInteroperability = Int32.Parse(convertedParams.Where(p => p.Nome == "IsEnabledInteroperability").First().Valore.ToString());
                    keepPrivate = Int32.Parse(convertedParams.Where(p => p.Nome == "KeepPrivate").First().Valore.ToString());
                    managementMode = convertedParams.Where(p => p.Nome == "ManagementMode").First().Valore.ToString();
                }

            }

            return new InteroperabilitySettings()
                {
                    IsEnabledInteroperability = isEnabledInteroperability == 1,
                    RegistryId = registryId,
                    RoleId = roleId,
                    UserId = userId,
                    KeepPrivate = keepPrivate == 1,
                    ManagementMode = String.IsNullOrEmpty(managementMode) ?
                        ManagementType.M : (ManagementType)Enum.Parse(typeof(ManagementType), managementMode)
                };

        }

        /// <summary>
        /// Metodo per la verifica dello stato di attivazione di un RF all'interoperabilità
        /// semplificata
        /// </summary>
        /// <param name="objectId">Id dell'oggetto di cui verificare se è interoperante</param>
        /// <param name="rf">True se si stanno richiedendo informazioni sull'interoperabilità di un RF</param>
        /// <returns>Esito della verifica</returns>
        public bool IsElementInteroperable(String objectId, bool rf)
        {
            bool retVal = false;

            int isInteroperable = 0;

            using (DBProvider dbProvider = new DBProvider())
            {
                ArrayList parameters = new ArrayList();
                parameters.Add(new ParameterSP("ObjectId", objectId, 100, DirectionParameter.ParamInput, System.Data.DbType.Int32));
                parameters.Add(new ParameterSP("IsRf", rf ? 1 : 0, 100, DirectionParameter.ParamInput, System.Data.DbType.Int32));
                parameters.Add(new ParameterSP("IsInteroperable", isInteroperable, 100, DirectionParameter.ParamOutput, System.Data.DbType.Int32));

                if (dbProvider.ExecuteStoredProcedure("IS_IsElementInteroperable", parameters, null) == 1)
                {
                    // Lettura del valore restituito dalla store
                    ParameterSP[] convertedParams = (ParameterSP[])parameters.ToArray(typeof(ParameterSP));
                    retVal = Int32.Parse(convertedParams.Where(p => p.Nome == "IsInteroperable").First().Valore.ToString()) == 1;
                }

            }

            return retVal;

        }

        /// <summary>
        /// Metodo per il caricamento delle informazioni sui corrispondenti cui deve essere
        /// effettuata la trasmissione del documento creato per interoperabilità semplificata
        /// </summary>
        /// <param name="registryId">Id del registro (o RF)</param>
        /// <param name="privateManagement">Flag indicante se devono essere recuperati i ruoli per la gestione dei privati</param>
        /// <returns>Insieme dei corrispondenti cui effettuare la trasmissione</returns>
        public List<Corrispondente> LoadTransmissionReceiverData(String registryId, bool privateManagement)
        {
            // Lista dei corrispondenti da restituire
            List<Corrispondente> receivers = new List<Corrispondente>();

            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("S_ROLE_WITH_FUNCTION");
                query.setParam("registryId", registryId);
                query.setParam("functionName", privateManagement ? InteropFunction.PRAUISP.ToString() : InteropFunction.PRAUISNP.ToString());
                string commandText = query.getSQL();
                logger.Debug("RAFFREDDORE - " + commandText);
                using (IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL()))
                    while (dataReader.Read())
                        receivers.Add(new Utenti().GetCorrispondente(dataReader[0].ToString(), true));
            }

            // Restituzione dell'insieme dei corrispondenti destinatari della trasmissione
            return receivers;
 
        }

        /// <summary>
        /// Questo metodo verifica se un documento ricevuto per interoperabilità semplificata
        /// è stato ricevuto marcato come privato
        /// </summary>
        /// <param name="documentId">Id del documento da controllare</param>
        /// <returns>Flag indicante se il documento è stato ricevuto marcato come privato</returns>
        public bool IsDocumentReceivedPrivate(String documentId)
        {
            int receivedPrivate = 0;
            
            using (DBProvider dbProvider = new DBProvider())
            {
                ArrayList parameters = new ArrayList();
                parameters.Add(new ParameterSP("p_ProfileId", documentId, 100, DirectionParameter.ParamInput, System.Data.DbType.Int32));
                parameters.Add(new ParameterSP("p_ReceivedPrivate", receivedPrivate, 100, DirectionParameter.ParamOutput, System.Data.DbType.Int32));

                if (dbProvider.ExecuteStoredProcedure("IS_LoadReceivedPrivateFlag", parameters, null) == 1)
                {
                    // Lettura dei valori aggiornati
                    ParameterSP[] convertedParams = (ParameterSP[])parameters.ToArray(typeof(ParameterSP));
                    receivedPrivate = Int32.Parse(convertedParams.Where(p => p.Nome == "p_ReceivedPrivate").First().Valore.ToString());
                    
                }

            }

            return receivedPrivate == 1;
        }

        /// <summary>
        /// Metodo per il caricamento delle informazioni sul documento mittente
        /// </summary>
        /// <param name="documentId">Id del documento</param>
        /// <param name="senderRecordInfo">Verrà valorizzato con le informazioni sul protocollo mittente</param>
        /// <param name="receiverRecordInfo">Verrà valorizzato con le informazioni sul protocollo destinatario</param>
        /// <param name="senderUrl">Url del mittente</param>
        /// <param name="receiverCode">Codice del destinatario</param>
        /// <returns>Esito del caricamento</returns>
        public bool LoadSenderDocInfo(String documentId, out RecordInfo senderRecordInfo, out RecordInfo receiverRecordInfo, out String senderUrl, out String receiverCode)
        {
            bool succeded = false;
            senderRecordInfo = null;
            receiverRecordInfo = null;
            senderUrl = String.Empty;
            receiverCode = String.Empty;
            using (DBProvider dbProvider = new DBProvider())
            {
                //15-11-2016: Problema SP oracle12c (tronca gli output parameter). Uso le query
                /*
                ArrayList parameters = new ArrayList();
                parameters.Add(new ParameterSP("p_DocumentId", documentId, 100, DirectionParameter.ParamInput, System.Data.DbType.UInt32));
                parameters.Add(new ParameterSP("p_SenderAdministrationCode", String.Empty, 100, DirectionParameter.ParamOutput, System.Data.DbType.AnsiString));
                parameters.Add(new ParameterSP("p_SenderAOOCode", String.Empty, 100, DirectionParameter.ParamOutput, System.Data.DbType.AnsiString));
                parameters.Add(new ParameterSP("p_SenderRecordNumber", 0, 100, DirectionParameter.ParamOutput, System.Data.DbType.UInt32));
                parameters.Add(new ParameterSP("p_SenderRecordDate", DateTime.MinValue, 100, DirectionParameter.ParamOutput, System.Data.DbType.DateTime));
                parameters.Add(new ParameterSP("p_ReceiverAdministrationCode", String.Empty, 100, DirectionParameter.ParamOutput, System.Data.DbType.AnsiString));
                parameters.Add(new ParameterSP("p_ReceiverAOOCode", String.Empty, 100, DirectionParameter.ParamOutput, System.Data.DbType.AnsiString));
                parameters.Add(new ParameterSP("p_ReceiverRecordNumber", 0, 100, DirectionParameter.ParamOutput, System.Data.DbType.UInt32));
                parameters.Add(new ParameterSP("p_ReceiverRecordDate", DateTime.MinValue, 100, DirectionParameter.ParamOutput, System.Data.DbType.DateTime));
                parameters.Add(new ParameterSP("p_SenderUrl", String.Empty, 2000, DirectionParameter.ParamOutput, System.Data.DbType.AnsiString));
                parameters.Add(new ParameterSP("p_ReceiverCode", String.Empty, 2000, DirectionParameter.ParamOutput, System.Data.DbType.AnsiString));

                succeded = dbProvider.ExecuteStoredProcedure("IS_LoadSimpInteropRecordInfo", parameters, null) == 1;
                if (succeded)
                {
                    // Lettura dei valori aggiornati
                    ParameterSP[] convertedParams = (ParameterSP[])parameters.ToArray(typeof(ParameterSP));
                    senderRecordInfo = new RecordInfo()
                        {
                            AdministrationCode = convertedParams.Where(p => p.Nome == "p_SenderAdministrationCode").First().Valore.ToString(),
                            AOOCode = convertedParams.Where(p => p.Nome == "p_SenderAOOCode").First().Valore.ToString(),
                            RecordDate = DateTime.Parse(convertedParams.Where(p => p.Nome == "p_SenderRecordDate").First().Valore.ToString()),
                            RecordNumber = convertedParams.Where(p => p.Nome == "p_SenderRecordNumber").First().Valore.ToString()
                        };

                    DateTime receiverDate = DateTime.MinValue;
                    DateTime.TryParse(convertedParams.Where(p => p.Nome == "p_ReceiverRecordDate").First().Valore.ToString(), out receiverDate);
                    receiverRecordInfo = new RecordInfo()
                    {
                        AdministrationCode = convertedParams.Where(p => p.Nome == "p_ReceiverAdministrationCode").First().Valore.ToString(),
                        AOOCode = convertedParams.Where(p => p.Nome == "p_ReceiverAOOCode").First().Valore.ToString(),
                        RecordDate = receiverDate,
                        RecordNumber = convertedParams.Where(p => p.Nome == "p_ReceiverRecordNumber").First().Valore.ToString()
                    };

                    senderUrl = convertedParams.Where(p => p.Nome == "p_SenderUrl").First().Valore.ToString();
                    receiverCode = convertedParams.Where(p => p.Nome == "p_ReceiverCode").First().Valore.ToString();

                }
                else
                {
                    senderRecordInfo = null;
                    receiverRecordInfo = null;
                    senderUrl = String.Empty;
                    receiverCode = String.Empty;
                }
                */
                //Caricamento delle informazioni sul protocollo mittente
                Query query = InitQuery.getInstance().getQuery("S_SIMPINTEROPRECEIVEDMESSAGE_BY_ID_PROFILE");
                query.setParam("idProfile", documentId);
                string commandText = query.getSQL();

                DataSet ds = new DataSet();
                if (dbProvider.ExecuteQuery(out ds, "SIMPINTEROPRECEIVEDMESSAGE", commandText))
                {
                    if (ds.Tables["SIMPINTEROPRECEIVEDMESSAGE"] != null && ds.Tables["SIMPINTEROPRECEIVEDMESSAGE"].Rows != null && ds.Tables["SIMPINTEROPRECEIVEDMESSAGE"].Rows.Count > 0)
                    {
                        DataRow dataRow = ds.Tables["SIMPINTEROPRECEIVEDMESSAGE"].Rows[0];
                        senderRecordInfo = new RecordInfo()
                            {
                                AdministrationCode = dataRow["SENDER_ADMINISTRATION_CODE"].ToString(),
                                AOOCode = dataRow["SENDER_AOO_CODE"].ToString(),
                                RecordDate = DateTime.Parse(dataRow["SENDER_RECORD_DATE"].ToString()),
                                RecordNumber = dataRow["SENDER_RECORD_NUMBER"].ToString()
                            };

                        senderUrl = dataRow["SENDER_URL"].ToString();
                        receiverCode = dataRow["RECEIVER_CODE"].ToString();

                        //Caricamento informazioni sul protocollo creato nell'amministrazione destinataria
                        query = InitQuery.getInstance().getQuery("S_PROFILE_RECEIVER_INFO");
                        query.setParam("idProfile", documentId);
                        commandText = query.getSQL();

                        if (dbProvider.ExecuteQuery(out ds, "PROFILE", commandText))
                        {
                            succeded = true;
                            if (ds.Tables["PROFILE"] != null && ds.Tables["PROFILE"].Rows != null && ds.Tables["PROFILE"].Rows.Count > 0)
                            {
                                DataRow row = ds.Tables["PROFILE"].Rows[0];
                                DateTime receiverDate = DateTime.MinValue;
                                DateTime.TryParse(row["RECEIVER_RECORD_DATE"].ToString(), out receiverDate);
                                receiverRecordInfo = new RecordInfo()
                                    {
                                        AdministrationCode = row["RECEIVER_ADMINISTRATOR_CODE"].ToString(),
                                        AOOCode = row["RECEIVER_AOO_CODE"].ToString(),
                                        RecordDate = receiverDate,
                                        RecordNumber = row["RECEIVER_RECORD_NUMBER"].ToString()
                                    };
                            }
                        }
                    }
                }
            }

            return succeded;

        }

        /// <summary>
        /// Metodo per il salvataggio delle informazioni su una ricevuta di conferma di ricezione
        /// </summary>
        /// <param name="senderRecordInfo">Informazioni sul protocollo in uscita</param>
        /// <param name="receiverRecordInfo">Informazioni sul protocollo ricevuto</param>
        /// <param name="receiverUrl">Url del mittente</param>
        /// <returns>Esto dell'aggiornamento</returns>
        public bool SaveReceivedProofData(RecordInfo senderRecordInfo, RecordInfo receiverRecordInfo, String receiverUrl, String receiverCode)
        {
            // Splitting dei codici dei corrispondenti (i codici arrivano come codici di corrispondenti nella loro
            // ultima versione, ma è possibile che da quando è stato creato il documento, il corrispondente abbia subito
            // delle storicizzazioni
            String[] corrs = receiverCode.Split(',');


            Query query = InitQuery.getInstance().getQuery("U_SET_RECEIVED_PROOF_DATA");

            // Recupero dell'id dell'amministrazione mittente
            String administrationId = new Amministrazione().GetIDAmm(senderRecordInfo.AdministrationCode);

            // Costruzione informazioni sul protocollo ricevuto
            String protoRec = String.Format("{0}{1}{2}{1}{3}",
                    receiverRecordInfo.RecordNumber,
                    DocsPaDB.Utils.Personalization.getInstance(administrationId).getSepSegnatura(),
                    receiverRecordInfo.AOOCode,
                    receiverRecordInfo.RecordDate.Year);

            query.setParam("receiverProtoRec", protoRec);
            query.setParam("receiverProtoDate", receiverRecordInfo.RecordDate.ToString("dd/MM/yyyy HH:mm:ss"));
            query.setParam("senderRecordNumber", senderRecordInfo.RecordNumber);
            query.setParam("senderProtoDate", senderRecordInfo.RecordDate.ToString("dd/MM/yyyy HH:mm:ss"));
            query.setParam("receiverAooCode", receiverRecordInfo.AOOCode);
            query.setParam("receiverAdmCode", receiverRecordInfo.AdministrationCode);
            query.setParam("senderAdmCode", senderRecordInfo.AdministrationCode);
            query.setParam("senderAooCode", senderRecordInfo.AOOCode);
            query.setParam("receiverUrl", receiverUrl);

            if (dbType.ToUpper().Equals("SQL"))
            {
                List<string> idCorr = new List<string>();
                string condition = string.Empty;
                DataSet ds;
                //selezione degli id associati alla storia dei corrispondenti, destinatari di una spedizione
                if (this.ExecuteQuery(out ds, "corr", this.GetCorrUnion(corrs).ToString()))
                {
                    if (ds.Tables["corr"] != null && ds.Tables["corr"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["corr"].Rows)
                        {
                            idCorr.Add(row["SYSTEM_ID"].ToString());
                        }
                        string listSystemId = string.Empty;
                        foreach (string id in idCorr)
                        {
                            listSystemId += string.IsNullOrEmpty(listSystemId) ? " '" + id + "'" : " ,'" + id + "'";
                        }
                        condition = listSystemId;
                        query.setParam("receiverCode", condition);
                    }
                }
            }
            else
            {
                query.setParam("receiverCode", this.GetCorrUnion(corrs));
            }

            logger.Debug(query.getSQL());

            using (DBProvider dbProvider = new DBProvider())
            {
                return dbProvider.ExecuteNonQuery(query.getSQL());
            }

        }

        /// <summary>
        /// Metodo per il caricamento dei dati per il salvataggio della ricevuta di avvenuta consegna
        /// </summary>
        /// <param name="documentId">Id del documento</param>
        /// <param name="authorId">Verrà popolato con l'id dell'autore dell'allegato</param>
        /// <param name="creatorRole">Verrà popolato con l'id del ruolo creatore</param>
        /// <returns>Esito del caricamento</returns>
        public bool LoadDataForDeliveredProof(String documentId, out String authorId, out String creatorRole)
        {
            
            authorId = String.Empty;
            creatorRole = String.Empty;

            Query query = InitQuery.getInstance().getQuery("S_LOAD_INFO_FOR_DELIVERED_PROOF");

            query.setParam("documentId", documentId);

            bool retVal = false;
            using (DBProvider dbProvider = new DBProvider())
            {
                using (IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL()))
                    while (dataReader.Read())
                    {
                        authorId = dataReader["author"].ToString();
                        creatorRole = dataReader["id_ruolo_creatore"].ToString();
                    }
                 
            }

            return retVal;
        }

        /// <summary>
        /// Metodo utilizzato per verificare se un documento è stato ricevuto per interoperabilità semplificata.
        /// Questo metodo viene utilizzato per verificare, durante l'annullamento, se bisogna seguire la strada
        /// dell'invio classica o se seguire la strada dell'IS
        /// </summary>
        /// <param name="documentId">Id del documento da verificare</param>
        /// <returns>Esito della verifica</returns>
        public bool IsDocumentReceivedWithIS(String documentId)
        {
            String query = String.Format("Select * From SimpInteropReceivedMessage Where ProfileId = {0}", documentId);
            bool retVal = false;
            using (DBProvider dbProvider = new DBProvider())
            using (IDataReader dataReader = dbProvider.ExecuteReader(query))
                while (dataReader.Read())
                    retVal = true;

            return retVal;
        }

        /// <summary>
        /// Metodo per il salvataggio delle informazioni sulla ricevuta di cancellazione o eccezione
        /// </summary>
        /// <param name="senderRecordInfo">Informazioni sul protocollo mittente</param>
        /// <param name="receiverRecordInfo">Informazioni sul protocollo destinatario</param>
        /// <param name="reason">Ragione dell'annullamento o dettaglio dell'eccezione</param>
        /// <param name="receiverUrl">Url del destinatario</param>
        /// <param name="droppedProof">Booleano utilizzato per indicare se si tratta di una ricevuta di annullamento</param>
        /// <param name="receiverCode">Codice del destinatario</param>
        /// <returns>Esito del salvataggio</returns>
        public bool SaveDocumentDroppedOrExceptionProofData(RecordInfo senderRecordInfo, RecordInfo receiverRecordInfo, String reason, string receiverUrl, bool droppedProof, String receiverCode)
        {
            // Splitting dei codici dei corrispondenti (i codici arrivano come codici di corrispondenti nella loro
            // ultima versione, ma è possibile che da quando è stato creato il documento, il corrispondente abbia subito
            // delle storicizzazioni
            String[] corrs = receiverCode.Split(',');

            Query query = InitQuery.getInstance().getQuery("U_SET_DROPPED_PROOF_DATA");

            query.setParam("value", droppedProof ? "1" : "E");
            // PEC 4 Modifica Maschera Caratteri
            query.setParam("statusmask", droppedProof ? "VNVVVNN" : "XNVNNXN");
            query.setParam("reason", reason);
            query.setParam("senderRecordNumber", senderRecordInfo.RecordNumber);
            query.setParam("senderProtoDate", senderRecordInfo.RecordDate.ToString("dd/MM/yyyy HH:mm:ss"));
            query.setParam("receiverAooCode", receiverRecordInfo.AOOCode);
            query.setParam("receiverAdmCode", receiverRecordInfo.AdministrationCode);
            query.setParam("senderAdmCode", senderRecordInfo.AdministrationCode);
            query.setParam("senderAooCode", senderRecordInfo.AOOCode);
            query.setParam("receiverUrl", receiverUrl);
            if (dbType.ToUpper().Equals("SQL"))
            {
                List<string> idCorr = new List<string>();
                string condition = string.Empty;
                DataSet ds;
                //selezione degli id associati alla storia dei corrispondenti, destinatari di una spedizione
                if (this.ExecuteQuery(out ds, "corr", this.GetCorrUnion(corrs).ToString()))
                {
                    if (ds.Tables["corr"] != null && ds.Tables["corr"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["corr"].Rows)
                        {
                            idCorr.Add(row["SYSTEM_ID"].ToString());
                        }
                        string listSystemId = string.Empty;
                        foreach (string id in idCorr)
                        {
                            listSystemId += string.IsNullOrEmpty(listSystemId) ? " '" + id + "'" : " ,'" + id + "'";
                        }
                        condition = listSystemId;
                        query.setParam("receiverCode", condition);
                    }
                }
            }
            else
            {
                query.setParam("receiverCode", this.GetCorrUnion(corrs));
            }

            using (DBProvider dbProvider = new DBProvider())
            {
                return dbProvider.ExecuteNonQuery(query.getSQL());
            }

        }

        /// <summary>
        /// Metodo per il caricamento delle informazioni sul mittente a partire dall'id della uo
        /// </summary>
        /// <param name="uoId">Id della UO</param>
        /// <returns>Codice del destinatario</returns>
        public String LoadSenderInfoFromUoId(String uoId)
        {
            String retVal = String.Empty;
            Query query = InitQuery.getInstance().getQuery("S_GET_SENDER_INFO_FROM_UO_ID");
            query.setParam("uoId", uoId);

            using (DBProvider dbProvider = new DBProvider())
            {
                using (IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL()))
                    while(dataReader.Read())
                        retVal = dataReader[0].ToString();
            }

            return retVal;
        }

        /// <summary>
        /// Metodo utilizzato per verificare se un corrispondente (mittente di un protocollo)
        /// è abilitato all'interoperabilità. Per essere abilitato deve avere valorizzati 
        /// l'id del registro
        /// </summary>
        /// <param name="corrId">Id del corrispondente</param>
        /// <returns>Esito della verifica</returns>
        public bool IsCorrEnabledToInterop(String corrId)
        {
            bool retVal = false;
            String query = String.Format("select InteropRegistryId from dpa_corr_globali where system_id = {0}", corrId);
            using (DBProvider dbProvider = new DBProvider())
            {
                using (IDataReader dataReader = dbProvider.ExecuteReader(query))
                {
                    while (dataReader.Read())
                    {
                        retVal = dataReader[0] != DBNull.Value;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Metodo per verificare se un RF compare fra i destinatari di una spedizione per un dato documento. L'individuazione
        /// si basa sul fatto che nel registro delle richieste di interoperabilità, è presente un campo (ReceiverCode) in cui
        /// sono riportati tutti i destinatari con il codice nel formato RC, racchiusi fra singoli apici e separati da virgola
        /// </summary>
        /// <param name="profileId">Id del documento</param>
        /// <param name="corrCode">Codice del corrispondente</param>
        /// <returns>Esito della verifica</returns>
        public bool IsCorrInReceivers(String profileId, String corrCode)
        {
            // Costruzione della query per la verifica
            String query = String.Format("Select Count(*) From SimpInteropReceivedMessage Where ProfileId = {0} And ReceiverCode Like '%''{1}''%'",
                profileId,
                corrCode);

            String val = String.Empty;
            using (DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteScalar(out val, query);

            }

            return !String.IsNullOrEmpty(val) && val != "0";
        }

        /// <summary>
        /// Metodo per la creazione di una serie di union per la selezione degli id associati alla storia dei
        /// corrispondenti, destinatari di una spedizione
        /// </summary>
        /// <param name="corrs">Lista dei codici dei corrispondenti nella loro ultima versione</param>
        /// <returns>Union delle condizioni necessarie per l'estrazione dei dati sui corrispondenti</returns>
        private String GetCorrUnion(string[] corrs)
        {
            bool first = true;
            StringBuilder query = new StringBuilder();
            foreach (var corr in corrs)
            {
                Query q = InitQuery.getInstance().getQuery("S_CORR_IDS_BY_ID");
                q.setParam("corrCode", corr);
                query.AppendFormat("{0} {1}",
                    first ? String.Empty : " union ",
                    q.getSQL());
                first = false;
            }

            return query.ToString();
        }

        /// <summary>
        /// Enumerazione delle microfunzioni relative all'interoperabilità semplificata
        /// </summary>
        private enum InteropFunction
        {
            /// <summary>
            /// Microfunzione utilizzata per abilitare un ruolo alla ricezione dei documento marcati come non
            /// privati dall'amministrazione mittente
            /// </summary>
            PRAUISNP,
            /// <summary>
            /// Microfunzione utilizzata per abilitare un ruolo alla ricezione dei documenti marcati come 
            /// privati dall'amministrazione mittente
            /// </summary>
            PRAUISP,
            /// <summary>
            /// Microfunzione utilizzata per consentire ad un ruolo rifiutare una trasmissione ricevuta per IS
            /// </summary>
            DELPREDIS
        }

        /// <summary>
        /// PEC 4 Modifica Maschera Caratteri
        /// Metodo di aggiornamento della status mask per i consegne e mancate consegne IS.
        /// 
        /// </summary>
        /// <returns></returns>
        public bool AggiornaStatusMask(string statusmask, string codAOO, string codAmm, string idDocument)
        {
            bool retval = false;
            logger.Debug("Ricevuta interoperabilità semplificata. Aggiorno la status mask");
            try
            {

                string condition = String.Format(" UPPER(VAR_CODICE_AMM) = UPPER('{0}') AND UPPER(VAR_CODICE_AOO)= UPPER('{1}') AND ID_PROFILE={2}", codAmm, codAOO, idDocument);
                string values = string.Format("STATUS_C_MASK = '{0}' ", statusmask);

                // Modifica per il punto esclamativo nel caso di mancata consegna IS.
                if (statusmask == "XNXNNNN")
                {
                    values += ", CHA_ANNULLATO='E', VAR_MOTIVO_ANNULLA='Errore di consegna IS' ";
                }

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPAStatoinvio");
                q.setParam("param2", condition);
                q.setParam("param1", values);

                string updateString = q.getSQL();
                logger.Warn(updateString);
                int RowTornate = 0;
                using (DBProvider dbProvider = new DBProvider())
                {
                    retval = this.ExecuteNonQuery(updateString, out RowTornate);
                    if (RowTornate > 0)
                    {

                        logger.Warn("Aggiornamento status mask. Righe modificate: " + RowTornate);
                    }
                    else
                    {
                        logger.ErrorFormat("Aggiornamento status mask. Nessuna riga modificata per il destinatario con codice AMM {0} e codice AOO {1} per il documento {2}. FIX.", codAmm, codAOO, idDocument);
                        //if (statusmask == "ANVAAAN")
                        //{
                        //    DocsPaUtils.Query qFIX = DocsPaUtils.InitQuery.getInstance().getQuery("FIX_PEC4_SPED_IS_AVV_CONSEGNA");
                        //    qFIX.setParam("idDoc", idDocument);
                        //    string FIXstring = qFIX.getSQL();
                        //    logger.Warn(FIXstring);
                        //    int fixRowTorn = 0;
                        //    retval = this.ExecuteNonQuery(FIXstring, out fixRowTorn);
                        //    if (fixRowTorn > 0)
                        //    {
                        //        logger.Warn("Fix eseguito, righe aggiornate:" + fixRowTorn);
                        //    }
                        //    else
                        //    {
                        //        logger.Error("Errore nell'esecuzione del FIX");
                        //    }
                        //}
                        //else
                        //{
                        //    DocsPaUtils.Query qFIX = DocsPaUtils.InitQuery.getInstance().getQuery("FIX_PEC4_SPED_IS_MANCATA_CONSEGNA");
                        //    qFIX.setParam("idDoc", idDocument);
                        //    string FIXstring = qFIX.getSQL();
                        //    logger.Warn(FIXstring);
                        //    int fixRowTorn = 0;
                        //    retval = this.ExecuteNonQuery(FIXstring, out fixRowTorn);
                        //    if (fixRowTorn > 0)
                        //    {
                        //        logger.Warn("Fix eseguito, righe aggiornate:" + fixRowTorn);
                        //    }
                        //    else
                        //    {
                        //        logger.Error("Errore nell'esecuzione del FIX");
                        //    }
                        //}

                    }
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Errore nell'aggiornamento della status mask: Messaggio {0} - StackTrace {1}", ex.Message, ex.StackTrace);

                retval = false;
            }

            return retval;
        }

        public bool IS_statusMaskUpdater(string idDoc)
        {
            bool retval = false;
            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query qFix = DocsPaUtils.InitQuery.getInstance().getQuery("FIX_PEC4_SPED_IS_AVV_CONSEGNA");
                    qFix.setParam("idDoc", idDoc);
                    string fixString = qFix.getSQL();
                    logger.Debug(fixString);
                    int fixRowTorn = 0;
                    retval = this.ExecuteNonQuery(fixString, out fixRowTorn);
                    if (fixRowTorn > 0)
                    {
                        logger.Warn("Fix eseguito avvenuta consegna , righe aggiornate:" + fixRowTorn);
                    }
                    else
                    {
                        logger.Warn("Errore nell'esecuzione del FIX, o nessuna ricevuta di avvenuta consegna");
                    }

                    DocsPaUtils.Query qFix2 = DocsPaUtils.InitQuery.getInstance().getQuery("FIX_PEC4_SPED_IS_AVV_CONSEGNA");
                    qFix2.setParam("idDoc", idDoc);
                    string fixString2 = qFix2.getSQL();
                    logger.Debug(fixString2);
                    int fixRowTorn2 = 0;
                    retval = this.ExecuteNonQuery(fixString2, out fixRowTorn2);
                    if (fixRowTorn2 > 0)
                    {
                        logger.Warn("Fix eseguito avvenuta , righe aggiornate:" + fixRowTorn2);
                    }
                    else
                    {
                        logger.Warn("Errore nell'esecuzione del FIX, o nessuna ricevuta mancata consegna");
                    }
                }
                
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Errore nell'aggiornamento della status mask per le spedizioni IS: Messaggio {0} - StackTrace {1}", ex.Message, ex.StackTrace);

                retval = false;
            }
            return retval;
        }
    }
}
