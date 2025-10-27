// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.ricerche;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Services.Configuration;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.FascicolazioneGetListaFascicoliPagingCustom
{
    public class FascicolazioneGetListaFascicoliPagingCustomCommandHandler : IRequestHandler<FascicolazioneGetListaFascicoliPagingCustomCommand, FascicolazioneGetListaFascicoliPagingCustomCommandResponse>
    {
        #region Public Members
        public FascicolazioneGetListaFascicoliPagingCustomCommandHandler(
            ILogger<FascicolazioneGetListaFascicoliPagingCustomCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext pi3DbContext,
            IConfigurationService configurationService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
            this._configurationService = configurationService;
        }


        public async Task<FascicolazioneGetListaFascicoliPagingCustomCommandResponse> Handle(FascicolazioneGetListaFascicoliPagingCustomCommand request, CancellationToken cancellationToken)
        {
            var output = new DocsPaVO.Grids.SearchObject[0];
            int nRec = 0;
            int numTotPage = 0;
            var idProjects = new List<SearchResultInfo>();

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
                var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
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
                await request.AppendFiltroCodiceFascicolo(appendContext);
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

                query = appendContext.Query;

                nRec = await query
                    .Select(p => p.SYSTEM_ID)
                    .CountAsync();

                numTotPage = nRec / request.pageSize;
                if ((nRec % request.pageSize) > 0)
                    numTotPage++;

                var maxRows = await GetConfigMaxRowsSearchable(idTenant.ToString());
                if (maxRows < nRec)
                    numTotPage = -2;
                else if (nRec > 0)
                {
                    var dataQuery = query
                        .OrderBy(request, this._pi3DbContext)
                        .Skip(request.numPage * request.pageSize - request.pageSize)
                        .Take(request.pageSize)
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
                                        AsFieldAtipicita(p.CHA_COD_T_A),
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

                    if (request.visibleFieldsTemplate != null)
                    {
                        foreach (var item in output)
                        {
                            var customObjectsAsJson = item.SearchObjectField
                                .Where(f => f.SearchObjectFieldID == "GetValProfObjsPrjAsJson")
                                .Select(f => f.SearchObjectFieldValue)
                                .First();

                            var objectFields = new List<DocsPaVO.Grids.SearchObjectField>();

                            var objs = System.Text.Json.JsonSerializer
                                    .Deserialize<GetValProfObjPrj[]>(CleanJson(customObjectsAsJson))!
                                    .Where(obj => request.visibleFieldsTemplate.Any(ft => ft.CustomObjectId == Convert.ToInt32(obj.id)));

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

                    if (request.getSystemIdList)
                    {
                        idProjects = query.Select(p => new
                        {
                            p.VAR_CODICE,
                            p.SYSTEM_ID
                        }).Select(p => new SearchResultInfo()
                        {
                            Id = p.SYSTEM_ID.ToString(),
                            Codice = p.VAR_CODICE
                        })
                        .ToList();
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

            return new(output, numTotPage, nRec, idProjects);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneGetListaFascicoliPagingCustomCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IConfigurationService _configurationService;
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

        private static DocsPaVO.Grids.SearchObjectField AsFieldAtipicita(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return new DocsPaVO.Grids.SearchObjectField()
                {
                    SearchObjectFieldID = value.Substring(1, value.Length - 6),
                    SearchObjectFieldValue = new DocsPaVO.Security.InfoAtipicita()
                    {
                        CodiceAtipicita = value
                    }
                                .DescrizioneAtipicita
                };

                //return AsField(value.Substring(1, value.Length - 6),
                //                    new DocsPaVO.Security.InfoAtipicita()
                //                    {
                //                        CodiceAtipicita = value
                //                    }
                //                    .DescrizioneAtipicita);
            }
            else
            {
                return new DocsPaVO.Grids.SearchObjectField()
                {
                    SearchObjectFieldID = "P23",
                    SearchObjectFieldValue = string.Empty
                };

                //return AsField("P23", string.Empty);
            }
        }

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
        public static IQueryable<ProjectEntity> OrderBy(this IQueryable<ProjectEntity> query, FascicolazioneGetListaFascicoliPagingCustomCommand request, IPi3DbContext pi3DbContext)
        {
            var orderBy = request.GetValoreFiltroRicerca<string>("ORACLE_FIELD_FOR_ORDER");
            var orderDirection = request.GetValoreFiltroRicerca<string>("ORDER_DIRECTION");
            var orderByForProfiledField = request.GetValoreFiltroRicerca<string>("PROFILATION_FIELD_FOR_ORDER");
            bool directionIsDescending = DocsPaVO.Grid.Grid.OrderDirectionEnum.Desc.ToString() == orderDirection;
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
            {
                if (!string.IsNullOrEmpty(orderDirection))
                {
                    if (orderDirection == "DESC")
                        query = query.OrderByDescending(p => p.DTA_CREAZIONE == null ? -1 : 0).ThenByDescending(p => p.DTA_CREAZIONE);
                    else
                        query = query.OrderByDescending(p => p.DTA_CREAZIONE == null ? -1 : 0).ThenBy(p => p.DTA_CREAZIONE);
                }
                else
                {
                    query = query.OrderByDescending(p => p.DTA_CREAZIONE);
                }
            }




            return query;

        }
    }

    internal static class FascicolazioneGetListaFascicoliPagingCustomCommandExtensions
    {
        public static bool HasFiltroRicerca(this FascicolazioneGetListaFascicoliPagingCustomCommand request, string argomento)
        {
            return request.listaFiltri.Any(f => f.argomento == argomento);
        }
        private static DateTime GetDateSuccessivaAl(DateTime dateTime)
        {
            var initDate = dateTime.AddHours(23).AddMinutes(59).AddSeconds(59);

            return (initDate);
        }
        public static DocsPaVO.filtri.FiltroRicerca? FindFiltroRicerca(this FascicolazioneGetListaFascicoliPagingCustomCommand request, string argomento)
        {
            return request.listaFiltri?
                           .Where(f => f.argomento == argomento)
                           .FirstOrDefault();
        }

        public static T GetValoreFiltroRicerca<T>(this FascicolazioneGetListaFascicoliPagingCustomCommand request, string argomento)
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
            var endDate = initDate.AddHours(23).AddMinutes(59).AddSeconds(59);

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
            this FascicolazioneGetListaFascicoliPagingCustomCommand request,
            ProjectSearchAppendContext context)
        {
            if (request.registro != null)
            {
                context.Query = context.Query.Where(p => p.ID_REGISTRO == null || p.ID_REGISTRO == request.registro.systemId.AsLong());
            }
        }

        public static async Task AppendFiltroSecurity(
            this FascicolazioneGetListaFascicoliPagingCustomCommand request,
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
            this FascicolazioneGetListaFascicoliPagingCustomCommand request,
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
            this FascicolazioneGetListaFascicoliPagingCustomCommand request,
            ProjectSearchAppendContext context)
        {
            if (request.classificazione != null && !string.IsNullOrWhiteSpace(request.classificazione.varcodliv1))
            {
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
                                                p2.VAR_COD_LIV1!.StartsWith(request.classificazione.varcodliv1)
                                                : p2.VAR_COD_LIV1 == request.classificazione.varcodliv1))
                                .Any());
            }
        }

        public static async Task AppendFiltroCodiceFascicolo(
            this FascicolazioneGetListaFascicoliPagingCustomCommand request,
            ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("CODICE_FASCICOLO");
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(p => p.VAR_CODICE.ToUpper().Equals(filtro.ToUpper()));
            }
        }

        public static async Task AppendFiltroStato(
                this FascicolazioneGetListaFascicoliPagingCustomCommand request,
                ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("STATO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                if (filtro.ToUpper().Equals("O"))
                {
                    filtro = "A";
                }
                context.Query = context.Query.Where(p => p.CHA_STATO == filtro);
            }

        }

        public static async Task AppendFiltroTipoFascicolo(
                this FascicolazioneGetListaFascicoliPagingCustomCommand request,
                ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("TIPO_FASCICOLO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(p => p.CHA_TIPO_FASCICOLO == filtro);
            }
        }

        public static async Task AppendFiltroAperturaIl(
               this FascicolazioneGetListaFascicoliPagingCustomCommand request,
               ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("APERTURA_IL");

            if (filtro > DateTime.MinValue)
            {
                var date = filtro;
                var beginDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                var endDate = beginDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_APERTURA >= beginDate && p.DTA_APERTURA <= endDate);
            }
        }

        public static async Task AppendFiltroAperturaSuccessivaAl(
               this FascicolazioneGetListaFascicoliPagingCustomCommand request,
               ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("APERTURA_SUCCESSIVA_AL");

            if (filtro > DateTime.MinValue)
            {
                var date = filtro;
                var beginDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);

                context.Query = context.Query.Where(p => p.DTA_APERTURA >= beginDate);
            }
        }

        public static async Task AppendFiltroAperturaPrecedenteIl(
               this FascicolazioneGetListaFascicoliPagingCustomCommand request,
               ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("APERTURA_PRECEDENTE_IL");

            if (filtro > DateTime.MinValue)
            {
                var date = filtro;
                var beginDate = date.AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_APERTURA <= beginDate);
            }
        }

        public static async Task AppendFiltroAperturaSC(
               this FascicolazioneGetListaFascicoliPagingCustomCommand request,
               ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("APERTURA_SC"))
            {
                var currentDate = DateTime.Now;
                int diff = (7 + (currentDate.DayOfWeek - DayOfWeek.Monday)) % 7;
                var date = currentDate.AddDays(-1 * diff).Date;

                var firstDayOfWeek = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                var lastDayOfWeek = firstDayOfWeek.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_APERTURA >= firstDayOfWeek && p.DTA_APERTURA <= lastDayOfWeek);
            }
        }

        public static async Task AppendFiltroAperturaMC(
               this FascicolazioneGetListaFascicoliPagingCustomCommand request,
               ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("APERTURA_MC"))
            {
                var currentDate = DateTime.Now;
                var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_APERTURA >= firstDayOfMonth && p.DTA_APERTURA <= lastDayOfMonth);
            }
        }

        public static async Task AppendFiltroAperturaToday(
             this FascicolazioneGetListaFascicoliPagingCustomCommand request,
             ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("APERTURA_TODAY"))
            {
                var currentDate = DateTime.Now;
                var initDate = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0);
                var endDate = initDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_APERTURA >= initDate && p.DTA_APERTURA <= endDate);
            }
        }

        public static async Task AppendFiltroChiusuraIl(
           this FascicolazioneGetListaFascicoliPagingCustomCommand request,
           ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("CHIUSURA_IL");

            if (filtro > DateTime.MinValue)
            {
                var currentDate = filtro;
                var initDate = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0);
                var endDate = initDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_CHIUSURA >= initDate && p.DTA_CHIUSURA <= endDate);
            }
        }

        public static async Task AppendFiltroChiusuraSuccessivaAl(
           this FascicolazioneGetListaFascicoliPagingCustomCommand request,
           ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("CHIUSURA_SUCCESSIVA_AL");

            if (filtro > DateTime.MinValue)
            {
                var currentDate = filtro;
                var beginDate = currentDate.AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_CHIUSURA >= beginDate);
            }
        }

        public static async Task AppendFiltroChiusuraPrecedenteIl(
          this FascicolazioneGetListaFascicoliPagingCustomCommand request,
          ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("CHIUSURA_PRECEDENTE_IL");

            if (filtro > DateTime.MinValue)
            {
                var currentDate = filtro;
                var initDate = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0);

                context.Query = context.Query.Where(p => p.DTA_CHIUSURA <= initDate);
            }
        }

        public static async Task AppendFiltroChiusuraSC(
             this FascicolazioneGetListaFascicoliPagingCustomCommand request,
             ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("CHIUSURA_SC"))
            {
                var currentDate = DateTime.Now;
                int diff = (7 + (currentDate.DayOfWeek - DayOfWeek.Monday)) % 7;
                var date = currentDate.AddDays(-1 * diff).Date;

                var firstDayOfWeek = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                var lastDayOfWeek = firstDayOfWeek.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_CHIUSURA >= firstDayOfWeek && p.DTA_CHIUSURA <= lastDayOfWeek);
            }
        }

        public static async Task AppendFiltroChiusuraMC(
               this FascicolazioneGetListaFascicoliPagingCustomCommand request,
               ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("CHIUSURA_MC"))
            {
                var currentDate = DateTime.Now;
                var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_CHIUSURA >= firstDayOfMonth && p.DTA_CHIUSURA <= lastDayOfMonth);
            }
        }

        public static async Task AppendFiltroChiusuraToday(
           this FascicolazioneGetListaFascicoliPagingCustomCommand request,
           ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("CHIUSURA_TODAY"))
            {
                var currentDate = DateTime.Now;
                var initDate = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0);
                var endDate = initDate.AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_CHIUSURA >= initDate && p.DTA_APERTURA <= endDate);
            }
        }

        public static async Task AppendFiltroSottoFascicolo(
                this FascicolazioneGetListaFascicoliPagingCustomCommand request,
                ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("SOTTOFASCICOLO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(p => context.Pi3DbContext.ProjectEntities.AsNoTracking()
                                                .Where(p2 => p2.CHA_TIPO_PROJ == "C"
                                                        && p2.DESCRIPTION!.ToUpper().Contains(filtro)
                                                        && p2.ID_FASCICOLO != p2.ID_PARENT)
                                                .Any());
            }
        }

        public static async Task AppendFiltroTitolo(
                this FascicolazioneGetListaFascicoliPagingCustomCommand request,
                ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("TITOLO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(p => p.DESCRIPTION!.ToUpper().Contains(filtro.ToUpper()));
            }
        }

        public static async Task AppendAnnoCreazione(
              this FascicolazioneGetListaFascicoliPagingCustomCommand request,
              ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("ANNO_FASCICOLO");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.ANNO_CREAZIONE == filtro);
            }
        }

        public static async Task AppendNumeroFascicolo(
              this FascicolazioneGetListaFascicoliPagingCustomCommand request,
              ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("NUMERO_FASCICOLO");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.NUM_FASCICOLO == filtro);
            }
        }

        public static async Task AppendFiltroCreazioneIl(
          this FascicolazioneGetListaFascicoliPagingCustomCommand request,
          ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("CREAZIONE_IL");

            if (filtro > DateTime.MinValue)
            {
                var currentDate = filtro;
                var initDate = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0);
                var endDate = initDate.AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_CREAZIONE >= initDate && p.DTA_CREAZIONE <= endDate);
            }
        }

        public static async Task AppendFiltroCreazioneSuccessivaAl(
            this FascicolazioneGetListaFascicoliPagingCustomCommand request,
            ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("CREAZIONE_SUCCESSIVA_AL");

            if (filtro > DateTime.MinValue)
            {
                var date = filtro;
                var beginDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);

                context.Query = context.Query.Where(p => p.DTA_CREAZIONE >= beginDate);
            }
        }

        public static async Task AppendFiltroCreazionePrecedenteIl(
           this FascicolazioneGetListaFascicoliPagingCustomCommand request,
           ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("CREAZIONE_PRECEDENTE_IL");

            if (filtro > DateTime.MinValue)
            {
                var date = filtro;
                var beginDate = date.AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_CREAZIONE <= beginDate);
            }
        }

        public static async Task AppendFiltroCreazioneSC(
           this FascicolazioneGetListaFascicoliPagingCustomCommand request,
           ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("CREAZIONE_SC"))
            {
                var currentDate = DateTime.Now;
                int diff = (7 + (currentDate.DayOfWeek - DayOfWeek.Monday)) % 7;
                var date = currentDate.AddDays(-1 * diff).Date;

                var firstDayOfWeek = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                var lastDayOfWeek = firstDayOfWeek.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_CREAZIONE >= firstDayOfWeek && p.DTA_CREAZIONE <= lastDayOfWeek);
            }
        }

        public static async Task AppendFiltroCreazioneMC(
             this FascicolazioneGetListaFascicoliPagingCustomCommand request,
             ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("CREAZIONE_MC"))
            {
                var currentDate = DateTime.Now;
                var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_CREAZIONE >= firstDayOfMonth && p.DTA_CREAZIONE <= lastDayOfMonth);
            }
        }

        public static async Task AppendFiltroCreazioneToday(
                   this FascicolazioneGetListaFascicoliPagingCustomCommand request,
                   ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("CREAZIONE_TODAY"))
            {
                var currentDate = DateTime.Now;
                var initDate = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0);
                var endDate = initDate.AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_CREAZIONE >= initDate && p.DTA_CREAZIONE <= endDate);
            }
        }

        public static async Task AppendFiltroCreazioneIeri(
                   this FascicolazioneGetListaFascicoliPagingCustomCommand request,
                   ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("CREAZIONE_IERI"))
            {
                var currentDate = DateTime.Now.AddDays(-1);
                var initDate = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0);
                var endDate = initDate.AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_CREAZIONE >= initDate && p.DTA_CREAZIONE <= endDate);
            }
        }

        public static async Task AppendFiltroCreazioneUltimi7Giorni(
                  this FascicolazioneGetListaFascicoliPagingCustomCommand request,
                  ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("CREAZIONE_ULTIMI_SETTE_GIORNI"))
            {
                var currentDate = DateTime.Now.AddDays(-7);
                var initDate = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0);
                var endDate = initDate.AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_CREAZIONE >= initDate && p.DTA_CREAZIONE <= endDate);
            }
        }

        public static async Task AppendFiltroCreazioneUltimi31Giorni(
                 this FascicolazioneGetListaFascicoliPagingCustomCommand request,
                 ProjectSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("CREAZIONE_ULTMI_TRENTUNO_GIORNI"))
            {
                var currentDate = DateTime.Now.AddDays(-31);
                var initDate = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0);
                var endDate = initDate.AddHours(23).AddMinutes(59).AddSeconds(59);

                context.Query = context.Query.Where(p => p.DTA_CREAZIONE >= initDate && p.DTA_CREAZIONE <= endDate);
            }
        }

        public static async Task AppendFiltroIdUOLF(
                 this FascicolazioneGetListaFascicoliPagingCustomCommand request,
                 ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("ID_UO_LF");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.ID_UO_LF == filtro);
            }
        }

        public static async Task AppendFiltroDataLFIl(
         this FascicolazioneGetListaFascicoliPagingCustomCommand request,
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
             this FascicolazioneGetListaFascicoliPagingCustomCommand request,
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
            this FascicolazioneGetListaFascicoliPagingCustomCommand request,
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
          this FascicolazioneGetListaFascicoliPagingCustomCommand request,
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
              this FascicolazioneGetListaFascicoliPagingCustomCommand request,
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
                  this FascicolazioneGetListaFascicoliPagingCustomCommand request,
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
                 this FascicolazioneGetListaFascicoliPagingCustomCommand request,
                 ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("ID_UO_REF");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.ID_UO_REF == filtro);
            }
        }

        public static async Task AppendFiltroNote(
                 this FascicolazioneGetListaFascicoliPagingCustomCommand request,
                 ProjectSearchAppendContext context)
        {
            // TODO            
        }

        public static async Task AppendFiltroTipologiaFascicolo(
                 this FascicolazioneGetListaFascicoliPagingCustomCommand request,
                 ProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("TIPOLOGIA_FASCICOLO");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.ID_TIPO_FASC == filtro ||
                context.Pi3DbContext.TipoFascEntities.AsNoTracking().Where(t => t.SYSTEM_ID == filtro && t.IPERFASCICOLO == 1).Any());
            }
        }

        public static async Task AppendFiltroProfilazioneDinamica(
              this FascicolazioneGetListaFascicoliPagingCustomCommand request,
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
                                                .Any(at => at.ID_PROJECT != null && at.ID_PROJECT == t.SYSTEM_ID.ToString()
                                                    && at.ID_OGGETTO == oggetto.SYSTEM_ID
                                                    && (oggetto.TIPO_RICERCA_STRINGA == DocsPaVO.ProfilazioneDinamica.TipoRicercaStringaEnum.PAROLA_INTERA ?
                                                            at.VALORE_OGGETTO_DB.ToUpper() == oggetto.VALORE_DATABASE.ToUpper()
                                                            : (oggetto.TIPO_RICERCA_STRINGA == DocsPaVO.ProfilazioneDinamica.TipoRicercaStringaEnum.PARTE_DELLA_PAROLA ?
                                                                at.VALORE_OGGETTO_DB.ToUpper().Contains(oggetto.VALORE_DATABASE.ToUpper())
                                                                    : at.VALORE_OGGETTO_DB.ToUpper().StartsWith(oggetto.VALORE_DATABASE.ToUpper())))));
                                }
                                break;
                            case "CasellaDiSelezione":
                                foreach (var casella in oggetto.VALORI_SELEZIONATI.Where(v => !string.IsNullOrEmpty(v)).ToList())
                                {
                                    context.Query = context.Query.Where(t =>
                                            context.Pi3DbContext.AssTemplatesFascEntities.AsNoTracking()
                                                .Any(at => at.ID_PROJECT != null && at.ID_PROJECT == t.SYSTEM_ID.ToString()
                                                        && at.ID_OGGETTO == oggetto.SYSTEM_ID
                                                        && at.VALORE_OGGETTO_DB.ToUpper() == casella.ToUpper()));
                                }
                                break;
                            case "MenuATendina":
                                if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                {
                                    context.Query = context.Query.Where(t =>
                                            context.Pi3DbContext.AssTemplatesFascEntities.AsNoTracking()
                                                .Any(at => at.ID_PROJECT != null && at.ID_PROJECT == t.SYSTEM_ID.ToString()
                                                        && at.ID_OGGETTO == oggetto.SYSTEM_ID
                                                        && at.VALORE_OGGETTO_DB.ToUpper() == oggetto.VALORE_DATABASE.ToUpper()));
                                }
                                break;
                            case "SelezioneEsclusiva":
                                if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                {
                                    context.Query = context.Query.Where(t =>
                                            context.Pi3DbContext.AssTemplatesFascEntities.AsNoTracking()
                                                .Any(at => at.ID_PROJECT != null && at.ID_PROJECT == t.SYSTEM_ID.ToString()
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
                                        var endDate = GetDateSuccessivaAl(dataInserimento[1].AsDateTime());

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
                                        .Any(at => at.ID_PROJECT != null && at.ID_PROJECT == t.SYSTEM_ID.ToString()));
                                }
                                break;
                            case "Data":
                                if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                {
                                    DateTime? init = null;
                                    DateTime? end = null;
                                    if (oggetto.DATA_INSERIMENTO.IndexOf('@') != -1)
                                    {
                                        string[] dataInserimento = oggetto.DATA_INSERIMENTO.Split('@');
                                        init = GetDateSuccessivaAl(dataInserimento[0].AsDateTime());
                                        end = GetDateSuccessivaAl(dataInserimento[1].AsDateTime());
                                    }
                                    else
                                    {
                                        var range = GetDateRangeIl(oggetto.DATA_INSERIMENTO.AsDateTime());
                                        init = range.Item1;
                                        end = range.Item2;
                                    }
                                    context.Query = context.Query.Where(t =>
                                            context.Pi3DbContext.AssTemplatesFascEntities.AsNoTracking()
                                                .Any(at => at.ID_PROJECT != null && at.ID_PROJECT == t.SYSTEM_ID.ToString()
                                                        && at.ID_OGGETTO == oggetto.SYSTEM_ID && at.VALORE_OGGETTO_DB != null
                                                        && Convert.ToDateTime(at.VALORE_OGGETTO_DB) >= init && Convert.ToDateTime(at.VALORE_OGGETTO_DB) <= end));
                                }
                                break;
                            case "Corrispondente":
                                if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                {
                                    long idCorrGlobali = oggetto.VALORE_DATABASE.AsLong();

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
              this FascicolazioneGetListaFascicoliPagingCustomCommand request,
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
                 this FascicolazioneGetListaFascicoliPagingCustomCommand request,
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
           this FascicolazioneGetListaFascicoliPagingCustomCommand request,
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
           this FascicolazioneGetListaFascicoliPagingCustomCommand request,
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
          this FascicolazioneGetListaFascicoliPagingCustomCommand request,
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
             this FascicolazioneGetListaFascicoliPagingCustomCommand request,
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
               this FascicolazioneGetListaFascicoliPagingCustomCommand request,
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
           this FascicolazioneGetListaFascicoliPagingCustomCommand request,
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

        // Riprendere dal friltro per CONSERVAZIONE su docspadb.fascicolo.getsqlquery


    }
}
