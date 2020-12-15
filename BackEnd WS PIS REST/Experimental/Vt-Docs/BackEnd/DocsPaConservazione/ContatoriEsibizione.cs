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
    public sealed class ContatoriEsibizione
    {
        // 
        // il modificatore sealed impedisce ad altre classi di ereditare da tale classe.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <returns></returns>
        public static ContatoriEsibizione GetByCodice(string codiceAmministrazione, string idGruppo, string idPeople)
        {
            DocsPaVO.utente.Amministrazione[] amministrazioni = (DocsPaVO.utente.Amministrazione[])
                        BusinessLogic.Amministrazione.AmministraManager.GetAmministrazioni().ToArray(typeof(DocsPaVO.utente.Amministrazione));

            if (amministrazioni.Count(e => e.codice.ToLowerInvariant() == codiceAmministrazione.ToLowerInvariant()) == 0)
            {
                throw new ApplicationException(string.Format("Amministrazione con codice '{0}' non trovata", codiceAmministrazione));
            }
            else
            {
                ContatoriEsibizione retValue = new ContatoriEsibizione();

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_ITEMS_ESIBIZIONE");
                    queryDef.setParam("codiceAmministrazione", codiceAmministrazione);
                    queryDef.setParam("idPeople", idPeople);
                    
                    string q1 = " SELECT dpa_corr_globali.system_id FROM dpa_corr_globali WHERE dpa_corr_globali.id_gruppo = " + idGruppo;
                    string idRuoloInUo = string.Empty;
                    dbProvider.ExecuteScalar(out idRuoloInUo, q1);

                    queryDef.setParam("idRuoloInUo", idRuoloInUo);

                    string commandText = queryDef.getSQL();

                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                string stato = reader.GetString(reader.GetOrdinal("STATO"));
                                string cha_certificata = string.Empty;
                                try
                                {
                                    cha_certificata = reader.GetString(reader.GetOrdinal("CERTIFICATA"));
                                }
                                catch (Exception e)
                                {
                                    cha_certificata = string.Empty;
                                }
                                // Campo totale della Count
                                int totale = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("TOTALE")).ToString());

                                if (stato == DocsPaConservazione.StatoIstanzaEsibizione.NUOVA)
                                {
                                    if (string.IsNullOrEmpty(cha_certificata) || (!string.IsNullOrEmpty(cha_certificata) && cha_certificata == "0"))
                                        retValue.Nuove = totale;

                                    if (!string.IsNullOrEmpty(cha_certificata) && cha_certificata == "1")
                                        retValue.Nuove_Certificata = totale;
                                }
                                else
                                    if (stato == DocsPaConservazione.StatoIstanzaEsibizione.CHIUSA)
                                    {
                                        if (string.IsNullOrEmpty(cha_certificata) || (!string.IsNullOrEmpty(cha_certificata) && cha_certificata == "0"))
                                            retValue.Chiuse = totale;

                                        if (!string.IsNullOrEmpty(cha_certificata) && cha_certificata == "1")
                                            retValue.Chiuse_Certificata = totale;
                                    }
                                    else
                                        if (stato == DocsPaConservazione.StatoIstanzaEsibizione.RIFIUTATA)
                                        {
                                            if (string.IsNullOrEmpty(cha_certificata) || (!string.IsNullOrEmpty(cha_certificata) && cha_certificata == "0"))
                                                retValue.Rifiutate = totale;

                                            if (!string.IsNullOrEmpty(cha_certificata) && cha_certificata == "1")
                                                retValue.Rifiutate_Certificata = totale;
                                        }
                                        else
                                            if (stato == DocsPaConservazione.StatoIstanzaEsibizione.IN_ATTESA_DI_CERTIFICAZIONE)
                                            {
                                                if (string.IsNullOrEmpty(cha_certificata) || (!string.IsNullOrEmpty(cha_certificata) && cha_certificata == "0"))
                                                    retValue.InAttesaDiCertificazione = totale;

                                                if (!string.IsNullOrEmpty(cha_certificata) && cha_certificata == "1")
                                                    retValue.InAttesaDiCertificazione_Certificata = totale;
                                            }
                                            else
                                                if (stato == DocsPaConservazione.StatoIstanzaEsibizione.IN_TRANSIZIONE)
                                                {
                                                    if (string.IsNullOrEmpty(cha_certificata) || (!string.IsNullOrEmpty(cha_certificata) && cha_certificata == "0"))
                                                        retValue.Transizione = totale;

                                                    if (!string.IsNullOrEmpty(cha_certificata) && cha_certificata == "1")
                                                        retValue.Transizione_Certificata = totale;
                                                }
                            }
                        }
                    }
                }

                return retValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static ContatoriEsibizione Get(DocsPaVO.utente.InfoUtente infoUtente, string idGruppo)
        {
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT VAR_CODICE_AMM FROM DPA_AMMINISTRA WHERE SYSTEM_ID = (SELECT ID_AMM FROM PEOPLE WHERE SYSTEM_ID = {0})", infoUtente.idPeople);

                string field;
                if (!dbProvider.ExecuteScalar(out field, commandText))
                    throw new ApplicationException("Amministrazione non trovata per l'utente");

                return GetByCodice(field, idGruppo, infoUtente.idPeople);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static ContatoriEsibizione GetConservazione(DocsPaVO.utente.InfoUtente infoUtente)
        {
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT VAR_CODICE_AMM FROM DPA_AMMINISTRA WHERE SYSTEM_ID = (SELECT ID_AMM FROM PEOPLE WHERE SYSTEM_ID = {0})", infoUtente.idPeople);

                string field;
                if (!dbProvider.ExecuteScalar(out field, commandText))
                    throw new ApplicationException("Amministrazione non trovata per l'utente");

                return GetByCodiceConservazione(field);
                //return GetByCodice(field, idGruppo, infoUtente.idPeople);
            }
        }


        public static ContatoriEsibizione GetByCodiceConservazione(string codiceAmministrazione)
        {
            DocsPaVO.utente.Amministrazione[] amministrazioni = (DocsPaVO.utente.Amministrazione[])
            BusinessLogic.Amministrazione.AmministraManager.GetAmministrazioni().ToArray(typeof(DocsPaVO.utente.Amministrazione));

            if (amministrazioni.Count(e => e.codice.ToLowerInvariant() == codiceAmministrazione.ToLowerInvariant()) == 0)
            {
                throw new ApplicationException(string.Format("Amministrazione con codice '{0}' non trovata", codiceAmministrazione));
            }
            else
            {
                ContatoriEsibizione retValue = new ContatoriEsibizione();

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_ITEMS_ESIBIZIONE_CONSERVAZIONE");
                    queryDef.setParam("codiceAmministrazione", codiceAmministrazione);

                    string commandText = queryDef.getSQL();

                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                string stato = reader.GetString(reader.GetOrdinal("STATO"));
                                string cha_certificata = string.Empty;
                                try
                                {
                                    cha_certificata = reader.GetString(reader.GetOrdinal("CERTIFICATA"));
                                }
                                catch (Exception e)
                                {
                                    cha_certificata = string.Empty;
                                }
                                // Campo totale della Count
                                int totale = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("TOTALE")).ToString());

                                if (stato == DocsPaConservazione.StatoIstanzaEsibizione.NUOVA)
                                {
                                    if (string.IsNullOrEmpty(cha_certificata) || (!string.IsNullOrEmpty(cha_certificata) && cha_certificata == "0"))
                                        retValue.Nuove = totale;

                                    if (!string.IsNullOrEmpty(cha_certificata) && cha_certificata == "1")
                                        retValue.Nuove_Certificata = totale;
                                }
                                else
                                    if (stato == DocsPaConservazione.StatoIstanzaEsibizione.CHIUSA)
                                    {
                                        if (string.IsNullOrEmpty(cha_certificata) || (!string.IsNullOrEmpty(cha_certificata) && cha_certificata == "0"))
                                            retValue.Chiuse = totale;

                                        if (!string.IsNullOrEmpty(cha_certificata) && cha_certificata == "1")
                                            retValue.Chiuse_Certificata = totale;
                                    }
                                    else
                                        if (stato == DocsPaConservazione.StatoIstanzaEsibizione.RIFIUTATA)
                                        {
                                            if (string.IsNullOrEmpty(cha_certificata) || (!string.IsNullOrEmpty(cha_certificata) && cha_certificata == "0"))
                                                retValue.Rifiutate = totale;

                                            if (!string.IsNullOrEmpty(cha_certificata) && cha_certificata == "1")
                                                retValue.Rifiutate_Certificata = totale;
                                        }
                                        else
                                            if (stato == DocsPaConservazione.StatoIstanzaEsibizione.IN_ATTESA_DI_CERTIFICAZIONE)
                                            {
                                                if (string.IsNullOrEmpty(cha_certificata) || (!string.IsNullOrEmpty(cha_certificata) && cha_certificata == "0"))
                                                    retValue.InAttesaDiCertificazione = totale;

                                                if (!string.IsNullOrEmpty(cha_certificata) && cha_certificata == "1")
                                                    retValue.InAttesaDiCertificazione_Certificata = totale;
                                            }
                                            else
                                                if (stato == DocsPaConservazione.StatoIstanzaEsibizione.IN_TRANSIZIONE)
                                                {
                                                    if (string.IsNullOrEmpty(cha_certificata) || (!string.IsNullOrEmpty(cha_certificata) && cha_certificata == "0"))
                                                        retValue.Transizione = totale;

                                                    if (!string.IsNullOrEmpty(cha_certificata) && cha_certificata == "1")
                                                        retValue.Transizione_Certificata = totale;
                                                }
                            }
                        }
                    }
                }

                return retValue;
            }
        }

        /// <summary>
        /// Numero totale di istanze di esibizione in stato Nuova (N) e CHA_CERTIFICATA = 1
        /// </summary>
        public int Nuove_Certificata
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale di istanze di esibizione in stato Nuova (N)
        /// </summary>
        public int Nuove
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale di istanze di esibizione in stato Chiusa (C) e CHA_CERTIFICATA = 1
        /// </summary>
        public int Chiuse_Certificata
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale di istanze di esibizione in stato Chiusa (C)
        /// </summary>
        public int Chiuse
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale di istanze di esibizione in stato Rifiutata (R) e CHA_CERTIFICATA = 1
        /// </summary>
        public int Rifiutate_Certificata
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale di istanze di esibizione in stato Rifiutata (R)
        /// </summary>
        public int Rifiutate
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale di istanze di esibizione in stato in attesa di certificazione (I) e CHA_CERTIFICATA = 1
        /// </summary>
        public int InAttesaDiCertificazione_Certificata
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale di istanze di esibizione in stato in attesa di certificazione (I)
        /// </summary>
        public int InAttesaDiCertificazione
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale di istanze di esibizione in stato Transizione (T) e CHA_CERTIFICATA = 1
        /// </summary>
        public int Transizione_Certificata
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale di istanze di esibizione in stato Transizione (T)
        /// </summary>
        public int Transizione
        {
            get;
            set;
        }
    }
}
