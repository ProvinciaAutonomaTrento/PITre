// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.ricerche;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetQueryDocumentoPagingCustom;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FascicolazioneGetDocumentiPagingWithFiltersCustomRequest = Pi3.App.Legacy.WebApi.Application.Requests.FascicolazioneGetDocumentiPagingWithFiltersCustom;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneGetDocumentiPagingWithFiltersCustom
{
    public class FascicolazioneGetDocumentiPagingWithFiltersCustomHandler : IRequestHandler<FascicolazioneGetDocumentiPagingWithFiltersCustomRequest, FascicolazioneGetDocumentiPagingWithFiltersCustomResult>
    {
        #region Public Members

        public FascicolazioneGetDocumentiPagingWithFiltersCustomHandler(
            ILogger<FascicolazioneGetDocumentiPagingWithFiltersCustomHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<FascicolazioneGetDocumentiPagingWithFiltersCustomResult> Handle(FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request, CancellationToken cancellationToken)
        {
            var output = new DocsPaVO.Grids.SearchObject[0];
            int nRec = 0;
            int numTotPage = 0;
            var idProfiles = new List<SearchResultInfo>();

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
                    this._pi3DbContext.ProfileEntities
                        .AsNoTracking()
                        .Where(p => p.CHA_IN_CESTINO == null || p.CHA_IN_CESTINO == "0");

                var appendContext = new ProfileProjectSearchAppendContext(this._pi3DbContext, query);

                await request.AppendFiltroIdFascicolo(appendContext);
                await request.AppendFiltroSecurity(appendContext);
                await request.AppendFiltroTipoProto(appendContext);
                await request.AppendFiltroTipoAtto(appendContext);
                await request.AppendFiltroProfilazioneDinamica(appendContext);
                await request.AppendFiltroSegnatura(appendContext);
                await request.AppendFiltroNumeroProtocollo(appendContext);
                await request.AppendFiltroNumeroProtocolloDal(appendContext);
                await request.AppendFiltroNumeroProtocolloAl(appendContext);
                await request.AppendFiltroDataProtIl(appendContext);
                await request.AppendFiltroDataProtSuccessivaAl(appendContext);
                await request.AppendFiltroDataProtPrecedenteIl(appendContext);
                await request.AppendFiltroAnnoProtocollo(appendContext);
                await request.AppendFiltroDocNumber(appendContext);
                await request.AppendFiltroDocNumberDal(appendContext);
                await request.AppendFiltroDocNumberAl(appendContext);
                await request.AppendFiltroDataCreazioneIl(appendContext);
                await request.AppendFiltroDataCreazioneSuccessivaAl(appendContext);
                await request.AppendFiltroDataCreazionePrecedenteIl(appendContext);
                await request.AppendFiltroOggetto(appendContext);
                await request.AppendFiltroMittDest(appendContext);
                await request.AppendFiltroIdMittDest(appendContext);
                await request.AppendFiltroIdDestinatario(appendContext);
                await request.AppendFiltroDescrizioneDestinatario(appendContext);
                await request.AppendFiltroFirmato(appendContext);
                await request.AppendFiltroTipoFileAcquisito(appendContext);
                await request.AppendFiltroTipoFileAcquisito(appendContext);
                await request.AppendFiltroExport(appendContext);
                
                query = appendContext.Query;


                var ids = await query
                    .Select(p =>
                        new
                        {
                            SYSTEM_ID = p.SYSTEM_ID,
                            CODICE = p.NUM_PROTO.HasValue ? p.NUM_PROTO.ToString() : p.DOCNUMBER.ToString()
                        })
                    .ToListAsync();

                nRec = ids.Count();

                if (request.compileIdProfileList)
                {
                    idProfiles = ids.Select(p => new SearchResultInfo()
                    {
                        Id = p.SYSTEM_ID.ToString(),
                        Codice = p.CODICE
                    })
                    .ToList();
                }

                numTotPage = nRec / request.pageSize;

                if (nRec > 0)
                {
                    if(!request.export)
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
                                        new DocsPaVO.Grids.SearchObjectField("D1", p.DOCNUMBER.HasValue ? p.DOCNUMBER.ToString() : null!),
                                        new DocsPaVO.Grids.SearchObjectField("CODICE", p.NUM_PROTO.HasValue ? p.NUM_PROTO.ToString() : p.DOCNUMBER.ToString()),
                                        new DocsPaVO.Grids.SearchObjectField("ID_REGISTRO", p.ID_REGISTRO.HasValue ? p.ID_REGISTRO.ToString() : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D2", p.ID_REGISTRO.HasValue ?  IPi3DbContextMappedFunctions.GetCodReg(p.ID_REGISTRO.Value) : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D3", p.CHA_TIPO_PROTO),
                                        new DocsPaVO.Grids.SearchObjectField("D4", p.VAR_PROF_OGGETTO),
                                        new DocsPaVO.Grids.SearchObjectField("D5", IPi3DbContextMappedFunctions.CorrCat(p.SYSTEM_ID, p.CHA_TIPO_PROTO)),
                                        new DocsPaVO.Grids.SearchObjectField("D6", IPi3DbContextMappedFunctions.CorrCatByTipo(p.SYSTEM_ID, p.CHA_TIPO_PROTO, "M")),
                                        new DocsPaVO.Grids.SearchObjectField("D7", IPi3DbContextMappedFunctions.CorrCatByTipo(p.SYSTEM_ID, p.CHA_TIPO_PROTO, "D")),
                                        new DocsPaVO.Grids.SearchObjectField("D9", p.DTA_PROTO.HasValue ? p.DTA_PROTO.AsDateFormat() : p.CREATION_TIME.AsDateFormat()),
                                        new DocsPaVO.Grids.SearchObjectField("D8", p.VAR_SEGNATURA),
                                        new DocsPaVO.Grids.SearchObjectField("D10", IPi3DbContextMappedFunctions.GetEsitoPubblicazione(p.SYSTEM_ID)),
                                        new DocsPaVO.Grids.SearchObjectField("D11", p.DTA_ANNULLA.HasValue ? p.DTA_ANNULLA.AsDateFormat() : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D12", p.NUM_PROTO.HasValue ? p.NUM_PROTO.ToString() : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D13", p.AUTHOR.HasValue ? IPi3DbContextMappedFunctions.GetPeopleUserId(p.AUTHOR.Value) : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D14", p.ARCHIVE_DATE.HasValue ? p.ARCHIVE_DATE.AsDateFormat() : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D15", p.CHA_PERSONALE),
                                        new DocsPaVO.Grids.SearchObjectField("D16", p.CHA_PRIVATO),
                                        new DocsPaVO.Grids.SearchObjectField("D17", IPi3DbContextMappedFunctions.GetTestoUltimaNota("D", p.SYSTEM_ID, idRuoloInUO, p.AUTHOR.Value, p.ID_RUOLO_CREATORE.Value)),
                                        new DocsPaVO.Grids.SearchObjectField("D18", IPi3DbContextMappedFunctions.ClassCat(p.SYSTEM_ID)),
                                        new DocsPaVO.Grids.SearchObjectField("D19", p.AUTHOR.HasValue ? IPi3DbContextMappedFunctions.GetPeopleName(p.AUTHOR.Value) : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D20", p.ID_RUOLO_CREATORE.HasValue ? IPi3DbContextMappedFunctions.GetDescCorr(p.ID_RUOLO_CREATORE.Value) : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D21", IPi3DbContextMappedFunctions.GetDataArrivoDoc(p.DOCNUMBER.Value).AsDateFormat()),
                                        new DocsPaVO.Grids.SearchObjectField("D22", IPi3DbContextMappedFunctions.GetDiagrammiStato(p.SYSTEM_ID, "D")),
                                        new DocsPaVO.Grids.SearchObjectField("D23", p.EXT),
                                        //AsFieldAtipicita(p.CHA_COD_T_A),
                                        new DocsPaVO.Grids.SearchObjectField("D26", p.ID_PEOPLE_PROT.HasValue ? IPi3DbContextMappedFunctions.GetPeopleUserId(p.ID_PEOPLE_PROT.Value) : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D27", p.ID_PEOPLE_PROT.HasValue ? IPi3DbContextMappedFunctions.GetPeopleName(p.ID_PEOPLE_PROT.Value) : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D28", p.ID_RUOLO_PROT.HasValue ? IPi3DbContextMappedFunctions.GetDescCorr(p.ID_RUOLO_PROT.Value) : null!),
                                        new DocsPaVO.Grids.SearchObjectField("IN_ADL", IPi3DbContextMappedFunctions.GetInAdl(p.SYSTEM_ID, "D", idGroup, idUser)),
                                        new DocsPaVO.Grids.SearchObjectField("IN_ADLROLE", IPi3DbContextMappedFunctions.GetInAdl(p.SYSTEM_ID, "D", idGroup, 0)),
                                        new DocsPaVO.Grids.SearchObjectField("IN_CONSERVAZIONE", IPi3DbContextMappedFunctions.GetInConservazione(p.SYSTEM_ID, 0, "D",  idUser, idGroup).ToString()),
                                        new DocsPaVO.Grids.SearchObjectField("CHA_IN_ARCHIVIO", p.CHA_IN_ARCHIVIO),
                                        new DocsPaVO.Grids.SearchObjectField("ID_TIPO_ATTO", p.ID_TIPO_ATTO.HasValue ? p.ID_TIPO_ATTO.ToString() : null!),
                                        new DocsPaVO.Grids.SearchObjectField("U1", p.ID_TIPO_ATTO.HasValue ? IPi3DbContextMappedFunctions.GetDescTipoDoc(p.ID_TIPO_ATTO.Value) : null!),
                                        new DocsPaVO.Grids.SearchObjectField("ID_DOCUMENTO_PRINCIPALE", p.ID_DOCUMENTO_PRINCIPALE.HasValue ? p.ID_DOCUMENTO_PRINCIPALE.ToString() : null!),
                                        new DocsPaVO.Grids.SearchObjectField("CHA_FIRMATO", IPi3DbContextMappedFunctions.GetChaFirmato(p.SYSTEM_ID)),
                                        new DocsPaVO.Grids.SearchObjectField("CHA_TIPO_FIRMA", IPi3DbContextMappedFunctions.GetChaTipoFirma(p.DOCNUMBER.Value)),
                                        new DocsPaVO.Grids.SearchObjectField("PROT_TIT", p.PROT_TIT),
                                        new DocsPaVO.Grids.SearchObjectField("ESISTE_NOTA", IPi3DbContextMappedFunctions.EsisteNotaVisibile("D", p.SYSTEM_ID, idRuoloInUO, idUser, p.ID_RUOLO_CREATORE.Value)),
                                        new DocsPaVO.Grids.SearchObjectField("CONTATORE", IPi3DbContextMappedFunctions.GetContatoreDoc(p.SYSTEM_ID, "R")),
                                        new DocsPaVO.Grids.SearchObjectField("ISTANZECONSERVAZIONE", IPi3DbContextMappedFunctions.GetInConservazioneNoSec(0, p.SYSTEM_ID, "D")),
                                        new DocsPaVO.Grids.SearchObjectField("IMPRONTA", IPi3DbContextMappedFunctions.GetImprontaWithAttachSearch(p.DOCNUMBER.Value)),
                                        new DocsPaVO.Grids.SearchObjectField("NOME_ORIGINALE", IPi3DbContextMappedFunctions.GetNomeOriginale(p.SYSTEM_ID)),
                                        new DocsPaVO.Grids.SearchObjectField("COD_EXT_APP", p.COD_EXT_APP),
                                        new DocsPaVO.Grids.SearchObjectField("DTA_ADL", IPi3DbContextMappedFunctions.GetDateInADL(p.SYSTEM_ID, "D", idGroup, idUser) != null ? IPi3DbContextMappedFunctions.GetDateInADL(p.SYSTEM_ID, "D", idGroup, idUser).AsDateFormat() : null!),
                                        new DocsPaVO.Grids.SearchObjectField("MOTIVO_ADL", IPi3DbContextMappedFunctions.GetMotivoADL(p.SYSTEM_ID, "D", idGroup, idUser)),
                                        new DocsPaVO.Grids.SearchObjectField("esito_spedizione", IPi3DbContextMappedFunctions.GetEsitoSpedizione(p.SYSTEM_ID)),
                                        new DocsPaVO.Grids.SearchObjectField("count_ric_interop", IPi3DbContextMappedFunctions.GetCountRicevuteInterop(p.SYSTEM_ID, String.Empty)),
                                        new DocsPaVO.Grids.SearchObjectField("stato_conservazione", IPi3DbContextMappedFunctions.GetStatoConservazione(p.SYSTEM_ID)),
                                        new DocsPaVO.Grids.SearchObjectField("CODICE_POLICY", IPi3DbContextMappedFunctions.GetPolicyVersamentoCod(p.SYSTEM_ID)),
                                        new DocsPaVO.Grids.SearchObjectField("CONTATORE_POLICY", IPi3DbContextMappedFunctions.GetPolicyVersamentoCounter(p.SYSTEM_ID)),
                                        new DocsPaVO.Grids.SearchObjectField("DATA_ESECUZIONE_POLICY", IPi3DbContextMappedFunctions.GetPolicyVersamentoDataExec(p.SYSTEM_ID)),
                                        new DocsPaVO.Grids.SearchObjectField("is_doc_conservato", null!),
                                        new DocsPaVO.Grids.SearchObjectField("CHA_TASK_STATUS", p.CHA_TASK_STATUS),
                                        new DocsPaVO.Grids.SearchObjectField("GetValProfObjsDocAsJson", IPi3DbContextMappedFunctions.GetValProfObjsDocAsJson(p.SYSTEM_ID)),
                                        new DocsPaVO.Grids.SearchObjectField("IN_LIBROFIRMA", p.IN_LIBROFIRMA)
                                        }
                                    ).ToArray()
                                ));

                        output = await dataQuery.ToArrayAsync();
                    }
                    else
                    {

                        var dataQuery = query
                            .OrderBy(request, this._pi3DbContext)
                            .Select(p =>
                                    AsSearchObject(
                                        p.SYSTEM_ID.ToString(),
                                        (new List<DocsPaVO.Grids.SearchObjectField>()
                                            {
                                        new DocsPaVO.Grids.SearchObjectField("D1", p.DOCNUMBER.HasValue ? p.DOCNUMBER.ToString() : null!),
                                        new DocsPaVO.Grids.SearchObjectField("CODICE", p.NUM_PROTO.HasValue ? p.NUM_PROTO.ToString() : p.DOCNUMBER.ToString()),
                                        new DocsPaVO.Grids.SearchObjectField("ID_REGISTRO", p.ID_REGISTRO.HasValue ? p.ID_REGISTRO.ToString() : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D2", p.ID_REGISTRO.HasValue ?  IPi3DbContextMappedFunctions.GetCodReg(p.ID_REGISTRO.Value) : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D3", p.CHA_TIPO_PROTO),
                                        new DocsPaVO.Grids.SearchObjectField("D4", p.VAR_PROF_OGGETTO),
                                        new DocsPaVO.Grids.SearchObjectField("D5", IPi3DbContextMappedFunctions.CorrCat(p.SYSTEM_ID, p.CHA_TIPO_PROTO)),
                                        new DocsPaVO.Grids.SearchObjectField("D6", IPi3DbContextMappedFunctions.CorrCatByTipo(p.SYSTEM_ID, p.CHA_TIPO_PROTO, "M")),
                                        new DocsPaVO.Grids.SearchObjectField("D7", IPi3DbContextMappedFunctions.CorrCatByTipo(p.SYSTEM_ID, p.CHA_TIPO_PROTO, "D")),
                                        new DocsPaVO.Grids.SearchObjectField("D9", p.DTA_PROTO.HasValue ? p.DTA_PROTO.AsDateFormat() : p.CREATION_TIME.AsDateFormat()),
                                        new DocsPaVO.Grids.SearchObjectField("D8", p.VAR_SEGNATURA),
                                        new DocsPaVO.Grids.SearchObjectField("D10", IPi3DbContextMappedFunctions.GetEsitoPubblicazione(p.SYSTEM_ID)),
                                        new DocsPaVO.Grids.SearchObjectField("D11", p.DTA_ANNULLA.HasValue ? p.DTA_ANNULLA.AsDateFormat() : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D12", p.NUM_PROTO.HasValue ? p.NUM_PROTO.ToString() : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D13", p.AUTHOR.HasValue ? IPi3DbContextMappedFunctions.GetPeopleUserId(p.AUTHOR.Value) : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D14", p.ARCHIVE_DATE.HasValue ? p.ARCHIVE_DATE.AsDateFormat() : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D15", p.CHA_PERSONALE),
                                        new DocsPaVO.Grids.SearchObjectField("D16", p.CHA_PRIVATO),
                                        new DocsPaVO.Grids.SearchObjectField("D17", IPi3DbContextMappedFunctions.GetTestoUltimaNota("D", p.SYSTEM_ID, idRuoloInUO, p.AUTHOR.Value, p.ID_RUOLO_CREATORE.Value)),
                                        new DocsPaVO.Grids.SearchObjectField("D18", IPi3DbContextMappedFunctions.ClassCat(p.SYSTEM_ID)),
                                        new DocsPaVO.Grids.SearchObjectField("D19", p.AUTHOR.HasValue ? IPi3DbContextMappedFunctions.GetPeopleName(p.AUTHOR.Value) : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D20", p.ID_RUOLO_CREATORE.HasValue ? IPi3DbContextMappedFunctions.GetDescCorr(p.ID_RUOLO_CREATORE.Value) : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D21", IPi3DbContextMappedFunctions.GetDataArrivoDoc(p.DOCNUMBER.Value).AsDateFormat()),
                                        new DocsPaVO.Grids.SearchObjectField("D22", IPi3DbContextMappedFunctions.GetDiagrammiStato(p.SYSTEM_ID, "D")),
                                        new DocsPaVO.Grids.SearchObjectField("D23", p.EXT),
                                        //AsFieldAtipicita(p.CHA_COD_T_A),
                                        new DocsPaVO.Grids.SearchObjectField("D26", p.ID_PEOPLE_PROT.HasValue ? IPi3DbContextMappedFunctions.GetPeopleUserId(p.ID_PEOPLE_PROT.Value) : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D27", p.ID_PEOPLE_PROT.HasValue ? IPi3DbContextMappedFunctions.GetPeopleName(p.ID_PEOPLE_PROT.Value) : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D28", p.ID_RUOLO_PROT.HasValue ? IPi3DbContextMappedFunctions.GetDescCorr(p.ID_RUOLO_PROT.Value) : null!),
                                        new DocsPaVO.Grids.SearchObjectField("IN_ADL", IPi3DbContextMappedFunctions.GetInAdl(p.SYSTEM_ID, "D", idGroup, idUser)),
                                        new DocsPaVO.Grids.SearchObjectField("IN_ADLROLE", IPi3DbContextMappedFunctions.GetInAdl(p.SYSTEM_ID, "D", idGroup, 0)),
                                        new DocsPaVO.Grids.SearchObjectField("IN_CONSERVAZIONE", IPi3DbContextMappedFunctions.GetInConservazione(p.SYSTEM_ID, 0, "D",  idUser, idGroup).ToString()),
                                        new DocsPaVO.Grids.SearchObjectField("CHA_IN_ARCHIVIO", p.CHA_IN_ARCHIVIO),
                                        new DocsPaVO.Grids.SearchObjectField("ID_TIPO_ATTO", p.ID_TIPO_ATTO.HasValue ? p.ID_TIPO_ATTO.ToString() : null!),
                                        new DocsPaVO.Grids.SearchObjectField("U1", p.ID_TIPO_ATTO.HasValue ? IPi3DbContextMappedFunctions.GetDescTipoDoc(p.ID_TIPO_ATTO.Value) : null!),
                                        new DocsPaVO.Grids.SearchObjectField("ID_DOCUMENTO_PRINCIPALE", p.ID_DOCUMENTO_PRINCIPALE.HasValue ? p.ID_DOCUMENTO_PRINCIPALE.ToString() : null!),
                                        new DocsPaVO.Grids.SearchObjectField("CHA_FIRMATO", IPi3DbContextMappedFunctions.GetChaFirmato(p.SYSTEM_ID)),
                                        new DocsPaVO.Grids.SearchObjectField("CHA_TIPO_FIRMA", IPi3DbContextMappedFunctions.GetChaTipoFirma(p.DOCNUMBER.Value)),
                                        new DocsPaVO.Grids.SearchObjectField("PROT_TIT", p.PROT_TIT),
                                        new DocsPaVO.Grids.SearchObjectField("ESISTE_NOTA", IPi3DbContextMappedFunctions.EsisteNotaVisibile("D", p.SYSTEM_ID, idRuoloInUO, idUser, p.ID_RUOLO_CREATORE.Value)),
                                        new DocsPaVO.Grids.SearchObjectField("CONTATORE", IPi3DbContextMappedFunctions.GetContatoreDoc(p.SYSTEM_ID, "R")),
                                        new DocsPaVO.Grids.SearchObjectField("ISTANZECONSERVAZIONE", IPi3DbContextMappedFunctions.GetInConservazioneNoSec(0, p.SYSTEM_ID, "D")),
                                        new DocsPaVO.Grids.SearchObjectField("IMPRONTA", IPi3DbContextMappedFunctions.GetImprontaWithAttachSearch(p.DOCNUMBER.Value)),
                                        new DocsPaVO.Grids.SearchObjectField("NOME_ORIGINALE", IPi3DbContextMappedFunctions.GetNomeOriginale(p.SYSTEM_ID)),
                                        new DocsPaVO.Grids.SearchObjectField("COD_EXT_APP", p.COD_EXT_APP),
                                        new DocsPaVO.Grids.SearchObjectField("DTA_ADL", IPi3DbContextMappedFunctions.GetDateInADL(p.SYSTEM_ID, "D", idGroup, idUser) != null ? IPi3DbContextMappedFunctions.GetDateInADL(p.SYSTEM_ID, "D", idGroup, idUser).AsDateFormat() : null!),
                                        new DocsPaVO.Grids.SearchObjectField("MOTIVO_ADL", IPi3DbContextMappedFunctions.GetMotivoADL(p.SYSTEM_ID, "D", idGroup, idUser)),
                                        new DocsPaVO.Grids.SearchObjectField("esito_spedizione", IPi3DbContextMappedFunctions.GetEsitoSpedizione(p.SYSTEM_ID)),
                                        new DocsPaVO.Grids.SearchObjectField("count_ric_interop", IPi3DbContextMappedFunctions.GetCountRicevuteInterop(p.SYSTEM_ID, String.Empty)),
                                        new DocsPaVO.Grids.SearchObjectField("stato_conservazione", IPi3DbContextMappedFunctions.GetStatoConservazione(p.SYSTEM_ID)),
                                        new DocsPaVO.Grids.SearchObjectField("CODICE_POLICY", IPi3DbContextMappedFunctions.GetPolicyVersamentoCod(p.SYSTEM_ID)),
                                        new DocsPaVO.Grids.SearchObjectField("CONTATORE_POLICY", IPi3DbContextMappedFunctions.GetPolicyVersamentoCounter(p.SYSTEM_ID)),
                                        new DocsPaVO.Grids.SearchObjectField("DATA_ESECUZIONE_POLICY", IPi3DbContextMappedFunctions.GetPolicyVersamentoDataExec(p.SYSTEM_ID)),
                                        new DocsPaVO.Grids.SearchObjectField("is_doc_conservato", null!),
                                        new DocsPaVO.Grids.SearchObjectField("CHA_TASK_STATUS", p.CHA_TASK_STATUS),
                                        new DocsPaVO.Grids.SearchObjectField("GetValProfObjsDocAsJson", IPi3DbContextMappedFunctions.GetValProfObjsDocAsJson(p.SYSTEM_ID)),
                                        new DocsPaVO.Grids.SearchObjectField("IN_LIBROFIRMA", p.IN_LIBROFIRMA)
                                            }
                                        ).ToArray()
                                    ));

                        output = await dataQuery.ToArrayAsync();
                    }

                    if (request.visibleFieldsTemplate != null)
                    {
                        foreach (var item in output)
                        {
                            var customObjectsAsJson = item.SearchObjectField
                                .Where(f => f.SearchObjectFieldID == "GetValProfObjsDocAsJson")
                                .Select(f => f.SearchObjectFieldValue)
                                .First();
                            var docnumber = item.SearchObjectField
                                .Where(f => f.SearchObjectFieldID == "D1")
                                .Select(f => f.SearchObjectFieldValue)
                                .First();
                            var objectFields = new List<DocsPaVO.Grids.SearchObjectField>();

                            var objs = System.Text.Json.JsonSerializer
                                    .Deserialize<GetValProfObjPrj[]>(customObjectsAsJson)!
                                    .Where(obj => request.visibleFieldsTemplate.Any(ft => ft.CustomObjectId == Convert.ToInt32(obj.id)));

                            foreach (var obj in objs)
                            {
                                string value = obj.valore;
                                var key = $"T{obj.id}";
                                var of = objectFields.FirstOrDefault(of => of.SearchObjectFieldID == key);

                                var objData = await this._pi3DbContext.OggettiCustomEntities.AsNoTracking().Where(o => o.SYSTEM_ID == obj.id.AsLong())
                               .Join(this._pi3DbContext.TipoOggettoEntities.AsNoTracking(), o => o.ID_TIPO_OGGETTO, t => t.SYSTEM_ID, (o, t) => new
                               {
                                   desc = t.DESCRIZIONE,
                                   tipoCont = o.CHA_TIPO_TAR,
                                   repert = o.REPERTORIO
                               }).FirstOrDefaultAsync();


                                var tipoOgg = objData?.desc;
                                var tipoCont = objData?.tipoCont;
                                var repert = objData?.repert;

                                if ("Corrispondente".Equals(tipoOgg) && !string.IsNullOrEmpty(obj.valore))
                                {
                                    var corrMatch = await this._pi3DbContext.CorrGlobaliEntities.AsNoTracking().Where(cor => cor.SYSTEM_ID == obj.valore.AsLong()).FirstOrDefaultAsync();
                                    if (corrMatch != null)
                                        value = string.Join(" - ", corrMatch.VAR_COD_RUBRICA, corrMatch.VAR_DESC_CORR);
                                }
                                else if ("CasellaDiSelezione".Equals(tipoOgg))
                                {
                                    var casSel = await this._pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking().Where(
                                        a => a.ID_OGGETTO == obj.id.AsLong() && a.VALORE_OGGETTO_DB != null &&
                                        a.DOC_NUMBER == docnumber
                                        ).Select(a => a.VALORE_OGGETTO_DB).ToListAsync();
                                    if (casSel.Any())
                                    {
                                        value = string.Join("; ", casSel);
                                    }
                                }
                                else if ("Contatore".Equals(tipoOgg) && repert == 1)
                                {
                                    value = "#CONTATORE_DI_REPERTORIO#";
                                }
                                else if ("Contatore".Equals(tipoOgg) || "ContatoreSottocontatore".Equals(tipoOgg))
                                {
                                    value = await this._pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking().Where(a => a.ID_OGGETTO == obj.id.AsLong() && a.DOC_NUMBER == docnumber)
                                        .Select(a => IPi3DbContextMappedFunctions.GetContatoreDoc2(docnumber.AsLong(), tipoCont, obj.id.AsLong())).FirstOrDefaultAsync();
                                }


                                objectFields.Add(new DocsPaVO.Grids.SearchObjectField(key, value));
                            }



                            if (objectFields.Any())
                            {
                                item.SearchObjectField.AddRange(objectFields);
                            }

                            item.SearchObjectField.RemoveAll(f => f.SearchObjectFieldID == "GetValProfObjsDocAsJson" || f.SearchObjectFieldID == "-1" || f.SearchObjectFieldID == "-1D");

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

            return new FascicolazioneGetDocumentiPagingWithFiltersCustomResult(output, numTotPage, nRec, idProfiles.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneGetDocumentiPagingWithFiltersCustomHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IConfigurationService _configurationService;

        protected record GetValProfObjPrj(string id, string nome, string valore);

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
            }
            else
            {
                return new DocsPaVO.Grids.SearchObjectField()
                {
                    SearchObjectFieldID = "D24",
                    SearchObjectFieldValue = string.Empty
                };
            }
        }

        #endregion
    }

    internal class ProfileProjectSearchAppendContext
    {
        public ProfileProjectSearchAppendContext(IPi3DbContext pi3DbContext, IQueryable<ProfileEntity> query)
        {
            Pi3DbContext = pi3DbContext;
            Query = query;
        }

        public IPi3DbContext Pi3DbContext { get; set; }

        public IQueryable<ProfileEntity> Query { get; set; }
    }

    internal static class QueryableProfileProjectExtentions
    {
        public static IQueryable<ProfileEntity> OrderBy(this IQueryable<ProfileEntity> query, FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request, IPi3DbContext pi3DbContext)
        {
            var orderDirection = request.GetValoreFiltroOrder<string>("ORDER_DIRECTION");
            var orderByForProfiledField = request.GetValoreFiltroOrder<string>("PROFILATION_FIELD_FOR_ORDER");
            bool directionIsDescending = DocsPaVO.Grid.Grid.OrderDirectionEnum.Desc.ToString() == orderDirection;
            var field = request.FindFiltroOrder("ORACLE_FIELD_FOR_ORDER");
            bool wasFieldOrderingApplied = false;
            var contatoreNoCustom = request.GetValoreFiltroRicerca<string>("CONTATORE_GRIGLIE_NO_CUSTOM");

            // CAMPI STANDARD
            if (field != null && !string.IsNullOrEmpty(field.valore))
            {
                if (!string.IsNullOrEmpty(field.nomeCampo))
                    wasFieldOrderingApplied = true;

                switch (field.nomeCampo)
                {
                    // Doc
                    case "D1":
                        query = query.OrderByDescending(a => a.DOCNUMBER == null ? 0 : 1).OrderByFieldDirection(directionIsDescending, a => a.DOCNUMBER);
                        break;
                    // Registro
                    case "D2":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(IPi3DbContextMappedFunctions.GetCodReg(a.ID_REGISTRO.GetValueOrDefault())) ? 0 : 1).OrderByFieldDirection(directionIsDescending, a => IPi3DbContextMappedFunctions.GetCodReg(a.ID_REGISTRO.GetValueOrDefault()));
                        break;
                    // Tipo
                    case "D3":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(a.CHA_TIPO_PROTO.ToUpper().Trim()) ? 0 : 1).OrderByFieldDirection(directionIsDescending, a => a.CHA_TIPO_PROTO.ToUpper().Trim());
                        break;
                    // Oggetto
                    case "D4":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(a.VAR_PROF_OGGETTO.ToUpper().Trim()) ? 0 : 1).OrderByFieldDirection(directionIsDescending, a => a.VAR_PROF_OGGETTO.ToUpper().Trim());
                        break;
                    // Mitt/Dest
                    case "D5":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(IPi3DbContextMappedFunctions.CorrCat(a.SYSTEM_ID, a.CHA_TIPO_PROTO)) ? 0 : 1).OrderByFieldDirection(directionIsDescending, a => IPi3DbContextMappedFunctions.CorrCat(a.SYSTEM_ID, a.CHA_TIPO_PROTO)); ;
                        break;
                    // Mittente
                    case "D6":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(IPi3DbContextMappedFunctions.CorrCatByTipo(a.DOCNUMBER.GetValueOrDefault(), a.CHA_TIPO_PROTO, "M")) ? 0 : 1).OrderByFieldDirection(directionIsDescending, a => IPi3DbContextMappedFunctions.CorrCatByTipo(a.DOCNUMBER.GetValueOrDefault(), a.CHA_TIPO_PROTO, "M"));
                        break;
                    // Destinatari
                    case "D7":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(IPi3DbContextMappedFunctions.CorrCatByTipo(a.DOCNUMBER.GetValueOrDefault(), a.CHA_TIPO_PROTO, "D")) ? 0 : 1).OrderByFieldDirection(directionIsDescending, a => IPi3DbContextMappedFunctions.CorrCatByTipo(a.DOCNUMBER.GetValueOrDefault(), a.CHA_TIPO_PROTO, "D"));
                        break;
                    // Segnatura
                    case "D8":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(a.VAR_SEGNATURA.ToUpper().Trim()) ? 0 : 1).OrderByFieldDirection(directionIsDescending, a => a.VAR_SEGNATURA.ToUpper().Trim());
                        break;
                    // Data protocollazione / Creazione
                    case "D9":
                        query = query.OrderByDescending(p => 1).OrderByFieldDirection(directionIsDescending, p => 1);
                        break;
                    // Esito pubblicazione
                    case "D10":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(IPi3DbContextMappedFunctions.GetEsitoPubblicazione(a.SYSTEM_ID)) ? 0 : 1).OrderByFieldDirection(directionIsDescending, a => IPi3DbContextMappedFunctions.GetEsitoPubblicazione(a.SYSTEM_ID));
                        break;
                    // Data annullamento
                    case "D11":
                        query = query.OrderByDescending(a => a.DTA_ANNULLA == null ? 0 : 1).OrderByFieldDirection(directionIsDescending, a => a.DTA_ANNULLA);
                        break;
                    // Num. Prot.
                    case "D12":
                        query = query.OrderByDescending(a => a.NUM_PROTO == null ? 0 : 1).OrderByFieldDirection(directionIsDescending, a => a.NUM_PROTO);
                        break;
                    // Codice autore
                    case "D13":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(IPi3DbContextMappedFunctions.GetPeopleUserId(a.AUTHOR.GetValueOrDefault()).ToUpper().Trim()) ? 0 : 1).OrderByFieldDirection(directionIsDescending, a => IPi3DbContextMappedFunctions.GetPeopleUserId(a.AUTHOR.GetValueOrDefault()).ToUpper().Trim());
                        break;
                    // Data archiviazione
                    case "D14":
                        query = query.OrderByDescending(a => a.ARCHIVE_DATE == null ? 0 : 1).OrderByFieldDirection(directionIsDescending, a => a.ARCHIVE_DATE);
                        break;
                    // Personale
                    case "D15":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(a.CHA_PERSONALE) ? 0 : 1).OrderByFieldDirection(directionIsDescending, a => a.CHA_PERSONALE);
                        break;
                    // Privato
                    case "D16":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(a.CHA_PRIVATO) ? 0 : 1).OrderByFieldDirection(directionIsDescending, a => a.CHA_PRIVATO);
                        break;
                    // Cod. Fascicoli
                    case "D18":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(IPi3DbContextMappedFunctions.ClassCat(a.SYSTEM_ID)) ? 0 : 1).OrderByFieldDirection(directionIsDescending, a => IPi3DbContextMappedFunctions.ClassCat(a.SYSTEM_ID));
                        break;
                    // Nome e cognome autore
                    case "D19":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(IPi3DbContextMappedFunctions.GetPeopleName(a.AUTHOR.GetValueOrDefault())) ? 0 : 1).OrderByFieldDirection(directionIsDescending, a => IPi3DbContextMappedFunctions.GetPeopleName(a.AUTHOR.GetValueOrDefault()));
                        break;
                    // Ruolo autore
                    case "D20":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(IPi3DbContextMappedFunctions.GetDescCorr(a.ID_RUOLO_CREATORE.GetValueOrDefault())) ? 0 : 1).OrderByFieldDirection(directionIsDescending, a => IPi3DbContextMappedFunctions.GetDescCorr(a.ID_RUOLO_CREATORE.GetValueOrDefault()));
                        break;
                    // Data arrivo
                    case "D21":
                        query = query.OrderByDescending(a => IPi3DbContextMappedFunctions.GetDataArrivoDoc(a.DOCNUMBER.GetValueOrDefault()) == null ? 0 : 1).OrderByFieldDirection(directionIsDescending, a => IPi3DbContextMappedFunctions.GetDataArrivoDoc(a.DOCNUMBER.GetValueOrDefault()));
                        break;
                    // Nome e cognome protocollatore
                    case "D27":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(IPi3DbContextMappedFunctions.GetPeopleName(a.ID_PEOPLE_PROT.GetValueOrDefault())) ? 0 : 1).OrderByFieldDirection(directionIsDescending, a => IPi3DbContextMappedFunctions.GetPeopleName(a.ID_PEOPLE_PROT.GetValueOrDefault()));
                        break;
                    // Ruolo protocollatore
                    case "D28":
                        if (directionIsDescending)
                            query = query.OrderByDescending(a => string.IsNullOrEmpty(IPi3DbContextMappedFunctions.GetDescCorr(a.ID_RUOLO_CREATORE.GetValueOrDefault())) ? 0 : 1).ThenByDescending(a => IPi3DbContextMappedFunctions.GetDescCorr(a.ID_RUOLO_CREATORE.GetValueOrDefault()));
                        else
                            query = query.OrderByDescending(a => string.IsNullOrEmpty(IPi3DbContextMappedFunctions.GetDescCorr(a.ID_RUOLO_CREATORE.GetValueOrDefault())) ? 0 : 1).ThenBy(a => IPi3DbContextMappedFunctions.GetDescCorr(a.ID_RUOLO_CREATORE.GetValueOrDefault()));
                        break;
                    // Tipologia
                    case "U1":
                        var tempQueryTipo = query.Join(pi3DbContext.TipoAttoEntities.AsNoTracking(), p => p.ID_TIPO_ATTO, t => t.SYSTEM_ID, (p, t) =>
                        new
                        {
                            p,
                            t.VAR_DESC_ATTO
                        });
                        if (directionIsDescending)
                            tempQueryTipo = tempQueryTipo.OrderByDescending(r => r.VAR_DESC_ATTO != null).ThenByDescending(r => r.VAR_DESC_ATTO.ToUpper()).ThenByDescending(p => p.p.DTA_PROTO != null ? p.p.DTA_PROTO : p.p.CREATION_TIME);
                        else
                            tempQueryTipo = tempQueryTipo.OrderByDescending(r => r.VAR_DESC_ATTO != null).OrderBy(r => r.VAR_DESC_ATTO.ToUpper()).ThenByDescending(p => p.p.DTA_PROTO != null ? p.p.DTA_PROTO : p.p.CREATION_TIME);

                        query = tempQueryTipo.Select(r => r.p);
                        break;
                    // Esito Spedizione
                    case "esito_spedizione":
                        query = query.OrderByDescending(a => string.IsNullOrEmpty(IPi3DbContextMappedFunctions.GetEsitoSpedizione(a.SYSTEM_ID)) ? 0 : 1).OrderByFieldDirection(directionIsDescending, a => IPi3DbContextMappedFunctions.GetEsitoSpedizione(a.SYSTEM_ID));
                        break;
                    // Num. Ricevute
                    case "count_ric_interop":
                        string tipoRicevuta = string.Empty;
                        var filtroRic = request.GetValoreFiltroRicerca<string>("DOC_SPEDITI");

                        if (!string.IsNullOrEmpty(filtroRic))
                            tipoRicevuta = filtroRic;

                        query = query.OrderByDescending(a => string.IsNullOrEmpty(IPi3DbContextMappedFunctions.GetCountRicevuteInterop(a.SYSTEM_ID, tipoRicevuta)) ? 0 : 1).OrderByFieldDirection(directionIsDescending, a => IPi3DbContextMappedFunctions.GetCountRicevuteInterop(a.SYSTEM_ID, tipoRicevuta));
                        break;
                    default:
                        break;

                }
            }
            // CAMPI PROFILATI
            else if (!string.IsNullOrEmpty(orderByForProfiledField))
            {
                wasFieldOrderingApplied = true;

                var fieldTemp = request.visibleFieldsTemplate.Where(e => e.CustomObjectId.ToString() == orderByForProfiledField).FirstOrDefault();

                if (!(contatoreNoCustom != null && !request.showGridPersonalization) && fieldTemp != null)
                {
                    if (fieldTemp.IsNumber)
                    {

                        if (directionIsDescending)
                            query = query.OrderByDescending(a => IPi3DbContextMappedFunctions.GetValCampoProfDocOrder(a.DOCNUMBER.GetValueOrDefault(), orderByForProfiledField.AsLong()) == null ? -1 : 0)
                                .ThenByDescending(a => IPi3DbContextMappedFunctions.GetValCampoProfDocOrder(a.DOCNUMBER.GetValueOrDefault(), orderByForProfiledField.AsLong()))
                                .ThenByDescending(p => p.DTA_PROTO != null ? p.DTA_PROTO : p.CREATION_TIME);

                        else
                        {
                            query = query.OrderByDescending(a => IPi3DbContextMappedFunctions.GetValCampoProfDocOrder(a.DOCNUMBER.GetValueOrDefault(), orderByForProfiledField.AsLong()) == null ? -1 : 0)
                                .ThenBy(a => IPi3DbContextMappedFunctions.GetValCampoProfDocOrder(a.DOCNUMBER.GetValueOrDefault(), orderByForProfiledField.AsLong()))
                                .ThenByDescending(p => p.DTA_PROTO != null ? p.DTA_PROTO : p.CREATION_TIME);
                        }
                    }
                    else
                    {
                        if (directionIsDescending)
                            query = query.OrderByDescending(a => IPi3DbContextMappedFunctions.GetValCampoProfDoc(a.DOCNUMBER.GetValueOrDefault(), orderByForProfiledField.AsLong()) == null ? -1 : 0)
                                .ThenByDescending(a => IPi3DbContextMappedFunctions.GetValCampoProfDoc(a.DOCNUMBER.GetValueOrDefault(), orderByForProfiledField.AsLong()))
                                .ThenByDescending(p => p.DTA_PROTO != null ? p.DTA_PROTO : p.CREATION_TIME);
                        else
                        {
                            query = query.OrderByDescending(a => IPi3DbContextMappedFunctions.GetValCampoProfDoc(a.DOCNUMBER.GetValueOrDefault(), orderByForProfiledField.AsLong()) == null ? -1 : 0)
                                .ThenBy(a => IPi3DbContextMappedFunctions.GetValCampoProfDoc(a.DOCNUMBER.GetValueOrDefault(), orderByForProfiledField.AsLong()))
                                .ThenByDescending(p => p.DTA_PROTO != null ? p.DTA_PROTO : p.CREATION_TIME);
                        }
                    }
                }
            }

            if (!wasFieldOrderingApplied)
                query = query.OrderByDescending(p => p.DTA_PROTO != null ? p.DTA_PROTO : p.CREATION_TIME);

            return query;
        }

    }

    internal static class FascicolazioneGetDocumentiPagingWithFiltersCustomRequestExtensions
    {
        public static bool HasFiltroRicerca(this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request, string argomento)
        {
            return request.filtriRicerca.Any(f => f.Any(f2 => f2.argomento == argomento));
        }

        public static DocsPaVO.filtri.FiltroRicerca? FindFiltroRicerca(this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request, string argomento)
        {
            return request.filtriRicerca?[0]
                .Where(f => f.argomento == argomento)
                .Select(f => f)
                .FirstOrDefault();
        }

        public static T GetValoreFiltroRicerca<T>(this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request, string argomento)
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

        public static T GetValoreFiltroOrder<T>(this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request, string argomento)
        {
            var filtroRicerca = FindFiltroOrder(request, argomento);
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

        public static DocsPaVO.filtri.FiltroRicerca? FindFiltroOrder(this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request, string argomento)
        {
            return request.orderRicerca?[0]
                .Where(f => f.argomento == argomento)
                .Select(f => f)
                .FirstOrDefault();
        }

        private static (DateTime, DateTime) GetDateRangeLast7Days(DateTime dateTime)
        {
            var currentDate = dateTime.AddDays(-7);
            var initDate = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0);
            var endDate = initDate.AddHours(23).AddMinutes(59).AddSeconds(59);

            return (initDate, endDate);
        }

        private static (DateTime, DateTime) GetDateRangeLast31Days(DateTime dateTime)
        {
            var currentDate = dateTime.AddDays(-31);
            var initDate = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0);
            var endDate = initDate.AddHours(23).AddMinutes(59).AddSeconds(59);

            return (initDate, endDate);
        }

        private static (DateTime, DateTime) GetDateRangeYear(int year)
        {
            var initDate = new DateTime(year, 1, 1, 0, 0, 0);
            var endDate = initDate.AddDays(365).AddHours(23).AddMinutes(59).AddSeconds(59);

            return (initDate, endDate);
        }

        private static (DateTime, DateTime) GetDateRangeIl(DateTime dateTime)
        {
            var initDate = new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0);
            var endDate = initDate.AddHours(23).AddMinutes(59).AddSeconds(59);

            return (initDate, endDate);
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

        public static async Task AppendFiltroIdFascicolo(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            context.Query = context.Query.Where(p => context.Pi3DbContext.ProjectComponentEntities.AsNoTracking()
                                .Where(pc => pc.LINK == p.SYSTEM_ID && pc.PROJECT_ID == request.folder.systemID.AsLong())
                                .Any());
        }

        public static async Task AppendFiltroSecurity(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            context.Query = context.Query.Where(p => context.Pi3DbContext.SecurityEntities.AsNoTracking()
                                .Where(s => s.THING == p.SYSTEM_ID
                                        && s.ACCESSRIGHTS > 0
                                        && (s.PERSONORGROUP == request.infoUtente.idGruppo.AsLong()
                                        || s.PERSONORGROUP == request.infoUtente.idPeople.AsLong()))
                                .Select(s => s.THING)
                                .Any());
        }

        public static async Task AppendFiltroTipoProto(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            var filtro = request.FindFiltroRicerca("TIPO");

            if (filtro != null)
            {
                if (filtro.valore == "T")
                {
                    context.Query = context.Query.Where(p => 
                        p.CHA_TIPO_PROTO == "A" 
                        || p.CHA_TIPO_PROTO == "I" 
                        || p.CHA_TIPO_PROTO == "P" 
                        || p.CHA_TIPO_PROTO == "G");
                }
                else 
                {
                    context.Query = context.Query.Where(p =>
                        p.CHA_TIPO_PROTO == filtro.valore);
                }
            }
        }

        public static async Task AppendFiltroTipoAtto(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("TIPO_ATTO");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p =>
                        p.ID_TIPO_ATTO == filtro);
            }
        }

        public static async Task AppendFiltroProfilazioneDinamica(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            var filtro = request.FindFiltroRicerca("PROFILAZIONE_DINAMICA");

            if (filtro != null)
            {
                // TODO
            }
        }

        public static async Task AppendFiltroSegnatura(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("SEGNATURA");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(p =>
                        p.VAR_SEGNATURA == filtro);

            }
        }

        public static async Task AppendFiltroNumeroProtocollo(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("NUM_PROTOCOLLO");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.NUM_PROTO == filtro);
            }
        }

        public static async Task AppendFiltroNumeroProtocolloDal(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("NUM_PROTOCOLLO_DAL");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.NUM_PROTO >= filtro);
            }
        }

        public static async Task AppendFiltroNumeroProtocolloAl(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("NUM_PROTOCOLLO_AL");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.NUM_PROTO <= filtro);
            }
        }

        public static async Task AppendFiltroDataProtIl(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_PROT_IL");

            if (filtro > DateTime.MinValue)
            {
                var range = GetDateRangeIl(filtro);

                context.Query = context.Query.Where(p => p.DTA_PROTO >= range.Item1 && p.DTA_PROTO <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataProtSuccessivaAl(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_PROT_SUCCESSIVA_AL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDateSuccessivaAl(filtro);

                context.Query = context.Query.Where(p => p.DTA_PROTO >= initDate);
            }
        }

        public static async Task AppendFiltroDataProtPrecedenteIl(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_PROT_PRECEDENTE_IL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDatePrecedenteIl(filtro);

                context.Query = context.Query.Where(p => p.DTA_PROTO <= initDate);
            }
        }

        public static async Task AppendFiltroAnnoProtocollo(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("ANNO_PROTOCOLLO");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.NUM_ANNO_PROTO == filtro);
            }
        }

        public static async Task AppendFiltroDocNumber(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("DOCNUMBER");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.DOCNUMBER == filtro);
            }
        }

        public static async Task AppendFiltroDocNumberDal(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("DOCNUMBER_DAL");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.DOCNUMBER >= filtro);
            }
        }

        public static async Task AppendFiltroDocNumberAl(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("DOCNUMBER_AL");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.DOCNUMBER <= filtro);
            }
        }

        public static async Task AppendFiltroDataCreazioneIl(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_CREAZIONE_IL");

            if (filtro > DateTime.MinValue)
            {
                var range = GetDateRangeIl(filtro);

                context.Query = context.Query.Where(p => p.CREATION_DATE >= range.Item1 && p.CREATION_DATE <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataCreazioneSuccessivaAl(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_CREAZIONE_SUCCESSIVA_AL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDateSuccessivaAl(filtro);

                context.Query = context.Query.Where(p => p.CREATION_DATE >= initDate);
            }
        }

        public static async Task AppendFiltroDataCreazionePrecedenteIl(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_CREAZIONE_PRECEDENTE_IL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDatePrecedenteIl(filtro);

                context.Query = context.Query.Where(p => p.CREATION_DATE <= initDate);
            }
        }

        public static async Task AppendFiltroOggetto(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("OGGETTO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(p => p.VAR_PROF_OGGETTO.ToUpper().Contains(filtro.ToUpper()));
            }
        }

        public static async Task AppendFiltroIdMittDest(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("ID_MITT_DEST");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p =>
                    context.Pi3DbContext.DocArrivoParEntities.AsNoTracking()
                        .Where(dap => dap.ID_PROFILE == p.SYSTEM_ID
                                && dap.ID_MITT_DEST == filtro
                                && dap.CHA_TIPO_MITT_DEST == "M")
                        .Any());
            }
        }

        public static async Task AppendFiltroMittDest(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("MITT_DEST");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                string value = filtro.ToUpper();
                string valueA = value;
                if (valueA.Contains("&&"))
                    valueA = valueA.Replace("&&", "");
                bool casoA = false;
                if (value.Substring(0, value.Length - 1).Contains("%") && !value.Substring(0, value.Length - 1).Contains("%&&"))
                    casoA = true;
                if (value.Contains("&&"))
                {
                    string result = string.Empty;
                    foreach (string filter in new Regex("&&", RegexOptions.None, TimeSpan.FromSeconds(5)).Split(value))
                        if (!string.IsNullOrEmpty(filter))
                            result += filter + " AND ";
                    value = result.Substring(0, result.Length - 5);
                }
                if (value.Contains("%") && value.IndexOf("%") != value.Length - 1)
                {
                    bool finale = value.EndsWith("%");
                    string result = string.Empty;
                    foreach (string filter in new Regex("%", RegexOptions.None, TimeSpan.FromSeconds(5)).Split(value))
                        if (!string.IsNullOrEmpty(filter))
                            result += filter + "% AND ";
                    value = result.Substring(0, result.Length - 6);
                    if (finale)
                        value = value + "%";
                }
                if (value.ToUpper().Contains(" AND  AND "))
                    value = value.ToUpper().Replace(" AND  AND ", " AND ");
                context.Query = context.Query.Where(p =>
                                    context.Pi3DbContext.DocArrivoParEntities.AsNoTracking()
                                        .Join(context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking(),
                                            ap => ap.ID_MITT_DEST,
                                            cg => cg.SYSTEM_ID,
                                            (ap, cg) => new { ap = ap, cg = cg })
                                        .Where(r => r.ap.ID_PROFILE == p.SYSTEM_ID
                                                && context.Pi3DbContext.CorrGlobaliFullText(value).AsNoTracking().Any(f => f.SYSTEM_ID == r.cg.SYSTEM_ID)
                                                && (casoA ? EF.Functions.Like(r.cg.VAR_DESC_CORR.ToUpper(), $"%{valueA.ToUpper()}%") : true))
                                        .Any());
            }
        }

        public static async Task AppendFiltroIdDestinatario(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("ID_DESTINATARIO");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p =>
                    context.Pi3DbContext.DocArrivoParEntities.AsNoTracking()
                        .Where(dap => dap.ID_PROFILE == p.SYSTEM_ID
                                && dap.ID_MITT_DEST == filtro
                                && (dap.CHA_TIPO_MITT_DEST == "D" 
                                        || dap.CHA_TIPO_MITT_DEST == "C"
                                        || dap.CHA_TIPO_MITT_DEST == "F"))
                        .Any());
            }
        }

        public static async Task AppendFiltroDescrizioneDestinatario(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("ID_DESCR_DESTINATARIO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                string value = filtro.ToUpper();
                string valueA = value;
                if (valueA.Contains("&&"))
                    valueA = valueA.Replace("&&", "");
                bool casoA = false;
                if (value.Substring(0, value.Length - 1).Contains("%") && !value.Substring(0, value.Length - 1).Contains("%&&"))
                    casoA = true;
                if (value.Contains("&&"))
                {
                    string result = string.Empty;
                    foreach (string filter in new Regex("&&", RegexOptions.None, TimeSpan.FromSeconds(5)).Split(value))
                        if (!string.IsNullOrEmpty(filter))
                            result += filter + " AND ";
                    value = result.Substring(0, result.Length - 5);
                }
                if (value.Contains("%") && value.IndexOf("%") != value.Length - 1)
                {
                    bool finale = value.EndsWith("%");
                    string result = string.Empty;
                    foreach (string filter in new Regex("%", RegexOptions.None, TimeSpan.FromSeconds(5)).Split(value))
                        if (!string.IsNullOrEmpty(filter))
                            result += filter + "% AND ";
                    value = result.Substring(0, result.Length - 6);
                    if (finale)
                        value = value + "%";
                }
                if (value.ToUpper().Contains(" AND  AND "))
                    value = value.ToUpper().Replace(" AND  AND ", " AND ");
                context.Query = context.Query.Where(p =>
                                    context.Pi3DbContext.DocArrivoParEntities.AsNoTracking()
                                        .Join(context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking(),
                                            ap => ap.ID_MITT_DEST,
                                            cg => cg.SYSTEM_ID,
                                            (ap, cg) => new { ap = ap, cg = cg })
                                        .Where(r => r.ap.ID_PROFILE == p.SYSTEM_ID
                                                && (r.ap.CHA_TIPO_MITT_DEST == "D"
                                                    || r.ap.CHA_TIPO_MITT_DEST == "C"
                                                    || r.ap.CHA_TIPO_MITT_DEST == "F")
                                                && context.Pi3DbContext.CorrGlobaliFullText(value).AsNoTracking().Any(f => f.SYSTEM_ID == r.cg.SYSTEM_ID)
                                                && (casoA ? EF.Functions.Like(r.cg.VAR_DESC_CORR.ToUpper(), $"%{valueA.ToUpper()}%") : true))
                                        .Any());
            }
        }

        public static async Task AppendFiltroFirmato(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("FIRMATO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(p =>
                            context.Pi3DbContext.ComponentEntities.AsNoTracking()
                                .Where(c => c.DOCNUMBER == p.DOCNUMBER &&
                                    context.Pi3DbContext.VersionEntities.AsNoTracking()
                                        .Join(context.Pi3DbContext.ComponentEntities.AsNoTracking(),
                                            v1 => v1.VERSION_ID,
                                            c2 => c2.VERSION_ID,
                                            (v1, c2) => new { v1, c2 })
                                        .Where(vc => vc.v1.DOCNUMBER == p.DOCNUMBER && vc.c2.FILE_SIZE > 0)
                                        .OrderByDescending(vc => vc.v1.VERSION_ID)
                                        .Select(vc => vc.v1.VERSION_ID.Value)
                                        .First() == c.VERSION_ID
                                        && c.CHA_FIRMATO == filtro)
                                .Any());
            }
        }

        public static async Task AppendFiltroTipoFileAcquisito(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("TIPO_FILE_ACQUISITO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(p =>
                            context.Pi3DbContext.ComponentEntities.AsNoTracking()
                                .Where(c => c.DOCNUMBER == p.DOCNUMBER &&
                                    context.Pi3DbContext.VersionEntities.AsNoTracking()
                                        .Join(context.Pi3DbContext.ComponentEntities.AsNoTracking(),
                                            v1 => v1.VERSION_ID,
                                            c2 => c2.VERSION_ID,
                                            (v1, c2) => new { v1, c2 })
                                        .Where(vc => vc.v1.DOCNUMBER == p.DOCNUMBER && vc.c2.EXT.ToUpper() == filtro.ToUpper())
                                        .OrderByDescending(vc => vc.v1.VERSION_ID)
                                        .Select(vc => vc.v1.VERSION_ID.Value)
                                        .First() == c.VERSION_ID
                                        && c.CHA_FIRMATO == filtro)
                                .Any());
            }
        }

        public static async Task AppendFiltroExport(
             this FascicolazioneGetDocumentiPagingWithFiltersCustomRequest request,
             ProfileProjectSearchAppendContext context)
        {
            if (request.export
                && request.documentsSystemId != null
                && request.documentsSystemId.Length > 0)
            {
                var predicate = PredicateBuilder.New<ProfileEntity>();

                foreach (var id in request.documentsSystemId.Select(id => id.AsLong()))
                    predicate = predicate.Or(p => p.SYSTEM_ID == id);

                context.Query = context.Query.Where(predicate);
            }
        }

    }
}