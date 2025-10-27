// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExportIndiceSistematicoRequest = Pi3.App.Legacy.WebApi.Application.Requests.ExportIndiceSistematico;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ExportIndiceSistematico
{
    public class ExportIndiceSistematicoHandler : IRequestHandler<ExportIndiceSistematicoRequest, ExportIndiceSistematicoResult>
    {
        #region Public Members

        public ExportIndiceSistematicoHandler(ILogger<ExportIndiceSistematicoHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext, ISpreadsheetService spreadsheetService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._spreadsheetService = spreadsheetService;
        }

        public async Task<ExportIndiceSistematicoResult> Handle(ExportIndiceSistematicoRequest request, CancellationToken cancellationToken)
        {
            FileDocumento output = new FileDocumento()
            {
                name = Resources.FileName,
            };

            DocsPaVO.amministrazione.OrgTitolario titolario = request.titolario;

            try
            {
                long idTitolario = titolario.ID.AsLong();
                var nodoVociIndice = await this._dbContext.ProjectEntities
                    .LeftJoin(this._dbContext.AssIndxSisEntities, p => p.SYSTEM_ID, ais => ais.ID_PROJECT, (p, ais) => new { p, ais })
                    .LeftJoin(this._dbContext.IndexSisEntities, j1 => j1.ais.ID_INDICE_SIS, idxs => idxs.SYSTEM_ID, (j1, idxs) => new { p = j1.p, ais = j1.ais, idxs })
                    .Where(x => !string.IsNullOrEmpty(x.p.CHA_TIPO_PROJ) && x.p.CHA_TIPO_PROJ.Equals("T") && x.p.ID_TITOLARIO == idTitolario)
                    .Select(x => new NodoVociIndice
                    {
                        VAR_CODICE = x.p.VAR_CODICE ?? string.Empty,
                        DESCRIPTION = x.p.DESCRIPTION ?? string.Empty,
                        ID_REGISTRO = x.p.ID_REGISTRO,
                        VOCE_INDICE = x.idxs.VOCE_INDICE ?? string.Empty,
                        VAR_COD_LIV1 = x.p.VAR_COD_LIV1 ?? string.Empty
                    })
                    .OrderBy(x => x.VAR_COD_LIV1)
                    .ToListAsync();

                var voceNodiDiTitolario = await this._dbContext.ProjectEntities
                    .LeftJoin(this._dbContext.AssIndxSisEntities, p => p.SYSTEM_ID, ais => ais.ID_PROJECT, (p, ais) => new { p, ais })
                    .LeftJoin(this._dbContext.IndexSisEntities, j1 => j1.ais.ID_INDICE_SIS, idxs => idxs.SYSTEM_ID, (j1, idxs) => new { p = j1.p, ais = j1.ais, idxs })
                    .Where(x => !string.IsNullOrEmpty(x.p.CHA_TIPO_PROJ) && x.p.CHA_TIPO_PROJ.Equals("T") && x.p.ID_TITOLARIO == idTitolario)
                    .Select(x => new VoceNodiDiTitolario
                    {
                        VAR_CODICE = x.p.VAR_CODICE ?? string.Empty,
                        DESCRIPTION = x.p.DESCRIPTION ?? string.Empty,
                        ID_REGISTRO = x.p.ID_REGISTRO,
                        VOCE_INDICE = x.idxs.VOCE_INDICE ?? string.Empty
                    })
                    .OrderBy(x => x.VOCE_INDICE)
                    .ToListAsync();

                var model = new SpreadsheetModel();
                var sheet1 = await GetSheet1(nodoVociIndice);

                model.AddSheet(sheet1);

                var sheet2 = GetSheet2(voceNodiDiTitolario);

                model.AddSheet(sheet2);


                using (MemoryStream stream = new MemoryStream())
                {
                    var generatedReport = await this._spreadsheetService.Write(model, stream);
                    output.content = stream.ToArray();
                    output.length = Convert.ToInt32(stream.Length);
                    output.estensioneFile = Path.GetExtension(generatedReport.FileName);
                    output.contentType = generatedReport.ContentType;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new ExportIndiceSistematicoResult(output);
        }


        #endregion

        #region Private Members
        private SheetModel GetSheet2(List<VoceNodiDiTitolario> ds_voceNodiDiTitolario)
        {
            var sheet = new SheetModel()
            {
                Name = Resources.Sheet2Name
            };

            // Intestazione
            List<string> headers = new List<string> { Resources.Sheet2Col1Name, Resources.Sheet2Col2Name };
            int i = 0;
            headers.ForEach(x =>
            {
                sheet.AddCell(cellModel: new CellModel
                {
                    Row = 0,
                    Column = i,
                    ValueAsString = x,
                    CellStyle = this.SpreadsheetHeaderCellStyle,
                    Name = x + "_0"
                });
                i++;
            });

            int start = 2;
            string voceIndice = ds_voceNodiDiTitolario[0].VOCE_INDICE;
            string nodiDiTitolario = ds_voceNodiDiTitolario[0].VAR_CODICE + " - " + ds_voceNodiDiTitolario[0].DESCRIPTION;

            sheet.AddCell(cellModel: new CellModel
            {
                Row = 1,
                Column = 0,
                ValueAsString = ds_voceNodiDiTitolario[0].VAR_CODICE,
                CellStyle = this.SpreadsheetRowCellStyle,
                Name = Resources.Sheet2Col1Name + "_" + start
            });
            sheet.AddCell(cellModel: new CellModel
            {
                Row = 1,
                Column = 1,
                ValueAsString = voceIndice,
                CellStyle = this.SpreadsheetRowCellStyle,
                Name = Resources.Sheet2Col2Name + "_" + start
            });

            foreach (VoceNodiDiTitolario row in ds_voceNodiDiTitolario)
            {
                //if (start == 0)
                //    nodiDiTitolario = row.VAR_CODICE + " - " + row.DESCRIPTION;
                //else
                //{
                    if (!string.IsNullOrEmpty(voceIndice))
                    {
                        if (voceIndice == row.VOCE_INDICE)
                            nodiDiTitolario += row.VAR_CODICE + " - " + row.DESCRIPTION + ";";
                        else
                        {
                            //righe += inserisciRigaIndiceVoceNodi(voceIndice, nodiDiTitolario);
                            sheet.AddCell(cellModel: new CellModel
                            {
                                Row = start,
                                Column = 0,
                                ValueAsString = row.VAR_CODICE,
                                CellStyle = this.SpreadsheetRowCellStyle,
                                Name = Resources.Sheet2Col1Name + "_" + start
                            });
                            sheet.AddCell(cellModel: new CellModel
                            {
                                Row = start,
                                Column = 1,
                                ValueAsString = row.VOCE_INDICE,
                                CellStyle = this.SpreadsheetRowCellStyle,
                                Name = Resources.Sheet2Col2Name + "_" + start
                            });
                            voceIndice = row.VOCE_INDICE;
                            nodiDiTitolario = row.VAR_CODICE + " - " + row.DESCRIPTION;
                        }
                    //}
                }
                start++;
            }

            return sheet;
        }
        private async Task<SheetModel> GetSheet1(List<NodoVociIndice> nodoVociIndice)
        {
            var sheet = new SheetModel()
            {
                Name = Resources.Sheet1Name
            };

            // Intestazione
            List<string> headers = new List<string> { Resources.Sheet1Col1Name, Resources.Sheet1Col2Name, Resources.Sheet1Col3Name, Resources.Sheet1Col4Name };
            int i = 0;
            headers.ForEach(x =>
            {
                sheet.AddCell(cellModel: new CellModel
                {
                    Row = 0,
                    Column = i,
                    ValueAsString = x,
                    CellStyle = this.SpreadsheetHeaderCellStyle,
                    Name = x + "_0"
                });
                i++;
            });

            string vociIndice = string.Empty;
            string codiceRegistro = await this._dbContext.RegistroEntities.Where(x => x.SYSTEM_ID == nodoVociIndice[0].ID_REGISTRO).Select(x => x.VAR_CODICE).FirstOrDefaultAsync() ?? string.Empty;
            string codiceNodo = nodoVociIndice[0].VAR_CODICE ?? string.Empty;
            string descrizioneNodo = nodoVociIndice[0].DESCRIPTION ?? string.Empty;
            string voceIndice0 = nodoVociIndice[0].VOCE_INDICE ?? string.Empty;

            sheet.AddCell(cellModel: new CellModel
            {
                Row = 1,
                Column = 0,
                ValueAsString = codiceNodo,
                CellStyle = this.SpreadsheetRowCellStyle,
                Name = Resources.Sheet1Col1Name + "_1"
            });
            sheet.AddCell(cellModel: new CellModel
            {
                Row = 1,
                Column = 1,
                ValueAsString = descrizioneNodo,
                CellStyle = this.SpreadsheetRowCellStyle,
                Name = Resources.Sheet1Col2Name + "_1"
            });
            sheet.AddCell(cellModel: new CellModel
            {
                Row = 1,
                Column = 2,
                ValueAsString = codiceRegistro,
                CellStyle = this.SpreadsheetRowCellStyle,
                Name = Resources.Sheet1Col3Name + "_1"
            });
            sheet.AddCell(cellModel: new CellModel
            {
                Row = 1,
                Column = 3,
                ValueAsString = voceIndice0,
                CellStyle = this.SpreadsheetRowCellStyle,
                Name = Resources.Sheet1Col4Name + "_1"
            });

            string righe = string.Empty;
            for (int j = 1; j < nodoVociIndice.Count(); j++)
            {
                var n = nodoVociIndice[j];
                int row = j+1;
                //if (start == 1)
                //{
                //    vociIndice = n.VOCE_INDICE ?? string.Empty;
                //}
                //else
                //{
                //Codici nodo uguali e registri nulli

                if (codiceNodo == n.VAR_CODICE &&
                        n.ID_REGISTRO != null &&
                        !string.IsNullOrEmpty(n.VOCE_INDICE))
                {
                    vociIndice += n.VOCE_INDICE + ";";
                }

                //Codici nodo uguali e registri uguali
                if (codiceNodo == n.VAR_CODICE &&
                     n.ID_REGISTRO != null &&
                    codiceRegistro == await this.GetCodiceRegistro(n.ID_REGISTRO) &&
                    !string.IsNullOrEmpty(n.VOCE_INDICE))
                {
                    vociIndice += n.VOCE_INDICE + ";";
                }

                //Inserimento riga
                if (codiceNodo != n.VAR_CODICE)
                {
                    //righe += this.InserisciRigaIndiceNodoVoci(codiceNodo, descrizioneNodo, codiceRegistro, vociIndice, voceNodiDiTitolario);
                    sheet.AddCell(cellModel: new CellModel
                    {
                        Row = row,
                        Column = 0,
                        ValueAsString = n.VAR_CODICE,
                        CellStyle = this.SpreadsheetRowCellStyle,
                        Name = Resources.Sheet1Col1Name + "_" + row
                    });
                    sheet.AddCell(cellModel: new CellModel
                    {
                        Row = row,
                        Column = 1,
                        ValueAsString = n.DESCRIPTION,
                        CellStyle = this.SpreadsheetRowCellStyle,
                        Name = Resources.Sheet1Col2Name + "_" + row
                    });
                    sheet.AddCell(cellModel: new CellModel
                    {
                        Row = row,
                        Column = 2,
                        ValueAsString = await this.GetCodiceRegistro(n.ID_REGISTRO),
                        CellStyle = this.SpreadsheetRowCellStyle,
                        Name = Resources.Sheet1Col3Name + "_" + row
                    });
                    sheet.AddCell(cellModel: new CellModel
                    {
                        Row = row,
                        Column = 3,
                        ValueAsString = vociIndice,
                        CellStyle = this.SpreadsheetRowCellStyle,
                        Name = Resources.Sheet1Col4Name + "_" + row
                    });

                    vociIndice = n.VOCE_INDICE;
                    codiceNodo = n.VAR_CODICE;
                    descrizioneNodo = n.DESCRIPTION;
                    if (n.ID_REGISTRO != null)
                        codiceRegistro = await this.GetCodiceRegistro(n.ID_REGISTRO);

                }
                //}
            }
            return sheet;
        }
        private async Task<string> GetCodiceRegistro(long? idRegistro)
        {
            return await this._dbContext.RegistroEntities.Where(r => r.SYSTEM_ID == idRegistro).Select(x => x.VAR_CODICE).FirstOrDefaultAsync() ?? string.Empty;
        }

        protected readonly ILogger<ExportIndiceSistematicoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected ISpreadsheetService _spreadsheetService;

        private CellStyleModel SpreadsheetHeaderCellStyle = new CellStyleModel
        {
            FontName = "ARIAL",
            FontSize = 8,
            FontIsBold = true,
            FontColor = System.Drawing.Color.White,
            ForegroundColor = System.Drawing.Color.DarkRed
        };
        private CellStyleModel SpreadsheetRowCellStyle = new CellStyleModel
        {
            FontName = "Arial",
            FontSize = 8,
            FontIsBold = false
        };

        private class VoceNodiDiTitolario
        {
            public string VAR_CODICE { get; set; }
            public string DESCRIPTION { get; set; }
            public long? ID_REGISTRO { get; set; }
            public string VOCE_INDICE { get; set; }
        }

        private class NodoVociIndice
        {
            public string VAR_CODICE { get; set; }
            public string DESCRIPTION { get; set; }
            public long? ID_REGISTRO { get; set; }
            public string VOCE_INDICE { get; set; }
            public string VAR_COD_LIV1 { get; set; }
        }
        #endregion
    }
}