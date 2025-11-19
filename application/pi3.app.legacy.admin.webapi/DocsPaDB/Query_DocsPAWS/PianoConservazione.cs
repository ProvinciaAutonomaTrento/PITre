// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DocsPaDB.Query_DocsPAWS;

public class PianoConservazione : DBProvider
{
    #region Const

    private static ILogger logger = Serilog.Log.ForContext(typeof(PianoConservazione));

    #endregion

    public bool InsertPianoConservazione(DocsPaVO.PianoConservazione piano)
    {
        bool retVal = false;

        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_P3_PIANO_CONSERVAZIONE");
            string idElemento = string.Empty;

            if (DBType.ToUpper().Equals("ORACLE"))
                q.setParam("systemId", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("P3_PIANO_CONSERVAZIONE"));

            q.setParam("idClassificazione", piano.IdClassificazione);
            q.setParam("codiceClassificazione", piano.CodiceClassificazione);
            q.setParam("voceProcedimento", (!string.IsNullOrEmpty(piano.VoceProcedimento) ? piano.VoceProcedimento.Replace("'", "''") : string.Empty));
            q.setParam("numeroProcedimento", (!string.IsNullOrEmpty(piano.NumeroProcedimento) ? piano.NumeroProcedimento.Replace("'", "''") : string.Empty));
            q.setParam("tipologiaFascicolo", piano.TipologiaFascicolo.Replace("'", "''"));
            q.setParam("tempoConservazione", piano.TempoConservazione.Replace("'", "''"));
            q.setParam("noteChiusuraFascicolo", (!string.IsNullOrEmpty(piano.NoteChiusuraFascicolo) ? piano.NoteChiusuraFascicolo.Replace("'", "''") : string.Empty));
            q.setParam("noteScartabilitaDocumenti", (!string.IsNullOrEmpty(piano.NoteScartabilitaDocumenti) ? piano.NoteScartabilitaDocumenti.Replace("'", "''") : string.Empty));
            q.setParam("noteDocumenti", (!string.IsNullOrEmpty(piano.NoteDocumenti) ? piano.NoteDocumenti.Replace("'", "''") : string.Empty));
            q.setParam("dataInizio", DocsPaDbManagement.Functions.Functions.GetDate());
            q.setParam("idTitolario", piano.IdTitolario);
            q.setParam("idAmm", piano.IdAmm);
            q.setParam("idRegistro", !string.IsNullOrEmpty(piano.IdRegistro)? piano.IdRegistro : "NULL" );

            string query = q.getSQL();
            logger.Debug("InsertPianoConservazione: " + query);

            if (!ExecuteNonQuery(query))
                throw new Exception("Errore nell'esecuzione della query");

            retVal = true;
        }
        catch (Exception e)
        {
            logger.Error("Errore in InsertPianoConservazione: " + e.Message);
        }

        return retVal;
    }

    public List<DocsPaVO.PianoConservazione> GetPianoConservazione(string idTitolario)
    {
        List<DocsPaVO.PianoConservazione> pianoConservazione = new List<DocsPaVO.PianoConservazione>();
        DataSet ds = new DataSet();

        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_P3_PIANO_CONSERVAZIONE");
            q.setParam("idTitolario", idTitolario);

            string query = q.getSQL();
            logger.Debug("GetPianoConservazione: " + query);

            if (this.ExecuteQuery(out ds, "pianoConservazione", query))
            {
                if (ds.Tables["pianoConservazione"] != null && ds.Tables["pianoConservazione"].Rows.Count > 0)
                {
                    DocsPaVO.PianoConservazione piano;
                    foreach (DataRow row in ds.Tables["pianoConservazione"].Rows)
                    {
                        piano = new DocsPaVO.PianoConservazione()
                        {
                            SystemId = row["SYSTEM_ID"].ToString(),
                            CodiceClassificazione = row["CODICE_CLASSIFICAZIONE"].ToString(),
                            IdClassificazione = row["ID_CLASSIFICAZIONE"].ToString(),
                            NumeroProcedimento = string.IsNullOrEmpty(row["NUMERO_PROCEDIMENTO"].ToString()) ? string.Empty : row["NUMERO_PROCEDIMENTO"].ToString(),
                            TipologiaFascicolo = row["TIPOLOGIA_FASCICOLO"].ToString(),
                            TempoConservazione = row["TEMPO_CONSERVAZIONE"].ToString(),
                            NoteChiusuraFascicolo = row["NOTE_CHIUSURA_FASCICOLO"].ToString(),
                            NoteScartabilitaDocumenti = row["NOTE_SCARTABILITA_DOC"].ToString(),
                            NoteDocumenti = row["NOTE_DOCUMENTI"].ToString()
                        };

                        pianoConservazione.Add(piano);
                    }
                }
            }
            else
            {
                throw new Exception("Errore durante l'estrazione del piano di conservazione: " + query);
            }

        }
        catch (Exception e)
        {
            logger.Error("Errore in GetPianoConservazione: " + e.Message);
        }

        return pianoConservazione;
    }

    public bool IsPianoConservazioneAcquisito(string idTitolario, string idRegistro)
    {
        bool retValue = false;
        DataSet ds = new DataSet();

        try
        {
            string registro = string.Empty;
            if (!string.IsNullOrEmpty(idRegistro))
                registro = " AND ID_REGISTRO=" + idRegistro;
            else
                registro = "AND ID_REGISTRO IS NULL";

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_P3_PIANO_CONSERVAZIONE_EXISTS");
            q.setParam("idTitolario", idTitolario);
            q.setParam("registro", registro);

            string query = q.getSQL();
            logger.Debug("IsPianoConservazioneAcquisito: " + query);

            if (this.ExecuteQuery(out ds, "pianoConservazione", query))
            {
                if(ds.Tables["pianoConservazione"] != null && ds.Tables["pianoConservazione"].Rows.Count > 0)
                    retValue = true;
            }
            else
            {
                throw new Exception("Errore durante l'estrazione del piano di conservazione: " + query);
            }
        }
        catch (Exception e)
        {
            logger.Error("Errore in IsPianoConservazioneAcquisito: " + e.Message);
        }
        return retValue;
    }

    public bool StoricizzaPianoConservazione(string idTitolario, string idRegistro)
    {
        bool retValue = true;
        string query = string.Empty;
        try
        {
            string registro = string.Empty;
            if (!string.IsNullOrEmpty(idRegistro))
                registro = " AND ID_REGISTRO=" + idRegistro;
            else
                registro = "AND ID_REGISTRO IS NULL";

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_P3_PIANO_CONSERVAZIONE_DTA_FINE");
            q.setParam("idTitolario", idTitolario);
            q.setParam("registro", registro);
            q.setParam("dataFine", DocsPaDbManagement.Functions.Functions.GetDate());

            query = q.getSQL();
            logger.Debug("StoricizzaPianoConservazione QUERY: " + query);
            int rowsAffected = 0;
            if (!ExecuteNonQuery(query, out rowsAffected))
            {
                throw new Exception("Errore durante la storicizzazione del piano di conservazione: " + query);
            }

        }
        catch(Exception e)
        {
            retValue = false;
            logger.Error("Errore in StoricizzaPianoConservazione: " + e.Message);
        }

        return retValue;
    }

    public List<DocsPaVO.PianoConservazione> GetPianoConservazioneByIdClassificazione(string idClassificazione)
    {
        List<DocsPaVO.PianoConservazione> pianoConservazione = new List<DocsPaVO.PianoConservazione>();
        DataSet ds = new DataSet();

        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_P3_PIANO_CONSERVAZIONE_BY_ID_CLASSIFICAZIONE");
            q.setParam("idClassificazione", idClassificazione);

            string query = q.getSQL();
            logger.Debug("GetPianoConservazioneByIdClassificazione: " + query);

            if (this.ExecuteQuery(out ds, "pianoConservazione", query))
            {
                if (ds.Tables["pianoConservazione"] != null && ds.Tables["pianoConservazione"].Rows.Count > 0)
                {
                    DocsPaVO.PianoConservazione piano;
                    foreach (DataRow row in ds.Tables["pianoConservazione"].Rows)
                    {
                        piano = new DocsPaVO.PianoConservazione()
                        {
                            SystemId = row["SYSTEM_ID"].ToString(),
                            IdClassificazione = row["ID_CLASSIFICAZIONE"].ToString(),
                            NumeroProcedimento = string.IsNullOrEmpty(row["NUMERO_PROCEDIMENTO"].ToString()) ? string.Empty : row["NUMERO_PROCEDIMENTO"].ToString(),
                            TipologiaFascicolo = row["TIPOLOGIA_FASCICOLO"].ToString(),
                            TempoConservazione = row["TEMPO_CONSERVAZIONE"].ToString()
                        };

                        pianoConservazione.Add(piano);
                    }
                }
            }
            else
            {
                throw new Exception("Errore durante l'estrazione del piano di conservazione: " + query);
            }

        }
        catch (Exception e)
        {
            logger.Error("Errore in GetPianoConservazioneByIdClassificazione: " + e.Message);
        }

        return pianoConservazione;
    }

    public bool UpdatePianoConservazioneById(DocsPaVO.PianoConservazione pianoConservazione)
    {
        bool retValue = true;
        string query = string.Empty;
        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_P3_PIANO_CONSERVAZIONE_BY_ID");
            q.setParam("systemId", pianoConservazione.SystemId);
            q.setParam("numeroProcedimento", (!string.IsNullOrEmpty(pianoConservazione.NumeroProcedimento) ? pianoConservazione.NumeroProcedimento : string.Empty));
            q.setParam("tipologiaFascicolo", pianoConservazione.TipologiaFascicolo.Replace("'", "''"));
            q.setParam("tempoConservazione", pianoConservazione.TempoConservazione.Replace("'", "''"));

            query = q.getSQL();
            logger.Debug("UpdatePianoConservazioneById QUERY: " + query);
            int rowsAffected = 0;
            if (!ExecuteNonQuery(query, out rowsAffected))
            {
                throw new Exception("Errore durante la modifica del piano di conservazione: " + query);
            }

        }
        catch (Exception e)
        {
            retValue = false;
            logger.Error("Errore in UpdatePianoConservazioneById: " + e.Message);
        }

        return retValue;
    }

    public bool DeletePianoConservazioneById(string idPianoConservazione)
    {
        bool retValue = true;
        string query = string.Empty;
        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_P3_PIANO_CONSERVAZIONE_BY_ID");
            q.setParam("idPianoConservazione", idPianoConservazione);
            q.setParam("dataFine", DocsPaDbManagement.Functions.Functions.GetDate());

            query = q.getSQL();
            logger.Debug("DeletePianoConservazioneById QUERY: " + query);
            int rowsAffected = 0;
            if (!ExecuteNonQuery(query, out rowsAffected))
            {
                throw new Exception("Errore durante la storicizzazione del piano di conservazione: " + query);
            }

        }
        catch (Exception e)
        {
            retValue = false;
            logger.Error("Errore in DeletePianoConservazioneById: " + e.Message);
        }

        return retValue;
    }

    public DocsPaVO.PianoConservazione GetPianoConservazioneById(string idPianoConservazione)
    {
       DocsPaVO.PianoConservazione pianoConservazione = null;
        DataSet ds = new DataSet();

        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_P3_PIANO_CONSERVAZIONE_BY_ID");
            q.setParam("idPianoConservazione", idPianoConservazione);

            string query = q.getSQL();
            logger.Debug("GetPianoConservazioneById: " + query);

            if (this.ExecuteQuery(out ds, "pianoConservazione", query))
            {
                if (ds.Tables["pianoConservazione"] != null && ds.Tables["pianoConservazione"].Rows.Count > 0)
                {
                    DataRow row = ds.Tables["pianoConservazione"].Rows[0];
                    pianoConservazione = new DocsPaVO.PianoConservazione()
                        {
                            SystemId = row["SYSTEM_ID"].ToString(),
                            CodiceClassificazione = row["CODICE_CLASSIFICAZIONE"].ToString(),
                            VoceProcedimento = row["VOCE_PROCEDIMENTO"].ToString(),
                            IdClassificazione = row["ID_CLASSIFICAZIONE"].ToString(),
                            NumeroProcedimento = string.IsNullOrEmpty(row["NUMERO_PROCEDIMENTO"].ToString()) ? string.Empty : row["NUMERO_PROCEDIMENTO"].ToString(),
                            TipologiaFascicolo = row["TIPOLOGIA_FASCICOLO"].ToString(),
                            TempoConservazione = row["TEMPO_CONSERVAZIONE"].ToString(),
                            NoteChiusuraFascicolo = row["NOTE_CHIUSURA_FASCICOLO"].ToString(),
                            NoteScartabilitaDocumenti = row["NOTE_SCARTABILITA_DOC"].ToString(),
                            NoteDocumenti = row["NOTE_DOCUMENTI"].ToString()
                    };
                }
            }
            else
            {
                throw new Exception("Errore durante l'estrazione del piano di conservazione: " + query);
            }

        }
        catch (Exception e)
        {
            logger.Error("Errore in GetPianoConservazioneById: " + e.Message);
        }

        return pianoConservazione;
    }

    public bool InsertPianoConservazioneTipoFasc(string idPianoConservazione, string idTipoFasc, string idAmministrazione)
    {
        bool retVal = false;

        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_P3_PIANO_CONS_TIPO_FASC");
            string idElemento = string.Empty;

            if (DBType.ToUpper().Equals("ORACLE"))
                q.setParam("systemId", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("P3_PIANO_CONS_TIPO_FASC"));

            q.setParam("idPianoConservazione", idPianoConservazione);
            q.setParam("idTipoFasc", idTipoFasc);
            q.setParam("idAmm", idAmministrazione);

            string query = q.getSQL();
            logger.Debug("InsertPianoConservazioneTipoFasc: " + query);

            if (!ExecuteNonQuery(query))
                throw new Exception("Errore nell'esecuzione della query");

            retVal = true;
        }
        catch (Exception e)
        {
            logger.Error("Errore in InsertPianoConservazioneTipoFasc: " + e.Message);
        }

        return retVal;
    }

    public bool InsertPianoConservazioneTipoAtto(string idPianoConservazione, string idTipoAtto, string idAmministrazione)
    {
        bool retVal = false;

        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_P3_PIANO_CONS_TIPO_ATTO");
            string idElemento = string.Empty;

            if (DBType.ToUpper().Equals("ORACLE"))
                q.setParam("systemId", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("P3_PIANO_CONS_TIPO_ATTO"));

            q.setParam("idPianoConservazione", idPianoConservazione);
            q.setParam("idTipoAtto", idTipoAtto);
            q.setParam("idAmm", idAmministrazione);

            string query = q.getSQL();
            logger.Debug("InsertPianoConservazioneTipoAtto: " + query);

            if (!ExecuteNonQuery(query))
                throw new Exception("Errore nell'esecuzione della query");

            retVal = true;
        }
        catch (Exception e)
        {
            logger.Error("Errore in InsertPianoConservazioneTipoAtto: " + e.Message);
        }

        return retVal;
    }

    public DocsPaVO.PianoConservazione GetPianoConservazioneByIdTipoFasc(string idTipoFasc)
    {
        DocsPaVO.PianoConservazione pianoConservazione = null;
        DataSet ds = new DataSet();

        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_P3_PIANO_CONS_TIPO_FASC_BY_ID_FASC");
            q.setParam("idTipoFasc", idTipoFasc);

            string query = q.getSQL();
            logger.Debug("GetPianoConservazioneByIdTipoFasc: " + query);

            if (this.ExecuteQuery(out ds, "pianoConservazione", query))
            {
                if (ds.Tables["pianoConservazione"] != null && ds.Tables["pianoConservazione"].Rows.Count > 0)
                {
                    DataRow row = ds.Tables["pianoConservazione"].Rows[0];
                    pianoConservazione = new DocsPaVO.PianoConservazione()
                    {
                        SystemId = row["SYSTEM_ID"].ToString(),
                        IdClassificazione = row["ID_CLASSIFICAZIONE"].ToString(),
                        NumeroProcedimento = string.IsNullOrEmpty(row["NUMERO_PROCEDIMENTO"].ToString()) ? string.Empty : row["NUMERO_PROCEDIMENTO"].ToString(),
                        TipologiaFascicolo = row["TIPOLOGIA_FASCICOLO"].ToString(),
                        TempoConservazione = row["TEMPO_CONSERVAZIONE"].ToString(),
                        CodiceClassificazione = row["CODICE_CLASSIFICAZIONE"].ToString(),
                        IdRegistro = string.IsNullOrEmpty(row["ID_REGISTRO"].ToString()) ? "0" : row["ID_REGISTRO"].ToString()
                    };
                }
            }
            else
            {
                throw new Exception("Errore durante l'estrazione del piano di conservazione: " + query);
            }

        }
        catch (Exception e)
        {
            logger.Error("Errore in GetPianoConservazioneByIdTipoFasc: " + e.Message);
        }

        return pianoConservazione;
    }

    public bool UpdatePianoConservazioneTipoFasc(string idTipoFasc, string idPianoConservazione)
    {
        logger.Debug("Inizio Metodo UpdatePianoConservazioneTipoFasc");
        bool result = true;
        string query;
        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_P3_PIANO_CONS_TIPO_FASC");
            q.setParam("idTipoFasc", idTipoFasc);
            q.setParam("idPianoConservazione", idPianoConservazione);
            query = q.getSQL();
            logger.Debug("UpdatePianoConservazioneTipoFasc: " + query);
            int rowsAffected = 0;
            if (!ExecuteNonQuery(query, out rowsAffected))
            {
                throw new Exception("Errore durante l'aggiornamento del piano di conservazione: " + query);
            }
        }
        catch (Exception ex)
        {
            logger.Error("Errore nel Metodo UpdatePianoConservazioneTipoFasc " + ex.Message);
            return false;
        }
        logger.Debug("Fine Metodo UpdatePianoConservazioneTipoFasc");
        return result;
    }

    public bool DeletePianoConservazioneTipoFasc(string idTipoFasc)
    {
        bool retValue = true;
        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_P3_PIANO_CONS_TIPO_FASC");
            q.setParam("idTipoFasc", idTipoFasc);
            string query = q.getSQL();
            logger.Debug("DeletePianoConservazioneTipoFasc: " + query);
            if (!ExecuteNonQuery(query))
            {
                throw new Exception("Errore durante la rimozione del piano di conservazione: " + query);
            }
        }
        catch (Exception e)
        {
            logger.Error("Errore in DeletePianoConservazioneTipoFasc " + e.Message);
            retValue = false;
        }
        return retValue;
    }

    public DocsPaVO.PianoConservazione GetPianoConservazioneByIdTipoAtto(string idTipoAtto)
    {
        DocsPaVO.PianoConservazione pianoConservazione = null;
        DataSet ds = new DataSet();

        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_P3_PIANO_CONS_TIPO_ATTO_BY_ID_ATTO");
            q.setParam("idTipoAtto", idTipoAtto);

            string query = q.getSQL();
            logger.Debug("GetPianoConservazioneByIdTipoAtto: " + query);

            if (this.ExecuteQuery(out ds, "pianoConservazione", query))
            {
                if (ds.Tables["pianoConservazione"] != null && ds.Tables["pianoConservazione"].Rows.Count > 0)
                {
                    DataRow row = ds.Tables["pianoConservazione"].Rows[0];
                    pianoConservazione = new DocsPaVO.PianoConservazione()
                    {
                        SystemId = row["SYSTEM_ID"].ToString(),
                        IdClassificazione = row["ID_CLASSIFICAZIONE"].ToString(),
                        NumeroProcedimento = string.IsNullOrEmpty(row["NUMERO_PROCEDIMENTO"].ToString()) ? string.Empty : row["NUMERO_PROCEDIMENTO"].ToString(),
                        TipologiaFascicolo = row["TIPOLOGIA_FASCICOLO"].ToString(),
                        TempoConservazione = row["TEMPO_CONSERVAZIONE"].ToString(),
                        CodiceClassificazione = row["CODICE_CLASSIFICAZIONE"].ToString(),
                        IdRegistro = string.IsNullOrEmpty(row["ID_REGISTRO"].ToString()) ? "0" : row["ID_REGISTRO"].ToString()
                    };
                }
            }
            else
            {
                throw new Exception("Errore durante l'estrazione del piano di conservazione: " + query);
            }

        }
        catch (Exception e)
        {
            logger.Error("Errore in GetPianoConservazioneByIdTipoAtto: " + e.Message);
        }

        return pianoConservazione;
    }

    public bool UpdatePianoConservazioneTipoAtto(string idTipoAtto, string idPianoConservazione)
    {
        logger.Debug("Inizio Metodo UpdatePianoConservazioneTipoFasc");
        bool result = true;
        string query;
        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_P3_PIANO_CONS_TIPO_ATTO");
            q.setParam("idTipoAtto", idTipoAtto);
            q.setParam("idPianoConservazione", idPianoConservazione);
            query = q.getSQL();
            logger.Debug("UpdatePianoConservazioneTipoAtto: " + query);
            int rowsAffected = 0;
            if (!ExecuteNonQuery(query, out rowsAffected))
            {
                throw new Exception("Errore durante l'aggiornamento del piano di conservazione: " + query);
            }
        }
        catch (Exception ex)
        {
            logger.Error("Errore nel Metodo UpdatePianoConservazioneTipoAtto " + ex.Message);
            return false;
        }
        logger.Debug("Fine Metodo UpdatePianoConservazioneTipoAtto");
        return result;
    }

    public bool DeletePianoConservazioneTipoAtto(string idTipoAtto)
    {
        bool retValue = true;
        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_P3_PIANO_CONS_TIPO_ATTO");
            q.setParam("idTipoAtto", idTipoAtto);
            string query = q.getSQL();
            logger.Debug("DeletePianoConservazioneTipoAtto: " + query);
            if (!ExecuteNonQuery(query))
            {
                throw new Exception("Errore durante la rimozione del piano di conservazione: " + query);
            }
        }
        catch (Exception e)
        {
            logger.Error("Errore in DeletePianoConservazioneTipoAtto " + e.Message);
            retValue = false;
        }
        return retValue;
    }

    public List<string> GetListaIdTipiFascByIdPianoConservazione(string idPianoConservazione)
    {
        List<string> listIdTipiFasc = new List<string>();
        DataSet ds = new DataSet();

        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_P3_PIANO_CONS_TIPO_FASC_BY_ID_PIANO");
            q.setParam("idPianoConservazione", idPianoConservazione);

            string query = q.getSQL();
            logger.Debug("GetListaIdTipiFascByIdPianoConservazione: " + query);

            if (this.ExecuteQuery(out ds, "pianoConservazione", query))
            {
                if (ds.Tables["pianoConservazione"] != null && ds.Tables["pianoConservazione"].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables["pianoConservazione"].Rows)
                        listIdTipiFasc.Add(row["ID_TIPO_FASC"].ToString());
                }
            }
            else
            {
                throw new Exception("Errore durante l'estrazione dei tipi fascicoli: " + query);
            }

        }
        catch (Exception e)
        {
            logger.Error("Errore in GetListaIdTipiFascByIdPianoConservazione: " + e.Message);
        }

        return listIdTipiFasc;
    }

    public string GetMaxTempoConservazioneDocumento(string idProfile)
    {
        string maxTempoConservazione = "";
        DataSet ds = new DataSet();

        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_MAX_TEMPO_CONSERVAZIONE_DOCUMENTO");
            q.setParam("idProfile", idProfile);

            string query = q.getSQL();
            logger.Debug("GetMaxTempoConservazioneDocumento: " + query);

            if (this.ExecuteQuery(out ds, "maxTempoConservazione", query))
            {
                if (ds.Tables["maxTempoConservazione"] != null && ds.Tables["maxTempoConservazione"].Rows.Count > 0)
                    maxTempoConservazione = ds.Tables["maxTempoConservazione"].Rows[0]["TEMPO_CONSERVAZIONE"].ToString();
            }
            else
            {
                throw new Exception("Errore durante l'estrazione del massimo tempo di conservazione: " + query);
            }

        }
        catch (Exception e)
        {
            logger.Error("Errore in GetMaxTempoConservazioneDocumento: " + e.Message);
        }
        return maxTempoConservazione;
    }

    public string GetTempoConservazioneNumber(string tempoConservazione)
    {
        string maxTempoConservazione = "";
        DataSet ds = new DataSet();

        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_TEMPO_CONSERVAZIONE_NUMBER");
            q.setParam("tempoConservazione", tempoConservazione);

            string query = q.getSQL();
            logger.Debug("GetTempoConservazioneNumber: " + query);

            if (this.ExecuteQuery(out ds, "maxTempoConservazione", query))
            {
                if (ds.Tables["maxTempoConservazione"] != null && ds.Tables["maxTempoConservazione"].Rows.Count > 0)
                    maxTempoConservazione = ds.Tables["maxTempoConservazione"].Rows[0]["TEMPO_CONSERVAZIONE_IN_ANNI"].ToString();
            }
            else
            {
                throw new Exception("Errore durante l'estrazione del massimo tempo di conservazione: " + query);
            }

        }
        catch (Exception e)
        {
            logger.Error("Errore in GetTempoConservazioneNumber: " + e.Message);
        }
        return maxTempoConservazione;
    }

    public string GetMaxTempoConservazioneNumberDocumento(string idProfile)
    {
        string maxTempoConservazione = "";
        DataSet ds = new DataSet();

        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_MAX_TEMPO_CONSERVAZIONE_NUMBER_DOCUMENTO");
            q.setParam("idProfile", idProfile);

            string query = q.getSQL();
            logger.Debug("GetMaxTempoConservazioneNumberDocumento: " + query);

            if (this.ExecuteQuery(out ds, "maxTempoConservazione", query))
            {
                if (ds.Tables["maxTempoConservazione"] != null && ds.Tables["maxTempoConservazione"].Rows.Count > 0)
                    maxTempoConservazione = ds.Tables["maxTempoConservazione"].Rows[0]["TEMPO_CONSERVAZIONE_IN_ANNI"].ToString();
            }
            else
            {
                throw new Exception("Errore durante l'estrazione del massimo tempo di conservazione: " + query);
            }

        }
        catch (Exception e)
        {
            logger.Error("Errore in GetMaxTempoConservazioneNumberDocumento: " + e.Message);
        }
        return maxTempoConservazione;
    }

    public int SearchCountArchivePlansRest(Dictionary<string,string> filters)
    {
        int retval = 0;
        string retquery;
        try
        {
            string condizione = ExtractFiltersArchivePlans(filters);
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_COUNT_ARCHIVE_PLANS_REST");
            q.setParam("condizione", condizione);
            string query = q.getSQL();
            logger.Debug("SearchCountArchivePlansRest: " + query);

            if (this.ExecuteScalar(out retquery, query))
            {
                retval = Int32.Parse(retquery);
            }
            else
            {
                throw new Exception("Errore durante la ricerca del numero di piani archiviazione da REST: " + query);
            }


        }
        catch (Exception ex)
        {
            logger.Error("Errore in SearchCountArchivePlansRest: " + ex);
        }
        return retval;
    }

    public List<ArchivePlanREST> SearchArchivePlansRest(Dictionary<string, string> filters, string pagenum, string pagesize)
    {
        List<ArchivePlanREST> retval = null;
        
        try
        {
            // TODO
            string condizione = ExtractFiltersArchivePlans(filters);
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SEARCH_ARCHIVE_PLANS_REST");
            q.setParam("condizione", condizione);
            string paginazione = string.Format(" offset ({1}*{0}) rows fetch next {1} rows only",pagenum,pagesize);
            q.setParam("paginazione", paginazione);
            string query = q.getSQL();
            logger.Debug("SearchArchivePlansRest: " + query);
            DataSet ds = new DataSet();
            if(this.ExecuteQuery(out ds,"archiveplans", query))
            {
                if (ds.Tables["archiveplans"] != null && ds.Tables["archiveplans"].Rows.Count > 0)
                {
                    retval = new List<ArchivePlanREST>();
                    foreach (DataRow row in ds.Tables["archiveplans"].Rows)
                    {
                        retval.Add(new ArchivePlanREST()
                        {
                            IdAmm = row["ID_AMM"].ToString(),
                            IdClassificazione = row["ID_CLASSIFICAZIONE"].ToString(),
                            CodiceClassificazione = row["CODICE_CLASSIFICAZIONE"].ToString(),
                            CodiceRegistro = row["codice_registro"].ToString(),
                            DescrizioneRegistro = row["desc_registro"].ToString(),
                            DescTitolario = row["desc_titolario"].ToString(),
                            IdRegistro = row["id_registro"].ToString(),
                            IdTitolario = row["id_titolario"].ToString(),
                            NoteChiusuraFascicolo = row["NOTE_CHIUSURA_FASCICOLO"].ToString(),
                            NoteDocumenti = row["NOTE_DOCUMENTI"].ToString(),
                            NoteScartabilitaDocumenti = row["NOTE_SCARTABILITA_DOC"].ToString(),
                            NumeroProcedimento = row["NUMERO_PROCEDIMENTO"].ToString(),
                            RegistroIsRF = row["registroisrf"].ToString(),
                            StatoRegistro = row["stato_registro"].ToString(),
                            SystemId = row["SYSTEM_ID"].ToString(),
                            TempoConservazione = row["TEMPO_CONSERVAZIONE"].ToString(),
                            TipologiaFascicolo = row["TIPOLOGIA_FASCICOLO"].ToString(),
                            TitolarioAttivo = row["stato_titolario"].ToString(),
                            VoceProcedimento = row["VOCE_PROCEDIMENTO"].ToString(),
                            TempoConservazioneAnni = row["TEMPO_CONSERVAZIONE_IN_ANNI"].ToString()
                        });
                    }
                }
            }
            else
            {
                throw new Exception("Errore durante la ricerca dei piani archiviazione da REST: " + query);
            }
        }
        catch (Exception ex)
        {
            logger.Error("Errore in SearchArchivePlansRest: " + ex);
        }
        return retval;
    }

    public ArchivePlanREST GetArchivePlanRESTById(string id)
    {
        ArchivePlanREST retval = null;
        try
        {
            // TODO
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ARCHIVE_PLAN_BY_ID_REST");
            q.setParam("idPiano", id);
            string query = q.getSQL();
            logger.Debug("GetArchivePlanRESTById: " + query);
            DataSet ds = new DataSet();
            if (this.ExecuteQuery(out ds, "archiveplans", query))
            {
                if (ds.Tables["archiveplans"] != null && ds.Tables["archiveplans"].Rows.Count > 0)
                {
                    DataRow row = ds.Tables["archiveplans"].Rows[0];

                    retval = new ArchivePlanREST()
                    {
                        IdAmm = row["ID_AMM"].ToString(),
                        IdClassificazione = row["ID_CLASSIFICAZIONE"].ToString(),
                        CodiceClassificazione = row["CODICE_CLASSIFICAZIONE"].ToString(),
                        CodiceRegistro = row["codice_registro"].ToString(),
                        DescrizioneRegistro = row["desc_registro"].ToString(),
                        DescTitolario = row["desc_titolario"].ToString(),
                        IdRegistro = row["id_registro"].ToString(),
                        IdTitolario = row["id_titolario"].ToString(),
                        NoteChiusuraFascicolo = row["NOTE_CHIUSURA_FASCICOLO"].ToString(),
                        NoteDocumenti = row["NOTE_DOCUMENTI"].ToString(),
                        NoteScartabilitaDocumenti = row["NOTE_SCARTABILITA_DOC"].ToString(),
                        NumeroProcedimento = row["NUMERO_PROCEDIMENTO"].ToString(),
                        RegistroIsRF = row["registroisrf"].ToString(),
                        StatoRegistro = row["stato_registro"].ToString(),
                        SystemId = row["SYSTEM_ID"].ToString(),
                        TempoConservazione = row["TEMPO_CONSERVAZIONE"].ToString(),
                        TipologiaFascicolo = row["TIPOLOGIA_FASCICOLO"].ToString(),
                        TitolarioAttivo = row["stato_titolario"].ToString(),
                        VoceProcedimento = row["VOCE_PROCEDIMENTO"].ToString(),
                        TempoConservazioneAnni = row["TEMPO_CONSERVAZIONE_IN_ANNI"].ToString()
                    };
                }
            }
            else
            {
                throw new Exception("Errore durante il get del piano archiviazione da REST: " + query);
            }
        }
        catch (Exception ex)
        {
            logger.Error("Errore in GetArchivePlanRESTById: " + ex);
        }

        return retval;
    }

    public string ExtractFiltersArchivePlans(Dictionary<string,string> filters)
    {
        string retval = "";
        StringBuilder condizione = new StringBuilder();
        try
        {
            if(filters != null && filters.Count > 0)
            {
                foreach(var f in filters)
                {                        
                    switch (f.Key)
                    {
                        case "CLASSIFICATION_NODE_CODE":
                            condizione.Append(string.Format(" and a.CODICE_CLASSIFICAZIONE = '{0}'",f.Value));
                            break;
                        case "CLASSIFICATION_NODE_ID":
                            condizione.Append(string.Format(" and a.ID_CLASSIFICAZIONE = {0}", f.Value));
                            break;
                        case "ADMINISTRATION_ID":
                            condizione.Append(string.Format(" and a.ID_AMM = {0}", f.Value));
                            break;
                        case "DESCRIPTION":
                            condizione.Append(string.Format(" and lower(a.TIPOLOGIA_FASCICOLO) like '%{0}%'", f.Value.ToLowerInvariant()));
                            break;
                        case "CLASSIFICATION_SCHEME_ID":
                            condizione.Append(string.Format(" and a.id_titolario = {0}", f.Value));
                            break;
                        case "REGISTER_ID":
                            condizione.Append(string.Format(" and a.id_registro = {0}", f.Value));
                            break;
                        case "REGISTER_CODE":
                            condizione.Append(string.Format(" and b.var_codice = '{0}'", f.Value));
                            break;
                        case "ID":
                            condizione.Append(string.Format(" and a.SYSTEM_ID = {0}", f.Value));
                            break;
                        case "DOCUMENT_TEMPLATE_ID":
                            condizione.Append(string.Format(" and a.system_id in (select id_piano_conservazione from P3_PIANO_CONS_TIPO_ATTO where id_tipo_atto = {0})", f.Value));
                            break;
                        case "PROJECT_TEMPLATE_ID":
                            condizione.Append(string.Format(" and a.system_id in (select id_piano_conservazione from P3_PIANO_CONS_TIPO_FASC where id_tipo_fasc = {0})", f.Value));
                            break;
                        default:
                            break;
                    }
                }
                retval= condizione.ToString();
            }

        }catch(Exception ex)
        {
            logger.Error("Errore in ExtractFiltersArchivePlans: " + ex);
        }

        return retval;
    }

    public List<PianoCons_IntegrPIS> SearchPianoCons_IntegrPIS(Dictionary<string, string> filters, string pagenum, string pagesize, string orderColumn, string orderDirection, string idAmministrazione)
    {
        List<PianoCons_IntegrPIS> retval = null;

        try
        {
            // TODO
            string condizione = ExtractFiltersPianoCons_IntegrPIS(filters);
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_P3_PIS_APPS_ARCHIVEPLANS");
            q.setParam("idamministrazione", idAmministrazione);
            q.setParam("condition", condizione);
            string paginazione = "";
            if(!string.IsNullOrEmpty(pagenum) || !string.IsNullOrEmpty(pagesize) || !string.IsNullOrEmpty(orderColumn) || !string.IsNullOrEmpty(orderDirection))
            {
                if (string.IsNullOrWhiteSpace(orderColumn)) orderColumn = "TIPOLOGIA_FASCICOLO";
                if (string.IsNullOrEmpty(orderDirection)) orderDirection = "asc";
                if (string.IsNullOrEmpty(pagenum)) pagenum = "0";
                if (string.IsNullOrEmpty(pagesize)) pagesize = "10";

                paginazione = string.Format(" order by {0} {1} offset ({2} * {3}) rows fetch next {3} rows only ",orderColumn,orderDirection,pagenum, pagesize);
            }

            q.setParam("paginazione", paginazione);
            string query = q.getSQL();
            logger.Debug("SearchPianoCons_IntegrPIS: " + query);
            DataSet ds = new DataSet();
            if (this.ExecuteQuery(out ds, "archiveplans", query))
            {
                if (ds.Tables["archiveplans"] != null && ds.Tables["archiveplans"].Rows.Count > 0)
                {
                    retval = new List<PianoCons_IntegrPIS>();
                    foreach (DataRow row in ds.Tables["archiveplans"].Rows)
                    {
                        retval.Add(new PianoCons_IntegrPIS()
                        {
                            SystemId = row["SYSTEM_ID"].ToString(),
                            IdIntegrazione = row["ID_INTEGRAZIONE"].ToString(),
                            IdAmministrazione = row["ID_AMM"].ToString(),
                            IdRegistro= row["ID_REGISTRO"].ToString(),
                            CodRegistro=row["COD_REGISTRO"].ToString(),
                            DescRegistro=row["VAR_DESC_REGISTRO"].ToString(),
                            IdPeopleIntegrazione = row["ID_PEOPLE"].ToString(),
                            UserIdIntegrazione = row["user_id"].ToString(),
                            UtenteIntegrazione = row["full_name"].ToString(),
                            IdGruppoIntegrazione = row["ID_GRUPPO"].ToString(),
                            CodRuoloIntegrazione = row["group_id"].ToString(),
                            DescRuoloIntegrazione = row["group_name"].ToString(),
                            CodiceClassifica = row["COD_CLASSIFICA"].ToString(),
                            CodeApplication = row["CODE_APPLICATION"].ToString(),
                            DescIntegrazione = row["DESC_INTEGRAZIONE"].ToString(),
                            IdPianoConservazione = row["ID_PIANO_CONSERVAZIONE"].ToString(),
                            DescPianoConservazione = row["TIPOLOGIA_FASCICOLO"].ToString(),
                            TempoConservazione = row["TEMPO_CONSERVAZIONE"].ToString(),
                            TempoConservazioneInMesi = row["tempo_conservazione_in_anni"].ToString(),
                            IdTipoDoc = row["ID_TIPO_DOC"].ToString(),
                            DescTipoDoc = row["var_desc_atto"].ToString(),
                            IdTipoFasc = row["ID_TIPO_FASC"].ToString(),
                            DescTipoFasc = row["var_desc_fasc"].ToString(),
                            DataInserimento = (DateTime)row["DTA_INS"]

                        });
                    }
                }
            }
            else
            {
                throw new Exception("Errore durante la ricerca dei piani archiviazione riguardanti le integrazioni: " + query);
            }
        }
        catch (Exception ex)
        {
            logger.Error("Errore in SearchPianoCons_IntegrPIS: " + ex);
        }
        return retval;
    }

    public string ExtractFiltersPianoCons_IntegrPIS(Dictionary<string, string> filters)
    {
        string retval = "";
        StringBuilder condizione = new StringBuilder();
        string parametriIntegrazione = "";

        try
        {
            if (filters != null && filters.Count > 0)
            {
                foreach (var f in filters)
                {
                    switch (f.Key)
                    {
                        case "ID":
                            condizione.Append(string.Format(" and a.SYSTEM_ID = {0}", f.Value));
                            break;
                        case "CODICE_CLASSIFICA":
                            condizione.Append(string.Format(" and lower(a.COD_CLASSIFICA) = '{0}'", f.Value.ToLowerInvariant()));
                            break;
                        case "ID_PEOPLE":
                            //condizione.Append(string.Format(" and a.ID_PEOPLE = {0}", f.Value.ToLowerInvariant()));
                            if (!string.IsNullOrWhiteSpace(parametriIntegrazione)) parametriIntegrazione += " OR";
                            parametriIntegrazione += " a.ID_PEOPLE = " + f.Value;
                            break;
                        case "ID_GRUPPO":
                            //condizione.Append(string.Format(" and a.ID_GRUPPO = {0}", f.Value));
                            if (!string.IsNullOrWhiteSpace(parametriIntegrazione)) parametriIntegrazione += " OR";
                            parametriIntegrazione += " a.ID_GRUPPO = " + f.Value;
                            break;
                        case "CODE_APPLICATION":
                            //condizione.Append(string.Format(" and lower(a.CODE_APPLICATION) = '{0}'", f.Value.ToLowerInvariant()));
                            if (!string.IsNullOrWhiteSpace(parametriIntegrazione)) parametriIntegrazione += " OR";
                            parametriIntegrazione += " a.CODE_APPLICATION = '" + f.Value+"'";
                            break;
                        case "ID_TIPO_DOC":
                            condizione.Append(string.Format(" and a.ID_TIPO_DOC = {0}", f.Value));
                            break;
                        case "ID_TIPO_FASC":
                            condizione.Append(string.Format(" and a.ID_TIPO_FASC = {0}", f.Value));
                            break;
                        case "ID_REGISTRO":
                            condizione.Append(string.Format(" and a.ID_REGISTRO = {0}", f.Value));
                            break;
                        default:
                            break;
                    }
                }
                if (!string.IsNullOrWhiteSpace(parametriIntegrazione)) condizione.Append(" AND ( "+parametriIntegrazione+" ) ");

                retval = condizione.ToString();
            }

        }
        catch (Exception ex)
        {
            logger.Error("Errore in ExtractFiltersArchivePlans: " + ex);
        }

        return retval;
    }

    public bool DeletePianoCons_IntegrPIS(string id)
    {
        bool retValue = true;
        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_P3_PIS_APPS_ARCHIVEPLANS");
            q.setParam("idriga", id);
            string query = q.getSQL();
            logger.Debug("DeletePianoCons_IntegrPIS: " + query);
            if (!ExecuteNonQuery(query))
            {
                throw new Exception("Errore durante la rimozione del piano di conservazione riguardante le integrazioni: " + query);
            }
        }
        catch (Exception e)
        {
            logger.Error("Errore in DeletePianoCons_IntegrPIS " + e.Message);
            retValue = false;
        }
        return retValue;
    }

    public bool InsertPianoCons_IntegrPIS(DocsPaVO.PianoCons_IntegrPIS input)
    {
        bool retVal = false;

        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_P3_PIS_APPS_ARCHIVEPLANS");
                            
            q.setParam("id_integrazione", !string.IsNullOrWhiteSpace(input.IdIntegrazione)? input.IdIntegrazione : "null");
            q.setParam("id_amministrazione", input.IdAmministrazione);
            q.setParam("id_registro", input.IdRegistro);
            q.setParam("id_people", !string.IsNullOrWhiteSpace(input.IdPeopleIntegrazione) ? input.IdPeopleIntegrazione : "null");
            q.setParam("id_gruppo", !string.IsNullOrWhiteSpace(input.IdGruppoIntegrazione) ? input.IdGruppoIntegrazione : "null");
            q.setParam("cod_classifica", !string.IsNullOrWhiteSpace(input.CodiceClassifica) ? string.Format("'{0}'",input.CodiceClassifica.Replace("'", "''")) : "null");
            q.setParam("code_application", !string.IsNullOrWhiteSpace(input.CodeApplication) ? string.Format("'{0}'", input.CodeApplication.Replace("'", "''")) : "null");
            q.setParam("desc_integrazione", !string.IsNullOrWhiteSpace(input.DescIntegrazione) ? string.Format("'{0}'", input.DescIntegrazione.Replace("'", "''")) : "null");
            q.setParam("id_piano_conservazione", !string.IsNullOrWhiteSpace(input.IdPianoConservazione) ? input.IdPianoConservazione : "null");
            q.setParam("id_tipo_doc", !string.IsNullOrWhiteSpace(input.IdTipoDoc) ? input.IdTipoDoc : "null");
            q.setParam("id_tipo_fasc", !string.IsNullOrWhiteSpace(input.IdTipoFasc) ? input.IdTipoFasc : "null");
            string query = q.getSQL();
            logger.Debug("InsertPianoCons_IntegrPIS: " + query);

            if (!ExecuteNonQuery(query))
                throw new Exception("Errore nell'esecuzione della query");

            retVal = true;
        }
        catch (Exception e)
        {
            logger.Error("Errore in InsertPianoCons_IntegrPIS: " + e.Message);
        }

        return retVal;
    }

    public bool UpdatePianoCons_IntegrPIS(DocsPaVO.PianoCons_IntegrPIS input)
    {
        bool retVal = false;

        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_P3_PIS_APPS_ARCHIVEPLANS");

            q.setParam("id_integrazione", !string.IsNullOrWhiteSpace(input.IdIntegrazione) ? input.IdIntegrazione : "null");
            q.setParam("id_amministrazione", input.IdAmministrazione);
            q.setParam("id_registro", input.IdRegistro);
            q.setParam("id_people", !string.IsNullOrWhiteSpace(input.IdPeopleIntegrazione) ? input.IdPeopleIntegrazione : "null");
            q.setParam("id_gruppo", !string.IsNullOrWhiteSpace(input.IdGruppoIntegrazione) ? input.IdGruppoIntegrazione : "null");
            q.setParam("cod_classifica", !string.IsNullOrWhiteSpace(input.CodiceClassifica) ? string.Format("'{0}'", input.CodiceClassifica.Replace("'", "''")) : "null");
            q.setParam("code_application", !string.IsNullOrWhiteSpace(input.CodeApplication) ? string.Format("'{0}'", input.CodeApplication.Replace("'", "''")) : "null");
            q.setParam("desc_integrazione", !string.IsNullOrWhiteSpace(input.DescIntegrazione) ? string.Format("'{0}'", input.DescIntegrazione.Replace("'", "''")) : "null");
            q.setParam("id_piano_conservazione", !string.IsNullOrWhiteSpace(input.IdPianoConservazione) ? input.IdPianoConservazione : "null");
            q.setParam("id_tipo_doc", !string.IsNullOrWhiteSpace(input.IdTipoDoc) ? input.IdTipoDoc : "null");
            q.setParam("id_tipo_fasc", !string.IsNullOrWhiteSpace(input.IdTipoFasc) ? input.IdTipoFasc : "null");
            q.setParam("idriga", input.SystemId);
            string query = q.getSQL();
            logger.Debug("UpdatePianoCons_IntegrPIS: " + query);

            if (!ExecuteNonQuery(query))
                throw new Exception("Errore nell'esecuzione della query");

            retVal = true;
        }
        catch (Exception e)
        {
            logger.Error("Errore in UpdatePianoCons_IntegrPIS: " + e.Message);
        }

        return retVal;
    }


}
