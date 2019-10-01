using System;
using System.IO;
using System.Linq;
using System.Data.OleDb;
using System.Collections.Generic;
using log4net;
using System.Collections;
using DocsPaVO.utente;
using DocsPaVO.Import.Pregressi;
using BusinessLogic.Documenti;
using DocsPaVO.rubrica;
using DocsPaVO.addressbook;
using BusinessLogic.Utenti;
using DocsPaVO.Import;
using DocsPaVO.documento;
using System.Timers;

namespace BusinessLogic.Amministrazione
{
    public class ImportPregressiManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(ImportPregressiManager));
        private static string separatoreMessaggi = "|";
        private static Dictionary<string, List<string>> listaCodiciUtenteVerificati = new Dictionary<string, List<string>>();
        private static bool abilitazioneRubricaComune;
        private static bool boolImporta;

        public static EsitoImportPregressi CheckFileData(byte[] dati, string nomeFile, string serverPath, DocsPaVO.utente.InfoUtente infoUtente, bool isAdministration)
        {
            bool warning = false;

            OleDbConnection xlsConn = new OleDbConnection();
            OleDbCommand xlsCmd = null;
            OleDbDataReader xlsReader = null;

            EsitoImportPregressi esitoControllo = new EsitoImportPregressi();
            esitoControllo.esito = true;



            if (!Directory.Exists(serverPath + "\\Modelli\\Import\\"))
                Directory.CreateDirectory(serverPath + "\\Modelli\\Import\\");

            DocsPaDB.Utils.SimpleLog sl = new DocsPaDB.Utils.SimpleLog(serverPath + "\\Modelli\\Import\\logImportazionePregressi");
            List<ItemReportPregressi> listaRigheDaValidare = new List<ItemReportPregressi>();

            try
            {
                #region Scrittura del file
                FileStream fs1 = new FileStream(serverPath + "\\Modelli\\Import\\" + nomeFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fs1.Write(dati, 0, dati.Length);
                fs1.Close();
                #endregion Scrittura del file

                #region Connessione al file excel
                xlsConn.ConnectionString = "Provider=" + System.Configuration.ConfigurationManager.AppSettings["DS_PROVIDER"] + "Data Source=" + serverPath + "\\Modelli\\Import\\" + nomeFile + ";Extended Properties='" + System.Configuration.ConfigurationManager.AppSettings["DS_EXTENDED_PROPERTIES"] + "IMEX=1';";
                xlsConn.Open();
                sl.Log("");
                sl.Log("Connessione al file : " + nomeFile + " aperta");
                #endregion Connessione al file excel

                sl.Log("");
                sl.Log("**** Inizio importazione UO - " + System.DateTime.Now.ToString());

                xlsCmd = new OleDbCommand("select * from [Protocolli Pregressi$]", xlsConn);
                xlsReader = xlsCmd.ExecuteReader();

                List<string> colum_mapper = new List<string>();
                List<string> controlloOrdinali = new List<string>();
                bool doppioOrdinale = false;

                for (int i = 0; i <= (xlsReader.FieldCount - 1); i++)
                {
                    colum_mapper.Add(xlsReader.GetName(i).ToUpper());
                }

                bool valido = true;

                if (!fileConforme(colum_mapper))
                    return null;

                while (xlsReader.Read())
                {
                    //Controllo se siamo arrivati all'ultima riga
                    if (get_string(xlsReader, 0) == "/")
                        break;

                    //CAMPI OBBLIGATORI
                    string ordinle = get_string(xlsReader, colum_mapper.IndexOf("ORDINALE"));
                    string dataproto = get_string(xlsReader, colum_mapper.IndexOf("DATA PROTOCOLLO"));
                    string numproto = get_string(xlsReader, colum_mapper.IndexOf("NUMERO DI PROTOCOLLO"));
                    string tipoproto = get_string(xlsReader, colum_mapper.IndexOf("TIPO PROTOCOLLO"));
                    string codregistro = get_string(xlsReader, colum_mapper.IndexOf("CODICE REGISTRO"));
                    string codutente = get_string(xlsReader, colum_mapper.IndexOf("CODICE UTENTE CREATORE"));
                    string codruoloute = get_string(xlsReader, colum_mapper.IndexOf("CODICE RUOLO CREATORE"));


                    string tipooperazione = ((colum_mapper.IndexOf("TIPO OPERAZIONE") != -1) ? get_string(xlsReader, colum_mapper.IndexOf("TIPO OPERAZIONE")) : null);

                    if (!string.IsNullOrEmpty(tipooperazione) && (tipooperazione.ToUpper().Equals("I") || tipooperazione.ToUpper().Equals("M") || tipooperazione.ToUpper().Equals("C")))
                    {
                        //Tolto isAdministration per RICHIESTA UTENTE.
                        //Controllo campi obbligatori
                        //if (isAdministration)
                        //{
                        if (tipooperazione.ToUpper().Equals("C") || tipooperazione.ToUpper().Equals("M"))
                        {
                            valido = ((string.IsNullOrEmpty(ordinle) || string.IsNullOrEmpty(numproto) || string.IsNullOrEmpty(codregistro)) ? false : true);
                        }
                        else
                        {
                            valido = ((string.IsNullOrEmpty(ordinle) || string.IsNullOrEmpty(dataproto) || string.IsNullOrEmpty(numproto) || string.IsNullOrEmpty(tipoproto) || string.IsNullOrEmpty(codregistro) || string.IsNullOrEmpty(codutente) || string.IsNullOrEmpty(codruoloute)) ? false : true);
                        }
                        //}
                        //else
                        //{
                        //    if (tipooperazione.ToUpper().Equals("C") || tipooperazione.ToUpper().Equals("M"))
                        //    {
                        //        valido = ((string.IsNullOrEmpty(ordinle) || string.IsNullOrEmpty(numproto) || string.IsNullOrEmpty(codregistro)) ? false : true);
                        //    }
                        //    else
                        //    {
                        //        valido = ((string.IsNullOrEmpty(ordinle) || string.IsNullOrEmpty(dataproto) || string.IsNullOrEmpty(numproto) || string.IsNullOrEmpty(tipoproto) || string.IsNullOrEmpty(codregistro)) ? false : true);

                        //        if ((string.IsNullOrEmpty(codutente) && !string.IsNullOrEmpty(codruoloute)) || (!string.IsNullOrEmpty(codutente) && string.IsNullOrEmpty(codruoloute)))
                        //        {
                        //            valido = false;
                        //        }
                        //    }
                        //}
                    }
                    else
                    {
                        valido = false;
                    }

                    if (controlloOrdinali.Contains(ordinle))
                    {
                        valido = false;
                        doppioOrdinale = true;
                    }
                    else
                    {
                        controlloOrdinali.Add(ordinle);
                        doppioOrdinale = false;
                    }

                    if (!valido)
                    {
                        string errormess = string.Empty;

                        ItemReportPregressi reportItem = new ItemReportPregressi();
                        reportItem.idNumProtocolloExcel = numproto;
                        reportItem.codRegistro = codregistro;
                        reportItem.ordinale = ordinle;
                        reportItem.tipoOperazione = tipooperazione;
                        reportItem.tipoProtocollo = tipoproto;
                        reportItem.idUtente = codutente;
                        reportItem.idRuolo = codruoloute;

                        if (string.IsNullOrEmpty(ordinle))
                            reportItem.errore = concatenaMessaggio(reportItem, "Campo ordinale mancante.");
                        if (string.IsNullOrEmpty(numproto))
                            reportItem.errore = concatenaMessaggio(reportItem, "Numero di protocollo mancante.");
                        if (string.IsNullOrEmpty(dataproto))
                            reportItem.errore = concatenaMessaggio(reportItem, "Data del protocollo mancante.");
                        if (string.IsNullOrEmpty(tipoproto))
                            //reportItem.errore = concatenaMessaggio(reportItem, "Mancante la specifica per il tipo protocollo.");
                            reportItem.errore = concatenaMessaggio(reportItem, "Tipo protocollo mancante.");
                        if (string.IsNullOrEmpty(codregistro))
                            reportItem.errore = concatenaMessaggio(reportItem, "Codice registro mancante.");
                        //Aggiunta condizione per Richiesta cliente:
                        // (!tipooperazione.ToUpper().Equals("I") || !tipooperazione.ToUpper().Equals("M") || !tipooperazione.ToUpper().Equals("C"))
                        if (string.IsNullOrEmpty(tipooperazione) || (!tipooperazione.ToUpper().Equals("I") && !tipooperazione.ToUpper().Equals("M") && !tipooperazione.ToUpper().Equals("C")))
                            //reportItem.errore = concatenaMessaggio(reportItem, "Tipo operazione mancante o errato.");
                            reportItem.errore = concatenaMessaggio(reportItem, "Tipo operazione mancante o errata.");

                        //if (isAdministration && string.IsNullOrEmpty(codutente))
                        //    reportItem.errore = concatenaMessaggio(reportItem, "Codice utente non specificato.");
                        //if (isAdministration && string.IsNullOrEmpty(codruoloute))
                        //    reportItem.errore = concatenaMessaggio(reportItem, "Codice ruolo utente non specificato.");
                        //if (!isAdministration && (string.IsNullOrEmpty(codutente) || string.IsNullOrEmpty(codruoloute)))
                        //    reportItem.errore = concatenaMessaggio(reportItem, "Utente proprietario incompleto. Specificare codice utente e codice ruolo.");

                        //Modificato per Richiesta Cliente
                        if (string.IsNullOrEmpty(codutente))
                            //reportItem.errore = concatenaMessaggio(reportItem, "Codice utente non specificato.");
                            reportItem.errore = concatenaMessaggio(reportItem, "Codice utente creatore non specificato.");
                        if (string.IsNullOrEmpty(codruoloute))
                            //reportItem.errore = concatenaMessaggio(reportItem, "Codice ruolo utente non specificato.");
                            reportItem.errore = concatenaMessaggio(reportItem, "Codice ruolo creatore non specificato.");


                        if (doppioOrdinale)
                        {
                            reportItem.errore = concatenaMessaggio(reportItem, "Ordinale " + ordinle + "  ripetuto più volte.");
                        }

                        reportItem.esito = "E";

                        esitoControllo.itemPregressi.Add(reportItem);
                        esitoControllo.esito = false;

                        sl.Log("");
                        sl.Log("ERROR : Campi obbligatori mancanti nel file inviato");

                    }
                    else
                    {
                        ItemReportPregressi reportItem = new ItemReportPregressi();
                        reportItem.idNumProtocolloExcel = numproto.Trim();
                        reportItem.codRegistro = codregistro.Trim();
                        reportItem.ordinale = ordinle.Trim();
                        reportItem.tipoOperazione = tipooperazione.Trim();
                        reportItem.tipoProtocollo = tipoproto.Trim();
                        reportItem.idUtente = codutente.Trim();
                        reportItem.idRuolo = codruoloute.Trim();
                        reportItem.data = getStringDataFormatted(get_string(xlsReader, colum_mapper.IndexOf("DATA PROTOCOLLO")));

                        //Campi non inclusi nel processo di controllo/validazione
                        //Andrea - Commentato cod_rf per prova
                        //reportItem.cod_rf = get_string(xlsReader, colum_mapper.IndexOf("CODICE RF"));
                        reportItem.cod_oggetto = get_string(xlsReader, colum_mapper.IndexOf("CODICE OGGETTO"));
                        reportItem.oggetto = get_string(xlsReader, colum_mapper.IndexOf("OGGETTO"));
                        reportItem.cod_corrispondenti = get_string(xlsReader, colum_mapper.IndexOf("CODICI CORRISPONDENTI"));
                        reportItem.corrispondenti = get_string(xlsReader, colum_mapper.IndexOf("CORRISPONDENTI"));
                        reportItem.pathname = get_string(xlsReader, colum_mapper.IndexOf("PATHNAME"));
                        reportItem.adl = get_string(xlsReader, colum_mapper.IndexOf("ADL"));
                        reportItem.note = get_string(xlsReader, colum_mapper.IndexOf("NOTE"));
                        reportItem.cod_modello_trasm = get_string(xlsReader, colum_mapper.IndexOf("CODICE MODELLO TRASMISSIONE"));
                        reportItem.tipo_documento = get_string(xlsReader, colum_mapper.IndexOf("TIPOLOGIA DOCUMENTO"));

                        //PROVA ANDREA
                        //Campi Relativi alla Fascicolazione

                        //reportItem.Titolario = string.Empty

                        string codiciFascicolo = get_string(xlsReader, colum_mapper.IndexOf("CODICE FASCICOLO"));
                        char[] delimiter = { ';' };
                        if (!string.IsNullOrEmpty(codiciFascicolo))
                        {
                            reportItem.ProjectCodes = codiciFascicolo.Split(delimiter);
                        }
                        reportItem.ProjectDescription = get_string(xlsReader, colum_mapper.IndexOf("DESCRIZIONE FASCICOLO"));

                        //Non più Utilizzati
                        //reportItem.FolderDescrition = get_string(xlsReader, colum_mapper.IndexOf("DESCRIZIONE SOTTOFASCICOLO"));
                        //reportItem.Titolario = get_string(xlsReader, colum_mapper.IndexOf("TITOLARIO"));
                        //reportItem.NodeCode = get_string(xlsReader, colum_mapper.IndexOf("CODICE NODO"));
                        reportItem.FolderDescrition = string.Empty;
                        reportItem.NodeCode = string.Empty;

                        reportItem.codiceRegistroFascicolo = get_string(xlsReader, colum_mapper.IndexOf("CODICE REGISTRO FASCICOLO"));

                        //
                        //Colonna Titolario può essere presente o meno:
                        //Se presente prendo il valore, altrimenti prendo implicitamente il titolario attivo.
                        bool ColonnaTitolarioPresente = false;
                        ColonnaTitolarioPresente = (colum_mapper.IndexOf("TITOLARIO") != -1 ? true : false);
                        if (ColonnaTitolarioPresente)
                        {
                            string tipoTitolario = get_string(xlsReader, colum_mapper.IndexOf("TITOLARIO"));
                            //Se il valore della riga è A o null prendo il titolario attivo
                            if (tipoTitolario.ToUpper().Equals("A") || string.IsNullOrEmpty(tipoTitolario))
                            {
                                reportItem.Titolario = string.Empty;
                            }

                            if (tipoTitolario.ToUpper().Equals("S"))
                            {
                                reportItem.Titolario = "S";
                            }
                        }
                        else
                        {
                            //Manca la colonna Titolario, quindi prendo in automatico il titolario attivo
                            reportItem.Titolario = string.Empty;
                        }
                        //
                        //End

                        //END PROVA ANDREA

                        if (!string.IsNullOrEmpty(reportItem.tipo_documento))
                            reportItem.valoriProfilati = getValoriProfilati(xlsReader, colum_mapper);

                        listaRigheDaValidare.Add(reportItem);
                    }
                }

                //Lettura del foglio Documenti Non Protocollati
                xlsCmd = new OleDbCommand("select * from [Documenti Non Protocollati$]", xlsConn);
                xlsReader = xlsCmd.ExecuteReader();

                //Lettura colonne dello sheet Documenti Non Protocollati
                List<string> colum_mapper_documenti_non_protocollati = new List<string>();
                for (int i = 0; i <= (xlsReader.FieldCount - 1); i++)
                {
                    colum_mapper_documenti_non_protocollati.Add(xlsReader.GetName(i).ToUpper());
                }

                while (xlsReader.Read())
                {
                    //controllo ultima riga
                    if (get_string(xlsReader, 0) == "/")
                        break;

                    //CAMPI OBBLIGATORI
                    string ordinale = get_string(xlsReader, colum_mapper_documenti_non_protocollati.IndexOf("ORDINALE"));
                    string id_vecchio_documento = get_string(xlsReader, colum_mapper_documenti_non_protocollati.IndexOf("ID VECCHIO DOCUMENTO"));
                    string data_creazione = get_string(xlsReader, colum_mapper_documenti_non_protocollati.IndexOf("DATA CREAZIONE"));
                    string codice_utente_creatore = get_string(xlsReader, colum_mapper_documenti_non_protocollati.IndexOf("CODICE UTENTE CREATORE"));
                    string codice_ruolo_creatore = get_string(xlsReader, colum_mapper_documenti_non_protocollati.IndexOf("CODICE RUOLO CREATORE"));

                    string tipo_operazione = ((colum_mapper_documenti_non_protocollati.IndexOf("TIPO OPERAZIONE") != -1) ? get_string(xlsReader, colum_mapper_documenti_non_protocollati.IndexOf("TIPO OPERAZIONE")) : null);

                    if (!string.IsNullOrEmpty(tipo_operazione) && (tipo_operazione.ToUpper().Equals("I") || tipo_operazione.ToUpper().Equals("M") || tipo_operazione.ToUpper().Equals("C")))
                    {
                        if (tipo_operazione.Equals("M") || tipo_operazione.Equals("C"))
                        {
                            valido = ((string.IsNullOrEmpty(ordinale) || string.IsNullOrEmpty(id_vecchio_documento)) ? false : true);
                        }
                        else
                        {
                            valido = ((string.IsNullOrEmpty(ordinale) || string.IsNullOrEmpty(data_creazione) || string.IsNullOrEmpty(id_vecchio_documento) || string.IsNullOrEmpty(codice_utente_creatore) || string.IsNullOrEmpty(codice_ruolo_creatore)) ? false : true);
                        }
                    }
                    else
                    {
                        valido = false;
                    }

                    if (controlloOrdinali.Contains(ordinale))
                    {
                        valido = false;
                        doppioOrdinale = true;
                    }
                    else
                    {
                        controlloOrdinali.Add(ordinale);
                        doppioOrdinale = false;
                    }

                    if (!valido)
                    {
                        string errormess = string.Empty;

                        ItemReportPregressi reportItem = new ItemReportPregressi();

                        reportItem.idNumProtocolloExcel = id_vecchio_documento;

                        reportItem.ordinale = ordinale;
                        reportItem.tipoOperazione = tipo_operazione;
                        reportItem.tipoProtocollo = "NP";
                        reportItem.idUtente = codice_utente_creatore;
                        reportItem.idRuolo = codice_ruolo_creatore;


                        if (string.IsNullOrEmpty(ordinale))
                            reportItem.errore = concatenaMessaggio(reportItem, "Campo ordinale del documento non protocollato mancante.");
                        if (string.IsNullOrEmpty(id_vecchio_documento))
                            reportItem.errore = concatenaMessaggio(reportItem, "Id del vecchio documento mancante.");
                        if (string.IsNullOrEmpty(data_creazione))
                            reportItem.errore = concatenaMessaggio(reportItem, "Data di creazione del documento mancante.");
                        if (string.IsNullOrEmpty(tipo_operazione) || (!tipo_operazione.ToUpper().Equals("I") && !tipo_operazione.ToUpper().Equals("M") && !tipo_operazione.ToUpper().Equals("C")))
                            //reportItem.errore = concatenaMessaggio(reportItem, "Tipo operazione mancante o errato.");
                            reportItem.errore = concatenaMessaggio(reportItem, "Tipo operazione mancante o errata.");
                        //Modificato per Richiesta Cliente
                        if (string.IsNullOrEmpty(codice_utente_creatore))
                            //reportItem.errore = concatenaMessaggio(reportItem, "Codice utente non specificato.");
                            reportItem.errore = concatenaMessaggio(reportItem, "Codice utente creatore non specificato.");
                        if (string.IsNullOrEmpty(codice_ruolo_creatore))
                            //reportItem.errore = concatenaMessaggio(reportItem, "Codice ruolo utente non specificato.");
                            reportItem.errore = concatenaMessaggio(reportItem, "Codice ruolo creatore non specificato.");

                        if (doppioOrdinale)
                        {
                            reportItem.errore = concatenaMessaggio(reportItem, "Ordinale " + ordinale + "  ripetuto più volte.");
                        }

                        reportItem.esito = "E";

                        esitoControllo.itemPregressi.Add(reportItem);
                        esitoControllo.esito = false;

                        sl.Log("");
                        sl.Log("ERROR : Campi obbligatori mancanti nel file inviato");
                    }
                    else
                    {
                        ItemReportPregressi reportItem = new ItemReportPregressi();
                        reportItem.idNumProtocolloExcel = id_vecchio_documento.Trim();
                        reportItem.ordinale = ordinale.Trim();
                        reportItem.tipoOperazione = tipo_operazione.Trim();
                        reportItem.tipoProtocollo = "NP";
                        reportItem.idUtente = codice_utente_creatore.Trim();
                        reportItem.idRuolo = codice_ruolo_creatore.Trim();
                        reportItem.data = getStringDataFormatted(get_string(xlsReader, colum_mapper_documenti_non_protocollati.IndexOf("DATA CREAZIONE")));

                        //Campi non inclusi nel processo di controllo/validazione
                        reportItem.cod_oggetto = get_string(xlsReader, colum_mapper_documenti_non_protocollati.IndexOf("CODICE OGGETTO"));
                        reportItem.oggetto = get_string(xlsReader, colum_mapper_documenti_non_protocollati.IndexOf("OGGETTO"));
                        reportItem.pathname = get_string(xlsReader, colum_mapper_documenti_non_protocollati.IndexOf("PATHNAME"));
                        reportItem.adl = get_string(xlsReader, colum_mapper_documenti_non_protocollati.IndexOf("ADL"));
                        reportItem.note = get_string(xlsReader, colum_mapper_documenti_non_protocollati.IndexOf("NOTE"));
                        reportItem.cod_modello_trasm = get_string(xlsReader, colum_mapper_documenti_non_protocollati.IndexOf("CODICE MODELLO TRASMISSIONE"));
                        reportItem.tipo_documento = get_string(xlsReader, colum_mapper_documenti_non_protocollati.IndexOf("TIPOLOGIA DOCUMENTO"));

                        //PROVA ANDREA
                        //Campi Relativi alla Fascicolazione

                        //reportItem.Titolario = string.Empty

                        string codiciFascicolo = get_string(xlsReader, colum_mapper_documenti_non_protocollati.IndexOf("CODICE FASCICOLO"));
                        char[] delimiter2 = { ';' };
                        if (!string.IsNullOrEmpty(codiciFascicolo))
                        {
                            reportItem.ProjectCodes = codiciFascicolo.Split(delimiter2);
                        }
                        reportItem.ProjectDescription = get_string(xlsReader, colum_mapper_documenti_non_protocollati.IndexOf("DESCRIZIONE FASCICOLO"));

                        //Non più utilizzati
                        //reportItem.FolderDescrition = get_string(xlsReader, colum_mapper_documenti_non_protocollati.IndexOf("DESCRIZIONE SOTTOFASCICOLO"));
                        //reportItem.Titolario = get_string(xlsReader, colum_mapper_documenti_non_protocollati.IndexOf("TITOLARIO"));
                        //reportItem.NodeCode = get_string(xlsReader, colum_mapper_documenti_non_protocollati.IndexOf("CODICE NODO"));
                        reportItem.FolderDescrition = string.Empty;
                        //reportItem.Titolario = string.Empty;
                        reportItem.NodeCode = string.Empty;

                        reportItem.codiceRegistroFascicolo = get_string(xlsReader, colum_mapper_documenti_non_protocollati.IndexOf("CODICE REGISTRO FASCICOLO"));

                        //
                        //Colonna Titolario può essere presente o meno:
                        //Se presente prendo il valore, altrimenti prendo implicitamente il titolario attivo.
                        bool ColonnaTitolarioNPPresente = false;
                        ColonnaTitolarioNPPresente = (colum_mapper_documenti_non_protocollati.IndexOf("TITOLARIO") != -1 ? true : false);
                        if (ColonnaTitolarioNPPresente)
                        {
                            string tipoTitolario = get_string(xlsReader, colum_mapper_documenti_non_protocollati.IndexOf("TITOLARIO"));
                            //Se il valore della riga è A o null prendo il titolario attivo
                            if (tipoTitolario.ToUpper().Equals("A") || string.IsNullOrEmpty(tipoTitolario))
                            {
                                reportItem.Titolario = string.Empty;
                            }

                            if (tipoTitolario.ToUpper().Equals("S"))
                            {
                                reportItem.Titolario = "S";
                            }
                        }
                        else
                        {
                            //Manca la colonna Titolario, quindi prendo in automatico il titolario attivo
                            reportItem.Titolario = string.Empty;
                        }
                        //
                        //End

                        //END PROVA ANDREA

                        if (!string.IsNullOrEmpty(reportItem.tipo_documento))
                            reportItem.valoriProfilati = getValoriProfilati(xlsReader, colum_mapper_documenti_non_protocollati);

                        listaRigheDaValidare.Add(reportItem);
                    }
                }
                //End Documenti Non Protocollati

                xlsCmd = new OleDbCommand("select * from [Allegati$]", xlsConn);
                xlsReader = xlsCmd.ExecuteReader();

                List<string> colum_mapper_allegati = new List<string>();
                for (int i = 0; i <= (xlsReader.FieldCount - 1); i++)
                {
                    colum_mapper_allegati.Add(xlsReader.GetName(i).ToUpper());
                }

                while (xlsReader.Read())
                {
                    if (get_string(xlsReader, 0) == "/")
                        break;

                    Allegati allegato = new Allegati();
                    string ordinle = get_string(xlsReader, colum_mapper_allegati.IndexOf("ORDINALE PRINCIPALE"));
                    string descrizione = get_string(xlsReader, colum_mapper_allegati.IndexOf("DESCRIZIONE"));
                    string pathname = get_string(xlsReader, colum_mapper_allegati.IndexOf("PATHNAME"));

                    if (!string.IsNullOrEmpty(ordinle))
                    {
                        allegato.ordinale = ordinle;
                        allegato.descrizione = descrizione;
                        allegato.pathname = pathname;

                        ItemReportPregressi itemDaModificare = listaRigheDaValidare.Where(x => x.ordinale == ordinle).FirstOrDefault();
                        if (itemDaModificare != null)
                        {
                            listaRigheDaValidare.Remove(itemDaModificare);
                            itemDaModificare.Allegati.Add(allegato);
                            listaRigheDaValidare.Add(itemDaModificare);
                        }
                    }
                }


                if (xlsReader != null)
                    xlsReader.Close();
                if (xlsConn != null)
                    xlsConn.Close();

                Dictionary<string, Registro> listaRegistri = new Dictionary<string, Registro>();
                Dictionary<string, List<ItemReportPregressi>> listaProtocolliDaInserire = new Dictionary<string, List<ItemReportPregressi>>();
                List<ItemReportPregressi> listaDocumentiNP = new List<ItemReportPregressi>();

                foreach (ItemReportPregressi protocollo in listaRigheDaValidare)
                {
                    bool inErrorOrWarning = false;

                    //Modifica dovuta alla presenza dei documenti non protocollati

                    if (protocollo.tipoProtocollo.ToUpper().Equals("NP"))
                    {
                        if (!string.IsNullOrEmpty(protocollo.tipoOperazione) && protocollo.tipoOperazione.ToUpper() == "I" && !idVecchioDocumentoUnivocoInFile(protocollo, listaRigheDaValidare))
                        {
                            protocollo.esito = "E";
                            protocollo.errore = concatenaMessaggio(protocollo, "Id del vecchio documento ripetuto nel file.");
                            inErrorOrWarning = true;
                            esitoControllo.esito = false;
                        }

                        if (!codiciUtenteValidi(protocollo, infoUtente.idAmministrazione))
                        {
                            if (!string.IsNullOrEmpty(protocollo.tipoOperazione) && protocollo.tipoOperazione.ToUpper() == "I")
                            {
                                protocollo.esito = "E";
                                //protocollo.errore = concatenaMessaggio(protocollo, "Codice utente e ruolo errati o non congrui.");
                                protocollo.errore = concatenaMessaggio(protocollo, "Codice utente creatore e codice ruolo creatore errati o non congrui.");
                                inErrorOrWarning = true;
                                esitoControllo.esito = false;
                            }
                            else
                            {
                                protocollo.idRuolo = string.Empty;
                                protocollo.idUtente = string.Empty;
                            }
                        }

                        if (!string.IsNullOrEmpty(protocollo.tipoOperazione) && protocollo.tipoOperazione.ToUpper() == "I")
                        {
                            listaDocumentiNP.Add(protocollo);
                        }

                        //Controllo che id_documento sia presente nella profile da fare.

                        //lista popolata per controllo data_creazione - id_vecchio_documento all'interno del file


                    }
                    else
                    {
                        Registro registro = null;
                        if (!listaRegistri.TryGetValue(protocollo.codRegistro, out registro))
                        {
                            registro = BusinessLogic.Utenti.RegistriManager.getRegistroByCodAOO(protocollo.codRegistro, infoUtente.idAmministrazione);
                            if (registro != null)
                            {
                                listaRegistri.Add(protocollo.codRegistro, registro);
                                protocollo.idRegistro = registro.systemId;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(protocollo.tipoOperazione) && protocollo.tipoOperazione.ToUpper() == "I")
                                {
                                    protocollo.esito = "E";
                                    protocollo.errore = concatenaMessaggio(protocollo, "Codice registro errato.");
                                    inErrorOrWarning = true;
                                    esitoControllo.esito = false;
                                }
                            }
                        }
                        else
                        {
                            protocollo.idRegistro = listaRegistri[protocollo.codRegistro].systemId;
                        }

                        if (!string.IsNullOrEmpty(protocollo.tipoOperazione) && protocollo.tipoOperazione.ToUpper() == "I" && !numProtoUnivocoInFile(protocollo, listaRigheDaValidare))
                        {
                            protocollo.esito = "E";
                            protocollo.errore = concatenaMessaggio(protocollo, "Numero di protocollo ripetuto nel file.");
                            inErrorOrWarning = true;
                            esitoControllo.esito = false;
                        }

                        //if (!string.IsNullOrEmpty(protocollo.tipoOperazione) && protocollo.tipoOperazione.ToUpper() == "I" && !tipoProtoCorretto(protocollo))
                        //{
                        //    protocollo.esito = "W";
                        //    protocollo.errore = concatenaMessaggio(protocollo, "Tipo protocollo specificato non riconosciuto. Valori ammessi: A (ingresso), P (uscita).");
                        //    inErrorOrWarning = true;
                        //}

                        //Richiesta Del Cliente
                        if (!string.IsNullOrEmpty(protocollo.tipoOperazione) && protocollo.tipoOperazione.ToUpper() == "I" && !tipoProtoCorretto(protocollo))
                        {
                            protocollo.esito = "E";
                            protocollo.errore = concatenaMessaggio(protocollo, "Tipo protocollo specificato non riconosciuto. Valori ammessi: A (ingresso), P (uscita), I (interno).");
                            inErrorOrWarning = true;
                        }

                        bool regDiPreg = true;
                        if (registroValido(registro))
                        {
                            if (!string.IsNullOrEmpty(protocollo.tipoOperazione) && protocollo.tipoOperazione.ToUpper() == "I" && !annoProtocolloValido(protocollo, registro))
                            {
                                protocollo.esito = "E";
                                protocollo.errore = concatenaMessaggio(protocollo, "L'anno del protocollo è diverso dall'anno del registro.");
                                inErrorOrWarning = true;
                                esitoControllo.esito = false;
                                regDiPreg = false;
                            }
                        }
                        else
                        {
                            protocollo.esito = "E";
                            protocollo.errore = concatenaMessaggio(protocollo, "Il codice di registro specificato non è di tipo pregresso.");
                            inErrorOrWarning = true;
                            esitoControllo.esito = false;
                            regDiPreg = false;
                        }

                        //Decommentato per richiesta del cliente - esito era W - ricommentato per modifica sopra
                        //if (!tipoOperazioneCorretta(protocollo))
                        //{
                        //    protocollo.esito = "E";
                        //    protocollo.errore = concatenaMessaggio(protocollo, "Tipo operazione non valida. Valori ammessi: I (inserimento), M (modifica), C (cancellazione).");
                        //    inErrorOrWarning = true;
                        //}
                        //else
                        //{
                        if (!string.IsNullOrEmpty(protocollo.codRegistro) && !string.IsNullOrEmpty(protocollo.idNumProtocolloExcel) && regDiPreg)
                        {
                            if (listaProtocolliDaInserire.ContainsKey(protocollo.codRegistro))
                            {
                                listaProtocolliDaInserire[protocollo.codRegistro].Add(protocollo);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(protocollo.tipoOperazione) && protocollo.tipoOperazione.ToUpper() == "I")
                                {
                                    List<ItemReportPregressi> nuovoItemProtoPerRegistro = new List<ItemReportPregressi>();
                                    nuovoItemProtoPerRegistro.Add(protocollo);
                                    listaProtocolliDaInserire.Add(protocollo.codRegistro, nuovoItemProtoPerRegistro);
                                }
                            }
                        }
                        //}

                        //if (registroValido(registro))
                        //{
                        //    if (!string.IsNullOrEmpty(protocollo.tipoOperazione) && protocollo.tipoOperazione.ToUpper() == "I" && !annoProtocolloValido(protocollo, registro))
                        //    {
                        //        protocollo.esito = "E";
                        //        protocollo.errore = concatenaMessaggio(protocollo, "L'anno del protocollo è diverso dall'anno del registro.");
                        //        inErrorOrWarning = true;
                        //        esitoControllo.esito = false;
                        //    }
                        //}
                        //else
                        //{
                        //    protocollo.esito = "E";
                        //    protocollo.errore = concatenaMessaggio(protocollo, "Il codice di registro specificato non è di tipo pregresso.");
                        //    inErrorOrWarning = true;
                        //    esitoControllo.esito = false;
                        //}

                        //Tolto per RICHIESTA CLIENTE
                        //if (isAdministration)
                        //{
                        if (!codiciUtenteValidi(protocollo, infoUtente.idAmministrazione))
                        {
                            if (!string.IsNullOrEmpty(protocollo.tipoOperazione) && protocollo.tipoOperazione.ToUpper() == "I")
                            {
                                protocollo.esito = "E";
                                //protocollo.errore = concatenaMessaggio(protocollo, "Codice utente e ruolo errati o non congrui.");
                                protocollo.errore = concatenaMessaggio(protocollo, "Codice utente creatore e codice ruolo creatore errati o non congrui.");
                                inErrorOrWarning = true;
                                esitoControllo.esito = false;
                            }
                            else
                            {
                                protocollo.idRuolo = string.Empty;
                                protocollo.idUtente = string.Empty;
                            }
                        }
                        //}
                        //else
                        //{
                        //    if (!codiciUtenteValidi(protocollo, infoUtente.idAmministrazione))
                        //    {
                        //        if (!string.IsNullOrEmpty(protocollo.idUtente) && !string.IsNullOrEmpty(protocollo.idRuolo) && (!string.IsNullOrEmpty(protocollo.tipoOperazione) && protocollo.tipoOperazione.ToUpper().Equals("I")))
                        //        {
                        //            protocollo.esito = "E";
                        //            protocollo.errore = concatenaMessaggio(protocollo, "Codice utente e ruolo errati o non congrui.");
                        //            inErrorOrWarning = true;
                        //            esitoControllo.esito = false;
                        //        }
                        //        else
                        //        {
                        //            protocollo.idUtente = infoUtente.idPeople;
                        //            protocollo.idRuolo = infoUtente.idGruppo;
                        //        }
                        //    }
                        //}
                    }
                    //End else tipo protocollo !NP

                    if (inErrorOrWarning)
                        esitoControllo.itemPregressi.Add(protocollo);
                    else
                    {
                        protocollo.esito = "S";
                        esitoControllo.itemPregressi.Add(protocollo);
                    }
                }//End foreach


                //Controllo esistenza documenti non protocollati da inserire
                if (listaDocumentiNP.Count > 0)
                {
                    string whaereQueryIn = string.Empty;
                    int k = 0;

                    foreach (ItemReportPregressi prot in listaDocumentiNP)
                    {
                        if (string.IsNullOrEmpty(whaereQueryIn))
                            whaereQueryIn = " IN (";

                        whaereQueryIn += prot.idNumProtocolloExcel;

                        if (k < listaDocumentiNP.Count - 1)
                        {
                            if (k % 998 == 0 && k > 0)
                            {
                                whaereQueryIn += ") OR ID_VECCHIO_DOCUMENTO IN (";
                            }
                            else
                            {
                                whaereQueryIn += ", ";
                            }
                        }
                        else
                        {
                            whaereQueryIn += ")";
                        }

                        k++;

                    }

                    //Se già presenti in PROFILE, mi restituisce una lista degli stessi id vecchio documento
                    List<string> listaNumeroIdVecchiPresenti = BusinessLogic.Import.ImportPregressi.PregressiManager.GetExistingNotProtocol(whaereQueryIn);

                    if (listaNumeroIdVecchiPresenti != null && listaNumeroIdVecchiPresenti.Count > 0)
                    {
                        foreach (string numPro in listaNumeroIdVecchiPresenti)
                        {
                            //Verifico se è già in W o E ed in caso lo aggiorno
                            //Aggiunto nel linq && !esitoControllo.esito
                            //ItemReportPregressi daAggiornare = (from tempProto in esitoControllo.itemPregressi where tempProto.idNumProtocolloExcel == numPro && !esitoControllo.esito select tempProto).FirstOrDefault();
                            //if (daAggiornare != null)
                            //{
                            //    esitoControllo.itemPregressi.Remove(daAggiornare);
                            //    daAggiornare.errore = concatenaMessaggio(daAggiornare, "Id Vecchio documento già presente");
                            //    daAggiornare.esito = "E";
                            //    esitoControllo.esito = false;
                            //    esitoControllo.itemPregressi.Add(daAggiornare);
                            //}
                            //else
                            //{
                            //    //altrimenti lo recupero dalla lista listaRigheDaValidare, lo aggiorno con l'errore verificato e lo aggiungo a esitoControllo
                            //    //Aggiunto nel linq && !listaRigheDaValidare.Contains(tempProto)
                            //    ItemReportPregressi daAggiungere = (from tempProto in listaRigheDaValidare where tempProto.idNumProtocolloExcel == numPro && !listaRigheDaValidare.Contains(tempProto) select tempProto).FirstOrDefault();
                            //    daAggiungere.esito = "E";
                            //    daAggiungere.errore = ("Id Vecchio documento già presente");
                            //    esitoControllo.esito = false;
                            //    esitoControllo.itemPregressi.Add(daAggiungere);
                            //}
                            //Verifico se è già in W o E ed in caso lo aggiorno
                            //Aggiunto nel linq && !esitoControllo.esito
                            //ItemReportPregressi daAggiornare = (from tempProto in esitoControllo.itemPregressi where tempProto.idNumProtocolloExcel == numPro && !esitoControllo.esito select tempProto).FirstOrDefault();
                            
                            List<ItemReportPregressi> listaDaAggiornare = esitoControllo.itemPregressi.Where(f => f.idNumProtocolloExcel == numPro).ToList();

                            if (listaDaAggiornare != null && listaDaAggiornare.Count > 0)
                            {
                                foreach (ItemReportPregressi daAggiornare in listaDaAggiornare)
                                {
                                    esitoControllo.itemPregressi.Remove(daAggiornare);
                                    daAggiornare.errore = concatenaMessaggio(daAggiornare, "Id Vecchio documento già presente");
                                    daAggiornare.esito = "E";
                                    esitoControllo.esito = false;
                                    esitoControllo.itemPregressi.Add(daAggiornare);
                                }
                            }
                            else
                            {
                                //altrimenti lo recupero dalla lista listaRigheDaValidare, lo aggiorno con l'errore verificato e lo aggiungo a esitoControllo
                                //Aggiunto nel linq && !listaRigheDaValidare.Contains(tempProto)
                                ItemReportPregressi daAggiungere = (from tempProto in listaRigheDaValidare where tempProto.idNumProtocolloExcel == numPro select tempProto).FirstOrDefault();
                                daAggiungere.esito = "E";
                                daAggiungere.errore = ("Id Vecchio documento già presente");
                                esitoControllo.esito = false;
                                esitoControllo.itemPregressi.Add(daAggiungere);
                            }
                        }
                    }

                    //Controllo successione numerico/temporale dei documenti non protocollati da inserire rispetto a quelli già esistenti in profile
                    List<ItemReportPregressi> listaItemInErroreProgressioneNumeriProfileNonProtocollati = getItemInErroreProgressioneNumeriProfileNonProtocollati(listaDocumentiNP);
                    if (listaItemInErroreProgressioneNumeriProfileNonProtocollati != null && listaItemInErroreProgressioneNumeriProfileNonProtocollati.Count > 0)
                    {
                        foreach (ItemReportPregressi protoInErr in listaItemInErroreProgressioneNumeriProfileNonProtocollati)
                        {
                            //Verifico se è già in W o E ed in caso lo aggiorno
                            ItemReportPregressi daAggiornare = (from tempProto in esitoControllo.itemPregressi where tempProto.idNumProtocolloExcel == protoInErr.idNumProtocolloExcel && tempProto.ordinale == protoInErr.ordinale select tempProto).FirstOrDefault();
                            if (daAggiornare != null)
                            {
                                esitoControllo.itemPregressi.Remove(daAggiornare);
                                daAggiornare.errore = protoInErr.errore;
                                daAggiornare.esito = "E";
                                esitoControllo.esito = false;
                                esitoControllo.itemPregressi.Add(daAggiornare);
                            }
                            else
                            {
                                esitoControllo.esito = false;
                                esitoControllo.itemPregressi.Add(protoInErr);
                            }
                        }
                    }

                }
                //

                //---------NP Controllo DATA - ID VECCHIO DOC all'interno del file.--------------
                //Verifico ogni numero documento NP precedente e successivo per verificare la corretta successione temporale della numerazione
                List<ItemReportPregressi> listaItemInErroreProgressioneNumeriDocumenti = getItemInErroreProgressioneNumeriDocumentiNonProtocollati(listaDocumentiNP);
                if (listaItemInErroreProgressioneNumeriDocumenti != null && listaItemInErroreProgressioneNumeriDocumenti.Count > 0)
                {
                    foreach (ItemReportPregressi protoInErr in listaItemInErroreProgressioneNumeriDocumenti)
                    {
                        //Verifico se è già in W o E ed in caso lo aggiorno
                        ItemReportPregressi daAggiornare = (from tempProto in esitoControllo.itemPregressi where tempProto.ordinale == protoInErr.ordinale && tempProto.idNumProtocolloExcel == protoInErr.idNumProtocolloExcel select tempProto).FirstOrDefault();
                        if (daAggiornare != null)
                        {
                            esitoControllo.itemPregressi.Remove(daAggiornare);
                            daAggiornare.errore = protoInErr.errore;
                            daAggiornare.esito = "E";
                            esitoControllo.esito = false;
                            esitoControllo.itemPregressi.Add(daAggiornare);
                        }
                        else
                        {
                            esitoControllo.esito = false;
                            esitoControllo.itemPregressi.Add(protoInErr);
                        }
                    }
                }
                //END NP--------------------------------------------------------------------------

                //Se ho protocolli pregressi con codice I
                if (listaProtocolliDaInserire.Count > 0 && listaRegistri.Count > 0)
                {
                    //prendo tutti i numero protocollo per ogni registro
                    foreach (string key in listaProtocolliDaInserire.Keys)
                    {
                        string whaereQueryIn = string.Empty;

                        int k = 0;

                        //concateno una where in (xx,xx,xx) con tutti i numeri protocollo del registro in esame
                        foreach (ItemReportPregressi prot in listaProtocolliDaInserire[key])
                        {

                            if (string.IsNullOrEmpty(whaereQueryIn))
                                whaereQueryIn = " IN (";

                            whaereQueryIn += prot.idNumProtocolloExcel;

                            if (k < listaProtocolliDaInserire[key].Count - 1)
                            {
                                if (k % 998 == 0 && k > 0)
                                {
                                    whaereQueryIn += ") OR NUM_PROTO IN (";
                                }
                                else
                                {
                                    whaereQueryIn += ", ";
                                }
                            }
                            else
                            {
                                whaereQueryIn += ")";
                            }

                            k++;

                        }

                        if (listaRegistri.ContainsKey(key))
                        {
                            string systemId_registro = listaRegistri[key].systemId;
                            string anno_registro = listaRegistri[key].anno_pregresso;
                            //Se già presenti in PROFILE, mi restituisce una lista degli stessi numeri protocollo
                            List<string> listaNumeriProtocolloEsistenti = BusinessLogic.Import.ImportPregressi.PregressiManager.GetExistingProtocolNumber(systemId_registro, whaereQueryIn, anno_registro);

                            if (listaNumeriProtocolloEsistenti != null && listaNumeriProtocolloEsistenti.Count > 0)
                            {
                                foreach (string numPro in listaNumeriProtocolloEsistenti)
                                {
                                    ////Verifico se è già in W o E ed in caso lo aggiorno
                                    ////Aggiunto nel linq && !esitoControllo.esito
                                    //ItemReportPregressi daAggiornare = (from tempProto in esitoControllo.itemPregressi where tempProto.idNumProtocolloExcel == numPro && !esitoControllo.esito select tempProto).FirstOrDefault();
                                    //if (daAggiornare != null)
                                    //{
                                    //    esitoControllo.itemPregressi.Remove(daAggiornare);
                                    //    daAggiornare.errore = concatenaMessaggio(daAggiornare, "Numero di protocollo già presente per il registro " + key);
                                    //    daAggiornare.esito = "E";
                                    //    esitoControllo.esito = false;
                                    //    esitoControllo.itemPregressi.Add(daAggiornare);
                                    //}
                                    //else
                                    //{
                                    //    //altrimenti lo recupero dalla lista listaRigheDaValidare, lo aggiorno con l'errore verificato e lo aggiungo a esitoControllo
                                    //    //Aggiunto nel linq && !listaRighedavalidare.contains(daAggiornare)
                                    //    ItemReportPregressi daAggiungere = (from tempProto in listaRigheDaValidare where tempProto.idNumProtocolloExcel == numPro && !listaRigheDaValidare.Contains(daAggiornare) select tempProto).FirstOrDefault();
                                    //    daAggiungere.esito = "E";
                                    //    daAggiungere.errore = ("Numero di protocollo già presente per il registro " + key);
                                    //    esitoControllo.esito = false;
                                    //    esitoControllo.itemPregressi.Add(daAggiungere);
                                    //}
                                    List<ItemReportPregressi> listaDaAggiornare = esitoControllo.itemPregressi.Where(f => f.idNumProtocolloExcel == numPro).ToList();

                                    if (listaDaAggiornare != null && listaDaAggiornare.Count > 0)
                                    {
                                        foreach (ItemReportPregressi daAggiornare in listaDaAggiornare)
                                        {
                                            esitoControllo.itemPregressi.Remove(daAggiornare);
                                            daAggiornare.errore = concatenaMessaggio(daAggiornare, "Numero di protocollo già presente per il registro " + key);
                                            daAggiornare.esito = "E";
                                            esitoControllo.esito = false;
                                            esitoControllo.itemPregressi.Add(daAggiornare);
                                        }
                                    }
                                    else
                                    {
                                        //altrimenti lo recupero dalla lista listaRigheDaValidare, lo aggiorno con l'errore verificato e lo aggiungo a esitoControllo
                                        //Aggiunto nel linq && !listaRigheDaValidare.Contains(tempProto)
                                        ItemReportPregressi daAggiungere = (from tempProto in listaRigheDaValidare where tempProto.idNumProtocolloExcel == numPro select tempProto).FirstOrDefault();
                                        daAggiungere.esito = "E";
                                        daAggiungere.errore = ("Numero di protocollo già presente per il registro " + key);
                                        esitoControllo.esito = false;
                                        esitoControllo.itemPregressi.Add(daAggiungere);
                                    }
                                }
                            }

                            //Controllo successione numerico/temporale dei num protocolli da inserire rispetto a quelli già esistenti in profile
                            List<ItemReportPregressi> listaItemInErroreProgressioneNumeriProfile = getItemInErroreProgressioneNumeriProfile(listaProtocolliDaInserire[key], systemId_registro);
                            if (listaItemInErroreProgressioneNumeriProfile != null && listaItemInErroreProgressioneNumeriProfile.Count > 0)
                            {
                                foreach (ItemReportPregressi protoInErr in listaItemInErroreProgressioneNumeriProfile)
                                {
                                    //Verifico se è già in W o E ed in caso lo aggiorno
                                    ItemReportPregressi daAggiornare = (from tempProto in esitoControllo.itemPregressi where tempProto.idNumProtocolloExcel == protoInErr.idNumProtocolloExcel && tempProto.ordinale == protoInErr.ordinale select tempProto).FirstOrDefault();
                                    if (daAggiornare != null)
                                    {
                                        esitoControllo.itemPregressi.Remove(daAggiornare);
                                        daAggiornare.errore = protoInErr.errore;
                                        daAggiornare.esito = "E";
                                        esitoControllo.esito = false;
                                        esitoControllo.itemPregressi.Add(daAggiornare);
                                    }
                                    else
                                    {
                                        esitoControllo.esito = false;
                                        esitoControllo.itemPregressi.Add(protoInErr);
                                    }
                                }
                            }

                        }

                        //Verifico ogni unmero protocollo precedente e successivo per verificare la corretta successione temporale della numerazione
                        List<ItemReportPregressi> listaItemInErroreProgressioneNumeriProtocollo = getItemInErroreProgressioneNumeriProtocollo(listaProtocolliDaInserire[key]);
                        if (listaItemInErroreProgressioneNumeriProtocollo != null && listaItemInErroreProgressioneNumeriProtocollo.Count > 0)
                        {
                            foreach (ItemReportPregressi protoInErr in listaItemInErroreProgressioneNumeriProtocollo)
                            {
                                //Verifico se è già in W o E ed in caso lo aggiorno
                                ItemReportPregressi daAggiornare = (from tempProto in esitoControllo.itemPregressi where tempProto.ordinale == protoInErr.ordinale && tempProto.idNumProtocolloExcel == protoInErr.idNumProtocolloExcel select tempProto).FirstOrDefault();
                                if (daAggiornare != null)
                                {
                                    esitoControllo.itemPregressi.Remove(daAggiornare);
                                    daAggiornare.errore = protoInErr.errore;
                                    daAggiornare.esito = "E";
                                    esitoControllo.esito = false;
                                    esitoControllo.itemPregressi.Add(daAggiornare);
                                }
                                else
                                {
                                    esitoControllo.esito = false;
                                    esitoControllo.itemPregressi.Add(protoInErr);
                                }
                            }
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                esitoControllo.itemPregressi = new List<ItemReportPregressi>();

                sl.Log("");
                sl.Log("ERRROE : " + ex.Message);
                ItemReportPregressi reportItem = new ItemReportPregressi();
                reportItem.idNumProtocolloExcel = "0";
                reportItem.codRegistro = "0";
                reportItem.ordinale = "0";
                reportItem.esito = "E";
                reportItem.errore = "File errato o non compatibile.";
                esitoControllo.esito = false;

                esitoControllo.itemPregressi.Add(reportItem);

                return esitoControllo;
            }
            finally
            {
                if (xlsReader != null)
                    xlsReader.Close();
                if (xlsConn != null)
                    xlsConn.Close();
            }

            //Ordino per riga del foglio excel
            if (esitoControllo != null && esitoControllo.itemPregressi != null && esitoControllo.itemPregressi.Count > 0)
            {
                try
                {
                    List<ItemReportPregressi> elementiConOrdinaliVuoti = new List<ItemReportPregressi>();
                    //Catturo elementi che hanno ordinale vuoto
                    foreach (ItemReportPregressi proto in esitoControllo.itemPregressi)
                    {
                        if (string.IsNullOrEmpty(proto.ordinale))
                        {
                            //Aggiungo elementi nella lista di appoggio
                            elementiConOrdinaliVuoti.Add(proto);
                        }
                    }

                    foreach (ItemReportPregressi proto in elementiConOrdinaliVuoti)
                    {
                        //Tolgo elementi dalla lista di output che hanno ordinali vuoti
                        esitoControllo.itemPregressi.Remove(proto);
                    }

                    //Ordino elementi a cui ho tolto ordinale vuoto nella lista
                    esitoControllo.itemPregressi = esitoControllo.itemPregressi.OrderBy(x => Convert.ToInt32(x.ordinale)).ToList();

                    //In ultimo aggiungo quelli con ordinale vuoto
                    foreach (ItemReportPregressi protoDaAggiungere in elementiConOrdinaliVuoti)
                    {
                        //Aggiungo elementi con ordinale vuoto
                        esitoControllo.itemPregressi.Add(protoDaAggiungere);
                    }
                }
                catch
                {
                }
            }

            return esitoControllo;
        }

        private static List<string> getValoriProfilati(OleDbDataReader xlsRead, List<string> colum_map)
        {
            List<string> result = new List<string>();

            int indexColonnaDocumento = colum_map.IndexOf("TIPOLOGIA DOCUMENTO");
            int indexColonnaCodiceFascicolo = colum_map.IndexOf("CODICE FASCICOLO");
            if (colum_map.Count > indexColonnaDocumento)
            {
                for (int i = indexColonnaDocumento + 1; i < colum_map.Count; i++)
                {
                    if (i < indexColonnaCodiceFascicolo)
                    {
                        string valore = get_string(xlsRead, i);
                        if (!string.IsNullOrEmpty(valore))
                            result.Add(valore);
                    }
                }
            }

            return result;
        }

        private static string getStringDataFormatted(string stringdata)
        {
            string result = stringdata;

            if ((!string.IsNullOrEmpty(stringdata)) && (stringdata.Length < 10))
            {
                string[] stringArray = stringdata.Split('/');
                if (stringArray.Count() == 3)
                {
                    result = (stringArray[0].Length > 1 ? stringArray[0] : "0" + stringArray[0]);
                    result += (stringArray[1].Length > 1 ? "/" + stringArray[1] : "/0" + stringArray[1]);
                    result += (stringArray[2].Length > 3 ? "/" + stringArray[2] : "/20" + stringArray[2]);
                }
            }

            return result;
        }

        private static List<ItemReportPregressi> getItemInErroreProgressioneNumeriProtocollo(List<ItemReportPregressi> listaItem)
        {
            bool errorFound = false;
            List<ItemReportPregressi> resultList = new List<ItemReportPregressi>();

            int indice = 0;
            List<ItemReportPregressi> listaOrdinata = listaItem.OrderBy(x => Convert.ToInt32(x.idNumProtocolloExcel)).ToList();
            foreach (ItemReportPregressi corrente in listaOrdinata)
            {
                ItemReportPregressi precedente = null;
                ItemReportPregressi successivo = null;

                if (indice > 0)
                    precedente = listaOrdinata[indice - 1];

                if (indice < (listaOrdinata.Count() - 1))
                    successivo = listaOrdinata[indice + 1];

                if (precedente != null && (corrente.idNumProtocolloExcel != precedente.idNumProtocolloExcel))
                {
                    if (Convert.ToDateTime(corrente.data).Date < Convert.ToDateTime(precedente.data).Date)
                    {
                        errorFound = true;
                        corrente.esito = "E";
                        //corrente.errore = concatenaMessaggio(corrente, "Progressione numero protocollo errata. Esiste un protocollo precedente con data maggiore");
                        corrente.errore = concatenaMessaggio(corrente, "Progressione numero protocollo errata. Esiste un protocollo precedente con data successiva");
                        resultList.Add(corrente);
                    }
                }

                if (successivo != null && (corrente.idNumProtocolloExcel != successivo.idNumProtocolloExcel))
                {
                    if (Convert.ToDateTime(corrente.data).Date > Convert.ToDateTime(successivo.data).Date)
                    {
                        ItemReportPregressi correntOld = corrente;
                        corrente.esito = "E";
                        //corrente.errore = concatenaMessaggio(corrente, "Progressione numero protocollo errata. Esiste un protocollo successivo con data minore");
                        corrente.errore = concatenaMessaggio(corrente, "Progressione numero protocollo errata. Esiste un protocollo successivo con data precedente");
                        if (errorFound)
                            resultList.Remove(correntOld);

                        resultList.Add(corrente);
                    }
                }

                errorFound = false;
                indice++;
            }

            return resultList;
        }

        private static List<ItemReportPregressi> getItemInErroreProgressioneNumeriDocumentiNonProtocollati(List<ItemReportPregressi> listaItem)
        {
            bool errorFound = false;
            List<ItemReportPregressi> resultList = new List<ItemReportPregressi>();

            int indice = 0;
            List<ItemReportPregressi> listaOrdinata = listaItem.OrderBy(x => Convert.ToInt32(x.idNumProtocolloExcel)).ToList();
            foreach (ItemReportPregressi corrente in listaOrdinata)
            {
                ItemReportPregressi precedente = null;
                ItemReportPregressi successivo = null;

                if (indice > 0)
                    precedente = listaOrdinata[indice - 1];

                if (indice < (listaOrdinata.Count() - 1))
                    successivo = listaOrdinata[indice + 1];

                if (precedente != null && (corrente.idNumProtocolloExcel != precedente.idNumProtocolloExcel))
                {
                    if (Convert.ToDateTime(corrente.data).Date < Convert.ToDateTime(precedente.data).Date)
                    {
                        errorFound = true;
                        corrente.esito = "E";
                        //corrente.errore = concatenaMessaggio(corrente, "Progressione id documento errata. Esiste un documento non protocollato precedente con data maggiore");
                        corrente.errore = concatenaMessaggio(corrente, "Progressione id documento errata. Esiste un documento non protocollato precedente con data successiva");
                        resultList.Add(corrente);
                    }
                }

                if (successivo != null && (corrente.idNumProtocolloExcel != successivo.idNumProtocolloExcel))
                {
                    if (Convert.ToDateTime(corrente.data).Date > Convert.ToDateTime(successivo.data).Date)
                    {
                        ItemReportPregressi correntOld = corrente;
                        corrente.esito = "E";
                        //corrente.errore = concatenaMessaggio(corrente, "Progressione id documento errata. Esiste un documento non protocollato successivo con data minore");
                        corrente.errore = concatenaMessaggio(corrente, "Progressione id documento errata. Esiste un documento non protocollato successivo con data precedente");
                        if (errorFound)
                            resultList.Remove(correntOld);

                        resultList.Add(corrente);
                    }
                }

                errorFound = false;
                indice++;
            }

            return resultList;
        }

        private static List<ItemReportPregressi> getItemInErroreProgressioneNumeriProfile(List<ItemReportPregressi> listaItem, string idregistro)
        {
            List<string> parametriDellaFunzioneDB = new List<string>();
            List<string> resultList = new List<string>();
            List<ItemReportPregressi> returnList = new List<ItemReportPregressi>();

            foreach (ItemReportPregressi corrente in listaItem)
            {
                if (!string.IsNullOrEmpty(corrente.idNumProtocolloExcel) && !string.IsNullOrEmpty(corrente.data))
                {
                    string tempstring = "(" + corrente.idNumProtocolloExcel.Trim() + "," + DocsPaDbManagement.Functions.Functions.ToDate(corrente.data) + "," + idregistro.Trim() + ")";
                    parametriDellaFunzioneDB.Add(tempstring);
                }
            }

            if (parametriDellaFunzioneDB.Count > 0)
                resultList = BusinessLogic.Import.ImportPregressi.PregressiManager.GetInvalidProtocolNumber(parametriDellaFunzioneDB);

            foreach (string idNumProto in resultList)
            {
                ItemReportPregressi tempProto = (from proto in listaItem where proto.idNumProtocolloExcel == idNumProto select proto).FirstOrDefault();
                if (tempProto != null)
                {
                    tempProto.esito = "E";
                    tempProto.errore = concatenaMessaggio(tempProto, "Progressione numerico temporale del protocollo in conflitto con protocolli esistenti.");
                    returnList.Add(tempProto);
                }
            }

            return returnList;
        }

        private static bool numProtoUnivocoInFile(ItemReportPregressi protocollo, List<ItemReportPregressi> listaProtocolli)
        {
            bool esito = false;

            int numprotofound = (from ItemReportPregressi tempProto in listaProtocolli where tempProto.idNumProtocolloExcel == protocollo.idNumProtocolloExcel && tempProto.codRegistro == protocollo.codRegistro && tempProto.tipoOperazione == "I" && !tempProto.tipoProtocollo.ToUpper().Equals("NP") select protocollo.idNumProtocolloExcel).Count();
            if (numprotofound < 2)
            {
                esito = true;
            }

            return esito;
        }

        //Andrea - Funzione per verificare l'esistenza di un id_vecchio_documento Univoco
        private static bool idVecchioDocumentoUnivocoInFile(ItemReportPregressi protocollo, List<ItemReportPregressi> listaProtocolli)
        {
            bool esito = false;

            int numIdVeccioDocfound = (from ItemReportPregressi tempDoc in listaProtocolli where tempDoc.idNumProtocolloExcel == protocollo.idNumProtocolloExcel && tempDoc.tipoOperazione == "I" && tempDoc.tipoProtocollo.ToUpper().Equals("NP") select protocollo.idNumProtocolloExcel).Count();
            if (numIdVeccioDocfound < 2)
            {
                esito = true;
            }

            return esito;
        }

        private static bool tipoProtoCorretto(ItemReportPregressi protocollo)
        {
            return (protocollo.tipoProtocollo.ToUpper().Equals("P") || protocollo.tipoProtocollo.ToUpper().Equals("A") || protocollo.tipoProtocollo.ToUpper().Equals("I"));
        }

        private static bool tipoOperazioneCorretta(ItemReportPregressi protocollo)
        {
            return (protocollo.tipoOperazione.ToUpper().Equals("M") || protocollo.tipoOperazione.Equals("I") || protocollo.tipoOperazione.Equals("C"));
        }

        private static bool annoProtocolloValido(ItemReportPregressi protocollo, Registro registro)
        {
            DateTime dataProtocollo = DateTime.ParseExact(protocollo.data, "dd/mm/yyyy", null);

            return (dataProtocollo.Year.ToString().Equals(registro.anno_pregresso.ToString()));
        }

        private static bool registroValido(Registro registro)
        {
            return (registro != null ? registro.flag_pregresso : false);
        }

        private static bool codiciUtenteValidi(ItemReportPregressi protocollo, string id_amm)
        {
            bool esito = false;
            string key = protocollo.idRuolo + protocollo.idUtente;
            if (listaCodiciUtenteVerificati.ContainsKey(key))
            {
                if (listaCodiciUtenteVerificati[key].ElementAtOrDefault(0) != null)
                {
                    protocollo.idRuolo = listaCodiciUtenteVerificati[key].ElementAtOrDefault(0);
                    protocollo.idUtente = listaCodiciUtenteVerificati[key].ElementAtOrDefault(1);
                    esito = true;
                }
            }
            else
            {
                List<string> listaIdPeopleIdRuoloFromCodici = BusinessLogic.Import.ImportPregressi.PregressiManager.GetIdPeopleIdRuoloFromCodici(protocollo.idUtente, protocollo.idRuolo, id_amm);
                if (listaIdPeopleIdRuoloFromCodici != null && listaIdPeopleIdRuoloFromCodici.Count > 0)
                {
                    listaCodiciUtenteVerificati.Add(key, listaIdPeopleIdRuoloFromCodici);
                    protocollo.idRuolo = listaIdPeopleIdRuoloFromCodici.ElementAtOrDefault(0);
                    protocollo.idUtente = listaIdPeopleIdRuoloFromCodici.ElementAtOrDefault(1);
                    esito = true;
                }
            }

            return esito;
        }

        private static string get_string(OleDbDataReader dr, int field)
        {
            if (dr[field] == null || dr[field] == System.DBNull.Value)
                return "";
            else
                return dr[field].ToString();
        }

        private static string concatenaMessaggio(ItemReportPregressi prot, string messaggio)
        {
            if (string.IsNullOrEmpty(prot.errore))
                return messaggio;
            else
                return prot.errore + separatoreMessaggi + messaggio;
        }

        private static bool numProtoUnivoco(ItemReportPregressi protocollo, List<ItemReportPregressi> listaProtocolli)
        {
            bool esito = false;

            //Verifico che non esista già il numero protocollo per il registro specificato
            //if (classe.bo.eiste()
            return esito;
        }

        private static bool fileConforme(List<string> colonne)
        {
            bool esito = false;

            if (colonne.Where(t => t == "ORDINALE" || t == "TIPO PROTOCOLLO" || t == "TIPO OPERAZIONE" || t == "NUMERO DI PROTOCOLLO" || t == "DATA PROTOCOLLO" || t == "CODICE REGISTRO").Count() == 6)
                if (colonne.Where(t => t == "CODICE OGGETTO" || t == "OGGETTO").Count() > 0)
                    if (colonne.Where(t => t == "CODICI CORRISPONDENTI" || t == "CORRISPONDENTI").Count() > 0)
                        esito = true;

            return esito;
        }

        //Importazione dei dati nel database.
        //Andrea - aggiunta parametro descrizione al report
        public static ReportPregressi ImportaPregresso(DocsPaVO.utente.InfoUtente infoUtente, EsitoImportPregressi esitoPregressi, string descrizione)
        {
            ILog logger = LogManager.GetLogger(typeof(ImportPregressiManager));

            ReportPregressi report = new ReportPregressi();
            report.dataEsecuzione = DateTime.Today.ToShortDateString();
            report.idUtenteCreatore = infoUtente.idPeople;
            report.idAmm = infoUtente.idAmministrazione;
            report.idRuoloCreatore = infoUtente.idGruppo;
            //Andrea - descrizione del report
            report.descrizione = descrizione;


            foreach (ItemReportPregressi protocollo in esitoPregressi.itemPregressi)
            {
                report.AddItemReportPregressi(protocollo);
            }

            try
            {
                //report.numDoc = report.itemPregressi.Count().ToString();
                DocsPaDB.Query_DocsPAWS.ImportPregressi importazione = new DocsPaDB.Query_DocsPAWS.ImportPregressi();
                report.systemId = importazione.InsertReportPregressi(report);

                if (!string.IsNullOrEmpty(report.systemId))
                {
                    foreach (ItemReportPregressi item in report.itemPregressi)
                    {
                        string id_item = importazione.InsertItemReportPregressi(item, report.systemId);
                        if (!string.IsNullOrEmpty(id_item))
                        {
                            item.systemId = id_item;
                            foreach (Allegati allegato in item.Allegati)
                            {
                                string id_all = importazione.InsertAllegatoItemReportPregressi(allegato, id_item);
                                if (!string.IsNullOrEmpty(id_all))
                                {
                                    allegato.systemId = id_all;
                                }
                                else
                                {
                                    //ERRORE INSERIMENTO ALLEGATO
                                    logger.Debug("Inserimento allegato all'item pregressi " + id_item + " fallito.");
                                }
                            }
                        }
                        else
                        {
                            //ERRORE INSERIMENTO PROTOCOLLO PREGRESSO
                            logger.Debug("Inserimento item pregresso per il report " + report.systemId + " fallito.");
                        }
                    }
                }
                else
                {
                    //ERRORE INSERIMENTO REPORT PREGRESSI
                    logger.Debug("Inserimento report pregresso nel db fallito.");
                }
            }
            catch (Exception ex)
            {
                logger.Debug("ERRROE : " + ex.Message);
            }

            return report;
        }

        //Creazione dei documenti
        public static void CreaDocumenti(DocsPaVO.utente.InfoUtente infoUtente, ReportPregressi report)
        {
            //Tengo traccia degli oggetti già calcolati
            Dictionary<string, ArrayList> DictionaryIdRuoli = new Dictionary<string, ArrayList>();
            Dictionary<string, DocsPaVO.utente.Utente> utenti = new Dictionary<string, DocsPaVO.utente.Utente>();
            Dictionary<string, DocsPaVO.utente.Ruolo> ruoli = new Dictionary<string, DocsPaVO.utente.Ruolo>();
            Dictionary<string, DocsPaVO.utente.Registro> registri = new Dictionary<string, DocsPaVO.utente.Registro>();
            List<DocsPaVO.documento.Oggetto> oggettario = new List<DocsPaVO.documento.Oggetto>();
            List<DocsPaVO.utente.Corrispondente> corrispondenti = new List<DocsPaVO.utente.Corrispondente>();
            Dictionary<string, DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione> modelli = new Dictionary<string, DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione>();
            Dictionary<string, DocsPaVO.utente.Registro> listaRf = new Dictionary<string, DocsPaVO.utente.Registro>();
            Dictionary<string, DocsPaVO.ProfilazioneDinamica.Templates> tipiDocumento = new Dictionary<string, DocsPaVO.ProfilazioneDinamica.Templates>();
            abilitazioneRubricaComune = BusinessLogic.RubricaComune.Configurazioni.GetConfigurazioni(infoUtente).GestioneAbilitata;
            string intervalloTimer = string.Empty;
            System.Timers.Timer aTimer = new System.Timers.Timer();
            boolImporta = false;
            Dictionary<string, Registro> DictionaryCodRegFasc_RegFasc = new Dictionary<string, Registro>();
            Dictionary<string, DocsPaVO.fascicolazione.Fascicolo> DictionaryCodFasc_Fasc = new Dictionary<string, DocsPaVO.fascicolazione.Fascicolo>();

            if (report != null && report.itemPregressi != null && report.itemPregressi.Count > 0)
            {


                DocsPaDB.Query_DocsPAWS.ImportPregressi importazione = new DocsPaDB.Query_DocsPAWS.ImportPregressi();

                intervalloTimer = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TIMER_PREGRESSI");

                if (!string.IsNullOrEmpty(intervalloTimer) && !intervalloTimer.Equals("0"))
                {
                    boolImporta = BusinessLogic.UserLog.UserLog.VerificaLogAggiungiDocumento(intervalloTimer);
                    int valoreTimer = Int32.Parse(intervalloTimer);
                    valoreTimer = valoreTimer * 1000;
                    // Hook up the Elapsed event for the timer.
                    aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                    // Set the Interval to 2 seconds (2000 milliseconds).
                    aTimer.Interval = valoreTimer;
                    aTimer.Enabled = true;
                    aTimer.Start();
                }

                foreach (ItemReportPregressi item in report.itemPregressi)
                {
                    try
                    {
                        bool esitoAcqFile = true;
                        bool esitoTrasmissioni = true;
                        string messageError = string.Empty;
                        List<string> errori = new List<string>();
                        DocsPaVO.utente.Utente user = null;
                        DocsPaVO.utente.Ruolo role = null;
                        DocsPaVO.utente.Registro register = null;
                        DocsPaVO.utente.Registro rf = null;
                        DocsPaVO.ProfilazioneDinamica.Templates template = null;
                        bool inserisci = true;
                        string oggetto = string.Empty;

                        //INSERIMENTO SEMAFORO DI ATTESA
                        if (!string.IsNullOrEmpty(intervalloTimer) && !intervalloTimer.Equals("0"))
                        {
                            while (!boolImporta)
                            {
                                //In attesa della protocollazione
                            }
                        }

                        if (!string.IsNullOrEmpty(item.tipoOperazione) && (item.tipoOperazione.ToUpper().Equals("I") || item.tipoOperazione.ToUpper().Equals("M") || item.tipoOperazione.ToUpper().Equals("C")))
                        {

                            if (item.tipoProtocollo.ToUpper().Equals("NP"))
                            {
                                #region INSERIMENTO
                                if (item.tipoOperazione.ToUpper().Equals("I"))
                                {
                                    DocsPaVO.utente.InfoUtente infoUtenteProprietario = null;
                                    if (!string.IsNullOrEmpty(item.idUtente) && !string.IsNullOrEmpty(item.idRuolo) && (string.IsNullOrEmpty(infoUtente.idPeople) || string.IsNullOrEmpty(infoUtente.idGruppo) || !infoUtente.idPeople.Equals(item.idUtente) || !infoUtente.idGruppo.Equals(item.idRuolo)))
                                    {
                                        //Prendo l'utente
                                        user = GetUtente(item, ref utenti);
                                        //Prendo il ruolo
                                        role = GetRuolo(item, ref ruoli);
                                        infoUtenteProprietario = ImpostaInfoUtente(user, role);
                                    }
                                    else
                                    {
                                        infoUtenteProprietario = infoUtente;
                                        //Prendo l'utente
                                        ItemReportPregressi tempUs = new ItemReportPregressi();
                                        tempUs.idUtente = infoUtenteProprietario.idPeople;
                                        tempUs.idRuolo = infoUtenteProprietario.idGruppo;
                                        user = GetUtente(tempUs, ref utenti);
                                        //Prendo il ruolo
                                        role = GetRuolo(tempUs, ref ruoli);
                                    }

                                    //Controllo e se presente prendo l'oggetto
                                    oggetto = GetOggetto(item, null, infoUtente, ref errori, ref inserisci, ref oggettario);

                                    //Creo documento
                                    DocsPaVO.documento.SchedaDocumento schedaDoc = DocManager.NewSchedaDocumento(infoUtenteProprietario);
                                    //Creazione Documento
                                    schedaDoc.tipoProto = "G";

                                    template = GetTemplate(item, infoUtenteProprietario, user, role, null, null, ref errori, ref inserisci, ref tipiDocumento, DictionaryIdRuoli);

                                    if (inserisci)
                                    {
                                        schedaDoc = CreaDocumentoNonProtocollato(item, oggetto, infoUtenteProprietario, template, role, schedaDoc, ref esitoAcqFile);

                                        //Aggiorno le date CreationDate, CreationTime, LastEditDate nella PROFILE.
                                        schedaDoc.dataCreazione = item.data;
                                        UpdateDataCreazioneDocGrigio(schedaDoc, item.idNumProtocolloExcel);

                                        if (schedaDoc == null && schedaDoc.systemId == null)
                                        {
                                            item.esito = "E";
                                            errori.Add("Errore nella creazione del documento");
                                            foreach (string err in errori)
                                            {
                                                item.errore += err + " - ";
                                            }
                                            importazione.UpdateItemReportPregressi(item, item.systemId);
                                        }
                                        else
                                        {
                                            //Caso di successo
                                            item.esito = "S";
                                            item.idDocumento = schedaDoc.docNumber;
                                            importazione.UpdateItemReportPregressi(item, item.systemId);

                                            bool esitoAllegati = true;
                                            int allegati = 0;

                                            if (item.Allegati != null && item.Allegati.Count > 0)
                                            {
                                                schedaDoc.allegati = new ArrayList();

                                                foreach (Allegati allTemp in item.Allegati)
                                                {
                                                    try
                                                    {
                                                        Allegato allegato = CreaAllegato(allTemp, ref schedaDoc, infoUtenteProprietario);
                                                        schedaDoc.allegati.Add(allegato);
                                                        allTemp.esito = "S";
                                                        importazione.UpdateAllegatoItemReportPregressi(allTemp, item.systemId);
                                                    }
                                                    catch
                                                    {
                                                        esitoAllegati = false;
                                                        //errori.Add("Errore nella creazione dell'allegato");
                                                        allTemp.esito = "E";
                                                        importazione.UpdateAllegatoItemReportPregressi(allTemp, item.systemId);
                                                        allegati++;
                                                    }
                                                }


                                            }

                                            ControlloADL(item, infoUtenteProprietario, role, schedaDoc);

                                            ControlloTrasmissioni(item, infoUtenteProprietario, infoUtente, role, schedaDoc, importazione, ref errori, ref esitoTrasmissioni, ref DictionaryIdRuoli);

                                            if (!esitoAllegati)
                                            {
                                                item.esito = "W";
                                                string totaleErrore = string.Empty;
                                                if (allegati > 1)
                                                {
                                                    totaleErrore = "Errore nell'acquisizione del file di" + allegati + "allegati";
                                                }
                                                else
                                                {
                                                    totaleErrore = "Errore nell'acquisizione del file di un allegato";
                                                }

                                                item.errore = concatenaMessaggio(item, totaleErrore);
                                                //importazione.UpdateItemReportPregressi(item, item.systemId);
                                            }

                                            if (!esitoAllegati || !esitoTrasmissioni || !esitoAcqFile)
                                            {
                                                item.esito = "W";
                                                importazione.UpdateItemReportPregressi(item, item.systemId);
                                            }
                                            
                                            //ANDREA - Occorre inserire il documento all'interno del fascicolo
                                            #region FASCICOLAZIONE

                                            string administrationSyd = string.Empty;
                                            //Popolo l'id dell'amministrazione a partire dall'infoUtenteProprietario
                                            administrationSyd = infoUtenteProprietario.idAmministrazione;

                                            //Dictionary per non calcolare ogni volta il Registro del Fascicolo
                                            //Dictionary<string, Registro> DictionaryCodRegFasc_RegFasc = new Dictionary<string, Registro>();
                                            Registro registro_Fascicolo = null;

                                            if (!string.IsNullOrEmpty(item.codiceRegistroFascicolo))
                                            {
                                                if (DictionaryCodRegFasc_RegFasc.ContainsKey(item.codiceRegistroFascicolo))
                                                {
                                                    registro_Fascicolo = DictionaryCodRegFasc_RegFasc[item.codiceRegistroFascicolo];
                                                }
                                                else
                                                {
                                                    //Calcolo il Registro del facicolo a partire dal codice del registro
                                                    registro_Fascicolo = RegistriManager.getRegistroByCode(item.codiceRegistroFascicolo);
                                                    if (registro_Fascicolo != null)
                                                        DictionaryCodRegFasc_RegFasc.Add(item.codiceRegistroFascicolo, registro_Fascicolo);
                                                }
                                            }
                                            //End Dictionary Registro Facsicolo

                                            string idTitolario = string.Empty;
                                            //item.Titolario è stato cablato in CheckFileData in modo da prendere il titolario attivo
                                            
                                            /*
                                            //
                                            //Se nel Projectcode c'è # allora cerco il titolario, altrimenti prendo il titolario attivo
                                            if (item.ProjectCodes[0].Contains("#"))
                                                item.Titolario = "titolario non attivo";
                                            else
                                                item.Titolario = string.Empty;
                                            //
                                            //End impostazione del titolario
                                            */

                                            //
                                            //Gestione per il recupero dell'IDTitolario (Attivo oppure Storicizzato):
                                            bool continuaFasc = true;
                                            if (!string.IsNullOrEmpty(item.Titolario))
                                            {
                                                //
                                                //Titolario non attivo - Storicizzato
                                                
                                                //
                                                //TO DO: Recupero IDTitolario
                                                //cerchiamo il titolario su cui fascicolare a partire dal codice del facsicolo o dalla descrizione
                                                if (registro_Fascicolo != null)
                                                {
                                                    List<string> ListaidTitolario = new List<string>();

                                                    ListaidTitolario = BusinessLogic.Import.ImportUtils.getTitolarioByCodeFascicolo(item.ProjectCodes[0], item.ProjectDescription, administrationSyd, registro_Fascicolo.systemId);
                                                    if (ListaidTitolario.Count > 0)
                                                    {
                                                        //
                                                        //Ho trovato almeno un IDTitolario per quel CodiceFascicolo
                                                        if (ListaidTitolario.Count > 1)
                                                        {
                                                            //
                                                            //C'è più di un IDTitolario, quindi non è possibile individuare un Titolario univoco in cui reperire quel fascicolo e non è possibile fascicolare.
                                                            //Aggiorno l'esito della riga del foglio excel che si vuole importare.
                                                            idTitolario = string.Empty;
                                                            item.esito = "W";
                                                            string messErr = "il fascicolo è presente in più di un titolario.";
                                                            item.errore = concatenaMessaggio(item, messErr);
                                                            importazione.UpdateItemReportPregressi(item, item.systemId);
                                                            continuaFasc = false;
                                                        }
                                                        else
                                                        {
                                                            //
                                                            //Ho trovato un solo titolario su cui fascicolare
                                                            idTitolario = ListaidTitolario[0];
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //Nessun Id Titolario Trovato
                                                        idTitolario = string.Empty;
                                                        item.esito = "W";
                                                        string messErr = "Nessun titolario trovato per il fascicolo " + item.codiceRegistroFascicolo;
                                                        item.errore = concatenaMessaggio(item, messErr);
                                                        importazione.UpdateItemReportPregressi(item, item.systemId);
                                                        continuaFasc = false;
                                                    }
                                                    //
                                                    //End recupero IDTitolario
                                                }
                                                else
                                                {
                                                    item.esito = "W";
                                                    string messErr = "Registro associato al fascicolo mancante.";
                                                    item.errore = concatenaMessaggio(item, messErr);
                                                    importazione.UpdateItemReportPregressi(item, item.systemId);
                                                    continuaFasc = false;
                                                }
                                                //try
                                                //{
                                                //    //Recupero del titolario Attivo a partire dall'amministrazione e dal nome del titolario
                                                //    idTitolario = BusinessLogic.Import.ImportUtils.GetTitolarioId(item.Titolario, administrationSyd);
                                                //}
                                                //catch (Exception e)
                                                //{
                                                //}
                                            }
                                            else
                                            {
                                                //
                                                //Recupero Titolario Attivo
                                                try
                                                {
                                                    string codAmministrazione = string.Empty;
                                                    idTitolario = BusinessLogic.Amministrazione.TitolarioManager.getTitolarioAttivo(administrationSyd, out codAmministrazione);
                                                }
                                                catch (Exception e)
                                                {
                                                }
                                            }

                                            if (continuaFasc)
                                            {
                                                List<string> tempProblems = new List<string>();
                                                List<string> projectIds = new List<string>();

                                                ////Dictionary per non calcolare ogni volta il Registro del Fascicolo
                                                ////Dictionary<string, Registro> DictionaryCodRegFasc_RegFasc = new Dictionary<string, Registro>();
                                                //Registro registro_Fascicolo = null;

                                                //if (!string.IsNullOrEmpty(item.codiceRegistroFascicolo))
                                                //{
                                                //    if (DictionaryCodRegFasc_RegFasc.ContainsKey(item.codiceRegistroFascicolo))
                                                //    {
                                                //        registro_Fascicolo = DictionaryCodRegFasc_RegFasc[item.codiceRegistroFascicolo];
                                                //    }
                                                //    else
                                                //    {
                                                //        //Calcolo il Registro del facicolo a partire dal codice del registro
                                                //        registro_Fascicolo = RegistriManager.getRegistroByCode(item.codiceRegistroFascicolo);
                                                //        if (registro_Fascicolo != null)
                                                //            DictionaryCodRegFasc_RegFasc.Add(item.codiceRegistroFascicolo, registro_Fascicolo);
                                                //    }
                                                //}
                                                ////End Dictionary Registro Facsicolo

                                                //Prendo il registro relativo al codice del registro fascicolo
                                                //Registro registroFascicolo = RegistriManager.getRegistroByCode(item.codiceRegistroFascicolo);

                                                // Reperimento degli identificativi dei fascicoli se almeno uno fra i seguenti campi risulta valorizzato
                                                // ProjectCodes, ProjectDescription, FolderDescription, NodeCode
                                                if ((item.ProjectCodes != null && item.ProjectCodes.Length > 0) ||
                                                    !String.IsNullOrEmpty(item.ProjectDescription) ||
                                                    !String.IsNullOrEmpty(item.FolderDescrition) ||
                                                    !String.IsNullOrEmpty(item.NodeCode))
                                                {
                                                    // La lista dei fascicoli in cui inserire il documento
                                                    //projectIds = GetProjectsForClassification(item, infoUtenteProprietario, role, administrationSyd, register.systemId, rf, idTitolario, out tempProblems, false);
                                                    //projectIds = GetProjectsForClassification(item, infoUtenteProprietario, role, administrationSyd, registroFascicolo.systemId, null, idTitolario, out tempProblems, false);
                                                    if (registro_Fascicolo != null)
                                                    {
                                                        projectIds = GetProjectsForClassification(item, infoUtenteProprietario, role, administrationSyd, registro_Fascicolo.systemId, null, idTitolario, out tempProblems, false, ref DictionaryCodFasc_Fasc);
                                                    }
                                                    else
                                                    {
                                                        tempProblems.Add("Registro associato al fascicolo mancante.");
                                                    }
                                                }

                                                if (projectIds != null && projectIds.Count > 0)
                                                {
                                                    //Aggiungo il Documento al fascicolo
                                                    tempProblems = AddDocToProjects(infoUtenteProprietario, schedaDoc.systemId, projectIds, false);
                                                }

                                                // Aggiunta dei problemi alla lista dei problemi
                                                if (tempProblems.Count > 0)
                                                {
                                                    item.esito = "W";
                                                    foreach (string erroriFascicolazione in tempProblems)
                                                    {
                                                        item.errore = concatenaMessaggio(item, erroriFascicolazione);
                                                    }
                                                    //Aggiorno item di report pregressi
                                                    importazione.UpdateItemReportPregressi(item, item.systemId);
                                                }
                                            }

                                            #endregion
                                            //END ANDREA

                                        }
                                    }
                                    else
                                    {
                                        item.esito = "E";
                                        foreach (string err in errori)
                                        {
                                            item.errore += err + " - ";
                                        }
                                        //In caso di errore update dell'item con errore
                                        importazione.UpdateItemReportPregressi(item, item.systemId);
                                    }
                                    //schedaDoc = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtenteProprietario, role);
                                }
                                #endregion

                                #region MODIFICA
                                if (item.tipoOperazione.ToUpper().Equals("M"))
                                {
                                    if (!string.IsNullOrEmpty(item.idNumProtocolloExcel))
                                    {
                                        bool daAggiornareUffRef = false;
                                        DocsPaVO.utente.InfoUtente infoUtenteProprietario = infoUtente;

                                        //IN ATTESA: il parametro item.idNumProtocolloExcel è l'idvecchiodocumento che per ora non sappiamo dove sia (Probabilmente nella PROFILE)
                                        DocsPaVO.documento.SchedaDocumento schedaDocModificato = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurityByIDVecchioDoc(infoUtente, item.idNumProtocolloExcel);

                                        if (schedaDocModificato != null)
                                        {
                                            //Prendo l'utente e ruolo proprietario se diversi da null
                                            if (!string.IsNullOrEmpty(item.idUtente) && !string.IsNullOrEmpty(item.idRuolo))
                                            {
                                                user = GetUtente(item, ref utenti);
                                                role = GetRuolo(item, ref ruoli);
                                                //non so se serve
                                                infoUtenteProprietario = ImpostaInfoUtente(user, role);
                                            }

                                            //Se presente modifico l'oggetto
                                            if (!string.IsNullOrEmpty(item.oggetto) || !string.IsNullOrEmpty(item.cod_oggetto))
                                            {
                                                oggetto = item.oggetto;
                                                oggetto = GetOggetto(item, null, infoUtente, ref errori, ref inserisci, ref oggettario);
                                                schedaDocModificato.oggetto.daAggiornare = true;
                                                schedaDocModificato.oggetto.descrizione = oggetto;
                                            }

                                            schedaDocModificato.template = GetTemplate(item, infoUtenteProprietario, user, role, null, null, ref errori, ref inserisci, ref tipiDocumento, DictionaryIdRuoli);                                            

                                            if (inserisci)
                                            {
                                                schedaDocModificato = BusinessLogic.Documenti.DocSave.save(infoUtente, schedaDocModificato, false, out daAggiornareUffRef, role);
                                                if (schedaDocModificato != null)
                                                {
                                                    //Caso di successo
                                                    item.esito = "S";
                                                    item.idDocumento = schedaDocModificato.docNumber;
                                                    importazione.UpdateItemReportPregressi(item, item.systemId);

                                                    //ANDREA - Occorre inserire il documento all'interno del fascicolo
                                                    #region FASCICOLAZIONE

                                                    string administrationSyd = string.Empty;
                                                    //Popolo l'id dell'amministrazione a partire dall'infoUtenteProprietario
                                                    administrationSyd = infoUtenteProprietario.idAmministrazione;

                                                    //Dictionary per non calcolare ogni volta il Registro del Fascicolo
                                                    //Dictionary<string, Registro> DictionaryCodRegFasc_RegFasc = new Dictionary<string, Registro>();
                                                    Registro registro_Fascicolo = null;

                                                    if (!string.IsNullOrEmpty(item.codiceRegistroFascicolo))
                                                    {
                                                        if (DictionaryCodRegFasc_RegFasc.ContainsKey(item.codiceRegistroFascicolo))
                                                        {
                                                            registro_Fascicolo = DictionaryCodRegFasc_RegFasc[item.codiceRegistroFascicolo];
                                                        }
                                                        else
                                                        {
                                                            //Calcolo il Registro del facicolo a partire dal codice del registro
                                                            registro_Fascicolo = RegistriManager.getRegistroByCode(item.codiceRegistroFascicolo);
                                                            if (registro_Fascicolo != null)
                                                                DictionaryCodRegFasc_RegFasc.Add(item.codiceRegistroFascicolo, registro_Fascicolo);
                                                        }
                                                    }
                                                    //End Dictionary Registro Facsicolo

                                                    string idTitolario = string.Empty;
                                                    //item.Titolario è stato cablato in CheckFileData in modo da prendere il titolario attivo

                                                    /*
                                                    //
                                                    //Se nel Projectcode c'è # allora cerco il titolario, altrimenti prendo il titolario attivo
                                                    if (item.ProjectCodes[0].Contains("#"))
                                                        item.Titolario = "titolario non attivo";
                                                    else
                                                        item.Titolario = string.Empty;
                                                    //
                                                    //End impostazione del titolario
                                                    */

                                                    //
                                                    //Gestione per il recupero dell'IDTitolario (Attivo oppure Storicizzato):
                                                    bool continuaFasc = true;
                                                    if (!string.IsNullOrEmpty(item.Titolario))
                                                    {
                                                        //
                                                        //Titolario non attivo - Storicizzato

                                                        //
                                                        //TO DO: Recupero IDTitolario
                                                        //cerchiamo il titolario su cui fascicolare a partire dal codice del facsicolo o dalla descrizione
                                                        if (registro_Fascicolo != null)
                                                        {
                                                            List<string> ListaidTitolario = new List<string>();

                                                            ListaidTitolario = BusinessLogic.Import.ImportUtils.getTitolarioByCodeFascicolo(item.ProjectCodes[0], item.ProjectDescription, administrationSyd, registro_Fascicolo.systemId);
                                                            if (ListaidTitolario.Count > 0)
                                                            {
                                                                //
                                                                //Ho trovato almeno un IDTitolario per quel CodiceFascicolo
                                                                if (ListaidTitolario.Count > 1)
                                                                {
                                                                    //
                                                                    //C'è più di un IDTitolario, quindi non è possibile individuare un Titolario univoco in cui reperire quel fascicolo e non è possibile fascicolare.
                                                                    //Aggiorno l'esito della riga del foglio excel che si vuole importare.
                                                                    idTitolario = string.Empty;
                                                                    item.esito = "W";
                                                                    string messErr = "il fascicolo è presente in più di un titolario.";
                                                                    item.errore = concatenaMessaggio(item, messErr);
                                                                    importazione.UpdateItemReportPregressi(item, item.systemId);
                                                                    continuaFasc = false;
                                                                }
                                                                else
                                                                {
                                                                    //
                                                                    //Ho trovato un solo titolario su cui fascicolare
                                                                    idTitolario = ListaidTitolario[0];
                                                                }
                                                            }
                                                            else
                                                            {
                                                                //Nessun Id Titolario Trovato
                                                                idTitolario = string.Empty;
                                                                item.esito = "W";
                                                                string messErr = "Nessun titolario trovato per il fascicolo " + item.codiceRegistroFascicolo;
                                                                item.errore = concatenaMessaggio(item, messErr);
                                                                importazione.UpdateItemReportPregressi(item, item.systemId);
                                                                continuaFasc = false;
                                                            }
                                                            //
                                                            //End recupero IDTitolario
                                                        }
                                                        else
                                                        {
                                                            item.esito = "W";
                                                            string messErr = "Registro associato al fascicolo mancante.";
                                                            item.errore = concatenaMessaggio(item, messErr);
                                                            importazione.UpdateItemReportPregressi(item, item.systemId);
                                                            continuaFasc = false;
                                                        }
                                                        //try
                                                        //{
                                                        //    //Recupero del titolario Attivo a partire dall'amministrazione e dal nome del titolario
                                                        //    idTitolario = BusinessLogic.Import.ImportUtils.GetTitolarioId(item.Titolario, administrationSyd);
                                                        //}
                                                        //catch (Exception e)
                                                        //{
                                                        //}
                                                    }
                                                    else
                                                    {
                                                        //
                                                        //Recupero Titolario Attivo
                                                        try
                                                        {
                                                            string codAmministrazione = string.Empty;
                                                            idTitolario = BusinessLogic.Amministrazione.TitolarioManager.getTitolarioAttivo(administrationSyd, out codAmministrazione);
                                                        }
                                                        catch (Exception e)
                                                        {
                                                        }
                                                    }

                                                    if (continuaFasc)
                                                    {
                                                        List<string> tempProblems = new List<string>();
                                                        List<string> projectIds = new List<string>();

                                                        ////Dictionary per non calcolare ogni volta il Registro del Fascicolo
                                                        ////Dictionary<string, Registro> DictionaryCodRegFasc_RegFasc = new Dictionary<string, Registro>();
                                                        //Registro registro_Fascicolo = null;

                                                        //if (!string.IsNullOrEmpty(item.codiceRegistroFascicolo))
                                                        //{
                                                        //    if (DictionaryCodRegFasc_RegFasc.ContainsKey(item.codiceRegistroFascicolo))
                                                        //    {
                                                        //        registro_Fascicolo = DictionaryCodRegFasc_RegFasc[item.codiceRegistroFascicolo];
                                                        //    }
                                                        //    else
                                                        //    {
                                                        //        //Calcolo il Registro del facicolo a partire dal codice del registro
                                                        //        registro_Fascicolo = RegistriManager.getRegistroByCode(item.codiceRegistroFascicolo);
                                                        //        if (registro_Fascicolo != null)
                                                        //            DictionaryCodRegFasc_RegFasc.Add(item.codiceRegistroFascicolo, registro_Fascicolo);
                                                        //    }
                                                        //}
                                                        ////End Dictionary Registro Facsicolo

                                                        //Prendo il registro relativo al codice del registro fascicolo
                                                        //Registro registroFascicolo = RegistriManager.getRegistroByCode(item.codiceRegistroFascicolo);

                                                        // Reperimento degli identificativi dei fascicoli se almeno uno fra i seguenti campi risulta valorizzato
                                                        // ProjectCodes, ProjectDescription, FolderDescription, NodeCode
                                                        if ((item.ProjectCodes != null && item.ProjectCodes.Length > 0) ||
                                                            !String.IsNullOrEmpty(item.ProjectDescription) ||
                                                            !String.IsNullOrEmpty(item.FolderDescrition) ||
                                                            !String.IsNullOrEmpty(item.NodeCode))
                                                        {
                                                            // La lista dei fascicoli in cui inserire il documento
                                                            //projectIds = GetProjectsForClassification(item, infoUtenteProprietario, role, administrationSyd, register.systemId, rf, idTitolario, out tempProblems, false);
                                                            //projectIds = GetProjectsForClassification(item, infoUtenteProprietario, role, administrationSyd, registroFascicolo.systemId, null, idTitolario, out tempProblems, false);
                                                            if (registro_Fascicolo != null)
                                                            {
                                                                projectIds = GetProjectsForClassification(item, infoUtenteProprietario, role, administrationSyd, registro_Fascicolo.systemId, null, idTitolario, out tempProblems, false, ref DictionaryCodFasc_Fasc);
                                                            }
                                                            else
                                                            {
                                                                tempProblems.Add("Registro associato al fascicolo mancante.");
                                                            }
                                                        }

                                                        if (projectIds != null && projectIds.Count > 0)
                                                        {
                                                            List<string> projID = new List<string>();
                                                            foreach (string idProj in projectIds)
                                                            {
                                                                bool isFascicolato = BusinessLogic.Documenti.DocManager.GetSeDocFascicolato(schedaDocModificato.systemId, idProj);
                                                                if (!isFascicolato)
                                                                {
                                                                    //creo una lista di stringhe da passare alla fascicolazione
                                                                    projID.Add(idProj);
                                                                }
                                                            }

                                                            if (projID != null && projID.Count > 0)
                                                            {
                                                                //Aggiungo il Documento al fascicolo
                                                                tempProblems = AddDocToProjects(infoUtenteProprietario, schedaDocModificato.systemId, projectIds, false);
                                                            }
                                                        }

                                                        // Aggiunta dei problemi alla lista dei problemi
                                                        if (tempProblems.Count > 0)
                                                        {
                                                            item.esito = "W";
                                                            foreach (string erroriFascicolazione in tempProblems)
                                                            {
                                                                item.errore = concatenaMessaggio(item, erroriFascicolazione);
                                                            }
                                                            //Aggiorno item di report pregressi
                                                            importazione.UpdateItemReportPregressi(item, item.systemId);
                                                        }
                                                    }

                                                    #endregion
                                                    //END ANDREA
                                                }
                                                else
                                                {
                                                    item.esito = "E";
                                                    errori.Add("Errore generico nella modifica del documento");
                                                    importazione.UpdateItemReportPregressi(item, item.systemId);
                                                }
                                            }
                                            else
                                            {
                                                item.esito = "E";
                                                foreach (string err in errori)
                                                {
                                                    item.errore += err + " - ";
                                                }
                                                importazione.UpdateItemReportPregressi(item, item.systemId);
                                            }
                                        }
                                        else
                                        {
                                            item.esito = "E";
                                            item.errore = "Documento non trovato";
                                            importazione.UpdateItemReportPregressi(item, item.systemId);
                                        }
                                    }
                                    else
                                    {
                                        item.esito = "E";
                                        errori.Add("id vecchio documento mancante");
                                        importazione.UpdateItemReportPregressi(item, item.systemId);
                                    }
                                }
                                #endregion

                                #region CANCELLA
                                if (item.tipoOperazione.ToUpper().Equals("C"))
                                {
                                    if (!string.IsNullOrEmpty(item.idNumProtocolloExcel))
                                    {
                                        DocsPaVO.utente.InfoUtente infoUtenteProprietario = infoUtente;

                                        //IN ATTESA: il parametro item.idNumProtocolloExcel è l'idvecchiodocumento che per ora non sappiamo dove sia (Probabilmente nella PROFILE)
                                        DocsPaVO.documento.SchedaDocumento schedaDocModificato = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurityByIDVecchioDoc(infoUtente, item.idNumProtocolloExcel);

                                        if (schedaDocModificato != null)
                                        {
                                            //Prendo l'utente e ruolo proprietario se diversi da null
                                            if (!string.IsNullOrEmpty(item.idUtente) && !string.IsNullOrEmpty(item.idRuolo))
                                            {
                                                user = GetUtente(item, ref utenti);
                                                role = GetRuolo(item, ref ruoli);
                                                //non so se serve
                                                infoUtenteProprietario = ImpostaInfoUtente(user, role);
                                            }

                                            if (inserisci)
                                            {
                                                bool success = BusinessLogic.Documenti.ProtoManager.DeleteProtocollo(infoUtente, schedaDocModificato);
                                                if (success)
                                                {
                                                    //Caso di successo
                                                    item.esito = "S";
                                                    item.idDocumento = schedaDocModificato.docNumber;
                                                    importazione.UpdateItemReportPregressi(item, item.systemId);
                                                }
                                                else
                                                {
                                                    item.esito = "E";
                                                    errori.Add("Errore generico nella cancellazione del documento");
                                                    importazione.UpdateItemReportPregressi(item, item.systemId);
                                                }
                                            }
                                            else
                                            {
                                                item.esito = "E";
                                                foreach (string err in errori)
                                                {
                                                    item.errore += err + " - ";
                                                }
                                                importazione.UpdateItemReportPregressi(item, item.systemId);
                                            }
                                        }
                                        else
                                        {
                                            item.esito = "E";
                                            item.errore = "Documento non trovato";
                                            importazione.UpdateItemReportPregressi(item, item.systemId);
                                        }
                                    }
                                    else
                                    {
                                        item.esito = "E";
                                        item.errore = "Id vecchio documento mancante";
                                        importazione.UpdateItemReportPregressi(item, item.systemId);
                                    }
                                }
                                #endregion

                            }
                            else
                            {
                                #region INSERIMENTO
                                if (item.tipoOperazione.ToUpper().Equals("I"))
                                {
                                    DocsPaVO.utente.InfoUtente infoUtenteProprietario = null;
                                    if (!string.IsNullOrEmpty(item.idUtente) && !string.IsNullOrEmpty(item.idRuolo) && (string.IsNullOrEmpty(infoUtente.idPeople) || string.IsNullOrEmpty(infoUtente.idGruppo) || !infoUtente.idPeople.Equals(item.idUtente) || !infoUtente.idGruppo.Equals(item.idRuolo)))
                                    {
                                        //Prendo l'utente
                                        user = GetUtente(item, ref utenti);
                                        //Prendo il ruolo
                                        role = GetRuolo(item, ref ruoli);
                                        infoUtenteProprietario = ImpostaInfoUtente(user, role);
                                    }
                                    else
                                    {
                                        infoUtenteProprietario = infoUtente;
                                        //Prendo l'utente
                                        ItemReportPregressi tempUs = new ItemReportPregressi();
                                        tempUs.idUtente = infoUtenteProprietario.idPeople;
                                        tempUs.idRuolo = infoUtenteProprietario.idGruppo;
                                        user = GetUtente(tempUs, ref utenti);
                                        //Prendo il ruolo
                                        role = GetRuolo(tempUs, ref ruoli);
                                    }
                                    //Prendo il registro
                                    register = GetRegistro(item, ref registri);
                                    //Se presente prendo l'rf
                                    rf = GetRF(item, infoUtente, ref listaRf, ref errori, ref inserisci, role);
                                    //Controllo sul tipo di protocollo
                                    ControlloTipoProtocollo(item, ref errori, ref inserisci);
                                    //Controllo e se presente prendo l'oggetto
                                    oggetto = GetOggetto(item, register, infoUtente, ref errori, ref inserisci, ref oggettario);
                                    //Utente che sarà il proprietario del documento

                                    //Controllo che id_ruolo_creatore sia associato al registro di pregresso
                                    if (role != null && register != null)
                                    {
                                        CheckIdRuoloCreatoreAssociatoRegistroPregresso(role, register, ref inserisci, ref errori, ref DictionaryIdRuoli);
                                    }
                                    //Creo documento
                                    DocsPaVO.documento.SchedaDocumento schedaDoc = DocManager.NewSchedaDocumento(infoUtenteProprietario);

                                    if (item.tipoProtocollo.ToUpper().Equals("A"))
                                    {
                                        schedaDoc.tipoProto = "A";
                                        schedaDoc.protocollo = new ProtocolloEntrata();
                                    }
                                    else
                                    {
                                        if (item.tipoProtocollo.ToUpper().Equals("P"))
                                        {
                                            schedaDoc.tipoProto = "P";
                                            schedaDoc.protocollo = new ProtocolloUscita();
                                        }
                                        else
                                        {
                                            schedaDoc.tipoProto = "I";
                                            schedaDoc.protocollo = new ProtocolloInterno();
                                        }
                                    }

                                    ControlloCorrispondenti(item, infoUtenteProprietario, register, rf, false, ref errori, ref inserisci, ref schedaDoc, ref corrispondenti, DictionaryIdRuoli, role);

                                    template = GetTemplate(item, infoUtenteProprietario, user, role, register, rf, ref errori, ref inserisci, ref tipiDocumento, DictionaryIdRuoli);

                                    if (inserisci)
                                    {

                                        ResultProtocollazione risultatoProtocollazione = new ResultProtocollazione();

                                        risultatoProtocollazione = CreaDocumento(item, register, rf, oggetto, infoUtenteProprietario, template, role, ref schedaDoc, ref esitoAcqFile);

                                        if (schedaDoc == null || schedaDoc.protocollo == null || string.IsNullOrEmpty(schedaDoc.protocollo.segnatura))
                                        {
                                            item.esito = "E";
                                            errori.Add("Errore nella creazione del documento");
                                            foreach (string err in errori)
                                            {
                                                item.errore += err + " - ";
                                            }
                                            importazione.UpdateItemReportPregressi(item, item.systemId);
                                        }
                                        else
                                        {
                                            //Caso di successo
                                            item.esito = "S";
                                            item.idDocumento = schedaDoc.docNumber;
                                            importazione.UpdateItemReportPregressi(item, item.systemId);

                                            bool esitoAllegati = true;
                                            int allegati = 0;

                                            if (item.Allegati != null && item.Allegati.Count > 0)
                                            {
                                                schedaDoc.allegati = new ArrayList();

                                                foreach (Allegati allTemp in item.Allegati)
                                                {
                                                    try
                                                    {
                                                        Allegato allegato = CreaAllegato(allTemp, ref schedaDoc, infoUtenteProprietario);
                                                        schedaDoc.allegati.Add(allegato);
                                                        allTemp.esito = "S";
                                                        importazione.UpdateAllegatoItemReportPregressi(allTemp, item.systemId);
                                                    }
                                                    catch
                                                    {
                                                        esitoAllegati = false;
                                                        //errori.Add("Errore nella creazione dell'allegato");
                                                        allTemp.esito = "E";
                                                        importazione.UpdateAllegatoItemReportPregressi(allTemp, item.systemId);
                                                        allegati++;
                                                    }
                                                }


                                            }

                                            ControlloADL(item, infoUtenteProprietario, role, schedaDoc);

                                            ControlloTrasmissioni(item, infoUtenteProprietario, infoUtente, role, schedaDoc, importazione, ref errori, ref esitoTrasmissioni, ref DictionaryIdRuoli);

                                            if (!esitoAllegati)
                                            {
                                                item.esito = "W";
                                                string totaleErrore = string.Empty;
                                                if (allegati > 1)
                                                {
                                                    totaleErrore = "Errore nell'acquisizione del file di" + allegati + "allegati";
                                                }
                                                else
                                                {
                                                    totaleErrore = "Errore nell'acquisizione del file di un allegato";
                                                }

                                                item.errore = concatenaMessaggio(item, totaleErrore);
                                                //importazione.UpdateItemReportPregressi(item, item.systemId);
                                            }

                                            if (!esitoAllegati || !esitoTrasmissioni || !esitoAcqFile)
                                            {
                                                item.esito = "W";
                                                importazione.UpdateItemReportPregressi(item, item.systemId);
                                            }

                                            //ANDREA - Occorre inserire il documento all'interno del fascicolo
                                            #region FASCICOLAZIONE

                                            string administrationSyd = string.Empty;
                                            //Popolo l'id dell'amministrazione a partire dall'infoUtenteProprietario
                                            administrationSyd = infoUtenteProprietario.idAmministrazione;

                                            //Prendo il registro relativo al codice del registro fascicolo
                                            //Registro registroFascicolo = RegistriManager.getRegistroByCode(item.codiceRegistroFascicolo);

                                            //Dictionary per non calcolare ogni volta il Registro del Fascicolo
                                            //Dictionary<string, Registro> DictionaryCodRegFasc_RegFasc = new Dictionary<string, Registro>();
                                            Registro registro_Fascicolo = null;

                                            if (!string.IsNullOrEmpty(item.codiceRegistroFascicolo))
                                            {
                                                if (DictionaryCodRegFasc_RegFasc.ContainsKey(item.codiceRegistroFascicolo))
                                                {
                                                    registro_Fascicolo = DictionaryCodRegFasc_RegFasc[item.codiceRegistroFascicolo];
                                                }
                                                else
                                                {
                                                    //Calcolo il Registro del facicolo a partire dal codice del registro
                                                    registro_Fascicolo = RegistriManager.getRegistroByCode(item.codiceRegistroFascicolo);
                                                    if (registro_Fascicolo != null)
                                                        DictionaryCodRegFasc_RegFasc.Add(item.codiceRegistroFascicolo, registro_Fascicolo);
                                                }
                                            }
                                            //End Dictionary Registro Facsicolo

                                            //item.Titolario è stato cablato in CheckFileData in modo da prendere il titolario attivo
                                            string idTitolario = string.Empty;

                                            /*
                                            //
                                            //Se nel Projectcode c'è # allora cerco il titolario, altrimenti prendo il titolario attivo
                                            if (item.ProjectCodes[0].Contains("#"))
                                                item.Titolario = "titolario non attivo";
                                            else
                                                item.Titolario = string.Empty;
                                            //
                                            //End impostazione del titolario
                                            */

                                            //
                                            //Gestione per il recupero dell'IDTitolario (Attivo oppure Storicizzato):
                                            bool continuaFasc = true;
                                            if (!string.IsNullOrEmpty(item.Titolario))
                                            {
                                                //
                                                //Titolario non attivo - Storicizzato

                                                //
                                                //TO DO: Recupero IDTitolario
                                                //cerchiamo il titolario su cui fascicolare a partire dal codice del facsicolo o dalla descrizione
                                                if (registro_Fascicolo != null)
                                                {
                                                    List<string> ListaidTitolario = new List<string>();

                                                    ListaidTitolario = BusinessLogic.Import.ImportUtils.getTitolarioByCodeFascicolo(item.ProjectCodes[0], item.ProjectDescription, administrationSyd, registro_Fascicolo.systemId);
                                                    if (ListaidTitolario.Count > 0)
                                                    {
                                                        //
                                                        //Ho trovato almeno un IDTitolario per quel CodiceFascicolo
                                                        if (ListaidTitolario.Count > 1)
                                                        {
                                                            //
                                                            //C'è più di un IDTitolario, quindi non è possibile individuare un Titolario univoco in cui reperire quel fascicolo e non è possibile fascicolare.
                                                            //Aggiorno l'esito della riga del foglio excel che si vuole importare.
                                                            idTitolario = string.Empty;
                                                            item.esito = "W";
                                                            string messErr = "il fascicolo è presente in più di un titolario.";
                                                            item.errore = concatenaMessaggio(item, messErr);
                                                            importazione.UpdateItemReportPregressi(item, item.systemId);
                                                            continuaFasc = false;
                                                        }
                                                        else
                                                        {
                                                            //
                                                            //Ho trovato un solo titolario su cui fascicolare
                                                            idTitolario = ListaidTitolario[0];
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //Nessun Id Titolario Trovato
                                                        idTitolario = string.Empty;
                                                        item.esito = "W";
                                                        string messErr = "Nessun titolario trovato per il fascicolo " + item.codiceRegistroFascicolo;
                                                        item.errore = concatenaMessaggio(item, messErr);
                                                        importazione.UpdateItemReportPregressi(item, item.systemId);
                                                        continuaFasc = false;
                                                    }
                                                    //
                                                    //End recupero IDTitolario
                                                }
                                                else
                                                {
                                                    item.esito = "W";
                                                    string messErr = "Registro associato al fascicolo mancante.";
                                                    item.errore = concatenaMessaggio(item, messErr);
                                                    importazione.UpdateItemReportPregressi(item, item.systemId);
                                                    continuaFasc = false;
                                                }

                                                //try
                                                //{
                                                //    //Recupero del titolario Attivo a partire dall'amministrazione e dal nome del titolario
                                                //    idTitolario = BusinessLogic.Import.ImportUtils.GetTitolarioId(item.Titolario, administrationSyd);
                                                //}
                                                //catch (Exception e)
                                                //{
                                                //}
                                            }
                                            else
                                            {
                                                //
                                                //Recupero Titolario Attivo
                                                try
                                                {
                                                    string codAmministrazione = string.Empty;
                                                    idTitolario = BusinessLogic.Amministrazione.TitolarioManager.getTitolarioAttivo(administrationSyd, out codAmministrazione);
                                                }
                                                catch (Exception e)
                                                {
                                                }
                                            }

                                            if (continuaFasc)
                                            {
                                                List<string> tempProblems = new List<string>();
                                                List<string> projectIds = new List<string>();

                                                ////Prendo il registro relativo al codice del registro fascicolo
                                                ////Registro registroFascicolo = RegistriManager.getRegistroByCode(item.codiceRegistroFascicolo);

                                                ////Dictionary per non calcolare ogni volta il Registro del Fascicolo
                                                ////Dictionary<string, Registro> DictionaryCodRegFasc_RegFasc = new Dictionary<string, Registro>();
                                                //Registro registro_Fascicolo = null;

                                                //if (!string.IsNullOrEmpty(item.codiceRegistroFascicolo))
                                                //{
                                                //    if (DictionaryCodRegFasc_RegFasc.ContainsKey(item.codiceRegistroFascicolo))
                                                //    {
                                                //        registro_Fascicolo = DictionaryCodRegFasc_RegFasc[item.codiceRegistroFascicolo];
                                                //    }
                                                //    else
                                                //    {
                                                //        //Calcolo il Registro del facicolo a partire dal codice del registro
                                                //        registro_Fascicolo = RegistriManager.getRegistroByCode(item.codiceRegistroFascicolo);
                                                //        if (registro_Fascicolo != null)
                                                //            DictionaryCodRegFasc_RegFasc.Add(item.codiceRegistroFascicolo, registro_Fascicolo);
                                                //    }
                                                //}
                                                ////End Dictionary Registro Facsicolo

                                                // Reperimento degli identificativi dei fascicoli se almeno uno fra i seguenti campi risulta valorizzato
                                                // ProjectCodes, ProjectDescription, FolderDescription, NodeCode
                                                if ((item.ProjectCodes != null && item.ProjectCodes.Length > 0) ||
                                                    !String.IsNullOrEmpty(item.ProjectDescription) ||
                                                    !String.IsNullOrEmpty(item.FolderDescrition) ||
                                                    !String.IsNullOrEmpty(item.NodeCode))
                                                {
                                                    // La lista dei fascicoli in cui inserire il documento
                                                    //projectIds = GetProjectsForClassification(item, infoUtenteProprietario, role, administrationSyd, register.systemId, rf, idTitolario, out tempProblems, false);
                                                    //projectIds = GetProjectsForClassification(item, infoUtenteProprietario, role, administrationSyd, register.systemId, null, idTitolario, out tempProblems, false);
                                                    //projectIds = GetProjectsForClassification(item, infoUtenteProprietario, role, administrationSyd, registroFascicolo.systemId, null, idTitolario, out tempProblems, false);
                                                    if (registro_Fascicolo != null)
                                                    {
                                                        projectIds = GetProjectsForClassification(item, infoUtenteProprietario, role, administrationSyd, registro_Fascicolo.systemId, null, idTitolario, out tempProblems, false, ref DictionaryCodFasc_Fasc);
                                                    }
                                                    else
                                                    {
                                                        tempProblems.Add("Registro associato al fascicolo mancante.");
                                                    }
                                                }

                                                if (projectIds != null && projectIds.Count > 0)
                                                {
                                                    //Aggiungo il Documento al fascicolo
                                                    tempProblems = AddDocToProjects(infoUtenteProprietario, schedaDoc.systemId, projectIds, false);
                                                }

                                                // Aggiunta dei problemi alla lista dei problemi
                                                if (tempProblems.Count > 0)
                                                {
                                                    item.esito = "W";
                                                    foreach (string erroriFascicolazione in tempProblems)
                                                    {
                                                        item.errore = concatenaMessaggio(item, erroriFascicolazione);
                                                    }
                                                    //Aggiorno item di report pregressi
                                                    importazione.UpdateItemReportPregressi(item, item.systemId);
                                                }
                                            }
                                            #endregion
                                            //END ANDREA

                                        }
                                    }
                                    else
                                    {
                                        item.esito = "E";
                                        foreach (string err in errori)
                                        {
                                            item.errore += err + " - ";
                                        }
                                        //In caso di errore update dell'item con errore
                                        importazione.UpdateItemReportPregressi(item, item.systemId);
                                    }
                                }

                                #endregion

                                #region MODIFICA
                                if (item.tipoOperazione.ToUpper().Equals("M"))
                                {
                                    if (!string.IsNullOrEmpty(item.idNumProtocolloExcel) && !string.IsNullOrEmpty(item.idRegistro))
                                    {
                                        bool daAggiornareUffRef = false;
                                        DocsPaVO.utente.InfoUtente infoUtenteProprietario = infoUtente;

                                        DocsPaVO.documento.SchedaDocumento schedaDocModificato = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurityByNumProtoEIDRegistro(infoUtente, item.idNumProtocolloExcel, item.idRegistro);
                                        if (schedaDocModificato != null)
                                        {
                                            //Prendo l'utente e ruolo proprietario se diversi da null
                                            if (!string.IsNullOrEmpty(item.idUtente) && !string.IsNullOrEmpty(item.idRuolo))
                                            {
                                                user = GetUtente(item, ref utenti);
                                                role = GetRuolo(item, ref ruoli);
                                            }

                                            //Prendo il registro
                                            if (!string.IsNullOrEmpty(item.idRegistro))
                                            {
                                                register = GetRegistro(item, ref registri);
                                            }

                                            //Prendo il registro
                                            if (!string.IsNullOrEmpty(item.cod_rf))
                                            {
                                                //Se presente prendo l'rf
                                                rf = GetRF(item, infoUtente, ref listaRf, ref errori, ref inserisci, role);
                                            }

                                            //Controllo che id_ruolo_creatore sia associato al registro di pregresso
                                            if (role != null && register != null)
                                            {
                                                CheckIdRuoloCreatoreAssociatoRegistroPregresso(role, register, ref inserisci, ref errori, ref DictionaryIdRuoli);
                                            }

                                            //Se presente modifico l'oggetto
                                            if (!string.IsNullOrEmpty(item.oggetto) || !string.IsNullOrEmpty(item.cod_oggetto))
                                            {
                                                oggetto = item.oggetto;
                                                oggetto = GetOggetto(item, register, infoUtente, ref errori, ref inserisci, ref oggettario);
                                                schedaDocModificato.oggetto.daAggiornare = true;
                                                schedaDocModificato.oggetto.descrizione = oggetto;
                                            }

                                            //Se presenti modifico i mittenti/destinatari
                                            if (!string.IsNullOrEmpty(item.corrispondenti) || !string.IsNullOrEmpty(item.cod_corrispondenti))
                                            {
                                                ControlloCorrispondenti(item, infoUtenteProprietario, register, rf, true, ref errori, ref inserisci, ref schedaDocModificato, ref corrispondenti, DictionaryIdRuoli, role);
                                            }

                                            if (!string.IsNullOrEmpty(item.tipo_documento))
                                            {
                                                schedaDocModificato.template = GetTemplate(item, infoUtenteProprietario, user, role, register, rf, ref errori, ref inserisci, ref tipiDocumento, DictionaryIdRuoli);
                                            }

                                            if (inserisci)
                                            {
                                                schedaDocModificato = BusinessLogic.Documenti.DocSave.save(infoUtente, schedaDocModificato, false, out daAggiornareUffRef, role);
                                                if (schedaDocModificato != null)
                                                {
                                                    //Caso di successo
                                                    item.esito = "S";
                                                    item.idDocumento = schedaDocModificato.docNumber;
                                                    importazione.UpdateItemReportPregressi(item, item.systemId);

                                                    //ANDREA - Occorre inserire il documento all'interno del fascicolo
                                                    #region FASCICOLAZIONE

                                                    string administrationSyd = string.Empty;
                                                    //Popolo l'id dell'amministrazione a partire dall'infoUtenteProprietario
                                                    administrationSyd = infoUtenteProprietario.idAmministrazione;

                                                    //Dictionary per non calcolare ogni volta il Registro del Fascicolo
                                                    //Dictionary<string, Registro> DictionaryCodRegFasc_RegFasc = new Dictionary<string, Registro>();
                                                    Registro registro_Fascicolo = null;

                                                    if (!string.IsNullOrEmpty(item.codiceRegistroFascicolo))
                                                    {
                                                        if (DictionaryCodRegFasc_RegFasc.ContainsKey(item.codiceRegistroFascicolo))
                                                        {
                                                            registro_Fascicolo = DictionaryCodRegFasc_RegFasc[item.codiceRegistroFascicolo];
                                                        }
                                                        else
                                                        {
                                                            //Calcolo il Registro del facicolo a partire dal codice del registro
                                                            registro_Fascicolo = RegistriManager.getRegistroByCode(item.codiceRegistroFascicolo);
                                                            if (registro_Fascicolo != null)
                                                                DictionaryCodRegFasc_RegFasc.Add(item.codiceRegistroFascicolo, registro_Fascicolo);
                                                        }
                                                    }
                                                    //End Dictionary Registro Facsicolo

                                                    string idTitolario = string.Empty;
                                                    //item.Titolario è stato cablato in CheckFileData in modo da prendere il titolario attivo

                                                    /*
                                                    //
                                                    //Se nel Projectcode c'è # allora cerco il titolario, altrimenti prendo il titolario attivo
                                                    if (item.ProjectCodes[0].Contains("#"))
                                                        item.Titolario = "titolario non attivo";
                                                    else
                                                        item.Titolario = string.Empty;
                                                    //
                                                    //End impostazione del titolario
                                                    */

                                                    //
                                                    //Gestione per il recupero dell'IDTitolario (Attivo oppure Storicizzato):
                                                    bool continuaFasc = true;
                                                    if (!string.IsNullOrEmpty(item.Titolario))
                                                    {
                                                        //
                                                        //Titolario non attivo - Storicizzato

                                                        //
                                                        //TO DO: Recupero IDTitolario
                                                        //cerchiamo il titolario su cui fascicolare a partire dal codice del facsicolo o dalla descrizione
                                                        if (registro_Fascicolo != null)
                                                        {
                                                            List<string> ListaidTitolario = new List<string>();

                                                            ListaidTitolario = BusinessLogic.Import.ImportUtils.getTitolarioByCodeFascicolo(item.ProjectCodes[0], item.ProjectDescription, administrationSyd, registro_Fascicolo.systemId);
                                                            if (ListaidTitolario.Count > 0)
                                                            {
                                                                //
                                                                //Ho trovato almeno un IDTitolario per quel CodiceFascicolo
                                                                if (ListaidTitolario.Count > 1)
                                                                {
                                                                    //
                                                                    //C'è più di un IDTitolario, quindi non è possibile individuare un Titolario univoco in cui reperire quel fascicolo e non è possibile fascicolare.
                                                                    //Aggiorno l'esito della riga del foglio excel che si vuole importare.
                                                                    idTitolario = string.Empty;
                                                                    item.esito = "W";
                                                                    string messErr = "il fascicolo è presente in più di un titolario.";
                                                                    item.errore = concatenaMessaggio(item, messErr);
                                                                    importazione.UpdateItemReportPregressi(item, item.systemId);
                                                                    continuaFasc = false;
                                                                }
                                                                else
                                                                {
                                                                    //
                                                                    //Ho trovato un solo titolario su cui fascicolare
                                                                    idTitolario = ListaidTitolario[0];
                                                                }
                                                            }
                                                            else
                                                            {
                                                                //Nessun Id Titolario Trovato
                                                                idTitolario = string.Empty;
                                                                item.esito = "W";
                                                                string messErr = "Nessun titolario trovato per il fascicolo " + item.codiceRegistroFascicolo;
                                                                item.errore = concatenaMessaggio(item, messErr);
                                                                importazione.UpdateItemReportPregressi(item, item.systemId);
                                                                continuaFasc = false;
                                                            }
                                                            //
                                                            //End recupero IDTitolario
                                                        }
                                                        else
                                                        {
                                                            item.esito = "W";
                                                            string messErr = "Registro associato al fascicolo mancante.";
                                                            item.errore = concatenaMessaggio(item, messErr);
                                                            importazione.UpdateItemReportPregressi(item, item.systemId);
                                                            continuaFasc = false;
                                                        }

                                                        //try
                                                        //{
                                                        //    //Recupero del titolario Attivo a partire dall'amministrazione e dal nome del titolario
                                                        //    idTitolario = BusinessLogic.Import.ImportUtils.GetTitolarioId(item.Titolario, administrationSyd);
                                                        //}
                                                        //catch (Exception e)
                                                        //{
                                                        //}
                                                    }
                                                    else
                                                    {
                                                        //
                                                        //Recupero Titolario Attivo
                                                        try
                                                        {
                                                            string codAmministrazione = string.Empty;
                                                            idTitolario = BusinessLogic.Amministrazione.TitolarioManager.getTitolarioAttivo(administrationSyd, out codAmministrazione);
                                                        }
                                                        catch (Exception e)
                                                        {
                                                        }
                                                    }

                                                    if (continuaFasc)
                                                    {
                                                        List<string> tempProblems = new List<string>();
                                                        List<string> projectIds = new List<string>();

                                                        ////Dictionary per non calcolare ogni volta il Registro del Fascicolo
                                                        ////Dictionary<string, Registro> DictionaryCodRegFasc_RegFasc = new Dictionary<string, Registro>();
                                                        //Registro registro_Fascicolo = null;

                                                        //if (!string.IsNullOrEmpty(item.codiceRegistroFascicolo))
                                                        //{
                                                        //    if (DictionaryCodRegFasc_RegFasc.ContainsKey(item.codiceRegistroFascicolo))
                                                        //    {
                                                        //        registro_Fascicolo = DictionaryCodRegFasc_RegFasc[item.codiceRegistroFascicolo];
                                                        //    }
                                                        //    else
                                                        //    {
                                                        //        //Calcolo il Registro del facicolo a partire dal codice del registro
                                                        //        registro_Fascicolo = RegistriManager.getRegistroByCode(item.codiceRegistroFascicolo);
                                                        //        if (registro_Fascicolo != null)
                                                        //            DictionaryCodRegFasc_RegFasc.Add(item.codiceRegistroFascicolo, registro_Fascicolo);
                                                        //    }
                                                        //}
                                                        ////End Dictionary Registro Facsicolo

                                                        //Prendo il registro relativo al codice del registro fascicolo
                                                        //Registro registroFascicolo = RegistriManager.getRegistroByCode(item.codiceRegistroFascicolo);

                                                        // Reperimento degli identificativi dei fascicoli se almeno uno fra i seguenti campi risulta valorizzato
                                                        // ProjectCodes, ProjectDescription, FolderDescription, NodeCode
                                                        if ((item.ProjectCodes != null && item.ProjectCodes.Length > 0) ||
                                                            !String.IsNullOrEmpty(item.ProjectDescription) ||
                                                            !String.IsNullOrEmpty(item.FolderDescrition) ||
                                                            !String.IsNullOrEmpty(item.NodeCode))
                                                        {
                                                            // La lista dei fascicoli in cui inserire il documento
                                                            //projectIds = GetProjectsForClassification(item, infoUtenteProprietario, role, administrationSyd, register.systemId, rf, idTitolario, out tempProblems, false);
                                                            //projectIds = GetProjectsForClassification(item, infoUtenteProprietario, role, administrationSyd, registroFascicolo.systemId, null, idTitolario, out tempProblems, false);
                                                            if (registro_Fascicolo != null)
                                                            {
                                                                projectIds = GetProjectsForClassification(item, infoUtenteProprietario, role, administrationSyd, registro_Fascicolo.systemId, null, idTitolario, out tempProblems, false, ref DictionaryCodFasc_Fasc);
                                                            }
                                                            else
                                                            {
                                                                tempProblems.Add("Registro associato al fascicolo mancante.");
                                                            }
                                                        }

                                                        if (projectIds != null && projectIds.Count > 0)
                                                        {
                                                            List<string> projID = new List<string>();
                                                            foreach (string idProj in projectIds)
                                                            {
                                                                bool isFascicolato = BusinessLogic.Documenti.DocManager.GetSeDocFascicolato(schedaDocModificato.systemId, idProj);
                                                                if (!isFascicolato)
                                                                {
                                                                    //creo una lista di stringhe da passare alla fascicolazione
                                                                    projID.Add(idProj);
                                                                }
                                                            }

                                                            if (projID != null && projID.Count > 0)
                                                            {
                                                                //Aggiungo il Documento al fascicolo
                                                                tempProblems = AddDocToProjects(infoUtenteProprietario, schedaDocModificato.systemId, projectIds, false);
                                                            }
                                                        }

                                                        // Aggiunta dei problemi alla lista dei problemi
                                                        if (tempProblems.Count > 0)
                                                        {
                                                            item.esito = "W";
                                                            foreach (string erroriFascicolazione in tempProblems)
                                                            {
                                                                item.errore = concatenaMessaggio(item, erroriFascicolazione);
                                                            }
                                                            //Aggiorno item di report pregressi
                                                            importazione.UpdateItemReportPregressi(item, item.systemId);
                                                        }
                                                    }
                                                    #endregion
                                                    //END ANDREA

                                                }
                                                else
                                                {
                                                    item.esito = "E";
                                                    errori.Add("Errore generico nella modifica del documento");
                                                    importazione.UpdateItemReportPregressi(item, item.systemId);
                                                }
                                            }
                                            else
                                            {
                                                item.esito = "E";
                                                foreach (string err in errori)
                                                {
                                                    item.errore += err + " - ";
                                                }
                                                importazione.UpdateItemReportPregressi(item, item.systemId);
                                            }
                                        }
                                        else
                                        {
                                            item.esito = "E";
                                            item.errore = "Documento non trovato";
                                            importazione.UpdateItemReportPregressi(item, item.systemId);
                                        }
                                    }
                                    else
                                    {
                                        item.esito = "E";
                                        errori.Add("Numero di protocollo o id registro mancanti");
                                        importazione.UpdateItemReportPregressi(item, item.systemId);
                                    }
                                }
                                #endregion

                                #region CANCELLA
                                if (item.tipoOperazione.ToUpper().Equals("C"))
                                {
                                    if (!string.IsNullOrEmpty(item.idNumProtocolloExcel) && !string.IsNullOrEmpty(item.idRegistro))
                                    {
                                        DocsPaVO.utente.InfoUtente infoUtenteProprietario = infoUtente;

                                        DocsPaVO.documento.SchedaDocumento schedaDocModificato = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurityByNumProtoEIDRegistro(infoUtente, item.idNumProtocolloExcel, item.idRegistro);
                                        if (schedaDocModificato != null)
                                        {
                                            //Prendo l'utente e ruolo proprietario se diversi da null
                                            if (!string.IsNullOrEmpty(item.idUtente) && !string.IsNullOrEmpty(item.idRuolo))
                                            {
                                                user = GetUtente(item, ref utenti);
                                                role = GetRuolo(item, ref ruoli);
                                            }

                                            //Prendo il registro
                                            if (!string.IsNullOrEmpty(item.idRegistro))
                                            {
                                                register = GetRegistro(item, ref registri);
                                            }

                                            if (inserisci)
                                            {
                                                bool success = BusinessLogic.Documenti.ProtoManager.DeleteProtocollo(infoUtente, schedaDocModificato);
                                                if (success)
                                                {
                                                    //Caso di successo
                                                    item.esito = "S";
                                                    item.idDocumento = schedaDocModificato.docNumber;
                                                    importazione.UpdateItemReportPregressi(item, item.systemId);
                                                }
                                                else
                                                {
                                                    item.esito = "E";
                                                    errori.Add("Errore generico nella cancellazione del documento");
                                                    importazione.UpdateItemReportPregressi(item, item.systemId);
                                                }
                                            }
                                            else
                                            {
                                                item.esito = "E";
                                                foreach (string err in errori)
                                                {
                                                    item.errore += err + " - ";
                                                }
                                                importazione.UpdateItemReportPregressi(item, item.systemId);
                                            }
                                        }
                                        else
                                        {
                                            item.esito = "E";
                                            item.errore = "Documento non trovato";
                                            importazione.UpdateItemReportPregressi(item, item.systemId);
                                        }
                                    }
                                    else
                                    {
                                        item.esito = "E";
                                        item.errore = "Numero di protocollo e/o registro mancante";
                                        importazione.UpdateItemReportPregressi(item, item.systemId);
                                    }
                                }
                                #endregion
                            }

                        }
                        else
                        {
                            //Errrore nel caso di mancanza di tipologia di operazione
                            item.esito = "E";
                            item.errore = "Tipo di operazione I, M o C non specificato";
                            importazione.UpdateItemReportPregressi(item, item.systemId);
                        }
                    }
                    catch (Exception ex)
                    {
                        //In caso di errore generico update dell'item con errore
                        item.esito = "E";
                        item.errore = "Errore nella creazione del documento";
                        importazione.UpdateItemReportPregressi(item, item.systemId);
                        logger.Debug("ERRROE (BusinessLogic/ImportPregressiManager.cs): " + ex.Message);
                    }

                    if (!string.IsNullOrEmpty(intervalloTimer) && !intervalloTimer.Equals("0"))
                    {
                        GC.KeepAlive(aTimer);
                    }
                }
                //Fine report
                importazione.UpdateReportPregressi(report.systemId);
                //
                if (!string.IsNullOrEmpty(intervalloTimer) && !intervalloTimer.Equals("0"))
                {
                    aTimer.Stop();
                }
            }
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            boolImporta = false;

            boolImporta = BusinessLogic.UserLog.UserLog.VerificaLogAggiungiDocumento(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TIMER_PREGRESSI"));

        }


        private static Allegato CreaAllegato(Allegati allTemp, ref SchedaDocumento schedaDoc, DocsPaVO.utente.InfoUtente infoUtenteProprietario)
        {
            Allegato allegato = new Allegato();
            allegato.descrizione = allTemp.descrizione;
            allegato.docNumber = schedaDoc.docNumber;
            allegato.version = "0";
            allegato.idPeopleDelegato = "0";
            // Impostazione del repositoryContext associato al documento
            allegato.repositoryContext = schedaDoc.repositoryContext;
            allegato.position = (schedaDoc.allegati.Count + 1);
            allegato = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtenteProprietario, allegato);
            allegato.path = allTemp.pathname;

            if (!string.IsNullOrEmpty(allegato.path))
            {
                byte[] fileDaFTP = AcquireFile(allegato.path);

                DocsPaVO.documento.FileDocumento fileDocumento = new DocsPaVO.documento.FileDocumento
                {
                    name = allTemp.pathname,
                    fullName = allTemp.pathname,
                    content = fileDaFTP,
                    //contentType = fileDaFTP.MimeType,
                    length = fileDaFTP.Length,
                    bypassFileContentValidation = true
                };

                FileRequest frq = (FileRequest)allegato;

                string erroreMessage = string.Empty;
                if (!BusinessLogic.Documenti.FileManager.putFile(ref frq, fileDocumento, infoUtenteProprietario, false, out erroreMessage))
                {
                    allegato.msgErr = erroreMessage;
                }
            }

            return allegato;
        }



        private static ResultProtocollazione CreaDocumento(ItemReportPregressi item, DocsPaVO.utente.Registro register, DocsPaVO.utente.Registro rf, string oggetto, DocsPaVO.utente.InfoUtente infoUtenteProprietario, DocsPaVO.ProfilazioneDinamica.Templates template, DocsPaVO.utente.Ruolo role, ref DocsPaVO.documento.SchedaDocumento schedaDoc, ref bool esitoAcqFile)
        {
            ResultProtocollazione risultatoProtocollazione = new ResultProtocollazione();

            schedaDoc.oggetto.descrizione = oggetto;
            schedaDoc.oggetto.daAggiornare = true;
            schedaDoc.tipoProto = item.tipoProtocollo;
            schedaDoc.registro = register;
            schedaDoc.protocollo.anno = register.anno_pregresso;
            schedaDoc.protocollo.dataProtocollazione = item.data;
            schedaDoc.protocollo.numero = item.idNumProtocolloExcel;
            schedaDoc.pregresso = true;
            if (rf != null)
            {
                schedaDoc.cod_rf_prot = rf.codRegistro;
                schedaDoc.id_rf_prot = rf.systemId;
                schedaDoc.id_rf_invio_ricevuta = rf.systemId;
            }
            //Nota
            schedaDoc.noteDocumento = new List<DocsPaVO.Note.InfoNota>();
            if (!string.IsNullOrEmpty(item.note))
            {
                DocsPaVO.Note.InfoNota tempNot = new DocsPaVO.Note.InfoNota();
                tempNot.DaInserire = true;
                tempNot.Testo = item.note;
                tempNot.TipoVisibilita = DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti;
                tempNot.UtenteCreatore = new DocsPaVO.Note.InfoUtenteCreatoreNota();
                tempNot.UtenteCreatore.IdRuolo = infoUtenteProprietario.idGruppo;
                tempNot.UtenteCreatore.IdUtente = infoUtenteProprietario.idPeople;
                schedaDoc.noteDocumento.Add(tempNot);
            }

            if (template != null)
            {
                schedaDoc.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
                schedaDoc.tipologiaAtto.systemId = template.SYSTEM_ID.ToString();
                schedaDoc.tipologiaAtto.descrizione = template.DESCRIZIONE.ToString();
                schedaDoc.template = template;
            }

            schedaDoc = BusinessLogic.Documenti.ProtoManager.protocolla(schedaDoc, role, infoUtenteProprietario, out risultatoProtocollazione);

            if (!string.IsNullOrEmpty(item.pathname))
            {
                try
                {
                    byte[] fileDaFTP = AcquireFile(item.pathname);

                    if (!InsertFile(item.pathname, fileDaFTP, schedaDoc, infoUtenteProprietario))
                    {
                        item.esito = "W";
                        item.errore = "Errorri in attach del file.";
                        //DocsPaDB.Query_DocsPAWS.ImportPregressi importazione = new DocsPaDB.Query_DocsPAWS.ImportPregressi();
                        //importazione.UpdateItemReportPregressi(item, item.systemId);
                        esitoAcqFile = false;
                    }
                }
                catch (Exception e)
                {
                    item.esito = "W";
                    //item.errore = concatenaMessaggio(item, "Errore nell'acquisizione del documento principale.");
                    item.errore = concatenaMessaggio(item, "Errore nell'acquisizione del documento principale.");
                    esitoAcqFile = false;
                }
            }

            return risultatoProtocollazione;
        }

        private static SchedaDocumento CreaDocumentoNonProtocollato(ItemReportPregressi item, string oggetto, DocsPaVO.utente.InfoUtente infoUtenteProprietario, DocsPaVO.ProfilazioneDinamica.Templates template, DocsPaVO.utente.Ruolo role, DocsPaVO.documento.SchedaDocumento schedaDoc, ref bool esitoAcqFile)
        {

            schedaDoc.oggetto.descrizione = oggetto;
            schedaDoc.oggetto.daAggiornare = true;
            schedaDoc.dataCreazione = item.data;
            //schedaDoc.protocollo.dataProtocollazione = item.data;
            //schedaDoc.systemId = item.idNumProtocolloExcel;
            //schedaDoc.protocollo.numero = item.idNumProtocolloExcel;
            schedaDoc.pregresso = true;
            
            //Nota - Nel campo nota aggiungo anche ID_VECCHIO_DOCUMENTO
            schedaDoc.noteDocumento = new List<DocsPaVO.Note.InfoNota>();
            if (!string.IsNullOrEmpty(item.idNumProtocolloExcel)) 
            {
                DocsPaVO.Note.InfoNota id_vecchio_doc = new DocsPaVO.Note.InfoNota();
                id_vecchio_doc.DaInserire = true;
                //Modificato per richiesta del Cliente: ID vecchio documento diventa Numero identificativo documento pregresso
                //id_vecchio_doc.Testo = "ID vecchio documento: " + item.idNumProtocolloExcel;
                id_vecchio_doc.Testo = "Numero identificativo documento pregresso: " + item.idNumProtocolloExcel;
                id_vecchio_doc.TipoVisibilita = DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti;
                id_vecchio_doc.UtenteCreatore = new DocsPaVO.Note.InfoUtenteCreatoreNota();
                id_vecchio_doc.UtenteCreatore.IdRuolo = infoUtenteProprietario.idGruppo;
                id_vecchio_doc.UtenteCreatore.IdUtente = infoUtenteProprietario.idPeople;
                schedaDoc.noteDocumento.Add(id_vecchio_doc);
            }

            if (!string.IsNullOrEmpty(item.note))
            {
                DocsPaVO.Note.InfoNota tempNot = new DocsPaVO.Note.InfoNota();
                tempNot.DaInserire = true;
                tempNot.Testo = item.note;
                tempNot.TipoVisibilita = DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti;
                tempNot.UtenteCreatore = new DocsPaVO.Note.InfoUtenteCreatoreNota();
                tempNot.UtenteCreatore.IdRuolo = infoUtenteProprietario.idGruppo;
                tempNot.UtenteCreatore.IdUtente = infoUtenteProprietario.idPeople;
                schedaDoc.noteDocumento.Add(tempNot);
            }

            if (template != null)
            {
                schedaDoc.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
                schedaDoc.tipologiaAtto.systemId = template.SYSTEM_ID.ToString();
                schedaDoc.tipologiaAtto.descrizione = template.DESCRIZIONE.ToString();
                schedaDoc.template = template;
            }

            schedaDoc = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtenteProprietario, role);

            if (!string.IsNullOrEmpty(item.pathname))
            {
                try
                {
                    byte[] fileDaFTP = AcquireFile(item.pathname);

                    if (!InsertFile(item.pathname, fileDaFTP, schedaDoc, infoUtenteProprietario))
                    {
                        item.esito = "W";
                        item.errore = "Errorri in attach del file.";
                        //DocsPaDB.Query_DocsPAWS.ImportPregressi importazione = new DocsPaDB.Query_DocsPAWS.ImportPregressi();
                        //importazione.UpdateItemReportPregressi(item, item.systemId);
                        esitoAcqFile = false;
                    }
                }
                catch (Exception e)
                {
                    item.esito = "W";
                    //item.errore = concatenaMessaggio(item, "Errore nell'acquisizione del documento principale.");
                    item.errore = concatenaMessaggio(item, "Errore nell'acquisizione del documento principale.");
                    esitoAcqFile = false;
                }
            }

            return schedaDoc;
        }

        private static void UpdateDataCreazioneDocGrigio(SchedaDocumento schedaDoc, string id_vecchio_documento)
        {
            if (schedaDoc != null)
            {
            DocsPaDB.Query_DocsPAWS.Documenti docum = new DocsPaDB.Query_DocsPAWS.Documenti();
            docum.updateDataCreazioneDocGrigio(schedaDoc, id_vecchio_documento);
            }
        }

        private static byte[] AcquireFile(string path)
        {
            string ftpAddress = System.Configuration.ConfigurationManager.AppSettings["FTP_ADDRESS"];
            string ftpUsername = System.Configuration.ConfigurationManager.AppSettings["FTP_USERNAME"];
            string ftpPassword = System.Configuration.ConfigurationManager.AppSettings["FTP_PASSWORD"];

            byte[] fileContent;

            try
            {
                // Apertura, lettura e chiusura del file
                fileContent = BusinessLogic.Import.ImportUtils.DownloadFileFromFTP(
                     ftpAddress,
                     path,
                     ftpUsername,
                     ftpPassword);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return fileContent;
        }

        private static bool InsertFile(string path, byte[] fileContent, SchedaDocumento doc, DocsPaVO.utente.InfoUtente userInfo)
        {
            FileRequest fileRequest;
            FileDocumento fileDocumento;
            bool result = false;

            #region Creazione dell'oggetto fileDocumento

            // Creazione dell'oggetto fileDocumento
            fileDocumento = new FileDocumento();

            // Impostazione del nome del file
            fileDocumento.name = Path.GetFileName(path);

            // Impostazione del full name
            fileDocumento.fullName = path;

            // Impostazione del path
            fileDocumento.path = Path.GetPathRoot(path);

            // Impostazione della grandezza del file
            fileDocumento.length = fileContent.Length;

            // Impostazione del content del documento
            fileDocumento.content = fileContent;

            #endregion

            #region Creazione dell'oggetto fileRequest

            fileRequest = (FileRequest)doc.documenti[0];

            #endregion

            #region Acquisizione del file

            try
            {
                //19-02-2013: Aggiunto il parametro false per il VerifyFileFormat per il problema dei file P7M da Importare
                FileManager.putFile(fileRequest, fileDocumento, userInfo, false);
                result = true;
            }
            catch (Exception e)
            {
                // Aggiunta del problema alla lista dei problemi
                throw new Exception("Errore durante l'upload del file.");
            }

            #endregion

            return result;
        }

        private static DocsPaVO.ProfilazioneDinamica.Templates GetTemplate(ItemReportPregressi item, DocsPaVO.utente.InfoUtente infoUtenteProprietario, DocsPaVO.utente.Utente user, DocsPaVO.utente.Ruolo role, DocsPaVO.utente.Registro register, DocsPaVO.utente.Registro rf, ref List<string> errori, ref bool inserisci, ref Dictionary<string, DocsPaVO.ProfilazioneDinamica.Templates> tipiDocumento, Dictionary<string, ArrayList> DictionaryIdRuoli)
        {
            DocsPaVO.ProfilazioneDinamica.Templates template = null;

            if (!string.IsNullOrEmpty(item.tipo_documento))
            {
                if (tipiDocumento.ContainsKey(item.tipo_documento))
                {
                    template = tipiDocumento[item.tipo_documento];
                }
                else
                {
                    template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateByDescrizione(item.tipo_documento, infoUtenteProprietario.idAmministrazione);
                    if (template != null)
                    {
                        tipiDocumento.Add(item.tipo_documento, template);
                    }
                    else
                    {
                        //Errrore nel caso di template insesistente
                        inserisci = false;
                        item.esito = "E";
                        errori.Add("Tipologia di documento non esistente");
                    }
                }
                if (template != null)
                {
                    template = GetTemplateValorizzato(infoUtenteProprietario, template, item.valoriProfilati, user, role, register, rf, ref errori, ref inserisci, DictionaryIdRuoli);
                }

            }

            return template;
        }

        private static void ControlloCorrispondenti(ItemReportPregressi item, DocsPaVO.utente.InfoUtente infoUtenteProprietario, DocsPaVO.utente.Registro register, DocsPaVO.utente.Registro rf, bool modifica, ref List<string> errori, ref bool inserisci, ref DocsPaVO.documento.SchedaDocumento schedaDoc, ref List<DocsPaVO.utente.Corrispondente> corrispondenti, Dictionary<string, ArrayList> DictionaryIdRuoli, Ruolo role)
        {
            if (string.IsNullOrEmpty(item.cod_corrispondenti) && string.IsNullOrEmpty(item.corrispondenti))
            {
                //Corrispondenti non presenti
                inserisci = false;
                item.esito = "E";
                errori.Add("Obbligatorio un valore tra codice corrispondenti o corrispondenti.");
            }
            else
            {
                //Delimiter per lo split
                char[] delimiterChar = { ';' };
                string[] tempCorrispondenti = null;
                string[] tempCodCorrispondenti = null;
                List<string> listM_Corr = new List<string>();
                List<string> listD_Corr = new List<string>();
                List<string> listCC_Corr = new List<string>();
                List<string> listM_CodCorr = new List<string>();
                List<string> listD_CodCorr = new List<string>();
                List<string> listCC_CodCorr = new List<string>();
                string idRf = string.Empty;
                if (rf != null)
                {
                    idRf = rf.systemId;
                }

                //Per ogni iten popolo delle liste di (Mittenti, destinatari, CC)
                if (!string.IsNullOrEmpty(item.corrispondenti))
                {
                    tempCorrispondenti = item.corrispondenti.Split(delimiterChar);
                }

                //Per ogni iten popolo delle liste di (Mittenti, destinatari, CC)
                if (!string.IsNullOrEmpty(item.cod_corrispondenti))
                {
                    tempCodCorrispondenti = item.cod_corrispondenti.Split(delimiterChar);
                }

                if (tempCorrispondenti != null)
                {
                    for (int i = 0; i < tempCorrispondenti.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(tempCorrispondenti[i]))
                        {
                            string[] tempcorr = tempCorrispondenti[i].Split('#');
                            if (tempcorr.Count() > 1)
                            {
                                string tipoCorr = tempcorr[1];
                                switch (tipoCorr.ToUpper())
                                {
                                    case "M":
                                        listM_Corr.Add(tempcorr[0]);
                                        break;
                                    case "D":
                                        listD_Corr.Add(tempcorr[0]);
                                        break;
                                    case "CC":
                                        listCC_Corr.Add(tempcorr[0]);
                                        break;
                                }
                            }
                        }
                    }
                }

                if (tempCodCorrispondenti != null)
                {
                    for (int i = 0; i < tempCodCorrispondenti.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(tempCodCorrispondenti[i]))
                        {
                            string[] tempcorr = tempCodCorrispondenti[i].Split('#');
                            if (tempcorr.Count() > 1)
                            {
                                string tipoCorr = tempcorr[1];
                                switch (tipoCorr.ToUpper())
                                {
                                    case "M":
                                        listM_CodCorr.Add(tempcorr[0]);
                                        break;
                                    case "D":
                                        listD_CodCorr.Add(tempcorr[0]);
                                        break;
                                    case "CC":
                                        listCC_CodCorr.Add(tempcorr[0]);
                                        break;
                                }
                            }
                        }
                    }
                }

                //Controllo mittenti
                if ((listM_CodCorr != null && listM_CodCorr.Count > 0 && listM_Corr != null && listM_Corr.Count > 0) || (listM_CodCorr != null && listM_CodCorr.Count > 1) || (listM_Corr != null && listM_Corr.Count > 1))
                {
                    inserisci = false;
                    item.esito = "E";
                    errori.Add("Non possono essere presenti più mittenti contemporaneamente per i corrispondenti.");
                }
                else
                {
                    if ((listM_CodCorr == null || listM_CodCorr.Count == 0) && (listM_Corr == null || listM_Corr.Count == 0))
                    {
                        inserisci = false;
                        item.esito = "E";
                        errori.Add("Nessun mittente presente.");
                    }
                    else
                    {
                        if (listM_CodCorr != null && listM_CodCorr.Count > 0)
                        {
                            DocsPaVO.utente.Corrispondente corr = null;
                            //Ricerca Mittente

                            corr = corrispondenti.Where(f => f.codiceRubrica == listM_CodCorr[0] && f.idAmministrazione == infoUtenteProprietario.idAmministrazione && (f.idRegistro == register.systemId || f.idRegistro == idRf)).FirstOrDefault();
                            if (corr == null)
                            {
                                corr = CercaCorrispondenteDaCodice(listM_CodCorr[0], schedaDoc.tipoProto, "M", register.systemId, idRf, infoUtenteProprietario, DictionaryIdRuoli, role);
                                if (corr == null)
                                {
                                    inserisci = false;
                                    item.esito = "E";
                                    errori.Add("Corrispondente " + listM_CodCorr[0] + " non trovato");
                                }
                                else
                                {
                                    corrispondenti.Add(corr);
                                    if (schedaDoc.tipoProto.Equals("A"))
                                    {
                                        ((ProtocolloEntrata)schedaDoc.protocollo).mittente = corr;
                                        if (modifica)
                                        {
                                            ((ProtocolloEntrata)schedaDoc.protocollo).daAggiornareMittente = true;
                                        }
                                    }
                                    else
                                    {
                                        if (schedaDoc.tipoProto.Equals("P"))
                                        {
                                            ((ProtocolloUscita)schedaDoc.protocollo).mittente = corr;
                                            if (modifica)
                                            {
                                                ((ProtocolloUscita)schedaDoc.protocollo).daAggiornareMittente = true;
                                            }
                                        }
                                        else
                                        {
                                            ((ProtocolloInterno)schedaDoc.protocollo).mittente = corr;
                                            if (modifica)
                                            {
                                                ((ProtocolloInterno)schedaDoc.protocollo).daAggiornareMittente = true;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (schedaDoc.tipoProto.Equals("A"))
                                {
                                    ((ProtocolloEntrata)schedaDoc.protocollo).mittente = corr;
                                    if (modifica)
                                    {
                                        ((ProtocolloEntrata)schedaDoc.protocollo).daAggiornareMittente = true;
                                    }
                                }
                                else
                                {
                                    if (schedaDoc.tipoProto.Equals("P"))
                                    {
                                        ((ProtocolloUscita)schedaDoc.protocollo).mittente = corr;
                                        if (modifica)
                                        {
                                            ((ProtocolloUscita)schedaDoc.protocollo).daAggiornareMittente = true;
                                        }
                                    }
                                    else
                                    {
                                        ((ProtocolloInterno)schedaDoc.protocollo).mittente = corr;
                                        if (modifica)
                                        {
                                            ((ProtocolloInterno)schedaDoc.protocollo).daAggiornareMittente = true;
                                        }
                                    }
                                }
                            }

                        }
                        else
                        {
                            //Creazione Mittente Occasionale
                            if (schedaDoc.tipoProto.Equals("A"))
                            {
                                ((ProtocolloEntrata)schedaDoc.protocollo).mittente = GetCorrispondenteOccasionale(listM_Corr[0], infoUtenteProprietario);
                                if (modifica)
                                {
                                    ((ProtocolloEntrata)schedaDoc.protocollo).daAggiornareMittente = true;
                                }
                            }
                            else
                            {
                                if (schedaDoc.tipoProto.Equals("P"))
                                {
                                    ((ProtocolloUscita)schedaDoc.protocollo).mittente = GetCorrispondenteOccasionale(listM_Corr[0], infoUtenteProprietario);
                                    if (modifica)
                                    {
                                        ((ProtocolloUscita)schedaDoc.protocollo).daAggiornareMittente = true;
                                    }
                                }
                                else
                                {
                                    ((ProtocolloInterno)schedaDoc.protocollo).mittente = GetCorrispondenteOccasionale(listM_Corr[0], infoUtenteProprietario);
                                    if (modifica)
                                    {
                                        ((ProtocolloInterno)schedaDoc.protocollo).daAggiornareMittente = true;
                                    }
                                }
                            }
                        }
                    }
                }

                //Controllo destinatari

                if (schedaDoc.tipoProto.Equals("P") || schedaDoc.tipoProto.Equals("I"))
                {

                    if ((listD_CodCorr == null || listD_CodCorr.Count == 0) && (listD_Corr == null || listD_Corr.Count == 0))
                    {
                        inserisci = false;
                        item.esito = "E";
                        errori.Add("Nessun destinatario presente.");
                    }
                    else
                    {
                        if (listD_CodCorr != null && listD_CodCorr.Count > 0)
                        {
                            foreach (string codiceCorr in listD_CodCorr)
                            {
                                DocsPaVO.utente.Corrispondente corr = null;
                                //Ricerca Destinatario
                                if (corrispondenti != null & corrispondenti.Count > 0)
                                {
                                    corr = corrispondenti.Where(f => f.codiceRubrica == codiceCorr && f.idAmministrazione == infoUtenteProprietario.idAmministrazione && (f.idRegistro == register.systemId || f.idRegistro == idRf)).FirstOrDefault();
                                    if (corr == null)
                                    {
                                        corr = CercaCorrispondenteDaCodice(codiceCorr, schedaDoc.tipoProto, "D", register.systemId, idRf, infoUtenteProprietario, DictionaryIdRuoli, role);
                                        if (corr == null)
                                        {
                                            inserisci = false;
                                            item.esito = "E";
                                            errori.Add("Corrispondente " + codiceCorr + " non trovato");
                                        }
                                        else
                                        {
                                            corrispondenti.Add(corr);
                                            if (schedaDoc.tipoProto.Equals("P"))
                                            {
                                                if (((ProtocolloUscita)schedaDoc.protocollo).destinatari == null)
                                                {
                                                    ((ProtocolloUscita)schedaDoc.protocollo).destinatari = new ArrayList();
                                                }
                                                ((ProtocolloUscita)schedaDoc.protocollo).destinatari.Add(corr);
                                                if (modifica)
                                                {
                                                    ((ProtocolloUscita)schedaDoc.protocollo).daAggiornareDestinatari = true;
                                                }
                                            }
                                            else
                                            {
                                                if (((ProtocolloInterno)schedaDoc.protocollo).destinatari == null)
                                                {
                                                    ((ProtocolloInterno)schedaDoc.protocollo).destinatari = new ArrayList();
                                                }
                                                ((ProtocolloInterno)schedaDoc.protocollo).destinatari.Add(corr);
                                                if (modifica)
                                                {
                                                    ((ProtocolloInterno)schedaDoc.protocollo).daAggiornareDestinatari = true;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (schedaDoc.tipoProto.Equals("P"))
                                        {
                                            if (((ProtocolloUscita)schedaDoc.protocollo).destinatari == null)
                                            {
                                                ((ProtocolloUscita)schedaDoc.protocollo).destinatari = new ArrayList();
                                            }
                                            ((ProtocolloUscita)schedaDoc.protocollo).destinatari.Add(corr);
                                            if (modifica)
                                            {
                                                ((ProtocolloUscita)schedaDoc.protocollo).daAggiornareDestinatari = true;
                                            }
                                        }
                                        else
                                        {
                                            if (((ProtocolloInterno)schedaDoc.protocollo).destinatari == null)
                                            {
                                                ((ProtocolloInterno)schedaDoc.protocollo).destinatari = new ArrayList();
                                            }
                                            ((ProtocolloInterno)schedaDoc.protocollo).destinatari.Add(corr);
                                            if (modifica)
                                            {
                                                ((ProtocolloInterno)schedaDoc.protocollo).daAggiornareDestinatari = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (listD_Corr != null && listD_Corr.Count > 0)
                        {
                            foreach (string codiceCorr in listD_Corr)
                            {
                                //Creazione destinatario Occasionale
                                if (schedaDoc.tipoProto.Equals("P"))
                                {
                                    if (((ProtocolloUscita)schedaDoc.protocollo).destinatari == null)
                                    {
                                        ((ProtocolloUscita)schedaDoc.protocollo).destinatari = new ArrayList();
                                    }
                                    ((ProtocolloUscita)schedaDoc.protocollo).destinatari.Add(GetCorrispondenteOccasionale(codiceCorr, infoUtenteProprietario));
                                    if (modifica)
                                    {
                                        ((ProtocolloUscita)schedaDoc.protocollo).daAggiornareDestinatari = true;
                                    }
                                }
                                else
                                {
                                    if (((ProtocolloInterno)schedaDoc.protocollo).destinatari == null)
                                    {
                                        ((ProtocolloInterno)schedaDoc.protocollo).destinatari = new ArrayList();
                                    }
                                    ((ProtocolloInterno)schedaDoc.protocollo).destinatari.Add(GetCorrispondenteOccasionale(codiceCorr, infoUtenteProprietario));
                                    if (modifica)
                                    {
                                        ((ProtocolloInterno)schedaDoc.protocollo).daAggiornareDestinatari = true;
                                    }
                                }
                            }
                        }
                    }


                    if (listCC_CodCorr != null && listCC_CodCorr.Count > 0)
                    {
                        foreach (string codiceCorr in listCC_CodCorr)
                        {
                            DocsPaVO.utente.Corrispondente corr = null;
                            //Ricerca Destinatari
                            if (corrispondenti != null & corrispondenti.Count > 0)
                            {
                                corr = corrispondenti.Where(f => f.codiceRubrica == codiceCorr && f.idAmministrazione == infoUtenteProprietario.idAmministrazione && (f.idRegistro == register.systemId || f.idRegistro == idRf)).FirstOrDefault();
                                if (corr == null)
                                {
                                    corr = CercaCorrispondenteDaCodice(codiceCorr, schedaDoc.tipoProto, "D", register.systemId, idRf, infoUtenteProprietario, DictionaryIdRuoli, role);
                                    if (corr == null)
                                    {
                                        inserisci = false;
                                        item.esito = "E";
                                        errori.Add("Corrispondente " + codiceCorr + " non trovato");
                                    }
                                    else
                                    {
                                        corrispondenti.Add(corr);
                                        if (schedaDoc.tipoProto.Equals("P"))
                                        {
                                            if (((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza == null)
                                            {
                                                ((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza = new ArrayList();
                                            }
                                            ((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(corr);
                                            if (modifica)
                                            {
                                                ((ProtocolloUscita)schedaDoc.protocollo).daAggiornareDestinatariConoscenza = true;
                                            }
                                        }
                                        else
                                        {
                                            if (((ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza == null)
                                            {
                                                ((ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza = new ArrayList();
                                            }
                                            ((ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza.Add(corr);
                                            if (modifica)
                                            {
                                                ((ProtocolloInterno)schedaDoc.protocollo).daAggiornareDestinatariConoscenza = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (schedaDoc.tipoProto.Equals("P"))
                                    {
                                        if (((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza == null)
                                        {
                                            ((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza = new ArrayList();
                                        }
                                        ((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(corr);
                                        if (modifica)
                                        {
                                            ((ProtocolloUscita)schedaDoc.protocollo).daAggiornareDestinatariConoscenza = true;
                                        }
                                    }
                                    else
                                    {
                                        if (((ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza == null)
                                        {
                                            ((ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza = new ArrayList();
                                        }
                                        ((ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza.Add(corr);
                                        if (modifica)
                                        {
                                            ((ProtocolloInterno)schedaDoc.protocollo).daAggiornareDestinatariConoscenza = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (listCC_Corr != null && listCC_Corr.Count > 0)
                    {
                        foreach (string codiceCorr in listCC_Corr)
                        {
                            //Creazione destinatario Occasionale
                            if (schedaDoc.tipoProto.Equals("P"))
                            {
                                if (((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza == null)
                                {
                                    ((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza = new ArrayList();
                                }
                                ((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(GetCorrispondenteOccasionale(codiceCorr, infoUtenteProprietario));
                                if (modifica)
                                {
                                    ((ProtocolloUscita)schedaDoc.protocollo).daAggiornareDestinatariConoscenza = true;
                                }
                            }
                            else
                            {
                                if (((ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza == null)
                                {
                                    ((ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza = new ArrayList();
                                }
                                ((ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza.Add(GetCorrispondenteOccasionale(codiceCorr, infoUtenteProprietario));
                                if (modifica)
                                {
                                    ((ProtocolloInterno)schedaDoc.protocollo).daAggiornareDestinatariConoscenza = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static string GetOggetto(ItemReportPregressi item, Registro register, DocsPaVO.utente.InfoUtente infoUtente, ref List<string> errori, ref bool inserisci, ref List<DocsPaVO.documento.Oggetto> oggettario)
        {
            string oggetto = item.oggetto;
            if (string.IsNullOrEmpty(item.oggetto) && string.IsNullOrEmpty(item.cod_oggetto))
            {
                inserisci = false;
                errori.Add("Oggetto del protocollo obbligatorio");
            }
            else
            {
                if (!string.IsNullOrEmpty(item.oggetto) && !string.IsNullOrEmpty(item.cod_oggetto))
                {
                    inserisci = false;
                    errori.Add("Valorizzare soltanto un campo tra Oggetto o Codice oggetto");
                }
                else
                {
                    if (register != null)
                    {
                        if (!string.IsNullOrEmpty(item.cod_oggetto))
                        {
                            if (oggettario.Where(f => f.codOggetto == item.cod_oggetto && (f.idRegistro == register.systemId || string.IsNullOrEmpty(f.idRegistro))).FirstOrDefault() != null)
                            {
                                oggetto = oggettario.Where(f => f.codOggetto == item.cod_oggetto && (f.idRegistro == register.systemId || string.IsNullOrEmpty(f.idRegistro))).FirstOrDefault().descrizione;
                            }
                            else
                            {
                                DocsPaVO.documento.QueryOggetto objQueryOggetto = new DocsPaVO.documento.QueryOggetto();
                                objQueryOggetto.idRegistri = new ArrayList();
                                //objQueryOggetto.idRegistri.Add(register.systemId);
                                objQueryOggetto.queryCodice = item.cod_oggetto;
                                objQueryOggetto.idAmministrazione = infoUtente.idAmministrazione;
                                ArrayList arrayOggetti = BusinessLogic.Documenti.ProtoManager.getListaOggetti(objQueryOggetto);
                                if (arrayOggetti != null && arrayOggetti.Count > 0)
                                {
                                    oggetto = ((DocsPaVO.documento.Oggetto)arrayOggetti[0]).descrizione;
                                    oggettario.Add((DocsPaVO.documento.Oggetto)arrayOggetti[0]);
                                }
                                else
                                {
                                    inserisci = false;
                                    errori.Add("Il codice oggetto non è presente");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.cod_oggetto))
                        {
                            if (oggettario.Where(f => f.codOggetto == item.cod_oggetto && (f.idRegistro == string.Empty || string.IsNullOrEmpty(f.idRegistro))).FirstOrDefault() != null)
                            {
                                oggetto = oggettario.Where(f => f.codOggetto == item.cod_oggetto && (f.idRegistro == string.Empty || string.IsNullOrEmpty(f.idRegistro))).FirstOrDefault().descrizione;
                            }
                            else
                            {
                                DocsPaVO.documento.QueryOggetto objQueryOggetto = new DocsPaVO.documento.QueryOggetto();
                                objQueryOggetto.idRegistri = new ArrayList();
                                //objQueryOggetto.idRegistri.Add(string.Empty);
                                objQueryOggetto.queryCodice = item.cod_oggetto;
                                objQueryOggetto.idAmministrazione = infoUtente.idAmministrazione;
                                ArrayList arrayOggetti = BusinessLogic.Documenti.ProtoManager.getListaOggetti(objQueryOggetto);
                                if (arrayOggetti != null && arrayOggetti.Count > 0)
                                {
                                    oggetto = ((DocsPaVO.documento.Oggetto)arrayOggetti[0]).descrizione;
                                    oggettario.Add((DocsPaVO.documento.Oggetto)arrayOggetti[0]);
                                }
                                else
                                {
                                    inserisci = false;
                                    errori.Add("Il codice oggetto non è presente");
                                }
                            }
                        }
                    }
                }
            }
            return oggetto;
        }

        private static bool ControlloTipoProtocollo(ItemReportPregressi item, ref List<string> errori, ref bool inserisci)
        {
            bool result = true;
            if (string.IsNullOrEmpty(item.tipoProtocollo) || (!string.IsNullOrEmpty(item.tipoProtocollo) && !item.tipoProtocollo.Equals("A") && !item.tipoProtocollo.Equals("P") && !item.tipoProtocollo.Equals("I")))
            {
                result = false;
                inserisci = false;
                errori.Add("Tipo di protocollo non presente o errato");
            }
            return result;
        }

        private static DocsPaVO.utente.Registro GetRF(ItemReportPregressi item, DocsPaVO.utente.InfoUtente infoUtente, ref Dictionary<string, DocsPaVO.utente.Registro> listaRf, ref List<string> errori, ref bool inserisci, DocsPaVO.utente.Ruolo role)
        {
            DocsPaVO.utente.Registro rf = null;
            if (!string.IsNullOrEmpty(item.cod_rf))
            {
                if (listaRf.ContainsKey(item.cod_rf))
                {
                    rf = listaRf[item.cod_rf];
                }
                else
                {
                    rf = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(item.cod_rf);
                    if (rf == null)
                    {
                        inserisci = false;
                        errori.Add("RF non presente");
                    }
                    else
                    {
                        bool trovato = false;
                        //ArrayList arrayRF = BusinessLogic.Utenti.RegistriManager.getListaRegistriRfRuolo(item.idRuolo, "1", infoUtente.idAmministrazione);
                        ArrayList arrayRF = BusinessLogic.Utenti.RegistriManager.getListaRegistriRfRuolo(role.systemId, "1", string.Empty);
                        if (arrayRF != null && arrayRF.Count > 0)
                        {
                            foreach (DocsPaVO.utente.Registro regTemp in arrayRF)
                            {
                                if (rf.systemId.Equals(regTemp.systemId))
                                {
                                    trovato = true;
                                    break;
                                }
                            }
                        }
                        if (trovato)
                        {
                            listaRf.Add(item.cod_rf, rf);
                        }
                        else
                        {
                            inserisci = false;
                            rf = null;
                            errori.Add("RF non visibile dal ruolo proprietario");
                        }
                    }
                }
            }
            return rf;
        }

        private static DocsPaVO.utente.Registro GetRegistro(ItemReportPregressi item, ref Dictionary<string, DocsPaVO.utente.Registro> registri)
        {
            DocsPaVO.utente.Registro register = null;
            if (registri.ContainsKey(item.idRegistro))
            {
                register = registri[item.idRegistro];
            }
            else
            {
                register = BusinessLogic.Utenti.RegistriManager.getRegistro(item.idRegistro);
                registri.Add(item.idRegistro, register);
            }
            return register;
        }

        private static DocsPaVO.utente.Ruolo GetRuolo(ItemReportPregressi item, ref Dictionary<string, DocsPaVO.utente.Ruolo> ruoli)
        {
            DocsPaVO.utente.Ruolo role = null;
            if (ruoli.ContainsKey(item.idRuolo))
            {
                role = ruoli[item.idRuolo];
            }
            else
            {
                role = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(item.idRuolo);
                ruoli.Add(item.idRuolo, role);
            }

            return role;
        }

        private static DocsPaVO.utente.Utente GetUtente(ItemReportPregressi item, ref Dictionary<string, DocsPaVO.utente.Utente> utenti)
        {
            DocsPaVO.utente.Utente user = null;
            if (utenti.ContainsKey(item.idUtente))
            {
                user = utenti[item.idUtente];
            }
            else
            {
                user = BusinessLogic.Utenti.UserManager.getUtenteById(item.idUtente);
                utenti.Add(item.idUtente, user);
            }
            return user;
        }

        private static DocsPaVO.utente.Corrispondente CercaCorrispondenteDaCodice(string codice, string tipo, string tipoCorrispondente, string idRegistro, string idRf, DocsPaVO.utente.InfoUtente infoUtente, Dictionary<string, ArrayList> DictionaryIdRuoli, Ruolo role)
        {
            DocsPaVO.utente.Corrispondente result = new Corrispondente();
            string listaReg = string.Empty;
            SetUserId(infoUtente);
            ParametriRicercaRubrica qc = new ParametriRicercaRubrica();

            qc.caller = new ParametriRicercaRubrica.CallerIdentity();
            qc.parent = "";
            qc.caller.IdRuolo = infoUtente.idGruppo;
            qc.caller.IdRegistro = idRegistro;
            qc.codice = codice;
            qc.queryCodiceEsatta = true;
            listaReg = " " + idRegistro;

            string retValue = "";

            foreach (Registro item in DictionaryIdRuoli[role.systemId])
            {
                if (item != null)
                {
                    retValue += " " + item.systemId + ",";
                }
            }
            if (retValue.EndsWith(","))
            {
                retValue = retValue.Remove(retValue.LastIndexOf(","));
            }

            //Forse togliere
            //if (!string.IsNullOrEmpty(idRf))
            //{
            //    listaReg = " ," + idRf;
            //}

            //qc.caller.filtroRegistroPerRicerca = listaReg;
            qc.caller.filtroRegistroPerRicerca = retValue;

            if (tipo.Equals("A"))
            {
                if (tipoCorrispondente.Equals("M"))
                {
                    qc.calltype = ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_IN;
                    qc.tipoIE = TipoUtente.GLOBALE;
                    qc.doRuoli = true;
                    qc.doUo = true;
                    qc.doUtenti = true;
                    qc.doListe = false;
                    qc.doRF = false;
                    if (abilitazioneRubricaComune)
                    {
                        qc.doRubricaComune = true;
                    }
                }
            }

            if (tipo.Equals("P"))
            {
                if (tipoCorrispondente.Equals("M"))
                {
                    qc.calltype = ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT_MITT;
                    qc.tipoIE = TipoUtente.INTERNO;
                    qc.doListe = false;
                    qc.doRF = false;
                    qc.doRuoli = true;
                    qc.doUtenti = true;
                    qc.doUo = true;
                }
                else
                {
                    qc.calltype = ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT;
                    qc.tipoIE = TipoUtente.GLOBALE;
                    qc.doListe = true;
                    qc.doRF = true;
                    qc.doRubricaComune = true;
                    if (abilitazioneRubricaComune)
                    {
                        qc.doRubricaComune = true;
                    }
                    qc.doRuoli = true;
                    qc.doUo = true;
                    qc.doUtenti = true;
                }
            }

            if (tipo.Equals("I"))
            {
                if (tipoCorrispondente.Equals("M"))
                {
                    qc.calltype = ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_MITT;
                    qc.tipoIE = TipoUtente.INTERNO;
                    qc.doListe = false;
                    qc.doRF = false;
                    qc.doRuoli = true;
                    qc.doUtenti = true;
                    qc.doUo = true;
                }
                else
                {
                    qc.calltype = ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST;
                    qc.tipoIE = TipoUtente.GLOBALE;
                    qc.doListe = true;
                    qc.doRF = true;
                    qc.doRubricaComune = true;
                    if (abilitazioneRubricaComune)
                    {
                        qc.doRubricaComune = true;
                    }
                    qc.doRuoli = true;
                    qc.doUo = true;
                    qc.doUtenti = true;
                }
            }

            ArrayList listaElementi = new ArrayList();
            DocsPaDB.Query_DocsPAWS.Rubrica query = new DocsPaDB.Query_DocsPAWS.Rubrica(infoUtente);
            listaElementi = query.GetElementiRubrica(qc);

            if (listaElementi != null && listaElementi.Count > 0)
            {
                string idCorr = ((ElementoRubrica)listaElementi[0]).systemId;
                result = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(idCorr);
            }
            else
            {
                result = null;
            }

            return result;
        }

        private static DocsPaVO.utente.Corrispondente GetCorrispondenteOccasionale(string descrizione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.utente.Corrispondente result = new DocsPaVO.utente.Corrispondente();

            result.descrizione = descrizione;

            result.tipoCorrispondente = "O";

            result.idAmministrazione = infoUtente.idAmministrazione;

            return result;
        }

        private static DocsPaVO.utente.InfoUtente ImpostaInfoUtente(DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo)
        {
            DocsPaVO.utente.InfoUtente result = new InfoUtente();
            result.idAmministrazione = utente.idAmministrazione;
            result.idCorrGlobali = utente.idAmministrazione;
            result.idGruppo = ruolo.idGruppo;
            result.idPeople = utente.idPeople;
            result.userId = utente.userId;
            result.dst = utente.dst;
            result.idCorrGlobali = ruolo.systemId;
            if (string.IsNullOrEmpty(utente.dst))
            {
                // Reperimento token superutente
                result.dst = UserManager.getSuperUserAuthenticationToken();
            }

            return result;
        }

        private static DocsPaVO.ProfilazioneDinamica.Templates GetTemplateValorizzato(InfoUtente infoUtenteProprietario, DocsPaVO.ProfilazioneDinamica.Templates template, List<string> listaValori, Utente user, Ruolo role, Registro register, Registro rf, ref List<string> errori, ref bool inserisci, Dictionary<string, ArrayList> DictionaryIdRuoli)
        {
            DocsPaVO.ProfilazioneDinamica.Templates result = template;
            Dictionary<string, string> valoriTemplate = new Dictionary<string, string>();

            //Prendo i valori e li inserisco nel dictionary chiave nome campo.
            if (listaValori != null && listaValori.Count > 0)
            {
                foreach (string valore in listaValori)
                {
                    string[] valTemp = valore.Split('=');
                    if (valTemp != null && valTemp.Length > 0)
                    {
                        valoriTemplate.Add(valTemp[0], valTemp[1]);
                    }
                }
            }


            if (valoriTemplate != null && result != null && result.ELENCO_OGGETTI != null && result.ELENCO_OGGETTI.Count > 0)
            {
                //List<String> listaCampiProfilatiCompilati;

                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in result.ELENCO_OGGETTI)
                {
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom tempObjCustom = oggettoCustom;
                    if (valoriTemplate.ContainsKey(oggettoCustom.DESCRIZIONE))
                    {
                        if (!CompileProfilationField(tempObjCustom, valoriTemplate[oggettoCustom.DESCRIZIONE], infoUtenteProprietario, role, rf, infoUtenteProprietario.idAmministrazione, register, ref errori, DictionaryIdRuoli))
                        {
                            inserisci = false;
                            errori.Add("Errore nel campo  " + oggettoCustom.DESCRIZIONE + " nella tipologia di documento");
                        }

                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(oggettoCustom.CAMPO_OBBLIGATORIO) && oggettoCustom.CAMPO_OBBLIGATORIO.ToUpper().Equals("SI"))
                        {
                            inserisci = false;
                            errori.Add("Valore " + oggettoCustom.DESCRIZIONE + " obbligatorio per la tipologia di documento");
                        }
                    }
                }
            }

            return result;
        }

        public static bool CompileProfilationField(DocsPaVO.ProfilazioneDinamica.OggettoCustom customObject, String values, InfoUtente userInfo, Ruolo role, Registro rf, string administrationId, Registro regist, ref List<string> errori, Dictionary<string, ArrayList> DictionaryIdRuoli)
        {
            bool boolresult = true;

            Registro registry;
            DocsPaVO.ProfilazioneDinamica.ValoreOggetto objectValue;

            string[] fieldValues;
            string RFCode = string.Empty;
            if (rf != null && !string.IsNullOrEmpty(rf.codRegistro))
            {
                RFCode = rf.codRegistro;
            }
            string registryCode = string.Empty;
            if (regist != null && !string.IsNullOrEmpty(regist.codRegistro))
            {
                registryCode = regist.codRegistro;
            }
            // ...a seconda del tipo di oggetto bisogna intraprendere
            // operazioni diverse per quanto riguarda l'assegnazione dei valori
            switch (customObject.TIPO.DESCRIZIONE_TIPO.ToUpper())
            {
                case "CASELLADISELEZIONE":

                    fieldValues = values.Split(';');

                    foreach (string selectedValue in fieldValues)
                    {
                        objectValue = ((DocsPaVO.ProfilazioneDinamica.ValoreOggetto[])customObject.ELENCO_VALORI.ToArray(typeof(DocsPaVO.ProfilazioneDinamica.ValoreOggetto))).Where(
                            e => e.VALORE.ToUpper().Equals(selectedValue.ToUpper())).FirstOrDefault();

                        // Se il valore non è stato reperito correttamente, viene lanciata una eccezione
                        if (objectValue == null)
                            throw new Exception(String.Format("Valore '{0}' non valido", selectedValue));

                        customObject.VALORI_SELEZIONATI[customObject.ELENCO_VALORI.IndexOf(objectValue)] = selectedValue;
                    }
                    break;

                case "CORRISPONDENTE":
                    // Se il ruolo possiede i diritti di modifica sul campo...
                    Corrispondente corr = FindCorrispondente(values, customObject, userInfo, role, rf, regist, administrationId, false, DictionaryIdRuoli);
                    customObject.VALORE_DATABASE = corr.systemId;
                    break;

                case "CONTATORE":
                case "CONTATORESOTTOCONTATORE":
                    try
                    {
                        // Reperimento dei dati sul registro
                        registry = BusinessLogic.Utenti.RegistriManager.getRegistroByCodAOO(
                            values.ToUpper(), administrationId);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            String.Format("Errore durante il reperimento delle informazioni sul Registro/RF {0}",
                                values));
                    }

                    switch (customObject.TIPO_CONTATORE.ToUpper())
                    {
                        case "A":   // Contatore di AOO
                            customObject.ID_AOO_RF = registry.systemId;
                            break;
                        case "R":   // Contatore di RF
                            // Se il registro non è un registro di RF, eccezione
                            if (!(registry.chaRF == "1"))
                                throw new Exception(String.Format(
                                    "Il registro {0} non è un RF",
                                    values));

                            // Impostazione dell'id registro
                            customObject.ID_AOO_RF = registry.systemId;

                            break;
                    }

                    // Se il contatore è abilitato allo scatto differito...
                    if (customObject.CONTA_DOPO == "0")
                        // Se il ruolo ha diritti di modifica sul contatore...
                        // Il contatore deve scattare
                        customObject.CONTATORE_DA_FAR_SCATTARE = true;

                    break;
                case "LINK":
                    fieldValues = values.Split(';');

                    if (fieldValues.Length < 2) throw new Exception("Per costruire un oggetto link sono necessari due valori");
                    if ("INTERNO".Equals(customObject.TIPO_LINK))
                    {
                        if ("DOCUMENTO".Equals(customObject.TIPO_OBJ_LINK))
                        {
                            DocsPaVO.documento.InfoDocumento infoDoc = null;
                            try
                            {
                                infoDoc = DocManager.GetInfoDocumento(userInfo, fieldValues[1], null, true);
                            }
                            catch (Exception e)
                            {
                                throw new Exception(String.Format("Errore nel reperimento del documento con id {0}", fieldValues[1]));
                            }
                            if (infoDoc == null) throw new Exception(String.Format("Il documento con id {0} non è presente", fieldValues[1]));
                            string errorMessage = "";
                            int result = DocManager.VerificaACL("D", infoDoc.idProfile, userInfo, out errorMessage);
                            if (result != 2)
                            {
                                throw new Exception(String.Format("Non si possiedono i diritti per reperire il documento con id {0}", fieldValues[1]));
                            }
                        }
                        else
                        {
                            DocsPaVO.fascicolazione.Fascicolo fasc = null;
                            try
                            {
                                // fasc = FascicoloManager.getFascicoloById(fieldValues[1], userInfo);
                                fasc = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloDaCodice(userInfo, fieldValues[1], null, false, true);
                            }
                            catch (Exception e)
                            {
                                throw new Exception(String.Format("Errore nel reperimento del fascicolo con id {0}", fieldValues[1]));
                            }
                            if (fasc == null) throw new Exception(String.Format("Il fascicolo con id {0} non è presente", fieldValues[1]));
                            string errorMessage = "";
                            int result = DocManager.VerificaACL("F", fasc.systemID, userInfo, out errorMessage);
                            if (result != 2)
                            {
                                throw new Exception(String.Format("Non si possiedono i diritti per reperire il fascicolo con id {0}", fieldValues[1]));
                            }
                        }
                    }
                    customObject.VALORE_DATABASE = values + "||||" + fieldValues[1];
                    break;
                case "OGGETTOESTERNO":
                    fieldValues = values.Split(';');
                    if (fieldValues.Length < 2) throw new Exception("Per costruire un oggetto esterno sono necessari due valori");
                    customObject.MANUAL_INSERT = true;
                    customObject.CODICE_DB = fieldValues[0];
                    customObject.VALORE_DATABASE = fieldValues[1];
                    break;
                default:
                    // In tutti gli altri casi il valore è uno solo
                    // Se il ruolo ha diritti di modofica, viene impostato
                    // il valore altrimenti viene inserito un messaggio nella
                    // lista dei warning

                    customObject.VALORE_DATABASE = values;

                    break;

            }

            // Restituzione della lista delle eventuali segnalazioni
            return boolresult;
        }

        private static Corrispondente FindCorrispondente(string codice, DocsPaVO.ProfilazioneDinamica.OggettoCustom customObject, InfoUtente userInfo, Ruolo role, Registro RF, Registro registry, string administrationId, bool isEnabledSmistamento, Dictionary<string, ArrayList> DictionaryIdRuoli)
        {
            string rfSyd = string.Empty;
            string registrySyd = string.Empty;
            DocsPaVO.addressbook.TipoUtente userType;

            Corrispondente corrispondenteResult = new Corrispondente();

            if (RF != null && registry != null)
            {

                rfSyd = RF.systemId;
                registrySyd = registry.systemId;

                // Individuazione del tipo di utente da ricercare
                switch (customObject.TIPO_RICERCA_CORR.ToUpper())
                {
                    case "INTERNI":
                        userType = DocsPaVO.addressbook.TipoUtente.INTERNO;

                        break;

                    case "ESTERNI":
                        userType = DocsPaVO.addressbook.TipoUtente.ESTERNO;

                        break;

                    default:
                        userType = DocsPaVO.addressbook.TipoUtente.GLOBALE;

                        break;
                }

                // Impostazione del corrispondente
                corrispondenteResult = GetCorrispondenteByCode(
                    ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST,
                    codice,
                    role,
                    userInfo,
                    registrySyd,
                    rfSyd,
                    isEnabledSmistamento,
                    userType, DictionaryIdRuoli);

            }

            return corrispondenteResult;
        }

        public static Corrispondente GetCorrispondenteByCode(
            ParametriRicercaRubrica.CallType callType,
            string corrCode,
            Ruolo role,
            InfoUtente userInfo,
            string registrySyd,
            string rfSyd,
            bool isEnabledSmistamento,
            TipoUtente userTypeForProject, Dictionary<string, ArrayList> DictionaryIdRuoli)
        {
            #region Dichiarazione Variabili

            // L'oggetto utilizzato per memorizzare i parametri di ricerca
            ParametriRicercaRubrica searchParameters;

            // L'oggetto per memorizzare le impostazioni sullo smistamento
            SmistamentoRubrica smistamentoRubrica;

            // L'oggetto per l'effettuazione delle ricerche nella rubrica
            BusinessLogic.Rubrica.DPA3_RubricaSearchAgent corrSearcher;

            // La lista degli elementi resttiuiti dalla ricerca
            ArrayList corrList;

            // Il corrispondente da restituire
            Corrispondente toReturn = null;

            #endregion

            #region Impostazione parametri di ricerca

            // Creazione oggetto per la memorizzazione dei parametri di ricerca
            searchParameters = new ParametriRicercaRubrica();

            // Impostazione del call type
            searchParameters.calltype = callType;

            // Impostazione del codice da ricercare
            searchParameters.codice = corrCode;

            // Impostazione del flag per la ricerca del codice esatta
            searchParameters.queryCodiceEsatta = true;

            // Creazione del caller
            searchParameters.caller = new ParametriRicercaRubrica.CallerIdentity();

            // Impostazione del calltype
            searchParameters.caller.IdRuolo = role.systemId;

            // Impostazione dell'id utente
            searchParameters.caller.IdUtente = userInfo.idPeople;

            //

            string retValue = "";

            foreach (Registro item in DictionaryIdRuoli[role.systemId])
            {
                if (item != null)
                {
                    retValue += " " + item.systemId + ",";
                }
            }
            if (retValue.EndsWith(","))
            {
                retValue = retValue.Remove(retValue.LastIndexOf(","));
            }

            // Impostazione dell'id registro
            searchParameters.caller.IdRegistro = registrySyd;

            // Impostazione del filtro registro per la ricerca
            //searchParameters.caller.filtroRegistroPerRicerca = registrySyd;
            searchParameters.caller.filtroRegistroPerRicerca = retValue;

            // La ricerca va effettuata su Uffici, Utenti, Ruoli, RF
            searchParameters.doUo = true;
            searchParameters.doUtenti = true;
            searchParameters.doRuoli = true;
            searchParameters.doRF = true;
            bool abilitazioneRubricaComune = BusinessLogic.RubricaComune.Configurazioni.GetConfigurazioni(userInfo).GestioneAbilitata;
            searchParameters.doRubricaComune = abilitazioneRubricaComune;
            #endregion

            #region Impostazione parametri per smistamento

            // Creazione oggetto per parametri smistamento
            smistamentoRubrica = new SmistamentoRubrica();

            // Abilitazione smistamento
            smistamentoRubrica.smistamento = isEnabledSmistamento ? "1" : "0";

            // Impostazione calltype
            smistamentoRubrica.calltype = callType;

            // Impostazione informazioni sull'utente
            smistamentoRubrica.infoUt = userInfo;

            // Impostazione ruolo
            smistamentoRubrica.ruoloProt = role;

            // Impostazione dell'id del registro
            smistamentoRubrica.idRegistro = registrySyd;

            #endregion

            #region Impostazione parametri dipendenti dal contesto

            // Impostazione parametri dipendenti dal contesto
            switch (callType)
            {
                case ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_IN:
                    if (!String.IsNullOrEmpty(rfSyd))
                        searchParameters.caller.filtroRegistroPerRicerca += ", " + rfSyd;
                    searchParameters.doRubricaComune = abilitazioneRubricaComune;
                    searchParameters.doRubricaComune = true;
                    smistamentoRubrica.daFiltrareSmistamento = "0";
                    searchParameters.tipoIE = TipoUtente.GLOBALE;

                    break;

                case ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT_MITT:
                    smistamentoRubrica.daFiltrareSmistamento = "0";
                    searchParameters.tipoIE = TipoUtente.INTERNO;

                    break;

                case ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT:
                    searchParameters.doListe = true;
                    searchParameters.doRubricaComune = abilitazioneRubricaComune;
                    if (!String.IsNullOrEmpty(rfSyd))
                        searchParameters.caller.filtroRegistroPerRicerca += ", " + rfSyd;

                    smistamentoRubrica.daFiltrareSmistamento = "1";
                    searchParameters.tipoIE = TipoUtente.GLOBALE;

                    break;

                case ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_MITT:
                    smistamentoRubrica.daFiltrareSmistamento = "0";
                    searchParameters.tipoIE = TipoUtente.INTERNO;

                    break;

                case ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST:
                    searchParameters.doListe = true;
                    smistamentoRubrica.daFiltrareSmistamento = "1";
                    searchParameters.tipoIE = TipoUtente.INTERNO;

                    break;

                case ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST:
                    if (!String.IsNullOrEmpty(rfSyd))
                        searchParameters.caller.filtroRegistroPerRicerca += ", " + rfSyd;
                    searchParameters.doRubricaComune = true;
                    searchParameters.tipoIE = userTypeForProject;
                    smistamentoRubrica.daFiltrareSmistamento = "0";

                    break;

            }

            #endregion

            #region Esecuzione Ricerca

            // Creazione oggetto per la ricerca
            corrSearcher = new BusinessLogic.Rubrica.DPA3_RubricaSearchAgent(userInfo);

            // Esecuzione della ricerca
            corrList = corrSearcher.Search(searchParameters, smistamentoRubrica);

            #endregion

            #region Gestione risultato

            // Se non sono stati restituiti corrispondenti eccezione
            if (corrList == null || corrList.Count == 0)
                throw new Exception(String.Format(
                    "Nessun corrispondente rilevato con il codice {0}",
                    corrCode));

            // Se sono stati restituiti più corrispondenti, ambiguità
            if (corrList.Count > 1)
                throw new Exception(String.Format(
                    "La ricerca del corrispondente con codice {0} ha restituito {1} risultati. Provare a restringere il campo di ricerca specificando un RF.",
                    corrCode, corrList.Count));

            #endregion

            // Reperimento dei dati sul corrispondente
            try
            {
                ElementoRubrica temp = (ElementoRubrica)corrList[0];
                if (temp.isRubricaComune)
                {
                    Corrispondente tempCorr = BusinessLogic.RubricaComune.RubricaServices.UpdateCorrispondente(userInfo, temp.codice);
                    toReturn = tempCorr;
                }
                else
                {
                    toReturn = UserManager.getCorrispondenteBySystemID(temp.systemId);
                }
            }
            catch (Exception e)
            {
                throw new ImportException(
                    String.Format(
                        "Errore durante il reperimento dei dati sul corrispondente con codice '{0}'",
                        corrCode));
            }

            // Restituzione del corrispondente
            return toReturn;

        }

        private static void SetUserId(InfoUtente infoUtente)
        {
            if (infoUtente != null) SetUserId(infoUtente.userId);
        }

        private static void SetUserId(string userId)
        {
            if (!string.IsNullOrEmpty(userId)) LogicalThreadContext.Properties["userId"] = userId.ToUpper();
        }

        private static void ControlloADL(ItemReportPregressi item, DocsPaVO.utente.InfoUtente infoUtenteProprietario, DocsPaVO.utente.Ruolo role, DocsPaVO.documento.SchedaDocumento schedaDoc)
        {
            if (!string.IsNullOrEmpty(item.adl) && item.adl.ToUpper().Equals("SI"))
            {
                string id_corr_globali = infoUtenteProprietario.idCorrGlobali;

                if (role != null)
                {
                    infoUtenteProprietario.idCorrGlobali = role.systemId;
                }

                //Inserisci in area di lavoro
                BusinessLogic.Documenti.areaLavoroManager.execAddLavoroMethod(schedaDoc.docNumber, schedaDoc.tipoProto, item.idRegistro, infoUtenteProprietario, null);

                //Ripristino il valore di idCorrGlobali
                infoUtenteProprietario.idCorrGlobali = id_corr_globali;
            }
        }

        private static void ControlloTrasmissioni(ItemReportPregressi item, DocsPaVO.utente.InfoUtente infoUtenteProprietario, InfoUtente infoUtente, DocsPaVO.utente.Ruolo role, DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaDB.Query_DocsPAWS.ImportPregressi importazione, ref List<string> errori, ref bool esitoTrasmissioni, ref Dictionary<string, ArrayList> DictionaryIdRuoli)
        {
            esitoTrasmissioni = true;
            ArrayList listaRegistri = new ArrayList();
            ArrayList modelliDiTrasm = new ArrayList();

            if (!string.IsNullOrEmpty(item.cod_modello_trasm))
            {
                bool result = false;

                DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione model = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();

                string idModel = item.cod_modello_trasm.Substring(3);

                if (!string.IsNullOrEmpty(idModel))
                {
                    Registro[] arrRegRuolo = null;

                    #region REGISTRI DI QUEL RUOLO
                    //Verifico che il Dictionary contenga IdRuolo
                    if (DictionaryIdRuoli.ContainsKey(role.systemId))
                    {
                        //Prendo la lista registri associata a quel Ruolo
                        listaRegistri = DictionaryIdRuoli[role.systemId];
                        if (listaRegistri != null && listaRegistri.Count > 0)
                        {
                            arrRegRuolo = new Registro[listaRegistri.Count];
                            int i = 0;
                            foreach (Registro reg in listaRegistri)
                            {
                                arrRegRuolo[i] = reg;
                                i++;
                            }
                            //ArrayList castato ad un array di registri
                            //arrRegRuolo = (Registro[])listaRegistri.ToArray();
                        }
                    }
                    else
                    {
                        //Dictionary non contiene id ruolo
                        listaRegistri = BusinessLogic.Utenti.RegistriManager.getListaRegistriRfRuolo(role.systemId, string.Empty, string.Empty);
                        if (listaRegistri != null && listaRegistri.Count > 0)
                        {
                            DictionaryIdRuoli.Add(role.systemId, listaRegistri);
                            arrRegRuolo = new Registro[listaRegistri.Count];
                            int i = 0;
                            foreach (Registro reg in listaRegistri)
                            {
                                arrRegRuolo[i] = reg;
                                i++;
                            }
                            //ArrayList castato ad un array di registri
                            //arrRegRuolo = (Registro[])listaRegistri.ToArray();
                        }
                    }
                    #endregion

                    modelliDiTrasm = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelliPerTrasmLite(infoUtenteProprietario.idAmministrazione, arrRegRuolo, infoUtenteProprietario.idPeople, infoUtenteProprietario.idCorrGlobali, string.Empty, string.Empty, string.Empty, "D", string.Empty, infoUtenteProprietario.idGruppo, false, string.Empty);

                    if (modelliDiTrasm != null && modelliDiTrasm.Count > 0)
                    {
                        foreach (DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod in modelliDiTrasm)
                        {
                            if (mod.SYSTEM_ID.ToString().Equals(idModel))
                            {
                                model = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelloByIDSoloConNotifica(infoUtenteProprietario.idAmministrazione, idModel);
                                break;
                            }
                        }
                    }

                    //model = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelloByIDSoloConNotifica(infoUtente.idAmministrazione, idModel);

                    if (model != null && !string.IsNullOrEmpty(model.CODICE))
                    {
                        result = BusinessLogic.Trasmissioni.TrasmManager.TransmissionExecuteDocTransmFromModelCodeSoloConNotifica(infoUtenteProprietario, string.Empty, schedaDoc, item.cod_modello_trasm, role, out model);
                        if (!result)
                        {
                            item.esito = "W";
                            //13/07/2012 Andrea - evita che vengano sovrascritti altri warning precedenti
                            item.errore = concatenaMessaggio(item, "La trasmissione da modello ha avuto esito negativo");
                            esitoTrasmissioni = false;
                            //Portare Fuori se trasm is false
                            //importazione.UpdateItemReportPregressi(item, item.systemId);

                        }
                    }
                    else
                    {
                        item.esito = "W";
                        //13/07/2012 Andrea - evita che vengano sovrascritti altri warning precedenti
                        item.errore = concatenaMessaggio(item, "Codice del modello di trasmissione non trovato");
                        esitoTrasmissioni = false;
                        //importazione.UpdateItemReportPregressi(item, item.systemId);
                    }
                }
            }
        }

        private static List<ItemReportPregressi> getItemInErroreProgressioneNumeriProfileNonProtocollati(List<ItemReportPregressi> listaItem)
        {
            List<string> parametriDellaFunzioneDB = new List<string>();
            List<string> resultList = new List<string>();
            List<ItemReportPregressi> returnList = new List<ItemReportPregressi>();

            foreach (ItemReportPregressi corrente in listaItem)
            {
                if (!string.IsNullOrEmpty(corrente.idNumProtocolloExcel) && !string.IsNullOrEmpty(corrente.data))
                {
                    string tempstring = "(" + corrente.idNumProtocolloExcel.Trim() + "," + DocsPaDbManagement.Functions.Functions.ToDate(corrente.data) + ")";
                    parametriDellaFunzioneDB.Add(tempstring);
                }
            }

            if (parametriDellaFunzioneDB.Count > 0)
                resultList = BusinessLogic.Import.ImportPregressi.PregressiManager.GetInvalidOldNumber(parametriDellaFunzioneDB);

            foreach (string idNumProto in resultList)
            {
                ItemReportPregressi tempProto = (from proto in listaItem where proto.idNumProtocolloExcel == idNumProto select proto).FirstOrDefault();
                if (tempProto != null)
                {
                    tempProto.esito = "E";
                    //tempProto.errore = concatenaMessaggio(tempProto, "Progressione numerico temporale del documento non protocollato in conflitto con documenti protocollati esistenti.");
                    tempProto.errore = concatenaMessaggio(tempProto, "Progressione numerico temporale del documento non protocollato in conflitto con documenti non protocollati esistenti.");
                    returnList.Add(tempProto);
                }
            }

            return returnList;
        }

        private static void CheckIdRuoloCreatoreAssociatoRegistroPregresso(DocsPaVO.utente.Ruolo role, DocsPaVO.utente.Registro register, ref bool inserisci, ref List<string> errori, ref Dictionary<string, ArrayList> DictionaryIdRuoli)
        {
             ArrayList listaRegistri = new ArrayList();
             //Verifico che il Dictionary contenga IdRuolo
             if(DictionaryIdRuoli.ContainsKey(role.systemId))
             {
                 //Prendo la lista registri associata a quel Ruolo
                 listaRegistri = DictionaryIdRuoli[role.systemId];
                 if (listaRegistri != null && listaRegistri.Count > 0)
                 {
                     bool trovato = false;
                     foreach (DocsPaVO.utente.Registro reg in listaRegistri)
                     {
                         if (reg.codRegistro.Equals(register.codRegistro))
                         {
                             trovato = true;
                             break;
                         }
                     }

                     if (!trovato)
                     {
                         inserisci = false;
                         errori.Add("Il Ruolo Creatore non è associato al Registro di Pregresso");
                     }
                 }
                 //Verifico se la listaRegistri del dictionary per quel idRole contiene il registro in questione.
                 //if (DictionaryIdRuoli[role.systemId].Contains(register))
                 //{
                 //    inserisci = true;
                 //}
                 //else
                 //{
                 //    inserisci = false;
                 //    errori.Add("Il Ruolo Creatore non è associato al Registro di Pregresso");
                 //}
             }
             else
             {
                 //Dictionary non contiene id ruolo
                 //listaRegistri = BusinessLogic.Utenti.RegistriManager.getRegistriRuolo(role.systemId);
                 listaRegistri = BusinessLogic.Utenti.RegistriManager.getListaRegistriRfRuolo(role.systemId, string.Empty, string.Empty);
                 if (listaRegistri != null && listaRegistri.Count > 0)
                 {
                     DictionaryIdRuoli.Add(role.systemId, listaRegistri);
                     bool trovato = false;
                     foreach (DocsPaVO.utente.Registro reg in listaRegistri) 
                     {
                         if (reg.codRegistro.Equals(register.codRegistro)) 
                         {
                             trovato = true;
                             break;
                         }
                     }

                     if (!trovato)
                     {
                         inserisci = false;
                         errori.Add("Il Ruolo Creatore non è associato al Registro di Pregresso");
                     }
                     //if (listaRegistri.Contains(register))
                     //{
                     //    inserisci = true;
                     //}
                     //else
                     //{
                     //    inserisci = false;
                     //    errori.Add("Il Ruolo Creatore non è associato al Registro di Pregresso");
                     //}
                 }
             }
         }

        //private static void RuoloCreatoreListaRegistriRF(DocsPaVO.utente.Ruolo role, DocsPaVO.utente.Registro register, ref List<string> errori, ref Dictionary<string, ArrayList> DictionaryIdRuoli)
        //{
        //    ArrayList listaRegistri = new ArrayList();
        //    //Verifico che il Dictionary contenga IdRuolo
        //    if (!DictionaryIdRuoli.ContainsKey(role.systemId))
        //    {
        //        ////Prendo la lista registri associata a quel Ruolo
        //        //listaRegistri = DictionaryIdRuoli[role.systemId];
        //        listaRegistri = BusinessLogic.Utenti.RegistriManager.getListaRegistriRfRuolo(role.systemId, string.Empty, string.Empty);
        //        if (listaRegistri != null && listaRegistri.Count > 0)
        //        {
        //            DictionaryIdRuoli.Add(role.systemId, listaRegistri);
        //        }
        //    }
        //    //else
        //    //{
        //    //    listaRegistri = BusinessLogic.Utenti.RegistriManager.getListaRegistriRfRuolo(role.systemId, string.Empty, string.Empty);
        //    //    if (listaRegistri != null && listaRegistri.Count > 0)
        //    //    {
        //    //        DictionaryIdRuoli.Add(role.systemId, listaRegistri);
        //    //    }
        //    //}
        //}

        protected static List<string> GetProjectsForClassification(ItemReportPregressi rowData, InfoUtente userInfo, Ruolo role, string administrationSyd, string registrySyd, string rfSyd, string idTitolario, out List<String> problems, bool isEnabledSmistamento, ref Dictionary<string, DocsPaVO.fascicolazione.Fascicolo> DictionaryCodFasc_Fascicolo)
        {
            #region Dichiarazione variabili

            // La lista con gli id dei fascicoli
            List<string> toReturn = new List<string>();

            // La lista degli eventuali problemi emersi durante la ricerca
            // dei fascioli
            List<string> projectProblems = new List<string>();

            // La lista di fascicoli restituira da sistema di ricerca
            DocsPaVO.fascicolazione.Fascicolo[] projects = null;

            // Il fascicolo trovato
            DocsPaVO.fascicolazione.Fascicolo project = null;

            // Il registro
            Registro registry = null;

            // I filtri per la ricerca di un fascicolo
            DocsPaVO.filtri.FiltroRicerca[] filters;

            // Numero massimo di elementi da restituire e
            // numero di elementi
            int totRec, nRec;

            // La stringa con i codici dei fascicoli trovati
            System.Text.StringBuilder prjCodes;

            // Lista dei system id dei fascicoli restituiti dalla ricerca
            // Non viene utilizzata ma è richiesta dalla funzione
            List<DocsPaVO.ricerche.SearchResultInfo> idProjectList;

            #endregion

            try
            {
                // Caricamento delle informazioni sul registro
                registry = RegistriManager.getRegistro(registrySyd);
            }
            catch (Exception e)
            {
                projectProblems.Add(String.Format(
                    "Impossibile recuperare le informazioni sul registro {0}",
                    rowData.codRegistro));

            }

            // Se esistono dei codici fascicoli e registry è valorizzato...
            if (rowData.ProjectCodes != null && registry != null)
            {
                // Per ogni codice, si effettua la ricerca del fascicolo
                foreach (string cod in rowData.ProjectCodes)
                {
                    if (DictionaryCodFasc_Fascicolo.ContainsKey(cod))
                    {
                        project = DictionaryCodFasc_Fascicolo[cod];
                        toReturn.Add(project.systemID);
                    }
                    else
                    {
                        try
                        {
                            project = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloDaCodice2(
                                administrationSyd,
                                role.idGruppo,
                                userInfo.idPeople,
                                cod,
                                registry,
                                true,
                                true,
                                idTitolario);

                            if (project != null)
                            {
                                DictionaryCodFasc_Fascicolo.Add(cod, project);
                            }

                            // Se il fascicolo è stato recuperato con successo,
                            // si inserisce il suo system id nella lista dei fascicoli
                            toReturn.Add(project.systemID);
                        }
                        catch (Exception e)
                        {
                            // Viene aggiunta una riga alla lista dei problemi
                            projectProblems.Add(String.Format(
                                "Impossibile recuperare le informazioni sul fascicolo {0}",
                                cod));
                        }
                    }
                }
            }
            else
            {
                // Altrimenti si procede con la ricerca del fascicolo i cui
                // dati sono riportati nei campi dedicati al fascicolo
                // 1. Calcolo dei filtri di ricerca
                filters = GetFilters(
                    rowData,
                    registrySyd,
                    rfSyd,
                    idTitolario,
                    administrationSyd,
                    role,
                    userInfo,
                    isEnabledSmistamento);

                try
                {
                    // 2. Ricerca dei fascicoli
                    projects = (DocsPaVO.fascicolazione.Fascicolo[])BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliPaging(
                        userInfo,
                        null,
                        registry,
                        filters,
                        false,
                        true,
                        false,
                        out totRec,
                        out nRec,
                        1,
                        2,
                        false,
                        out idProjectList, null, string.Empty).ToArray(typeof(DocsPaVO.fascicolazione.Fascicolo));

                    // 3. Se sono stati restituiti più fascicoli -> ambiguità -> errore
                    if (projects.Length > 1)
                    {
                        // Creazione della stringa contente i codici dei fascicoli trovati
                        prjCodes = new System.Text.StringBuilder();

                        // Aggiunta dei codici dei fascicoli
                        foreach (DocsPaVO.fascicolazione.Fascicolo prj in projects)
                            prjCodes.AppendFormat("{0}, ", prj.codice);

                        // Rimozione dell'ultima virgola
                        prjCodes.Remove(prjCodes.Length - 2, 2);

                        // Viene aggiunta una riga alla lista dei problemi
                        //projectProblems.Add(String.Format(
                        //    "I parametri di ricerca specificati nei campi dedicati alla tipologia fascicolo hanno restituito i seguenti {0} fascicoli: {1}. Provare a restringere il campo di ricerca specificando più parametri o parametri più specifici.",
                        //        projects.Length, prjCodes));
                        projectProblems.Add(String.Format(
                            "Non è stato possibile individuare un fascicolo univoco. I parametri di ricerca specificati hanno restituito i seguenti {0} fascicoli: {1}.",
                                projects.Length, prjCodes));
                    }
                    else
                        // Altrimenti viene aggiunto il system id del fascicolo trovato
                        // alla lista dei system id fascicoli da restituire
                        toReturn.Add(projects[0].systemID);

                }
                catch (Exception e)
                {
                    // Viene aggiunta una riga alla lista dei problemi
                    projectProblems.Add(
                        "Errore durante il reperimento dei dati sul fascicolo specificato.");

                }

            }

            // Impostazione della lista dei problemi
            problems = projectProblems;

            // Restituzione della lista con i system id dei fascicoli in
            // cui fascicolare il documento
            return toReturn;

        }

        private static DocsPaVO.filtri.FiltroRicerca[] GetFilters(ItemReportPregressi rowData, string registrySyd, string rfSyd, string idTitolario, string administrationSyd, Ruolo role, InfoUtente userInfo, bool isSmistamentoEnabled)
        {
            #region Dichiarazione variabili

            // La lista dei filtri da restituire
            List<DocsPaVO.filtri.FiltroRicerca> toReturn;

            // Filtro temporaneo
            DocsPaVO.filtri.FiltroRicerca tempFilter;

            // La lista dei template
            DocsPaVO.ProfilazioneDinamica.Templates[] templates = null;

            // Il template da utilizzare per la profilazione
            DocsPaVO.ProfilazioneDinamica.Templates template = null;

            // I campi
            string[] values;

            // Il tipo di utente da ricercare
            TipoUtente userType;

            #endregion

            // Creazione della lista dei filtri
            toReturn = new List<DocsPaVO.filtri.FiltroRicerca>();

            #region Filtro Codice Fascicolo

            // Se è valorizzato il codice fascicolo, si procede alla creazione
            // di un filtro per esso
            if (rowData.ProjectCodes != null && !String.IsNullOrEmpty(rowData.ProjectCodes[0]))
            {
                // Creazione di un filtro temporaneo
                tempFilter = new DocsPaVO.filtri.FiltroRicerca();

                // Impostazione del nome del campo
                tempFilter.argomento = "NUMERO_FASCICOLO";

                // Impostazione del valore
                tempFilter.valore = rowData.ProjectCodes[0];

                // Aggiunta del filtro alla lista dei filtri
                toReturn.Add(tempFilter);

            }

            #endregion

            #region Filtro Descrizione Fascicolo

            // Se è valorizzata la descrizione del fascicolo,
            // si procede alla creazione di un filtro per essa
            if (!String.IsNullOrEmpty(rowData.ProjectDescription))
            {
                // Creazione di un filtro temporaneo
                tempFilter = new DocsPaVO.filtri.FiltroRicerca();

                // Impostazione del nome del campo
                tempFilter.argomento = "TITOLO";

                // Impostazione del valore
                tempFilter.valore = rowData.ProjectDescription;

                // Aggiunta del filtro alla lista dei filtri
                toReturn.Add(tempFilter);
            }

            #endregion

            #region Filtro Descrizione Sottofascicolo

            // Se è valorizzata la descrizione del sottofascicolo,
            // si procede alla creazione di un filtro per essa
            if (!String.IsNullOrEmpty(rowData.FolderDescrition))
            {
                // Creazione di un filtro temporaneo
                tempFilter = new DocsPaVO.filtri.FiltroRicerca();

                // Impostazione del nome del campo
                tempFilter.argomento = "SOTTOFASCICOLO";

                // Impostazione del valore
                tempFilter.valore = rowData.FolderDescrition;

                // Aggiunta del filtro alla lista dei filtri
                toReturn.Add(tempFilter);
            }

            #endregion

            #region Filtro Titolario

            // Se il parametro idTitolario è valorizzato,
            // viene creato un filtro per esso
            if (!String.IsNullOrEmpty(idTitolario))
            {
                // Creazione di un filtro temporaneo
                tempFilter = new DocsPaVO.filtri.FiltroRicerca();

                // Impostazione del nome del campo
                tempFilter.argomento = "ID_TITOLARIO";

                // Impostazione del valore
                tempFilter.valore = idTitolario;

                // Aggiunta del filtro alla lista dei filtri
                toReturn.Add(tempFilter);
            }

            #endregion

            #region Filtro Codice Nodo

            // Se è valorizzato il codice nodo,
            // si procede alla creazione di un filtro per esso
            if (!String.IsNullOrEmpty(rowData.NodeCode))
            {
                // Creazione di un filtro temporaneo
                tempFilter = new DocsPaVO.filtri.FiltroRicerca();

                // Impostazione del nome del campo
                tempFilter.argomento = "CODICE_CLASSIFICA";

                // Impostazione del valore
                tempFilter.valore = rowData.NodeCode;

                // Aggiunta del filtro alla lista dei filtri
                toReturn.Add(tempFilter);
            }

            #endregion

            #region Filtro Tipologia Fascicolo e Campi Profilati

            // Se è valorizzato il campo tipologia, vengono creati,
            // i filtri di ricerca per la profilazione
            //if (!String.IsNullOrEmpty(rowData.ProjectTipology))
            //{

            //    try
            //    {
            //        // Prelevamento della lista dei template creati per l'amministrazione
            //        templates = (DocsPaVO.ProfilazioneDinamica.Templates[])BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTipoFascFromRuolo(
            //            administrationSyd,
            //            role.idGruppo,
            //            "2").ToArray(typeof(DocsPaVO.ProfilazioneDinamica.Templates));

            //        // Prelevamento del template di interesse
            //        template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascById(templates.Where(
            //            e => e.DESCRIZIONE.ToUpper() == rowData.ProjectTipology).
            //            FirstOrDefault().SYSTEM_ID.ToString());
            //    }
            //    catch (Exception e)
            //    {
            //    }

            //    // Se è stato rilevato un template, si procede con la
            //    // compilazione dei campi relativi alla profilazione
            //    if (template != null)
            //    {
            //        // Il primo campo riporta l'id del template
            //        // Creazione di un filtro temporaneo
            //        tempFilter = new DocsPaVO.filtri.FiltroRicerca();

            //        // Impostazione della tipologia fascicolo
            //        tempFilter.argomento = "TIPOLOGIA_FASCICOLO";

            //        // Impostazione del valore
            //        tempFilter.valore = template.SYSTEM_ID.ToString();

            //        // Aggiunta del filtro alla lista dei filtri
            //        toReturn.Add(tempFilter);

            //        // Aggiunta delle informazioni sul template
            //        tempFilter = new DocsPaVO.filtri.FiltroRicerca();

            //        // Impostazione dell'argomento
            //        tempFilter.argomento = "PROFILAZIONE_DINAMICA";

            //        // Impostazione del valore
            //        tempFilter.valore = " Profilazione Dinamica";

            //        // Impostazione del template
            //        tempFilter.template = template;

            //        // Per ogni oggetto custom...
            //        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom obj in template.ELENCO_OGGETTI)
            //        {
            //            // ...switch sul tipo di oggetto
            //            switch (obj.TIPO.DESCRIZIONE_TIPO.ToUpper())
            //            {
            //                case "CONTATORE":
            //                    try
            //                    {
            //                        // Prelevamento dei valori assegnati al campo
            //                        values = rowData.GetProjectProfilationField(obj.DESCRIZIONE);

            //                        // Nel caso di contatore bisogna impostare per prima cosa
            //                        // l'id del registro
            //                        obj.ID_AOO_RF = ImportUtils.GetRegistrySystemId(
            //                            values[0],
            //                            rowData.AdminCode);

            //                        // Se l'array dei valori contiene il secondo elemento, ...
            //                        if (values.Length > 1)
            //                            // ...viene impostato il valore minimo
            //                            obj.VALORE_DATABASE = values[1] + "@";

            //                        // Se l'array contiene il terzo campo...
            //                        if (values.Length > 2)
            //                            // ...viene impostato il valore massimo
            //                            obj.VALORE_DATABASE += values[2];

            //                    }
            //                    catch (Exception e)
            //                    { }

            //                    break;

            //                case "DATA":
            //                    // Prelevamento dei valori associati al campo
            //                    values = rowData.GetProjectProfilationField(obj.DESCRIZIONE);

            //                    // Se l'array è valorizzato...
            //                    if (values != null)
            //                    {
            //                        // ...se contiene il primo elemento...
            //                        if (values.Length > 0)
            //                            // ...il primo campo è il valore data minimo
            //                            obj.VALORE_DATABASE = values[0] + "@";

            //                        // ...se è presente anche il secondo elemento...
            //                        if (values.Length > 1)
            //                            // ...il secondo è il valore di data massima
            //                            obj.VALORE_DATABASE += values[1];

            //                    }

            //                    break;

            //                case "CORRISPONDENTE":
            //                    try
            //                    {
            //                        switch (obj.TIPO_RICERCA_CORR.ToUpper())
            //                        {
            //                            case "INTERNO":
            //                                userType = TipoUtente.INTERNO;
            //                                break;

            //                            case "ESTERNO":
            //                                userType = TipoUtente.ESTERNO;
            //                                break;

            //                            default:
            //                                userType = TipoUtente.GLOBALE;
            //                                break;

            //                        }

            //                        // Prelevamento del system id del corrispondente
            //                        obj.VALORE_DATABASE = ImportUtils.GetCorrispondenteByCode(
            //                            ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST,
            //                            rowData.GetProjectProfilationField(obj.DESCRIZIONE)[0],
            //                            role,
            //                            userInfo,
            //                            registrySyd,
            //                            rfSyd,
            //                            isSmistamentoEnabled,
            //                            userType).systemId;

            //                    }
            //                    catch (Exception e) { }

            //                    break;

            //                case "CASELLADISELEZIONE":
            //                    try
            //                    {
            //                        // In questo caso è possibile che siano selezionati
            //                        // più valori
            //                        obj.VALORI_SELEZIONATI = new ArrayList(
            //                            rowData.GetProjectProfilationField(obj.DESCRIZIONE));
            //                    }
            //                    catch (Exception e) { }

            //                    break;

            //                default:
            //                    try
            //                    {
            //                        // In tutti gli alti casi è ammesso un solo valore
            //                        obj.VALORE_DATABASE = rowData.GetProjectProfilationField(obj.DESCRIZIONE)[0];
            //                    }
            //                    catch (Exception e)
            //                    {
            //                    }

            //                    break;

            //            }

            //        }

            //        // Aggiunta del filtro alla lista dei filtri
            //        toReturn.Add(tempFilter);

            //    }

            //}

            #endregion

            // Restituzione della lista dei filtri
            return toReturn.ToArray<DocsPaVO.filtri.FiltroRicerca>();

        }

        protected static List<string> AddDocToProjects(InfoUtente userInfo, string idProfile, List<string> projectIds, bool isRapidClassificationRequired)
        {
            #region Dichiarazione variabili

            // La lista dei problemi riscontrati in fascicolazione
            List<string> toReturn;

            // Valore booleano utilizzato per valutare il risultato della fascicolazione
            bool prjResult = false;

            // L'eventuale messagio restituito dalla funzione di inserimento documento in fascicolo
            string message = String.Empty;

            // Booleano utilizzato per indicare se l'id in analisi è il primo della lista
            // In questo caso se è richiesta fascicolazione rapida, il primo fascicolo
            // viene considerato come fascicolazione rapida
            bool firstProject = true;

            #endregion

            // Creazione della lista dei problemi da restituire
            toReturn = new List<string>();

            // Per ogni id fascicolo contenuto della lista degli id dei fascicoli
            foreach (string projectId in projectIds)
            {
                try
                {
                    // Azzeramento del booleano
                    prjResult = false;

                    // Si procede con la fascicolazione
                    prjResult = BusinessLogic.Fascicoli.FascicoloManager.addDocFascicolo(
                        userInfo,
                        idProfile,
                        projectId,
                        firstProject ? true : false,
                        out message);

                    // Il prossimo elemento sicuramente non è il primo
                    firstProject = false;

                }
                catch (Exception e)
                {
                    // Aggiunta di un problema alla lista dei problemi
                    toReturn.Add("Si è verificato un errore durante la fascicolazione.");
                }

                // Se il risultato di fascicolazione è negativo,
                // viene aggiunto un messaggio di avviso alla lista dei problemi
                //if (!prjResult)
                //    toReturn.Add("Si è verificato un errore durante la fascicolazione.");

            }

            // Restituzione della lista dei problemi
            return toReturn;

        }
    }
}
