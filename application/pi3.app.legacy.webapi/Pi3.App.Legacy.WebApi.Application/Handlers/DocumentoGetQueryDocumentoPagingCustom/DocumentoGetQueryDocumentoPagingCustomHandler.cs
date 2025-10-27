// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.filtri;
using DocsPaVO.Grids;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.ricerche;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Vml.Office;
using DocumentFormat.OpenXml.Wordprocessing;
using LinqKit;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using Microsoft.IdentityModel.Protocols.WsTrust;
using Microsoft.IdentityModel.Tokens;
using Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneGetListaFascicoliPagingCustom;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Chilkat.Http;
using DocumentoGetQueryDocumentoPagingCustomRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetQueryDocumentoPagingCustom;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetQueryDocumentoPagingCustom
{
    public class DocumentoGetQueryDocumentoPagingCustomHandler : IRequestHandler<DocumentoGetQueryDocumentoPagingCustomRequest, DocumentoGetQueryDocumentoPagingCustomResult>
    {
        #region Public Members

        public DocumentoGetQueryDocumentoPagingCustomHandler(
            ILogger<DocumentoGetQueryDocumentoPagingCustomHandler> logger,
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

        public async Task<DocumentoGetQueryDocumentoPagingCustomResult> Handle(DocumentoGetQueryDocumentoPagingCustomRequest request, CancellationToken cancellationToken)
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
                var useTxtIndex = await GetConfigTextIndex();
                var query =
                    this._pi3DbContext.ProfileEntities
                        .AsNoTracking();

                var appendContext = new ProfileSearchAppendContext(this._pi3DbContext, query);
                await request.AppendFiltroExport(appendContext);

                if (request.security)
                    await request.AppendFiltroSecurity(appendContext);

                //await request.AppendFiltroDaProto(appendContext);
                await request.AppendFiltroTipoProto(appendContext);
                await request.AppendFiltroTipo(appendContext);
                await request.AppendFiltroDocInAdl(appendContext);
                await request.AppendFiltroNumeroProtocollo(appendContext);
                await request.AppendFiltroNumeroProtocolloDal(appendContext);
                await request.AppendFiltroNumeroProtocolloAl(appendContext);
                await request.AppendFiltroIdMittDest(appendContext);
                await request.AppendFiltroCodMittDest(appendContext);
                await request.AppendFiltroMittDest(appendContext);
                await request.AppendFiltroDescAuthor(appendContext);               
                await request.AppendFiltroDataScadenzaIl(appendContext);
                await request.AppendFiltroDataScadenzaSuccessivaAl(appendContext);
                await request.AppendFiltroDataScadenzaPrecedenteIl(appendContext);
                await request.AppendFiltroDataScadenzaSC(appendContext);
                await request.AppendFiltroDataScadenzaMC(appendContext);
                await request.AppendFiltroDataScadenzaToday(appendContext);
                await request.AppendFiltroDataCreazioneIl(appendContext);
                await request.AppendFiltroDataCreazioneSuccessivaAl(appendContext);
                await request.AppendFiltroDataCreazionePrecedenteIl(appendContext);
                await request.AppendFiltroDataCreazioneSC(appendContext);
                await request.AppendFiltroDataCreazioneMC(appendContext);
                await request.AppendFiltroDataCreazioneToday(appendContext);
                await request.AppendFiltroDataCreazioneYesterday(appendContext);
                await request.AppendFiltroDataCreazioneUltimi7Giorni(appendContext);
                await request.AppendFiltroDataCreazioneUltimi31Giorni(appendContext);
                await request.AppendFiltroDataProtIl(appendContext);
                await request.AppendFiltroDataProtSuccessivaAl(appendContext);
                await request.AppendFiltroDataProtPrecedenteIl(appendContext);
                await request.AppendFiltroDataProtSC(appendContext);
                await request.AppendFiltroDataProtMC(appendContext);
                await request.AppendFiltroDataProtToday(appendContext);
                await request.AppendFiltroDataProtYesterday(appendContext);
                await request.AppendFiltroDataProtUltimi7Giorni(appendContext);
                await request.AppendFiltroDataProtUltimi31Giorni(appendContext);
                await request.AppendFiltroAnnoProtocollo(appendContext);
                await request.AppendFiltroDocNumber(appendContext);
                await request.AppendFiltroDocNumberDal(appendContext);
                await request.AppendFiltroDocNumberAl(appendContext);
                await request.AppendFiltroIdOggetto(appendContext);
                await request.AppendFiltroOggetto(appendContext);
                await request.AppendFiltroSearchDocumentSimple(appendContext);
                await request.AppendFiltroOggettoAllegato(appendContext);
                await request.AppendFiltroRegistro(appendContext);
                await request.AppendFiltroTipoDocumento(appendContext);
                await request.AppendFiltroCodExtApp(appendContext);
                await request.AppendFiltroParoleChiave(appendContext);
                await request.AppendFiltroTipoAtto(appendContext);
                await request.AppendFiltroNote(appendContext);
                await request.AppendFiltroFirmatarioNome(appendContext);
                await request.AppendFiltroFirmatarioCognome(appendContext);
                await request.AppendFiltroEvidenza(appendContext);
                await request.AppendFiltroInChildRicEstesa(appendContext);
                await request.AppendFiltroEstendiANodiFigliEFascicoli(appendContext);
                await request.AppendFiltroMancanzaImmagine(appendContext);
                await request.AppendFiltroMancanzaFascicolazione(appendContext);
                await request.AppendFiltroTrasmessiCon(appendContext);
                await request.AppendFiltroTrasmessiSenza(appendContext);
                //await request.AppendFiltroDocMaiTrasmessiDaUtente(appendContext);
                //await request.AppendFiltroDocMaiTrasmessiDaRuolo(appendContext);
                await request.AppendFiltroProfilazioneDinamica(appendContext);
                await request.AppendFiltroDiagrammaStatoDoc(appendContext);
                await request.AppendFiltroCodiceFascicolo(appendContext);
                await request.AppendFiltroIdTitolario(appendContext);
                await request.AppendFiltroNumProtTitolario(appendContext);
                await request.AppendFiltroFirmato(appendContext);
                await request.AppendFiltroIdParent(appendContext);
                await request.AppendFiltroIdRepertorio(appendContext);
                await request.AppendFiltroIdAuthor(appendContext);
                await request.AppendFiltroIdOwner(appendContext);
                await request.AppendFiltroDescOwner(appendContext);             
                await request.AppendFiltroFirmaElettronica(appendContext);
                await request.AppendFiltroTipoFileAcquisito(appendContext);
                await request.AppendFiltroMezzoSpedizione(appendContext);
                await request.AppendFiltroDtaProtoMitt(appendContext);
                await request.AppendFiltroDtaProtoMittPrecIl(appendContext);
                await request.AppendFiltroDtaProtoMittSuccAl(appendContext);
                await request.AppendFiltroDtaProtoMittSettCorr(appendContext);
                await request.AppendFiltroDtaProtoMittMeseCorr(appendContext);
                await request.AppendFiltroDtaProtoMittToday(appendContext);
                await request.AppendFiltroProtoMitt(appendContext, useTxtIndex);
                await request.AppendFiltroIdUoProto(appendContext);
                await request.AppendFiltroIdRuoProto(appendContext);
                await request.AppendFiltroIdPeopleProto(appendContext);
                await request.AppendFiltroDescProto(appendContext, useTxtIndex);
                await request.AppendFiltroIdMittenteIntermedio(appendContext);
                await request.AppendFiltroMittenteIntermedio(appendContext);
                await request.AppendFiltroDtaArrivoIl(appendContext);
                await request.AppendFiltroDtaArrivoPrecedenteIl(appendContext);
                await request.AppendFiltroDtaArrivoSuccessivaAl(appendContext);
                await request.AppendFiltroDtaArrivoSettCorr(appendContext);
                await request.AppendFiltroDtaArrivoMeseCorr(appendContext);
                await request.AppendFiltroDtaArrivoToday(appendContext);
                await request.AppendFiltroNumProtoEmerg(appendContext);
                await request.AppendFiltroDataProtoEmergenzaIl(appendContext);
                await request.AppendFiltroDataProtoEmergenzaIl(appendContext);
                await request.AppendFiltroMancanzaImg(appendContext);
                await request.AppendFiltroMancanzaFasc(appendContext);
                await request.AppendFiltroDocMaiSpediti(appendContext);
                await request.AppendFiltroDocMaiTrasmessiMaiSpeditiAiDestinatari(appendContext);
                await request.AppendFiltroAnnullato(appendContext);
                await request.AppendFiltroSegnatura(appendContext);
                await request.AppendFiltroStatoConservazione(appendContext);
                await request.AppendFiltroDataVersamentoDa(appendContext);
                await request.AppendFiltroDataVersamentoA(appendContext);
                await request.AppendFiltroDataVersamentoIl(appendContext);
                await request.AppendFiltroCodicePolicy(appendContext);
                await request.AppendFiltroNumeroEsecuzionePolicy(appendContext);
                await request.AppendFiltroDataEsecuzionePolicyIl(appendContext);
                await request.AppendFiltroDataEsecuzionePolicySuccessivaAl(appendContext);
                await request.AppendFiltroDataEsecuzionePolicyPrecedenteIl(appendContext);
                await request.AppendFiltroDataEsecuzionePolicyYesterday(appendContext);
                await request.AppendFiltroStatoConsolidamento(appendContext);
                await request.AppendFiltroDataConsolidamentoDa(appendContext);
                await request.AppendFiltroDataConsolidamentoA(appendContext);
                await request.AppendFiltroIdUtenteConsolidante(appendContext);
                await request.AppendFiltroIdRuoloConsolidante(appendContext);
                await request.AppendFiltroNumAllegatiTipo(appendContext);
                await request.AppendFiltroRicevute(appendContext);
                await request.AppendFiltroNonConforme(appendContext);
                // riprendere da TIPO_FILE_ACQUISITO, in getquerycondcoumni

                //Filtri per le stampe di registro protocollo/repertorio
                if (request.GetValoreFiltroRicerca<string>("TIPO") == "C" || request.GetValoreFiltroRicerca<string>("TIPO") == "R")
                {
                    await request.AppendFiltroDataStampaRegistroProtocollo(appendContext);
                    await request.AppendFiltroDataStampaRegistroProtocolloDal(appendContext);
                    await request.AppendFiltroDataStampaRegistroProtocolloAl(appendContext);
                    await request.AppendFiltroDataStampaRegistroProtocolloSC(appendContext);
                    await request.AppendFiltroDataStampaRegistroProtocolloMC(appendContext);
                    await request.AppendFiltroDataStampaRegistroProtocolloToday(appendContext);
                    await request.AppendFiltroAnnoProtocolloStampa(appendContext);
                    await request.AppendFiltroNumProtocolloStampa(appendContext);
                    await request.AppendFiltroNumProtocolloStampaDal(appendContext);
                    await request.AppendFiltroNumProtocolloStampaAl(appendContext);
                    await request.AppendFiltroDataStampaRegistroRepertorio(appendContext);
                    await request.AppendFiltroDataStampaRegistroRepertorioDal(appendContext);
                    await request.AppendFiltroDataStampaRegistroRepertorioAl(appendContext);
                    await request.AppendFiltroAnnoRepertorioStampa(appendContext);
                    await request.AppendFiltroNumRepertorioStampa(appendContext);
                    await request.AppendFiltroNumRepertorioStampaDal(appendContext);
                    await request.AppendFiltroNumRepertorioStampaAl(appendContext);
                    await request.AppendFiltroStampaRepertorioFirmata(appendContext);
                    await request.AppendFiltroStampaIdRepertorio(appendContext);
                }
                await request.AppendFiltroAllegato(appendContext);

                query = appendContext.Query;

                var ids = await query
                    .Select(p =>
                    new
                    {
                        SYSTEM_ID = p.SYSTEM_ID,
                        CODICE = p.NUM_PROTO.HasValue ? p.NUM_PROTO : p.DOCNUMBER
                    })
                    .ToListAsync();

                nRec = ids.Count();



                if (request.getIdProfilesList)
                {
                    idProfiles = ids.Select(itm => new SearchResultInfo()
                    {
                        Id = itm.SYSTEM_ID.ToString(),
                        Codice = itm.CODICE.ToString()
                    }).ToList();
                }

                numTotPage = nRec / request.pageSize;
                if ((nRec % request.pageSize) > 0)
                    numTotPage++;

                var maxRows = await GetConfigMaxRowsSearchable(idTenant.ToString());
                if (maxRows < nRec)
                {
                    numTotPage = -2;
                }
                else if (nRec > 0)
                {
                    var dataQuery = query.OrderBy(request, this._pi3DbContext)
                                .Skip(request.numPage * request.pageSize - request.pageSize)
                                .Take(request.pageSize).Select(p =>
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
                                        new DocsPaVO.Grids.SearchObjectField("D8", p.VAR_SEGNATURA),
                                        new DocsPaVO.Grids.SearchObjectField("D9", p.DTA_PROTO.HasValue ? p.DTA_PROTO.AsDateFormat() : p.CREATION_TIME.AsDateFormat()),
                                        new DocsPaVO.Grids.SearchObjectField("D10", IPi3DbContextMappedFunctions.GetEsitoPubblicazione(p.SYSTEM_ID)),
                                        new DocsPaVO.Grids.SearchObjectField("D11", p.DTA_ANNULLA.HasValue ? p.DTA_ANNULLA.AsDateFormat() : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D12", p.NUM_PROTO.HasValue ? p.NUM_PROTO.ToString() : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D13", p.AUTHOR.HasValue ? IPi3DbContextMappedFunctions.GetPeopleUserId(p.AUTHOR.Value) : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D14", p.ARCHIVE_DATE.HasValue ? p.ARCHIVE_DATE.AsDateFormat() : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D15", p.CHA_PERSONALE),
                                        new DocsPaVO.Grids.SearchObjectField("D16", p.CHA_PRIVATO),
                                        new DocsPaVO.Grids.SearchObjectField("D17", IPi3DbContextMappedFunctions.GetTestoUltimaNota("D", p.SYSTEM_ID, idRuoloInUO, p.AUTHOR.Value, request.infoUtente.idGruppo.AsLong())),
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
                                        new DocsPaVO.Grids.SearchObjectField("ESISTE_NOTA", IPi3DbContextMappedFunctions.EsisteNotaVisibile("D", p.SYSTEM_ID, request.infoUtente.idGruppo.AsLong(), idUser, request.infoUtente.idGruppo.AsLong())),
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
                                        new DocsPaVO.Grids.SearchObjectField("IN_LIBROFIRMA", p.IN_LIBROFIRMA),
                                        }
                                    ).ToArray()
                                ));


                    output = await dataQuery.ToArrayAsync();


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

            return new DocumentoGetQueryDocumentoPagingCustomResult(output, numTotPage, nRec, idProfiles);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetQueryDocumentoPagingCustomHandler> _logger;
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

        private async Task<string> GetConfigTextIndex()
        {
            string output = string.Empty;
            (output, bool found) = await this._configurationService.TryGetValue<string>("USE_TEXT_INDEX");
            if (!found)
                output = "0";

            return output;
        }

        #endregion
    }



    internal class ProfileSearchAppendContext
    {
        public ProfileSearchAppendContext(IPi3DbContext pi3DbContext, IQueryable<ProfileEntity> query)
        {
            Pi3DbContext = pi3DbContext;
            Query = query;
        }

        public IPi3DbContext Pi3DbContext { get; set; }

        public IQueryable<ProfileEntity> Query { get; set; }
    }

    internal static class QueryableProfileExtentions
    {
        public static IQueryable<ProfileEntity> OrderByFieldDirection<T>(this IOrderedQueryable<ProfileEntity> query,
            bool isDescending,
            Expression<Func<ProfileEntity, T>> predicate)
        {

            if (isDescending)
                query = query.ThenByDescending(predicate).ThenByDescending(p => p.DTA_PROTO != null ? p.DTA_PROTO : p.CREATION_TIME);
            else
            {
                query = query.ThenBy(predicate).ThenByDescending(p => p.DTA_PROTO != null ? p.DTA_PROTO : p.CREATION_TIME);
            }
            return query;
        }

        public static IQueryable<ProfileEntity> OrderBy(this IQueryable<ProfileEntity> query, DocumentoGetQueryDocumentoPagingCustomRequest request, IPi3DbContext pi3DbContext)
        {
            var orderBy = request.GetValoreFiltroRicerca<string>("ORACLE_FIELD_FOR_ORDER");
            var orderDirection = request.GetValoreFiltroRicerca<string>("ORDER_DIRECTION");
            var orderByForProfiledField = request.GetValoreFiltroRicerca<string>("PROFILATION_FIELD_FOR_ORDER");
            bool directionIsDescending = DocsPaVO.Grid.Grid.OrderDirectionEnum.Desc.ToString() == orderDirection;
            var field = request.FindFiltroRicerca("ORACLE_FIELD_FOR_ORDER");
            bool wasFieldOrderingApplied = false;
            var contatoreNoCustom = request.GetValoreFiltroRicerca<string>("CONTATORE_GRIGLIE_NO_CUSTOM");



            // CAMPI STANDARD
            if (field != null && !string.IsNullOrEmpty(orderBy))
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

                if (!(contatoreNoCustom != null && !request.gridPersonalization) && fieldTemp != null)
                {
                    if (fieldTemp.IsNumber)
                    {
                        if (directionIsDescending)
                            query = query.OrderByDescending(a => IPi3DbContextMappedFunctions.GetValCampoProfDocOrderToNumber(a.DOCNUMBER.GetValueOrDefault(), orderByForProfiledField.AsLong()) == null ? -1 : 0)
                                .ThenByDescending(a => IPi3DbContextMappedFunctions.GetValCampoProfDocOrderToNumber(a.DOCNUMBER.GetValueOrDefault(), orderByForProfiledField.AsLong()))
                                .ThenByDescending(p => p.DTA_PROTO != null ? p.DTA_PROTO : p.CREATION_TIME);
                        else
                            query = query.OrderByDescending(a => IPi3DbContextMappedFunctions.GetValCampoProfDocOrderToNumber(a.DOCNUMBER.GetValueOrDefault(), orderByForProfiledField.AsLong()) == null ? -1 : 0)
                               .ThenBy(a => IPi3DbContextMappedFunctions.GetValCampoProfDocOrderToNumber(a.DOCNUMBER.GetValueOrDefault(), orderByForProfiledField.AsLong()))
                               .ThenByDescending(p => p.DTA_PROTO != null ? p.DTA_PROTO : p.CREATION_TIME);
                    }
                    else
                    {
                        var tipoOggetto = pi3DbContext.OggettiCustomEntities.AsNoTracking()
                            .Join(pi3DbContext.TipoOggettoEntities.AsNoTracking(),
                                c => c.ID_TIPO_OGGETTO,
                                o => o.SYSTEM_ID,
                                (c, o) => new { c, o })
                            .Where(j => j.c.SYSTEM_ID == orderByForProfiledField.AsLong())
                            .Select(j => j.o.TIPO)
                            .FirstOrDefault();

                        if (tipoOggetto != null && tipoOggetto.ToUpper().Equals("DATA"))
                        {
                            if (directionIsDescending)
                                query = query.OrderByDescending(a => IPi3DbContextMappedFunctions.GetValCampoProfDocToDate(a.DOCNUMBER.GetValueOrDefault(), orderByForProfiledField.AsLong()) == null ? -1 : 0)
                                    .ThenByDescending(a => IPi3DbContextMappedFunctions.GetValCampoProfDocToDate(a.DOCNUMBER.GetValueOrDefault(), orderByForProfiledField.AsLong()))
                                    .ThenByDescending(p => p.DTA_PROTO != null ? p.DTA_PROTO : p.CREATION_TIME);
                            else
                                query = query.OrderByDescending(a => IPi3DbContextMappedFunctions.GetValCampoProfDocToDate(a.DOCNUMBER.GetValueOrDefault(), orderByForProfiledField.AsLong()) == null ? -1 : 0)
                                   .ThenBy(a => IPi3DbContextMappedFunctions.GetValCampoProfDocToDate(a.DOCNUMBER.GetValueOrDefault(), orderByForProfiledField.AsLong()))
                                   .ThenByDescending(p => p.DTA_PROTO != null ? p.DTA_PROTO : p.CREATION_TIME);

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
            }

            if (!wasFieldOrderingApplied)
                query = query.OrderByDescending(p => p.DTA_PROTO != null ? p.DTA_PROTO : p.CREATION_TIME);

            return query;


            //switch (orderBy.ToUpper())
            //{
            //    case "A.DTA_CREAZIONE":
            //        if (orderDirection.ToUpper() == DESC)
            //            return query.OrderByDescending(p => p.DTA_CREAZIONE);
            //        else
            //            return query.OrderBy(p => p.DTA_CREAZIONE);

            //    case "A.VAR_CODICE":
            //        if (orderDirection.ToUpper() == DESC)
            //            return query.OrderByDescending(p => p.VAR_CODICE);
            //        else
            //            return query.OrderBy(p => p.VAR_CODICE);

            //    case "A.DESCRIPTION":
            //        if (orderDirection.ToUpper() == DESC)
            //            return query.OrderByDescending(p => p.DESCRIPTION);
            //        else
            //            return query.OrderBy(p => p.DESCRIPTION);

            //    case "A.DTA_APERTURA":
            //        if (orderDirection.ToUpper() == DESC)
            //            return query.OrderByDescending(p => p.DTA_APERTURA);
            //        else
            //            return query.OrderBy(p => p.DTA_APERTURA);

            //    case "A.DTA_CHIUSURA":
            //        if (orderDirection.ToUpper() == DESC)
            //            return query.OrderByDescending(p => p.DTA_CHIUSURA);
            //        else
            //            return query.OrderBy(p => p.DTA_CHIUSURA);

            //    default:
            //        return query.OrderByDescending(p => p.DTA_CREAZIONE);
            //}
        }
    }

    internal static class DocumentoGetQueryDocumentoPagingCustomRequestExtensions
    {
        public static bool HasFiltroRicerca(this DocumentoGetQueryDocumentoPagingCustomRequest request, string argomento)
        {
            return request.queryList.Any(f => f.Any(f2 => f2.argomento == argomento));
        }

        public static DocsPaVO.filtri.FiltroRicerca? FindFiltroRicerca(this DocumentoGetQueryDocumentoPagingCustomRequest request, string argomento)
        {
            return request.queryList?[0]
                .Where(f => f.argomento == argomento)
                .Select(f => f)
                .FirstOrDefault();
        }

        public static T GetValoreFiltroRicerca<T>(this DocumentoGetQueryDocumentoPagingCustomRequest request, string argomento)
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

        private static (DateTime, DateTime) GetDateRangeLast7Days(DateTime dateTime)
        {
            var currentDate = dateTime.AddDays(-7);
            var initDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 0, 0, 0);
            var endDate = dateTime.AddHours(23).AddMinutes(59).AddSeconds(59);

            return (initDate, endDate);
        }

        private static (DateTime, DateTime) GetDateRangeLast31Days(DateTime dateTime)
        {
            var currentDate = dateTime.AddDays(-31);
            var initDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 0, 0, 0);
            var endDate = dateTime.AddHours(23).AddMinutes(59).AddSeconds(59);

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
            var initDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
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

        public static async Task AppendFiltroExport(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
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

        public static async Task AppendFiltroSecurity(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var existsFiltroAllegato = !string.IsNullOrEmpty(request.GetValoreFiltroRicerca<string>("ALLEGATO"));

            context.Query = context.Query.Where(p => context.Pi3DbContext.SecurityEntities.AsNoTracking()
                            .Where(s => s.THING == (existsFiltroAllegato ? (p.ID_DOCUMENTO_PRINCIPALE ?? p.SYSTEM_ID) : p.SYSTEM_ID)
                                    && s.ACCESSRIGHTS > 0
                                    && (s.PERSONORGROUP == request.infoUtente.idGruppo.AsLong()
                                    || s.PERSONORGROUP == request.infoUtente.idPeople.AsLong()))
                            .Select(s => s.THING)
                            .Any());
        }
        public static async Task AppendFiltroSecurityAttachment(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            context.Query = context.Query.Where(p => context.Pi3DbContext.SecurityEntities.AsNoTracking()
                                .Where(s => ((p.ID_DOCUMENTO_PRINCIPALE.HasValue && s.THING == p.ID_DOCUMENTO_PRINCIPALE))
                                        && s.ACCESSRIGHTS > 0
                                        && (s.PERSONORGROUP == request.infoUtente.idGruppo.AsLong()
                                        || s.PERSONORGROUP == request.infoUtente.idPeople.AsLong()))
                                .Select(s => s.THING)
                                .Any());
        }
        public static async Task AppendFiltroDaProto(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var predisposto = request.GetValoreFiltroRicerca<bool>("PREDISPOSTO") ? "1" : "0";
            context.Query = context.Query.Where(p => p.CHA_DA_PROTO == predisposto);
        }

        public static async Task AppendFiltroTipoProto(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var almostOne = false;

            var predicate = PredicateBuilder.New<ProfileEntity>();

            if (request.GetValoreFiltroRicerca<bool>("GRIGIO"))
            {
                almostOne = true;
                predicate = predicate.Or(p => p.CHA_TIPO_PROTO == "G" && p.CHA_DA_PROTO == "0" && p.ID_DOCUMENTO_PRINCIPALE == null);
            }

            if (request.GetValoreFiltroRicerca<bool>("PROT_ARRIVO"))
            {
                almostOne = true;
                predicate = predicate.Or(p => p.CHA_TIPO_PROTO == "A" && p.CHA_DA_PROTO == "0" && p.ID_DOCUMENTO_PRINCIPALE == null);
            }

            if (request.GetValoreFiltroRicerca<bool>("PROT_PARTENZA"))
            {
                almostOne = true;
                predicate = predicate.Or(p => p.CHA_TIPO_PROTO == "P" && p.CHA_DA_PROTO == "0" && p.ID_DOCUMENTO_PRINCIPALE == null);
            }

            if (request.GetValoreFiltroRicerca<bool>("PROT_INTERNO"))
            {
                almostOne = true;
                predicate = predicate.Or(p => p.CHA_TIPO_PROTO == "I" && p.CHA_DA_PROTO == "0" && p.ID_DOCUMENTO_PRINCIPALE == null);
            }

            if (request.GetValoreFiltroRicerca<bool>("PREDISPOSTO"))
            {
                almostOne = true;
                predicate = predicate.Or(p => (p.CHA_TIPO_PROTO == "A" || p.CHA_TIPO_PROTO == "I" || p.CHA_TIPO_PROTO == "P")
                    && p.CHA_DA_PROTO == "1" && p.ID_DOCUMENTO_PRINCIPALE == null);
            }

            if (request.GetValoreFiltroRicerca<string>("TIPO") == "T")
            {
                almostOne = true;
                predicate = predicate.Or(p => (p.CHA_TIPO_PROTO == "G" || p.CHA_TIPO_PROTO == "A" || p.CHA_TIPO_PROTO == "I" || p.CHA_TIPO_PROTO == "P")
                    && p.CHA_DA_PROTO == "0" && p.ID_DOCUMENTO_PRINCIPALE == null);
            }

            if (request.GetValoreFiltroRicerca<string>("TIPO") == "C")
            {
                almostOne = true;
                predicate = predicate.Or(p => (p.CHA_TIPO_PROTO == "G" || p.CHA_TIPO_PROTO == "A" || p.CHA_TIPO_PROTO == "I" || p.CHA_TIPO_PROTO == "P" || p.CHA_TIPO_PROTO == "C")
                    && p.ID_DOCUMENTO_PRINCIPALE == null
                    && context.Pi3DbContext.StampaRepertoriEntities.AsNoTracking().Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER));
            }

            if (request.GetValoreFiltroRicerca<string>("TIPO") == "R")
            {
                almostOne = true;
                predicate = predicate.Or(p => (p.CHA_TIPO_PROTO == "G" || p.CHA_TIPO_PROTO == "A" || p.CHA_TIPO_PROTO == "I" || p.CHA_TIPO_PROTO == "P" || p.CHA_TIPO_PROTO == "R")
                    && p.ID_DOCUMENTO_PRINCIPALE == null
                    && context.Pi3DbContext.StampaRegistriEntities.AsNoTracking().Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER));
            }

            if (almostOne)
            {
                var allegatoPredicate = await CreateFiltroAllegato(request, context);
                if(allegatoPredicate != null)
                {
                    predicate = predicate.Or(allegatoPredicate);
                }

                context.Query = context.Query.Where(predicate);
            }
        }

        public static async Task AppendFiltroProfilazioneDinamica(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var filtro = request.FindFiltroRicerca("PROFILAZIONE_DINAMICA");
            bool firstFilter = true;
            bool wasPredUsed = false;

            if (filtro != null && filtro.template != null)
            {
                if (!filtro.template.ELENCO_OGGETTI.Any())
                {
                    context.Query = context.Query.Where(p =>
                        context.Pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking()
                            .Where(at => at.ID_TEMPLATE == filtro.template.ID_TIPO_ATTO.AsLong())
                            .Select(at => !string.IsNullOrEmpty(at.DOC_NUMBER) ? at.DOC_NUMBER.AsLong() : 0)
                            .Contains(p.SYSTEM_ID));
                }
                else
                {

                    foreach (var oggetto in filtro.template.ELENCO_OGGETTI)
                    {
                        switch (oggetto.TIPO.DESCRIZIONE_TIPO)
                        {
                            case "CampoDiTesto":
                                if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                {
                                    context.Query = context.Query.Where(t =>
                                         context.Pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                             .Any(at => at.DOC_NUMBER != null && Convert.ToInt64(at.DOC_NUMBER) == t.SYSTEM_ID
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
                                         context.Pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                             .Any(at => at.DOC_NUMBER != null && Convert.ToInt64(at.DOC_NUMBER) == t.SYSTEM_ID
                                                     && at.ID_OGGETTO == oggetto.SYSTEM_ID
                                                     && at.VALORE_OGGETTO_DB.ToUpper() == casella.ToUpper()));
                                }
                                break;
                            case "MenuATendina":
                                if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                {
                                    context.Query = context.Query.Where(t =>
                                         context.Pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                             .Any(at => at.DOC_NUMBER != null && Convert.ToInt64(at.DOC_NUMBER) == t.SYSTEM_ID
                                                     && at.ID_OGGETTO == oggetto.SYSTEM_ID
                                                     && at.VALORE_OGGETTO_DB.ToUpper() == oggetto.VALORE_DATABASE.ToUpper()));
                                }
                                break;
                            case "SelezioneEsclusiva":
                                if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                {
                                    context.Query = context.Query.Where(t =>
                                         context.Pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                             .Any(at => at.DOC_NUMBER != null && Convert.ToInt64(at.DOC_NUMBER) == t.SYSTEM_ID
                                                     && at.ID_OGGETTO == oggetto.SYSTEM_ID
                                                     && at.VALORE_OGGETTO_DB.ToUpper() == oggetto.VALORE_DATABASE.ToUpper()));
                                }
                                break;
                            case "Contatore":
                                IQueryable<AssociazioneTemplatesEntity> associazioneTemplatesQueryable = null;
                                var predicate = PredicateBuilder.New<AssociazioneTemplatesEntity>();

                                if (!string.IsNullOrEmpty(oggetto.DATA_INSERIMENTO))
                                {
                                    if (oggetto.DATA_INSERIMENTO.IndexOf('@') != -1)
                                    {
                                        string[] dataInserimento = oggetto.DATA_INSERIMENTO.Split('@');
                                        var initDate = GetDateSuccessivaAl(dataInserimento[0].AsDateTime());
                                        var endDate = GetDatePrecedenteIl(dataInserimento[1].AsDateTime());
                                        predicate = predicate.And(dpa0 => dpa0.VALORE_OGGETTO_DB != null && dpa0.DTA_INS >= initDate && dpa0.DTA_INS <= endDate);
                                    }
                                    else
                                    {
                                        var range = GetDateRangeIl(oggetto.DATA_INSERIMENTO.AsDateTime());
                                        predicate = predicate.And(dpa0 => dpa0.VALORE_OGGETTO_DB != null && dpa0.DTA_INS >= range.Item1 && dpa0.DTA_INS <= range.Item2);
                                    }
                                }

                                switch (oggetto.TIPO_CONTATORE)
                                {
                                    case "T":
                                        if (!oggetto.VALORE_DATABASE.Equals(""))
                                        {
                                            if (oggetto.VALORE_DATABASE.IndexOf('@') != -1)
                                            {
                                                string[] contatore = oggetto.VALORE_DATABASE.Split('@');
                                                var init = contatore[0].AsLong();
                                                var end = contatore[1].AsLong();
                                                predicate = predicate.And(dpa0 => Convert.ToInt64(dpa0.VALORE_OGGETTO_DB) >= init
                                                        && Convert.ToInt64(dpa0.VALORE_OGGETTO_DB) <= end);
                                            }
                                            else
                                            {
                                                predicate = predicate.And(dpa0 => Convert.ToInt64(dpa0.VALORE_OGGETTO_DB) >= oggetto.VALORE_DATABASE.AsLong());
                                            }
                                        }
                                        break;
                                    //O � di tipo "A" o di tipo "R"
                                    default:
                                        if (!string.IsNullOrEmpty(oggetto.ID_AOO_RF) && oggetto.ID_AOO_RF != "0")
                                        {
                                            //Nr.Contatore SI - Aoo/Rf SI
                                            if (!oggetto.VALORE_DATABASE.Equals(""))
                                            {
                                                if (oggetto.VALORE_DATABASE.IndexOf('@') != -1)
                                                {
                                                    string[] contatore = oggetto.VALORE_DATABASE.Split('@');
                                                    var init = contatore[0].AsLong();
                                                    var end = contatore[1].AsLong();
                                                    predicate = predicate.And(dpa0 => Convert.ToInt64(dpa0.VALORE_OGGETTO_DB) >= init
                                                            && Convert.ToInt64(dpa0.VALORE_OGGETTO_DB) <= end);
                                                }
                                                else
                                                {
                                                    predicate = predicate.And(dpa0 => Convert.ToInt64(dpa0.VALORE_OGGETTO_DB) >= oggetto.VALORE_DATABASE.AsLong());
                                                }
                                            }

                                            predicate = predicate.And(dpa0 => dpa0.ID_AOO_RF == oggetto.ID_AOO_RF.AsLong());
                                        }
                                        else
                                        {
                                            if (!oggetto.VALORE_DATABASE.Equals(""))
                                            {
                                                //Nr.Contatore SI - Aoo/Rf NO
                                                if (oggetto.VALORE_DATABASE.IndexOf('@') != -1)
                                                {
                                                    string[] contatore = oggetto.VALORE_DATABASE.Split('@');
                                                    var init = contatore[0].AsLong();
                                                    var end = contatore[1].AsLong();
                                                    predicate = predicate.And(dpa0 => Convert.ToInt64(dpa0.VALORE_OGGETTO_DB) >= init
                                                            && Convert.ToInt64(dpa0.VALORE_OGGETTO_DB) <= end);
                                                }
                                                else
                                                {
                                                    predicate = predicate.And(dpa0 => Convert.ToInt64(dpa0.VALORE_OGGETTO_DB) >= oggetto.VALORE_DATABASE.AsLong());
                                                }
                                            }
                                        }
                                        break;
                                }

                                if (predicate.IsStarted)
                                {
                                    predicate = predicate.And(dpa0 => dpa0.ID_OGGETTO == oggetto.SYSTEM_ID);
                                    associazioneTemplatesQueryable = context.Pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking().Where(predicate);
                                }

                                if (associazioneTemplatesQueryable != null)
                                {
                                    context.Query = context.Query.Where(t => associazioneTemplatesQueryable
                                        .Any(at => at.DOC_NUMBER != null && Convert.ToInt64(at.DOC_NUMBER) == t.SYSTEM_ID));
                                }

                                /*

                                if (!string.IsNullOrEmpty(oggetto.DATA_INSERIMENTO) ||
                                    !string.IsNullOrEmpty(oggetto.VALORE_DATABASE) ||
                                    !string.IsNullOrEmpty(oggetto.ID_AOO_RF))
                                {
                                    associazioneTemplatesQueryable = context.Pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                        .Where(at => at.ID_OGGETTO == oggetto.SYSTEM_ID && at.VALORE_OGGETTO_DB != null);
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
                                        .Any(at => at.DOC_NUMBER != null && at.DOC_NUMBER == t.SYSTEM_ID.ToString()));
                                }
                                */
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
                                            context.Pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                                .Any(at => at.DOC_NUMBER != null && Convert.ToInt64(at.DOC_NUMBER) == t.SYSTEM_ID
                                                        && at.ID_OGGETTO == oggetto.SYSTEM_ID && at.VALORE_OGGETTO_DB != null
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
                                            context.Pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                                .Any(at => at.DOC_NUMBER != null && Convert.ToInt64(at.DOC_NUMBER) == t.SYSTEM_ID
                                                        && at.ID_OGGETTO == oggetto.SYSTEM_ID && at.VALORE_OGGETTO_DB != null
                                                        && roles.Contains(Convert.ToInt64(at.VALORE_OGGETTO_DB))));
                                    }
                                    else
                                    {
                                        var corrQueryable = context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                                            .Where(c => EF.Functions.Like(c.VAR_DESC_CORR.ToUpper(), $"%{oggetto.VALORE_DATABASE.ToUpper()}%"));

                                        context.Query = context.Query.Where(t =>
                                            context.Pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                                .Any(at => at.DOC_NUMBER != null && Convert.ToInt64(at.DOC_NUMBER) == t.SYSTEM_ID
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

        public static async Task AppendFiltroTipo(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("TIPO");

            if (!string.IsNullOrWhiteSpace(filtro))
                context.Query = context.Query.Where(p => p.CHA_IN_CESTINO == null || p.CHA_IN_CESTINO == "0");
        }


        public static async Task AppendFiltroDocInAdl(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DOC_IN_ADL");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var pairs = filtro.Split('@');
                context.Query = context.Query.Where(p =>
                            context.Pi3DbContext.AreaLavoroEntities.AsNoTracking()
                                .Any(al => al.ID_PROFILE == p.SYSTEM_ID
                                        && al.ID_PEOPLE == pairs[0].AsLong()
                                        && al.ID_RUOLO_IN_UO == pairs[1].AsLong()));
            }
        }

        public static async Task AppendFiltroDataScadenzaIl(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_SCADENZA_IL");

            if (filtro > DateTime.MinValue)
            {
                var range = GetDateRangeIl(filtro);

                context.Query = context.Query.Where(p => p.DTA_SCADENZA >= range.Item1 && p.DTA_SCADENZA <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataScadenzaSuccessivaAl(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_SCADENZA_SUCCESSIVA_AL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDateSuccessivaAl(filtro);

                context.Query = context.Query.Where(p => p.DTA_SCADENZA >= initDate);
            }
        }

        public static async Task AppendFiltroDataScadenzaPrecedenteIl(
         this DocumentoGetQueryDocumentoPagingCustomRequest request,
         ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_SCADENZA_PRECEDENTE_IL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDatePrecedenteIl(filtro);

                context.Query = context.Query.Where(p => p.DTA_SCADENZA <= initDate);
            }
        }

        public static async Task AppendFiltroDataScadenzaSC(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_SCAD_SC"))
            {
                var range = GetDateRangeSC(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_SCADENZA >= range.Item1 && p.DTA_SCADENZA <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataScadenzaMC(
              this DocumentoGetQueryDocumentoPagingCustomRequest request,
              ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_SCAD_MC"))
            {
                var range = GetDateRangeMC(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_SCADENZA >= range.Item1 && p.DTA_SCADENZA <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataScadenzaToday(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_SCADENZA_TODAY"))
            {
                var range = GetDateRangeIl(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_SCADENZA >= range.Item1 && p.DTA_SCADENZA <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataCreazioneIl(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_CREAZIONE_IL");

            if (filtro > DateTime.MinValue)
            {
                var range = GetDateRangeIl(filtro);

                context.Query = context.Query.Where(p => p.CREATION_DATE >= range.Item1 && p.CREATION_DATE <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataCreazioneSuccessivaAl(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_CREAZIONE_SUCCESSIVA_AL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDateSuccessivaAl(filtro);

                context.Query = context.Query.Where(p => p.CREATION_DATE >= initDate);
            }
        }

        public static async Task AppendFiltroDataCreazionePrecedenteIl(
         this DocumentoGetQueryDocumentoPagingCustomRequest request,
         ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_CREAZIONE_PRECEDENTE_IL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDatePrecedenteIl(filtro);

                context.Query = context.Query.Where(p => p.CREATION_DATE <= initDate);
            }
        }

        public static async Task AppendFiltroDataCreazioneSC(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_CREAZ_SC"))
            {
                var range = GetDateRangeSC(DateTime.Now);

                context.Query = context.Query.Where(p => p.CREATION_DATE >= range.Item1 && p.CREATION_DATE <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataCreazioneMC(
              this DocumentoGetQueryDocumentoPagingCustomRequest request,
              ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_CREAZ_MC"))
            {
                var range = GetDateRangeMC(DateTime.Now);

                context.Query = context.Query.Where(p => p.CREATION_DATE >= range.Item1 && p.CREATION_DATE <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataCreazioneToday(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_CREAZ_TODAY"))
            {
                var range = GetDateRangeIl(DateTime.Now);

                context.Query = context.Query.Where(p => p.CREATION_DATE >= range.Item1 && p.CREATION_DATE <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataCreazioneYesterday(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_CREAZ_YESTERDAY"))
            {
                var range = GetDateRangeIl(DateTime.Now.AddDays(-1));

                context.Query = context.Query.Where(p => p.CREATION_DATE >= range.Item1 && p.CREATION_DATE <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataCreazioneUltimi7Giorni(
                  this DocumentoGetQueryDocumentoPagingCustomRequest request,
                  ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_CREAZ_LAST_SEVEN_DAYS"))
            {
                var range = GetDateRangeLast7Days(DateTime.Now);

                context.Query = context.Query.Where(p => p.CREATION_DATE >= range.Item1 && p.CREATION_DATE <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataCreazioneUltimi31Giorni(
                 this DocumentoGetQueryDocumentoPagingCustomRequest request,
                 ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_CREAZ_LAST_THIRTY_ONE_DAYS"))
            {
                var range = GetDateRangeLast31Days(DateTime.Now);

                context.Query = context.Query.Where(p => p.CREATION_DATE >= range.Item1 && p.CREATION_DATE <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataProtIl(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_PROT_IL");

            if (filtro > DateTime.MinValue)
            {
                var range = GetDateRangeIl(filtro);

                context.Query = context.Query.Where(p => p.DTA_PROTO >= range.Item1 && p.DTA_PROTO <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataProtSuccessivaAl(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_PROT_SUCCESSIVA_AL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDateSuccessivaAl(filtro);

                context.Query = context.Query.Where(p => p.DTA_PROTO >= initDate);
            }
        }

        public static async Task AppendFiltroDataProtPrecedenteIl(
         this DocumentoGetQueryDocumentoPagingCustomRequest request,
         ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_PROT_PRECEDENTE_IL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDatePrecedenteIl(filtro);

                context.Query = context.Query.Where(p => p.DTA_PROTO <= initDate);
            }
        }

        public static async Task AppendFiltroDataProtSC(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_PROT_SC"))
            {
                var range = GetDateRangeSC(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_PROTO >= range.Item1 && p.DTA_PROTO <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataProtMC(
              this DocumentoGetQueryDocumentoPagingCustomRequest request,
              ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_PROT_MC"))
            {
                var range = GetDateRangeMC(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_PROTO >= range.Item1 && p.DTA_PROTO <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataProtToday(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_PROT_TODAY"))
            {
                var range = GetDateRangeIl(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_PROTO >= range.Item1 && p.DTA_PROTO <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataProtYesterday(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_PROTO_YESTERDAY"))
            {
                var range = GetDateRangeIl(DateTime.Now.AddDays(-1));

                context.Query = context.Query.Where(p => p.DTA_PROTO >= range.Item1 && p.DTA_PROTO <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataProtUltimi7Giorni(
                  this DocumentoGetQueryDocumentoPagingCustomRequest request,
                  ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_PROTO_LAST_SEVEN_DAYS"))
            {
                var range = GetDateRangeLast7Days(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_PROTO >= range.Item1 && p.DTA_PROTO <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataProtUltimi31Giorni(
                 this DocumentoGetQueryDocumentoPagingCustomRequest request,
                 ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_PROTO_LAST_THIRTY_ONE_DAYS"))
            {
                var range = GetDateRangeLast31Days(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_PROTO >= range.Item1 && p.DTA_PROTO <= range.Item2);
            }
        }

        public static async Task AppendFiltroAnnoProtocollo(
                 this DocumentoGetQueryDocumentoPagingCustomRequest request,
                 ProfileSearchAppendContext context)
        {
            var filtro = request.FindFiltroRicerca("ANNO_PROTOCOLLO");

            if (filtro != null && Int32.TryParse(filtro.valore, out int year))
            {
                var range = GetDateRangeYear(year);

                context.Query = context.Query.Where(p => (p.DTA_PROTO ?? p.CREATION_TIME).Value >= range.Item1 && (p.DTA_PROTO ?? p.CREATION_TIME).Value <= range.Item2);
            }
        }

        public static async Task AppendFiltroDocNumber(
        this DocumentoGetQueryDocumentoPagingCustomRequest request,
        ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("DOCNUMBER");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.DOCNUMBER == filtro);
            }
        }

        public static async Task AppendFiltroDocNumberDal(
                this DocumentoGetQueryDocumentoPagingCustomRequest request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("DOCNUMBER_DAL");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.DOCNUMBER >= filtro);
            }
        }

        public static async Task AppendFiltroDocNumberAl(
               this DocumentoGetQueryDocumentoPagingCustomRequest request,
               ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("DOCNUMBER_AL");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.DOCNUMBER <= filtro);
            }
        }

        public static async Task AppendFiltroDocNumberPrincipale(
        this DocumentoGetQueryDocumentoPagingCustomRequest request,
        ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("DOCNUMBER_PRINCIPALE");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.DOCNUMBER == filtro);
            }
        }

        public static async Task AppendFiltroDocNumberPrincipaleDal(
                this DocumentoGetQueryDocumentoPagingCustomRequest request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("DOCNUMBER_PRINCIPALE_DAL");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.DOCNUMBER >= filtro);
            }
        }

        public static async Task AppendFiltroDocNumberPrincipaleAl(
               this DocumentoGetQueryDocumentoPagingCustomRequest request,
               ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("DOCNUMBER_PRINCIPALE_AL");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.DOCNUMBER <= filtro);
            }
        }

        public static async Task AppendFiltroIdMittDest(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("ID_MITT_DEST");

            if (filtro > 0)
            {
                if (request.HasFiltroRicerca("VIS_STORICO_MITT_DEST"))
                {
                    context.Query = context.Query.Where(p =>
                        context.Pi3DbContext.DocArrivoParEntities.AsNoTracking()
                            .Where(ap => ap.ID_PROFILE == p.SYSTEM_ID
                                    && (context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                                            .Where(cg => cg.SYSTEM_ID == filtro)
                                            .Select(cg => cg.ID_OLD)
                                            .ToList()).Contains(ap.ID_MITT_DEST))
                            .Any());
                }
                else
                {
                    context.Query = context.Query.Where(p =>
                                context.Pi3DbContext.DocArrivoParEntities.AsNoTracking()
                                    .Where(ap => ap.ID_PROFILE == p.SYSTEM_ID
                                            && ap.ID_MITT_DEST == filtro)
                                    .Any());
                }
            }
        }

        public static async Task AppendFiltroCodMittDest(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("COD_MITT_DEST");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                if (!request.HasFiltroRicerca("MITT_DEST_STORICIZZATI"))
                {
                    context.Query = context.Query.Where(p =>
                        context.Pi3DbContext.DocArrivoParEntities.AsNoTracking()
                            .Join(context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking(),
                                ap => ap.ID_MITT_DEST,
                                cg => cg.SYSTEM_ID,
                                (ap, cg) => new { ap = ap, cg = cg })
                            .Where(r => r.ap.ID_PROFILE == p.SYSTEM_ID
                                    && r.cg.VAR_CODICE.ToUpper() == filtro.ToUpper())
                            .Any());
                }
                else
                {

                    var corrs = await context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking().Select(c => new
                    {
                        c.VAR_CODICE,
                        c.SYSTEM_ID
                    }).Where(c => c.VAR_CODICE.ToUpper() == filtro.ToUpper() || c.VAR_CODICE.ToUpper() == filtro.ToUpper() + "_" + c.SYSTEM_ID.ToString()).Select(c => c.SYSTEM_ID).ToListAsync();

                    context.Query = context.Query.Where(p =>
                        context.Pi3DbContext.DocArrivoParEntities.AsNoTracking().Where(d => corrs.Contains(d.ID_MITT_DEST.GetValueOrDefault()) && d.ID_PROFILE == p.SYSTEM_ID)
                            .Any());
                }
            }
        }

        public static async Task AppendFiltroMittDest(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
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

        public static async Task AppendFiltroDescAuthor(
                this DocumentoGetQueryDocumentoPagingCustomRequest request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DESC_AUTHOR");
            var filtroTipoCorr = request.GetValoreFiltroRicerca<string>("CORR_TYPE_OWNER");
            var filtroExtendHist = request.GetValoreFiltroRicerca<string>("EXTEND_TO_HISTORICIZED_AUTHOR");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var corrQueryable = context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking();

                if (!string.IsNullOrEmpty(filtroTipoCorr))
                {
                    foreach (var l in filtro.Split("&&"))
                        corrQueryable = corrQueryable.Where(c => EF.Functions.Like(c.VAR_DESC_CORR.ToUpper(), $"%{filtro.ToUpper()}%"));

                    switch (filtroTipoCorr)
                    {
                        case "R":
                            if (filtroExtendHist != null && !Convert.ToBoolean(filtroExtendHist))
                                corrQueryable = corrQueryable.Where(c => c.DTA_FINE == null);

                            context.Query = context.Query.Where(p => corrQueryable.Where(c => c.SYSTEM_ID == p.ID_RUOLO_CREATORE).Any());
                            break;
                        case "P":
                            context.Query = context.Query.Where(p => corrQueryable.Where(c => c.ID_PEOPLE == p.AUTHOR).Any());
                            break;
                        case "U":
                            context.Query = context.Query.Where(p => corrQueryable.Where(c => c.SYSTEM_ID == p.ID_UO_CREATORE).Any());
                            break;
                    }
                }

            }
        }

        public static async Task AppendFiltroNumeroProtocollo(
                this DocumentoGetQueryDocumentoPagingCustomRequest request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("NUM_PROTOCOLLO");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.NUM_PROTO == filtro);
            }
        }

        public static async Task AppendFiltroNumeroProtocolloDal(
                this DocumentoGetQueryDocumentoPagingCustomRequest request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("NUM_PROTOCOLLO_DAL");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.NUM_PROTO >= filtro);
            }
        }

        public static async Task AppendFiltroNumeroProtocolloAl(
               this DocumentoGetQueryDocumentoPagingCustomRequest request,
               ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("NUM_PROTOCOLLO_AL");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.NUM_PROTO <= filtro);
            }
        }

        public static async Task AppendFiltroIdOggetto(
               this DocumentoGetQueryDocumentoPagingCustomRequest request,
               ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("ID_OGGETTO");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.ID_OGGETTO == filtro);
            }
        }

        public static async Task AppendFiltroOggetto(
               this DocumentoGetQueryDocumentoPagingCustomRequest request,
               ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("OGGETTO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                /*
                foreach (var l in filtro.Split("&&"))
                    context.Query = context.Query.Where(p => EF.Functions.Like(p.VAR_PROF_OGGETTO.ToUpper(), $"%{l.ToUpper()}%"));
                */

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
                context.Query = context.Query.Where(p => context.Pi3DbContext.ProfileFullText(value).AsNoTracking().Any(f => f.SYSTEM_ID == p.SYSTEM_ID));
                if (casoA)
                    context.Query = context.Query.Where(p => EF.Functions.Like(p.VAR_PROF_OGGETTO.ToUpper(), $"%{valueA.ToUpper()}%"));         
            }
        }

        public static async Task AppendFiltroOggettoDocumentoPrincipale(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("OGGETTO_DOCUMENTO_PRINCIPALE");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                foreach (var l in filtro.Split("&&"))
                    context.Query = context.Query.Where(p => EF.Functions.Like(p.VAR_PROF_OGGETTO.ToUpper(), $"%{l.ToUpper()}%"));
            }
        }
        public static async Task AppendFiltroSearchDocumentSimple(
               this DocumentoGetQueryDocumentoPagingCustomRequest request,
               ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("SEARCH_DOCUMENT_SIMPLE");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var predicate = PredicateBuilder.New<ProfileEntity>(true);
                long? numProto = null;
                try
                {
                    numProto = filtro.AsLong();
                    predicate = predicate.And(p => p.NUM_PROTO == numProto);
                }
                catch
                {
                    numProto = null;
                }

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
                predicate = predicate.Or(p => context.Pi3DbContext.ProfileFullText(value).AsNoTracking().Any(f => f.SYSTEM_ID == p.SYSTEM_ID));

                predicate = predicate.Or(p => context.Pi3DbContext.DocArrivoParEntities.AsNoTracking()
                    .Join(context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking(),
                        d => d.ID_MITT_DEST,
                        c => c.SYSTEM_ID,
                        (d, c) => new { d, c })
                    .Any(j => j.d.ID_PROFILE == p.SYSTEM_ID && context.Pi3DbContext.CorrGlobaliFullText(filtro).AsNoTracking().Any(f => f.SYSTEM_ID == j.c.SYSTEM_ID)));

                context.Query = context.Query.Where(predicate);

            }
        }

        public static async Task<ExpressionStarter<ProfileEntity>> CreateFiltroAllegato(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            ExpressionStarter<ProfileEntity> allegatoPredicate = null;
            string filtroAllegato = request.GetValoreFiltroRicerca<string>("ALLEGATO");

            if (!string.IsNullOrEmpty(filtroAllegato))
            {
                switch (filtroAllegato)
                {
                    case "pec":
                        allegatoPredicate = PredicateBuilder.New<ProfileEntity>(true);
                        allegatoPredicate = allegatoPredicate.And(a => a.ID_DOCUMENTO_PRINCIPALE.HasValue && a.CHA_TIPO_PROTO == "G");
                        allegatoPredicate = allegatoPredicate.And(p => context.Pi3DbContext.NotificaEntities.AsNoTracking()
                            .Where(n => n.DOCNUMBER == p.ID_DOCUMENTO_PRINCIPALE && n.VERSION_ID.HasValue &&
                                 n.VERSION_ID == context.Pi3DbContext.VersionEntities.AsNoTracking()
                                .Where(v => v.DOCNUMBER.HasValue && v.DOCNUMBER == n.DOCNUMBER && v.VERSION_ID.HasValue)
                                .Select(v => v.VERSION_ID)
                                .Max())
                            .Select(x => 1)
                            .Union(context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                .Where(p1 => p.DOCNUMBER.HasValue && EF.Functions.Like(p.VAR_PROF_OGGETTO, "Ricevuta di ritorno delle Mail%") && p.DOCNUMBER == p1.DOCNUMBER)
                                .Select(x => 1))
                            .Any());
                        break;
                    case "user":
                        allegatoPredicate = PredicateBuilder.New<ProfileEntity>(true);
                        allegatoPredicate = allegatoPredicate.And(a => a.ID_DOCUMENTO_PRINCIPALE.HasValue && a.CHA_TIPO_PROTO == "G");
                        allegatoPredicate = allegatoPredicate.And(p => !context.Pi3DbContext.NotificaEntities.AsNoTracking()
                            .Where(n => n.DOCNUMBER == p.ID_DOCUMENTO_PRINCIPALE && n.VERSION_ID.HasValue &&
                                n.VERSION_ID == context.Pi3DbContext.VersionEntities.AsNoTracking()
                                .Where(v => v.DOCNUMBER.HasValue && v.DOCNUMBER == n.DOCNUMBER)
                                .Select(v => v.VERSION_ID)
                                .Max())
                            .Select(x => 1)
                            .Union(context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                .Where(p1 => EF.Functions.Like(p.VAR_PROF_OGGETTO, "Ricevuta di ritorno delle Mail%") &&
                                    p.DOCNUMBER.HasValue && p.DOCNUMBER == p1.DOCNUMBER)
                                .Select(x => 1)
                                .Union(context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                    .Where(p1 => EF.Functions.Like(p.VAR_PROF_OGGETTO, "Ricevuta di avvenuta%") &&
                                         p.DOCNUMBER.HasValue && p.DOCNUMBER == p1.DOCNUMBER)
                                    .Select(x => 1))
                                .Union(context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                    .Where(p1 => EF.Functions.Like(p.VAR_PROF_OGGETTO, "Ricevuta di mancata consegna%") &&
                                        p.DOCNUMBER.HasValue && p.DOCNUMBER == p1.DOCNUMBER)
                                    .Select(x => 1)))
                            .Any()
                        && !context.Pi3DbContext.VersionEntities.AsNoTracking()
                            .Any(v => v.CHA_ALLEGATI_ESTERNO == "1" && v.DOCNUMBER.HasValue && v.DOCNUMBER == p.DOCNUMBER));
                        break;
                    case "SIMPLIFIEDINTEROPERABILITY":
                        allegatoPredicate = PredicateBuilder.New<ProfileEntity>(true);
                        allegatoPredicate = allegatoPredicate.And(a => a.ID_DOCUMENTO_PRINCIPALE.HasValue && a.CHA_TIPO_PROTO == "G");
                        allegatoPredicate = allegatoPredicate.And(p => context.Pi3DbContext.NotificaEntities.AsNoTracking()
                            .Where(n => n.DOCNUMBER == p.ID_DOCUMENTO_PRINCIPALE && n.VERSION_ID.HasValue &&
                                n.VERSION_ID == context.Pi3DbContext.VersionEntities.AsNoTracking()
                                    .Where(v => v.DOCNUMBER.HasValue && v.DOCNUMBER == n.DOCNUMBER && v.VERSION_ID.HasValue)
                                    .Select(v => v.VERSION_ID)
                                    .Max()).Select(x => 1)
                                    .Union(context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                        .Where(p1 => EF.Functions.Like(p.VAR_PROF_OGGETTO, "Ricevuta di avvenuta%") &&
                                            p.DOCNUMBER.HasValue && p.DOCNUMBER == p1.DOCNUMBER)
                                        .Select(x => 1)
                                        .Union(context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                            .Where(p1 => EF.Functions.Like(p.VAR_PROF_OGGETTO, "Ricevuta di mancata consegna%") &&
                                                p.DOCNUMBER.HasValue && p.DOCNUMBER == p1.DOCNUMBER)
                                            .Select(x => 1)))
                                    .Any());
                        break;
                    case "esterni":
                        allegatoPredicate = PredicateBuilder.New<ProfileEntity>(true);
                        allegatoPredicate = allegatoPredicate.And(a => a.ID_DOCUMENTO_PRINCIPALE.HasValue && a.CHA_TIPO_PROTO == "G");
                        allegatoPredicate = allegatoPredicate.And(p => context.Pi3DbContext.VersionEntities.AsNoTracking()
                            .Any(v => v.DOCNUMBER.HasValue && v.CHA_ALLEGATI_ESTERNO == "1" && v.DOCNUMBER == p.DOCNUMBER));
                        break;
                    case "albopubb":
                        allegatoPredicate = PredicateBuilder.New<ProfileEntity>(true);
                        allegatoPredicate = allegatoPredicate.And(a => a.ID_DOCUMENTO_PRINCIPALE.HasValue && a.CHA_TIPO_PROTO == "G");
                        allegatoPredicate = allegatoPredicate.And(p => context.Pi3DbContext.VersionEntities.AsNoTracking()
                            .Any(v => v.CHA_ALLEGATI_ESTERNO == "1" && p.DOCNUMBER.HasValue && v.DOCNUMBER == p.DOCNUMBER) &&
                                context.Pi3DbContext.AlboDocPubbEntities.AsNoTracking()
                                    .Any(v => v.DA_PUBB == "S" && v.DOCNUMBER == p.DOCNUMBER));
                        break;
                    case "tutti":
                        allegatoPredicate = PredicateBuilder.New<ProfileEntity>(true);
                        allegatoPredicate = allegatoPredicate.And(a => a.ID_DOCUMENTO_PRINCIPALE.HasValue && a.CHA_TIPO_PROTO == "G");
                        break;
                }
            }

            return allegatoPredicate;
        }

        public static async Task AppendFiltroAllegato(
              this DocumentoGetQueryDocumentoPagingCustomRequest request,
              ProfileSearchAppendContext context)
        {
            var filtroProtPartenza = request.GetValoreFiltroRicerca<bool>("PROT_PARTENZA");
            var filtroProtInterno = request.GetValoreFiltroRicerca<bool>("PROT_INTERNO");
            var filtroProtArrivo = request.GetValoreFiltroRicerca<bool>("PROT_ARRIVO");

            bool searchOther = filtroProtPartenza || filtroProtInterno || filtroProtArrivo;
            bool searchGrigio = request.GetValoreFiltroRicerca<bool>("GRIGIO");
                        
            var allegatoPredicate = await CreateFiltroAllegato(request, context);
            bool searchAllegato = allegatoPredicate != null;

            if(searchAllegato && !searchGrigio && !searchOther)
            {
                //Sto cercando solo allegati
                var tempContextAllegato = new ProfileSearchAppendContext(context.Pi3DbContext, context.Pi3DbContext.ProfileEntities.AsNoTracking());
                await request.AppendFiltroSecurityAttachment(tempContextAllegato);
                //await request.AppendFiltroTipo(tempContext);
                //await request.AppendFiltroRegistro(tempContext);
                await request.AppendFiltroDataCreazioneIl(tempContextAllegato);
                await request.AppendFiltroDataCreazioneSuccessivaAl(tempContextAllegato);
                await request.AppendFiltroDataCreazionePrecedenteIl(tempContextAllegato);
                await request.AppendFiltroDataCreazioneSC(tempContextAllegato);
                await request.AppendFiltroDataCreazioneMC(tempContextAllegato);
                await request.AppendFiltroDataCreazioneToday(tempContextAllegato);
                await request.AppendFiltroDataCreazioneYesterday(tempContextAllegato);
                await request.AppendFiltroDataCreazioneUltimi7Giorni(tempContextAllegato);
                await request.AppendFiltroDataCreazioneUltimi31Giorni(tempContextAllegato);
                await request.AppendFiltroOggetto(tempContextAllegato);
                await request.AppendFiltroAnnoProtocollo(tempContextAllegato);

                //Filtri documento principale
                var tempContextDocPrincipale = new ProfileSearchAppendContext(context.Pi3DbContext, context.Pi3DbContext.ProfileEntities.AsNoTracking());
                await request.AppendFiltroDataProtIl(tempContextDocPrincipale);
                await request.AppendFiltroDataProtSuccessivaAl(tempContextDocPrincipale);
                await request.AppendFiltroDataProtPrecedenteIl(tempContextDocPrincipale);
                await request.AppendFiltroDataProtSC(tempContextDocPrincipale);
                await request.AppendFiltroDataProtMC(tempContextDocPrincipale);
                await request.AppendFiltroDataProtToday(tempContextDocPrincipale);
                await request.AppendFiltroDataProtYesterday(tempContextDocPrincipale);
                await request.AppendFiltroDataProtUltimi7Giorni(tempContextDocPrincipale);
                await request.AppendFiltroDataProtUltimi31Giorni(tempContextDocPrincipale);
                await request.AppendFiltroDocNumberPrincipale(tempContextDocPrincipale);
                await request.AppendFiltroDocNumberPrincipaleDal(tempContextDocPrincipale);
                await request.AppendFiltroDocNumberPrincipaleAl(tempContextDocPrincipale);
                await request.AppendFiltroOggettoDocumentoPrincipale(tempContextDocPrincipale);
                await request.AppendFiltroTipoAtto(tempContextDocPrincipale);
                await request.AppendFiltroProfilazioneDinamica(tempContextDocPrincipale);
                await request.AppendFiltroDiagrammaStatoDoc(tempContextDocPrincipale);
                await request.AppendFiltroInChildRicEstesa(tempContextDocPrincipale);
                await request.AppendFiltroEstendiANodiFigliEFascicoli(tempContextDocPrincipale);

                tempContextAllegato.Query = tempContextAllegato.Query.Where(a => tempContextDocPrincipale.Query.Any(p => p.SYSTEM_ID == a.ID_DOCUMENTO_PRINCIPALE));

                context.Query = tempContextAllegato.Query.Where(allegatoPredicate);           
            }
        }
        public static async Task AppendFiltroOggettoAllegato(
              this DocumentoGetQueryDocumentoPagingCustomRequest request,
              ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("OGGETTO_ALLEGATO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                // ??????????????????
            }
        }

        public static async Task AppendFiltroRegistro(
              this DocumentoGetQueryDocumentoPagingCustomRequest request,
              ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("REGISTRO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var items = filtro.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(s => s.AsLong()).ToList();

                context.Query = context.Query.Where(p => items.Contains(p.ID_REGISTRO.Value) || p.ID_REGISTRO == null);
            }
        }

        public static async Task AppendFiltroTipoDocumento(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("TIPO_DOCUMENTO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(p =>
                                context.Pi3DbContext.DocumentTypesEntities.AsNoTracking()
                                .Where(dt => dt.SYSTEM_ID == p.DOCUMENTTYPE
                                        && dt.TYPE_ID == filtro)
                                .Any());
            }
        }

        public static async Task AppendFiltroCodExtApp(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("COD_EXT_APP");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(p => p.COD_EXT_APP == filtro);
            }
        }

        public static async Task AppendFiltroParoleChiave(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("PAROLE_CHIAVE");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var items = filtro.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(s => s.AsLong()).ToList();

                context.Query = context.Query.Where(p =>
                                context.Pi3DbContext.ProfParoleEntities.AsNoTracking()
                                .Where(par => items.Contains(par.ID_PAROLA.Value)
                                    && par.ID_PROFILE == p.SYSTEM_ID)
                                .Any());
            }
        }

        public static async Task AppendFiltroTipoAtto(
          this DocumentoGetQueryDocumentoPagingCustomRequest request,
          ProfileSearchAppendContext context)
        {
            var filtro = request.FindFiltroRicerca("TIPO_ATTO");

            if (filtro != null)
            {
                if (filtro.valore != "0")
                {
                    if (await context.Pi3DbContext.TipoAttoEntities.AsNoTracking().AnyAsync(t => t.SYSTEM_ID == filtro.valore.AsLong() && t.IPERDOCUMENTO != 1))
                    {
                        context.Query = context.Query.Where(p => p.ID_TIPO_ATTO == filtro.valore.AsLong());
                    }
                }
                else
                {
                    context.Query = context.Query.Where(p => p.ID_TIPO_ATTO == null);
                }
            }
        }

        public static async Task AppendFiltroNote(
         this DocumentoGetQueryDocumentoPagingCustomRequest request,
         ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("NOTE");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var items = filtro.Split("@-@", StringSplitOptions.None);
                var nota = items[0]?.ToUpper();
                var tipoRicerca = items[1];
                var rf = items[2];

                switch (tipoRicerca)
                {
                    case "Q":
                        context.Query = context.Query.Where(p => context.Pi3DbContext.NoteEntities.AsNoTracking()
                            .Any(n => n.TIPOOGGETTOASSOCIATO == "D"
                                && n.IDOGGETTOASSOCIATO == p.SYSTEM_ID
                                && n.TESTO.ToUpper().Contains(nota)
                                && (n.TIPOVISIBILITA == "T"
                                    || (n.TIPOVISIBILITA == "P" && n.IDUTENTECREATORE == request.infoUtente.idPeople.AsLong())
                                    || (n.TIPOVISIBILITA == "R" && n.IDRUOLOCREATORE == request.infoUtente.idGruppo.AsLong())
                                    || (n.TIPOVISIBILITA == "R"
                                        && context.Pi3DbContext.RuoloRegistroEntities.AsNoTracking()
                                            .Join(context.Pi3DbContext.RegistroEntities.AsNoTracking(),
                                                rr => rr.ID_REGISTRO,
                                                r => r.SYSTEM_ID,
                                                (rr, r) => new { rr, r })
                                            .Any(j => j.rr.ID_RUOLO_IN_UO == request.infoUtente.idCorrGlobali.AsLong()
                                                && j.r.CHA_RF == "1"
                                                && j.rr.ID_REGISTRO == n.IDRFASSOCIATO)))));
                        break;
                    case "T":
                        context.Query = context.Query.Where(p => context.Pi3DbContext.NoteEntities.AsNoTracking()
                            .Any(n => n.TIPOOGGETTOASSOCIATO == "D"
                                && n.IDOGGETTOASSOCIATO == p.SYSTEM_ID
                                && n.TESTO.ToUpper().Contains(nota)
                                && n.TIPOVISIBILITA == "T"));
                        break;
                    case "P":
                        context.Query = context.Query.Where(p => context.Pi3DbContext.NoteEntities.AsNoTracking()
                            .Any(n => n.TIPOOGGETTOASSOCIATO == "D"
                                && n.IDOGGETTOASSOCIATO == p.SYSTEM_ID
                                && n.TESTO.ToUpper().Contains(nota)
                                && n.TIPOVISIBILITA == "P" && n.IDUTENTECREATORE == request.infoUtente.idPeople.AsLong()));
                        break;
                    case "R":
                        context.Query = context.Query.Where(p => context.Pi3DbContext.NoteEntities.AsNoTracking()
                            .Any(n => n.TIPOOGGETTOASSOCIATO == "D"
                                && n.IDOGGETTOASSOCIATO == p.SYSTEM_ID
                                && n.TESTO.ToUpper().Contains(nota)
                                && n.TIPOVISIBILITA == "R" && n.IDRUOLOCREATORE == request.infoUtente.idGruppo.AsLong()));
                        break;
                    case "F":
                        context.Query = context.Query.Where(p => context.Pi3DbContext.NoteEntities.AsNoTracking()
                            .Any(n => n.TIPOOGGETTOASSOCIATO == "D"
                                && n.IDOGGETTOASSOCIATO == p.SYSTEM_ID
                                && n.TESTO.ToUpper().Contains(nota)
                                && n.TIPOVISIBILITA == "F" && n.IDRFASSOCIATO == rf.AsLong()));
                        break;
                }
            }
        }

        public static async Task AppendFiltroFirmatarioNome(
          this DocumentoGetQueryDocumentoPagingCustomRequest request,
          ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("FIRMATARIO_NOME");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                // TODO: da gestire?
            }
        }

        public static async Task AppendFiltroFirmatarioCognome(
          this DocumentoGetQueryDocumentoPagingCustomRequest request,
          ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("FIRMATARIO_COGNOME");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                // TODO: da gestire?
            }
        }

        public static async Task AppendFiltroEvidenza(
              this DocumentoGetQueryDocumentoPagingCustomRequest request,
              ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("EVIDENZA");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(p => p.CHA_EVIDENZA == filtro);
            }
        }

        public static async Task AppendFiltroInChildRicEstesa(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("IN_CHILD_RIC_ESTESA");

            if (!string.IsNullOrEmpty(filtro))
            {
                var ric = filtro.Substring(4, filtro.Length - 5).Split(",");
                var ricAsLong = new List<long?>();
                ric.ForEach(r => ricAsLong.Add(r.Trim().AsLong()));

                context.Query = context.Query.Where(p =>
                        context.Pi3DbContext.ProjectComponentEntities.AsNoTracking()
                            .Where(pc => pc.LINK == p.SYSTEM_ID
                                    && ricAsLong.Contains(pc.PROJECT_ID))
                            .Any());
            }
        }

        public static async Task AppendFiltroEstendiANodiFigliEFascicoli(
              this DocumentoGetQueryDocumentoPagingCustomRequest request,
              ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("ESTENDI_A_NODI_FIGLI_E_FASCICOLI");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var predicate = PredicateBuilder.New<ProfileEntity>();
                predicate.Or((p =>
                        context.Pi3DbContext.ProjectComponentEntities.AsNoTracking()
                            .Where(pc => pc.LINK == p.SYSTEM_ID)
                            .Select(pc => pc.PROJECT_ID)
                            .Where(pc => context.Pi3DbContext.ProjectEntities.AsNoTracking()
                                            .Where(prj => prj.VAR_CODICE.ToUpper().StartsWith(filtro.ToUpper()))
                                            .Select(prj => prj.SYSTEM_ID)
                                            .Any())
                                .Any()));
                predicate.Or((p =>
                        context.Pi3DbContext.ProjectComponentEntities.AsNoTracking()
                            .Where(pc => pc.LINK == p.SYSTEM_ID)
                            .Select(pc => pc.PROJECT_ID)
                            .Where(pc => context.Pi3DbContext.ProjectEntities.AsNoTracking()
                                            .Where(prj => prj.VAR_CODICE.ToUpper().StartsWith(filtro.ToUpper()))
                                            .Select(prj => prj.ID_FASCICOLO)
                                            .Any())
                                .Any()));

                context.Query = context.Query.Where(predicate);
            }
        }

        public static async Task AppendFiltroMancanzaImmagine(
         this DocumentoGetQueryDocumentoPagingCustomRequest request,
         ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("MANCANZA_IMMAGINE");

            if (filtro == "1")
                context.Query = context.Query.Where(p => p.EXT == null);
            else if (filtro == "0")
                context.Query = context.Query.Where(p => p.EXT != null);
        }

        public static async Task AppendFiltroMancanzaFascicolazione(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("MANCANZA_FASCICOLAZIONE");

            if (filtro == "1")
                context.Query = context.Query.Where(p => p.CHA_FASCICOLATO == "0");
            else if (filtro == "0")
                context.Query = context.Query.Where(p => p.CHA_FASCICOLATO == "1");
        }

        public static async Task AppendFiltroTrasmessiCon(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var filtro = request.FindFiltroRicerca("TRASMESSI_CON");

            if (filtro != null)
            {
                if (!(filtro.valore ?? string.Empty).ToUpper().Equals("TUTTE"))
                {
                    context.Query = context.Query.Where(p => context.Pi3DbContext.TrasmissioneEntities.AsNoTracking()
                                        .Join(context.Pi3DbContext.TrasmSingolaEntities.AsNoTracking(),
                                            t => t.SYSTEM_ID,
                                            ts => ts.ID_TRASMISSIONE,
                                            (t, ts) => new { t, ts })
                                        .Join(context.Pi3DbContext.RagioneTrasmissioneEntities.AsNoTracking(),
                                            j => j.ts.ID_RAGIONE,
                                            r => r.SYSTEM_ID,
                                            (j, r) => new
                                            {
                                                ID_PROFILE = j.t.ID_PROFILE,
                                                VAR_DESC_RAGIONE = r.VAR_DESC_RAGIONE
                                            })
                                        .Where(t => t.ID_PROFILE == p.SYSTEM_ID && t.VAR_DESC_RAGIONE!.ToUpper() == filtro.valore.ToUpper())
                                        .Any());
                }
                else
                {
                    context.Query = context.Query.Where(p => context.Pi3DbContext.TrasmissioneEntities.AsNoTracking()
                                                        .Where(t => t.ID_PROFILE == p.SYSTEM_ID)
                                                        .Any());
                }
            }
        }

        public static async Task AppendFiltroTrasmessiSenza(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var filtro = request.FindFiltroRicerca("TRASMESSI_SENZA");

            if (filtro != null)
            {
                IQueryable<TrasmissioneEntity> queryable = context.Pi3DbContext.TrasmissioneEntities.AsNoTracking();

                if (!(filtro.valore ?? string.Empty).ToUpper().Equals("TUTTE"))
                {
                    queryable = queryable.Join(context.Pi3DbContext.TrasmSingolaEntities.AsNoTracking(),
                                        t => t.SYSTEM_ID,
                                        ts => ts.ID_TRASMISSIONE,
                                        (t, ts) => new { t, ts })
                                    .Join(context.Pi3DbContext.RagioneTrasmissioneEntities.AsNoTracking(),
                                        j => j.ts.ID_RAGIONE,
                                        r => r.SYSTEM_ID,
                                        (j, r) => new
                                        {
                                            ID_PROFILE = j.t.ID_PROFILE,
                                            VAR_DESC_RAGIONE = r.VAR_DESC_RAGIONE,
                                            TRASMISSIONE = j.t
                                        })
                                    .Where(t => t.VAR_DESC_RAGIONE!.ToUpper() == filtro.valore.ToUpper())
                                    .Select(t => t.TRASMISSIONE);
                }

                var idPeopleTrasm = request.GetValoreFiltroRicerca<long>("DOC_MAI_TRASMESSI_DA_UTENTE");
                if (idPeopleTrasm > 0)
                {
                    queryable = queryable.Where(t => t.ID_PEOPLE == idPeopleTrasm);
                }

                var idRuoloTrasm = request.GetValoreFiltroRicerca<long>("DOC_MAI_TRASMESSI_DA_RUOLO");
                if (idRuoloTrasm > 0)
                {
                    queryable = queryable.Where(t => t.ID_RUOLO_IN_UO == idRuoloTrasm);
                }

                context.Query = context.Query.Where(p => !queryable.Where(t => t.ID_PROFILE == p.SYSTEM_ID).Any());
            }
        }

        public static async Task AppendFiltroDocMaiTrasmessiDaUtente(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("DOC_MAI_TRASMESSI_DA_UTENTE");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => !context.Pi3DbContext.TrasmissioneEntities.AsNoTracking()
                                    .Where(t => t.ID_PROFILE == p.SYSTEM_ID && t.ID_PEOPLE == filtro)
                                    .Any());
            }
        }

        public static async Task AppendFiltroDocMaiTrasmessiDaRuolo(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("DOC_MAI_TRASMESSI_DA_RUOLO");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => !context.Pi3DbContext.TrasmissioneEntities.AsNoTracking()
                                    .Where(t => t.ID_PROFILE == p.SYSTEM_ID && t.ID_RUOLO_IN_UO == filtro)
                                    .Any());
            }
        }

        public static async Task AppendFiltroDiagrammaStatoDoc(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.FindFiltroRicerca("DIAGRAMMA_STATO_DOC");

            if (filtro != null)
            {
                var valore = filtro.valore.AsLong();

                context.Query = context.Query.Where(p => context.Pi3DbContext.DiagrammiEntities.AsNoTracking()
                                    .Where(d => p.SYSTEM_ID == d.DOC_NUMBER && (filtro.nomeCampo.ToUpper() == "UNEQUALS" ? (d.ID_STATO != null && d.ID_STATO != valore) : (d.ID_STATO != null && d.ID_STATO == valore)))
                                    .Any());
            }
        }

        public static async Task AppendFiltroCodiceFascicolo(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("CODICE_FASCICOLO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(p =>
                                context.Pi3DbContext.ProjectComponentEntities.AsNoTracking()
                                    .Where(pc =>
                                        context.Pi3DbContext.ProjectEntities.AsNoTracking()
                                            .Where(prj =>
                                                context.Pi3DbContext.ProjectEntities.AsNoTracking()
                                                    .Where(prj2 => prj2.VAR_CODICE == filtro)
                                                    .Select(prj2 => prj2.SYSTEM_ID)
                                                    .Any()
                                                && prj.CHA_TIPO_PROJ == "C")
                                            .Any())
                                    .Select(pc => pc.PROJECT_ID)
                                    .Any());
            }
        }

        public static async Task AppendFiltroIdTitolario(
          this DocumentoGetQueryDocumentoPagingCustomRequest request,
          ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("ID_TITOLARIO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var predicate = PredicateBuilder.New<ProfileEntity>();

                filtro
                    .Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.AsLong())
                    .ForEach(id =>
                        predicate = predicate.Or(p => p.ID_TITOLARIO == 0));

                context.Query = context.Query.Where(predicate);
            }
        }

        public static async Task AppendFiltroNumProtTitolario(
          this DocumentoGetQueryDocumentoPagingCustomRequest request,
          ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("NUM_PROT_TITOLARIO");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.NUM_PROT_TIT == filtro);
            }
        }

        public static async Task AppendFiltroFirmato(
         this DocumentoGetQueryDocumentoPagingCustomRequest request,
         ProfileSearchAppendContext context)
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

        public static async Task AppendFiltroIdParent(
         this DocumentoGetQueryDocumentoPagingCustomRequest request,
         ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("ID_PARENT");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.ID_PARENT == filtro);
            }

        }

        public static async Task AppendFiltroIdRepertorio(
         this DocumentoGetQueryDocumentoPagingCustomRequest request,
         ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("ID_REPERTORIO");

            if (!string.IsNullOrEmpty(filtro) && !filtro.Equals("ALL"))
            {
                var idRepertorio = filtro.AsLong();
                context.Query = context.Query.Where(p =>
                                context.Pi3DbContext.StampaRepertoriEntities.AsNoTracking()
                                    .Where(s => s.ID_REPERTORIO == idRepertorio
                                        && s.DOCNUMBER == p.DOCNUMBER)
                                .Any());
            }
        }

        private class CorrData
        {
            public long SystemId { get; set; }
            public long? IdOld { get; set; }
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

        public static async Task AppendFiltroIdAuthor(
         this DocumentoGetQueryDocumentoPagingCustomRequest request,
         ProfileSearchAppendContext context)
        {
            var filtroIdAuthor = request.GetValoreFiltroRicerca<long>("ID_AUTHOR");
            var filtroCorrType = request.GetValoreFiltroRicerca<string>("CORR_TYPE_AUTHOR");
            var filtroSearchHist = request.GetValoreFiltroRicerca<bool>("EXTEND_TO_HISTORICIZED_AUTHOR");
            if (!string.IsNullOrEmpty(filtroCorrType) && filtroIdAuthor > 0)
            {

                switch (filtroCorrType)
                {
                    case "R":
                        if (filtroSearchHist)
                        {
                            var roles = (await GetRoleHierarchy(filtroIdAuthor, new(), context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()))?.Select(c => c.SystemId).ToList();
                            if (roles != null)
                            {
                                context.Query = context.Query.Where(p => p.ID_RUOLO_CREATORE != null && roles.Contains((long)p.ID_RUOLO_CREATORE));
                            }
                        }
                        else
                        {
                            context.Query = context.Query.Where(p => p.ID_RUOLO_CREATORE == filtroIdAuthor);
                        }
                        break;
                    case "P":
                        var idPeopleAuthor = await context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                            .Where(c => c.SYSTEM_ID == filtroIdAuthor).Select(c => c.ID_PEOPLE).FirstOrDefaultAsync();
                        context.Query = context.Query.Where(p => idPeopleAuthor != null && p.AUTHOR == idPeopleAuthor);
                        break;
                    case "U":
                        context.Query = context.Query.Where(p => p.ID_UO_CREATORE == filtroIdAuthor);
                        break;

                }
            }

        }

        public static async Task AppendFiltroIdOwner(
                 this DocumentoGetQueryDocumentoPagingCustomRequest request,
                 ProfileSearchAppendContext context)
        {
            var filtroIdOwner = request.GetValoreFiltroRicerca<long>("ID_OWNER");
            var filtroCorrType = request.GetValoreFiltroRicerca<string>("CORR_TYPE_OWNER");
            IQueryable<long?>? queryOwner = null;
            if (!string.IsNullOrEmpty(filtroCorrType) && filtroIdOwner > 0)
            {
                switch (filtroCorrType)
                {
                    case "R":
                        queryOwner = context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == filtroIdOwner).Select(c => c.ID_GRUPPO);
                        break;
                    case "P":
                        queryOwner = context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == filtroIdOwner).Select(c => c.ID_PEOPLE);
                        break;
                    case "U":
                        queryOwner = context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_UO == filtroIdOwner).Select(c => c.ID_GRUPPO);
                        break;

                }
                if (queryOwner != null)
                {
                    context.Query = context.Query.Where(p => context.Pi3DbContext.SecurityEntities.AsNoTracking().Where(s => s.THING == p.SYSTEM_ID &&
                            s.CHA_TIPO_DIRITTO != null && s.CHA_TIPO_DIRITTO.Equals("P") && queryOwner.Contains(s.PERSONORGROUP)).Any());
                }
            }
        }

        public static async Task AppendFiltroFirmaElettronica(
                 this DocumentoGetQueryDocumentoPagingCustomRequest request,
                 ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("FIRMA_ELETTRONICA");
            var filtroIdUt = request.GetValoreFiltroRicerca<string>("ID_UTENTE_FIRMATARIO_ELETTRONICA");
            var filtroIdRu = request.GetValoreFiltroRicerca<string>("ID_RUOLO_FIRMATARIO_ELETTRONICA");
            var filtroDesc = request.GetValoreFiltroRicerca<string>("DESC_FIRMATARIO_ELETTRONICA");

            if (!string.IsNullOrWhiteSpace(filtro) && "1".Equals(filtro))
            {
                var predicate = PredicateBuilder.New<FirmaElettronicaEntity>();

                if (!string.IsNullOrEmpty(filtroIdUt) && !filtroIdUt.Equals("0"))
                {
                    predicate = predicate.And(f => f.XML != null && EF.Functions.Like(f.XML.ToUpper(), $"%UTENTE ID=\"{filtroIdUt}\"%"));
                }

                if (!string.IsNullOrEmpty(filtroIdRu) && !filtroIdRu.Equals("0"))
                {
                    predicate = predicate.And(f => f.XML != null && EF.Functions.Like(f.XML.ToUpper(), $"%RUOLO ID=\"{filtroIdRu}\"%"));
                }

                if (!string.IsNullOrEmpty(filtroDesc) && !filtroDesc.Equals("0"))
                {
                    predicate = predicate.And(f => f.XML != null && EF.Functions.Like(f.XML.ToUpper(), $"%{filtroDesc}%"));
                }

                context.Query = context.Query.Where(p => (context.Pi3DbContext.FirmaElettronicaEntities.AsNoTracking().Where(f => f.ID_DOCUMENTO == p.SYSTEM_ID && f.XML != null).Where(predicate).Any()));
            }
        }

        public static async Task AppendFiltroTipoFileAcquisito(
                 this DocumentoGetQueryDocumentoPagingCustomRequest request,
                 ProfileSearchAppendContext context)
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
                                                        .Where(vc => vc.v1.DOCNUMBER == p.DOCNUMBER && vc.c2.FILE_SIZE > 0)
                                                        .OrderByDescending(vc => vc.v1.VERSION_ID)
                                                        .Select(vc => vc.v1.VERSION_ID.Value)
                                                        .First() == c.VERSION_ID
                                                        && c.EXT != null && c.EXT.ToUpper().Equals(filtro.ToUpper()))
                                                .Any());
            }
        }


        public static async Task AppendFiltroMezzoSpedizione(
                 this DocumentoGetQueryDocumentoPagingCustomRequest request,
                 ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("MEZZO_SPEDIZIONE");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var fVal = Convert.ToInt32(filtro);
                context.Query = context.Query.Where(p => context.Pi3DbContext.DocArrivoParEntities.AsNoTracking().Where(d => d.ID_PROFILE == p.SYSTEM_ID && d.ID_DOCUMENTTYPES == fVal).Any());
            }
        }


        public static async Task AppendFiltroDtaProtoMitt(
                 this DocumentoGetQueryDocumentoPagingCustomRequest request,
                 ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_PROT_MITTENTE_IL");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(p => p.DTA_PROTO_IN == filtro.AsDateTime());
            }
        }


        public static async Task AppendFiltroDtaProtoMittPrecIl(
                 this DocumentoGetQueryDocumentoPagingCustomRequest request,
                 ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_PROT_MITTENTE_PRECEDENTE_IL");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var fDt = filtro.AsDateTime();
                var initDate = new DateTime(fDt.Year, fDt.Month, fDt.Day, 0, 0, 0);
                initDate.AddHours(23);
                initDate.AddMinutes(59);

                context.Query = context.Query.Where(p => p.DTA_PROTO_IN < initDate);
            }
        }

        public static async Task AppendFiltroDtaProtoMittSuccAl(
                this DocumentoGetQueryDocumentoPagingCustomRequest request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_PROT_MITTENTE_SUCCESSIVA_AL");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var fDt = filtro.AsDateTime();

                context.Query = context.Query.Where(p => p.DTA_PROTO_IN > fDt);
            }
        }

        public static async Task AppendFiltroDtaProtoMittSettCorr(
                this DocumentoGetQueryDocumentoPagingCustomRequest request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_PROT_MITTENTE_SC");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var fDt = filtro.AsDateTime();
                (var start, var end) = GetDateRangeLast7Days(fDt);

                context.Query = context.Query.Where(p => p.DTA_PROTO_IN >= start && p.DTA_PROTO_IN <= end);
            }
        }

        public static async Task AppendFiltroDtaProtoMittMeseCorr(
                this DocumentoGetQueryDocumentoPagingCustomRequest request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_PROT_MITTENTE_MC");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var fDt = filtro.AsDateTime();
                (var start, var end) = GetDateRangeMC(fDt);

                context.Query = context.Query.Where(p => p.DTA_PROTO_IN >= start && p.DTA_PROTO_IN <= end);
            }
        }

        public static async Task AppendFiltroDtaProtoMittToday(
                this DocumentoGetQueryDocumentoPagingCustomRequest request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_PROT_MITTENTE_TODAY");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var fDt = filtro.AsDateTime();
                var start = new DateTime(fDt.Year, fDt.Month, fDt.Day, 0, 0, 0);
                var end = new DateTime(fDt.Year, fDt.Month, fDt.Day, 23, 59, 59);

                context.Query = context.Query.Where(p => p.DTA_PROTO_IN >= start && p.DTA_PROTO_IN <= end);
            }
        }

        public static async Task AppendFiltroProtoMitt(
                this DocumentoGetQueryDocumentoPagingCustomRequest request,
                ProfileSearchAppendContext context,
                string useTextIndex)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("PROTOCOLLO_MITTENTE");


            if (!string.IsNullOrWhiteSpace(filtro))
            {
                if ("0".Equals(useTextIndex))
                {
                    context.Query = context.Query.Where(p => EF.Functions.Like(p.VAR_PROTO_IN, $"%{filtro.ToUpper().Replace("'", "''")}%"));

                }
                if ("2".Equals(useTextIndex))
                {/*
                    string value = filtro.Replace("'", "''").ToUpper();
                    string valueA = value;
                    if (valueA.Contains("&&"))
                        valueA = valueA.Replace("&&", "");
                    bool casoA = false;
                    if (value.Substring(0, value.Length - 1).Contains("%") && !value.Substring(0, value.Length - 1).Contains("%&&"))
                        casoA = true;

                    if (value.Contains("&&"))
                    {
                        string result = string.Empty;
                        foreach (string filter in new Regex("&&").Split(value))
                            if (!string.IsNullOrEmpty(filter))
                                result += filter + " AND ";
                        value = result.Substring(0, result.Length - 5);
                    }
                    if (value.Contains("%") && value.IndexOf("%") != value.Length - 1)
                    {
                        string result = string.Empty;
                        foreach (string filter in new Regex("%").Split(value))
                            if (!string.IsNullOrEmpty(filter))
                                result = filter + "% AND ";
                        value = result.Substring(0, result.Length - 5);
                    }

                    if (value.ToUpper().Contains(" AND  AND "))
                        value = value.ToUpper().Replace(" AND  AND ", " AND ");
                    
                    // TO DO RICERCA campo ftext

                    if (casoA)
                        context.Query = context.Query.Where(p => EF.Functions.Like(p.VAR_PROTO_IN, $"%{filtro.ToUpper().Replace("'", "''")}%"));
                    */
                    context.Query = context.Query.Where(p => EF.Functions.Like(p.VAR_PROTO_IN, $"%{filtro.ToUpper().Replace("'", "''")}%"));

                }

            }
        }


        public static async Task AppendFiltroIdUoProto(
                this DocumentoGetQueryDocumentoPagingCustomRequest request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("ID_UO_PROTOCOLLATORE");


            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var val = filtro.AsLong();
                context.Query = context.Query.Where(p => p.ID_UO_PROT == val);

            }
        }


        public static async Task AppendFiltroIdRuoProto(
                this DocumentoGetQueryDocumentoPagingCustomRequest request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("ID_RUOLO_PROTOCOLLATORE");
            var filtroExtendHist = request.GetValoreFiltroRicerca<string>("EXTEND_TO_HISTORICIZED_PROTOCOLLATORE");


            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var val = filtro.AsLong();

                if (!string.IsNullOrEmpty(filtroExtendHist) && Convert.ToBoolean(filtroExtendHist))
                {
                    var roles = (await GetRoleHierarchy(val, new(), context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()))?.Select(c => c.SystemId).ToList();
                    if (roles != null)
                    {
                        context.Query = context.Query.Where(p => p.ID_RUOLO_PROT != null && roles.Contains((long)p.ID_RUOLO_PROT));
                    }
                }
                else
                {
                    context.Query = context.Query.Where(p => p.ID_RUOLO_PROT == val);
                }
            }
        }


        public static async Task AppendFiltroIdPeopleProto(
                this DocumentoGetQueryDocumentoPagingCustomRequest request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("ID_PEOPLE_PROTOCOLLATORE");


            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var val = filtro.AsLong();
                context.Query = context.Query.Where(p => context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == val && c.ID_PEOPLE == p.ID_PEOPLE_PROT).Any());

            }
        }


        public static async Task AppendFiltroDescProto(
                this DocumentoGetQueryDocumentoPagingCustomRequest request,
                ProfileSearchAppendContext context,
                string textIndexConfig)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DESC_PROTOCOLLATORE");
            var filtroTipoCorr = request.GetValoreFiltroRicerca<string>("TIPO_CORR_PROTOCOLLATORE");
            var filtroExtendHist = request.GetValoreFiltroRicerca<string>("EXTEND_TO_HISTORICIZED_PROTOCOLLATORE");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var corrQueryable = context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking();

                if (!string.IsNullOrEmpty(filtroTipoCorr))
                {
                    foreach (var l in filtro.Split("&&"))
                        corrQueryable = corrQueryable.Where(c => EF.Functions.Like(c.VAR_DESC_CORR.ToUpper(), $"%{filtro.ToUpper()}%"));

                    switch (filtroTipoCorr)
                    {
                        case "R":
                            if (filtroExtendHist != null && !Convert.ToBoolean(filtroExtendHist))
                                corrQueryable = corrQueryable.Where(c => c.DTA_FINE == null);

                            context.Query = context.Query.Where(p => corrQueryable.Where(c => c.SYSTEM_ID == p.ID_RUOLO_PROT).Any());
                            break;
                        case "P":
                            context.Query = context.Query.Where(p => corrQueryable.Where(c => c.ID_PEOPLE == p.ID_PEOPLE_PROT).Any());
                            break;
                        case "U":
                            context.Query = context.Query.Where(p => corrQueryable.Where(c => c.SYSTEM_ID == p.ID_UO_PROT).Any());
                            break;
                    }
                }

            }
        }

        public static async Task AppendFiltroDescOwner(
               this DocumentoGetQueryDocumentoPagingCustomRequest request,
               ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DESC_OWNER");
            var filtroTipoCorr = request.GetValoreFiltroRicerca<string>("CORR_TYPE_OWNER");
            var filtroExtendHist = request.GetValoreFiltroRicerca<string>("EXTEND_TO_HISTORICIZED_PROTOCOLLATORE");
            if (!string.IsNullOrEmpty(filtro))
            {

                IQueryable<long?>? queryOwner = null;
                var corrQueryable = context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking();

                foreach (var l in filtro.Split("&&"))
                    corrQueryable = corrQueryable.Where(c => EF.Functions.Like(c.VAR_DESC_CORR.ToUpper(), $"%{filtro.ToUpper()}%"));

                if (!string.IsNullOrEmpty(filtroTipoCorr))
                {
                    switch (filtroTipoCorr)
                    {
                        case "R":
                            queryOwner = corrQueryable.Select(c => c.ID_GRUPPO);
                            break;
                        case "P":
                            queryOwner = corrQueryable.Select(c => c.ID_PEOPLE);
                            break;
                        case "U":
                            queryOwner = corrQueryable.Select(c => c.ID_GRUPPO);
                            break;

                    }
                    if (queryOwner != null)
                    {
                        context.Query = context.Query.Where(p => context.Pi3DbContext.SecurityEntities.AsNoTracking().Where(s => s.THING == p.SYSTEM_ID &&
                                s.CHA_TIPO_DIRITTO != null && s.CHA_TIPO_DIRITTO.Equals("P") && queryOwner.Contains(s.PERSONORGROUP)).Any());
                    }
                }
            }
        }

        public static async Task AppendFiltroIdMittenteIntermedio(
                this DocumentoGetQueryDocumentoPagingCustomRequest request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("ID_MITTENTE_INTERMEDIO");
            var filtroStorico = request.GetValoreFiltroRicerca<string>("VIS_STORICO_MITT_DEST");


            if (!string.IsNullOrEmpty(filtro))
            {
                long idMitt = filtro.AsLong();

                var storicoPredicate = PredicateBuilder.New<DocArrivoParEntity>();

                if (string.IsNullOrEmpty(filtroStorico))
                {
                    storicoPredicate = storicoPredicate.And(a => a.ID_MITT_DEST == idMitt);
                }
                else
                {

                    var hierarchy = (await GetRoleHierarchy(idMitt, new(), context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking())).Select(c => c.SystemId).ToList();
                    storicoPredicate = storicoPredicate.And(a => a.ID_MITT_DEST != null && hierarchy.Contains(a.ID_MITT_DEST.GetValueOrDefault()));

                }


                context.Query = context.Query.Where(p => context.Pi3DbContext.DocArrivoParEntities.AsNoTracking().Where(a =>
                    p.SYSTEM_ID == a.ID_PROFILE && "I".Equals(a.CHA_TIPO_MITT_DEST)).Where(storicoPredicate).Any());
            }
        }

        public static async Task AppendFiltroMittenteIntermedio(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("MITTENTE_INTERMEDIO");


            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Replace("'", "''");

                context.Query = context.Query.Where(p => context.Pi3DbContext.DocArrivoParEntities.AsNoTracking().
                Where(d => d.ID_PROFILE == p.SYSTEM_ID && context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking().
                Where(g => g.SYSTEM_ID == d.ID_MITT_DEST && EF.Functions.Like(g.VAR_DESC_CORR, $"%{filtro}%")).Any()).Any()
                );


            }
        }

        public static async Task AppendFiltroDtaArrivoIl(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_ARRIVO_IL");
            if (!string.IsNullOrEmpty(filtro))
            {
                var fData = filtro.AsDateTime();
                (var startDay, var endDay) = GetDateRangeIl(fData);

                context.Query = context.Query.Where(p => context.Pi3DbContext.VersionEntities.AsNoTracking().
                    Where(v => v.DOCNUMBER != null && v.DOCNUMBER == p.DOCNUMBER && v.DTA_ARRIVO >= startDay && v.DTA_ARRIVO <= endDay).Any());
            }

        }


        public static async Task AppendFiltroDtaArrivoPrecedenteIl(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_ARRIVO_PRECEDENTE_IL");
            if (!string.IsNullOrEmpty(filtro))
            {
                var fData = filtro.AsDateTime();

                context.Query = context.Query.Where(p => context.Pi3DbContext.VersionEntities.AsNoTracking().
                    Where(v => v.DOCNUMBER != null && v.DOCNUMBER == p.DOCNUMBER && v.DTA_ARRIVO <= fData).Any());
            }

        }

        public static async Task AppendFiltroDtaArrivoSuccessivaAl(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_ARRIVO_SUCCESSIVA_AL");
            if (!string.IsNullOrEmpty(filtro))
            {
                var fData = filtro.AsDateTime();

                context.Query = context.Query.Where(p => context.Pi3DbContext.VersionEntities.AsNoTracking().
                    Where(v => v.DOCNUMBER != null && v.DOCNUMBER == p.DOCNUMBER && v.DTA_ARRIVO >= fData).Any());
            }

        }

        public static async Task AppendFiltroDtaArrivoSettCorr(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_ARRIVO_SC");
            if (!string.IsNullOrEmpty(filtro))
            {
                var fData = filtro.AsDateTime();
                (var startDay, var endDay) = GetDateRangeSC(fData);
                context.Query = context.Query.Where(p => context.Pi3DbContext.VersionEntities.AsNoTracking().
                    Where(v => v.DOCNUMBER != null && v.DOCNUMBER == p.DOCNUMBER && v.DTA_ARRIVO >= startDay && v.DTA_ARRIVO <= endDay).Any());
            }

        }


        public static async Task AppendFiltroDtaArrivoMeseCorr(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_ARRIVO_MC");
            if (!string.IsNullOrEmpty(filtro))
            {
                var fData = filtro.AsDateTime();
                (var startDay, var endDay) = GetDateRangeMC(fData);
                context.Query = context.Query.Where(p => context.Pi3DbContext.VersionEntities.AsNoTracking().
                    Where(v => v.DOCNUMBER != null && v.DOCNUMBER == p.DOCNUMBER && v.DTA_ARRIVO >= startDay && v.DTA_ARRIVO <= endDay).Any());
            }

        }

        public static async Task AppendFiltroDtaArrivoToday(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_ARRIVO_TODAY");
            if (!string.IsNullOrEmpty(filtro))
            {
                var fData = DateTime.Today;
                (var startDay, var endDay) = GetDateRangeIl(fData);

                context.Query = context.Query.Where(p => context.Pi3DbContext.VersionEntities.AsNoTracking().
                    Where(v => v.DOCNUMBER != null && v.DOCNUMBER == p.DOCNUMBER && v.DTA_ARRIVO >= startDay && v.DTA_ARRIVO <= endDay).Any());
            }

        }

        public static async Task AppendFiltroNumProtoEmerg(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("NUM_PROTO_EMERGENZA");
            if (!string.IsNullOrEmpty(filtro))
            {
                var fData = DateTime.Today;
                (var startDay, var endDay) = GetDateRangeIl(fData);
                filtro = filtro.Replace("'", "''").ToUpper();

                context.Query = context.Query.Where(p => p.VAR_PROTO_EME != null && EF.Functions.Like(p.VAR_PROTO_EME.ToUpper(), $"%{filtro}%"));
            }

        }

        public static async Task AppendFiltroDataProtoEmergenzaIl(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_PROTO_EMERGENZA_IL");

            if (!string.IsNullOrEmpty(filtro))
            {
                var fData = filtro.AsDateTime();
                (var startDay, var endDay) = GetDateRangeIl(fData);
                filtro = filtro.Replace("'", "''").ToUpper();

                context.Query = context.Query.Where(p => p.DTA_PROTO_EME >= startDay && p.DTA_PROTO_EME <= endDay);
            }
        }

        public static async Task AppendFiltroDataProtoEmergenzaPrecedenteIl(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_PROTO_EMERGENZA_PRECEDENTE_IL");

            if (!string.IsNullOrEmpty(filtro))
            {
                var fData = filtro.AsDateTime();
                var day = GetDatePrecedenteIl(fData);

                context.Query = context.Query.Where(p => p.DTA_PROTO_EME <= day);
            }
        }
        public static async Task AppendFiltroDataProtoEmergenzaSuccessivaAl(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_PROTO_EMERGENZA_SUCCESSIVA_AL");

            if (!string.IsNullOrEmpty(filtro))
            {
                var fData = filtro.AsDateTime();

                context.Query = context.Query.Where(p => p.DTA_PROTO_EME >= fData);
            }
        }
        public static async Task AppendFiltroDataProtoEmergenzaSettCorr(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_PROTO_EMERGENZA_SC");

            if (!string.IsNullOrEmpty(filtro))
            {
                var fData = filtro.AsDateTime();
                (var start, var end) = GetDateRangeSC(fData);

                context.Query = context.Query.Where(p => p.DTA_PROTO_EME >= start && p.DTA_PROTO_EME <= end);
            }
        }

        public static async Task AppendFiltroDataProtoEmergenzaMeseCorr(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_PROT_EMERGENZA_MC");

            if (!string.IsNullOrEmpty(filtro))
            {
                var fData = filtro.AsDateTime();
                (var start, var end) = GetDateRangeMC(fData);

                context.Query = context.Query.Where(p => p.DTA_PROTO_EME >= start && p.DTA_PROTO_EME <= end);
            }
        }

        public static async Task AppendFiltroDataProtoEmergenzaToday(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_PROTO_EMERGENZA_TODAY");

            if (!string.IsNullOrEmpty(filtro))
            {
                var fData = filtro.AsDateTime();
                (var start, var end) = GetDateRangeIl(fData);

                context.Query = context.Query.Where(p => p.DTA_PROTO_EME >= start && p.DTA_PROTO_EME <= end);
            }
        }

        public static async Task AppendFiltroMancanzaImg(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("MANCANZA_IMMAGINE");

            if (!string.IsNullOrEmpty(filtro))
            {
                if (!filtro.Equals("0"))
                {
                    context.Query = context.Query.Where(p => p.EXT == null);
                }
                else
                {
                    context.Query = context.Query.Where(p => p.EXT != null);
                }
            }
        }

        public static async Task AppendFiltroMancanzaFasc(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("MANCANZA_FASCICOLAZIONE");

            if (!string.IsNullOrEmpty(filtro))
            {
                if (filtro.Equals("0"))
                {
                    context.Query = context.Query.Where(p => "1".Equals(p.CHA_FASCICOLATO));

                }
                else
                {
                    context.Query = context.Query.Where(p => "0".Equals(p.CHA_FASCICOLATO));
                }
            }
        }

        public static async Task AppendFiltroDocMaiSpediti(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var filtro = request.FindFiltroRicerca("DOC_MAI_SPEDITI");
            if (filtro != null)
            {
                context.Query = context.Query.Where(p =>
                               !context.Pi3DbContext.StatoInvioEntities.AsNoTracking()
                               .Any(s => s.ID_PROFILE == p.SYSTEM_ID));
            }

            var maiSpeditiUtente = request.GetValoreFiltroRicerca<string>("DOC_MAI_SPEDITI_DA_UTENTE");
            if (!string.IsNullOrEmpty(maiSpeditiUtente))
            {
                var idPeople = maiSpeditiUtente.AsLong();
                context.Query = context.Query.Where(p =>
                              !context.Pi3DbContext.LogEntities.AsNoTracking()
                              .Any(l => l.ID_OGGETTO == p.SYSTEM_ID && l.ID_PEOPLE_OPERATORE == idPeople && l.VAR_COD_AZIONE == "DOCUMENTOSPEDISCI"));
            }

            var maiSpeditiRuolo = request.GetValoreFiltroRicerca<string>("DOC_MAI_SPEDITI_DA_RUOLO");
            if (!string.IsNullOrEmpty(maiSpeditiRuolo))
            {
                var idRuolo = maiSpeditiRuolo.AsLong();
                context.Query = context.Query.Where(p =>
                              !context.Pi3DbContext.SendStoEntities.AsNoTracking()
                              .Any(s => s.ID_PROFILE == p.SYSTEM_ID && s.ID_GROUP_SENDER == idRuolo));
            }
        }
        
        public static async Task AppendFiltroDocMaiTrasmessiMaiSpeditiAiDestinatari(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var maiTrasmessi = request.GetValoreFiltroRicerca<string>("MAI_TRASMESSI_AI_DESTINATARI");
            var maiSpediti = request.GetValoreFiltroRicerca<string>("MAI_SPEDITI_AI_DESTINATARI");

            if (!string.IsNullOrEmpty(maiTrasmessi) && !string.IsNullOrEmpty(maiSpediti))
            {
                context.Query = context.Query.Where(p => (IPi3DbContextMappedFunctions.EsisteDestinatarioMaiTrasmesso(p.SYSTEM_ID) == 1 || 
                        IPi3DbContextMappedFunctions.EsisteDestinatarioMaiSpedito(p.SYSTEM_ID) == 1));
            }
            else if(!string.IsNullOrEmpty(maiTrasmessi))
            {
                context.Query = context.Query.Where(p => (IPi3DbContextMappedFunctions.EsisteDestinatarioMaiTrasmesso(p.SYSTEM_ID) == 1));
            }
            else if (!string.IsNullOrEmpty(maiSpediti))
            {
                context.Query = context.Query.Where(p => (IPi3DbContextMappedFunctions.EsisteDestinatarioMaiSpedito(p.SYSTEM_ID) == 1));
            }
        }


        public static async Task AppendFiltroDataStampaRegistroProtocollo(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_STAMPA_REGISTRO");

            if (filtro > DateTime.MinValue)
            {
                var range = GetDateRangeIl(filtro);

                if (request.GetValoreFiltroRicerca<string>("TIPO") == "R")
                {
                    context.Query = context.Query.Where(p => context.Pi3DbContext.StampaRegistriEntities.AsNoTracking()
                                                .Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER && s.DTA_STAMPA >= range.Item1 && s.DTA_STAMPA <= range.Item2));
                }
                else
                {
                    context.Query = context.Query.Where(p => context.Pi3DbContext.StampaRepertoriEntities.AsNoTracking()
                                                .Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER && s.DTA_STAMPA >= range.Item1 && s.DTA_STAMPA <= range.Item2));
                }

            }
        }

        public static async Task AppendFiltroDataStampaRegistroProtocolloDal(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_STAMPA_REGISTRO_DAL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDateSuccessivaAl(filtro);
                if (request.GetValoreFiltroRicerca<string>("TIPO") == "R")
                {
                    context.Query = context.Query.Where(p => context.Pi3DbContext.StampaRegistriEntities.AsNoTracking()
                                                    .Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER && s.DTA_STAMPA >= initDate));
                }
                else
                {
                    context.Query = context.Query.Where(p => context.Pi3DbContext.StampaRepertoriEntities.AsNoTracking()
                                                    .Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER && s.DTA_STAMPA >= initDate));
                }
            }
        }

        public static async Task AppendFiltroDataStampaRegistroProtocolloAl(
         this DocumentoGetQueryDocumentoPagingCustomRequest request,
         ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_STAMPA_REGISTRO_AL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDatePrecedenteIl(filtro);

                if (request.GetValoreFiltroRicerca<string>("TIPO") == "R")
                {
                    context.Query = context.Query.Where(p => context.Pi3DbContext.StampaRegistriEntities.AsNoTracking()
                                                   .Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER && s.DTA_STAMPA <= initDate));
                }
                else
                {
                    context.Query = context.Query.Where(p => context.Pi3DbContext.StampaRepertoriEntities.AsNoTracking()
                                                   .Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER && s.DTA_STAMPA <= initDate));
                }
            }
        }

        public static async Task AppendFiltroDataStampaRegistroProtocolloSC(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_STAMPA_SC"))
            {

                var range = GetDateRangeSC(DateTime.Now);

                context.Query = context.Query.Where(p => context.Pi3DbContext.StampaRegistriEntities.AsNoTracking()
                                              .Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER && s.DTA_STAMPA >= range.Item1 && s.DTA_STAMPA <= range.Item2));
            }
        }

        public static async Task AppendFiltroDataStampaRegistroProtocolloMC(
              this DocumentoGetQueryDocumentoPagingCustomRequest request,
              ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_STAMPA_MC"))
            {
                var range = GetDateRangeMC(DateTime.Now);

                context.Query = context.Query.Where(p => context.Pi3DbContext.StampaRegistriEntities.AsNoTracking()
                                             .Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER && s.DTA_STAMPA >= range.Item1 && s.DTA_STAMPA <= range.Item2));
            }
        }

        public static async Task AppendFiltroDataStampaRegistroProtocolloToday(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {

            if (request.HasFiltroRicerca("DATA_STAMPA_TODAY"))
            {
                var range = GetDateRangeIl(DateTime.Now);

                context.Query = context.Query.Where(p => context.Pi3DbContext.StampaRegistriEntities.AsNoTracking()
                                              .Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER && s.DTA_STAMPA >= range.Item1 && s.DTA_STAMPA <= range.Item2));
            }
        }

        public static async Task AppendFiltroAnnoProtocolloStampa(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("ANNO_PROTOCOLLO_STAMPA");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => context.Pi3DbContext.StampaRegistriEntities.AsNoTracking()
                    .Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER && s.NUM_ANNO == filtro));
            }
        }

        public static async Task AppendFiltroNumProtocolloStampa(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("NUM_PROTOCOLLO_STAMPA");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => context.Pi3DbContext.StampaRegistriEntities.AsNoTracking()
                                                .Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER
                                                    && s.NUM_PROTO_START <= filtro && s.NUM_PROTO_END >= filtro));
            }
        }

        public static async Task AppendFiltroNumProtocolloStampaDal(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("NUM_PROTOCOLLO_STAMPA_DAL");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => context.Pi3DbContext.StampaRegistriEntities.AsNoTracking()
                                                .Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER
                                                    && s.NUM_PROTO_START <= filtro));
            }
        }

        public static async Task AppendFiltroNumProtocolloStampaAl(
          this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("NUM_PROTOCOLLO_STAMPA_AL");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => context.Pi3DbContext.StampaRegistriEntities.AsNoTracking()
                                                .Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER
                                                    && s.NUM_PROTO_END >= filtro));
            }
        }

        public static async Task AppendFiltroDataStampaRegistroRepertorio(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_STAMPA_REP");

            if (filtro > DateTime.MinValue)
            {
                var range = GetDateRangeIl(filtro);

                context.Query = context.Query.Where(p => context.Pi3DbContext.StampaRepertoriEntities.AsNoTracking()
                                             .Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER && s.DTA_STAMPA >= range.Item1 && s.DTA_STAMPA <= range.Item2));
            }
        }

        public static async Task AppendFiltroDataStampaRegistroRepertorioDal(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_STAMPA_REP_DAL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDateSuccessivaAl(filtro);

                context.Query = context.Query.Where(p => context.Pi3DbContext.StampaRepertoriEntities.AsNoTracking()
                                            .Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER && s.DTA_STAMPA >= initDate));
            }
        }

        public static async Task AppendFiltroDataStampaRegistroRepertorioAl(
         this DocumentoGetQueryDocumentoPagingCustomRequest request,
         ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_STAMPA_REP_AL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDatePrecedenteIl(filtro);

                context.Query = context.Query.Where(p => context.Pi3DbContext.StampaRepertoriEntities.AsNoTracking()
                                                            .Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER && s.DTA_STAMPA <= initDate));
            }
        }

        public static async Task AppendFiltroAnnoRepertorioStampa(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("ANNO_REP_STAMPA");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => context.Pi3DbContext.StampaRepertoriEntities.AsNoTracking()
                    .Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER && s.NUM_ANNO == filtro));
            }
        }

        public static async Task AppendFiltroNumRepertorioStampa(
             this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("NUM_REP_STAMPA");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => context.Pi3DbContext.StampaRepertoriEntities.AsNoTracking()
                                                .Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER
                                                    && s.NUM_REP_START <= filtro && s.NUM_REP_END >= filtro));
            }
        }

        public static async Task AppendFiltroNumRepertorioStampaDal(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("NUM_REP_STAMPA_DAL");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => context.Pi3DbContext.StampaRepertoriEntities.AsNoTracking()
                                                .Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER
                                                    && s.NUM_REP_START <= filtro));
            }
        }

        public static async Task AppendFiltroNumRepertorioStampaAl(
          this DocumentoGetQueryDocumentoPagingCustomRequest request,
             ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("NUM_REP_STAMPA_AL");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => context.Pi3DbContext.StampaRepertoriEntities.AsNoTracking()
                                                .Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER
                                                    && s.NUM_REP_END >= filtro));
            }
        }

        public static async Task AppendFiltroStampaRepertorioFirmata(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("REP_FIRMATO");

            if (!string.IsNullOrEmpty(filtro))
            {
                context.Query = context.Query.Where(p => p.CHA_FIRMATO == filtro);
            }
        }

        public static async Task AppendFiltroStampaIdRepertorio(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("ID_REPERTORIO");

            if (!string.IsNullOrEmpty(filtro) && !filtro.Equals("ALL"))
            {
                context.Query = context.Query.Where(p => context.Pi3DbContext.StampaRepertoriEntities.AsNoTracking()
                                                .Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER
                                                    && s.ID_REPERTORIO == filtro.AsLong()));
            }
        }

        public static async Task AppendFiltroAnnullato(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("ANNULLATO");

            if (!string.IsNullOrEmpty(filtro))
            {
                if (filtro.Equals("0"))
                {
                    context.Query = context.Query.Where(p => !p.DTA_ANNULLA.HasValue);

                }
                else
                {
                    context.Query = context.Query.Where(p =>p.DTA_ANNULLA.HasValue);
                }
            }
        }

        public static async Task AppendFiltroSegnatura(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("SEGNATURA");

            if (!string.IsNullOrEmpty(filtro))
            {
                context.Query = context.Query.Where(p => EF.Functions.Like(p.VAR_SEGNATURA.ToUpper(), $"{filtro.ToUpper()}%"));
            }
        }

        public static async Task AppendFiltroIdPeopleCreatore(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("ID_PEOPLE_CREATORE");

            if (!string.IsNullOrEmpty(filtro))
            {
                var idPeopleCr = Convert.ToInt64(filtro);
                context.Query = context.Query.Where(p => p.AUTHOR == idPeopleCr);
            }
        }

        public static async Task AppendFiltroDescUoCreatore(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DESC_UO_CREATORE");

            if (!string.IsNullOrEmpty(filtro))
            {
                context.Query = context.Query.Where(p => context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.VAR_DESC_CORR != null && EF.Functions.Like(c.VAR_DESC_CORR.ToUpper(), $"%{filtro.ToUpper()}%") && p.ID_UO_CREATORE == c.SYSTEM_ID).Any());
            }
        }
        public static async Task AppendFiltroDescRuoCreatore(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DESC_RUOLO_CREATORE");

            if (!string.IsNullOrEmpty(filtro))
            {
                context.Query = context.Query.Where(p => context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.VAR_DESC_CORR != null && EF.Functions.Like(c.VAR_DESC_CORR.ToUpper(), $"%{filtro.ToUpper()}%") && p.ID_RUOLO_CREATORE == c.SYSTEM_ID).Any());
            }
        }

        public static async Task AppendFiltroDescPeopleCreatore(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DESC_PEOPLE_CREATORE");

            if (!string.IsNullOrEmpty(filtro))
            {
                context.Query = context.Query.Where(p => context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.VAR_DESC_CORR != null && EF.Functions.Like(c.VAR_DESC_CORR.ToUpper(), $"%{filtro.ToUpper()}%") && p.AUTHOR == c.ID_PEOPLE).Any());
            }
        }


        public static async Task AppendFiltroIdRuoCreatore(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("ID_RUOLO_CREATORE");

            if (!string.IsNullOrEmpty(filtro))
            {
                var idRuoCr = Convert.ToInt64(filtro);
                context.Query = context.Query.Where(p => p.ID_RUOLO_CREATORE == idRuoCr);
            }
        }

        public static async Task AppendFiltroIdUoCreatore(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("ID_UO_CREATORE");

            if (!string.IsNullOrEmpty(filtro))
            {
                var idRuoCr = Convert.ToInt64(filtro);
                context.Query = context.Query.Where(p => p.ID_UO_CREATORE == idRuoCr);
            }
        }


        public static async Task AppendFiltroNumVersioni(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("NUMERO_VERSIONI");

            if (!string.IsNullOrEmpty(filtro))
            {
                var numV = Convert.ToInt64(filtro.Substring(1));
                context.Query = context.Query.Where(p => context.Pi3DbContext.VersionEntities.AsNoTracking().Where(v => v.DOCNUMBER == p.DOCNUMBER).Select(v => v.VERSION_ID).Count() == numV);
            }
        }
        #region numAll
        public static async Task AppendFiltroNumAllegatiTipo(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtroTipoAllegato = request.GetValoreFiltroRicerca<string>("NUMERO_ALLEGATI_TIPO");
            var filtroNumeroAllegati = request.GetValoreFiltroRicerca<string>("NUMERO_ALLEGATI");

            if(!string.IsNullOrEmpty(filtroTipoAllegato))
            {
                var tipoOperatore = filtroNumeroAllegati[0].ToString();
                var numeroAllegati = filtroNumeroAllegati[1].ToString().AsLong();


                IQueryable<ProfileEntity> queryable = context.Pi3DbContext.ProfileEntities.AsNoTracking();
                string tipoAllegato = string.Empty;
                switch (filtroTipoAllegato)
                {
                    case "pec":
                        queryable = queryable.Join(context.Pi3DbContext.VersionEntities.AsNoTracking(),
                                            p => p.DOCNUMBER,
                                            v => v.DOCNUMBER,
                                            (p, v) => new { p, v })
                                        .Where(j => j.v.CHA_ALLEGATI_ESTERNO == "P")
                                        .Select(j => j.p);
                        break;
                    case "user":
                        queryable = queryable.Join(context.Pi3DbContext.VersionEntities.AsNoTracking(),
                                            p => p.DOCNUMBER,
                                            v => v.DOCNUMBER,
                                            (p, v) => new { p, v })
                                        .Where(j => j.v.CHA_ALLEGATI_ESTERNO == "0")
                                        .Select(j => j.p);
                        break;
                    case "esterni":
                        queryable = queryable.Join(context.Pi3DbContext.VersionEntities.AsNoTracking(),
                                            p => p.DOCNUMBER,
                                            v => v.DOCNUMBER,
                                            (p, v) => new { p, v })
                                        .Where(j => j.v.CHA_ALLEGATI_ESTERNO == "1")
                                        .Select(j => j.p);
                        break;
                    case "SIMPLIFIEDINTEROPERABILITY":
                        queryable = queryable.Join(context.Pi3DbContext.VersionEntities.AsNoTracking(),
                                            p => p.DOCNUMBER,
                                            v => v.DOCNUMBER,
                                            (p, v) => new { p, v })
                                        .Where(j => j.v.CHA_ALLEGATI_ESTERNO == "I")
                                        .Select(j => j.p);
                        break;
                    case "albopubb":
                        queryable = queryable.Join(context.Pi3DbContext.VersionEntities.AsNoTracking(),
                                            p => p.DOCNUMBER,
                                            v => v.DOCNUMBER,
                                            (p, v) => new { p, v })
                                        .Join(context.Pi3DbContext.AlboDocPubbEntities.AsNoTracking(),
                                            j => j.p.DOCNUMBER,
                                            a => a.DOCNUMBER,
                                            (j, a) => new { j.p, j.v, a })
                                        .Where(j => j.v.CHA_ALLEGATI_ESTERNO == "0" && j.a.DA_PUBB == "S")
                                        .Select(j => j.p);
                        break;
                }

                switch (tipoOperatore)
                {
                    case "<":
                        context.Query = context.Query.Where(p => queryable.Count(q => q.ID_DOCUMENTO_PRINCIPALE.HasValue && q.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER) < numeroAllegati);
                        break;
                    case ">":
                        context.Query = context.Query.Where(p => queryable.Count(q => q.ID_DOCUMENTO_PRINCIPALE.HasValue && q.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER) > numeroAllegati);
                        break;
                    case "=":
                        context.Query = context.Query.Where(p => queryable.Count(q => q.ID_DOCUMENTO_PRINCIPALE.HasValue && q.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER) == numeroAllegati);
                        break;
                }
            }
        }
        #endregion


        public static async Task AppendFiltroDocMaiSpeditiDaUtente(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DOC_MAI_SPEDITI_DA_UTENTE");

            if (!string.IsNullOrEmpty(filtro))
            {
                var idPeople = filtro.AsLong();
                context.Query = context.Query.Where(p => context.Pi3DbContext.LogEntities.AsNoTracking().Where(x => x.ID_OGGETTO == p.SYSTEM_ID && x.ID_PEOPLE_OPERATORE == idPeople && "DOCUMENTOSPEDISCI".Equals(x.VAR_COD_AZIONE)).FirstOrDefault() == null);
            }
        }

        public static async Task AppendFiltroDocMaiSpeditiDaRuolo(
          this DocumentoGetQueryDocumentoPagingCustomRequest request,
          ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DOC_MAI_SPEDITI_DA_RUOLO");

            if (!string.IsNullOrEmpty(filtro))
            {
                var idPeople = filtro.AsLong();
                context.Query = context.Query.Where(p => context.Pi3DbContext.SendStoEntities.AsNoTracking().Where(x => x.ID_PROFILE == p.SYSTEM_ID && x.ID_GROUP_SENDER == idPeople).FirstOrDefault() == null);
            }
        }

        public static async Task AppendFiltroImprontaDoc(
          this DocumentoGetQueryDocumentoPagingCustomRequest request,
          ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("IMPRONTA_DOCUMENTO");

            if (!string.IsNullOrEmpty(filtro))
            {
                context.Query = context.Query.Where(p =>
                            context.Pi3DbContext.ComponentEntities.AsNoTracking()
                                .Where(c => c.DOCNUMBER == p.DOCNUMBER &&
                                    context.Pi3DbContext.VersionEntities.AsNoTracking()
                                        .Join(context.Pi3DbContext.ComponentEntities.AsNoTracking(),
                                            v1 => v1.VERSION_ID,
                                            c2 => c2.VERSION_ID,
                                            (v1, c2) => new { v1, c2 })
                                        .Where(vc => vc.v1.DOCNUMBER == p.DOCNUMBER)
                                        .OrderByDescending(vc => vc.v1.VERSION_ID)
                                        .Select(vc => vc.v1.VERSION_ID.Value)
                                        .First() == c.VERSION_ID
                                        && c.VAR_IMPRONTA != null && c.VAR_IMPRONTA.ToUpper().Equals(filtro.ToUpper()))
                                .Any());
            }
        }

        public static async Task AppendFiltroDocSpeditiEsito(
          this DocumentoGetQueryDocumentoPagingCustomRequest request,
          ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DOC_SPEDITI_ESITO");

            if (!string.IsNullOrEmpty(filtro))
            {
                switch (filtro)
                {
                    case "X":
                        context.Query = context.Query.Where(p => context.Pi3DbContext.StatoInvioEntities.AsNoTracking().Where(s => s.ID_PROFILE == p.SYSTEM_ID &&
                        "0".Equals(p.CHA_DA_PROTO) && "P".Equals(p.CHA_TIPO_PROTO) && s.DTA_SPEDIZIONE.HasValue && (context.Pi3DbContext.StatoInvioEntities.AsNoTracking().Where(
                            s2 => s2.ID_PROFILE == p.SYSTEM_ID && s2.DTA_SPEDIZIONE.HasValue && (EF.Functions.Like(s2.STATUS_C_MASK, $"XX____%") || EF.Functions.Like(s2.STATUS_C_MASK, $"__X___%"))
                            ).Any())
                        ).Any()
                        );
                        break;
                    case "A":
                        context.Query = context.Query.Where(p => context.Pi3DbContext.StatoInvioEntities.AsNoTracking().Where(s => s.ID_PROFILE == p.SYSTEM_ID &&
                        "0".Equals(p.CHA_DA_PROTO) && "P".Equals(p.CHA_TIPO_PROTO) && s.DTA_SPEDIZIONE.HasValue && (context.Pi3DbContext.StatoInvioEntities.AsNoTracking().Where(
                            s2 => s2.ID_PROFILE == p.SYSTEM_ID && s2.DTA_SPEDIZIONE.HasValue && (EF.Functions.Like(s2.STATUS_C_MASK, $"AA____%") || EF.Functions.Like(s2.STATUS_C_MASK, $"__A___%"))
                            ).Any()) && !(context.Pi3DbContext.StatoInvioEntities.AsNoTracking().Where(
                            s2 => s2.ID_PROFILE == p.SYSTEM_ID && s2.DTA_SPEDIZIONE.HasValue && (EF.Functions.Like(s2.STATUS_C_MASK, $"XX____%") || EF.Functions.Like(s2.STATUS_C_MASK, $"__X___%"))
                            ).Any())
                        ).Any()
                        );
                        break;
                    case "V":
                        context.Query = context.Query.Where(p => context.Pi3DbContext.StatoInvioEntities.AsNoTracking().Where(s => s.ID_PROFILE == p.SYSTEM_ID &&
                        "0".Equals(p.CHA_DA_PROTO) && "P".Equals(p.CHA_TIPO_PROTO) && s.DTA_SPEDIZIONE.HasValue &&


                        context.Pi3DbContext.StatoInvioEntities.AsNoTracking().Where(s => s.ID_PROFILE == p.SYSTEM_ID && s.DTA_SPEDIZIONE.HasValue).Count() == (context.Pi3DbContext.StatoInvioEntities.AsNoTracking().Where(
                            s2 => s2.ID_PROFILE == p.SYSTEM_ID && s2.DTA_SPEDIZIONE.HasValue && (EF.Functions.Like(s2.STATUS_C_MASK, $"VV____%") || EF.Functions.Like(s2.STATUS_C_MASK, $"__V___%")))
                            ).Count()

                        ).Any()
                        );
                        break;
                }

            }
        }

        public static async Task AppendFiltroStatoConservazione(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("STATO_CONSERVAZIONE");

            if (!string.IsNullOrEmpty(filtro) && !filtro.Equals("NVWCRETFBK"))
            {
                var searchAllegato = !string.IsNullOrEmpty(request.GetValoreFiltroRicerca<string>("ALLEGATO"));
                var predicate = PredicateBuilder.New<ProfileEntity>();
                if(filtro.Contains("N"))
                {
                    if (!searchAllegato)
                    {
                        predicate = predicate.Or(p => !context.Pi3DbContext.VersamentoEntities.AsNoTracking()
                                        .Any(s => p.DOCNUMBER.HasValue && s.ID_PROFILE == p.SYSTEM_ID));
                    }
                    else
                    {
                        predicate = predicate.Or(p => !context.Pi3DbContext.VersamentoEntities.AsNoTracking()
                                        .Any(s => p.DOCNUMBER.HasValue && (s.ID_PROFILE == p.SYSTEM_ID || s.ID_PROFILE == p.ID_DOCUMENTO_PRINCIPALE)));
                    }
                }
                var elencoStati = "VWCRETFBK".ToCharArray();
                List<string> stati = filtro.ToArray().Where(f => elencoStati.Contains(f)).Select(f => f.ToString()).ToList();
                if (stati.Count() > 0)
                {
                    if (!searchAllegato)
                    {
                        predicate = predicate.Or(p => context.Pi3DbContext.VersamentoEntities.AsNoTracking()
                                        .Any(s => p.DOCNUMBER.HasValue && s.ID_PROFILE == p.SYSTEM_ID && stati.Contains(s.CHA_STATO)));
                    }
                    else
                    {
                        predicate = predicate.Or(p => context.Pi3DbContext.VersamentoEntities.AsNoTracking()
                                        .Any(s => p.DOCNUMBER.HasValue && stati.Contains(s.CHA_STATO) && (s.ID_PROFILE == p.SYSTEM_ID || s.ID_PROFILE == p.ID_DOCUMENTO_PRINCIPALE)));
                    }
                }

                if(predicate.IsStarted)
                    context.Query = context.Query.Where(predicate);
            }
        }

        public static async Task AppendFiltroDataVersamentoDa(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_VERSAMENTO_DA");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDateSuccessivaAl(filtro);

                context.Query = context.Query.Where(p => context.Pi3DbContext.VersamentoEntities.AsNoTracking()
                                            .Any(s => p.DOCNUMBER.HasValue && s.ID_PROFILE == p.DOCNUMBER && s.DTA_INVIO >= initDate));
            }
        }

        public static async Task AppendFiltroDataVersamentoA(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_VERSAMENTO_A");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDatePrecedenteIl(filtro);

                context.Query = context.Query.Where(p => context.Pi3DbContext.VersamentoEntities.AsNoTracking()
                                            .Any(s => p.DOCNUMBER.HasValue && s.ID_PROFILE == p.DOCNUMBER && s.DTA_INVIO <= initDate));
            }
        }

        public static async Task AppendFiltroDataVersamentoIl(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_VERSAMENTO_IL");

            if (!string.IsNullOrEmpty(filtro))
            {
                var fData = filtro.AsDateTime();
                (var startDay, var endDay) = GetDateRangeIl(fData);
                filtro = filtro.Replace("'", "''").ToUpper();

                context.Query = context.Query.Where(p => context.Pi3DbContext.VersamentoEntities.AsNoTracking()
                                            .Any(s => s.ID_PROFILE == p.DOCNUMBER && s.DTA_INVIO >= startDay && s.DTA_INVIO <= endDay));
            }
        }

        public static async Task AppendFiltroCodicePolicy(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("POLICY_CODICE");

            if (!string.IsNullOrEmpty(filtro))
            {
                context.Query = context.Query.Where(p => context.Pi3DbContext.PolicyParerEntities.AsNoTracking()
                                                        .Join(context.Pi3DbContext.VersamentiPolicyEntities.AsNoTracking(),
                                                              i => i.SYSTEM_ID,
                                                              v => v.ID_POLICY,
                                                              (i, v) => new { i, v})
                                                        .Any(j => j.v.ID_PROFILE == p.SYSTEM_ID && j.i.VAR_CODICE.ToUpper() == filtro.ToUpper()));
            }
        }

        public static async Task AppendFiltroNumeroEsecuzionePolicy(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("POLICY_NUM_ESECUZIONE");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => context.Pi3DbContext.VersamentiPolicyEntities.AsNoTracking()
                                                        .Any(v => v.ID_PROFILE == p.SYSTEM_ID && v.NUM_ESECUZIONE_POLICY == filtro));
            }
        }

        public static async Task AppendFiltroDataEsecuzionePolicyIl(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_EXEC_POLICY_IL");

            if (filtro > DateTime.MinValue)
            {
                var range = GetDateRangeIl(filtro);

                context.Query = context.Query.Where(p => context.Pi3DbContext.VersamentiPolicyEntities.AsNoTracking()
                                                       .Any(v => v.ID_PROFILE == p.SYSTEM_ID && v.DATA_ESECUZIONE_POLICY >= range.Item1 && v.DATA_ESECUZIONE_POLICY <= range.Item2));
            }
        }

        public static async Task AppendFiltroDataEsecuzionePolicySuccessivaAl(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_EXEC_POLICY_DA");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDateSuccessivaAl(filtro);

                context.Query = context.Query.Where(p => context.Pi3DbContext.VersamentiPolicyEntities.AsNoTracking()
                                                       .Any(v => v.ID_PROFILE == p.SYSTEM_ID && v.DATA_ESECUZIONE_POLICY >= initDate));
            }
        }

        public static async Task AppendFiltroDataEsecuzionePolicyPrecedenteIl(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_EXEC_POLICY_A");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDatePrecedenteIl(filtro);

                context.Query = context.Query.Where(p => context.Pi3DbContext.VersamentiPolicyEntities.AsNoTracking()
                                                       .Any(v => v.ID_PROFILE == p.SYSTEM_ID && v.DATA_ESECUZIONE_POLICY <= initDate));
            }
        }

        public static async Task AppendFiltroDataEsecuzionePolicyYesterday(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_EXEC_POLICY_YESTERDAY"))
            {
                var range = GetDateRangeIl(DateTime.Now.AddDays(-1));

                context.Query = context.Query.Where(p => context.Pi3DbContext.VersamentiPolicyEntities.AsNoTracking()
                                                      .Any(v => v.ID_PROFILE == p.SYSTEM_ID && v.DATA_ESECUZIONE_POLICY >= range.Item1 && v.DATA_ESECUZIONE_POLICY <= range.Item2));
            }
        }

        public static async Task AppendFiltroStatoConsolidamento(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("STATO_CONSOLIDAMENTO");

            if (!string.IsNullOrEmpty(filtro))
            {
                var predicate = PredicateBuilder.New<ProfileEntity>();

                var item = filtro.Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                predicate = predicate.Or(p => item.Contains(p.CONSOLIDATION_STATE));

                if (item.Contains("0"))
                    predicate = predicate.Or(p => p.CONSOLIDATION_STATE == null);

                context.Query = context.Query.Where(predicate);
            }
        }

        public static async Task AppendFiltroDataConsolidamentoDa(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_CONSOLIDAMENTO_DA");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDateSuccessivaAl(filtro);

                context.Query = context.Query.Where(p => p.CONSOLIDATION_DATE >= initDate);
            }
        }

        public static async Task AppendFiltroDataConsolidamentoA(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_CONSOLIDAMENTO_A");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDatePrecedenteIl(filtro);

                context.Query = context.Query.Where(p => p.CONSOLIDATION_DATE <= initDate);
            }
        }

        public static async Task AppendFiltroIdUtenteConsolidante(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("ID_UTENTE_CONSOLIDANTE");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.CONSOLIDATION_AUTHOR == null || p.CONSOLIDATION_AUTHOR == (context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.SYSTEM_ID == filtro)
                    .Select(p => p.ID_PEOPLE))
                    .FirstOrDefault());
            }
        }

        public static async Task AppendFiltroIdRuoloConsolidante(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("ID_RUOLO_CONSOLIDANTE");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.CONSOLIDATION_ROLE == null || p.CONSOLIDATION_ROLE == (context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.SYSTEM_ID == filtro)
                    .Select(p => p.ID_GRUPPO))
                    .FirstOrDefault());
            }
        }

        public static async Task AppendFiltroRicevute(
           this DocumentoGetQueryDocumentoPagingCustomRequest request,
           ProfileSearchAppendContext context)
        {
            //RICEVUTE PEC
            var codiceTipoNotifica = request.GetValoreFiltroRicerca<string>("CODICE_TIPO_NOTIFICA");
            if(!string.IsNullOrEmpty(codiceTipoNotifica))
            {
                var queryable = context.Pi3DbContext.NotificaEntities.AsNoTracking()
                        .Join(context.Pi3DbContext.TipoNotificaEntities.AsNoTracking(),
                              n => n.ID_TIPO_NOTIFICA,
                              t => t.SYSTEM_ID,
                              (n, t) => new { n, t });

                if (!codiceTipoNotifica.ToLower().Equals("tutti"))
                    queryable = queryable.Where(j => j.t.VAR_CODICE_NOTIFICA == codiceTipoNotifica);

                if (request.HasFiltroRicerca("DATA_TIPO_NOTIFICA_TODAY"))
                {
                    var range = GetDateRangeIl(DateTime.Now);
                    queryable = queryable.Where(j => j.n.VAR_GIORNO_ORA >= range.Item1 && j.n.VAR_GIORNO_ORA <= range.Item2);
                }
               
                var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_TIPO_NOTIFICA_DA");
                if (filtro > DateTime.MinValue)
                {
                    var initDate = GetDateSuccessivaAl(filtro);
                    queryable = queryable.Where(j => j.n.VAR_GIORNO_ORA >= initDate);
                }

                filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_TIPO_NOTIFICA_A");
                if (filtro > DateTime.MinValue)
                {
                    var initDate = GetDatePrecedenteIl(filtro);
                    queryable = queryable.Where(j => j.n.VAR_GIORNO_ORA <= initDate);
                }

                context.Query = context.Query.Where(p => queryable.Any(j => j.n.DOCNUMBER == p.DOCNUMBER));
            }

            //RICEVUTE PITRE
            var codiceTipoNotificaPiTre = request.GetValoreFiltroRicerca<string>("CODICE_TIPO_NOTIFICA_PITRE");
            if (!string.IsNullOrEmpty(codiceTipoNotificaPiTre))
            {
                var queryable = context.Pi3DbContext.NotificaEntities.AsNoTracking()
                        .Join(context.Pi3DbContext.TipoNotificaEntities.AsNoTracking(),
                              n => n.ID_TIPO_NOTIFICA,
                              t => t.SYSTEM_ID,
                              (n, t) => new { n, t })
                        .Where(j => EF.Functions.Like(j.n.VAR_MITTENTE.ToLower(), "http%"));

                if (!codiceTipoNotificaPiTre.ToLower().Equals("tutti"))
                    queryable = queryable.Where(j => j.t.VAR_CODICE_NOTIFICA == codiceTipoNotificaPiTre);

                if (request.HasFiltroRicerca("DATA_TIPO_NOTIFICA_TODAY_PITRE"))
                {
                    var range = GetDateRangeIl(DateTime.Now);
                    queryable = queryable.Where(j => j.n.VAR_GIORNO_ORA >= range.Item1 && j.n.VAR_GIORNO_ORA <= range.Item2);
                }

                var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_TIPO_NOTIFICA_DA_PITRE");
                if (filtro > DateTime.MinValue)
                {
                    var initDate = GetDateSuccessivaAl(filtro);
                    queryable = queryable.Where(j => j.n.VAR_GIORNO_ORA >= initDate);
                }

                filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_TIPO_NOTIFICA_A_PITRE");
                if (filtro > DateTime.MinValue)
                {
                    var initDate = GetDatePrecedenteIl(filtro);
                    queryable = queryable.Where(j => j.n.VAR_GIORNO_ORA <= initDate);
                }

                context.Query = context.Query.Where(p => queryable.Any(j => j.n.DOCNUMBER == p.DOCNUMBER));
            }
        }

        public static async Task AppendFiltroNonConforme(
            this DocumentoGetQueryDocumentoPagingCustomRequest request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("NON_CONFORME");

            if (!string.IsNullOrEmpty(filtro))
            {
                context.Query = context.Query.Where(p => context.Pi3DbContext.InfoFileEntities.AsNoTracking()
                    .Any(i => (i.ID_PROFILE == p.SYSTEM_ID || i.ID_DOCUMENTO_PRINCIPALE == p.SYSTEM_ID) 
                            && i.CHA_CONFORME == 0.ToString()));
            }
        }
    }
}