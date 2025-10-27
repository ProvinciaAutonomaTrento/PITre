// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.FriendApplication;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Exceptions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RegistriStampaRequest = Pi3.App.Legacy.WebApi.Application.Requests.RegistriStampa;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.RegistriStampa
{
    public class RegistriStampaHandler : IRequestHandler<RegistriStampaRequest, RegistriStampaResult>
    {
        #region Public members
        public RegistriStampaHandler(ILogger<RegistriStampaHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext,
            IReportGeneratorService reportGeneratorService, IFileConverterService fileConverterService, IDocumentoAmministrativoRepository docRepository, 
            IDocumentBlobRepository blobRepository, IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalServce = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._reportGeneratorService = reportGeneratorService;
            this._fileConverterService = fileConverterService;
            this._docRepository = docRepository;
            this._blobRepository = blobRepository;
            this._webMethodLoggerService = webMethodLoggerService;
        }
        public async Task<RegistriStampaResult> Handle(RegistriStampaRequest request, CancellationToken cancellationToken)
        {
            var idRegistroAsLong = request.registro.systemId.AsLong();

            var idTenant = this._claimsPrincipalServce.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

            var output = new StampaRegistroResult();

            try
            {

                var registroEntity = await this._dbContext.RegistroEntities.FirstAsync(x => x.SYSTEM_ID == idRegistroAsLong);

                if (registroEntity.CHA_STATO == "A")
                {
                    throw new RegistroInStatoApertoPi3Exception(request.registro.systemId);
                }

                var amministraEntity = await this._dbContext.AmministraEntities.FirstAsync(x => x.SYSTEM_ID == idTenant);

                long? year = default;
                long? numStart = default;
                long? numEnd = default;
                DateTime? lastPrintDate = default;

                var stampaRegistriEntities = await this._dbContext.StampaRegistriEntities.AsNoTracking()
                    .Where(x => x.ID_REGISTRO == idRegistroAsLong)
                    .OrderByDescending(x => x.NUM_ANNO).ThenByDescending(x => x.NUM_PROTO_END).ThenByDescending(x => x.DTA_STAMPA)
                    .ToListAsync();

                if (!stampaRegistriEntities.Any())
                {
                    // Se non ci sono stampe si inizia la stampa del registro di protocollo dal primo numero del primo anno di protocollazione

                    var profileEntities = await this._dbContext.ProfileEntities.AsNoTracking()
                        .Where(x => x.ID_REGISTRO == idRegistroAsLong
                        && (x.CHA_TIPO_PROTO == "A" || x.CHA_TIPO_PROTO == "P" || x.CHA_TIPO_PROTO == "I")
                        && x.NUM_ANNO_PROTO.HasValue)
                        .GroupBy(x => x.NUM_ANNO_PROTO)
                        .OrderBy(x => x.Key)
                        .FirstOrDefaultAsync();

                    if (profileEntities is null || !profileEntities.Any())
                    {
                        // Nessun protocollo nel registro
                        throw new DocumentsNotFoundPi3Exception();
                    }
                    else
                    {
                        // ProtoStart, ProtoEnd, Anno
                        year = profileEntities.Key;
                        numStart = profileEntities.Min(x => x.NUM_PROTO);
                        numEnd = profileEntities.Max(x => x.NUM_PROTO);
                    }
                }
                else
                {
                    // Se ci sono stampe devo partire dall'ultimo numero stampato e stampare fino all'ultimo numero registrato per l'anno di riferimento
                    year = stampaRegistriEntities.First().NUM_ANNO;
                    numStart = stampaRegistriEntities.First().NUM_PROTO_END + 1;
                    numEnd = await this.GetMaxNumProto(idRegistroAsLong, year);
                    lastPrintDate = stampaRegistriEntities.First().DTA_STAMPA;
                }

                if (!numEnd.HasValue || numStart >= numEnd) throw new DocumentsNotFoundPi3Exception();

                List<ReportRegistriItem> entities = await this._dbContext.ProfileEntities.AsNoTracking()
                    .Where(x => x.ID_REGISTRO == idRegistroAsLong
                    && x.NUM_ANNO_PROTO == year
                    && (x.CHA_TIPO_PROTO == "A" || x.CHA_TIPO_PROTO == "P" || x.CHA_TIPO_PROTO == "I")
                    && x.NUM_PROTO >= numStart
                    && x.NUM_PROTO <= numEnd)
                    .OrderBy(x => x.NUM_PROTO)
                    .Select(x => new ReportRegistriItem
                    {
                        Docnumber = x.SYSTEM_ID,
                        RecordNumber = x.NUM_PROTO,
                        RecordDate = x.DTA_PROTO,
                        RecordType = x.CHA_TIPO_PROTO,
                        CancellationDate = x.DTA_ANNULLA,
                        Subject = x.VAR_PROF_OGGETTO,
                        SenderRecipients = IPi3DbContextMappedFunctions.CorrCat(x.SYSTEM_ID, x.CHA_TIPO_PROTO!),
                        Folders = IPi3DbContextMappedFunctions.ClassCat(x.SYSTEM_ID),
                        Hash = IPi3DbContextMappedFunctions.GetImprontaWithAllegati(x.SYSTEM_ID),
                        IsSubjectModified = !(IPi3DbContextMappedFunctions.IsOggettoModificato(x.SYSTEM_ID) == "0"),
                        IsSenderOrRecipientsModified = this._dbContext.CorrStoEntities.AsNoTracking().Any(y => y.ID_PROFILE == x.SYSTEM_ID && y.DTA_MODIFICA > x.DTA_PROTO)
                    })
                    .ToListAsync();

                List<ReportRegistriItem> modifiedEntities = default;

                if (lastPrintDate.HasValue)
                {
                    // Ricerca variazioni
                    var listaVariazioni = await this.GetListaVariazioni(idRegistroAsLong, year, numStart, lastPrintDate.Value);

                    if (listaVariazioni.Any()) modifiedEntities = new List<ReportRegistriItem>(listaVariazioni);
                }

                // Estrazione lettere documenti
                this.lettereDocumenti = await this._dbContext.AssLettereDocumentiEntities.AsNoTracking()
                    .Join(this._dbContext.LetteraDocumentoEntities.AsNoTracking(), a => a.ID_LETTERADOC, b => b.SYSTEM_ID, (a, b) => new { a, b })
                    .Where(x => x.a.ID_AMM == idTenant)
                    .Select(x => x.b)
                    .ToListAsync();

                // Estrazione motivo modifica
                var modifiedItemsKey = (await this._dbContext.ChiaviConfigurazioneEntities.FirstOrDefaultAsync(x => x.VAR_CODICE == "BE_RIF_PROV_AUTORIZZAZIONE"))?.VAR_VALORE;
                this.modifiedSubjectReasonInfo = string.IsNullOrWhiteSpace(modifiedItemsKey) ? string.Empty : string.Format(Resources.TextModifiedSubject, modifiedItemsKey);

                var report = new ReportModel
                {
                    Size = PageSizes.A4,
                    Orientation = PageOrientations.Landscape,
                    OutputType = ReportOutputTypes.AsPdf
                };

                report.AddSection(this.GetTitleSection(amministraEntity.VAR_DESC_AMM!));
                report.AddSection(this.GetTitleSection(string.Format(Resources.SubTitleRegisterInfo, registroEntity.VAR_CODICE, registroEntity.VAR_DESC_REGISTRO)));
                report.AddSection(this.GetSubTitleSection(string.Format(Resources.SubTitleReportRange, numStart, numEnd)));

                // Intestazione
                var gridNewItems = new GridSectionModel
                {
                    Style = new GridSectionStyleModel { WithPercentage = 100 }
                };

                gridNewItems.AddRow(this.GetHeaderRow());

                entities.ForEach(x => gridNewItems.AddRow(this.GetReportRow(x)));

                report.AddSection(gridNewItems);

                // Sezione protocolli modificati
                if (modifiedEntities is not null && modifiedEntities.Any())
                {
                    report.AddSection(this.GetSubTitleSection(string.Format(Resources.SubTitleModifiedItems, lastPrintDate!.Value.ToString("dd/MM/yyyy HH:mm:ss"))));
                    var gridModifiedItems = new GridSectionModel
                    {
                        Style = new GridSectionStyleModel { WithPercentage = 100 }
                    };

                    gridModifiedItems.AddRow(this.GetHeaderRow());

                    modifiedEntities.ForEach(x => gridModifiedItems.AddRow(this.GetReportRow(x)));

                    report.AddSection(gridModifiedItems);
                }

                var aggregate = new DocumentoAmministrativo(
                    idTenant.ToString(),
                    DateTime.Now,
                    new OggettoDelDocumento
                    {
                        Descrizione = new(string.Format(Resources.ReportSubject,
                            year,
                            numStart,
                            numEnd))
                    },
                    new DatiRegistro()
                    {
                         IdRegistro = request.registro.systemId
                    },
                    null,
                    TipologieVisibilitaEnum.Gerarchica,
                    null);

                aggregate.AssignDatiStampa(new DatiStampa()
                {
                    TipoStampa = TipologieStampaEnum.StampaRegistroProtocollo,
                    AnnoStampa = year,
                    CodiceRegistro = registroEntity.VAR_CODICE,
                    PrimoElementoStampato = new DatiRegistrazioneProtocollo()
                    {
                          NumeroProtocollo = numStart,
                          
                    },
                    UltimoElementoStampato = new DatiRegistrazioneProtocollo()
                    {
                        NumeroProtocollo = numEnd
                    }
                });

                var filename = $"{registroEntity.VAR_CODICE} - {registroEntity.VAR_DESC_REGISTRO}_{numStart}_{numEnd}.docx";

                var blobAggregate = new DocumentBlob(
                    idTenant.ToString(),
                    DateTime.Now,
                    new TextValue { Value = filename }
                    );

                using (var stream = new MemoryStream())
                {
                    var generatedReport = await this._reportGeneratorService.Generate(report, stream);
                    var content = stream.ToArray();

                    //File.WriteAllBytes("C:\\TEMP\\PITRE\\stamparegistro.docx", stream.ToArray());

                    // Inserimento nell'aggregate
                    //blobAggregate.UploadStream(stream, filename);
                    //blobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                    //var convertedReport = await this._fileConverterService.Convert("report.docx", stream.ToArray(), FileConverterOutputFormatsEnum.ToPdf);

                    filename = $"{registroEntity.VAR_CODICE} - {registroEntity.VAR_DESC_REGISTRO}_{numStart}_{numEnd}.pdf";
                    // Inserimento nell'aggregate
                    blobAggregate.UploadStream(new MemoryStream(content), filename, generatedReport.ContentType);
                    blobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);
                    //File.WriteAllBytes("C:\\TEMP\\PITRE\\stamparegistro.pdf", convertedReport.Content);

                    await this._blobRepository.Add(blobAggregate);

                    var hash = blobAggregate.Hash;

                    aggregate.AssignDocumentBlobRef(
                            new Core.AggregateModels.DocumentAggregate.ValueObjects.DocumentBlobRef
                            {
                                IdBlob = blobAggregate.Id,
                                CreationDate = await _dbContext.GetSystemDateTime(),
                                ContentType = generatedReport.ContentType,
                                FileName = filename,
                                FileSize = blobAggregate.FileSize,
                                Hash = hash,
                                HashName = Core.AggregateModels.DocumentAggregate.ValueObjects.HashNamesEnum.SHA256
                            },
                            new Core.AggregateModels.DocumentAggregate.ValueObjects.TargetVersionBehavior
                            {
                                CreateNewVersion = true,
                                Name = new TextValue { Value = filename },
                            }
                            );
                }

                await this._docRepository.Add(aggregate);

                // Aggiornamento profile
                var profilesToUpdateEntities = this._dbContext.ProfileEntities
                    .Where(x => (x.CHA_TIPO_PROTO == "A" || x.CHA_TIPO_PROTO == "P" || x.CHA_TIPO_PROTO == "I")
                    && x.ID_REGISTRO == idRegistroAsLong
                    && x.NUM_ANNO_PROTO == year
                    && x.NUM_PROTO >= numStart
                    && x.NUM_PROTO <= numEnd
                    );

                await profilesToUpdateEntities.ForEachAsync(x => x.CHA_CONGELATO = "1");

                output.docNumber = aggregate.Id;

                // TO DO Invio in conservazione

                await ((DbContext)this._dbContext).SaveChangesAsync();

                //Metadati AGID
                await this._webMethodLoggerService.LogOK("DOCUMENTOADDDOCGRIGIA", output.docNumber, string.Format(Resources.LogDocumentoAddDocGrigio, output.docNumber));

                await this._webMethodLoggerService.LogOK("REGISTRIDISTAMPA", request.registro.systemId, string.Format(Resources.LogEntry, request.registro.codice));
            }
            catch(RegistroInStatoApertoPi3Exception ex)
            {
                this._logger.LogWarning(ex.Message);
                output.errore = ErrorDescriptions.RegisterOpen;
            }
            catch(DocumentsNotFoundPi3Exception ex)
            {
                this._logger.LogWarning(ex.Message);
                output.errore = ErrorDescriptions.NoItemsFound;
            }
            catch(Exception ex)
        {
                this._logger.LogError(ex.Message);
                await this._webMethodLoggerService.LogKO("REGISTRIDISTAMPA", request.registro.systemId, string.Format(Resources.LogEntry, request.registro.codice));
        }

            return new RegistriStampaResult(output);
        }
        #endregion

        #region Private members
        private ILogger<RegistriStampaHandler> _logger;
        protected IClaimsPrincipalService _claimsPrincipalServce;
        protected IMediator _mediator;
        protected IPi3DbContext _dbContext;
        protected IReportGeneratorService _reportGeneratorService;
        protected IFileConverterService _fileConverterService;
        protected IDocumentoAmministrativoRepository _docRepository;
        protected IDocumentBlobRepository _blobRepository;
        protected IWebMethodLoggerService _webMethodLoggerService;

        protected List<LetteraDocumentoEntity>? lettereDocumenti;
        protected string? modifiedSubjectReasonInfo;

        protected async Task<long?> GetMaxNumProto(long? idRegistro, long? year)
        {
            return (await this._dbContext.ProfileEntities.AsNoTracking()
                .Where(x => x.ID_REGISTRO == idRegistro
                && x.NUM_ANNO_PROTO == year
                && (x.CHA_TIPO_PROTO == "P" || x.CHA_TIPO_PROTO == "A" || x.CHA_TIPO_PROTO == "I")
                && x.NUM_PROTO.HasValue)
                .OrderByDescending(x => x.NUM_PROTO.Value)
                .FirstAsync()).NUM_PROTO;
        }

        protected async Task<List<ReportRegistriItem>> GetListaVariazioni(long? idRegistro, long? year, long? numStart, DateTime lastPrintDate)
        {
            var rootPredicate = PredicateBuilder.New<ProfileEntity>(true);
            rootPredicate.And(x => x.ID_REGISTRO == idRegistro);
            rootPredicate.And(x => x.CHA_TIPO_PROTO == "A" || x.CHA_TIPO_PROTO == "P" || x.CHA_TIPO_PROTO == "I");

            var predicate = PredicateBuilder.New<ProfileEntity>(false);

            // 1 - Data annullamento
            predicate.Or(x => x.DTA_ANNULLA.HasValue && x.DTA_ANNULLA > lastPrintDate);

            // 2 - Modifica oggetto
            var oggettiStoEntities = await this._dbContext.OggettiStoEntities.AsNoTracking()
                .Where(x => x.DTA_MODIFICA > lastPrintDate)
                .Select(x => x.ID_PROFILE)
                .ToListAsync();

            predicate.Or(x => x.CHA_MOD_OGGETTO == "1" && oggettiStoEntities.Any(y => y == x.SYSTEM_ID));

            // 3 - Corrispondenti storicizzati
            var corrStoEntities = await this._dbContext.CorrStoEntities.AsNoTracking()
                .Where(x => x.DTA_MODIFICA > lastPrintDate).
                Select(x => x.ID_PROFILE)
                .ToListAsync();

            predicate.Or(x => (x.CHA_MOD_MITT_DEST == "1" || x.CHA_MOD_MITT_INT == "1") && corrStoEntities.Any(y => y == x.SYSTEM_ID));

            // 4 - Nuove versioni
            var versionEntities = await this._dbContext.VersionEntities.AsNoTracking()
                .Where(x => x.DTA_CREAZIONE > lastPrintDate)
                .Select(x => x.DOCNUMBER)
                .ToListAsync();

            predicate.Or(x => versionEntities.Any(y => y == x.SYSTEM_ID));

            // 5 - File acquisiti dopo l'ultima stampa
            var componentsEntities = await this._dbContext.ComponentEntities.AsNoTracking()
                .Where(x => x.DTA_FILE_ACQUIRED > lastPrintDate)
                .Select(x => x.DOCNUMBER)
                .ToListAsync();

            predicate.Or(x => componentsEntities.Any(y => y == x.SYSTEM_ID));

            // 6 - Nuove versioni allegati
            var versionAllegatiEntities = await this._dbContext.VersionEntities.AsNoTracking()
                .Join(this._dbContext.ProfileEntities.AsNoTracking(), v => v.DOCNUMBER, p => p.DOCNUMBER, (v, p) => new { v, p })
                .Where(x => x.v.DTA_CREAZIONE > lastPrintDate)
                .Select(x => x.p.ID_DOCUMENTO_PRINCIPALE)
                .ToListAsync();

            predicate.Or(x => versionAllegatiEntities.Any(y => y == x.SYSTEM_ID));

            var currentYearPredicate = PredicateBuilder.New<ProfileEntity>(true);
            currentYearPredicate.And(rootPredicate);
            currentYearPredicate.And(x => x.NUM_ANNO_PROTO == year);
            currentYearPredicate.And(x => x.NUM_PROTO <= numStart);
            currentYearPredicate.And(predicate);

            var currentYearEntities = await this._dbContext.ProfileEntities.AsNoTracking()
                .Where(currentYearPredicate)
                .Select(x => new ReportRegistriItem
                {
                    Docnumber = x.SYSTEM_ID,
                    RecordNumber = x.NUM_PROTO,
                    RecordDate = x.DTA_PROTO,
                    RecordType = x.CHA_TIPO_PROTO,
                    CancellationDate = x.DTA_ANNULLA,
                    Subject = x.VAR_PROF_OGGETTO,
                    SenderRecipients = IPi3DbContextMappedFunctions.CorrCat(x.SYSTEM_ID, x.CHA_TIPO_PROTO!),
                    Folders = IPi3DbContextMappedFunctions.ClassCat(x.SYSTEM_ID),
                    Hash = IPi3DbContextMappedFunctions.GetImprontaWithAllegati(x.SYSTEM_ID),
                    IsSubjectModified = !(IPi3DbContextMappedFunctions.IsOggettoModificato(x.SYSTEM_ID) == "0"),
                    IsSenderOrRecipientsModified = this._dbContext.CorrStoEntities.AsNoTracking().Any(y => y.ID_PROFILE == x.SYSTEM_ID && y.DTA_MODIFICA > x.DTA_PROTO)
                })
                .ToListAsync();

            var pastYearsPredicate = PredicateBuilder.New<ProfileEntity>(true);
            pastYearsPredicate.And(rootPredicate);
            pastYearsPredicate.And(x => x.NUM_ANNO_PROTO < year);
            pastYearsPredicate.And(predicate);

            var pastYearsEntities = await this._dbContext.ProfileEntities.AsNoTracking()
                .Where(pastYearsPredicate)
                .Select(x => new ReportRegistriItem
                {
                    Docnumber = x.SYSTEM_ID,
                    RecordNumber = x.NUM_PROTO,
                    RecordDate = x.DTA_PROTO,
                    RecordType = x.CHA_TIPO_PROTO,
                    CancellationDate = x.DTA_ANNULLA,
                    Subject = x.VAR_PROF_OGGETTO,
                    SenderRecipients = IPi3DbContextMappedFunctions.CorrCat(x.SYSTEM_ID, x.CHA_TIPO_PROTO!),
                    Folders = IPi3DbContextMappedFunctions.ClassCat(x.SYSTEM_ID),
                    Hash = IPi3DbContextMappedFunctions.GetImprontaWithAllegati(x.SYSTEM_ID),
                    IsSubjectModified = !(IPi3DbContextMappedFunctions.IsOggettoModificato(x.SYSTEM_ID) == "0"),
                    IsSenderOrRecipientsModified = this._dbContext.CorrStoEntities.AsNoTracking().Any(y => y.ID_PROFILE == x.SYSTEM_ID && y.DTA_MODIFICA > x.DTA_PROTO)
                })
                .ToListAsync();

            var modifiedEntities = new List<ReportRegistriItem>();
            modifiedEntities.AddRange(currentYearEntities);
            modifiedEntities.AddRange(pastYearsEntities);

            return modifiedEntities.OrderBy(x => x.Docnumber).ToList();
        }

        protected TextSectionModel GetTitleSection(string text)
        {
            return new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Left },
                Content = new TextContentModel
                {
                    Style = new TextStyleModel
                    {
                        FontName = "ARIAL",
                        FontSize = 14,
                        FontIsBold = true,
                    },
                    Value = text
                }
            };
        }

        protected TextSectionModel GetSubTitleSection(string text)
        {
            return new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Left },
                Content = new TextContentModel
                {
                    Style = new TextStyleModel
                    {
                        FontName = "ARIAL",
                        FontSize = 12,
                        FontIsBold = false,
                    },
                    Value = text
                }
            };
        }

        protected GridRowModel GetHeaderRow()
        {
            var header = new GridRowModel();

            var cellStyle = new TextStyleModel
            {
                FontName = "ARIAL",
                FontSize = 8,
                FontIsBold = true
            };

            header.AddCell(new GridCellModel { Style = this.HeaderCellStyle(percentage: 7), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderRegistrationNumber } });
            header.AddCell(new GridCellModel { Style = this.HeaderCellStyle(percentage: 6), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderRegistrationDate } });
            header.AddCell(new GridCellModel { Style = this.HeaderCellStyle(percentage: 8), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderCancellationDate } });
            header.AddCell(new GridCellModel { Style = this.HeaderCellStyle(percentage: 3), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderRecordType } });
            header.AddCell(new GridCellModel { Style = this.HeaderCellStyle(percentage: 28), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderSubject } });
            header.AddCell(new GridCellModel { Style = this.HeaderCellStyle(percentage: 24), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderSenderRecipients } });
            header.AddCell(new GridCellModel { Style = this.HeaderCellStyle(percentage: 8), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderFolders  } });
            header.AddCell(new GridCellModel { Style = this.HeaderCellStyle(percentage: 13), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderHash } });
            header.AddCell(new GridCellModel { Style = this.HeaderCellStyle(percentage: 3), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderAttachments } });

            return header;
        }

        protected GridRowModel GetReportRow(ReportRegistriItem item)
        {
            var row = new GridRowModel();

            var textStyle = new TextStyleModel
            {
                FontName = "ARIAL",
                FontSize = 7
            };

            row.AddCell(cell: new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = $"{item.RecordNumber} {item.EmergencyRecordNumber ?? string.Empty}" } });
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = item.RecordDate.HasValue ? item.RecordDate.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty } });
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = item.CancellationDate.HasValue ? item.CancellationDate.Value.ToString("dd/MM/yyyy") : string.Empty} });
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = this.GetLetteraDocumento(item.RecordType) } });
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleLeft, Content = new TextContentModel { Style = textStyle, Value = this.GetSubjectField(item) } });
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleLeft, Content = new TextContentModel { Style = textStyle, Value = this.GetSenderRecipientField(item) } });
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = item.Folders ?? string.Empty } }); // FASCICOLI
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleLeft, Content = new TextContentModel { Style = textStyle, Value = item.Hash ?? string.Empty } });
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = item.AttachmentsNumber.ToString() } });

            return row;
        }

        protected string GetLetteraDocumento(string? codice)
        {
            return this.lettereDocumenti.FirstOrDefault(x => x.CODICE == codice)?.DESCRIZIONE ?? string.Empty;
        }

        protected string GetSubjectField(ReportRegistriItem item)
        {
            if(item.IsSubjectModified && !string.IsNullOrWhiteSpace(this.modifiedSubjectReasonInfo))
            {
                return item.Subject + this.modifiedSubjectReasonInfo;
            }
            else return item.Subject ?? string.Empty;
        }

        protected string GetSenderRecipientField(ReportRegistriItem item)
        {
            if (item.IsSenderOrRecipientsModified && !string.IsNullOrWhiteSpace(this.modifiedSubjectReasonInfo))
            {
                return item.SenderRecipients + this.modifiedSubjectReasonInfo;
            }
            else return item.SenderRecipients ?? string.Empty;
        }

        protected async Task<string> GetAttachmentsNumberField(long docnumber)
        {
            return (await this._dbContext.ProfileEntities.AsNoTracking()
                .Where(x => x.ID_DOCUMENTO_PRINCIPALE == docnumber)
                .CountAsync())
                .ToString();
        }

        private GridCellStyleModel CellStyleCentered
            => new GridCellStyleModel
            {
                Justification = Justifications.Center,
                VerticalAlignment = VerticalAlignments.Top
            };

        private GridCellStyleModel CellStyleLeft
            => new GridCellStyleModel
            {
                Justification = Justifications.Left,
                VerticalAlignment = VerticalAlignments.Top
            };

        private GridCellStyleModel HeaderCellStyle(int percentage)
            => new GridCellStyleModel
            {
                Justification = Justifications.Center,
                WithPercentage = percentage,
                ForegroundColor = System.Drawing.Color.Silver
            };

        public class ReportRegistriItem : ValueObject
        {
            public long Docnumber { get; set; }

            public long? RecordNumber { get; set; }

            public DateTime? RecordDate { get; set; }

            public string? RecordType { get; set; }

            public DateTime? CancellationDate { get; set; }
            
            public string? EmergencyRecordNumber { get; set; } 

            public string? Subject { get; set; }

            public string? SenderRecipients { get; set; }

            public string? Folders { get; set; }

            public string? Hash { get; set; }

            public long AttachmentsNumber { get; set; }

            public bool IsSubjectModified { get; set; }

            public bool IsSenderOrRecipientsModified { get; set; }

        }

        #endregion
    }
}