// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using DocsPaVO.FriendApplication;
using DocsPaVO.PrjDocImport;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2013.Word;
using DocumentFormat.OpenXml.Office2016.Excel;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PopolaUnitaOrganizzativaRequest = Pi3.App.Legacy.WebApi.Application.Requests.PopolaUnitaOrganizzativa;
using Pi3.Core.Services.File.PAdES;
using Pi3.Core.Services.File.CAdES;
using System.Xml;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Pi3.Core.Services.File.FileValidator;
using Pi3.Infrastructure.Legacy.EF.Services.FileValidator;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.PopolaUnitaOrganizzativa
{
    public class PopolaUnitaOrganizzativaHandler : IRequestHandler<PopolaUnitaOrganizzativaRequest, PopolaUnitaOrganizzativaResult>
    {
        #region Public Members

        public PopolaUnitaOrganizzativaHandler(ILogger<PopolaUnitaOrganizzativaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            ISpreadsheetService spreadsheetService,
            IPi3DbContext dbContext,
            IConfigurationService configurationService,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            IWebMethodLoggerService webMethodLoggerService,
            IDocumentBlobRepository documentBlobRepository,
            IPAdESService pAdESService,
            IFileValidatorService fileValidatorService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._spreadsheetService = spreadsheetService;
            this._dbContext = dbContext;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
            this._configurationService = configurationService;
            this._webMethodLoggerService = webMethodLoggerService;
            this._documentBlobRepository = documentBlobRepository;
            this._pAdESService = pAdESService;
            this._fileValidatorService = fileValidatorService;
        }

        public async Task<PopolaUnitaOrganizzativaResult> Handle(PopolaUnitaOrganizzativaRequest request, CancellationToken cancellationToken)
        {
            var output = true;
            Dictionary<string, string> docPrinc = new Dictionary<string, string>();

            try
            {
                var repositoryRootPath = await this._configurationService.GetValue<string>("REPOSITORY_ROOT_PATH");
                var codiceUO = await _dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == request.idUO.AsLong()).Select(c => c.VAR_COD_RUBRICA).FirstOrDefaultAsync();
                var codiceAmm = await _dbContext.AmministraEntities.AsNoTracking().Where(a => a.SYSTEM_ID == request.infoUtente.idAmministrazione.AsLong()).Select(a => a.VAR_CODICE_AMM).FirstOrDefaultAsync();

                var pathImportDocumentiUO = Path.Combine(
                            repositoryRootPath,
                            "Formazione",
                            codiceAmm.ToUpper(),
                            codiceUO,
                            "importDocumentiUO.xlsx")
                .PathAsUnixPath();

                byte[] content = File.ReadAllBytes(pathImportDocumentiUO);

                var spreadsheetModel = await _spreadsheetService.Read(new MemoryStream(content));

                //Creazione documenti Non protocollati
                var sheetModelNonProtocollati = spreadsheetModel.Sheets.FirstOrDefault(s => s.Name.Equals("NON PROTOCOLLATI", StringComparison.InvariantCultureIgnoreCase));
                List<DocumentRowData> documentRowDataNonProtocollati = await ReadRowDataNonProtocollati(sheetModelNonProtocollati);

                foreach(var documentRowData in documentRowDataNonProtocollati)
                {
                    var docnumber = await CreaDocumentoAmministrativo(documentRowData, codiceUO, codiceAmm, false);
                    docPrinc.Add(documentRowData.Ordinale, docnumber);
                }

                //Creazione allegati
                var sheetModelAllegati = spreadsheetModel.Sheets.FirstOrDefault(s => s.Name.Equals("ALLEGATI", StringComparison.InvariantCultureIgnoreCase));
                List<DocumentRowData> documentRowDatAllegati = await ReadRowDataAllegati(sheetModelAllegati);
                foreach (var documentRowData in documentRowDatAllegati)
                {
                    if (docPrinc.ContainsKey(documentRowData.OrdinalePrincipale))
                    {
                        await CreaDocumentoAmministrativo(documentRowData, codiceUO, codiceAmm, true, docPrinc[documentRowData.OrdinalePrincipale]);
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
            }

            return new PopolaUnitaOrganizzativaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<PopolaUnitaOrganizzativaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly ISpreadsheetService _spreadsheetService;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IConfigurationService _configurationService;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly IPAdESService _pAdESService;
        protected readonly IFileValidatorService _fileValidatorService;

        protected virtual async Task<List<DocumentRowData>> ReadRowDataNonProtocollati(SheetModel sheetModel)
        {
            List<DocumentRowData> listDocumentRowData = new List<DocumentRowData>();
            DocumentRowData documentRowData = null;

            var columns = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            sheetModel.Cells.Where(c => c.Row == 0).ForEach(c => columns.Add(c.ValueAsString, c.Column));

            var totalRows = sheetModel.Cells.Max(c => c.Row);
            for (int rowIndex = 1; rowIndex <= totalRows; rowIndex++)
            {
                var row = sheetModel.Cells.Where(c => c.Row == rowIndex).ToList();
                var ordinale = row.GetCellValueAsString(columns[Resources.ColumnOrdinale]);
                if (string.IsNullOrEmpty(ordinale))
                    break;

                documentRowData = new DocumentRowData()
                {
                    Ordinale = row.GetCellValueAsString(columns[Resources.ColumnOrdinale]),
                    Oggetto = row.GetCellValueAsString(columns[Resources.ColumnOggetto]),
                    CodiceUtenteCreatore = row.GetCellValueAsString(columns[Resources.ColumnCodiceUtenteCreatore]),
                    CodiceRuoloCreatore = row.GetCellValueAsString(columns[Resources.ColumnCodiceRuoloCreatore]),
                    CodiceAmm = row.GetCellValueAsString(columns[Resources.ColumnCodiceAmministrazione]),
                    TipoDocumento = row.GetCellValueAsString(columns[Resources.ColumnTipo]),
                    CodiceFascicolo = row.GetCellValueAsString(columns[Resources.ColumnCodiceFascicolo]).Replace(",", "."),
                    CodiceRegistro = row.GetCellValueAsString(columns[Resources.ColumnCodiceRegistro]),
                    NomeFile = row.GetCellValueAsString(columns[Resources.ColumnPathname]),
                    CodiceUtenteAreaLavoro = row.GetCellValueAsString(columns[Resources.ColumnCodiceUtenteADL])
                };

                listDocumentRowData.Add(documentRowData);
            }

            return listDocumentRowData;
        }

        protected virtual async Task<List<DocumentRowData>> ReadRowDataAllegati(SheetModel sheetModel)
        {
            List<DocumentRowData> listDocumentRowData = new List<DocumentRowData>();
            DocumentRowData documentRowData = null;

            var columns = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            sheetModel.Cells.Where(c => c.Row == 0).ForEach(c => columns.Add(c.ValueAsString, c.Column));

            var totalRows = sheetModel.Cells.Max(c => c.Row);
            for (int rowIndex = 1; rowIndex <= totalRows; rowIndex++)
            {
                var row = sheetModel.Cells.Where(c => c.Row == rowIndex).ToList();
                var ordinale = row.GetCellValueAsString(columns[Resources.ColumnOrdinale]);
                if (string.IsNullOrEmpty(ordinale))
                    break;

                documentRowData = new DocumentRowData()
                {
                    Ordinale = row.GetCellValueAsString(columns[Resources.ColumnOrdinale]),
                    OrdinalePrincipale = row.GetCellValueAsString(columns[Resources.ColumnOrdinalePrincipale]),
                    Oggetto = row.GetCellValueAsString(columns[Resources.ColumnDescrizione]),
                    CodiceUtenteCreatore = row.GetCellValueAsString(columns[Resources.ColumnCodiceUtenteCreatore]),
                    CodiceRuoloCreatore = row.GetCellValueAsString(columns[Resources.ColumnCodiceRuoloCreatore]),
                    CodiceAmm = row.GetCellValueAsString(columns[Resources.ColumnCodiceAmministrazione]),
                    NomeFile = row.GetCellValueAsString(columns[Resources.ColumnPathname])
                };

                listDocumentRowData.Add(documentRowData);
            }

            return listDocumentRowData;
        }


        protected virtual async Task<string> CreaDocumentoAmministrativo(DocumentRowData documentRowData, string codiceUO, string codiceAmministrazione, bool isAllegato, string? idDocumentoPrincipale = null)
        {
            string docnumber = string.Empty;
            try
            {
                var idTenant = await _dbContext.AmministraEntities.AsNoTracking()
                    .Where(a => a.VAR_CODICE_AMM.ToUpper().Equals(documentRowData.CodiceAmm.ToUpper()))
                    .Select(a => a.SYSTEM_ID)
                    .FirstOrDefaultAsync();

                var utenteCreatore = await _dbContext.PeopleEntities.AsNoTracking()
                    .Where(p => p.USER_ID.ToUpper().Equals(documentRowData.CodiceUtenteCreatore.ToUpper())
                        && p.ID_AMM == idTenant)
                    .Select(p => new
                    {
                        p.SYSTEM_ID,
                        p.USER_ID,
                        p.FULL_NAME,
                        p.VAR_NOME,
                        p.VAR_COGNOME
                    })
                    .FirstOrDefaultAsync();

                var ruoloCreatore = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(g => g.VAR_COD_RUBRICA.ToUpper().Equals(documentRowData.CodiceRuoloCreatore.ToUpper()))
                    .Select(g => new
                    {
                        g.SYSTEM_ID,
                        g.VAR_COD_RUBRICA,
                        g.VAR_DESC_CORR,
                        g.ID_GRUPPO
                    })
                    .FirstOrDefaultAsync();

                _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.UserId, utenteCreatore.USER_ID);
                _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.IdUser, utenteCreatore.SYSTEM_ID.ToString());
                _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.UserName, utenteCreatore.VAR_NOME);
                _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.UserSurname, utenteCreatore.VAR_COGNOME);
                _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.IdGroup, ruoloCreatore.ID_GRUPPO.ToString());
                _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.GroupCode, ruoloCreatore.VAR_COD_RUBRICA);
                _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.GroupDescription, ruoloCreatore.VAR_DESC_CORR);

                DatiRegistro? datiRegistro = null;
                long? idRegistro = null;
                string? codiceRegistro = null;
                if (!isAllegato)
                {
                    var registro = await _dbContext.RegistroEntities.AsNoTracking()
                           .Where(r => r.VAR_CODICE.ToUpper().Equals(documentRowData.CodiceRegistro.ToUpper()))
                           .Select(r => new
                           {
                               r.SYSTEM_ID,
                               r.VAR_CODICE
                           })
                           .FirstOrDefaultAsync();

                    idRegistro = registro.SYSTEM_ID;
                    codiceRegistro = registro.VAR_CODICE;
                    datiRegistro = new DatiRegistro()
                    {
                        IdRegistro = idRegistro.ToString()
                    };
                }

                var documentoAmministrativoAggregate = new DocumentoAmministrativo(idTenant.ToString(),
                    DateTime.Now,
                    new OggettoDelDocumento()
                    {
                        Descrizione = new TextValue(documentRowData.Oggetto),
                    },
                    datiRegistro,
                    null,
                    TipologieVisibilitaEnum.Gerarchica, 
                    isAllegato ? new IdDoc()
                    {
                        Identiticativo = idDocumentoPrincipale
                    } : null
                    );

                FileValidationResult fileValidateAllegatoResult = null;

                if (!string.IsNullOrEmpty(documentRowData.NomeFile))
                {
                    var repositoryRootPath = await this._configurationService.GetValue<string>("REPOSITORY_ROOT_PATH");

                    var pathFile = Path.Combine(
                                repositoryRootPath,
                                "Formazione",
                                codiceAmministrazione.ToUpper(),
                                codiceUO,
                                "File");
                    if (isAllegato)
                        pathFile = Path.Combine(pathFile, "allegati");

                    pathFile = Path.Combine(pathFile, documentRowData.NomeFile).PathAsUnixPath();
                    byte[] content = File.ReadAllBytes(pathFile);

                    TipoFirmaEnum tipoFirmaEnum = TipoFirmaEnum.Nessuna;
                    if (documentRowData.NomeFile.ToUpper().EndsWith("P7M"))
                    {
                        tipoFirmaEnum = TipoFirmaEnum.Cades;
                    }
                    if (documentRowData.NomeFile.ToUpper().EndsWith("TSD"))
                    {
                        tipoFirmaEnum = TipoFirmaEnum.Tsd;
                    }
                    if (documentRowData.NomeFile.ToUpper().EndsWith("PDF") && await _pAdESService.IsPAdESFile(new MemoryStream(content)))
                    {
                        tipoFirmaEnum = TipoFirmaEnum.Pades;
                    }
                    if (documentRowData.NomeFile.ToUpper().EndsWith("XML") && await IsSignedXades(content))
                    {
                        tipoFirmaEnum = TipoFirmaEnum.Xades;
                    }

                    var newDocumentBlobAggregate = new DocumentBlob(idTenant.ToString(), DateTime.Now, new TextValue(documentRowData.NomeFile));
                    newDocumentBlobAggregate.UploadStream(new MemoryStream(content), documentRowData.NomeFile);
                    newDocumentBlobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                    await _documentBlobRepository.Add(newDocumentBlobAggregate);

                    documentoAmministrativoAggregate.AssignDocumentBlobRef(
                    new DocumentBlobRef()
                    {
                        IdBlob = newDocumentBlobAggregate.Id,
                        FileName = newDocumentBlobAggregate.FileName,
                        ContentType = newDocumentBlobAggregate.ContentType,
                        FileSize = newDocumentBlobAggregate.FileSize,
                        CreationDate = await _dbContext.GetSystemDateTime(),
                        Hash = newDocumentBlobAggregate.Hash,
                        HashName = HashNamesEnum.SHA256,
                        Cartaceo = false,
                        SegnaturaPermanente = false,
                        TipoFirma = tipoFirmaEnum
                    },
                    new TargetVersionBehavior()
                    {
                        CreateNewVersion = true
                    });

                    fileValidateAllegatoResult = await _fileValidatorService.Validate(new FileToValidate()
                    {
                        Name = newDocumentBlobAggregate.FileName,
                        Stream = newDocumentBlobAggregate.Stream
                    });
                }

                await _documentoAmministrativoRepository.Add(documentoAmministrativoAggregate);

                await _mediator.Send(new Requests.DocumentoAddInfoFileRequest(new DocsPaVO.documento.FileRequest()
                {
                    docNumber = documentoAmministrativoAggregate.Id,
                    versionId = documentoAmministrativoAggregate.CurrentVersion.Id,
                    fileName = documentRowData.NomeFile,
                    dataAcquisizione = (await _dbContext.GetSystemDateTime()).AsDateTimeFormat()
                },
                  documentoAmministrativoAggregate.IdDocPrimario?.Identiticativo.AsLong(),
                  fileValidateAllegatoResult));

                docnumber = documentoAmministrativoAggregate.IdDoc.Identiticativo;

                if(!isAllegato)
                    await this._webMethodLoggerService.LogOK("DOCUMENTOADDDOCGRIGIA", docnumber, string.Format(Resources.LogDocumentoAddDocGrigio, docnumber));

                if(!string.IsNullOrEmpty(documentRowData.CodiceUtenteAreaLavoro))
                {
                    var utenteAreaLavoro= await _dbContext.PeopleEntities.AsNoTracking()
                    .Where(p => p.USER_ID.ToUpper().Equals(documentRowData.CodiceUtenteAreaLavoro.ToUpper())
                        && p.ID_AMM == idTenant)
                    .Select(p => new
                    {
                        p.SYSTEM_ID,
                        p.USER_ID,
                        p.FULL_NAME,
                        p.VAR_NOME,
                        p.VAR_COGNOME
                    })
                    .FirstOrDefaultAsync();

                    if (!isAllegato)
                    {
                        AreaLavoroEntity area = new AreaLavoroEntity()
                        {
                            ID_PEOPLE = utenteAreaLavoro.SYSTEM_ID,
                            ID_RUOLO_IN_UO = ruoloCreatore.SYSTEM_ID,
                            ID_PROFILE = docnumber.AsLong(),
                            ID_PROJECT = null,
                            CHA_TIPO_DOC = "G",
                            CHA_TIPO_FASC = null,
                            DTA_INS = await _dbContext.GetSystemDateTime(),
                            ID_REGISTRO = idRegistro
                        };

                        await this._dbContext.AreaLavoroEntities.AddAsync(area);
                    }

                    await ((DbContext)this._dbContext).SaveChangesAsync();
                }

                if(!string.IsNullOrEmpty(documentRowData.CodiceFascicolo))
                {
                    var idTitolario = await _dbContext.ProjectEntities.AsNoTracking()
                        .Where(p => p.ID_AMM == idTenant &&
                            p.CHA_STATO != null &&
                            p.CHA_STATO.Equals("A") &&
                            p.ID_TITOLARIO == 0 &&
                            p.VAR_CODICE != null &&
                            p.VAR_CODICE.Equals("T") &&
                            p.ID_PARENT == 0)
                        .Select(p => p.SYSTEM_ID)
                        .FirstOrDefaultAsync();

                    var fascicolo = (await _mediator.Send(new Requests.FascicolazioneGetFascicoloDaCodice2(idTenant.ToString(),
                        ruoloCreatore.ID_GRUPPO.ToString(),
                        utenteCreatore.SYSTEM_ID.ToString(),
                        documentRowData.CodiceFascicolo,
                        new Registro()
                        {
                            systemId = idRegistro.ToString(),
                            codice = codiceRegistro
                        },
                        true,
                        true,
                        idTitolario.ToString()))).output;

                    if (fascicolo != null && !string.IsNullOrEmpty(fascicolo.systemID))
                    {
                        await _mediator.Send(new Requests.FascicolazioneAddDocFascicolo(new InfoUtente()
                        {
                            idPeople = utenteCreatore.SYSTEM_ID.ToString(),
                            idGruppo = ruoloCreatore.ID_GRUPPO.ToString(),
                            idCorrGlobali = ruoloCreatore.SYSTEM_ID.ToString(),
                            idAmministrazione = idTenant.ToString()
                        },
                        docnumber,
                        fascicolo,
                        true
                        ));
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return docnumber;
        }

        protected class DocumentRowData
        {
            public string Ordinale { get; set; }
            public string OrdinalePrincipale { get; set; }
            public string Oggetto {  get; set; }
            public string CodiceUtenteCreatore { get; set; }
            public string CodiceRuoloCreatore { get; set; }
            public string CodiceUtenteAreaLavoro { get; set; }
            public string CodiceAmm { get; set; }
            public string TipoDocumento { get; set; }
            public string CodiceFascicolo { get; set; }
            public string CodiceRegistro { get; set; }
            public string NomeFile { get; set; }
        }

        protected virtual async Task<bool> IsSignedXades(byte[] content)
        {
            bool result = false;
            XmlDocument Xmlfile = new XmlDocument();
            XmlTextReader tr = new XmlTextReader(new System.IO.MemoryStream(content));
            tr.XmlResolver = null;
            try
            {
                Xmlfile.Load(tr);
                XmlNodeList signature = Xmlfile.DocumentElement.GetElementsByTagName("ds:Signature");
                if (signature != null && signature.Count > 0)
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Errore nel metodo IsSignedXades " + e.Message);
                result = false;
            }
            finally
            {
                tr.Close();
            }

            return result;
        }
        #endregion
    }

    internal static class CellModelExtensions
    {
        public static string GetCellValueAsString(this IEnumerable<CellModel> cellsPerRow, int column)
        {
            string value = null!;

            var cell = cellsPerRow.FirstOrDefault(c => c.Column == column);
            if (cell != null)
            {
                value = (cell?.ValueAsString! ?? String.Empty).Trim();
            }

            return value! ?? String.Empty;
        }
    }
}