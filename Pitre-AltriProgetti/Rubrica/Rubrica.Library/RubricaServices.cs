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
    /// Servizi per la gestione degli elementi nella rubrica
    /// </summary>
    public sealed class RubricaServices
    {
        /// <summary>
        /// 
        /// </summary>
        private RubricaServices()
        { }

        #region Public methods

        /// <summary>
        /// Ricerca di elementi nella rubrica
        /// </summary>
        /// <param name="opzioniRicerca"></param>
        /// <returns></returns>
        public static ElementoRubrica[] Search(OpzioniRicerca opzioniRicerca)
        {
            try
            {
                SecurityHelper.CheckAuthenticatedPrincipal();
                
                return InternalSearch(opzioniRicerca);
            }
            catch (RubricaException rubricaEx)
            {
                Utils.Log.Write(rubricaEx);

                throw rubricaEx;
            }
            catch (Exception ex)
            {
                RubricaSearchException exception = new RubricaSearchException(Properties.Resources.SearchElementiRubricaException, ex)
                {
                    OpzioniRicerca = opzioniRicerca
                };

                Utils.Log.Write(exception);

                throw exception;
            }
        }

        /// <summary>
        /// Reperimento di un elemento esistente in rubrica
        /// </summary>
        /// <param name="id"></param>        
        /// <returns></returns>
        public static ElementoRubrica Get(int id)
        {
            try
            {
                SecurityHelper.CheckAuthenticatedPrincipal();

                return InternalGet(id);
            }
            catch (RubricaException rubricaEx)
            {
                Utils.Log.Write(rubricaEx);

                throw rubricaEx;
            }
            catch (Exception ex)
            {
                RubricaException exception = new RubricaException(Properties.Resources.GetElementoRubricaException, ex);

                Utils.Log.Write(exception);

                throw exception;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elemento"></param>
        /// <returns></returns>
        public static ElementoRubrica Insert(ElementoRubrica elemento)
        {
            try
            {
                // Controllo autenticazione
                SecurityHelper.CheckAuthenticatedPrincipal();

                // Validazione dati di input
                ValidateForInsert(elemento);

                return InternalInsert(elemento);
            }
            catch (RubricaException rubricaEx)
            {
                Utils.Log.Write(rubricaEx);

                throw rubricaEx;
            }
            catch (Exception ex)
            {
                RubricaException exception = new RubricaException(Properties.Resources.InsertElementoRubricaException, ex);

                Utils.Log.Write(exception);

                throw exception;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ElementoRubrica Update(ElementoRubrica elemento, bool multicasella)
        {
            try
            {
                SecurityHelper.CheckAuthenticatedPrincipal();

                // Validazione dati di input
                ValidateForUpdate(elemento);

                return InternalUpdate(elemento, multicasella);
            }
            catch (RubricaException rubricaEx)
            {
                Utils.Log.Write(rubricaEx);

                throw rubricaEx;
            }
            catch (Exception ex)
            {
                RubricaException exception = new RubricaException(Properties.Resources.UpdateElementoRubricaException, ex);

                Utils.Log.Write(exception);

                throw exception;
            }
        }

        /// <summary>
        /// Rimozione di un elemento in rubrica
        /// </summary>
        /// <param name="id"></param>
        public static void Delete(ElementoRubrica elemento)
        {
            try
            {
                SecurityHelper.CheckAuthenticatedPrincipal();

                // Validazione dati di input
                ValidateForDelete(elemento);

                InternalDelete(elemento);
            }
            catch (RubricaException rubricaEx)
            {
                Utils.Log.Write(rubricaEx);

                throw rubricaEx;
            }
            catch (Exception ex)
            {
                RubricaException exception = new RubricaException(Properties.Resources.DeleteElementoRubricaException, ex);

                Utils.Log.Write(exception);

                throw exception;
            }
        }

        #endregion

        #region Data Access methods

        /// <summary>
        /// Stored Procedure per l'accesso ai dati
        /// </summary>
        private const string SP_SEARCH = "GetElementiRubrica";
        private const string SP_INSERT = "InsertElementoRubrica";
        private const string SP_GET = "GetElementoRubrica";
        private const string SP_DELETE = "DeleteElementoRubrica";
        private const string SP_UPDATE = "UpdateElementoRubrica";
        private const string SP_CONTAINS = "ContainsElementoRubrica";
        private const string SP_INSERT_EMAIL = "InsertEmail";
        private const string SP_REMOVE_EMAILS = "RemoveEmails";
        private const string SP_GET_EMAILS = "GetEmails";

        /// <summary>
        /// Attributi dell'elemento rubrica
        /// </summary>
        private const string ID_PROPERTY = "Id";
        private const string CODICE_PROPERTY = "Codice";
        private const string DESCRIZIONE_PROPERTY = "Descrizione";
        private const string INDIRIZZO_PROPERTY = "Indirizzo";
        private const string CITTA_PROPERTY = "Citta";
        private const string CAP_PROPERTY = "Cap";
        private const string PROVINCIA_PROPERTY = "Provincia";
        private const string NAZIONE_PROPERTY = "Nazione";
        private const string TELEFONO_PROPERTY = "Telefono";
        private const string FAX_PROPERTY = "Fax";
        private const string INTEROPERANTE_PROPERTY = "Interoperante";
        //private const string EMAIL_PROPERTY = "EMail";
        private const string AMMINISTRAZIONE_PROPERTY = "Amministrazione";
        private const string AOO_PROPERTY = "AOO";
        private const string DATACREAZIONE_PROPERTY = "DataCreazione";
        private const string DATAULTIMAMODIFICA_PROPERTY = "DataUltimaModifica";
        private const string UTENTECREATORE_PROPERTY = "UtenteCreatore";
        private const string TIPOCORRISPONDENTE_PROPERTY = "TipoCorrispondente";
        private const string URL_PROPERTY = "Url";
        private const string NOTE_PROPERTY = "Note";
        private const string EMAIL_PROPERTY = "Email";
        private const string PREFERITA_PROPERTY = "Preferita";
        private const string PUBBLICATO_PROPERTY = "CHA_PUBBLICATO";
        
        //Emanuela: aggiunco per l'elemento in Rubrica Comune i campi partita iva e codice fiscale
        private const string CODICE_FISCALE_PROPERTY = "VAR_COD_FISC";
        private const string PARTITA_IVA_PROPERTY = "VAR_COD_PI";
        /// <summary>
        /// Verifica se l'elemento è già presente
        /// </summary>
        /// <param name="codice"></param>
        /// <returns></returns>
        private static bool Contains(string codice)
        {
            bool retValue = false;

            Dpa.DataAccess.Database db = RubricaDatabase.CreateDatabase();

            using (Dpa.DataAccess.DBCommandWrapper cw = db.GetStoredProcCommandWrapper(RubricaDatabase.GetSpNameForPackage(SP_CONTAINS)))
            {
                cw.AddInParameter("pCodice", DbType.String, codice);
                cw.AddOutParameter("pRet", DbType.Int32, 0);

                Utils.Log.Write(cw.Command);

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
        private static ElementoRubrica[] InternalSearch(OpzioniRicerca opzioniRicerca)
        {
            List<ElementoRubrica> elementi = new List<ElementoRubrica>();

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

                Utils.Log.Write(cw.Command);

                using (IDataReader reader = db.ExecuteReader(cw))
                {
                    while (reader.Read())
                        elementi.Add(CreateElementoRubrica(reader, false));
                }
                
                if (opzioniRicerca != null && opzioniRicerca.CriteriPaginazione != null)
                    opzioniRicerca.CriteriPaginazione.SetTotaleOggetti(Convert.ToInt32(cw.GetParameterValue("pTotaleOggetti")));
            }

            return elementi.ToArray();
        }

        /// <summary>
        /// Repreimento oggetto DataObject
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static ElementoRubrica InternalGet(int id)
        {
            ElementoRubrica elemento = null;

            try
            {
                Dpa.DataAccess.Database db = RubricaDatabase.CreateDatabase();

                using (Dpa.DataAccess.DBCommandWrapper cw = db.GetStoredProcCommandWrapper(RubricaDatabase.GetSpNameForPackage(SP_GET)))
                {
                    cw.AddInParameter("pId", DbType.Int32, id);

                    Utils.Log.Write(cw.Command);

                    using (IDataReader reader = db.ExecuteReader(cw))
                        if (reader.Read())
                        {
                            elemento = CreateElementoRubrica(reader, true);
                        }
                }

                if (elemento == null)
                    throw new ApplicationException(Properties.Resources.DataNotFoundException);
            }
            catch (Exception ex)
            {
                RubricaException exception = new RubricaException(string.Format(Properties.Resources.GetElementoRubricaException, id), ex);

                Utils.Log.Write(exception);

                throw exception;
            }

            return elemento;
        }

        /// <summary>
        /// Inserimento nuovo oggetto in tabella ElementiRubrica
        /// </summary>
        /// <param name="elemento"></param>
        /// <returns></returns>
        private static ElementoRubrica InternalInsert(ElementoRubrica elemento)
        {
            Dpa.DataAccess.Database db = RubricaDatabase.CreateDatabase();

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();

                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    using (Dpa.DataAccess.DBCommandWrapper cw = db.GetStoredProcCommandWrapper(RubricaDatabase.GetSpNameForPackage(SP_INSERT)))
                    {
                        elemento.DataCreazione = DateTime.Now;
                        elemento.DataUltimaModifica = DateTime.Now;

                        cw.AddInParameter("pCodice", DbType.String, elemento.Codice);
                        cw.AddInParameter("pDescrizione", DbType.String, elemento.Descrizione);
                        cw.AddInParameter("pIndirizzo", DbType.String, elemento.Indirizzo);
                        cw.AddInParameter("pCitta", DbType.String, elemento.Citta);
                        cw.AddInParameter("pCap", DbType.String, elemento.Cap);
                        cw.AddInParameter("pProvincia", DbType.String, elemento.Provincia);
                        cw.AddInParameter("pNazione", DbType.String, elemento.Nazione);
                        cw.AddInParameter("pTelefono", DbType.String, elemento.Telefono);
                        cw.AddInParameter("pFax", DbType.String, elemento.Fax);
                        //cw.AddInParameter("pEmail", DbType.String, elemento.Email);
                        cw.AddInParameter("pAOO", DbType.String, elemento.AOO);
                        cw.AddInParameter("pDataCreazione", DbType.DateTime, elemento.DataCreazione);
                        cw.AddInParameter("pDataUltimaModifica", DbType.DateTime, elemento.DataUltimaModifica);
                        cw.AddInParameter("pUtenteCreatore", DbType.String, Security.RubricaPrincipal.Current.Identity.Name);
                        cw.AddInParameter("pTipoCorrispondente", DbType.String, elemento.TipoCorrispondente.ToString());
                        cw.AddInParameter("pAmministrazione", DbType.String, elemento.Amministrazione);
                        if (elemento.Urls.Count > 0)
                            cw.AddInParameter("pUrl", DbType.String, elemento.Urls[0].Url);
                        else
                            cw.AddInParameter("pUrl", DbType.String, String.Empty);
                        if (string.IsNullOrEmpty(elemento.CHA_Pubblicato))
                            elemento.CHA_Pubblicato = "0";
                        cw.AddInParameter("pChaPubblica", DbType.String, elemento.CHA_Pubblicato);
                        cw.AddOutParameter("pId", DbType.Int32, 0);

                        //Emanuela: aggiunta campi partita iva e codice fiscale
                        cw.AddInParameter("pCodiceFiscale", DbType.String, elemento.CodiceFiscale);
                        cw.AddInParameter("pPartitaIva", DbType.String, elemento.PartitaIva);

                        Utils.Log.Write(cw.Command);

                        db.ExecuteNonQuery(cw);

                        if (cw.RowsAffected > 0)
                        {
                            elemento.Id = Convert.ToInt32(cw.GetParameterValue("pId"));

                            // Rimozione delle email e associazione delle nuove
                            if (!String.IsNullOrEmpty(elemento.Email) && elemento.Emails.Count(m => m.Email == elemento.Email) == 0)
                                elemento.Emails.Add(new EmailInfo(elemento.Email, true));

                            elemento.Emails.ForEach(d => InternalInsertEmail(d, elemento.Id));

                            // Commit transazione
                            transaction.Commit();
                        }
                    }
                }
            }

            return elemento;
        }

        /// <summary>
        /// Aggiornamento oggetto in tabella ElementiRubrica
        /// </summary>
        /// <param name="elemento"></param>
        /// <param name="multicasella">
        ///     Flag utilizzato per indicare se bisogna gestire il corrispondente come corrispondente multicasella.
        ///     Se un corrispondente è multicasella, la gestione avviene esclusivamente prendendo in considerazione
        ///     la lista delle caselle email altrimenti viene considerata come casella quella passata attraverso
        ///     la valorizzazione della proprietà Email.
        ///     Nel caso monocasella, se l'attributo Mail dell'oggetto ElementoRubrica è valorizzato, viene aggiunta 
        ///     (se non esiste) una nuova casella preferita al corrispondente, se viene passato nullo, viene cancellata
        ///     la casella preferita e, se presenti altre email associate al corrispondente, viene impostata 
        ///     come preferita la prima casella disponibile.
        /// </param>
        /// <returns></returns>
        private static ElementoRubrica InternalUpdate(ElementoRubrica elemento, bool multicasella)
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
                        cw.AddInParameter("pId", DbType.Int32, elemento.Id);
                        cw.AddInParameter("pDescrizione", DbType.String, elemento.Descrizione);
                        cw.AddInParameter("pIndirizzo", DbType.String, elemento.Indirizzo);
                        cw.AddInParameter("pCitta", DbType.String, elemento.Citta);
                        cw.AddInParameter("pCap", DbType.String, elemento.Cap);
                        cw.AddInParameter("pProvincia", DbType.String, elemento.Provincia);
                        cw.AddInParameter("pNazione", DbType.String, elemento.Nazione);
                        cw.AddInParameter("pTelefono", DbType.String, elemento.Telefono);
                        cw.AddInParameter("pFax", DbType.String, elemento.Fax);
                        //cw.AddInParameter("pEmail", DbType.String, elemento.Email);
                        cw.AddInParameter("pAOO", DbType.String, elemento.AOO);
                        cw.AddInParameter("pDataUltimaModifica", DbType.DateTime, dataUltimaModifica);
                        cw.AddInParameter("pOldDataUltimaModifica", DbType.DateTime, elemento.DataUltimaModifica);
                        cw.AddInParameter("pTipoCorrispondente", DbType.String, elemento.TipoCorrispondente.ToString());
                        cw.AddInParameter("pAmministrazione", DbType.String, elemento.Amministrazione);

                        if (elemento.Urls.Count > 0)
                            cw.AddInParameter("pUrl", DbType.String, elemento.Urls[0].Url);
                        else
                            cw.AddInParameter("pUrl", DbType.String, String.Empty);

                        if (string.IsNullOrEmpty(elemento.CHA_Pubblicato))
                            elemento.CHA_Pubblicato = "0";
                        cw.AddInParameter("pChaPubblica", DbType.String, elemento.CHA_Pubblicato);

                        //Emanuela: aggiunta campi partita iva e codice fiscale
                        cw.AddInParameter("pCodiceFiscale", DbType.String, elemento.CodiceFiscale);
                        cw.AddInParameter("pPartitaIva", DbType.String, elemento.PartitaIva);


                        Utils.Log.Write(cw.Command);

                        db.ExecuteNonQuery(cw);

                        if (cw.RowsAffected == 0)
                            throw new RubricaException(Properties.Resources.ConcurrencyException);
                        else
                        {
                            elemento.DataUltimaModifica = dataUltimaModifica;

                            // Aggiornamento delle emails
                            UpdateEmails(elemento, multicasella);
                            
                            transaction.Commit();
                        }
                    }
                }
            }

            return elemento;
        }

        /// <summary>
        /// Rimozione di un oggetto in tabella ElementiRubrica
        /// </summary>
        /// <param name="id"></param>
        private static void InternalDelete(ElementoRubrica elemento)
        {
            Dpa.DataAccess.Database db = RubricaDatabase.CreateDatabase();

            using (Dpa.DataAccess.DBCommandWrapper cw = db.GetStoredProcCommandWrapper(RubricaDatabase.GetSpNameForPackage(SP_DELETE)))
            {
                cw.AddInParameter("pId", DbType.Int32, elemento.Id);
                cw.AddInParameter("pDataUltimaModifica", DbType.DateTime, elemento.DataUltimaModifica);
                
                Utils.Log.Write(cw.Command);

                db.ExecuteNonQuery(cw);

                if (cw.RowsAffected == 0)
                    throw new RubricaException(Properties.Resources.ConcurrencyException);
            }
        }

        /// <summary>
        /// Inserimento di una email relativa ad un corrispondente
        /// </summary>
        /// <param name="emails">Email da inserire</param>
        /// <param name="idElementoRubrica">Id dell'elemento rubrica cui si riferisce la mail</param>
        private static void InternalInsertEmail(EmailInfo emails, int idElementoRubrica)
        {
            Dpa.DataAccess.Database db = RubricaDatabase.CreateDatabase();

            using (Dpa.DataAccess.DBCommandWrapper cw = db.GetStoredProcCommandWrapper(RubricaDatabase.GetSpNameForPackage(SP_INSERT_EMAIL)))
            {
                
                cw.AddInParameter("pId", DbType.Int32, idElementoRubrica);
                cw.AddInParameter("pEmail", DbType.AnsiString, emails.Email);
                cw.AddInParameter("pNote", DbType.AnsiString, emails.Note);
                cw.AddInParameter("pPreferita", DbType.AnsiString, emails.Preferita ? "1" : "0");

                Utils.Log.Write(cw.Command);

                db.ExecuteNonQuery(cw);

                if (cw.RowsAffected == 0)
                    throw new RubricaException(Properties.Resources.ConcurrencyException);
            }
 
        }

        /// <summary>
        /// Metodo per l'aggiornamento delle caselle email associate al corrispondente
        /// </summary>
        /// <param name="elemento">Elemento di cui aggiornare le caselle</param>
        /// <param name="multicasella">Flag utilizzato per indicare se bisogna gestire il corrispondente in modalità multicasella</param>
        /// <exception cref="RubricaException">Sollevata in caso di problemi durante la rimozione o il salvataggio delle email</exception>
        private static void UpdateEmails(ElementoRubrica elemento, bool multicasella)
        {
            // Email da salvare
            EMailList emails = null;

            // Aggiornamento delle emails all'utente
            // Nel caso di multicasella, vengono cancellate le email presenti
            // e quindi vengono salvate le nuove mails
            if (multicasella)
            {
                emails = elemento.Emails;

                // Se non ci sono caselle di posta preferite, viene impostata come preferita la prima 
                // mail disponibile
                emails.SetFirstMailAsPreferred();
            }
            else
            {
                // Recupero delle caselle associate al corrispondente
                emails = InternalGetEmails(elemento.Id);

                // Nel caso monocasella, se l'attributo mail è valorizzato, vengono caricate le email
                // associate al corrispondente e quindi viene aggiunta la nuova mail impostandola come
                // preferita altrimenti viene cancellata la mail preferita e, se ne sono disponibili altre,
                // viene impostata come preferita la prima disponibile, quindi si procede al salvataggio delle
                // emails
                if (String.IsNullOrEmpty(elemento.Email))
                    emails.RemovePreferred();
                else
                    emails.Add(new EmailInfo(elemento.Email, true));

            }

            // Rimozione delle vecchie mail memorizzate e memorizzazione delle nuove
            InternalRemoveEmails(elemento.Id);

            emails.ForEach(e => InternalInsertEmail(e, elemento.Id));
        }

        /// <summary>
        /// Rimozione delle emails relative ad un corrispondente
        /// </summary>
        /// <param name="idElementoRubrica">Id dell'elemento rubrica cui si riferiscono le mails</param>
        private static void InternalRemoveEmails(int idElementoRubrica)
        {
            Dpa.DataAccess.Database db = RubricaDatabase.CreateDatabase();

            using (Dpa.DataAccess.DBCommandWrapper cw = db.GetStoredProcCommandWrapper(RubricaDatabase.GetSpNameForPackage(SP_REMOVE_EMAILS)))
            {

                cw.AddInParameter("pId", DbType.Int32, idElementoRubrica);
                
                Utils.Log.Write(cw.Command);

                db.ExecuteNonQuery(cw);

                if (cw.RowsAffected == 0)
                    throw new RubricaException(Properties.Resources.ConcurrencyException);
            }

        }

        /// <summary>
        /// Reperimento delle emails relative ad un corrispondente
        /// </summary>
        /// <param name="idElementoRubrica">Id dell'elemento rubrica cui si riferiscono le mails</param>
        private static EMailList InternalGetEmails(int idElementoRubrica)
        {
            EMailList emails = new EMailList();

            Dpa.DataAccess.Database db = RubricaDatabase.CreateDatabase();

            using (Dpa.DataAccess.DBCommandWrapper cw = db.GetStoredProcCommandWrapper(RubricaDatabase.GetSpNameForPackage(SP_GET_EMAILS)))
            {
                cw.AddInParameter("pId", DbType.Int32, idElementoRubrica);

                Utils.Log.Write(cw.Command);

                using (IDataReader reader = db.ExecuteReader(cw))
                {
                    while (reader.Read())
                        emails.Add(CreateEmailInfo(reader));
                }
            }

            return emails;

        }

        /// <summary>
        /// Creazione oggetto ElementoRubrica
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="datiAggiuntivi"></param>
        /// <returns></returns>
        private static ElementoRubrica CreateElementoRubrica(IDataReader reader, bool datiAggiuntivi)
        {
            object [] array = new object[reader.FieldCount];
            reader.GetValues(array);
            ElementoRubrica elemento = new ElementoRubrica
            {
                Id = Convert.ToInt32(Dpa.DataAccess.Helper.DataReaderHelper.GetValue<object>(reader, ID_PROPERTY, false)),
                Codice = Dpa.DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, CODICE_PROPERTY, false),
                Descrizione = Dpa.DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, DESCRIZIONE_PROPERTY, false),
                Indirizzo = Dpa.DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, INDIRIZZO_PROPERTY, true, string.Empty),
                Citta = Dpa.DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, CITTA_PROPERTY, true, string.Empty),
                Cap = Dpa.DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, CAP_PROPERTY, true, string.Empty),
                Provincia = Dpa.DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, PROVINCIA_PROPERTY, true, string.Empty),
                Nazione = Dpa.DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, NAZIONE_PROPERTY, true, string.Empty),
                Telefono = Dpa.DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, TELEFONO_PROPERTY, true, string.Empty),
                Fax = Dpa.DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, FAX_PROPERTY, true, string.Empty),
                Amministrazione = Dpa.DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, AMMINISTRAZIONE_PROPERTY, true, string.Empty),
                AOO = Dpa.DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, AOO_PROPERTY, true, string.Empty),
                UtenteCreatore = Dpa.DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, UTENTECREATORE_PROPERTY, false),
                DataCreazione = Dpa.DataAccess.Helper.DataReaderHelper.GetValue<DateTime>(reader, DATACREAZIONE_PROPERTY, false),
                DataUltimaModifica = Dpa.DataAccess.Helper.DataReaderHelper.GetValue<DateTime>(reader, DATAULTIMAMODIFICA_PROPERTY, false),
                TipoCorrispondente = (Tipo)Enum.Parse(typeof(Tipo), Dpa.DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, TIPOCORRISPONDENTE_PROPERTY, false)),
                CHA_Pubblicato = Dpa.DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, PUBBLICATO_PROPERTY, true),
                CodiceFiscale = Dpa.DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, CODICE_FISCALE_PROPERTY, true, string.Empty),
                PartitaIva = Dpa.DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, PARTITA_IVA_PROPERTY, true, string.Empty),
            };

            // Attualmente l'url è uno solo
            String url = Dpa.DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, URL_PROPERTY, true);
            if (!String.IsNullOrEmpty(url))
                elemento.Urls.Add(new UrlInfo(url));

            elemento.Emails = InternalGetEmails(elemento.Id);
            EmailInfo prefMail = elemento.Emails.GetPreferredMail();
            if (prefMail != null)
                elemento.Email = prefMail.Email;
            else
                elemento.Email = String.Empty;

            bool amm = false;
            bool aoo = false;
            bool emails = false;
            bool urls = false;


            if(elemento!=null &&  !string.IsNullOrEmpty(elemento.Amministrazione))
                amm=true;

            if(elemento!=null && !string.IsNullOrEmpty(elemento.AOO) )
                aoo = true;


            if(elemento!=null &&  elemento.Emails!=null && elemento.Emails.Count>0) 
                emails = true;

            if (elemento.Urls != null && elemento.Urls.Count > 0)
                urls = true;

            if (amm && aoo && urls)
                elemento.Canale = "INTEROPERABILITA PITRE";
            else
                if (amm && aoo && emails && !urls)
                    elemento.Canale = "INTEROPERABILITA";
                else
                    if (emails && !urls && !(amm && aoo))
                        elemento.Canale = "MAIL";
                    else
                        if (!emails && !urls && !(amm && aoo)) 
                            elemento.Canale = "LETTERA";
            

            return elemento;

        }

        /// <summary>
        /// Estrazione delle informazioni su una email associata al corrispondente
        /// </summary>
        /// <param name="reader">Reader da cui estrarre i dati</param>
        /// <returns>Informazioni su una mail associata al corrispondente</returns>
        private static EmailInfo CreateEmailInfo(IDataReader reader)
        {
            return new EmailInfo() 
            {
                Email = Dpa.DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, EMAIL_PROPERTY, false),
                Note = Dpa.DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, NOTE_PROPERTY, true),
                Preferita = Dpa.DataAccess.Helper.DataReaderHelper.GetValue<decimal>(reader, PREFERITA_PROPERTY, false) == 1 ? true : false
            };

        }

        /// <summary>
        /// Validazione dati per l'inserimento
        /// </summary>
        /// <param name="elemento"></param>
        private static void ValidateForInsert(ElementoRubrica elemento)
        {
            Utils.Validator.CheckProperty<string>(elemento, "Codice", true, 255);
            Utils.Validator.CheckProperty<string>(elemento, "Descrizione", true, 255);
            Utils.Validator.CheckProperty<string>(elemento, "Indirizzo", false, 255);
            Utils.Validator.CheckProperty<string>(elemento, "Telefono", false, 20);
            Utils.Validator.CheckProperty<string>(elemento, "Fax", false, 20);
            Utils.Validator.CheckProperty<string>(elemento, "Citta", false, 50);
            Utils.Validator.CheckProperty<string>(elemento, "Cap", false, 5);
            Utils.Validator.CheckProperty<string>(elemento, "Provincia", false, 2);
            Utils.Validator.CheckProperty<string>(elemento, "Nazione", false, 50);
            Utils.Validator.CheckProperty<Tipo>(elemento, "TipoCorrispondente", false, 5);

            // Validazione campi per interoperabilità
            Utils.Validator.CheckProperty<string>(elemento, "Email", false, 100);
            Utils.Validator.CheckProperty<string>(elemento, "Amministrazione", false, 255);
            Utils.Validator.CheckProperty<string>(elemento, "AOO", false, 255);
            elemento.Urls.ForEach(u => Utils.Validator.CheckUrl(u.Url));
            if (!string.IsNullOrEmpty(elemento.CodiceFiscale))
            { 
                if(elemento.CodiceFiscale.Length==16)
                    Utils.Validator.CheckTaxCode(elemento.CodiceFiscale);
                else
                    Utils.Validator.CheckVatNumber(elemento.CodiceFiscale, Properties.Resources.CodiceFiscaleException);
            }
               
            if (!string.IsNullOrEmpty(elemento.PartitaIva))
                Utils.Validator.CheckVatNumber(elemento.PartitaIva, Properties.Resources.PartitaIvaException);

            // Validazione indirizzo mail
            //if (!string.IsNullOrEmpty(elemento.Email)) Utils.Validator.CheckMailAddress(elemento.Email);
            elemento.Emails.ForEach(e => Utils.Validator.CheckMailAddress(e.Email));

            // Verifica se il codice non è già presente
            if (Contains(elemento.Codice))
            {
                throw new RubricaException(string.Format(Properties.Resources.ElementoRubricaAlreadyExistException, elemento.Codice));
            }
        }

        /// <summary>
        /// Validazione dati per l'aggiornamento
        /// </summary>
        /// <param name="elemento"></param>
        private static void ValidateForUpdate(ElementoRubrica elemento)
        {
            Utils.Validator.CheckProperty<int>(elemento, "Id", true, 0);
            Utils.Validator.CheckProperty<string>(elemento, "Codice", true, 255);
            Utils.Validator.CheckProperty<string>(elemento, "Descrizione", true, 255);
            Utils.Validator.CheckProperty<string>(elemento, "Indirizzo", false, 255);
            Utils.Validator.CheckProperty<string>(elemento, "Telefono", false, 20);
            Utils.Validator.CheckProperty<string>(elemento, "Fax", false, 20);
            
            Utils.Validator.CheckProperty<string>(elemento, "Citta", false, 50);
            Utils.Validator.CheckProperty<string>(elemento, "Cap", false, 5);
            Utils.Validator.CheckProperty<string>(elemento, "Provincia", false, 2);
            Utils.Validator.CheckProperty<string>(elemento, "Nazione", false, 50);
            Utils.Validator.CheckProperty<Tipo>(elemento, "TipoCorrispondente", false, 5);

            // Validazione campi per interoperabilità;
            // sono obbligatori solo se la proprietà interoperante è true
            Utils.Validator.CheckProperty<string>(elemento, "Email", false, 100);
            Utils.Validator.CheckProperty<string>(elemento, "Amministrazione", false, 255);
            Utils.Validator.CheckProperty<string>(elemento, "AOO", false, 255);
            elemento.Urls.ForEach(u => Utils.Validator.CheckUrl(u.Url));

            if (!string.IsNullOrEmpty(elemento.CodiceFiscale))
            {
                if (elemento.CodiceFiscale.Length == 16)
                    Utils.Validator.CheckTaxCode(elemento.CodiceFiscale);
                else
                    Utils.Validator.CheckVatNumber(elemento.CodiceFiscale, Properties.Resources.CodiceFiscaleException);
            }
            if (!string.IsNullOrEmpty(elemento.PartitaIva))
                Utils.Validator.CheckVatNumber(elemento.PartitaIva, Properties.Resources.PartitaIvaException);

            // Validazione indirizzo mail
            //if (!string.IsNullOrEmpty(elemento.Email)) Utils.Validator.CheckMailAddress(elemento.Email);
            elemento.Emails.ForEach(e => Utils.Validator.CheckMailAddress(e.Email));

            Utils.Validator.CheckProperty<string>(elemento, "UtenteCreatore", true, 50);
            Utils.Validator.CheckProperty<DateTime>(elemento, "DataCreazione", true, 0);
            Utils.Validator.CheckProperty<DateTime>(elemento, "DataUltimaModifica", true, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        private static void ValidateForDelete(ElementoRubrica elemento)
        {
            Utils.Validator.CheckProperty<int>(elemento, "Id", true, 0);
            Utils.Validator.CheckProperty<string>(elemento, "UtenteCreatore", true, 50);
            Utils.Validator.CheckProperty<DateTime>(elemento, "DataCreazione", true, 0);
            Utils.Validator.CheckProperty<DateTime>(elemento, "DataUltimaModifica", true, 0);
        }

        #endregion
    }
}