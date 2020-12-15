using System;
using System.Data;
using System.Collections;
using DocsPaVO.amministrazione;
using DocsPaVO.Validations;

namespace BusinessLogic.Amministrazione
{
	/// <summary>
	/// Summary description for MezziSpedizioneManager.
	/// </summary>
	public class MezziSpedizioneManager
	{
        public MezziSpedizioneManager()
		{

		}

		#region Public methods

		/// <summary>
		/// Lista Mezzi Spedizione
		/// </summary>
		/// <param name="idAmm">idAmm</param>
		/// <returns></returns>
        public static ArrayList ListaMezziSpedizione(string idAmm, bool vediTutti)
		{
			DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DataSet ds = dbAmm.GetMezzoSpedizione();
			dbAmm = null;
			DocsPaVO.amministrazione.MezzoSpedizione m_sped = null;
			ArrayList retValue = new ArrayList();

			if (ds.Tables.Count>0)
			{
                foreach (DataRow row in ds.Tables["AMM_MEZZO_SPED_LIST"].Rows)
				{
					m_sped = new DocsPaVO.amministrazione.MezzoSpedizione();
                    m_sped.Descrizione = row["DESCRIPTION"].ToString();
                    m_sped.IDAmministrazione = idAmm;
                    m_sped.IDSystem = row["IDSYSTEM_MS"].ToString();
                    m_sped.chaTipoCanale = row["CHA_TIPO_CANALE"].ToString();
                    if (row["DISABLED"] != null)
                        m_sped.Disabled = row["DISABLED"].ToString();
                    else
                        m_sped.Disabled = "";

                    if(vediTutti)
    					retValue.Add(m_sped);
                    else
                        if(m_sped.Disabled.Equals(""))
                            retValue.Add(m_sped);

					m_sped=null;
				}
			}
			return retValue;
        }

        /// <summary>
        /// Reperimento di un mezzo di spedizione
        /// </summary>
        /// <param name="idMezzoSpedizione"></param>
        /// <returns></returns>
        public static MezzoSpedizione GetMezzoSpedizione(string idMezzoSpedizione)
        {
            MezzoSpedizione retValue = null;

            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DataSet ds = dbAmm.GetDsMezzoSpedizione(idMezzoSpedizione);
            dbAmm = null;

            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable tableMezziSpedizione = ds.Tables["MEZZO_SPEDIZIONE"];

                if (tableMezziSpedizione.Rows.Count == 1)
                    retValue = GetMezzoSpedizione(tableMezziSpedizione.Rows[0]);
            }

            return retValue;
        }

        public static DocsPaVO.Validations.ValidationResultInfo DeleteMezzoSpedizione(MezzoSpedizione m_sped)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = CanUpdateMezzoSpedizione(m_sped);

            if (retValue.Value)
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                retValue.Value = dbAmm.DeleteMezzoSpedizione(m_sped);
            }

            return retValue;
        }

        public static DocsPaVO.Validations.ValidationResultInfo InsertMezzoSpedizione(MezzoSpedizione m_sped)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = CanInsertMezzoSpedizione(m_sped);

            if (retValue.Value)
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                retValue.Value = dbAmm.InsertMezzoSpedizione(m_sped);

                //if (!retValue.Value)
                //{
                //    // Errore nell'inserimento di un tipo ruolo
                //    retValue.BrokenRules.Add(
                //            new DocsPaVO.Validations.BrokenRule("DB_ERROR", "Errore in inserimento di un tipo ruolo"));
                //}
            }

            return retValue;
        }

        /// <summary>
        /// Verifica Verifica vincoli in inserimento di un tiporuolo 
        /// </summary>
        /// <param name="tipoRuolo"></param>
        /// <returns></returns>
        public static DocsPaVO.Validations.ValidationResultInfo CanInsertMezzoSpedizione(MezzoSpedizione m_sped)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = new ValidationResultInfo();

            // Verifica univocità descrizione mezzo di spedizione
                DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                retValue.Value = dbAmm.CheckUniqueDescrizioneMezzoSpedizione(m_sped.Descrizione);

                //if (!retValue.Value)
                //    retValue.BrokenRules.Add(new DocsPaVO.Validations.BrokenRule("CODICE_TIPO_RUOLO", "Codice tipo ruolo già presente"));

            return retValue;
        }

        public static DocsPaVO.Validations.ValidationResultInfo UpdateMezzoSpedizione(MezzoSpedizione m_sped)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = new ValidationResultInfo();

                DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                retValue.Value = dbAmm.UpdateMezzoSpedizione(m_sped);

            return retValue;
        }

        public static DocsPaVO.Validations.ValidationResultInfo CanUpdateMezzoSpedizione(MezzoSpedizione m_sped)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = new ValidationResultInfo();

                DocsPaDB.Query_DocsPAWS.Amministrazione dbAmministrazione = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                string descrizione = dbAmministrazione.GetDescrizioneMezzoSpedizione(m_sped.IDSystem);

                if (!m_sped.Descrizione.ToUpper().Equals(descrizione))
                    retValue.Value = false;
                else
                    retValue.Value = true;

            return retValue;
        }

        #endregion

        #region Private methods

        private enum DBActionTypeTipoRuoloEnum
		{
			InsertMode,
			UpdateMode,
			DeleteMode
		}

		/// <summary>
		/// Creazione di un nuovo oggetto "MezzoSpedizione"
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		private static MezzoSpedizione GetMezzoSpedizione(DataRow row)
		{
			MezzoSpedizione m_sped=new MezzoSpedizione();

            m_sped.IDSystem = row["ID_MEZZO_SPEDIZIONE"].ToString();
            m_sped.chaTipoCanale = row["CHA_TIPO_CANALE"].ToString();
            m_sped.Descrizione = row["DESCRIZIONE"].ToString();
            m_sped.Disabled = row["DISABLED"].ToString();

            return m_sped;
		}

		#endregion
	}
}
