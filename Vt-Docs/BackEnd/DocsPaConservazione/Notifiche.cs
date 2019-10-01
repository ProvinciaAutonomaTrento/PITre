using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Linq;
using log4net;

namespace DocsPaConservazione
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class InfoNotifica
    {
        /// <summary>
        /// Id dell'istanza cui si riferisce la notifica
        /// </summary>
        public string IdIstanza
        {
            get;
            set;
        }

        /// <summary>
        /// Id del supporto cui si riferisce la notifica
        /// </summary>
        public string IdSupporto
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int GiorniScadenza
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime DataScadenza
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Descrizione
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Gestione delle notifiche
    /// MEV CS 1.5:
    /// aggiunte notifiche su verifiche di leggibilità
    /// </summary>
    public sealed class Notifiche
    {
        /// <summary>
        /// 
        /// </summary>
        private static ILog logger = LogManager.GetLogger(typeof(Notifiche));

        

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int GetCountNotifiche(string codiceAmministrazione)
        {
            int totalCount = 0;

            // Reperimento dei supporti per cui è necessario effettuare l'integrità
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                //int giorniConservazione = 10;

                //string value =  System.Configuration.ConfigurationManager.AppSettings["CONSERVAZIONE_GG_NOTIFICHE"];

                //MEV CS 1.5
                string q = string.Format("SELECT SYSTEM_ID FROM DPA_AMMINISTRA WHERE VAR_CODICE_AMM='{0}'", codiceAmministrazione);
                string idAmm = string.Empty;

                if(!dbProvider.ExecuteScalar(out idAmm, q))
                    throw new Exception("Errore nel reperimento dell'id amm");

                string value = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "BE_CONSERVAZIONE_GG_NOTIFICHE");

                if (!string.IsNullOrEmpty(value))
                {
                    //Int32.TryParse(value, out giorniConservazione);
                }

                //DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_NOTIFICHE_INTEGRITA_SUPPORTI_CONSERVAZIONE");
                string queryDefinition = "";
                if (DocsPaConsManager.supportiRimovibiliVerificabili())
                {
                    queryDefinition = "S_CONSERVAZIONE_GET_COUNT_NOTIFICHE_VERIFICHE_SUPPORTI";
                }
                else
                {
                    queryDefinition = "S_CONSERVAZIONE_GET_COUNT_NOTIFICHE_VERIFICHE_SUPPORTI_REMOTI";
                }
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery(queryDefinition);

                // MEV CS 1.5
                // il termine per l'invio delle notifiche deve essere recuperato da chiave di DB
                int giorniConservazione = Int32.Parse(value);

                queryDef.setParam("dataConfronto", DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Today.AddDays(giorniConservazione).ToString("dd/MM/yyyy")));
                queryDef.setParam("codiceAmministrazione", codiceAmministrazione);

                string commandText = queryDef.getSQL();
                logger.InfoFormat("{1}: {0}", commandText,queryDefinition);

                string field;
                if (!dbProvider.ExecuteScalar(out field, commandText))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);

                if (!string.IsNullOrEmpty(field))
                    totalCount += Convert.ToInt32(field);
            }

            return totalCount;
        }

        /// <summary>
        /// Reperimento delle notifiche di conservazione
        /// </summary>
        /// <returns></returns>
        public static InfoNotifica[] GetNotifiche(string idAmm)
        {
            List<InfoNotifica> list = new List<InfoNotifica>();

            // Reperimento dei supporti per cui è necessario effettuare l'integrità
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                //DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_NOTIFICHE_INTEGRITA_SUPPORTI_CONSERVAZIONE");
                string queryDefinition = "";
                if (DocsPaConsManager.supportiRimovibiliVerificabili())
                {
                    queryDefinition = "S_CONSERVAZIONE_GET_NOTIFICHE_VERIFICHE_SUPPORTI";
                }
                else
                {
                    queryDefinition = "S_CONSERVAZIONE_GET_NOTIFICHE_VERIFICHE_SUPPORTI_REMOTI";
                }
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery(queryDefinition);
                
                int giorniConservazione = 10;

                //
                // MEV CSC 1.3 - La chiave di Web Config deve diventare di DB
                string value = string.Empty;
                if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "BE_CONSERVAZIONE_GG_NOTIFICHE"))) 
                {
                    value = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "BE_CONSERVAZIONE_GG_NOTIFICHE");
                }
                else
                    value = System.Configuration.ConfigurationManager.AppSettings["CONSERVAZIONE_GG_NOTIFICHE"];

                //string value = System.Configuration.ConfigurationManager.AppSettings["CONSERVAZIONE_GG_NOTIFICHE"];
                
                // End MEV
                //

                if (!string.IsNullOrEmpty(value))
                {
                    Int32.TryParse(value, out giorniConservazione);
                }

                queryDef.setParam("idAmm", idAmm);

                queryDef.setParam("dataConfronto", DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Today.AddDays(giorniConservazione).ToString("dd/MM/yyyy")));

                string commandText = queryDef.getSQL();
                logger.InfoFormat("{1}: {0}", commandText,queryDefinition);

                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                        list.Add(CreateInfoNotificaIntegritaSupporto(reader));
                }
            }

            return list.OrderByDescending(e => e.GiorniScadenza).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static InfoNotifica CreateInfoNotificaIntegritaSupporto(IDataReader reader)
        {
            const string DESCRIZIONE_IN_SCADENZA = "Il termine per la verifica di integrità del supporto {1} scadrà tra {0} giorni.";
            const string DESCRIZIONE_SCADUTO = "Il termine per la verifica di integrità del supporto {1} è scaduto da {0} giorni.";
            const string DESCRIZIONE_SCADUTO_IERI = "Il termine per la verifica di integrità del supporto {0} è scaduto ieri.";
            const string DESCRIZIONE_SCADENZA_ODIERNA = "Il termine per la verifica di integrità del supporto {0} scadrà oggi.";
            const string DESCRIZIONE_SCADENZA_DOMANI = "Il termine per la verifica di integrità del supporto {0} scadrà domani.";
            
            DateTime dataScadenza = reader.GetDateTime(reader.GetOrdinal("DataVerificaSupporto"));
            string tipoSupporto = reader.GetValue(reader.GetOrdinal("TipoSupporto")).ToString().ToLower();
            if (string.IsNullOrEmpty(tipoSupporto)) tipoSupporto = "esterno";
            int giorniScadenza = DateTime.Today.Subtract(dataScadenza).Days;

            string descrizione = string.Empty;

            DateTime date = new DateTime(dataScadenza.Year, dataScadenza.Month, dataScadenza.Day);

            if (DateTime.Today > date)
            {
                if (DateTime.Today.Subtract(date).Days == 1)
                    descrizione = string.Format(DESCRIZIONE_SCADUTO_IERI,tipoSupporto);
                else
                    descrizione = string.Format(DESCRIZIONE_SCADUTO, giorniScadenza,tipoSupporto);
            }
            else if (DateTime.Today == date)
                descrizione = string.Format(DESCRIZIONE_SCADENZA_ODIERNA,tipoSupporto);
            else
            {
                if (date.Subtract(DateTime.Today).Days == 1)
                    descrizione = string.Format(DESCRIZIONE_SCADENZA_DOMANI,tipoSupporto);
                else
                    descrizione = string.Format(DESCRIZIONE_IN_SCADENZA, System.Math.Abs(giorniScadenza),tipoSupporto);
            }
           
            return new InfoNotifica
            {
                IdIstanza = reader.GetValue(reader.GetOrdinal("IdIstanza")).ToString(),
                IdSupporto = reader.GetValue(reader.GetOrdinal("IdSupporto")).ToString(),
                DataScadenza = dataScadenza,
                GiorniScadenza = giorniScadenza,
                Descrizione = descrizione
            };
        }

        // MEV CS 1.5 - notifiche scadenza verifiche di leggibilità
        
        public static int GetCountNotificheLeggibilita(string codiceAmministrazione)
        {
            int totalCount = 0;

            // Reperimento dei supporti per cui è necessario effettuare l'integrità
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                //int giorniConservazione = 10;
                
                //reperimento chiave di configurazione
                string q = string.Format("SELECT SYSTEM_ID FROM DPA_AMMINISTRA WHERE VAR_CODICE_AMM='{0}'", codiceAmministrazione);
                string idAmm = string.Empty;
                if (!dbProvider.ExecuteScalar(out idAmm, q))
                    throw new Exception("Errore nel reperimento dell'id amministrazione");

                string value = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "BE_CONS_VER_LEG_GG_NOTIFICHE");

                if (string.IsNullOrEmpty(value))
                {
                    //se non è definita la chiave?
                    throw new Exception("Chiave di configurazione non definita.");
                }

                string queryDefinition = "";
                if (DocsPaConsManager.supportiRimovibiliVerificabili())
                {
                    queryDefinition = "S_CONSERVAZIONE_GET_COUNT_NOTIFICHE_VERIFICHE_LEGG_SUPPORTI";
                }
                else
                {
                    queryDefinition = "S_CONSERVAZIONE_GET_COUNT_NOTIFICHE_VERIFICHE_LEGG_SUPPORTI_REMOTI";
                }

                int giorniConservazione = Int32.Parse(value);

                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery(queryDefinition);
                queryDef.setParam("dataConfronto", DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Today.AddDays(giorniConservazione).ToString("dd/MM/yyyy")));
                queryDef.setParam("codiceAmministrazione", codiceAmministrazione);

                string commandText = queryDef.getSQL();
                logger.InfoFormat("{1}: {0}", commandText, queryDefinition);

                string field;
                if (!dbProvider.ExecuteScalar(out field, commandText))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);

                if (!string.IsNullOrEmpty(field))
                    totalCount += Convert.ToInt32(field);
            }

            return totalCount;
        }

        public static InfoNotifica[] GetNotificheTotali(string idAmm)
        {
            List<InfoNotifica> retVal = new List<InfoNotifica>();
            
            //aggiungo le notifiche di integrità
            retVal.AddRange(GetNotifiche(idAmm));
            
            //aggiungo le notifiche di leggibilità
            retVal.AddRange(GetNotificheLeggibilita(idAmm));

            return retVal.OrderByDescending(e => e.GiorniScadenza).ToArray();
        }

        public static InfoNotifica[] GetNotificheLeggibilita(string idAmm)
        {
            List<InfoNotifica> list = new List<InfoNotifica>();

            // Reperimento dei supporti per cui è necessario effettuare la verifica di leggibilità
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                
                string queryDefinition = "";
                if (DocsPaConsManager.supportiRimovibiliVerificabili())
                {
                    queryDefinition = "S_CONSERVAZIONE_GET_NOTIFICHE_VERIFICHE_LEGG_SUPPORTI";
                }
                else
                {
                    queryDefinition = "S_CONSERVAZIONE_GET_NOTIFICHE_VERIFICHE_LEGG_SUPPORTI_REMOTI";
                }
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery(queryDefinition);

                //int giorniConservazione = 10;

                string value = string.Empty;
                if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "BE_CONS_VER_LEG_GG_NOTIFICHE")))
                {
                    value = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "BE_CONS_VER_LEG_GG_NOTIFICHE");
                }
                //else
                    //value = System.Configuration.ConfigurationManager.AppSettings["CONSERVAZIONE_GG_NOTIFICHE"];


                if (!string.IsNullOrEmpty(value))
                {
                    //Int32.TryParse(value, out giorniConservazione);
                }

                int giorniConservazione = Int32.Parse(value);

                queryDef.setParam("idAmm", idAmm);

                queryDef.setParam("dataConfronto", DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Today.AddDays(giorniConservazione).ToString("dd/MM/yyyy")));

                string commandText = queryDef.getSQL();
                logger.InfoFormat("{1}: {0}", commandText, queryDefinition);

                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                        list.Add(CreateInfoNotificaLeggibilitaSupporto(reader));
                }
            }

            return list.OrderByDescending(e => e.GiorniScadenza).ToArray();
        }

        private static InfoNotifica CreateInfoNotificaLeggibilitaSupporto(IDataReader reader)
        {
            const string DESCRIZIONE_IN_SCADENZA = "Il termine per la verifica di leggibilità dei documenti del supporto {1} scadrà tra {0} giorni.";
            const string DESCRIZIONE_SCADUTO = "Il termine per la verifica di leggibilità dei documenti del supporto {1} è scaduto da {0} giorni.";
            const string DESCRIZIONE_SCADUTO_IERI = "Il termine per la verifica di leggibilità dei documenti del supporto {0} è scaduto ieri.";
            const string DESCRIZIONE_SCADENZA_ODIERNA = "Il termine per la verifica di leggibilità dei documenti del supporto {0} scadrà oggi.";
            const string DESCRIZIONE_SCADENZA_DOMANI = "Il termine per la verifica di leggibilità dei documenti del supporto {0} scadrà domani.";

            DateTime dataScadenza = reader.GetDateTime(reader.GetOrdinal("DataVerificaSupporto"));
            string tipoSupporto = reader.GetValue(reader.GetOrdinal("TipoSupporto")).ToString().ToLower();
            if (string.IsNullOrEmpty(tipoSupporto)) tipoSupporto = "esterno";
            int giorniScadenza = DateTime.Today.Subtract(dataScadenza).Days;

            string descrizione = string.Empty;

            DateTime date = new DateTime(dataScadenza.Year, dataScadenza.Month, dataScadenza.Day);

            if (DateTime.Today > date)
            {
                if (DateTime.Today.Subtract(date).Days == 1)
                    descrizione = string.Format(DESCRIZIONE_SCADUTO_IERI, tipoSupporto);
                else
                    descrizione = string.Format(DESCRIZIONE_SCADUTO, giorniScadenza, tipoSupporto);
            }
            else if (DateTime.Today == date)
                descrizione = string.Format(DESCRIZIONE_SCADENZA_ODIERNA, tipoSupporto);
            else
            {
                if (date.Subtract(DateTime.Today).Days == 1)
                    descrizione = string.Format(DESCRIZIONE_SCADENZA_DOMANI, tipoSupporto);
                else
                    descrizione = string.Format(DESCRIZIONE_IN_SCADENZA, System.Math.Abs(giorniScadenza), tipoSupporto);
            }

            return new InfoNotifica
            {
                IdIstanza = reader.GetValue(reader.GetOrdinal("IdIstanza")).ToString(),
                IdSupporto = reader.GetValue(reader.GetOrdinal("IdSupporto")).ToString(),
                DataScadenza = dataScadenza,
                GiorniScadenza = giorniScadenza,
                Descrizione = descrizione
            };
        }

        // fine MEV CS 1.5
    }
}