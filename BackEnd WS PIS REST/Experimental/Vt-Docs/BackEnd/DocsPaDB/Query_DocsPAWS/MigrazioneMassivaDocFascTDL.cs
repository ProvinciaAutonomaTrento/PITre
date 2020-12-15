using System;
using System.Data;
using System.Xml;
using System.Globalization;
using System.Collections.Generic;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{
    public class MigrazioneMassivaDocFascTDL : DBProvider
    {
        #region check Ruoli
        public bool CheckMittDestTrasm(string codice_ruolo, string id_modello, string tipo)
        {
            bool res = false;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("CHECK_MITT_DEST_MODELLO");
            q.setParam("codice_ruolo", codice_ruolo);
            q.setParam("codice_modello", id_modello);
            q.setParam("tipo", tipo);
            logger.Debug("CheckMittDestTrasm " + q.getSQL());
            String p = string.Empty;
            using (IDataReader reader = ExecuteReader(q.getSQL()))
            {
                if (reader.FieldCount > 0)
                    while (reader.Read())
                    {
                        if (!String.IsNullOrEmpty(reader[0].ToString()))
                            res = true;
                        else res = false;
                    }
            }
            return res;
        }
        #endregion

        #region set note accettazione trasmissioni
        public bool SetNoteGenerali(string id_trasm, string note)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("SET_NOTE_ACCETTAZIONE_UT");
            q.setParam("system_id", id_trasm);
            q.setParam("note", note);
            String queryString = q.getSQL();
            return this.ExecuteNonQuery(queryString);
        }

        #endregion
        
        private ILog logger = LogManager.GetLogger(typeof(MigrazioneMassivaDocFascTDL));
        #region Documenti
        public string[] GetSystemIdDocumentiRWconRisalita(string codice_ruolo)
        {
            List<string> lista = new List<string>();

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("DOCUMENTI_R_W_CON_RISALITA");

            q.setParam("codice_ruolo", codice_ruolo);
            logger.Debug("GetSystemIdDocumentiRWconRisalita " + q.getSQL());

            using (IDataReader reader = ExecuteReader(q.getSQL()))
            {
                if (reader.FieldCount > 0)
                    while (reader.Read())
                        lista.Add(reader[0].ToString());
            }

            return lista.ToArray();
        }

        public string[] GetSystemIdDocumentiRconRisalita(string codice_ruolo)
        {
            List<string> lista = new List<string>();

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("DOCUMENTI_R_CON_RISALITA");
            q.setParam("codice_ruolo", codice_ruolo);
            logger.Debug("GetSystemIdDocumentiRconRisalita " + q.getSQL());

            using (IDataReader reader = ExecuteReader(q.getSQL()))
            {
                if (reader.FieldCount > 0)
                    while (reader.Read())
                        lista.Add(reader[0].ToString());
            }

            return lista.ToArray();
        }

        public string[] GetSystemIdDocumentiRWsenzaRisalita(string codice_ruolo)
        {
            List<string> lista = new List<string>();

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("DOCUMENTI_R_W_SENZA_RISALITA");
            q.setParam("codice_ruolo", codice_ruolo);
            logger.Debug("GetSystemIdDocumentiRWsenzaRisalita " + q.getSQL());

            using (IDataReader reader = ExecuteReader(q.getSQL()))
            {
                if (reader.FieldCount > 0)
                    while (reader.Read())
                        lista.Add(reader[0].ToString());
            }

            return lista.ToArray();
        }

        public string[] GetSystemIdDocumentiRsenzaRisalita(string codice_ruolo)
        {
            List<string> lista = new List<string>();

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("DOCUMENTI_R_SENZA_RISALITA");
            q.setParam("codice_ruolo", codice_ruolo);
            logger.Debug("GetSystemIdDocumentiRsenzaRisalita " + q.getSQL());

            using (IDataReader reader = ExecuteReader(q.getSQL()))
            {
                if (reader.FieldCount > 0)
                    while (reader.Read())
                        lista.Add(reader[0].ToString());
            }

            return lista.ToArray();
        }

        #endregion

        #region fascicoli
        public string GetCodiceFasc(string systemId)
        {
            string codice = string.Empty;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("GET_CODICE_FASC");
            q.setParam("system_id", systemId);
            using (IDataReader reader = ExecuteReader(q.getSQL()))
            {
                if (reader.FieldCount > 0)
                    while (reader.Read())
                        codice = reader[0].ToString();
            }
            return codice;
        }

        public string[] GetSystemIdFascicoliRWconRisalita(string codice_ruolo)
        {
            List<string> lista = new List<string>();

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("FASCICOLI_R_W_CON_RISALITA");
            q.setParam("codice_ruolo", codice_ruolo);
            logger.Debug("GetSystemIdFascicoliRWconRisalita " + q.getSQL());

            using (IDataReader reader = ExecuteReader(q.getSQL()))
            {
                if (reader.FieldCount > 0)
                    while (reader.Read())
                        lista.Add(reader[0].ToString());
            }

            return lista.ToArray();
        }

        public string[] GetSystemIdFascicoliRconRisalita(string codice_ruolo)
        {
            List<string> lista = new List<string>();

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("FASCICOLI_R_CON_RISALITA");
            q.setParam("codice_ruolo", codice_ruolo);
            logger.Debug("GetSystemIdFascicoliRconRisalita " + q.getSQL());

            using (IDataReader reader = ExecuteReader(q.getSQL()))
            {
                if (reader.FieldCount > 0)
                    while (reader.Read())
                        lista.Add(reader[0].ToString());
            }

            return lista.ToArray();
        }

        public string[] GetSystemIdFascicoliRWsenzaRisalita(string codice_ruolo)
        {
            List<string> lista = new List<string>();

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("FASCICOLI_R_W_SENZA_RISALITA");
            q.setParam("codice_ruolo", codice_ruolo);
            logger.Debug("GetSystemIdFascicoliRWsenzaRisalita " + q.getSQL());

            using (IDataReader reader = ExecuteReader(q.getSQL()))
            {
                if (reader.FieldCount > 0)
                    while (reader.Read())
                        lista.Add(reader[0].ToString());
            }

            return lista.ToArray();
        }

        public string[] GetSystemIdFascicoliRsenzaRisalita(string codice_ruolo)
        {
            List<string> lista = new List<string>();

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("FASCICOLI_R_SENZA_RISALITA");
            q.setParam("codice_ruolo", codice_ruolo);
            logger.Debug("GetSystemIdFascicoliRsenzaRisalita " + q.getSQL());

            using (IDataReader reader = ExecuteReader(q.getSQL()))
            {
                if (reader.FieldCount > 0)
                    while (reader.Read())
                        lista.Add(reader[0].ToString());
            }

            return lista.ToArray();
        }
        #endregion
    }
}
