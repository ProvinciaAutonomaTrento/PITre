// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.ProspettiRiepilogativi;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Office2013.Word;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using ImportaRubricaRequest = Pi3.App.Legacy.WebApi.Application.Requests.ImportaRubrica;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ImportaRubrica
{
    public class ImportaRubricaHandler : IRequestHandler<ImportaRubricaRequest, ImportaRubricaResult>
    {
        #region Public Members



        public ImportaRubricaHandler(
            ILogger<ImportaRubricaHandler> logger,
            IPi3DbContext dbContext,
            ISpreadsheetService spreadsheetService,
            IConfiguration configuration,
            IMediator mediator,
            IWebMethodLoggerService webMethodLoggerService
            )
        {
                this._logger = logger;
                this._dbContext = dbContext;
                this._spreadsheetService = spreadsheetService;
                this._configuration = configuration;
                this._logPath = configuration.GetSection("LogImportazioniOptions:LogRootPath").Value ?? string.Empty;
                this._mediator = mediator;
                this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<ImportaRubricaResult> Handle(ImportaRubricaRequest request,CancellationToken cancellationToken)
        {
            ImportRes output = new();
            try
            {
                output = await this.ImportaRubrica(request.infoUtente,request.dati,request.flagListe);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception:ex,message:ex.Message);
            }
            return new(output.Outcome,output.CorrInseriti,output.CorrAggiornati,output.CorrRimossi,output.CorrNonInseriti,output.CorrNonAggiornati,output.CorrNonRimossi);
        }

        #endregion


        private async Task<ImportRes> ImportaRubrica(DocsPaVO.utente.InfoUtente infoUtente, byte[] dati, int flagListe)
        {
            bool result = true;
            string messaggio = string.Empty;
            int corrRimossi = 0;
            int corrAggiornati = 0;
            int corrNonAggiornati = 0;
            int corrInseriti = 0;
            int corrNonInseriti = 0;
            int corrNonRimossi = 0;
            List<string> log = new();
            string storicizza = string.Empty;
            string codRegistro = string.Empty;
            string codRegistroNuovo = string.Empty;
            string corrType = string.Empty;
            string codRubrica = string.Empty;
            string codAmm = string.Empty;
            string codAOO = string.Empty;
            string descrizione = string.Empty;
            string cognome = string.Empty;
            string nome = string.Empty;
            string indirizzo = string.Empty;
            string cap = string.Empty;
            string citta = string.Empty;
            string provincia = string.Empty;
            string nazione = string.Empty;
            string codiceFiscale = string.Empty;
            string partitaIva = string.Empty;
            string telefono1 = string.Empty;
            string telefono2 = string.Empty;
            string fax = string.Empty;
            string mail = string.Empty;
            string localita = string.Empty;
            string note = string.Empty;
            string canaleP = string.Empty;
            string[] mailCorri = new string[0];

            try 
            {
                string json = Encoding.UTF8.GetString(dati);
                List<DatiModificaCorr> datiModificaCorrispondenti = JsonConvert.DeserializeObject<List<DatiModificaCorr>>(json);

                log.Add(this.GetLogEntry(Resources.InImport));
                if (datiModificaCorrispondenti != null && datiModificaCorrispondenti.Count > 0)
                {
                    foreach (var datiCorr in datiModificaCorrispondenti)
                    {
                        storicizza = datiCorr.storicizza;
                        if (!string.IsNullOrEmpty(storicizza))
                        {
                            codRegistro = datiCorr.codice;
                            codRubrica = datiCorr.codRubrica;
                            codAmm = datiCorr.codiceAmm;
                            codAOO = datiCorr.codiceAoo;
                            corrType = datiCorr.tipoCorrispondente;
                            descrizione = datiCorr.descCorr ?? string.Empty;
                            cognome = datiCorr.cognome;
                            nome = datiCorr.nome;
                            indirizzo = datiCorr.indirizzo;
                            cap = datiCorr.cap;
                            citta = datiCorr.citta;
                            provincia = datiCorr.provincia;
                            nazione = datiCorr.nazione;
                            codiceFiscale = datiCorr.codFiscale;
                            partitaIva = datiCorr.partitaIva;
                            telefono1 = datiCorr.telefono;
                            telefono2 = datiCorr.telefono2;
                            fax = datiCorr.fax;
                            mail = datiCorr.email;
                            localita = datiCorr.localita;
                            note = datiCorr.note;
                            codRegistroNuovo = datiCorr.codiceNuovoRegistro;

                            //creo l'oggetto canale
                            DocsPaVO.utente.Canale canale = new DocsPaVO.utente.Canale();

                            if (!string.IsNullOrEmpty(datiCorr.descrizioneCanalePreferenziale))
                            {
                                canaleP = datiCorr.descrizioneCanalePreferenziale;
                            }
                            else
                            {
                                log.Add(Resources.MissingChannel);
                            }

                            if (!string.IsNullOrEmpty(canaleP))
                            {
                                canale.systemId = await this.GetSystemIdCanale(canaleP);
                            }
                            else
                            {
                                canale.systemId = await this.GetSystemIdCanale();
                                canaleP = "LETTERA";
                            }

                            // Recupero l'id dell'amministrazione
                            string idAmm = infoUtente.idAmministrazione;

                            string idRegistro = await this.GetRegistroDaCodice(codRegistro);
                            if ("0".Equals(idRegistro))
                                idRegistro = "";

                            string idRegistroNuovo = string.Empty;
                            if (!string.IsNullOrEmpty(codRegistroNuovo))
                            {
                                idRegistroNuovo = await this.GetRegistroDaCodice(codRegistroNuovo);
                            }
                            #region Insert Corr
                            if (storicizza.ToUpper() == "I")
                            {
                                bool inserisci = true;
                                if (codRubrica == "" ||
                                   (corrType.ToUpper() == "U" && (descrizione == null || descrizione.Trim() == "")) ||
                                   (corrType.ToUpper() == "R" && (descrizione == null || descrizione.Trim() == "")) ||
                                   (corrType.ToUpper() == "P" && (cognome.Trim() == "" || nome.Trim() == ""))
                                   )
                                {
                                    messaggio += Resources.MissingArgs;
                                    inserisci = false;
                                }
                                else
                                {
                                    if (corrType == "" || (corrType.ToUpper() != "P" && corrType.ToUpper() != "U" && corrType.ToUpper() != "R"))
                                    {
                                        messaggio += Resources.InvalidCorrType;
                                        inserisci = false;
                                    }

                                    if (idRegistro == null)
                                    {
                                        messaggio += Resources.InvalidReg;
                                        inserisci = false;
                                    }
                                    if (!this.CodiceRubricaValido(codRubrica))
                                    {
                                        messaggio += Resources.InvalidCodRub;
                                        inserisci = false;
                                    }

                                    if (!string.IsNullOrEmpty(mail))
                                        mailCorri = mail.Split(';');
                                    bool primaEmail = true;
                                    foreach (string m in mailCorri)
                                    {
                                        string[] split = { "##" };
                                        string[] splitEmailNota = m.Split(split, StringSplitOptions.None);
                                        if (splitEmailNota[0].Trim() != "")
                                        {
                                            int presente = (from mc in mailCorri where mc.Split(split, StringSplitOptions.None).ElementAt(0).Trim().Equals(splitEmailNota[0].Trim()) select mc).Count();
                                            if (presente > 1)
                                            {
                                                messaggio += Resources.DuplicateMail;
                                                inserisci = false;
                                                break;
                                            }
                                        }
                                        if (!splitEmailNota[0].Trim().Equals("") && !this.IsValidEmail(splitEmailNota[0].Trim()))
                                        {
                                            messaggio += Resources.InvalidMail;
                                            inserisci = false;
                                            break;
                                        }
                                        if (!splitEmailNota[0].Trim().Equals("") && primaEmail)
                                        {
                                            mail = splitEmailNota[0].Trim();
                                            primaEmail = false;
                                        }
                                    }

                                    if (codAmm != "" && !this.CodiceRubricaValido(codAmm))
                                    {
                                        messaggio += Resources.InvalidAmmCode;
                                        inserisci = false;
                                    }
                                    if (codAOO != "" && !this.CodiceRubricaValido(codAOO))
                                    {
                                        messaggio += Resources.InvalidAooCode;
                                        inserisci = false;
                                    }
                                    if (cap != null && !cap.Equals("") && !this.IsNumeric(cap))
                                    {
                                        messaggio += Resources.InvalidCap;
                                        inserisci = false;
                                    }
                                    if (provincia != null && !provincia.Equals("") && !this.IsCorrectProv(provincia))
                                    {
                                        messaggio += Resources.InvalidProv;
                                        inserisci = false;
                                    }
                                    if (corrType.ToUpper().Equals("U"))
                                    {
                                        if ((codiceFiscale != null && !codiceFiscale.Trim().Equals("")) && ((codiceFiscale.Trim().Length == 11 && CheckVatNumber(codiceFiscale.Trim()) != 0) || (codiceFiscale.Trim().Length == 16 && CheckTaxCode(codiceFiscale.Trim()) != 0) || (codiceFiscale.Trim().Length != 11 && codiceFiscale.Trim().Length != 16)))
                                        {
                                            messaggio += Resources.InvalidCF;
                                            inserisci = false;
                                        }
                                    }
                                    else
                                        if (codiceFiscale != null && !codiceFiscale.Trim().Equals("") && CheckTaxCode(codiceFiscale.Trim()) != 0)
                                    {
                                        messaggio += Resources.InvalidTaxCode;
                                        inserisci = false;
                                    }
                                    if (partitaIva != null && !partitaIva.Trim().Equals("") && CheckVatNumber(partitaIva.Trim()) != 0)
                                    {
                                        messaggio += Resources.InvalidPIva;
                                        inserisci = false;
                                    }

                                    switch (canaleP.ToUpper())
                                    {
                                        case "MAIL":
                                            if ((mail == null || mail.Trim().Equals("")))
                                            {
                                                messaggio += Resources.InvalidChNoMail;
                                                inserisci = false;
                                                break;
                                            }
                                            break;

                                        case "INTEROPERABILITA":
                                            if ((mail == null || mail.Trim().Equals("")) || (codAmm == null || codAmm.Trim().Equals("")) || (codAOO == null || codAOO.Trim().Equals("")))
                                            {
                                                messaggio += Resources.InvalidChInternopNoSm;
                                                inserisci = false;
                                                break;
                                            }
                                            break;

                                        case "INTEROPERABILITA PITRE":
                                            messaggio += Resources.InvalidChInterop;
                                            inserisci = false;
                                            break;

                                        case "LETTERA":
                                            break;

                                        case "RACCOMANDATA":
                                            break;

                                        case "FAX":
                                            break;

                                        case "CONSEGNA A MANO":
                                            break;

                                        default:
                                            messaggio += Resources.InvalidCh;
                                            inserisci = false;
                                            break;
                                    }

                                    if (!string.IsNullOrEmpty(telefono1) && telefono1.Length > 16)
                                    {
                                        messaggio += Resources.InvalidTel1;
                                        inserisci = false;
                                    }
                                    if (!string.IsNullOrEmpty(telefono2) && telefono2.Length > 16)
                                    {
                                        messaggio += Resources.InvalidTel2;
                                        inserisci = false;
                                    }
                                    if (!string.IsNullOrEmpty(fax) && fax.Length > 16)
                                    {
                                        messaggio += Resources.InvalidFax;
                                        inserisci = false;
                                    }
                                }

                                if (inserisci)
                                {
                                    DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
                                    DocsPaVO.utente.Corrispondente res = null;

                                    DocsPaVO.addressbook.DettagliCorrispondente dettagli = new DocsPaVO.addressbook.DettagliCorrispondente();
                                    if (corrType == "" || corrType == null)
                                        corrType = "U";

                                    Application.Requests.AddressbookInsertCorrispondente req = null;
                                    switch (corrType.ToUpper())
                                    {
                                        case "U":
                                            DocsPaVO.utente.UnitaOrganizzativa uo = new DocsPaVO.utente.UnitaOrganizzativa();
                                            uo.tipoCorrispondente = "U";
                                            uo.info = new DocsPaVO.addressbook.DettagliCorrispondente();
                                            uo.codiceCorrispondente = codRubrica.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                            uo.codiceRubrica = codRubrica.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                            uo.codiceAmm = codAmm.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                            uo.codiceAOO = codAOO.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                            uo.email = mail.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                            uo.descrizione = descrizione.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                            uo.idAmministrazione = infoUtente.idAmministrazione;

                                            uo.idRegistro = idRegistro;


                                            dettagli.Corrispondente.AddCorrispondenteRow(
                                                indirizzo.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), citta.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), cap.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                                provincia.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), nazione.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), telefono1.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                                telefono2.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), fax, codiceFiscale.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), note.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), localita.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), string.Empty, string.Empty, string.Empty, partitaIva.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()));

                                            uo.info = dettagli;
                                            uo.dettagli = true;


                                            uo.canalePref = canale;
                                            uo.note = note;
                                            req = new Application.Requests.AddressbookInsertCorrispondente(uo, null, infoUtente);
                                            break;

                                        case "R":
                                            res = new DocsPaVO.utente.Corrispondente();
                                            DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                                            ruolo.tipoCorrispondente = "R";
                                            ruolo.codiceCorrispondente = codRubrica;
                                            ruolo.codiceRubrica = codRubrica;
                                            ruolo.descrizione = descrizione;


                                            ruolo.idRegistro = idRegistro;



                                            ruolo.email = mail;
                                            ruolo.codiceAmm = codAmm;
                                            ruolo.codiceAOO = codAOO;
                                            ruolo.idAmministrazione = infoUtente.idAmministrazione;
                                            DocsPaVO.utente.UnitaOrganizzativa parent_uo = new();
                                            parent_uo.descrizione = "";
                                            parent_uo.systemId = "0";

                                            ruolo.canalePref = canale;
                                            ruolo.note = note;
                                            req = new Application.Requests.AddressbookInsertCorrispondente(ruolo, parent_uo, infoUtente);
                                            break;

                                        case "P":
                                            res = new DocsPaVO.utente.Corrispondente();
                                            DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();
                                            utente.codiceCorrispondente = codRubrica;
                                            utente.codiceRubrica = codRubrica;
                                            utente.cognome = cognome;
                                            utente.nome = nome;
                                            utente.email = mail;
                                            utente.codiceAmm = codAmm;
                                            utente.codiceAOO = codAOO;
                                            utente.descrizione = cognome + nome;
                                            utente.idAmministrazione = infoUtente.idAmministrazione;

                                            utente.idRegistro = idRegistro;
                                            utente.canalePref = canale;
                                            utente.note = note;
                                            if ((indirizzo != null && !indirizzo.Equals("")) ||
                                                    (cap != null && !cap.Equals("")) ||
                                                    (citta != null && !citta.Equals("")) ||
                                                    (provincia != null && !provincia.Equals("")) ||
                                                    (nazione != null && !nazione.Equals("")) ||
                                                    (telefono1 != null && !telefono1.Equals("")) ||
                                                    (telefono2 != null && !telefono2.Equals("")) ||
                                                    (fax != null && !fax.Equals("")) ||
                                                    (codiceFiscale != null && !codiceFiscale.Equals("")) ||
                                                    (localita != null && !localita.Equals("")) ||
                                                     partitaIva != null && !partitaIva.Equals(""))
                                            {
                                                dettagli.Corrispondente.AddCorrispondenteRow(
                                                indirizzo, citta, cap,
                                                provincia, nazione, telefono1,
                                                telefono2, fax, codiceFiscale.Trim(), note, localita, "", "", "", partitaIva.Trim());

                                                utente.info = dettagli;
                                                utente.dettagli = true;
                                            }
                                            req = new Application.Requests.AddressbookInsertCorrispondente(utente, null, infoUtente);
                                            break;
                                    }
                                    try
                                    {
                                        res = (await this._mediator.Send(req)).output;
                                        if (res == null)
                                        {
                                            result = false;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        res = null;
                                        this._logger.LogError(exception: ex, message: ex.Message);
                                    }

                                    if (res != null && res.errore == null)
                                    {
                                        corrInseriti++;
                                        log.Add("");
                                        log.Add(string.Format(Resources.CorrEntry, codRubrica));

                                        //await this._webMethodLoggerService.LogOK("IMPORTARUBRICACREA", string.Format(Resources.CorrEntry, codRubrica));

                                        List<MailCorrispondente> listMail = new List<MailCorrispondente>();
                                        bool primaEmail = true;

                                        foreach (string m1 in mailCorri)
                                        {
                                            if (!string.IsNullOrEmpty(m1.Trim()))
                                            {
                                                MailCorrispondente mc = new MailCorrispondente();

                                                string[] split = { "##" };
                                                string[] noteEmail = m1.Split(split, StringSplitOptions.None);
                                                if (noteEmail.Length > 1 && noteEmail[1].Trim() != "" && noteEmail[0].Trim() != "")
                                                {
                                                    mc.Note = noteEmail[1].Trim();
                                                }
                                                if (noteEmail[0].Trim() != "")
                                                {
                                                    mc.Email = noteEmail[0].Trim();
                                                    if (primaEmail)
                                                    {
                                                        mc.Principale = "1";
                                                        primaEmail = false;
                                                    }
                                                    listMail.Add(mc);
                                                }
                                            }
                                        }

                                        bool insMailCorrEst = (await this._mediator.Send(new Application.Requests.InsertMailCorrispondenteEsterno(listMail, res.systemId))).output;

                                    }
                                    else
                                    {
                                        corrNonInseriti++;
                                        log.Add("");
                                        log.Add(string.Format(Resources.CorrNotAddedEntry, codRubrica + " " + res.errore ?? string.Empty));
                                        if (!string.IsNullOrEmpty(messaggio))
                                            log.AddRange(messaggio.Substring(2).Split("\n"));
                                        //await this._webMethodLoggerService.LogKO("IMPORTARUBRICACREA", string.Format(Resources.CorrNotAddedEntry, codRubrica));
                                    }
                                }
                                else
                                {
                                    corrNonInseriti++;
                                    log.Add("");
                                    log.Add(string.Format(Resources.CorrNotAddedEntry, codRubrica));
                                    if (!string.IsNullOrEmpty(messaggio))
                                        log.AddRange(messaggio.Substring(2).Split("\n"));
                                    //await this._webMethodLoggerService.LogKO("IMPORTARUBRICACREA", string.Format(Resources.CorrNotAddedEntry, codRubrica));
                                }
                            }
                            #endregion

                            string idCorrGlobali = await this.GetSystemIDCorr(codRubrica, idAmm, idRegistro);
                            bool primEmail = true;

                            #region modifica corrispondente
                            if (storicizza.ToUpper() == "M")
                            {
                                if (this.VerificaSelezione(corrType, descrizione, cognome,
                                                            nome, telefono1, telefono2, cap, mail,
                                                            provincia, ref codiceFiscale, ref partitaIva,
                                                            ref note, ref messaggio, codRubrica, canaleP, codAOO, codAmm))
                                {
                                    if (!string.IsNullOrEmpty(mail))
                                        mailCorri = mail.Split(';');
                                    if (mailCorri != null && mailCorri.Length > 0)
                                    {

                                        string[] split = { "##" };
                                        string[] splitEmailNota = mailCorri[0].Split(split, StringSplitOptions.None);
                                        if (!splitEmailNota[0].Trim().Equals("") && primEmail)
                                        {
                                            mail = splitEmailNota[0].Trim();
                                            primEmail = false;
                                        }
                                    }
                                    DocsPaVO.utente.DatiModificaCorr corr = new DocsPaVO.utente.DatiModificaCorr();
                                    corr.idCorrGlobali = idCorrGlobali;
                                    if (!string.IsNullOrEmpty(idRegistroNuovo))
                                    {
                                        corr.idRegistro = idRegistroNuovo;
                                    }
                                    else
                                    {
                                        corr.idRegistro = idRegistro;
                                    }
                                    corr.codRubrica = codRubrica;
                                    corr.codiceAmm = codAmm;
                                    corr.codiceAoo = codAOO;
                                    corr.tipoCorrispondente = corrType;
                                    if (corrType.ToUpper() == "P")
                                        corr.descCorr = cognome + " " + nome;
                                    else
                                        corr.descCorr = descrizione;
                                    corr.cognome = cognome;
                                    corr.nome = nome;
                                    corr.indirizzo = indirizzo;
                                    corr.cap = cap;
                                    corr.citta = citta;
                                    corr.provincia = provincia;
                                    corr.nazione = nazione;
                                    corr.codFiscale = codiceFiscale.Trim();
                                    corr.partitaIva = partitaIva.Trim();
                                    corr.telefono = telefono1;
                                    corr.telefono2 = telefono2;
                                    corr.fax = fax;
                                    corr.email = mail;
                                    corr.localita = localita;
                                    corr.idCanalePref = canale.systemId;
                                    corr.note = note;

                                    bool res = false;
                                    string message = string.Empty;
                                    if (corr.idRegistro != null)
                                    {
                                        try
                                        {
                                            //modify
                                            var upRes = (await this._mediator.Send(new Application.Requests.CorrispondentiDeleteModifyCorrispondenteEsternoWithId(infoUtente, corr, flagListe, "M")));
                                            res = upRes.output;
                                            message = upRes.message;
                                            string newIDCorr = upRes.newIdCorr;

                                            if (!string.IsNullOrEmpty(newIDCorr) && !newIDCorr.Equals("0"))
                                                corr.idCorrGlobali = newIDCorr;
                                        }
                                        catch (Exception e)
                                        {
                                            this._logger.LogError(exception: e, message: e.Message);
                                            res = false;
                                        }

                                    }
                                    if (res && message.Equals("OK"))
                                    {
                                        corrAggiornati++;
                                        log.Add("");
                                        log.Add(string.Format(Resources.UpdateCorr, corr.codRubrica));
                                        //await this._webMethodLoggerService.LogOK("IMPORTARUBRICAMODIFICA", string.Format(Resources.UpdateCorr, corr.codRubrica));

                                        List<MailCorrispondente> listMail = new List<MailCorrispondente>();
                                        bool primaEmail = true;

                                        foreach (string m1 in mailCorri)
                                        {
                                            if (!string.IsNullOrEmpty(m1.Trim()))
                                            {
                                                MailCorrispondente mc = new MailCorrispondente();

                                                string[] split = { "##" };
                                                string[] noteEmail = m1.Split(split, StringSplitOptions.None);
                                                if (noteEmail.Length > 1 && noteEmail[1].Trim() != "")
                                                {
                                                    mc.Note = noteEmail[1].Trim();
                                                }
                                                if (noteEmail[0].Trim() != "")
                                                {
                                                    mc.Email = noteEmail[0].Trim();
                                                    if (primaEmail)
                                                    {
                                                        mc.Principale = "1";
                                                        primaEmail = false;
                                                    }
                                                    listMail.Add(mc);
                                                }
                                            }
                                        }
                                        bool insMailCorrEst = (await this._mediator.Send(new Application.Requests.InsertMailCorrispondenteEsterno(listMail, corr.idCorrGlobali))).output;
                                    }
                                    else
                                    {
                                        corrNonAggiornati++;
                                        log.Add("");
                                        log.Add(string.Format(Resources.NonUpdatedCorr, corr.codRubrica));
                                        if (!string.IsNullOrEmpty(messaggio))
                                            log.AddRange(messaggio.Substring(2).Split("\n"));
                                        //await this._webMethodLoggerService.LogOK("IMPORTARUBRICAMODIFICA", string.Format(Resources.NonUpdatedCorr, corr.codRubrica));
                                    }
                                }
                                else
                                {
                                    corrNonAggiornati++;
                                    log.Add("");
                                    log.Add(string.Format(Resources.NonUpdatedCorr, codRubrica));
                                    if (!string.IsNullOrEmpty(messaggio))
                                        log.AddRange(messaggio.Substring(2).Split("\n"));
                                    //await this._webMethodLoggerService.LogOK("IMPORTARUBRICAMODIFICA", string.Format(Resources.NonUpdatedCorr, codRubrica));
                                }
                            }
                            #endregion

                            #region elimina corrispondente

                            if (storicizza.ToUpper() == "C")
                            {
                                bool res = false;
                                if (idRegistro != null)
                                {
                                    DatiModificaCorr datiModificaCorr = new()
                                    {
                                        idCorrGlobali = idCorrGlobali
                                    };
                                    CorrispondentiDeleteModifyCorrispondenteEsternoWithIdResult delRes = null;
                                    try
                                    {
                                        delRes = (await this._mediator.Send(new Application.Requests.CorrispondentiDeleteModifyCorrispondenteEsternoWithId(infoUtente, datiModificaCorr, flagListe, "D")));
                                    }
                                    catch (Exception ex)
                                    {
                                        corrNonRimossi++;
                                        log.Add("");
                                        log.Add(string.Format(Resources.NonDelCorr, codRubrica));

                                    }
                                    if (delRes != null)
                                    {
                                        res = delRes.output;
                                        string message = delRes.message;
                                        if (res)
                                        {
                                            corrRimossi++;
                                            log.Add("");
                                            log.Add(string.Format(Resources.DelCorr, codRubrica));
                                            //await this._webMethodLoggerService.LogOK("IMPORTARUBRICAMODIFICA", string.Format(Resources.DelCorr, codRubrica));

                                            bool delMailCorrEst = (await this._mediator.Send(new Application.Requests.DeleteMailCorrispondenteEsterno(idCorrGlobali))).output;
                                            if (delMailCorrEst)
                                            {
                                                log.Add(string.Format(Resources.DelMailCorr, codRubrica));
                                            }
                                            else
                                            {
                                                log.Add(string.Format(Resources.NonDelMailCorr, codRubrica));
                                            }
                                        }
                                        else
                                        {
                                            corrNonRimossi++;
                                            log.Add("");
                                            log.Add(string.Format(Resources.NonDelCorr, codRubrica));


                                            if (message.Equals("NOTOK"))
                                            {
                                                log.Add(string.Format(Resources.DelCorrInList, codRubrica));
                                                //await this._webMethodLoggerService.LogKO("IMPORTARUBRICACANCELLA", this.GetLogEntry(string.Format(Resources.DelCorrInList, codRubrica)));
                                            }

                                            if (message.StartsWith("ERROR"))
                                            {
                                                log.Add(string.Format(Resources.NonDelCorrError, message));
                                                //await this._webMethodLoggerService.LogKO("IMPORTARUBRICACANCELLA", this.GetLogEntry(string.Format(Resources.NonDelCorrError, message)));
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    corrNonRimossi++;
                                    log.Add("");
                                    log.Add(string.Format(Resources.NonDelCorrCodReg, codRubrica));
                                    if (!string.IsNullOrEmpty(messaggio))
                                        log.AddRange(messaggio.Substring(2).Split("\n"));
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            log.Add("");
                            log.Add(Resources.StorMissing);
                        }
                    }
                    log.Add("");
                    log.Add(this.GetLogEntry(Resources.EndOfImport));
                    log.Add(string.Format(Resources.ResMessage, corrInseriti, corrAggiornati, corrRimossi, corrNonInseriti, corrNonAggiornati, corrNonRimossi));
                }
            }
            catch (Exception ex)
            {
                //await this._webMethodLoggerService.LogKO("IMPORTARUBRICAEXCEPTION", string.Format(Resources.ErrorImport,ex.Message));
                result = false;
            }
            finally
            {
                if (!Directory.Exists(this._logPath))
                    Directory.CreateDirectory(this._logPath);

                using (var logFile = new StreamWriter(Path.Combine(this._logPath, $"logImportRubrica_{infoUtente.idPeople}.log")))
                {
                    log.ForEach(l => logFile.WriteLine(l.Replace("\n","")));
                }
            }
            
            ImportRes output = new()
            {
                Outcome = result,
                CorrAggiornati = corrAggiornati,
                CorrNonAggiornati = corrNonAggiornati,
                CorrInseriti = corrInseriti,
                CorrNonInseriti = corrNonInseriti,
                CorrRimossi = corrRimossi,
                CorrNonRimossi = corrNonRimossi
            };
            return output;
        }
        #region Non Public fields
        protected ILogger<ImportaRubricaHandler> _logger;
        protected IPi3DbContext _dbContext;
        protected ISpreadsheetService _spreadsheetService;
        protected IConfiguration _configuration;
        protected readonly string _logPath;
        protected IMediator _mediator;
        protected IWebMethodLoggerService _webMethodLoggerService;
        #endregion
        #region data validation methods

        private bool VerificaSelezione(string corrType, string descrizione, string cognome,
                                       string nome, string tel1, string tel2, string cap, string mail, 
                                       string provincia, ref string codiceFiscale, ref string partitaIva,
                                       ref string note, ref string messaggio, string codRubrica, string canaleP, 
                                       string codAOO, string codAmm)
        {
            bool resultCheck = true;


            if ((corrType == "U" && descrizione.Trim() == "") ||
                (corrType == "R" && descrizione.Trim() == "") ||
                (corrType == "P" && (cognome.Trim() == "" || nome.Trim() == "")))
            {
                
                messaggio += Resources.MissingArgs;
                resultCheck = false;
            }
            else
            {
                if (corrType == "" || (corrType.ToUpper() != "P" && corrType.ToUpper() != "U" && corrType.ToUpper() != "R"))
                {
                    messaggio += Resources.InvalidCorrType;
                    resultCheck = false;
                }

                if ((tel1 == null || tel1.Equals(""))
                    && !(tel2 == null || tel2.Equals("")))
                {
                    messaggio += Resources.InvalidTel1;
                    resultCheck = false;
                }

                if (cap != null && !cap.Equals("") && !this.IsNumeric(cap))
                {
                    messaggio += Resources.InvalidCap;
                    resultCheck = false;
                }

                if (provincia != null && !provincia.Equals("") && !this.IsCorrectProv(provincia))
                {
                    messaggio += Resources.InvalidProv;
                    resultCheck = false;
                }
                string[] mailCorri = new string[0];
                if (!string.IsNullOrEmpty(mail))
                    mailCorri = mail.Split(';');
                foreach (string m in mailCorri)
                {
                    string[] split = { "##" };
                    string[] splitEmailNota = m.Split(split, StringSplitOptions.None);
                    if (splitEmailNota[0].Trim() != "")
                    {
                        int presente = (from mc in mailCorri where mc.Split(split, StringSplitOptions.None).ElementAt(0).Trim().Equals(splitEmailNota[0].Trim()) select mc).Count();
                        if (presente > 1)
                        {
                            messaggio += Resources.DuplicateMail;
                            resultCheck = false;
                            break;
                        }
                    }
                    if (!splitEmailNota[0].Trim().Equals("") && !IsValidEmail(splitEmailNota[0].Trim()))
                    {
                        messaggio += Resources.InvalidMail;
                        resultCheck = false;
                        break;
                    }
                }

                if (corrType.ToUpper().Equals("U"))
                {
                    if ((codiceFiscale != null && !codiceFiscale.Trim().Equals("")) && ((codiceFiscale.Trim().Length == 11 && CheckVatNumber(codiceFiscale.Trim()) != 0) || (codiceFiscale.Trim().Length == 16 && CheckTaxCode(codiceFiscale.Trim()) != 0) || (codiceFiscale.Trim().Length != 11 && codiceFiscale.Trim().Length != 16)))
                    {
                        messaggio += Resources.InvalidCF;
                        resultCheck = false;
                    }
                }
                else
                    if (codiceFiscale != null && !codiceFiscale.Trim().Equals("") && CheckTaxCode(codiceFiscale.Trim()) != 0)
                {
                    messaggio += Resources.InvalidCF;
                    resultCheck = false;
                }


                if (partitaIva != null && !partitaIva.Trim().Equals("") && CheckVatNumber(partitaIva.Trim()) != 0)
                {
                    messaggio += Resources.InvalidPIva;
                    resultCheck = false;
                }

                switch (canaleP.ToUpper())
                {
                    case "MAIL":
                        if ((mail == null || mail.Trim().Equals("")))
                        {
                            messaggio += Resources.MailChan;
                            resultCheck = false;
                            break;
                        }
                        break;

                    case "INTEROPERABILITA":
                        if ((mail == null || mail.Trim().Equals("")) || (codAmm == null || codAmm.Trim().Equals("")) || (codAOO == null || codAOO.Trim().Equals("")))
                        {
                            messaggio += Resources.InteropChan;
                            resultCheck = false;
                            break;
                        }
                        break;

                    case "INTEROPERABILITA PITRE":
                        messaggio += Resources.InvalidChInterop;
                        resultCheck = false;
                        break;

                    case "LETTERA":
                        break;

                    case "RACCOMANDATA":
                        break;

                    case "FAX":
                        break;

                    case "CONSEGNA A MANO":
                        break;

                    default:
                        messaggio += Resources.InvalidCh;
                        resultCheck = false;
                        break;
                }

            }
            return resultCheck;
        }

        private string GetValue(int index , List<CellModel> row)
        {
            var cell = row.Where(c => c.Column == index).FirstOrDefault();
            if (cell != null)
            {
                return cell.ValueAsString ?? string.Empty;
            }
            return string.Empty;
        }
        private int CheckTaxCode(string taxCode)
        {
            taxCode = taxCode.Replace(" ", "");
            bool result = false;
            const int character = 16;
            const string omocode = "LMNPQRSTUV";
            const string listControl = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            int[] listEquivalent = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            int[] listaUnequal = { 1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 2, 4, 18, 20, 11, 3, 6, 8, 12, 14, 16, 10, 22, 25, 24, 23 };

            result = (string.IsNullOrEmpty(taxCode) || taxCode.Length != character);
            if (result)
                return -1;
            taxCode = taxCode.ToUpper();
            char[] arrTaxCode = taxCode.ToCharArray();
            for (int k = 6; k < 15; k++)
            {
                if ((k == 8) || (k == 11))
                    continue;
                int x = (omocode.IndexOf(arrTaxCode[k]));
                if (x != -1)
                    arrTaxCode[k] = x.ToString()[0];
            }

            Regex rgx = new Regex(@"^[A-Z]{6}[0-9]{2}[A-Z][0-9]{2}[A-Z][0-9]{3}[A-Z]$", RegexOptions.None, TimeSpan.FromSeconds(5));
            Match m = rgx.Match(new string(arrTaxCode));
            result = m.Success;
            if (!result)
                return -2;
            int somma = 0;
            arrTaxCode = taxCode.ToCharArray();
            for (int i = 0; i < 15; i++)
            {
                char c = arrTaxCode[i];
                int x = "0123456789".IndexOf(c);
                if (x != -1)
                    c = listControl.Substring(x, 1)[0];
                x = listControl.IndexOf(c);
                if ((i % 2) == 0)
                    x = listaUnequal[x];
                else
                    x = listEquivalent[x];
                somma += x;
            }
            result = (listControl.Substring(somma % 26, 1) == taxCode.Substring(15, 1));
            if (!result)
                return -3;
            return 0;
        }

        private int CheckVatNumber(string vatNum)
        {
            bool result = false;
            const int character = 11;
            string vatNumber = vatNum;
            Regex pregex = new Regex("^\\d{" + character.ToString() + "}$", RegexOptions.None, TimeSpan.FromSeconds(5));

            if (string.IsNullOrEmpty(vatNumber) || vatNum.Length != character)
                return -1;
            Match m = pregex.Match(vatNumber);
            result = m.Success;
            if (!result)
                return -2;
            result = (int.Parse(vatNumber.Substring(0, 7)) != 0);
            if (!result)
                return -3;
            result = ((int.Parse(vatNumber.Substring(7, 3)) >= 0) && (int.Parse(vatNumber.Substring(7, 3)) < 201));
            if (!result)
                return -4;

            int sum = 0;
            for (int i = 0; i < character - 1; i++)
            {
                int j = int.Parse(vatNumber.Substring(i, 1));
                if ((i + 1) % 2 == 0)
                {
                    j *= 2;
                    char[] c = j.ToString("00").ToCharArray();
                    sum += int.Parse(c[0].ToString());
                    sum += int.Parse(c[1].ToString());
                }
                else
                    sum += j;
            }
            if ((sum.ToString("00").Substring(1, 1).Equals("0")) && (!vatNumber.Substring(10, 1).Equals("0")))
                return -5;
            sum = int.Parse(vatNumber.Substring(10, 1)) + int.Parse(sum.ToString("00").Substring(1, 1));
            if (!sum.ToString("00").Substring(1, 1).Equals("0"))
                return -5;
            return 0;
        }


        private bool IsCorrectProv(string val)
        {
            string appo = val.ToUpper();
            Regex regExp = new Regex("([A-Z]{2})", RegexOptions.None, TimeSpan.FromSeconds(5));

            return regExp.IsMatch(appo);
        }

        private bool IsValidEmail(string strToCheck)
        {
            return Regex.IsMatch(strToCheck, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.None, TimeSpan.FromSeconds(5));
        }
        
        private bool IsNumeric(string val)
        {
            string appo = val;
            if (!string.IsNullOrEmpty(appo))
                appo = appo.Trim();
            Regex regExp = new Regex("\\D", RegexOptions.None, TimeSpan.FromSeconds(5));
            return !regExp.IsMatch(appo);
        }

        private bool CodiceRubricaValido(string cod)
        {
            if (cod == null || cod.Trim() == "")
                return false;

            Regex rx = new Regex(@"^[0-9A-Za-z_\ \.\-]+$", RegexOptions.None, TimeSpan.FromSeconds(5));
            return rx.IsMatch(cod);
        }

        private async Task<string> GetRegistroDaCodice(string codice)
        {
            var idReg = await (from r in this._dbContext.RegistroEntities.AsNoTracking()
             where r.VAR_CODICE != null && r.VAR_CODICE.Equals(codice)
             select r.SYSTEM_ID).FirstOrDefaultAsync();

            if(idReg != null)
            {
                return idReg.ToString();
            }
            else
            {
                return "0";
            }
        }
        #endregion
        private async Task<string> GetSystemIdCanale(string descCanale = "LETTERA")
        {
            var sysId = await (from dt in this._dbContext.DocumentTypesEntities.AsNoTracking()
             where dt.TYPE_ID != null && dt.TYPE_ID.ToUpper().Equals(descCanale.ToUpper())
             select dt.SYSTEM_ID.ToString()).FirstOrDefaultAsync();

            if(sysId != null)
            {
                return sysId;
            }
            else
            {
                return "0";
            }
        }

        private async Task<string> GetSystemIDCorr(string codRubrica, string idAmm, string idRegistro)
        {
            var bQuery = (from c in this._dbContext.CorrGlobaliEntities.AsNoTracking()
             where c.VAR_COD_RUBRICA != null && c.VAR_COD_RUBRICA.ToUpper().Equals(codRubrica.ToUpper()) && c.ID_AMM == idAmm.AsLong()
             select new
             {
                 c.ID_REGISTRO,
                 c.SYSTEM_ID
             });

            if (!string.IsNullOrEmpty(idRegistro))
            {
                bQuery.Where(c => c.ID_REGISTRO == idRegistro.AsLong());
            }
            var corr = await bQuery.FirstOrDefaultAsync();

            if(corr != null)
            {
                return corr.SYSTEM_ID != null ? corr.SYSTEM_ID.ToString() : string.Empty;
             }
            return string.Empty;
        }

        protected string GetLogEntry(string text)
        {
            return $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} {text}";
        }


        private class ImportRes
        {
            public bool Outcome { get; set; }
            public int CorrInseriti { get; set; }
            public int CorrAggiornati { get; set; }
            public int CorrRimossi { get; set; }
            public int CorrNonInseriti { get; set; }
            public int CorrNonAggiornati { get; set; }
            public int CorrNonRimossi { get; set; }
        }

    }
}
