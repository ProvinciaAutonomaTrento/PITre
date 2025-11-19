// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.ricerche;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Linq.Expressions;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentoGetQueryDocumentoPagingCustom
{
    public class DocumentoGetQueryDocumentoPagingCustomHandler : IRequestHandler<DocumentoGetQueryDocumentoPagingCustomCommand, DocumentoGetQueryDocumentoPagingCustomResponse>
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
        public async Task<DocumentoGetQueryDocumentoPagingCustomResponse> Handle(DocumentoGetQueryDocumentoPagingCustomCommand request, CancellationToken cancellationToken)
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
                await request.AppendFiltroDocMaiTrasmessiDaUtente(appendContext);
                await request.AppendFiltroMaiSpeditiADestinatari(appendContext);
                await request.AppendFiltroDocMaiTrasmessiDaRuolo(appendContext);
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
                        CODICE = p.NUM_PROTO.HasValue ? p.NUM_PROTO.ToString() : p.DOCNUMBER.ToString()
                    })
                    .ToListAsync();

                nRec = ids.Count();



                if (request.GetIdProfilesList)
                {
                    idProfiles = ids.Select(itm => new SearchResultInfo()
                    {
                        Id = itm.SYSTEM_ID.ToString(),
                        Codice = itm.CODICE!
                    }).ToList();
                }

                numTotPage = nRec / request.PageSize;
                if ((nRec % request.PageSize) > 0)
                    numTotPage++;

                var maxRows = await GetConfigMaxRowsSearchable(idTenant.ToString());
                if (maxRows < nRec)
                {
                    numTotPage = -2;
                }
                if (nRec > 0)
                {
                    var dataQuery = query.OrderBy(request, this._pi3DbContext)
                                .Skip(request.NumPage * request.PageSize - request.PageSize)
                                .Take(request.PageSize).Select(p =>
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
                                        new DocsPaVO.Grids.SearchObjectField("D17", IPi3DbContextMappedFunctions.GetTestoUltimaNota("D", p.SYSTEM_ID, idRuoloInUO, p.AUTHOR.Value, p.ID_RUOLO_CREATORE.Value)),
                                        new DocsPaVO.Grids.SearchObjectField("D18", IPi3DbContextMappedFunctions.ClassCat(p.SYSTEM_ID)),
                                        new DocsPaVO.Grids.SearchObjectField("D19", p.AUTHOR.HasValue ? IPi3DbContextMappedFunctions.GetPeopleName(p.AUTHOR.Value) : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D20", p.ID_RUOLO_CREATORE.HasValue ? IPi3DbContextMappedFunctions.GetDescCorr(p.ID_RUOLO_CREATORE.Value) : null!),
                                        new DocsPaVO.Grids.SearchObjectField("D21", IPi3DbContextMappedFunctions.GetDataArrivoDoc(p.DOCNUMBER.Value).AsDateFormat()),
                                        new DocsPaVO.Grids.SearchObjectField("D22", IPi3DbContextMappedFunctions.GetDiagrammiStato(p.SYSTEM_ID, "D")),
                                        new DocsPaVO.Grids.SearchObjectField("D23", p.EXT),
                                        AsFieldAtipicita(p.CHA_COD_T_A),
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
                                        new DocsPaVO.Grids.SearchObjectField("CHA_FIRMATO", p.CHA_FIRMATO),
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
                                        new DocsPaVO.Grids.SearchObjectField("IN_LIBROFIRMA", p.IN_LIBROFIRMA),
                                        }
                                    ).ToArray()
                                ));


                    output = await dataQuery.ToArrayAsync();


                    if (request.VisibleFieldsTemplate != null)
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
                                    .Where(obj => request.VisibleFieldsTemplate.Any(ft => ft.CustomObjectId == Convert.ToInt32(obj.id)));

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

            return new DocumentoGetQueryDocumentoPagingCustomResponse(output, numTotPage, nRec, idProfiles);
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

    #region Helpers
    #region Classi
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

        public static IQueryable<ProfileEntity> OrderBy(this IQueryable<ProfileEntity> query, DocumentoGetQueryDocumentoPagingCustomCommand request, IPi3DbContext pi3DbContext)
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

                var fieldTemp = request.VisibleFieldsTemplate.Where(e => e.CustomObjectId.ToString() == orderByForProfiledField).FirstOrDefault();

                if (!(contatoreNoCustom != null && !request.GridPersonalization) && fieldTemp != null)
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

    internal static class DocumentoGetQueryDocumentoPagingCustomCommandExtensions
    {
        public static bool HasFiltroRicerca(this DocumentoGetQueryDocumentoPagingCustomCommand request, string argomento)
        {
            return request.QueryList.Any(f => f.Any(f2 => f2.argomento == argomento));
        }

        public static DocsPaVO.filtri.FiltroRicerca? FindFiltroRicerca(this DocumentoGetQueryDocumentoPagingCustomCommand request, string argomento)
        {
            return request.QueryList?[0]
                .Where(f => f.argomento == argomento)
                .Select(f => f)
                .FirstOrDefault();
        }

        public static T GetValoreFiltroRicerca<T>(this DocumentoGetQueryDocumentoPagingCustomCommand request, string argomento)
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            if (request.Export
                && request.DocumentsSystemId != null
                && request.DocumentsSystemId.Length > 0)
            {
                var predicate = PredicateBuilder.New<ProfileEntity>();

                foreach (var id in request.DocumentsSystemId.Select(id => id.AsLong()))
                    predicate = predicate.Or(p => p.SYSTEM_ID == id);

                context.Query = context.Query.Where(predicate);
            }
        }

        public static async Task AppendFiltroSecurity(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            context.Query = context.Query.Where(p => context.Pi3DbContext.SecurityEntities.AsNoTracking()
                                .Where(s => s.THING == p.SYSTEM_ID
                                        && s.ACCESSRIGHTS > 0
                                        && (s.PERSONORGROUP == request.InfoUtente.idGruppo.AsLong()
                                        || s.PERSONORGROUP == request.InfoUtente.idPeople.AsLong()))
                                .Select(s => s.THING)
                                .Any());
        }
        public static async Task AppendFiltroSecurityAttachment(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            context.Query = context.Query.Where(p => context.Pi3DbContext.SecurityEntities.AsNoTracking()
                                .Where(s => ((p.ID_DOCUMENTO_PRINCIPALE.HasValue && s.THING == p.ID_DOCUMENTO_PRINCIPALE) || (!p.ID_DOCUMENTO_PRINCIPALE.HasValue && s.THING == p.SYSTEM_ID))
                                        && s.ACCESSRIGHTS > 0
                                        && (s.PERSONORGROUP == request.InfoUtente.idGruppo.AsLong()
                                        || s.PERSONORGROUP == request.InfoUtente.idPeople.AsLong()))
                                .Select(s => s.THING)
                                .Any());
        }
        public static async Task AppendFiltroDaProto(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            var predisposto = request.GetValoreFiltroRicerca<bool>("PREDISPOSTO") ? "1" : "0";
            context.Query = context.Query.Where(p => p.CHA_DA_PROTO == predisposto);
        }

        public static async Task AppendFiltroTipoProto(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                context.Query = context.Query.Where(predicate);
            }
        }

        public static async Task AppendFiltroProfilazioneDinamica(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                                                .Any(at => at.DOC_NUMBER != null && at.DOC_NUMBER == t.SYSTEM_ID.ToString()
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
                                            context.Pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                                .Any(at => at.DOC_NUMBER != null && at.DOC_NUMBER == t.SYSTEM_ID.ToString()
                                                        && at.ID_OGGETTO == oggetto.SYSTEM_ID
                                                        && at.VALORE_OGGETTO_DB.ToUpper() == casella.ToUpper()));
                                }
                                break;
                            case "MenuATendina":
                                if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                {
                                    context.Query = context.Query.Where(t =>
                                            context.Pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                                .Any(at => at.DOC_NUMBER != null && at.DOC_NUMBER == t.SYSTEM_ID.ToString()
                                                        && at.ID_OGGETTO == oggetto.SYSTEM_ID
                                                        && at.VALORE_OGGETTO_DB.ToUpper() == oggetto.VALORE_DATABASE.ToUpper()));
                                }
                                break;
                            case "SelezioneEsclusiva":
                                if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                {
                                    context.Query = context.Query.Where(t =>
                                            context.Pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                                .Any(at => at.DOC_NUMBER != null && at.DOC_NUMBER == t.SYSTEM_ID.ToString()
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
                                        var endDate = GetDateSuccessivaAl(dataInserimento[1].AsDateTime());
                                        predicate = predicate.And(dpa0 => dpa0.VALORE_OGGETTO_DB != null && dpa0.DTA_INS >= initDate && dpa0.DTA_INS <= endDate);
                                    }
                                    else
                                    {
                                        var range = GetDateRangeIl(oggetto.DATA_INSERIMENTO.AsDateTime());
                                        predicate = predicate.And(dpa0 => dpa0.DTA_INS >= range.Item1 && dpa0.DTA_INS <= range.Item2);
                                    }
                                }

                                switch (oggetto.TIPO_CONTATORE)
                                {
                                    case "T":
                                        predicate = predicate.And(dpa0 => dpa0.ID_OGGETTO == oggetto.SYSTEM_ID);
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
                                    //O è di tipo "A" o di tipo "R"
                                    default:
                                        predicate = predicate.And(dpa0 => dpa0.ID_OGGETTO == oggetto.SYSTEM_ID);
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
                                                    predicate = predicate.And(dpa0 => Convert.ToInt64(dpa0.VALORE_OGGETTO_DB) >= oggetto.VALORE_DATABASE.AsLong() &&
                                                        dpa0.ID_AOO_RF == oggetto.ID_AOO_RF.AsLong());
                                                }
                                            }
                                            else
                                            {
                                                //Nr.Contatore NO - Aoo/Rf SI
                                                predicate = predicate.And(dpa0 => dpa0.ID_AOO_RF == oggetto.ID_AOO_RF.AsLong());
                                            }
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
                                associazioneTemplatesQueryable = context.Pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking().Where(predicate);

                                if (associazioneTemplatesQueryable != null)
                                {
                                    context.Query = context.Query.Where(t => associazioneTemplatesQueryable
                                        .Any(at => at.DOC_NUMBER != null && at.DOC_NUMBER == t.SYSTEM_ID.ToString()));
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
                                            context.Pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                                .Any(at => at.DOC_NUMBER != null && at.DOC_NUMBER == t.SYSTEM_ID.ToString()
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
                                        context.Pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                            .Any(at => at.DOC_NUMBER != null && at.DOC_NUMBER == t.SYSTEM_ID.ToString()
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

        public static async Task AppendFiltroTipo(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("TIPO");

            if (!string.IsNullOrWhiteSpace(filtro))
                context.Query = context.Query.Where(p => p.CHA_IN_CESTINO == null || p.CHA_IN_CESTINO == "0");
        }


        public static async Task AppendFiltroDocInAdl(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_SCAD_SC"))
            {
                var range = GetDateRangeSC(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_SCADENZA >= range.Item1 && p.DTA_SCADENZA <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataScadenzaMC(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_SCAD_MC"))
            {
                var range = GetDateRangeMC(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_SCADENZA >= range.Item1 && p.DTA_SCADENZA <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataScadenzaToday(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_SCADENZA_TODAY"))
            {
                var range = GetDateRangeIl(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_SCADENZA >= range.Item1 && p.DTA_SCADENZA <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataCreazioneIl(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_CREAZ_SC"))
            {
                var range = GetDateRangeSC(DateTime.Now);

                context.Query = context.Query.Where(p => p.CREATION_DATE >= range.Item1 && p.CREATION_DATE <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataCreazioneMC(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_CREAZ_MC"))
            {
                var range = GetDateRangeMC(DateTime.Now);

                context.Query = context.Query.Where(p => p.CREATION_DATE >= range.Item1 && p.CREATION_DATE <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataCreazioneToday(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_CREAZ_TODAY"))
            {
                var range = GetDateRangeIl(DateTime.Now);

                context.Query = context.Query.Where(p => p.CREATION_DATE >= range.Item1 && p.CREATION_DATE <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataCreazioneYesterday(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_CREAZ_YESTERDAY"))
            {
                var range = GetDateRangeIl(DateTime.Now.AddDays(-1));

                context.Query = context.Query.Where(p => p.CREATION_DATE >= range.Item1 && p.CREATION_DATE <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataCreazioneUltimi7Giorni(
                    this DocumentoGetQueryDocumentoPagingCustomCommand request,
                    ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_CREAZ_LAST_SEVEN_DAYS"))
            {
                var range = GetDateRangeLast7Days(DateTime.Now);

                context.Query = context.Query.Where(p => p.CREATION_DATE >= range.Item1 && p.CREATION_DATE <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataCreazioneUltimi31Giorni(
                    this DocumentoGetQueryDocumentoPagingCustomCommand request,
                    ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_CREAZ_LAST_THIRTY_ONE_DAYS"))
            {
                var range = GetDateRangeLast31Days(DateTime.Now);

                context.Query = context.Query.Where(p => p.CREATION_DATE >= range.Item1 && p.CREATION_DATE <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataProtIl(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_PROT_SC"))
            {
                var range = GetDateRangeSC(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_PROTO >= range.Item1 && p.DTA_PROTO <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataProtMC(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_PROT_MC"))
            {
                var range = GetDateRangeMC(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_PROTO >= range.Item1 && p.DTA_PROTO <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataProtToday(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_PROT_TODAY"))
            {
                var range = GetDateRangeIl(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_PROTO >= range.Item1 && p.DTA_PROTO <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataProtYesterday(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_PROTO_YESTERDAY"))
            {
                var range = GetDateRangeIl(DateTime.Now.AddDays(-1));

                context.Query = context.Query.Where(p => p.DTA_PROTO >= range.Item1 && p.DTA_PROTO <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataProtUltimi7Giorni(
                    this DocumentoGetQueryDocumentoPagingCustomCommand request,
                    ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_PROTO_LAST_SEVEN_DAYS"))
            {
                var range = GetDateRangeLast7Days(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_PROTO >= range.Item1 && p.DTA_PROTO <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataProtUltimi31Giorni(
                    this DocumentoGetQueryDocumentoPagingCustomCommand request,
                    ProfileSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_PROTO_LAST_THIRTY_ONE_DAYS"))
            {
                var range = GetDateRangeLast31Days(DateTime.Now);

                context.Query = context.Query.Where(p => p.DTA_PROTO >= range.Item1 && p.DTA_PROTO <= range.Item2);
            }
        }

        public static async Task AppendFiltroAnnoProtocollo(
                    this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
        this DocumentoGetQueryDocumentoPagingCustomCommand request,
        ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("DOCNUMBER");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.DOCNUMBER == filtro);
            }
        }

        public static async Task AppendFiltroDocNumberDal(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("DOCNUMBER_DAL");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.DOCNUMBER >= filtro);
            }
        }

        public static async Task AppendFiltroDocNumberAl(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("DOCNUMBER_AL");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.DOCNUMBER <= filtro);
            }
        }

        public static async Task AppendFiltroIdMittDest(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("MITT_DEST");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(p =>
                    context.Pi3DbContext.DocArrivoParEntities.AsNoTracking()
                        .Join(context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking(),
                            ap => ap.ID_MITT_DEST,
                            cg => cg.SYSTEM_ID,
                            (ap, cg) => new { ap = ap, cg = cg })
                        .Where(r => r.ap.ID_PROFILE == p.SYSTEM_ID
                                && r.cg.VAR_DESC_CORR.ToUpper().Contains(filtro.ToUpper()))
                        .Any());
            }
        }

        public static async Task AppendFiltroNumeroProtocollo(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("NUM_PROTOCOLLO");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.NUM_PROTO == filtro);
            }
        }

        public static async Task AppendFiltroNumeroProtocolloDal(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("NUM_PROTOCOLLO_DAL");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.NUM_PROTO >= filtro);
            }
        }

        public static async Task AppendFiltroNumeroProtocolloAl(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("NUM_PROTOCOLLO_AL");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.NUM_PROTO <= filtro);
            }
        }

        public static async Task AppendFiltroIdOggetto(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("ID_OGGETTO");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.ID_OGGETTO == filtro);
            }
        }

        public static async Task AppendFiltroOggetto(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("OGGETTO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                //context.Query = context.Query.Where(p => IPi3DbContextMappedFunctions.Contains(p.VAR_PROF_OGGETTO, filtro) > 0);
                context.Query = context.Query.Where(p => p.VAR_PROF_OGGETTO.ToUpper().Contains(filtro.ToUpper()));
            }
        }
        public static async Task AppendFiltroSearchDocumentSimple(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                predicate = predicate.Or(p => p.VAR_PROF_OGGETTO.ToUpper().Contains(filtro.ToUpper()));
                context.Query = context.Query.Where(predicate);

            }
        }
        public static async Task AppendFiltroAllegato(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            var filtroProtPartenza = request.GetValoreFiltroRicerca<bool>("PROT_PARTENZA");
            var filtroProtInterno = request.GetValoreFiltroRicerca<bool>("PROT_INTERNO");
            var filtroProtArrivo = request.GetValoreFiltroRicerca<bool>("PROT_ARRIVO");

            bool searchOther = filtroProtPartenza || filtroProtInterno || filtroProtArrivo;
            bool searchGrigio = request.GetValoreFiltroRicerca<bool>("GRIGIO");

            string searchAll = request.GetValoreFiltroRicerca<string>("ALLEGATO");
            var pecPredicate = PredicateBuilder.New<ProfileEntity>(true);
            if (!string.IsNullOrEmpty(searchAll))
            {

                switch (searchAll)
                {
                    case "pec":
                        pecPredicate = pecPredicate.And(p =>
                        context.Pi3DbContext.NotificaEntities.AsNoTracking().Where(n => n.DOCNUMBER == p.ID_DOCUMENTO_PRINCIPALE && n.VERSION_ID ==
                        context.Pi3DbContext.VersionEntities.AsNoTracking().Where(v => v.DOCNUMBER == n.DOCNUMBER).Select(v => v.VERSION_ID).Max()
                        ).Select(x => 1).Union(context.Pi3DbContext.ProfileEntities.AsNoTracking().Where(p1 => EF.Functions.Like(p.VAR_PROF_OGGETTO, "Ricevuta di ritorno delle Mail%") && p.DOCNUMBER == p1.DOCNUMBER)
                        .Select(x => 1)).Any()
                        );
                        break;
                    case "user":
                        pecPredicate = pecPredicate.And(p =>
                        !context.Pi3DbContext.NotificaEntities.AsNoTracking().Where(n => n.DOCNUMBER == p.ID_DOCUMENTO_PRINCIPALE && n.VERSION_ID ==
                        context.Pi3DbContext.VersionEntities.AsNoTracking().Where(v => v.DOCNUMBER == n.DOCNUMBER).Select(v => v.VERSION_ID).Max()).Select(x => 1).Union(
                        context.Pi3DbContext.ProfileEntities.AsNoTracking().Where(p1 => EF.Functions.Like(p.VAR_PROF_OGGETTO, "Ricevuta di ritorno delle Mail%")
                        && p.DOCNUMBER == p1.DOCNUMBER).Select(x => 1)
                        .Union(context.Pi3DbContext.ProfileEntities.AsNoTracking().Where(p1 => EF.Functions.Like(p.VAR_PROF_OGGETTO, "Ricevuta di avvenuta%")
                        && p.DOCNUMBER == p1.DOCNUMBER).Select(x => 1))
                        .Union(context.Pi3DbContext.ProfileEntities.AsNoTracking().Where(p1 => EF.Functions.Like(p.VAR_PROF_OGGETTO, "Ricevuta di mancata consegna%")
                        && p.DOCNUMBER == p1.DOCNUMBER).Select(x => 1))
                        ).Any()
                        &&
                        !context.Pi3DbContext.VersionEntities.AsNoTracking().Any(v => v.CHA_ALLEGATI_ESTERNO == "1" && v.DOCNUMBER == p.DOCNUMBER)
                        );
                        break;
                    case "SIMPLIFIEDINTEROPERABILITY":
                        pecPredicate = pecPredicate.And(p =>
                        context.Pi3DbContext.NotificaEntities.AsNoTracking().Where(n => n.DOCNUMBER == p.ID_DOCUMENTO_PRINCIPALE && n.VERSION_ID ==
                        context.Pi3DbContext.VersionEntities.AsNoTracking().Where(v => v.DOCNUMBER == n.DOCNUMBER).Select(v => v.VERSION_ID).Max()).Select(x => 1).Union(
                        context.Pi3DbContext.ProfileEntities.AsNoTracking().Where(p1 => EF.Functions.Like(p.VAR_PROF_OGGETTO, "Ricevuta di avvenuta%")
                        && p.DOCNUMBER == p1.DOCNUMBER).Select(x => 1)
                        .Union(context.Pi3DbContext.ProfileEntities.AsNoTracking().Where(p1 => EF.Functions.Like(p.VAR_PROF_OGGETTO, "Ricevuta di mancata consegna%")
                        && p.DOCNUMBER == p1.DOCNUMBER).Select(x => 1))
                        ).Any()
                        );
                        break;
                    case "esterni":
                        pecPredicate = pecPredicate.And(p => context.Pi3DbContext.VersionEntities.AsNoTracking().Any(v => v.CHA_ALLEGATI_ESTERNO == "1" && v.DOCNUMBER == p.DOCNUMBER));
                        break;
                    case "albopubb":
                        pecPredicate = pecPredicate.And(p => context.Pi3DbContext.VersionEntities.AsNoTracking().Any(v => v.CHA_ALLEGATI_ESTERNO == "1" && v.DOCNUMBER == p.DOCNUMBER)
                        && context.Pi3DbContext.AlboDocPubbEntities.AsNoTracking().Any(v => v.DA_PUBB == "S" && v.DOCNUMBER == p.DOCNUMBER)
                        );
                        break;
                }

                pecPredicate = pecPredicate.And(p => p.ID_DOCUMENTO_PRINCIPALE.HasValue);

                var tempContext = new ProfileSearchAppendContext(context.Pi3DbContext, context.Pi3DbContext.ProfileEntities.AsNoTracking());
                await request.AppendFiltroSecurityAttachment(tempContext);
                await request.AppendFiltroTipo(tempContext);
                await request.AppendFiltroRegistro(tempContext);
                await request.AppendFiltroDataCreazioneIl(tempContext);
                await request.AppendFiltroDataCreazioneSuccessivaAl(tempContext);
                await request.AppendFiltroDataCreazionePrecedenteIl(tempContext);
                await request.AppendFiltroDataCreazioneSC(tempContext);
                await request.AppendFiltroDataCreazioneMC(tempContext);
                await request.AppendFiltroDataCreazioneToday(tempContext);
                await request.AppendFiltroDataCreazioneYesterday(tempContext);
                await request.AppendFiltroDataCreazioneUltimi7Giorni(tempContext);
                await request.AppendFiltroDataCreazioneUltimi31Giorni(tempContext);
                await request.AppendFiltroOggetto(tempContext);
                await request.AppendFiltroAnnoProtocollo(tempContext);

                if (searchGrigio)
                {
                    pecPredicate = pecPredicate.And(p => p.CHA_TIPO_PROTO == "G");
                    context.Query = context.Query.Union(tempContext.Query.Where(pecPredicate));
                }
                else if (!searchOther)
                {
                    context.Query = tempContext.Query.Where(pecPredicate);
                }
                else
                {
                    pecPredicate = pecPredicate.And(p => p.CHA_TIPO_PROTO == "G");
                    context.Query = context.Query.Union(tempContext.Query.Where(pecPredicate));
                }

            }


        }
        public static async Task AppendFiltroOggettoAllegato(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("OGGETTO_ALLEGATO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                // ??????????????????
            }
        }

        public static async Task AppendFiltroRegistro(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("COD_EXT_APP");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(p => p.COD_EXT_APP == filtro);
            }
        }

        public static async Task AppendFiltroParoleChiave(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.FindFiltroRicerca("TIPO_ATTO");

            if (filtro != null)
            {
                if (filtro.valore != "0")
                    context.Query = context.Query.Where(p => p.ID_TIPO_ATTO == filtro.valore.AsLong());
                else
                    context.Query = context.Query.Where(p => p.ID_TIPO_ATTO == null);
            }
        }

        public static async Task AppendFiltroNote(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("NOTE");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var items = filtro.Split("@-@", StringSplitOptions.None);
                var rf = items[2];

                // TODO: integrare function GetCountNote
                //context.Query = context.Query.Where(p => IPi3DbContextMappedFunctions.GetCountNote("D", p.SYSTEM_ID, items[0].Replace("'", "''"), 
            }
        }

        public static async Task AppendFiltroFirmatarioNome(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("FIRMATARIO_NOME");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                // TODO: da gestire?
            }
        }

        public static async Task AppendFiltroFirmatarioCognome(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("FIRMATARIO_COGNOME");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                // TODO: da gestire?
            }
        }

        public static async Task AppendFiltroEvidenza(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("EVIDENZA");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(p => p.CHA_EVIDENZA == filtro);
            }
        }

        public static async Task AppendFiltroInChildRicEstesa(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("MANCANZA_IMMAGINE");

            if (filtro == "1")
                context.Query = context.Query.Where(p => p.EXT == null);
            else if (filtro == "0")
                context.Query = context.Query.Where(p => p.EXT != null);
        }

        public static async Task AppendFiltroMancanzaFascicolazione(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("MANCANZA_FASCICOLAZIONE");

            if (filtro == "1")
                context.Query = context.Query.Where(p => p.CHA_FASCICOLATO == "0");
            else if (filtro == "0")
                context.Query = context.Query.Where(p => p.CHA_FASCICOLATO == "1");
        }

        public static async Task AppendFiltroTrasmessiCon(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.FindFiltroRicerca("TRASMESSI_SENZA");

            if (filtro != null)
            {
                if (!(filtro.valore ?? string.Empty).ToUpper().Equals("TUTTE"))
                {
                    context.Query = context.Query.Where(p => !context.Pi3DbContext.TrasmissioneEntities.AsNoTracking()
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
                    context.Query = context.Query.Where(p => !context.Pi3DbContext.TrasmissioneEntities.AsNoTracking()
                                                        .Where(t => t.ID_PROFILE == p.SYSTEM_ID)
                                                        .Any());
                }
            }
        }

        public static async Task AppendFiltroDocMaiTrasmessiDaUtente(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
        public static async Task AppendFiltroMaiSpeditiADestinatari(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<bool>("MAI_SPEDITI_AI_DESTINATARI");

            if (filtro)
            {
                context.Query = context.Query.Where(p => (context.Pi3DbContext.DocArrivoParEntities.AsNoTracking()
                .Where(d => d.ID_PROFILE == p.SYSTEM_ID && (d.CHA_TIPO_MITT_DEST == "D" || d.CHA_TIPO_MITT_DEST == "C" || d.CHA_TIPO_MITT_DEST == "F")).Join(
                    context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking().Where(g => g.CHA_TIPO_IE == "E" && !g.DTA_FINE.HasValue),
                    d => d.ID_MITT_DEST,
                    g => g.SYSTEM_ID,
                    (d, g) => new
                    {
                        d,
                        g
                    }).Join(
                    context.Pi3DbContext.DocumentTypesEntities.AsNoTracking().Where(t => t.CHA_TIPO_CANALE == "I" || t.CHA_TIPO_CANALE == "M" || t.CHA_TIPO_CANALE == "S"),
                    dg => dg.d.ID_DOCUMENTTYPES,
                    t => t.SYSTEM_ID,
                    (dg, t) => new
                    {
                        dg,
                        t
                    }).Where(dgt => !context.Pi3DbContext.SendStoEntities.AsNoTracking().Where(s =>
                        s.ID_PROFILE == dgt.dg.d.ID_PROFILE &&
                        dgt.dg.g.SYSTEM_ID == s.ID_CORR_GLOBALE &&
                        s.ID_DOCUMENTTYPE == dgt.t.SYSTEM_ID
                    ).Any())
                    ).Any());
            }
        }
        public static async Task AppendFiltroDocMaiTrasmessiDaRuolo(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.FindFiltroRicerca("DIAGRAMMA_STATO_DOC");

            if (filtro != null)
            {
                context.Query = context.Query.Where(p => context.Pi3DbContext.DiagrammiEntities.AsNoTracking()
                                    .Where(d => p.SYSTEM_ID == d.DOC_NUMBER && (filtro.nomeCampo.ToUpper() == "UNEQUALS" ? (d.ID_STATO != null && d.ID_STATO.ToString() != filtro.valore) : (d.ID_STATO != null && d.ID_STATO.ToString() == filtro.valore)))
                                    .Any());
            }
        }

        public static async Task AppendFiltroCodiceFascicolo(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("NUM_PROT_TITOLARIO");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.NUM_PROT_TIT == filtro);
            }
        }

        public static async Task AppendFiltroFirmato(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("ID_PARENT");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p => p.ID_PARENT == filtro);
            }

        }

        public static async Task AppendFiltroIdRepertorio(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("ID_REPERTORIO");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(p =>
                                context.Pi3DbContext.StampaRepertoriEntities.AsNoTracking()
                                    .Where(s => s.ID_REPERTORIO == filtro
                                        && s.DOCNUMBER == p.DOCNUMBER)
                                .Any());
            }

        }
        private class CorrData
        {
            public long SystemId { get; set; }
            public long? IdOld { get; set; }
        }
        #endregion
        #region Metodi
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                    this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                    this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                    this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                    this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                    this DocumentoGetQueryDocumentoPagingCustomCommand request,
                    ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_PROT_MITTENTE_IL");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(p => p.DTA_PROTO_IN == filtro.AsDateTime());
            }
        }


        public static async Task AppendFiltroDtaProtoMittPrecIl(
                    this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context,
                string textIndexConfig)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DESC_PROTOCOLLATORE");
            var filtroTipoCorr = request.GetValoreFiltroRicerca<string>("TIPO_CORR_PROTOCOLLATORE");
            var filtroExtendHist = request.GetValoreFiltroRicerca<string>("EXTEND_TO_HISTORICIZED_PROTOCOLLATORE");


            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var val = filtro.AsLong();
                var corrQueryable = context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking();



                if (!string.IsNullOrEmpty(filtroTipoCorr))
                {
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

                    // currently implemented as simple like condition __ TO DO: use ftext search
                    if (textIndexConfig.Equals("2"))
                    {
                        context.Query = context.Query.Where(p => corrQueryable.Where(c => c.SYSTEM_ID == p.ID_UO_PROT && EF.Functions.Like(c.VAR_DESC_CORR, $"%{filtro}%")).Any());
                    }

                    context.Query = context.Query.Where(p => context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == val && c.ID_PEOPLE == p.ID_PEOPLE_PROT).Any());

                }

            }
        }

        public static async Task AppendFiltroIdMittenteIntermedio(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_ARRIVO_IL");
            if (!string.IsNullOrEmpty(filtro))
            {
                var fData = filtro.AsDateTime();
                (var startDay, var endDay) = GetDateRangeIl(fData);

                context.Query = context.Query.Where(p => context.Pi3DbContext.VersionEntities.AsNoTracking().
                    Where(v => v.DOCNUMBER == p.DOCNUMBER && v.DTA_ARRIVO >= startDay && v.DTA_ARRIVO <= endDay).Any());
            }

        }


        public static async Task AppendFiltroDtaArrivoPrecedenteIl(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_ARRIVO_PRECEDENTE_IL");
            if (!string.IsNullOrEmpty(filtro))
            {
                var fData = filtro.AsDateTime();

                context.Query = context.Query.Where(p => context.Pi3DbContext.VersionEntities.AsNoTracking().
                    Where(v => v.DOCNUMBER == p.DOCNUMBER && v.DTA_ARRIVO <= fData).Any());
            }

        }

        public static async Task AppendFiltroDtaArrivoSuccessivaAl(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
                ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_ARRIVO_SUCCESSIVA_AL");
            if (!string.IsNullOrEmpty(filtro))
            {
                var fData = filtro.AsDateTime();

                context.Query = context.Query.Where(p => context.Pi3DbContext.VersionEntities.AsNoTracking().
                    Where(v => v.DOCNUMBER == p.DOCNUMBER && v.DTA_ARRIVO >= fData).Any());
            }

        }

        public static async Task AppendFiltroDtaArrivoSettCorr(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_ARRIVO_SC");
            if (!string.IsNullOrEmpty(filtro))
            {
                var fData = filtro.AsDateTime();
                (var startDay, var endDay) = GetDateRangeSC(fData);
                context.Query = context.Query.Where(p => context.Pi3DbContext.VersionEntities.AsNoTracking().
                    Where(v => v.DOCNUMBER == p.DOCNUMBER && v.DTA_ARRIVO >= startDay && v.DTA_ARRIVO <= endDay).Any());
            }

        }


        public static async Task AppendFiltroDtaArrivoMeseCorr(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_ARRIVO_MC");
            if (!string.IsNullOrEmpty(filtro))
            {
                var fData = filtro.AsDateTime();
                (var startDay, var endDay) = GetDateRangeMC(fData);
                context.Query = context.Query.Where(p => context.Pi3DbContext.VersionEntities.AsNoTracking().
                    Where(v => v.DOCNUMBER == p.DOCNUMBER && v.DTA_ARRIVO >= startDay && v.DTA_ARRIVO <= endDay).Any());
            }

        }

        public static async Task AppendFiltroDtaArrivoToday(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DATA_ARRIVO_TODAY");
            if (!string.IsNullOrEmpty(filtro))
            {
                var fData = DateTime.Today;
                (var startDay, var endDay) = GetDateRangeIl(fData);

                context.Query = context.Query.Where(p => context.Pi3DbContext.VersionEntities.AsNoTracking().
                    Where(v => v.DOCNUMBER == p.DOCNUMBER && v.DTA_ARRIVO >= startDay && v.DTA_ARRIVO <= endDay).Any());
            }

        }

        public static async Task AppendFiltroNumProtoEmerg(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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

        public static async Task AppendFiltroDataStampaRegistroProtocollo(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_STAMPA_REGISTRO");

            if (filtro > DateTime.MinValue)
            {
                var range = GetDateRangeIl(filtro);

                context.Query = context.Query.Where(p => context.Pi3DbContext.StampaRegistriEntities.AsNoTracking()
                                            .Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER && s.DTA_STAMPA >= range.Item1 && s.DTA_STAMPA <= range.Item2));
            }
        }

        public static async Task AppendFiltroDataStampaRegistroProtocolloDal(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_STAMPA_REGISTRO_DAL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDateSuccessivaAl(filtro);

                context.Query = context.Query.Where(p => context.Pi3DbContext.StampaRegistriEntities.AsNoTracking()
                                                    .Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER && s.DTA_STAMPA >= initDate));
            }
        }

        public static async Task AppendFiltroDataStampaRegistroProtocolloAl(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_STAMPA_REGISTRO_AL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDatePrecedenteIl(filtro);

                context.Query = context.Query.Where(p => context.Pi3DbContext.StampaRegistriEntities.AsNoTracking()
                                                    .Any(s => s.DOCNUMBER.HasValue && p.DOCNUMBER.HasValue && s.DOCNUMBER == p.DOCNUMBER && s.DTA_STAMPA <= initDate));
            }
        }

        public static async Task AppendFiltroDataStampaRegistroProtocolloSC(
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
                this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("REP_FIRMATO");

            if (!string.IsNullOrEmpty(filtro))
            {
                context.Query = context.Query.Where(p => p.CHA_FIRMATO == filtro);
            }
        }

        public static async Task AppendFiltroStampaIdRepertorio(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("ANNULLATO");

            if (!string.IsNullOrEmpty(filtro))
            {
                if (filtro.Equals("0"))
                {
                    context.Query = context.Query.Where(p => null == p.DTA_ANNULLA);

                }
                else
                {
                    context.Query = context.Query.Where(p => null != p.DTA_ANNULLA);
                }
            }
        }

        public static async Task AppendFiltroSegnatura(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("SEGNATURA");

            if (!string.IsNullOrEmpty(filtro))
            {

                context.Query = context.Query.Where(p => EF.Functions.Like(p.VAR_SEGNATURA, $"{filtro.ToUpper().Replace("'", "''")}%"));
            }
        }

        public static async Task AppendFiltroIdPeopleCreatore(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DESC_UO_CREATORE");

            if (!string.IsNullOrEmpty(filtro))
            {
                context.Query = context.Query.Where(p => context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.VAR_DESC_CORR != null && EF.Functions.Like(c.VAR_DESC_CORR.ToUpper(), $"%{filtro.ToUpper()}%") && p.ID_UO_CREATORE == c.SYSTEM_ID).Any());
            }
        }
        public static async Task AppendFiltroDescRuoCreatore(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DESC_RUOLO_CREATORE");

            if (!string.IsNullOrEmpty(filtro))
            {
                context.Query = context.Query.Where(p => context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.VAR_DESC_CORR != null && EF.Functions.Like(c.VAR_DESC_CORR.ToUpper(), $"%{filtro.ToUpper()}%") && p.ID_RUOLO_CREATORE == c.SYSTEM_ID).Any());
            }
        }

        public static async Task AppendFiltroDescPeopleCreatore(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DESC_PEOPLE_CREATORE");

            if (!string.IsNullOrEmpty(filtro))
            {
                context.Query = context.Query.Where(p => context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.VAR_DESC_CORR != null && EF.Functions.Like(c.VAR_DESC_CORR.ToUpper(), $"%{filtro.ToUpper()}%") && p.AUTHOR == c.ID_PEOPLE).Any());
            }
        }


        public static async Task AppendFiltroIdRuoCreatore(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
            ProfileSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("NUMERO_ALLEGATI_TIPO");
            var filtroNumAll = request.GetValoreFiltroRicerca<string>("NUMERO_ALLEGATI");
            int op = 0;
            /*
            if (!string.IsNullOrEmpty(filtro))
            {
                if (!string.IsNullOrEmpty(filtroNumAll))
                {
                    var numAll = filtroNumAll.Substring(1).AsLong();


                    switch (filtroNumAll[0])
                    {
                        case '<':
                            op = 1;
                            break;

                        case '>':
                            op = 2;
                            break;

                        case '=':
                            op = 3;
                            break;
                    }


                    switch (filtro)
                    {
                        case "SIMPLIFIEDINTEROPERABILITY":
                            if (op == 1)
                            {
                                context.Query = context.Query.Where(p =>
                                context.Pi3DbContext.ProfileEntities.AsNoTracking().Join(context.Pi3DbContext.NotificaEntities.AsNoTracking(),
                                p1 => p1.DOCNUMBER,
                                n1 => n1.DOCNUMBER,
                                (p1, n1) => new { p1, n1 }
                                ).Where(o => o.p1.DOCNUMBER == p.DOCNUMBER).Select(o => o.p1.SYSTEM_ID).Any()
                                &&
                                context.Pi3DbContext.ProfileEntities.AsNoTracking().
                                Where(o => o.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER 
                                && ( EF.Functions.Like(o.VAR_PROF_OGGETTO, "Ricevuta di mancata consegna%") || EF.Functions.Like(o.VAR_PROF_OGGETTO, "Ricevuta di avvenuta%"))
                                ).Select(o => o.SYSTEM_ID).Count() < numAll
                                );
                            }
                            else if (op == 2)
                            {
                                context.Query = context.Query.Where(p =>
                                context.Pi3DbContext.ProfileEntities.AsNoTracking().Join(context.Pi3DbContext.NotificaEntities.AsNoTracking(),
                                p1 => p1.DOCNUMBER,
                                n1 => n1.DOCNUMBER,
                                (p1, n1) => new { p1, n1 }
                                ).Where(o => o.p1.DOCNUMBER == p.DOCNUMBER).Select(o => o.p1.SYSTEM_ID).Any()
                                &&
                                context.Pi3DbContext.ProfileEntities.AsNoTracking().
                                Where(o => o.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER 
                                && (EF.Functions.Like(o.VAR_PROF_OGGETTO, "Ricevuta di mancata consegna%") || EF.Functions.Like(o.VAR_PROF_OGGETTO, "Ricevuta di avvenuta%")))
                                .Select(o => o.SYSTEM_ID).Count() > numAll
                                );
                            }
                            else
                            {
                                context.Query = context.Query.Where(p =>
                                context.Pi3DbContext.ProfileEntities.AsNoTracking().Join(context.Pi3DbContext.NotificaEntities.AsNoTracking(),
                                p1 => p1.DOCNUMBER,
                                n1 => n1.DOCNUMBER,
                                (p1, n1) => new { p1, n1 }
                                ).Where(o => o.p1.DOCNUMBER == p.DOCNUMBER).Select(o => o.p1.SYSTEM_ID).Any()
                                &&
                                context.Pi3DbContext.ProfileEntities.AsNoTracking().
                                Where(o => o.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER 
                                && (EF.Functions.Like(o.VAR_PROF_OGGETTO, "Ricevuta di mancata consegna%") || EF.Functions.Like(o.VAR_PROF_OGGETTO, "Ricevuta di avvenuta%")))
                                .Select(o => o.SYSTEM_ID).Count() == numAll
                                );
                            }
                            break;

                        case "user":

                            if (op == 1)
                            {
                                context.Query = context.Query.Where(p =>
                                    context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                    .Where(o => o.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER).Select(o => o.SYSTEM_ID).Count() < numAll
                                    &&
                                    (
                                    context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                    .Where(o => o.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER).Select(o => o.SYSTEM_ID).Count() -

                                    context.Pi3DbContext.ProfileEntities.AsNoTracking().Where(o => o.DOCNUMBER == p.DOCNUMBER).Join(context.Pi3DbContext.NotificaEntities.AsNoTracking(),
                                    p1 => p1.DOCNUMBER,
                                    n1 => n1.DOCNUMBER,
                                    (p1, n1) => new { p1, n1 }
                                    ).Select(o => o.p1.SYSTEM_ID).Count() -

                                    context.Pi3DbContext.ProfileEntities.AsNoTracking().Where(o => o.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER).Join(
                                    context.Pi3DbContext.VersionEntities.AsNoTracking().Where(o => "1" == o.CHA_ALLEGATI_ESTERNO),
                                    p1 => p1.DOCNUMBER,
                                    v => v.DOCNUMBER,
                                    (p1, v) => p1.SYSTEM_ID
                                    ).Count()) < numAll
                                );
                            }
                            else if (op == 2)
                            {
                                context.Query = context.Query.Where(p =>
                                    context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                    .Where(o => o.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER).Select(o => o.SYSTEM_ID).Count() > numAll
                                    &&
                                    (
                                    context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                    .Where(o => o.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER).Select(o => o.SYSTEM_ID).Count() -

                                    context.Pi3DbContext.ProfileEntities.AsNoTracking().Where(o => o.DOCNUMBER == p.DOCNUMBER).Join(context.Pi3DbContext.NotificaEntities.AsNoTracking(),
                                    p1 => p1.DOCNUMBER,
                                    n1 => n1.DOCNUMBER,
                                    (p1, n1) => new { p1, n1 }
                                    ).Select(o => o.p1.SYSTEM_ID).Count() -

                                    context.Pi3DbContext.ProfileEntities.AsNoTracking().Where(o => o.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER).Join(
                                    context.Pi3DbContext.VersionEntities.AsNoTracking().Where(o => "1" == o.CHA_ALLEGATI_ESTERNO),
                                    p1 => p1.DOCNUMBER,
                                    v => v.DOCNUMBER,
                                    (p1, v) => p1.SYSTEM_ID
                                    ).Count()) > numAll
                                );
                            }
                            else
                            {
                                context.Query = context.Query.Where(p =>
                                    context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                    .Where(o => o.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER).Select(o => o.SYSTEM_ID).Count() == numAll
                                    &&
                                    (
                                    context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                    .Where(o => o.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER).Select(o => o.SYSTEM_ID).Count() -

                                    context.Pi3DbContext.ProfileEntities.AsNoTracking().Where(o => o.DOCNUMBER == p.DOCNUMBER).Join(context.Pi3DbContext.NotificaEntities.AsNoTracking(),
                                    p1 => p1.DOCNUMBER,
                                    n1 => n1.DOCNUMBER,
                                    (p1, n1) => new { p1, n1 }
                                    ).Select(o => o.p1.SYSTEM_ID).Count() -

                                    context.Pi3DbContext.ProfileEntities.AsNoTracking().Where(o => o.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER).Join(
                                    context.Pi3DbContext.VersionEntities.AsNoTracking().Where(o => "1" == o.CHA_ALLEGATI_ESTERNO),
                                    p1 => p1.DOCNUMBER,
                                    v => v.DOCNUMBER,
                                    (p1, v) => p1.SYSTEM_ID
                                    ).Count()) == numAll
                                );
                            }

                            break;

                        case "esterni":
                            if (op == 1)
                            {
                                context.Query = context.Query.Where( p => 
                                    context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                    .Where(o => o.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER).Select(o=>o.SYSTEM_ID).Count() < numAll
                                    && context.Pi3DbContext.ProfileEntities.AsNoTracking().Join(
                                        context.Pi3DbContext.VersionEntities.AsNoTracking(),
                                        p1 => p1.DOCNUMBER,
                                        v => v.DOCNUMBER,
                                        (p1,v) => new
                                        {
                                            p1.SYSTEM_ID,
                                            v.CHA_ALLEGATI_ESTERNO,
                                            p1.ID_DOCUMENTO_PRINCIPALE
                                        }).Where(o => o.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER && "1" == o.CHA_ALLEGATI_ESTERNO)
                                        .Select(o => o.SYSTEM_ID).Count() < numAll
                                );
                            }
                            else if (op == 2)
                            {
                                context.Query = context.Query.Where(p =>
                                    context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                    .Where(o => o.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER).Select(o => o.SYSTEM_ID).Count() > numAll
                                    && context.Pi3DbContext.ProfileEntities.AsNoTracking().Join(
                                        context.Pi3DbContext.VersionEntities.AsNoTracking(),
                                        p1 => p1.DOCNUMBER,
                                        v => v.DOCNUMBER,
                                        (p1, v) => new
                                        {
                                            p1.SYSTEM_ID,
                                            v.CHA_ALLEGATI_ESTERNO,
                                            p1.ID_DOCUMENTO_PRINCIPALE
                                        }).Where(o => o.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER && "1" == o.CHA_ALLEGATI_ESTERNO)
                                        .Select(o => o.SYSTEM_ID).Count() > numAll
                                );
                            }
                            else
                            {
                                context.Query = context.Query.Where(p =>
                                    context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                    .Where(o => o.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER).Select(o => o.SYSTEM_ID).Count() == numAll
                                    && context.Pi3DbContext.ProfileEntities.AsNoTracking().Join(
                                        context.Pi3DbContext.VersionEntities.AsNoTracking(),
                                        p1 => p1.DOCNUMBER,
                                        v => v.DOCNUMBER,
                                        (p1, v) => new
                                        {
                                            p1.SYSTEM_ID,
                                            v.CHA_ALLEGATI_ESTERNO,
                                            p1.ID_DOCUMENTO_PRINCIPALE
                                        }).Where(o => o.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER && "1" == o.CHA_ALLEGATI_ESTERNO)
                                        .Select(o => o.SYSTEM_ID).Count() == numAll
                                );
                            }
                            break;
                        case "albopubb":
                            if (op == 1)
                            {
                                context.Query = context.Query.Where(p =>
                                    (from p1 in context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                    from v in context.Pi3DbContext.VersionEntities.AsNoTracking()
                                    from adp in context.Pi3DbContext.AlboDocPubbEntities.AsNoTracking()
                                    where (p1.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER) &&
                                    (v.DOCNUMBER == p1.DOCNUMBER) &&
                                    (adp.DOCNUMBER == p1.DOCNUMBER) &&
                                    ("0" == v.CHA_ALLEGATI_ESTERNO) &&
                                    ("S" == adp.DA_PUBB)
                                    select p1.SYSTEM_ID
                                    ).Count() < numAll
                                    );
                            }
                            else if(op == 2)
                            {
                                context.Query = context.Query.Where(p =>
                                    (from p1 in context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                    from v in context.Pi3DbContext.VersionEntities.AsNoTracking()
                                    from adp in context.Pi3DbContext.AlboDocPubbEntities.AsNoTracking()
                                    where (p1.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER) &&
                                    (v.DOCNUMBER == p1.DOCNUMBER) &&
                                    (adp.DOCNUMBER == p1.DOCNUMBER) &&
                                    ("0" == v.CHA_ALLEGATI_ESTERNO) &&
                                    ("S" == adp.DA_PUBB)
                                    select p1.SYSTEM_ID
                                    ).Count() > numAll
                                    );
                            }
                            else
                            {
                                context.Query = context.Query.Where(p =>
                                    (from p1 in context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                        from v in context.Pi3DbContext.VersionEntities.AsNoTracking()
                                        from adp in context.Pi3DbContext.AlboDocPubbEntities.AsNoTracking()
                                        where (p1.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER) &&
                                        (v.DOCNUMBER == p1.DOCNUMBER) &&
                                        (adp.DOCNUMBER == p1.DOCNUMBER) &&
                                        ("0" == v.CHA_ALLEGATI_ESTERNO) &&
                                        ("S" == adp.DA_PUBB)
                                        select p1.SYSTEM_ID
                                        ).Count() == numAll
                                    );
                            }

                            break;

                        case "pec":
                            if (op == 1)
                            {
                                context.Query = context.Query.Where(p =>
                                context.Pi3DbContext.ProfileEntities.AsNoTracking().Join(context.Pi3DbContext.NotificaEntities.AsNoTracking(),
                                p1 => p1.DOCNUMBER,
                                n1 => n1.DOCNUMBER,
                                (p1, n1) => new { p1, n1 }
                                ).Where(o => o.p1.DOCNUMBER == p.DOCNUMBER).Select(o => o.p1.SYSTEM_ID).Any()
                                &&
                                context.Pi3DbContext.ProfileEntities.AsNoTracking().
                                Where(o => o.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER && EF.Functions.Like(o.VAR_PROF_OGGETTO, "Ricevuta di ritorno delle Mail%")).Select(o => o.SYSTEM_ID).Count() < numAll
                                );
                            }
                            else if (op == 2)
                            {
                                context.Query = context.Query.Where(p =>
                                context.Pi3DbContext.ProfileEntities.AsNoTracking().Join(context.Pi3DbContext.NotificaEntities.AsNoTracking(),
                                p1 => p1.DOCNUMBER,
                                n1 => n1.DOCNUMBER,
                                (p1, n1) => new { p1, n1 }
                                ).Where(o => o.p1.DOCNUMBER == p.DOCNUMBER).Select(o => o.p1.SYSTEM_ID).Any()
                                &&
                                context.Pi3DbContext.ProfileEntities.AsNoTracking().
                                Where(o => o.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER && EF.Functions.Like(o.VAR_PROF_OGGETTO, "Ricevuta di ritorno delle Mail%")).Select(o => o.SYSTEM_ID).Count() > numAll
                                );
                            }
                            else
                            {
                                context.Query = context.Query.Where(p =>
                                context.Pi3DbContext.ProfileEntities.AsNoTracking().Join(context.Pi3DbContext.NotificaEntities.AsNoTracking(),
                                p1 => p1.DOCNUMBER,
                                n1 => n1.DOCNUMBER,
                                (p1, n1) => new { p1, n1 }
                                ).Where(o => o.p1.DOCNUMBER == p.DOCNUMBER).Select(o => o.p1.SYSTEM_ID).Any()
                                &&
                                context.Pi3DbContext.ProfileEntities.AsNoTracking().
                                Where(o => o.ID_DOCUMENTO_PRINCIPALE == p.DOCNUMBER && EF.Functions.Like(o.VAR_PROF_OGGETTO, "Ricevuta di ritorno delle Mail%")).Select(o => o.SYSTEM_ID).Count() == numAll
                                );
                            }

                            break;
                        case "tutti":

                            var pred = PredicateBuilder.New<ProfileEntity>();
                            if (op == 1)
                            {
                                pred = pred.And(a => context.Pi3DbContext.ProfileEntities.AsNoTracking().Where(x => x.ID_DOCUMENTO_PRINCIPALE == a.DOCNUMBER).Select(x => x.SYSTEM_ID).Count() < numAll);
                            }
                            else if (op == 2)
                            {
                                pred = pred.And(a => context.Pi3DbContext.ProfileEntities.AsNoTracking().Where(x => x.ID_DOCUMENTO_PRINCIPALE == a.DOCNUMBER).Select(x => x.SYSTEM_ID).Count() > numAll);
                            }
                            else
                            {
                                pred = pred.And(a => context.Pi3DbContext.ProfileEntities.AsNoTracking().Where(x => x.ID_DOCUMENTO_PRINCIPALE == a.DOCNUMBER).Select(x => x.SYSTEM_ID).Count() == numAll);
                            }
                            context.Query = context.Query.Where(pred);

                            break;
                    }

                } 
            }
            */
        }
        #endregion


        public static async Task AppendFiltroDocMaiSpeditiDaUtente(
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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
            this DocumentoGetQueryDocumentoPagingCustomCommand request,
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

        #endregion
        #endregion
    }
}
