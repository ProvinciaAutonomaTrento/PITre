using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaConservazione
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public sealed class Contatori
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <returns></returns>
        public static Contatori GetByCodice(string codiceAmministrazione)
        {
            DocsPaVO.utente.Amministrazione[] amministrazioni = (DocsPaVO.utente.Amministrazione[])
                        BusinessLogic.Amministrazione.AmministraManager.GetAmministrazioni().ToArray(typeof(DocsPaVO.utente.Amministrazione));
            
            if (amministrazioni.Count(e => e.codice.ToLowerInvariant() == codiceAmministrazione.ToLowerInvariant()) == 0)
            {
                throw new ApplicationException(string.Format("Amministrazione con codice '{0}' non trovata", codiceAmministrazione));
            }
            else
            {
                Contatori retValue = new Contatori();

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_ITEMS_CONSERVAZIONE");
                    queryDef.setParam("codiceAmministrazione", codiceAmministrazione);

                    string commandText = queryDef.getSQL();

                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            string stato = reader.GetString(reader.GetOrdinal("STATO"));
                            int totale = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("TOTALE")).ToString());

                            if (stato == DocsPaConservazione.StatoIstanza.DA_INVIARE)
                                retValue.DaInviare = totale;
                            else if (stato == DocsPaConservazione.StatoIstanza.INVIATA)
                                retValue.Inviate = totale;
                            else if (stato == DocsPaConservazione.StatoIstanza.RIFIUTATA)
                                retValue.Rifiutate = totale;
                            else if (stato == DocsPaConservazione.StatoIstanza.IN_LAVORAZIONE)
                                retValue.InLavorazione = totale;
                            else if (stato == DocsPaConservazione.StatoIstanza.FIRMATA)
                                retValue.Firmate = totale;
                            else if (stato == DocsPaConservazione.StatoIstanza.CONSERVATA)
                                retValue.Conservate = totale;
                            else if (stato == DocsPaConservazione.StatoIstanza.CHIUSA)
                                retValue.Chiuse = totale;
                        }
                    }
                }

                // Reperimento contantore delle notifiche
                //retValue.Notifiche = DocsPaConservazione.Notifiche.GetCountNotifiche(codiceAmministrazione); 

                // MEV CS 1.5
                // aggiungo le notifiche di leggibilità al contatore
                retValue.Notifiche = DocsPaConservazione.Notifiche.GetCountNotifiche(codiceAmministrazione) + DocsPaConservazione.Notifiche.GetCountNotificheLeggibilita(codiceAmministrazione);

                return retValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static Contatori Get(DocsPaVO.utente.InfoUtente infoUtente)
        {
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {   
                string commandText = string.Format("SELECT VAR_CODICE_AMM FROM DPA_AMMINISTRA WHERE SYSTEM_ID = (SELECT ID_AMM FROM PEOPLE WHERE SYSTEM_ID = {0})", infoUtente.idPeople);

                string field;
                if (!dbProvider.ExecuteScalar(out field, commandText))
                    throw new ApplicationException("Amministrazione non trovata per l'utente");

                return GetByCodice(field);
            }
        }

        /// <summary>
        /// Numero totale di istanze da inviare
        /// </summary>
        public int DaInviare
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale di istanze inviate
        /// </summary>
        public int Inviate
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale di istanze rifiutate
        /// </summary>
        public int Rifiutate
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale di istanze in lavorazione
        /// </summary>
        public int InLavorazione
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale di istanze firmate
        /// </summary>
        public int Firmate
        {
            get;
            set;
        }

        /// <summary>
        /// Indica il numero totale di istanza in stato conservate
        /// </summary>
        public int Conservate
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale di istanze chiuse
        /// </summary>
        public int Chiuse
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale delle notifiche di conservazione
        /// </summary>
        public int Notifiche
        {
            get;
            set;
        }
    }
}
