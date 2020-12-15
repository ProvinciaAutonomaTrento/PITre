using System;
using System.Data;
using System.Collections;
using DocsPaVO.amministrazione;
using DocsPaVO.Validations;

namespace BusinessLogic.Amministrazione
{
	/// <summary>
	/// Summary description for TipiRuoloManager.
	/// </summary>
	public class TipiRuoloManager
	{
		public TipiRuoloManager()
		{

		}

		#region Public methods

		/// <summary>
		/// Lista Ruoli
		/// </summary>
		/// <param name="idUO">ID UO</param>
		/// <returns></returns>
		public static ArrayList ListTipoRuoloUtenti(string codTipoRuolo, string idAmm)
		{
			DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
			DataSet ds = dbAmm.GetListRuoli(codTipoRuolo, idAmm);
			dbAmm = null;

			DocsPaVO.amministrazione.OrgRuolo ruolo = null;

			ArrayList retValue = new ArrayList();

			if (ds.Tables.Count>0)
			{
				foreach (DataRow row in ds.Tables["AMM_RUOLI_LIST"].Rows)
				{
					ruolo = new DocsPaVO.amministrazione.OrgRuolo();

					ruolo.IDCorrGlobale = row["IDCORRGLOBALE"].ToString();
					ruolo.IDGruppo = row["IDGRUPPO"].ToString();					
					ruolo.IDTipoRuolo = row["IDTIPORUOLO"].ToString();	
					ruolo.Codice = row["CODICE"].ToString();
					ruolo.CodiceRubrica = row["CODICERUBRICA"].ToString();
					ruolo.Descrizione = row["DESCRIZIONE"].ToString();
					ruolo.DiRiferimento = string.Empty;
					ruolo.IDAmministrazione = idAmm;
					
					ruolo.Utenti = GetListUtentiRuolo(ruolo.IDGruppo);

					retValue.Add(ruolo);					

					ruolo=null;
				}
			}

			return retValue;
		}

		/// <summary>
		/// Lista Utenti
		/// </summary>
		/// <param name="idRuolo">ID Ruolo</param>O</param>
		/// <returns></returns>
		public static ArrayList GetListUtentiRuolo(string idRuolo)
		{
			DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
			DataSet ds = dbAmm.GetListUtRuolo(idRuolo);
			dbAmm = null;

			DocsPaVO.amministrazione.OrgUtente utente = null;
			
			ArrayList retValue = new ArrayList();

			if (ds.Tables.Count>0)
			{
				foreach (DataRow row in ds.Tables["AMM_UTENTI_LIST"].Rows)
				{
					utente = new DocsPaVO.amministrazione.OrgUtente();

					utente.IDCorrGlobale = row["IDCORRGLOBALE"].ToString();
					utente.IDPeople = row["IDPEOPLE"].ToString();
					utente.Codice = row["CODICE"].ToString();
					utente.CodiceRubrica = row["CODICERUBRICA"].ToString();
					utente.Nome = row["NOME"].ToString();
					utente.Cognome = row["COGNOME"].ToString();
					utente.IDAmministrazione = row["IDAMMINISTRAZIONE"].ToString();

					retValue.Add(utente);					

					utente=null;
				}
			}

			return retValue;
		}

		/// <summary>
		/// Reperimento tipi ruolo in un'amministrazione
		/// </summary>
		/// <param name="codiceAmministrazione"></param>
		/// <returns></returns>
		public static ArrayList GetTipiRuolo(string codiceAmministrazione)
		{
			DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm=new DocsPaDB.Query_DocsPAWS.Amministrazione();
			string idAmministrazione=dbAmm.GetIDAmm(codiceAmministrazione);
			dbAmm=null;

			return OrganigrammaManager.GetListTipiRuolo(idAmministrazione);
		}

		/// <summary>
		/// Reperimento di un tipo ruolo
		/// </summary>
		/// <param name="codiceAmministrazione"></param>
		/// <returns></returns>
		public static OrgTipoRuolo GetTipoRuolo(string idTipoRuolo)
		{
			OrgTipoRuolo retValue=null;

			DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm=new DocsPaDB.Query_DocsPAWS.Amministrazione();
			DataSet ds=dbAmm.GetDsTipoRuolo(idTipoRuolo);
			dbAmm=null;
			
			if (ds!=null && ds.Tables.Count>0)
			{
				DataTable tableTipiRuoli=ds.Tables["TIPI_RUOLI"];

				if (tableTipiRuoli.Rows.Count==1)
					retValue=GetTipoRuolo(tableTipiRuoli.Rows[0]);
			}

			return retValue;
		}

		/// <summary>
		/// Verifica Verifica vincoli in inserimento di un tiporuolo 
		/// </summary>
		/// <param name="tipoRuolo"></param>
		/// <returns></returns>
		public static DocsPaVO.Validations.ValidationResultInfo CanInsertTipoRuolo(OrgTipoRuolo tipoRuolo)
		{
			// Verifica presenza dati obbligatori
			DocsPaVO.Validations.ValidationResultInfo retValue=IsValidRequiredFieldsTipoRuolo(DBActionTypeTipoRuoloEnum.InsertMode,tipoRuolo);
			
			// Verifica univocità codice
			if (retValue.Value)
			{
				DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm=new DocsPaDB.Query_DocsPAWS.Amministrazione();
				retValue.Value=dbAmm.CheckUniqueCodiceTipoRuolo(tipoRuolo.Codice,tipoRuolo.IDAmministrazione);

				if (!retValue.Value)
					retValue.BrokenRules.Add(new DocsPaVO.Validations.BrokenRule("CODICE_TIPO_RUOLO","Codice tipo ruolo già presente"));
			}

			return retValue;
		}

		public static DocsPaVO.Validations.ValidationResultInfo InsertTipoRuolo(OrgTipoRuolo tipoRuolo)
		{
			DocsPaVO.Validations.ValidationResultInfo retValue=CanInsertTipoRuolo(tipoRuolo);

			if (retValue.Value)
			{
				DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm=new DocsPaDB.Query_DocsPAWS.Amministrazione();
				retValue.Value=dbAmm.InsertTipoRuolo(tipoRuolo);

				if (!retValue.Value)
				{
					// Errore nell'inserimento di un tipo ruolo
					retValue.BrokenRules.Add(
							new DocsPaVO.Validations.BrokenRule("DB_ERROR","Errore in inserimento di un tipo ruolo"));
				}
			}

			return retValue;
		}

		public static DocsPaVO.Validations.ValidationResultInfo CanUpdateTipoRuolo(OrgTipoRuolo tipoRuolo)
		{
			// Verifica presenza dati obbligatori
			DocsPaVO.Validations.ValidationResultInfo retValue=IsValidRequiredFieldsTipoRuolo(DBActionTypeTipoRuoloEnum.UpdateMode,tipoRuolo);

			if (retValue.Value)
			{
				// Verifica che non sia stato modificato il codice del tipo ruolo
				DocsPaDB.Query_DocsPAWS.Amministrazione dbAmministrazione=new DocsPaDB.Query_DocsPAWS.Amministrazione();
				string codice=dbAmministrazione.GetCodiceTipoRuolo(tipoRuolo.IDTipoRuolo);

				if (!tipoRuolo.Codice.Equals(codice))
				{
					retValue.BrokenRules.Add(
						new DocsPaVO.Validations.BrokenRule("CODICE_TIPO_RUOLO","Il codice tipo ruolo non può essere modificato"));
				}
			}
			
			return retValue;
		}

		public static DocsPaVO.Validations.ValidationResultInfo UpdateTipoRuolo(OrgTipoRuolo tipoRuolo)
		{
			DocsPaVO.Validations.ValidationResultInfo retValue=CanUpdateTipoRuolo(tipoRuolo);

			if (retValue.Value)
			{
				DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm=new DocsPaDB.Query_DocsPAWS.Amministrazione();
				retValue.Value=dbAmm.UpdateTipoRuolo(tipoRuolo);

				if (!retValue.Value)
				{
					// Errore nell'aggiornamento di un tipo ruolo
					retValue.BrokenRules.Add(
						new DocsPaVO.Validations.BrokenRule("DB_ERROR","Errore nell'aggiornamento di un tipo ruolo"));
				}
			}

			return retValue;

		}

		public static DocsPaVO.Validations.ValidationResultInfo CanDeleteTipoRuolo(OrgTipoRuolo tipoRuolo)
		{
			// Verifica presenza dati obbligatori
			DocsPaVO.Validations.ValidationResultInfo retValue=IsValidRequiredFieldsTipoRuolo(DBActionTypeTipoRuoloEnum.DeleteMode,tipoRuolo);

			if (retValue.Value)
			{
				// Verifica presenza ruoli nel tipo ruolo
				DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm=new DocsPaDB.Query_DocsPAWS.Amministrazione();
				if (dbAmm.GetCountRuoliInTipoRuolo(tipoRuolo.IDTipoRuolo)>0)
				{
					retValue.Value=false;

					retValue.BrokenRules.Add(
						new DocsPaVO.Validations.BrokenRule("CONTAIN_ROLES","Al tipo ruolo risulta associato almeno un ruolo in una UO"));
				}
			}
			
			return retValue;
		}

		public static DocsPaVO.Validations.ValidationResultInfo DeleteTipoRuolo(OrgTipoRuolo tipoRuolo)
		{
			DocsPaVO.Validations.ValidationResultInfo retValue=CanDeleteTipoRuolo(tipoRuolo);

			if (retValue.Value)
			{
				DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm=new DocsPaDB.Query_DocsPAWS.Amministrazione();
				retValue.Value=dbAmm.DeleteTipoRuolo(tipoRuolo);

				if (!retValue.Value)
				{
					// Errore nella cancellazione di un tipo ruolo
					retValue.BrokenRules.Add(
						new DocsPaVO.Validations.BrokenRule("DB_ERROR","Errore nella cancellazione di un tipo ruolo"));
				}
			}

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
		/// Verifica presenza dati obbligatori del tipo ruolo
		/// </summary>
		/// <param name="actionType"></param>
		/// <param name="tipoRuolo"></param>
		/// <returns></returns>
		private static ValidationResultInfo IsValidRequiredFieldsTipoRuolo(
											DBActionTypeTipoRuoloEnum actionType,
											DocsPaVO.amministrazione.OrgTipoRuolo tipoRuolo)
		{
			ValidationResultInfo retValue=new ValidationResultInfo();

			if (actionType!=DBActionTypeTipoRuoloEnum.InsertMode &&
				(tipoRuolo.IDTipoRuolo==null || 
				tipoRuolo.IDTipoRuolo==string.Empty || 
				tipoRuolo.IDTipoRuolo=="0"))
			{
				retValue.Value=false;
				retValue.BrokenRules.Add(new BrokenRule("ID_TIPO_RUOLO","ID tipo ruolo mancante"));
			}

			if (actionType==DBActionTypeTipoRuoloEnum.InsertMode ||
				actionType==DBActionTypeTipoRuoloEnum.UpdateMode)
			{
				if (tipoRuolo.Codice==null || tipoRuolo.Codice==string.Empty)
				{
					retValue.Value=false;
					retValue.BrokenRules.Add(new BrokenRule("CODICE_TIPO_RUOLO","Codice tipo ruolo mancante"));
				}

				if (tipoRuolo.Descrizione==null || tipoRuolo.Descrizione==string.Empty)
				{
					retValue.Value=false;
					retValue.BrokenRules.Add(new BrokenRule("DESCRIZIONE_TIPO_RUOLO","Descrizione tipo ruolo mancante"));
				}

				if (tipoRuolo.Livello==null || tipoRuolo.Livello==string.Empty)
				{
					retValue.Value=false;
					retValue.BrokenRules.Add(new BrokenRule("LIVELLO_TIPO_RUOLO","Livello mancante"));
				}
			}

			return retValue;
		}

		/// <summary>
		/// Creazione di un nuovo oggetto "OrgTipoRuolo"
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		private static OrgTipoRuolo GetTipoRuolo(DataRow row)
		{
			OrgTipoRuolo tipoRuolo=new OrgTipoRuolo();

			tipoRuolo.IDTipoRuolo=row["ID_TIPO_RUOLO"].ToString();
			tipoRuolo.Codice=row["CODICE"].ToString();
			tipoRuolo.Descrizione=row["DESCRIZIONE"].ToString();
			tipoRuolo.IDAmministrazione=row["ID_AMMINISTRAZIONE"].ToString();
			tipoRuolo.Livello=row["LIVELLO"].ToString();

			return tipoRuolo;
		}

		#endregion
	}
}
