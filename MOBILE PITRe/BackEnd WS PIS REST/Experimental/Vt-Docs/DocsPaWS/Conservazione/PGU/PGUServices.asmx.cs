using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace DocsPaWS.Conservazione.PGU
{
    /// <summary>
    /// Servizi Web per il Pannello Grafico Unificato
    /// </summary>
    [WebService(Namespace = "http://www.valueteam.com/Conservazione/PGU/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class PGUServices : System.Web.Services.WebService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public GetTokenResponse GetToken(GetTokenRequest request)
        {
            GetTokenResponse response = new GetTokenResponse();

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    //const string QUERY = "SELECT USER_ID FROM PEOPLE WHERE UPPER(USER_ID) = UPPER('{0}')";

                    string QUERY = "Select User_id||'_'||id_amm as tokenroot from people where upper(user_id)=upper('{0}') and id_amm=(select system_id from dpa_amministra where upper(var_codice_amm)=upper('" + request.Ente.ToString() + "'))";

                    string userId1 = string.Format("{0}_{1}", request.Ente, request.UserId);
                    string userId2 = string.Format("{0}", request.UserId);

                    string commandText = string.Format(QUERY, userId1);

                    string field;
                    if (!dbProvider.ExecuteScalar(out field, commandText))
                        throw new ApplicationException(dbProvider.LastExceptionMessage);

                    if (string.IsNullOrEmpty(field))
                    {
                        commandText = string.Format(QUERY, userId2);

                        if (!dbProvider.ExecuteScalar(out field, commandText))
                            throw new ApplicationException(dbProvider.LastExceptionMessage);
                    }

                    if (string.IsNullOrEmpty(field))
                        throw new ApplicationException(string.Format("L'utente non risulta censito nel sistema con i seguenti codici: '{0}' oppure '{1}'", userId1, userId2));
                    else
                    {
                        DocsPaUtils.Security.CryptoString cripto = new DocsPaUtils.Security.CryptoString("PGU");

                        response.Success = true;
                        response.Token = cripto.Encrypt(field);
                        response.Exception = null;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public GetCountIstanzeConservazioneResponse GetCountIstanzeConservazione(GetCountIstanzeConservazioneRequest request)
        {
            GetCountIstanzeConservazioneResponse response = new GetCountIstanzeConservazioneResponse();

            try
            {
                DocsPaConservazione.Contatori contatori = DocsPaConservazione.Contatori.GetByCodice(request.CodiceAmministrazione);

                response.DaInviare = contatori.DaInviare;
                response.Inviate = contatori.Inviate;
                response.Rifiutate = contatori.Rifiutate;
                response.InLavorazione = contatori.InLavorazione;
                response.Firmate = contatori.Firmate;
                response.Conservate = contatori.Conservate;
                response.Chiuse = contatori.Chiuse;
                response.Notifiche = contatori.Notifiche;

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public GetCountIstanzeEsibizioneResponse GetCountIstanzeEsibizione(GetCountIstanzeEsibizioneRequest request)
        {
            GetCountIstanzeEsibizioneResponse response = new GetCountIstanzeEsibizioneResponse();

            try
            {
                DocsPaConservazione.ContatoriEsibizione contatori = DocsPaConservazione.ContatoriEsibizione.GetByCodiceConservazione(request.CodiceAmministrazione);

                response.DaCertificare = contatori.InAttesaDiCertificazione + contatori.InAttesaDiCertificazione_Certificata;
                
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// WebMethod GetUser per verificare la presenza di un utente nella People
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public GetUserResponse GetUser(GetUserRequest request)
        {
            //
            // Creazione della Response da ritornare al BE del PGU
            GetUserResponse response = new GetUserResponse();

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    //
                    // Creazione della query
                    const string QUERY = "select * from people p where upper(p.user_id) = upper('{0}' )and upper(getcodamm(p.id_Amm)) = upper('{1}')";

                    //
                    // Assegno i parametri alla query
                    string commandText = string.Format(QUERY, request.UserCode, request.Ente);

                    System.Data.DataSet ds;
                    //
                    // dovrebbe tornare una e una sola riga
                    dbProvider.ExecuteQuery(out ds, commandText);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        // Utente già esistente
                        System.Data.DataRow dataRow = ds.Tables[0].Rows[0];
                        string abilitato_cesntro_servizi = dataRow["ABILITATO_CENTRO_SERVIZI"].ToString();

                        response.AbilitatoCentroServizi = abilitato_cesntro_servizi.Equals("1") ? true : false;
                        response.UtentePresente = true;

                        ds.Dispose();
                    }
                    else
                    {
                        ds.Dispose();
                        response.UtentePresente = false;
                    }

                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// WebMethod InsertUser per l'inserimento di un utente nella People
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public InsertUserResponse InsertUser(InsertUserRequest request)
        {
            //
            // Creazione della Response da restituire al BE del PGU
            InsertUserResponse response = new InsertUserResponse();

            try
            {
                string idAmm = string.Empty;

                //
                // Reperimento idAmm a partire dal codice amministrazione
                idAmm = this.getIdAmmByCod(request.Ente);

                //
                // Controllo valore idAmm
                if (!string.IsNullOrEmpty(idAmm))
                {
                    //
                    // Impostazione Utente da inserire in PITRE
                    DocsPaVO.amministrazione.OrgUtente utente = new DocsPaVO.amministrazione.OrgUtente();

                    #region Impostazione parametri Utente
                    //
                    // Old Code
                    //utente.Abilitato = "0";
                    
                    // New Code
                    // Se il documentale è DOCUMENTUM, l'utente deve risultare abilitato per poter effettuare operazioni su di esso.
                    // Per preservare il requisito utente, viene eseguito un update solo sul documentale ETNOTEAM per la proprietà utente.Abilitato
                    if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["documentale"])
                        && System.Configuration.ConfigurationManager.AppSettings["documentale"].ToUpper().Equals("PITRE"))
                        utente.Abilitato = "1";
                    else
                        utente.Abilitato = "0";
                    //
                    // End New Code

                    utente.AbilitatoCentroServizi = true;
                    utente.AbilitatoChiaviConfigurazione = false;

                    //
                    // PEOPLE.cha_amministratore = 0
                    utente.Amministratore = "0";
                    // Censisco l'utente come systemAdministrator
                    //utente.Amministratore = "1";
                    utente.AutenticatoInLdap = false;
                    utente.Codice = request.UserCode;
                    utente.CodiceRubrica = request.UserCode;
                    utente.Cognome = request.Cognome;
                    utente.DispositivoStampa = null;
                    utente.Dominio = string.Empty;
                    utente.Email = request.Email;
                    utente.FromEmail = string.Empty;
                    utente.IDAmministrazione = idAmm;
                    utente.IdClientSideModelProcessor = 0;
                    utente.IDCorrGlobale = string.Empty;
                    utente.IDPeople = string.Empty;
                    utente.IdSincronizzazioneLdap = string.Empty;
                    utente.Matricola = string.Empty;
                    utente.NessunaScadenzaPassword = true;
                    utente.Nome = request.Nome;
                    utente.NotificaTrasm = "null";
                    
                    utente.Password = request.Password;

                    utente.Qualifiche = new List<DocsPaVO.Qualifica.PeopleGroupsQualifiche>();
                    utente.Sede = string.Empty;
                    utente.SincronizzaLdap = false;

                    // Configurazione SmartClient
                    #region smartClientConfiguration
                    DocsPaVO.SmartClient.SmartClientConfigurations scc = new DocsPaVO.SmartClient.SmartClientConfigurations();
                    scc.ApplyPdfConvertionOnScan = false;
                    scc.BrowserType = "null";
                    scc.ComponentsType = "1";
                    scc.IsEnabled = false;
                    utente.SmartClientConfigurations = scc;
                    #endregion

                    utente.UserId = request.UserCode.ToUpper();
                    #endregion

                    string result = string.Empty;

                    //
                    // Inserimento Nuovo Utente

                    // Old Code
                    //using (DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                    //    result = dbAmm.AmmInsNuovoUtente(utente);
                    // End Old Code
                    //

                    //
                    // -------- New - Post Mev MultiAmm ---------------
                    DocsPaVO.utente.InfoUtente infoUtente = null;
                    infoUtente = this.GetDatiAmministratorePGU("docspadmin".ToUpper(), "0");
                    infoUtente.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();
                    
                    //infoUtente.userId = "docspadmin".ToUpper();
                    
                    DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
                    try
                    {
                        esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmInsNuovoUtente(infoUtente, utente);
                        result = esito.Codice.ToString();
                        
                        //BusinessLogic.UserLog.UserLog.getInstance().WriteLog(infoUtente, "AMMNEWUSER", utente.IDPeople, "Creazione utente " + utente.Nome + " " + utente.Cognome, DocsPaVO.Logger.CodAzione.Esito.OK, idAmm);
                    }
                    catch (Exception e)
                    {
                        //BusinessLogic.UserLog.UserLog.getInstance().WriteLog(infoUtente, "AMMNEWUSER", utente.IDPeople, "Creazione utente " + utente.Nome + " " + utente.Cognome, DocsPaVO.Logger.CodAzione.Esito.KO, idAmm);
                        
                        // Log Applicativo

                        // Attività di LOG di emergenza.
                        // Da attivare per tracciare eventuali problemi in PRODUZIONE
                        // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                        // quindi il FE può rimanere in pagina bianca.
                        //System.IO.File.AppendAllText("c:\\pgulog.txt", "Errore in PGUServices.asmx  - metodo: InsertUser - AmmInsNuovoUtente: "+ e.Message + System.Environment.NewLine);

                        //logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmInsUtente - ", e);
                        
                        esito.Codice = 1;
                        esito.Descrizione = "si è verificato un errore";
                        result = esito.Codice.ToString();
                    }
                    //
                    // ---------- End New - Post Mev MultiAmm ------------

                    if (result.Equals("0"))
                    {
                        //
                        // Inserimento avvenuto correttamente
                        response.EsitoInserimento = true;

                        // New Code :
                        // Viene commentato poichè per il corretto funzionalento delle operazioni del centro servizi,
                        // è necessaria che l'utente che compie le operazioni sia Disabled = N
                        // Decommentare /* .. */ per ripristino
                        #region Modifica Utente
                        /*
                         * 
                        // Se il documentale è DOCUMENTUM, occorre disabilitare l'utente
                        if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["documentale"])
                            && System.Configuration.ConfigurationManager.AppSettings["documentale"].ToUpper().Equals("PITRE"))
                        {
                            //get SystemID dalla people
                            string idPeople = string.Empty;
                            
                            try
                            {
                                idPeople = BusinessLogic.Amministrazione.OrganigrammaManager.AmmGetIdPeopleByUserIdAndIdAmm(utente.UserId, utente.IDAmministrazione);
                            }
                            catch (Exception e) 
                            {
                                // Log Applicativo

                                // Attività di LOG di emergenza.
                                // Da attivare per tracciare eventuali problemi in PRODUZIONE
                                // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                                // quindi il FE può rimanere in pagina bianca.
                                //System.IO.File.AppendAllText("c:\\pgulog.txt", "Errore in PGUServices.asmx  - metodo: InsertUser - AmmGetIdPeopleByUserIdAndIdAmm: " + e.Message + System.Environment.NewLine);

                            }

                            if (!string.IsNullOrEmpty(idPeople))
                            {
                                try
                                {
                                    //
                                    // Disabilita utente
                                    esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmDisabilitaUtente(idPeople);
                                }
                                catch (Exception e) 
                                {
                                    //response.EsitoInserimento = false;

                                    // Attività di LOG di emergenza.
                                    // Da attivare per tracciare eventuali problemi in PRODUZIONE
                                    // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                                    // quindi il FE può rimanere in pagina bianca.
                                    //System.IO.File.AppendAllText("c:\\pgulog.txt", "Errore in PGUServices.asmx  - metodo: InsertUser - AmmDisabilitaUtente: " + e.Message + System.Environment.NewLine);

                                }

                                if (esito != null &&
                                    !string.IsNullOrEmpty(esito.Codice.ToString()) &&
                                    !esito.Codice.ToString().Equals("0"))
                                {
                                    //response.EsitoInserimento = false;
                                    // Log Applicativo

                                    // Attività di LOG di emergenza.
                                    // Da attivare per tracciare eventuali problemi in PRODUZIONE
                                    // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                                    // quindi il FE può rimanere in pagina bianca.
                                    //System.IO.File.AppendAllText("c:\\pgulog.txt", "Errore in PGUServices.asmx  - metodo: InsertUser - AmmDisabilitaUtente Esito: " + esito.Descrizione + System.Environment.NewLine);

                                }
                            }
                        }
                         *
                         */
                        #endregion
                        // End New Code
                        
                    }
                    else
                        response.EsitoInserimento = false;
                }
                else
                {
                    //
                    // IdAmm non trovato
                    response.EsitoInserimento = false;
                }
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Funzione per reperire l'idAmm a partire dal codice amministrazione
        /// </summary>
        /// <param name="CodAmm"></param>
        /// <returns></returns>
        private string getIdAmmByCod(string CodAmm)
        {
            string retval = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                //
                // Creazione della query
                const string QUERY = "select a.SYSTEM_ID from DPA_AMMINISTRA a where upper(a.VAR_CODICE_AMM) = upper('{0}')";

                //
                // Assegno i parametri alla query
                string commandText = string.Format(QUERY, CodAmm);
                string idAmm = string.Empty;

                //
                // dovrebbe tornare una e una sola riga
                dbProvider.ExecuteScalar(out idAmm, commandText);

                retval = idAmm;
            }

            return retval;

        }



        /// <summary>
        /// Funzione per recuperare le informazioni sull'infoUtenteAmministratore dal PGU
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        [WebMethod()]
        public DocsPaVO.amministrazione.InfoUtenteAmministratore GetDatiAmministratorePGU(string userid, string idAmm)
        {
            DocsPaVO.amministrazione.InfoUtenteAmministratore result = null;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_LOGIN_PGU");
            queryDef.setParam("userId", userid);
            queryDef.setParam("paramIdAmm", idAmm);

            string commandText = queryDef.getSQL();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                    {
                        result = new DocsPaVO.amministrazione.InfoUtenteAmministratore();
                        result.userId = userid;
                        FetchDatiAmministratore(result, reader);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Creazione oggetto "DatiAmministratore"
        /// </summary>
        /// <param name="datiAmministratore"></param>
        /// <param name="reader"></param>
        private static void FetchDatiAmministratore(DocsPaVO.amministrazione.InfoUtenteAmministratore datiAmministratore, System.Data.IDataReader reader)
        {
            datiAmministratore.idPeople = reader.GetValue(reader.GetOrdinal("ID")).ToString();
            datiAmministratore.tipoAmministratore = reader.GetValue(reader.GetOrdinal("TIPO")).ToString();
            datiAmministratore.nome = reader.GetValue(reader.GetOrdinal("NOME")).ToString();
            datiAmministratore.cognome = reader.GetValue(reader.GetOrdinal("COGNOME")).ToString();
            datiAmministratore.idAmministrazione = reader.GetValue(reader.GetOrdinal("IDAMM")).ToString();
            datiAmministratore.idCorrGlobali = reader.GetValue(reader.GetOrdinal("ID_CORR_GLOBALI")).ToString();

            //if (!datiAmministratore.tipoAmministratore.Equals("1"))
            //  datiAmministratore.idCorrGlobali = getIdCorrispondente(datiAmministratore.idPeople);
        }

        /// <summary>
        /// WebMethod per reperire l'idAmm a partire dal codice amministrazione
        /// </summary>
        /// <param name="CodAmm"></param>
        /// <returns></returns>
        [WebMethod()]
        public string getIdAmmByCodice(string CodAmm)
        {
            return this.getIdAmmByCod(CodAmm);
        }

        /// <summary>
        /// WebMethod UpdateUser per l'aggiornamento di un utente nella People
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public UpdateUserResponse UpdatetUser(UpdateUserRequest request)
        {
            //
            // Creazione della response da ritornare al BE del PGU
            UpdateUserResponse response = new UpdateUserResponse();

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    //
                    // Update
                    bool retVal = false;

                    string codUtente = string.Empty;
                    string codAmm = string.Empty;

                    codUtente = request.UserCode;
                    codAmm = request.Ente;

                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_People");
                    q.setParam("param1", "ABILITATO_CENTRO_SERVIZI = '1'");
                    q.setParam("param2", "upper(user_id) = upper('" + codUtente + "' )and getcodamm(id_Amm)='" + codAmm + "'");

                    int rowsAffected = 0;
                    retVal = dbProvider.ExecuteNonQuery(q.getSQL(), out rowsAffected);

                    if (rowsAffected > 0)
                    {
                        response.EsitoAggiornamento = true;
                    }
                    else
                    {
                        response.EsitoAggiornamento = false;
                    }

                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public GetStampaRegistroResponse GetStampaRegistro(GetStampaRegistroRequest request)
        {
            GetStampaRegistroResponse response = new GetStampaRegistroResponse();

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    //
                    // Creazione della query di Select
                    //Gabriele Melini 24-09-2013
                    //modifiche per gestione ora di stampa
                    //string QUERY = "Select s.PRINT_FREQ||'_'||s.DISABLED as frequenza_disabled from DPA_CONFIG_STAMPA_CONS s where id_amm = {0}";
                    string QUERY = "Select s.PRINT_FREQ||'_'||s.DISABLED||'_'||s.print_hour as frequenza_disabled from DPA_CONFIG_STAMPA_CONS s where id_amm = {0}";
                    string idAmm = string.Format("{0}", request.idAmm);
                    string commandText = string.Format(QUERY, idAmm);
                    //System.IO.File.AppendAllText("c:\\pgulog.txt", commandText + "\\n");
                    
                    // Attività di LOG di emergenza.
                    // Da attivare per tracciare eventuali problemi in PRODUZIONE
                    // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                    // quindi il FE può rimanere in pagina bianca.
                    //System.IO.File.AppendAllText("c:\\pgulog.txt", commandText + System.Environment.NewLine);
                    
                    //
                    // Esecuzione query
                    string field;
                    if (!dbProvider.ExecuteScalar(out field, commandText))
                        throw new ApplicationException(dbProvider.LastExceptionMessage);

                    if (string.IsNullOrEmpty(field))
                        throw new ApplicationException(string.Format("La configurazione non risulta censito nel sistema per il seguente idAmm: '{0}'", idAmm));
                    else
                    {
                        response.Success = true;
                        //System.IO.File.AppendAllText("c:\\pgulog.txt", "Success \\n");

                        // Attività di LOG di emergenza.
                        // Da attivare per tracciare eventuali problemi in PRODUZIONE
                        // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                        // quindi il FE può rimanere in pagina bianca.
                        //System.IO.File.AppendAllText("c:\\pgulog.txt", "Success " + System.Environment.NewLine);
                        
                        //
                        // Recupero i valori:
                        response.FrequenzaStampaRegistro = field.ToString().Split('_')[0];
                        //
                        // Se DISABLED = 0 => Abilitato = true;
                        // Se DISABLED = 1 => Abilitato = false;
                        response.Abilitato = (field.ToString().Split('_')[1].Equals("1") ? false : true);
                        //Gabriele Melini 24-09-2013
                        //ora stampa
                        response.OraStampa = field.ToString().Split('_')[2];
                        response.Exception = null;
                    }
                }
            }
            catch (Exception ex)
            {
                //System.IO.File.AppendAllText("c:\\pgulog.txt", ex.Message + "\\n");

                // Attività di LOG di emergenza.
                // Da attivare per tracciare eventuali problemi in PRODUZIONE
                // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                // quindi il FE può rimanere in pagina bianca.
                //System.IO.File.AppendAllText("c:\\pgulog.txt", ex.Message + System.Environment.NewLine);
                
                response.Success = false;
                response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public SaveStampaRegistroResponse SaveStampaRegistro(SaveStampaRegistroRequest request)
        {
            SaveStampaRegistroResponse response = new SaveStampaRegistroResponse();

            try
            {
                string system_id = string.Empty;

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    dbProvider.BeginTransaction();

                    //GM 24-09-2013 nuova query con ora di stampa
                    //DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_CONFIG_STAMPA_CONS");
                    DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_CONFIG_STAMPA_CONS_NEW");
                    //
                    // Modifica per SQL
                    query.setParam("col_id", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    // End Modifica

                    query.setParam("seq", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CONFIG_STAMPA_CONS"));
                    query.setParam("id_amm", request.idAmm.ToString());
                    query.setParam("disabled", request.disabled.ToString());
                    //query.setParam("printer_role_id", DBNull.Value.ToString());
                    //query.setParam("printer_user_id", DBNull.Value.ToString());
                    query.setParam("print_freq", request.print_freq.ToString());
                    //query.setParam("dta_last_print", DBNull.Value.ToString());
                    //query.setParam("dta_next_print", DBNull.Value.ToString());
                    query.setParam("print_hour", request.print_hour.ToString());
                    switch (request.print_freq)
                    {
                        case 10:
                            double day = 1;
                            //query.setParam("dta_next_print", (DateTime.Now.AddDays(day)).ToString("dd/MM/yyyy"));
                            query.setParam("dta_next_print", (DateTime.Now.AddDays(day)).ToShortDateString());
                            //System.IO.File.AppendAllText("c:\\pgulog.txt", DateTime.Now.AddDays(day).ToShortDateString() + "\\n");

                            // Attività di LOG di emergenza.
                            // Da attivare per tracciare eventuali problemi in PRODUZIONE
                            // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                            // quindi il FE può rimanere in pagina bianca.
                            //System.IO.File.AppendAllText("c:\\pgulog.txt", DateTime.Now.AddDays(day).ToShortDateString() + System.Environment.NewLine);

                            break;
                        case 20:
                            double days = 7;
                            //query.setParam("dta_next_print", (DateTime.Now.AddDays(days)).ToString("dd/MM/yyyy"));
                            query.setParam("dta_next_print", (DateTime.Now.AddDays(days)).ToShortDateString());
                            //System.IO.File.AppendAllText("c:\\pgulog.txt", (DateTime.Now.AddDays(days)).ToShortDateString() + "\\n");

                            // Attività di LOG di emergenza.
                            // Da attivare per tracciare eventuali problemi in PRODUZIONE
                            // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                            // quindi il FE può rimanere in pagina bianca.
                            //System.IO.File.AppendAllText("c:\\pgulog.txt", (DateTime.Now.AddDays(days)).ToShortDateString() + System.Environment.NewLine);

                            break;
                        case 30:
                            int month = 1;
                            //query.setParam("dta_next_print", (DateTime.Now.AddMonths(month)).ToString("dd/MM/yyyy"));
                            query.setParam("dta_next_print", (DateTime.Now.AddMonths(month)).ToShortDateString());
                            //System.IO.File.AppendAllText("c:\\pgulog.txt", (DateTime.Now.AddMonths(month)).ToShortDateString() + "\\n");

                            // Attività di LOG di emergenza.
                            // Da attivare per tracciare eventuali problemi in PRODUZIONE
                            // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                            // quindi il FE può rimanere in pagina bianca.
                            //System.IO.File.AppendAllText("c:\\pgulog.txt", (DateTime.Now.AddMonths(month)).ToShortDateString() + System.Environment.NewLine);

                            break;
                        case 40:
                            int year = 1;
                            //query.setParam("dta_next_print", (DateTime.Now.AddYears(year)).ToString("dd/MM/yyyy"));
                            query.setParam("dta_next_print", (DateTime.Now.AddYears(year)).ToShortDateString());
                            //System.IO.File.AppendAllText("c:\\pgulog.txt", (DateTime.Now.AddYears(year)).ToShortDateString() + "\\n");

                            // Attività di LOG di emergenza.
                            // Da attivare per tracciare eventuali problemi in PRODUZIONE
                            // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                            // quindi il FE può rimanere in pagina bianca.
                            //System.IO.File.AppendAllText("c:\\pgulog.txt", (DateTime.Now.AddYears(year)).ToShortDateString() + System.Environment.NewLine);

                            break;
                    }
                    //query.setParam("id_last_printed_event", DBNull.Value.ToString());

                    string commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - SaveStampaRegistro - QUERY : " + commandText);
                   
                    if (!dbProvider.ExecuteNonQuery(commandText))
                    {
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }
                    else
                    {
                        // Recupero systemid appena inserito
                        string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_CONFIG_STAMPA_CONS");
                        System.Diagnostics.Debug.WriteLine("SQL - SaveStampaRegistro - QUERY : " + sql);
                        dbProvider.ExecuteScalar(out system_id, sql);

                        dbProvider.CommitTransaction();
                        //System.IO.File.AppendAllText("c:\\pgulog.txt", "Commit Query: " + commandText + "\\n");

                        // Attività di LOG di emergenza.
                        // Da attivare per tracciare eventuali problemi in PRODUZIONE
                        // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                        // quindi il FE può rimanere in pagina bianca.
                        //System.IO.File.AppendAllText("c:\\pgulog.txt", "Commit Query: " + commandText + System.Environment.NewLine);

                        //
                        // Inserimento avvenuto correttamente
                        response.Success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Exception = ex.Message;
                //System.IO.File.AppendAllText("c:\\pgulog.txt", "Save stampa registro Exception: " + ex.Message + "\\n");

                // Attività di LOG di emergenza.
                // Da attivare per tracciare eventuali problemi in PRODUZIONE
                // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                // quindi il FE può rimanere in pagina bianca.
                //System.IO.File.AppendAllText("c:\\pgulog.txt", "Save stampa registro Exception: " + ex.Message + System.Environment.NewLine);

            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public UpdateStampaRegistroResponse UpdateStampaRegistro(UpdateStampaRegistroRequest request)
        {
            UpdateStampaRegistroResponse response = new UpdateStampaRegistroResponse();

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    dbProvider.BeginTransaction();

                    //
                    // Get DTA_LAST_PRINT
                    #region Recupero campo DTA_LAST_PRINT

                    DocsPaUtils.Query query_DTA_LAST_PRINT = DocsPaUtils.InitQuery.getInstance().getQuery("S_DTA_LAST_PRINT_FROM_DPA_CONFIG_STAMPA_CONS");
                    query_DTA_LAST_PRINT.setParam("idAmm", request.idAmm.ToString());

                    string commandText_DTA_LAST_PRINT = query_DTA_LAST_PRINT.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - S_DTA_LAST_PRINT_FROM_DPA_CONFIG_STAMPA_CONS - QUERY : " + commandText_DTA_LAST_PRINT);

                    //
                    // Esecuzione query
                    string field_DTA_LAST_PRINT;
                    try
                    {
                        if (!dbProvider.ExecuteScalar(out field_DTA_LAST_PRINT, commandText_DTA_LAST_PRINT))
                            throw new ApplicationException(dbProvider.LastExceptionMessage);

                        if (string.IsNullOrEmpty(field_DTA_LAST_PRINT))
                            throw new ApplicationException(string.Format("La configurazione non risulta censita nel sistema per il seguente idAmm: '{0}'", request.idAmm.ToString()));
                    }
                    catch (Exception e)
                    {
                        //
                        //Non posso prelevare il campo DTA_LAST_PRINT
                        field_DTA_LAST_PRINT = string.Empty;
                    }

                    string dta_next_print = string.Empty;

                    //
                    // Controllo il valore del camp DTA_LAST_PRINT
                    if (string.IsNullOrEmpty(field_DTA_LAST_PRINT))
                    {
                        //
                        // Imposto la data di ultima stampa alla data odierna + l'intervallo della frequenza di stampa, poichè essa non è ancora stata definita.
                        switch (request.print_freq)
                        {
                            case 10:
                                double day = 1;
                                //dta_next_print = (DateTime.Now.AddDays(day)).ToString("dd/MM/yyyy");
                                dta_next_print = (DateTime.Now.AddDays(day)).ToShortDateString();
                                //System.IO.File.AppendAllText("c:\\pgulog.txt", (DateTime.Now.AddDays(day)).ToShortDateString() + "\\n");

                                // Attività di LOG di emergenza.
                                // Da attivare per tracciare eventuali problemi in PRODUZIONE
                                // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                                // quindi il FE può rimanere in pagina bianca.
                                //System.IO.File.AppendAllText("c:\\pgulog.txt", (DateTime.Now.AddDays(day)).ToShortDateString() + System.Environment.NewLine);

                                break;
                            case 20:
                                double days = 7;
                                //dta_next_print = (DateTime.Now.AddDays(days)).ToString("dd/MM/yyyy");
                                dta_next_print = (DateTime.Now.AddDays(days)).ToShortDateString();
                                //System.IO.File.AppendAllText("c:\\pgulog.txt", (DateTime.Now.AddDays(days)).ToShortDateString() + "\\n");

                                // Attività di LOG di emergenza.
                                // Da attivare per tracciare eventuali problemi in PRODUZIONE
                                // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                                // quindi il FE può rimanere in pagina bianca.
                                //System.IO.File.AppendAllText("c:\\pgulog.txt", (DateTime.Now.AddDays(days)).ToShortDateString() + System.Environment.NewLine);

                                break;
                            case 30:
                                int month = 1;
                                //dta_next_print = (DateTime.Now.AddMonths(month)).ToString("dd/MM/yyyy");
                                dta_next_print = (DateTime.Now.AddMonths(month)).ToShortDateString();
                                //System.IO.File.AppendAllText("c:\\pgulog.txt", (DateTime.Now.AddMonths(month)).ToShortDateString() + "\\n");

                                // Attività di LOG di emergenza.
                                // Da attivare per tracciare eventuali problemi in PRODUZIONE
                                // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                                // quindi il FE può rimanere in pagina bianca.
                                //System.IO.File.AppendAllText("c:\\pgulog.txt", (DateTime.Now.AddMonths(month)).ToShortDateString() + System.Environment.NewLine);

                                break;
                            case 40:
                                int year = 1;
                                //dta_next_print = (DateTime.Now.AddYears(year)).ToString("dd/MM/yyyy");
                                dta_next_print = (DateTime.Now.AddYears(year)).ToShortDateString();
                                //System.IO.File.AppendAllText("c:\\pgulog.txt", (DateTime.Now.AddYears(year)).ToShortDateString() + "\\n");

                                // Attività di LOG di emergenza.
                                // Da attivare per tracciare eventuali problemi in PRODUZIONE
                                // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                                // quindi il FE può rimanere in pagina bianca.
                                //System.IO.File.AppendAllText("c:\\pgulog.txt", (DateTime.Now.AddYears(year)).ToShortDateString() + System.Environment.NewLine);

                                break;
                        }
                    }
                    else
                    {
                        //
                        // Converto la data prelevata in un DateTime
                        DateTime DTA_LAST_PRINT_DateTime = DateTime.Parse(field_DTA_LAST_PRINT);

                        //
                        // Imposto la data della prossima stampa:
                        // Se la data (dell'ultima stampa + la frequenza) > della data odierna => data prossima stampa = ultima stampa + la frequenza
                        // Se la data (dell'ultima stampa + la frequenza) < della data odierna => data prossima stampa = data odierna + la frequenza

                        switch (request.print_freq)
                        {
                            case 10:
                                double day = 1;
                                //Gabriele Melini 20-09-2013
                                //con l'introduzione dell'ora di stampa il confronte deve
                                //essere effettuato SOLO sulle date
                                if (DTA_LAST_PRINT_DateTime.AddDays(day).Date < DateTime.Now.Date)
                                    //dta_next_print = (DateTime.Now.AddDays(day)).ToString("dd/MM/yyyy");
                                    dta_next_print = (DateTime.Now.AddDays(day)).ToShortDateString();
                                else
                                {
                                    //Gabriele Melini 20-09-2013
                                    //se l'ora di stampa impostata è minore dell'ora attuale, la stampa viene schedulata al giorno successivo
                                    if (DTA_LAST_PRINT_DateTime.AddDays(day).Date == DateTime.Now.Date && (DateTime.Now.Hour >= Convert.ToInt32(request.print_hour)))
                                        dta_next_print = (DTA_LAST_PRINT_DateTime.AddDays(day + 1)).ToShortDateString();
                                    else
                                        //dta_next_print = (DTA_LAST_PRINT_DateTime.AddDays(day)).ToString("dd/MM/yyyy");
                                        dta_next_print = (DTA_LAST_PRINT_DateTime.AddDays(day)).ToShortDateString();
                                }

                                // Attività di LOG di emergenza.
                                // Da attivare per tracciare eventuali problemi in PRODUZIONE
                                // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                                // quindi il FE può rimanere in pagina bianca.
                                //System.IO.File.AppendAllText("c:\\pgulog.txt", dta_next_print + System.Environment.NewLine);

                                break;
                            case 20:
                                double days = 7;
                                if (DTA_LAST_PRINT_DateTime.AddDays(days).Date < DateTime.Now.Date)
                                    //dta_next_print = (DateTime.Now.AddDays(days)).ToString("dd/MM/yyyy");
                                    dta_next_print = (DateTime.Now.AddDays(days)).ToShortDateString();
                                else
                                {
                                    if (DTA_LAST_PRINT_DateTime.AddDays(days).Date == DateTime.Now.Date && (DateTime.Now.Hour >= Convert.ToInt32(request.print_hour)))
                                        dta_next_print = (DTA_LAST_PRINT_DateTime.AddDays(days + 1)).ToShortDateString();
                                    else
                                        //dta_next_print = (DTA_LAST_PRINT_DateTime.AddDays(days)).ToString("dd/MM/yyyy");
                                        dta_next_print = (DTA_LAST_PRINT_DateTime.AddDays(days)).ToShortDateString();
                                    //System.IO.File.AppendAllText("c:\\pgulog.txt", dta_next_print + "\\n");
                                }

                                // Attività di LOG di emergenza.
                                // Da attivare per tracciare eventuali problemi in PRODUZIONE
                                // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                                // quindi il FE può rimanere in pagina bianca.
                                //System.IO.File.AppendAllText("c:\\pgulog.txt", dta_next_print + System.Environment.NewLine);

                                break;
                            case 30:
                                int month = 1;
                                if (DTA_LAST_PRINT_DateTime.AddMonths(month).Date < DateTime.Now.Date)
                                    //dta_next_print = (DateTime.Now.AddDays(month)).ToString("dd/MM/yyyy");
                                    dta_next_print = (DateTime.Now.AddDays(month)).ToShortDateString();
                                else
                                {
                                    if (DTA_LAST_PRINT_DateTime.AddMonths(month).Date == DateTime.Now.Date && (DateTime.Now.Hour >= Convert.ToInt32(request.print_hour)))
                                        dta_next_print = (DTA_LAST_PRINT_DateTime.AddMonths(month).AddDays(1)).ToShortDateString();
                                    else
                                        //dta_next_print = (DTA_LAST_PRINT_DateTime.AddMonths(month)).ToString("dd/MM/yyyy");
                                        dta_next_print = (DTA_LAST_PRINT_DateTime.AddMonths(month)).ToShortDateString();
                                    //System.IO.File.AppendAllText("c:\\pgulog.txt", dta_next_print + "\\n");
                                }

                                // Attività di LOG di emergenza.
                                // Da attivare per tracciare eventuali problemi in PRODUZIONE
                                // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                                // quindi il FE può rimanere in pagina bianca.
                                //System.IO.File.AppendAllText("c:\\pgulog.txt", dta_next_print + System.Environment.NewLine);

                                break;
                            case 40:
                                int year = 1;
                                if (DTA_LAST_PRINT_DateTime.AddYears(year).Date < DateTime.Now.Date)
                                    //dta_next_print = (DateTime.Now.AddDays(year)).ToString("dd/MM/yyyy");
                                    dta_next_print = (DateTime.Now.AddDays(year)).ToShortDateString();
                                else
                                {
                                    if (DTA_LAST_PRINT_DateTime.AddYears(year).Date == DateTime.Now.Date && (DateTime.Now.Hour >= Convert.ToInt32(request.print_hour)))
                                        dta_next_print = (DTA_LAST_PRINT_DateTime.AddYears(year).AddDays(1)).ToShortDateString();
                                    else
                                        //dta_next_print = (DTA_LAST_PRINT_DateTime.AddYears(year)).ToString("dd/MM/yyyy");
                                        dta_next_print = (DTA_LAST_PRINT_DateTime.AddYears(year)).ToShortDateString();
                                    //System.IO.File.AppendAllText("c:\\pgulog.txt", dta_next_print + "\\n");
                                }

                                // Attività di LOG di emergenza.
                                // Da attivare per tracciare eventuali problemi in PRODUZIONE
                                // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                                // quindi il FE può rimanere in pagina bianca.
                                //System.IO.File.AppendAllText("c:\\pgulog.txt", dta_next_print + System.Environment.NewLine);

                                break;
                        }
                    }

                    #endregion

                    //GM 24-09-2013 nuova query con ora di stampa
                    //DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_CONFIG_STAMPA_CONS");
                    DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_CONFIG_STAMPA_CONS_NEW");

                    query.setParam("disbled", request.disabled.ToString());
                    query.setParam("print_freq", request.print_freq.ToString());
                    query.setParam("print_hour", request.print_hour.ToString());

                    //
                    // Campo DTA_NEXT_PRINT
                    query.setParam("dta_next_print", dta_next_print);

                    query.setParam("idAmm", request.idAmm.ToString());

                    string commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - UpdateStampaRegistro - QUERY : " + commandText);

                    if (!dbProvider.ExecuteNonQuery(commandText))
                    {
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }
                    else
                    {
                        dbProvider.CommitTransaction();
                        //System.IO.File.AppendAllText("c:\\pgulog.txt", "Commit Update stampa registro \\n");

                        // Attività di LOG di emergenza.
                        // Da attivare per tracciare eventuali problemi in PRODUZIONE
                        // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                        // quindi il FE può rimanere in pagina bianca.
                        //System.IO.File.AppendAllText("c:\\pgulog.txt", "Commit Update stampa registro " + System.Environment.NewLine);

                        response.Success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Exception = ex.Message;
                //System.IO.File.AppendAllText("c:\\pgulog.txt", "Exception Update stampa registro: " + ex.Message + "\\n");

                // Attività di LOG di emergenza.
                // Da attivare per tracciare eventuali problemi in PRODUZIONE
                // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                // quindi il FE può rimanere in pagina bianca.
                //System.IO.File.AppendAllText("c:\\pgulog.txt", "Exception Update stampa registro: " + ex.Message + System.Environment.NewLine);

            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public SaveFormatiResponse SaveFormati(SaveFormatiRequest request)
        {
            SaveFormatiResponse response = new SaveFormatiResponse();

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    dbProvider.BeginTransaction();

                    DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("U_UPDATE_SUPPORTED_FILE_TYPES_FROM_PGU");

                    //query.setParam("disbled", request.disabled.ToString());
                    //query.setParam("print_freq", request.print_freq.ToString());

                    if (request.FileTypePreservation)
                        query.setParam("fileTypePreservation", "1");
                    else
                        query.setParam("fileTypePreservation", "0");

                    if (request.FileTypeValidation)
                        query.setParam("fileTypeValidation", "1");
                    else
                        query.setParam("fileTypeValidation", "0");

                    query.setParam("systemId", request.SystemId.ToString());

                    string commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - UpdateSupportedFileTypeFromPgu - QUERY : " + commandText);

                    //System.IO.File.AppendAllText("c:\\pgulog.txt", "commandText \\n");

                    // Attività di LOG di emergenza.
                    // Da attivare per tracciare eventuali problemi in PRODUZIONE
                    // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                    // quindi il FE può rimanere in pagina bianca.
                    //System.IO.File.AppendAllText("c:\\pgulog.txt", "commandText " + System.Environment.NewLine);


                    if (!dbProvider.ExecuteNonQuery(commandText))
                    {
                        dbProvider.RollbackTransaction();
                        //System.IO.File.AppendAllText("c:\\pgulog.txt", "rollback \\n");

                        // Attività di LOG di emergenza.
                        // Da attivare per tracciare eventuali problemi in PRODUZIONE
                        // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                        // quindi il FE può rimanere in pagina bianca.
                        //System.IO.File.AppendAllText("c:\\pgulog.txt", "rollback " + System.Environment.NewLine);

                        throw new Exception();
                    }
                    else
                    {
                        dbProvider.CommitTransaction();
                        //System.IO.File.AppendAllText("c:\\pgulog.txt", "commit \\n");

                        // Attività di LOG di emergenza.
                        // Da attivare per tracciare eventuali problemi in PRODUZIONE
                        // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                        // quindi il FE può rimanere in pagina bianca.
                        //System.IO.File.AppendAllText("c:\\pgulog.txt", "commit " + System.Environment.NewLine);

                        response.Success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Exception = ex.Message;
            }

            return response;
        }


        #region MEV CS 1.5

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public GetVerifichePeriodicheResponse GetVerifichePeriodiche(GetVerifichePeriodicheRequest request)
        {
            GetVerifichePeriodicheResponse response = new GetVerifichePeriodicheResponse();

            try
            {
                // reperimento delle chiavi di configurazione
                string output = string.Empty;

                string query = " SELECT var_valore, var_codice FROM dpa_chiavi_configurazione WHERE id_amm=" + request.idAmm +
                               " AND var_codice IN ('BE_CONSERVAZIONE_INTERVALLO', 'BE_CONSERVAZIONE_GG_NOTIFICHE', 'BE_CONS_VER_LEG_INTERVALLO', 'BE_CONS_VER_LEG_GG_NOTIFICHE') " + 
                               " ORDER BY var_codice ";

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(query))
                    {
                        while (reader.Read())
                        {
                            output = output + reader.GetValue(reader.GetOrdinal("var_valore")).ToString() + "§";
                        }
                    }

                    output = output.Substring(0, output.Length - 1);

                    if (string.IsNullOrEmpty(output))
                    {
                        throw new Exception("La configurazione non è censita per l'amministrazione corrente - " + request.idAmm);
                    }
                    else
                    {
                        // inserisco i parametri nella response
                        response.GiorniNotificheIntegrita = output.Split('§')[0];
                        response.ScadenzaIntegrita = output.Split('§')[1];
                        response.GiorniNotificheLeggibilita = output.Split('§')[2];
                        response.ScadenzaLeggibilita = output.Split('§')[3];

                        response.Success = true;
                        response.Exception = null;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Exception = ex.Message;
            }

            return response;

        }

        [WebMethod()]
        public SaveVerifichePeriodicheResponse SaveVerifichePeriodiche(SaveVerifichePeriodicheRequest request)
        {
            SaveVerifichePeriodicheResponse response = new SaveVerifichePeriodicheResponse();

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    dbProvider.BeginTransaction();

                    // reperimento delle chiavi esistenti
                    string output = string.Empty;
                    string query = " SELECT var_valore, var_codice FROM dpa_chiavi_configurazione WHERE id_amm=" + request.idAmm +
                                   " AND var_codice IN ('BE_CONSERVAZIONE_INTERVALLO', 'BE_CONSERVAZIONE_GG_NOTIFICHE', 'BE_CONS_VER_LEG_INTERVALLO', 'BE_CONS_VER_LEG_GG_NOTIFICHE') " +
                                   " ORDER BY var_codice ";

                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(query))
                    {
                        while (reader.Read())
                        {
                            output = output + reader.GetValue(reader.GetOrdinal("var_valore")).ToString() + "§";
                        }

                    }

                    output = output.Substring(0, output.Length - 1);

                    if (!string.IsNullOrEmpty(output))
                    {
                        string giorniNotificheIntegrita = output.Split('§')[0];
                        string intervalloIntegrita = output.Split('§')[1];
                        string giorniNotificheLeggibilita = output.Split('§')[2];
                        string intervalloLeggibilita = output.Split('§')[3];

                        #region parametri verifiche integrità
                        if (!(intervalloIntegrita == request.ScadenzaIntegrita))
                        {
                            string q = string.Format(" UPDATE dpa_chiavi_configurazione SET var_valore={0} WHERE id_amm={1} AND var_codice='BE_CONSERVAZIONE_INTERVALLO'", request.ScadenzaIntegrita, request.idAmm.ToString());
                            if (!dbProvider.ExecuteNonQuery(q))
                            {
                                dbProvider.RollbackTransaction();
                                throw new Exception();
                            }
                        }
                        if (!(giorniNotificheIntegrita == request.GiorniNotificheIntegrita))
                        {
                            string q = string.Format(" UPDATE dpa_chiavi_configurazione SET var_valore={0} WHERE id_amm={1} AND var_codice='BE_CONSERVAZIONE_GG_NOTIFICHE'", request.GiorniNotificheIntegrita, request.idAmm.ToString());
                            if (!dbProvider.ExecuteNonQuery(q))
                            {
                                dbProvider.RollbackTransaction();
                                throw new Exception();
                            }
                        }
                        #endregion

                        #region parametri verifiche leggibilità
                        if (!(intervalloLeggibilita == request.ScadenzaLeggibilita))
                        {
                            string q = string.Format(" UPDATE dpa_chiavi_configurazione SET var_valore={0} WHERE id_amm={1} AND var_codice='BE_CONS_VER_LEG_INTERVALLO'", request.ScadenzaLeggibilita, request.idAmm.ToString());
                            if (!dbProvider.ExecuteNonQuery(q))
                            {
                                dbProvider.RollbackTransaction();
                                throw new Exception();
                            }
                        }
                        if (!(giorniNotificheLeggibilita == request.GiorniNotificheLeggibilita))
                        {
                            string q = string.Format(" UPDATE dpa_chiavi_configurazione SET var_valore={0} WHERE id_amm={1} AND var_codice='BE_CONS_VER_LEG_GG_NOTIFICHE'", request.GiorniNotificheLeggibilita, request.idAmm.ToString());
                            if (!dbProvider.ExecuteNonQuery(q))
                            {
                                dbProvider.RollbackTransaction();
                                throw new Exception();
                            }
                        }
                        #endregion

                        dbProvider.CommitTransaction();
                        response.Success = true;
                    }
                    else
                    {
                        throw new Exception("La configurazione non risulta censita per l'amministrazione " + request.idAmm.ToString());
                    }

                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Exception = ex.Message;
            }

            return response;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public GetGestioneAlertResponse GetGestioneAlert(GetGestioneAlertRequest request)
        {
            GetGestioneAlertResponse response = new GetGestioneAlertResponse();
            int counter = 0;

            try
            {

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONS_ALERT_GET_CONF");
                    query.setParam("idAmm", request.idAmm);
                    string commandText = query.getSQL();

                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        
                        while (reader.Read())
                        {
                            counter++;
                            response.abilitatoVerificaAnt = (reader.GetValue(reader.GetOrdinal("cha_alert_leggibilita_scadenza")).ToString() == "1") ? true : false;
                            response.abilitatoVerificaPerc = (reader.GetValue(reader.GetOrdinal("cha_alert_leggibilita_max_doc")).ToString() == "1") ? true : false;
                            response.maxVerificaPerc = reader.GetValue(reader.GetOrdinal("num_leggibilita_max_doc_perc")).ToString();
                            response.abilitatoVerificaDoc = (reader.GetValue(reader.GetOrdinal("cha_alert_leggibilita_sing")).ToString() == "1") ? true : false;
                            response.maxOperVerificaDoc = reader.GetValue(reader.GetOrdinal("num_legg_sing_max_oper")).ToString();
                            response.periodoMonVerificaDoc = reader.GetValue(reader.GetOrdinal("num_legg_sing_periodo_mon")).ToString();
                            response.abilitatoDownload = (reader.GetValue(reader.GetOrdinal("cha_alert_download")).ToString() == "1") ? true : false;
                            response.maxOperDownload = reader.GetValue(reader.GetOrdinal("num_download_max_oper")).ToString();
                            response.periodoMonDownload = reader.GetValue(reader.GetOrdinal("num_download_periodo_mon")).ToString();
                            response.abilitatoSfoglia = (reader.GetValue(reader.GetOrdinal("cha_alert_sfoglia")).ToString() == "1") ? true : false;
                            response.maxOperSfoglia = reader.GetValue(reader.GetOrdinal("num_sfoglia_max_oper")).ToString();
                            response.periodoMonSfoglia = reader.GetValue(reader.GetOrdinal("num_sfoglia_periodo_mon")).ToString();

                            response.serverSMTP = (reader.GetValue(reader.GetOrdinal("var_server_smtp")).ToString()).Trim();
                            response.portaSMTP = reader.GetValue(reader.GetOrdinal("num_porta_smtp")).ToString();
                            response.useSSL = (reader.GetValue(reader.GetOrdinal("cha_smtp_ssl")).ToString() == "1") ? true : false;
                            response.userSMTP = (reader.GetValue(reader.GetOrdinal("var_user_mail")).ToString()).Trim();
                            response.pwdSMTP = DocsPaUtils.Security.Crypter.Decode((reader.GetValue((reader.GetOrdinal("var_pwd_mail"))).ToString()).Trim(), response.userSMTP);
                            response.mailFrom = (reader.GetValue(reader.GetOrdinal("var_mail_notifica")).ToString()).Trim();
                            response.mailTo = (reader.GetValue(reader.GetOrdinal("var_mail_destinatario")).ToString()).Trim();

                        }
                    }

                    if (counter == 1)
                    {
                        response.Success = true;
                        response.Exception = null;
                    }
                    else
                    {
                        if (counter == 0)
                            throw new Exception("La configurazione non è presente per l'amministrazione id=" + request.idAmm);
                        else
                            throw new Exception("Errore nel recupero della configurazione per l'amministrazione id=" + request.idAmm);
                    }
                }


            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Exception = ex.Message;
            }


            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public SaveGestioneAlertResponse SaveGestioneAlert(SaveGestioneAlertRequest request)
        {
            SaveGestioneAlertResponse response = new SaveGestioneAlertResponse();

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    dbProvider.BeginTransaction();
                    string system_id = string.Empty;

                    int alertAttivi = 0;

                    // conto il numero di alert attivi
                    if (request.abilitatoVerificaAnt)
                        alertAttivi++;
                    if (request.abilitatoVerificaPerc)
                        alertAttivi++;
                    if (request.abilitatoVerificaDoc)
                        alertAttivi++;
                    if (request.abilitatoDownload)
                        alertAttivi++;
                    if (request.abilitatoSfoglia)
                        alertAttivi++;

                    string chaVerificaAnt = request.abilitatoVerificaAnt ? "1" : "0";
                    string chaVerificaPerc = request.abilitatoVerificaPerc ? "1" : "0";
                    string chaVerificaDoc = request.abilitatoVerificaDoc ? "1" : "0";
                    string chaDownload = request.abilitatoDownload ? "1" : "0";
                    string chaSfoglia = request.abilitatoSfoglia ? "1" : "0";

                    string useSSL = request.useSSL ? "1" : "0";

                    DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("I_CONS_ALERT_SAVE_CONF");

                    if (dbProvider.DBType.ToUpper().Equals("SQL"))
                    {
                        // sql
                    }
                    else
                    {
                        // oracle
                        query.setParam("sysID", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CONFIG_ALERT_CONS"));
                    }
                    query.setParam("idAmm", request.idAmm);
                    query.setParam("serverSMTP", request.serverSMTP);
                    query.setParam("portaSMTP", request.portaSMTP);
                    query.setParam("chaSSL", useSSL);
                    query.setParam("userid", request.userSMTP);
                    query.setParam("pass", DocsPaUtils.Security.Crypter.Encode(request.pwdSMTP, request.userSMTP));
                    query.setParam("mailFrom", request.mailFrom);
                    query.setParam("mailTo", request.mailTo);
                    query.setParam("chaScadenza", chaVerificaAnt);
                    query.setParam("chaMaxDoc", chaVerificaPerc);
                    query.setParam("chaSingleDoc", chaVerificaDoc);
                    query.setParam("chaDownload", chaDownload);
                    query.setParam("chaSfoglia", chaSfoglia);

                    #region parametri alert

                    //per ogni alert attivo imposto i parametri corrispondenti
                    string param1 = string.Empty;
                    string param2 = string.Empty;

                    if (alertAttivi > 0)
                    {
                        param1 = param1 + ", ";
                        param2 = param2 + ", ";

                    }

                    if (chaVerificaAnt == "1")
                    {
                        alertAttivi = alertAttivi - 1;
                        param1 = param1 + " num_legg_scadenza_termine, num_legg_scadenza_tolleranza ";
                        param2 = param2 + string.Format(" {0}, {1} ",
                            DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(request.idAmm, "BE_CONS_VER_LEG_INTERVALLO"),
                            DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(request.idAmm, "BE_CONS_VER_LEG_GG_NOTIFICHE")
                            );
                        if (alertAttivi > 0)
                        {
                            param1 = param1 + ", ";
                            param2 = param2 + ", ";
                        }
                    }

                    if (chaVerificaPerc == "1")
                    {
                        alertAttivi = alertAttivi - 1;
                        param1 = param1 + " num_leggibilita_max_doc_perc ";
                        param2 = param2 + string.Format(" {0} ", request.maxVerificaPerc);
                        if (alertAttivi > 0)
                        {
                            param1 = param1 + ", ";
                            param2 = param2 + ", ";
                        }
                    }

                    if (chaVerificaDoc == "1")
                    {
                        alertAttivi = alertAttivi - 1;
                        param1 = param1 + " num_legg_sing_max_oper, num_legg_sing_periodo_mon ";
                        param2 = param2 + string.Format(" {0}, {1} ", request.maxOperVerificaDoc, request.periodoMonVerificaDoc);
                        if (alertAttivi > 0)
                        {
                            param1 = param1 + ", ";
                            param2 = param2 + ", ";
                        }
                    }

                    if (chaDownload == "1")
                    {
                        alertAttivi = alertAttivi - 1;
                        param1 = param1 + " num_download_max_oper, num_download_periodo_mon ";
                        param2 = param2 + string.Format(" {0}, {1} ", request.maxOperDownload, request.periodoMonDownload);
                        if (alertAttivi > 0)
                        {
                            param1 = param1 + ", ";
                            param2 = param2 + ", ";
                        }

                    }

                    if (chaSfoglia == "1")
                    {
                        param1 = param1 + " num_sfoglia_max_oper, num_sfoglia_periodo_mon ";
                        param2 = param2 + string.Format(" {0}, {1} ", request.maxOperSfoglia, request.periodoMonSfoglia);
                    }

                    #endregion

                    query.setParam("param1", param1);
                    query.setParam("param2", param2);

                    string commandText = query.getSQL();

                    if (!dbProvider.ExecuteNonQuery(commandText))
                    {
                        dbProvider.RollbackTransaction();
                        throw new Exception("Errore nell'inserimento della nuova configurazione per l'amministrazione - idAmm: " + request.idAmm); 
                    }
                    else
                    {
                        string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_CONFIG_ALERT_CONS");
                        dbProvider.ExecuteScalar(out system_id, sql);

                        dbProvider.CommitTransaction();
                        response.Success = true;
                        response.Exception = null;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public UpdateGestioneAlertResponse UpdateGestioneAlert(UpdateGestioneAlertRequest request)
        {
            UpdateGestioneAlertResponse response = new UpdateGestioneAlertResponse();

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    dbProvider.BeginTransaction();

                    int alertAttivi = 0;

                    // conto il numero di alert attivi
                    if (request.abilitatoVerificaAnt)
                        alertAttivi++;
                    if (request.abilitatoVerificaPerc)
                        alertAttivi++;
                    if (request.abilitatoVerificaDoc)
                        alertAttivi++;
                    if (request.abilitatoDownload)
                        alertAttivi++;
                    if (request.abilitatoSfoglia)
                        alertAttivi++;

                    string chaVerificaAnt = request.abilitatoVerificaAnt ? "1" : "0";
                    string chaVerificaPerc = request.abilitatoVerificaPerc ? "1" : "0";
                    string chaVerificaDoc = request.abilitatoVerificaDoc ? "1" : "0";
                    string chaDownload = request.abilitatoDownload ? "1" : "0";
                    string chaSfoglia = request.abilitatoSfoglia ? "1" : "0";

                    string useSSL = request.useSSL ? "1" : "0";

                    DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("U_CONS_ALERT_AGGIORNA_CONF");
                    query.setParam("idAmm", request.idAmm);
                    query.setParam("serverSMTP", request.serverSMTP);
                    query.setParam("portaSMTP", request.portaSMTP);
                    query.setParam("chaSSL", useSSL);
                    query.setParam("userID", request.userSMTP);
                    query.setParam("pass", DocsPaUtils.Security.Crypter.Encode(request.pwdSMTP, request.userSMTP));
                    query.setParam("mailFrom", request.mailFrom);
                    query.setParam("mailTo", request.mailTo);
                    query.setParam("chaScadenza", chaVerificaAnt);
                    query.setParam("chaMaxDoc", chaVerificaPerc);
                    query.setParam("chaSingleDoc", chaVerificaDoc);
                    query.setParam("chaDownload", chaDownload);
                    query.setParam("chaSfoglia", chaSfoglia);

                    string param = string.Empty;

                    if (alertAttivi > 0)
                        param = param + ", ";

                    if (request.abilitatoVerificaAnt)
                    {
                        alertAttivi = alertAttivi - 1;
                        string p1 = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(request.idAmm, "BE_CONS_VER_LEG_INTERVALLO");
                        string p2 = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(request.idAmm, "BE_CONS_VER_LEG_GG_NOTIFICHE");

                        param = param + string.Format(" num_legg_scadenza_termine={0}, num_legg_scadenza_tolleranza={1} ", p1, p2);
                        if (alertAttivi > 0)
                            param = param + ", ";
                    
                    }
                    if (request.abilitatoVerificaPerc)
                    {
                        alertAttivi = alertAttivi - 1;
                        param = param + string.Format(" num_leggibilita_max_doc_perc={0} ", request.maxVerificaPerc);
                        if (alertAttivi > 0)
                            param = param + ", ";
                    }
                    if (request.abilitatoVerificaDoc)
                    {
                        alertAttivi = alertAttivi - 1;
                        param = param + string.Format(" num_legg_sing_max_oper={0}, num_legg_sing_periodo_mon={1} ", request.maxOperVerificaDoc, request.periodoMonVerificaDoc);
                        if (alertAttivi > 0)
                            param = param + ", ";
                    }
                    if (request.abilitatoDownload)
                    {
                        alertAttivi = alertAttivi - 1;
                        param = param + string.Format(" num_download_max_oper={0}, num_download_periodo_mon={1} ", request.maxOperDownload, request.periodoMonDownload);
                        if (alertAttivi > 0)
                            param = param + ", ";
                    }
                    if (request.abilitatoSfoglia)
                    {
                        alertAttivi = alertAttivi - 1;
                        param = param + string.Format(" num_sfoglia_max_oper={0}, num_sfoglia_periodo_mon={1} ", request.maxOperSfoglia, request.periodoMonSfoglia);
                    }

                    query.setParam("alert", param);
                    string commandText = query.getSQL();
                    if (!dbProvider.ExecuteNonQuery(commandText))
                    {
                        dbProvider.RollbackTransaction();
                        throw new Exception("Errore nell'aggiornamento dei dati");
                    }
                    else
                    {
                        dbProvider.CommitTransaction();
                        response.Success = true;
                        response.Exception = null;
                    }

                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Exception = ex.Message;
            }


            return response;
        }

        #endregion
    }
}