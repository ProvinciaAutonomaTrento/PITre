using System;
using System.Collections;
using System.Data;
using DocsPaDocumentale;
using System.IO;
using System.Data.OleDb;
using log4net;

namespace BusinessLogic.Oggettario
{
	/// <summary>
	/// </summary>
	public class OggettarioManager 
	{
        private static ILog logger = LogManager.GetLogger(typeof(OggettarioManager));

        public static DocsPaVO.amministrazione.EsitoOperazione AmmInsNuovoOggetto(DocsPaVO.amministrazione.Oggettario oggetto, bool update)
        {
            DocsPaVO.amministrazione.EsitoOperazione result = new DocsPaVO.amministrazione.EsitoOperazione();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            // Creazione del contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                //DocsPaDocumentale.Documentale.OrganigrammaManager organigrammaManager = new DocsPaDocumentale.Documentale.OrganigrammaManager(infoUtente);
                //result = organigrammaManager.InserisciUtente(utente);
                if (!oggetto.Codice.Equals(""))
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAOggettario");
                    q.setParam("param1", "SYSTEM_ID");
                    string where = " UPPER(VAR_COD_OGGETTO) = UPPER('" + oggetto.Codice + "')";
                    q.setParam("param2", where);
                    string commandText = q.getSQL();
                    string numOggetti;
                    logger.Debug(commandText);
                    dbProvider.ExecuteScalar(out numOggetti, commandText);
                    if (numOggetti != null && !numOggetti.Equals("0"))
                    {
                        if (update)
                        {
                            result.Codice = 1;
                            return result;
                        }
                        else
                        {
                            result.Codice = 3;
                            logger.Debug("Tentativo di inserimento di un codice oggetto già presente nella tabella dpa_oggettario");
                        }
                    }
                }

                if (result.Codice != 3)
                {
                    DocsPaUtils.Query q1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAOggettario");
                    q1.setParam("param1", "SYSTEM_ID");
                    string where1 = " ID_AMM = " + oggetto.IDAmministrazione + " AND UPPER(VAR_DESC_OGGETTO) = UPPER('" + oggetto.Oggetto + "')";
                    if (!oggetto.Codice.Equals(""))
                        where1 += " AND UPPER(VAR_COD_OGGETTO) = UPPER('" + oggetto.Codice + "')";
                    q1.setParam("param2", where1);
                    string commandText1 = q1.getSQL();
                    string numOggetti1;
                    logger.Debug(commandText1);
                    dbProvider.ExecuteScalar(out numOggetti1, commandText1);

                    if (numOggetti1 != null && !numOggetti1.Equals("0"))
                    {
                        result.Codice = 1;
                        logger.Debug("Tentativo di inserimento di un oggetto presente nella tabella dpa_oggettario");
                    }
                    else
                    {
                        try
                        {
                            q1 = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPAOggettario");
                            q1.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName() + " ID_REGISTRO, ID_AMM, VAR_DESC_OGGETTO, CHA_OCCASIONALE, VAR_COD_OGGETTO");
                            if (!oggetto.Codice.Equals(""))
                            {
                                q1.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_OGGETTARIO") + "'" +
                                    oggetto.IDRegistro + "', " + oggetto.IDAmministrazione + ", '" + oggetto.Oggetto.Replace("'", "''") + "', '0','" + oggetto.Codice + "'");
                            }
                            else
                            {
                                q1.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_OGGETTARIO") + "'" +
                                    oggetto.IDRegistro + "', " + oggetto.IDAmministrazione + ", '" + oggetto.Oggetto.Replace("'", "''") + "', '0',null");
                            }
                            string queryString = q1.getSQL();
                            logger.Debug(queryString);
                            string res;
                            dbProvider.InsertLocked(out res, queryString, "DPA_OGGETTARIO");
                            oggetto.systemId = res;

                            logger.Debug("Inserito nuovo oggetto. SYSTEM_ID = " + res);
                            result.Codice = 0;
                            //	this.CloseConnection();
                        }
                        catch (Exception e)
                        {
                            logger.Debug(e.Message);
                            //	this.CloseConnection();
                            logger.Debug("Errore nell'inserimento oggetto in tabella oggettario", e);
                            result.Codice = 2;
                        }
                    }
                }
                if (result.Codice == 0)
                {
                    // Se l'inserimento è andato a buon fine, viene completata la transazione
                    transactionContext.Complete();
                }
            }

            return result;
        }


        #region ImportaOggettario
        public static bool importaOggettario(byte[] dati, string nomeFile, string serverPath, string codiceAmm, bool update, ref int oggInseriti, ref int oggAggiornati, ref int oggErrati)
        {
            bool result = true;

            OleDbConnection xlsConn = new OleDbConnection();
            OleDbDataReader xlsReader = null;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //Controllo se esiste la Directory "Import" nel path dove vengono salvati i modelli per la profilazione dinamica.
                //Se esiste copio il file excel li' dentro, altrimenti la creo e ci copio il file.
                //In ogni caso poichè il nome del file è fisso, anche se quest'ultimo esiste viene sovrascritto.
                logger.Debug("Metodo \"importaOggettario\" classe \"OggettarioManager\" : inizio scrittura file \"importOggettario.xls\""); 
                if (Directory.Exists(serverPath + "\\Modelli\\Import\\"))
                {
                    FileStream fs1 = new FileStream(serverPath + "\\Modelli\\Import\\" + nomeFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs1.Write(dati, 0, dati.Length);
                    fs1.Close();
                }
                else
                {
                    Directory.CreateDirectory(serverPath + "\\Modelli\\Import\\");
                    FileStream fs1 = new FileStream(serverPath + "\\Modelli\\Import\\" + nomeFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs1.Write(dati, 0, dati.Length);
                    fs1.Close();
                }
                logger.Debug("Metodo \"importaOggettario\" classe \"OggettarioManager\" : fine scrittura file \"importOggettario.xls\"");

                logger.Debug("Metodo \"importaOggettario\" classe \"OggettarioManager\" : inizio lettura file \"importOggettario.xls\""); 
                //Comincio la lettura del file appena scritto
                DocsPaDB.Utils.SimpleLog sl = new DocsPaDB.Utils.SimpleLog(serverPath + "\\Modelli\\Import\\logImportOggettario");
                sl.Log("Inizio importazione Oggettario - " + System.DateTime.Now.ToString());

                xlsConn.ConnectionString = "Provider=" + System.Configuration.ConfigurationManager.AppSettings["DS_PROVIDER"] + "Data Source=" + serverPath + "\\Modelli\\Import\\" + nomeFile + ";Extended Properties='" + System.Configuration.ConfigurationManager.AppSettings["DS_EXTENDED_PROPERTIES"] + "IMEX=1'";
                xlsConn.Open();
                OleDbCommand xlsCmd = new OleDbCommand("select * from [Oggettario$]", xlsConn);
                xlsReader = xlsCmd.ExecuteReader();
                string idAmministrazione = string.Empty;
                int contatore = 0;
                
                while (xlsReader.Read())
                {
                    contatore++;

                    //Controllo se siamo arrivati all'ultima riga
                    if (get_string(xlsReader, 0) == "/")
                        break;

                    //Controllo che il codice dell'amministrazione è uguale a quello della amministrazione
                    //dalla quali si sta effettuando l'importazione Oggettario
                    if (codiceAmm.ToUpper() == get_string(xlsReader, 0).ToUpper())
                    {
                        //Verifico che i campi obbligatori "CODICE AMMINISTRAZIONE - AOO/RF - oggetto - codice" siano presenti
                        //nel foglio excel, altrimenti ignoro l'inserimento
                        if (get_string(xlsReader, 0) != "" && get_string(xlsReader, 2) != "")
                        {
                            //Per quanto riguarda il controllo dell'esistenza di un utente con questi dati,
                            //demando il tutto al metodo "AmmInsNuovoUtente", che trovando eventuali ripetizioni non procede all'inserimento
                            DocsPaVO.amministrazione.Oggettario oggetto = new DocsPaVO.amministrazione.Oggettario();
                            // recupero ID Amministrazione dal codice inserito nel foglio excel
                            oggetto.IDAmministrazione = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(get_string(xlsReader, 0).ToUpper());
                            oggetto.IDRegistro = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDReg(get_string(xlsReader, 1).ToUpper());
                            oggetto.Oggetto = get_string(xlsReader, 2);
                            oggetto.Codice = get_string(xlsReader, 3).ToUpper();

                            if (oggetto.IDRegistro != null)
                            {
                                DocsPaVO.amministrazione.EsitoOperazione esito = AmmInsNuovoOggetto(oggetto, update);

                                switch (esito.Codice)
                                {
                                    case 0:
                                        oggInseriti++;
                                        sl.Log("");
                                        sl.Log("Oggetto Inserito - SystemId: " + oggetto.systemId + " - Oggetto: " + oggetto.Oggetto + " - Codice: " + oggetto.Codice);
                                        break;

                                    case 1:
                                        if (update)
                                        {
                                            //logger.Debug("Metodo \"importaUtenti\" classe \"UserManager\" : modifica utente : " + utente.UserId.ToString());
                                            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPAOggettario");
                                            q.setParam("param1", oggetto.Oggetto.Replace("'", "''"));
                                            q.setParam("param2", oggetto.IDAmministrazione);
                                            q.setParam("idRegistro", oggetto.IDRegistro);
                                            q.setParam("varCodOggetto", oggetto.Codice.ToUpper());

                                            string queryString = q.getSQL();
                                            logger.Debug(queryString);
                                            int rows;
                                            dbProvider.ExecuteNonQuery(queryString, out rows);

                                            if (rows > 0)
                                            {
                                                oggAggiornati++;
                                                sl.Log("");
                                                sl.Log("Oggetto Aggiornato - Oggetto: " + oggetto.Oggetto + " - Codice: " + oggetto.Codice);
                                            }
                                        }
                                        else
                                        {
                                            oggErrati++;
                                            sl.Log("");
                                            sl.Log("Oggetto NON Aggiornato - Riga: " + contatore + " - Non si è richiesto l'obbligo di aggiornamento della tabella attraverso il check box");
                                        }
                                        break;

                                    case 2:
                                        oggErrati++;
                                        sl.Log("");
                                        sl.Log("Oggetto NON Inserito - Riga: " + contatore + " - Errore nell'inserimento in tabella");
                                        break;

                                    case 3:
                                        oggErrati++;
                                        sl.Log("");
                                        sl.Log("Oggetto NON Inserito - Riga: " + contatore + " - Codice Oggetto già presente in tabella");
                                        break;

                                }
                            }
                            else
                            {
                                oggErrati++;
                                sl.Log("");
                                sl.Log("Oggetto NON Inserito - Riga: " + contatore + " per errato Codice Registro");
                            }
                        }
                        else
                        {
                            oggErrati++;
                            sl.Log("");
                            sl.Log("Oggetto NON Inserito - Riga: " + contatore + " per mancanza di Codice Amministrazione o Oggetto");
                        }
                    }
                    else
                    {
                        oggErrati++;
                        sl.Log("");
                        sl.Log("Oggetto NON Inserito - Riga: " + contatore + " Codice Amministrazione errato");
                    }
                }
                sl.Log("");
                sl.Log("Fine importazione Oggettario - " + System.DateTime.Now.ToString());
                sl.Log("Oggetti Inseriti : " + oggInseriti + " - Oggetti Aggiornati : " + oggAggiornati + " - Oggetti Errati : " + oggErrati);
                logger.Debug("Metodo \"importaOggettario\" classe \"OggettarioManager\" : fine lettura file \"importOggettario.xls\""); 
            }
            catch( Exception ex ) 
			{
                logger.Debug("Metodo \"importaOggettario\" classe \"OggettarioManager\" ERRORE : "+ex.Message); 
                result = false;
                return result;
			} 
			finally 
			{
                xlsReader.Close();
				xlsConn.Close();				
            }

            return result;
        }

        private static string get_string(OleDbDataReader dr, int field)
        {
            if (dr[field] == null || dr[field] == System.DBNull.Value)
                return "";
            else
                return dr[field].ToString();
        }

        public static ArrayList getLogImportOggettario(string serverPath)
        {
            ArrayList fileLog = new ArrayList();
            string sLine = string.Empty;

            try
            {
                StreamReader objReader = new StreamReader(serverPath + "\\Modelli\\Import\\logImportOggettario.log");
                while (sLine != null)
                {
                    sLine = objReader.ReadLine();
                    if (sLine != null)
                        fileLog.Add(sLine);
                }
                objReader.Close();

                return fileLog;
            }
            catch (Exception e)
            {
                logger.Debug("Metodo \"getLogImportOggettario\" classe \"OggettarioManager\" ERRORE : " + e.Message);
                return fileLog;
            }
        }


		#endregion        

	}
}
