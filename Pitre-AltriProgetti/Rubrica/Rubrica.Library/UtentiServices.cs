using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Rubrica.Library.Data;
using Rubrica.Library.Security;

namespace Rubrica.Library
{
    /// <summary>
    /// Servizi per la gestione degli utenti cui è consentito l'accesso alla rubrica
    /// </summary>
    public sealed class UtentiServices
    {
        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        private UtentiServices()
        { }

        /// <summary>
        /// Validazione credenziali utente
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="amministratore"></param>
        /// <returns></returns>
        public static SecurityCredentialsResult ValidateCredentials(SecurityCredentials credentials)
        {
            return SecurityHelper.ValidateCredentials(credentials);
        }
 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="opzioniRicerca"></param>
        /// <returns></returns>
        public static Utente[] Search(OpzioniRicerca opzioniRicerca)
        {
            try
            {
                SecurityHelper.CheckAdminAuthenticatedPrincipal();

                return InternalSearch(opzioniRicerca);
            }
            catch (RubricaException rubricaEx)
            {
                Utils.Log.Write(rubricaEx);

                throw rubricaEx;
            }
            catch (Exception ex)
            {
                RubricaSearchException exception = new RubricaSearchException(Properties.Resources.SearchUserException, ex)
                {
                    OpzioniRicerca = opzioniRicerca
                };

                Utils.Log.Write(exception);

                throw exception;
            }
        }

        /// <summary>
        /// Reperimento di un utente
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Utente Get(int id)
        {
            try
            {
                SecurityHelper.CheckAdminAuthenticatedPrincipal();

                return InternalGet(id);
            }
            catch (RubricaException rubricaEx)
            {
                Utils.Log.Write(rubricaEx);

                throw rubricaEx;
            }
            catch (Exception ex)
            {
                RubricaException exception = new RubricaException(Properties.Resources.GetUserException, ex);

                Utils.Log.Write(exception);

                throw exception;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public static Utente Insert(Utente utente)
        {
            try
            {
                SecurityHelper.CheckAdminAuthenticatedPrincipal();

                // Validazione dati di input
                ValidateForInsert(utente);

                return InternalInsert(utente);
            }
            catch (RubricaException rubricaEx)
            {
                Utils.Log.Write(rubricaEx);

                throw rubricaEx;
            }
            catch (Exception ex)
            {
                RubricaException exception = new RubricaException(Properties.Resources.InsertUserException, ex);

                Utils.Log.Write(exception);

                throw exception;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public static Utente Update(Utente utente)
        {
            try
            {
                SecurityHelper.CheckAdminAuthenticatedPrincipal();

                // Validazione dati di input
                ValidateForUpdate(utente);

                return InternalUpdate(utente);
            }
            catch (RubricaException rubricaEx)
            {
                Utils.Log.Write(rubricaEx);

                throw rubricaEx;
            }
            catch (Exception ex)
            {
                RubricaException exception = new RubricaException(Properties.Resources.UpdateUserException, ex);

                Utils.Log.Write(exception);

                throw exception;
            }
        }

        /// <summary>
        /// Rimozione di un utente
        /// </summary>
        /// <param name="utente"></param>
        public static void Delete(Utente utente)
        {
            try
            {
                SecurityHelper.CheckAdminAuthenticatedPrincipal();

                ValidateForDelete(utente);

                InternalDelete(utente);
            }
            catch (RubricaException rubricaEx)
            {
                Utils.Log.Write(rubricaEx);

                throw rubricaEx;
            }
            catch (Exception ex)
            {
                RubricaException exception = new RubricaException(Properties.Resources.DeleteUserException, ex);

                Utils.Log.Write(exception);

                throw exception;
            }
        }

        /// <summary>
        /// Modifica password utente
        /// </summary>
        /// <param name="data"></param>
        public static void ChangePassword(ChangePwdSecurityCredentials credentials)
        {
            SecurityHelper.CheckAuthenticatedPrincipal();

            RubricaIdentity identity = (RubricaIdentity) SecurityHelper.AuthenticatedPrincipal.Identity;
            
            // Le password possono essere modificate solo se dall'utente stesso
            if (identity.Name == credentials.UserName)
                InternalChangePassword(credentials);
            else
                throw new ApplicationException(Properties.Resources.AuthorizationException);
        }

        #endregion

        #region Data Access methods

        private const string SP_SEARCH = "GetUsers";
        private const string SP_INSERT = "InsertUser";
        private const string SP_GET = "GetUser";
        private const string SP_UPDATE = "UpdateUser";
        private const string SP_DELETE = "DeleteUser";
        private const string SP_CHANGE_PWD = "ChangeUserPassword";
        private const string SP_CONTAINS = "ContainsUser";

        private const string Id_PROPERTY = "Id";
        private const string NOME_PROPERTY = "Nome";
        private const string PASSWORD_PROPERTY = "Password";
        private const string AMMINISTRATORE_PROPERTY = "Amministratore";


        /// <summary>
        /// Verifica se l'utente è già presente
        /// </summary>
        /// <param name="nomeUtente"></param>
        /// <returns></returns>
        private static bool Contains(string nomeUtente)
        {
            bool retValue = false;

            Dpa.DataAccess.Database db = RubricaDatabase.CreateDatabase();

            using (Dpa.DataAccess.DBCommandWrapper cw = db.GetStoredProcCommandWrapper(RubricaDatabase.GetSpNameForPackage(SP_CONTAINS)))
            {
                cw.AddInParameter("pNomeUtente", DbType.String, nomeUtente);
                cw.AddOutParameter("pRet", DbType.Int32, 0);

                db.ExecuteScalar(cw);

                retValue = (Convert.ToInt32(cw.GetParameterValue("pRet")) > 0);
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="opzioniRicerca"></param>
        /// <returns></returns>
        private static Utente[] InternalSearch(OpzioniRicerca opzioniRicerca)
        {
            List<Utente> utenti = new List<Utente>();

            string filtro = string.Empty, ordinamento = string.Empty;
            int pagina = 0, oggettiPerPagina = 0;

            if (opzioniRicerca != null)
            {
                if (opzioniRicerca.CriteriRicerca != null) filtro = opzioniRicerca.CriteriRicerca.ToSql();
                if (opzioniRicerca.CriteriOrdinamento != null) ordinamento = opzioniRicerca.CriteriOrdinamento.ToSql();
                if (opzioniRicerca.CriteriPaginazione != null) pagina = opzioniRicerca.CriteriPaginazione.Pagina;
                if (opzioniRicerca.CriteriPaginazione != null) oggettiPerPagina = opzioniRicerca.CriteriPaginazione.OggettiPerPagina;
            }

            Dpa.DataAccess.Database db = RubricaDatabase.CreateDatabase();

            using (Dpa.DataAccess.DBCommandWrapper cw = db.GetStoredProcCommandWrapper(RubricaDatabase.GetSpNameForPackage(SP_SEARCH)))
            {
                cw.AddInParameter("pFiltro", DbType.String, filtro);
                cw.AddInParameter("pOrdinamento", DbType.String, ordinamento);
                cw.AddInParameter("pPagina", DbType.Int32, pagina);
                cw.AddInParameter("pOggettiPagina", DbType.Int32, oggettiPerPagina);
                cw.AddOutParameter("pTotaleOggetti", DbType.Int32, 0);

                using (IDataReader reader = db.ExecuteReader(cw))
                {
                    while (reader.Read())
                        utenti.Add(GetUtente(reader));
                }

                if (opzioniRicerca != null && opzioniRicerca.CriteriPaginazione != null)
                    opzioniRicerca.CriteriPaginazione.SetTotaleOggetti(Convert.ToInt32(cw.GetParameterValue("pTotaleOggetti")));
            }

            return utenti.ToArray();
        }

        /// <summary>
        /// Repreimento oggetto DataObject
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static Utente InternalGet(int id)
        {
            Utente utente = null;

            Dpa.DataAccess.Database db = RubricaDatabase.CreateDatabase();

            using (Dpa.DataAccess.DBCommandWrapper cw = db.GetStoredProcCommandWrapper(RubricaDatabase.GetSpNameForPackage(SP_GET)))
            {
                cw.AddInParameter("pId", DbType.Int32, id);

                using (IDataReader reader = db.ExecuteReader(cw))
                {
                    if (reader.Read())
                        utente = GetUtente(reader);
                }
            }

            if (utente == null)
                throw new ApplicationException(Properties.Resources.DataNotFoundException);

            return utente;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        private static void InternalDelete(Utente utente)
        {
            Dpa.DataAccess.Database db = RubricaDatabase.CreateDatabase();

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();

                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    using (Dpa.DataAccess.DBCommandWrapper cw = db.GetStoredProcCommandWrapper(RubricaDatabase.GetSpNameForPackage(SP_DELETE)))
                    {
                        cw.AddInParameter("pId", DbType.Int32, utente.Id);
                        cw.AddInParameter("pDataUltimaModifica", DbType.DateTime, utente.DataUltimaModifica);

                        db.ExecuteNonQuery(cw);

                        if (cw.RowsAffected == 0)
                            throw new RubricaException(Properties.Resources.ConcurrencyException);
                        else
                            transaction.Commit();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        private static Utente InternalInsert(Utente utente)
        {
            Dpa.DataAccess.Database db = RubricaDatabase.CreateDatabase();

            utente.DataCreazione = DateTime.Now;
            utente.DataUltimaModifica = utente.DataCreazione;

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();

                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    using (Dpa.DataAccess.DBCommandWrapper cw = db.GetStoredProcCommandWrapper(RubricaDatabase.GetSpNameForPackage(SP_INSERT)))
                    {
                        cw.AddInParameter("pNomeUtente", DbType.String, utente.Nome);
                        cw.AddInParameter("pPassword", DbType.String, SecurityCredentials.GetPasswordHash(utente.Nome, utente.Password));
                        cw.AddInParameter("pAmministratore", DbType.AnsiStringFixedLength, (utente.Amministratore ? "1" : "0"));
                        cw.AddInParameter("pDataCreazione", DbType.DateTime, utente.DataCreazione);
                        cw.AddInParameter("pDataUltimaModifica", DbType.DateTime, utente.DataUltimaModifica);
                        cw.AddOutParameter("pId", DbType.Int32, 0);

                        db.ExecuteNonQuery(cw, transaction);

                        if (cw.RowsAffected == 0)
                            throw new ApplicationException(Properties.Resources.ConcurrencyException);
                        else
                        {
                            // Reperimento identity
                            utente.Id = Convert.ToInt32(cw.GetParameterValue("pId"));

                            transaction.Commit();
                        }
                    }
                }
            }

            return utente;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        private static Utente InternalUpdate(Utente utente)
        {
            Dpa.DataAccess.Database db = RubricaDatabase.CreateDatabase();
            
            DateTime dataUltimaModifica = DateTime.Now;

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();

                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    using (Dpa.DataAccess.DBCommandWrapper cw = db.GetStoredProcCommandWrapper(RubricaDatabase.GetSpNameForPackage(SP_UPDATE)))
                    {
                        cw.AddInParameter("pId", DbType.Int32, utente.Id);
                        cw.AddInParameter("pAmministratore", DbType.AnsiStringFixedLength, (utente.Amministratore ? "1" : "0"));
                        cw.AddInParameter("pDataUltimaModifica", DbType.DateTime, dataUltimaModifica);
                        cw.AddInParameter("pOldDataUltimaModifica", DbType.DateTime, utente.DataUltimaModifica);

                        db.ExecuteNonQuery(cw);

                        if (cw.RowsAffected == 0)
                            throw new RubricaException(Properties.Resources.ConcurrencyException);
                        else
                        {
                            utente.DataUltimaModifica = dataUltimaModifica;
                            
                            transaction.Commit();
                        }
                    }
                }
            }

            return utente;
        }

        /// <summary>
        /// Modifica password per l'utente
        /// </summary>
        /// <param name="credentials"></param>        
        /// <returns></returns>
        private static void InternalChangePassword(ChangePwdSecurityCredentials credentials)
        {
            Dpa.DataAccess.Database db = RubricaDatabase.CreateDatabase();

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();

                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    using (Dpa.DataAccess.DBCommandWrapper cw = db.GetStoredProcCommandWrapper(RubricaDatabase.GetSpNameForPackage(SP_CHANGE_PWD)))
                    {
                        cw.AddInParameter("pNomeUtente", DbType.String, credentials.UserName);
                        cw.AddInParameter("pPassword", DbType.String, SecurityCredentials.GetPasswordHash(credentials.UserName, credentials.Password));
                        cw.AddInParameter("pNewPassword", DbType.String, SecurityCredentials.GetPasswordHash(credentials.UserName, credentials.NewPassword));

                        db.ExecuteNonQuery(cw);

                        if (cw.RowsAffected == 0)
                            throw new ApplicationException(Properties.Resources.ConcurrencyException);
                        else
                            transaction.Commit();
                    }
                }
            }
        }

        /// <summary>
        /// Creazione oggetto DataObject a partire dai dati estratti dal database
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static Utente GetUtente(IDataReader reader)
        {
            return new Utente
            {
                Id = Convert.ToInt32(Dpa.DataAccess.Helper.DataReaderHelper.GetValue<object>(reader, "Id", false)),
                Nome = Dpa.DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "Nome", false),
                Amministratore = (Dpa.DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "Amministratore", false) == "1"),
                DataCreazione = Dpa.DataAccess.Helper.DataReaderHelper.GetValue<DateTime>(reader, "DataCreazione", false),
                DataUltimaModifica = Dpa.DataAccess.Helper.DataReaderHelper.GetValue<DateTime>(reader, "DataUltimaModifica", false)
            };
        }

        /// <summary>
        /// Validazione dati per l'inserimento
        /// </summary>
        /// <param name="utente"></param>
        private static void ValidateForInsert(Utente utente)
        {
            Utils.Validator.CheckProperty<string>(utente, "Nome", true, 50);
            Utils.Validator.CheckProperty<string>(utente, "Password", true, 50);

            if (Contains(utente.Nome))
                throw new RubricaException(string.Format(Properties.Resources.UserAlreadyException, utente.Nome));
        }

        /// <summary>
        /// Validazione dati per l'aggiornamento
        /// </summary>
        /// <param name="elemento"></param>
        private static void ValidateForUpdate(Utente utente)
        {
            Utils.Validator.CheckProperty<int>(utente, "Id", true, 0);
            Utils.Validator.CheckProperty<string>(utente, "Nome", true, 50);
            Utils.Validator.CheckProperty<DateTime>(utente, "DataCreazione", true, 50);
            Utils.Validator.CheckProperty<DateTime>(utente, "DataUltimaModifica", true, 50);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        private static void ValidateForDelete(Utente utente)
        {
            Utils.Validator.CheckProperty<int>(utente, "Id", true, 0);
            Utils.Validator.CheckProperty<DateTime>(utente, "DataCreazione", true, 50);
            Utils.Validator.CheckProperty<DateTime>(utente, "DataUltimaModifica", true, 50);
        }

        #endregion
    }
}