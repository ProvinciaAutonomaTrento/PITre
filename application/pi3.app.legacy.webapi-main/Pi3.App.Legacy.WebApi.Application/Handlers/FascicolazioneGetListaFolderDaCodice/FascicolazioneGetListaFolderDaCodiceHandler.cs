// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO;
using DocsPaVO.fascicolazione;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneGetListaFolderDaCodice
{

    // Richiede libreria MediatR
    public class FascicolazioneGetListaFolderDaCodiceHandler : IRequestHandler<Application.Requests.FascicolazioneGetListaFolderDaCodice, FascicolazioneGetListaFolderDaCodiceResult>
    {
        #region Public Members

        public FascicolazioneGetListaFolderDaCodiceHandler(ILogger<FascicolazioneGetListaFolderDaCodiceHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<FascicolazioneGetListaFolderDaCodiceResult> Handle(Application.Requests.FascicolazioneGetListaFolderDaCodice request, CancellationToken cancellationToken)
        {
            List<Folder> lstFolder = new List<Folder>();
            DocsPaVO.utente.InfoUtente infoUtente = request.infoUtente;
            string codiceFascicolo = request.codiceFascicolo;
            string descrFolder = request.descrFolder;
            DocsPaVO.utente.Registro registro = request.registro;
            bool enableProfilazione = request.enableProfilazione;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

                DocsPaVO.fascicolazione.Folder folderObject = null;

                var query = this._dbContext.ProjectEntities
                    .Where(x => (x.ID_AMM == null || x.ID_AMM == idTenant) &&
                        this._dbContext.SecurityEntities.Any(s => x.SYSTEM_ID == s.THING &&
                            (s.PERSONORGROUP == idUser || s.PERSONORGROUP == idGroup) &&
                            s.ACCESSRIGHTS > 0) &&
                            x.CHA_TIPO_PROJ.Equals("F"));

                if (!string.IsNullOrEmpty(codiceFascicolo))
                {
                    List<long> nodiTitolario = await this._dbContext.ProjectEntities.Where(x => x.ID_TITOLARIO == 0 && !x.CHA_STATO.Equals("D")).Select(x => x.SYSTEM_ID).ToListAsync();
                    query = query.Where(x => nodiTitolario.Contains((long)x.ID_TITOLARIO) && x.VAR_CODICE.ToUpper().Equals(codiceFascicolo.ToUpper()));
                }

                if (registro != null)
                    query = query.Where(x => x.ID_REGISTRO == null || x.ID_REGISTRO == registro.systemId.AsLong());

                DocsPaVO.fascicolazione.Fascicolo fascicolo = await query.Select(x => new DocsPaVO.fascicolazione.Fascicolo()
                {
                    systemID = x.SYSTEM_ID.ToString()
                }).FirstOrDefaultAsync();

                var rights = await this._dbContext.GetSecurityRights(fascicolo.systemID, idUser.ToString(), idGroup.ToString());
                var rigthsAsLong = Convert.ToInt32(rights);


                fascicolo.accessRights = GetAccessRigths(rights);
                //fascicolo.codiceRegistroNodoTit = await GetCodReg(fascicolo.ID_REGISTRO);
                //fascicolo.isFascConsentita = await GetChaConsentiClass(fascicolo.ID_PARENT, fascicolo.CHA_TIPO_PROJ, fascicolo.ID_FASCICOLO);
                //fascicolo.isFascicolazioneConsentita = await GetChaConsentiFasc(fascicolo.ID_PARENT, fascicolo.CHA_TIPO_PROJ, fascicolo.CHA_TIPO_FASCICOLO);
                //fascicolo.InAreaLavoro = await GetInAdl(fascicolo.SYSTEM_ID, idGroup, idUser);

                string[] separatore = { "//" };
                string[] appo = descrFolder.Split(separatore, StringSplitOptions.RemoveEmptyEntries);
                int inizio = 0;

                for (int i = 0; i < appo.Length; i++)
                {
                    var join = this._dbContext.ProjectEntities
                        .Join(this._dbContext.SecurityEntities, p => p.SYSTEM_ID, b => b.THING, (p, b) => new { p, b });

                    if (inizio == 0)
                    {
                        var idParentList = await this._dbContext.ProjectEntities.Where(x => x.ID_PARENT == fascicolo.systemID.AsLong())
                            .Select(x => x.SYSTEM_ID)
                            .ToListAsync();
                        join = join.Where(x => idParentList.Contains((long)x.p.ID_PARENT));
                    }
                    else
                    {
                        join = join.Where(x => x.p.ID_PARENT == lstFolder[i - 1].systemID.AsLong());
                    }

                    var list = await join.Where(x => x.p.ID_FASCICOLO == fascicolo.systemID.AsLong() &&
                            (x.b.PERSONORGROUP == idUser || x.b.PERSONORGROUP == idGroup) &&
                            x.p.DESCRIPTION.ToUpper().Equals(appo[i].ToUpper()) &&
                            x.b.ACCESSRIGHTS >= 0)
                        .Select(x => new
                        {
                            ID_FOLDER = x.p.SYSTEM_ID,
                            ID_PARENT_FOLDER = x.p.ID_PARENT,
                            ID_FASCICOLO = x.p.ID_FASCICOLO,
                            FOLDER_DESCRIPTION = x.p.DESCRIPTION,
                            CHA_TIPO_FASCICOLO = x.p.CHA_TIPO_FASCICOLO,
                            APERTURA = x.p.DTA_APERTURA.AsDateTimeFormat(),
                            NUM_LIVELLO = x.p.NUM_LIVELLO,
                            VAR_COD_LIV1 = x.p.VAR_COD_LIV1
                        })
                        .ToListAsync();

                    inizio++;

                    if (list != null && list.Count > 0)
                    {
                        list.DistinctBy(x => x.ID_FOLDER).OrderBy(x => x.FOLDER_DESCRIPTION).ToList().ForEach(f =>
                        {
                            lstFolder.Add(new Folder()
                            {
                                systemID = f.ID_FOLDER.ToString(),
                                idParent = f.ID_PARENT_FOLDER.ToString(),
                                idFascicolo = f.ID_FASCICOLO.ToString(),
                                descrizione = f.FOLDER_DESCRIPTION.ToString(),
                                dtaApertura = f.APERTURA.ToString().Trim(),
                                livello = f.NUM_LIVELLO == null ? string.Empty : f.NUM_LIVELLO.ToString(),
                                codicelivello = f.VAR_COD_LIV1 == null ? string.Empty : f.VAR_COD_LIV1.ToString()
                            });
                        });
                    }
                    else
                    {
                        return new FascicolazioneGetListaFolderDaCodiceResult(null);
                    }
                }

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }


            return new FascicolazioneGetListaFolderDaCodiceResult(lstFolder.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneGetListaFolderDaCodiceHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        private async Task<string> GetInAdl(long systemId, long idGroup, long idUser)
        {
            var idCorrGlobaliRuolo = this._dbContext.CorrGlobaliEntities.Where(x => x.ID_GRUPPO == idGroup).Select(x => x.SYSTEM_ID).FirstOrDefault();
            return await this._dbContext.AreaLavoroEntities.AnyAsync(x => x.ID_PROJECT == systemId && x.ID_PEOPLE == idUser && x.ID_RUOLO_IN_UO == idCorrGlobaliRuolo) ? "1" : "0";
        }

        private async Task<string> GetChaConsentiClass(long? idParent, string? tipoProj, long? idFascicolo)
        {
            string result = "0";

            switch (tipoProj)
            {
                case "F":
                    result = await this._dbContext.ProjectEntities.Where(x => x.SYSTEM_ID == idParent).Select(x => x.CHA_CONSENTI_CLASS).FirstOrDefaultAsync();
                    break;
                case "C":
                    var idParentList = await this._dbContext.ProjectEntities.Where(x => x.SYSTEM_ID == idFascicolo).Select(x => x.ID_PARENT).ToListAsync();
                    result = await this._dbContext.ProjectEntities.Where(x => idParentList.Contains(x.SYSTEM_ID)).Select(x => x.CHA_CONSENTI_CLASS).FirstOrDefaultAsync();
                    break;
            }

            return result;
        }

        private string GetAccessRigths(SecurityRightTypesEnum rights)
        {
            switch (rights)
            {
                case SecurityRightTypesEnum.FullControl:
                    return "255";
                    break;
                case SecurityRightTypesEnum.Write:
                    return "63";
                    break;
                case SecurityRightTypesEnum.Read:
                    return "45";
                    break;
                default:
                    return "-1";
                    break;
            }
        }

        private async Task<string> GetCodReg(long? idRegistro)
        {
            return await this._dbContext.RegistroEntities.Where(x => x.SYSTEM_ID == idRegistro).Select(x => x.VAR_CODICE).FirstOrDefaultAsync();
        }

        private async Task<bool> GetChaConsentiFasc(long? idParent, string? tipoProj, string? tipoFascicolo)
        {
            bool result = true;

            if (tipoProj.Equals("F") && tipoFascicolo.Equals("P"))
                result = await this._dbContext.ProjectEntities.Where(x => x.SYSTEM_ID == idParent).Select(x => x.CHA_CONSENTI_FASC).FirstOrDefaultAsync() == "1";

            return result;
        }

        private async Task<PianoConservazione> GetPianoConservazioneById(long? idPianoConservazione)
        {
            return await this._dbContext.PianoConservazioneEntities.Where(x => x.SYSTEM_ID == idPianoConservazione)
                .Select(x => new PianoConservazione()
                {
                    SystemId = x.SYSTEM_ID.ToString(),
                    IdClassificazione = x.ID_CLASSIFICAZIONE != null ? x.ID_CLASSIFICAZIONE.ToString() : string.Empty,
                    CodiceClassificazione = x.CODICE_CLASSIFICAZIONE,
                    NumeroProcedimento = x.NUMERO_PROCEDIMENTO,
                    TipologiaFascicolo = x.TIPOLOGIA_FASCICOLO,
                    TempoConservazione = x.TEMPO_CONSERVAZIONE,
                    VoceProcedimento = x.VOCE_PROCEDIMENTO,
                    NoteChiusuraFascicolo = x.NOTE_CHIUSURA_FASCICOLO,
                    NoteScartabilitaDocumenti = x.NOTE_SCARTABILITA_DOC,
                    NoteDocumenti = x.NOTE_DOCUMENTI
                })
                .FirstOrDefaultAsync();
        }

        #endregion
    }

}
