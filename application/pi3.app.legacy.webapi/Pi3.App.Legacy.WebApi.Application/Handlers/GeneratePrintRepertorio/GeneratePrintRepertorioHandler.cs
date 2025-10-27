// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GeneratePrintRepertorioRequest = Pi3.App.Legacy.WebApi.Application.Requests.GeneratePrintRepertorio;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GeneratePrintRepertorio
{
    public class GeneratePrintRepertorioHandler : IRequestHandler<GeneratePrintRepertorioRequest, GeneratePrintRepertorioResult>
    {
        #region Public members
        public GeneratePrintRepertorioHandler(ILogger<GeneratePrintRepertorioHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext,
            IDocumentoAmministrativoRepository documentRepository, IDocumentBlobRepository documentBlobRepository, IReportGeneratorService reportGeneratorService, IConfigurationService configurationService,
            IFileConverterService converterService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._documentRepository = documentRepository;
            this._documentBlobRepository = documentBlobRepository;
            this._reportGeneratorService = reportGeneratorService;
            this._configurationService = configurationService;
            this._converterService = converterService;
        }
        public async Task<GeneratePrintRepertorioResult> Handle(GeneratePrintRepertorioRequest request, CancellationToken cancellationToken)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

            var printRequest = request.request;

            var contatoreEntity = await this._dbContext.TipoOggettoEntities.AsNoTracking().FirstAsync(x => x.DESCRIZIONE.ToLower() == "contatore");

            var repertorioEntity = await this.GetRepertorioEntity(printRequest.RegistryId, printRequest.RfId, printRequest.CounterId);

            if (repertorioEntity.COUNTERSTATE == "O")
            {
                throw new RegistryOpenPi3Exception();
            }

            if(!await this.ExistsRepToPrintAsync(printRequest.RegistryId, printRequest.RfId, printRequest.CounterId, contatoreEntity.SYSTEM_ID, repertorioEntity.DTALASTPRINT))
            {
                // Eccezione documenti non trovati
                throw new DocumentsNotFoundPi3Exception();
            }

            var amministraEntity = this._dbContext.AmministraEntities.AsNoTracking().First(x => x.SYSTEM_ID == idTenant);

            var tipoAttoEntity = await this._dbContext.TipoAttoEntities.AsNoTracking().FirstAsync(x => x.SYSTEM_ID == repertorioEntity.TIPOLOGYID);

            var nomeTipologia = tipoAttoEntity is not null ? tipoAttoEntity.VAR_DESC_ATTO + " " : string.Empty;

            // Estrazione repertori da stampare
            var getRepertoriPrintRanges = await this._mediator.Send(new Requests.GetRepertoriPrintRanges(new DocsPaVO.utente.Repertori.RequestAndResponse.GetRepertoriPrintRangesRequest
            {
                CounterId = printRequest.CounterId,
                RegistryId = printRequest.RegistryId,
                RfId = printRequest.RfId
            }, false));

            var regRfList = this.GetRegRfList(printRequest.RegistryId, printRequest.RfId);

            foreach(var range in getRepertoriPrintRanges.output.Ranges)
            {
                try
                {
                    // Estrazione dati repertorio nuovi
                    var reportNewItems = await this.GetReportDataNew(
                        repertorioEntity.TIPOLOGYID,
                        repertorioEntity.COUNTERID,
                        range.FirstNumber,
                        range.LastNumber,
                        range.Year,
                        regRfList
                        );

                    // Estrazione dati repertori modificati
                    var reportModifiedItems = await this.GetReportModifiedData(
                        repertorioEntity.TIPOLOGYID,
                        repertorioEntity.COUNTERID,
                        range.FirstNumber,
                        range.LastNumber,
                        range.Year,
                        printRequest.RegistryId,
                        printRequest.RfId,
                        regRfList
                        );

                    var report = new ReportModel
                    {
                        Size = PageSizes.A4,
                        Orientation = PageOrientations.Landscape,
                        OutputType = ReportOutputTypes.AsPdf
                    };

                    // Repertori nuovi
                    report.AddSection(this.GetTitleTextSection(amministraEntity.VAR_DESC_AMM!));

                    if (!string.IsNullOrEmpty(printRequest.RfId))
                    {
                        var registroEntity = await this._dbContext.RegistroEntities.AsNoTracking().FirstOrDefaultAsync(x => x.SYSTEM_ID == printRequest.RfId.AsLong());
                        report.AddSection(this.GetTitleTextSection($"[{registroEntity?.VAR_CODICE}] {registroEntity?.VAR_DESC_REGISTRO}"));
                    }

                    report.AddSection(this.GetTitleTextSection(
                        string.Format(Resources.TextAdditionalInformation,
                            range.Year,
                            range.FirstNumber,
                            range.LastNumber)));

                    report.AddSection(this.GetTitleTextSection(string.Format("{0} - Stampa generata il {1}", nomeTipologia, DateTime.Today.ToString())));

                    report.AddSection(this.GetSubTitleTextSection("Nuovi documenti"));

                    var gridRepertoriNuovi = new GridSectionModel
                    {
                        Style = new GridSectionStyleModel { WithPercentage = 100 }
                    };

                    this.AddHeader(gridRepertoriNuovi, false);

                    foreach (var group in reportNewItems.GroupBy(x => x.DocNumber))
                    {
                        var item = group.First();
                        await this.AddRow(gridRepertoriNuovi, item);

                        if (group.Any(x => x.EnabledHistory == "1"))
                        {
                            var itemsWithEnabledHistory = group.Where(x => x.Equals("1")).ToList();

                            foreach (var i in itemsWithEnabledHistory)
                            {
                                if (!gridRepertoriNuovi.Rows.First().Cells.Any(x => x.Content.Value == i.DescrizioneCampo))
                                    gridRepertoriNuovi.Rows.First().AddCell(new GridCellModel
                                    {
                                        Style = new GridCellStyleModel { WithPercentage = 5 },
                                        Content = new TextContentModel { Style = new TextStyleModel { FontIsBold = true }, Value = i.DescrizioneCampo! }
                                    });

                                gridRepertoriNuovi.Rows.Last().AddCell(new GridCellModel
                                {
                                    Content = new TextContentModel
                                    {
                                        Value = i.ObjType?.ToLower() != "corrispondente" ?
                                            i.ValoreCampo :
                                            (await this._dbContext.CorrGlobaliEntities.AsNoTracking().FirstOrDefaultAsync(c => c.SYSTEM_ID == i.ValoreCampo.AsLong()))?.VAR_DESC_CORR
                                    }
                                });
                            }
                        }
                    }

                    report.AddSection(gridRepertoriNuovi);

                    // Repertori modificati
                    report.AddSection(this.GetTitleTextSection(amministraEntity.VAR_DESC_AMM!));

                    if (!string.IsNullOrEmpty(printRequest.RfId))
                    {
                        var registroEntity = await this._dbContext.RegistroEntities.AsNoTracking().FirstOrDefaultAsync(x => x.SYSTEM_ID == printRequest.RfId.AsLong());
                        report.AddSection(this.GetTitleTextSection($"[{registroEntity?.VAR_CODICE}] {registroEntity?.VAR_DESC_REGISTRO}"));
                    }

                    report.AddSection(this.GetTitleTextSection(
                        string.Format(Resources.TextAdditionalInformation,
                            range.Year,
                            range.FirstNumber,
                            range.LastNumber)));

                    report.AddSection(this.GetTitleTextSection(string.Format("{0} - Stampa generata il {1}", nomeTipologia, DateTime.Today.ToString())));

                    report.AddSection(this.GetSubTitleTextSection("Documenti modificati"));

                    var gridRepertoriModificati = new GridSectionModel
                    {
                        Style = new GridSectionStyleModel { WithPercentage = 100 }
                    };

                    this.AddHeader(gridRepertoriModificati, false);

                    foreach (var group in reportModifiedItems.GroupBy(x => x.DocNumber))
                    {
                        var item = group.First();
                        await this.AddRow(gridRepertoriModificati, item);

                        if (group.Any(x => x.EnabledHistory == "1"))
                        {
                            var itemsWithEnabledHistory = group.Where(x => x.Equals("1")).ToList();

                            foreach (var i in itemsWithEnabledHistory)
                            {
                                if (!gridRepertoriNuovi.Rows.First().Cells.Any(x => x.Content.Value == i.DescrizioneCampo))
                                    gridRepertoriNuovi.Rows.First().AddCell(new GridCellModel
                                    {
                                        Style = new GridCellStyleModel { WithPercentage = 5 },
                                        Content = new TextContentModel { Style = new TextStyleModel { FontIsBold = true }, Value = i.DescrizioneCampo! }
                                    });

                                gridRepertoriNuovi.Rows.Last().AddCell(new GridCellModel
                                {
                                    Content = new TextContentModel
                                    {
                                        Value = i.ObjType?.ToLower() != "corrispondente" ?
                                            i.ValoreCampo :
                                            (await this._dbContext.CorrGlobaliEntities.AsNoTracking().FirstOrDefaultAsync(c => c.SYSTEM_ID == i.ValoreCampo.AsLong()))?.VAR_DESC_CORR
                                    }
                                });
                            }
                        }
                    }

                    report.AddSection(gridRepertoriNuovi);

                    // Creazione aggregate e upload file
                    var aggregate = new DocumentoAmministrativo(
                        idTenant.ToString(),
                        DateTime.Now,
                        new OggettoDelDocumento
                        {
                            Descrizione = new(string.Format(Resources.TextReportSubject,
                                nomeTipologia,
                                range.Year,
                                range.FirstNumber,
                                range.LastNumber))
                        },
                        new DatiRegistro()
                        {
                            IdRegistro = printRequest.RegistryId
                        },
                        null,
                        TipologieVisibilitaEnum.Gerarchica,
                        null
                        );

                    aggregate.AssignDatiStampa(new DatiStampa()
                    {
                        TipoStampa = TipologieStampaEnum.StampaRegistroRepertorio
                    });

                    var fileName = $"Report_{DateTime.Now.ToString("dd-MM-yyyy")}.pdf";

                    var blobAggregate = new DocumentBlob(
                        idTenant.ToString(),
                        DateTime.Now,
                        new Core.SeedWork.TextValue { Value = fileName }
                        );

                    using (var stream = new MemoryStream())
                    {
                        // Generazione stream report (docx)
                        var generatedReport = await this._reportGeneratorService.Generate(report, stream);
                        var content = stream.ToArray();

                        // Inserimento nell'aggregate
                        blobAggregate.UploadStream(new MemoryStream(content), fileName);

                        blobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                        await this._documentBlobRepository.Add(blobAggregate);

                        var hash = blobAggregate.Hash;

                        aggregate.AssignDocumentBlobRef(
                        new Core.AggregateModels.DocumentAggregate.ValueObjects.DocumentBlobRef
                        {
                            IdBlob = blobAggregate.Id,
                            CreationDate = await _dbContext.GetSystemDateTime(),
                            ContentType = blobAggregate.ContentType,
                            FileName = blobAggregate.FileName,
                            FileSize = blobAggregate.FileSize,
                            Hash = hash,
                            HashName = Core.AggregateModels.DocumentAggregate.ValueObjects.HashNamesEnum.SHA256
                        },
                        new Core.AggregateModels.DocumentAggregate.ValueObjects.TargetVersionBehavior
                        {
                            CreateNewVersion = true,
                            Name = new TextValue { Value = $"Report_{DateTime.Now.ToString("dd-MM-YYYY")}.pdf" },
                        }
                        );
                    }

                    await this._documentRepository.Add(aggregate);

                    // Inserimento entity stampa repertori
                    var registryId = printRequest.RegistryId ?? printRequest.RfId;

                    await this._dbContext.StampaRepertoriEntities.AddAsync(new StampaRepertoriEntity
                    {
                        DOCNUMBER = aggregate.Id.AsLong(),
                        NUM_REP_START = range.FirstNumber,
                        NUM_REP_END = range.LastNumber,
                        NUM_ANNO = range.Year,
                        ID_REPERTORIO = printRequest.CounterId.AsLong(),
                        REGISTRYID = !string.IsNullOrEmpty(registryId) ? registryId.AsLong() : null,
                        DTA_STAMPA = DateTime.Now
                    });

                    // Estensione visibilità responsabile stampa repertorio
                    if (repertorioEntity.PRINTERROLERESPID.HasValue)
                    {
                        var responsabileSecurityEntity = new SecurityEntity
                        {
                            THING = aggregate.Id.AsLong(),
                            PERSONORGROUP = repertorioEntity.PRINTERROLERESPID,
                            ACCESSRIGHTS = repertorioEntity.RESPRIGHTS == "R" ? 45 : 63,
                            CHA_TIPO_DIRITTO = "A",
                            ID_GRUPPO_TRASM = null,
                            HIDE_DOC_VERSIONS = null
                        };

                        await this._dbContext.SecurityEntities.AddAsync(responsabileSecurityEntity);
                    }

                    // TO DO Visibilità ruoli superiori

                    // TO DO Visibilità per repertori tipologia/RF

                    // TO DO Invio in conservazione

                    // TO DO Metadati AGID

                    if (!string.IsNullOrWhiteSpace(printRequest.RegistryId) && string.IsNullOrWhiteSpace(printRequest.RfId))
                        await ((DbContext)this._dbContext)
                            .Database
                            .ExecuteSqlAsync($"UPDATE DPA_REGISTRI_REPERTORIO SET LASTPRINTEDNUMBER={range.LastNumber}, DTALASTPRINT={DateTime.Now} WHERE COUNTERID={printRequest.CounterId} AND REGISTRYID={printRequest.RegistryId} AND RFID IS NULL");
                    else if (string.IsNullOrWhiteSpace(printRequest.RegistryId) && !string.IsNullOrWhiteSpace(printRequest.RfId))
                        await ((DbContext)this._dbContext)
                            .Database
                            .ExecuteSqlAsync($"UPDATE DPA_REGISTRI_REPERTORIO SET LASTPRINTEDNUMBER={range.LastNumber}, DTALASTPRINT={DateTime.Now} WHERE COUNTERID={printRequest.CounterId} AND REGISTRYID IS NULL AND RFID={printRequest.RfId}");
                    else
                        await ((DbContext)this._dbContext)
                            .Database
                            .ExecuteSqlAsync($"UPDATE DPA_REGISTRI_REPERTORIO SET LASTPRINTEDNUMBER={range.LastNumber}, DTALASTPRINT={DateTime.Now} WHERE COUNTERID={printRequest.CounterId} AND REGISTRYID IS NULL AND RFID IS NULL");

                    await ((DbContext)this._dbContext).SaveChangesAsync();
                }
                catch(Exception ex)
                {
                    this._logger.LogError(exception: ex, message: ex.Message);
                }
            }

            return new GeneratePrintRepertorioResult(new());
        }
        #endregion

        #region Private members
        protected ILogger<GeneratePrintRepertorioHandler> _logger;
        protected IClaimsPrincipalService _claimsPrincipalService;
        protected IMediator _mediator;
        protected IPi3DbContext _dbContext;
        protected IDocumentoAmministrativoRepository _documentRepository;
        protected IDocumentBlobRepository _documentBlobRepository;
        protected IReportGeneratorService _reportGeneratorService;
        protected IConfigurationService _configurationService;
        protected IFileConverterService _converterService;

        private string GetRegRfList(string? registryId, string? rfId)
        {
            if (string.IsNullOrWhiteSpace(registryId) && string.IsNullOrWhiteSpace(rfId)) return "0";
            else return string.IsNullOrWhiteSpace(registryId) ? rfId! : registryId;

        }

        private async Task<RegistriRepertorioEntity?> GetRepertorioEntity(string? registryId, string? rfId, string counterId)
        {
            var repertoriQueryable = this._dbContext.RegistriRepertorioEntities
                .Where(x => x.COUNTERID == counterId.AsLong())
                .AsQueryable();

            if (string.IsNullOrWhiteSpace(registryId)) repertoriQueryable = repertoriQueryable.Where(x => x.REGISTRYID == null).AsQueryable();
            else repertoriQueryable = repertoriQueryable.Where(x => x.REGISTRYID == registryId.AsLong()).AsQueryable();

            if (string.IsNullOrWhiteSpace(rfId)) repertoriQueryable = repertoriQueryable.Where(x => x.RFID == null).AsQueryable();
            else repertoriQueryable = repertoriQueryable.Where(x => x.RFID == rfId.AsLong()).AsQueryable();

            return await repertoriQueryable.FirstOrDefaultAsync();
        }

        private async Task<bool> ExistsRepToPrintAsync(string? registryId, string? rfId, string counterId, long typeId, DateTime? lastPrintDate)
        {
            var assTemplatesQueryable = this._dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                .Join(this._dbContext.OggettiCustomEntities.AsNoTracking(), a => a.ID_OGGETTO, b => b.SYSTEM_ID, (a, b) => new { a, b })
                .Where(x => x.a.ID_AOO_RF == this.GetRegRfList(registryId, rfId).AsLong()
                && x.a.ID_OGGETTO == counterId.AsLong()
                && x.b.ID_TIPO_OGGETTO == typeId
                );

            if (lastPrintDate.HasValue) assTemplatesQueryable = assTemplatesQueryable.Where(x => x.a.DTA_INS > lastPrintDate);

            return await assTemplatesQueryable.AnyAsync();
        }

        private async Task<List<ReportRepertoriItem>> GetReportDataNew(long typologyId, long counterId, long? firstNumber, long? lastNumber, long year, string regRfList)
        {
            var ammCode = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantCode);

            var assTemplatesEntities = await this._dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                .Where(x => x.ID_OGGETTO == counterId
                && x.ANNO == year
                && x.ID_AOO_RF == regRfList.AsLong()
                && !string.IsNullOrWhiteSpace(x.DOC_NUMBER))
                .ToListAsync();

            var docNumberPredicate = PredicateBuilder.New<AssociazioneTemplatesEntity>();

            docNumberPredicate.And(x => int.Parse(x.VALORE_OGGETTO_DB ?? "0") >= (firstNumber ?? 0));

            if (lastNumber.HasValue)
                docNumberPredicate.And(x => int.Parse(x.VALORE_OGGETTO_DB ?? "0") <= lastNumber);

            var docNumberList = assTemplatesEntities.Where(docNumberPredicate)
                .Select(x => x.DOC_NUMBER)
                .ToList();

            var reportDataList = await this._dbContext.ProfileEntities.AsNoTracking()
                .Join(this._dbContext.MvDocumentiCustomEntities.AsNoTracking(), p => p.DOCNUMBER.ToString(), mv => mv.DOC_NUMBER, (p, mv) => new { p, mv })
                .Join(this._dbContext.TipoOggettoEntities.AsNoTracking(), a=> a.mv.SYSTEM_ID_TIPO_OGGETTO, t => t.SYSTEM_ID, (a, t) => new {a.p, a.mv, t})
                .Where(x => (x.p.CHA_TIPO_PROTO == "A" || x.p.CHA_TIPO_PROTO == "P" || x.p.CHA_TIPO_PROTO == "I" || x.p.CHA_TIPO_PROTO == "G")
                && x.mv.ID_TEMPLATE == typologyId
                && docNumberList.Any(y => y == x.mv.DOC_NUMBER))
                .Select(x => x)
                .Select(x => new ReportRepertoriItem
                {
                    IdDoc = !string.IsNullOrWhiteSpace(x.p.VAR_SEGNATURA) ? x.p.VAR_SEGNATURA : x.p.DOCNUMBER.ToString(),
                    Oggetto = x.p.VAR_PROF_OGGETTO,
                    IsModificato = IPi3DbContextMappedFunctions.IsOggettoModificato(x.p.DOCNUMBER!.Value),
                    DescrizioneCampo = x.mv.DESCRIZIONE,
                    ValoreCampo = x.mv.VALORE_OGGETTO_DB,
                    IdAmm = x.mv.ID_AMM,
                    DataInserimento = x.mv.DTA_INS,
                    DataAnnullamento = x.mv.DTA_ANNULLAMENTO,
                    DocNumber = x.mv.DOC_NUMBER,
                    ObjType = x.t.DESCRIZIONE,
                    EnabledHistory = x.mv.ENABLEDHISTORY,
                    IdObject = x.mv.SYSTEM_ID_OGG_CUSTOM,
                    CampoComune = x.mv.CAMPO_COMUNE,
                    Tipologia = x.mv.VAR_DESC_ATTO,
                    Impronta = IPi3DbContextMappedFunctions.GetImprontaWithAllegati(x.p.DOCNUMBER!.Value)
                }).ToListAsync();

            foreach (var x in reportDataList)
            { 
                x.SegnaturaRepertorio = (await this._dbContext.AssociazioneTemplatesEntities
                    .FirstAsync(a => a.DOC_NUMBER == x.DocNumber && a.ID_OGGETTO == x.IdObject))
                    .VAR_SEGNATURA;
            }

            return reportDataList;
        }
        
        private async Task<List<ReportRepertoriItem>> GetReportModifiedData(long typologyId, long counterId, long? firstNumber, long? lastNumber, long year, string? registryId, string? rfId, string regRfList)
        {
            var registriRepertorioEntity = await this._dbContext.RegistriRepertorioEntities.AsNoTracking()
                .FirstOrDefaultAsync(x => x.COUNTERID == counterId
                && !string.IsNullOrWhiteSpace(registryId) ? x.REGISTRYID == registryId.AsLong() : x.REGISTRYID == null
                && !string.IsNullOrWhiteSpace(rfId) ? x.RFID == rfId.AsLong() : x.RFID == null);

            var lastPrintDate = registriRepertorioEntity?.DTALASTPRINT?.Year == year ?
                registriRepertorioEntity?.DTALASTPRINT :
                new DateTime((int)year, 1, 1);

            var reportDataList = this._dbContext.ProfileEntities.AsNoTracking()
                .Join(this._dbContext.AssociazioneTemplatesEntities.AsNoTracking(), p => p.DOCNUMBER.ToString(), at => at.DOC_NUMBER, (p, at) => new { p, at })
                .Join(this._dbContext.OggettiCustomEntities.AsNoTracking(), a => a.at.ID_OGGETTO, oc => oc.SYSTEM_ID, (a, oc) => new { a.p, a.at, oc })
                .Join(this._dbContext.TipoAttoEntities.AsNoTracking(), a => a.at.ID_TEMPLATE, t => t.SYSTEM_ID, (a, t) => new { a.p, a.at, a.oc, t })
                .Join(this._dbContext.TipoOggettoEntities.AsNoTracking(), a => a.oc.ID_TIPO_OGGETTO, to => to.SYSTEM_ID, (a, to) => new { a.p, a.at, a.oc, a.t, to })
                .Join(this._dbContext.OggettiCustomCompEntities.AsNoTracking(), a => a.oc.SYSTEM_ID, occ => occ.ID_OGG_CUSTOM, (a, occ) => new { a.p, a.at, a.oc, a.t, a.to, occ })
                .Where(x => (x.p.CHA_TIPO_PROTO == "A" || x.p.CHA_TIPO_PROTO == "P" || x.p.CHA_TIPO_PROTO == "I" || x.p.CHA_TIPO_PROTO == "G")
                    && x.p.CREATION_DATE < lastPrintDate
                    && x.p.LAST_EDIT_DATE > lastPrintDate
                    )
                .Select(x => x)
                .Select(x => new ReportRepertoriItem
                {
                    IdDoc = !string.IsNullOrWhiteSpace(x.p.VAR_SEGNATURA) ? x.p.VAR_SEGNATURA : x.p.DOCNUMBER.ToString(),
                    Oggetto = x.p.VAR_PROF_OGGETTO,
                    IsModificato = IPi3DbContextMappedFunctions.IsOggettoModificato(x.p.DOCNUMBER!.Value),
                    DescrizioneCampo = x.oc.DESCRIZIONE,
                    ValoreCampo = x.at.VALORE_OGGETTO_DB,
                    IdAmm = x.t.ID_AMM,
                    DataInserimento = x.at.DTA_INS,
                    DataAnnullamento = x.at.DTA_ANNULLAMENTO,
                    DocNumber = x.at.DOC_NUMBER,
                    ObjType = x.to.DESCRIZIONE,
                    EnabledHistory = x.occ.ENABLEDHISTORY,
                    IdObject = x.oc.SYSTEM_ID,
                    CampoComune = x.oc.CAMPO_COMUNE,
                    Tipologia = x.t.VAR_DESC_ATTO,
                    SegnaturaRepertorio = x.at.VAR_SEGNATURA,
                    Impronta = IPi3DbContextMappedFunctions.GetImprontaWithAllegati(x.p.DOCNUMBER!.Value)
                }).ToList();
                

            // Documenti da rimuovere dalla lista dei modificati
            var docToRemove = new List<string?>();

            docToRemove.AddRange(await this._dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                .Where(x => x.ID_OGGETTO == counterId &&
                x.ID_AOO_RF == regRfList.AsLong()
                && x.DTA_INS == null
                && string.IsNullOrWhiteSpace(x.VALORE_OGGETTO_DB))
                .Select(x => x.DOC_NUMBER)
                .ToListAsync());

            docToRemove.AddRange(await this._dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                .Join(this._dbContext.ProfileEntities, at => at.DOC_NUMBER, p => p.DOCNUMBER.ToString(), (at, p) => new { at, p })
                .Where(x => x.at.ID_OGGETTO == counterId
                    && x.at.DTA_INS < x.p.LAST_EDIT_DATE
                ).Select(x => x.at.DOC_NUMBER)
                .ToListAsync());

            docToRemove.AddRange(await this._dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                .Join(this._dbContext.ProfilStoEntities.AsNoTracking(), at => at.DOC_NUMBER, p => p.ID_PROFILE.ToString(), (at, p) => new { at, p })
                .Where(x => x.at.ID_OGGETTO == counterId
                && x.p.DTA_MODIFICA > lastPrintDate
                && x.p.DTA_MODIFICA < new DateTime((int)year, 12, 31))
                .Select(x => x.at.DOC_NUMBER)
                .ToListAsync());

            if (docToRemove.Any()) reportDataList.RemoveAll(x => docToRemove.Any(y => y == x.DocNumber));

            return reportDataList;
            
        }

        private TextSectionModel GetTitleTextSection(string text)
        {
            return new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Left},
                Content = new TextContentModel
                {
                    Value = text,
                    Style = new TextStyleModel
                    {
                        FontName = "Arial",
                        FontSize = 16
                    }
                }
            };
        }

        private TextSectionModel GetSubTitleTextSection(string text)
        {
            return new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Left },
                Content = new TextContentModel
                {
                    Value = text,
                    Style = new TextStyleModel
                    {
                        FontName = "Helvetica",
                        FontSize = 12
                    }
                }
            };
        }

        private TextSectionModel GetSummaryTextSection(string text)
        {
            return new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Left },
                Content = new TextContentModel
                {
                    Value = text,
                    Style = new TextStyleModel
                    {
                        FontName = "Helvetica",
                        FontSize = 10
                    }
                }
            };
        }

        private void AddHeader(GridSectionModel grid, bool isRepertorioCampoComune)
        {
            var gridHeader = new GridRowModel();
            gridHeader.AddCell(new GridCellModel { Style = new GridCellStyleModel { WithPercentage = 10 }, Content = new TextContentModel { Style = new TextStyleModel { FontIsBold = true }, Value = Resources.HeaderIdDoc } });
            gridHeader.AddCell(new GridCellModel { Style = new GridCellStyleModel { WithPercentage = 20 }, Content = new TextContentModel { Style = new TextStyleModel { FontIsBold = true }, Value = Resources.HeaderSubject } });
            gridHeader.AddCell(new GridCellModel { Style = new GridCellStyleModel { WithPercentage = 10 }, Content = new TextContentModel { Style = new TextStyleModel { FontIsBold = true }, Value = Resources.HeaderRegistrationDate  } });
            gridHeader.AddCell(new GridCellModel { Style = new GridCellStyleModel { WithPercentage = 10 }, Content = new TextContentModel { Style = new TextStyleModel { FontIsBold = true }, Value = Resources.HeaderCancellationDate  } });
            gridHeader.AddCell(new GridCellModel { Style = new GridCellStyleModel { WithPercentage = 30 }, Content = new TextContentModel { Style = new TextStyleModel { FontIsBold = true }, Value = Resources.HeaderFileHash  } });
            gridHeader.AddCell(new GridCellModel { Style = new GridCellStyleModel { WithPercentage = 10 }, Content = new TextContentModel { Style = new TextStyleModel { FontIsBold = true }, Value = Resources.HeaderSignature  } });

            if(isRepertorioCampoComune)
            {
                gridHeader.AddCell(new GridCellModel { Style = new GridCellStyleModel { WithPercentage = 10 }, Content = new TextContentModel { Style = new TextStyleModel { FontIsBold = true }, Value = Resources.HeaderTypology  } });
            }

            grid.AddRow(gridHeader);
        }
        
        private async Task AddRow(GridSectionModel grid, ReportRepertoriItem item)
        {
            var row = new GridRowModel();

            row.AddCell(new GridCellModel { Content = new TextContentModel { Value = item.IdDoc } });
            row.AddCell(new GridCellModel { Content = new TextContentModel { Value = item.IsModificato == "1" ? await this.GetModifiedSubjectField(item.Oggetto, item.DocNumber) : item.Oggetto } });
            row.AddCell(new GridCellModel { Content = new TextContentModel { Value = (await this.GetRegistrationDate(item.DocNumber, item.IdObject.Value)).ToString()  } });
            row.AddCell(new GridCellModel { Content = new TextContentModel { Value = item.DataAnnullamento.HasValue ? item.DataAnnullamento.ToString() : string.Empty } });
            row.AddCell(new GridCellModel { Content = new TextContentModel { Value = "" } });
            row.AddCell(new GridCellModel { Content = new TextContentModel { Value = item.SegnaturaRepertorio } });

            grid.AddRow(row);
        }

        private async Task<string?> GetModifiedSubjectField(string? subject, string? docnumber)
        {
            var motivo = string.Empty;

            var motivoKey = await this._configurationService.TryGetValue<string>("BE_RIF_PROV_AUTORIZZAZIONE");
            
            if (motivoKey.Item2)
            {
                motivo = string.Format(Resources.TextSubjectInfo, motivoKey.Item1);
                subject += motivo;
            }

            return subject;
        }

        private async Task<DateTime?> GetRegistrationDate(string? docNumber, long counterId)
        {
            return (await this._dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                .FirstOrDefaultAsync(x => x.DOC_NUMBER == docNumber && x.ID_OGGETTO == counterId))?.DTA_INS;
        }

        protected class ReportRepertoriItem : ValueObject
        {
            public string? IdDoc { get; init; }

            public string? Oggetto { get; init; }

            public string? IsModificato { get; init; }

            public string? DescrizioneCampo { get; init; }

            public string? ValoreCampo { get; init; }

            public long? IdAmm { get; init; }

            public DateTime? DataInserimento { get; init; }

            public DateTime? DataAnnullamento { get; init; }

            public string? DocNumber { get; init; }

            public string? ObjType { get; init; }

            public string? EnabledHistory { get; init; }

            public long? IdObject { get; init; }

            public long? CampoComune { get; init; }

            public string? Tipologia { get; init; }

            public string? SegnaturaRepertorio { get; set; }

            public string? Impronta { get; set; }

        }

        #endregion
    }

}
