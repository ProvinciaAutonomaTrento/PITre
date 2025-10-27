// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.LibroFirma;
using DocsPaVO.utente;
using MediatR;
using Org.BouncyCastle.Asn1.Ocsp;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentoGetFileConSegnatura;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Pi3.Core.Services.File.SigilloElettronico;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.RemotePdfSignStamp
{
    public class RemotePdfSignStampCommandHandler : IRequestHandler<RemotePdfSignStampCommand, RemotePdfSignStampCommandResponse>
    {
        #region Public Members

        public RemotePdfSignStampCommandHandler(ILogger<RemotePdfSignStampCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IConfigurationService configurationService,
            ISigilloElettronicoService sigilloElettronicoService,
            IDocumentBlobRepository documentBlobRepository,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._configurationService = configurationService;
            this._sigilloElettronicoService = sigilloElettronicoService;
            this._documentBlobRepository = documentBlobRepository;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
        }

        public async Task<RemotePdfSignStampCommandResponse> Handle(RemotePdfSignStampCommand request, CancellationToken cancellationToken)
        {
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
            var result = ResultSigilloElettronico.OK;

            var output = request.schedaDoc;
            var idDocumentoAsLong = request.schedaDoc.docNumber.AsLong();
            var labelPdf = request.labelPdf;
            var codiceAmmIPA = string.Empty;
            var codiceAOOIPA = string.Empty;
            var stampText = string.Empty;

            try
            {
                var componentsEntity = await this._dbContext.ComponentEntities
                           .Where(c => c.DOCNUMBER == idDocumentoAsLong)
                           .OrderByDescending(c => c.VERSION_ID)
                           .FirstAsync();

                if (componentsEntity.FILE_SIZE == 0)
                    result = ResultSigilloElettronico.FILE_NON_ACQUISITO;

                if (componentsEntity.EXT != null && componentsEntity.EXT.ToUpper().Equals(".PDF"))
                    result = ResultSigilloElettronico.FORMATO_FILE_NON_VALIDO;

                var fileContent = await GetFileDocument(idTenant, componentsEntity.PATH);
                var nomeOriginale = componentsEntity.VAR_NOMEORIGINALE;
                var name = request.fr.fileName;

                if ((await this._configurationService.GetValue<string>(idTenant, "FE_SEGNATURA_PERMANENTE")) != "1")
                    throw new RemotePdfSignDisabledPi3Exception();

                var amministraEntity = await _dbContext.AmministraEntities.AsNoTracking().FirstAsync(a => a.SYSTEM_ID == idTenant.AsLong());
                codiceAmmIPA = amministraEntity.VAR_CODICE_AMM_IPA;

                var documentoAmministrativoAggregate = await this._documentoAmministrativoRepository.Get(idTenant, idDocumentoAsLong.ToString(), new ILoadBehavior[1]
                   {
                        new GetDocumentoAmministrativoLoadBehavior()
                        {
                            LoadProfiles = true,
                            LoadClassifications = false,
                            LoadAllegati = false,
                            LoadAggregazioni = false,
                            LoadVersions = true,
                            LoadPermissions = false,
                            LoadMittentiDestinatari = true,
                            LoadKeywords = false,
                            LoadNote = false
                        }
                   });

                var idFileRequestOld = documentoAmministrativoAggregate.Versions[documentoAmministrativoAggregate.Versions.Count - 1].Id;

                var idRegistro = output.registro.systemId.AsLong();
                if (!string.IsNullOrEmpty(documentoAmministrativoAggregate.IdDoc.Segnatura))
                {
                    stampText = documentoAmministrativoAggregate.IdDoc.Segnatura;
                }
                else
                {
                    //caso di documento repertoriato e non protocollato
                    if (documentoAmministrativoAggregate.Profiles != null && documentoAmministrativoAggregate.Profiles.Count > 0)
                    {
                        var oggettoCustomEntity = await _dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                            .Join(_dbContext.OggettiCustomEntities.AsNoTracking(),
                                ass => ass.ID_OGGETTO,
                                oggetto => oggetto.SYSTEM_ID,
                                (ass, oggetto) => new { ass, oggetto })
                            .Join(_dbContext.TipoOggettoEntities.AsNoTracking(),
                                j => j.oggetto.ID_TIPO_OGGETTO,
                                tipo => tipo.SYSTEM_ID,
                                (j, tipo) => new { j.ass, j.oggetto, tipo })
                            .Join(_dbContext.TipoAttoEntities.AsNoTracking(),
                                j => j.ass.ID_TEMPLATE,
                        atto => atto.SYSTEM_ID,
                                (j, atto) => new { j.ass, j.oggetto, j.tipo, atto })
                            .Where(j => j.ass.DOC_NUMBER == idDocumentoAsLong.ToString()
                                && j.oggetto.REPERTORIO == 1
                                && j.tipo.DESCRIZIONE.Equals("Contatore"))
                            .Select(j => new
                            {
                                j.ass.VAR_SEGNATURA,
                                j.ass.ID_AOO_RF,
                                j.oggetto,
                                VAR_DESCRIZIONE_TEMPLATE = j.atto.VAR_DESC_ATTO
                            })
                            .FirstOrDefaultAsync();

                        if (oggettoCustomEntity != null && !string.IsNullOrEmpty(oggettoCustomEntity.VAR_SEGNATURA))
                        {
                            idRegistro = oggettoCustomEntity.oggetto.CHA_TIPO_TAR == "R" ? await _dbContext.RegistroEntities.AsNoTracking()
                                        .Where(r => r.SYSTEM_ID == oggettoCustomEntity.ID_AOO_RF)
                                        .Select(r => r.ID_AOO_COLLEGATA.Value)
                                        .FirstAsync() : oggettoCustomEntity.ID_AOO_RF.Value;

                            stampText = $"{oggettoCustomEntity.VAR_SEGNATURA} - {oggettoCustomEntity.VAR_DESCRIZIONE_TEMPLATE}";
                        }
                    }
                }

                codiceAOOIPA = await _dbContext.RegistroEntities.AsNoTracking()
                    .Where(r => r.SYSTEM_ID == idRegistro)
                    .Select(r => r.VAR_CODICE_IPA)
                    .FirstOrDefaultAsync();

                int leftX = 0; int leftY = 0; int rightX = 0; int rightY = 0;

                double a = Convert.ToDouble(labelPdf.pdfHeight);
                double b = Convert.ToDouble(labelPdf.pdfWidth);

                int h = Convert.ToInt32(a);
                int w = Convert.ToInt32(b);
                int areaH = Convert.ToInt32(labelPdf.font_size);  //parametriziamo in base all' alteza del font?
                int areaW = w;
                switch (labelPdf.default_position)
                {
                    case "pos_upSx":
                        leftX = int.Parse((labelPdf.positions[0] as position).PosX); // in basso a sinistra
                        leftY = h - int.Parse((labelPdf.positions[0] as position).PosY); // in basso a sinistra zero parte dal basso del pdf
                        rightX = leftX + areaW;  //in alto a destra
                        rightY = leftY - areaH;
                        break;
                    case "pos_upDx":
                        leftX = int.Parse((labelPdf.positions[1] as position).PosX);
                        leftY = h - int.Parse((labelPdf.positions[1] as position).PosY); // in basso a sinistra zero parte dal basso del pdf
                        rightX = leftX + areaW; //dan verificare w semrba troppo piccolo w -5;
                        rightY = leftY - areaH;
                        break;
                    case "pos_downSx":
                        leftX = int.Parse((labelPdf.positions[2] as position).PosX);
                        leftY = h - int.Parse((labelPdf.positions[2] as position).PosY);
                        rightX = leftX + areaW;
                        rightY = leftY - areaH;

                        break;
                    case "pos_downDx":
                        leftX = int.Parse((labelPdf.positions[3] as position).PosX);
                        leftY = h - int.Parse((labelPdf.positions[3] as position).PosY);
                        rightX = leftX + areaW; //dan verificare w  w - 5;
                        rightY = leftY - areaH;
                        break;
                    default:
                        if ((from position x in labelPdf.positions where x.posName == "pos_pers" select x).FirstOrDefault() != null)
                        {
                            leftX = (from position x in labelPdf.positions where x.posName == "pos_pers" select int.Parse(x.PosX)).FirstOrDefault(); // in basso a sinistra
                            leftY = h - (from position x in labelPdf.positions where x.posName == "pos_pers" select int.Parse(x.PosY)).FirstOrDefault();// in basso a sinistra zero parte dal basso del pdf
                        }
                        else
                        {
                            leftX = Convert.ToInt32(labelPdf.default_position.Split('-')[0]); // in basso a sinistra
                            leftY = h - Convert.ToInt32(labelPdf.default_position.Split('-')[1]); // in basso a sinistra zero parte dal basso del pdf
                        }
                        rightX = leftX + areaW;  //in alto a destra
                        rightY = leftY - areaH;
                        break;
                }

                var responseSigilloElettronicoService = await _sigilloElettronicoService.SignPdf(new SignPDFType()
                {
                    FileDaFirmare = fileContent,
                    CodiceAOOIPA = codiceAOOIPA,
                    CodiceEnteIPA = codiceAmmIPA,
                    Apparence = new ApparenceType()
                    {
                        LeftX = leftX,
                        LeftY = leftY,
                        Page = 1,
                        RightX = rightX,
                        RightY = rightY,
                        Testo = stampText,
                        ScaleFont = false,
                        ShowDateTime = false
                    }
                });

                var newDocumentBlobAggregate = new DocumentBlob(idTenant, DateTime.Now, new TextValue(name));
                newDocumentBlobAggregate.UploadStream(new MemoryStream(responseSigilloElettronicoService.FileFirmato), nomeOriginale);
                newDocumentBlobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                await _documentBlobRepository.Add(newDocumentBlobAggregate);

                documentoAmministrativoAggregate.AssignDocumentBlobRef(
                    new DocumentBlobRef()
                    {
                        IdBlob = newDocumentBlobAggregate.Id,
                        CreationDate = await _dbContext.GetSystemDateTime(),
                        ContentType = newDocumentBlobAggregate.ContentType,
                        FileName = newDocumentBlobAggregate.FileName,
                        FileSize = newDocumentBlobAggregate.FileSize,
                        Hash = newDocumentBlobAggregate.Hash,
                        HashName = newDocumentBlobAggregate.HashName == Pi3.Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256
                            ? HashNamesEnum.SHA256 : HashNamesEnum.SHA512,
                        Cartaceo = false,
                        SegnaturaPermanente = true,
                        TipoFirma = TipoFirmaEnum.Pades
                    },
                    new TargetVersionBehavior()
                    {
                        CreateNewVersion = true,
                        Name = null
                    });

                await _documentoAmministrativoRepository.Update(documentoAmministrativoAggregate);

                //Aggiorno la versione della scheda documento
                if (output.documenti != null && output.documenti.Count() > 0)
                {
                    List<Documento> listNewDocument = new List<Documento>();

                    request.fr.conSegnaturaPermanente = true;
                    request.fr.firmato = "1";
                    request.fr.versionId = documentoAmministrativoAggregate.Versions[documentoAmministrativoAggregate.Versions.Count - 1].Id;
                    request.fr.version = documentoAmministrativoAggregate.Versions[documentoAmministrativoAggregate.Versions.Count - 1].VersionNumber.ToString();

                    listNewDocument.Add(request.fr as Documento);
                    listNewDocument.AddRange((output.documenti.Cast<Documento>()).ToList());
                    output.documenti = listNewDocument.ToArray();
                }

                List<DocsPaVO.LibroFirma.FirmaElettronica> firmaElettronica = await this.GetFirmaElettronicaDaFileRequest(idFileRequestOld.AsLong());
                if (firmaElettronica.Any())
                {
                    long versionId = request.fr.versionId.AsLong();
                    long docNumber = request.fr.docNumber.AsLong();
                    string impronta = await this._dbContext.ComponentEntities
                        .Where(x => x.VERSION_ID == versionId && x.DOCNUMBER == docNumber)
                        .Select(x => x.VAR_IMPRONTA)
                        .FirstOrDefaultAsync() ?? string.Empty;

                    foreach (DocsPaVO.LibroFirma.FirmaElettronica firma in firmaElettronica)
                    {
                        firma.UpdateXml(impronta, request.fr.versionId, request.fr.version);
                        await this.InserisciFirmaElettronica(firma);
                    }
                }
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                result = ResultSigilloElettronico.SYSTEM_ERROR;
            }
            return new RemotePdfSignStampCommandResponse()
            {
                output = output,
                result = result
            };
        }

        #endregion

        #region Private Members

        protected readonly ILogger<RemotePdfSignStampCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IConfigurationService _configurationService;
        protected readonly ISigilloElettronicoService _sigilloElettronicoService;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;

        protected async Task InserisciFirmaElettronica(FirmaElettronica firma)
        {
            var newFirmaElettronicaEntity = new FirmaElettronicaEntity()
            {
                ID_DOCUMENTO = firma.Docnumber.AsLong(),
                VERSION_ID = firma.Versionid.AsLong(),
                DOC_ALL = firma.DocAll,
                NUM_ALL = firma.NumAll.AsLong(),
                NUMERO_VERSIONE = firma.NumVersione.AsLong(),
                DATA_APPOSIZIONE = DateTime.Now,
                XML = firma.Xml
            };

            await this._dbContext.FirmaElettronicaEntities.AddAsync(newFirmaElettronicaEntity);

            await ((Pi3DbContext)_dbContext).SaveChangesAsync();
        }
        protected async Task<List<FirmaElettronica>> GetFirmaElettronicaDaFileRequest(long versionId)
        {
            List<FirmaElettronica> returnList = new List<FirmaElettronica>();
            var firmaEntities = await this._dbContext.FirmaElettronicaEntities
                .Where(x => x.VERSION_ID == versionId && !string.IsNullOrEmpty(x.XML))
                .Select(x => new
                {
                    ID_FIRMA = x.ID_FIRMA,
                    ID_DOCUMENTO = x.ID_DOCUMENTO,
                    VERSION_ID = x.VERSION_ID,
                    DOC_ALL = x.DOC_ALL,
                    NUM_ALL = x.NUM_ALL,
                    NUMERO_VERSIONE = x.NUMERO_VERSIONE,
                    XML = x.XML,
                    DATA_APPOSIZIONE = x.DATA_APPOSIZIONE
                })
                .OrderBy(x => x.ID_FIRMA)
                .ToListAsync();

            foreach (var fe in firmaEntities)
            {
                returnList.Add(new FirmaElettronica()
                {
                    IdFirma = fe.ID_FIRMA.ToString(),
                    Docnumber = fe.ID_DOCUMENTO.ToString(),
                    Versionid = fe.VERSION_ID.ToString(),
                    DocAll = fe.DOC_ALL,
                    NumAll = fe.NUM_ALL.ToString() ?? string.Empty,
                    NumVersione = fe.NUMERO_VERSIONE.ToString(),
                    Xml = fe.XML ?? string.Empty,
                    DataApposizione = fe.DATA_APPOSIZIONE.ToString()
                });
            }

            return returnList;
        }

        protected async Task<byte[]> GetFileDocument(string idTenant, string path)
        {
            byte[] fileContent = null;

            DocumentBlob blob = await this._documentBlobRepository.Get(idTenant, path);
            using (var memoryStream = new MemoryStream())
            {
                blob.Stream.CopyTo(memoryStream);
                fileContent = memoryStream.ToArray();
            }

            return fileContent;
        }
        #endregion
    }

}
