// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Note;
using DocsPaVO.PrjDocImport;
using DocsPaVO.ricerche;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.NotaRFAggregate;
using Pi3.Core.AggregateModels.NotaRFAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.NotaRFAggregate.Exceptions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.InsertNotaInElencoDaExcel
{
    // Richiede libreria MediatR
    public class InsertNotaInElencoDaExcelHandler : IRequestHandler<Application.Requests.InsertNotaInElencoDaExcel, InsertNotaInElencoDaExcelResult>
    {
        #region Public Members

        public InsertNotaInElencoDaExcelHandler(ILogger<InsertNotaInElencoDaExcelHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            INotaRFRepository repository,
            ISpreadsheetService spreadsheetService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._repository = repository;
            this._spreadsheetService = spreadsheetService;
        }

        public async Task<InsertNotaInElencoDaExcelResult> Handle(Application.Requests.InsertNotaInElencoDaExcel request, CancellationToken cancellationToken)
        {
            List<ImportResult> importResult = new List<ImportResult>();
            List<NotaElenco> notes = new List<NotaElenco>();

            int importedNote = 0;
            int notImportedNote = 0;
            ImportResult temp = null;
            List<string> creationProblems = null;
            NotaElenco nota = null;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

                byte[] content = request.dati;
                if (content == null || content.Length == 0)
                    throw new NoContentException();

                var idCorrGlobaliRuolo = this._dbContext.CorrGlobaliEntities.Where(x => x.ID_GRUPPO == idGroup).Select(x => x.SYSTEM_ID).FirstOrDefault();
                if (idCorrGlobaliRuolo == null)
                    throw new CorrGlobaliByGroupIdNotFoundPi3Exception(idGroup);

                List<Register> listaRf = new List<Register>();
                listaRf = GetListaRegistriRfRuolo(idCorrGlobaliRuolo);

                var model = await _spreadsheetService.Read(new MemoryStream(content));


                List<CellModel> cells = model.Sheets[0].Cells.ToList();

                //conteggio righe
                bool existData = cells.Any(x => x.Row == 1);

                if (!existData)
                    importResult.Add(new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.OK,
                        Message = Resources.NoNotesToImport
                    });

                int maxRowNum = cells.Max(x => x.Row);
                int codRfIndexColumn = cells.Where(x => x.ValueAsString.ToUpper().Equals(Resources.RFCodeHeaderName)).Select(x => x.Column).FirstOrDefault();
                int descIndexColumn = cells.Where(x => x.ValueAsString.ToUpper().Equals(Resources.DescHeaderName)).Select(x => x.Column).FirstOrDefault();

                List<CellModel> cellInHeaderRow = cells.Where(x => x.Row == 0).ToList();
                bool isCodRFHeaderCorrect = cellInHeaderRow.Where(x => x.Column == 0).FirstOrDefault().ValueAsString.ToUpper().Equals(Resources.RFCodeHeaderName);
                bool isDescHeaderCorrect = cellInHeaderRow.Where(x => x.Column == 1).FirstOrDefault().ValueAsString.ToUpper().Equals(Resources.DescHeaderName);

                if (!(isCodRFHeaderCorrect || isDescHeaderCorrect))
                {
                    importResult.Add(new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.KO,
                        Message = Resources.WrongColumnsName
                    });
                    return new InsertNotaInElencoDaExcelResult(importResult.ToArray());
                }

                //foreach (var cell in cells)
                for (int i = 1; i <= maxRowNum; i++)
                {
                    nota = new NotaElenco();
                    List<CellModel> cellInRow = cells.Where(x => x.Row == i).ToList();
                    foreach (var cell in cellInRow)
                    {
                        int row = cell.Row;
                        int column = cell.Column;
                        string valueAsString = cell.ValueAsString ?? string.Empty;

                        if(column == codRfIndexColumn)
                            nota.codRegRf = valueAsString;
                        if(column == descIndexColumn)
                            nota.descNota = valueAsString;
                    }

                    nota.idRegRf = listaRf.Where(x => x.VAR_CODICE.Equals(nota.codRegRf)).Select(x => x.SYSTEM_ID).FirstOrDefault();

                    if (CheckDataValidity(nota, out creationProblems, listaRf))
                    {
                        //controllo se la nota esiste già
                        //long idRegRfAsLong = nota.idRegRf.AsLong();
                        //bool noteExists = await this._dbContext.ElencoNoteEntities.AnyAsync(x => x.ID_REG_RF == idRegRfAsLong && x.VAR_DESC_NOTA.Equals(nota.descNota) && x.COD_REG_RF.Equals(nota.codRegRf));

                        //if (noteExists)
                        //{
                        //    importResult.Add(new ImportResult()
                        //    {
                        //        Outcome = ImportResult.OutcomeEnumeration.KO,
                        //        Message = "Nota già presente in elenco"
                        //    });
                        //    return new InsertNotaInElencoDaExcelResult(importResult.ToArray());
                        //}

                        var aggregate = new NotaRF(idTenant, DateTime.Now, new TextValue(nota.descNota), null, nota.idRegRf, nota.codRegRf, null);
                        await this._repository.Add(aggregate);

                        if (string.IsNullOrEmpty(aggregate.Id))
                            throw new InsertNotePi3Exception();

                        importResult.Add(new ImportResult()
                        {
                            Outcome = ImportResult.OutcomeEnumeration.OK,
                            Message = nota.descNota + " --- " + nota.codRegRf
                        });
                    }
                    else
                    {
                        importResult.Add(new ImportResult()
                        {
                            Outcome = ImportResult.OutcomeEnumeration.KO,
                            OtherInformation = creationProblems
                        });
                        notImportedNote++;
                    }
                }
            }
            catch (NotaRFAlreadyExistsPi3Exception exPi3)
            {
                this._logger.LogError(exPi3, null, null);
                importResult.Add(new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.KO,
                    Message = Resources.NoteAlreadyExists
                });
                notImportedNote++;
            }
            catch (InsertNotePi3Exception exPi3)
            {
                this._logger.LogError(exPi3, null, null);
                importResult.Add(new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.KO,
                    Message = exPi3.Message
                });
                notImportedNote++;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                importResult.Add(new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.KO,
                    Message = String.Format(Resources.ExceptionMessage, ex.Message)
                });
                notImportedNote++;
            }
            finally
            {
                importResult.Add(new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.OK,
                    Message = String.Format(Resources.ResultImportNotes,
                    importedNote, notImportedNote)
                });
            }

            return new InsertNotaInElencoDaExcelResult(importResult.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<InsertNotaInElencoDaExcelHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly INotaRFRepository _repository;
        protected readonly ISpreadsheetService _spreadsheetService;

        private List<Register> GetListaRegistriRfRuolo(long idCorrGlobaliRuolo)
        {
            List<Register> listaRegRf = new List<Register>();
            var join = this._dbContext.RuoloRegistroEntities.Join(this._dbContext.RegistroEntities, b => b.ID_REGISTRO, a => a.SYSTEM_ID, (b, a) => new { b, a });

            var query = join.Where(x => x.b.ID_RUOLO_IN_UO == idCorrGlobaliRuolo && x.a.CHA_RF.Equals("1")).Select(x => new
            {
                SYSTEM_ID = x.a.SYSTEM_ID,
                VAR_CODICE = x.a.VAR_CODICE,
                //NUM_RIF = x.a.NUM_RIF,
                //VAR_DESC_REGISTRO = x.a.VAR_DESC_REGISTRO,
                //VAR_EMAIL_REGISTRO = x.a.VAR_EMAIL_REGISTRO,
                //CHA_STATO = x.a.CHA_STATO,
                //ID_AMM = x.a.ID_AMM,
                //DTA_OPEN = x.a.DTA_OPEN,
                //DTA_CLOSE = x.a.DTA_CLOSE,
                //DTA_ULTIMO_PROTO = x.a.DTA_ULTIMO_PROTO,
                //ID_RUOLO_AOO = x.a.ID_RUOLO_AOO,
                //ID_RUOLO_RESP = x.a.ID_RUOLO_RESP,
                //ID_PEOPLE_AOO = x.a.ID_PEOPLE_AOO,
                //CHA_AUTO_INTEROP = x.a.CHA_AUTO_INTEROP,
                //CHA_RF = x.a.CHA_RF,
                //CHA_DISABILITATO = x.a.CHA_DISABILITATO,
                //ID_AOO_COLLEGATA = x.a.ID_AOO_COLLEGATA,
                //INVIO_RICEVUTA_MANUALE = x.a.INVIO_RICEVUTA_MANUALE,
                //VAR_PREG = x.a.VAR_PREG,
                CHA_PREFERITO = x.b.CHA_PREFERITO
            }).OrderByDescending(x => x.CHA_PREFERITO == null).ThenBy(x => x.VAR_CODICE).ToList();

            query.ForEach(x =>
            {
                listaRegRf.Add(new Register() { SYSTEM_ID = x.SYSTEM_ID.ToString(), VAR_CODICE = x.VAR_CODICE.ToString() });
            });

            return listaRegRf;
        }

        private bool CheckDataValidity(NotaElenco nota, out List<string> notValidData, List<Register> listaRF)
        {
            bool validationResult = true;
            notValidData = new List<string>();

            if (String.IsNullOrEmpty(nota.codRegRf))
            {
                validationResult = false;
                notValidData.Add(Resources.ErrorRFCodeRequired);
            }

            if (String.IsNullOrEmpty(nota.descNota))
            {
                validationResult = false;
                notValidData.Add(Resources.ErrorDescRequired);
            }

            if (!listaRF.Any(x => x.SYSTEM_ID == nota.idRegRf))
            {
                validationResult = false;
                notValidData.Add(Resources.ErrorRFNotVisibleToRole);
            }

            if (!validationResult)
                notValidData.Insert(0, Resources.ErrorNotInsertedNote);

            return validationResult;
        }


        protected class Register
        {
            public string SYSTEM_ID { get; set; }
            public string VAR_CODICE { get; set; }
        }

        #endregion
    }

}
