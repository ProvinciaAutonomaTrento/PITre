// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Conferma;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.StandardCase;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using AnnullamentoProtocollazioneType = DocsPaVO.Interoperabilita.Segnatura.AnnullamentoProtocollazioneType;
using DO_getIdProfileByDataResultRequest = Pi3.App.Legacy.WebApi.Application.Requests.DO_getIdProfileByData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Globalization;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using System.ServiceModel.Channels;
using Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetQueryDocumentoPagingCustom;
using LinqKit;
using Pi3.Core.Extensions;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Annullamento
{
    internal class AnnullamentoHandler : IRequestHandler<AnnullamentoRequest, AnnullamentoResult>
    {
        #region Public members
        public AnnullamentoHandler(ILogger<AnnullamentoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IConfigurationService configurationService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._configurationService = configurationService;
        }

        public async Task<AnnullamentoResult> Handle(AnnullamentoRequest request, CancellationToken cancellationToken)
        {
            var output = new ProcessorOutput();

            try
            {
                // Da specifiche va implementato solo il meccanismo per la nuova normativa AGID 2021
                output.Success = await ProcessaXmlAnnullamento(request.content, request.mailId);
            }
            catch (Exception ex)
            {
                output = new ProcessorOutput
                {
                    Success = false,
                    DocNumber = null,
                    ErrorMessage = ex.Message,
                    ProcessedAttachments = 0
                };
            }

            return new AnnullamentoResult(output);
        }
        #endregion

        #region Private members
        protected readonly ILogger<AnnullamentoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;

        private async Task<bool> ProcessaXmlAnnullamento(byte[] xmlContent, string mailId)
        {
            string validaSegnErrorMessage = string.Empty;

            AnnullamentoProtocollazioneType annullamento = null;

            XmlDocument doc = new XmlDocument();
            InteropResolver my = new InteropResolver();

            //xmlContent è il byte array dell'allegato Annullamento.xml
            MemoryStream xms = new MemoryStream(xmlContent);
            XmlTextReader xtr = new XmlTextReader(xms); // { Namespaces = false };
            xtr.WhitespaceHandling = WhitespaceHandling.None;
            XmlValidatingReader xvr = new XmlValidatingReader(xtr);
            xvr.EntityHandling = System.Xml.EntityHandling.ExpandCharEntities;
            xvr.XmlResolver = my;

            try
            {
                //Leggo xml
                doc.Load(xvr);

                //Verifico la validità della segnatura con Xsd
                ValidaRicevuta valid = new ValidaRicevuta(doc.OuterXml, xmlContent, out validaSegnErrorMessage);

                if (!string.IsNullOrEmpty(validaSegnErrorMessage))
                    _logger.LogDebug(validaSegnErrorMessage);

                if (valid.ValidationErrorList.Count > 0)
                    throw new System.Xml.Schema.XmlSchemaException();

            }
            catch (System.Xml.Schema.XmlSchemaException e)
            {
                _logger.LogError(ErrorDescriptions.ValidationError, e.Message);

                //Inserimento in DPA_MAIL_ELABORATE Email Sospsese CHA_RAGIONE_ELAB = 'D'
                var mailElaborataEntity = new MailElaborataEntity()
                {
                    VAR_MESSAGE = mailId,
                    CHA_RAGIONE_ELAB = "D",
                    DTA_ELAB = DateTime.Now,
                    ID_REGISTRO = null,
                    ID_PROFILE = null,
                    VAR_EMAIL = null
                };

                await this._dbContext.MailElaborataEntities.AddAsync(mailElaborataEntity);

                int rowAffected = await ((DbContext)this._dbContext).SaveChangesAsync();

                if (rowAffected > 0)
                    _logger.LogDebug(DebugDescriptions.EmailSuspendedDegug);
                else
                    _logger.LogDebug(DebugDescriptions.EmailNotSuspendedDegug);

                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(ErrorDescriptions.EmailSuspendedError + e.Message);

                //Inserimento in DPA_MAIL_ELABORATE Email Sospsese CHA_RAGIONE_ELAB = 'U'
                var mailElaborataEntity = new MailElaborataEntity()
                {
                    VAR_MESSAGE = mailId,
                    CHA_RAGIONE_ELAB = "U",
                    DTA_ELAB = DateTime.Now,
                    ID_REGISTRO = null,
                    ID_PROFILE = null,
                    VAR_EMAIL = null
                };

                await this._dbContext.MailElaborataEntities.AddAsync(mailElaborataEntity);

                int rowAffetcetd = await ((DbContext)this._dbContext).SaveChangesAsync();

                if (rowAffetcetd > 0)
                    _logger.LogDebug(DebugDescriptions.EmailSuspendedDegug);
                else
                    _logger.LogDebug(DebugDescriptions.EmailNotSuspendedDegug);

                return false;
            }
            finally
            {
                xvr.Close();
                xtr.Close();
                xms.Close();
            }

            try
            {
                if (string.IsNullOrEmpty(doc.DocumentElement.NamespaceURI) || doc.DocumentElement.NamespaceURI.Contains(Resources.www_digitPa_gov_it_protocollo))
                {
                    // Processa XML Annullamento con DTD
                    await ProcessaXmlAnnullamentoDTD(xmlContent, doc.OuterXml, mailId);
                }
                else
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(AnnullamentoProtocollazioneType));

                    using (MemoryStream ms = new MemoryStream(xmlContent))
                    {
                        using (StreamReader reader = new StreamReader(ms))
                        {
                            annullamento = (AnnullamentoProtocollazioneType)serializer.Deserialize(reader);
                        }
                    }

                    CultureInfo ci = new CultureInfo("it-IT");
                    string[] formati = { "yyyy-MM-dd" };

                    string codiceAmministrazione = annullamento.Identificatore.CodiceAmministrazione.Value.Trim();
                    string codiceAOO = annullamento.Identificatore.CodiceAOO.Value.Trim();

                    string codiceRegistro = annullamento.Identificatore.CodiceRegistro.Trim();
                    string numeroRegistrazione = annullamento.Identificatore.NumeroRegistrazione.Trim();
                    DateTime dataRegistrazione = annullamento.Identificatore.DataRegistrazione;
                    string motivo = annullamento.Motivo.Trim();
                    string provvedimento = annullamento.Provvedimento.Trim();

                    //si esegue l'update della tabella stato invio
                    if (codiceAOO != null && !codiceAOO.Equals("") && codiceAmministrazione != null && !codiceAmministrazione.Equals(""))
                    {
                        _logger.LogDebug(DebugDescriptions.UpdateStatoInvioDebug, codiceAOO, codiceAmministrazione, dataRegistrazione.ToString("dd/MM/yyyy"));

                        //si fa un aggiornamento inserendo anche l'annullamento ed il motivo
                        bool res_update = await UpdateStatoInvioAnnulla(null, codiceAOO, codiceAmministrazione, dataRegistrazione.ToString("dd/MM/yyyy"), numeroRegistrazione, dataRegistrazione.Year, motivo, provvedimento, codiceRegistro);

                        if (!res_update)
                        {
                            _logger.LogDebug(DebugDescriptions.UpdateProfileDebug);

                            //Aggiornamento DPA_MAIL_ELABORATE Email Sospsese CHA_RAGIONE_ELAB = 'U'
                            var mailElaborataEntity = new MailElaborataEntity()
                            {
                                VAR_MESSAGE = mailId,
                                CHA_RAGIONE_ELAB = "U",
                                DTA_ELAB = DateTime.Now,
                                ID_REGISTRO = null,
                                ID_PROFILE = null,
                                VAR_EMAIL = null
                            };

                            await this._dbContext.MailElaborataEntities.AddAsync(mailElaborataEntity);

                            int rowAffected = await ((DbContext)this._dbContext).SaveChangesAsync();
                            if (rowAffected > 0)
                                _logger.LogDebug(DebugDescriptions.EmailSuspendedDegug);
                            else
                                _logger.LogDebug(DebugDescriptions.EmailNotSuspendedDegug);

                            return false;
                        }
                    }
                    else
                        _logger.LogDebug(DebugDescriptions.NoUpdateProfileDebug);
                }

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(ErrorDescriptions.EmailDiscarded + e.Message);
                return false;
            }
        }

        private async Task<bool> ProcessaXmlAnnullamentoDTD(byte[] xmlContent, string xmlString, string mailId)
        {
            string errMessage = string.Empty;

            XmlDocument doc = new XmlDocument();
            InteropResolver my = new InteropResolver();

            //xmlContent è il byte array dell'allegato Annullamento.xml
            MemoryStream xms = new MemoryStream(xmlContent);
            XmlTextReader xtr = new XmlTextReader(xms) { Namespaces = false };
            xtr.WhitespaceHandling = WhitespaceHandling.None;
            XmlValidatingReader xvr = new XmlValidatingReader(xtr);
            xvr.ValidationType = System.Xml.ValidationType.DTD;
            xvr.EntityHandling = System.Xml.EntityHandling.ExpandCharEntities;
            xvr.XmlResolver = my;

            try
            {
                doc.Load(xvr);
            }
            catch (System.Xml.Schema.XmlSchemaException e)
            {
                _logger.LogError(ErrorDescriptions.ValidationError, e.Message);

                //Aggiornamento DPA_MAIL_ELABORATE Email Sospsese CHA_RAGIONE_ELAB = 'D'
                var mailElaborataEntity = new MailElaborataEntity()
                {
                    VAR_MESSAGE = mailId,
                    CHA_RAGIONE_ELAB = "D",
                    DTA_ELAB = DateTime.Now,
                    ID_REGISTRO = null,
                    ID_PROFILE = null,
                    VAR_EMAIL = null
                };

                await this._dbContext.MailElaborataEntities.AddAsync(mailElaborataEntity);

                int rowAffected = await ((DbContext)this._dbContext).SaveChangesAsync();

                if (rowAffected > 0)
                    _logger.LogDebug(DebugDescriptions.EmailSuspendedDegug);
                else
                    _logger.LogDebug(DebugDescriptions.EmailNotSuspendedDegug);

                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(ErrorDescriptions.EmailSuspendedError, e.Message);

                //Aggiornamento DPA_MAIL_ELABORATE Email Sospsese CHA_RAGIONE_ELAB = 'U'
                var mailElaborataEntity = new MailElaborataEntity()
                {
                    VAR_MESSAGE = mailId,
                    CHA_RAGIONE_ELAB = "U",
                    DTA_ELAB = DateTime.Now,
                    ID_REGISTRO = null,
                    ID_PROFILE = null,
                    VAR_EMAIL = null
                };

                await this._dbContext.MailElaborataEntities.AddAsync(mailElaborataEntity);

                int rowAffected = await ((DbContext)this._dbContext).SaveChangesAsync();

                if (rowAffected > 0)
                    _logger.LogDebug(DebugDescriptions.EmailSuspendedDegug);
                else
                    _logger.LogDebug(DebugDescriptions.EmailNotSuspendedDegug);

                return false;
            }
            finally
            {
                xvr.Close();
                xtr.Close();
                xms.Close();
            } 

            try
            {

                var xmlnsManager = new XmlNamespaceManager(doc.NameTable);
                xmlnsManager.AddNamespace("ns", "http://www.digitPa.gov.it/protocollo/");

                CultureInfo ci = new CultureInfo("it-IT");
                string[] formati = { "yyyy-MM-dd" };

                XmlElement elIdentificatore = (XmlElement)doc.DocumentElement.SelectSingleNode("ns:Identificatore", xmlnsManager);
                string codiceAmministrazione = elIdentificatore.SelectSingleNode("ns:CodiceAmministrazione", xmlnsManager).InnerText.Trim();
                string codiceAOO = elIdentificatore.SelectSingleNode("ns:CodiceAOO", xmlnsManager).InnerText.Trim();
                string codiceRegistro = elIdentificatore.SelectSingleNode("ns:CodiceRegistro", xmlnsManager).InnerText.Trim();
                string numeroRegistrazione = elIdentificatore.SelectSingleNode("ns:NumeroRegistrazione", xmlnsManager).InnerText.Trim();
                DateTime dataRegistrazione = DateTime.ParseExact(elIdentificatore.SelectSingleNode("ns:DataRegistrazione", xmlnsManager).InnerText.Trim(), formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                XmlElement elMotivo = (XmlElement)doc.DocumentElement.SelectSingleNode("ns:Motivo", xmlnsManager);
                string motivo = elMotivo.InnerText.Trim();
                XmlElement elProvvedimento = (XmlElement)doc.DocumentElement.SelectSingleNode("ns:Provvedimento", xmlnsManager);
                string provvedimento = elProvvedimento.InnerText.Trim();

                //si esegue l'update della tabella stato invio
                if (codiceAOO != null && !codiceAOO.Equals("") && codiceAmministrazione != null && !codiceAmministrazione.Equals(""))
                {
                    _logger.LogDebug(DebugDescriptions.UpdateStatoInvioDebug, codiceAOO, codiceAmministrazione, dataRegistrazione.ToString("dd/MM/yyyy"));


                    //si fa un aggiornamento inserendo anche l'annullamento ed il motivo
                    bool res_update = await UpdateStatoInvioAnnulla(null, codiceAOO, codiceAmministrazione, dataRegistrazione.ToString("dd/MM/yyyy"), numeroRegistrazione, dataRegistrazione.Year, motivo, provvedimento, codiceRegistro);

                    if (!res_update)
                    {
                        _logger.LogDebug(DebugDescriptions.UpdateProfileDebug);

                        //Aggiornamento DPA_MAIL_ELABORATE Email Sospsese CHA_RAGIONE_ELAB = 'U'
                        var mailElaborataEntity = new MailElaborataEntity()
                        {
                            VAR_MESSAGE = mailId,
                            CHA_RAGIONE_ELAB = "U",
                            DTA_ELAB = DateTime.Now,
                            ID_REGISTRO = null,
                            ID_PROFILE = null,
                            VAR_EMAIL = null
                        };

                        await this._dbContext.MailElaborataEntities.AddAsync(mailElaborataEntity);

                        int rowAffected = await ((DbContext)this._dbContext).SaveChangesAsync();

                        if (rowAffected > 0)
                            _logger.LogDebug(DebugDescriptions.EmailSuspendedDegug);
                        else
                            _logger.LogDebug(DebugDescriptions.EmailNotSuspendedDegug);

                        return false;
                    }
                }
                else
                    _logger.LogDebug(DebugDescriptions.NoUpdateProfileDebug);


                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(ErrorDescriptions.EmailDiscarded + e.Message);
                return false;
            }
        }

        private async Task<bool> UpdateStatoInvioAnnulla(string idProfile, string codiceAOO, string codiceAmministrazione, string data, string numeroRegistrazione, int anno, string motivoAnnulla, string provvedimento, string codiceRegistro)
        {
            bool result = false;

            try
            {
                // PEC 4 Modifica Maschera Caratteri
                var query = this._dbContext.StatoInvioEntities.AsNoTracking();
                var appendContext = new StatoInvioAppendContext(this._dbContext, query);


                await AppendFiltroIdProfile(idProfile, appendContext);
                await AppendFiltroCodiceAOO(codiceAOO, appendContext);
                await AppendFiltroCodiceAmministrazione(codiceAmministrazione, appendContext);

                query = appendContext.Query;


                var prova = query.FirstOrDefault();
                string? statusmask = query.FirstOrDefault().STATUS_C_MASK;

                if (!string.IsNullOrEmpty(statusmask))
                {
                    char[] sm = statusmask.ToCharArray();
                    sm[4] = 'V';
                    statusmask = new string(sm);
                }

                if (!string.IsNullOrEmpty(motivoAnnulla))
                    motivoAnnulla = motivoAnnulla.Replace("'", "''");

                if (!string.IsNullOrEmpty(provvedimento))
                    provvedimento = provvedimento.Replace("'", "''");

                // Update StatoInvio
                string varProtoDest = numeroRegistrazione + "/" + codiceRegistro + "/" + anno;

                if (!string.IsNullOrEmpty(idProfile))
                {
                    //reperisco codice amm e codice AOO mittente
                    //var codiceAOOMitt = 



                    var statoInvioEntities = await this._dbContext.StatoInvioEntities
                    .Where(s => s.VAR_PROTO_DEST.ToUpper() == varProtoDest.ToUpper()
                    && s.DTA_PROTO_DEST == data.AsDateTime()
                    && s.VAR_CODICE_AOO.ToUpper() == codiceAOO.ToUpper()
                    && s.VAR_CODICE_AMM.ToUpper() == codiceAmministrazione.ToUpper()
                    && s.DTA_SPEDIZIONE != null
                    && s.ID_PROFILE == idProfile.AsLong())
                    .ToListAsync();

                    statoInvioEntities.ForEach(x => x.VAR_MOTIVO_ANNULLA = motivoAnnulla.Replace("'", "''"));
                    statoInvioEntities.ForEach(x => x.CHA_ANNULLATO = "1");
                    statoInvioEntities.ForEach(x => x.VAR_PROVVEDIMENTO = provvedimento);
                    // PEC 4 Modifica Maschera Caratteri
                    statoInvioEntities.ForEach(x => x.STATUS_C_MASK = string.IsNullOrEmpty(statusmask) ? "VVVVVNN" : statusmask);

                }
                else
                {
                    var statoInvioEntities = await this._dbContext.StatoInvioEntities
                    .Where(s => s.VAR_PROTO_DEST.ToUpper() == varProtoDest.ToUpper()
                    && s.DTA_PROTO_DEST == data.AsDateTime()
                    && s.VAR_CODICE_AOO.ToUpper() == codiceAOO.ToUpper()
                    && s.VAR_CODICE_AMM.ToUpper() == codiceAmministrazione.ToUpper()
                    && s.DTA_SPEDIZIONE != null)
                    .ToListAsync();

                    statoInvioEntities.ForEach(x => x.VAR_MOTIVO_ANNULLA = motivoAnnulla.Replace("'", "''"));
                    statoInvioEntities.ForEach(x => x.CHA_ANNULLATO = "1");
                    statoInvioEntities.ForEach(x => x.VAR_PROVVEDIMENTO = provvedimento);
                    // PEC 4 Modifica Maschera Caratteri
                    statoInvioEntities.ForEach(x => x.STATUS_C_MASK = string.IsNullOrEmpty(statusmask) ? "VVVVVNN" : statusmask);

                    int rowAffected = await ((DbContext)this._dbContext).SaveChangesAsync();

                    if (rowAffected > 0)
                        result = true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                result = false;
            }

            return result;
        }
        #endregion

        internal class StatoInvioAppendContext
        {
            public StatoInvioAppendContext(IPi3DbContext dbContext, IQueryable<StatoInvioEntity> query)
            {
                DbContext = dbContext;
                Query = query;
            }

            public IPi3DbContext DbContext { get; set; }

            public IQueryable<StatoInvioEntity> Query { get; set; }
        }

        public async Task AppendFiltroIdProfile(
             string idProfile,
             StatoInvioAppendContext context)
        {
            if (!string.IsNullOrEmpty(idProfile))
            {
                context.Query = context.Query.Where(s => s.ID_PROFILE == idProfile.AsLong());
            }
            else
            {

            }

        }

        public static async Task AppendFiltroCodiceAOO(
             string codiceAOO,
             StatoInvioAppendContext context)
        {
            if (!string.IsNullOrEmpty(codiceAOO))
            {
                context.Query = context.Query.Where(s => s.VAR_CODICE_AOO.ToUpper() == codiceAOO.ToUpper());
            }
            else
            {

            }

        }

        public static async Task AppendFiltroCodiceAmministrazione(
             string codiceAmministrazione,
             StatoInvioAppendContext context)
        {
            if (!string.IsNullOrEmpty(codiceAmministrazione))
            {
                context.Query = context.Query.Where(s => s.VAR_CODICE_AMM.ToUpper() == codiceAmministrazione.ToUpper());
            }
            else
            {

            }
        }

        private async Task<string>? findIdProfile(string codiceAOO, string numeroRegistrazione, int year)
        {
            try
            {

                var id = await this._dbContext.ProfileEntities.AsNoTracking().
                    Join(this._dbContext.RegistroEntities, a => a.ID_REGISTRO, b => b.SYSTEM_ID, (a, b) => new { a, b })
                    .Where(
                    c => c.b.VAR_CODICE.ToUpper() == codiceAOO.ToUpper() && c.a.NUM_PROTO == numeroRegistrazione.AsLong() && ((int?)c.a.NUM_ANNO_PROTO) == year)
                    .Select(c => c.a.SYSTEM_ID).FirstOrDefaultAsync();

                if (id == null)
                {
                    id = await this._dbContext.ProfileEntities.AsNoTracking().
                        Join(this._dbContext.RegistroEntities, a => a.ID_REGISTRO, b => b.SYSTEM_ID, (a, b) => new { a, b })
                        .Where(
                        c => c.b.VAR_CODICE.ToUpper() == codiceAOO.ToUpper() && c.a.NUM_PROTO == numeroRegistrazione.AsLong() && ((int?)c.a.NUM_ANNO_PROTO) == year - 1)
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

    }
}
