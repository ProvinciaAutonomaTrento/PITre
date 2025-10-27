// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.utente;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExportRubricaWithTitleNewRequest = Pi3.App.Legacy.WebApi.Application.Requests.ExportRubricaWithTitleNew;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ExportRubricaWithTitleNew
{
    public class ExportRubricaWithTitleNewHandler : IRequestHandler<ExportRubricaWithTitleNewRequest, ExportRubricaWithTitleNewResult>
    {
        #region Public Members

        public ExportRubricaWithTitleNewHandler(
            ILogger<ExportRubricaWithTitleNewHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext,
            ISpreadsheetService spreadsheetService
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._spreadsheetService = spreadsheetService;
            this._dbContext = dbContext;
        }

        public async Task<ExportRubricaWithTitleNewResult> Handle(ExportRubricaWithTitleNewRequest request, CancellationToken cancellationToken)
        {
            FileDocumento output = new();
            try
            {
                if (!string.IsNullOrEmpty(request.tipologia) && request.tipologia.ToUpper().Equals("JSON"))
                {
                    output = await this.GenerateRepJson(request.infoUtente.idCorrGlobali, "", "");
                }
                else
                {
                    output = await this.GenerateRep(request.infoUtente.idCorrGlobali, "", "");
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception : ex,message:ex.Message);
            }
            return new(output);
        }



        #endregion

        #region Private Members

        protected readonly ILogger<ExportRubricaWithTitleNewHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly ISpreadsheetService _spreadsheetService;



        private async Task<FileDocumento> GenerateRep(string idRuolo, string all, string idAooColl)
        {
            var data = await this.GetListaRegRfRuolo(idRuolo,all,idAooColl);

            FileDocumento output = null;

            var model = new SpreadsheetModel();
            var sheet = new SheetModel()
            {
                Name = Resources.sheetName
            };
            List<string> header = new List<string>()
            {
                "Storicizza",
                "Cod. Registro",
                "Cod. Rubrica",
                "Cod. Amm.",
                "Cod. AOO",
                "Tipo",
                "Descrizione",
                "Cognome",
                "Nome",
                "Indirizzo",
                "CAP",
                "Citt�",
                "Provincia",
                "Nazione",
                "Cod. Fiscale",
                "P. IVA",
                "Tel 1",
                "Tel 2",
                "Fax",
                "Email",
                "Localit�",
                "Note",
                "Nuovo Registro",
                "Canale preferenziale"
            };
            int column = 0;

            foreach (var cell in header)
            {

                sheet.AddCell(new CellModel()
                {
                    Row = 0,
                    Column = column,
                    ValueAsString = cell,
                    CellStyle = new CellStyleModel()
                    {
                        FontIsBold = true,
                        ForegroundColor = System.Drawing.Color.Gray,
                        FontName = "Arial",
                        FontSize = 14,
                        VerticalAlignment = CellTextAlignments.Center,
                        HorizontalAlignment = CellTextAlignments.Center,
                        Width = (column == 6 || column == 19) ? 50 : 30,
                        FontColor = System.Drawing.Color.Black,
                    }
                });
                column++;
            }


            int row = 1;
            column = 0;
            
            foreach (var dataRw in data)
            {
                var r = this.ExtractData(dataRw);
                for (int c=0;c<r.Count;c++ )
                {

                    sheet.AddCell(new CellModel()
                    {
                        Row = row,
                        Column = c,
                        ValueAsString = r[c],
                        CellStyle = new CellStyleModel()
                        {
                            FontIsBold = false,
                            FontName = "Arial",
                            FontSize = 12,
                            FontColor = System.Drawing.Color.Black,
                            FontIsStrikeout = true,
                            VerticalAlignment = CellTextAlignments.Center,
                            HorizontalAlignment = CellTextAlignments.Center,
                        }
                    });
                }
                row++;
            }

            
            model.AddSheet(sheet);

            using MemoryStream stream = new MemoryStream();
            var reportGenerated = await _spreadsheetService.Write(model, stream);

            output = new FileDocumento()
            {
                content = stream.ToArray(),
                length = Convert.ToInt32(stream.Length),
                contentType = reportGenerated.ContentType,
                estensioneFile = Path.GetExtension(reportGenerated.FileName),
                fullName = string.Format(Resources.FullNameExportXLSX, DateTime.Now.ToString("dd-MM-yyyy")),
                name = string.Format(Resources.FullNameExportXLSX, DateTime.Now.ToString("dd-MM-yyyy")),
            };
            return output;
        }

        private async Task<FileDocumento> GenerateRepJson(string idRuolo, string all, string idAooColl)
        {
            var data = await this.GetListaRegRfRuolo(idRuolo, all, idAooColl);
            var jsonResult = JsonConvert.SerializeObject(data);

            FileDocumento output = null;
            var content = Encoding.UTF8.GetBytes(jsonResult);

            output = new FileDocumento()
            {
                content = content,
                length = content.Length,
                contentType = "application/json",
                estensioneFile = Path.GetExtension(Resources.FullNameExportJSON),
                fullName = string.Format(Resources.FullNameExportJSON, DateTime.Now.ToString("dd-MM-yyyy")),
                name = string.Format(Resources.FullNameExportJSON, DateTime.Now.ToString("dd-MM-yyyy"))
            };
            return output;
        }

        private List<string> ExtractData(DatiModificaCorr corr)
        {
            List<string> row = new()
            {
                "",
                corr.codice ?? string.Empty,
                corr.codRubrica ?? string.Empty,
                corr.codiceAmm?? string.Empty,
                corr.codiceAoo ?? string.Empty,
                corr.tipoCorrispondente ?? string.Empty,
                corr.descCorr ?? string.Empty,
                corr.cognome ?? string.Empty,
                corr.nome ?? string.Empty,
                corr.indirizzo ?? string.Empty,
                corr.cap ?? string.Empty,
                corr.citta ?? string.Empty,
                corr.provincia ?? string.Empty,
                corr.nazione ?? string.Empty,
                corr.codFiscale ?? string.Empty,
                corr.partitaIva ?? string.Empty,
                corr.telefono ?? string.Empty,
                corr.telefono2 ?? string.Empty,
                corr.fax ?? string.Empty,
                corr.email ?? string.Empty,
                corr.localita ?? string.Empty,
                corr.note ?? string.Empty,
                string.Empty,
                corr.descrizioneCanalePreferenziale ?? string.Empty,
        };

            return row;
        }
        
        private async Task<List<DatiModificaCorr>> GetListaRegRfRuolo(string idRuolo, string all, string idAooColl)
        {
            var idAmm = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
            List<long> regs = new();
            var predicate = PredicateBuilder.New<QueryMetaData>();
            var baseQuery = (from b in this._dbContext.RuoloRegistroEntities.AsNoTracking()
             from a in this._dbContext.RegistroEntities.AsNoTracking()
             where a.SYSTEM_ID == b.ID_REGISTRO && b.ID_RUOLO_IN_UO == idRuolo.AsLong()
             select new QueryMetaData()
             {
                 A = a,
                 B = b
             });

            if (!string.IsNullOrEmpty(all))
            {
                if(!string.IsNullOrEmpty(idAooColl) && idAooColl.Equals("1"))
                {
                    predicate = predicate.And(r => r.A.CHA_RF != null && r.A.CHA_RF.Equals(all) && r.A.ID_AOO_COLLEGATA == idAooColl.AsLong());
                }
                else
                {
                    predicate = predicate.And(r => r.A.CHA_RF != null && r.A.CHA_RF.Equals(all));
                }
            }
            else
            {
                predicate = predicate.And(r => true);
            }
            
            var rows = await baseQuery.Where(predicate).OrderByDescending(r => r.B.CHA_PREFERITO != null).ThenBy(r => r.B.CHA_PREFERITO).ThenBy(r => r.A.VAR_CODICE).ToListAsync();

            foreach (var ent in rows)
            {
                long regId = ent.A.SYSTEM_ID;

                regs.Add(regId);
            }

            const string interop = "INTEROP_";
            List<string> occOrCom = new() { "C", "O" };
            List<DatiModificaCorr> query = new();

            if (regs.Count > 0)
            {
                //Getting corrs with regs found
                query = await (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                               from r in this._dbContext.RegistroEntities.AsNoTracking().Where(ri => ri.SYSTEM_ID == a.ID_REGISTRO).DefaultIfEmpty()
                               join bi in this._dbContext.DettGlobaliEntities.AsNoTracking() on a.SYSTEM_ID equals bi.ID_CORR_GLOBALI into bt
                               from b in bt.DefaultIfEmpty()
                               join tcc in this._dbContext.CanaleCorrEntities.AsNoTracking() on a.SYSTEM_ID equals tcc.ID_CORR_GLOBALE into tct
                               from tc in tct.DefaultIfEmpty()
                               join dt in this._dbContext.DocumentTypesEntities.AsNoTracking() on tc.ID_DOCUMENTTYPE equals dt.SYSTEM_ID
                               where
                               !occOrCom.Contains(a.CHA_TIPO_CORR) && a.CHA_TIPO_IE != null && a.CHA_TIPO_IE.Equals("E") && !a.DTA_FINE.HasValue
                               && a.ID_AMM == idAmm.AsLong() && (a.VAR_COD_RUBRICA == null || !a.VAR_COD_RUBRICA.ToUpper().StartsWith(interop)) && (r == null || regs.Contains(r.SYSTEM_ID))
                               orderby a.SYSTEM_ID , a.VAR_DESC_CORR
                               select new DocsPaVO.utente.DatiModificaCorr()
                               {
                                   codice = r.VAR_CODICE,
                                   codRubrica = a.VAR_COD_RUBRICA,
                                   codiceAmm = a.VAR_CODICE_AMM,
                                   codiceAoo = a.VAR_CODICE_AOO,
                                   tipoCorrispondente = a.CHA_TIPO_URP,
                                   descCorr = a.VAR_DESC_CORR,
                                   cognome = a.VAR_COGNOME,
                                   nome = a.VAR_NOME,
                                   indirizzo = b.VAR_INDIRIZZO,
                                   cap = b.VAR_CAP,
                                   citta = b.VAR_CITTA,
                                   provincia = b.VAR_PROVINCIA,
                                   localita = b.VAR_LOCALITA,
                                   nazione = b.VAR_NAZIONE,
                                   telefono = b.VAR_TELEFONO,
                                   telefono2 = b.VAR_TELEFONO2,
                                   fax = b.VAR_FAX,
                                   email = IPi3DbContextMappedFunctions.MailENoteCorrEsterni(a.SYSTEM_ID),
                                   note = b.VAR_NOTE,
                                   codFiscale = b.VAR_COD_FISC,
                                   partitaIva = b.VAR_COD_PI,
                                   descrizioneCanalePreferenziale = dt.DESCRIPTION

                               }).ToListAsync();


            }
            else
            {
                query = await (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                               from r in this._dbContext.RegistroEntities.AsNoTracking().Where(ri => ri.SYSTEM_ID == a.ID_REGISTRO).DefaultIfEmpty()
                               join bi in this._dbContext.DettGlobaliEntities.AsNoTracking() on a.SYSTEM_ID equals bi.ID_CORR_GLOBALI into bt
                               from b in bt.DefaultIfEmpty()
                               join tcc in this._dbContext.CanaleCorrEntities.AsNoTracking() on a.SYSTEM_ID equals tcc.ID_CORR_GLOBALE into tct
                               from tc in tct.DefaultIfEmpty()
                               join dt in this._dbContext.DocumentTypesEntities.AsNoTracking() on tc.ID_DOCUMENTTYPE equals dt.SYSTEM_ID
                               where
                               !occOrCom.Contains(a.CHA_TIPO_CORR) && a.CHA_TIPO_IE != null && a.CHA_TIPO_IE.Equals("E") && !a.DTA_FINE.HasValue
                               && a.ID_AMM == idAmm.AsLong() && (a.VAR_COD_RUBRICA == null || !a.VAR_COD_RUBRICA.ToUpper().StartsWith(interop))
                               orderby a.SYSTEM_ID, a.VAR_DESC_CORR
                               select new DocsPaVO.utente.DatiModificaCorr()
                               {
                                   codice = r.VAR_CODICE,
                                   codRubrica = a.VAR_COD_RUBRICA,
                                   codiceAmm = a.VAR_CODICE_AMM,
                                   codiceAoo = a.VAR_CODICE_AOO,
                                   tipoCorrispondente = a.CHA_TIPO_URP,
                                   descCorr = a.VAR_DESC_CORR,
                                   cognome = a.VAR_COGNOME,
                                   nome = a.VAR_NOME,
                                   indirizzo = b.VAR_INDIRIZZO,
                                   cap = b.VAR_CAP,
                                   citta = b.VAR_CITTA,
                                   provincia = b.VAR_PROVINCIA,
                                   localita = b.VAR_LOCALITA,
                                   nazione = b.VAR_NAZIONE,
                                   telefono = b.VAR_TELEFONO,
                                   telefono2 = b.VAR_TELEFONO2,
                                   fax = b.VAR_FAX,
                                   email = IPi3DbContextMappedFunctions.MailENoteCorrEsterni(a.SYSTEM_ID),
                                   note = b.VAR_NOTE,
                                   codFiscale = b.VAR_COD_FISC,
                                   partitaIva = b.VAR_COD_PI,
                                   descrizioneCanalePreferenziale = dt.DESCRIPTION

                               }).ToListAsync();
            }


            return query;
        }

        private class QueryMetaData
        {
            public RuoloRegistroEntity? B { get; set; }
            public RegistroEntity? A { get; set; }

        }

        #endregion
    }
}