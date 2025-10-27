// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.Interoperabilita.Segnatura;
using DocsPaVO.Spedizione;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using System.Data;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using static Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.CheckMailboxHandler;
using AddressbookGetCorrispondenteCompletoBySystemIdRequest = Pi3.App.Legacy.WebApi.Application.Requests.AddressbookGetCorrispondenteCompletoBySystemId;
using GetMailCorrEsternoRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetMailCorrEsterno;
using InteroperabilitaAggiornamentoConfermaRequest = Pi3.App.Legacy.WebApi.Application.Requests.InteroperabilitaAggiornamentoConferma;
using ricercaNotificaRequest = Pi3.App.Legacy.WebApi.Application.Requests.ricercaNotifica;
using getTipoNotificaRequest = Pi3.App.Legacy.WebApi.Application.Requests.getTipoNotifica;
using DocsPaVO.DatiCert;
using SpedizioneGetElementiStoricoSpedizioneRequest = Pi3.App.Legacy.WebApi.Application.Requests.SpedizioneGetElementiStoricoSpedizione;
using DocsPaVO.DiagrammaStato;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.AggiornaStatusMask;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Eccezione
{
    internal class EccezioneHandler : IRequestHandler<EccezioneRequest, EccezioneResult>
    {
        #region Public members
        public EccezioneHandler(ILogger<EccezioneHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IConfigurationService configurationService
        )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._configurationService = configurationService;

        }

        public bool controlloBloccante = true;
        public string eccezione_xml = null;
        public string numRegMitt = string.Empty;
        public bool documentOk = false;
        public string dettagli_eccezione = string.Empty;
        public bool searchAttachByDescription = false;


        public async Task<EccezioneResult> Handle(EccezioneRequest request, CancellationToken cancellationToken)
        {
            string errorMessage = string.Empty;
            var output = new ProcessorOutput();
            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var ruoloEntity = await this._dbContext.CorrGlobaliEntities.FirstAsync(x => x.ID_GRUPPO == idGroup);
            var idRole = ruoloEntity?.SYSTEM_ID;
            //string path;
            //string filename = 'eccezione.xml';
            string moreError;
            moreError = String.Empty;

            var message = request.message;
            var email = request.email;

            XmlDocument doc = new XmlDocument();
            InteropResolver my = new InteropResolver();
            //buffer
            XmlTextReader xtr = null;
            MemoryStream stream = new MemoryStream(message.AttachmentContent);
            xtr = new XmlTextReader(stream);
            xtr.WhitespaceHandling = WhitespaceHandling.None;
            XmlValidatingReader xvr = new XmlValidatingReader(xtr);
            xvr.EntityHandling = System.Xml.EntityHandling.ExpandCharEntities;
            xvr.XmlResolver = my;
            try
            {
                doc.Load(xvr);

                //Verifico la validità della segnatura con Xsd
                ValidaRicevuta valid = new ValidaRicevuta(doc.OuterXml, message.AttachmentContent, out errorMessage);
                if (valid.ValidationErrorList.Count > 0)
                    throw new System.Xml.Schema.XmlSchemaException();
            }
            catch (System.Xml.Schema.XmlSchemaException e)
            {
                _logger.LogError(ErrorDescription.XmlEccezioneInvalid + e.Message);
                moreError = ErrorDescription.XmlEccezioneInvalid + e.Message;

                if (await MailElaborata(message.Id, "D"))
                {
                    _logger.LogDebug(MessageDescription.Exec);
                }
                else
                {
                    _logger.LogDebug(MessageDescription.NonExec);
                }

                return new EccezioneResult(new ProcessorOutput
                {
                    Success = false,
                    DocNumber = 0,
                    ErrorMessage = moreError,
                    ProcessedAttachments = message.ProcessedAttachments
                });
            }
            catch (Exception e)
            {
                _logger.LogError(ErrorDescription.XmlEccezioneGeneric + e.Message);
                moreError = ErrorDescription.XmlEccezioneGeneric + e.Message;

                if (await MailElaborata(message.Id, "U"))
                {
                    _logger.LogDebug(MessageDescription.Exec);
                }
                else
                {
                    _logger.LogDebug(MessageDescription.NonExec);
                }

                return new EccezioneResult(new ProcessorOutput
                {
                    Success = false,
                    DocNumber = 0,
                    ErrorMessage = moreError,
                    ProcessedAttachments = message.ProcessedAttachments
                });
            }
            finally
            {
                xvr.Close();
                xtr.Close();
                stream.Close();

            }



            if (string.IsNullOrEmpty(doc.DocumentElement.NamespaceURI) || doc.DocumentElement.NamespaceURI.Contains(Resources.www_digitPa_gov_it_protocollo))
            {
                //return await processaXmlEccezioniOld(email, message, idGroup);
                return await processaXmlEccezioniOld(request.reg, idGroup, message, email, moreError);
            }
            else
            {
                //return await processaXmlEccezioni(email, message, idGroup);
                return await processaXmlEccezioni(request.reg, idGroup, message, email, moreError);
            }

        }



        #endregion

        #region Private members
        protected readonly ILogger<EccezioneHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IConfigurationService _configurationService;
        private long? _idRegister = null;

        private async Task<EccezioneResult> processaXmlEccezioni(Registro reg, long idGroup, AnalyzedMessage message, Core.Services.Email.BoxScanner.Email email, string moreError)
        {

            DocsPaVO.Interoperabilita.Segnatura.NotificaEccezioneType eccezione = null;
            string codiceAmministrazione = string.Empty;
            string docNumber = null;
            string motivo = null;
            string codiceAOO = string.Empty;
            string numeroRegistrazione = string.Empty;
            string codiceRegistro = string.Empty;
            DateTime dataRegistrazione = DateTime.MinValue;
            string DescrizioneMessaggio = string.Empty;
            int? ProcessedAttachments = message.ProcessedAttachments;

            //message.Subject = email.Subject.ToString();
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(NotificaEccezioneType));
                MemoryStream fs = new MemoryStream(message.AttachmentContent);
                using (StreamReader reader = new StreamReader(fs))
                {
                    eccezione = (NotificaEccezioneType)serializer.Deserialize(reader);
                }


                CultureInfo ci = new CultureInfo("it-IT");
                string[] formati = { "yyyy-MM-dd" };
                try
                {
                    //info sul messaggio
                    for (int i = 0; i < eccezione.MessaggioRicevuto.Items.Length; i++)
                    {
                        if (eccezione.MessaggioRicevuto.ItemsElementName[i] == ItemsChoiceType3.Identificatore)
                        {
                            codiceAmministrazione = (eccezione.MessaggioRicevuto.Items[i] as IdentificatoreType).CodiceAmministrazione.Value.Trim();
                            codiceAOO = (eccezione.MessaggioRicevuto.Items[i] as IdentificatoreType).CodiceAOO.Value.Trim();
                            numeroRegistrazione = (eccezione.MessaggioRicevuto.Items[i] as IdentificatoreType).NumeroRegistrazione.Trim();
                            codiceRegistro = (eccezione.MessaggioRicevuto.Items[i] as IdentificatoreType).CodiceRegistro.Trim();
                            dataRegistrazione = (eccezione.MessaggioRicevuto.Items[i] as IdentificatoreType).DataRegistrazione;
                        }
                        if (eccezione.MessaggioRicevuto.ItemsElementName[i] == ItemsChoiceType3.DescrizioneMessaggio)
                        {
                            DescrizioneMessaggio = (eccezione.MessaggioRicevuto.Items[i] as string).ToString();
                        }
                    }

                    //docNumber = Interoperabilità.InteroperabilitaUtils.findIdProfile(codiceAOO, numeroRegistrazione, dataRegistrazione.Year);
                    docNumber = await findIdProfile(codiceRegistro, numeroRegistrazione, dataRegistrazione.Year);

                }
                catch
                {
                    _logger.LogError(ErrorDescription.XmlEccezioneNoData);



                }
                finally
                {
                    fs.Close();
                }


                if (docNumber == null && !string.IsNullOrEmpty(DescrizioneMessaggio))
                {
                    try
                    {
                        docNumber = extractDocNumberFromSubject(DescrizioneMessaggio);

                    }
                    catch
                    {
                        _logger.LogDebug(ErrorDescription.XmlEccezioneNoData);

                    }
                }

                if (docNumber != null)
                {
                    motivo = eccezione.Motivo.Trim();

                }

                if (string.IsNullOrEmpty(docNumber))
                {
                    moreError = ErrorDescription.XmlEccezioneDocNotFound;
                    _logger.LogError(ErrorDescription.XmlEccezioneDocNotFound);
                    if (await MailElaborata(message.Id, "U"))
                    {
                        _logger.LogDebug(MessageDescription.Exec);
                    }
                    else
                    {
                        _logger.LogDebug(MessageDescription.NonExec);
                    };
                    return new EccezioneResult(new ProcessorOutput
                    {
                        Success = false,
                        DocNumber = null,
                        ErrorMessage = moreError,
                        ProcessedAttachments = ProcessedAttachments
                    });

                }


                await AggiornaDpa_StatoInvioConEccezione(email.Sender.Address, docNumber, motivo);


            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message);


            };


            Notifica? notifica = null;
            DocsPaVO.DatiCert.TipoNotifica tiponotifica = null;
            try
            {
                var ricercaNotificaResult = (await this._mediator.Send(new ricercaNotificaRequest(docNumber)));

                if (ricercaNotificaResult.output.Any())
                {
                    notifica = ricercaNotificaResult.output[0];
                }
                else
                {
                    notifica = new DocsPaVO.DatiCert.Notifica();
                    notifica.zona = "+100"; //default 
                }

                //Creazione in DPA_NOTIFICA  
                tiponotifica = await RicercaTipoNotificaByCodice("eccezione");

                if (tiponotifica == null)
                {
                    inserimentoTipoNotifica("eccezione");
                    tiponotifica = await RicercaTipoNotificaByCodice("eccezione");

                }

                notifica.docnumber = docNumber;
                notifica.oggetto = motivo;
                notifica.mittente = !string.IsNullOrEmpty(email.To.OfType<string>().ToList().FirstOrDefault()) ? email.To.OfType<string>().ToList().FirstOrDefault() : "";
                notifica.destinatario = email.Sender.Address;

                notifica.tipoDestinatario = "esterno";
                notifica.idTipoNotifica = tiponotifica.idTipoNotifica;
                notifica.msgid = message.Id;


                bool inserimento = await inserimentoNotifica(notifica, null);

                // Modifica PEC 4 requisito 2
                // Modifica dei destinatari delle modifiche, imitando il comportamento 

                if (!inserimento)
                    _logger.LogDebug(ErrorDescription.XmlEccezioneNotificationError + notifica.msgid);
                //else
                //    await this._mediator.Send(new AggiornaStatusMaskRequest(notifica));
                //


                string ruoloDestinatari = idGroup.ToString();


                // Recupero il ruolo che ha effettuato l'ultima spedizione PEC, dallo storico delle spedizioni. 
                var listHistorySendDoc = (await this._mediator.Send(new SpedizioneGetElementiStoricoSpedizioneRequest(
                                notifica.docnumber))).output;



                if (listHistorySendDoc != null && listHistorySendDoc.ToList().Count > 0)
                {
                    Object lastSendPec = (from record in listHistorySendDoc.ToArray()
                                          where ((ElStoricoSpedizioni)record).Mail.Equals(notifica.destinatario) && ((ElStoricoSpedizioni)record).Esito.Equals("Spedito")
                                          select record).ToList().OrderBy(z => ((ElStoricoSpedizioni)z).Id).LastOrDefault();
                    if (lastSendPec != null)
                    {
                        ruoloDestinatari = ((ElStoricoSpedizioni)lastSendPec).IdGroupSender;
                    }
                }

            }
            catch (Exception ex)
            {
                if (notifica.docnumber != null)
                    await this._webMethodLoggerService.LogKO("EXCEPTION_INTEROPERABILITY_PEC", notifica.docnumber, tiponotifica.descrizioneNotifica ?? string.Empty + " è stata inviata il '" + notifica.data_ora ?? string.Empty + "'"
                              + "'. Il destinatario '" + notifica.destinatario ?? string.Empty + "' ha una mail di tipo: '" + ((notifica.tipoDestinatario.ToUpper().Equals("ESTERNO") ? "MAIL NON CERTIFICATA" : notifica.tipoDestinatario) ?? string.Empty) + "'."
                                  + "Motivo: '" + notifica.oggetto ?? string.Empty + "'.");

                return new EccezioneResult(new ProcessorOutput
                {
                    Success = false,
                    DocNumber = notifica.docnumber?.AsLong(),
                    ErrorMessage = moreError,
                    ProcessedAttachments = 0
                });
            }
            //"DOCUMENTOADDDOCGRIGIA", schedaDocumento.systemId, string.Format(Resources.LogDocumentoAddDocGrigio, schedaDocumento.systemId)


            return new EccezioneResult(new ProcessorOutput
            {
                Success = true,
                DocNumber = notifica.docnumber?.AsLong(),
                ErrorMessage = moreError,
                ProcessedAttachments = +1
            });
        }

        private async Task<EccezioneResult> processaXmlEccezioniOld(Registro reg, long idGroup, AnalyzedMessage message, Core.Services.Email.BoxScanner.Email email, string moreError)
        {

            var output = new ProcessorOutput();

            string path;
            //string filename = 'eccezione.xml';
            DocsPaVO.Interoperabilita.Segnatura.NotificaEccezioneType eccezione = null;
            XmlDocument doc = new XmlDocument();
            InteropResolver my = new InteropResolver();
            //buffer
            XmlTextReader xtr = null;
            MemoryStream stream = new MemoryStream(message.AttachmentContent);
            xtr = new XmlTextReader(stream);
            //XmlTextReader(Stream input)
            //XmlTextReader xtr = new XmlTextReader(path + "\\" + filename);// { Namespaces = false };
            xtr.WhitespaceHandling = WhitespaceHandling.None;
            XmlValidatingReader xvr = new XmlValidatingReader(xtr);

            xvr.EntityHandling = System.Xml.EntityHandling.ExpandCharEntities;
            xvr.XmlResolver = my;
            int? ProcessedAttachments = message.ProcessedAttachments;

            try
            {
                doc.Load(xvr);
            }
            catch (System.Xml.Schema.XmlSchemaException e)
            {
                _logger.LogError(ErrorDescription.XmlEccezioneInvalid + e.Message);
                moreError = ErrorDescription.XmlEccezioneInvalid + e.Message;

                if (await MailElaborata(message.Id, "D"))
                {
                    _logger.LogDebug(MessageDescription.Exec);
                }
                else
                {
                    _logger.LogDebug(MessageDescription.NonExec);
                }

                return new EccezioneResult(new ProcessorOutput
                {
                    Success = false,
                    DocNumber = 0,
                    ErrorMessage = moreError,
                    ProcessedAttachments = 0
                });
            }
            catch (Exception e)
            {
                _logger.LogError(ErrorDescription.XmlEccezioneInvalid + e.Message);
                moreError = ErrorDescription.XmlEccezioneInvalid + e.Message;

                if (await MailElaborata(message.Id, "U"))
                {
                    _logger.LogDebug(MessageDescription.Exec);
                }
                else
                {
                    _logger.LogDebug(MessageDescription.NonExec);
                }

                return new EccezioneResult(new ProcessorOutput
                {
                    Success = false,
                    DocNumber = 0,
                    ErrorMessage = moreError,
                    ProcessedAttachments = 0
                });
            }
            finally
            {

                xvr.Close();
                xtr.Close();
                stream.Close();
            }

            string docNumber = null;
            string motivo = null;
            string codiceAmministrazione = string.Empty;
            string codiceAOO = string.Empty;
            string codiceRegistro = string.Empty;
            string numeroRegistrazione = string.Empty;
            DateTime dataRegistrazione = DateTime.MinValue;

            var xmlnsManager = new XmlNamespaceManager(doc.NameTable);
            xmlnsManager.AddNamespace("ns", "http://www.digitPa.gov.it/protocollo/");


            try
            {
                CultureInfo ci = new CultureInfo("it-IT");
                string[] formati = { "yyyy-MM-dd" };
                try
                {
                    XmlElement elIdentificatore = (XmlElement)doc.DocumentElement.SelectSingleNode("//ns:Identificatore", xmlnsManager);
                    //XmlElement elIdentificatore = (XmlElement)doc.DocumentElement.SelectSingleNode("MessaggioRicevuto/Identificatore");
                    codiceAmministrazione = elIdentificatore.SelectSingleNode("ns:CodiceAmministrazione", xmlnsManager).InnerText.Trim();
                    codiceAOO = elIdentificatore.SelectSingleNode("ns:CodiceAOO", xmlnsManager).InnerText.Trim();
                    codiceRegistro = elIdentificatore.SelectSingleNode("ns:CodiceRegistro", xmlnsManager).InnerText.Trim();
                    numeroRegistrazione = elIdentificatore.SelectSingleNode("ns:NumeroRegistrazione", xmlnsManager).InnerText.Trim();
                    dataRegistrazione = DateTime.ParseExact(elIdentificatore.SelectSingleNode("ns:DataRegistrazione", xmlnsManager).InnerText.Trim(), formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                    _logger.LogDebug("Ricerca id del profilo...");
                    docNumber = await findIdProfile(codiceRegistro, numeroRegistrazione, dataRegistrazione.Year);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ErrorDescription.XmlEccezioneNoData);
                }


                if (docNumber == null)
                {
                    try
                    {
                        XmlElement elMsgRicevuto = (XmlElement)doc.DocumentElement.SelectSingleNode("//ns:MessaggioRicevuto", xmlnsManager);
                        string DescrizioneMessaggio = elMsgRicevuto.SelectSingleNode("ns:DescrizioneMessaggio", xmlnsManager).InnerText.Trim();

                        docNumber = extractDocNumberFromSubject(DescrizioneMessaggio);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug(ErrorDescription.XmlEccezioneNoData); // ??? Va bene come log Debug? nel vecchio era così ma usa la stessa stringa di logError

                    }
                }

                if (docNumber != null)
                {
                    XmlElement elMotivo = (XmlElement)doc.DocumentElement.SelectSingleNode("ns:Motivo", xmlnsManager);
                    motivo = elMotivo.InnerText.Trim();

                }



                if (docNumber == null)
                {
                    moreError = ErrorDescription.XmlEccezioneDocNotFound;
                    _logger.LogError(ErrorDescription.XmlEccezioneDocNotFound);
                    if (await MailElaborata(message.Id, "U"))
                    {
                        _logger.LogDebug(MessageDescription.Exec);
                    }
                    else
                    {
                        _logger.LogDebug(MessageDescription.NonExec);
                    };
                    return new EccezioneResult(new ProcessorOutput
                    {
                        Success = false,
                        DocNumber = 0,
                        ErrorMessage = moreError,
                        ProcessedAttachments = ++ProcessedAttachments
                    });
                }

                await AggiornaDpa_StatoInvioConEccezione(email.Sender.Address, docNumber, motivo);


            }

            catch (Exception ex)
            {
                _logger.LogError(ErrorDescription.XmlEccezioneNoData);
            }

            Notifica? notifica = null;
            DocsPaVO.DatiCert.TipoNotifica tiponotifica = null;

            try
            {

                notifica = (await this._mediator.Send(new ricercaNotificaRequest(
                                docNumber

                               ))).output[0];

                //Creazione in DPA_NOTIFICA  
                tiponotifica = await RicercaTipoNotificaByCodice("eccezione");

                if (tiponotifica == null)
                {
                    inserimentoTipoNotifica("eccezione");
                    tiponotifica = await RicercaTipoNotificaByCodice("eccezione");

                }

                if (notifica == null)
                {
                    _logger.LogDebug(MessageDescription.NullNotification, docNumber);
                    notifica = new DocsPaVO.DatiCert.Notifica();
                    notifica.zona = "+100"; //default 
                }

                notifica.docnumber = docNumber;
                notifica.oggetto = motivo;
                notifica.mittente = !string.IsNullOrEmpty(email.To.OfType<string>().ToList().FirstOrDefault()) ? email.To.OfType<string>().ToList().FirstOrDefault() : "";
                notifica.destinatario = email.Sender.Address;

                notifica.tipoDestinatario = "esterno";
                notifica.idTipoNotifica = tiponotifica.idTipoNotifica;
                notifica.msgid = message.Id;

                bool inserimento = await inserimentoNotifica(notifica, null);
                // Modifica PEC 4 requisito 2
                // Modifica dei destinatari delle modifiche, imitando il comportamento IS

                if (!inserimento)
                    _logger.LogDebug(ErrorDescription.XmlEccezioneNotificationError + notifica.msgid);

                string ruoloDestinatari = idGroup.ToString();
                // Recupero il ruolo che ha effettuato l'ultima spedizione PEC, dallo storico delle spedizioni. 
                //ArrayList listHistorySendDoc = BusinessLogic.Spedizione.SpedizioneManager.GetElementiStoricoSpedizione(notifica.docnumber);
                var listHistorySendDoc = (await this._mediator.Send(new SpedizioneGetElementiStoricoSpedizioneRequest(
                                 notifica.docnumber

                                ))).output;

                if (listHistorySendDoc != null && listHistorySendDoc.ToList().Count > 0)
                {
                    Object lastSendPec = (from record in listHistorySendDoc.ToArray()
                                          where ((ElStoricoSpedizioni)record).Mail.Equals(notifica.destinatario) && ((ElStoricoSpedizioni)record).Esito.Equals("Spedito")
                                          select record).ToList().OrderBy(z => ((ElStoricoSpedizioni)z).Id).LastOrDefault();
                    if (lastSendPec != null)
                    {
                        ruoloDestinatari = ((ElStoricoSpedizioni)lastSendPec).IdGroupSender;
                    }
                }

            }
            catch (Exception ex)
            {
                if (notifica.docnumber != null)
                    await this._webMethodLoggerService.LogKO("EXCEPTION_INTEROPERABILITY_PEC", notifica.docnumber, tiponotifica.descrizioneNotifica + " è stata inviata il '" + notifica.data_ora + "'"
                                + "'. Il destinatario '" + notifica.destinatario + "' ha una mail di tipo: '" + (notifica.tipoDestinatario.ToUpper().Equals("ESTERNO") ? "MAIL NON CERTIFICATA" : notifica.tipoDestinatario) + "'."
                                    + "Motivo: '" + notifica.oggetto + "'.");

                return new EccezioneResult(new ProcessorOutput
                {
                    Success = false,
                    DocNumber = notifica.docnumber.AsLong(),
                    ErrorMessage = moreError,
                    ProcessedAttachments = 0
                });
            }

            return new EccezioneResult(new ProcessorOutput
            {
                Success = true,
                DocNumber = notifica.docnumber.AsLong(),
                ErrorMessage = moreError,
                ProcessedAttachments = +1
            });
        }

        //stava in InteroperabilitàUtils quindi messo qui dentro
        private async Task<string>? findIdProfile(string codiceRegistro, string numeroRegistrazione, int year)
        {
            try
            {

                var id = await this._dbContext.ProfileEntities.AsNoTracking().
                    Join(this._dbContext.RegistroEntities, a => a.ID_REGISTRO, b => b.SYSTEM_ID, (a, b) => new { a, b })
                    .Where(
                    c => c.b.VAR_CODICE.ToUpper() == codiceRegistro.ToUpper() && c.a.NUM_PROTO == numeroRegistrazione.AsLong() && ((int?)c.a.NUM_ANNO_PROTO) == year)
                    .Select(c => c.a.SYSTEM_ID).FirstOrDefaultAsync();

                if (id == null)
                {
                    id = await this._dbContext.ProfileEntities.AsNoTracking().
                        Join(this._dbContext.RegistroEntities, a => a.ID_REGISTRO, b => b.SYSTEM_ID, (a, b) => new { a, b })
                        .Where(
                        c => c.b.VAR_CODICE.ToUpper() == codiceRegistro.ToUpper() && c.a.NUM_PROTO == numeroRegistrazione.AsLong() && ((int?)c.a.NUM_ANNO_PROTO) == year - 1)
                        .Select(c => c.a.SYSTEM_ID).FirstOrDefaultAsync();
                }

                return id.ToString();

            }
            catch (Exception e)
            {
                _logger.LogError(ErrorDescription.XmlEccezioneBasic + e.Message);
                return null;
            }
        }

        //stava in InteroperabilitàUtils quindi messo qui dentro
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



                _logger.LogDebug(MessageDescription.ElabSuccess, emailID);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(ErrorDescription.XmlEccezioneElabFail, emailID, e.Message);

                return false;
            }
        }

        //OK 
        private string? extractDocNumberFromSubject(string descrizioneMessaggio)
        {
            string retval = string.Empty;
            if (descrizioneMessaggio.Contains("#"))
            {
                string[] split = descrizioneMessaggio.Split('#');
                if (split.Length > 1)
                    retval = split[1];
            }
            retval = retval.Replace("#", string.Empty);
            //try
            //{
            //    BusinessLogic.Documenti.DocManager.GetTipoDocumento(retval);
            //}
            //catch (Exception e)
            //{
            //    logger.ErrorFormat("Problemi nel reperimento del documento numero {0} errore {1}", retval, e.Message);
            //    retval = null;
            //}
            return retval;
        }

        //BENE QUI
        private async Task<bool> AggiornaDpa_StatoInvioConEccezione(string address, string docNumber, string? motivo)
        {  //Metodo ha problemi .. non sempre torna quello che vogliamo.
            //Avendo il DocNumber e l'email del mittente, cerco in dpa stato invio
            //Ma prima mi serve il corrid 

            //Dato un docnumber cerco tutte le entry nella  stato invio
            //ArrayList idCorrList = doc.GetIdCorrInStatoInvio(docNumber);
            var idCorrList = await this._dbContext.StatoInvioEntities.AsNoTracking()
                .Where(ie => ie.ID_PROFILE.Equals(docNumber.AsLong()))
                .Select(ie => ie.ID_CORR_GLOBALE.ToString()).ToListAsync();


            List<DocsPaVO.documento.ProtocolloDestinatario> pdList = new List<ProtocolloDestinatario>();

            foreach (string corrId in idCorrList)
            {

                DocsPaVO.utente.Corrispondente? corr = await this.getCorrispondenteBySystemID(corrId);
                //corr.Emails = this.GetMailCorrispondente(corrId);
                corr.Emails = (await this._mediator.Send(new GetMailCorrEsternoRequest(
                             corrId
                            ))).output;


                if ((corr.Emails == null) || (corr.Emails.Count == 0))
                {
                    List<MailCorrispondente> mcl = new List<MailCorrispondente>();
                    mcl.Add(new MailCorrispondente { Email = corr.email });
                    corr.Emails = mcl;
                }

                foreach (MailCorrispondente mcItem in corr.Emails)
                {
                    if (mcItem.Email == address)
                    {


                        var statoInvioAL = (await this._mediator.Send(new InteroperabilitaAggiornamentoConfermaRequest(
                             docNumber,
                             corr

                            ))).output;

                        foreach (DocsPaVO.documento.ProtocolloDestinatario p in statoInvioAL)
                            pdList.Add(p);
                    }
                }

            }

            if (pdList.Count == 1)
            {
                bool res_update = await updateStatoInvioAnnulla(pdList[0].systemId, motivo);
                if (!res_update)
                {
                    _logger.LogError(ErrorDescription.XmlEccezioneErroreUpProfile);
                    return false;
                }
            }
            else
            {
                _logger.LogDebug(MessageDescription.ManyDocuments, docNumber, pdList.Count());
                foreach (DocsPaVO.documento.ProtocolloDestinatario p in pdList)
                    _logger.LogDebug(MessageDescription.ProtInfo, p.systemId, p.protocolloDestinatario);

                //Urca! piu di uno e mo? bho, torno false..
                return false;
            }
            return true;
        }

        //BENE QUI fa due richieste con altri handler 
        private async Task<DocsPaVO.utente.Corrispondente?> getCorrispondenteBySystemID(string systemID)
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
                        _logger.LogError(string.Format(ErrorDescription.XmlEccezioneCorrNotFound, systemID));



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
                _logger.LogError(string.Format(ErrorDescription.XmlEccezioneCorrNotFound, systemID), ex);
            }
            finally
            {
                _logger.LogDebug("END - GetCorrispondenteBySystemID");
            }

            return corrispondente;
        }

        // ora nel nuovo handler
        private async Task<bool> updateStatoInvioAnnulla(string systemID, string motivo_annulla)
        {
            try
            {
                string statusmask = await getStatusMask1("", "", "", systemID);
                if (!string.IsNullOrEmpty(statusmask))
                {
                    char[] sm = statusmask.ToCharArray();
                    if (sm[5] == 'A' && sm[2] == 'V')
                    {
                        sm[0] = 'X';
                        sm[3] = 'N';
                        sm[4] = 'N';
                        sm[5] = 'X';
                        if (motivo_annulla.Contains("Il documento è stato ricevuto dall’Amministrazione destinataria e pertanto non occorre effettuare rispedizioni."))
                        {
                            sm[0] = 'A';
                            sm[3] = 'A';
                            sm[4] = 'A';
                        }
                        statusmask = new string(sm);
                    }
                }


                var statiInvio = await this._dbContext.StatoInvioEntities
                    .Where(si => si.SYSTEM_ID.ToString() == systemID).ToListAsync();

                statiInvio.ForEach(si =>
                {
                    si.VAR_MOTIVO_ANNULLA = motivo_annulla.Replace("'", "''");
                    si.CHA_ANNULLATO = "E";

                    if (!string.IsNullOrEmpty(statusmask))
                    {
                        si.STATUS_C_MASK = statusmask;
                    }
                    else
                    {
                        if (motivo_annulla.Contains("Il documento è stato ricevuto dall’Amministrazione destinataria e pertanto non occorre effettuare rispedizioni."))
                        {
                            si.STATUS_C_MASK = "AVVAAXN";
                        }
                        else
                        {
                            si.STATUS_C_MASK = "'XVVNNXN";
                        }
                    }

                });


                await ((DbContext)this._dbContext).SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(ErrorDescription.XmlEccezioneBasic + e.Message);
                return false;
            }

        }

        //BENE QUI (messo nel nuovo handler)
        private async Task<string?> getStatusMask1(string idProf, string codiceAOO, string codiceAmministrazione, string systemidDPASI = "")
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

        // Ok
        private async System.Threading.Tasks.Task inserimentoTipoNotifica(string codiceNotifica)
        {
            bool retval = false;

            //DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert daticertDB = new DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert();
            TipoNotifica tipoNotifica = new TipoNotifica();
            tipoNotifica.codiceNotifica = codiceNotifica;
            tipoNotifica.descrizioneNotifica = "email di " + codiceNotifica;

            try
            {
                TipoNotificaEntity tipoNotificaEntity = new()
                {
                    VAR_CODICE_NOTIFICA = codiceNotifica,
                    VAR_DESCRIZIONE = "email di " + codiceNotifica
                };

                this._dbContext.TipoNotificaEntities.Add(tipoNotificaEntity);
                var mCount = await ((DbContext)this._dbContext).SaveChangesAsync();
                retval = mCount > 0;
            }
            catch (Exception e)
            {
                _logger.LogError(ErrorDescription.XmlEccezioneInsertTipoNotError + e.Message);
            }

            //return retval;

        }

        //Ok
        private async Task<TipoNotifica?> RicercaTipoNotificaByCodice(string codiceNotifica)
        {
            TipoNotifica retval = new TipoNotifica();

            //DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert daticertDB = new DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert();
            try
            {
                var not = await (from n in this._dbContext.TipoNotificaEntities.AsNoTracking()
                                 where (n.VAR_CODICE_NOTIFICA != null && n.VAR_CODICE_NOTIFICA.Equals(codiceNotifica)) || (codiceNotifica == n.VAR_CODICE_NOTIFICA)
                                 select n).ToListAsync();

                not.ForEach(e => retval = new()
                {
                    idTipoNotifica = e.SYSTEM_ID.ToString(),
                    codiceNotifica = e.VAR_CODICE_NOTIFICA,
                    descrizioneNotifica = e.VAR_DESCRIZIONE
                });
            }
            catch (Exception e)
            {
                _logger.LogError(ErrorDescription.XmlEccezioneSearchTipoNotError + e.Message);
            }

            return retval;
        }

        //MANCA CHIAMATA A NUOVO DatiCertHandler
        private async Task<bool> inserimentoNotifica(Notifica notifica, string idAllegato)
        {
            bool output = false;

            try
            {

                NotificaEntity? notificaEntity = null;

                notificaEntity = new()
                {
                    ID_TIPO_NOTIFICA = notifica.idTipoNotifica != null ? notifica.idTipoNotifica.AsLong() : default,
                    DOCNUMBER = notifica.docnumber.AsLong(),
                    VAR_MITTENTE = notifica.mittente?.Replace("'", "''"),
                    VAR_TIPO_DESTINATARIO = notifica.tipoDestinatario,
                    VAR_DESTINATARIO = notifica.destinatario.Replace("'", "''"),
                    VAR_RISPOSTE = notifica.risposte.Replace("'", "''"),
                    VAR_OGGETTO = notifica.oggetto.Replace("'", "''"),
                    VAR_GESTIONE_EMITTENTE = notifica.gestioneEmittente,
                    VAR_ZONA = notifica.zona,
                    VAR_GIORNO_ORA = notifica.data_ora.AsDateTime(),
                    VAR_IDENTIFICATIVO = notifica.identificativo,
                    VAR_MSGID = notifica.msgid,
                    VAR_TIPO_RICEVUTA = notifica.tipoRicevuta,
                    VAR_CONSEGNA = notifica.consegna,
                    VAR_RICEZIONE = notifica.ricezione,
                };

                if (!string.IsNullOrEmpty(notifica.errore_esteso))
                {
                    notificaEntity.VAR_ERRORE_ESTESO = notifica.errore_esteso.Replace("'", "''");
                }
                else
                {
                    notificaEntity.VAR_ERRORE_ESTESO = string.Empty;
                }

                if (!string.IsNullOrEmpty(notifica.erroreRicevuta))
                {
                    notificaEntity.VAR_ERRORE_RICEVUTA = notifica.erroreRicevuta.Replace("'", "''");
                }
                else
                {
                    notificaEntity.VAR_ERRORE_RICEVUTA = string.Empty;
                }

                if (!string.IsNullOrEmpty(idAllegato))
                    notificaEntity.VERSION_ID = idAllegato.AsLong();
                else
                {
                    notificaEntity.VERSION_ID = null;
                }
                this._dbContext.NotificaEntities.Add(notificaEntity);

                try
                {
                    var nC = await ((DbContext)this._dbContext).SaveChangesAsync();
                    output = nC > 0;
                }
                catch (Exception ex)
                {
                    this._logger.LogError(exception: ex, message: ex.Message);
                }



            }
            catch (Exception e)
            {
                _logger.LogError(ErrorDescription.XmlEccezioneInsertNotError + e.Message);
            }

            return output;
        }


        #endregion
    }
}
