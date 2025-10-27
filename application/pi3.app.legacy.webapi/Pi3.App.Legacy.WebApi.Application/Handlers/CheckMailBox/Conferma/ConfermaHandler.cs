// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Interoperabilita.Segnatura;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using DocsPaVO.utente;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Core.Services.Email.BoxScanner;


using AddressbookGetCorrispondenteCompletoBySystemIdRequest = Pi3.App.Legacy.WebApi.Application.Requests.AddressbookGetCorrispondenteCompletoBySystemId;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using DocsPaVO.ProfilazioneDinamicaLite;


namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Conferma
{
    internal class ConfermaHandler : IRequestHandler<ConfermaRequest, ConfermaResult>
    {
        string errorMessage = string.Empty;
        string motivo = string.Empty;
        ConfermaType conferma = null!;
        XmlDocument doc = new XmlDocument();
        XmlTextReader xtr = null;
        MemoryStream stream;
        XmlValidatingReader xvr;
        string codiceAmministrazioneMitt, codiceAOOMitt = string.Empty, CodiceRegistroMitt = string.Empty, numeroRegistrazioneMitt = string.Empty, descrizioneMessaggio = string.Empty;
        XmlElement elIdentificatore;
        DateTime dataRegistrazioneMitt = DateTime.MinValue;
        CultureInfo ci;
        int? ProcessedAttachments;
        string DescrizioneMessaggioMitt = string.Empty;
        string idProf = null;
        string[] formati = { "yyyy-MM-dd" };

        #region Public members
        public ConfermaHandler(ILogger<ConfermaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IConfigurationService configurationService,
            IWebMethodLoggerService webMethodLoggerService
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._configurationService = configurationService;
            this._webMethodLoggerService = webMethodLoggerService;
        }
        public async Task<ConfermaResult> Handle(ConfermaRequest request, CancellationToken cancellationToken)
        {

            InteropResolver interopResolver = new InteropResolver();

            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

            var ruoloEntity = await this._dbContext.CorrGlobaliEntities.FirstAsync(x => x.ID_GRUPPO == idGroup);
            var idRole = ruoloEntity?.SYSTEM_ID;

            var email = request.email;
            var message = request.message;

            stream = new MemoryStream(message.AttachmentContent);
            xtr = new XmlTextReader(stream);
            xtr.WhitespaceHandling = WhitespaceHandling.None;

            xvr = new XmlValidatingReader(xtr)
            {
                ValidationType = System.Xml.ValidationType.DTD,
                EntityHandling = System.Xml.EntityHandling.ExpandCharEntities,
                XmlResolver = interopResolver
            };


            try
            {
                doc.Load(xvr);

                ValidaRicevuta valid = new ValidaRicevuta(doc.OuterXml, message.AttachmentContent, out errorMessage);

                if (!string.IsNullOrEmpty(errorMessage))
                    _logger.LogDebug(errorMessage);

                if (valid.ValidationErrorList.Count > 0)
                    throw new System.Xml.Schema.XmlSchemaException();
            }

            #region catch
            catch (XmlSchemaException e)
            {
                _logger.LogError("La mail viene sospesa perché il file confermaRicezione.xml non è valido. Eccezione: " + e.Message);

                var mailElaborataEntity = new MailElaborataEntity()
                {
                    VAR_MESSAGE = request.MailId,
                    CHA_RAGIONE_ELAB = "D",
                    DTA_ELAB = DateTime.Now,
                    ID_REGISTRO = null,
                    ID_PROFILE = null,
                    VAR_EMAIL = request.MailAddress
                };

                await _dbContext.MailElaborataEntities.AddAsync(mailElaborataEntity);
                int rowAffected = await ((DbContext)this._dbContext).SaveChangesAsync(cancellationToken);

                if (rowAffected > 0)
                    _logger.LogDebug("Sospensione eseguita");
                else
                    _logger.LogDebug("Sospensione non eseguita");

                return new ConfermaResult(new ProcessorOutput { Success = false, ErrorMessage = "File non valido" });
            }
            catch (Exception e)
            {
                _logger.LogError("La mail viene sospesa. Eccezione: " + e.Message);

                var mailElaborataEntity = new MailElaborataEntity()
                {
                    VAR_MESSAGE = request.MailId,
                    CHA_RAGIONE_ELAB = "U",
                    DTA_ELAB = DateTime.Now,
                    ID_REGISTRO = null,
                    ID_PROFILE = null,
                    VAR_EMAIL = request.MailAddress
                };

                await _dbContext.MailElaborataEntities.AddAsync(mailElaborataEntity);
                int rowAffected = await ((DbContext)this._dbContext).SaveChangesAsync(cancellationToken);

                if (rowAffected > 0)
                    _logger.LogDebug("Sospensione eseguita");
                else
                    _logger.LogDebug("Sospensione non eseguita");

                return new ConfermaResult(new ProcessorOutput { Success = false, ErrorMessage = "Errore generico" });
            }

            #endregion
            finally
            {
                xvr.Close();
                xtr.Close();
                stream.Close();
            }

            try
            {
                ci = new("it-IT");



                string idProf = string.Empty;
                ProcessedAttachments = message.ProcessedAttachments;

                message.Subject = email.Subject.ToString();
                if (!string.IsNullOrEmpty(doc.DocumentElement.NamespaceURI) || doc.DocumentElement.NamespaceURI.Contains(Resources.www_digitPa_gov_it_protocollo))
                {
                    return await ProcessaXmlConfermaConforme(message, errorMessage);
                }
                else
                {
                    return await ProcessaXmlConfermaNonConforme(request.Reg, idGroup, message, email, errorMessage);

                }

            }
            catch (Exception e)
            {
                _logger.LogError("Errore nell'elaborazione del file: " + e.Message);
                return new ConfermaResult(new ProcessorOutput { Success = false, ErrorMessage = "Errore di elaborazione" });
            }

        }





        #endregion

        #region Private members
        protected readonly ILogger<ConfermaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        private long? _idRegister = null;
        private async Task<ConfermaResult> ProcessaXmlConfermaNonConforme(Registro reg, long idGroup, AnalyzedMessage message, Email email, string errorMessage)
        {
            try
            {
                string codiceAmministrazione;
                string codiceAOO;
                string numeroRegistrazione;
                string descrizioneMessaggioMitt = string.Empty;
                DateTime dataRegistrazione;
                XmlElement elIdentificatore, elIdentificatoreMitt;


                var xmlnsManager = new XmlNamespaceManager(doc.NameTable);
                xmlnsManager.AddNamespace("ns", "http://www.digitPa.gov.it/protocollo/");


                elIdentificatore = (XmlElement)doc.DocumentElement.SelectSingleNode("//ns:Identificatore", xmlnsManager);
                codiceAmministrazione = elIdentificatore.SelectSingleNode("ns:CodiceAmministrazione", xmlnsManager).InnerText.Trim();
                codiceAOO = elIdentificatore.SelectSingleNode("ns:CodiceAOO", xmlnsManager).InnerText.Trim();
                numeroRegistrazione = elIdentificatore.SelectSingleNode("ns:NumeroRegistrazione", xmlnsManager).InnerText.Trim();
                dataRegistrazione = DateTime.ParseExact(elIdentificatore.SelectSingleNode("ns:DataRegistrazione", xmlnsManager).InnerText.Trim(), formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);



                //info sul messaggio
                elIdentificatoreMitt = (XmlElement)doc.DocumentElement.SelectSingleNode("//ns:MessaggioRicevuto/ns:Identificatore", xmlnsManager);
                if (elIdentificatoreMitt != null)
                {
                    string codiceAmministrazioneMitt, codiceAOOMitt, numeroRegistrazioneMitt, CodiceRegistroMitt;
                    DateTime dataRegistrazioneMitt;

                    codiceAmministrazioneMitt = elIdentificatoreMitt.SelectSingleNode("ns:CodiceAmministrazione", xmlnsManager).InnerText.Trim();
                    codiceAOOMitt = elIdentificatoreMitt.SelectSingleNode("ns:CodiceAOO", xmlnsManager).InnerText.Trim();
                    numeroRegistrazioneMitt = elIdentificatoreMitt.SelectSingleNode("ns:NumeroRegistrazione", xmlnsManager).InnerText.Trim();
                    CodiceRegistroMitt = elIdentificatoreMitt.SelectSingleNode("ns:CodiceRegistro", xmlnsManager).InnerText.Trim();
                    dataRegistrazioneMitt = DateTime.ParseExact(elIdentificatoreMitt.SelectSingleNode("ns:DataRegistrazione", xmlnsManager).InnerText, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);

                    // Trova il numero del documento
                    string idProf = string.Empty;
                    if (!string.IsNullOrEmpty(numeroRegistrazioneMitt))
                    {
                        idProf = await FindIdProfile(CodiceRegistroMitt, numeroRegistrazioneMitt, dataRegistrazioneMitt.Year);
                    }
                    else if (!string.IsNullOrEmpty(descrizioneMessaggioMitt))
                    {
                        int start = descrizioneMessaggioMitt.IndexOf("#") + 1;
                        int end = descrizioneMessaggioMitt.LastIndexOf("#");
                        idProf = descrizioneMessaggioMitt.Substring(start, end - start);
                    }

                    if (string.IsNullOrEmpty(idProf))
                    {
                        errorMessage = Conferma.ErrorDescription.XmlConfermaDocNotFound;
                        _logger.LogError(Conferma.ErrorDescription.XmlConfermaDocNotFound);
                        if (await MailElaborata(message.Id, "U"))
                        {
                            _logger.LogDebug("Sospensione eseguita");
                        }
                        else
                        {
                            _logger.LogDebug("Sospensione non eseguita");
                        }
                    }
                    await AggiornaDpa_StatoInvioConConferma(idProf, codiceAOO, codiceAmministrazione, dataRegistrazione, numeroRegistrazione, dataRegistrazione.Year, CodiceRegistroMitt);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            };
            return new ConfermaResult(new ProcessorOutput
            {
                Success = true,
                //DocNumber = notifica.docnumber?.AsLong(),
                ErrorMessage = errorMessage,
                ProcessedAttachments = ++ProcessedAttachments
            });
        }

        private async Task<ConfermaResult> ProcessaXmlConfermaConforme(AnalyzedMessage message, string errorMessage)
        {
            try
            {
                XmlSerializer serializer = new(typeof(ConfermaType));
                MemoryStream fs = new(message.AttachmentContent);
                using (StreamReader reader = new(fs))
                {
                    conferma = (ConfermaType)serializer.Deserialize(reader);
                }
                string codiceAmministrazione = conferma.Identificatore.CodiceAmministrazione.Value.Trim();
                string codiceAOO = conferma.Identificatore.CodiceAOO.Value.Trim();
                string numeroRegistrazione = conferma.Identificatore.NumeroRegistrazione.Trim();
                string codiceRegistro = conferma.Identificatore.CodiceRegistro.Trim();
                DateTime dataRegistrazione = conferma.Identificatore.DataRegistrazione;

                try
                {
                    //info sul messaggio
                    for (int i = 0; i < conferma.MessaggioRicevuto.Items.Length; i++)
                    {
                        if (conferma.MessaggioRicevuto.ItemsElementName[i] == ItemsChoiceType3.Identificatore)
                        {
                            codiceAmministrazioneMitt = (conferma.MessaggioRicevuto.Items[i] as IdentificatoreType).CodiceAmministrazione.Value.Trim();
                            codiceAOOMitt = (conferma.MessaggioRicevuto.Items[i] as IdentificatoreType).CodiceAOO.Value.Trim();
                            numeroRegistrazioneMitt = (conferma.MessaggioRicevuto.Items[i] as IdentificatoreType).NumeroRegistrazione.Trim();
                            CodiceRegistroMitt = (conferma.MessaggioRicevuto.Items[i] as IdentificatoreType).CodiceRegistro.Trim();
                            dataRegistrazioneMitt = (conferma.MessaggioRicevuto.Items[i] as IdentificatoreType).DataRegistrazione;
                        }
                        if (conferma.MessaggioRicevuto.ItemsElementName[i] == ItemsChoiceType3.DescrizioneMessaggio)
                        {
                            DescrizioneMessaggioMitt = (conferma.MessaggioRicevuto.Items[i] as string).ToString();
                        }
                    }
                    if (!string.IsNullOrEmpty(numeroRegistrazioneMitt))
                    {
                        idProf = await FindIdProfile(CodiceRegistroMitt, numeroRegistrazioneMitt, dataRegistrazioneMitt.Year);
                    }
                    else if (!string.IsNullOrEmpty(DescrizioneMessaggioMitt))
                    {
                        int start = DescrizioneMessaggioMitt.IndexOf("#") + 1;
                        int end = DescrizioneMessaggioMitt.LastIndexOf("#");
                        idProf = DescrizioneMessaggioMitt.Substring(start, end - start);
                    }

                }
                catch
                {
                    _logger.LogError(Conferma.ErrorDescription.XmlConfermaNoData);



                }
                finally
                {
                    fs.Close();
                }



                if (idProf == null && !string.IsNullOrEmpty(DescrizioneMessaggioMitt))
                {
                    try
                    {
                        idProf = ExtractDocNumberFromSubject(DescrizioneMessaggioMitt);

                    }
                    catch
                    {
                        _logger.LogDebug(Conferma.ErrorDescription.XmlConfermaNoData);

                    }
                }

                if (string.IsNullOrEmpty(idProf))
                {
                    errorMessage = Conferma.ErrorDescription.XmlConfermaDocNotFound;
                    _logger.LogError(Conferma.ErrorDescription.XmlConfermaDocNotFound);
                    if (await MailElaborata(message.Id, "U"))
                    {
                        _logger.LogDebug("Sospensione eseguita");
                    }
                    else
                    {
                        _logger.LogDebug("Sospensione non eseguita");
                    };
                    return new ConfermaResult(new ProcessorOutput
                    {
                        Success = false,
                        DocNumber = null,
                        ErrorMessage = errorMessage,
                        ProcessedAttachments = ProcessedAttachments
                    });

                }
                await AggiornaDpa_StatoInvioConConferma(idProf, codiceAOO, codiceAmministrazione, dataRegistrazione, numeroRegistrazione, dataRegistrazione.Year, codiceRegistro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);


            };
            return new ConfermaResult(new ProcessorOutput
            {
                Success = true,
                //DocNumber = notifica.docnumber?.AsLong(),
                ErrorMessage = errorMessage,
                ProcessedAttachments = ++ProcessedAttachments
            });
        }


        private async Task<string?> FindIdProfile(string codiceRegistro, string numeroRegistrazione, int year)
        {
            try
            {

                var value = await this._dbContext.ProfileEntities.AsNoTracking().
                    Join(this._dbContext.RegistroEntities, a => a.ID_REGISTRO, b => b.SYSTEM_ID, (a, b) => new { a, b })
                    .Where(
                    c => c.b.VAR_CODICE.ToUpper() == codiceRegistro.ToUpper() && c.a.NUM_PROTO == numeroRegistrazione.AsLong() && c.a.NUM_ANNO_PROTO == year)
                    .Select(c => c.a.SYSTEM_ID).FirstOrDefaultAsync();

                if (value == null)
                {
                    value = await this._dbContext.ProfileEntities.AsNoTracking().
                        Join(this._dbContext.RegistroEntities, a => a.ID_REGISTRO, b => b.SYSTEM_ID, (a, b) => new { a, b })
                        .Where(
                        c => c.b.VAR_CODICE.ToUpper() == codiceRegistro.ToUpper() && c.a.NUM_PROTO == numeroRegistrazione.AsLong() && c.a.NUM_ANNO_PROTO == year - 1)
                        .Select(c => c.a.SYSTEM_ID).FirstOrDefaultAsync();
                }

                return value.ToString();

            }
            catch (Exception e)
            {
                _logger.LogError("Eccezione: " + e.Message);
                return null;
            }
        }

        private string? ExtractDocNumberFromSubject(string descrizioneMessaggio)
        {
            string retval = string.Empty;
            if (descrizioneMessaggio.Contains("#"))
            {
                string[] split = descrizioneMessaggio.Split('#');
                if (split.Length > 1)
                    retval = split[1];
            }
            retval = retval.Replace("#", string.Empty);
            return retval;
        }

        private async Task<bool> MailElaborata(string emailID, string ragione)
        {
            try
            {

                //DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                //obj.insMailElab(mailId, v);

                //Aggiornamento DPA_MAIL_ELABORATE
                var mailElaborataEntity = new MailElaborataEntity()
                {
                    VAR_MESSAGE = emailID,
                    CHA_RAGIONE_ELAB = ragione,
                    DTA_ELAB = DateTime.Now,
                    ID_REGISTRO = this._idRegister ?? null,
                    ID_PROFILE = null,
                    VAR_EMAIL = null
                };

                await this._dbContext.MailElaborataEntities.AddAsync(mailElaborataEntity);

                await ((DbContext)this._dbContext).SaveChangesAsync();



                _logger.LogDebug($"Elaborazione della mail con id: {0} avvenuta con successo", emailID);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(Conferma.ErrorDescription.XmlConfermaElabFail, emailID, e.Message);

                return false;
            }
        }
        private async Task<bool> AggiornaDpa_StatoInvioConConferma(string idProf, string codiceAOO, string codiceAmministrazione, DateTime data, string numeroRegistrazione, int anno, string codiceRegistro)
        {
            bool res = false;
            string statusmask = await GetStatusMask1(idProf, codiceAOO, codiceAmministrazione);
            if (!string.IsNullOrEmpty(statusmask))
            {
                char[] sm = statusmask.ToCharArray();
                sm[3] = 'V';
                sm[0] = 'V';
                if (sm[5] != 'X')
                    sm[5] = 'N';
                statusmask = new string(sm);
            }
            var record = await this._dbContext.StatoInvioEntities.Where(e => e.ID_PROFILE.ToString().Equals(idProf) && e.VAR_CODICE_AOO == codiceAOO && e.VAR_CODICE_AMM == codiceAmministrazione).ToListAsync();


            record.ForEach(si =>
            {
                si.VAR_PROTO_DEST = $"{numeroRegistrazione}/{codiceRegistro}/{anno}";
                si.DTA_PROTO_DEST = data;

                if (!string.IsNullOrEmpty(statusmask))
                {
                    si.STATUS_C_MASK = statusmask;
                }
                else
                {
                    si.STATUS_C_MASK = "VVVVANN";
                }
            });


            int rowAffected = await ((DbContext)this._dbContext).SaveChangesAsync();

            if (rowAffected > 0)
                res = true;


            return res;
        }

        private async Task<DocsPaVO.utente.Corrispondente?> GetCorrispondenteBySystemID(string systemID)
        {

            DocsPaVO.utente.Corrispondente corrispondente = null;
            DocsPaVO.utente.Corrispondente corrExists = null;

            try
            {
                if (!string.IsNullOrEmpty(systemID))
                {
                    corrispondente = new DocsPaVO.utente.Corrispondente();
                    corrExists = new DocsPaVO.utente.Corrispondente();
                    bool existCorrispondente = false;
                    string idAmministrazione = string.Empty;

                    DocsPaVO.addressbook.TipoUtente tipoUtente = DocsPaVO.addressbook.TipoUtente.GLOBALE;

                    var corr = this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(cg => cg.SYSTEM_ID == systemID.AsLong());
                    corrExists = await corr.Select(cg => new DocsPaVO.utente.Corrispondente
                    {
                        systemId = cg.ID_AMM.ToString(),
                        tipoIE = cg.CHA_TIPO_IE
                    }).FirstOrDefaultAsync();


                    if (corrExists != null)
                    {
                        existCorrispondente = true;
                        idAmministrazione = corrExists.idAmministrazione ?? string.Empty;
                        if (corrExists.tipoIE.Equals("I"))
                            tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                        else if (corrExists.tipoIE.Equals("E"))
                            tipoUtente = DocsPaVO.addressbook.TipoUtente.ESTERNO;

                    }

                    if (existCorrispondente)
                        // Reperimento dei metadati del corrisponente

                        corrispondente = (await this._mediator.Send(new AddressbookGetCorrispondenteCompletoBySystemIdRequest(
                             systemID,
                             tipoUtente,
                             new InfoUtente()

                            ))).output;


                    else
                        _logger.LogError(string.Format(ErrorDescription.XmlConfermaCorrNotFound, systemID));



                    var enableFlussoAutomatico = await this._configurationService.GetValue<string>("FE_ENABLE_FLUSSO_AUTOMATICO");

                    if (!string.IsNullOrEmpty(enableFlussoAutomatico) && enableFlussoAutomatico.Equals("1"))
                    {
                        //Query flusso automatico
                        DocsPaVO.FlussoAutomatico.Flusso flusso = new DocsPaVO.FlussoAutomatico.Flusso();
                        var isInteroperanteRGS = this._dbContext.CorrInteropEntities.AsNoTracking()
                            .Where(ci => ci.ID_CORR == systemID.AsLong())
                            .Select(ci => ci.CHA_INTEROPERANTE_RGS).FirstOrDefaultAsync().ToString();
                        if (!string.IsNullOrEmpty(isInteroperanteRGS) && isInteroperanteRGS.Equals("1"))
                            corrispondente.interoperanteRGS = true;
                    }


                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(ErrorDescription.XmlConfermaCorrNotFound, systemID), ex);
            }
            finally
            {
                _logger.LogDebug("END - GetCorrispondenteBySystemID");
            }

            return corrispondente;
        }
        private async Task<string?> GetStatusMask1(string idProf, string codiceAOO, string codiceAmministrazione, string systemidDPASI = "")
        {
            string retVal = string.Empty;

            var statusMask = _dbContext.StatoInvioEntities.AsNoTracking();
            if (!string.IsNullOrEmpty(systemidDPASI))
            {
                statusMask = statusMask.Where(si => si.SYSTEM_ID.ToString() == systemidDPASI);
            }
            else
            {
                if (!string.IsNullOrEmpty(idProf))
                {
                    statusMask = statusMask.Where(si => si.ID_PROFILE.ToString() == idProf);
                }
                if (!string.IsNullOrEmpty(codiceAOO))
                {
                    statusMask = statusMask.Where(si => si.VAR_CODICE_AOO.ToUpper() == codiceAOO.ToUpper());
                }
                if (!string.IsNullOrEmpty(codiceAmministrazione))
                {
                    statusMask = statusMask.Where(si => si.VAR_CODICE_AMM.ToUpper() == codiceAmministrazione.ToUpper());
                }
            }

            retVal = await statusMask.Select(x => x.STATUS_C_MASK).FirstOrDefaultAsync();


            return retVal;
        }
        #endregion
    }
}
