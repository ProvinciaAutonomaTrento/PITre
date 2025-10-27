// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.Decorators;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using DocumentoGetFileConSegnaturaRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetFileConSegnatura;
using DocumentoGetAllegatiRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetAllegati;
using MediatR.NotificationPublishers;
using FascicolazioneGetFoldersDocumentRequest = Pi3.App.Legacy.WebApi.Application.Requests.FascicolazioneGetFoldersDocument;
using DocsPaVO.fascicolazione;
using FascicolazioneGetFascicoliDaDocRequest = Pi3.App.Legacy.WebApi.Application.Requests.FascicolazioneGetFascicoliDaDoc;
using Microsoft.Extensions.Options;
using Pi3.App.Legacy.WebApi.Application.Models;
using Pi3.Core.Services.Configuration;
using Pi3.App.Legacy.WebApi.Application.Services.SessionRepository;
using Pi3.Core.Services.File.CAdES;
using AutoMapper;
using Pi3.Core.Services.File.FirmaDigitale2;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetFileConSegnatura
{
    public class DocumentoGetFileConSegnaturaHandler : IRequestHandler<DocumentoGetFileConSegnaturaRequest, DocumentoGetFileConSegnaturaResult>
    {
        public DocumentoGetFileConSegnaturaHandler(ILogger<DocumentoGetFileConSegnaturaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IFileDecoratorService decoratorService,
            IFileConverterFactory fileConverterFactory,
            IMediator mediator,
            IDistributedCache distributedCache,
            IDocumentBlobRepository documentBlobRepository,
            IWebMethodLoggerService webMethodLoggerService,
            IPi3DbContext dbContext,
            IConfigurationService configurationService,
            ISessionRepositoryService sessionRepositoryService,
            ICAdESService cAdESService,
            IFirmaDigitale2Service firmaDigitale2Service)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._documentBlobRepository = documentBlobRepository;
            this._decoratorService = decoratorService;
            this._distributedCache = distributedCache;
            this._webMethodLoggerService = webMethodLoggerService;
            this._configurationService = configurationService;
            this._fileConverterFactory = fileConverterFactory;
            this._sessionRepositoryService = sessionRepositoryService;
            this._cAdESService = cAdESService;
            this._firmaDigitale2Service = firmaDigitale2Service;

            this.InitializeMapper();
        }


        public async Task<DocumentoGetFileConSegnaturaResult> Handle(DocumentoGetFileConSegnaturaRequest request, CancellationToken cancellationToken)
        {
            SchedaDocumento sch = request.sch;
            DocsPaVO.documento.FileDocumento fileDoc = new DocsPaVO.documento.FileDocumento();
            try
            {
                if (!request.Forced)
                {
                    fileDoc = await GetFileConSegnatura(request.fileRequest, request.sch, request.infoUtente, request.position, request.convertToPdf);
                    await this._webMethodLoggerService.LogOK("DOCUMENTOGETFILE", request.sch.docNumber, string.Format(Resources.LogGetFileSegnaturaForced, request.fileRequest.docNumber, request.fileRequest.version));
                }
                else
                {
                    fileDoc = await GetVoidFileConSegnatura(request.fileRequest, request.sch, request.infoUtente, request.position);
                    if (sch.protocollo != null)
                        await this._webMethodLoggerService.LogOK("DOCUMENTOGETFILE", 0.ToString(), string.Format(Resources.LogGetFileSegnatura, request.sch.protocollo.segnatura));
                }

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                fileDoc = null;
                if (!request.Forced)
                {
                    await this._webMethodLoggerService.LogKO("DOCUMENTOGETFILE", request.sch.docNumber, string.Format(Resources.LogGetFileSegnaturaForced, request.fileRequest.docNumber, request.fileRequest.version));
                }
                else
                {
                    if (sch.protocollo != null)
                        await this._webMethodLoggerService.LogKO("DOCUMENTOGETFILE", request.sch.docNumber, string.Format(Resources.LogGetFileSegnatura, request.sch.protocollo.segnatura));
                }

            }

            return new DocumentoGetFileConSegnaturaResult(fileDoc);

        }

        #region Private Members

        protected readonly ILogger<DocumentoGetFileConSegnaturaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly IFileDecoratorService _decoratorService;
        protected readonly IDistributedCache _distributedCache;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IConfigurationService _configurationService;
        protected readonly IFileConverterFactory _fileConverterFactory;
        protected readonly ISessionRepositoryService _sessionRepositoryService;
        protected readonly ICAdESService _cAdESService;
        protected readonly IFirmaDigitale2Service _firmaDigitale2Service;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AmministrazioneEntity, Amministrazione>()
                    .ForMember(dest => dest.id, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.codice, src => src.MapFrom(opt => opt.VAR_CODICE_AMM))
                    .ForMember(dest => dest.colore, src => src.MapFrom(opt => opt.ID_COLORE_DF))
                    .ForMember(dest => dest.orientamento, src => src.MapFrom(opt => opt.ORIENTAMENTO))
                    .ForMember(dest => dest.posizione, src => src.MapFrom(opt => opt.ID_POS_DF))
                    .ForMember(dest => dest.rotazione, src => src.MapFrom(opt => opt.TIPO_ROTAZ ?? "0"))
                    .ForMember(dest => dest.timbroPDF, src => src.MapFrom(opt => opt.VAR_FORMATO_TIMBRO))
                    .ForMember(dest => dest.carattere, src => src.MapFrom(opt => (opt.ID_CARAT_DF == 0 || opt.ID_CARAT_DF == null) ? "1" : opt.ID_CARAT_DF.ToString()))
                    .ForMember(dest => dest.colore, src => src.MapFrom(opt => (opt.ID_COLORE_DF == 0 || opt.ID_COLORE_DF == null) ? "1" : opt.ID_COLORE_DF.ToString()))
                    .ForMember(dest => dest.posizione, src => src.MapFrom(opt => (opt.ID_POS_DF == 0 || opt.ID_POS_DF == null) ? "1" : opt.ID_POS_DF.ToString()));
            });

            this._mapper = configuration.CreateMapper();

        }

        protected async Task<FileDocumento> GetFileConSegnatura(FileRequest fileRequest, SchedaDocumento schedaDocumento, InfoUtente infoUtente, labelPdf labelPdf, bool convertToPdf)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);

            int indice = fileRequest.fileName.LastIndexOf(@"\");
            var text = string.Empty;
            var fileDocumento = new FileDocumento();
            fileDocumento.LabelPdf.default_position = string.Empty;
            fileDocumento.LabelPdf.sel_color = !string.IsNullOrEmpty(labelPdf.sel_color) ? labelPdf.sel_color : fileDocumento.LabelPdf.sel_color;
            fileDocumento.LabelPdf.sel_font = !string.IsNullOrEmpty(labelPdf.sel_font) ? labelPdf.sel_font : fileDocumento.LabelPdf.sel_font;
            fileDocumento.path = fileRequest.docServerLoc + fileRequest.path;
            fileDocumento.name = indice < (fileRequest.fileName.Length - 1) ? fileRequest.fileName.Substring(indice + 1) : fileRequest.fileName;
            fileDocumento.fullName = string.IsNullOrEmpty(fileDocumento.path) ? fileDocumento.name : fileDocumento.path + '\u005C'.ToString() + fileDocumento.name;

            if (fileRequest.repositoryContext != null)
            {
                FileDocumento fileDocRepositotyContext = await _sessionRepositoryService.GetFile(fileRequest.repositoryContext, fileRequest);
                fileDocumento.content = fileDocRepositotyContext.content;
                fileDocumento.estensioneFile = await GetEstensioneIntoSignedFile(fileDocumento.name);
            }
            else
            {
                var componentsEntity = await _dbContext.ComponentEntities.AsNoTracking()
                    .Where(c => c.VERSION_ID == fileRequest.versionId.AsLong())
                    .Select(c => new
                    {
                        c.PATH,
                        c.VAR_NOMEORIGINALE,
                        c.EXT
                    })
                    .FirstAsync();

                fileDocumento.path = componentsEntity.PATH;
                fileDocumento.nomeOriginale = componentsEntity.VAR_NOMEORIGINALE;
                fileDocumento.estensioneFile = componentsEntity.EXT;

                DocumentBlob blob = await this._documentBlobRepository.Get(idTenant.ToString(), fileDocumento.path);

                using (var memoryStream = new MemoryStream())
                {
                    blob.Stream.CopyTo(memoryStream);
                    fileDocumento.content = memoryStream.ToArray();
                }
            }

            var applications = (await GetApplications(fileDocumento.estensioneFile))[0];
            fileDocumento.contentType = applications != null && !string.IsNullOrEmpty(applications.mimeType) ? applications.mimeType : "application/x-" + fileDocumento.estensioneFile;

            var originalFile = fileDocumento.content;
            if ((fileDocumento.fullName.ToUpper().EndsWith("P7M")) || //cades
                    (fileDocumento.fullName.ToUpper().EndsWith("M7M")))//timestamp
            {
                using (var msSignedFile = new MemoryStream(fileDocumento.content))
                {
                    using (var msOriginalFile = new MemoryStream())
                    {
                        await this._cAdESService.LoadOriginalFile(fileDocumento.fullName, msSignedFile, msOriginalFile);
                        fileDocumento.content = msOriginalFile.ToArray();
                    }
                }

                fileDocumento.name = Path.GetFileNameWithoutExtension(fileDocumento.name);
            }

            if (fileDocumento.fullName.ToUpper().EndsWith("TSD")) //timestamp
            {
                var verificaFirmaResponse = await this._firmaDigitale2Service.Verifica(new VerificaRequest()
                {
                    VerificaCompleta = false,
                    FileFirmato = originalFile,
                    DataVerifica = DateTime.Now,
                    TipoVerifica = TipiVerifica.Appiattita,
                    ReturnFileOriginale = true,
                    ReturnXmlCompleto = true
                });

                fileDocumento.content = verificaFirmaResponse.Documento.FileOriginale;
                fileDocumento.name = Path.GetFileNameWithoutExtension(fileDocumento.name);

            }

            fileDocumento.length = fileDocumento.content.Length;

            if (fileDocumento != null && !string.IsNullOrEmpty(fileDocumento.name) && (Path.GetExtension(fileDocumento.name).ToLowerInvariant() != ".pdf") 
                && (convertToPdf || (await _mediator.Send(new Requests.CanConvertFileToPdf(fileDocumento.name))).output))
            {
                try
                {
                    var creation = await _fileConverterFactory.TryCreate(fileDocumento.name);
                    if (creation.Success)
                    {
                        using var stream = new MemoryStream(fileDocumento.content);
                        var convertResult = await creation.Service.Convert(
                               fileDocumento.name,
                               stream,
                               FileConverterOutputFormatsEnum.ToPdf);

                        fileDocumento.content = convertResult.Content;
                        fileDocumento.length = convertResult.Content.Length;
                        fileDocumento.contentType = convertResult.ContentType;
                        fileDocumento.name += ".pdf";
                        fileDocumento.fullName += ".pdf";
                        fileDocumento.estensioneFile = "pdf";

                    }
                }
                catch (Exception ex)
                {
                    this._logger.LogCritical(exception: ex, message: ex.Message);
                }
            }

            //Applico l'etichetta solo su file pdf e su versione senza segnatura permanente
            if (!fileRequest.conSegnaturaPermanente && fileDocumento.estensioneFile.ToLowerInvariant() == "pdf")
            {
                string fileName = @"\" + Guid.NewGuid() + "_" + userId + ".pdf";

                var amministraEntity = await _dbContext.AmministraEntities.AsNoTracking()
                    .Where(a => a.SYSTEM_ID == idTenant)
                    .FirstAsync();

                var amministrazione = this._mapper.Map<Amministrazione>(amministraEntity);
                amministrazione.caratTimbro = this._dbContext.CaratTimbroEntities.OrderBy(ct => ct.SYSTEM_ID).ToList();
                amministrazione.coloreTimbro = this._dbContext.ColoreTimbroEntities.OrderBy(ct => ct.SYSTEM_ID).ToList();
                amministrazione.posizioneTimbro = this._dbContext.PosizTimbroEntities.OrderBy(ct => ct.SYSTEM_ID).ToList();

                //caricamento preferenze utente per LabelPDF
                LoadXmlLabelProperties(fileDocumento, labelPdf.position, amministrazione);

                //Stringa del timbro o della segnatura
                int maxT = 0;
                string maxTimbro = "";
                string escape = "\n";
                var dati = string.Empty;
                var datiProtRepertorio = string.Empty;
                var datiFirma = string.Empty;

                var idDoc = schedaDocumento.documentoPrincipale != null ? schedaDocumento.documentoPrincipale.docNumber.AsLong() : schedaDocumento.docNumber.AsLong();

                var infoDocumentoPrincipale = (from p in this._dbContext.ProfileEntities
                                               join ta in this._dbContext.TipoAttoEntities on p.ID_TIPO_ATTO equals ta.SYSTEM_ID into profile
                                               from pr in profile.DefaultIfEmpty()
                                               where p.SYSTEM_ID == idDoc
                                               select new InfoProfile
                                               {
                                                   DOCNUMBER = p.DOCNUMBER,
                                                   NUM_PROTO = p.NUM_PROTO,
                                                   VAR_SEGNATURA = p.VAR_SEGNATURA,
                                                   ID_TIPO_ATTO = p.ID_TIPO_ATTO,
                                                   VAR_DESC_ATTO = pr.VAR_DESC_ATTO
                                               }).FirstOrDefault();

                if (labelPdf.notimbro == false && !labelPdf.repertorioTrasversale)
                {
                    dati = await GetDatiEtichetta(infoUtente, amministrazione, labelPdf, schedaDocumento, fileDocumento.signatureResult, string.Empty);
                    //rimuovo l'ultimo separatore!!!
                    dati = dati.TrimEnd(Convert.ToChar(escape));

                    // se il documento è repertoriato, visualizza il protocollo di repertorio in alternativa alla segnatura
                    if (string.IsNullOrEmpty(infoDocumentoPrincipale.VAR_SEGNATURA) && infoDocumentoPrincipale.ID_TIPO_ATTO != null)
                    {
                        datiProtRepertorio = await GetDatiEtichettaProtocolloRepertorio(infoDocumentoPrincipale, amministrazione.codice);
                    }

                    if (!string.IsNullOrEmpty(datiProtRepertorio))
                        // il protocollo di repertorio viene visualizzato solo se non è attivo la visualizzazione del timbro (orizzontale/verticale)
                        if (labelPdf.orientamento != null && !labelPdf.orientamento.ToLower().Equals("orizzontale") && !labelPdf.orientamento.ToLower().Equals("verticale"))
                            dati = datiProtRepertorio;

                    if (schedaDocumento.documentoPrincipale != null || fileRequest.GetType() == typeof(Allegato))
                    {
                        //var allegato = (await this._mediator.Send(new DocumentoGetAllegatiRequest(schedaDocumento.documentoPrincipale.docNumber, string.Empty, string.Empty)))
                        //    .output.Where(a => a.docNumber == schedaDocumento.docNumber).First();
                        string tipoAllegato = string.Empty;
                        int position = 0;
                        string versionLabel = string.Empty;
                        if(fileRequest.GetType() == typeof(Allegato))
                        {
                            tipoAllegato = DecodeTypeAttachment((fileRequest as Allegato).TypeAttachment);
                            position = (fileRequest as Allegato).position;
                            versionLabel = (fileRequest as Allegato).versionLabel;
                        }
                        else
                        {

                            var allegato = (await this._mediator.Send(new DocumentoGetAllegatiRequest(schedaDocumento.documentoPrincipale.docNumber, string.Empty, string.Empty)))
                                .output.Where(a => a.docNumber == schedaDocumento.docNumber).First();

                            tipoAllegato = DecodeTypeAttachment(allegato.TypeAttachment);
                            position = allegato.position;
                            versionLabel = allegato.versionLabel;
                        }
                        var orientamento = labelPdf.orientamento.ToLower() == "verticale" ? "\n" : " - ";
                        dati += $"{orientamento}Allegato {tipoAllegato} {position} ({versionLabel})";
                    }
                }
                if (labelPdf.repertorioTrasversale && infoDocumentoPrincipale.ID_TIPO_ATTO != null)
                {
                    datiProtRepertorio = await GetDatiEtichettaProtocolloRepertorioTrasversale(infoDocumentoPrincipale, amministrazione.codice);
                    if (!string.IsNullOrEmpty(datiProtRepertorio))
                        dati = datiProtRepertorio;
                }

                string[] rTimbro = dati.Split(Convert.ToChar(escape));

                for (int t = 0; t < rTimbro.Length; t++)
                {
                    if (rTimbro[t].Length > maxT)
                    {
                        maxTimbro = rTimbro[t];
                        maxT = rTimbro[t].Length;
                    }
                    //Rimuovo gli spazi bianchi dai dati del timbro che in caso di orientamento
                    //verticale creano un disallineamento!
                    rTimbro[t] = rTimbro[t].Trim();
                }

                //string timbro = String.Join("", rTimbro);
                string timbro = dati;

                //se dal frontEnd ho valorizzato la rotazione allora la sostituisco a quella configurata
                if (labelPdf.label_rotation != String.Empty)
                    amministrazione.rotazione = labelPdf.label_rotation;

                //valorizzo i dati da passare al frontEnd
                fileDocumento.LabelPdf.label_rotation = amministrazione.rotazione;
                fileDocumento.LabelPdf.orientamento = labelPdf.orientamento;
                fileDocumento.LabelPdf.tipoLabel = labelPdf.tipoLabel;

                if (labelPdf.digitalSignInfo != null && fileRequest.firmato == "1")
                {
                    var verificaFirmaResponse = await this._firmaDigitale2Service.Verifica(new VerificaRequest()
                    {
                        VerificaCompleta = fileDocumento.fullName.ToUpper().EndsWith("P7M"),
                        FileFirmato = originalFile,
                        DataVerifica = DateTime.Now,
                        TipoVerifica = TipiVerifica.Appiattita,
                        ReturnFileOriginale = false,
                        ReturnXmlCompleto = false
                    });
                    datiFirma = Environment.NewLine + await GetDatiFirmaPerEtichetta(verificaFirmaResponse, amministraEntity.VAR_DETTAGLIO_FIRMA, labelPdf, amministraEntity.SYSTEM_ID.ToString());

                    //Aggiunta informazioni di firma elettronica
                    datiFirma += Environment.NewLine + await GetDatiFirmaElettronica(schedaDocumento.docNumber, fileRequest.versionId, labelPdf, infoUtente);
                }

                //TODO: Per ogni valore nell'oggetto occorre una chiamata al decorator?
                try
                {
                    List<KeyValuePair<int, string>> infoPageEtichetta = new List<KeyValuePair<int, string>>();
                    System.Collections.Generic.Dictionary<int, string[]> pageContentStructure = new System.Collections.Generic.Dictionary<int, string[]>();
                    infoPageEtichetta.Add(new KeyValuePair<int, string>(1, timbro));

                    //Faccio una chiamata a vuoto al decoretor, per ottenere il numero di pagine
                    var fileDecoratedContent = await _decoratorService.Decorate(fileName, fileDocumento.content, new FileDecoratorInstructions(), FileDecoratorOutputFormatsEnum.ToPdf);

                    //var result = fileDecoratedContent;
                    string numberOfPages = fileDecoratedContent.Metadata.Where(k => k.Key == "document.Pages.Count").Select(k => k.Value).FirstOrDefault();

                    if (!string.IsNullOrEmpty(datiFirma))
                    {
                        infoPageEtichetta = new List<KeyValuePair<int, string>>();
                        string[] datiFirmaAsArray = datiFirma.Split(Convert.ToChar(escape));

                        if (labelPdf.digitalSignInfo.printOnFirstPage)
                        {
                            // Se si è richiesta la stampa dei dati di firma sulla prima pagina, 
                            // viene accodata alla struttura l'item relativo alla pagina e il contenuto
                            //System.Collections.Generic.List<string> content = new System.Collections.Generic.List<string>(pageContentStructure[1]);
                            //content.AddRange(datiFirmaAsArray);
                            timbro += datiFirma;
                            infoPageEtichetta.Add(new KeyValuePair<int, string>(1, timbro));
                        }

                        if (labelPdf.digitalSignInfo.printOnLastPage)
                        {
                            //TODO: devo ricavare il numero delle pagine
                            // Se si è richiesta la stampa dei dati di firma sull'ultima pagina, viene inserita nella struttura l'item relativo alla pagina e il contenuto
                            if (numberOfPages == "1")
                            {
                                //System.Collections.Generic.List<string> content = new System.Collections.Generic.List<string>(pageContentStructure[1]);
                                //content.AddRange(datiFirmaAsArray);
                                //pageContentStructure[1] = content.ToArray();
                                timbro += datiFirma;
                                infoPageEtichetta.Add(new KeyValuePair<int, string>(1, timbro));
                            }
                            else
                            {
                                infoPageEtichetta.Add(new KeyValuePair<int, string>(1, timbro));
                                infoPageEtichetta.Add(new KeyValuePair<int, string>(Int32.Parse(numberOfPages), datiFirma));

                            }
                        }
                        fileDocumento.LabelPdf.digitalSignInfo = labelPdf.digitalSignInfo;
                    }

                    #region Decorator
                    LayerRotationsEnum rotation = LayerRotationsEnum.None;

                    switch (labelPdf.label_rotation)
                    {
                        case "90":
                            rotation = LayerRotationsEnum.Degrees90;
                            break;
                        case "180":
                            rotation = LayerRotationsEnum.Degrees180;
                            break;
                        case "270":
                            rotation = LayerRotationsEnum.Degrees270;
                            break;
                        default:
                            rotation = LayerRotationsEnum.None;
                            break;
                    }


                    FileDecoratorInstructions decoratorInstructions = new FileDecoratorInstructions();
                    if (infoPageEtichetta != null)
                    {
                        int layersCount = infoPageEtichetta.Count();

                        decoratorInstructions.Layers = new ILayer[layersCount];
                        int i = 0;
                        Point? posPers = new Point();
                        LayerDefaultPositionsEnum? defPos = new LayerDefaultPositionsEnum();

                        posPers = null;
                        DocsPaVO.documento.position position = fileDocumento.LabelPdf.positions.Where(p => p.posName == fileDocumento.LabelPdf.default_position).FirstOrDefault();
                        if (position != null)
                        {
                            posPers = new Point(Int32.Parse(position.PosX), Int32.Parse(position.PosY));
                            defPos = null;
                        }
                        else
                        {
                            switch (fileDocumento.LabelPdf.default_position)
                            {

                                case "pos_upSx":
                                    defPos = LayerDefaultPositionsEnum.TopLeft;
                                    break;
                                case "pos_upDx":
                                    defPos = LayerDefaultPositionsEnum.TopRight;
                                    break;
                                case "pos_downSx":
                                    defPos = LayerDefaultPositionsEnum.BottomLeft;
                                    break;
                                case "pos_downDx":
                                    defPos = LayerDefaultPositionsEnum.BottomRight;
                                    break;
                                default:
                                    defPos = LayerDefaultPositionsEnum.TopLeft;
                                    break;
                            }
                        }

                        TextLayerFontRgbColor color = new TextLayerFontRgbColor()
                        {
                            R = 255,
                            G = 0,
                            B = 0
                        };

                        if (fileDocumento.LabelPdf.font_color != null && fileDocumento.LabelPdf.font_color == "BLACK")
                        {
                            color = new TextLayerFontRgbColor()
                            {
                                R = 0,
                                G = 0,
                                B = 0
                            };
                        }

                        foreach (KeyValuePair<int, string> pair in infoPageEtichetta)
                        {
                            decoratorInstructions.Layers[i] = new TextLayer()
                            {
                                CustomPosition = posPers,
                                Rotation = rotation,
                                Position = defPos,
                                FontForeColor = color,
                                FontName = fileDocumento.LabelPdf.font_type,
                                FontSize = float.Parse(fileDocumento.LabelPdf.font_size),
                                Text = pair.Value,
                                PageNumbersToApplyLayer = new int[1] { pair.Key }
                            };
                            i++;
                        }
                    }

                    _logger.LogInformation("INIZIO chiamata al _decoratorService");
                    text = (decoratorInstructions.Layers[0] as TextLayer).Text;
                    var fileDocumentDecoratedContent = await _decoratorService.Decorate(fileName, fileDocumento.content, decoratorInstructions, FileDecoratorOutputFormatsEnum.ToPdf);
                    _logger.LogInformation("FINE chiamata al _decoratorService");

                    fileDocumento.name = fileName;
                    fileDocumento.estensioneFile = "PDF";
                    fileDocumento.content = fileDocumentDecoratedContent.Content;
                    fileDocumento.contentType = fileDocumentDecoratedContent.ContentType;
                    fileDocumento.LabelPdf.pdfHeight = fileDocumentDecoratedContent.Metadata.Where(m => m.Key == "PageInfo.Height").Select(k => k.Value.Replace("pt", "")).FirstOrDefault();
                    fileDocumento.LabelPdf.pdfWidth = fileDocumentDecoratedContent.Metadata.Where(m => m.Key == "PageInfo.Width").Select(k => k.Value.Replace("pt", "")).FirstOrDefault();

                    if (!string.IsNullOrEmpty(fileDocumento.LabelPdf.pdfHeight) && fileDocumento.LabelPdf.pdfHeight.IndexOf('.') != -1)
                        fileDocumento.LabelPdf.pdfHeight = fileDocumento.LabelPdf.pdfHeight.Substring(0, fileDocumento.LabelPdf.pdfHeight.IndexOf('.'));

                    if (!string.IsNullOrEmpty(fileDocumento.LabelPdf.pdfWidth) && fileDocumento.LabelPdf.pdfWidth.IndexOf('.') != -1)
                        fileDocumento.LabelPdf.pdfWidth = fileDocumento.LabelPdf.pdfWidth.Substring(0, fileDocumento.LabelPdf.pdfWidth.IndexOf('.'));
                }
                catch (Pi3Exception pi3Ex)
                {
                    _logger.LogError(exception: pi3Ex, message: $"Id documento: {fileRequest.docNumber} {pi3Ex.Message}. Decorator instructions: {text}");
                    
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(exception: ex, message: $"Id documento: {fileRequest.docNumber} {ex.Message}. Decorator instructions: {text}");
                }
                #endregion
            }

            _logger.LogInformation("FINE chiamata al GetFileConSegnatura");

            return fileDocumento;
        }

        protected async Task<FileDocumento> GetVoidFileConSegnatura(FileRequest fileRequest, SchedaDocumento schedaDocumento, InfoUtente infoUtente, labelPdf labelPdf)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var fileDocumento = new FileDocumento();

            //aggiungo il carattere ed il colore selezionati all'oggetto label del fileDoc
            if (labelPdf != null)
            {
                if (!string.IsNullOrEmpty(labelPdf.sel_font))
                {
                    fileDocumento.LabelPdf.sel_font = labelPdf.sel_font;
                }
                if (!string.IsNullOrEmpty(labelPdf.sel_color))
                {
                    fileDocumento.LabelPdf.sel_color = labelPdf.sel_color;
                }
            }

            var amministraEntity = await _dbContext.AmministraEntities.AsNoTracking()
                   .Where(a => a.SYSTEM_ID == idTenant)
                   .FirstAsync();

            var amministrazione = this._mapper.Map<Amministrazione>(amministraEntity);
            amministrazione.caratTimbro = this._dbContext.CaratTimbroEntities.OrderBy(ct => ct.SYSTEM_ID).ToList();
            amministrazione.coloreTimbro = this._dbContext.ColoreTimbroEntities.OrderBy(ct => ct.SYSTEM_ID).ToList();
            amministrazione.posizioneTimbro = this._dbContext.PosizTimbroEntities.OrderBy(ct => ct.SYSTEM_ID).ToList();

            //caricamento preferenze utente per LabelPDF
            LoadXmlLabelProperties(fileDocumento, labelPdf.position, amministrazione);

            if (fileDocumento.content == null)
            {
                fileDocumento.content = Resources.PDF_Empty;
            }
            fileDocumento.length = fileDocumento.content.Length;
            fileDocumento.fullName = @"\" + schedaDocumento.docNumber + "_" + infoUtente.userId + ".pdf";
            fileDocumento.name = fileDocumento.fullName;
            fileDocumento.estensioneFile = "PDF";
            fileDocumento.nomeOriginale = fileDocumento.fullName;
            fileDocumento.contentType = "application/pdf";

            //Stringa del timbro o della segnatura
            int maxT = 0;
            string maxTimbro = "";
            string escape = "\n";
            string dati = await GetDatiEtichetta(infoUtente, amministrazione, labelPdf, schedaDocumento, fileDocumento.signatureResult);

            //se l'etichetta è vuota significa che il documento non è stato ancora creato, metto quindi una string fittizia
            if (string.IsNullOrEmpty(dati))
                dati = System.DateTime.Now.ToString();
            //rimuovo l'ultimo separatore!!!
            dati = dati.TrimEnd(Convert.ToChar(escape));

            string[] rTimbro = dati.Split(Convert.ToChar(escape));

            for (int t = 0; t < rTimbro.Length; t++)
            {
                if (rTimbro[t].Length > maxT)
                {
                    maxTimbro = rTimbro[t];
                    maxT = rTimbro[t].Length;
                }
                //Rimuovo gli spazi bianchi dai dati del timbro che in caso di orientamento
                //verticale creano un disallineamento!
                rTimbro[t] = rTimbro[t].Trim();
            }
            string timbro = dati;

            //se dal frontEnd ho valorizzato la rotazione allora la sostituisco a quella configurata
            if (labelPdf.label_rotation != String.Empty)
                amministrazione.rotazione = labelPdf.label_rotation;

            //valorizzo i dati da passare al frontEnd
            fileDocumento.LabelPdf.label_rotation = amministrazione.rotazione;
            fileDocumento.LabelPdf.orientamento = labelPdf.orientamento;
            fileDocumento.LabelPdf.tipoLabel = labelPdf.tipoLabel;

            List<KeyValuePair<int, string>> infoPageEtichetta = new List<KeyValuePair<int, string>>();
            Dictionary<int, string[]> pageContentStructure = new Dictionary<int, string[]>();
            infoPageEtichetta.Add(new KeyValuePair<int, string>(1, timbro));

            //Faccio una chiamata a vuoto al decoretor, per ottenere il numero di pagine
            var fileDecoratedContent = await _decoratorService.Decorate(fileDocumento.nomeOriginale, fileDocumento.content, new FileDecoratorInstructions(), FileDecoratorOutputFormatsEnum.ToPdf);
            string numberOfPages = fileDecoratedContent.Metadata.Where(k => k.Key == "document.Pages.Count").Select(k => k.Value).FirstOrDefault();

            LayerRotationsEnum rotation = LayerRotationsEnum.None;

            switch (labelPdf.label_rotation)
            {
                case "90":
                    rotation = LayerRotationsEnum.Degrees90;
                    break;
                case "180":
                    rotation = LayerRotationsEnum.Degrees180;
                    break;
                case "270":
                    rotation = LayerRotationsEnum.Degrees270;
                    break;
                default:
                    rotation = LayerRotationsEnum.None;
                    break;
            }

            FileDecoratorInstructions decoratorInstructions = new FileDecoratorInstructions();
            if (infoPageEtichetta != null && fileDocumento != null)
            {
                int layersCount = infoPageEtichetta.Count();

                decoratorInstructions.Layers = new ILayer[layersCount];
                int i = 0;
                Point? posPers = new Point();
                LayerDefaultPositionsEnum? defPos = new LayerDefaultPositionsEnum();

                posPers = null;
                DocsPaVO.documento.position position = fileDocumento.LabelPdf.positions.Where(p => p.posName == fileDocumento.LabelPdf.default_position).FirstOrDefault();
                if (position != null)
                {
                    posPers = new Point(Int32.Parse(position.PosX), Int32.Parse(position.PosY));
                    defPos = null;
                }
                else
                {
                    switch (fileDocumento.LabelPdf.default_position)
                    {

                        case "pos_upSx":
                            defPos = LayerDefaultPositionsEnum.TopLeft;
                            break;
                        case "pos_upDx":
                            defPos = LayerDefaultPositionsEnum.TopRight;
                            break;
                        case "pos_downSx":
                            defPos = LayerDefaultPositionsEnum.BottomLeft;
                            break;
                        case "pos_downDx":
                            defPos = LayerDefaultPositionsEnum.BottomRight;
                            break;
                        default:
                            defPos = LayerDefaultPositionsEnum.TopLeft;
                            break;
                    }
                }

                TextLayerFontRgbColor color = new TextLayerFontRgbColor()
                {
                    R = 255,
                    G = 0,
                    B = 0
                };

                if (fileDocumento.LabelPdf.font_color != null && fileDocumento.LabelPdf.font_color == "BLACK")
                {
                    color = new TextLayerFontRgbColor()
                    {
                        R = 0,
                        G = 0,
                        B = 0
                    };
                }

                foreach (System.Collections.Generic.KeyValuePair<int, string> pair in infoPageEtichetta)
                {
                    decoratorInstructions.Layers[i] = new TextLayer()
                    {
                        CustomPosition = posPers,
                        Rotation = rotation,
                        Position = defPos,
                        FontForeColor = color,
                        FontName = "Courier",
                        FontSize = float.Parse(fileDocumento.LabelPdf.font_size),
                        Text = pair.Value,
                        PageNumbersToApplyLayer = new int[1] { pair.Key }
                    };
                    i++;
                }
            }
            try
            {
                var fileDocumentoDecoratedContent = await _decoratorService.Decorate(fileDocumento.name, fileDocumento.content, decoratorInstructions, FileDecoratorOutputFormatsEnum.ToPdf);
                fileDocumento.content = fileDocumentoDecoratedContent.Content;
                fileDocumento.contentType = fileDocumentoDecoratedContent.ContentType;
                fileDocumento.LabelPdf.pdfHeight = fileDocumentoDecoratedContent.Metadata.Where(m => m.Key == "PageInfo.Height").Select(k => k.Value.Replace("pt", "")).FirstOrDefault();
                fileDocumento.LabelPdf.pdfWidth = fileDocumentoDecoratedContent.Metadata.Where(m => m.Key == "PageInfo.Width").Select(k => k.Value.Replace("pt", "")).FirstOrDefault();
            }
            catch (Pi3Exception ex)
            {
                _logger.LogError(exception: ex, message: $"Id documento: {fileRequest.docNumber} {ex.Message}");
            }
            return fileDocumento;
        }
        private static string DecodeTypeAttachment(int typeAttachment)
        {
            switch (typeAttachment)
            {
                case 1: //allegato tipo utente
                    return "Utente";
                case 2: //allegato tipo PEC
                    return "Pec";
                case 3: //allegato tipo IS
                    return "P.I.Tre.";
                case 4: //allegato tipo SE
                    return "Sist. Esterni";
                default:
                    return string.Empty;
            }
        }

        protected virtual async Task<string> GetEstensioneIntoSignedFile(string fileName)
        {
            var estensione = Path.GetExtension(fileName).Replace(".", string.Empty);

            if (fileName.ToUpper().EndsWith("P7M") ||
                fileName.ToUpper().EndsWith("TSD") ||
                fileName.ToUpper().EndsWith("M7M"))
            {
                estensione = fileName.Substring(fileName.IndexOf(".") + 1);

                while (estensione.LastIndexOf(".") > -1)
                {
                    if (!estensione.ToUpper().EndsWith("P7M") &&
                        !estensione.ToUpper().EndsWith("TSD") &&
                        !estensione.ToUpper().EndsWith("M7M"))
                        break;

                    estensione = estensione.Remove(estensione.LastIndexOf("."));
                }

                //Vado a rimuovere il (1) aggiunto dai browser
                if (estensione.EndsWith(")") && estensione.LastIndexOf("(") != -1)
                    estensione = estensione.Remove(estensione.LastIndexOf("("));

                //Può accadere che il nome del file contenga "." questo fa sì che l'estensione 
                //risulti sporca, per evitare ciò alla fine del precdente while ricalcolo l'estensione
                if (!string.IsNullOrEmpty(Path.GetExtension(estensione)))
                    estensione = Path.GetExtension(estensione).Replace(".", string.Empty);
            }

            return estensione;
        }

        protected string removeIllegalChars(string filename)
        {

            if (string.IsNullOrEmpty(filename))
                return filename;

            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            foreach (char c in invalid)
                filename = filename.Replace(c.ToString(), "_");

            return filename;
        }

        protected virtual async Task<string> GetDatiEtichetta(
                        DocsPaVO.utente.InfoUtente utente,
                        Amministrazione Amm,
                        DocsPaVO.documento.labelPdf labelInfo,
                        DocsPaVO.documento.SchedaDocumento sch,
                        DocsPaVO.documento.VerifySignatureResult signatureResult,
                        string version_label_allegato = ""
                            )
        {
            string retValue = string.Empty;
            //separatore fra un valore e l'altro del timbro
            string separatore = " ";
            //valore di fine riga per determinare se andare a capo oppure no
            string escape = "\n";
            //valore che verrà restituito come output alla richiesta di dati da stampare su pdf!!!
            string Timbro = String.Empty;
            //oggetto all'interno del quale leggere tutti i dati del timbro!!!
            Amministrazione currAmm = Amm;
            //parametri per recuperare la/le classifica
            string profile = sch.systemId;
            string people = utente.idPeople;
            string gruppo = utente.idGruppo;
            //è l'ultimo valore sostituito nella fase di creazione del timbro
            string[] lastVal = { "COD_AMM", "COD_REG", "NUM_PROTO", "DATA_COMP", "ORA", "NUM_ALLEG", "CLASSIFICA", "IN_OUT", "COD_UO_PROT", "COD_UO_VIS", "COD_RF_PROT", "COD_RF_VIS" };
            if (labelInfo.tipoLabel)
            {

                //orientamento del timbro non applicabile alla segnatura
                if (labelInfo.orientamento == String.Empty || labelInfo.orientamento == null)
                {
                    labelInfo.orientamento = currAmm.orientamento;
                }
                //nel caso che anche il valore letto in amministrazione sia null di default stampo il timbro in verticale
                if (labelInfo.orientamento.ToLower() == "orizzontale")
                {
                    escape = "";
                }
                else
                {
                    escape = "\n";
                }

                string TimbroIniziale = currAmm.timbroPDF;
                string datiTimbro = currAmm.timbroPDF;
                string sep = GetSeparatore(currAmm.id);

                //nome amministrazione
                if (datiTimbro.Contains("COD_AMM"))
                {
                    var codAmm = await _dbContext.AmministraEntities.Where(am => am.SYSTEM_ID == currAmm.id).Select(am => am.VAR_CODICE_AMM).FirstAsync();
                    if (codAmm != String.Empty)
                    {
                        datiTimbro = datiTimbro.Replace("COD_AMM", (codAmm + escape));
                        lastVal[0] = codAmm + escape;
                    }
                    else
                    {
                        datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_AMM", lastVal);
                        datiTimbro = datiTimbro.Replace("COD_AMM", "");
                    }
                }

                //questo è il codice AOO ed è valorizzato solo sui documenti protocollati
                if (datiTimbro.Contains("COD_REG"))
                {
                    // se si tratta di un allegato ad un protocollo lo recupero dal COD_REG del protocollo
                    if ((sch.documentoPrincipale != null) && (sch.documentoPrincipale.codRegistro != null))
                    {
                        lastVal[1] = sch.documentoPrincipale.codRegistro + escape;
                        datiTimbro = datiTimbro.Replace("COD_REG", (sch.documentoPrincipale.codRegistro + escape));
                    }
                    //bisogna effettuare un controllo sul fatto che sia valorizzato oppure no!
                    else if (sch.registro != null)
                    {
                        lastVal[1] = sch.registro.codRegistro + escape;
                        datiTimbro = datiTimbro.Replace("COD_REG", (sch.registro.codRegistro + escape));
                    }
                    else
                    {
                        datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_REG", lastVal);
                        datiTimbro = datiTimbro.Replace("COD_REG", "");
                    }
                }

                //numero protocollo
                if (datiTimbro.Contains("NUM_PROTO"))
                {
                    //Normalizzo il numero di protocollo secondo lo standard a 7 cifre
                    int MAX_LENGTH = 7;
                    string zeroes = "";
                    string numProto = "";

                    // se si tratta di un allegato ad un protocollo lo recupero dal NUM_PROTO del protocollo
                    if ((sch.documentoPrincipale != null) && (sch.documentoPrincipale.numProt != null))
                    {
                        numProto = sch.documentoPrincipale.numProt;
                        for (int ind = 1; ind <= MAX_LENGTH - numProto.Length; ind++)
                        {
                            zeroes = zeroes + "0";
                        }
                        numProto = zeroes + numProto;

                        datiTimbro = datiTimbro.Replace("NUM_PROTO", (numProto + escape));
                        lastVal[2] = numProto + escape;

                    }
                    // se si tratta di un allegato ad un grigio lo recupero dal docnumber del documento principale 
                    else if ((sch.documentoPrincipale != null) && (sch.documentoPrincipale.numProt == null) && sch.documentoPrincipale.docNumber != null)
                    {
                        numProto = sch.documentoPrincipale.docNumber;
                        for (int ind = 1; ind <= MAX_LENGTH - numProto.Length; ind++)
                        {
                            zeroes = zeroes + "0";
                        }
                        numProto = zeroes + numProto;

                        datiTimbro = datiTimbro.Replace("NUM_PROTO", ("ID: " + numProto + escape));
                        lastVal[2] = numProto + escape;

                    }
                    else if (sch.protocollo != null)
                    {
                        numProto = sch.protocollo.numero != null ? sch.protocollo.numero : string.Empty;
                        for (int ind = 1; ind <= MAX_LENGTH - numProto.Length; ind++)
                        {
                            zeroes = zeroes + "0";
                        }
                        numProto = zeroes + numProto;

                        //***********
                        datiTimbro = datiTimbro.Replace("NUM_PROTO", (numProto + escape));
                        lastVal[2] = numProto + escape;
                    }
                    // Per far funzionare il timbro anche con i documenti grigi
                    else if ((sch.documentoPrincipale == null) && (sch.tipoProto == "G"))
                    {
                        numProto = sch.docNumber;
                        for (int ind = 1; ind <= MAX_LENGTH - numProto.Length; ind++)
                        {
                            zeroes = zeroes + "0";
                        }
                        numProto = zeroes + numProto;
                        datiTimbro = datiTimbro.Replace("NUM_PROTO", ("ID: " + numProto + escape));
                        lastVal[2] = "ID: " + numProto + escape;
                    }
                    else
                    {
                        datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "NUM_PROTO", lastVal);
                        //Metodo alternativo al RemoveDesc che lascia le etichette ma gestisce la nuova linea
                        //datiTimbro = datiTimbro.Remove((datiTimbro.IndexOf("NUM_PROTO") - 1), 1);
                        //datiTimbro = datiTimbro.Replace("NUM_PROTO", escape);
                        datiTimbro = datiTimbro.Replace("NUM_PROTO", "");
                    }
                }

                //data protocollazione
                if (datiTimbro.Contains("DATA_COMP"))
                {
                    // se si tratta di un allegato ad un protocollo lo recupero dal DATA_COMP del protocollo
                    if (sch.documentoPrincipale != null)
                    {
                        //datiTimbro = datiTimbro.Replace("DATA_COMP", (sch.dataCreazione + escape));
                        //protocollo.dataProtocollazione + escape));

                        //PALUMBO: nel caso di documento Protocollato il metodo GetProtoData valorizza la dataApertura con la DataProtocollo
                        if (sch.documentoPrincipale.dataApertura != null)
                            datiTimbro = datiTimbro.Replace("DATA_COMP", (sch.documentoPrincipale.dataApertura + escape));
                        else
                            datiTimbro = datiTimbro.Replace("DATA_COMP", (sch.dataCreazione + escape));
                        lastVal[3] = sch.dataCreazione + escape;

                    }
                    // Per far funzionare il timbro anche con i documenti grigi
                    else if ((sch.documentoPrincipale == null) && (sch.tipoProto == "G"))
                    {
                        datiTimbro = datiTimbro.Replace("DATA_COMP", (sch.dataCreazione + escape));

                        lastVal[3] = sch.dataCreazione + escape;
                    }

                    else if (sch.protocollo != null)
                    {
                        datiTimbro = datiTimbro.Replace("DATA_COMP", (sch.protocollo.dataProtocollazione + escape));
                        lastVal[3] = sch.protocollo.dataProtocollazione + escape;
                    }
                    else
                    {
                        datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "DATA_COMP", lastVal);
                        //Metodo alternativo al RemoveDesc che lascia le etichette ma gestisce la nuova linea
                        //datiTimbro = datiTimbro.Remove((datiTimbro.IndexOf("DATA_COMP") - 1), 1);
                        //datiTimbro = datiTimbro.Replace("DATA_COMP", escape);
                        datiTimbro = datiTimbro.Replace("DATA_COMP", "");
                    }
                }

                //ora di protocollazione
                if (datiTimbro.Contains("ORA"))
                {
                    //aggiunta dell'ora di protocollazione nel timbro
                    string ora = sch.oraCreazione;
                    if ((ora != null) && (ora != ""))
                    {
                        //se l'ora è nel formato comprensivo dei secondi devo rimuovere i secondi prima di inserire l'ora!
                        if (ora.Length > 5)
                        {
                            ora = ora.Remove((ora.Length - 3), 3);
                        }
                        datiTimbro = datiTimbro.Replace("ORA", (ora + escape));
                        lastVal[4] = ora + escape;
                    }
                    else
                    {
                        datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "ORA", lastVal);
                        //Metodo alternativo al RemoveDesc che lascia le etichette ma gestisce la nuova linea
                        //datiTimbro = datiTimbro.Remove((datiTimbro.IndexOf("ORA") - 1), 1);
                        //datiTimbro = datiTimbro.Replace("ORA", escape);
                        datiTimbro = datiTimbro.Replace("ORA", "");
                    }
                }

                //numero allegati
                if (datiTimbro.Contains("NUM_ALLEG"))
                {
                    datiTimbro = datiTimbro.Replace("NUM_ALLEG", (sch.allegati.Count() + escape));
                    //datiTimbro = datiTimbro.Replace("NUM_ALLEG", "");
                    lastVal[5] = System.Convert.ToString(sch.allegati.Count()) + escape;
                }

                //classificazione o classificazioni del corrente documento protocollato
                if (datiTimbro.Contains("CLASSIFICA"))
                {
                    string padding = string.Empty;
                    List<Fascicolo> classifica = new List<Fascicolo>();
                    List<Folder> folders = new List<Folder>();
                    // Bisogna mettere il controllo su classifica perchè potrebbe non essere ancora assegnata!!!
                    // controllo prima se sono su un allegato ad un protocollo
                    string idprofile = sch.documentoPrincipale != null ? sch.documentoPrincipale.docNumber : profile;

                    var classificaResult = await this._mediator.Send(new FascicolazioneGetFascicoliDaDocRequest(utente, idprofile));
                    classifica = classificaResult.output.ToList();
                    // classifica = Fascicoli.FascicoloManager.getFascicoliDaDoc(utente, idprofile);



                    //TODO? sembra non esistere in collaudo

                    string key_beprojectlevel = await this._configurationService.GetValue<string>("BE_PROJECT_LEVEL");
                    if (!string.IsNullOrEmpty(key_beprojectlevel) && key_beprojectlevel.Equals("1"))
                    {
                        var foldersResult = await this._mediator.Send(new FascicolazioneGetFoldersDocumentRequest(sch.docNumber));
                        folders = foldersResult.output.ToList();
                    }

                    //Fascicoli.FolderManager.GetFoldersDocument(sch.docNumber);

                    if (classifica != null)
                    {
                        for (int i = 0; i < classifica.Count; i++)
                        {
                            DocsPaVO.fascicolazione.Fascicolo fascicolo = (DocsPaVO.fascicolazione.Fascicolo)classifica[i];
                            if (fascicolo.codice != String.Empty)
                            {
                                string temp = string.Empty;
                                var folder = (from DocsPaVO.fascicolazione.Folder f in folders
                                              where f.idFascicolo == fascicolo.systemID
                                              select f).FirstOrDefault();

                                if (folder != null)
                                {
                                    for (int j = 1; j < folder.codicelivello.Length / 4; j++)
                                    {
                                        string val = folder.codicelivello.Substring(j * 4, 4);
                                        temp += string.Format(".{0}", Convert.ToInt32(val));
                                    }

                                    // Questo approccio genera un errore ma al momento 
                                    // non mi viene in mente niente di più semplice
                                    //TODO
                                    //Timbro += padding + separatore + (i == 0 ? "[" : "; ") +
                                    //    GetCodiceFascicolo(currAmm.Fascicolatura, fascicolo.codice, temp) + escape;
                                }
                                else if (!string.IsNullOrEmpty(key_beprojectlevel) && key_beprojectlevel.Equals("1"))
                                {
                                    Timbro += padding + separatore + (i == 0 ? "[" : "; ") + fascicolo.codice + escape;
                                    if (escape != "")
                                        padding = ReturnDesc(TimbroIniziale, datiTimbro, "CLASSIFICA", lastVal);
                                }
                                else
                                {
                                    Timbro = Timbro + padding + separatore + (i == 0 ? "[" : "; ") + fascicolo.codice + escape;
                                    //Nel caso di orientamento verticale replico il separatore letto in amministrazione
                                    //altrimenti uso il seguente separatore
                                    padding = " -";
                                    if (escape != "")
                                        padding = ReturnDesc(TimbroIniziale, datiTimbro, "CLASSIFICA", lastVal);
                                }
                            }
                        }
                        if (Timbro == string.Empty)
                        {
                            datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "CLASSIFICA", lastVal);
                            //Metodo alternativo al RemoveDesc che lascia le etichette ma gestisce la nuova linea
                            //datiTimbro = datiTimbro.Remove((datiTimbro.IndexOf("CLASSIFICA") - 1), 1);
                            //datiTimbro = datiTimbro.Replace("CLASSIFICA", escape);
                            datiTimbro = datiTimbro.Replace("CLASSIFICA", "");
                        }
                        else
                        {
                            Timbro += "]";
                            datiTimbro = datiTimbro.Replace("CLASSIFICA", Timbro);
                            lastVal[6] = Timbro;
                        }
                    }
                }

                //Tipo di protocollo
                if (datiTimbro.Contains("IN_OUT"))
                {
                    string arrPart = "";
                    if (sch.protocollatore != null)
                    {
                        if (sch.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloEntrata))
                        {
                            arrPart = "A";
                        }
                        else if (sch.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloUscita))
                        {
                            arrPart = "P";
                        }
                        else if (sch.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloInterno))
                        {
                            arrPart = "I";
                        }
                    }
                    if (arrPart == string.Empty)
                    {
                        datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "IN_OUT", lastVal);
                        //Metodo alternativo al RemoveDesc che lascia le etichette ma gestisce la nuova linea
                        //datiTimbro = datiTimbro.Remove((datiTimbro.IndexOf("IN_OUT") - 1), 1);
                        //datiTimbro = datiTimbro.Replace("IN_OUT", escape);
                        datiTimbro = datiTimbro.Replace("IN_OUT", "");
                    }
                    else
                    {
                        datiTimbro = datiTimbro.Replace("IN_OUT", arrPart + escape);
                        lastVal[7] = arrPart + escape;
                    }
                }


                // GESTIONE UNITA' ORGANIZZATIVE ED RF ***********************************************
                if (datiTimbro.Contains("COD_UO_PROT"))
                {
                    if (sch.protocollatore != null)
                    {
                        //string uo = getCodiceUO(sch.creatoreDocumento.idCorrGlob_Ruolo);

                        //string uo = getCodiceUO(sch.protocollatore.ruolo_idCorrGlobali);
                        //ABBATANGELI - Necessario per creare il timbro correttamente anche se il ruolo è stato storicizzato
                        //TODO
                        //var ruoli = await (from components in this._dbContext.CorrGlobaliEntities
                        //                                           join people in this._dbContext.PeopleEntities
                        //                                               on components.ID_PEOPLE_PUTFILE equals people.SYSTEM_ID into autore
                        //                                           from people in autore.DefaultIfEmpty()
                        //                                           join peopleDelegato in this._dbContext.PeopleEntities
                        //                                              on components.ID_PEOPLE_DELEGATO_PUTFILE equals peopleDelegato.SYSTEM_ID into delegato
                        //                                           from peopleDelegato in delegato.DefaultIfEmpty()
                        //                                           where components.DOCNUMBER == profileEntity.SYSTEM_ID
                        //                                           select new { Component = components, AuthorUserId = people.USER_ID, DelegatoUserId = peopleDelegato.USER_ID })
                        //                    .AsNoTracking()
                        //                    .ToListAsync();


                        var ruolo = await (from corGlob in this._dbContext.CorrGlobaliEntities
                                           join tipoRuolo in this._dbContext.TipoRuoloEntities
                                               on corGlob.ID_TIPO_RUOLO equals tipoRuolo.SYSTEM_ID into tipo
                                           from tipoRuolo in tipo.DefaultIfEmpty()
                                           join peopleGroups in this._dbContext.PeopleGroupEntities
                                              on corGlob.ID_GRUPPO equals peopleGroups.GROUPS_SYSTEM_ID into associazioni
                                           from peopleGroups in associazioni.DefaultIfEmpty()
                                           join corGlobUO in this._dbContext.CorrGlobaliEntities
                                              on corGlob.ID_UO equals corGlobUO.SYSTEM_ID into uoRole
                                           from corGlobUO in uoRole.DefaultIfEmpty()
                                           where corGlob.SYSTEM_ID == sch.creatoreDocumento.idCorrGlob_Ruolo.AsLong()

                                           select new { Ruolo = corGlob, NumLivello = corGlobUO.NUM_LIVELLO, IdUO = corGlobUO.SYSTEM_ID })
                                           .OrderByDescending(o => o.NumLivello)
                                            .AsNoTracking()
                                            .FirstAsync();

                        var connectByPrior = (await this._configurationService.GetValue<string>("USA_CONNECTBYPRIOR_OR_WITH"));
                        if (!String.IsNullOrEmpty(connectByPrior) && connectByPrior.Equals("1"))
                        {
                            IQueryable<CorrGlobaliEntity> query = this._dbContext.CorrGlobaliEntities.Where(cg => cg.CHA_TIPO_IE == "I" && cg.CHA_TIPO_URP == "U" && cg.DTA_FINE == null).AsQueryable();
                            List<long?> connectByPriorList = await GetConnectByPrior(ruolo.IdUO);
                            //  CONNECT BY PRIOR id_parent = system_id START WITH system_id =" + row["ID_UO"].ToString()
                            //var ruoliUtente = this._dbContext.CorrGlobaliEntities.Where(cg => cg.CHA_TIPO_IE == "I" && cg.CHA_TIPO_URP == "U" && cg.DTA_FINE == null)
                            if (connectByPriorList.Count > 0)
                            {
                                query = query.Where(cg => connectByPrior.Contains(cg.ID_UO.ToString()));
                            }
                        }
                        string uo = ""; //getCodiceUOEnabledAndDisabled(sch.protocollatore.ruolo_idCorrGlobali);
                        //uo = sch.protocollatore.uo_codiceCorrGlobali;
                        if (!string.IsNullOrEmpty(uo))
                        {
                            datiTimbro = datiTimbro.Replace("COD_UO_PROT", (uo + escape));
                            lastVal[8] = uo + escape;
                        }
                        else
                        {
                            datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_UO_PROT", lastVal);
                            datiTimbro = datiTimbro.Replace("COD_UO_PROT", "");
                        }
                    }
                    else
                    {
                        datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_UO_PROT", lastVal);
                        datiTimbro = datiTimbro.Replace("COD_UO_PROT", "");
                    }
                }

                if (datiTimbro.Contains("COD_UO_VIS"))
                {
                    if (utente.idCorrGlobali != string.Empty)
                    {
                        //TODO
                        string uo = ""; //getCodiceUO(utente.idCorrGlobali);
                        if (!string.IsNullOrEmpty(uo))
                        {
                            datiTimbro = datiTimbro.Replace("COD_UO_VIS", (uo + escape));
                            lastVal[9] = uo + escape;
                        }
                        else
                        {
                            datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_UO_VIS", lastVal);
                            datiTimbro = datiTimbro.Replace("COD_UO_VIS", "");
                        }
                    }
                    else
                    {
                        datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_UO_VIS", lastVal);
                        datiTimbro = datiTimbro.Replace("COD_UO_VIS", "");
                    }
                }

                if (datiTimbro.Contains("COD_RF_PROT"))
                {
                    if (sch.protocollatore != null)
                    {
                        //TODO
                        string rf = "";//getCodiceRF(sch.protocollatore.ruolo_idCorrGlobali);
                        if (!string.IsNullOrEmpty(rf))
                        {
                            datiTimbro = datiTimbro.Replace("COD_RF_PROT", (rf + escape));
                            lastVal[10] = rf + escape;
                        }
                        else
                        {
                            datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_RF_PROT", lastVal);
                            datiTimbro = datiTimbro.Replace("COD_RF_PROT", "");
                        }
                    }
                    else
                    {
                        datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_RF_PROT", lastVal);
                        datiTimbro = datiTimbro.Replace("COD_RF_PROT", "");
                    }
                }

                if (datiTimbro.Contains("COD_RF_VIS"))
                {
                    if (utente.idCorrGlobali != string.Empty)
                    {
                        //TODO
                        string rf = "";//getCodiceRF(utente.idCorrGlobali);
                        if (!string.IsNullOrEmpty(rf))
                        {
                            datiTimbro = datiTimbro.Replace("COD_RF_VIS", (rf + escape));
                            lastVal[11] = rf + escape;
                        }
                        else
                        {
                            datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_RF_VIS", lastVal);
                            datiTimbro = datiTimbro.Replace("COD_RF_VIS", "");
                        }
                    }
                    else
                    {
                        datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_RF_VIS", lastVal);
                        datiTimbro = datiTimbro.Replace("COD_RF_VIS", "");
                    }
                }
                // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


                Timbro = datiTimbro;

                //nel caso di documento annullato stampo il timbro di annullamento come segue
                if (sch.protocollo != null && sch.protocollo.protocolloAnnullato != null)
                    Timbro = separatore + sch.protocollo.segnatura + "\n" + separatore + "Annullato il: " + sch.protocollo.protocolloAnnullato.dataAnnullamento + "\n" + separatore + "Motivo: " + sch.protocollo.protocolloAnnullato.autorizzazione;

                retValue = string.IsNullOrEmpty(version_label_allegato) ? Timbro : Timbro + " - " + version_label_allegato;
                //retValue = Timbro;
            }
            else
            {

                //nel caso di documento annullato stampo il timbro di annullamento come segue
                if (sch.protocollo != null && sch.protocollo.protocolloAnnullato != null)
                {
                    Timbro = sch.protocollo.segnatura + "\n" + "Annullato il: " + sch.protocollo.protocolloAnnullato.dataAnnullamento + "\n" + "Motivo: " + sch.protocollo.protocolloAnnullato.autorizzazione;
                    retValue = Timbro;
                }
                else
                {
                    //protocolli
                    if (sch != null && (sch.protocollo != null
                     && sch.protocollo.segnatura != null
                     && sch.protocollo.segnatura != ""))
                    {
                        //segnatura in alternativa al timbro ed all'annullamento!!!
                        retValue = string.IsNullOrEmpty(version_label_allegato) ? sch.protocollo.segnatura : sch.protocollo.segnatura + " - " + version_label_allegato;
                    }
                    //predisposti o grigi
                    else if (sch != null && (sch.protocollo != null
                     && string.IsNullOrEmpty(sch.protocollo.segnatura)
                     && !string.IsNullOrEmpty(sch.systemId))

                        || (sch.protocollo == null
                            && !string.IsNullOrEmpty(sch.systemId)))

                        if (sch.documentoPrincipale != null)
                        {
                            if (!string.IsNullOrEmpty(sch.documentoPrincipale.segnatura))
                                retValue = string.IsNullOrEmpty(version_label_allegato) ? sch.documentoPrincipale.segnatura : sch.documentoPrincipale.segnatura + " - " + version_label_allegato;
                            else
                                retValue = string.IsNullOrEmpty(version_label_allegato) ? sch.documentoPrincipale.docNumber + "  " + sch.dataCreazione.ToString() : sch.documentoPrincipale.docNumber + "  " + sch.dataCreazione.ToString() + " - " + version_label_allegato;
                        }

                        else
                            retValue = string.IsNullOrEmpty(version_label_allegato) ? sch.docNumber + "  " + sch.dataCreazione.ToString() : sch.docNumber + "  " + sch.dataCreazione.ToString() + " - " + version_label_allegato;
                }
            }

            return retValue;
        }

        protected virtual async Task<List<long?>> GetConnectByPrior(long system_id)
        {
            List<long?> result = new List<long?>();

            result.Add(system_id);

            while (system_id > 0)
            {
                var id = _dbContext.CorrGlobaliEntities.Where(w => w.ID_PARENT == system_id).Select(s => s.SYSTEM_ID).FirstOrDefault();

                if (id > 0)
                    result.Add(id);

                system_id = id;
            }

            return result;
        }

        /// <summary>
        /// Questo metodo rimuove i separatori o la descrizione dei campi non valorizzati del timbro
        /// </summary>
        /// <returns></returns>
        protected virtual string RemoveDesc(string timbro_iniziale, string currTimbro, string currVal, string[] dati)
        {
            int count = 0;
            int start = 0;
            int inizio = 0;
            string specialChar = "#%*@";
            //Uso un ciclo while nel caso ci sia per errore più di un'occorrenza del codice da rimuovere!
            while (currTimbro.Contains(currVal))
            {
                //devo ricalcolare l'indice dei codici precedenti
                int[] ordine = codicePrec(timbro_iniziale);

                if (currVal.Equals("COD_AMM"))
                {
                    if (ordine[0] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[0]]) + dati[ordine[0]].Length;
                        count = currTimbro.IndexOf("COD_AMM") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("COD_AMM");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("COD_REG"))
                {
                    if (ordine[1] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[1]]) + dati[ordine[1]].Length;
                        count = currTimbro.IndexOf("COD_REG") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("COD_REG");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("NUM_PROTO"))
                {
                    if (ordine[2] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[2]]) + dati[ordine[2]].Length;
                        count = currTimbro.IndexOf("NUM_PROTO") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("NUM_PROTO");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("DATA_COMP"))
                {
                    if (ordine[3] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[3]]) + dati[ordine[3]].Length;
                        count = currTimbro.IndexOf("DATA_COMP") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("DATA_COMP");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("ORA"))
                {
                    if (ordine[4] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[4]]) + dati[ordine[4]].Length;
                        count = currTimbro.IndexOf("ORA") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("ORA");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("NUM_ALLEG"))
                {
                    if (ordine[5] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[5]]) + dati[ordine[5]].Length;
                        count = currTimbro.IndexOf("NUM_ALLEG") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("NUM_ALLEG");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("CLASSIFICA"))
                {
                    if (ordine[6] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[6]]) + dati[ordine[6]].Length;
                        count = currTimbro.IndexOf("CLASSIFICA") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("CLASSIFICA");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("IN_OUT"))
                {
                    if (ordine[7] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[7]]) + dati[ordine[7]].Length;
                        count = currTimbro.IndexOf("IN_OUT") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("IN_OUT");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }

                // GESTIONE UNITA' ORGANIZZATIVE ED RF ***********************************************
                if (currVal.Equals("COD_UO_PROT"))
                {
                    if (ordine[8] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[8]]) + dati[ordine[8]].Length;
                        count = currTimbro.IndexOf("COD_UO_PROT") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("COD_UO_PROT");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("COD_UO_VIS"))
                {
                    if (ordine[9] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[9]]) + dati[ordine[9]].Length;
                        count = currTimbro.IndexOf("COD_UO_VIS") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("COD_UO_VIS");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("COD_RF_PROT"))
                {
                    if (ordine[10] >= 0)
                    {
                        start = GetStartIndex(dati, ordine, currTimbro, 10);
                        count = currTimbro.IndexOf("COD_RF_PROT") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("COD_RF_PROT");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("COD_RF_VIS"))
                {
                    if (ordine[11] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[11]]) + dati[ordine[11]].Length;
                        count = currTimbro.IndexOf("COD_RF_VIS") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("COD_RF_VIS");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            }
            currTimbro = currTimbro.Replace(specialChar, currVal);
            return currTimbro;
        }

        private static int GetStartIndex(string[] dati, int[] ordine, string currTimbro, int index)
        {
            int start = 0;
            if (ordine[index] >= 0)
            {
                if (currTimbro.IndexOf(dati[ordine[index]]) < 0)
                {
                    start = GetStartIndex(dati, ordine, currTimbro, ordine[index]);
                }
                else
                {
                    start = currTimbro.IndexOf(dati[ordine[index]]) + dati[ordine[index]].Length;
                }
            }
            return start;
        }

        private static int[] codicePrec(string timbro_iniziale)
        {
            int[] ordine = new int[12];
            int i = -1;
            while (timbro_iniziale != string.Empty)
            {
                string appo = timbro_iniziale;
                if (timbro_iniziale.StartsWith("COD_AMM"))
                {
                    ordine[0] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_AMM", "");
                    i = 0;
                }
                if (timbro_iniziale.StartsWith("COD_REG"))
                {
                    ordine[1] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_REG", "");
                    i = 1;
                }
                if (timbro_iniziale.StartsWith("NUM_PROTO"))
                {
                    ordine[2] = i;
                    timbro_iniziale = timbro_iniziale.Replace("NUM_PROTO", "");
                    i = 2;
                }
                if (timbro_iniziale.StartsWith("DATA_COMP"))
                {
                    ordine[3] = i;
                    timbro_iniziale = timbro_iniziale.Replace("DATA_COMP", "");
                    i = 3;
                }
                if (timbro_iniziale.StartsWith("ORA"))
                {
                    ordine[4] = i;
                    timbro_iniziale = timbro_iniziale.Replace("ORA", "");
                    i = 4;
                }
                if (timbro_iniziale.StartsWith("NUM_ALLEG"))
                {
                    ordine[5] = i;
                    timbro_iniziale = timbro_iniziale.Replace("NUM_ALLEG", "");
                    i = 5;
                }
                if (timbro_iniziale.StartsWith("CLASSIFICA"))
                {
                    ordine[6] = i;
                    timbro_iniziale = timbro_iniziale.Replace("CLASSIFICA", "");
                    i = 6;
                }
                if (timbro_iniziale.StartsWith("IN_OUT"))
                {
                    ordine[7] = i;
                    timbro_iniziale = timbro_iniziale.Replace("IN_OUT", "");
                    i = 7;
                }

                // GESTIONE UNITA' ORGANIZZATIVE ED RF ***********************************************
                if (timbro_iniziale.StartsWith("COD_UO_PROT"))
                {
                    ordine[8] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_UO_PROT", "");
                    i = 8;
                }
                if (timbro_iniziale.StartsWith("COD_UO_VIS"))
                {
                    ordine[9] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_UO_VIS", "");
                    i = 9;
                }
                if (timbro_iniziale.StartsWith("COD_RF_PROT"))
                {
                    ordine[10] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_RF_PROT", "");
                    i = 10;
                }
                if (timbro_iniziale.StartsWith("COD_RF_VIS"))
                {
                    ordine[11] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_RF_VIS", "");
                    i = 11;
                }
                // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                //se il timbro iniziale è rimasto invariato rimuovo un carattere ed inizio di nuovo la ricerca
                if (timbro_iniziale == appo)
                {
                    timbro_iniziale = timbro_iniziale.Remove(0, 1);
                }
            }
            return ordine;
        }
        private string GetSeparatore(long idAmm)
        {
            string separatore = _dbContext.AmministraEntities.Where(w => w.SYSTEM_ID == idAmm).Select(s => s.CHA_SEPARATORE).FirstOrDefault();

            if (string.IsNullOrEmpty(separatore))
                separatore = "/";

            return separatore;
        }

        private string ReturnDesc(string timbro_iniziale, string currTimbro, string currVal, string[] dati)
        {
            int[] ordine = new int[12];
            int i = -1;
            while (timbro_iniziale != string.Empty)
            {
                string appo = timbro_iniziale;
                if (timbro_iniziale.StartsWith("COD_AMM"))
                {
                    ordine[0] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_AMM", "");
                    i = 0;
                }
                if (timbro_iniziale.StartsWith("COD_REG"))
                {
                    ordine[1] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_REG", "");
                    i = 1;
                }
                if (timbro_iniziale.StartsWith("NUM_PROTO"))
                {
                    ordine[2] = i;
                    timbro_iniziale = timbro_iniziale.Replace("NUM_PROTO", "");
                    i = 2;
                }
                if (timbro_iniziale.StartsWith("DATA_COMP"))
                {
                    ordine[3] = i;
                    timbro_iniziale = timbro_iniziale.Replace("DATA_COMP", "");
                    i = 3;
                }
                if (timbro_iniziale.StartsWith("ORA"))
                {
                    ordine[4] = i;
                    timbro_iniziale = timbro_iniziale.Replace("ORA", "");
                    i = 4;
                }
                if (timbro_iniziale.StartsWith("NUM_ALLEG"))
                {
                    ordine[5] = i;
                    timbro_iniziale = timbro_iniziale.Replace("NUM_ALLEG", "");
                    i = 5;
                }
                if (timbro_iniziale.StartsWith("CLASSIFICA"))
                {
                    ordine[6] = i;
                    timbro_iniziale = timbro_iniziale.Replace("CLASSIFICA", "");
                    i = 6;
                }
                if (timbro_iniziale.StartsWith("IN_OUT"))
                {
                    ordine[7] = i;
                    timbro_iniziale = timbro_iniziale.Replace("IN_OUT", "");
                    i = 7;
                }

                // GESTIONE UNITA' ORGANIZZATIVE ED RF ***********************************************
                if (timbro_iniziale.StartsWith("COD_UO_PROT"))
                {
                    ordine[8] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_UO_PROT", "");
                    i = 8;
                }
                if (timbro_iniziale.StartsWith("COD_UO_VIS"))
                {
                    ordine[9] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_UO_VIS", "");
                    i = 9;
                }
                if (timbro_iniziale.StartsWith("COD_RF_PROT"))
                {
                    ordine[10] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_RF_PROT", "");
                    i = 10;
                }
                if (timbro_iniziale.StartsWith("COD_RF_VIS"))
                {
                    ordine[11] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_RF_VIS", "");
                    i = 11;
                }
                // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                //se il timbro iniziale è rimasto invariato rimuovo un carattere ed inizio di nuovo la ricerca
                if (timbro_iniziale == appo)
                {
                    timbro_iniziale = timbro_iniziale.Remove(0, 1);
                }
            }

            int count = 0;
            int start = 0;
            if (currVal.Equals("COD_AMM"))
            {
                if (ordine[0] >= 0)
                {
                    start = currTimbro.IndexOf(dati[ordine[0]]) + dati[ordine[0]].Length;
                    count = currTimbro.IndexOf("COD_AMM") - start;
                    currTimbro = currTimbro.Substring(start, count);
                }
                else
                {
                    count = currTimbro.IndexOf("COD_AMM");
                    currTimbro = currTimbro.Substring(0, count);
                }
            }
            if (currVal.Equals("COD_REG"))
            {
                if (ordine[1] >= 0)
                {
                    start = currTimbro.IndexOf(dati[ordine[1]]) + dati[ordine[1]].Length;
                    count = currTimbro.IndexOf("COD_REG") - start;
                    currTimbro = currTimbro.Substring(start, count);
                }
                else
                {
                    count = currTimbro.IndexOf("COD_REG");
                    currTimbro = currTimbro.Substring(0, count);
                }
            }
            if (currVal.Equals("NUM_PROTO"))
            {
                if (ordine[2] >= 0)
                {
                    start = currTimbro.IndexOf(dati[ordine[2]]) + dati[ordine[2]].Length;
                    count = currTimbro.IndexOf("NUM_PROTO") - start;
                    currTimbro = currTimbro.Substring(start, count);
                }
                else
                {
                    count = currTimbro.IndexOf("NUM_PROTO");
                    currTimbro = currTimbro.Substring(0, count);
                }
            }
            if (currVal.Equals("DATA_COMP"))
            {
                if (ordine[3] >= 0)
                {
                    start = currTimbro.IndexOf(dati[ordine[3]]) + dati[ordine[3]].Length;
                    count = currTimbro.IndexOf("DATA_COMP") - start;
                    currTimbro = currTimbro.Substring(start, count);
                }
                else
                {
                    count = currTimbro.IndexOf("DATA_COMP");
                    currTimbro = currTimbro.Substring(0, count);
                }
            }
            if (currVal.Equals("ORA"))
            {
                if (ordine[4] >= 0)
                {
                    start = currTimbro.IndexOf(dati[ordine[4]]) + dati[ordine[4]].Length;
                    count = currTimbro.IndexOf("ORA") - start;
                    currTimbro = currTimbro.Substring(start, count);
                }
                else
                {
                    count = currTimbro.IndexOf("ORA");
                    currTimbro = currTimbro.Substring(0, count);
                }
            }
            if (currVal.Equals("NUM_ALLEG"))
            {
                if (ordine[5] >= 0)
                {
                    start = currTimbro.IndexOf(dati[ordine[5]]) + dati[ordine[5]].Length;
                    count = currTimbro.IndexOf("NUM_ALLEG") - start;
                    currTimbro = currTimbro.Substring(start, count);
                }
                else
                {
                    count = currTimbro.IndexOf("NUM_ALLEG");
                    currTimbro = currTimbro.Substring(0, count);
                }
            }
            if (currVal.Equals("CLASSIFICA"))
            {
                if (ordine[6] >= 0)
                {
                    start = currTimbro.IndexOf(dati[ordine[6]]) + dati[ordine[6]].Length;
                    count = currTimbro.IndexOf("CLASSIFICA") - start;
                    currTimbro = currTimbro.Substring(start, count);
                }
                else
                {
                    count = currTimbro.IndexOf("CLASSIFICA");
                    currTimbro = currTimbro.Substring(0, count);
                }
            }
            if (currVal.Equals("IN_OUT"))
            {
                if (ordine[7] >= 0)
                {
                    start = currTimbro.IndexOf(dati[ordine[7]]) + dati[ordine[7]].Length;
                    count = currTimbro.IndexOf("IN_OUT") - start;
                    currTimbro = currTimbro.Substring(start, count);
                }
                else
                {
                    count = currTimbro.IndexOf("IN_OUT");
                    currTimbro = currTimbro.Substring(0, count);
                }
            }
            return currTimbro;
        }

        protected virtual async void LoadXmlLabelProperties(DocsPaVO.documento.FileDocumento file, string position, Amministrazione Amm)
        {
            string delimitatore = "-";
            try
            {
                //carico info Font
                CaratTimbroEntity carat = new CaratTimbroEntity();
                for (int i = 0; i < Amm.caratTimbro.Count(); i++)
                {
                    carat = Amm.caratTimbro[i];
                    //Se da front-end ho selezionato un tipo di font lo utilizzo altrimenti uso quello
                    //configurato in amministrazione...
                    if (string.IsNullOrEmpty(file.LabelPdf.sel_font))
                    {
                        file.LabelPdf.sel_font = Amm.carattere;
                    }
                    if (carat.SYSTEM_ID == file.LabelPdf.sel_font.AsLong())
                    {
                        file.LabelPdf.font_type = carat.VAR_NOME;
                        file.LabelPdf.font_size = carat.DIMENSIONE;
                    }
                }

                ColoreTimbroEntity colore = new ColoreTimbroEntity();

                for (int j = 0; j < Amm.coloreTimbro.Count(); j++)
                {
                    colore = Amm.coloreTimbro[j];
                    //Se selezionato uso il colore scelto da front-end
                    if (string.IsNullOrEmpty(file.LabelPdf.sel_color))
                    {
                        file.LabelPdf.sel_color = Amm.colore;
                    }
                    //if (colore.id == Amm.Timbro_colore)
                    if (colore.SYSTEM_ID == file.LabelPdf.sel_color.AsLong())
                    {
                        file.LabelPdf.font_color = colore.VAR_NOME;
                    }
                }
                file.LabelPdf.label_rotation = Amm.rotazione;

                #region LoadDefaulPosition
                // carico le 4 posizioni
                string default_pos = string.Empty;
                DocsPaVO.documento.position pos_upSx = new DocsPaVO.documento.position();
                DocsPaVO.documento.position pos_upDx = new DocsPaVO.documento.position();
                DocsPaVO.documento.position pos_downSx = new DocsPaVO.documento.position();
                DocsPaVO.documento.position pos_downDx = new DocsPaVO.documento.position();
                //DocsPaVO.amministrazione.posizione pos = new DocsPaVO.amministrazione.posizione();
                PosizTimbroEntity pos = new PosizTimbroEntity();
                List<position> posizioni = new List<position>();
                //file.LabelPdf.positions = new position[4];
                for (int k = 0; k < Amm.posizioneTimbro.Count(); k++)
                {
                    pos = Amm.posizioneTimbro[k];
                    // posizione Alto Sinistra
                    if (pos.TIPO_POS == "pos_upSx")
                    {
                        pos_upSx.posName = pos.TIPO_POS;
                        pos_upSx.PosX = pos.POS_X;
                        pos_upSx.PosY = pos.POS_Y;
                        posizioni.Add(pos_upSx);
                        //file.LabelPdf.positions.Add(pos_upSx);
                    }
                    // posizione Alto Destra
                    if (pos.TIPO_POS == "pos_upDx")
                    {
                        pos_upDx.posName = pos.TIPO_POS;
                        pos_upDx.PosX = pos.POS_X;
                        pos_upDx.PosY = pos.POS_Y;
                        posizioni.Add(pos_upDx);
                        //file.LabelPdf.positions.Append(pos_upDx);
                    }
                    // posizione basso Sinistra
                    if (pos.TIPO_POS == "pos_downSx")
                    {
                        pos_downSx.posName = pos.TIPO_POS;
                        pos_downSx.PosX = pos.POS_X;
                        pos_downSx.PosY = pos.POS_Y;
                        posizioni.Add(pos_downSx);
                        //file.LabelPdf.positions.Append(pos_downSx);
                    }
                    // posizione basso Destra
                    if (pos.TIPO_POS == "pos_downDx")
                    {
                        pos_downDx.posName = pos.TIPO_POS;
                        pos_downDx.PosX = pos.POS_X;
                        pos_downDx.PosY = pos.POS_Y;
                        posizioni.Add(pos_downDx);
                        //file.LabelPdf.positions.Append(pos_downDx);
                    }
                    // posizione di default
                    if (pos.SYSTEM_ID == Amm.posizione.AsLong())
                    {
                        default_pos = pos.TIPO_POS;
                    }
                }

                #endregion

                if ((position == null) || (position == ""))
                {
                    //prendo la default su XML
                    file.LabelPdf.default_position = default_pos;
                }
                else
                {
                    //verifico le scelte utente
                    string[] posPers = position.Split(Convert.ToChar(delimitatore));
                    // è stata scelta una posizione standard
                    if (posPers.Length == 1)
                    {
                        //forzo la scelta utente con default
                        file.LabelPdf.default_position = position;
                    }
                    else
                    {
                        //prima di passare alle coordinate personalizzate verifico se la x e la y corrispondono ad
                        //una delle coordinate di default...
                        if (posPers[0].ToString() == pos_upSx.PosX)
                        {
                            if (posPers[1].ToString() == pos_upSx.PosY)
                            {
                                file.LabelPdf.default_position = pos_upSx.posName;
                            }
                            else
                            {
                                if (posPers[1].ToString() == pos_downSx.PosY)
                                {
                                    file.LabelPdf.default_position = pos_downSx.posName;
                                }
                                else
                                {
                                    file.LabelPdf.default_position = "pos_pers";
                                    DocsPaVO.documento.position pos_pers = new DocsPaVO.documento.position();
                                    pos_pers.posName = "pos_pers";
                                    pos_pers.PosX = posPers[0].ToString();
                                    pos_pers.PosY = posPers[1].ToString();
                                    posizioni.Add(pos_pers);
                                }
                            }
                        }
                        else
                        {
                            if (posPers[0].ToString() == pos_upDx.PosX)
                            {
                                if (posPers[1].ToString() == pos_upDx.PosY)
                                {
                                    file.LabelPdf.default_position = pos_upDx.posName;
                                }
                                else
                                {
                                    if (posPers[1].ToString() == pos_downDx.PosY)
                                    {
                                        file.LabelPdf.default_position = pos_downDx.posName;
                                    }
                                    else
                                    {
                                        file.LabelPdf.default_position = "pos_pers";
                                        DocsPaVO.documento.position pos_pers = new DocsPaVO.documento.position();
                                        pos_pers.posName = "pos_pers";
                                        pos_pers.PosX = posPers[0].ToString();
                                        pos_pers.PosY = posPers[1].ToString();
                                        posizioni.Add(pos_pers);
                                    }
                                }
                            }
                            else
                            {
                                file.LabelPdf.default_position = "pos_pers";
                                DocsPaVO.documento.position pos_pers = new DocsPaVO.documento.position();
                                pos_pers.posName = "pos_pers";
                                pos_pers.PosX = posPers[0].ToString();
                                pos_pers.PosY = posPers[1].ToString();
                                posizioni.Add(pos_pers);
                            }
                        }

                    }
                }
                file.LabelPdf.positions = posizioni.ToArray();
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        protected virtual string AppendDatiEtichettaAllegato(DocsPaVO.documento.Allegato allegato, string orientamento)
        {
            string tipoAttac = string.Empty;
            switch (allegato.TypeAttachment)
            {
                case 1: //allegato tipo utente
                    tipoAttac = "Utente";
                    break;
                case 2: //allegato tipo PEC
                    tipoAttac = "Pec";
                    break;
                case 3: //allegato tipo IS
                    tipoAttac = "P.I.Tre.";
                    break;
                case 4: //allegato tipo SE
                    tipoAttac = "Sist. Esterni";
                    break;
                default:
                    tipoAttac = string.Empty;
                    break;
            }

            return string.Format("{0}Allegato {1} {2} ({3})", ((orientamento.ToLower() == "verticale") ? "\n" : " - "),
                                                                 tipoAttac, allegato.position, allegato.versionLabel);
        }

        protected virtual async Task<string> GetDatiEtichettaProtocolloRepertorio(InfoProfile infoProfile, string codiceAmministrazione)
        {
            var datiProtRepertorio = string.Empty;

            try
            {
                var objRepertorio = this._dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                    .Join(_dbContext.OggettiCustomEntities.AsNoTracking(), at => at.ID_OGGETTO, oc => oc.SYSTEM_ID, (at, oc) => new { at, oc })
                    .Where(j => j.at.DOC_NUMBER == infoProfile.DOCNUMBER.ToString() && (j.oc.CAMPO_COMUNE ?? 0) == 0 && j.oc.REPERTORIO.ToString() == "1")
                    .Select(j => new
                      InfoRepertorio
                    {
                        anno = j.at.ANNO.ToString(),
                        codiceDB = j.at.CODICE_DB,
                        dataInserimento = j.at.DTA_INS.AsDateTimeFormat(),
                        formatoContatore = j.oc.FORMATO_CONTATORE,
                        idAOORF = j.at.ID_AOO_RF.ToString(),
                        valoreDb = j.at.VALORE_OGGETTO_DB,
                    }).FirstOrDefault();

                if (objRepertorio != null)
                {
                    datiProtRepertorio = $"{await GetFormattedProtocolloDiRepertorio(objRepertorio, codiceAmministrazione)} - {infoProfile.VAR_DESC_ATTO}";
                }
            }
            catch (Exception ex)
            {
                datiProtRepertorio = string.Empty;
            }

            return datiProtRepertorio;
        }

        protected virtual async Task<string> GetDatiEtichettaProtocolloRepertorioTrasversale(InfoProfile infoProfile, string codiceAmministrazione)
        {
            var datiProtRepertorio = string.Empty;

            try
            {
                var objRepertorio = this._dbContext.AssociazioneTemplatesEntities
                    .Join(_dbContext.OggettiCustomEntities.AsNoTracking(), at => at.ID_OGGETTO, oc => oc.SYSTEM_ID, (at, oc) => new { at, oc })
                    .Where(j => j.at.DOC_NUMBER == infoProfile.DOCNUMBER.ToString() && j.oc.CAMPO_COMUNE == 1 && j.oc.REPERTORIO.ToString() == "1")
                    .Select(j => new
                        InfoRepertorio
                    {
                        anno = j.at.ANNO.ToString(),
                        codiceDB = j.at.CODICE_DB,
                        dataInserimento = j.at.DTA_INS.AsDateTimeFormat(),
                        formatoContatore = j.oc.FORMATO_CONTATORE,
                        idAOORF = j.at.ID_AOO_RF.ToString(),
                        valoreDb = j.at.VALORE_OGGETTO_DB,
                    }).FirstOrDefault();

                if (objRepertorio != null)
                {
                    datiProtRepertorio = $"{await GetFormattedProtocolloDiRepertorio(objRepertorio, codiceAmministrazione)} - {infoProfile.VAR_DESC_ATTO}";
                }
            }
            catch (Exception ex)
            {
                datiProtRepertorio = string.Empty;
            }

            return datiProtRepertorio;
        }

        protected virtual async Task<string> GetFormattedProtocolloDiRepertorio(InfoRepertorio objAtt, string codiceAmministrazione)
        {
            string ProtocolloFormattedResult = string.Empty;
            if (objAtt.valoreDb != null && objAtt.valoreDb != "")
            {
                ProtocolloFormattedResult = objAtt.formatoContatore;
                ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("ANNO", objAtt.anno.ToString());
                ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("CONTATORE", objAtt.valoreDb);
                ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("COD_AMM", codiceAmministrazione);
                ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("COD_UO", objAtt.codiceDB);
                if (objAtt.dataInserimento != null)
                {
                    int fine = objAtt.dataInserimento.LastIndexOf(".");
                    if (fine == -1) fine = objAtt.dataInserimento.LastIndexOf(":");
                    ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("gg/mm/aaaa hh:mm", objAtt.dataInserimento.Substring(0, fine));
                    ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("gg/mm/aaaa", objAtt.dataInserimento.Substring(0, 10));
                }

                if (!String.IsNullOrEmpty(objAtt.idAOORF) && objAtt.idAOORF != "0")
                {
                    RegistroEntity reg = await this._dbContext.RegistroEntities.AsNoTracking()
                        .Where(r => r.SYSTEM_ID == objAtt.idAOORF.AsLong())
                        .FirstOrDefaultAsync();

                    if (reg != null)
                    {
                        if (!string.IsNullOrEmpty(reg.CHA_RF) && reg.CHA_RF == "1")
                        {
                            ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("RF", reg.VAR_CODICE);

                            if (reg.ID_AOO_COLLEGATA != null)
                            {
                                RegistroEntity registro = await this._dbContext.RegistroEntities.AsNoTracking()
                                    .Where(r => r.SYSTEM_ID == reg.ID_AOO_COLLEGATA)
                                    .FirstOrDefaultAsync();

                                if (registro != null)
                                    ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("AOO", registro.VAR_CODICE);
                            }
                        }
                        else //se contatore di AOO non ho i dati per ricavare RF perchè non mi viene passato in input. 
                        {
                            ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("RF", reg.VAR_CODICE);
                            ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("AOO", reg.VAR_CODICE);
                        }
                    }
                }
            }
            // codice protocollo di repertorio
            return string.Format("{0}", ProtocolloFormattedResult);
        }

        protected virtual async Task<string> GetDatiFirmaElettronica(string idDocumento, string versionId, labelPdf lblPdf, DocsPaVO.utente.InfoUtente infoUtente)
        {
            int maxCharsPerRow = 0;
            if (lblPdf.position.ToUpper().Contains("DX"))
                maxCharsPerRow = 30; // numero di caratteri massimo per riga per allineamento a destra
            else
                maxCharsPerRow = 90; // numero di caratteri massimo per riga per allineamento a sinistra

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            bool isFirstSigner = true;
            List<DocsPaVO.LibroFirma.FirmaElettronica> listSign = (await this._mediator.Send(new Application.Requests.GetElectronicSignatureDocument(idDocumento, versionId, infoUtente))).output;
            if (listSign != null && listSign.Count() > 0)
            {
                foreach (DocsPaVO.LibroFirma.FirmaElettronica sign in listSign)
                {
                    if (isFirstSigner)
                    {
                        sb.AppendFormat("{0}", FormatLabel(Resources.DocumentoFirmatoElettronicamenteDa, maxCharsPerRow));
                        sb.AppendLine();
                        sb.AppendFormat("{0}", FormatLabel(sign.Firmatario, maxCharsPerRow));
                        sb.AppendLine();
                        sb.AppendFormat("il {0}", FormatLabel(sign.DataApposizione, maxCharsPerRow));
                        isFirstSigner = false;
                    }
                    else
                    {
                        sb.AppendLine();
                        sb.AppendFormat("{0}", FormatLabel(sign.Firmatario, maxCharsPerRow));
                        sb.AppendLine();
                        sb.AppendFormat("il {0}", FormatLabel(sign.DataApposizione, maxCharsPerRow));
                    }
                }
            }
            return sb.ToString();
        }

        protected virtual async Task<string> GetDatiFirmaPerEtichetta(VerificaResponse verificaResponse,
                                              string dettaglioFirma, DocsPaVO.documento.labelPdf lblPdf,
                                              string idAmm)
        {
            //PEC Firma - Posizionamento dei dati firma per allineamento a destra/sinistra
            int maxCharsPerRow = 0;
            if (lblPdf.position.ToUpper().Contains("DX"))
                maxCharsPerRow = 30; // numero di caratteri massimo per riga per allineamento a destra
            else
                maxCharsPerRow = 90; // numero di caratteri massimo per riga per allineamento a sinistra

            // Apposizione dati della firma digitale
            List<DatiFirmatari> datiFirmatari = null;
            if(verificaResponse != null)
                datiFirmatari = verificaResponse.Esito != null ? verificaResponse.Esito.DatiFirmatari : verificaResponse.Warning.DettaglioFirmaDigitale.DatiFirmatari;

            if (datiFirmatari != null)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                if (lblPdf.digitalSignInfo.printFormatSign == DocsPaVO.documento.labelPdfDigitalSignInfo.TypePrintFormatSign.Sign_Extended)
                {
                    string keyDettagliFirma = await this._configurationService.GetValue<string>(idAmm, "FE_DETTAGLI_FIRMA");
                    if (keyDettagliFirma == "1")
                        if (!string.IsNullOrEmpty(dettaglioFirma))
                        {
                            sb.AppendLine();
                            sb.AppendFormat("{0}", FormatLabel(dettaglioFirma, maxCharsPerRow));
                        }
                }

                bool isFirstSigner = true;
                foreach (DatiFirmatari datiFirmatario in datiFirmatari)
                {
                    if (lblPdf.digitalSignInfo.printFormatSign == DocsPaVO.documento.labelPdfDigitalSignInfo.TypePrintFormatSign.Sign_Short)
                    {
                        if (isFirstSigner)
                        {
                            sb.AppendLine();
                            sb.AppendFormat(Resources.DocumentoFirmatoDigitalmenteDa, datiFirmatario.Firmatario.CommonName);
                            isFirstSigner = false;
                        }
                        else sb.AppendFormat(", {0}", datiFirmatario.Firmatario.CommonName);

                        //MEV CONTRO-FIRMATARI:   aggiunta info sintetica controfirmatari
                        //if (signer.counterSignatures != null && signer.counterSignatures.Count() > 0)
                        //    sb.Append(GetDatiControFirmatari(signer));
                    }
                    else
                    {
                        sb.AppendLine(GetDatiFirmaPerEtichettaInfoFirmaCompleta(datiFirmatario.Firmatario, maxCharsPerRow));

                        //MEV CONTRO-FIRMATARI: aggiunta info completa controfirmatari
                        //if (signer.counterSignatures != null && signer.counterSignatures.Count() > 0)
                        //{
                        //    sb.AppendLine(GetDatiControFirmatariCompleta(signer, maxCharsPerRow));
                        //}
                    }

                    // gestione del ritorno a capo con allineamento SX/DX relativo al dettaglio sintetico della firma
                    if ((!string.IsNullOrEmpty(sb.ToString())) && (lblPdf.digitalSignInfo.printFormatSign == DocsPaVO.documento.labelPdfDigitalSignInfo.TypePrintFormatSign.Sign_Short))
                        sb.Replace(sb.ToString(), FormatLabel(sb.ToString(), maxCharsPerRow));
                }

                return sb.ToString();
            }
            else
                return string.Empty;
        }

        /// <summary>
        /// Formatta le informazioni relative alla firma completa
        /// </summary>
        protected string GetDatiFirmaPerEtichettaInfoFirmaCompleta(Core.Services.File.FirmaDigitale2.Firmatario firmatario, int maxCharsPerRow)
        {
            System.Text.StringBuilder sbTextInfoFirma = new StringBuilder();
            sbTextInfoFirma.AppendLine();
            sbTextInfoFirma.AppendFormat(FormatLabel(string.Format("ENTE CERTIFICATORE: {0}", firmatario.CnCertAuthority), maxCharsPerRow));
            sbTextInfoFirma.AppendLine();
            sbTextInfoFirma.AppendFormat(FormatLabel(string.Format("SN CERTIFICATO: {0}", firmatario.SerialNumber), maxCharsPerRow));
            sbTextInfoFirma.AppendLine();
            sbTextInfoFirma.AppendFormat(FormatLabel(string.Format("VALIDO DA: {0}", firmatario.DataInizioValiditaCert.AsDateTimeFormat()), maxCharsPerRow));
            sbTextInfoFirma.AppendLine();
            sbTextInfoFirma.AppendFormat(FormatLabel(string.Format("VALIDO AL: {0}", firmatario.DataFineValiditaCert.AsDateTimeFormat()), maxCharsPerRow));
            sbTextInfoFirma.AppendLine();
            sbTextInfoFirma.AppendFormat(FormatLabel(string.Format("FIRMATARI: {0}", firmatario.CommonName), maxCharsPerRow));

            return sbTextInfoFirma.ToString();
        }

        /// <summary>
        /// Formatta i dati relativi all'ente certificatore
        /// </summary>
        /// <param name="issuesName">IssuesName con formato generico CN={0},CN={1}, OU={2}, O={3}, C={4}</param>
        /// <returns>Ente certificatore contenete le info di CN, O, C  </returns>
        protected string GetEnteCertificatore(string issuerName)
        {
            string enteCertCN = string.Empty;
            string enteCertO = string.Empty;
            string enteCertC = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(issuerName)) return string.Empty; //no issuesName
                string[] issuerNamePars = issuerName.Split(',');
                // recupera CN
                if (issuerName.Contains("CN="))
                    enteCertCN = issuerNamePars.Where(a => a.Contains("CN=")).SingleOrDefault().Split('=')[1];
                // recupera O
                if (issuerName.Contains("O="))
                    enteCertO = string.Format("{0}{1}", (string.IsNullOrEmpty(enteCertCN) ? string.Empty : ", "), issuerNamePars.Where(a => a.Contains("O=")).SingleOrDefault().Split('=')[1]);
                if (issuerName.Contains("C="))
                    enteCertC = string.Format(", {0}", issuerNamePars.Where(a => a.Contains("C=")).SingleOrDefault().Split('=')[1]);
            }
            catch (Exception e)
            {
                this._logger.LogCritical("Errore in getEnteCertificatore: " + e.Message);
            }
            return string.Format("{0}{1}{2}", enteCertCN, enteCertO, enteCertC);
        }

        /// <summary>
        /// Formattazione label su documenti in formato A4 (ritorno a capo) 
        /// </summary>
        /// <param name="text">testo da formattare</param>
        /// <param name="maxCharsPerRow">numero massimo di caratteri per riga</param>
        /// <returns></returns>
        protected string FormatLabel(string text, int maxCharsPerRow)
        {
            string rowWord = string.Empty;
            string prevRowWord = string.Empty;
            string formattedText = string.Empty;
            if (text.Length <= maxCharsPerRow) return text;
            string[] words = text.Split(' ');
            foreach (string word in words)
            {
                prevRowWord = rowWord;
                rowWord += (string.IsNullOrEmpty(prevRowWord)) ? word : string.Format(" {0}", word);
                if (rowWord.Length > maxCharsPerRow)
                {
                    formattedText += prevRowWord + "\r\n";
                    rowWord = word;
                }
            }
            return formattedText + rowWord;
        }

        protected virtual async Task<List<Applicazione>> GetApplications(string ext)
        {
            this._logger.LogDebug($"DocumentoGetFileConSegnatura > GetApplications > ext: {ext}");
            List<Applicazione> output = new List<Applicazione>();

            var appsEntity = await this._dbContext.AppEntities.AsNoTracking()
                .Where(a => a.DEFAULT_EXTENSION.ToUpper() == ext.ToUpper())
                .Select(a => new
                {
                    a.DEFAULT_EXTENSION,
                    a.MIME_TYPE
                })
                .ToListAsync();

            appsEntity.ForEach(a =>
             output.Add(new Applicazione
             {
                 estensione = a.DEFAULT_EXTENSION,
                 mimeType = a.MIME_TYPE
             })
            );

            if (output.Count == 0)
            {
                this._logger.LogDebug($"DocumentoGetFileConSegnatura > GetApplications > inserisco ext: {ext}");
                var appEntity = new AppEntity
                {
                    APPLICATION = "GEN_" + ext,
                    DESCRIPTION = "GEN_" + ext,
                    FILING_SCHEME = 2,
                    DEFAULT_EXTENSION = ext
                };

                await this._dbContext.AppEntities.AddAsync(appEntity);

                await ((DbContext)_dbContext).SaveChangesAsync();

                output.Add(new Applicazione
                {
                    estensione = appEntity.DEFAULT_EXTENSION,
                    mimeType = appEntity.MIME_TYPE
                });
            }

            return output;
        }


        protected class Amministrazione
        {
            public long id { get; set; }
            public string codice { get; set; }
            public string carattere { get; set; }
            public string colore { get; set; }
            public string orientamento { get; set; }
            public string posizione { get; set; }
            public string rotazione { get; set; }
            public string timbroPDF { get; set; }

            public List<CaratTimbroEntity> caratTimbro { get; set; }

            public List<ColoreTimbroEntity> coloreTimbro { get; set; }

            public List<PosizTimbroEntity> posizioneTimbro { get; set; }
        }

        protected class InfoProfile
        {
            public long? DOCNUMBER { get; set; }
            public long? NUM_PROTO { get; set; }

            public string? VAR_SEGNATURA { get; set; }

            public long? ID_TIPO_ATTO { get; set; }

            public string? VAR_DESC_ATTO { get; set; }


        }

        protected class AddEtichettaResult
        {
            public FileDocumento infoDocumento { get; set; }
            public List<KeyValuePair<int, string>>? infoPositionAndText { get; set; }
        }
        protected class InfoRepertorio
        {
            public string valoreDb { get; set; }
            public string anno { get; set; }
            public string codiceDB { get; set; }

            public string dataInserimento { get; set; }

            public string idAOORF { get; set; }
            public string formatoContatore { get; set; }
        }

        private class ResultGetFile
        {
            public bool result { get; set; }
            public FileDocumento fileDocument { get; set; }

        }

        #endregion

    }
}
