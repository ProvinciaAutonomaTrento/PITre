using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.DatiCert;
using DocsPaDbManagement.Functions;
using System.Data;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{
    public class InteroperabilitaDatiCert : DBProvider
    {
        private ILog logger = LogManager.GetLogger(typeof(Documentale));

        public bool verificaPresenzaNotifica(Notifica notifica)
        {
            bool verifica = false;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_VERIFICA_PRESENZA_DPA_NOTIFICA");

            q.setParam("idtiponotifica", notifica.idTipoNotifica);
            q.setParam("docnumber", notifica.docnumber);
            q.setParam("mittente", notifica.mittente.Replace("'", "''"));
            q.setParam("risposte", notifica.risposte.Replace("'", "''"));
            q.setParam("oggetto", notifica.oggetto.Replace("'", "''"));
            q.setParam("gestioneemittente", notifica.gestioneEmittente);
            q.setParam("zona", notifica.zona);
            q.setParam("giorno", Functions.ToDate(notifica.data_ora));
            q.setParam("identificativo", notifica.identificativo);
            q.setParam("msgid", notifica.msgid);
            q.setParam("tiporicevuta", notifica.tipoRicevuta);
            q.setParam("consegna", notifica.consegna);
            q.setParam("ricezione", notifica.ricezione);
            if (!string.IsNullOrEmpty(notifica.errore_esteso))
            {
                q.setParam("erroreesteso", notifica.errore_esteso.Replace("'", "''"));
            }
            else
            {
                q.setParam("erroreesteso", "");
            }
            if (!string.IsNullOrEmpty(notifica.erroreRicevuta))
            {
                q.setParam("errorericevuta", notifica.erroreRicevuta.Replace("'", "''"));
            }
            else
            {
                q.setParam("errorericevuta", "");
            }
            logger.Debug("inserimento in notifica: " + q.getSQL());

            using (IDataReader reader = ExecuteReader(q.getSQL()))
            {
                if (reader != null && reader.Read())
                    verifica = true;
            }

            return verifica;
        }

        public bool inserisciTipoNotifica(TipoNotifica tipoNotifica)
        {
            bool retval = false;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_TIPO_NOTIFICA");


            if (dbType.ToUpper().Equals("ORACLE"))
            {
                q.setParam("var_systemid", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                q.setParam("systemid", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_TIPO_NOTIFICA"));
            }
            else
            {
                q.setParam("var_systemid", string.Empty);
                q.setParam("systemid", String.Empty);
            }
            q.setParam("codicenotifica", tipoNotifica.codiceNotifica);
            q.setParam("descrizione", tipoNotifica.descrizioneNotifica);

            logger.Debug("inserimento in tipo_notifica: " + q.getSQL());
            retval = ExecuteNonQuery(q.getSQL());

            return retval;
        }

        public bool updateTipoNotifica(TipoNotifica tipoNotifica)
        {
            bool retval = false;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TIPO_NOTIFICA");
            q.setParam("systemid", tipoNotifica.idTipoNotifica);
            q.setParam("codicenotifica", tipoNotifica.codiceNotifica);
            q.setParam("descrizione", tipoNotifica.descrizioneNotifica);

            logger.Debug("update tipo_notifica: " + q.getSQL());
            retval = ExecuteNonQuery(q.getSQL());

            return retval;
        }

        public bool inserisciNotifica(Notifica notifica, string idAllegato)
        {
            bool retval = false;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_NOTIFICA");


            logger.Debug("Inserimento parametri");
            if (dbType.ToUpper().Equals("ORACLE"))
            {
                q.setParam("var_systemid", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                q.setParam("systemid", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_NOTIFICA"));
            }
            else
            {
                q.setParam("var_systemid", string.Empty);
                q.setParam("systemid", String.Empty);
            }

            q.setParam("systemid", notifica.idNotifica);
            q.setParam("idtiponotifica", notifica.idTipoNotifica);
            q.setParam("docnumber", notifica.docnumber);
            q.setParam("mittente", notifica.mittente.Replace("'", "''"));
            q.setParam("tipoDestinatario", notifica.tipoDestinatario);
            q.setParam("destinatario", notifica.destinatario.Replace("'", "''"));
            q.setParam("risposte", notifica.risposte.Replace("'", "''"));
            q.setParam("oggetto", notifica.oggetto.Replace("'", "''"));
            q.setParam("gestioneemittente", notifica.gestioneEmittente);
            q.setParam("zona", notifica.zona);
            q.setParam("giorno",Functions.ToDate(notifica.data_ora));
            logger.Debug("parametro giorno inserito");
            q.setParam("identificativo", notifica.identificativo);
            q.setParam("msgid", notifica.msgid);
            q.setParam("tiporicevuta", notifica.tipoRicevuta);
            q.setParam("consegna", notifica.consegna);
            q.setParam("ricezione", notifica.ricezione);
            if (!string.IsNullOrEmpty(notifica.errore_esteso))
            {
                q.setParam("erroreesteso", notifica.errore_esteso.Replace("'", "''"));
            }
            else
            {
                q.setParam("erroreesteso", "");
            }
            if (!string.IsNullOrEmpty(notifica.erroreRicevuta))
            {
                q.setParam("errorericevuta", notifica.erroreRicevuta.Replace("'", "''"));
            }
            else
            {
                q.setParam("errorericevuta", "");
            }

            if (!string.IsNullOrEmpty(idAllegato))
            {
                q.setParam("idAllegato", idAllegato);
            }
            else
            {
                q.setParam("idAllegato", "null");
            }

                logger.Debug("inserimento in notifica: " + q.getSQL());
            retval = ExecuteNonQuery(q.getSQL());

            return retval;
        }

        public bool updateNotifica(Notifica notifica)
        {
            bool retval = false;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_NOTIFICA");
            q.setParam("systemid", notifica.idNotifica);
            q.setParam("idtiponotifica", notifica.idTipoNotifica);
            q.setParam("docnumber", notifica.docnumber);
            q.setParam("mittente", notifica.mittente.Replace("'", "''"));
            q.setParam("tipoDestinatario", notifica.tipoDestinatario);
            q.setParam("destinatario", notifica.destinatario.Replace("'", "''"));
            q.setParam("risposte", notifica.risposte.Replace("'", "''"));
            q.setParam("oggetto", notifica.oggetto.Replace("'", "''"));
            q.setParam("gestioneemittente", notifica.gestioneEmittente);
            q.setParam("zona", notifica.zona);
            q.setParam("giorno", Functions.ToDate(notifica.data_ora));
            q.setParam("identificativo", notifica.identificativo);
            q.setParam("msgid", notifica.msgid);
            q.setParam("tiporicevuta", notifica.tipoRicevuta);
            q.setParam("consegna", notifica.consegna);
            q.setParam("ricezione", notifica.ricezione);
            q.setParam("erroreesteso", notifica.errore_esteso.Replace("'", "''"));
            q.setParam("errorericevuta", notifica.erroreRicevuta.Replace("'", "''"));

            logger.Debug("update in notifica: " + q.getSQL());
            retval = ExecuteNonQuery(q.getSQL());

            return retval;
        }


        public bool deleteNotifica(string docnumber)
        {
            bool retval = false;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_NOTIFICA");
            q.setParam("docnumber", docnumber);

            logger.Debug("delete in notifica: " + q.getSQL());
            retval = ExecuteNonQuery(q.getSQL());

            return retval;
        }


        public TipoNotifica[] ricercaTipoNotifiche()
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TIPO_NOTIFICA");
            List<TipoNotifica> Tiponotifica = new List<TipoNotifica>();
            
            logger.Debug("ricerca tipo notifica " + q.getSQL());

            using (IDataReader reader = ExecuteReader(q.getSQL()))
            {
                if (reader.FieldCount > 0)
                    if (reader.Read())
                    {
                        TipoNotifica tipoNotifica = new TipoNotifica();
                        tipoNotifica.idTipoNotifica = reader["SYSTEM_ID"].ToString();
                        tipoNotifica.codiceNotifica = reader["VAR_CODICE_NOTIFICA"].ToString();
                        tipoNotifica.descrizioneNotifica = reader["VAR_DESCRIZIONE"].ToString();
                        Tiponotifica.Add(tipoNotifica);
                    }
            }

            return Tiponotifica.ToArray();

        }


        public TipoNotifica ricercaTipoNotificheByCodice(string codiceNotifica)
        {
            TipoNotifica tipoNotifica = null;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TIPO_NOTIFICA_BY_CODICE");
            q.setParam("codicenotifica", codiceNotifica);

            logger.Debug("ricerca tipo notifica by codice : " + q.getSQL());
            logger.Debug("execute reader");

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                using (IDataReader reader = dbProvider.ExecuteReader(q.getSQL()))
                {
                    logger.Debug("fieldCount: " + reader.FieldCount);

                    if (reader.FieldCount > 0)
                    {
                        if (reader.Read())
                        {
                            tipoNotifica = new TipoNotifica();
                            logger.Debug("SYSTEM_ID: " + reader["SYSTEM_ID"].ToString());
                            tipoNotifica.idTipoNotifica = reader["SYSTEM_ID"].ToString();
                            logger.Debug("VAR_CODICE_NOTIFICA: " + reader["VAR_CODICE_NOTIFICA"].ToString());
                            tipoNotifica.codiceNotifica = reader["VAR_CODICE_NOTIFICA"].ToString();
                            logger.Debug("VAR_DESCRIZIONE: " + reader["VAR_DESCRIZIONE"].ToString());
                            tipoNotifica.descrizioneNotifica = reader["VAR_DESCRIZIONE"].ToString();
                            logger.Debug("tipoNotifica creato");
                        }
                        logger.Debug("fuori dal primo if");
                    }
                    logger.Debug("fuori dal secondo if");
                }
                logger.Debug("fuori dal primo using");
            }
            logger.Debug("fuori dal secondo using");
            return tipoNotifica;

        }


        public Notifica[] ricercaNotifica(string docnumber)
        {
            List<Notifica> lista = new List<Notifica>();

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_NOTIFICA");
            q.setParam("docnumber", docnumber);

            logger.Debug("ricerca notifica: " + q.getSQL());

            using (IDataReader reader = ExecuteReader(q.getSQL()))
            {

                while (reader.Read())
                {
                    Notifica notifica = new Notifica();


                    notifica.idNotifica = reader["SYSTEM_ID"].ToString();
                    notifica.mittente = reader["VAR_MITTENTE"].ToString();
                    notifica.tipoDestinatario = reader["VAR_TIPO_DESTINATARIO"].ToString();
                    notifica.destinatario = reader["VAR_DESTINATARIO"].ToString();

                    // Se il mittente è un url, del destinatario viene recuperata la descrizione
                    if (Uri.IsWellFormedUriString(notifica.mittente, UriKind.Absolute))
                        notifica.destinatario = this.GetDestinatarioPerIs(notifica.destinatario);

                    notifica.risposte = reader["VAR_RISPOSTE"].ToString();
                    notifica.oggetto = reader["VAR_OGGETTO"].ToString();
                    notifica.gestioneEmittente = reader["VAR_GESTIONE_EMITTENTE"].ToString();
                    notifica.zona = reader["VAR_ZONA"].ToString();
                    notifica.data_ora = ((DateTime)reader["VAR_GIORNO_ORA"]).ToString("dd/MM/yyyy HH:mm:ss");
                    notifica.identificativo = reader["VAR_IDENTIFICATIVO"].ToString();
                    notifica.msgid = reader["VAR_MSGID"].ToString();
                    notifica.tipoRicevuta = reader["VAR_TIPO_RICEVUTA"].ToString();
                    notifica.consegna = reader["VAR_CONSEGNA"].ToString();
                    notifica.ricezione = reader["VAR_RICEZIONE"].ToString();
                    notifica.errore_esteso = reader["VAR_ERRORE_ESTESO"].ToString();
                    notifica.docnumber = reader["DOCNUMBER"].ToString();
                    notifica.idTipoNotifica = reader["ID_TIPO_NOTIFICA"].ToString();
                    notifica.erroreRicevuta = reader["VAR_ERRORE_RICEVUTA"].ToString();
                    lista.Add(notifica);
                }
            }

            return lista.ToArray();
        }

        public Notifica[] ricercaNotificheFiltrate(DocsPaVO.filtri.FiltroRicerca[] filtri)
        {
            bool existFilterType = ((from f in filtri where f.argomento.Equals("filterTipo") && !string.IsNullOrEmpty(f.valore) select f).Count() > 0) ? true : false;
            bool existFilterMail = ((from f in filtri where (f.argomento.Equals("filterDest") && !string.IsNullOrEmpty(f.valore)) select f).Count() > 0) ? true : false;
            bool existFilterCode = ((from f in filtri where (f.argomento.Equals("filterCodiceIS") && !string.IsNullOrEmpty(f.valore)) select f).Count() > 0) ? true : false;
            List<Notifica> lista = new List<Notifica>();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_NOTIFICA3");
            q.setParam("docnumber", "docnumber = " + filtri[0].valore);
            // Impostazione filtri
            StringBuilder filterCond = new StringBuilder();
            //filtro tipo ricevuta
            if (existFilterType)
            {
                string[] arrType = (from f in filtri where f.argomento.Equals("filterTipo") select f.valore).First().Split('#');
                filterCond.AppendFormat(" AND(");
                // PALUMBO: modifica per consentire il filtro delle ricevute di preavviso-errore-consegna trattate come mancata consegna
                if (arrType[0].Equals("errore-consegna"))
                    filterCond.AppendFormat("tn.var_codice_notifica LIKE '%{0}'", arrType[0]);
                else
                    filterCond.AppendFormat("tn.var_codice_notifica='{0}'", arrType[0]);
                for (int i = 1; i < arrType.Length - 1; i++)
                {
                    
                    if (arrType[i].Equals("errore-consegna"))
                        filterCond.AppendFormat(" OR tn.var_codice_notifica LIKE '%{0}'", arrType[i]);                        
                    else
                        filterCond.AppendFormat(" OR tn.var_codice_notifica='{0}'", arrType[i]);
                }
                filterCond.AppendFormat(")");
            }

            if (existFilterMail || existFilterCode)
            {
                filterCond.AppendFormat(" AND(");
                // Filtro Mail
                if (existFilterMail)
                {
                    string[] arrMail = (from f in filtri where (f.argomento.Equals("filterDest")) select f.valore).First().Split('#');
                    filterCond.AppendFormat("lower(VAR_DESTINATARIO) LIKE '%{0}%'", arrMail[0]);
                    for (int i = 1; i < arrMail.Length - 1; i++)
                    {
                        filterCond.AppendFormat(" OR lower(VAR_DESTINATARIO) LIKE '%{0}%'", arrMail[i]);
                    }
                }
                // Filtro Codice corrispondente(IS)
                if (existFilterCode)
                {
                    if (existFilterMail)
                        filterCond.AppendFormat(" OR ");
                    filterCond.AppendFormat("lower(VAR_DESTINATARIO) LIKE lower('%{0}%')", (from f in filtri where (f.argomento.Equals("filterCodiceIS")) select f.valore).First());
                }
                filterCond.AppendFormat(")");
            }

            q.setParam("filterCond", filterCond.ToString());
           

            logger.Debug("ricerca notifiche filtrate: " + q.getSQL());
            using (IDataReader reader = ExecuteReader(q.getSQL()))
            {
                while (reader.Read())
                {
                    Notifica notifica = new Notifica();
                    notifica.idNotifica = reader["SYSTEM_ID"].ToString();
                    notifica.mittente = reader["VAR_MITTENTE"].ToString();
                    notifica.tipoDestinatario = reader["VAR_TIPO_DESTINATARIO"].ToString();
                    notifica.destinatario = reader["VAR_DESTINATARIO"].ToString();

                    // Se il mittente è un url, del destinatario viene recuperata la descrizione
                    if (Uri.IsWellFormedUriString(notifica.mittente, UriKind.Absolute))
                        notifica.destinatario = this.GetDestinatarioPerIs(notifica.destinatario);

                    notifica.risposte = reader["VAR_RISPOSTE"].ToString();
                    notifica.oggetto = reader["VAR_OGGETTO"].ToString();
                    notifica.gestioneEmittente = reader["VAR_GESTIONE_EMITTENTE"].ToString();
                    notifica.zona = reader["VAR_ZONA"].ToString();
                    notifica.data_ora = ((DateTime)reader["VAR_GIORNO_ORA"]).ToString("dd/MM/yyyy HH:mm:ss");
                    notifica.identificativo = reader["VAR_IDENTIFICATIVO"].ToString();
                    notifica.msgid = reader["VAR_MSGID"].ToString();
                    notifica.tipoRicevuta = reader["VAR_TIPO_RICEVUTA"].ToString();
                    notifica.consegna = reader["VAR_CONSEGNA"].ToString();
                    notifica.ricezione = reader["VAR_RICEZIONE"].ToString();
                    notifica.errore_esteso = reader["VAR_ERRORE_ESTESO"].ToString();
                    notifica.docnumber = reader["DOCNUMBER"].ToString();
                    notifica.idTipoNotifica = reader["ID_TIPO_NOTIFICA"].ToString();
                    notifica.erroreRicevuta = reader["VAR_ERRORE_RICEVUTA"].ToString();
                    lista.Add(notifica);
                }
            }

            return lista.ToArray();
        }

        /// <summary>
        /// Metodo per la formattazione del destinatario di una notifica nel caso di interoperabilità semplificata
        /// </summary>
        /// <param name="destinatario">Destinatario da cui estrarre i codici dei corrispondenti e crearne l'elenco di descrizioni separate da virgola</param>
        /// <returns>Corrispondente / i riportati per descrizione separati da virgola</returns>
        public String GetDestinatarioPerIs(String destinatario)
        {
            // Pulizia dei codici (nel caso di notifica di rifiuto della spedizione, vendono 
            // inviati dei codici di corrispondenti racchiusi fra singolo apice e separati da virgola
            String[] codici = destinatario.Replace("'", String.Empty).Split(',');

            // Descrizione dei corrispondenti
            String descrizione = String.Empty;

            // Risoluzione di tutti i codici
            foreach (var codice in codici)
                descrizione += String.Format("{0} ({1}), ",
                    Rubrica.GetCorrAttribute(codice.Trim(), Rubrica.CorrAttributes.VAR_DESC_CORR),
                    codice);

            return descrizione.Substring(0, descrizione.Length - 2);

        }

        public TipoNotifica ricercaTipoNotificheBySystemId(string systemId)
        {
            TipoNotifica tipoNotifica = null;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TIPO_NOTIFICA_BY_SYSTEM_ID");
            q.setParam("systemid", systemId);

            logger.Debug("ricerca tipo notifica by system id: " + q.getSQL());

            using (IDataReader reader = ExecuteReader(q.getSQL()))
            {
                if (reader.FieldCount > 0)
                    if (reader.Read())
                    {
                        tipoNotifica = new TipoNotifica();
                        tipoNotifica.idTipoNotifica = reader["SYSTEM_ID"].ToString();
                        tipoNotifica.codiceNotifica = reader["VAR_CODICE_NOTIFICA"].ToString();
                        tipoNotifica.descrizioneNotifica = reader["VAR_DESCRIZIONE"].ToString();
                    }
            }

            return tipoNotifica;

        }

        /// <summary>
        /// PEC 4 Modifica Maschera Caratteri
        /// Servono alcuni dati della stato invio per aggiornare la maschera dello status durante l'inserimento delle notifiche
        /// </summary>
        /// <param name="address"></param>
        /// <param name="idProfile"></param>
        /// <returns></returns>
        public DocsPaVO.StatoInvio.StatoInvio getStatoInvioFromAddressAndProfile(string address, string idProfile)
        {
            DocsPaVO.StatoInvio.StatoInvio retval = null;
            //query cablata, da perfezionare
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPASI_STATUSMASK");
            q.setParam("idprofile", idProfile);
            q.setParam("address", address);
            string query = q.getSQL();
            logger.Debug(query);
            //string query = "select a.status_c_mask as statusmask, b.type_id as tipo from dpa_stato_invio a, documenttypes b where a.var_indirizzo='"+address+"' and a.id_profile="+idProfile+" and a.id_documenttype=b.system_id";
            using (IDataReader reader = ExecuteReader(query))
            {
                if (reader.FieldCount > 0)
                    if (reader.Read())
                    {
                        retval = new DocsPaVO.StatoInvio.StatoInvio();
                        retval.statusMask = reader["statusmask"].ToString();
                        retval.tipoCanale = reader["tipo"].ToString();

                        
                    }
            }

            return retval;
        }

        /// <summary>
        /// PEC 4 Modifica Maschera Caratteri
        /// Aggiornamento della maschera a partire da indirizzo destinatario e id documento.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="idProfile"></param>
        /// <param name="statusmask"></param>
        /// <returns></returns>
        public bool AggiornaStatusMaskFromAddressAndProfile(string address, string idProfile, string statusmask,bool eliminareEccezione)
        {
            bool retval = false;
            try
            {

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPAStatoinvio");
                string updateParam = "STATUS_C_MASK='" + statusmask + "'";
                if (eliminareEccezione)
                    updateParam += ", CHA_ANNULLATO = NULL, VAR_MOTIVO_ANNULLA = NULL";                
                q.setParam("param1", updateParam);
                q.setParam("param2", " UPPER(var_indirizzo)= UPPER('" + address + "') and id_profile=" + idProfile);
                logger.Debug("Aggiornamento maschera status: " + q.getSQL());
                retval = ExecuteNonQuery(q.getSQL());

            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Errore nell'aggiornamento della status mask. Messaggio {0}, Stacktrace {1}",ex.Message, ex.StackTrace);
            }
            return retval;
        }
    }
}
