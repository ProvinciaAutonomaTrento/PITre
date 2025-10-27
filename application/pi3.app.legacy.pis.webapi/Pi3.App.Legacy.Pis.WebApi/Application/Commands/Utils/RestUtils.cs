// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Conservazione.Rapporto;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.trasmissione;
using DocsPaVO.utente;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Registers;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate;
using Pi3.Core.Extensions;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;
using System.Data;
using System.Reflection.Emit;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Xml.Linq;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.CorrispondentiDeleteModifyCorrispondenteEsterno;
using MediatR;
using DocsPaVO.documento;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetFile;
using System;
using DocsPaVO.Notification;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils
{
    public class RestUtils
    {
        public static string Decrypt(string cipherString)
        {
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = Convert.FromBase64String(cipherString);
                byte[] resultArray;

                //La chiave deve essere di 24 caratteri
                string key = "ValueTeamDocsPa3Services";

                keyArray = UTF8Encoding.UTF8.GetBytes(key);


                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyArray;
                    aes.Mode = CipherMode.ECB;
                    aes.Padding = PaddingMode.PKCS7;

                    ICryptoTransform cTransform = aes.CreateDecryptor();
                    resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                    aes.Clear();
                }
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void CheckTokenExpiration(string dateFromToken, string idAmm, IPi3DbContext dbContext)
        {
            // durata di default
            int pisTokenDuration = 20;
            Int32.TryParse(
                dbContext.ChiaviConfigurazioneEntities.Where(c => c.VAR_CODICE == "BE_PIS_TOKEN_DURATION" && c.ID_AMM == idAmm.AsLong()).Select(c => c.VAR_VALORE).FirstOrDefault(), out pisTokenDuration);
            // Se impostata a zero viene interpretata come durata illimitata
            if (pisTokenDuration == 0) return;
            DateTime dataToken = new DateTime();

            try
            {
                dataToken = DateTime.ParseExact(dateFromToken, "s", null);
            }
            catch (Exception exc)
            {
                return;
            }
            if (dataToken == null) throw new RestException("INVALID_TOKEN");
            else
            {
                if (dataToken < DateTime.Now.AddMinutes(pisTokenDuration * -1)) throw new RestException("TOKEN_EXPIRED");
            }

        }

        public static string CalcolaImpronta256(byte[] stream)
        {
            SHA256Managed sha = new SHA256Managed();
            byte[] impronta = sha.ComputeHash(stream);
            return BitConverter.ToString(impronta).Replace("-", "");
        }

        public static InfoUtente ControlloToken(IHttpContextAccessor httpContext, IPi3DbContext dbContext, out Utente utente, out Ruolo ruolo)
        {
            InfoUtente infoUtente = null;
            utente = null;
            ruolo = new Ruolo();
            string token = "";
            StringValues header1 = new StringValues();
            _ = httpContext.HttpContext.Request.Headers.TryGetValue("AuthToken", out header1);
            if (header1 != StringValues.Empty)
                token = header1.FirstOrDefault() ?? "";
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new RestException("AUTHTOKEN_MISSING");
            }

            string authInfoString = RestUtils.Decrypt(token.Substring(4));
            string[] authInfoArray = authInfoString.Split('|');
            RestUtils.CheckTokenExpiration(authInfoArray[9], authInfoArray[4], dbContext);

            utente = DBUtils.getUtente(authInfoArray[5], authInfoArray[4], dbContext);
            ruolo = DBUtils.getRuoloById(authInfoArray[0], dbContext);
            infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
            infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

            return infoUtente;
        }

        public static bool IsRuoloAuthorized(Ruolo ruolo, string tipologia)
        {
            bool trovato = false;

            if (ruolo == null || ruolo.funzioni == null)
                return false;

            foreach (DocsPaVO.utente.Funzione f in ruolo.funzioni)
            {
                if (f.codice.Equals(tipologia))
                {
                    trovato = true;
                    break;
                }
            }
            return trovato;
        }


        #region Mappatura oggetti Entity Framework in DocsPaVO

        public static DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli CreateAssDocFascRuoli(AROggCustomDocEntity ogg)
        {
            DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli output = new()
            {
                ID_GRUPPO = ogg.ID_RUOLO != null ? ogg.ID_RUOLO.ToString() : null,
                ID_TIPO_DOC_FASC = ogg.ID_TEMPLATE != null ? ogg.ID_TEMPLATE.ToString() : null,
                ID_OGGETTO_CUSTOM = ogg.ID_OGGETTO_CUSTOM != null ? ogg.ID_OGGETTO_CUSTOM.ToString() : null,
                INS_MOD_OGG_CUSTOM = ogg.INS_MOD != null ? ogg.INS_MOD.ToString() : null,
                VIS_OGG_CUSTOM = ogg.VIS != null ? ogg.VIS.ToString() : null,
                ANNULLA_REPERTORIO = ogg.DEL_REP != null ? ogg.DEL_REP.ToString() : null
            };

            return output;
        }
        public static DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli CreateAssDocFascRuoliFasc(AROggCustomFascEntity ogg)
        {
            DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli output = new()
            {
                ID_GRUPPO = ogg.ID_RUOLO != null ? ogg.ID_RUOLO.ToString() : null,
                ID_TIPO_DOC_FASC = ogg.ID_TEMPLATE != null ? ogg.ID_TEMPLATE.ToString() : null,
                ID_OGGETTO_CUSTOM = ogg.ID_OGGETTO_CUSTOM != null ? ogg.ID_OGGETTO_CUSTOM.ToString() : null,
                INS_MOD_OGG_CUSTOM = ogg.INS_MOD != null ? ogg.INS_MOD.ToString() : null,
                VIS_OGG_CUSTOM = ogg.VIS != null ? ogg.VIS.ToString() : null,
            };

            return output;
        }
        public static DocsPaVO.amministrazione.OrgTitolario CreateTitolario(ProjectEntity rowTitolario)
        {
            DocsPaVO.amministrazione.OrgTitolario titolario = new DocsPaVO.amministrazione.OrgTitolario();

            titolario.ID = rowTitolario.SYSTEM_ID.ToString();
            titolario.Codice = rowTitolario.VAR_CODICE ?? "";
            titolario.CodiceAmministrazione = "";
            titolario.Commento = rowTitolario.VAR_NOTE ?? "";
            titolario.DataAttivazione = rowTitolario.DTA_ATTIVAZIONE?.ToString();
            titolario.DataCessazione = rowTitolario.DTA_CESSAZIONE?.ToString();
            titolario.DescrizioneLite = rowTitolario.DESCRIPTION ?? "";

            switch (rowTitolario.CHA_STATO.ToString())
            {
                case "D":
                    titolario.Descrizione = rowTitolario.DESCRIPTION ?? "" + " - In definizione";
                    titolario.Stato = DocsPaVO.amministrazione.OrgStatiTitolarioEnum.InDefinizione;
                    break;
                case "A":
                    titolario.Descrizione = rowTitolario.DESCRIPTION ?? "" + " - Attivo";
                    titolario.Stato = DocsPaVO.amministrazione.OrgStatiTitolarioEnum.Attivo;
                    break;
                case "C":
                    DateTime dataAttivazione = (DateTime)rowTitolario.DTA_ATTIVAZIONE;
                    DateTime dataCessazione = (DateTime)rowTitolario.DTA_CESSAZIONE;
                    titolario.Descrizione = rowTitolario.DESCRIPTION ?? "" + " - In vigore dal " + dataAttivazione.ToString("dd/MM/yyyy") + " al " + dataCessazione.ToString("dd/MM/yyyy");
                    titolario.Stato = DocsPaVO.amministrazione.OrgStatiTitolarioEnum.Chiuso;
                    break;
            }

            titolario.MaxLivTitolario = rowTitolario.MAX_LIV_TIT ?? "";
            titolario.EtichettaTit = rowTitolario.ET_TITOLARIO ?? "";
            titolario.EtichettaLiv1 = rowTitolario.ET_LIVELLO1 ?? "";
            titolario.EtichettaLiv2 = rowTitolario.ET_LIVELLO2 ?? "";
            titolario.EtichettaLiv3 = rowTitolario.ET_LIVELLO3 ?? "";
            titolario.EtichettaLiv4 = rowTitolario.ET_LIVELLO4 ?? "";
            titolario.EtichettaLiv5 = rowTitolario.ET_LIVELLO5 ?? "";
            titolario.EtichettaLiv6 = rowTitolario.ET_LIVELLO6 ?? "";


            return titolario;
        }

        public static DocsPaVO.utente.Registro CreateRegistro(RegistroEntity rowRegistro)
        {
            DocsPaVO.utente.Registro registro = new DocsPaVO.utente.Registro();

            registro.systemId = rowRegistro.SYSTEM_ID.ToString();
            registro.codRegistro = rowRegistro.VAR_CODICE;
            registro.codice = rowRegistro.VAR_CODICE;
            registro.descrizione = rowRegistro.VAR_DESC_REGISTRO;
            registro.chaRF = rowRegistro.CHA_RF;
            registro.stato = rowRegistro.CHA_STATO;
            registro.email = rowRegistro.VAR_EMAIL_REGISTRO;
            return registro;
        }
        #endregion

        #region Mappatura oggetti DocsPaVO in Model Rest
        public static CorrespondentAdvanced GetCorrAdvanced(IPi3DbContext dbContext, DocsPaVO.utente.Corrispondente corr, DocsPaVO.utente.InfoUtente infoUtente)
        {
            CorrespondentAdvanced output = new CorrespondentAdvanced();
            if (corr != null)
            {

                output.Address = corr.indirizzo;
                output.AdmCode = corr.codiceAmm ?? string.Empty;
                output.AOOCode = corr.codiceAOO ?? string.Empty;
                output.Cap = corr.cap;
                output.City = corr.citta;
                output.Code = corr.codiceRubrica;
                if (!string.IsNullOrEmpty(corr.idRegistro) && !corr.idRegistro.Equals("NULL"))
                {
                    DocsPaVO.utente.Registro reg = DBUtils.getRegistro(corr.idRegistro, dbContext);
                    if (reg == null)
                    {
                        throw new Exception("Registro non trovato");
                    }
                    else
                    {
                        output.CodeRegisterOrRF = reg.codRegistro;
                    }
                }
                if (!string.IsNullOrEmpty(corr.tipoCorrispondente))
                {
                    output.CorrespondentType = corr.tipoCorrispondente;
                    if (corr.tipoCorrispondente == "P")
                    {
                        output.Name = (string.IsNullOrEmpty(corr.nome) ? "" : corr.nome);
                        output.Surname = (string.IsNullOrEmpty(corr.cognome) ? "" : corr.cognome);
                    }
                }
                output.Description = corr.descrizione;
                output.Email = corr.email ?? string.Empty;
                output.OtherEmails = new List<string>();
                if (corr.Emails != null)
                {
                    var otherEmails = corr.Emails.Where(r => r.Principale == "0").ToList();
                    if (otherEmails.Count() > 0)
                    {
                        foreach (var email in otherEmails)
                        {
                            output.OtherEmails.Add(email.Email);
                        }
                    }

                    if (corr.Emails.Count > 0)
                    {
                        output.EmailsDetailed = new List<CorrespondentEmail>();
                        foreach (var email in corr.Emails)
                        {
                            output.EmailsDetailed.Add(new CorrespondentEmail()
                            {
                                Email = email.Email,
                                Note = email.Note,
                                Main = email.Principale
                            });
                        }
                    }
                }
                output.Fax = corr.fax;
                output.Id = corr.systemId;
                output.Location = corr.localita;
                output.Nation = corr.nazionalita;
                output.NationalIdentificationNumber = corr.codfisc;
                output.Note = corr.note;
                output.PhoneNumber = corr.telefono1;
                output.PhoneNumber2 = corr.telefono2;
                output.IsCommonAddress = corr.inRubricaComune;
                if (corr.canalePref != null && !string.IsNullOrEmpty(corr.canalePref.descrizione))
                {
                    output.PreferredChannel = corr.canalePref.descrizione;
                }
                output.Province = corr.prov;
                if (corr.tipoIE != null)
                {
                    output.Type = corr.tipoIE;
                }

                if (corr.systemId != null)
                {
                    DocsPaVO.utente.Corrispondente dettaglio = DBUtils.GetDettaglioCorrispondente(corr.systemId, dbContext);
                    if (dettaglio != null)
                    {
                        output.Address = dettaglio.indirizzo;
                        output.Cap = dettaglio.cap;
                        output.Province = dettaglio.prov;
                        output.Nation = dettaglio.nazionalita;
                        output.NationalIdentificationNumber = dettaglio.codfisc;
                        output.PhoneNumber = dettaglio.telefono1;
                        output.PhoneNumber2 = dettaglio.telefono2;
                        output.Fax = dettaglio.fax;
                        output.Note = dettaglio.note;
                        output.City = dettaglio.citta;
                        output.VatNumber = dettaglio.partitaiva;
                        output.Location = dettaglio.localita;
                    }
                }
            }
            else
            {
                output = null;
            }

            return output;
        }


        public static Correspondent GetCorrespondentFromCorrispondente(IPi3DbContext dbContext, DocsPaVO.utente.Corrispondente corrispondente)
        {
            if (corrispondente == null)
                return null;

            var corr = new Correspondent()
            {
                Id = corrispondente.systemId,
                Code = corrispondente.codiceRubrica,
                CorrespondentType = corrispondente.tipoCorrispondente,
                Type = corrispondente.tipoIE,
                Description = corrispondente.descrizione,
                AOOCode = corrispondente.codiceAOO,
                AdmCode = corrispondente.codiceAmm,
                Email = corrispondente.email,
                IsCommonAddress = corrispondente.inRubricaComune,
                Name = corrispondente.nome,
                Surname = corrispondente.cognome
            };

            if (!string.IsNullOrEmpty(corrispondente.idRegistro))
            {
                var reg = DBUtils.getRegistro(corrispondente.idRegistro, dbContext);
                if (reg != null) corr.CodeRegisterOrRF = reg.codRegistro;
            }

            if (corrispondente.inRubricaComune)
            {
                corr.Address = corrispondente.indirizzo;
                corr.Cap = corrispondente.cap;
                corr.Note = corrispondente.note;
                corr.City = corrispondente.citta;
                corr.Fax = corrispondente.fax;
                corr.Location = corrispondente.localita;
                corr.Nation = corrispondente.nazionalita;
                corr.NationalIdentificationNumber = corrispondente.codfisc;
                corr.PhoneNumber = corrispondente.telefono1;
                corr.PhoneNumber2 = corrispondente.telefono2;
                corr.Province = corrispondente.prov;
                corr.VatNumber = corrispondente.partitaiva;
            }
            else
            {
                var idCorr = corrispondente.systemId;
                var dettagli = (from b in dbContext.DettGlobaliEntities
                                where b.ID_CORR_GLOBALI == idCorr.AsLong()
                                select b).FirstOrDefault();

                if (dettagli != null && dettagli.ID_CORR_GLOBALI > 0)
                {
                    corr.Address = dettagli.VAR_INDIRIZZO;
                    corr.Cap = dettagli.VAR_CAP;
                    corr.Note = dettagli.VAR_NOTE;
                    corr.City = dettagli.VAR_CITTA;
                    corr.Fax = dettagli.VAR_FAX;
                    corr.Location = dettagli.VAR_LOCALITA;
                    corr.Nation = dettagli.VAR_NAZIONE;
                    corr.NationalIdentificationNumber = dettagli.VAR_COD_FISC;
                    corr.PhoneNumber = dettagli.VAR_TELEFONO;
                    corr.PhoneNumber2 = dettagli.VAR_TELEFONO2;
                    corr.Province = dettagli.VAR_PROVINCIA;
                    corr.VatNumber = dettagli.VAR_COD_PI;
                }
                string canalePreferenziale = (from a in dbContext.DocumentTypesEntities
                                              join b in dbContext.CanaleCorrEntities on a.SYSTEM_ID equals b.ID_DOCUMENTTYPE
                                              where b.ID_CORR_GLOBALE == idCorr.AsLong() && b.CHA_PREFERITO == "1"
                                              select a.DESCRIPTION).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(canalePreferenziale)) corr.PreferredChannel = canalePreferenziale;
            }

            return corr;
        }

        public static Project GetProjectFromSearchObject(IPi3DbContext dbContext, DocsPaVO.Grids.SearchObject obj, bool estrazioneTemplates, DocsPaVO.Grid.Field[] visibilityFields, bool prelievoRegistro)
        {
            Project result = new Project();
            string value = string.Empty;
            if (obj != null)
            {
                result.Id = obj.SearchObjectID;
                result.Number = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P14")).FirstOrDefault().SearchObjectFieldValue;
                result.ClassificationScheme = new();
                result.ClassificationScheme.Description = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P10")).FirstOrDefault().SearchObjectFieldValue;
                result.Code = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P3")).FirstOrDefault().SearchObjectFieldValue;
                result.Description = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P4")).FirstOrDefault().SearchObjectFieldValue;
                result.CreationDate = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P5")).FirstOrDefault().SearchObjectFieldValue;
                result.ClosureDate = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P6")).FirstOrDefault().SearchObjectFieldValue ?? string.Empty;
                if (!string.IsNullOrEmpty(obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P9")).FirstOrDefault().SearchObjectFieldValue) && obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P9")).FirstOrDefault().SearchObjectFieldValue.Equals("1"))
                {
                    result.Private = true;
                }
                if (!string.IsNullOrEmpty(obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P11")).FirstOrDefault().SearchObjectFieldValue) && obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P9")).FirstOrDefault().SearchObjectFieldValue.Equals("1"))
                {
                    result.Paper = true;
                }
                result.Type = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P1")).FirstOrDefault().SearchObjectFieldValue;
                if (!string.IsNullOrEmpty(obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P16")).FirstOrDefault().SearchObjectFieldValue))
                {
                    if (obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P16")).FirstOrDefault().SearchObjectFieldValue.Equals("A"))
                    {
                        result.Open = true;
                    }
                }
                // errore nell'estrazione del template. P16 non riguarda il template ma lo stato. Modifico l'if
                //if (!string.IsNullOrEmpty(obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P16")).FirstOrDefault().SearchObjectFieldValue))
                if (!string.IsNullOrEmpty(obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ID_TIPO_FASC")).FirstOrDefault().SearchObjectFieldValue) &&
                    !string.IsNullOrEmpty(obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("U1")).FirstOrDefault().SearchObjectFieldValue))
                {
                    result.Template = new Template();
                    result.Template.Id = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ID_TIPO_FASC")).FirstOrDefault().SearchObjectFieldValue;
                    result.Template.Name = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("U1")).FirstOrDefault().SearchObjectFieldValue;
                    if (estrazioneTemplates)
                    {
                        List<Field> campiPIS = new List<Field>();
                        Field campoPIS = null;
                        foreach (DocsPaVO.Grid.Field campoProf in visibilityFields)
                        {
                            campoPIS = new Field();
                            campoPIS.Id = campoProf.CustomObjectId.ToString();
                            campoPIS.Name = campoProf.Label;
                            campoPIS.Value = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(campoProf.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            campiPIS.Add(campoPIS);
                        }
                        if (campiPIS.Count > 0)
                        {
                            result.Template.Fields = campiPIS.ToArray();
                        }
                    }
                }

                // Verifico la presenza di un registro
                if (prelievoRegistro && !string.IsNullOrEmpty(obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ID_REGISTRO")).FirstOrDefault().SearchObjectFieldValue))
                {
                    string idRegister = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ID_REGISTRO")).FirstOrDefault().SearchObjectFieldValue;
                    var registro = DBUtils.getRegistro(idRegister, dbContext);
                    if (registro != null)
                    {
                        result.Register = RestUtils.GetRegister(registro);
                    }
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        public static Register GetRegister(DocsPaVO.utente.Registro registro)
        {
            Register result = new Register();
            if (registro != null)
            {
                result.Code = registro.codRegistro;
                result.Description = registro.descrizione;
                result.Id = registro.systemId;
                if (!string.IsNullOrEmpty(registro.chaRF) && registro.chaRF.Equals("1"))
                {
                    result.IsRF = true;
                }
                if (!string.IsNullOrEmpty(registro.stato))
                {
                    if (registro.stato.Equals("A"))
                    {
                        result.State = "Open";
                    }
                    else
                    {
                        result.State = "Closed";
                    }
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        public static Role getRole(DocsPaVO.utente.Ruolo ruolo)
        {
            Role retval = new Role();
            if (ruolo != null)
            {
                retval.Code = ruolo.codice;
                retval.Description = ruolo.descrizione;
                retval.Id = ruolo.idGruppo;
                if (ruolo.registri != null && ruolo.registri.Count() > 0)
                {
                    Register regD = null;
                    retval.Registers = new Register[ruolo.registri.Count()];
                    int i = 0;
                    foreach (DocsPaVO.utente.Registro reg in ruolo.registri)
                    {
                        regD = GetRegister(reg);
                        retval.Registers[i] = regD;
                        i++;
                    }
                }
            }
            return retval;
        }

        public static User getUser(DocsPaVO.utente.Utente utente)
        {
            User retval = new User();
            if (utente != null)
            {
                retval.Id = utente.idPeople;
                retval.Description = utente.descrizione;
                retval.Name = utente.nome;
                retval.Surname = utente.cognome;
                retval.UserId = utente.userId;
                retval.NationalIdentificationNumber = utente.codfisc;
            }
            else retval = null;
            return retval;
        }

        public static TransmissionModel GetModel(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione model)
        {
            TransmissionModel result = new TransmissionModel();
            if (model == null)
            {
                result = null;
                //modello di trasmissione non trovato
                throw new RestException("TRANSMISSION_MODEL_NOT_FOUND");
            }
            else
            {
                result.Code = model.CODICE;
                result.Description = model.NOME;
                result.Id = model.SYSTEM_ID.ToString();
                result.Type = model.CHA_TIPO_OGGETTO;
            }
            return result;
        }

        #endregion

        #region Mappatura aggregati in Model Rest


        public static Document getDocFromAggregate(DocumentoAmministrativo aggregate, IPi3DbContext dbContext, IMediator mediator = null,
            labelPdf position = null, bool getFile = false, bool segnatura = false, bool timbro = false, bool fileConFirma = false)
        {
            Document responseDoc = new Document();
            responseDoc.Id = aggregate.Id;
            responseDoc.Object = aggregate.OggettoDelDocumento.Descrizione.Value;
            responseDoc.DocNumber = aggregate.Id;
            responseDoc.Signature = aggregate.IdDoc.Segnatura;
            responseDoc.CreationDate = aggregate.CreationDate.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            responseDoc.Annulled = aggregate.Annullamento != null && aggregate.Annullamento.Motivo != null && !string.IsNullOrWhiteSpace(aggregate.Annullamento.Motivo.Value);
            responseDoc.MeansOfSending = aggregate.MezzoSpedizione == null || aggregate.MezzoSpedizione.Descrizione == null ? "" : aggregate.MezzoSpedizione.Descrizione.Value;
            responseDoc.InBasket = aggregate.InRecycleBin;
            //TODO: Proprietà da popolare
            if (aggregate.TipologiaFlusso != null)
            {
                switch (aggregate.TipologiaFlusso)
                {
                    case TipologiaFlussoEnum.E:
                        responseDoc.DocumentType = "A";
                        break;
                    case TipologiaFlussoEnum.U:
                        responseDoc.DocumentType = "P";
                        break;
                    case TipologiaFlussoEnum.I:
                        responseDoc.DocumentType = "I";
                        break;
                    default:
                        responseDoc.DocumentType = "G";
                        break;
                }
            }
            else
            {
                responseDoc.DocumentType = "G";
            }
            if (aggregate.IdDoc != null && !string.IsNullOrWhiteSpace(aggregate.IdDoc.Segnatura) &&
                aggregate.DatiRegistrazione != null && aggregate.DatiRegistrazione.GetType() == typeof(DatiRegistrazioneProtocollo))
            {
                responseDoc.ProtocolNumber = ((DatiRegistrazioneProtocollo)(aggregate.DatiRegistrazione)).NumeroProtocollo.ToString();
                responseDoc.ProtocolDate = ((DatiRegistrazioneProtocollo)(aggregate.DatiRegistrazione)).DataProtocollazione.Value.ToString("dd/MM/yyyy");
                responseDoc.ProtocolYear = ((DatiRegistrazioneProtocollo)(aggregate.DatiRegistrazione)).DataProtocollazione.Value.Year.ToString();
            }

            if (aggregate.Mittente != null && !string.IsNullOrEmpty(aggregate.Mittente.Id))
            {
                responseDoc.Sender = DBUtils.GetCorrespondentFromDB(aggregate.Mittente.Id, dbContext);
            }

            if (aggregate.DatiRegistrazione != null && !string.IsNullOrWhiteSpace(aggregate.DatiRegistrazione.IdRegistro))
            {
                responseDoc.Register = GetRegister(DBUtils.getRegistro(aggregate.DatiRegistrazione.IdRegistro, dbContext));
            }

            if (aggregate.Destinatari != null && aggregate.Destinatari.Any())
            {
                List<Correspondent> dests = new List<Correspondent>();
                foreach (var dest in aggregate.Destinatari)
                {
                    dests.Add(DBUtils.GetCorrespondentFromDB(dest.Id, dbContext));
                }
                responseDoc.Recipients = dests.ToArray();
            }

            if (aggregate.DestinatariCc != null && aggregate.DestinatariCc.Any())
            {
                List<Correspondent> dests = new List<Correspondent>();
                foreach (var dest in aggregate.DestinatariCc)
                {
                    dests.Add(DBUtils.GetCorrespondentFromDB(dest.Id, dbContext));
                }
                responseDoc.RecipientsCC = dests.ToArray();
            }

            if (aggregate.MittentiMultipli != null && aggregate.MittentiMultipli.Any())
            {
                List<Correspondent> sends = new List<Correspondent>();
                foreach (var sender in aggregate.MittentiMultipli)
                {
                    sends.Add(DBUtils.GetCorrespondentFromDB(sender.Id, dbContext));
                }
                responseDoc.MultipleSenders = sends.ToArray();
            }

            // non mi pare ci sia una proprietà nell aggregato, lo deduco
            responseDoc.Predisposed = responseDoc.DocumentType != "G" && string.IsNullOrWhiteSpace(responseDoc.Signature);


            responseDoc.PrivateDocument = aggregate.Riservato.GetValueOrDefault();

            responseDoc.PersonalDocument = aggregate.Reserved;
            responseDoc.IsAttachments = (aggregate.IdDocPrimario != null);
            responseDoc.ConsolidationState = null;
            if (aggregate.ProtocolloMittente != null && !string.IsNullOrWhiteSpace(aggregate.ProtocolloMittente.Segnatura))
            {
                responseDoc.ProtocolSender = aggregate.ProtocolloMittente.Segnatura;
                responseDoc.DataProtocolSender = aggregate.ProtocolloMittente.Data.Value.ToString("dd/MM/yyyy");
                responseDoc.ArrivalDate = aggregate.ProtocolloMittente.DataArrivo.Value.ToString("dd/MM/yyyy");

            }
            responseDoc.Template = null;

            responseDoc.MainDocument = null;
            responseDoc.Attachments = null;
            responseDoc.IdParent = aggregate.IdParentDocument;
            responseDoc.ParentDocument = null;
            responseDoc.LinkedDocuments = null;

            var profileEntity = dbContext.ProfileEntities.AsNoTracking().FirstOrDefault(p => p.SYSTEM_ID == aggregate.Id.AsLong());
            if (profileEntity != null)
            {
                if (profileEntity.CHA_TIPO_PROTO == "A" ||
                    profileEntity.CHA_TIPO_PROTO == "P" ||
                    profileEntity.CHA_TIPO_PROTO == "I"
                    )
                {
                    responseDoc.ProtocolNumber = profileEntity.NUM_PROTO.ToString();
                    responseDoc.ProtocolDate = profileEntity.DTA_PROTO?.ToString("dd/MM/yyyy");
                    responseDoc.Signature = profileEntity.VAR_SEGNATURA?.ToString();
                    responseDoc.ProtocolYear = profileEntity.NUM_ANNO_PROTO.ToString();
                }
            }

            //if (aggregate.IdDocPrimario != null && !string.IsNullOrEmpty(aggregate.IdDocPrimario.Identiticativo))
            //{

            var versionEntity = (from versions in dbContext.VersionEntities.Where(v => v.VERSION_ID == aggregate.CurrentVersion.Id.AsLong())
                                 join people in dbContext.PeopleEntities
                                     on versions.AUTHOR equals people.SYSTEM_ID
                                 join peopleDelegato in dbContext.PeopleEntities
                                     on versions.ID_PEOPLE_DELEGATO equals peopleDelegato.SYSTEM_ID into delegato
                                 from peopleDelegato in delegato.DefaultIfEmpty()
                                 join firmaVers in dbContext.FirmaVersEntities
                                    on versions.VERSION_ID equals firmaVers.ID_VERSIONE into firmaVersions
                                 from firmaVers in firmaVersions.DefaultIfEmpty()
                                 join firmatari in dbContext.FirmatarioEntities
                                      on firmaVers.ID_FIRMATARIO equals firmatari.SYSTEM_ID into firmatario
                                 from firmatari in firmatario.DefaultIfEmpty()
                                 where versions.DOCNUMBER == profileEntity.SYSTEM_ID
                                 select new { Version = versions, AuthorUserId = people.USER_ID, DelegatoUserId = peopleDelegato.USER_ID, Firmatario = firmatari })
                                   .AsNoTracking()
                                   .FirstOrDefault();

            if (mediator != null)
            {
                var componentEntities = (from components in dbContext.ComponentEntities
                                         join people in dbContext.PeopleEntities
                                             on components.ID_PEOPLE_PUTFILE equals people.SYSTEM_ID into autore
                                         from people in autore.DefaultIfEmpty()
                                         join peopleDelegato in dbContext.PeopleEntities
                                            on components.ID_PEOPLE_DELEGATO_PUTFILE equals peopleDelegato.SYSTEM_ID into delegato
                                         from peopleDelegato in delegato.DefaultIfEmpty()
                                         where components.DOCNUMBER == profileEntity.SYSTEM_ID
                                         select new { Component = components, AuthorUserId = people.USER_ID, DelegatoUserId = peopleDelegato.USER_ID })
                                            .AsNoTracking()
                                            .ToList();

                var componentEntity = componentEntities
                .Where(c => c.Component.VERSION_ID == aggregate.CurrentVersion.Id.AsLong())
                .Select(c => c.Component)
                .FirstOrDefault();

                var documento = new DocsPaVO.documento.Documento();

                documento.autore = versionEntity.AuthorUserId;
                documento.autoreFile = (componentEntity != null ? componentEntity.ID_PEOPLE_PUTFILE.ToString() : null);
                documento.cartaceo = versionEntity.Version.CARTACEO.GetValueOrDefault() > 0;
                documento.conSegnaturaPermanente = versionEntity.Version.CHA_SEGNATURA == "1";
                documento.dataAcquisizione = (componentEntity != null ? componentEntity.DTA_FILE_ACQUIRED.AsDateTimeFormat() : null);
                documento.dataInserimento = versionEntity.Version.DTA_CREAZIONE.AsDateTimeFormat();
                documento.descrizione = versionEntity.Version.COMMENTS;
                documento.docNumber = versionEntity.Version.DOCNUMBER.ToString();
                documento.fileName = ((componentEntity != null && componentEntity.VAR_NOMEORIGINALE != null) ? componentEntity.VAR_NOMEORIGINALE : string.Empty);
                documento.fileSize = (componentEntity != null ? (componentEntity.FILE_SIZE ?? 0).ToString() : null);
                documento.firmato = (componentEntity != null ? componentEntity.CHA_FIRMATO : string.Empty);
                if (versionEntity.Firmatario != null)
                {
                    documento.firmatari[0] =
                    new DocsPaVO.documento.Firmatario()
                    {
                        systemId = versionEntity.Firmatario.SYSTEM_ID.ToString(),
                        cognome = versionEntity.Firmatario.VAR_COGNOME,
                        nome = versionEntity.Firmatario.VAR_NOME,
                        codiceFiscale = versionEntity.Firmatario.VAR_COD_FISCALE
                    };
                }
                documento.idPeople = versionEntity.Version.AUTHOR.ToString();
                documento.idPeopleDelegato = versionEntity.Version.ID_PEOPLE_DELEGATO.ToString();
                documento.impronta = (componentEntity != null ? componentEntity.VAR_IMPRONTA : null);
                documento.inLibroFirma = profileEntity.IN_LIBROFIRMA == "1";
                documento.path = (componentEntity != null ? componentEntity.PATH : null);
                documento.repositoryContext = null;
                documento.subVersion = versionEntity.Version.SUBVERSION;
                documento.tipoFirma = (componentEntity != null ? componentEntity.CHA_TIPO_FIRMA : null);
                documento.version = versionEntity.Version.VERSION.GetValueOrDefault().ToString();
                documento.versionId = versionEntity.Version.VERSION_ID.GetValueOrDefault().ToString();
                documento.versionLabel = versionEntity.Version.VERSION_LABEL;
                documento.dataArrivo = versionEntity.Version.DTA_ARRIVO?.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);


                var getFileTask = mediator.Send(new GetFileCommand()
                {
                    getFile = getFile,
                    timbro = timbro,
                    segnatura = segnatura,
                    position = position,
                    fileConFirma = fileConFirma,
                    fileRequest = (DocsPaVO.documento.FileRequest)documento
                });

                getFileTask.Wait();
                responseDoc.MainDocument = (getFileTask.Result).File;
            }
            responseDoc.ArrivalDate = versionEntity.Version.DTA_ARRIVO?.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            //}



            return responseDoc;
        }

        public static TransmissionModel getTransmModFromAggregate(ModelloTrasmissione aggregate)
        {
            TransmissionModel retval = new TransmissionModel();

            retval.Id = aggregate.Id;
            retval.Description = aggregate.Name.Value;
            retval.Code = string.Concat("MT_", aggregate.Id);
            switch (aggregate.TipoOggettoTrasmesso)
            {
                case Core.AggregateModels.ModelloTrasmissioneAggregate.ValueObjects.TipiOggettiTrasmessiEnum.DocumentoAmministrativo:
                    retval.Type = "D";
                    break;
                case Core.AggregateModels.ModelloTrasmissioneAggregate.ValueObjects.TipiOggettiTrasmessiEnum.Fascicolo:
                    retval.Type = "F";
                    break;
            }

            return retval;
        }

        public static Project getProjFromAggregate(AggregazioneDocumentale aggregate, IPi3DbContext dbContext)
        {
            //TODO associazione campi

            Project retval = new Project();
            retval.ClassificationScheme = null;
            retval.ClosureDate = aggregate.DataChiusura != null ? aggregate.DataChiusura.Value.ToString("dd/MM/yyyy") : null;
            retval.Code = aggregate.DatiRegistrazione.Codice;
            retval.CodeNodeClassification = null;
            retval.CollocationDate = null;
            retval.Controlled = false;
            retval.CreationDate = aggregate.CreationDate.ToString("dd/MM/yyyy");
            retval.Description = aggregate.Description.Value;
            retval.Id = aggregate.Id;
            retval.IdParent = null;
            retval.Note = null;
            retval.Number = aggregate.Progressivo.ToString();
            retval.Open = aggregate.DataChiusura == null;
            retval.OpeningDate = aggregate.DataApertura.ToString("dd/MM/yyyy");
            retval.Paper = false;
            retval.PhysicsCollocation = null;
            retval.Private = aggregate.TipologiaVisibilita == Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.TipologieVisibilitaEnum.Privata;
            retval.Register = GetRegister(DBUtils.getRegistro(aggregate.DatiRegistrazione.IdRegistro, dbContext));
            retval.Template = null;
            retval.Type = null;


            return retval;
        }

        #endregion

        #region Mappatura da Model Rest a DocsPaVO
        public static DocsPaVO.ProfilazioneDinamica.Templates GetTemplatesFromTemplate(Template templatePis, IPi3DbContext dbContext)
        {
            long idTemplate = templatePis.Id.AsLong();
            //oggetto da restituire
            DocsPaVO.ProfilazioneDinamica.Templates templateToReturn = new();

            //prendo i valori dal db per popolare l'oggetto templateToReturn
            var q1 = dbContext.TipoAttoEntities
                .Join(dbContext.AssociazioneTemplatesEntities, ta => ta.SYSTEM_ID, at => at.ID_TEMPLATE, (ta, at) => new { ta, at });

            var q2 = q1
                //.Where(x => string.IsNullOrEmpty(x.at.DOC_NUMBER))
                .Join(dbContext.OggettiCustomEntities, q1 => q1.at.ID_OGGETTO, oc => oc.SYSTEM_ID, (q1, oc) => new { q1.ta, q1.at, oc });

            var q3 = q2
                .Join(dbContext.OggettiCustomCompEntities, q2 => q2.oc.SYSTEM_ID, occ => occ.ID_OGG_CUSTOM, (q2, occ) => new { q2.ta, q2.at, q2.oc, occ });

            var q4 = q3
                .Join(dbContext.TipoOggettoEntities, q3 => q3.oc.ID_TIPO_OGGETTO, to => to.SYSTEM_ID, (q3, to) => new { q3.ta, q3.at, q3.oc, q3.occ, to });

            var values = q4.Where(x => x.ta.VAR_DESC_ATTO.ToUpper().Equals(templatePis.Name.ToUpper()) && string.IsNullOrEmpty(x.at.DOC_NUMBER) && x.occ.ID_TEMPLATE == x.ta.SYSTEM_ID).ToList();
            var t = dbContext.TipoAttoEntities
                .FirstOrDefault(x => x.SYSTEM_ID == idTemplate);
            var v = values[0];

            //assegnazione campi
            int result = 0;
            if (int.TryParse(templatePis.Id, out result))
            {
                templateToReturn.SYSTEM_ID = result;
            }
            templateToReturn.DESCRIZIONE = templatePis.Name;
            templateToReturn.DOC_NUMBER = v.at.DOC_NUMBER;
            templateToReturn.ABILITATO_SI_NO = t.ABILITATO_SI_NO.ToString();
            templateToReturn.IN_ESERCIZIO = t.IN_ESERCIZIO;
            templateToReturn.NUM_MESI_CONSERVAZIONE = t.NUM_MESI_CONSERVAZIONE.ToString();
            templateToReturn.IS_TYPE_INSTANCE = Convert.ToChar(t.IS_TYPE_INSTANCE);
            templateToReturn.INVIO_CONSERVAZIONE = t.CHA_INVIO_CONSERVAZIONE ?? "0";
            templateToReturn.PATH_MODELLO_1 = t.PATH_MOD_1;
            templateToReturn.PATH_MODELLO_2 = t.PATH_MOD_2;
            templateToReturn.PATH_MODELLO_STAMPA_UNIONE = t.PATH_MOD_SU;
            templateToReturn.PATH_ALLEGATO_1 = t.PATH_ALL_1;
            templateToReturn.PATH_MODELLO_EXCEL = t.PATH_MOD_EXC;
            templateToReturn.PATH_XSD_ASSOCIATO = t.PATH_XSD_ASSOCIATO;
            templateToReturn.CHA_ASSOC_MANUALE = t.CHA_ASSOC_MANUALE ?? "0";
            templateToReturn.ID_TIPO_ATTO = t.SYSTEM_ID.ToString();
            //templateToReturn.ID_TIPO_FASC = t.CHA_INVIO_CONSERVAZIONE ?? "0";
            templateToReturn.SCADENZA = t.GG_SCADENZA.ToString();
            templateToReturn.PRE_SCADENZA = t.GG_PRE_SCADENZA.ToString();
            templateToReturn.PRIVATO = t.CHA_PRIVATO ?? "0";
            templateToReturn.IPER_FASC_DOC = (t.IPERDOCUMENTO != null && t.IPERDOCUMENTO == 1) ? "1" : "0";
            templateToReturn.ID_AMMINISTRAZIONE = t.ID_AMM.ToString();
            templateToReturn.CODICE_MODELLO_TRASM = t.COD_MOD_TRASM;
            templateToReturn.CODICE_CLASSIFICA = t.COD_CLASS;
            templateToReturn.PATH_MODELLO_1_EXT = t.EXT_MOD_1;
            templateToReturn.PATH_MODELLO_2_EXT = t.EXT_MOD_2;
            //templateToReturn.OLD_OGG_CUSTOM = t.CHA_INVIO_CONSERVAZIONE ?? "0";
            //templateToReturn.ID_TEMPLATE_STRUTTURA = t.CHA_INVIO_CONSERVAZIONE ?? "0";
            templateToReturn.ID_CONTESTO_PROCEDURALE = t.ID_CONTESTO_PROCEDURALE.ToString();

            DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = null;
            List<DocsPaVO.ProfilazioneDinamica.OggettoCustom> elencoOggettiList = new List<DocsPaVO.ProfilazioneDinamica.OggettoCustom>();
            values.ForEach(v =>
            {
                //Imposto i valori degli oggetti custom dei tipi oggetto
                oggettoCustom = new DocsPaVO.ProfilazioneDinamica.OggettoCustom
                {
                    SYSTEM_ID = Convert.ToInt32(v.oc.SYSTEM_ID),
                    CAMPO_DI_RICERCA = v.oc.CAMPO_DI_RICERCA,
                    CAMPO_OBBLIGATORIO = v.oc.CAMPO_OBBLIGATORIO,
                    ASTERISCO_OBBLIGATORIETA = v.oc.CAMPO_OBBLIGATORIO,
                    DESCRIZIONE = v.oc.DESCRIZIONE,
                    MULTILINEA = v.oc.MULTILINEA,
                    NUMERO_DI_LINEE = v.oc.NUMERO_DI_LINEE,
                    ORIZZONTALE_VERTICALE = v.oc.ORIZZONTALE_VERTICALE,
                    POSIZIONE = v.occ.POSIZIONE.ToString(),
                    CAMPO_XML_ASSOC = v.occ.CAMPO_XML_ASSOC,
                    OPZIONI_XML_ASSOC = v.occ.OPZIONI_XML_ASSOC,
                    RESETTA_CONTATORE_INIZIO_ANNO = v.oc.RESET_ANNO,
                    FORMATO_CONTATORE = v.oc.FORMATO_CONTATORE,
                    TIPO_RICERCA_CORR = v.oc.RICERCA_CORR,
                    ID_RUOLO_DEFAULT = v.oc.ID_R_DEFAULT,
                    CAMPO_COMUNE = v.oc.CAMPO_COMUNE.ToString(),
                    TIPO_CONTATORE = v.oc.CHA_TIPO_TAR,
                    CONTA_DOPO = v.oc.CONTA_DOPO.ToString(),
                    REPERTORIO = v.oc.REPERTORIO.ToString(),
                    CONS_REPERTORIO = v.oc.CHA_CONS_REPERTORIO,
                    DA_VISUALIZZARE_RICERCA = v.oc.DA_VISUALIZZARE_RICERCA.ToString(),
                    ANNO = v.at.ANNO.ToString(),
                    VALORE_DATABASE = v.at.VALORE_OGGETTO_DB,
                    ID_AOO_RF = v.at.ID_AOO_RF != null ? v.at.ID_AOO_RF.ToString() : "0",
                    FORMATO_ORA = v.oc.FORMATO_ORA,
                    TIPO_LINK = v.oc.TIPO_LINK,
                    TIPO_OBJ_LINK = v.oc.TIPO_OBJ_LINK,
                    MODULO_SOTTOCONTATORE = v.oc.MODULO_SOTTOCONTATORE.ToString(),
                    VALORE_SOTTOCONTATORE = v.at.VALORE_SC.ToString(),
                    DATA_INSERIMENTO = v.at.DTA_INS.ToString(),
                    DATA_ANNULLAMENTO = v.at.DTA_ANNULLAMENTO.ToString(),
                    CODICE_DB = v.at.CODICE_DB,
                    MANUAL_INSERT = v.at.MANUAL_INSERT != null && v.at.MANUAL_INSERT == 1,
                    ANNO_ACC = v.at.ANNO_ACC,
                    CONSOLIDAMENTO = v.oc.CHA_CONSOLIDAMENTO ?? "0",
                    CONSERVAZIONE = v.oc.CHA_CONSERVAZIONE ?? "0",
                };

                //Controllo contatore
                if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_CONTATORE))
                {
                    var contCustomDoc = dbContext.ContCustomDocEntities
                        .Where(x => x.ID_OGG == oggettoCustom.SYSTEM_ID)
                        .ToList()
                        .FirstOrDefault();

                    if (contCustomDoc != null)
                    {
                        oggettoCustom.DATA_INIZIO = contCustomDoc.DATA_INIZIO.ToString();
                        oggettoCustom.DATA_FINE = contCustomDoc.DATA_FINE.ToString();
                    }
                }

                //set tipo oggetto
                oggettoCustom.TIPO = new DocsPaVO.ProfilazioneDinamica.TipoOggetto
                {
                    SYSTEM_ID = Convert.ToInt32(v.to.SYSTEM_ID),
                    DESCRIZIONE_TIPO = v.to.DESCRIZIONE
                };

                if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("OGGETTOESTERNO"))
                {
                    string config = dbContext.OggettiCustomEntities
                        .Where(x => x.SYSTEM_ID == oggettoCustom.SYSTEM_ID)
                        .Select(x => x.CONFIG_OBJ_EST)
                        .FirstOrDefault()
                        .ToString();
                    oggettoCustom.CONFIG_OBJ_EST = config;
                }

                //Seleziono i valori per l'oggettoCustom
                var oggCustomValues = dbContext.AssociazioneValoriEntities
                    .Where(x => x.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID)
                    .ToList();
                List<DocsPaVO.ProfilazioneDinamica.ValoreOggetto> elencoValoriList = new List<DocsPaVO.ProfilazioneDinamica.ValoreOggetto>();
                List<string> valoriSel = new();
                oggCustomValues.ForEach(v =>
                {
                    DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoreOggetto = new DocsPaVO.ProfilazioneDinamica.ValoreOggetto
                    {
                        SYSTEM_ID = Convert.ToInt32(v.SYSTEM_ID),
                        DESCRIZIONE_VALORE = v.DESCRIZIONE_VALORE,
                        VALORE = v.VALORE,
                        VALORE_DI_DEFAULT = v.VALORE_DI_DEFAULT,
                        COLOR_BG = v.COLOR_BG,
                        ABILITATO = Convert.ToInt32(v.ABILITATO)
                    };
                    elencoValoriList.Add(valoreOggetto);
                    valoriSel.Add(string.Empty);
                });

                oggettoCustom.ELENCO_VALORI = elencoValoriList.ToArray();
                oggettoCustom.VALORI_SELEZIONATI = valoriSel.ToArray();
                elencoOggettiList.Add(oggettoCustom);
            });
            templateToReturn.ELENCO_OGGETTI = elencoOggettiList.ToArray();



            return templateToReturn;
        }
        #endregion

        public static DocsPaVO.utente.Corrispondente GetCorrespondentFromPis(Correspondent corr, DocsPaVO.utente.InfoUtente infoUtente, IPi3DbContext pi3DbContext)
        {
            DocsPaVO.utente.Corrispondente result = new DocsPaVO.utente.Corrispondente();

            DocsPaVO.addressbook.DettagliCorrispondente dettagli = new DocsPaVO.addressbook.DettagliCorrispondente();

            if (corr != null)
            {
                if (corr.CorrespondentType != null && corr.CorrespondentType.Equals("U"))
                {
                    result = new DocsPaVO.utente.UnitaOrganizzativa();
                    result.tipoCorrispondente = "U";
                    result.info = new DocsPaVO.addressbook.DettagliCorrispondente();
                    result.codiceCorrispondente = corr.Code;
                    result.codiceRubrica = corr.Code;
                    result.codiceAmm = corr.AdmCode;
                    result.codiceAOO = corr.AOOCode;
                    result.descrizione = corr.Description;
                    result.idAmministrazione = infoUtente.idAmministrazione;
                    result.localita = corr.Location;

                    if (!string.IsNullOrEmpty(corr.CodeRegisterOrRF))
                    {
                        DocsPaVO.utente.Registro reg = DBUtils.getRegistroByCodAOO(corr.CodeRegisterOrRF, infoUtente.idAmministrazione, pi3DbContext);
                        if (reg == null)
                        {
                            //Registro non trovato
                            throw new RestException("REGISTER_NOT_FOUND");
                        }
                        else
                        {
                            result.idRegistro = reg.systemId;
                        }
                    }


                    dettagli.Corrispondente.AddCorrispondenteRow(
                        corr.Address,
                        corr.City,
                        corr.Cap,
                        corr.Province,
                        corr.Nation,
                        corr.PhoneNumber,
                        string.Empty,
                        corr.Fax,
                        corr.NationalIdentificationNumber,
                        corr.Note,
                        corr.Location,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        corr.VatNumber
                        //, string.Empty
                        );

                    result.email = corr.Email;
                    result.info = dettagli;
                    result.dettagli = true;
                    if (corr.PreferredChannel != null)
                    {

                        var mezziSpedizione = DBUtils.ListaMezziSpedizione(infoUtente.idAmministrazione, true, pi3DbContext);
                        foreach (DocsPaVO.amministrazione.MezzoSpedizione m_spediz in mezziSpedizione)
                        {
                            if (m_spediz.Descrizione.ToUpper().Equals(corr.PreferredChannel.ToUpper()))
                            {
                                result.canalePref = new DocsPaVO.utente.Canale();
                                result.canalePref.descrizione = m_spediz.Descrizione;
                                result.canalePref.systemId = m_spediz.IDSystem;
                                result.canalePref.tipoCanale = m_spediz.chaTipoCanale;
                                break;
                            }
                        }

                    }
                }

                if (corr.CorrespondentType != null && corr.CorrespondentType.Equals("P"))
                {
                    result = new DocsPaVO.utente.Utente();
                    result.codiceCorrispondente = corr.Code;
                    result.codiceRubrica = corr.Code;
                    result.cognome = corr.Surname;
                    result.nome = corr.Name;
                    result.codiceAmm = corr.AdmCode;
                    result.codiceAOO = corr.AOOCode;
                    result.descrizione = corr.Description;
                    result.idAmministrazione = infoUtente.idAmministrazione;

                    if (!string.IsNullOrEmpty(corr.CodeRegisterOrRF))
                    {
                        DocsPaVO.utente.Registro reg = DBUtils.getRegistroByCodAOO(corr.CodeRegisterOrRF, infoUtente.idAmministrazione, pi3DbContext);
                        if (reg == null)
                        {
                            //Registro non trovato
                            throw new RestException("REGISTER_NOT_FOUND");
                        }
                        else
                        {
                            result.idRegistro = reg.systemId;
                        }
                    }

                    result.tipoCorrispondente = "P";
                    result.email = corr.Email;

                    dettagli.Corrispondente.AddCorrispondenteRow(
                        corr.Address,
                        corr.City,
                        corr.Cap,
                        corr.Province,
                        corr.Nation,
                        corr.PhoneNumber,
                        string.Empty,
                        corr.Fax,
                        corr.NationalIdentificationNumber,
                        corr.Note,
                        corr.Location,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        corr.VatNumber
                        );

                    result.info = dettagli;
                    result.dettagli = true;
                }

                //Occasionale
                if (corr.Type == null || corr.Type.Equals("O"))
                {
                    result.codiceCorrispondente = corr.Code;
                    result.codiceRubrica = corr.Code;
                    result.cognome = corr.Surname;
                    result.nome = corr.Name;
                    result.codiceAmm = corr.AdmCode;
                    result.codiceAOO = corr.AOOCode;
                    result.descrizione = corr.Description;
                    result.idAmministrazione = infoUtente.idAmministrazione;
                    result.partitaiva = corr.VatNumber;
                    result.codfisc = corr.NationalIdentificationNumber;

                    if (!string.IsNullOrEmpty(corr.CodeRegisterOrRF))
                    {
                        DocsPaVO.utente.Registro reg = DBUtils.getRegistroByCodAOO(corr.CodeRegisterOrRF, infoUtente.idAmministrazione, pi3DbContext);
                        if (reg == null)
                        {
                            throw new RestException("REGISTER_NOT_FOUND");
                        }
                        else
                        {
                            result.idRegistro = reg.systemId;
                        }
                    }

                    result.tipoCorrispondente = "O";
                    result.email = corr.Email;
                }
                if (corr != null && !string.IsNullOrEmpty(corr.Type))
                {
                    result.tipoIE = corr.Type;
                }
            }
            else
            {
                result = null;
            }

            return result;
        }
        public static TipologiaFlussoEnum? AsTipologiaFlusso(string tipoProto)
        {
            switch (tipoProto)
            {
                case "A":
                    return TipologiaFlussoEnum.E;
                case "P":
                    return TipologiaFlussoEnum.U;
                case "I":
                    return TipologiaFlussoEnum.I;
                default:
                    return null;
            }
        }

        public static DocsPaVO.filtri.FiltroRicerca[] AddToArrayFiltroRicerca(DocsPaVO.filtri.FiltroRicerca[] array, DocsPaVO.filtri.FiltroRicerca nuovoElemento)
        {
            DocsPaVO.filtri.FiltroRicerca[] nuovaLista;
            if (array != null)
            {
                int len = array.Length;
                nuovaLista = new DocsPaVO.filtri.FiltroRicerca[len + 1];
                array.CopyTo(nuovaLista, 0);
                nuovaLista[len] = nuovoElemento;
                return nuovaLista;
            }
            else
            {
                nuovaLista = new DocsPaVO.filtri.FiltroRicerca[1];
                nuovaLista[0] = nuovoElemento;
                return nuovaLista;
            }
        }


        public static void CheckFilterTypes(Filter[] filter)
        {
            if (filter != null)
            {
                foreach (Filter f in filter)
                {
                    if (f != null)
                    {
                        switch (f.Type)
                        {
                            case FilterTypeEnum.Number:
                                try
                                {
                                    Convert.ToInt32(f.Value);
                                }
                                catch (Exception e)
                                {
                                    throw new RestException("INVALID_FIELD_FORMAT_NUMBER");
                                }
                                break;

                            case FilterTypeEnum.String:
                                break;

                            case FilterTypeEnum.Bool:
                                try
                                {
                                    Convert.ToBoolean(f.Value);
                                    break;
                                }
                                catch (Exception e)
                                {
                                    throw new RestException("INVALID_FIELD_FORMAT_BOOL");
                                }

                            case FilterTypeEnum.Date:
                                try
                                {
                                    /*
                                     *  Date Format Example: 01/03/2012
                                     *  dd = day (01)
                                     *  MM = month (03)
                                     *  yyyy = year (2012)
                                     *  / = date separator
                                     */

                                    string pattern = "dd/MM/yyyy";
                                    DateTime dateVal;
                                    if (!DateTime.TryParseExact(f.Value, pattern, null, System.Globalization.DateTimeStyles.None, out dateVal))
                                    {
                                        throw new RestException("INVALID_FIELD_FORMAT_DATE");
                                    }
                                }
                                catch (Exception e)
                                {
                                    throw new RestException("INVALID_FIELD_FORMAT_DATE");
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}
