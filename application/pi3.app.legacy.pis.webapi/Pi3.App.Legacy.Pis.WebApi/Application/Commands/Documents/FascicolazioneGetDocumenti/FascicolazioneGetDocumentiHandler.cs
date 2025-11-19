// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.ricerche;
using DocumentFormat.OpenXml.InkML;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentoGetQueryDocumentoPagingCustom;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.FascicolazioneGetDocumenti
{
    public class FascicolazioneGetDocumentiHandler : IRequestHandler<FascicolazioneGetDocumentiCommand, FascicolazioneGetDocumentiCommandResponse>
    {
        #region Public Members
        public FascicolazioneGetDocumentiHandler(
            ILogger<FascicolazioneGetDocumentiHandler> logger,
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
        public async Task<FascicolazioneGetDocumentiCommandResponse> Handle(FascicolazioneGetDocumentiCommand request, CancellationToken cancellationToken)
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

                var folderIdAsLong = request.Folder.Id.AsLong();

                var query =
                    this._pi3DbContext.ProfileEntities
                        .AsNoTracking()
                        .Where(p => (p.CHA_IN_CESTINO == null || p.CHA_IN_CESTINO == "0") && this._pi3DbContext.ProjectComponentEntities.AsNoTracking()
                                .Where(pc => pc.LINK == p.SYSTEM_ID && pc.PROJECT_ID == folderIdAsLong)
                                .Any());

                nRec = await query
                    .Select(p => p.SYSTEM_ID)
                    .CountAsync();

                numTotPage = nRec / request.PageSize;

                if (nRec > 0)
                {
                    var dataQuery = query
                        .OrderByDescending(p => p.DTA_PROTO ?? p.CREATION_DATE)
                        .Skip(request.NumPage * request.PageSize - request.PageSize)
                        .Take(!request.Export ? request.PageSize : nRec)
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
                                        new DocsPaVO.Grids.SearchObjectField("GetValProfObjsDocAsJson", "[]"), 
                                        new DocsPaVO.Grids.SearchObjectField("IN_LIBROFIRMA", p.IN_LIBROFIRMA)
                                        }
                                    ).ToArray()
                                ));

                    output = await dataQuery.ToArrayAsync();
                    if (request.VisibleFieldsTemplate != null)
                    {
                        output.ForEach(item =>
                        {
                            var customObjectsAsJson = item.SearchObjectField
                                .Where(f => f.SearchObjectFieldID == "GetValProfObjsDocAsJson")
                                .Select(f => f.SearchObjectFieldValue)
                                .First();

                            var objectFields = new List<DocsPaVO.Grids.SearchObjectField>();

                            System.Text.Json.JsonSerializer
                                    .Deserialize<GetValProfObjPrj[]>(customObjectsAsJson)!
                                    .Where(obj => request.VisibleFieldsTemplate.Any(ft => ft.CustomObjectId == Convert.ToInt32(obj.id)))
                                    .ForEach(obj =>
                                    {
                                        var key = $"A{obj.id}";
                                        var of = objectFields.FirstOrDefault(of => of.SearchObjectFieldID == key);

                                        if (of != null)
                                            of.SearchObjectFieldValue += $"; {obj.valore}";
                                        else
                                            objectFields.Add(new DocsPaVO.Grids.SearchObjectField(key, obj.valore));
                                    });

                            if (objectFields.Any())
                            {
                                item.SearchObjectField.AddRange(objectFields);
                            }

                            item.SearchObjectField.RemoveAll(f => f.SearchObjectFieldID == "GetValProfObjsDocAsJson");
                        });
                    }

                    if (request.CompileIdProfileList)
                    {
                        idProfiles = output.Select(p => new SearchResultInfo()
                        {
                            Id = p.SearchObjectID,
                            Codice = p.SearchObjectField.First(f => f.SearchObjectFieldID == "CODICE").SearchObjectFieldValue
                        })
                        .ToList();
                    }

                }

            }
            catch (Exception ex)
            {
                output = null!;
                this._logger.LogCritical(ex, ex.Message);
            }

            return new FascicolazioneGetDocumentiCommandResponse(output, numTotPage, nRec, idProfiles.ToArray());
        }
        #endregion

        #region Private members
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
                    }.DescrizioneAtipicita
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

        protected readonly ILogger<FascicolazioneGetDocumentiHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IConfigurationService _configurationService;

        protected record GetValProfObjPrj(string id, string nome, string valore);
        #endregion
    }
}
