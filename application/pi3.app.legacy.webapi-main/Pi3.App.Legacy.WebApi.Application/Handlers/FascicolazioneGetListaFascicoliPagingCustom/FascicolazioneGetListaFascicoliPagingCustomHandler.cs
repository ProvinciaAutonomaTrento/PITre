// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper.Internal;
using DocsPaVO.areaConservazione;
using DocsPaVO.Grids;
using DocsPaVO.PrjDocImport;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.ProfilazioneDinamicaLite;
using DocsPaVO.ricerche;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Office2019.Excel.ThreadedComments;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Wordprocessing;
using LinqKit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using Newtonsoft.Json;
using Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetQueryDocumentoPagingCustom;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using static Chilkat.Http;
using FascicolazioneGetListaFascicoliPagingCustomRequest = Pi3.App.Legacy.WebApi.Application.Requests.FascicolazioneGetListaFascicoliPagingCustom;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneGetListaFascicoliPagingCustom
{
    public class FascicolazioneGetListaFascicoliPagingCustomHandler : IRequestHandler<FascicolazioneGetListaFascicoliPagingCustomRequest, FascicolazioneGetListaFascicoliPagingCustomResult>
    {
        #region Public Members

        public FascicolazioneGetListaFascicoliPagingCustomHandler(
            ILogger<FascicolazioneGetListaFascicoliPagingCustomHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext pi3DbContext,
            IConfigurationService configurationService,
            ISpreadsheetService spreadsheetService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
            this._configurationService = configurationService;
            this._spreadsheetService = spreadsheetService;
        }

        public async Task<FascicolazioneGetListaFascicoliPagingCustomResult> Handle(FascicolazioneGetListaFascicoliPagingCustomRequest request, CancellationToken cancellationToken)
        {
            var output = new DocsPaVO.Grids.SearchObject[0];
            int nRec = 0;
            int numTotPage = 0;
            var idProjects = new List<SearchResultInfo>();

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, false);
                if (idTenant == 0)
                    idTenant = request.infoUtente.idAmministrazione.AsLong();

                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, false);
                if (idUser == 0)
                    idUser = request.infoUtente.idPeople.AsLong();

                var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, false);
                if (idGroup == 0)
                    idGroup = request.infoUtente.idGruppo.AsLong();

                var idRuoloInUO = await this._pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(cg => cg.ID_GRUPPO == idGroup)
                    .Select(cg => cg.SYSTEM_ID)
                    .FirstAsync();

                var query =
                    this._pi3DbContext.ProjectEntities
                        .AsNoTracking()
                        .Where(p => p.ID_AMM == idTenant
                                    && p.CHA_TIPO_PROJ == "F");

                var appendContext = new ProjectSearchAppendContext(this._pi3DbContext, query);

                await request.AppendFiltroRegistro(appendContext);
                await request.AppendFiltroSecurity(appendContext);
                await request.AppendFiltroTitolario(appendContext);
                await request.AppendFiltroCodiceClassifica(appendContext);
                await request.AppendFiltroTipoFascicolo(appendContext);
                await request.AppendFiltroStato(appendContext);
                await request.AppendFiltroAperturaIl(appendContext);
                await request.AppendFiltroAperturaPrecedenteIl(appendContext);
                await request.AppendFiltroAperturaSuccessivaAl(appendContext);
                await request.AppendFiltroAperturaSC(appendContext);
                await request.AppendFiltroAperturaMC(appendContext);
                await request.AppendFiltroAperturaToday(appendContext);
                await request.AppendFiltroChiusuraIl(appendContext);
                await request.AppendFiltroChiusuraPrecedenteIl(appendContext);
                await request.AppendFiltroChiusuraSuccessivaAl(appendContext);
                await request.AppendFiltroChiusuraSC(appendContext);
                await request.AppendFiltroChiusuraMC(appendContext);
                await request.AppendFiltroChiusuraToday(appendContext);
                await request.AppendFiltroSottoFascicolo(appendContext);
                await request.AppendFiltroTitolo(appendContext);
                await request.AppendAnnoCreazione(appendContext);
                await request.AppendNumeroFascicolo(appendContext);
                await request.AppendFiltroCreazioneIl(appendContext);
                await request.AppendFiltroCreazionePrecedenteIl(appendContext);
                await request.AppendFiltroCreazioneSuccessivaAl(appendContext);
                await request.AppendFiltroCreazioneSC(appendContext);
                await request.AppendFiltroCreazioneMC(appendContext);
                await request.AppendFiltroCreazioneIeri(appendContext);
                await request.AppendFiltroCreazioneUltimi7Giorni(appendContext);
                await request.AppendFiltroCreazioneUltimi31Giorni(appendContext);
                await request.AppendFiltroIdUOLF(appendContext);
                await request.AppendFiltroDataLFIl(appendContext);
                await request.AppendFiltroDataLFPrecedenteIl(appendContext);
                await request.AppendFiltroDataLFSuccessivaAl(appendContext);
                await request.AppendFiltroDataLFSC(appendContext);
                await request.AppendFiltroDataLFMC(appendContext);
                await request.AppendFiltroDataLFToday(appendContext);
                await request.AppendFiltroIdUOREF(appendContext);
                await request.AppendFiltroNote(appendContext);
                await request.AppendFiltroTipologiaFascicolo(appendContext);
                await request.AppendFiltroProfilazioneDinamica(appendContext);
                await request.AppendFiltroDiagrammaStatoFasc(appendContext);
                await request.AppendFiltroDocInAdlFasc(appendContext);
                await request.AppendFiltroScadenzaIl(appendContext);
                await request.AppendFiltroScadenzaPrecedenteIl(appendContext);
                await request.AppendFiltroScadenzaSuccessivaAl(appendContext);
                await request.AppendFiltroScadenzaSC(appendContext);
                await request.AppendFiltroScadenzaMC(appendContext);
                await request.AppendFiltroScadenzaToday(appendContext);
                await request.AppendFiltroIdAuthor(appendContext);
                await request.AppendFiltroIdOwner(appendContext);
                await request.AppendFiltroExport(appendContext);

                await request.AppendFiltroExcel(appendContext, this._spreadsheetService);

                query = appendContext.Query;

                var ids = await query
                   .Select(p =>
                   new
                   {
                       SYSTEM_ID = p.SYSTEM_ID,
                       CODICE = p.VAR_CODICE
                   })
                   .ToListAsync();

                if (request.getSystemIdList)
                {
                    idProjects = ids.Select(p => new SearchResultInfo()
                    {
                        Id = p.SYSTEM_ID.ToString(),
                        Codice = p.CODICE
                    })
                    .ToList();
                }

                nRec = ids.Count();

                numTotPage = nRec / request.pageSize;
                if ((nRec % request.pageSize) > 0)
                    numTotPage++;

                var maxRows = await GetConfigMaxRowsSearchable(idTenant.ToString());
                if (maxRows < nRec)
                    numTotPage = -2;
                else if (nRec > 0)
                {
                    var skip = request.numPage * request.pageSize - request.pageSize;
                    var take = request.pageSize;

                    if (request.export)
                    {
                        skip = 0;
                        take = nRec;
                    }

                    var dataQuery = query
                        .OrderBy(request, this._pi3DbContext)
                        .Skip(skip)
                        .Take(take)
                        .Select(p =>
                                AsSearchObject(
                                    p.SYSTEM_ID.ToString(),
                                    (new List<DocsPaVO.Grids.SearchObjectField>()
                                        {
                                        new DocsPaVO.Grids.SearchObjectField("P1", p.CHA_TIPO_FASCICOLO!),
                                        new DocsPaVO.Grids.SearchObjectField("ID_REGISTRO", p.ID_REGISTRO.HasValue ? p.ID_REGISTRO.ToString() : null),
                                        new DocsPaVO.Grids.SearchObjectField("P2", p.ID_PARENT.HasValue ? IPi3DbContextMappedFunctions.GetCodTit2(p.ID_PARENT.Value) : null),
                                        new DocsPaVO.Grids.SearchObjectField("P3", p.VAR_CODICE!),
                                        new DocsPaVO.Grids.SearchObjectField("P4", p.DESCRIPTION!),
                                        new DocsPaVO.Grids.SearchObjectField("P5", p.DTA_APERTURA.HasValue ? p.DTA_APERTURA.AsDateFormat() : null),
                                        new DocsPaVO.Grids.SearchObjectField("P6", p.DTA_CHIUSURA.HasValue ? p.DTA_CHIUSURA.AsDateFormat() : null),
                                        new DocsPaVO.Grids.SearchObjectField("P7", p.ID_REGISTRO.HasValue ? IPi3DbContextMappedFunctions.GetCodReg(p.ID_REGISTRO.Value) : null),
                                        new DocsPaVO.Grids.SearchObjectField("P8", IPi3DbContextMappedFunctions.GetTestoUltimaNota("F", p.SYSTEM_ID, idRuoloInUO, p.AUTHOR.Value, p.ID_RUOLO_CREATORE.Value)),
                                        new DocsPaVO.Grids.SearchObjectField("P9", p.CHA_PRIVATO),
                                        new DocsPaVO.Grids.SearchObjectField("P10", p.ID_TITOLARIO.HasValue ? IPi3DbContextMappedFunctions.GetDescTitolario(p.ID_TITOLARIO.Value) : null),
                                        new DocsPaVO.Grids.SearchObjectField("P11", p.CARTACEO),
                                        new DocsPaVO.Grids.SearchObjectField("P12", p.CHA_IN_ARCHIVIO),
                                        new DocsPaVO.Grids.SearchObjectField("P13", IPi3DbContextMappedFunctions.GetInConservazione(0, p.SYSTEM_ID, "F", idUser, idGroup).ToString()),
                                        new DocsPaVO.Grids.SearchObjectField("P14", p.NUM_FASCICOLO.HasValue ? p.NUM_FASCICOLO.ToString() : null),
                                        new DocsPaVO.Grids.SearchObjectField("P15", p.NUM_MESI_CONSERVAZIONE.HasValue ? p.NUM_MESI_CONSERVAZIONE.ToString() : null),
                                        new DocsPaVO.Grids.SearchObjectField("P16", IPi3DbContextMappedFunctions.GetDiagrammiStato(p.SYSTEM_ID, "F")),
                                        new DocsPaVO.Grids.SearchObjectField("P17", p.AUTHOR.HasValue ? IPi3DbContextMappedFunctions.GetPeopleName(p.AUTHOR.Value) : null),
                                        new DocsPaVO.Grids.SearchObjectField("P18", p.ID_RUOLO_CREATORE.HasValue ? IPi3DbContextMappedFunctions.GetDescCorr(p.ID_RUOLO_CREATORE.Value): null),
                                        new DocsPaVO.Grids.SearchObjectField("P19", p.ID_UO_CREATORE.HasValue ? IPi3DbContextMappedFunctions.GetDescCorr(p.ID_UO_CREATORE.Value) : null),
                                        new DocsPaVO.Grids.SearchObjectField("IN_ADL", IPi3DbContextMappedFunctions.GetInAdl(p.SYSTEM_ID, "F", idGroup, idUser)),
                                        new DocsPaVO.Grids.SearchObjectField("IN_ADLROLE", IPi3DbContextMappedFunctions.GetInAdl(p.SYSTEM_ID, "F", idGroup, 0)),
                                        new DocsPaVO.Grids.SearchObjectField("ID_TIPO_FASC", p.ID_TIPO_FASC.HasValue ? p.ID_TIPO_FASC.ToString() : null),
                                        new DocsPaVO.Grids.SearchObjectField("U1", p.ID_TIPO_FASC.HasValue ? IPi3DbContextMappedFunctions.GetDescTipoFasc(p.ID_TIPO_FASC.Value) : null),
                                        new DocsPaVO.Grids.SearchObjectField("P20", p.DTA_CREAZIONE.HasValue ? p.DTA_CREAZIONE.AsDateFormat() : null),
                                        new DocsPaVO.Grids.SearchObjectField("P22", p.ID_UO_LF.HasValue ? IPi3DbContextMappedFunctions.GetDescCorr(p.ID_UO_LF.Value) : null),
                                        new DocsPaVO.Grids.SearchObjectField("DTA_ADL", IPi3DbContextMappedFunctions.GetDateInADL(p.SYSTEM_ID, "F", idGroup, idUser) != null ? IPi3DbContextMappedFunctions.GetDateInADL(p.SYSTEM_ID, "F", idGroup, idUser).AsDateFormat() : null),
                                        new DocsPaVO.Grids.SearchObjectField("MOTIVO_ADL", IPi3DbContextMappedFunctions.GetMotivoADL(p.SYSTEM_ID, "F", idGroup, idUser)),
                                        new DocsPaVO.Grids.SearchObjectField("TIPOLOGIA_FASCICOLO", p.ID_PIANO_CONSERVAZIONE.HasValue ? IPi3DbContextMappedFunctions.GetTipologiaFascicoloPianoCons(p.ID_PIANO_CONSERVAZIONE.Value) : null),
                                        //AsFieldAtipicita(p.CHA_COD_T_A),
                                        new DocsPaVO.Grids.SearchObjectField("ESISTE_NOTA", IPi3DbContextMappedFunctions.EsisteNotaVisibile("F", p.SYSTEM_ID, idRuoloInUO, idUser, p.ID_RUOLO_CREATORE.Value)),
                                        new DocsPaVO.Grids.SearchObjectField("CONTATORE", IPi3DbContextMappedFunctions.GetContatoreFasc(p.SYSTEM_ID, "R")),
                                        new DocsPaVO.Grids.SearchObjectField("ISTANZECONSERVAZIONE", IPi3DbContextMappedFunctions.GetInConservazioneNoSec(0, p.SYSTEM_ID, "F")),
                                        new DocsPaVO.Grids.SearchObjectField("COD_EXT_APP", p.COD_EXT_APP),
                                        new DocsPaVO.Grids.SearchObjectField("StatoConservazione", IPi3DbContextMappedFunctions.GetStatoConservazioneFasc(p.SYSTEM_ID)),
                                        new DocsPaVO.Grids.SearchObjectField("GetValProfObjsPrjAsJson", IPi3DbContextMappedFunctions.GetValProfObjsPrjAsJson(p.SYSTEM_ID)),
                                        new DocsPaVO.Grids.SearchObjectField("ProjId", p.SYSTEM_ID.ToString()),
                                        }
                                    ).ToArray()
                                ));

                    output = await dataQuery.ToArrayAsync();

                    //CONTROLLO TEMPORANEO
                    //In caso di fasciciolo senza descrizione su DB (caso che non dovrebbe esistere, ma a volte si presenta diventando bloccante),
                    //Controllo in un for loop che non esistano casi del genre.
                    //Se esistono scambio il valore nullo in una stringa vuota (cosa che non mi lascia fare oracle nel passaggio precedente)
                    //In questo modo evito un errore in FE quando cerca di troncare il valore nullo 
                    foreach (var item in output)
                    {
                        foreach (var field in item.SearchObjectField.Where(x => x.SearchObjectFieldID == "P4" && x.SearchObjectFieldValue == null))
                        {

                            field.SearchObjectFieldValue = " ";
                        }
                    }


                    if (request.visibleFieldsTemplate != null)
                    {
                        foreach (var item in output)
                        {
                            var customObjectsAsJson = item.SearchObjectField
                                .Where(f => f.SearchObjectFieldID == "GetValProfObjsPrjAsJson")
                                .Select(f => f.SearchObjectFieldValue)
                                .First();

                            var objectFields = new List<DocsPaVO.Grids.SearchObjectField>();

                            //var objs = System.Text.Json.JsonSerializer
                            //        .Deserialize<GetValProfObjPrj[]>(CleanJson(customObjectsAsJson))!
                            //.Where(obj => request.visibleFieldsTemplate.Any(ft => ft.CustomObjectId == Convert.ToInt32(obj.id)));

                            var objs = System.Text.Json.JsonSerializer
                                    .Deserialize<GetValProfObjPrj[]>(CleanJson(customObjectsAsJson))!;

                            var projId = item.SearchObjectField
                                .Where(f => f.SearchObjectFieldID == "ProjId")
                                .Select(f => f.SearchObjectFieldValue)
                                .First();

                            foreach (var obj in objs)
                            {
                                var key = $"T{obj.id}";
                                var of = objectFields.FirstOrDefault(of => of.SearchObjectFieldID == key);
                                string value = obj.valore;
                                var objData = await this._pi3DbContext.OggettiCustomFascEntities.AsNoTracking().Where(o => o.SYSTEM_ID == obj.id.AsLong())
                                .Join(this._pi3DbContext.TipoOggettoFascEntities.AsNoTracking(), o => o.ID_TIPO_OGGETTO, t => t.SYSTEM_ID, (o, t) => new
                                {
                                    t.DESCRIZIONE,
                                    o.CHA_TIPO_TAR
                                }
                                ).FirstOrDefaultAsync();
                                var tipoOgg = objData.DESCRIZIONE;
                                var tipoCont = objData.CHA_TIPO_TAR;

                                if ("Corrispondente".Equals(tipoOgg) && !string.IsNullOrEmpty(obj.valore))
                                {
                                    var corrMatch = await this._pi3DbContext.CorrGlobaliEntities.AsNoTracking().Where(cor => cor.SYSTEM_ID == obj.valore.AsLong()).FirstOrDefaultAsync();
                                    if (corrMatch != null)
                                        value = string.Join(" - ", corrMatch.VAR_COD_RUBRICA, corrMatch.VAR_DESC_CORR);
                                }
                                else if ("CasellaDiSelezione".Equals(tipoOgg))
                                {
                                    var casSel = await this._pi3DbContext.AssTemplatesFascEntities.AsNoTracking().Where(
                                        a => a.ID_OGGETTO == obj.id.AsLong() && a.VALORE_OGGETTO_DB != null &&
                                        a.ID_PROJECT == projId
                                        ).Select(a => a.VALORE_OGGETTO_DB).ToListAsync();
                                    if (casSel.Any())
                                    {
                                        value = string.Join("; ", casSel);
                                    }
                                }
                                else if ("Contatore".Equals(tipoOgg))
                                {
                                    value = await this._pi3DbContext.AssTemplatesFascEntities.AsNoTracking().Where(a => a.ID_OGGETTO == obj.id.AsLong() && a.ID_PROJECT == projId)
                                        .Select(o => IPi3DbContextMappedFunctions.GetContatoreFasc(projId.AsLong(), tipoCont)).FirstOrDefaultAsync();
                                }
                                objectFields.Add(new DocsPaVO.Grids.SearchObjectField(key, value));
                            }

                            if (objectFields.Any())
                            {
                                item.SearchObjectField.AddRange(objectFields);
                            }

                            item.SearchObjectField.RemoveAll(f => f.SearchObjectFieldID == "GetValProfObjsPrjAsJson" || f.SearchObjectFieldID == "ProjId");
                        }
                    }
                }
            }
            catch (Pi3Exception pi3Ex)
            {
                output = null!;

                this._logger.LogError(pi3Ex, pi3Ex.Message);
            }
            catch (Exception ex)
            {
                output = null!;

                this._logger.LogCritical(ex, ex.Message);
            }

            return new FascicolazioneGetListaFascicoliPagingCustomResult(output, numTotPage, nRec, idProjects);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneGetListaFascicoliPagingCustomHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IConfigurationService _configurationService;
        protected readonly ISpreadsheetService _spreadsheetService;
        protected record GetValProfObjPrj(string id, string nome, string valore);

        private async Task<int> GetConfigMaxRowsSearchable(string idAmm)
        {

            int output = 0;
            (output, bool found) = await this._configurationService.TryGetValue<int>(idAmm, "MAX_ROW_SEARCHABLE");

            if (!found)
            {
                (output, _) = await this._configurationService.TryGetValue<int>("MAX_ROW_SEARCHABLE");
            }

            return output;
        }
        private string CleanJson(string? jsonEncodedObj)
        {
            string output = string.Empty;

            if (jsonEncodedObj == null)
                return output;

            output = jsonEncodedObj.Replace('\n', ' ');
            output = output.Replace('\t', ' ');
            output = output.Replace('\r', ' ');
            output = output.Replace('\v', ' ');

            return output;
        }

        //private static DocsPaVO.Grids.SearchObjectField AsFieldAtipicita(string value)
        //{
        //    if (!string.IsNullOrWhiteSpace(value))
        //    {
        //        return new DocsPaVO.Grids.SearchObjectField()
        //        {
        //            SearchObjectFieldID = value.Substring(1, value.Length - 6),
        //            SearchObjectFieldValue = new DocsPaVO.Security.InfoAtipicita()
        //            {
        //                CodiceAtipicita = value
        //            }
        //                        .DescrizioneAtipicita
        //        };

        //        //return AsField(value.Substring(1, value.Length - 6),
        //        //                    new DocsPaVO.Security.InfoAtipicita()
        //        //                    {
        //        //                        CodiceAtipicita = value
        //        //                    }
        //        //                    .DescrizioneAtipicita);
        //    }
        //    else
        //    {
        //        return new DocsPaVO.Grids.SearchObjectField()
        //        {
        //            SearchObjectFieldID = "P23",
        //            SearchObjectFieldValue = string.Empty
        //        };

        //        //return AsField("P23", string.Empty);
        //    }
        //}

        //private static string? FieldAsString(DateTime? value)
        //{
        //    return value.HasValue ? value.AsDateTimeFormat() : null!;
        //}

        //private static DocsPaVO.Grids.SearchObjectField AsField(string name, DateTime? value)
        //{
        //    //return AsField(name, value.HasValue ? value.AsDateTimeFormat() : null!);

        //    return new DocsPaVO.Grids.SearchObjectField()
        //    {
        //        SearchObjectFieldName = name,
        //        SearchObjectFieldValue = value.ToString() // value.HasValue ? value.AsDateTimeFormat() : null!
        //    };
        //}

        //private static DocsPaVO.Grids.SearchObjectField AsField(string name, long? value)
        //{
        //    //return AsField(name, value.HasValue ? value.ToString() : null!);
        //    return new DocsPaVO.Grids.SearchObjectField()
        //    {
        //        SearchObjectFieldName = name,
        //        SearchObjectFieldValue = value.ToString() // value.HasValue ? value.ToString() : null!
        //    };
        //}

        //private static DocsPaVO.Grids.SearchObjectField AsField(string name, string? value)
        //{
        //    return new DocsPaVO.Grids.SearchObjectField()
        //    {
        //        SearchObjectFieldName = name,
        //        SearchObjectFieldValue = value // ?? string.Empty
        //    };
        //}

        private static DocsPaVO.Grids.SearchObject AsSearchObject(
            string id,
            DocsPaVO.Grids.SearchObjectField[] fields)
        {
            return new DocsPaVO.Grids.SearchObject()
            {
                SearchObjectID = id,
                SearchObjectField = fields.ToList()
            };
        }

        #endregion
    }

    internal class ProjectSearchAppendContext
    {
        public ProjectSearchAppendContext(IPi3DbContext pi3DbContext, IQueryable<ProjectEntity> query)
        {
            Pi3DbContext = pi3DbContext;
            Query = query;
        }

        public IPi3DbContext Pi3DbContext { get; set; }

        public IQueryable<ProjectEntity> Query { get; set; }
    }

    internal static class QueryableProjectExtentions
    {
        public static IQueryable<ProjectEntity> OrderByFieldDirection<T>(this IOrderedQueryable<ProjectEntity> query,
            bool isDescending,
            Expression<Func<ProjectEntity, T>> predicate)
        {

            if (isDescending)
                query = query.ThenByDescending(predicate).ThenByDescending(p => p.DTA_CREAZIONE);
            else
            {
                query = query.ThenBy(predicate).ThenByDescending(p => p.DTA_CREAZIONE);
            }
            return query;
        }
        public static IQueryable<ProjectEntity> OrderBy(this IQueryable<ProjectEntity> query, FascicolazioneGetListaFascicoliPagingCustomRequest request, IPi3DbContext pi3DbContext)
        {
            var orderBy = request.GetValoreFiltroRicerca<string>("ORACLE_FIELD_FOR_ORDER");
            var orderDirection = request.GetValoreFiltroRicerca<string>("ORDER_DIRECTION");
            var orderByForProfiledField = request.GetValoreFiltroRicerca<string>("PROFILATION_FIELD_FOR_ORDER");
            bool directionIsDescending = DocsPaVO.Grid.Grid.OrderDirectionEnum.Desc.ToString().ToUpper() == orderDirection?.ToUpper();
            var field = request.FindFiltroRicerca("ORACLE_FIELD_FOR_ORDER");
            bool wasFieldOrderingApplied = false;
            var fieldTemp = request.visibleFieldsTemplate?.Where(e => e.CustomObjectId.ToString() == orderByForProfiledField).FirstOrDefault();
            var contatoreNoCustom = request.GetValoreFiltroRicerca<string>("CONTATORE_GRIGLIE_NO_CUSTOM");


            // CAMPI STANDARD
            if (field != null && !string.IsNullOrEmpty(orderBy))
            {
                if (!string.IsNullOrEmpty(field.nomeCampo))
                    wasFieldOrderingApplied = true;

                switch (field.nomeCampo)
                {
                    //Tipo
                    case "P1":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(a.CHA_TIPO_FASCICOLO) ? 0 : 1).OrderByFieldDirection(directionIsDescending, a => a.CHA_TIPO_FASCICOLO);
                        break;
                    //Cod Class
                    case "P2":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(IPi3DbContextMappedFunctions.GetCodTit(a.ID_PARENT.GetValueOrDefault())) ? 0 : 1)
                            .OrderByFieldDirection(directionIsDescending, a => IPi3DbContextMappedFunctions.GetCodTit(a.ID_PARENT.GetValueOrDefault()));
                        break;
                    //Codice
                    case "P3":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(a.VAR_CODICE) ? 0 : 1)
                            .OrderByFieldDirection(directionIsDescending, a => a.VAR_CODICE);
                        break;
                    //Descrizione
                    case "P4":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(a.DESCRIPTION) ? 0 : 1)
                            .OrderByFieldDirection(directionIsDescending, a => a.DESCRIPTION.ToUpper().Trim());
                        break;
                    //Apertura
                    case "P5":
                        query = query.OrderByDescending(a => null == a.DTA_APERTURA ? 0 : 1)
                            .OrderByFieldDirection(directionIsDescending, a => a.DTA_APERTURA);
                        break;
                    //Chiusura
                    case "P6":
                        query = query.OrderByDescending(a => null == a.DTA_CHIUSURA ? 0 : 1)
                            .OrderByFieldDirection(directionIsDescending, a => a.DTA_CHIUSURA);
                        break;
                    //AOO
                    case "P7":
                        query = query.OrderByDescending(a => IPi3DbContextMappedFunctions.GetCodReg(a.ID_REGISTRO.GetValueOrDefault()) == null ? 0 : 1)
                            .OrderByFieldDirection(directionIsDescending, a => IPi3DbContextMappedFunctions.GetCodReg(a.ID_REGISTRO.GetValueOrDefault()));
                        break;
                    //Privato
                    case "P9":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(a.CHA_PRIVATO) ? 0 : 1)
                            .OrderByFieldDirection(directionIsDescending, a => a.CHA_PRIVATO);
                        break;
                    //Titolario
                    case "P10":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(IPi3DbContextMappedFunctions.GetDescTitolario(a.ID_TITOLARIO.GetValueOrDefault())) ? 0 : 1)
                            .OrderByFieldDirection(directionIsDescending, a => IPi3DbContextMappedFunctions.GetDescTitolario(a.ID_TITOLARIO.GetValueOrDefault()));
                        break;
                    //Cartaceo
                    case "P11":
                        query = query.OrderByDescending(a => a.CARTACEO)
                            .OrderByFieldDirection(directionIsDescending, a => a.CARTACEO);
                        break;
                    //In archivio
                    case "P12":
                        query = query.OrderByDescending(a => a.CHA_IN_ARCHIVIO)
                            .OrderByFieldDirection(directionIsDescending, a => a.CHA_IN_ARCHIVIO);
                        break;
                    //Num fascicolo
                    case "P14":
                        query = query.OrderByDescending(a => a.NUM_FASCICOLO == null ? 0 : 1).OrderByFieldDirection(directionIsDescending, a => a.NUM_FASCICOLO);
                        break;
                    //Stato
                    case "P16":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(IPi3DbContextMappedFunctions.GetDiagrammiStato(a.SYSTEM_ID, "F")) ? 0 : 1).
                            OrderByFieldDirection(directionIsDescending, a => IPi3DbContextMappedFunctions.GetDiagrammiStato(a.SYSTEM_ID, "F"));
                        break;
                    //Nome e cognome autore
                    case "P17":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(IPi3DbContextMappedFunctions.GetPeopleName(a.AUTHOR.GetValueOrDefault())) ? 0 : 1).
                            OrderByFieldDirection(directionIsDescending, a => IPi3DbContextMappedFunctions.GetPeopleName(a.AUTHOR.GetValueOrDefault()));
                        break;
                    //Ruolo autore
                    case "P18":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(IPi3DbContextMappedFunctions.GetDescCorr(a.ID_RUOLO_CREATORE.GetValueOrDefault())) ? 0 : 1).
                            OrderByFieldDirection(directionIsDescending, a => IPi3DbContextMappedFunctions.GetDescCorr(a.ID_RUOLO_CREATORE.GetValueOrDefault()));
                        break;
                    //Uo creatore
                    case "P19":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(IPi3DbContextMappedFunctions.GetDescCorr(a.ID_UO_CREATORE.GetValueOrDefault())) ? 0 : 1).
                            OrderByFieldDirection(directionIsDescending, a => IPi3DbContextMappedFunctions.GetDescCorr(a.ID_UO_CREATORE.GetValueOrDefault()));
                        break;
                    //Data creazione
                    case "P20":
                        query = query.OrderByDescending(a => a.DTA_CREAZIONE == null ? 0 : 1).
                            OrderByFieldDirection(directionIsDescending, a => a.DTA_CREAZIONE);
                        break;
                    //Collocazione fisica
                    case "P22":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(IPi3DbContextMappedFunctions.GetDescCorr(a.ID_UO_REF.GetValueOrDefault())) ? 0 : 1).
                            OrderByFieldDirection(directionIsDescending, a => IPi3DbContextMappedFunctions.GetDescCorr(a.ID_UO_REF.GetValueOrDefault()));
                        break;
                    case "TIPOLOGIA_FASCICOLO":
                        //query = query.OrderByDescending(a => string.IsNullOrEmpty(IPi3DbContextMappedFunctions.GetTipologiaFascicoloPianoCons(a.ID_PIANO_CONSERVAZIONE.GetValueOrDefault())) ? 0 : 1).
                        //   OrderByFieldDirection(directionIsDescending, a => IPi3DbContextMappedFunctions.GetTipologiaFascicoloPianoCons(a.ID_PIANO_CONSERVAZIONE.GetValueOrDefault()));
                        break;
                    //Tipologia
                    case "U1":
                        var tempQueryTipo = query.Join(pi3DbContext.TipoFascEntities.AsNoTracking(), p => p.ID_TIPO_FASC, t => t.SYSTEM_ID, (p, t) =>
                        new
                        {
                            p,
                            t.VAR_DESC_FASC
                        });
                        if (directionIsDescending)
                            tempQueryTipo = tempQueryTipo.OrderByDescending(r => string.IsNullOrEmpty(r.VAR_DESC_FASC) ? 0 : 1).ThenByDescending(r => r.VAR_DESC_FASC).ThenByDescending(p => p.p.DTA_CREAZIONE);
                        else
                            tempQueryTipo = tempQueryTipo.OrderByDescending(r => string.IsNullOrEmpty(r.VAR_DESC_FASC) ? 0 : 1).OrderBy(r => r.VAR_DESC_FASC).ThenByDescending(p => p.p.DTA_CREAZIONE);

                        query = tempQueryTipo.Select(r => r.p);
                        break;
                }
            }
            else if (orderByForProfiledField != null)
            {

                if (!(contatoreNoCustom != null && !request.showGridPersonalization) && fieldTemp != null && fieldTemp.IsNumber)
                {
                    if (directionIsDescending)
                        query = query.OrderByDescending(a => IPi3DbContextMappedFunctions.GetValProfObjPrjOrder(a.SYSTEM_ID, orderByForProfiledField.AsLong()) == null ? -1 : 0)
                            .ThenByDescending(a => IPi3DbContextMappedFunctions.GetValProfObjPrjOrder(a.SYSTEM_ID, orderByForProfiledField.AsLong()))
                            .ThenByDescending(p => p.DTA_CREAZIONE);
                    else
                        query = query.OrderByDescending(a => IPi3DbContextMappedFunctions.GetValProfObjPrjOrder(a.SYSTEM_ID, orderByForProfiledField.AsLong()) == null ? -1 : 0)
                            .ThenBy(a => IPi3DbContextMappedFunctions.GetValProfObjPrjOrder(a.SYSTEM_ID, orderByForProfiledField.AsLong()))
                            .ThenByDescending(p => p.DTA_CREAZIONE);
                }
                else
                {
                    if (directionIsDescending)
                        query = query.OrderByDescending(a => IPi3DbContextMappedFunctions.GetValProfObjPrj(a.SYSTEM_ID, orderByForProfiledField.AsLong()) == null ? -1 : 0)
                            .ThenByDescending(a => IPi3DbContextMappedFunctions.GetValProfObjPrj(a.SYSTEM_ID, orderByForProfiledField.AsLong()))
                            .ThenByDescending(p => p.DTA_CREAZIONE);
                    else
                        query = query.OrderByDescending(a => IPi3DbContextMappedFunctions.GetValProfObjPrj(a.SYSTEM_ID, orderByForProfiledField.AsLong()) == null ? -1 : 0)
                            .ThenBy(a => IPi3DbContextMappedFunctions.GetValProfObjPrj(a.SYSTEM_ID, orderByForProfiledField.AsLong()))
                            .ThenByDescending(p => p.DTA_CREAZIONE);

                }

            }
            if (!wasFieldOrderingApplied)
                query = query.OrderBy(p => p.DTA_CREAZIONE == null)
                    .ThenByDescending(p => p.DTA_CREAZIONE);

            return query;

        }
    }

    internal static class FascicolazioneGetListaFascicoliPagingCustomRequestExtensions
    {
        public static bool HasFiltroRicerca(this FascicolazioneGetListaFascicoliPagingCustomRequest request, string argomento)
        {
            return request.listaFiltri.Any(f => f.argomento == argomento);
        }
        private static DateTime GetDateSuccessivaAl(DateTime dateTime)
        {
            var initDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);

            return (initDate);
        }

        private static DateTime GetDatePrecedenteIl(DateTime dateTime)
        {
            var initDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);

            return (initDate);
        }

        private static (DateTime, DateTime) GetDateRangeSC(DateTime dateTime)
        {
            int diff = (7 + (dateTime.DayOfWeek - DayOfWeek.Monday)) % 7;
            var date = dateTime.AddDays(-1 * diff).Date;

            var initDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            var endDate = initDate.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);

            return (initDate, endDate);
        }

        private static (DateTime, DateTime) GetDateRangeMC(DateTime dateTime)
        {
            var initDate = new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0);
            var endDate = initDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

            return (initDate, endDate);
        }

        public static DocsPaVO.filtri.FiltroRicerca? FindFiltroRicerca(this FascicolazioneGetListaFascicoliPagingCustomRequest request, string argomento)
        {
            return request.listaFiltri?
                           .Where(f => f.argomento == argomento)
                           .FirstOrDefault();
        }

        public static T GetValoreFiltroRicerca<T>(this FascicolazioneGetListaFascicoliPagingCustomRequest request, string argomento)
        {
            var filtroRicerca = FindFiltroRicerca(request, argomento);
            if (filtroRicerca != null)
            {
                if (typeof(T) == typeof(DateTime))
                    return (T)Convert.ChangeType(filtroRicerca.valore.AsDateTime(), typeof(T));
                else
                    return (T)Convert.ChangeType(filtroRicerca.valore, typeof(T));
            }
            else
                return default(T)!;
        }

        private static (DateTime, DateTime) GetDateRangeIl(DateTime dateTime)
        {
            var initDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
            var endDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);

            return (initDate, endDate);
        }
        private static async Task<List<CorrData>> GetRoleHierarchy(long? idCorrGlob, List<CorrData> roleListToReturn, IQueryable<CorrGlobaliEntity> cors)
        {
            if (cors.Any())
            {
                var corr = cors.Where(x => x.SYSTEM_ID == idCorrGlob).Select(c => new CorrData()
                {
                    SystemId = c.SYSTEM_ID,
                    IdOld = c.ID_OLD
                }).FirstOrDefault();
                if (corr != null)
                {
                    roleListToReturn.Add(corr);
                    return await GetRoleHierarchy(corr.IdOld, roleListToReturn, cors);
                }
            }
            return roleListToReturn;
        }
        private class CorrData
        {
            public long SystemId { get; set; }
            public long? IdOld { get; set; }
        }

        public static async Task AppendFiltroRegistro(
            this FascicolazioneGetListaFascicoliPagingCustomRequest request,
            ProjectSearchAppendContext context)
        {
            if (request.registro != null)
                context.Query = context.Query.Where(p => p.ID_REGISTRO == null || p.ID_REGISTRO == request.registro.systemId.AsLong());
        }

        public static async Task AppendFiltroSecurity(
            this FascicolazioneGetListaFascicoliPagingCustomRequest request,
            ProjectSearchAppendContext context)
        {
            context.Query = context.Query.Where(p => context.Pi3DbContext.SecurityEntities.AsNoTracking()
                                .Where(s => s.THING == p.SYSTEM_ID
                                        && s.ACCESSRIGHTS > 0
                                        && (s.PERSONORGROUP == request.infoUtente.idGruppo.AsLong()
                                        || s.PERSONORGROUP == request.infoUtente.idPeople.AsLong()))
                                .Select(s => s.THING)
                                .Any());
        }

        public static async Task AppendFiltroTitolario(
            this FascicolazioneGetListaFascicoliPagingCustomRequest request,
            ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("ID_TITOLARIO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var listaIdTitolari = filtro
                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(s => long.Parse(s))
                                .ToArray();

                context.Query = context.Query.Where(p => listaIdTitolari.Contains(p.ID_TITOLARIO.Value));
            }
        }

        public static async Task AppendFiltroCodiceClassifica(
            this FascicolazioneGetListaFascicoliPagingCustomRequest request,
            ProjectSearchAppendContext context)
        {
            if (request.classificazione != null && !string.IsNullOrWhiteSpace(request.classificazione.varcodliv1))
            {
                var idTenant = request.infoUtente.idAmministrazione.AsLong();

                context.Query = context.Query.Where(p =>
                            context.Pi3DbContext.ProjectEntities.AsNoTracking()
                                .Where(p2 => context.Pi3DbContext.SecurityEntities.AsNoTracking()
                                        .Where(s => s.THING == p2.SYSTEM_ID
                                                && s.ACCESSRIGHTS > 0
                                                && (s.PERSONORGROUP == request.infoUtente.idGruppo.AsLong()
                                                || s.PERSONORGROUP == request.infoUtente.idPeople.AsLong()))
                                        .Select(s => s.THING)
                                        .Any()
                                        && p2.CHA_TIPO_PROJ == "T"
                                        && p2.SYSTEM_ID == p.ID_PARENT
                                        && (request.childs ?
                                                EF.Functions.Like(p2.VAR_COD_LIV1, $"{request.classificazione.varcodliv1}%")
                                                : p2.VAR_COD_LIV1 == request.classificazione.varcodliv1)
                                        && (!p2.ID_REGISTRO.HasValue || p2.ID_REGISTRO == request.registro.systemId.AsLong())
                                        && p2.ID_AMM == idTenant)
                                .Any());
            }
        }

        //"SQL_FIELD_FOR_ORDER"

        public static async Task AppendFiltroStato(
                this FascicolazioneGetListaFascicoliPagingCustomRequest request,
                ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("STATO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(p => p.CHA_STATO == filtro);
            }
        }

        public static async Task AppendFiltroTipoFascicolo(
                this FascicolazioneGetListaFascicoliPagingCustomRequest request,
                ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("TIPO_FASCICOLO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(p => p.CHA_TIPO_FASCICOLO == filtro);
            }
        }

        public static async Task AppendFiltroAperturaIl(
               this FascicolazioneGetListaFascicoliPagingCustomRequest request,
               ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("APERTURA_IL");

            if (filtro > DateTime.MinValue)
            {
                var range = GetDateRangeIl(filtro);

                context.Query = context.Query.Where(p => p.DTA_APERTURA >= range.Item1 && p.DTA_APERTURA <= range.Item2);
            }
        }

        public static async Task AppendFiltroAperturaSuccessivaAl(
               this FascicolazioneGetListaFascicoliPagingCustomRequest request,
               ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("APERTURA_SUCCESSIVA_AL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDateSuccessivaAl(filtro);

                context.Query = context.Query.Where(p => p.DTA_APERTURA >= initDate);
            }
        }

        public static async Task AppendFiltroAperturaPrecedenteIl(
               this FascicolazioneGetListaFascicoliPagingCustomRequest request,
               ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("APERTURA_PRECEDENTE_IL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDatePrecedenteIl(filtro);

                context.Query = context.Query.Where(p => p.DTA_APERTURA <= initDate);
            }
        }

        public static async Task AppendFiltroAperturaSC(
               this FascicolazioneGetListaFascicoliPagingCustomRequest request,
               ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("APERTURA_SC"))
            {
                var range = GetDateRangeSC(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_APERTURA >= range.Item1 && p.DTA_APERTURA <= range.Item2);
            }
        }

        public static async Task AppendFiltroAperturaMC(
               this FascicolazioneGetListaFascicoliPagingCustomRequest request,
               ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("APERTURA_MC"))
            {
                var range = GetDateRangeMC(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_APERTURA >= range.Item1 && p.DTA_APERTURA <= range.Item2);
            }
        }

        public static async Task AppendFiltroAperturaToday(
             this FascicolazioneGetListaFascicoliPagingCustomRequest request,
             ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("APERTURA_TODAY"))
            {
                var range = GetDateRangeIl(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_APERTURA >= range.Item1 && p.DTA_APERTURA <= range.Item2);
            }
        }

        public static async Task AppendFiltroChiusuraIl(
           this FascicolazioneGetListaFascicoliPagingCustomRequest request,
           ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("CHIUSURA_IL");

            if (filtro > DateTime.MinValue)
            {
                var range = GetDateRangeIl(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_CHIUSURA >= range.Item1 && p.DTA_CHIUSURA <= range.Item2);
            }
        }

        public static async Task AppendFiltroChiusuraSuccessivaAl(
           this FascicolazioneGetListaFascicoliPagingCustomRequest request,
           ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("CHIUSURA_SUCCESSIVA_AL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDateSuccessivaAl(filtro);

                context.Query = context.Query.Where(p => p.DTA_CHIUSURA >= initDate);
            }
        }

        public static async Task AppendFiltroChiusuraPrecedenteIl(
          this FascicolazioneGetListaFascicoliPagingCustomRequest request,
          ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("CHIUSURA_PRECEDENTE_IL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDatePrecedenteIl(filtro);

                context.Query = context.Query.Where(p => p.DTA_CHIUSURA <= initDate);
            }
        }

        public static async Task AppendFiltroChiusuraSC(
             this FascicolazioneGetListaFascicoliPagingCustomRequest request,
             ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("CHIUSURA_SC"))
            {
                var range = GetDateRangeSC(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_CHIUSURA >= range.Item1 && p.DTA_CHIUSURA <= range.Item2);
            }
        }

        public static async Task AppendFiltroChiusuraMC(
               this FascicolazioneGetListaFascicoliPagingCustomRequest request,
               ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("CHIUSURA_MC"))
            {
                var range = GetDateRangeMC(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_CHIUSURA >= range.Item1 && p.DTA_CHIUSURA <= range.Item2);
            }
        }

        public static async Task AppendFiltroChiusuraToday(
           this FascicolazioneGetListaFascicoliPagingCustomRequest request,
           ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("CHIUSURA_TODAY"))
            {
                var range = GetDateRangeIl(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_CHIUSURA >= range.Item1 && p.DTA_CHIUSURA <= range.Item2);
            }
        }

        public static async Task AppendFiltroSottoFascicolo(
                this FascicolazioneGetListaFascicoliPagingCustomRequest request,
                ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("SOTTOFASCICOLO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                //AND A.SYSTEM_ID in (select id_fascicolo from project where CHA_TIPO_PROJ = 'C' AND UPPER(description) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + " % ' and id_fascicolo != id_parent)"

                //var sottofascicoli = 
                context.Query = context.Query.Where(p => context.Pi3DbContext.ProjectEntities.AsNoTracking()
                                                .Where(p2 => p2.CHA_TIPO_PROJ == "C"
                                                        && EF.Functions.Like(p2.DESCRIPTION!.ToUpper(), $"%{filtro.ToUpper()}%")
                                                        && p2.ID_FASCICOLO != p2.ID_PARENT
                                                        && p.SYSTEM_ID == p2.ID_FASCICOLO)
                                                .Any());
            }
        }

        public static async Task AppendFiltroTitolo(
                this FascicolazioneGetListaFascicoliPagingCustomRequest request,
                ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("TITOLO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                foreach (var l in filtro.Split("&&"))
                    context.Query = context.Query.Where(p => EF.Functions.Like(p.DESCRIPTION!.ToUpper(), $"%{l.ToUpper()}%"));

                //context.Query = context.Query.Where(p => p.DESCRIPTION!.ToUpper().Contains(filtro.ToUpper()));
            }
        }

        public static async Task AppendAnnoCreazione(
              this FascicolazioneGetListaFascicoliPagingCustomRequest request,
              ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("ANNO_FASCICOLO");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.ANNO_CREAZIONE == filtro);
            }
        }

        public static async Task AppendNumeroFascicolo(
              this FascicolazioneGetListaFascicoliPagingCustomRequest request,
              ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("NUMERO_FASCICOLO");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.NUM_FASCICOLO == filtro);
            }
        }

        public static async Task AppendFiltroCreazioneIl(
          this FascicolazioneGetListaFascicoliPagingCustomRequest request,
          ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("CREAZIONE_IL");

            if (filtro > DateTime.MinValue)
            {
                var range = GetDateRangeIl(filtro);

                context.Query = context.Query.Where(p => p.DTA_CREAZIONE >= range.Item1 && p.DTA_CREAZIONE <= range.Item2);
            }
        }

        public static async Task AppendFiltroCreazioneSuccessivaAl(
            this FascicolazioneGetListaFascicoliPagingCustomRequest request,
            ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("CREAZIONE_SUCCESSIVA_AL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDateSuccessivaAl(filtro);

                context.Query = context.Query.Where(p => p.DTA_CREAZIONE >= initDate);
            }
        }

        public static async Task AppendFiltroCreazionePrecedenteIl(
           this FascicolazioneGetListaFascicoliPagingCustomRequest request,
           ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("CREAZIONE_PRECEDENTE_IL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDatePrecedenteIl(filtro);

                context.Query = context.Query.Where(p => p.DTA_CREAZIONE <= initDate);
            }
        }

        public static async Task AppendFiltroCreazioneSC(
           this FascicolazioneGetListaFascicoliPagingCustomRequest request,
           ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("CREAZIONE_SC"))
            {
                var range = GetDateRangeSC(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_CREAZIONE >= range.Item1 && p.DTA_CREAZIONE <= range.Item2);
            }
        }

        public static async Task AppendFiltroCreazioneMC(
             this FascicolazioneGetListaFascicoliPagingCustomRequest request,
             ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("CREAZIONE_MC"))
            {
                var range = GetDateRangeMC(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_CREAZIONE >= range.Item1 && p.DTA_CREAZIONE <= range.Item2);
            }
        }

        public static async Task AppendFiltroCreazioneToday(
                   this FascicolazioneGetListaFascicoliPagingCustomRequest request,
                   ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("CREAZIONE_TODAY"))
            {
                var range = GetDateRangeIl(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_CREAZIONE >= range.Item1 && p.DTA_CREAZIONE <= range.Item2);
            }
        }

        public static async Task AppendFiltroCreazioneIeri(
                   this FascicolazioneGetListaFascicoliPagingCustomRequest request,
                   ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("CREAZIONE_IERI"))
            {
                var range = GetDateRangeIl(DateTime.Now.AddDays(-1));

                context.Query = context.Query.Where(p => p.DTA_CREAZIONE >= range.Item1 && p.DTA_CREAZIONE <= range.Item2);
            }
        }

        public static async Task AppendFiltroCreazioneUltimi7Giorni(
                  this FascicolazioneGetListaFascicoliPagingCustomRequest request,
                  ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("CREAZIONE_ULTIMI_SETTE_GIORNI"))
            {
                var initDate = GetDateSuccessivaAl(DateTime.Now.AddDays(-7));

                context.Query = context.Query.Where(p => p.DTA_CREAZIONE >= initDate);
            }
        }

        public static async Task AppendFiltroCreazioneUltimi31Giorni(
                 this FascicolazioneGetListaFascicoliPagingCustomRequest request,
                 ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("CREAZIONE_ULTMI_TRENTUNO_GIORNI"))
            {
                var initDate = GetDateSuccessivaAl(DateTime.Now.AddDays(-31));

                context.Query = context.Query.Where(p => p.DTA_CREAZIONE >= initDate);
            }
        }

        public static async Task AppendFiltroIdUOLF(
                 this FascicolazioneGetListaFascicoliPagingCustomRequest request,
                 ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("ID_UO_LF");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.ID_UO_LF == filtro);
            }
        }

        public static async Task AppendFiltroDataLFIl(
         this FascicolazioneGetListaFascicoliPagingCustomRequest request,
         ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_LF_IL");

            if (filtro > DateTime.MinValue)
            {
                var currentDate = filtro;
                var initDate = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0);
                var endDate = initDate.AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_UO_LF >= initDate && p.DTA_UO_LF <= endDate);
            }
        }

        public static async Task AppendFiltroDataLFPrecedenteIl(
             this FascicolazioneGetListaFascicoliPagingCustomRequest request,
             ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_LF_PRECEDENTE_IL");

            if (filtro > DateTime.MinValue)
            {
                var date = filtro;
                var beginDate = date.AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_UO_LF <= beginDate);
            }
        }

        public static async Task AppendFiltroDataLFSuccessivaAl(
            this FascicolazioneGetListaFascicoliPagingCustomRequest request,
            ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_LF_SUCCESSIVA_AL");

            if (filtro > DateTime.MinValue)
            {
                var currentDate = filtro;
                var beginDate = currentDate.AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_UO_LF > beginDate);
            }
        }

        public static async Task AppendFiltroDataLFSC(
          this FascicolazioneGetListaFascicoliPagingCustomRequest request,
          ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_LF_SC"))
            {
                var currentDate = DateTime.Now;
                int diff = (7 + (currentDate.DayOfWeek - DayOfWeek.Monday)) % 7;
                var date = currentDate.AddDays(-1 * diff).Date;

                var firstDayOfWeek = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                var lastDayOfWeek = firstDayOfWeek.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_UO_LF >= firstDayOfWeek && p.DTA_UO_LF <= lastDayOfWeek);
            }
        }

        public static async Task AppendFiltroDataLFMC(
              this FascicolazioneGetListaFascicoliPagingCustomRequest request,
              ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_LF_MC"))
            {
                var currentDate = DateTime.Now;
                var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_UO_LF >= firstDayOfMonth && p.DTA_UO_LF <= lastDayOfMonth);
            }
        }

        public static async Task AppendFiltroDataLFToday(
                  this FascicolazioneGetListaFascicoliPagingCustomRequest request,
                  ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_LF_TODAY"))
            {
                var currentDate = DateTime.Now;
                var initDate = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0);
                var endDate = initDate.AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_UO_LF >= initDate && p.DTA_UO_LF <= endDate);
            }
        }

        public static async Task AppendFiltroIdUOREF(
                 this FascicolazioneGetListaFascicoliPagingCustomRequest request,
                 ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("ID_UO_REF");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.ID_UO_REF == filtro);
            }
        }

        public static async Task AppendFiltroNote(
                 this FascicolazioneGetListaFascicoliPagingCustomRequest request,
                 ProjectSearchAppendContext context)
        {
            // TODO            
        }

        public static async Task AppendFiltroTipologiaFascicolo(
                 this FascicolazioneGetListaFascicoliPagingCustomRequest request,
                 ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("TIPOLOGIA_FASCICOLO");

            if (filtro > 0)
            {
                if (await context.Pi3DbContext.TipoFascEntities.AsNoTracking()
                    .AnyAsync(t => t.SYSTEM_ID == filtro && t.IPERFASCICOLO != 1))
                {
                    context.Query = context.Query.Where(p => p.ID_TIPO_FASC == filtro);
                }
            }
        }

        public static async Task AppendFiltroProfilazioneDinamica(
              this FascicolazioneGetListaFascicoliPagingCustomRequest request,
              ProjectSearchAppendContext context)
        {
            var filtro = request.FindFiltroRicerca("PROFILAZIONE_DINAMICA");
            bool firstFilter = true;
            bool wasPredUsed = false;

            if (filtro != null && filtro.template != null)
            {
                if (filtro.template.ELENCO_OGGETTI.Any())
                {
                    foreach (var oggetto in filtro.template.ELENCO_OGGETTI)
                    {
                        switch (oggetto.TIPO.DESCRIZIONE_TIPO)
                        {
                            case "CampoDiTesto":
                                if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                {
                                    context.Query = context.Query.Where(t =>
                                            context.Pi3DbContext.AssTemplatesFascEntities.AsNoTracking()
                                                .Any(at => at.ID_PROJECT != null && Convert.ToInt64(at.ID_PROJECT) == t.SYSTEM_ID
                                                    && at.ID_OGGETTO == oggetto.SYSTEM_ID
                                                    && (oggetto.TIPO_RICERCA_STRINGA == DocsPaVO.ProfilazioneDinamica.TipoRicercaStringaEnum.PAROLA_INTERA ?
                                                            at.VALORE_OGGETTO_DB.ToUpper() == oggetto.VALORE_DATABASE.ToUpper()
                                                            : (oggetto.TIPO_RICERCA_STRINGA == DocsPaVO.ProfilazioneDinamica.TipoRicercaStringaEnum.PARTE_DELLA_PAROLA ?
                                                                 EF.Functions.Like(at.VALORE_OGGETTO_DB.ToUpper(), $"%{oggetto.VALORE_DATABASE.ToUpper()}%")
                                                                : EF.Functions.Like(at.VALORE_OGGETTO_DB.ToUpper(), $"%{oggetto.VALORE_DATABASE.ToUpper()}")))));
                                }
                                break;
                            case "CasellaDiSelezione":
                                foreach (var casella in oggetto.VALORI_SELEZIONATI.Where(v => !string.IsNullOrEmpty(v)).ToList())
                                {
                                    context.Query = context.Query.Where(t =>
                                            context.Pi3DbContext.AssTemplatesFascEntities.AsNoTracking()
                                                .Any(at => at.ID_PROJECT != null && Convert.ToInt64(at.ID_PROJECT) == t.SYSTEM_ID
                                                        && at.ID_OGGETTO == oggetto.SYSTEM_ID
                                                        && at.VALORE_OGGETTO_DB.ToUpper() == casella.ToUpper()));
                                }
                                break;
                            case "MenuATendina":
                                if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                {
                                    context.Query = context.Query.Where(t =>
                                            context.Pi3DbContext.AssTemplatesFascEntities.AsNoTracking()
                                                .Any(at => at.ID_PROJECT != null && Convert.ToInt64(at.ID_PROJECT) == t.SYSTEM_ID
                                                        && at.ID_OGGETTO == oggetto.SYSTEM_ID
                                                        && at.VALORE_OGGETTO_DB.ToUpper() == oggetto.VALORE_DATABASE.ToUpper()));
                                }
                                break;
                            case "SelezioneEsclusiva":
                                if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                {
                                    context.Query = context.Query.Where(t =>
                                            context.Pi3DbContext.AssTemplatesFascEntities.AsNoTracking()
                                                .Any(at => at.ID_PROJECT != null && Convert.ToInt64(at.ID_PROJECT) == t.SYSTEM_ID
                                                        && at.ID_OGGETTO == oggetto.SYSTEM_ID
                                                        && at.VALORE_OGGETTO_DB.ToUpper() == oggetto.VALORE_DATABASE.ToUpper()));
                                }
                                break;
                            case "Contatore":
                                IQueryable<AssTemplatesFascEntity> associazioneTemplatesQueryable = null;
                                if (!string.IsNullOrEmpty(oggetto.DATA_INSERIMENTO) ||
                                    !string.IsNullOrEmpty(oggetto.VALORE_DATABASE) ||
                                    !string.IsNullOrEmpty(oggetto.ID_AOO_RF))
                                {
                                    associazioneTemplatesQueryable = context.Pi3DbContext.AssTemplatesFascEntities.AsNoTracking()
                                        .Where(at => at.ID_OGGETTO == oggetto.SYSTEM_ID);
                                }

                                if (!string.IsNullOrEmpty(oggetto.DATA_INSERIMENTO))
                                {
                                    if (oggetto.DATA_INSERIMENTO.IndexOf('@') != -1)
                                    {
                                        string[] dataInserimento = oggetto.DATA_INSERIMENTO.Split('@');
                                        var initDate = GetDateSuccessivaAl(dataInserimento[0].AsDateTime());
                                        var endDate = GetDatePrecedenteIl(dataInserimento[1].AsDateTime());

                                        associazioneTemplatesQueryable = associazioneTemplatesQueryable.Where(a => a.DTA_INS >= initDate && a.DTA_INS <= endDate);
                                    }
                                    else
                                    {
                                        var range = GetDateRangeIl(oggetto.DATA_INSERIMENTO.AsDateTime());
                                        associazioneTemplatesQueryable = associazioneTemplatesQueryable.Where(a => a.DTA_INS >= range.Item1 && a.DTA_INS <= range.Item2);
                                    }
                                }

                                if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                {
                                    if (oggetto.VALORE_DATABASE.IndexOf('@') != -1)
                                    {
                                        string[] contatore = oggetto.VALORE_DATABASE.Split('@');
                                        var init = contatore[0].AsLong();
                                        var end = contatore[1].AsLong();

                                        associazioneTemplatesQueryable = associazioneTemplatesQueryable
                                            .Where(a => a.VALORE_OGGETTO_DB != null && Convert.ToInt64(a.VALORE_OGGETTO_DB) >= init
                                                    && Convert.ToInt64(a.VALORE_OGGETTO_DB) <= end);
                                    }
                                    else
                                    {
                                        associazioneTemplatesQueryable = associazioneTemplatesQueryable
                                            .Where(a => Convert.ToInt64(a.VALORE_OGGETTO_DB) >= oggetto.VALORE_DATABASE.AsLong());
                                    }
                                }

                                if (oggetto.TIPO_CONTATORE != "T" && !string.IsNullOrEmpty(oggetto.ID_AOO_RF) && oggetto.ID_AOO_RF != "0")
                                {
                                    associazioneTemplatesQueryable = associazioneTemplatesQueryable
                                            .Where(a => a.ID_AOO_RF == oggetto.ID_AOO_RF.AsLong());
                                }

                                if (associazioneTemplatesQueryable != null)
                                {
                                    context.Query = context.Query.Where(t => associazioneTemplatesQueryable
                                        .Any(at => at.ID_PROJECT != null && Convert.ToInt64(at.ID_PROJECT) == t.SYSTEM_ID));
                                }
                                break;
                            case "Data":
                                if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                {
                                    DateTime? init = null;
                                    DateTime? end = null;
                                    if (oggetto.VALORE_DATABASE.IndexOf('@') != -1)
                                    {
                                        string[] dataInserimento = oggetto.VALORE_DATABASE.Split('@');
                                        init = GetDateSuccessivaAl(dataInserimento[0].AsDateTime());
                                        end = GetDatePrecedenteIl(dataInserimento[1].AsDateTime());
                                    }
                                    else
                                    {
                                        var range = GetDateRangeIl(oggetto.VALORE_DATABASE.AsDateTime());
                                        init = range.Item1;
                                        end = range.Item2;
                                    }

                                    context.Query = context.Query.Where(t =>
                                            context.Pi3DbContext.AssTemplatesFascEntities.AsNoTracking()
                                                .Any(at => at.ID_PROJECT != null && at.ID_PROJECT == t.SYSTEM_ID.ToString() 
                                                        && at.VALORE_OGGETTO_DB != null
                                                        && IPi3DbContextMappedFunctions.CompareDate(at.VALORE_OGGETTO_DB, init.Value, end) == 1));
                                }
                                break;
                            case "Corrispondente":
                                if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                {
                                    long idCorrGlobali = 0;
                                    if (long.TryParse(oggetto.VALORE_DATABASE, out idCorrGlobali))
                                    {
                                        var roles = new List<long>() { idCorrGlobali };
                                        if (oggetto.ESTENDI_STORICIZZATI)
                                        {
                                            roles = (await GetRoleHierarchy(idCorrGlobali, new(), context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()))?.Select(c => c.SystemId).ToList();
                                        }

                                        context.Query = context.Query.Where(t =>
                                            context.Pi3DbContext.AssTemplatesFascEntities.AsNoTracking()
                                                .Any(at => at.ID_PROJECT != null && at.ID_PROJECT == t.SYSTEM_ID.ToString()
                                                        && at.ID_OGGETTO == oggetto.SYSTEM_ID && at.VALORE_OGGETTO_DB != null
                                                        && roles.Contains(Convert.ToInt64(at.VALORE_OGGETTO_DB))));
                                    }
                                    else
                                    {
                                        var corrQueryable = context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                                            .Where(c => EF.Functions.Like(c.VAR_DESC_CORR.ToUpper(), $"%{oggetto.VALORE_DATABASE.ToUpper()}%"));

                                        context.Query = context.Query.Where(t =>
                                            context.Pi3DbContext.AssTemplatesFascEntities.AsNoTracking()
                                                .Any(at => at.ID_PROJECT != null && at.ID_PROJECT == t.SYSTEM_ID.ToString()
                                                        && at.ID_OGGETTO == oggetto.SYSTEM_ID && at.VALORE_OGGETTO_DB != null
                                                        && corrQueryable.Any(c => c.SYSTEM_ID == Convert.ToInt64(at.VALORE_OGGETTO_DB))));
                                    }
                                }
                                break;
                            case "ContatoreSottocontatore":
                                break;
                            case "OggettoEsterno":
                                break;

                        }
                    }

                }

            }
        }


        public static async Task AppendFiltroDiagrammaStatoFasc(
              this FascicolazioneGetListaFascicoliPagingCustomRequest request,
              ProjectSearchAppendContext context)
        {
            var filtro = request.FindFiltroRicerca("DIAGRAMMA_STATO_FASC");

            if (filtro != null)
            {
                context.Query = context.Query.Where(
                    p => context.Pi3DbContext.DiagrammiEntities.AsNoTracking()
                            .Where(d =>
                                (d.ID_PROJECT == p.SYSTEM_ID
                                && (!string.IsNullOrWhiteSpace(filtro.nomeCampo) && filtro.nomeCampo.ToUpper() == "UNEQUALS") ?
                                    d.ID_STATO != filtro.valore.AsLong() :
                                    d.ID_STATO == filtro.valore.AsLong()))
                            .Any());
            }
        }

        public static async Task AppendFiltroDocInAdlFasc(
                 this FascicolazioneGetListaFascicoliPagingCustomRequest request,
                 ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DOC_IN_FASC_ADL");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var pairs = filtro.Split('@');

                context.Query = context.Query.Where(p =>
                        context.Pi3DbContext.AreaLavoroEntities.AsNoTracking()
                            .Where(al => al.ID_PROJECT == p.SYSTEM_ID
                                    && al.ID_PEOPLE == pairs[0].AsLong()
                                    && al.ID_RUOLO_IN_UO == pairs[1].AsLong())
                            .Any());
            }
        }

        public static async Task AppendFiltroScadenzaIl(
           this FascicolazioneGetListaFascicoliPagingCustomRequest request,
           ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("SCADENZA_IL");

            if (filtro > DateTime.MinValue)
            {
                var currentDate = filtro;
                var initDate = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0);
                var endDate = initDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_SCADENZA >= initDate && p.DTA_SCADENZA <= endDate);
            }
        }

        public static async Task AppendFiltroScadenzaSuccessivaAl(
           this FascicolazioneGetListaFascicoliPagingCustomRequest request,
           ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("SCADENZA_SUCCESSIVA_AL");

            if (filtro > DateTime.MinValue)
            {
                var currentDate = filtro;
                var beginDate = currentDate.AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_SCADENZA >= beginDate);
            }
        }

        public static async Task AppendFiltroScadenzaPrecedenteIl(
          this FascicolazioneGetListaFascicoliPagingCustomRequest request,
          ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("SCADENZA_PRECEDENTE_IL");

            if (filtro > DateTime.MinValue)
            {
                var currentDate = filtro;
                var initDate = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0);

                context.Query = context.Query.Where(p => p.DTA_SCADENZA <= initDate);
            }
        }

        public static async Task AppendFiltroScadenzaSC(
             this FascicolazioneGetListaFascicoliPagingCustomRequest request,
             ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("SCADENZA_SC"))
            {
                var currentDate = DateTime.Now;
                int diff = (7 + (currentDate.DayOfWeek - DayOfWeek.Monday)) % 7;
                var date = currentDate.AddDays(-1 * diff).Date;

                var firstDayOfWeek = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                var lastDayOfWeek = firstDayOfWeek.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_SCADENZA >= firstDayOfWeek && p.DTA_SCADENZA <= lastDayOfWeek);
            }
        }

        public static async Task AppendFiltroScadenzaMC(
               this FascicolazioneGetListaFascicoliPagingCustomRequest request,
               ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("SCADENZA_MC"))
            {
                var currentDate = DateTime.Now;
                var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_SCADENZA >= firstDayOfMonth && p.DTA_CHIUSURA <= lastDayOfMonth);
            }
        }

        public static async Task AppendFiltroScadenzaToday(
           this FascicolazioneGetListaFascicoliPagingCustomRequest request,
           ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("SCADENZA_TODAY"))
            {
                var currentDate = DateTime.Now;
                var initDate = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0);
                var endDate = initDate.AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_SCADENZA >= initDate && p.DTA_SCADENZA <= endDate);
            }
        }

        public static async Task AppendFiltroIdAuthor(
           this FascicolazioneGetListaFascicoliPagingCustomRequest request,
           ProjectSearchAppendContext context)
        {
            var idAuthor = request.GetValoreFiltroRicerca<long>("ID_AUTHOR");

            if (idAuthor > 0)
            {
                var corrTypeAuthorId = request.GetValoreFiltroRicerca<string>("CORR_TYPE_AUTHOR");
                switch (corrTypeAuthorId)
                {
                    case "R":
                        var searchHistoricized = request.GetValoreFiltroRicerca<bool>("EXTEND_TO_HISTORICIZED_AUTHOR");
                        if (searchHistoricized)
                        {
                            var roles = (await GetRoleHierarchy(idAuthor, new(), context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()))?.Select(c => c.SystemId).ToList();
                            if (roles != null)
                            {
                                context.Query = context.Query.Where(p => p.ID_RUOLO_CREATORE != null && roles.Contains((long)p.ID_RUOLO_CREATORE));
                            }
                        }
                        else
                        {
                            context.Query = context.Query.Where(p => p.ID_RUOLO_CREATORE == idAuthor);
                        }
                        break;
                    case "P":
                        context.Query = context.Query.Where(p => context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                            .Any(c => c.SYSTEM_ID == idAuthor && p.AUTHOR == c.ID_PEOPLE));
                        break;
                    case "U":
                        context.Query = context.Query.Where(p => p.ID_UO_CREATORE == idAuthor);
                        break;
                }
            }
        }

        public static async Task AppendFiltroIdOwner(
           this FascicolazioneGetListaFascicoliPagingCustomRequest request,
           ProjectSearchAppendContext context)
        {
            var idOwner = request.GetValoreFiltroRicerca<long>("ID_OWNER");

            if (idOwner > 0)
            {
                var corrTypeOwnerId = request.GetValoreFiltroRicerca<string>("CORR_TYPE_OWNER");
                switch (corrTypeOwnerId)
                {
                    case "R":
                        context.Query = context.Query.Where(p => p.CHA_TIPO_FASCICOLO != "G" &&
                            context.Pi3DbContext.SecurityEntities.AsNoTracking()
                                .Any(s => s.THING == p.SYSTEM_ID && s.CHA_TIPO_DIRITTO == "P" &&
                                context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                                    .Any(c => c.SYSTEM_ID == idOwner && c.ID_GRUPPO == s.PERSONORGROUP)));
                        break;
                    case "P":
                        context.Query = context.Query.Where(p => p.CHA_TIPO_FASCICOLO != "G" &&
                            context.Pi3DbContext.SecurityEntities.AsNoTracking()
                                .Any(s => s.THING == p.SYSTEM_ID && s.CHA_TIPO_DIRITTO == "P" &&
                                context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                                    .Any(c => c.SYSTEM_ID == idOwner && c.ID_PEOPLE == s.PERSONORGROUP)));
                        break;
                    case "U":
                        context.Query = context.Query.Where(p => p.CHA_TIPO_FASCICOLO != "G" &&
                            context.Pi3DbContext.SecurityEntities.AsNoTracking()
                                .Any(s => s.THING == p.SYSTEM_ID && s.CHA_TIPO_DIRITTO == "P" &&
                                context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                                    .Any(c => c.ID_UO == idOwner && c.ID_GRUPPO == s.PERSONORGROUP)));
                        break;
                }
            }
        }

        public static async Task AppendFiltroExport(
             this FascicolazioneGetListaFascicoliPagingCustomRequest request,
           ProjectSearchAppendContext context)
        {
            if (request.export
                && request.documentsSystemId != null
                && request.documentsSystemId.Length > 0)
            {
                var predicate = PredicateBuilder.New<ProjectEntity>();

                foreach (var id in request.documentsSystemId.Select(id => id.AsLong()))
                    predicate = predicate.Or(p => p.SYSTEM_ID == id);

                context.Query = context.Query.Where(predicate);
            }
        }

        public static async Task AppendFiltroExcel(
             this FascicolazioneGetListaFascicoliPagingCustomRequest request,
           ProjectSearchAppendContext context,
           ISpreadsheetService spreadsheetService)
        {
            List<string> valoriAttributo = new List<string>();
            string nomeFile = request.GetValoreFiltroRicerca<string>("FILE_EXCEL");
            string nomeAttributo = request.GetValoreFiltroRicerca<string>("ATTRIBUTO_EXCEL");
            string nomeCampoExcel = nomeAttributo;
            var idTenant = request.infoUtente.idAmministrazione.AsLong();
            if (request.excelDati != null && !string.IsNullOrEmpty(nomeFile) && !string.IsNullOrEmpty(nomeAttributo))
            {
                //ricerca nel filtro il nome del fileExcel e la colonna del fileExcel da importare

                #region Elaborazione file excel
                var spreadsheetModel = await spreadsheetService.Read(new MemoryStream(request.excelDati));

                var sheetModel = spreadsheetModel.Sheets.FirstOrDefault(s => s.Name.Equals("FASCICOLI", StringComparison.InvariantCultureIgnoreCase));
                if (sheetModel == null)
                    throw new SheetNotFoundPi3Exception();

                if (nomeAttributo.StartsWith("TIPOLOGIA"))
                {
                    string[] attributi = nomeAttributo.Split('&');
                    nomeCampoExcel = attributi[3];
                }
                //Ricerca della posizione della colonna associata all'attributo selezionato
                var cells = sheetModel.Cells;
                var nomeAttributoCell = cells.FirstOrDefault(c => !string.IsNullOrWhiteSpace(c.ValueAsString) && c.ValueAsString.Equals(nomeCampoExcel));
                int posX = nomeAttributoCell.Row;
                int posY = nomeAttributoCell.Column;
                if (posX < 0 || posY < 0)
                    //throw new SheetNotFoundPi3Exception();
                    return;

                var valueList = cells.Where(c => c.Row > posX && c.Column == posY && !string.IsNullOrWhiteSpace(c.ValueAsString)).ToList();
                foreach (var v in valueList)
                {
                    var value = v.ValueAsString;
                    if (value.Equals("/"))
                        break;

                    switch (nomeAttributo.ToUpper())
                    {
                        case "DATA_APERTURA":
                            valoriAttributo.Add(value.AsDateTime().ToShortDateString());
                            break;
                        //case "NUMERO_FASCICOLO":
                        default:
                            valoriAttributo.Add(value);
                            break;
                    }
                }

                #endregion

                #region Ricerca
                if (valoriAttributo.Any())
                {
                    string[] attributiTipologia;
                    if (nomeAttributo.StartsWith("TIPOLOGIA"))
                    {
                        attributiTipologia = nomeAttributo.Split('&');
                        var iperfasc = (await context.Pi3DbContext.TipoFascEntities.Where(x => x.SYSTEM_ID == attributiTipologia[2].AsLong()).Select(x => x.IPERFASCICOLO).FirstOrDefaultAsync()).Equals(1);

                        var idTipoOggetto = attributiTipologia[4].AsLong();
                        var tipoOggetto = await context.Pi3DbContext.TipoOggettoFascEntities
                            .Join(context.Pi3DbContext.OggettiCustomFascEntities, t => t.SYSTEM_ID, o => o.ID_TIPO_OGGETTO, (t, o) => new { t, o })
                            .Where(x => x.o.SYSTEM_ID == idTipoOggetto).Select(x => x.t.TIPO).FirstOrDefaultAsync();

                        if (tipoOggetto.Equals("Corrispondente"))
                        {
                            var roles = new List<long>();
                            foreach (var v in valoriAttributo)
                            {
                                var idCorrGlobaliList = await context.Pi3DbContext.CorrGlobaliEntities.Where(x => x.VAR_COD_RUBRICA.Equals(v) && (x.ID_AMM == null || x.ID_AMM == idTenant)).Select(x => x.SYSTEM_ID).ToListAsync();

                                if (idCorrGlobaliList == null || !idCorrGlobaliList.Any()) //se non trovo i corrispondent non li inserisco in lista
                                    continue;

                                foreach (var idCorrGlobali in idCorrGlobaliList)
                                {
                                    roles.Add(idCorrGlobali);
                                    roles.AddRange((await GetRoleHierarchy(idCorrGlobali, new(), context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()))?.Select(c => c.SystemId).ToList());
                                }
                            }

                            context.Query = context.Query.Where(t => 
                                iperfasc ? 
                                    context.Pi3DbContext.AssTemplatesFascEntities.AsNoTracking()
                                        .Any(at => at.ID_PROJECT != null && at.ID_PROJECT == t.SYSTEM_ID.ToString()
                                                && at.ID_OGGETTO == attributiTipologia[4].AsLong() && at.VALORE_OGGETTO_DB != null
                                                && roles.Contains(Convert.ToInt64(at.VALORE_OGGETTO_DB))) :
                                    t.ID_TIPO_FASC == attributiTipologia[2].AsLong() && context.Pi3DbContext.AssTemplatesFascEntities.AsNoTracking()
                                        .Any(at => at.ID_PROJECT != null && at.ID_PROJECT == t.SYSTEM_ID.ToString()
                                                && at.ID_OGGETTO == attributiTipologia[4].AsLong() && at.VALORE_OGGETTO_DB != null
                                                && roles.Contains(Convert.ToInt64(at.VALORE_OGGETTO_DB))));

                        }
                        else
                        {
                            context.Query = context.Query.Where(t =>
                                iperfasc ?
                                    context.Pi3DbContext.AssTemplatesFascEntities.AsNoTracking()
                                        .Any(at => at.ID_PROJECT != null && at.ID_PROJECT == t.SYSTEM_ID.ToString()
                                            && at.ID_OGGETTO == attributiTipologia[4].AsLong()
                                            && valoriAttributo.Contains(at.VALORE_OGGETTO_DB)) :
                                    t.ID_TIPO_FASC == attributiTipologia[2].AsLong() && context.Pi3DbContext.AssTemplatesFascEntities.AsNoTracking()
                                        .Any(at => at.ID_PROJECT != null && at.ID_PROJECT == t.SYSTEM_ID.ToString()
                                            && at.ID_OGGETTO == attributiTipologia[4].AsLong()
                                            && valoriAttributo.Contains(at.VALORE_OGGETTO_DB)));
                        }

                    }
                    else
                    {
                        switch (nomeAttributo)
                        {
                            case "NUMERO_FASCICOLO":
                                //queryString += " and A.NUM_FASCICOLO IN ( " + valoriAttributi + ") ";
                                context.Query = context.Query.Where(t => valoriAttributo.Contains(t.NUM_FASCICOLO.ToString()));
                                break;
                            case "DATA_APERTURA":
                                //queryString += " and to_char(a.DTA_APERTURA, 'dd/mm/yyyy') IN ( " + valoriAttributi + ") ";
                                context.Query = context.Query.Where(t => valoriAttributo.Contains(t.DTA_APERTURA.AsDateFormat()));
                                break;
                            case "DESCRIZIONE_FASCICOLO":
                                //queryString += " and a.DESCRIPTION IN ( " + valoriAttributi + ") ";
                                context.Query = context.Query.Where(t => valoriAttributo.Contains(t.DESCRIPTION));
                                break;
                            case "CODICE_NODO":
                                //queryString += " and A.VAR_CODICE IN ( " + valoriAttributi + ") ";
                                context.Query = context.Query.Where(t => valoriAttributo.Contains(t.VAR_CODICE));
                                break;
                        }
                    }
                }

                #endregion
            }

        }
    }

}